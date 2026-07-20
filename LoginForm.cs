using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Scheduling_Application
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            RegionInfo region = new RegionInfo(CultureInfo.CurrentCulture.Name);
            lblLocation.Text = $"Location: {region.DisplayName}";

            TranslateLoginForm();
        }

        private void TranslateLoginForm()
        {
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "es")
            {
                lblUsername.Text = "Usuario";
                lblPassword.Text = "Contrasena";
                btnLogin.Text = "Iniciar sesion";
            }
            else
            {
                lblUsername.Text = "Username";
                lblPassword.Text = "Password";
                btnLogin.Text = "Login";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username != "test" || password != "test")
            {
                lblError.Text = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "es"
                    ? "El usuario y la contrasena no coinciden."
                    : "Username and password do not match.";
                return;
            }

            try
            {
                CheckUpcomingAppointments();

                File.AppendAllText(
                    "Login_History.txt",
                    $"{DateTime.Now:G} - {username} logged in{Environment.NewLine}"
                );

                this.Hide();

                MainForm mainform = new MainForm();
                mainform.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void CheckUpcomingAppointments()
        {
            try
            {
                DBConnection.StartConnection();

                DateTime nowUtc = DateTime.UtcNow;
                DateTime fifteenMinutesUtc = nowUtc.AddMinutes(15);

                string query =
                    "SELECT type, start " +
                    "FROM appointment " +
                    "WHERE start BETWEEN @now AND @fifteenMinutes " +
                    "ORDER BY start " +
                    "LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(query, DBConnection.conn);
                cmd.Parameters.AddWithValue("@now", nowUtc);
                cmd.Parameters.AddWithValue("@fifteenMinutes", fifteenMinutesUtc);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string type = reader["type"].ToString();
                    DateTime localStart = Convert.ToDateTime(reader["start"]).ToLocalTime();

                    MessageBox.Show($"Upcoming appointment: {type} at {localStart:g}");
                }
                else
                {
                    MessageBox.Show("You have no appointments scheduled within the next 15 minutes.");
                }

                reader.Close();
                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Appointment alert failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }
    }
}