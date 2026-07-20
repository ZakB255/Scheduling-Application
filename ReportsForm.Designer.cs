namespace Scheduling_Application
{
    partial class ReportsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTypesByMonth = new System.Windows.Forms.Button();
            this.btnUserSchedule = new System.Windows.Forms.Button();
            this.btnCustomReport = new System.Windows.Forms.Button();
            this.dgvReports = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.SuspendLayout();
            // 
            // btnTypesByMonth
            // 
            this.btnTypesByMonth.Location = new System.Drawing.Point(120, 27);
            this.btnTypesByMonth.Name = "btnTypesByMonth";
            this.btnTypesByMonth.Size = new System.Drawing.Size(75, 70);
            this.btnTypesByMonth.TabIndex = 0;
            this.btnTypesByMonth.Text = "Types By Month";
            this.btnTypesByMonth.UseVisualStyleBackColor = true;
            this.btnTypesByMonth.Click += new System.EventHandler(this.btnTypesByMonth_Click);
            // 
            // btnUserSchedule
            // 
            this.btnUserSchedule.Location = new System.Drawing.Point(120, 103);
            this.btnUserSchedule.Name = "btnUserSchedule";
            this.btnUserSchedule.Size = new System.Drawing.Size(75, 73);
            this.btnUserSchedule.TabIndex = 1;
            this.btnUserSchedule.Text = "User Schedule";
            this.btnUserSchedule.UseVisualStyleBackColor = true;
            this.btnUserSchedule.Click += new System.EventHandler(this.btnUserSchedule_Click);
            // 
            // btnCustomReport
            // 
            this.btnCustomReport.Location = new System.Drawing.Point(120, 182);
            this.btnCustomReport.Name = "btnCustomReport";
            this.btnCustomReport.Size = new System.Drawing.Size(75, 59);
            this.btnCustomReport.TabIndex = 2;
            this.btnCustomReport.Text = "Customer Appointment Count";
            this.btnCustomReport.UseVisualStyleBackColor = true;
            this.btnCustomReport.Click += new System.EventHandler(this.btnCustomReport_Click);
            // 
            // dgvReports
            // 
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.Location = new System.Drawing.Point(223, 12);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.Size = new System.Drawing.Size(565, 246);
            this.dgvReports.TabIndex = 3;
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvReports);
            this.Controls.Add(this.btnCustomReport);
            this.Controls.Add(this.btnUserSchedule);
            this.Controls.Add(this.btnTypesByMonth);
            this.Name = "ReportsForm";
            this.Text = "ReportsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTypesByMonth;
        private System.Windows.Forms.Button btnUserSchedule;
        private System.Windows.Forms.Button btnCustomReport;
        private System.Windows.Forms.DataGridView dgvReports;
    }
}