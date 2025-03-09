using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using MySql.Data;
using MySql;
using NAudio.Wave;
using System.Diagnostics;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;

namespace ПРОТОН
{
    public partial class AutorizationFormLoadInformaion : Form
    {
       

       
        public AutorizationFormLoadInformaion()
        {
            InitializeComponent();
            PasswordBox.UsePasswordChar = true;
        }

        private void AutorizationFormLoadInformaion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void emailAuth_ContentChanged(object sender, EventArgs e)
        {
            if (emailAuth.Content == "Адресс почты") emailAuth.Content = null;
        }


        public bool dwitherPass = true;
        private void SwitherPassword_Click(object sender, EventArgs e)
        {
            dwitherPass = !dwitherPass;
            if (dwitherPass == true)
            {
                PasswordBox.UsePasswordChar = true;
            }
            if (dwitherPass == false)
            {
                PasswordBox.UsePasswordChar = false;
            }
        }

        private void ExiteButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите выйти?", "Оповещение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
            if (result == DialogResult.No)
            {

            }

        }
        ToolTip t = new ToolTip();
        private void EmailPicture_MouseHover(object sender, EventArgs e)
        {
            t.SetToolTip(EmailPicture, "Почта");
            EmailPicture.ImageLocation = "red@.png";
        }

        private void SwitherPassword_MouseHover(object sender, EventArgs e)
        {
            t.SetToolTip(SwitherPassword, "Пароль");
            SwitherPassword.ImageLocation = "iconRedKey.png";
        }

        private void EmailPicture_MouseLeave(object sender, EventArgs e)
        {
            EmailPicture.ImageLocation = "email.png";
        }

        private void SwitherPassword_MouseLeave(object sender, EventArgs e)
        {
            SwitherPassword.ImageLocation = "key.png";
        }

        private void AuthGuest_Click(object sender, EventArgs e)
        {
            MainPage main = new MainPage();

            main.oneClic = false;

            this.Hide();
        }

        private void AuthForCode_Click(object sender, EventArgs e)
        {
            

            MySqlConnectionStringBuilder stringbuilder = new MySqlConnectionStringBuilder();
            stringbuilder.Server = "111111111111111111";
            stringbuilder.UserID = "1111111111";
            stringbuilder.Password = "11111111";
            stringbuilder.Database = "111111111111";
            string connectionString = stringbuilder.ToString();
            string email = emailAuth.Content;
            string password = PasswordBox.Content;
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
              dt.Clear();
                string query = "SELECT * FROM `Users` WHERE `EmailUsers` = @Email AND `PasswordUsers` = @Password";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd)) 
                    {
                    adapter.Fill(dt);
                    }
                    
                }
            }
            if (dt.Rows.Count > 0) 
            {
            string Role = dt.Rows[0]["Role"].ToString();
                string Username = dt.Rows[0]["Nickname"].ToString();
                switch (Role) 
                {
                    case "Пользователь":
                        MainPage main = new MainPage();
                        main.Nicname.Text = Username;
                        main.oneClic=false;
                       this.Hide();
                        break;
                        
                }
            }
        }
    
        AudioFileReader audio = new AudioFileReader("us1.mp3");
        WaveOutEvent waveOut =new WaveOutEvent();
        private void Icon_MouseEnter(object sender, EventArgs e)
        {
            waveOut.Init(audio);
            waveOut.Play();
        }

        private void Icon_MouseLeave(object sender, EventArgs e)
        {
            waveOut.Stop();
        }

        private void Icon_Click(object sender, EventArgs e)
        {

        }

        private void Register_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://asdopkbxgm.temp.swtest.ru/");
        }

        private void AuthEmailCode_Click(object sender, EventArgs e)
        {
            string email = emailAuth.Content;
            if (email.Contains("@")==false)
            {
                MessageBox.Show("Введите почту!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else 
            {
                MySqlConnectionStringBuilder stringbuilder = new MySqlConnectionStringBuilder();
                stringbuilder.Server = "1111111111111";
                stringbuilder.UserID = "11111111111111";
                stringbuilder.Password = "1111111111111111";
                stringbuilder.Database = "11111111111111";
                stringbuilder.Port = 111111111111111;
                string connectionString = stringbuilder.ToString();
              
                DataTable dt = new DataTable();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    dt.Clear();
                    string query = "SELECT * FROM `Users` WHERE `EmailUsers` = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    var random = new Random();
                    var SendCode=random.Next(1111,9999);
                    string uemail = "1111111111111";

                    MailAddress from = new MailAddress(uemail);
                    MailAddress to = new MailAddress(email);
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = "Код";
                    m.IsBodyHtml = true;
                    m.Body = $"<!DOCTYPE html>\r\n<html lang=\"ru\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n  <title>Код авторизации</title>\r\n  <style>\r\n    /* Базовые стили */\r\n    body \r\n      font-family: Arial, sans-serif;\r\n      margin: 0;\r\n      padding: 0;\r\n      background-color: #f4f4f4;\r\n    \r\n\r\n    table \r\n      width: 100%;\r\n      max-width: 600px;\r\n      margin: 20px auto;\r\n      background-color: #ffffff;\r\n      border-radius: 8px;\r\n      overflow: hidden;\r\n      border: 1px solid #dddddd;\r\n    \r\n\r\n    td \r\n      padding: 20px;\r\n      color: #333333;\r\n    \r\n\r\n    h1 \r\n      font-size: 24px;\r\n      color: #4CAF50;\r\n      margin-bottom: 20px;\r\n    \r\n\r\n    p \r\n      font-size: 16px;\r\n      line-height: 1.5;\r\n      color: #666666;\r\n    \r\n\r\n    .code \r\n      font-size: 24px;\r\n      font-weight: bold;\r\n      color: #333333;\r\n      background-color: #f1f1f1;\r\n      padding: 15px;\r\n      border-radius: 5px;\r\n      text-align: center;\r\n      margin-top: 20px;\r\n      letter-spacing: 3px;\r\n    \r\n\r\n    .footer \r\n      font-size: 12px;\r\n      text-align: center;\r\n      color: #999999;\r\n      padding-top: 10px;\r\n      border-top: 1px solid #dddddd;\r\n    \r\n\r\n    .button \r\n      display: inline-block;\r\n      padding: 10px 20px;\r\n      background-color: #4CAF50;\r\n      color: white;\r\n      text-decoration: none;\r\n      border-radius: 5px;\r\n      margin-top: 20px;\r\n    \r\n\r\n    .button:hover \r\n      background-color: #45a049;\r\n    \r\n  </style>\r\n</head>\r\n<body>\r\n  <table>\r\n    <tr>\r\n      <td>\r\n        <h1>Ваш код авторизации</h1>\r\n        <p>Здравствуйте,</p>\r\n        <p>Для подтверждения вашей личности и завершения процесса авторизации, пожалуйста, используйте следующий код:</p>\r\n        \r\n        <div class=\"code\">\r\n          {SendCode}\r\n        </div>\r\n\r\n        <p>Введите этот код в поле на приложения для продолжения.</p>\r\n\r\n        <p>Если вы не совершали запрос на авторизацию, пожалуйста, игнорируйте это сообщение.</p>\r\n\r\n        <div class=\"footer\">\r\n          <p>Это автоматическое сообщение. Пожалуйста, не отвечайте на него.</p>\r\n          <p>&copy; 2025 Протон</p>\r\n        </div>\r\n      </td>\r\n    </tr>\r\n  </table>\r\n</body>\r\n</html>\r\n";
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 587)
                    {
                        Credentials = new NetworkCredential("111111111111111111111u", "111111111111111111"),
                        EnableSsl = true,
                        Timeout = 30000,
                       
                    };
                    smtp.Send(m);
                    EmailCodeChec codeChec = new EmailCodeChec();
                    codeChec.SendCode=SendCode;
                    codeChec.ShowDialog();
                }
            }
        }
    }
}
