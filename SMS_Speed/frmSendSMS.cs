using SMS_Speed.DTO;
using SMS_Speed.eSMS;
using SMS_Speed.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMS_Speed.Services;
namespace SMS_Speed
{
    public partial class frmSendSMS : Form
    {
        Dictionary<string, string> configs = new Dictionary<string, string>();
        private MemberServices MemberService = new MemberServices();
        private bool isFirstStartMonth = true;
        private bool isFirstStartDate = true;
        private List<BrandnameDTO> brandnameDTOs;
        private Dictionary<string, List<TemplateDTO>> templates = new Dictionary<string, List<TemplateDTO>>();
        public frmSendSMS()
        {
            InitializeComponent();
            lbBalance.Text = SMSFunction.CheckBalance();
            InitConfig();

            
        }
        
        
        private void InitConfig()
        {

            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                configs.Add(currentProperty.Name, currentProperty.DefaultValue.ToString());
            }

            int x = 150;
            int width = 470;
            foreach (var config in configs)
            {
                var label = new Label();
                label.Text = config.Key + ": ";
                label.AutoSize = true;
                label.Location = new Point(0, 0);

                Panel panel = new Panel();

                var textbox = new TextBox();
                textbox.Text = config.Value;
                textbox.Width = width;
                textbox.Location = new Point(x, 0);
                textbox.Name = config.Key;

                panel.Name = config.Key;
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);
                panel.AutoSize = true;

                flpConfig.Controls.Add(panel);
            }
     
        }

        private void WriteLog(string LogText)
        {
            try
            {
                rtbLog.BeginInvoke((Action)(() =>
                {
                    rtbLog.AppendText(DateTime.Now.ToString("[MM/dd/yyyy HH:mm:ss] ") + LogText + "\n");
                    rtbLog.Select(rtbLog.TextLength, 0);
                    rtbLog.ScrollToCaret();
                }));
            }
            catch (Exception) { }
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime GetNextMonthFirstDate(DateTime start)
        {
            // Get the next month
            return new DateTime(start.AddMonths(1).Year, start.AddMonths(1).Month, 1);
        }
        //EventUI


        private void btnSave_Click(object sender, EventArgs e)
        {
            var panel = flpConfig.Controls.OfType<Panel>();
            var panellst = panel.ToList();
            foreach (var item in panellst)
            {
                TextBox tbx = item.Controls.OfType<TextBox>().ToList()[0];
                configs[tbx.Name] = tbx.Text;
            }
            bool result;
/*            string err = Config.WriteConfigFile(configs, out result);*/
           /* if (result)
            {
                MessageBox.Show("Save successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void flpConfig_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


    
        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }
        

        private void BtnStart_Click(object sender, EventArgs e)
        {
            //DateTime test = GetNextWeekday(DateTime.Now.AddDays(1), DayOfWeek.Monday);
            //WriteLog(test.ToString());
            //tmrDate.Enabled = true;
            //tmrDate.Interval = 1000;
            //tmrDate.Start();
            tmrMonth.Enabled= true;
            tmrMonth.Interval = 1000;
            tmrMonth.Start();

        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            //tmrDate.Enabled = false;
            //tmrDate.Stop();

        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void tmrDate_Tick(object sender, EventArgs e)
        {
            // calculate the miliseconds of next Monday
            DateTime now = DateTime.Now;
            DateTime nextMonday = GetNextWeekday(now, DayOfWeek.Monday);
            string strNextDay = nextMonday.ToString("MM/dd/yyyy");
            /*   01 / 14 / 20231 / 13 / 2023 11:24:59 AM*/
            strNextDay += " 11:00:00 AM";
            double timeInterval = (DateTime.Parse(strNextDay) - now).TotalMilliseconds;
            if (timeInterval == 0)
            {
                Console.WriteLine(timeInterval);

            }
            /*      tmrDate.Interval = Convert.ToInt32(timeInterval);*/
            tmrDate.Interval = 1000;
            WriteLog(timeInterval.ToString());
        }

        private void btnExitApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            tmrDate.Stop();
            tmrDate.Enabled=false;
            tmrMonth.Enabled=false;
            tmrMonth.Stop();
        }

        private void tmrMonth_Tick(object sender, EventArgs e)
        {
            //int arg = DateTime.Now.Month;
            int arg = 11;
            if (!bgwMonth.IsBusy)
            {
                bgwMonth.RunWorkerAsync(argument: arg);
            }
            DateTime nextMonthFirst = GetNextMonthFirstDate(DateTime.Now);
            string strNextMonthFirst = nextMonthFirst.ToString("MM/dd/yyyy");
            strNextMonthFirst += " 11:00:00 AM";
            double timeInterval = (DateTime.Parse(strNextMonthFirst) - DateTime.Now).TotalMilliseconds; 
            if (timeInterval == 0)
            {
                Console.WriteLine(timeInterval);
            }
            tmrMonth.Interval= Convert.ToInt32(timeInterval);
            WriteLog(strNextMonthFirst);
            
        }

        private void bgwMonth_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string month = Convert.ToString(e.Argument);
                List<MemberDTO> customers = MemberService.GetMemberByBirthday(month);
                if(customers.Count > 0)
                {

                }
                else
                {
                    WriteLog("No Customers found!");
                }
                //foreach (MemberDTO cust in customers)
                //{
                //    WriteLog("Phone: " + cust.HomeTele);
                //}
                
                //WriteLog(SMSFunction.getBrandNames());
            }
            catch(Exception ex) { }
            
            
        }

        private void frmSendSMS_Load(object sender, EventArgs e)
        {
            brandnameDTOs = SMSFunction.getBrandNames();
            
            foreach (BrandnameDTO b in brandnameDTOs)
            {
                WriteLog($"Brandname: {b.BrandName}; type: {b.Type}");
                List<TemplateDTO> tempChilds = SMSFunction.GetTemplate(b.Type,b.BrandName);
                if (!templates.ContainsKey($"{b.BrandName}-{b.Type}"))
                {
                    templates.Add($"{b.BrandName}-{b.Type}", tempChilds);
                }
                foreach(TemplateDTO t in tempChilds)
                {
                    WriteLog($"{b.BrandName}-{b.Type}: Networ {t.NetworkID}, Content {t.TempContent}, ID {t.TempId}");
                }
            }

            
        }
    }
}
