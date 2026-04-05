namespace LR1TrackEditor
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    public class OptionsForm : Form
    {
        private readonly static CultureInfo ci = CultureInfo.InvariantCulture;
        private readonly FormEditor parentform;
        private readonly IContainer components = null;
        private TrackBar trackBar1;
        private Label label2;
        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private Label label1;
        private CheckBox checkBoxTextures;
        private CheckBox checkBoxPowerups;
        private CheckBox checkBoxSkyboxes;
        private TextBox textBox2;
        private Label label3;
        private TrackBar trackBar2;
        private CheckBox checkBoxObjects;
        private CheckBox checkBoxVertexColors;
        private Label label4;
        private PictureBox pictureBox1;
        private Button button3;
        private TextBox textBox3;
        private Label label5;
        private TrackBar trackBar3;
        private CheckBox checkBoxConsole;
        private CheckBox checkBoxGhost;
        private Button button4;
        private GroupBox groupBoxTrackLoad;
        private CheckBox checkBoxTrackPowerups;
        private CheckBox checkBoxTrackRacerPaths;
        private CheckBox checkBoxTrackCollision;
        private CheckBox checkBoxTrackCheckpoints;
        private CheckBox checkBoxTrackEmitters;
        private CheckBox checkBoxTrackStartPositions;
        private CheckBox checkBoxTrackHazards;
        private CheckBox checkBoxTrackSkybox;

        public OptionsForm(FormEditor parentform)
        {
            this.InitializeComponent();
            this.parentform = parentform;
            this.checkBoxGhost.Checked = Settings.Default.GhostPlacing;
            this.checkBoxObjects.Checked = Settings.Default.AutoloadObject;
            this.checkBoxPowerups.Checked = Settings.Default.AutoloadPowerup;
            this.checkBoxSkyboxes.Checked = Settings.Default.doSkybox;
            this.checkBoxTextures.Checked = Settings.Default.doTextures;
            this.checkBoxVertexColors.Checked = Settings.Default.doVertexColors;
            this.checkBoxConsole.Checked = Settings.Default.ShowConsole;
            this.checkBoxTrackPowerups.Checked = Settings.Default.TrackLoadPowerups;
            this.checkBoxTrackRacerPaths.Checked = Settings.Default.TrackLoadRacerPaths;
            this.checkBoxTrackCollision.Checked = Settings.Default.TrackLoadCollisionGeometry;
            this.checkBoxTrackCheckpoints.Checked = Settings.Default.TrackLoadCheckpoints;
            this.checkBoxTrackEmitters.Checked = Settings.Default.TrackLoadEmitters;
            this.checkBoxTrackStartPositions.Checked = Settings.Default.TrackLoadStartPositions;
            this.checkBoxTrackHazards.Checked = Settings.Default.TrackLoadHazards;
            this.checkBoxTrackSkybox.Checked = Settings.Default.TrackLoadSkybox;
            this.pictureBox1.BackColor = Settings.Default.BackgroundColor;
            this.textBox1.Text = Settings.Default.RenderDistance.ToString(ci);
            this.trackBar1.Value = (int)Math.Min(Settings.Default.RenderDistance, (float)this.trackBar1.Maximum);
            this.textBox2.Text = Settings.Default.FlySpeed.ToString(ci);
            this.trackBar2.Value = (int)Math.Min(Settings.Default.FlySpeed * 10f, (float)this.trackBar2.Maximum);
            this.textBox3.Text = Settings.Default.FoV.ToString(ci);
            this.trackBar3.Value = (int)Math.Min(Settings.Default.FoV, (float)this.trackBar3.Maximum);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.AutoloadObject = this.checkBoxObjects.Checked;
            Settings.Default.AutoloadPowerup = this.checkBoxPowerups.Checked;
            Settings.Default.doSkybox = this.checkBoxSkyboxes.Checked;
            Settings.Default.BackgroundColor = this.pictureBox1.BackColor;
            Settings.Default.doTextures = this.checkBoxTextures.Checked;
            Settings.Default.doVertexColors = this.checkBoxVertexColors.Checked;
            Settings.Default.RenderDistance = float.Parse(this.textBox1.Text, ci);
            Settings.Default.FlySpeed = float.Parse(this.textBox2.Text, ci);
            Settings.Default.FoV = float.Parse(this.textBox3.Text, ci);
            Settings.Default.GhostPlacing = this.checkBoxGhost.Checked;
            Settings.Default.ShowConsole = this.checkBoxConsole.Checked;
            Settings.Default.TrackLoadPowerups = this.checkBoxTrackPowerups.Checked;
            Settings.Default.TrackLoadRacerPaths = this.checkBoxTrackRacerPaths.Checked;
            Settings.Default.TrackLoadCollisionGeometry = this.checkBoxTrackCollision.Checked;
            Settings.Default.TrackLoadCheckpoints = this.checkBoxTrackCheckpoints.Checked;
            Settings.Default.TrackLoadEmitters = this.checkBoxTrackEmitters.Checked;
            Settings.Default.TrackLoadStartPositions = this.checkBoxTrackStartPositions.Checked;
            Settings.Default.TrackLoadHazards = this.checkBoxTrackHazards.Checked;
            Settings.Default.TrackLoadSkybox = this.checkBoxTrackSkybox.Checked;
            Settings.Default.Save();
            Console.WriteLine("Settings saved.");
            base.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog
            {
                AllowFullOpen = true,
                SolidColorOnly = true,
                Color = this.pictureBox1.BackColor,
                FullOpen = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.BackColor = dialog.Color;
                Console.WriteLine("Backcolor = " + dialog.Color.ToArgb().ToString("X4").Substring(2));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.parentform.resetDefaultSize();
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
            this.trackBar1 = new TrackBar();
            this.label2 = new Label();
            this.textBox1 = new TextBox();
            this.button1 = new Button();
            this.button2 = new Button();
            this.label1 = new Label();
            this.checkBoxTextures = new CheckBox();
            this.checkBoxPowerups = new CheckBox();
            this.checkBoxSkyboxes = new CheckBox();
            this.textBox2 = new TextBox();
            this.label3 = new Label();
            this.trackBar2 = new TrackBar();
            this.checkBoxObjects = new CheckBox();
            this.checkBoxVertexColors = new CheckBox();
            this.label4 = new Label();
            this.pictureBox1 = new PictureBox();
            this.button3 = new Button();
            this.textBox3 = new TextBox();
            this.label5 = new Label();
            this.trackBar3 = new TrackBar();
            this.checkBoxConsole = new CheckBox();
            this.checkBoxGhost = new CheckBox();
            this.button4 = new Button();
            this.groupBoxTrackLoad = new GroupBox();
            this.checkBoxTrackPowerups = new CheckBox();
            this.checkBoxTrackRacerPaths = new CheckBox();
            this.checkBoxTrackCollision = new CheckBox();
            this.checkBoxTrackCheckpoints = new CheckBox();
            this.checkBoxTrackEmitters = new CheckBox();
            this.checkBoxTrackStartPositions = new CheckBox();
            this.checkBoxTrackHazards = new CheckBox();
            this.checkBoxTrackSkybox = new CheckBox();
            this.trackBar1.BeginInit();
            this.trackBar2.BeginInit();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            this.trackBar3.BeginInit();
            this.groupBoxTrackLoad.SuspendLayout();
            base.SuspendLayout();
            this.trackBar1.AutoSize = false;
            this.trackBar1.Location = new Point(0xee, 0x30);
            this.trackBar1.Maximum = 0x1388;
            this.trackBar1.Minimum = 2;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new Size(180, 0x17);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Tag = 1f;
            this.trackBar1.TickStyle = TickStyle.None;
            this.trackBar1.Value = 800;
            this.trackBar1.Scroll += new EventHandler(this.trackBar_Scroll);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x132, 0x20);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Render distance";
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Location = new Point(0x1a6, 0x30);
            this.textBox1.Name = "textBox1";
            this.textBox1.ShortcutsEnabled = false;
            this.textBox1.Size = new Size(50, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "1000";
            this.textBox1.KeyDown += new KeyEventHandler(this.textBox_KeyDown);
            this.textBox1.Validated += new EventHandler(this.textBox_Validated);
            this.button1.Location = new Point(0x13c, 0x1b1);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Location = new Point(0x18d, 0x1b1);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x4b, 0x17);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0xbc, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x66, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Edit default settings:";
            this.checkBoxTextures.AutoSize = true;
            this.checkBoxTextures.Location = new Point(12, 0x4e);
            this.checkBoxTextures.Name = "checkBoxTextures";
            this.checkBoxTextures.Size = new Size(100, 0x11);
            this.checkBoxTextures.TabIndex = 7;
            this.checkBoxTextures.Text = "Display textures";
            this.checkBoxTextures.UseVisualStyleBackColor = true;
            this.checkBoxPowerups.AutoSize = true;
            this.checkBoxPowerups.Location = new Point(12, 0x20);
            this.checkBoxPowerups.Name = "checkBoxPowerups";
            this.checkBoxPowerups.Size = new Size(120, 0x11);
            this.checkBoxPowerups.TabIndex = 8;
            this.checkBoxPowerups.Text = "Auto-load powerups";
            this.checkBoxPowerups.UseVisualStyleBackColor = true;
            this.checkBoxSkyboxes.AutoSize = true;
            this.checkBoxSkyboxes.Location = new Point(12, 0x7c);
            this.checkBoxSkyboxes.Name = "checkBoxSkyboxes";
            this.checkBoxSkyboxes.Size = new Size(0x6b, 0x11);
            this.checkBoxSkyboxes.TabIndex = 9;
            this.checkBoxSkyboxes.Text = "Display skyboxes";
            this.checkBoxSkyboxes.UseVisualStyleBackColor = true;
            this.textBox2.AcceptsReturn = true;
            this.textBox2.Location = new Point(0x1a6, 0x62);
            this.textBox2.Name = "textBox2";
            this.textBox2.ShortcutsEnabled = false;
            this.textBox2.Size = new Size(50, 20);
            this.textBox2.TabIndex = 12;
            this.textBox2.Text = "1.3";
            this.textBox2.KeyDown += new KeyEventHandler(this.textBox_KeyDown);
            this.textBox2.Validated += new EventHandler(this.textBox_Validated);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x142, 0x52);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x34, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Fly speed";
            this.trackBar2.AutoSize = false;
            this.trackBar2.Location = new Point(0xee, 0x62);
            this.trackBar2.Maximum = 200;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new Size(180, 0x17);
            this.trackBar2.TabIndex = 10;
            this.trackBar2.Tag = 10f;
            this.trackBar2.TickStyle = TickStyle.None;
            this.trackBar2.Value = 15;
            this.trackBar2.Scroll += new EventHandler(this.trackBar_Scroll);
            this.checkBoxObjects.AutoSize = true;
            this.checkBoxObjects.Location = new Point(12, 0x37);
            this.checkBoxObjects.Name = "checkBoxObjects";
            this.checkBoxObjects.Size = new Size(0x88, 0x11);
            this.checkBoxObjects.TabIndex = 13;
            this.checkBoxObjects.Text = "Auto-load static objects";
            this.checkBoxObjects.UseVisualStyleBackColor = true;
            this.checkBoxVertexColors.AutoSize = true;
            this.checkBoxVertexColors.Location = new Point(12, 0x65);
            this.checkBoxVertexColors.Name = "checkBoxVertexColors";
            this.checkBoxVertexColors.Size = new Size(0x7b, 0x11);
            this.checkBoxVertexColors.TabIndex = 14;
            this.checkBoxVertexColors.Text = "Display vertex colors";
            this.checkBoxVertexColors.UseVisualStyleBackColor = true;
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0xeb, 0xea);
            this.label4.Name = "label4";
            this.label4.Size = new Size(210, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Background color (when skybox is hidden):";
            this.pictureBox1.BackColor = SystemColors.AppWorkspace;
            this.pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            this.pictureBox1.Location = new Point(0x196, 0xfc);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x19, 0x19);
            this.pictureBox1.TabIndex = 0x10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new EventHandler(this.button3_Click);
            this.button3.Location = new Point(0x16a, 0xfc);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x26, 0x17);
            this.button3.TabIndex = 0x11;
            this.button3.Text = "Edit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.button3_Click);
            this.textBox3.AcceptsReturn = true;
            this.textBox3.Location = new Point(0x1a6, 0x93);
            this.textBox3.Name = "textBox3";
            this.textBox3.ShortcutsEnabled = false;
            this.textBox3.Size = new Size(50, 20);
            this.textBox3.TabIndex = 20;
            this.textBox3.Text = "60";
            this.textBox3.KeyDown += new KeyEventHandler(this.textBox_KeyDown);
            this.textBox3.Validated += new EventHandler(this.textBox_Validated);
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0x142, 0x83);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x43, 13);
            this.label5.TabIndex = 0x13;
            this.label5.Text = "Field of View";
            this.trackBar3.AutoSize = false;
            this.trackBar3.Location = new Point(0xee, 0x93);
            this.trackBar3.Maximum = 200;
            this.trackBar3.Minimum = 20;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new Size(180, 0x17);
            this.trackBar3.TabIndex = 0x12;
            this.trackBar3.Tag = 1f;
            this.trackBar3.TickStyle = TickStyle.None;
            this.trackBar3.Value = 60;
            this.trackBar3.Scroll += new EventHandler(this.trackBar_Scroll);
            this.checkBoxConsole.AutoSize = true;
            this.checkBoxConsole.Location = new Point(12, 0xb8);
            this.checkBoxConsole.Name = "checkBoxConsole";
            this.checkBoxConsole.Size = new Size(0x5d, 0x11);
            this.checkBoxConsole.TabIndex = 0x15;
            this.checkBoxConsole.Text = "Show console";
            this.checkBoxConsole.UseVisualStyleBackColor = true;
            this.checkBoxGhost.AutoSize = true;
            this.checkBoxGhost.Location = new Point(12, 0x93);
            this.checkBoxGhost.Name = "checkBoxGhost";
            this.checkBoxGhost.Size = new Size(0x9b, 0x11);
            this.checkBoxGhost.TabIndex = 0x16;
            this.checkBoxGhost.Text = "Display ghost when placing";
            this.checkBoxGhost.UseVisualStyleBackColor = true;
            this.button4.Location = new Point(12, 0xfc);
            this.button4.Name = "button4";
            this.button4.Size = new Size(80, 0x24);
            this.button4.TabIndex = 0x17;
            this.button4.Text = "Reset default form size";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new EventHandler(this.button4_Click);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackPowerups);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackRacerPaths);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackCollision);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackCheckpoints);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackEmitters);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackStartPositions);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackHazards);
            this.groupBoxTrackLoad.Controls.Add(this.checkBoxTrackSkybox);
            this.groupBoxTrackLoad.Location = new Point(12, 0x128);
            this.groupBoxTrackLoad.Name = "groupBoxTrackLoad";
            this.groupBoxTrackLoad.Size = new Size(0x1c8, 0x78);
            this.groupBoxTrackLoad.TabIndex = 0x18;
            this.groupBoxTrackLoad.TabStop = false;
            this.groupBoxTrackLoad.Text = "Load when opening a track (.RAB)";
            this.checkBoxTrackPowerups.AutoSize = true;
            this.checkBoxTrackPowerups.Location = new Point(12, 0x16);
            this.checkBoxTrackPowerups.Name = "checkBoxTrackPowerups";
            this.checkBoxTrackPowerups.Size = new Size(110, 0x11);
            this.checkBoxTrackPowerups.TabIndex = 0;
            this.checkBoxTrackPowerups.Text = "Power Up Blocks";
            this.checkBoxTrackPowerups.UseVisualStyleBackColor = true;
            this.checkBoxTrackRacerPaths.AutoSize = true;
            this.checkBoxTrackRacerPaths.Location = new Point(12, 0x2d);
            this.checkBoxTrackRacerPaths.Name = "checkBoxTrackRacerPaths";
            this.checkBoxTrackRacerPaths.Size = new Size(80, 0x11);
            this.checkBoxTrackRacerPaths.TabIndex = 1;
            this.checkBoxTrackRacerPaths.Text = "Racer Paths";
            this.checkBoxTrackRacerPaths.UseVisualStyleBackColor = true;
            this.checkBoxTrackCollision.AutoSize = true;
            this.checkBoxTrackCollision.Location = new Point(12, 0x44);
            this.checkBoxTrackCollision.Name = "checkBoxTrackCollision";
            this.checkBoxTrackCollision.Size = new Size(109, 0x11);
            this.checkBoxTrackCollision.TabIndex = 2;
            this.checkBoxTrackCollision.Text = "Collision Geometry";
            this.checkBoxTrackCollision.UseVisualStyleBackColor = true;
            this.checkBoxTrackCheckpoints.AutoSize = true;
            this.checkBoxTrackCheckpoints.Location = new Point(12, 0x5b);
            this.checkBoxTrackCheckpoints.Name = "checkBoxTrackCheckpoints";
            this.checkBoxTrackCheckpoints.Size = new Size(81, 0x11);
            this.checkBoxTrackCheckpoints.TabIndex = 3;
            this.checkBoxTrackCheckpoints.Text = "Checkpoints";
            this.checkBoxTrackCheckpoints.UseVisualStyleBackColor = true;
            this.checkBoxTrackEmitters.AutoSize = true;
            this.checkBoxTrackEmitters.Location = new Point(0xe6, 0x16);
            this.checkBoxTrackEmitters.Name = "checkBoxTrackEmitters";
            this.checkBoxTrackEmitters.Size = new Size(62, 0x11);
            this.checkBoxTrackEmitters.TabIndex = 4;
            this.checkBoxTrackEmitters.Text = "Emitters";
            this.checkBoxTrackEmitters.UseVisualStyleBackColor = true;
            this.checkBoxTrackStartPositions.AutoSize = true;
            this.checkBoxTrackStartPositions.Location = new Point(0xe6, 0x2d);
            this.checkBoxTrackStartPositions.Name = "checkBoxTrackStartPositions";
            this.checkBoxTrackStartPositions.Size = new Size(90, 0x11);
            this.checkBoxTrackStartPositions.TabIndex = 5;
            this.checkBoxTrackStartPositions.Text = "Start Positions";
            this.checkBoxTrackStartPositions.UseVisualStyleBackColor = true;
            this.checkBoxTrackHazards.AutoSize = true;
            this.checkBoxTrackHazards.Location = new Point(0xe6, 0x44);
            this.checkBoxTrackHazards.Name = "checkBoxTrackHazards";
            this.checkBoxTrackHazards.Size = new Size(64, 0x11);
            this.checkBoxTrackHazards.TabIndex = 6;
            this.checkBoxTrackHazards.Text = "Hazards";
            this.checkBoxTrackHazards.UseVisualStyleBackColor = true;
            this.checkBoxTrackSkybox.AutoSize = true;
            this.checkBoxTrackSkybox.Location = new Point(0xe6, 0x5b);
            this.checkBoxTrackSkybox.Name = "checkBoxTrackSkybox";
            this.checkBoxTrackSkybox.Size = new Size(61, 0x11);
            this.checkBoxTrackSkybox.TabIndex = 7;
            this.checkBoxTrackSkybox.Text = "Skybox";
            this.checkBoxTrackSkybox.UseVisualStyleBackColor = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1e4, 0x1d8);
            base.Controls.Add(this.groupBoxTrackLoad);
            base.Controls.Add(this.button4);
            base.Controls.Add(this.checkBoxGhost);
            base.Controls.Add(this.checkBoxConsole);
            base.Controls.Add(this.textBox3);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.trackBar3);
            base.Controls.Add(this.button3);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.checkBoxVertexColors);
            base.Controls.Add(this.checkBoxObjects);
            base.Controls.Add(this.textBox2);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.trackBar2);
            base.Controls.Add(this.checkBoxSkyboxes);
            base.Controls.Add(this.checkBoxPowerups);
            base.Controls.Add(this.checkBoxTextures);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.textBox1);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.trackBar1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "OptionsForm";
            this.Text = "Options";
            this.trackBar1.EndInit();
            this.trackBar2.EndInit();
            ((ISupportInitialize)this.pictureBox1).EndInit();
            this.trackBar3.EndInit();
            this.groupBoxTrackLoad.ResumeLayout(false);
            this.groupBoxTrackLoad.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            Keys[] source = new Keys[] { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8 };
            source[9] = Keys.D9;
            source[10] = Keys.NumPad0;
            source[11] = Keys.NumPad1;
            source[12] = Keys.NumPad2;
            source[13] = Keys.NumPad3;
            source[14] = Keys.NumPad4;
            source[15] = Keys.NumPad5;
            source[0x10] = Keys.NumPad6;
            source[0x11] = Keys.NumPad7;
            source[0x12] = Keys.NumPad8;
            source[0x13] = Keys.NumPad9;
            source[20] = Keys.OemPeriod;
            source[0x15] = Keys.Delete;
            source[0x16] = Keys.Back;
            source[0x17] = Keys.Left;
            source[0x18] = Keys.Right;
            if (!source.Contains<Keys>(e.KeyData))
            {
                if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Enter))
                {
                    base.Validate();
                }
                e.SuppressKeyPress = true;
            }
        }

        private void textBox_Validated(object sender, EventArgs e)
        {
            TextBox objA = sender as TextBox;
            TrackBar bar = null;
            if (ReferenceEquals(objA, this.textBox1))
            {
                bar = this.trackBar1;
            }
            else if (ReferenceEquals(objA, this.textBox2))
            {
                bar = this.trackBar2;
            }
            else if (ReferenceEquals(objA, this.textBox3))
            {
                bar = this.trackBar3;
            }
            float tag = (float)bar.Tag;
            float num2 = float.Parse(objA.Text, ci);
            if ((num2 * tag) < bar.Minimum)
            {
                num2 = ((float)bar.Minimum) / tag;
                objA.Text = num2.ToString(ci);
            }
            bar.Value = (int)Math.Min(num2 * tag, (float)bar.Maximum);
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar objA = sender as TrackBar;
            TextBox box = null;
            if (ReferenceEquals(objA, this.trackBar1))
            {
                box = this.textBox1;
            }
            else if (ReferenceEquals(objA, this.trackBar2))
            {
                box = this.textBox2;
            }
            else if (ReferenceEquals(objA, this.trackBar3))
            {
                box = this.textBox3;
            }
            float tag = (float)objA.Tag;
            box.Text = (((float)objA.Value) / tag).ToString(ci);
        }
    }
}

