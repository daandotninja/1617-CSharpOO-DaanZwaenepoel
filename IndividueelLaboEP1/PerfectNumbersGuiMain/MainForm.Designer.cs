namespace PerfectNumbersGuiMain
{
    partial class MainForm
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
            this.abort_btn = new System.Windows.Forms.Button();
            this.start_btn = new System.Windows.Forms.Button();
            this.numbers = new System.Windows.Forms.TextBox();
            this.number = new System.Windows.Forms.Label();
            this.checking = new System.Windows.Forms.TextBox();
            this.control = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // abort_btn
            // 
            this.abort_btn.Enabled = false;
            this.abort_btn.Location = new System.Drawing.Point(81, 402);
            this.abort_btn.Name = "abort_btn";
            this.abort_btn.Size = new System.Drawing.Size(75, 23);
            this.abort_btn.TabIndex = 0;
            this.abort_btn.Text = "Abort";
            this.abort_btn.UseVisualStyleBackColor = true;
            // 
            // start_btn
            // 
            this.start_btn.Location = new System.Drawing.Point(233, 402);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(75, 23);
            this.start_btn.TabIndex = 1;
            this.start_btn.Text = "Start Coalculations";
            this.start_btn.UseVisualStyleBackColor = true;
            // 
            // numbers
            // 
            this.numbers.Location = new System.Drawing.Point(81, 66);
            this.numbers.Multiline = true;
            this.numbers.Name = "numbers";
            this.numbers.Size = new System.Drawing.Size(310, 229);
            this.numbers.TabIndex = 2;
            // 
            // number
            // 
            this.number.AutoSize = true;
            this.number.Location = new System.Drawing.Point(78, 40);
            this.number.Name = "number";
            this.number.Size = new System.Drawing.Size(52, 13);
            this.number.TabIndex = 3;
            this.number.Text = "Numbers:";
            // 
            // checking
            // 
            this.checking.Location = new System.Drawing.Point(170, 331);
            this.checking.Name = "checking";
            this.checking.Size = new System.Drawing.Size(173, 20);
            this.checking.TabIndex = 4;
            // 
            // control
            // 
            this.control.Location = new System.Drawing.Point(170, 357);
            this.control.Name = "control";
            this.control.Size = new System.Drawing.Size(112, 20);
            this.control.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 338);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Checking:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(75, 360);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Control number:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 470);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.control);
            this.Controls.Add(this.checking);
            this.Controls.Add(this.number);
            this.Controls.Add(this.numbers);
            this.Controls.Add(this.start_btn);
            this.Controls.Add(this.abort_btn);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button abort_btn;
        private System.Windows.Forms.Button start_btn;
        private System.Windows.Forms.TextBox numbers;
        private System.Windows.Forms.Label number;
        private System.Windows.Forms.TextBox checking;
        private System.Windows.Forms.TextBox control;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

