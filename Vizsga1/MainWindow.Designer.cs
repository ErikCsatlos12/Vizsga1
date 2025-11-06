namespace UMFST.MIP.Bookstore
{
    partial class MainWindow
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabBooks = new System.Windows.Forms.TabPage();
            this.gridBooks = new System.Windows.Forms.DataGridView();
            this.panelBookControls = new System.Windows.Forms.Panel();
            this.btnSearchBooks = new System.Windows.Forms.Button();
            this.txtSearchBooks = new System.Windows.Forms.TextBox();
            this.btnRestock = new System.Windows.Forms.Button();
            this.tabOrders = new System.Windows.Forms.TabPage();
            this.splitOrders = new System.Windows.Forms.SplitContainer();
            this.gridOrders = new System.Windows.Forms.DataGridView();
            this.gridOrderItems = new System.Windows.Forms.DataGridView();
            this.lblOrderTotal = new System.Windows.Forms.Label();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.txtReportPreview = new System.Windows.Forms.TextBox();
            this.btnExportReport = new System.Windows.Forms.Button();
            this.panelTopControls = new System.Windows.Forms.Panel();
            this.btnImportData = new System.Windows.Forms.Button();
            this.btnResetDatabase = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlMain.SuspendLayout();
            this.tabBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBooks)).BeginInit();
            this.panelBookControls.SuspendLayout();
            this.tabOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitOrders)).BeginInit();
            this.splitOrders.Panel1.SuspendLayout();
            this.splitOrders.Panel2.SuspendLayout();
            this.splitOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrderItems)).BeginInit();
            this.tabReports.SuspendLayout();
            this.panelTopControls.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabBooks);
            this.tabControlMain.Controls.Add(this.tabOrders);
            this.tabControlMain.Controls.Add(this.tabReports);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 50);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(800, 374);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabBooks
            // 
            this.tabBooks.Controls.Add(this.gridBooks);
            this.tabBooks.Controls.Add(this.panelBookControls);
            this.tabBooks.Location = new System.Drawing.Point(4, 25);
            this.tabBooks.Name = "tabBooks";
            this.tabBooks.Padding = new System.Windows.Forms.Padding(3);
            this.tabBooks.Size = new System.Drawing.Size(792, 345);
            this.tabBooks.TabIndex = 0;
            this.tabBooks.Text = "Books";
            this.tabBooks.UseVisualStyleBackColor = true;
            // 
            // gridBooks
            // 
            this.gridBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBooks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridBooks.Location = new System.Drawing.Point(3, 43);
            this.gridBooks.Name = "gridBooks";
            this.gridBooks.RowHeadersWidth = 51;
            this.gridBooks.RowTemplate.Height = 24;
            this.gridBooks.Size = new System.Drawing.Size(786, 299);
            this.gridBooks.TabIndex = 2;
            // 
            // panelBookControls
            // 
            this.panelBookControls.Controls.Add(this.btnSearchBooks);
            this.panelBookControls.Controls.Add(this.txtSearchBooks);
            this.panelBookControls.Controls.Add(this.btnRestock);
            this.panelBookControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBookControls.Location = new System.Drawing.Point(3, 3);
            this.panelBookControls.Name = "panelBookControls";
            this.panelBookControls.Size = new System.Drawing.Size(786, 40);
            this.panelBookControls.TabIndex = 1;
            // 
            // btnSearchBooks
            // 
            this.btnSearchBooks.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSearchBooks.Location = new System.Drawing.Point(636, 0);
            this.btnSearchBooks.Name = "btnSearchBooks";
            this.btnSearchBooks.Size = new System.Drawing.Size(75, 40);
            this.btnSearchBooks.TabIndex = 2;
            this.btnSearchBooks.Text = "Keresés";
            this.btnSearchBooks.UseVisualStyleBackColor = true;
            this.btnSearchBooks.Click += new System.EventHandler(this.btnSearchBooks_Click);
            // 
            // txtSearchBooks
            // 
            this.txtSearchBooks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchBooks.Location = new System.Drawing.Point(0, 0);
            this.txtSearchBooks.Name = "txtSearchBooks";
            this.txtSearchBooks.Size = new System.Drawing.Size(711, 22);
            this.txtSearchBooks.TabIndex = 1;
            // 
            // btnRestock
            // 
            this.btnRestock.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRestock.Location = new System.Drawing.Point(711, 0);
            this.btnRestock.Name = "btnRestock";
            this.btnRestock.Size = new System.Drawing.Size(75, 40);
            this.btnRestock.TabIndex = 0;
            this.btnRestock.Text = "Készlet +10";
            this.btnRestock.UseVisualStyleBackColor = true;
            this.btnRestock.Click += new System.EventHandler(this.btnRestock_Click);
            // 
            // tabOrders
            // 
            this.tabOrders.Controls.Add(this.splitOrders);
            this.tabOrders.Location = new System.Drawing.Point(4, 25);
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.Padding = new System.Windows.Forms.Padding(3);
            this.tabOrders.Size = new System.Drawing.Size(792, 345);
            this.tabOrders.TabIndex = 1;
            this.tabOrders.Text = "Orders";
            this.tabOrders.UseVisualStyleBackColor = true;
            // 
            // splitOrders
            // 
            this.splitOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitOrders.Location = new System.Drawing.Point(3, 3);
            this.splitOrders.Name = "splitOrders";
            // 
            // splitOrders.Panel1
            // 
            this.splitOrders.Panel1.Controls.Add(this.gridOrders);
            // 
            // splitOrders.Panel2
            // 
            this.splitOrders.Panel2.Controls.Add(this.gridOrderItems);
            this.splitOrders.Panel2.Controls.Add(this.lblOrderTotal);
            this.splitOrders.Size = new System.Drawing.Size(786, 339);
            this.splitOrders.SplitterDistance = 262;
            this.splitOrders.TabIndex = 0;
            // 
            // gridOrders
            // 
            this.gridOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOrders.Location = new System.Drawing.Point(0, 0);
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.RowHeadersWidth = 51;
            this.gridOrders.RowTemplate.Height = 24;
            this.gridOrders.Size = new System.Drawing.Size(262, 339);
            this.gridOrders.TabIndex = 0;
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            // 
            // gridOrderItems
            // 
            this.gridOrderItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridOrderItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOrderItems.Location = new System.Drawing.Point(0, 0);
            this.gridOrderItems.Name = "gridOrderItems";
            this.gridOrderItems.RowHeadersWidth = 51;
            this.gridOrderItems.RowTemplate.Height = 24;
            this.gridOrderItems.Size = new System.Drawing.Size(520, 314);
            this.gridOrderItems.TabIndex = 1;
            // 
            // lblOrderTotal
            // 
            this.lblOrderTotal.AutoSize = true;
            this.lblOrderTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblOrderTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrderTotal.Location = new System.Drawing.Point(0, 314);
            this.lblOrderTotal.Name = "lblOrderTotal";
            this.lblOrderTotal.Size = new System.Drawing.Size(194, 25);
            this.lblOrderTotal.TabIndex = 0;
            this.lblOrderTotal.Text = "Végösszeg: 0 EUR";
            // 
            // tabReports
            // 
            this.tabReports.Controls.Add(this.txtReportPreview);
            this.tabReports.Controls.Add(this.btnExportReport);
            this.tabReports.Location = new System.Drawing.Point(4, 25);
            this.tabReports.Name = "tabReports";
            this.tabReports.Size = new System.Drawing.Size(792, 345);
            this.tabReports.TabIndex = 2;
            this.tabReports.Text = "Reports";
            this.tabReports.UseVisualStyleBackColor = true;
            // 
            // txtReportPreview
            // 
            this.txtReportPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReportPreview.Location = new System.Drawing.Point(0, 40);
            this.txtReportPreview.Multiline = true;
            this.txtReportPreview.Name = "txtReportPreview";
            this.txtReportPreview.ReadOnly = true;
            this.txtReportPreview.Size = new System.Drawing.Size(792, 305);
            this.txtReportPreview.TabIndex = 1;
            // 
            // btnExportReport
            // 
            this.btnExportReport.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnExportReport.Location = new System.Drawing.Point(0, 0);
            this.btnExportReport.Name = "btnExportReport";
            this.btnExportReport.Size = new System.Drawing.Size(792, 40);
            this.btnExportReport.TabIndex = 0;
            this.btnExportReport.Text = "Riport Exportálása (.txt)";
            this.btnExportReport.UseVisualStyleBackColor = true;
            this.btnExportReport.Click += new System.EventHandler(this.btnExportReport_Click);
            // 
            // panelTopControls
            // 
            this.panelTopControls.Controls.Add(this.btnImportData);
            this.panelTopControls.Controls.Add(this.btnResetDatabase);
            this.panelTopControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopControls.Location = new System.Drawing.Point(0, 0);
            this.panelTopControls.Name = "panelTopControls";
            this.panelTopControls.Size = new System.Drawing.Size(800, 50);
            this.panelTopControls.TabIndex = 1;
            // 
            // btnImportData
            // 
            this.btnImportData.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnImportData.Location = new System.Drawing.Point(0, 0);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(198, 50);
            this.btnImportData.TabIndex = 1;
            this.btnImportData.Text = "Adatok Importálása (JSON)";
            this.btnImportData.UseVisualStyleBackColor = true;
            this.btnImportData.Click += new System.EventHandler(this.btnImportData_Click);
            // 
            // btnResetDatabase
            // 
            this.btnResetDatabase.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnResetDatabase.Location = new System.Drawing.Point(662, 0);
            this.btnResetDatabase.Name = "btnResetDatabase";
            this.btnResetDatabase.Size = new System.Drawing.Size(138, 50);
            this.btnResetDatabase.TabIndex = 0;
            this.btnResetDatabase.Text = "Adatbázis Reset";
            this.btnResetDatabase.UseVisualStyleBackColor = true;
            this.btnResetDatabase.Click += new System.EventHandler(this.btnResetDatabase_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(78, 20);
            this.lblStatus.Text = "Készen áll.";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panelTopControls);
            this.Name = "MainWindow";
            this.Text = "Bookstore Manager";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabBooks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridBooks)).EndInit();
            this.panelBookControls.ResumeLayout(false);
            this.panelBookControls.PerformLayout();
            this.tabOrders.ResumeLayout(false);
            this.splitOrders.Panel1.ResumeLayout(false);
            this.splitOrders.Panel2.ResumeLayout(false);
            this.splitOrders.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitOrders)).EndInit();
            this.splitOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrderItems)).EndInit();
            this.tabReports.ResumeLayout(false);
            this.tabReports.PerformLayout();
            this.panelTopControls.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabBooks;
        private System.Windows.Forms.TabPage tabOrders;
        private System.Windows.Forms.TabPage tabReports;
        private System.Windows.Forms.Panel panelTopControls;
        private System.Windows.Forms.Button btnImportData;
        private System.Windows.Forms.Button btnResetDatabase;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.DataGridView gridBooks;
        private System.Windows.Forms.Panel panelBookControls;
        private System.Windows.Forms.TextBox txtSearchBooks;
        private System.Windows.Forms.Button btnRestock;
        private System.Windows.Forms.Button btnSearchBooks;
        private System.Windows.Forms.SplitContainer splitOrders;
        private System.Windows.Forms.DataGridView gridOrders;
        private System.Windows.Forms.DataGridView gridOrderItems;
        private System.Windows.Forms.Label lblOrderTotal;
        private System.Windows.Forms.TextBox txtReportPreview;
        private System.Windows.Forms.Button btnExportReport;
    }
}