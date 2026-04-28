namespace LR1TrackEditor
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    public class AboutForm : Form
    {
        private readonly IContainer components = null;
        private Label label1;
        private Label versionLabel;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox1;

        public AboutForm()
        {
            this.InitializeComponent();
            this.versionLabel.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (!(!disposing || this.components is null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.versionLabel = new Label();
            this.linkLabel1 = new LinkLabel();
            this.pictureBox1 = new PictureBox();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Tahoma", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x12e, 170);
            this.label1.TabIndex = 0;
            this.label1.Text = "TrackEditor\r\n\r\nOriginally made by grappigegovert\r\nMaintained and updated by Dust Storm\r\n\r\nUses LibLR1 by Will Kirkby\r\n\r\nThanks to:\r\nSluicer - for working out GDB and BMP formats\r\nWillKirkby - for LibLR1 and for the original GDB_Viewer";
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new Point(0x65, 10);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new Size(0x52, 13);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "v1.0.1000.5000";
            this.versionLabel.Click += new EventHandler(this.versionLabel_Click);
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new Point(12, 0x68);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new Size(0xb6, 13);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://bitbucket.org/WillKirkby/liblr1";
            this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.pictureBox1.Location = new Point(0xc3, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(130, 110);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x14c, 0xb8);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.linkLabel1);
            base.Controls.Add(this.versionLabel);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "AboutForm";
            this.Text = "About";
            ((ISupportInitialize)this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://bitbucket.org/WillKirkby/liblr1");
        }

        private void versionLabel_Click(object sender, EventArgs e)
        {
            this.pictureBox1.LoadAsync("http://i.imgur.com/WuElSLh.png");
            this.pictureBox1.Visible = true;
        }
    }
}

