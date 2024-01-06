namespace LoadTesting
{
    partial class LoadTesting
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
            this.start_Button = new System.Windows.Forms.Button();
            this.lblInputUrl = new System.Windows.Forms.Label();
            this.textBoxInputUrl = new System.Windows.Forms.TextBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.load_testing_app = new System.Windows.Forms.Label();
            this.lblOutput = new System.Windows.Forms.Label();
            this.lblInputNumberOfRequest = new System.Windows.Forms.Label();
            this.lblInputTimeOut = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.textBoxInputRequest = new System.Windows.Forms.TextBox();
            this.textBoxInputTimeout = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // start_Button
            // 
            this.start_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_Button.Location = new System.Drawing.Point(351, 164);
            this.start_Button.Name = "start_Button";
            this.start_Button.Size = new System.Drawing.Size(111, 44);
            this.start_Button.TabIndex = 4;
            this.start_Button.Text = "Start";
            this.start_Button.UseVisualStyleBackColor = true;
            this.start_Button.Click += new System.EventHandler(this.startButton_Click);
            // 
            // lblInputUrl
            // 
            this.lblInputUrl.AutoSize = true;
            this.lblInputUrl.Location = new System.Drawing.Point(12, 77);
            this.lblInputUrl.Name = "lblInputUrl";
            this.lblInputUrl.Size = new System.Drawing.Size(87, 20);
            this.lblInputUrl.TabIndex = 1;
            this.lblInputUrl.Text = "Input URL:";
            // 
            // textBoxInputUrl
            // 
            this.textBoxInputUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInputUrl.Location = new System.Drawing.Point(12, 110);
            this.textBoxInputUrl.Name = "textBoxInputUrl";
            this.textBoxInputUrl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxInputUrl.Size = new System.Drawing.Size(245, 30);
            this.textBoxInputUrl.TabIndex = 1;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(12, 264);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(776, 165);
            this.outputTextBox.TabIndex = 5;
            // 
            // load_testing_app
            // 
            this.load_testing_app.AutoSize = true;
            this.load_testing_app.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load_testing_app.Location = new System.Drawing.Point(314, 26);
            this.load_testing_app.Name = "load_testing_app";
            this.load_testing_app.Size = new System.Drawing.Size(188, 36);
            this.load_testing_app.TabIndex = 8;
            this.load_testing_app.Text = "Load Testing";
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(12, 229);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(62, 20);
            this.lblOutput.TabIndex = 9;
            this.lblOutput.Text = "Output:";
            // 
            // lblInputNumberOfRequest
            // 
            this.lblInputNumberOfRequest.AutoSize = true;
            this.lblInputNumberOfRequest.Location = new System.Drawing.Point(273, 77);
            this.lblInputNumberOfRequest.Name = "lblInputNumberOfRequest";
            this.lblInputNumberOfRequest.Size = new System.Drawing.Size(193, 20);
            this.lblInputNumberOfRequest.TabIndex = 10;
            this.lblInputNumberOfRequest.Text = "Input Number of Request:";
            // 
            // lblInputTimeOut
            // 
            this.lblInputTimeOut.AutoSize = true;
            this.lblInputTimeOut.Location = new System.Drawing.Point(539, 77);
            this.lblInputTimeOut.Name = "lblInputTimeOut";
            this.lblInputTimeOut.Size = new System.Drawing.Size(201, 20);
            this.lblInputTimeOut.TabIndex = 12;
            this.lblInputTimeOut.Text = "Input Timeout (in seconds):";
            // 
            // btnHelp
            // 
            this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.Location = new System.Drawing.Point(12, 466);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(111, 44);
            this.btnHelp.TabIndex = 6;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(677, 466);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(111, 44);
            this.btnExport.TabIndex = 9;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInfo.Location = new System.Drawing.Point(231, 466);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(111, 44);
            this.btnInfo.TabIndex = 7;
            this.btnInfo.Text = "Info";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(455, 466);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(111, 44);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // textBoxInputRequest
            // 
            this.textBoxInputRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInputRequest.Location = new System.Drawing.Point(277, 110);
            this.textBoxInputRequest.Name = "textBoxInputRequest";
            this.textBoxInputRequest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxInputRequest.Size = new System.Drawing.Size(245, 30);
            this.textBoxInputRequest.TabIndex = 2;
            this.textBoxInputRequest.TextChanged += new System.EventHandler(this.textBoxInputRequest_TextChanged);
            this.textBoxInputRequest.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxOnlyNumber_KeyPress);
            // 
            // textBoxInputTimeout
            // 
            this.textBoxInputTimeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInputTimeout.Location = new System.Drawing.Point(543, 110);
            this.textBoxInputTimeout.Name = "textBoxInputTimeout";
            this.textBoxInputTimeout.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxInputTimeout.Size = new System.Drawing.Size(245, 30);
            this.textBoxInputTimeout.TabIndex = 3;
            this.textBoxInputTimeout.TextChanged += new System.EventHandler(this.textBoxInputTimeout_TextChanged);
            this.textBoxInputTimeout.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxOnlyNumber_KeyPress);
            // 
            // LoadTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 541);
            this.Controls.Add(this.textBoxInputTimeout);
            this.Controls.Add(this.textBoxInputRequest);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lblInputTimeOut);
            this.Controls.Add(this.lblInputNumberOfRequest);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.load_testing_app);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.textBoxInputUrl);
            this.Controls.Add(this.lblInputUrl);
            this.Controls.Add(this.start_Button);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(822, 597);
            this.MinimumSize = new System.Drawing.Size(822, 597);
            this.Name = "LoadTesting";
            this.Text = "Load Testing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button start_Button;
        private System.Windows.Forms.Label lblInputUrl;
        private System.Windows.Forms.TextBox textBoxInputUrl;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label load_testing_app;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblInputNumberOfRequest;
        private System.Windows.Forms.Label lblInputTimeOut;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox textBoxInputRequest;
        private System.Windows.Forms.TextBox textBoxInputTimeout;
    }
}

