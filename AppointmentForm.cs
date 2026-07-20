using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Scheduling_Application
{
    public partial class AppointmentForm : Form
    {
        public AppointmentForm()
        {
            InitializeComponent();
        }

        private int GetCurrentUserId()
        {
            string query = "SELECT userId FROM user WHERE userName = @userName";

            MySqlCommand cmd = new MySqlCommand(query, DBConnection.conn);
            cmd.Parameters.AddWithValue("@userName", "test");

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private void ConvertAppointmentTimesToLocal(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                row["start"] = Convert.ToDateTime(row["start"]).ToLocalTime();
                row["end"] = Convert.ToDateTime(row["end"]).ToLocalTime();
            }
        }

        private void LoadAppointments()
        {
            try
            {
                DBConnection.StartConnection();

                string query = "SELECT a.appointmentId, a.customerId, a.type, a.start, a.end FROM appointment AS a";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                DataTable table = new DataTable();

                adapter.Fill(table);

                ConvertAppointmentTimesToLocal(table);

                dgvAppointments.DataSource = table;

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Appointments load failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            dgvAppointments.ReadOnly = true;
            dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAppointments.MultiSelect = false;
            dgvAppointments.AllowUserToAddRows = false;
            dgvAppointments.AllowUserToDeleteRows = false;

            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.CustomFormat = "MM/dd/yyyy hh:mm tt";
            dtpStart.ShowUpDown = true;

            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.CustomFormat = "MM/dd/yyyy hh:mm tt";
            dtpEnd.ShowUpDown = true;

            LoadCustomers();
            LoadAppointments();
        }

        private void LoadCustomers()
        {
            try
            {
                DBConnection.StartConnection();

                string query = "SELECT customerId, customerName FROM customer";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                DataTable table = new DataTable();

                adapter.Fill(table);

                cmbCustomer.DataSource = table;
                cmbCustomer.DisplayMember = "customerName";
                cmbCustomer.ValueMember = "customerId";

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Customer combo load failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private bool IsWithinBusinessHours(DateTime start, DateTime end)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            DateTime easternStart = TimeZoneInfo.ConvertTime(start, TimeZoneInfo.Local, easternZone);
            DateTime easternEnd = TimeZoneInfo.ConvertTime(end, TimeZoneInfo.Local, easternZone);

            if (easternStart.DayOfWeek == DayOfWeek.Saturday || easternStart.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            TimeSpan businessStart = new TimeSpan(9, 0, 0);
            TimeSpan businessEnd = new TimeSpan(17, 0, 0);

            return easternStart.TimeOfDay >= businessStart && easternEnd.TimeOfDay <= businessEnd;
        }

        private bool ValidateAppointment(int appointmentId = 0)
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtType.Text))
            {
                MessageBox.Show("Appointment type is required.");
                return false;
            }

            if (dtpEnd.Value <= dtpStart.Value)
            {
                MessageBox.Show("End time must be after start time.");
                return false;
            }

            if (!IsWithinBusinessHours(dtpStart.Value, dtpEnd.Value))
            {
                MessageBox.Show("Appointments must be scheduled Monday-Friday between 9:00 AM and 5:00 PM eastern time.");
                return false;
            }

            if (HasOverlappingAppointment(dtpStart.Value, dtpEnd.Value, appointmentId))
            {
                MessageBox.Show("Appointment times cannot overlap with an existing appointment.");
                return false;
            }

            return true;
        }

        private bool HasOverlappingAppointment(DateTime start, DateTime end, int appointmentId = 0)
        {
            try
            {
                DBConnection.StartConnection();

                DateTime startUtc = start.ToUniversalTime();
                DateTime endUtc = end.ToUniversalTime();

                string query = "SELECT COUNT(*) FROM appointment WHERE appointmentId != @appointmentId AND start < @end AND end > @start";

                MySqlCommand cmd = new MySqlCommand(query, DBConnection.conn);
                cmd.Parameters.AddWithValue("@appointmentId", appointmentId);
                cmd.Parameters.AddWithValue("@start", startUtc);
                cmd.Parameters.AddWithValue("@end", endUtc);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                DBConnection.CloseConnection();

                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Overlap check failed: " + ex.Message);
                DBConnection.CloseConnection();
                return true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateAppointment())
            {
                return;
            }

            try
            {
                DBConnection.StartConnection();

                int customerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                int userId = GetCurrentUserId();

                DateTime startUtc = dtpStart.Value.ToUniversalTime();
                DateTime endUtc = dtpEnd.Value.ToUniversalTime();

                string query = "INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, '', @start, @end, NOW(), @createdBy, NOW(), @lastUpdateBy)";

                MySqlCommand cmd = new MySqlCommand(query, DBConnection.conn);

                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@title", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("@description", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("@location", "Online");
                cmd.Parameters.AddWithValue("@contact", "test");
                cmd.Parameters.AddWithValue("@type", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("@start", startUtc);
                cmd.Parameters.AddWithValue("@end", endUtc);
                cmd.Parameters.AddWithValue("@createdBy", "test");
                cmd.Parameters.AddWithValue("@lastUpdateBy", "test");

                cmd.ExecuteNonQuery();

                DBConnection.CloseConnection();

                MessageBox.Show("Appointment added successfully.");

                LoadAppointments();

                txtType.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add appointment failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void dgvAppointments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                cmbCustomer.SelectedValue = dgvAppointments.Rows[e.RowIndex].Cells["customerId"].Value;

                txtType.Text = dgvAppointments.Rows[e.RowIndex].Cells["type"].Value.ToString();

                dtpStart.Value = Convert.ToDateTime(dgvAppointments.Rows[e.RowIndex].Cells["start"].Value);
                dtpEnd.Value = Convert.ToDateTime(dgvAppointments.Rows[e.RowIndex].Cells["end"].Value);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                MessageBox.Show("Please select an appointment to update.");
                return;
            }

            int appointmentId = Convert.ToInt32(dgvAppointments.CurrentRow.Cells["appointmentId"].Value);

            if (!ValidateAppointment(appointmentId))
            {
                return;
            }

            try
            {
                DBConnection.StartConnection();

                int customerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                int userId = GetCurrentUserId();

                DateTime startUtc = dtpStart.Value.ToUniversalTime();
                DateTime endUtc = dtpEnd.Value.ToUniversalTime();

                string query = "UPDATE appointment SET customerId = @customerId, userId = @userId, title = @title, description = @description, location = @location, contact = @contact, type = @type, start = @start, end = @end, lastUpdate = NOW(), lastUpdateBy = @lastUpdateBy WHERE appointmentId = @appointmentId";

                MySqlCommand cmd = new MySqlCommand(query, DBConnection.conn);

                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@title", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("@description", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("@location", "Online");
                cmd.Parameters.AddWithValue("@contact", "test");
                cmd.Parameters.AddWithValue("@type", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("@start", startUtc);
                cmd.Parameters.AddWithValue("@end", endUtc);
                cmd.Parameters.AddWithValue("@lastUpdateBy", "test");
                cmd.Parameters.AddWithValue("@appointmentId", appointmentId);

                cmd.ExecuteNonQuery();

                DBConnection.CloseConnection();

                MessageBox.Show("Appointment updated successfully.");

                LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                MessageBox.Show("Please select an appointment to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this appointment?",
                "Confirm Delete",
                MessageBoxButtons.YesNo
            );

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                DBConnection.StartConnection();

                int appointmentId = Convert.ToInt32(dgvAppointments.CurrentRow.Cells["appointmentId"].Value);

                string query = "DELETE FROM appointment WHERE appointmentId = @appointmentId";

                MySqlCommand cmd = new MySqlCommand(query, DBConnection.conn);
                cmd.Parameters.AddWithValue("@appointmentId", appointmentId);

                cmd.ExecuteNonQuery();

                DBConnection.CloseConnection();

                MessageBox.Show("Appointment deleted successfully.");

                LoadAppointments();

                txtType.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete appointment failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtType.Clear();
            cmbCustomer.SelectedIndex = -1;
            dgvAppointments.ClearSelection();

            dtpStart.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now.AddMinutes(30);
        }

        private void LoadAppointmentsByDate(DateTime selectedDate)
        {
            try
            {
                DBConnection.StartConnection();

                DateTime startOfDayUtc = selectedDate.Date.ToUniversalTime();
                DateTime endOfDayUtc = selectedDate.Date.AddDays(1).ToUniversalTime();

                string query = "SELECT a.appointmentId, a.customerId, a.type, a.start, a.end FROM appointment AS a WHERE a.start >= @startOfDay AND a.start < @endOfDay";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                adapter.SelectCommand.Parameters.AddWithValue("@startOfDay", startOfDayUtc);
                adapter.SelectCommand.Parameters.AddWithValue("@endOfDay", endOfDayUtc);

                DataTable table = new DataTable();
                adapter.Fill(table);

                ConvertAppointmentTimesToLocal(table);

                dgvAppointments.DataSource = table;

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Calendar view failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnViewDay_Click(object sender, EventArgs e)
        {
            LoadAppointmentsByDate(monthCalendar.SelectionStart);
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadAppointments();
        }
    }
}