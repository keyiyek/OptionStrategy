namespace OptionStrategy
{
    partial class frmMain
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.dtpExirationDate = new System.Windows.Forms.DateTimePicker();
            this.lbExpirationDate = new System.Windows.Forms.Label();
            this.cbxRight = new System.Windows.Forms.ComboBox();
            this.lbRight = new System.Windows.Forms.Label();
            this.btConnect = new System.Windows.Forms.Button();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tbpTickers = new System.Windows.Forms.TabPage();
            this.lsvResults = new System.Windows.Forms.ListView();
            this.ROC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ticker = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btSaveTickers = new System.Windows.Forms.Button();
            this.btRemoveTickers = new System.Windows.Forms.Button();
            this.lbAddTicker = new System.Windows.Forms.Label();
            this.btAddTicker = new System.Windows.Forms.Button();
            this.btLoadTickers = new System.Windows.Forms.Button();
            this.txbAddTickers = new System.Windows.Forms.TextBox();
            this.lbLoadTickers = new System.Windows.Forms.Label();
            this.lsbTickers = new System.Windows.Forms.ListBox();
            this.ofdLoadTickers = new System.Windows.Forms.OpenFileDialog();
            this.sfdSaveTickers = new System.Windows.Forms.SaveFileDialog();
            this.cbxMarketStatus = new System.Windows.Forms.ComboBox();
            this.lbMarketStatus = new System.Windows.Forms.Label();
            this.chbxParallelComputing = new System.Windows.Forms.CheckBox();
            this.tbcMain.SuspendLayout();
            this.tbpTickers.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpExirationDate
            // 
            this.dtpExirationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExirationDate.Location = new System.Drawing.Point(12, 24);
            this.dtpExirationDate.Name = "dtpExirationDate";
            this.dtpExirationDate.Size = new System.Drawing.Size(200, 20);
            this.dtpExirationDate.TabIndex = 0;
            this.dtpExirationDate.Enter += new System.EventHandler(this.dtpExirationDate_Enter);
            // 
            // lbExpirationDate
            // 
            this.lbExpirationDate.AutoSize = true;
            this.lbExpirationDate.Location = new System.Drawing.Point(12, 8);
            this.lbExpirationDate.Name = "lbExpirationDate";
            this.lbExpirationDate.Size = new System.Drawing.Size(79, 13);
            this.lbExpirationDate.TabIndex = 1;
            this.lbExpirationDate.Text = "Expiration Date";
            // 
            // cbxRight
            // 
            this.cbxRight.FormattingEnabled = true;
            this.cbxRight.Items.AddRange(new object[] {
            "CALL",
            "PUT",
            "Both"});
            this.cbxRight.Location = new System.Drawing.Point(218, 23);
            this.cbxRight.Name = "cbxRight";
            this.cbxRight.Size = new System.Drawing.Size(121, 21);
            this.cbxRight.TabIndex = 2;
            this.cbxRight.Enter += new System.EventHandler(this.cbxRight_Enter);
            // 
            // lbRight
            // 
            this.lbRight.AutoSize = true;
            this.lbRight.Location = new System.Drawing.Point(215, 9);
            this.lbRight.Name = "lbRight";
            this.lbRight.Size = new System.Drawing.Size(32, 13);
            this.lbRight.TabIndex = 3;
            this.lbRight.Text = "Right";
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(1505, 24);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(75, 23);
            this.btConnect.TabIndex = 4;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpTickers);
            this.tbcMain.Location = new System.Drawing.Point(12, 51);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1568, 824);
            this.tbcMain.TabIndex = 5;
            // 
            // tbpTickers
            // 
            this.tbpTickers.Controls.Add(this.lsvResults);
            this.tbpTickers.Controls.Add(this.btSaveTickers);
            this.tbpTickers.Controls.Add(this.btRemoveTickers);
            this.tbpTickers.Controls.Add(this.lbAddTicker);
            this.tbpTickers.Controls.Add(this.btAddTicker);
            this.tbpTickers.Controls.Add(this.btLoadTickers);
            this.tbpTickers.Controls.Add(this.txbAddTickers);
            this.tbpTickers.Controls.Add(this.lbLoadTickers);
            this.tbpTickers.Controls.Add(this.lsbTickers);
            this.tbpTickers.Location = new System.Drawing.Point(4, 22);
            this.tbpTickers.Name = "tbpTickers";
            this.tbpTickers.Padding = new System.Windows.Forms.Padding(3);
            this.tbpTickers.Size = new System.Drawing.Size(1560, 798);
            this.tbpTickers.TabIndex = 0;
            this.tbpTickers.Text = "Tickers";
            this.tbpTickers.UseVisualStyleBackColor = true;
            // 
            // lsvResults
            // 
            this.lsvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ROC,
            this.Ticker});
            this.lsvResults.Location = new System.Drawing.Point(320, 7);
            this.lsvResults.Name = "lsvResults";
            this.lsvResults.Size = new System.Drawing.Size(1234, 783);
            this.lsvResults.TabIndex = 8;
            this.lsvResults.UseCompatibleStateImageBehavior = false;
            this.lsvResults.View = System.Windows.Forms.View.Details;
            // 
            // ROC
            // 
            this.ROC.Width = 94;
            // 
            // Ticker
            // 
            this.Ticker.Width = 91;
            // 
            // btSaveTickers
            // 
            this.btSaveTickers.Location = new System.Drawing.Point(215, 767);
            this.btSaveTickers.Name = "btSaveTickers";
            this.btSaveTickers.Size = new System.Drawing.Size(75, 23);
            this.btSaveTickers.TabIndex = 7;
            this.btSaveTickers.Text = "Save";
            this.btSaveTickers.UseVisualStyleBackColor = true;
            this.btSaveTickers.Click += new System.EventHandler(this.btSaveTickers_Click);
            // 
            // btRemoveTickers
            // 
            this.btRemoveTickers.Location = new System.Drawing.Point(134, 767);
            this.btRemoveTickers.Name = "btRemoveTickers";
            this.btRemoveTickers.Size = new System.Drawing.Size(75, 23);
            this.btRemoveTickers.TabIndex = 6;
            this.btRemoveTickers.Text = "Remove";
            this.btRemoveTickers.UseVisualStyleBackColor = true;
            this.btRemoveTickers.Click += new System.EventHandler(this.btRemoveTickers_Click);
            // 
            // lbAddTicker
            // 
            this.lbAddTicker.AutoSize = true;
            this.lbAddTicker.Location = new System.Drawing.Point(134, 53);
            this.lbAddTicker.Name = "lbAddTicker";
            this.lbAddTicker.Size = new System.Drawing.Size(59, 13);
            this.lbAddTicker.TabIndex = 5;
            this.lbAddTicker.Text = "Add Ticker";
            // 
            // btAddTicker
            // 
            this.btAddTicker.Location = new System.Drawing.Point(239, 67);
            this.btAddTicker.Name = "btAddTicker";
            this.btAddTicker.Size = new System.Drawing.Size(75, 23);
            this.btAddTicker.TabIndex = 4;
            this.btAddTicker.Text = "Add";
            this.btAddTicker.UseVisualStyleBackColor = true;
            this.btAddTicker.Click += new System.EventHandler(this.btAddTicker_Click);
            // 
            // btLoadTickers
            // 
            this.btLoadTickers.Location = new System.Drawing.Point(133, 23);
            this.btLoadTickers.Name = "btLoadTickers";
            this.btLoadTickers.Size = new System.Drawing.Size(75, 23);
            this.btLoadTickers.TabIndex = 3;
            this.btLoadTickers.Text = "...";
            this.btLoadTickers.UseVisualStyleBackColor = true;
            this.btLoadTickers.Click += new System.EventHandler(this.btLoadTickers_Click);
            // 
            // txbAddTickers
            // 
            this.txbAddTickers.Location = new System.Drawing.Point(133, 69);
            this.txbAddTickers.Name = "txbAddTickers";
            this.txbAddTickers.Size = new System.Drawing.Size(100, 20);
            this.txbAddTickers.TabIndex = 2;
            this.txbAddTickers.Text = "Ticker Symbol";
            this.txbAddTickers.Enter += new System.EventHandler(this.txbAddTickers_Enter);
            // 
            // lbLoadTickers
            // 
            this.lbLoadTickers.AutoSize = true;
            this.lbLoadTickers.Location = new System.Drawing.Point(134, 7);
            this.lbLoadTickers.Name = "lbLoadTickers";
            this.lbLoadTickers.Size = new System.Drawing.Size(66, 13);
            this.lbLoadTickers.TabIndex = 1;
            this.lbLoadTickers.Text = "LoadTickers";
            // 
            // lsbTickers
            // 
            this.lsbTickers.FormattingEnabled = true;
            this.lsbTickers.Location = new System.Drawing.Point(7, 7);
            this.lsbTickers.Name = "lsbTickers";
            this.lsbTickers.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lsbTickers.Size = new System.Drawing.Size(120, 784);
            this.lsbTickers.TabIndex = 0;
            // 
            // ofdLoadTickers
            // 
            this.ofdLoadTickers.FileName = "openFileDialog1";
            this.ofdLoadTickers.Filter = "Text Files|*txt";
            // 
            // sfdSaveTickers
            // 
            this.sfdSaveTickers.DefaultExt = "txt";
            this.sfdSaveTickers.FileOk += new System.ComponentModel.CancelEventHandler(this.sfdSaveTickers_FileOk);
            // 
            // cbxMarketStatus
            // 
            this.cbxMarketStatus.FormattingEnabled = true;
            this.cbxMarketStatus.Items.AddRange(new object[] {
            "OPEN",
            "CLOSED"});
            this.cbxMarketStatus.Location = new System.Drawing.Point(346, 23);
            this.cbxMarketStatus.Name = "cbxMarketStatus";
            this.cbxMarketStatus.Size = new System.Drawing.Size(121, 21);
            this.cbxMarketStatus.TabIndex = 7;
            // 
            // lbMarketStatus
            // 
            this.lbMarketStatus.AutoSize = true;
            this.lbMarketStatus.Location = new System.Drawing.Point(346, 8);
            this.lbMarketStatus.Name = "lbMarketStatus";
            this.lbMarketStatus.Size = new System.Drawing.Size(73, 13);
            this.lbMarketStatus.TabIndex = 8;
            this.lbMarketStatus.Text = "Market Status";
            // 
            // chbxParallelComputing
            // 
            this.chbxParallelComputing.AutoSize = true;
            this.chbxParallelComputing.Location = new System.Drawing.Point(473, 25);
            this.chbxParallelComputing.Name = "chbxParallelComputing";
            this.chbxParallelComputing.Size = new System.Drawing.Size(92, 17);
            this.chbxParallelComputing.TabIndex = 9;
            this.chbxParallelComputing.Text = "DataHandler2";
            this.chbxParallelComputing.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AcceptButton = this.btConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1592, 887);
            this.Controls.Add(this.chbxParallelComputing);
            this.Controls.Add(this.lbMarketStatus);
            this.Controls.Add(this.cbxMarketStatus);
            this.Controls.Add(this.tbcMain);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.lbRight);
            this.Controls.Add(this.cbxRight);
            this.Controls.Add(this.lbExpirationDate);
            this.Controls.Add(this.dtpExirationDate);
            this.Name = "frmMain";
            this.Text = "Option Strategy Calculator";
            this.tbcMain.ResumeLayout(false);
            this.tbpTickers.ResumeLayout(false);
            this.tbpTickers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpExirationDate;
        private System.Windows.Forms.Label lbExpirationDate;
        private System.Windows.Forms.ComboBox cbxRight;
        private System.Windows.Forms.Label lbRight;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpTickers;
        private System.Windows.Forms.ListView lsvResults;
        private System.Windows.Forms.ColumnHeader ROC;
        private System.Windows.Forms.ColumnHeader Ticker;
        private System.Windows.Forms.Button btSaveTickers;
        private System.Windows.Forms.Button btRemoveTickers;
        private System.Windows.Forms.Label lbAddTicker;
        private System.Windows.Forms.Button btAddTicker;
        private System.Windows.Forms.Button btLoadTickers;
        private System.Windows.Forms.TextBox txbAddTickers;
        private System.Windows.Forms.Label lbLoadTickers;
        private System.Windows.Forms.ListBox lsbTickers;
        private System.Windows.Forms.OpenFileDialog ofdLoadTickers;
        private System.Windows.Forms.SaveFileDialog sfdSaveTickers;
        private System.Windows.Forms.ComboBox cbxMarketStatus;
        private System.Windows.Forms.Label lbMarketStatus;
        private System.Windows.Forms.CheckBox chbxParallelComputing;
    }
}

