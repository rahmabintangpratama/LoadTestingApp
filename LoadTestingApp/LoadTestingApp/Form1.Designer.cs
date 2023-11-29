namespace LoadTestingApp
{
    partial class LoadTestingApp
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
            this.inputUrl = new System.Windows.Forms.TextBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.load_testing_app = new System.Windows.Forms.Label();
            this.lblOutput = new System.Windows.Forms.Label();
            this.lblInputNumberOfRequest = new System.Windows.Forms.Label();
            this.lblInputTimeOut = new System.Windows.Forms.Label();
            this.timeoutNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // start_Button
            // 
            this.start_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_Button.Location = new System.Drawing.Point(351, 164);
            this.start_Button.Name = "start_Button";
            this.start_Button.Size = new System.Drawing.Size(111, 44);
            this.start_Button.TabIndex = 0;
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
            // inputUrl
            // 
            this.inputUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputUrl.Location = new System.Drawing.Point(12, 110);
            this.inputUrl.Multiline = true;
            this.inputUrl.Name = "inputUrl";
            this.inputUrl.Size = new System.Drawing.Size(245, 38);
            this.inputUrl.TabIndex = 3;
            this.inputUrl.TextChanged += new System.EventHandler(this.inputUrl_TextChanged);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(12, 264);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(776, 165);
            this.outputTextBox.TabIndex = 6;
            this.outputTextBox.TextChanged += new System.EventHandler(this.output);
            // 
            // numericUpDown
            // 
            this.numericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown.Location = new System.Drawing.Point(301, 110);
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(221, 38);
            this.numericUpDown.TabIndex = 7;
            this.numericUpDown.ValueChanged += new System.EventHandler(this.numericUpAndDown);
            this.numericUpDown.Click += new System.EventHandler(this.numericUpAndDown);
            // 
            // load_testing_app
            // 
            this.load_testing_app.AutoSize = true;
            this.load_testing_app.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load_testing_app.Location = new System.Drawing.Point(290, 26);
            this.load_testing_app.Name = "load_testing_app";
            this.load_testing_app.Size = new System.Drawing.Size(251, 36);
            this.load_testing_app.TabIndex = 8;
            this.load_testing_app.Text = "Load Testing App";
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
            this.lblInputNumberOfRequest.Location = new System.Drawing.Point(297, 77);
            this.lblInputNumberOfRequest.Name = "lblInputNumberOfRequest";
            this.lblInputNumberOfRequest.Size = new System.Drawing.Size(196, 20);
            this.lblInputNumberOfRequest.TabIndex = 10;
            this.lblInputNumberOfRequest.Text = "Input Number Of Request:";
            // 
            // lblInputTimeOut
            // 
            this.lblInputTimeOut.AutoSize = true;
            this.lblInputTimeOut.Location = new System.Drawing.Point(563, 77);
            this.lblInputTimeOut.Name = "lblInputTimeOut";
            this.lblInputTimeOut.Size = new System.Drawing.Size(114, 20);
            this.lblInputTimeOut.TabIndex = 12;
            this.lblInputTimeOut.Text = "Input TimeOut:";
            this.lblInputTimeOut.Click += new System.EventHandler(this.label1_Click);
            // 
            // timeoutNumericUpDown
            // 
            this.timeoutNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeoutNumericUpDown.Location = new System.Drawing.Point(567, 110);
            this.timeoutNumericUpDown.Name = "timeoutNumericUpDown";
            this.timeoutNumericUpDown.Size = new System.Drawing.Size(221, 38);
            this.timeoutNumericUpDown.TabIndex = 11;
            this.timeoutNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // LoadTestingApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblInputTimeOut);
            this.Controls.Add(this.timeoutNumericUpDown);
            this.Controls.Add(this.lblInputNumberOfRequest);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.load_testing_app);
            this.Controls.Add(this.numericUpDown);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.inputUrl);
            this.Controls.Add(this.lblInputUrl);
            this.Controls.Add(this.start_Button);
            this.MaximizeBox = false;
            this.Name = "LoadTestingApp";
            this.Text = "Load Testing App";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button start_Button;
        private System.Windows.Forms.Label lblInputUrl;
        private System.Windows.Forms.TextBox inputUrl;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label load_testing_app;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblInputNumberOfRequest;
        private System.Windows.Forms.Label lblInputTimeOut;
        private System.Windows.Forms.NumericUpDown timeoutNumericUpDown;
    }
}

