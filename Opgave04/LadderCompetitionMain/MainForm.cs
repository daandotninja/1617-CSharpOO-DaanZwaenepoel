using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Globals;

namespace LadderCompetitionMain
{
    public partial class MainForm : Form
    {
        private readonly ILogic logic;
        private TextBox rankingTextBox;
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;
        private ComboBox winnerComboBox;
        private ComboBox loserComboBox;
        private Label label1;
        private Label label2;
        private Button registerButton;
        private GroupBox newMatchGroupBox;
        private Label errorLabel;
        private bool inputIsValid;

        public MainForm(ILogic logic)
        {
            this.logic = logic;
            InitializeComponent();
            refreshRanking();
        }

        private void refreshRanking()
        {
            rankingTextBox.Lines = logic.Ranking.Select(p => p.ToString()).ToArray();
            rankingTextBox.Select(0, 0);
            winnerComboBox.DataSource = logic.Ranking;
            loserComboBox.DataSource = logic.Ranking;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logic.Close();
        }

      
        private void ComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // do nothing
            e.Handled = true;
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (inputIsValid)
            {
                logic.RegisterMatch((Player)winnerComboBox.SelectedItem, (Player)loserComboBox.SelectedItem);
                refreshRanking();
            }
            else
            {
                errorLabel.Text = "Invalid Player combination!";
            }

        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((winnerComboBox.SelectedIndex != -1) && (loserComboBox.SelectedIndex != -1))
            {
                var winner = (Player)winnerComboBox.SelectedItem;
                var loser = (Player)loserComboBox.SelectedItem;
                inputIsValid = logic.IsValidMatch(winner, loser);
                if (inputIsValid)
                {
                    newMatchGroupBox.BackColor = Color.LightGreen;
                    errorLabel.Text = "";
                }
                else
                {
                    newMatchGroupBox.BackColor = Color.PapayaWhip;
                }
                registerButton.Select();
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rankingTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.winnerComboBox = new System.Windows.Forms.ComboBox();
            this.loserComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.registerButton = new System.Windows.Forms.Button();
            this.newMatchGroupBox = new System.Windows.Forms.GroupBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.newMatchGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // rankingTextBox
            // 
            this.rankingTextBox.Location = new System.Drawing.Point(33, 14);
            this.rankingTextBox.Multiline = true;
            this.rankingTextBox.Name = "rankingTextBox";
            this.rankingTextBox.Size = new System.Drawing.Size(389, 655);
            this.rankingTextBox.TabIndex = 0;
            this.rankingTextBox.TextChanged += new System.EventHandler(this.rankingTextBox_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // winnerComboBox
            // 
            this.winnerComboBox.FormattingEnabled = true;
            this.winnerComboBox.Location = new System.Drawing.Point(92, 69);
            this.winnerComboBox.Name = "winnerComboBox";
            this.winnerComboBox.Size = new System.Drawing.Size(269, 21);
            this.winnerComboBox.TabIndex = 2;
            // 
            // loserComboBox
            // 
            this.loserComboBox.FormattingEnabled = true;
            this.loserComboBox.Location = new System.Drawing.Point(93, 130);
            this.loserComboBox.Name = "loserComboBox";
            this.loserComboBox.Size = new System.Drawing.Size(268, 21);
            this.loserComboBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Winner";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Loser";
            // 
            // registerButton
            // 
            this.registerButton.Location = new System.Drawing.Point(109, 207);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(165, 23);
            this.registerButton.TabIndex = 7;
            this.registerButton.Text = "Register Match ";
            this.registerButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
            // 
            // newMatchGroupBox
            // 
            this.newMatchGroupBox.Controls.Add(this.label1);
            this.newMatchGroupBox.Controls.Add(this.registerButton);
            this.newMatchGroupBox.Controls.Add(this.label2);
            this.newMatchGroupBox.Controls.Add(this.winnerComboBox);
            this.newMatchGroupBox.Controls.Add(this.loserComboBox);
            this.newMatchGroupBox.Location = new System.Drawing.Point(600, 108);
            this.newMatchGroupBox.Name = "newMatchGroupBox";
            this.newMatchGroupBox.Size = new System.Drawing.Size(404, 274);
            this.newMatchGroupBox.TabIndex = 8;
            this.newMatchGroupBox.TabStop = false;
            this.newMatchGroupBox.Text = "New Match Result";
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(597, 435);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 13);
            this.errorLabel.TabIndex = 9;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1059, 743);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.newMatchGroupBox);
            this.Controls.Add(this.rankingTextBox);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.newMatchGroupBox.ResumeLayout(false);
            this.newMatchGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void rankingTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
