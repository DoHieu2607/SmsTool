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
        private Member MemberService;
        public frmSendSMS()
        {
            InitializeComponent();
            cbxMonth.SelectedIndex = DateTime.Now.Month - 1;
            lbBalance.Text = SMSFunction.CheckBalance();
            InitConfig();
           
            
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            List<MemberDTO> custormer = MemberService.getMemberByBirthday("", "");
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void flpConfig_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

     
/*   private void cbxMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(cbxMonth.Text);
            Console.WriteLine("test");
        }*/
        private void cbxMonth_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            Console.WriteLine(cbxMonth.Text);
        
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void chooseDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(chooseDate.Text);
    
        }
    }
}
