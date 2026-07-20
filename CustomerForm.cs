using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Scheduling_Application
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtName.Text = dgvCustomers.Rows[e.RowIndex].Cells["customerName"].Value.ToString();
                txtAddress.Text = dgvCustomers.Rows[e.RowIndex].Cells["address"].Value.ToString();
                txtPhone.Text = dgvCustomers.Rows[e.RowIndex].Cells["phone"].Value.ToString();
            }
        }
        private void LoadCustomers()
        {
            try
            {
                DBConnection.StartConnection();

                string query = "SELECT c.customerId, c.customerName, a.address, a.phone FROM customer AS c INNER JOIN address AS a ON c.addressId = a.addressId";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, DBConnection.conn);
                DataTable table = new DataTable();

                adapter.Fill(table);

                dgvCustomers.DataSource = table;

                DBConnection.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Customer load failed: " + ex.Message);
            }
        }

        private void CustomersForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }
        private bool ValidateCustomer()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace (txtAddress.Text))
            {
                MessageBox.Show("Address is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Phone number is required");
                return false;
            }

            foreach (char c in txtPhone.Text)
            {
                if (!char.IsDigit(c) && c != '-')
                {
                    MessageBox.Show("Phone number can only contain digits and dashes");
                    return false;
                }
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateCustomer()) 
            {
                return;
            }

            try
            {
                DBConnection.StartConnection();

                string name = txtName.Text.Trim();
                string address = txtAddress.Text.Trim();
                string phone = txtPhone.Text.Trim();

                string insertAddress = @"INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@address, '', 1, '00000', @phone, NOW(), 'test', NOW(), 'test');";

                MySqlCommand addressCmd = new MySqlCommand(insertAddress, DBConnection.conn);
                addressCmd.Parameters.AddWithValue("@address", address);
                addressCmd.Parameters.AddWithValue("@phone", phone);
                addressCmd.ExecuteNonQuery();

                int addressId = (int)addressCmd.LastInsertedId;

                string insertCustomer = @"INSERT INTO customer (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES (@name, @addressId, 1, NOW(), 'test', NOW(), 'test');";

                MySqlCommand customerCmd = new MySqlCommand(insertCustomer, DBConnection.conn);
                customerCmd.Parameters.AddWithValue("@name", name);
                customerCmd.Parameters.AddWithValue("@addressId", addressId);
                customerCmd.ExecuteNonQuery();

                MessageBox.Show("Customer added successfully");

                LoadCustomers();

                txtName.Clear();
                txtAddress.Clear();
                txtPhone.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add customer failed: "+ ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateCustomer())
            {
                return;
            }

            try
            {
                DBConnection.StartConnection();

                int customerId = Convert.ToInt32(
                    dgvCustomers.CurrentRow.Cells["customerId"].Value);

                string updateCustomer =
                    "UPDATE customer SET customerName = @name, lastUpdate = NOW(), lastUpdateBy = NOW() WHERE customerId = @customerId";

                MySqlCommand cmd = new MySqlCommand(updateCustomer, DBConnection.conn);

                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@lastUpdateBy", "test");
                cmd.Parameters.AddWithValue("@customerId", customerId);

                cmd.ExecuteNonQuery();

                string updateAddress =
                    "UPDATE address AS a INNER JOIN customer AS c on c.addressId = a.addressId SET a.address = @address, a.phone = @phone, a.lastUpdate = NOW(), a.lastUpdateBy = @lastUpdateBy WHERE c.customerId = @customerId";

                MySqlCommand addressCmd = new MySqlCommand(updateAddress, DBConnection.conn);
                addressCmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                addressCmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                addressCmd.Parameters.AddWithValue("@lastUpdateBy", "test");
                addressCmd.Parameters.AddWithValue("@customerId", customerId);

                addressCmd.ExecuteNonQuery();
                DBConnection.CloseConnection();

                MessageBox.Show("Customer updated successfully.");

                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
                DBConnection.CloseConnection();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo
                );

            if (confirm != DialogResult.Yes) {
                return;
            }

            try
            {
                DBConnection.StartConnection();

                int customerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["customerId"].Value);

                string deleteCustomer = "DELETE FROM customer WHERE customerId = @customerId";

                MySqlCommand cmd = new MySqlCommand(deleteCustomer, DBConnection.conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Customer deleted.");

                LoadCustomers();

                txtName.Clear();
                txtAddress.Clear();
                txtPhone.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message );
                DBConnection.CloseConnection();
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();

            dgvCustomers.ClearSelection();
        }
    }
}
