using LogicLayer;

namespace ShaCollisionsGUIMain
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
            this.pass = new System.Windows.Forms.TextBox();
            this.hash = new System.Windows.Forms.TextBox();
            this.collision = new System.Windows.Forms.TextBox();
            this.length = new System.Windows.Forms.TextBox();
            this.count = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.calculate = new System.Windows.Forms.Button();
            this.searchCollisions = new System.Windows.Forms.Button();
            this.abort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(108, 88);
            this.pass.Name = "pass";
            this.pass.Size = new System.Drawing.Size(590, 20);
            this.pass.TabIndex = 0;
            // 
            // hash
            // 
            this.hash.Location = new System.Drawing.Point(108, 177);
            this.hash.Name = "hash";
            this.hash.Size = new System.Drawing.Size(590, 20);
            this.hash.TabIndex = 1;
            // 
            // collision
            // 
            this.collision.Location = new System.Drawing.Point(108, 291);
            this.collision.Name = "collision";
            this.collision.Size = new System.Drawing.Size(136, 20);
            this.collision.TabIndex = 2;
            // 
            // length
            // 
            this.length.Location = new System.Drawing.Point(502, 288);
            this.length.Name = "length";
            this.length.Size = new System.Drawing.Size(186, 20);
            this.length.TabIndex = 3;
            // 
            // count
            // 
            this.count.Location = new System.Drawing.Point(502, 356);
            this.count.Name = "count";
            this.count.Size = new System.Drawing.Size(186, 20);
            this.count.TabIndex = 4;
            // 
            // Password
            // 
            this.Password.AutoSize = true;
            this.Password.Location = new System.Drawing.Point(49, 88);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(56, 13);
            this.Password.TabIndex = 5;
            this.Password.Text = "Password:";
            this.Password.Click += new System.EventHandler(this.label1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Hash:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 291);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Collision:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(453, 291);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Length:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(453, 359);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Count:";
            // 
            // calculate
            // 
            this.calculate.Location = new System.Drawing.Point(889, 86);
            this.calculate.Name = "calculate";
            this.calculate.Size = new System.Drawing.Size(145, 23);
            this.calculate.TabIndex = 10;
            this.calculate.Text = "Calculate";
            this.calculate.UseVisualStyleBackColor = true;
            this.calculate.Click += new System.EventHandler(this.calculate_Click);
            // 
            // searchCollisions
            // 
            this.searchCollisions.Enabled = false;
            this.searchCollisions.Location = new System.Drawing.Point(889, 134);
            this.searchCollisions.Name = "searchCollisions";
            this.searchCollisions.Size = new System.Drawing.Size(145, 22);
            this.searchCollisions.TabIndex = 11;
            this.searchCollisions.Text = "Search Collisions";
            this.searchCollisions.UseVisualStyleBackColor = true;
            this.searchCollisions.Click += new System.EventHandler(this.searchCollisions_Click);
            // 
            // abort
            // 
            this.abort.Enabled = false;
            this.abort.Location = new System.Drawing.Point(889, 189);
            this.abort.Name = "abort";
            this.abort.Size = new System.Drawing.Size(144, 24);
            this.abort.TabIndex = 12;
            this.abort.Text = "Abort";
            this.abort.UseVisualStyleBackColor = true;
            this.abort.Click += new System.EventHandler(this.abort_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 439);
            this.Controls.Add(this.abort);
            this.Controls.Add(this.searchCollisions);
            this.Controls.Add(this.calculate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.count);
            this.Controls.Add(this.length);
            this.Controls.Add(this.collision);
            this.Controls.Add(this.hash);
            this.Controls.Add(this.pass);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pass;
        private System.Windows.Forms.TextBox hash;
        private System.Windows.Forms.TextBox collision;
        private System.Windows.Forms.TextBox length;
        private System.Windows.Forms.TextBox count;
        private System.Windows.Forms.Label Password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button calculate;
        private System.Windows.Forms.Button searchCollisions;
        private System.Windows.Forms.Button abort;
       

      
    }
}

