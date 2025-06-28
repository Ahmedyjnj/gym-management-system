using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication2.Dtos;
using WindowsFormsApplication2.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        public Form1()
        {

            InitializeComponent();
        }

        gymDataContext context = new gymDataContext();

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSubscriptions();
            Intializephotos();
            dataGridView1.DefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Regular);

            labelAllSubscripes.Text =context.Subscriptions.Count().ToString();
        }

        private void LoadSubscriptions()
        {

            var data = context.Subscriptions
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.SubscriptionPeriod,
                    s.Type,
                    s.PaymentHasBeenMade,
                    AttendanceCount = s.Attendances.Count
                })
                .ToList();

            dataGridView1.DataSource = data;


        }





        private void btnDelete_Click()
        {
            if (dataGridView1.CurrentRow != null)
            {
                var selected = (Subscription)dataGridView1.CurrentRow.DataBoundItem;
                context.Subscriptions.Remove(selected);
                context.SaveChanges();
                LoadSubscriptions();
            }
        }





        private void SubscriptionAdd()
        {

            // 1. أنشئ اشتراك جديد
            var newSub = new Subscription
            {
                Name = textBoxName.Text,
                SubscriptionPeriod = comboBoxPeriod.Text,
                Type = comboBoxType.Text,
                PaymentHasBeenMade = checkBoxIPayment.Checked
            };

            context.Subscriptions.Add(newSub);
            context.SaveChanges(); // مهم علشان يتم توليد Id
            message = "subscription Added ||";
            // 2. أضف الحضور وربطه بالاشتراك
            var newAttendance = new Attendance
            {
                SubscribeId = newSub.Id,
                AttendanceDate = DateTime.UtcNow
            };

            context.Attendances.Add(newAttendance);
            context.SaveChanges();
            message += "Attendance Added ||";


            if (newSub.PaymentHasBeenMade == true)
            {
                var newMoney = new Money
                {
                    SubscribeId = newSub.Id,
                    TotalAmount = TotalMoneycalc,
                    PaidAmount = TotalMoneycalc,
                    PaymentDate = DateTime.UtcNow
                };

                context.Moneys.Add(newMoney);
                context.SaveChanges();
                message += "Money Added ||";
            }
            else
            {
                var totalMoney = GetTotallmoney(newSub.Id);

                var totalPaid = GetTotalPaid(newSub.Id);




                var newMoney = new Money
                {
                    SubscribeId = newSub.Id,
                    TotalAmount = TotalMoneycalc,
                    PaidAmount = 0,
                    PaymentDate = DateTime.UtcNow
                };

                context.Moneys.Add(newMoney);
                context.SaveChanges();

            }

            MessageBox.Show(message);
            LoadSubscriptions(); // لو حابب تحدث الجدول بعد الإضافة
        }












        string message;

        private void AttendanceAdd()
        {
            int subId = int.Parse(TextboxId.Text);

            // 1. جلب الاشتراك المرتبط
            var subscription = context.Subscriptions.FirstOrDefault(s => s.Id == subId);

            if (subscription == null)
            {

                MessageBox.Show("Subscription not found!");
                return;
            }

            // 2. حساب عدد مرات الحضور السابقة
            int attendanceCount = context.Attendances.Count(a => a.SubscribeId == subId);

            // 3. تفسير مدة الاشتراك (مثال: "5 Times")
            int allowedAttendances = 0;


            switch (subscription.SubscriptionPeriod)
            {
                case "اشتراك يومى":
                    allowedAttendances = 1;
                    break;
                case "اشتراك شهرى":
                    allowedAttendances = 15; // افترض 7 أيام
                    break;
                case "اشتراك سنوى":
                    allowedAttendances = 200; // افترض 30 يوم
                    break;
                default:
                    allowedAttendances = 0;
                    break;

                 

            }


            // 4. التحقق هل ما زال مسموح له بالحضور
            if (attendanceCount >= allowedAttendances)
            {

                MessageBox.Show("This subscription has ended. No more attendance allowed.");
                return;
            }

            // 5. إضافة الحضور
            var newAttendance = new Attendance
            {
                SubscribeId = subId,
                AttendanceDate = DateTime.UtcNow
            };

            context.Attendances.Add(newAttendance);
            context.SaveChanges();

            MessageBox.Show("Attendance added successfully!");


            LoadSubscriptions();

        }



        private void clickrecord_Click(object sender, EventArgs e)
        {
            if (TypeofRecord == "Attendance")
            {
                AttendanceAdd();

            }
            else
            {
                SubscriptionAdd();
            }

        }

        string TypeofRecord;


        private void ButtonAttendance_Click(object sender, EventArgs e)
        {
            TypeofRecord = "Attendance";

            comboBoxPeriod.Visible = false;
            comboBoxType.Visible = false;
            checkBoxIPayment.Visible = false;
            labelMoney.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label8.Visible = false;
            TextboxId.Visible = true;
            label10.Visible = true;



        }

        private void ButtonSubscripe_Click(object sender, EventArgs e)
        {
            TypeofRecord = "Subscription";

            comboBoxPeriod.Visible = true;
            comboBoxType.Visible = true;
            checkBoxIPayment.Visible = true;
            labelMoney.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label8.Visible = true;
            TextboxId.Visible = false;
            label10.Visible = false;




        }

        private bool suppressTextChangedEvents = false;
        private void TextboxId_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChangedEvents) return;

            suppressTextChangedEvents = true;

            int subId;
            bool state = int.TryParse(TextboxId.Text, out subId);

            if (state == true)
            {
                var person = context.Subscriptions.Find(subId);

                if (person != null)
                {
                    textBoxName.Text = person.Name;
                }
                else
                {
                    textBoxName.Text = "";
                }
            }
            else
            {
                textBoxName.Text = "";
            }
            suppressTextChangedEvents = false;

        }
        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChangedEvents) return;

            suppressTextChangedEvents = true;


            string input = textBoxName.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(input))
            {
                // ابحث في الأسماء التي تحتوي على النص جزئياً وغير حساس لحالة الأحرف
                var person = context.Subscriptions
                    .FirstOrDefault(s => s.Name.ToLower().Contains(input));

                if (person != null)
                {
                    TextboxId.Text = person.Id.ToString();
                }
                else
                {
                    TextboxId.Text = ""; // لو مفيش تطابق
                }
            }
            else
            {
                TextboxId.Text = "";
            }

            suppressTextChangedEvents = false;

        }

        int MoneyType = 0;
        int MoneyPeriod = 0;
        int TotalMoneycalc = 0;

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {



            MoneyType = comboBoxType.SelectedItem.ToString() switch
            {
                "رفع اثقال" => 20,
                "كيك بوكسينغ" => 25,
                "لياقة بدنية" => 30,
                "علاج طبيعى" => 50,
                _ => 20
            };


            TotalMoneycalc = MoneyType * MoneyPeriod;
            labelMoney.Text = $"المبلغ المطلوب: {TotalMoneycalc} جنيه";

        }

        private void comboBoxPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {

            MoneyPeriod = comboBoxPeriod.SelectedItem.ToString() switch
            {
                "اشتراك يومى" => 1,
                "اشتراك شهرى" => 15,
                "اشتراك سنوى" => 100,
                _ => 1
            };
            TotalMoneycalc = MoneyType * MoneyPeriod;
            labelMoney.Text = $"المبلغ المطلوب: {TotalMoneycalc} جنيه";
        }

        private void checkBoxIPayment_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxIPayment.Checked = false;

        }

        private bool suppressEvents = false;

        private void paymentId_TextChanged(object sender, EventArgs e)
        {
            if (suppressEvents) return;

            suppressEvents = true;

            int subId;
            bool state = int.TryParse(paymentId.Text, out subId);

            if (state == true)
            {

                var person = context.Subscriptions.Find(subId);

                if (person != null)
                {
                    PaymentName.Text = person.Name;
                    try
                    {
                        label14.Text = (GetTotallmoney(subId) - GetTotalPaid(subId)).ToString();

                    }
                    catch
                    {

                    }
                }
                else
                {
                    PaymentName.Text = "";
                }
            }
            else
            {
                PaymentName.Text = "";
            }
            suppressEvents = false;



        }

        private void PaymentName_TextChanged(object sender, EventArgs e)
        {
            if (suppressEvents) return;

            suppressEvents = true;

            string input = PaymentName.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(input))
            {

                // ابحث في الأسماء التي تحتوي على النص جزئياً وغير حساس لحالة الأحرف
                var person = context.Subscriptions
                   .FirstOrDefault(s => s.Name.ToLower().Contains(input));

                if (person != null)
                {
                    paymentId.Text = person.Id.ToString();
                    try
                    {
                        label14.Text = (GetTotallmoney(int.Parse(paymentId.Text)) - GetTotalPaid(int.Parse(paymentId.Text))).ToString();

                    }
                    catch
                    {

                    }
                }
                else
                {
                    paymentId.Text = ""; // لو مفيش تطابق
                }
            }
            else
            {
                paymentId.Text = "";
            }
            suppressEvents = false;


        }



        private void money_Click(object sender, EventArgs e)
        {
            int subid;

            // تحقق من أن الإدخال رقم صالح
            bool state = int.TryParse(paymentId.Text, out subid);
            if (!state)
            {
                MessageBox.Show("الرجاء إدخال رقم اشتراك صحيح.");
                return;
            }
            var paymentState = context.Subscriptions
                .Where(i => i.Id == subid)
                .Select(s => s.PaymentHasBeenMade)
                .FirstOrDefault();

            if (paymentState == true)
            {
                MessageBox.Show("تم المفع مسبقا بالكامل ");
                return;
            }


            var totalMoney = GetTotallmoney(subid);

            var totalPaid = GetTotalPaid(subid);




            int paymentnow = 0;
            bool statepayment = int.TryParse(paymentmoney.Text, out paymentnow);

            if (!statepayment)
            {
                MessageBox.Show("الرجاء إدخال رقم دفع صحيح.");
                return;
            }

            var remaining = totalMoney - totalPaid;

            var totalpaidnow = totalPaid + paymentnow;

            if (remaining <= 0)
            {
                var subscription = context.Subscriptions.FirstOrDefault(i => i.Id == subid);

                if (subscription != null)
                {
                    subscription.PaymentHasBeenMade = true;

                    context.SaveChanges(); // حفظ التعديل في قاعدة البيانات
                }

                MessageBox.Show("لا يمكنك الدفع تم الانتهاء من الدفع ");
                return;
            }

            var newMoney = new Money
            {
                SubscribeId = subid,
                TotalAmount = totalMoney,
                PaidAmount = totalpaidnow,
                PaymentDate = DateTime.UtcNow
            };

            context.Moneys.Add(newMoney);
            context.SaveChanges();
            GetPaymentsBySubscriber();
            MessageBox.Show("paid Added succesuflly");

           

        }
        public decimal GetTotallmoney(int subid)
        {


            var totalRequired = context.Moneys
               .Where(m => m.SubscribeId == subid)
               .Select(s => (decimal)s.TotalAmount).OrderDescending().FirstOrDefault();

            return totalRequired;


        }
        public decimal GetTotalPaid(int subid)
        {
            var totalPaid = context.Moneys
               .Where(m => m.SubscribeId == subid)
               .Select(s => (decimal)s.PaidAmount).OrderDescending().FirstOrDefault();
            return totalPaid;

        }

        public List<object> GetSubscriptionsByPeriod(string? period)
        {
            return context.Subscriptions
                .Where(s => s.SubscriptionPeriod == period)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.SubscriptionPeriod,
                    s.Type,
                    PaymentStatus = s.PaymentHasBeenMade ,
                    AttendanceCount = s.Attendances.Count

                })
                .ToList<object>();
        }
        public List<object> GetSubscriptionsAll()
        {
            return context.Subscriptions
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.SubscriptionPeriod,
                    s.Type,
                    PaymentStatus = s.PaymentHasBeenMade,
                    AttendanceCount = s.Attendances.Count
                })
                .ToList<object>();
        }

        public List<object> GetSubscriptionsByType(string type)
        {
            return context.Subscriptions
                .Where(s => s.Type==type)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.SubscriptionPeriod,
                    s.Type,
                    PaymentStatus = s.PaymentHasBeenMade ,
                    AttendanceCount = s.Attendances.Count
                })
                .ToList<object>();
        }


        public List<object> GetAttendanceBySubscriber()
        {
            return context.Attendances
                .Select(a => new
                {
                    a.Id,
                    SubscriberName = a.Subscribe.Name, // الاسم من جدول الاشتراكات
                    a.AttendanceDate
                })
                .ToList<object>();
        }

        public List<PaymentRowDto> GetPaymentsBySubscriber()
        {
            var list= context.Moneys
                .Select(m => new PaymentRowDto
                {
                    Id = m.Id,
                    SubscriberName = m.Subscribe.Name,
                    TotalAmount = m.TotalAmount,
                    PaidAmount = m.PaidAmount,
                    PaymentDate = m.PaymentDate
                })
                .ToList();

            var TotalRequired= context.Moneys
                 .GroupBy(m => m.SubscribeId)
                 .Select(g => g.FirstOrDefault().TotalAmount)
                  .Sum();

            var totalPaid=context.Moneys.Select(s => s.PaidAmount).Sum();
            var countofpaid=context.Moneys.Where(s=>s.PaidAmount!=0).Count();

            list.Add(new PaymentRowDto
            {
                Id = null,
                SubscriberName = $"اجمالى عمليات الدفع {countofpaid}",
                TotalAmount = TotalRequired,
                PaidAmount = totalPaid,
                PaymentDate = null
            });

            return list;
        }



        private void Lists_SelectedValueChanged(object sender, EventArgs e)
        {
            string selectedOption = comboBoxLists.Text;

            switch (selectedOption)
            {
                case "الكل":

                    dataGridView1.DataSource = GetSubscriptionsAll();
                    break;
                case "اشتراك يومى":
                    
                    dataGridView1.DataSource= GetSubscriptionsByPeriod("اشتراك يومى");
                    break;

                case "اشتراك شهرى":
                    
                    dataGridView1.DataSource = GetSubscriptionsByPeriod("اشتراك شهرى");
                    break;

                case "اشتراك سنوى":
                    dataGridView1.DataSource = GetSubscriptionsByPeriod("اشتراك سنوى");
                    break;

                case "رفع اثقال":
                    dataGridView1.DataSource = GetSubscriptionsByType("رفع اثقال");
                    break;

                case "كيك بوكسينغ":
                    dataGridView1.DataSource = GetSubscriptionsByType("كيك بوكسينغ");
                    break;

                case "لياقة بدنية":
                    dataGridView1.DataSource = GetSubscriptionsByType("لياقة بدنية");
                    break;

                case "علاج طبيعى":
                    dataGridView1.DataSource = GetSubscriptionsByType("علاج طبيعى");
                    break;

                case "الحضور":
                    dataGridView1.DataSource = GetAttendanceBySubscriber();
                    break;

                case "المدفوعات":
                    dataGridView1.DataSource = GetPaymentsBySubscriber();
                    break;

                default:
                    MessageBox.Show("الرجاء اختيار نوع صحيح.");
                    break;
            }


        }

        public void Intializephotos()
        {
            
        }


    }


}

