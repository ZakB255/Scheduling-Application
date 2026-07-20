using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Scheduling_Application
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();
        }

        private void btnTypesByMonth_Click(object sender, EventArgs e)
        {
            try
            {
                DBConnection.StartConnection();

                string query =
                    "SELECT MONTH(start) AS Month, type AS Type, COUNT(*) AS Total " +
                    "FROM appointment " +
                    "GROUP BY MONTH(start), type " +
                    "ORDER BY Month";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                DataTable table = new DataTable();

                adapter.Fill(table);

                var reportRows = table.AsEnumerable()
                    .Where(row => row.ItemArray.Length > 0)
                    .ToList();

                dgvReports.DataSource = table;

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnUserSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                DBConnection.StartConnection();

                string query =
                    "SELECT u.userName AS User, c.customerName AS Customer, a.type AS Type, a.start AS Start, a.end AS End " +
                    "FROM appointment AS a " +
                    "INNER JOIN user AS u ON a.userId = u.userId " +
                    "INNER JOIN customer AS c ON a.customerId = c.customerId " +
                    "ORDER BY u.userName, a.start";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                DataTable table = new DataTable();

                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    row["Start"] = Convert.ToDateTime(row["Start"]).ToLocalTime();
                    row["End"] = Convert.ToDateTime(row["End"]).ToLocalTime();
                }

                var reportRows = table.AsEnumerable()
                    .Where(row => row.ItemArray.Length > 0)
                    .ToList();

                dgvReports.DataSource = table;

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnCustomReport_Click(object sender, EventArgs e)
        {
            try
            {
                DBConnection.StartConnection();

                string query =
                    "SELECT c.customerName AS Customer, COUNT(a.appointmentId) AS AppointmentCount " +
                    "FROM customer AS c " +
                    "LEFT JOIN appointment AS a ON c.customerId = a.customerId " +
                    "GROUP BY c.customerName " +
                    "ORDER BY AppointmentCount DESC";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                DataTable table = new DataTable();

                adapter.Fill(table);

                var reportRows = table.AsEnumerable()
                    .Where(row => row.ItemArray.Length > 0)
                    .ToList();

                dgvReports.DataSource = table;

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }
    }
}