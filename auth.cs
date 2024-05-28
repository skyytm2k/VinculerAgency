using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using MimeKit;

namespace adagent
{
    public partial class auth : Form
    {
        private Random random = new Random();
        private const string CharsAndDigits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Database database = new Database();
        public auth()
        {
            InitializeComponent();
        }
        public static string klient = "";
        public static string адрес = "";
        public static string фио = "";
        public static string баланс = "";
        public static string code = "";
        private void lbExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            var login = tblog.Text;
            var pass = tbpass.Text;
            code = GenerateRandomCode(6);
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string querystring = $"select authid,mail,password, admin from auth where mail = '{login}' and password = '{pass}'";
            SqlCommand command = new SqlCommand(querystring, database.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                klient = table.Rows[0][0].ToString();
                адрес = table.Rows[0][2].ToString();
                фио = table.Rows[0][1].ToString();
                баланс = table.Rows[0][3].ToString();
                SqlDataAdapter adapter1 = new SqlDataAdapter();
                DataTable table1 = new DataTable();

                string querystring1 = $"select admin, mail, password from auth where mail = '{login}' and password = '{pass}' and admin = '{true}'";
                SqlCommand command1 = new SqlCommand(querystring1, database.getConnection());

                adapter1.SelectCommand = command1;
                adapter1.Fill(table1);

                if (table1.Rows.Count == 1)
                {
                    SendEmail();
                    tblog.Enabled = false;
                    tbpass.Enabled = false;
                    label6.Visible = true;
                    tbcode.Visible = true;
                    btnEnter.Visible = false;
                    bntcode.Visible = true;
                    MessageBox.Show("Для успешного входа введите код из письма", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SendEmail();
                    tblog.Enabled = false;
                    tbpass.Enabled = false;
                    label6.Visible = true;
                    tbcode.Visible = true;
                    btnEnter.Visible = false;
                    btncodeus.Visible = true;
                    MessageBox.Show("Для успешного входа введите код из письма", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else

            {
                MessageBox.Show("Такого аккаунта не существует!", "Аккаунта не существует!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void SendEmail()
        {
            if (tblog.Text != "" || tbpass.Text != "")
            {
                var message = new MimeMessage(); //объявление переменной
                message.From.Add(new MailboxAddress("'agentsVinculer'", "niki_g0606@mail.ru")); // первые кавычки имя отправителя(можно любое), вторые кавычки почта, откуда будет отправляется письмо
                message.To.Add(new MailboxAddress("Пользователь", tblog.Text));//тут задаем куда будет отправлятся письмо(пользователь сам вводит данные в textbox-ы)
                message.Subject = "Код для подтверждение регистрации";//заголовок письма(при получении письма будет писаться в самом начале)

                var bodyBuilder = new BodyBuilder();//объявление переменной
                bodyBuilder.TextBody = "Проверочный код для успешной регистрации в приложении agentsVinculer \n" + code;//текст отправленного письма
                message.Body = bodyBuilder.ToMessageBody();//

                using (var client = new SmtpClient())//
                {
                    client.Connect("smtp.mail.ru", 587, false);//подключаем smtp сервер(можно узнать в интернете)
                    client.Authenticate("niki_g0606@mail.ru", "9zFvdeLKNCi47bv3sLsF");//в первые кавычки вводим почту откуда отправляется письмо, вторые кавычки код, для внешних приложений(создается только с подтверждённым номером телефона)
                    client.Send(message);// отправка сообщения
                    client.Disconnect(true);// отключаемся от smtp сервера
                }
            }
        }
        private string GenerateRandomCode(int length)
        {
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = CharsAndDigits[random.Next(CharsAndDigits.Length)];
            }
            return new string(code);
        }

        private void bntcode_Click(object sender, EventArgs e)
        {
            if (tbcode.Text == code)
            {
                MessageBox.Show("Вы успешно вошли как админ!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                menu frm1 = new menu();
                this.Hide();
                frm1.ShowDialog();
                this.Show();
                tblog.Text = "";
                tbpass.Text = "";
            }
            else
            {
                MessageBox.Show("Введённый код неверен!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
         
        private void btncodeus_Click(object sender, EventArgs e)
        {
            if (tbcode.Text == code)
            {
                MessageBox.Show("Вы успешно вошли как работник!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                usermenu frm1 = new usermenu();
                this.Hide();
                frm1.ShowDialog();
                this.Show();
                tblog.Text = "";
                tbpass.Text = "";
            }
            else
            {
                MessageBox.Show("Введённый код неверен!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }

        private void lbClear_Click(object sender, EventArgs e)
        {
            tbcode.Clear();
            tblog.Enabled = true;
            tblog.Clear();
            tbpass.Enabled = true;
            tbpass.Clear();
            label6.Visible = false;
            tbcode.Visible = false;
            btnEnter.Visible = true;
            bntcode.Visible = false;
            btncodeus.Visible = false;
        }
    }
}
