using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Mail;


namespace DelayDemoForms
{

	enum Scheduler
    {
        EveryMinutes,
        EveryHour,
        EveryHalfDay,
        EveryDay,
        EveryWeek,
        EveryMonth,
        EveryYear,
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            numHours.Value = DateTime.Now.Hour;
            numMins.Value = DateTime.Now.Minute;
			

		}
        CancellationTokenSource m_ctSource;

		public string strDate;
		public string StrDate2;

		/// <summary>
		/// Setting up time for running the code
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void startBtn_Click(object sender, EventArgs e)
        {

            //retrieve hour and minute from the form
            int hour = (int)numHours.Value;
            int minutes = (int)numMins.Value;

            //create next date which we need in order to run the code
            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, hour, minutes, 0);

            listBox1.Items.Add("**********Görevler  Başladı*****");

            //get nex date the code need to run
            var nextDateValue = getNextDate(date, getScheduler());

            runCodeAt(nextDateValue, getScheduler());

        }


        /// <summary>
        /// Schedule the time the need to be call
        /// </summary>
        /// <param name="date"></param>
        /// <param name="scheduler"></param>
        private void runCodeAt(DateTime date, Scheduler scheduler)
        {
            m_ctSource = new CancellationTokenSource();

            var dateNow = DateTime.Now;
            TimeSpan ts;
            if (date > dateNow)
                ts = date - dateNow;
            else
            {
                date = getNextDate(date, scheduler);
                ts = date - dateNow;
            }

            //enable the progressbar
            prepareControlForStart();



            //waits certan time and run the code, in meantime you can cancel the task at anty time
            Task.Delay(ts).ContinueWith((x) =>
                {
                    //run the code at the time
                    methodToCall(date);

                    //setup call next day
                    runCodeAt(getNextDate(date, scheduler), scheduler);

                }, m_ctSource.Token);
        }

        /// <summary>
        /// prepare the controls for starting scheduler
        /// </summary>
        private void prepareControlForStart()
        {
            progressBar1.Enabled = true;
            progressBar1.Style = ProgressBarStyle.Marquee;
            button1.Enabled = false;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            button3.Enabled = true;
        }
        /// <summary>
        /// prepare the controls for canceling the scheduler
        /// </summary>
        private void prepareControlsForCancel()
        {
            m_ctSource = null;
            progressBar1.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Blocks;
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            button3.Enabled = false;
            button1.Enabled = true;
        }
        /// <summary>
        /// returns next date the code to be run
        /// </summary>
        /// <param name="date"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        private DateTime getNextDate(DateTime date, Scheduler scheduler)
        {
            switch (scheduler)
            {
                case Scheduler.EveryMinutes:
                    return date.AddMinutes(1);
                case Scheduler.EveryHour:
                    return date.AddHours(1);
                case Scheduler.EveryHalfDay:
                    return date.AddHours(12);
                case Scheduler.EveryDay:
                    return date.AddDays(1);
                case Scheduler.EveryWeek:
                    return date.AddDays(7);
                case Scheduler.EveryMonth:
                    return date.AddMonths(1);
                case Scheduler.EveryYear:
                    return date.AddYears(1);
                default:
                    throw new Exception("geçersiz planlama");
            }

        }

        /// <summary>
        /// method to be called after period of time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private void methodToCall(DateTime time)
        {
            //setup next call
            var nextTimeToCall = getNextDate(time, getScheduler());

            this.BeginInvoke((Action)(() =>
            {
                var strText = string.Format("Mail   {0}'te Gönderildi. bir sonraki çalışma süresi {1}", time, nextTimeToCall);
                griddoldur();
                gonder();
                
                listBox1.Items.Add(strText);
                //MessageBox.Show(strText);

            }

            ));



        }

        /// <summary>
        /// based on the selected radion box returns the scheduler enum
        /// </summary>
        /// <returns></returns>
        private Scheduler getScheduler()
        {
            if (radioButton1.Checked)
                return Scheduler.EveryMinutes;
            if (radioButton2.Checked)
                return Scheduler.EveryHour;
            if (radioButton3.Checked)
                return Scheduler.EveryHalfDay;
            if (radioButton4.Checked)
                return Scheduler.EveryDay;
            if (radioButton5.Checked)
                return Scheduler.EveryWeek;
            if (radioButton6.Checked)
                return Scheduler.EveryMonth;
            if (radioButton7.Checked)
                return Scheduler.EveryYear;

            //default
            return Scheduler.EveryMinutes;
        }

        /// <summary>
        /// canceling the sheduler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(" Bütün planlı görevler duracak emin misin?", "Durdur", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ///////
                if (m_ctSource != null)
                {
                    m_ctSource.Cancel();
                    prepareControlsForCancel();

                    listBox1.Items.Add("**********Görevler Durduruldu!*****");
                }
                
            }
          


           
        }


        /// <summary>
        /// Exits the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitBtn_Click(object sender, EventArgs e)
        {
           
            if (progressBar1.Enabled)
                MessageBox.Show("Planlanmış görev var ilk önce durdur!");
            else
                if (MessageBox.Show("Güle Güle","Çıkış", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                Form2 form2 = new Form2();
                form2.Show();

            }
            
            
        }

        void griddoldur()
        {
			DateTime dt = DateTime.Now;
			strDate = dt.ToShortDateString();
			StrDate2 = dt.AddDays(1).ToShortDateString();

			string connectionString = "Data Source=192.168.0.24;Initial Catalog=Genius3;User ID=GENIUS4;Password=GENIUSOPEN";
            string sql = "SELECT CS.CODE,CS.NAME,ADDRESS_HOME,ADDRESS_LETTER,URL,CELL_PHONE FROM CUSTOMER CS JOIN CUSTOMER_EXTENSION CE ON CS.ID=CE.FK_CUSTOMER WHERE CS.FK_USER_MODIFY IN ('12048','12049')";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter dataadapter = new SqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "toyzz");
            connection.Close();
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "toyzz";


        }
        public string GetTableFromDataGrid()
        {
			
			

            StringBuilder strTable = new StringBuilder();
            try
            {strTable.Append("<font face='Arial' > ");
				strTable.Append("<table border='1' cellpadding='0' cellspacing='0' width='1024px' >");


				strTable.Append("<font face='Arial' > ");
				strTable.Append("<table border='1' cellpadding='0' cellspacing='0' width='1024px' >");
				
				




				strTable.Append("<tr>");
                //Create Header Row for Table
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    strTable.AppendFormat("<th><font color='RED'>{0}</font></th>", col.HeaderText);
                }
                strTable.Append("</tr>");
                //Create Table Rows
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    strTable.Append("<tr>");
                    foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                    {
                        if (cell.Value != null)
                        {
                            strTable.AppendFormat("<td'>{0}</td>", cell.Value.ToString());
							
						}
                    }
                    strTable.Append("</tr>");
                }
				
				strTable.Append("</table>");
				strTable.Append("</font>");
			}
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return strTable.ToString();

        }


        void gonder()
        {

            {
                try
                {
                    //Create instance of MailMessage Class
                    MailMessage mail = new MailMessage();
                    //Assign From mail address
                    mail.From = new MailAddress("tamer@toyzzshop.com");
                    //Set To mail address
                    mail.To.Add(new MailAddress("esra@toyzzshop.com"));
                    MailAddress copy = new MailAddress("tamer@toyzzshop.com");
                    mail.CC.Add(copy);

                    //Set Subject of mail
                    mail.Subject = Convert.ToString(strDate)+ " Tarihli Rapor" ;
					//Create Mail Body
					mail.Body = Convert.ToString(strDate) + "  girilen TCC Kartlar<br /> " +
                        "<br />" +
                       
                        "bilginize<br />" +
                        "İyi Çalışmalar."+
                        " <br /><br /><br />";
					
						
					

					mail.BodyEncoding= System.Text.Encoding.UTF8;
					
					
                    mail.Body += GetTableFromDataGrid();
                    //for sending body as HTML
                    mail.IsBodyHtml = true;
                    //Create Instance of SMTP Class
                    SmtpClient SmtpServer = new SmtpClient();
                    //Assign Host
                    SmtpServer.Host = "smtp.gmail.com";
                    //Assign Post Number
                    SmtpServer.Port = 587;
                    //Setting the credential for authentiicate the sender
                    SmtpServer.Credentials = new System.Net.NetworkCredential("tamer@toyzzshop.com", "23e4r5t6y.");
                    //Enable teh Secure Soket Layer to Encrypte the connection 
                    SmtpServer.EnableSsl = true;
                    //Sending the message
                    SmtpServer.Send(mail);
                    //MessageBox.Show("Mail Sends Successfully!!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
			

		
		}
		


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}