namespace LR1TrackEditor
{
    using LibLR1;
    using LibLR1.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class FormEditor : Form
    {
        private readonly GameView game;
        private FormWindowState previousstate = FormWindowState.Normal;
        private readonly float currentDPI;
        private readonly List<string> edits = new List<string>();
        private static readonly CultureInfo ci = CultureInfo.InvariantCulture;
        private IContainer components = null;
        private PictureBox pictureBox1;
        private MenuStrip menuStrip1;
        private Button button1;
        private ToolStripMenuItem tsmiFile;
        private ToolStripMenuItem tsmiEdit;
        private ToolStripMenuItem tsmiView;
        private ToolStripMenuItem tsmiTextures;
        private ToolStripMenuItem tsmiVertexColors;
        private ToolStripMenuItem tsmiPowerupBricks;
        private ToolStripMenuItem tsmiStaticObjects;
        private ToolStripMenuItem tsmiAnimatedObjects;
        private ToolStripMenuItem tsmiOpen;
        private ToolStripMenuItem tsmiOptions;
        private ToolStripMenuItem tsmiAbout;
        private ToolStripMenuItem tsmiSkybox;
        private ToolStripSeparator tssView1;
        private TabControl tabControl1;
        private TabPage tpSKB;
        private TabPage tpPWB;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private ListBox StaticObjectListBox;
        private Label StaticObjectLabel;
        private Label StaticObjectStatusLabel;
        private ListBox AnimatedObjectListBox;
        private ComboBox AnimatedObjectAnimationComboBox;
        private CheckBox AnimatedObjectLoopCheckBox;
        private Button AnimatedObjectRunButton;
        private Label AnimatedObjectLabel;
        private Label AnimatedObjectAnimationLabel;
        private Label AnimatedObjectStatusLabel;
        private Label label2;
        private Label label1;
        private GroupBox groupBox1;
        private TextBox Color1BoxB;
        private TextBox Color1BoxG;
        private TextBox Color1BoxR;
        private PictureBox Color3Box;
        private PictureBox Color2Box;
        private PictureBox Color1Box;
        private Label label3;
        private ComboBox SkyboxSelector;
        private ComboBox DefaultSkyboxSelector;
        private Label label13;
        private TextBox SkyboxFloatBox;
        private CheckBox SkyboxFloatCheckbox;
        private Button SkyboxApplybutton;
        private Label label10;
        private Label label11;
        private Label label12;
        private TextBox Color3BoxB;
        private TextBox Color3BoxG;
        private TextBox Color3BoxR;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox Color2BoxB;
        private TextBox Color2BoxG;
        private TextBox Color2BoxR;
        private Label label5;
        private Label label4;
        private Label label6;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Panel panel1;
        private TextBox SkyboxIntBox;
        private CheckBox SkyboxIntCheckbox;
        private ToolStripSeparator tssFile1;
        private ToolStripMenuItem tsmiSave;
        private ToolStripMenuItem tsmiSaveSkybox;
        private ToolStripMenuItem tsmiSavePowerups;
        private ToolStripMenuItem tsmiSaveObjects;
        private ToolStripMenuItem tsmiSaveAs;
        private ToolStripMenuItem tsmiSaveSkyboxAs;
        private ToolStripMenuItem tsmiSavePowerupsAs;
        private ToolStripMenuItem tsmiSaveObjectsAs;
        private ToolStripSeparator tssFile2;
        private ToolStripMenuItem tsmiExit;
        private ListBox PWBListBox;
        private Label label27;
        private Label CameraPositionLabel;
        private Label CameraSpeedHeaderLabel;
        private Label CameraSpeedLabel;
        private ComboBox BrickColorComboBox;
        private Button BrickDeleteButton;
        private Button button2;
        private Label label31;
        private Label label30;
        private Label label29;
        private Label label28;
        private TextBox BrickZtextBox;
        private TextBox BrickYtextBox;
        private TextBox BrickXtextBox;
        private Button BrickplaceStopButton;
        private Label Brickplacelabel;
        private Button button3;
        private ToolStripMenuItem tsmiReload;
        private ToolStripSeparator tssFile3;
        private TabPage tabPageRRB;
        private ToolStripMenuItem tsmiAIPaths;
        private ListBox RRBNodeListBox;
        private Label label33;
        private Label label32;
        private ContextMenuStrip RRBListViewContextMenu;
        private ToolStripMenuItem addNewContextMenuItem;
        private ToolStripMenuItem saveContextMenuItem;
        private ToolStripMenuItem tsmiUnloadAll;
        private ToolStripMenuItem unloadSelectedContextMenuItem;
        private ListView RRBListView;
        private ColumnHeader columnHeader1;
        private ToolStripMenuItem editColorContextMenuItem;
        private TabPage tabPage1;
        private Button DebugButton2;
        private Button DebugButton1;
        private ListView DebugListView;
        private CheckBox RRBEditCheckBox;
        private Label rightclickmelabel;
        private ImageList imageList1;
        private ToolStripMenuItem tsmiStartPositions;
        private ToolStripMenuItem tsmiCheckpoints;
        private ToolStripMenuItem tsmiHazards;
        private ToolStripMenuItem tsmiEmitters;
        private ToolStripMenuItem tsmiCollisionGeometry;

        public FormEditor(GameView game)
        {
            Graphics objA = base.CreateGraphics();
            try
            {
                this.currentDPI = objA.DpiX;
            }
            finally
            {
                if (objA is object)
                {
                    objA.Dispose();
                }
            }
            this.InitializeComponent();
            this.ScaleControls();
            this.PWBListBox.DisplayMember = "Description";
            this.game = Utils.game = game;
            game.form = InputHandler.form = Utils.form = this;
            this.tsmiSkybox.Checked = Settings.Default.doSkybox;
            this.tsmiTextures.Checked = Settings.Default.doTextures;
            this.tsmiVertexColors.Checked = Settings.Default.doVertexColors;
            this.tsmiPowerupBricks.Checked = Settings.Default.AutoloadPowerup;
            this.tsmiStaticObjects.Checked = Settings.Default.AutoloadObject;
            this.tsmiCollisionGeometry.Checked = game.doDrawCollision;
            base.Size = Settings.Default.FormSize;
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            AboutForm objA = new AboutForm();
            try
            {
                objA.ShowDialog();
            }
            finally
            {
                if (objA is object)
                {
                    objA.Dispose();
                }
            }
        }

        private void addNewContextMenuItem_Click(object sender, EventArgs e)
        {
            Utils.OpenFileDialog(3);
        }

        private void BrickColorComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Brick current;
            int num = 0;
            int num2 = 0;
            IEnumerator enumerator = this.PWBListBox.SelectedItems.GetEnumerator();
            try
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    current = (Brick)enumerator.Current;
                    int num3 = current.index;
                    if (current.colored)
                    {
                        num3 += this.game.pwb.ColorBricks.Count;
                    }
                    current.Color = (string)this.BrickColorComboBox.SelectedItem;
                    if (!current.colored)
                    {
                        if (((string)this.BrickColorComboBox.SelectedItem) != "White")
                        {
                            PWB_ColorBrick item = new PWB_ColorBrick(current.Position, current.ColorByte());
                            this.game.pwb.WhiteBricks.RemoveAt(current.index - num2);
                            this.game.pwb.ColorBricks.Add(item);
                            current.index = this.game.pwb.ColorBricks.Count - 1;
                            current.colored = true;
                            num2++;
                        }
                    }
                    else if (((string)this.BrickColorComboBox.SelectedItem) != "White")
                    {
                        this.game.pwb.ColorBricks[current.index].Color = current.ColorByte();
                    }
                    else
                    {
                        PWB_WhiteBrick item = new PWB_WhiteBrick(current.Position);
                        this.game.pwb.ColorBricks.RemoveAt(current.index - num);
                        this.game.pwb.WhiteBricks.Add(item);
                        current.index = this.game.pwb.WhiteBricks.Count - 1;
                        current.colored = false;
                        num++;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable objA)
                {
                    objA.Dispose();
                }
            }
            Brick[] destination = new Brick[this.PWBListBox.SelectedItems.Count];
            this.PWBListBox.SelectedItems.CopyTo(destination, 0);
            this.refreshPWB(false);
            Brick[] brickArray2 = destination;
            int index = 0;
            while (true)
            {
                if (index >= brickArray2.Length)
                {
                    if (!this.edits.Contains("PWB"))
                    {
                        this.edits.Add("PWB");
                    }
                    return;
                }
                current = brickArray2[index];
                int num4 = current.colored ? current.index : (current.index + this.game.pwb.ColorBricks.Count);
                this.PWBListBox.SelectedIndices.Add(num4);
                index++;
            }
        }

        private void BrickDeleteButton_Click(object sender, EventArgs e)
        {
            if (this.PWBListBox.SelectedItems.Count > 0)
            {
                this.DeleteSelectedBricks();
            }
        }

        public void BrickplaceStopButton_Click(object sender, EventArgs e)
        {
            this.game.placing = false;
            this.Brickplacelabel.Visible = false;
            this.BrickplaceStopButton.Visible = false;
            this.PWBListBox.Visible = true;
            this.tabControl1.Enabled = true;
            if (this.game.editmode == 1)
            {
                this.refreshPWB(false);
                int[] indices = new int[this.game.placed];
                bool[] colored = new bool[this.game.placed];
                int index = 0;
                while (true)
                {
                    if (index >= this.game.placed)
                    {
                        this.SelectBricks(indices, colored);
                        break;
                    }
                    indices[index] = this.game.pwb.ColorBricks.Count - (index + 1);
                    colored[index] = true;
                    index++;
                }
            }
            this.game.placed = 0;
        }

        private void BricktextBox_Validated(object sender, EventArgs e)
        {
            if (this.PWBListBox.SelectedItems.Count == 1)
            {
                Brick selectedItem = (Brick)this.PWBListBox.SelectedItem;
                NumberStyles @float = NumberStyles.Float;
                if ((!float.TryParse(this.BrickXtextBox.Text, @float, ci, out float num) || !float.TryParse(this.BrickYtextBox.Text, @float, ci, out float num2)) || !float.TryParse(this.BrickZtextBox.Text, @float, ci, out float num3))
                {
                    this.BrickXtextBox.Text = selectedItem.Position.X.ToString(ci);
                    this.BrickYtextBox.Text = selectedItem.Position.Y.ToString(ci);
                    this.BrickZtextBox.Text = selectedItem.Position.Z.ToString(ci);
                }
                else
                {
                    if (selectedItem.colored)
                    {
                        this.game.pwb.ColorBricks[selectedItem.index].Position = new LRVector3(num, num2, num3);
                    }
                    else
                    {
                        this.game.pwb.WhiteBricks[selectedItem.index].Position = new LRVector3(num, num2, num3);
                    }
                    this.MarkPwbEdited();
                    this.refreshPWB(true);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Focus();
            this.game.IsMouseVisible = false;
            MouseHelper.SetPosition(this.game.drawsurface, this.game.width / 2, this.game.height / 2);
            this.game.mouselock = true;
            Console.WriteLine("Mouse locked");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.game.pwb is null)
            {
                System.Windows.Forms.MessageBox.Show("Please load a PWB file before adding new bricks.", "Error");
            }
            else
            {
                this.SelectBricks(new int[0], new bool[0]);
                this.game.placing = true;
                this.game.placingmodel = this.game.pupbrick;
                this.PWBListBox.Visible = false;
                this.Brickplacelabel.Visible = true;
                this.Brickplacelabel.BringToFront();
                this.BrickplaceStopButton.Visible = true;
                this.BrickplaceStopButton.BringToFront();
                this.tabControl1.Enabled = false;
            }
        }

        public void ClearEdits(string format = null)
        {
            if (format is null)
            {
                this.edits.Clear();
            }
            else
            {
                this.edits.Remove(format);
            }
        }

        public void MarkPwbEdited()
        {
            if (!this.edits.Contains("PWB"))
            {
                this.edits.Add("PWB");
            }
        }

        public void RefreshSelectedBrickFields()
        {
            if (this.PWBListBox.SelectedItems.Count == 1)
            {
                Brick selectedItem = (Brick)this.PWBListBox.SelectedItem;
                this.BrickColorComboBox.SelectedItem = selectedItem.Color;
                this.BrickXtextBox.Text = selectedItem.Position.X.ToString(ci);
                this.BrickYtextBox.Text = selectedItem.Position.Y.ToString(ci);
                this.BrickZtextBox.Text = selectedItem.Position.Z.ToString(ci);
                return;
            }

            if (this.PWBListBox.SelectedItems.Count == 0)
            {
                this.BrickColorComboBox.SelectedIndex = -1;
                this.BrickXtextBox.Text = "";
                this.BrickYtextBox.Text = "";
                this.BrickZtextBox.Text = "";
                return;
            }

            string text = this.BrickZtextBox.Text = "";
            this.BrickXtextBox.Text = this.BrickYtextBox.Text = text;
            bool sameColor = true;
            string color = ((Brick)this.PWBListBox.SelectedItems[0]).Color;
            foreach (Brick current in this.PWBListBox.SelectedItems)
            {
                sameColor &= current.Color == color;
            }
            this.BrickColorComboBox.SelectedIndex = sameColor ? this.BrickColorComboBox.Items.IndexOf(color) : -1;
        }

        private void ColorBox_Click(object sender, EventArgs e)
        {
            PictureBox box = sender as PictureBox;
            ColorDialog dialog = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                SolidColorOnly = true,
                Color = box.BackColor
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                box.BackColor = dialog.Color;
                this.LoadColorInputBoxes();
            }
        }

        private void ColorBox_KeyDown(object sender, KeyEventArgs e)
        {
            List<System.Windows.Forms.Keys> list = new List<System.Windows.Forms.Keys> {
                System.Windows.Forms.Keys.D0,
                System.Windows.Forms.Keys.D1,
                System.Windows.Forms.Keys.D2,
                System.Windows.Forms.Keys.D3,
                System.Windows.Forms.Keys.D4,
                System.Windows.Forms.Keys.D5,
                System.Windows.Forms.Keys.D6,
                System.Windows.Forms.Keys.D7,
                System.Windows.Forms.Keys.D8,
                System.Windows.Forms.Keys.D9
            };
            System.Windows.Forms.Keys[] source = new System.Windows.Forms.Keys[] { System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.Right, System.Windows.Forms.Keys.Delete, System.Windows.Forms.Keys.Back };
            if ((sender == this.SkyboxIntBox) || (sender == this.SkyboxFloatBox))
            {
                list.Add(System.Windows.Forms.Keys.OemMinus);
                list.Add(System.Windows.Forms.Keys.Subtract);
            }
            if (sender == this.SkyboxFloatBox)
            {
                list.Add(System.Windows.Forms.Keys.OemPeriod);
                list.Add(System.Windows.Forms.Keys.Decimal);
            }
            if ((e.KeyData == System.Windows.Forms.Keys.Enter) || (e.KeyData == System.Windows.Forms.Keys.Enter))
            {
                base.Validate();
            }
            else if (!(list.Contains(e.KeyData) || source.Contains<System.Windows.Forms.Keys>(e.KeyCode)))
            {
                e.SuppressKeyPress = true;
            }
        }

        private void ColorBox_Validated(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            if (!int.TryParse(box.Text, out int num))
            {
                box.Text = "";
            }
            else
            {
                box.Text = Math.Max(Math.Min(num, 0xff), 0).ToString();
                this.LoadColorBoxes();
            }
        }

        private void DebugButton1_Click(object sender, EventArgs e)
        {
            this.DebugListView.Items.Clear();
            for (int i = 0; i < this.game.loadedmodel.parts.Count; i++)
            {
                this.DebugListView.Items.Add("Part " + i.ToString());
                this.DebugListView.Items[i].Checked = this.game.loadedmodel.parts[i].visible;
            }
        }

        private void DebugButton2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DebugListView.Items.Count; i++)
            {
                this.game.loadedmodel.parts[i].visible = this.DebugListView.Items[i].Checked;
            }
        }

        public void DeleteSelectedBricks()
        {
            int num = 0;
            int num2 = 0;
            this.game.SelectedBrickIndices.Clear();
            IEnumerator enumerator = this.PWBListBox.SelectedItems.GetEnumerator();
            try
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    Brick current = (Brick)enumerator.Current;
                    if (current.colored)
                    {
                        this.game.pwb.ColorBricks.RemoveAt(current.index - num2);
                        num2++;
                        continue;
                    }
                    this.game.pwb.WhiteBricks.RemoveAt(current.index - num);
                    num++;
                }
            }
            finally
            {
                if (enumerator is IDisposable objA)
                {
                    objA.Dispose();
                }
            }
            this.refreshPWB(false);
            if (!this.edits.Contains("PWB"))
            {
                this.edits.Add("PWB");
            }
        }

        public void DeselectBricks()
        {
            this.PWBListBox.SelectedIndices.Clear();
        }

        private void displayToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            string str = item.Checked ? "enabled" : "disabled";
            if (sender == this.tsmiTextures)
            {
                this.game.doTextures = item.Checked;
                Console.WriteLine("Textures " + str);
                InputHandler.overridePressed(Microsoft.Xna.Framework.Input.Keys.T, true);
            }
            else if (sender == this.tsmiVertexColors)
            {
                this.game.doVertexColors = item.Checked;
                Console.WriteLine("Vertex colors " + str);
                InputHandler.overridePressed(Microsoft.Xna.Framework.Input.Keys.V, true);
            }
            else if (sender == this.tsmiSkybox)
            {
                this.game.doDrawSKB = item.Checked;
                Console.WriteLine("Skybox " + str);
            }
            else if (sender == this.tsmiAIPaths)
            {
                this.game.doDrawRRB = item.Checked;
                Console.WriteLine("AI paths " + str);
            }
            else if (sender == this.tsmiPowerupBricks)
            {
                this.game.doDrawPWB = item.Checked;
                Console.WriteLine("Powerups " + str);
            }
            else if (sender == this.tsmiStaticObjects)
            {
                this.game.doDrawStaticObj = item.Checked;
                Console.WriteLine("Static objects " + str);
            }
            else if (sender == this.tsmiAnimatedObjects)
            {
                this.game.doDrawAnimObj = item.Checked;
                Console.WriteLine("Animated objects " + str);
            }
            else if (sender == this.tsmiStartPositions)
            {
                this.game.doDrawSPB = item.Checked;
                Console.WriteLine("Start positions " + str);
            }
            else if (sender == this.tsmiCheckpoints)
            {
                this.game.doDrawCPB = item.Checked;
                Console.WriteLine("Checkpoints " + str);
            }
            else if (sender == this.tsmiHazards)
            {
                this.game.doDrawHZB = item.Checked;
                Console.WriteLine("Hazards " + str);
            }
            else if (sender == this.tsmiEmitters)
            {
                this.game.doDrawEMB = item.Checked;
                Console.WriteLine("Emitters " + str);
            }
            else if (sender == this.tsmiCollisionGeometry)
            {
                this.game.doDrawCollision = item.Checked;
                Console.WriteLine("Collision geometry " + str);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!(!disposing || (this.components is null)))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void doTexturesChanged(bool value)
        {
            this.tsmiTextures.Checked = value;
        }

        public void doVertexColorsChanged(bool value)
        {
            this.tsmiVertexColors.Checked = value;
        }

        private void editColorContextMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                SolidColorOnly = true
            };
            Microsoft.Xna.Framework.Color color = this.game.rrbs[(int)this.RRBListView.SelectedItems[0].Tag].color;
            dialog.Color = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.game.rrbs[(int)this.RRBListView.SelectedItems[0].Tag].color = new Microsoft.Xna.Framework.Color(dialog.Color.R, dialog.Color.G, dialog.Color.B);
                this.game.rrbs[(int)this.RRBListView.SelectedItems[0].Tag].generatePoints();
                this.refreshRRB();
            }
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            //null.Substring(3, 2);
            base.Close();
        }

        private void FormEditor_DragEnter(object sender, DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            if (formats.Contains<string>("FileNameW"))
            {
                object data = e.Data.GetData("FileGroupDescriptorW");
                if (data is string[])
                {
                    //string[] strArray2 = (string[])data; Removed unused variable?
                }
            }
        }

        private void FormEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((this.edits.Count > 0) && (System.Windows.Forms.MessageBox.Show("You have made changes in: " + string.Join(", ", this.edits) + "\nDo you want to discard these changes and exit anyway?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No))
            {
                e.Cancel = true;
            }
            else
            {
                Settings.Default.FormSize = base.Size;
                Settings.Default.Save();
                this.game.Exit();
            }
        }

        private void FormEditor_Resize(object sender, EventArgs e)
        {
            if (!(((base.WindowState != FormWindowState.Maximized) || (this.previousstate != FormWindowState.Normal)) && ((base.WindowState != FormWindowState.Normal) || (this.previousstate != FormWindowState.Maximized))))
            {
                this.game.setSurface(this.getPCTHandle(), this.getPCTSize(), true);
                this.previousstate = base.WindowState;
            }
        }

        private void FormEditor_ResizeEnd(object sender, EventArgs e)
        {
            this.game.setSurface(this.getPCTHandle(), this.getPCTSize(), true);
        }

        public IntPtr getPCTHandle() =>
            this.pictureBox1.Handle;

        public Size getPCTSize() =>
            new Size(this.pictureBox1.Width - 2, this.pictureBox1.Height - 2);

        private void InitializeComponent()
        {
            this.components = new Container();
            this.pictureBox1 = new PictureBox();
            this.menuStrip1 = new MenuStrip();
            this.tsmiFile = new ToolStripMenuItem();
            this.tsmiOpen = new ToolStripMenuItem();
            this.tssFile1 = new ToolStripSeparator();
            this.tsmiReload = new ToolStripMenuItem();
            this.tssFile3 = new ToolStripSeparator();
            this.tsmiSave = new ToolStripMenuItem();
            this.tsmiSaveSkybox = new ToolStripMenuItem();
            this.tsmiSavePowerups = new ToolStripMenuItem();
            this.tsmiSaveObjects = new ToolStripMenuItem();
            this.tsmiSaveAs = new ToolStripMenuItem();
            this.tsmiSaveSkyboxAs = new ToolStripMenuItem();
            this.tsmiSavePowerupsAs = new ToolStripMenuItem();
            this.tsmiSaveObjectsAs = new ToolStripMenuItem();
            this.tssFile2 = new ToolStripSeparator();
            this.tsmiExit = new ToolStripMenuItem();
            this.tsmiEdit = new ToolStripMenuItem();
            this.tsmiView = new ToolStripMenuItem();
            this.tsmiSkybox = new ToolStripMenuItem();
            this.tsmiTextures = new ToolStripMenuItem();
            this.tsmiVertexColors = new ToolStripMenuItem();
            this.tssView1 = new ToolStripSeparator();
            this.tsmiPowerupBricks = new ToolStripMenuItem();
            this.tsmiAIPaths = new ToolStripMenuItem();
            this.tsmiStaticObjects = new ToolStripMenuItem();
            this.tsmiAnimatedObjects = new ToolStripMenuItem();
            this.tsmiStartPositions = new ToolStripMenuItem();
            this.tsmiCheckpoints = new ToolStripMenuItem();
            this.tsmiHazards = new ToolStripMenuItem();
            this.tsmiEmitters = new ToolStripMenuItem();
            this.tsmiCollisionGeometry = new ToolStripMenuItem();
            this.tsmiOptions = new ToolStripMenuItem();
            this.tsmiAbout = new ToolStripMenuItem();
            this.button1 = new Button();
            this.tabControl1 = new TabControl();
            this.tpSKB = new TabPage();
            this.DefaultSkyboxSelector = new ComboBox();
            this.label13 = new Label();
            this.SkyboxFloatBox = new TextBox();
            this.SkyboxFloatCheckbox = new CheckBox();
            this.SkyboxApplybutton = new Button();
            this.groupBox1 = new GroupBox();
            this.SkyboxIntBox = new TextBox();
            this.SkyboxIntCheckbox = new CheckBox();
            this.label10 = new Label();
            this.label11 = new Label();
            this.label12 = new Label();
            this.Color3BoxB = new TextBox();
            this.Color3BoxG = new TextBox();
            this.Color3BoxR = new TextBox();
            this.label7 = new Label();
            this.label8 = new Label();
            this.label9 = new Label();
            this.Color2BoxB = new TextBox();
            this.Color2BoxG = new TextBox();
            this.Color2BoxR = new TextBox();
            this.label5 = new Label();
            this.label4 = new Label();
            this.label6 = new Label();
            this.Color1BoxB = new TextBox();
            this.Color1BoxG = new TextBox();
            this.Color1BoxR = new TextBox();
            this.Color3Box = new PictureBox();
            this.Color2Box = new PictureBox();
            this.label1 = new Label();
            this.Color1Box = new PictureBox();
            this.label3 = new Label();
            this.label2 = new Label();
            this.SkyboxSelector = new ComboBox();
            this.tpPWB = new TabPage();
            this.button3 = new Button();
            this.label31 = new Label();
            this.label30 = new Label();
            this.label29 = new Label();
            this.label28 = new Label();
            this.BrickZtextBox = new TextBox();
            this.BrickYtextBox = new TextBox();
            this.BrickXtextBox = new TextBox();
            this.button2 = new Button();
            this.BrickColorComboBox = new ComboBox();
            this.BrickDeleteButton = new Button();
            this.PWBListBox = new ListBox();
            this.tabPageRRB = new TabPage();
            this.rightclickmelabel = new Label();
            this.RRBEditCheckBox = new CheckBox();
            this.RRBListView = new ListView();
            this.columnHeader1 = new ColumnHeader();
            this.RRBListViewContextMenu = new ContextMenuStrip(this.components);
            this.addNewContextMenuItem = new ToolStripMenuItem();
            this.editColorContextMenuItem = new ToolStripMenuItem();
            this.saveContextMenuItem = new ToolStripMenuItem();
            this.unloadSelectedContextMenuItem = new ToolStripMenuItem();
            this.tsmiUnloadAll = new ToolStripMenuItem();
            this.imageList1 = new ImageList(this.components);
            this.RRBNodeListBox = new ListBox();
            this.label33 = new Label();
            this.label32 = new Label();
            this.tabPage3 = new TabPage();
            this.tabPage4 = new TabPage();
            this.StaticObjectStatusLabel = new Label();
            this.StaticObjectListBox = new ListBox();
            this.StaticObjectLabel = new Label();
            this.AnimatedObjectStatusLabel = new Label();
            this.AnimatedObjectRunButton = new Button();
            this.AnimatedObjectLoopCheckBox = new CheckBox();
            this.AnimatedObjectAnimationComboBox = new ComboBox();
            this.AnimatedObjectAnimationLabel = new Label();
            this.AnimatedObjectListBox = new ListBox();
            this.AnimatedObjectLabel = new Label();
            this.tabPage1 = new TabPage();
            this.DebugButton2 = new Button();
            this.DebugButton1 = new Button();
            this.DebugListView = new ListView();
            this.BrickplaceStopButton = new Button();
            this.Brickplacelabel = new Label();
            this.label14 = new Label();
            this.label15 = new Label();
            this.label16 = new Label();
            this.label17 = new Label();
            this.label19 = new Label();
            this.label20 = new Label();
            this.label21 = new Label();
            this.label22 = new Label();
            this.label23 = new Label();
            this.label24 = new Label();
            this.label25 = new Label();
            this.label26 = new Label();
            this.panel1 = new Panel();
            this.label27 = new Label();
            this.CameraPositionLabel = new Label();
            this.CameraSpeedHeaderLabel = new Label();
            this.CameraSpeedLabel = new Label();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tpSKB.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)this.Color3Box).BeginInit();
            ((ISupportInitialize)this.Color2Box).BeginInit();
            ((ISupportInitialize)this.Color1Box).BeginInit();
            this.tpPWB.SuspendLayout();
            this.tabPageRRB.SuspendLayout();
            this.RRBListViewContextMenu.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.pictureBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(9, 0x1a);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x322, 0x25a);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseEnter += new EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseWheel += (s, e) => MouseHelper.AddScrollDelta(e.Delta);
            this.MouseWheel += (s, e) => MouseHelper.AddScrollDelta(e.Delta);
            this.menuStrip1.ImageScalingSize = new Size(20, 20);
            ToolStripItem[] menuStripItems = new ToolStripItem[] { this.tsmiFile, this.tsmiEdit, this.tsmiView, this.tsmiOptions, this.tsmiAbout };
            this.menuStrip1.Items.AddRange(menuStripItems);
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new Size(0x43c, 0x1c);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.tsmiFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ToolStripItem[] fileToolStripItems = new ToolStripItem[] { this.tsmiOpen, this.tssFile1, this.tsmiReload, this.tssFile3, this.tsmiSave, this.tsmiSaveAs, this.tssFile2, this.tsmiExit };
            this.tsmiFile.DropDownItems.AddRange(fileToolStripItems);
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new Size(0x2c, 0x18);
            this.tsmiFile.Text = "&File";
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.Size = new Size(0xdb, 0x1a);
            this.tsmiOpen.Text = "&Open...        (Ctrl+O)";
            this.tsmiOpen.TextImageRelation = TextImageRelation.Overlay;
            this.tsmiOpen.Click += new EventHandler(this.tsmiOpen_Click);
            this.tssFile1.Name = "tssFile1";
            this.tssFile1.Size = new Size(0xd8, 6);
            this.tsmiReload.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiReload.Name = "tsmiReload";
            this.tsmiReload.Size = new Size(0xdb, 0x1a);
            this.tsmiReload.Text = "&Reload         (Ctrl+R)";
            this.tsmiReload.Click += new EventHandler(this.tsmiReload_Click);
            this.tssFile3.Name = "tssFile3";
            this.tssFile3.Size = new Size(0xd8, 6);
            ToolStripItem[] saveToolStripItems = new ToolStripItem[] { this.tsmiSaveSkybox, this.tsmiSavePowerups, this.tsmiSaveObjects };
            this.tsmiSave.DropDownItems.AddRange(saveToolStripItems);
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new Size(0xdb, 0x1a);
            this.tsmiSave.Text = "&Save";
            this.tsmiSaveSkybox.Name = "tsmiSaveSkybox";
            this.tsmiSaveSkybox.Size = new Size(0xb7, 0x1a);
            this.tsmiSaveSkybox.Text = "Save &Skybox";
            this.tsmiSaveSkybox.Click += new EventHandler(this.tsmiSaveSkybox_Click);
            this.tsmiSavePowerups.Name = "tsmiSavePowerups";
            this.tsmiSavePowerups.Size = new Size(0xb7, 0x1a);
            this.tsmiSavePowerups.Text = "Save &Powerups";
            this.tsmiSavePowerups.Click += new EventHandler(this.tsmiSavePowerups_Click);
            this.tsmiSaveObjects.Name = "tsmiSaveObjects";
            this.tsmiSaveObjects.Size = new Size(0xb7, 0x1a);
            this.tsmiSaveObjects.Text = "Save &Objects";
            ToolStripItem[] saveAsToolStripItems = new ToolStripItem[] { this.tsmiSaveSkyboxAs, this.tsmiSavePowerupsAs, this.tsmiSaveObjectsAs };
            this.tsmiSaveAs.DropDownItems.AddRange(saveAsToolStripItems);
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.Size = new Size(0xdb, 0x1a);
            this.tsmiSaveAs.Text = "Save &As";
            this.tsmiSaveSkyboxAs.Name = "tsmiSaveSkyboxAs";
            this.tsmiSaveSkyboxAs.Size = new Size(0xd4, 0x1a);
            this.tsmiSaveSkyboxAs.Text = "Save &Skybox As...";
            this.tsmiSaveSkyboxAs.Click += new EventHandler(this.tsmiSaveSkyboxAs_Click);
            this.tsmiSavePowerupsAs.Name = "tsmiSavePowerupsAs";
            this.tsmiSavePowerupsAs.Size = new Size(0xd4, 0x1a);
            this.tsmiSavePowerupsAs.Text = "Save &Powerups As...";
            this.tsmiSavePowerupsAs.Click += new EventHandler(this.tsmiSavePowerupsAs_Click);
            this.tsmiSaveObjectsAs.Name = "tsmiSaveObjectsAs";
            this.tsmiSaveObjectsAs.Size = new Size(0xd4, 0x1a);
            this.tsmiSaveObjectsAs.Text = "Save &Objects As...";
            this.tssFile2.Name = "tssFile2";
            this.tssFile2.Size = new Size(0xd8, 6);
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new Size(0xdb, 0x1a);
            this.tsmiExit.Text = "&Exit";
            this.tsmiExit.Click += new EventHandler(this.tsmiExit_Click);
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new Size(0x2f, 0x18);
            this.tsmiEdit.Text = "&Edit";
            ToolStripItem[] viewToolStripItems = new ToolStripItem[] { this.tsmiSkybox, this.tsmiTextures, this.tsmiVertexColors, this.tssView1, this.tsmiPowerupBricks, this.tsmiAIPaths, this.tsmiStaticObjects, this.tsmiAnimatedObjects, this.tsmiCollisionGeometry, this.tsmiStartPositions, this.tsmiCheckpoints, this.tsmiHazards, this.tsmiEmitters };
            this.tsmiView.DropDownItems.AddRange(viewToolStripItems);
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.Size = new Size(0x35, 0x18);
            this.tsmiView.Text = "&View";
            this.tsmiSkybox.Checked = true;
            this.tsmiSkybox.CheckOnClick = true;
            this.tsmiSkybox.CheckState = CheckState.Checked;
            this.tsmiSkybox.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiSkybox.Name = "tsmiSkybox";
            this.tsmiSkybox.Size = new Size(0xb8, 0x1a);
            this.tsmiSkybox.Text = "&Skybox";
            this.tsmiSkybox.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiTextures.Checked = true;
            this.tsmiTextures.CheckOnClick = true;
            this.tsmiTextures.CheckState = CheckState.Checked;
            this.tsmiTextures.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiTextures.Name = "tsmiTextures";
            this.tsmiTextures.Size = new Size(0xb8, 0x1a);
            this.tsmiTextures.Text = "&Textures";
            this.tsmiTextures.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiVertexColors.Checked = true;
            this.tsmiVertexColors.CheckOnClick = true;
            this.tsmiVertexColors.CheckState = CheckState.Checked;
            this.tsmiVertexColors.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiVertexColors.Name = "tsmiVertexColors";
            this.tsmiVertexColors.Size = new Size(0xb8, 0x1a);
            this.tsmiVertexColors.Text = "&Vertex colors";
            this.tsmiVertexColors.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tssView1.Name = "tssView1";
            this.tssView1.Size = new Size(0xb5, 6);
            this.tsmiPowerupBricks.CheckOnClick = true;
            this.tsmiPowerupBricks.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiPowerupBricks.Name = "tsmiPowerupBricks";
            this.tsmiPowerupBricks.Size = new Size(0xb8, 0x1a);
            this.tsmiPowerupBricks.Text = "&Powerup bricks";
            this.tsmiPowerupBricks.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiAIPaths.Checked = true;
            this.tsmiAIPaths.CheckOnClick = true;
            this.tsmiAIPaths.CheckState = CheckState.Checked;
            this.tsmiAIPaths.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiAIPaths.Name = "tsmiAIPaths";
            this.tsmiAIPaths.Size = new Size(0xb8, 0x1a);
            this.tsmiAIPaths.Text = "&AI paths";
            this.tsmiAIPaths.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiStaticObjects.CheckOnClick = true;
            this.tsmiStaticObjects.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiStaticObjects.Name = "tsmiStaticObjects";
            this.tsmiStaticObjects.Size = new Size(0xb8, 0x1a);
            this.tsmiStaticObjects.Text = "Static &objects";
            this.tsmiStaticObjects.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiAnimatedObjects.CheckOnClick = true;
            this.tsmiAnimatedObjects.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiAnimatedObjects.Name = "tsmiAnimatedObjects";
            this.tsmiAnimatedObjects.Size = new Size(0xb8, 0x1a);
            this.tsmiAnimatedObjects.Text = "A&nimated objects";
            this.tsmiAnimatedObjects.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiCollisionGeometry.CheckOnClick = true;
            this.tsmiCollisionGeometry.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiCollisionGeometry.Name = "tsmiCollisionGeometry";
            this.tsmiCollisionGeometry.Size = new Size(0xb8, 0x1a);
            this.tsmiCollisionGeometry.Text = "&Collision geometry";
            this.tsmiCollisionGeometry.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiStartPositions.Checked = true;
            this.tsmiStartPositions.CheckOnClick = true;
            this.tsmiStartPositions.CheckState = CheckState.Checked;
            this.tsmiStartPositions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiStartPositions.Name = "tsmiStartPositions";
            this.tsmiStartPositions.Size = new Size(0xb8, 0x1a);
            this.tsmiStartPositions.Text = "Start &positions";
            this.tsmiStartPositions.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiCheckpoints.Checked = true;
            this.tsmiCheckpoints.CheckOnClick = true;
            this.tsmiCheckpoints.CheckState = CheckState.Checked;
            this.tsmiCheckpoints.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiCheckpoints.Name = "tsmiCheckpoints";
            this.tsmiCheckpoints.Size = new Size(0xb8, 0x1a);
            this.tsmiCheckpoints.Text = "&Checkpoints";
            this.tsmiCheckpoints.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiHazards.Checked = true;
            this.tsmiHazards.CheckOnClick = true;
            this.tsmiHazards.CheckState = CheckState.Checked;
            this.tsmiHazards.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiHazards.Name = "tsmiHazards";
            this.tsmiHazards.Size = new Size(0xb8, 0x1a);
            this.tsmiHazards.Text = "&Hazards";
            this.tsmiHazards.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiEmitters.Checked = true;
            this.tsmiEmitters.CheckOnClick = true;
            this.tsmiEmitters.CheckState = CheckState.Checked;
            this.tsmiEmitters.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsmiEmitters.Name = "tsmiEmitters";
            this.tsmiEmitters.Size = new Size(0xb8, 0x1a);
            this.tsmiEmitters.Text = "&Emitters";
            this.tsmiEmitters.CheckedChanged += new EventHandler(this.displayToolStripMenuItem_CheckedChanged);
            this.tsmiOptions.Name = "tsmiOptions";
            this.tsmiOptions.Size = new Size(0x52, 0x18);
            this.tsmiOptions.Text = "&Options...";
            this.tsmiOptions.Click += new EventHandler(this.tsmiOptions_Click);
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new Size(0x47, 0x18);
            this.tsmiAbout.Text = "&About...";
            this.tsmiAbout.Click += new EventHandler(this.tsmiAbout_Click);
            this.button1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(0x331, 0x264);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 2;
            this.button1.TabStop = false;
            this.button1.Text = "Lock mouse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.tabControl1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tabControl1.Controls.Add(this.tpSKB);
            this.tabControl1.Controls.Add(this.tpPWB);
            this.tabControl1.Controls.Add(this.tabPageRRB);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Enabled = false;
            this.tabControl1.Location = new System.Drawing.Point(0x331, 0x1a);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(0x10b, 0x19b);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tpSKB.AutoScroll = true;
            this.tpSKB.Controls.Add(this.DefaultSkyboxSelector);
            this.tpSKB.Controls.Add(this.label13);
            this.tpSKB.Controls.Add(this.SkyboxFloatBox);
            this.tpSKB.Controls.Add(this.SkyboxFloatCheckbox);
            this.tpSKB.Controls.Add(this.SkyboxApplybutton);
            this.tpSKB.Controls.Add(this.groupBox1);
            this.tpSKB.Controls.Add(this.SkyboxSelector);
            this.tpSKB.Location = new System.Drawing.Point(4, 0x19);
            this.tpSKB.Name = "tpSKB";
            this.tpSKB.Padding = new Padding(3);
            this.tpSKB.Size = new Size(0x103, 0x17e);
            this.tpSKB.TabIndex = 0;
            this.tpSKB.Text = "Skybox";
            this.tpSKB.UseVisualStyleBackColor = true;
            this.DefaultSkyboxSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            this.DefaultSkyboxSelector.FormattingEnabled = true;
            this.DefaultSkyboxSelector.Location = new System.Drawing.Point(15, 0xeb);
            this.DefaultSkyboxSelector.Name = "DefaultSkyboxSelector";
            this.DefaultSkyboxSelector.Size = new Size(0x79, 0x18);
            this.DefaultSkyboxSelector.TabIndex = 11;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 0xdb);
            this.label13.Name = "label13";
            this.label13.Size = new Size(0x76, 0x11);
            this.label13.TabIndex = 10;
            this.label13.Text = "Default skybox(?)";
            this.SkyboxFloatBox.Location = new System.Drawing.Point(0x73, 0x10d);
            this.SkyboxFloatBox.Name = "SkyboxFloatBox";
            this.SkyboxFloatBox.Size = new Size(100, 0x16);
            this.SkyboxFloatBox.TabIndex = 9;
            this.SkyboxFloatBox.Visible = false;
            this.SkyboxFloatCheckbox.AutoSize = true;
            this.SkyboxFloatCheckbox.CheckAlign = ContentAlignment.MiddleRight;
            this.SkyboxFloatCheckbox.Location = new System.Drawing.Point(11, 0x10f);
            this.SkyboxFloatCheckbox.Name = "SkyboxFloatCheckbox";
            this.SkyboxFloatCheckbox.Size = new Size(0x77, 0x15);
            this.SkyboxFloatCheckbox.TabIndex = 8;
            this.SkyboxFloatCheckbox.Text = "Unknown float";
            this.SkyboxFloatCheckbox.UseVisualStyleBackColor = true;
            this.SkyboxFloatCheckbox.CheckedChanged += new EventHandler(this.SkyboxFloatCheckbox_CheckedChanged);
            this.SkyboxApplybutton.Location = new System.Drawing.Point(0x5b, 0x133);
            this.SkyboxApplybutton.Name = "SkyboxApplybutton";
            this.SkyboxApplybutton.Size = new Size(0x4b, 0x17);
            this.SkyboxApplybutton.TabIndex = 7;
            this.SkyboxApplybutton.Text = "Apply";
            this.SkyboxApplybutton.UseVisualStyleBackColor = true;
            this.SkyboxApplybutton.Click += new EventHandler(this.SkyboxApplybutton_Click);
            this.groupBox1.Controls.Add(this.SkyboxIntBox);
            this.groupBox1.Controls.Add(this.SkyboxIntCheckbox);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.Color3BoxB);
            this.groupBox1.Controls.Add(this.Color3BoxG);
            this.groupBox1.Controls.Add(this.Color3BoxR);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.Color2BoxB);
            this.groupBox1.Controls.Add(this.Color2BoxG);
            this.groupBox1.Controls.Add(this.Color2BoxR);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Color1BoxB);
            this.groupBox1.Controls.Add(this.Color1BoxG);
            this.groupBox1.Controls.Add(this.Color1BoxR);
            this.groupBox1.Controls.Add(this.Color3Box);
            this.groupBox1.Controls.Add(this.Color2Box);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Color1Box);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 0x21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0xe5, 0xb7);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.SkyboxIntBox.Location = new System.Drawing.Point(0x6f, 150);
            this.SkyboxIntBox.Name = "SkyboxIntBox";
            this.SkyboxIntBox.Size = new Size(100, 0x16);
            this.SkyboxIntBox.TabIndex = 30;
            this.SkyboxIntBox.Visible = false;
            this.SkyboxIntBox.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.SkyboxIntCheckbox.AutoSize = true;
            this.SkyboxIntCheckbox.CheckAlign = ContentAlignment.MiddleRight;
            this.SkyboxIntCheckbox.Location = new System.Drawing.Point(5, 0x99);
            this.SkyboxIntCheckbox.Name = "SkyboxIntCheckbox";
            this.SkyboxIntCheckbox.Size = new Size(0x6b, 0x15);
            this.SkyboxIntCheckbox.TabIndex = 0x1d;
            this.SkyboxIntCheckbox.Text = "Unknown int";
            this.SkyboxIntCheckbox.UseVisualStyleBackColor = true;
            this.SkyboxIntCheckbox.CheckedChanged += new EventHandler(this.SkyboxIntCheckbox_CheckedChanged);
            this.label10.Location = new System.Drawing.Point(0x3d, 0x77);
            this.label10.Name = "label10";
            this.label10.Size = new Size(8, 13);
            this.label10.TabIndex = 0x1c;
            this.label10.Text = "R";
            this.label10.TextAlign = ContentAlignment.MiddleCenter;
            this.label11.Location = new System.Drawing.Point(0x66, 0x77);
            this.label11.Name = "label11";
            this.label11.Size = new Size(9, 13);
            this.label11.TabIndex = 0x1b;
            this.label11.Text = "G";
            this.label11.TextAlign = ContentAlignment.MiddleCenter;
            this.label12.Location = new System.Drawing.Point(0x8f, 0x77);
            this.label12.Name = "label12";
            this.label12.Size = new Size(8, 13);
            this.label12.TabIndex = 0x1a;
            this.label12.Text = "B";
            this.label12.TextAlign = ContentAlignment.MiddleCenter;
            this.Color3BoxB.Location = new System.Drawing.Point(0x97, 0x74);
            this.Color3BoxB.Name = "Color3BoxB";
            this.Color3BoxB.Size = new Size(0x1a, 0x16);
            this.Color3BoxB.TabIndex = 0x19;
            this.Color3BoxB.Text = "255";
            this.Color3BoxB.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color3BoxB.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color3BoxG.Location = new System.Drawing.Point(0x6f, 0x74);
            this.Color3BoxG.Name = "Color3BoxG";
            this.Color3BoxG.Size = new Size(0x1a, 0x16);
            this.Color3BoxG.TabIndex = 0x18;
            this.Color3BoxG.Text = "255";
            this.Color3BoxG.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color3BoxG.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color3BoxR.Location = new System.Drawing.Point(70, 0x74);
            this.Color3BoxR.Name = "Color3BoxR";
            this.Color3BoxR.Size = new Size(0x1a, 0x16);
            this.Color3BoxR.TabIndex = 0x17;
            this.Color3BoxR.Text = "255";
            this.Color3BoxR.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color3BoxR.Validated += new EventHandler(this.ColorBox_Validated);
            this.label7.Location = new System.Drawing.Point(0x3d, 0x48);
            this.label7.Name = "label7";
            this.label7.Size = new Size(8, 13);
            this.label7.TabIndex = 0x16;
            this.label7.Text = "R";
            this.label7.TextAlign = ContentAlignment.MiddleCenter;
            this.label8.Location = new System.Drawing.Point(0x66, 0x48);
            this.label8.Name = "label8";
            this.label8.Size = new Size(9, 13);
            this.label8.TabIndex = 0x15;
            this.label8.Text = "G";
            this.label8.TextAlign = ContentAlignment.MiddleCenter;
            this.label9.Location = new System.Drawing.Point(0x8f, 0x48);
            this.label9.Name = "label9";
            this.label9.Size = new Size(8, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "B";
            this.label9.TextAlign = ContentAlignment.MiddleCenter;
            this.Color2BoxB.Location = new System.Drawing.Point(0x97, 0x45);
            this.Color2BoxB.Name = "Color2BoxB";
            this.Color2BoxB.Size = new Size(0x1a, 0x16);
            this.Color2BoxB.TabIndex = 0x13;
            this.Color2BoxB.Text = "255";
            this.Color2BoxB.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color2BoxB.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color2BoxG.Location = new System.Drawing.Point(0x6f, 0x45);
            this.Color2BoxG.Name = "Color2BoxG";
            this.Color2BoxG.Size = new Size(0x1a, 0x16);
            this.Color2BoxG.TabIndex = 0x12;
            this.Color2BoxG.Text = "255";
            this.Color2BoxG.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color2BoxG.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color2BoxR.Location = new System.Drawing.Point(70, 0x45);
            this.Color2BoxR.Name = "Color2BoxR";
            this.Color2BoxR.Size = new Size(0x1a, 0x16);
            this.Color2BoxR.TabIndex = 0x11;
            this.Color2BoxR.Text = "255";
            this.Color2BoxR.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color2BoxR.Validated += new EventHandler(this.ColorBox_Validated);
            this.label5.Location = new System.Drawing.Point(0x3d, 0x19);
            this.label5.Name = "label5";
            this.label5.Size = new Size(8, 13);
            this.label5.TabIndex = 0x10;
            this.label5.Text = "R";
            this.label5.TextAlign = ContentAlignment.MiddleCenter;
            this.label4.Location = new System.Drawing.Point(0x66, 0x19);
            this.label4.Name = "label4";
            this.label4.Size = new Size(9, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "G";
            this.label4.TextAlign = ContentAlignment.MiddleCenter;
            this.label6.Location = new System.Drawing.Point(0x8f, 0x19);
            this.label6.Name = "label6";
            this.label6.Size = new Size(8, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "B";
            this.label6.TextAlign = ContentAlignment.MiddleCenter;
            this.Color1BoxB.Location = new System.Drawing.Point(0x97, 0x16);
            this.Color1BoxB.Name = "Color1BoxB";
            this.Color1BoxB.Size = new Size(0x1a, 0x16);
            this.Color1BoxB.TabIndex = 11;
            this.Color1BoxB.Text = "255";
            this.Color1BoxB.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color1BoxB.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color1BoxG.Location = new System.Drawing.Point(0x6f, 0x16);
            this.Color1BoxG.Name = "Color1BoxG";
            this.Color1BoxG.Size = new Size(0x1a, 0x16);
            this.Color1BoxG.TabIndex = 10;
            this.Color1BoxG.Text = "255";
            this.Color1BoxG.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color1BoxG.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color1BoxR.Location = new System.Drawing.Point(70, 0x16);
            this.Color1BoxR.Name = "Color1BoxR";
            this.Color1BoxR.Size = new Size(0x1a, 0x16);
            this.Color1BoxR.TabIndex = 9;
            this.Color1BoxR.Text = "255";
            this.Color1BoxR.KeyDown += new KeyEventHandler(this.ColorBox_KeyDown);
            this.Color1BoxR.Validated += new EventHandler(this.ColorBox_Validated);
            this.Color3Box.BorderStyle = BorderStyle.Fixed3D;
            this.Color3Box.Location = new System.Drawing.Point(0xbf, 0x70);
            this.Color3Box.Name = "Color3Box";
            this.Color3Box.Size = new Size(0x20, 0x20);
            this.Color3Box.TabIndex = 8;
            this.Color3Box.TabStop = false;
            this.Color3Box.Click += new EventHandler(this.ColorBox_Click);
            this.Color2Box.BorderStyle = BorderStyle.Fixed3D;
            this.Color2Box.Location = new System.Drawing.Point(0xbf, 0x40);
            this.Color2Box.Name = "Color2Box";
            this.Color2Box.Size = new Size(0x20, 0x20);
            this.Color2Box.TabIndex = 6;
            this.Color2Box.TabStop = false;
            this.Color2Box.Click += new EventHandler(this.ColorBox_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 0x19);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x35, 0x11);
            this.label1.TabIndex = 0;
            this.label1.Text = "Color 1";
            this.Color1Box.BorderStyle = BorderStyle.Fixed3D;
            this.Color1Box.Location = new System.Drawing.Point(0xbf, 0x10);
            this.Color1Box.Name = "Color1Box";
            this.Color1Box.Size = new Size(0x20, 0x20);
            this.Color1Box.TabIndex = 4;
            this.Color1Box.TabStop = false;
            this.Color1Box.Click += new EventHandler(this.ColorBox_Click);
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 0x77);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x35, 0x11);
            this.label3.TabIndex = 2;
            this.label3.Text = "Color 3";
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 0x48);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x35, 0x11);
            this.label2.TabIndex = 1;
            this.label2.Text = "Color 2";
            this.SkyboxSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            this.SkyboxSelector.FormattingEnabled = true;
            this.SkyboxSelector.Location = new System.Drawing.Point(0x44, 6);
            this.SkyboxSelector.Name = "SkyboxSelector";
            this.SkyboxSelector.Size = new Size(0x79, 0x18);
            this.SkyboxSelector.TabIndex = 5;
            this.SkyboxSelector.SelectedIndexChanged += new EventHandler(this.SkyboxSelector_SelectedIndexChanged);
            this.tpPWB.AutoScroll = true;
            this.tpPWB.Controls.Add(this.button3);
            this.tpPWB.Controls.Add(this.label31);
            this.tpPWB.Controls.Add(this.label30);
            this.tpPWB.Controls.Add(this.label29);
            this.tpPWB.Controls.Add(this.label28);
            this.tpPWB.Controls.Add(this.BrickZtextBox);
            this.tpPWB.Controls.Add(this.BrickYtextBox);
            this.tpPWB.Controls.Add(this.BrickXtextBox);
            this.tpPWB.Controls.Add(this.button2);
            this.tpPWB.Controls.Add(this.BrickColorComboBox);
            this.tpPWB.Controls.Add(this.BrickDeleteButton);
            this.tpPWB.Controls.Add(this.PWBListBox);
            this.tpPWB.Location = new System.Drawing.Point(4, 0x19);
            this.tpPWB.Name = "tpPWB";
            this.tpPWB.Padding = new Padding(3);
            this.tpPWB.Size = new Size(0x103, 0x17e);
            this.tpPWB.TabIndex = 1;
            this.tpPWB.Text = "Powerups";
            this.tpPWB.UseVisualStyleBackColor = true;
            this.button3.Location = new System.Drawing.Point(0x9f, 0x129);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x4b, 0x24);
            this.button3.TabIndex = 11;
            this.button3.Text = "Update location";
            this.button3.UseVisualStyleBackColor = true;
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(3, 0xfe);
            this.label31.Name = "label31";
            this.label31.Size = new Size(0x2d, 0x11);
            this.label31.TabIndex = 10;
            this.label31.Text = "Color:";
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(20, 0x14f);
            this.label30.Name = "label30";
            this.label30.Size = new Size(0x15, 0x11);
            this.label30.TabIndex = 9;
            this.label30.Text = "Z:";
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(20, 0x135);
            this.label29.Name = "label29";
            this.label29.Size = new Size(0x15, 0x11);
            this.label29.TabIndex = 8;
            this.label29.Text = "Y:";
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(20, 0x11b);
            this.label28.Name = "label28";
            this.label28.Size = new Size(0x15, 0x11);
            this.label28.TabIndex = 7;
            this.label28.Text = "X:";
            this.BrickZtextBox.Location = new System.Drawing.Point(0x2b, 0x14c);
            this.BrickZtextBox.Name = "BrickZtextBox";
            this.BrickZtextBox.Size = new Size(100, 0x16);
            this.BrickZtextBox.TabIndex = 6;
            this.BrickZtextBox.Validated += new EventHandler(this.BricktextBox_Validated);
            this.BrickYtextBox.Location = new System.Drawing.Point(0x2b, 0x132);
            this.BrickYtextBox.Name = "BrickYtextBox";
            this.BrickYtextBox.Size = new Size(100, 0x16);
            this.BrickYtextBox.TabIndex = 5;
            this.BrickYtextBox.Validated += new EventHandler(this.BricktextBox_Validated);
            this.BrickXtextBox.Location = new System.Drawing.Point(0x2b, 280);
            this.BrickXtextBox.Name = "BrickXtextBox";
            this.BrickXtextBox.Size = new Size(100, 0x16);
            this.BrickXtextBox.TabIndex = 4;
            this.BrickXtextBox.Validated += new EventHandler(this.BricktextBox_Validated);
            this.button2.Location = new System.Drawing.Point(0x8f, 0x164);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x5c, 0x17);
            this.button2.TabIndex = 3;
            this.button2.Text = "Add new brick";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.BrickColorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.BrickColorComboBox.FormattingEnabled = true;
            object[] brickColorItems = new object[] { "White", "Red", "Blue", "Green", "Yellow" };
            this.BrickColorComboBox.Items.AddRange(brickColorItems);
            this.BrickColorComboBox.Location = new System.Drawing.Point(0x2b, 250);
            this.BrickColorComboBox.Name = "BrickColorComboBox";
            this.BrickColorComboBox.Size = new Size(100, 0x18);
            this.BrickColorComboBox.TabIndex = 2;
            this.BrickColorComboBox.SelectionChangeCommitted += new EventHandler(this.BrickColorComboBox_SelectionChangeCommitted);
            this.BrickDeleteButton.Location = new System.Drawing.Point(0x16, 0x164);
            this.BrickDeleteButton.Name = "BrickDeleteButton";
            this.BrickDeleteButton.Size = new Size(0x5c, 0x17);
            this.BrickDeleteButton.TabIndex = 1;
            this.BrickDeleteButton.Text = "Delete selected";
            this.BrickDeleteButton.UseVisualStyleBackColor = true;
            this.BrickDeleteButton.Click += new EventHandler(this.BrickDeleteButton_Click);
            this.PWBListBox.Enabled = false;
            this.PWBListBox.FormattingEnabled = true;
            this.PWBListBox.ItemHeight = 0x10;
            object[] pwbItems = new object[] { "No PWB Loaded" };
            this.PWBListBox.Items.AddRange(pwbItems);
            this.PWBListBox.Location = new System.Drawing.Point(6, 6);
            this.PWBListBox.Name = "PWBListBox";
            this.PWBListBox.SelectionMode = SelectionMode.MultiExtended;
            this.PWBListBox.Size = new Size(0xe4, 0xd4);
            this.PWBListBox.TabIndex = 0;
            this.PWBListBox.SelectedIndexChanged += new EventHandler(this.PWBListBox_SelectedIndexChanged);
            this.PWBListBox.KeyDown += new KeyEventHandler(this.PWBListBox_KeyDown);
            this.tabPageRRB.AutoScroll = true;
            this.tabPageRRB.Controls.Add(this.rightclickmelabel);
            this.tabPageRRB.Controls.Add(this.RRBEditCheckBox);
            this.tabPageRRB.Controls.Add(this.RRBListView);
            this.tabPageRRB.Controls.Add(this.RRBNodeListBox);
            this.tabPageRRB.Controls.Add(this.label33);
            this.tabPageRRB.Controls.Add(this.label32);
            this.tabPageRRB.Location = new System.Drawing.Point(4, 0x19);
            this.tabPageRRB.Name = "tabPageRRB";
            this.tabPageRRB.Size = new Size(0x103, 0x17e);
            this.tabPageRRB.TabIndex = 4;
            this.tabPageRRB.Text = "AI Paths";
            this.tabPageRRB.UseVisualStyleBackColor = true;
            this.rightclickmelabel.AutoSize = true;
            this.rightclickmelabel.ForeColor = SystemColors.GrayText;
            this.rightclickmelabel.Location = new System.Drawing.Point(0x4c, 0x41);
            this.rightclickmelabel.Name = "rightclickmelabel";
            this.rightclickmelabel.Size = new Size(0x60, 0x11);
            this.rightclickmelabel.TabIndex = 6;
            this.rightclickmelabel.Text = "Right-click me";
            this.rightclickmelabel.MouseClick += new MouseEventHandler(this.rightclickmelabel_MouseClick);
            this.RRBEditCheckBox.AutoSize = true;
            this.RRBEditCheckBox.Location = new System.Drawing.Point(6, 140);
            this.RRBEditCheckBox.Name = "RRBEditCheckBox";
            this.RRBEditCheckBox.Size = new Size(0x36, 0x15);
            this.RRBEditCheckBox.TabIndex = 5;
            this.RRBEditCheckBox.Text = "Edit";
            this.RRBEditCheckBox.UseVisualStyleBackColor = true;
            this.RRBListView.CheckBoxes = true;
            ColumnHeader[] values = new ColumnHeader[] { this.columnHeader1 };
            this.RRBListView.Columns.AddRange(values);
            this.RRBListView.ContextMenuStrip = this.RRBListViewContextMenu;
            this.RRBListView.HideSelection = false;
            this.RRBListView.LabelWrap = false;
            this.RRBListView.Location = new System.Drawing.Point(6, 0x16);
            this.RRBListView.MultiSelect = false;
            this.RRBListView.Name = "RRBListView";
            this.RRBListView.Size = new Size(0xe4, 0x70);
            this.RRBListView.SmallImageList = this.imageList1;
            this.RRBListView.TabIndex = 4;
            this.RRBListView.UseCompatibleStateImageBehavior = false;
            this.RRBListView.View = View.SmallIcon;
            this.RRBListView.ItemChecked += new ItemCheckedEventHandler(this.RRBListView_ItemChecked);
            this.columnHeader1.Width = 0x4d;
            this.RRBListViewContextMenu.ImageScalingSize = new Size(20, 20);
            ToolStripItem[] contextMenuItems = new ToolStripItem[] { this.addNewContextMenuItem, this.editColorContextMenuItem, this.saveContextMenuItem, this.unloadSelectedContextMenuItem, this.tsmiUnloadAll };
            this.RRBListViewContextMenu.Items.AddRange(contextMenuItems);
            this.RRBListViewContextMenu.Name = "RRBListBoxContextMenu";
            this.RRBListViewContextMenu.ShowImageMargin = false;
            this.RRBListViewContextMenu.Size = new Size(0xa1, 0x7c);
            this.RRBListViewContextMenu.Opening += new CancelEventHandler(this.RRBListViewContextMenu_Opening);
            this.addNewContextMenuItem.Name = "addNewContextMenuItem";
            this.addNewContextMenuItem.Size = new Size(160, 0x18);
            this.addNewContextMenuItem.Text = "Add new...";
            this.addNewContextMenuItem.Click += new EventHandler(this.addNewContextMenuItem_Click);
            this.editColorContextMenuItem.Name = "editColorContextMenuItem";
            this.editColorContextMenuItem.Size = new Size(160, 0x18);
            this.editColorContextMenuItem.Text = "Edit color...";
            this.editColorContextMenuItem.Click += new EventHandler(this.editColorContextMenuItem_Click);
            this.saveContextMenuItem.Name = "saveContextMenuItem";
            this.saveContextMenuItem.Size = new Size(160, 0x18);
            this.saveContextMenuItem.Text = "Save selected";
            this.saveContextMenuItem.Click += new EventHandler(this.saveContextMenuItem_Click);
            this.unloadSelectedContextMenuItem.Name = "unloadSelectedContextMenuItem";
            this.unloadSelectedContextMenuItem.Size = new Size(160, 0x18);
            this.unloadSelectedContextMenuItem.Text = "Unload selected";
            this.unloadSelectedContextMenuItem.Click += new EventHandler(this.unloadSelectedContextMenuItem_Click);
            this.tsmiUnloadAll.Name = "tsmiUnloadAll";
            this.tsmiUnloadAll.Size = new Size(160, 0x18);
            this.tsmiUnloadAll.Text = "Unload all";
            this.tsmiUnloadAll.Click += new EventHandler(this.tsmiUnloadAll_Click);
            this.imageList1.ColorDepth = ColorDepth.Depth24Bit;
            this.imageList1.ImageSize = new Size(0x10, 0x10);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.RRBNodeListBox.FormattingEnabled = true;
            this.RRBNodeListBox.ItemHeight = 0x10;
            this.RRBNodeListBox.Location = new System.Drawing.Point(6, 0xb8);
            this.RRBNodeListBox.Name = "RRBNodeListBox";
            this.RRBNodeListBox.Size = new Size(0xe4, 0xa4);
            this.RRBNodeListBox.TabIndex = 3;
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(4, 0xa4);
            this.label33.Name = "label33";
            this.label33.Size = new Size(0xa8, 0x11);
            this.label33.TabIndex = 2;
            this.label33.Text = "Selected AI path's nodes:";
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(4, 6);
            this.label32.Name = "label32";
            this.label32.Size = new Size(0x73, 0x11);
            this.label32.TabIndex = 1;
            this.label32.Text = "Loaded AI paths:";
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Location = new System.Drawing.Point(4, 0x19);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new Padding(3);
            this.tabPage3.Size = new Size(0x103, 0x17e);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Static objects";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.Controls.Add(this.StaticObjectStatusLabel);
            this.tabPage3.Controls.Add(this.StaticObjectListBox);
            this.tabPage3.Controls.Add(this.StaticObjectLabel);
            this.StaticObjectLabel.AutoSize = true;
            this.StaticObjectLabel.Location = new System.Drawing.Point(6, 6);
            this.StaticObjectLabel.Name = "StaticObjectLabel";
            this.StaticObjectLabel.Size = new Size(95, 0x11);
            this.StaticObjectLabel.TabIndex = 0;
            this.StaticObjectLabel.Text = "Loaded static objects:";
            this.StaticObjectListBox.FormattingEnabled = true;
            this.StaticObjectListBox.ItemHeight = 0x10;
            this.StaticObjectListBox.Location = new System.Drawing.Point(6, 0x16);
            this.StaticObjectListBox.Name = "StaticObjectListBox";
            this.StaticObjectListBox.Size = new Size(0xe4, 0x10a);
            this.StaticObjectListBox.TabIndex = 1;
            this.StaticObjectStatusLabel.Location = new System.Drawing.Point(6, 0x125);
            this.StaticObjectStatusLabel.Name = "StaticObjectStatusLabel";
            this.StaticObjectStatusLabel.Size = new Size(0xe4, 0x55);
            this.StaticObjectStatusLabel.TabIndex = 2;
            this.StaticObjectStatusLabel.Text = "No static objects loaded.";
            this.tabPage4.Location = new System.Drawing.Point(4, 0x19);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new Padding(3);
            this.tabPage4.Size = new Size(0x103, 0x17e);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Animated Objects";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.Controls.Add(this.AnimatedObjectStatusLabel);
            this.tabPage4.Controls.Add(this.AnimatedObjectRunButton);
            this.tabPage4.Controls.Add(this.AnimatedObjectLoopCheckBox);
            this.tabPage4.Controls.Add(this.AnimatedObjectAnimationComboBox);
            this.tabPage4.Controls.Add(this.AnimatedObjectAnimationLabel);
            this.tabPage4.Controls.Add(this.AnimatedObjectListBox);
            this.tabPage4.Controls.Add(this.AnimatedObjectLabel);
            this.AnimatedObjectLabel.AutoSize = true;
            this.AnimatedObjectLabel.Location = new System.Drawing.Point(6, 6);
            this.AnimatedObjectLabel.Name = "AnimatedObjectLabel";
            this.AnimatedObjectLabel.Size = new Size(110, 0x11);
            this.AnimatedObjectLabel.TabIndex = 0;
            this.AnimatedObjectLabel.Text = "Loaded animated objects:";
            this.AnimatedObjectListBox.FormattingEnabled = true;
            this.AnimatedObjectListBox.ItemHeight = 0x10;
            this.AnimatedObjectListBox.Location = new System.Drawing.Point(6, 0x16);
            this.AnimatedObjectListBox.Name = "AnimatedObjectListBox";
            this.AnimatedObjectListBox.Size = new Size(0xe4, 0xb0);
            this.AnimatedObjectListBox.TabIndex = 1;
            this.AnimatedObjectListBox.SelectedIndexChanged += new EventHandler(this.AnimatedObjectListBox_SelectedIndexChanged);
            this.AnimatedObjectAnimationLabel.AutoSize = true;
            this.AnimatedObjectAnimationLabel.Location = new System.Drawing.Point(6, 0xcc);
            this.AnimatedObjectAnimationLabel.Name = "AnimatedObjectAnimationLabel";
            this.AnimatedObjectAnimationLabel.Size = new Size(0x40, 0x11);
            this.AnimatedObjectAnimationLabel.TabIndex = 2;
            this.AnimatedObjectAnimationLabel.Text = "Animation:";
            this.AnimatedObjectAnimationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.AnimatedObjectAnimationComboBox.FormattingEnabled = true;
            this.AnimatedObjectAnimationComboBox.Location = new System.Drawing.Point(9, 0xdf);
            this.AnimatedObjectAnimationComboBox.Name = "AnimatedObjectAnimationComboBox";
            this.AnimatedObjectAnimationComboBox.Size = new Size(0xe1, 0x18);
            this.AnimatedObjectAnimationComboBox.TabIndex = 3;
            this.AnimatedObjectLoopCheckBox.AutoSize = true;
            this.AnimatedObjectLoopCheckBox.Location = new System.Drawing.Point(9, 0x104);
            this.AnimatedObjectLoopCheckBox.Name = "AnimatedObjectLoopCheckBox";
            this.AnimatedObjectLoopCheckBox.Size = new Size(0x37, 0x15);
            this.AnimatedObjectLoopCheckBox.TabIndex = 4;
            this.AnimatedObjectLoopCheckBox.Text = "Loop";
            this.AnimatedObjectLoopCheckBox.UseVisualStyleBackColor = true;
            this.AnimatedObjectRunButton.Location = new System.Drawing.Point(9, 0x11f);
            this.AnimatedObjectRunButton.Name = "AnimatedObjectRunButton";
            this.AnimatedObjectRunButton.Size = new Size(0x63, 0x17);
            this.AnimatedObjectRunButton.TabIndex = 5;
            this.AnimatedObjectRunButton.Text = "Run animation";
            this.AnimatedObjectRunButton.UseVisualStyleBackColor = true;
            this.AnimatedObjectRunButton.Click += new EventHandler(this.AnimatedObjectRunButton_Click);
            this.AnimatedObjectStatusLabel.Location = new System.Drawing.Point(6, 0x13d);
            this.AnimatedObjectStatusLabel.Name = "AnimatedObjectStatusLabel";
            this.AnimatedObjectStatusLabel.Size = new Size(0xe4, 0x3d);
            this.AnimatedObjectStatusLabel.TabIndex = 6;
            this.AnimatedObjectStatusLabel.Text = "No animated objects loaded.";
            this.tabPage1.Controls.Add(this.DebugButton2);
            this.tabPage1.Controls.Add(this.DebugButton1);
            this.tabPage1.Controls.Add(this.DebugListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 0x19);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(0x103, 0x17e);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "Debug";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.DebugButton2.Location = new System.Drawing.Point(0xb0, 0xf6);
            this.DebugButton2.Name = "DebugButton2";
            this.DebugButton2.Size = new Size(0x4b, 0x23);
            this.DebugButton2.TabIndex = 2;
            this.DebugButton2.Text = "Update visibility";
            this.DebugButton2.UseVisualStyleBackColor = true;
            this.DebugButton2.Click += new EventHandler(this.DebugButton2_Click);
            this.DebugButton1.Location = new System.Drawing.Point(15, 0xf6);
            this.DebugButton1.Name = "DebugButton1";
            this.DebugButton1.Size = new Size(0x4b, 0x17);
            this.DebugButton1.TabIndex = 1;
            this.DebugButton1.Text = "Load";
            this.DebugButton1.UseVisualStyleBackColor = true;
            this.DebugButton1.Click += new EventHandler(this.DebugButton1_Click);
            this.DebugListView.CheckBoxes = true;
            this.DebugListView.Location = new System.Drawing.Point(15, 12);
            this.DebugListView.Name = "DebugListView";
            this.DebugListView.Size = new Size(0xec, 0xe4);
            this.DebugListView.TabIndex = 0;
            this.DebugListView.UseCompatibleStateImageBehavior = false;
            this.DebugListView.View = View.List;
            this.BrickplaceStopButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.BrickplaceStopButton.Location = new System.Drawing.Point(0x38a, 0xb3);
            this.BrickplaceStopButton.Name = "BrickplaceStopButton";
            this.BrickplaceStopButton.Size = new Size(0x4b, 0x24);
            this.BrickplaceStopButton.TabIndex = 12;
            this.BrickplaceStopButton.Text = "Abort brick placement";
            this.BrickplaceStopButton.UseVisualStyleBackColor = true;
            this.BrickplaceStopButton.Visible = false;
            this.BrickplaceStopButton.Click += new EventHandler(this.BrickplaceStopButton_Click);
            this.Brickplacelabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.Brickplacelabel.AutoSize = true;
            this.Brickplacelabel.BackColor = SystemColors.Window;
            this.Brickplacelabel.Location = new System.Drawing.Point(880, 160);
            this.Brickplacelabel.Name = "Brickplacelabel";
            this.Brickplacelabel.Size = new Size(0xac, 0x11);
            this.Brickplacelabel.TabIndex = 11;
            this.Brickplacelabel.Text = "Currently placing a brick...";
            this.Brickplacelabel.Visible = false;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new Size(0x40, 0x11);
            this.label14.TabIndex = 4;
            this.label14.Text = "Controls:";
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(0, 0x0B);
            this.label15.Name = "label15";
            this.label15.Size = new Size(0x9f, 0x11);
            this.label15.TabIndex = 5;
            this.label15.Text = "W,A,S,D - Move camera";
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(0, 0x23);
            this.label16.Name = "label16";
            this.label16.Size = new Size(210, 0x11);
            this.label16.TabIndex = 6;
            this.label16.Text = "Space, Lshift - Camera up-down";
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(0, 0x3B);
            this.label17.Name = "label17";
            this.label17.Size = new Size(0xec, 0x11);
            this.label17.TabIndex = 7;
            this.label17.Text = "Right mouse (drag) - Rotate camera";
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(0, 0x17);
            this.label19.Name = "label19";
            this.label19.Size = new Size(0x85, 0x11);
            this.label19.TabIndex = 9;
            this.label19.Text = "Esc - Unlock mouse";
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(0, 0x2F);
            this.label20.Name = "label20";
            this.label20.Size = new Size(0x7d, 0x11);
            this.label20.TabIndex = 10;
            this.label20.Text = "Del - Delete object";
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(0, 0x47);
            this.label21.Name = "label21";
            this.label21.Size = new Size(0xde, 0x11);
            this.label21.TabIndex = 11;
            this.label21.Text = "Mouse scroll - Change cam speed";
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(0, 0x6B);
            this.label22.Name = "label22";
            this.label22.Size = new Size(0x80, 0x11);
            this.label22.TabIndex = 12;
            this.label22.Text = "T - Toggle textures";
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(0, 0x53);
            this.label23.Name = "label23";
            this.label23.Size = new Size(0xa1, 0x11);
            this.label23.TabIndex = 13;
            this.label23.Text = "R - Reset view to (0,0,0)";
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(0, 0x5F);
            this.label24.Name = "label24";
            this.label24.Size = new Size(0xbc, 0x11);
            this.label24.TabIndex = 14;
            this.label24.Text = "Alt+Enter - Toggle fullscreen";
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(0, 0x77);
            this.label25.Name = "label25";
            this.label25.Size = new Size(0x89, 0x11);
            this.label25.TabIndex = 15;
            this.label25.Text = "F1 - Toggle Fillmode";
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(0, 0x83);
            this.label26.Name = "label26";
            this.label26.Size = new Size(0x8f, 0x11);
            this.label26.TabIndex = 0x10;
            this.label26.Text = "F2 - Toggle Cullmode";
            this.panel1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label25);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Location = new System.Drawing.Point(0x331, 0x1D2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x10b, 0x90);
            this.panel1.TabIndex = 0x11;
            this.label27.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(0x331, 0x1B7);
            this.label27.Name = "label27";
            this.label27.Size = new Size(0x58, 0x11);
            this.label27.TabIndex = 0x12;
            this.label27.Text = "Camera pos:";
            this.CameraPositionLabel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.CameraPositionLabel.AutoSize = true;
            this.CameraPositionLabel.Location = new System.Drawing.Point(0x38a, 0x1B7);
            this.CameraPositionLabel.Name = "CameraPositionLabel";
            this.CameraPositionLabel.Size = new Size(0x3a, 0x11);
            this.CameraPositionLabel.TabIndex = 0x13;
            this.CameraPositionLabel.Text = "{0, 0, 0}";
            this.CameraSpeedHeaderLabel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.CameraSpeedHeaderLabel.AutoSize = true;
            this.CameraSpeedHeaderLabel.Location = new System.Drawing.Point(0x331, 0x1C4);
            this.CameraSpeedHeaderLabel.Name = "CameraSpeedHeaderLabel";
            this.CameraSpeedHeaderLabel.TabIndex = 0x14;
            this.CameraSpeedHeaderLabel.Text = "Cam speed:";
            this.CameraSpeedLabel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.CameraSpeedLabel.AutoSize = true;
            this.CameraSpeedLabel.Location = new System.Drawing.Point(0x38a, 0x1C4);
            this.CameraSpeedLabel.Name = "CameraSpeedLabel";
            this.CameraSpeedLabel.TabIndex = 0x15;
            this.CameraSpeedLabel.Text = "0";
            this.AllowDrop = true;
            base.AutoScaleMode = AutoScaleMode.None;
            base.ClientSize = new Size(0x43c, 0x27d);
            base.Controls.Add(this.Brickplacelabel);
            base.Controls.Add(this.BrickplaceStopButton);
            base.Controls.Add(this.CameraSpeedLabel);
            base.Controls.Add(this.CameraSpeedHeaderLabel);
            base.Controls.Add(this.CameraPositionLabel);
            base.Controls.Add(this.label27);
            base.Controls.Add(this.panel1);
            base.Controls.Add(this.tabControl1);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.menuStrip1);
            base.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new Size(600, 300);
            base.Name = "FormEditor";
            this.Text = "Track Editor";
            base.FormClosing += new FormClosingEventHandler(this.FormEditor_FormClosing);
            base.ResizeEnd += new EventHandler(this.FormEditor_ResizeEnd);
            base.DragEnter += new DragEventHandler(this.FormEditor_DragEnter);
            base.Resize += new EventHandler(this.FormEditor_Resize);
            ((ISupportInitialize)this.pictureBox1).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tpSKB.ResumeLayout(false);
            this.tpSKB.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((ISupportInitialize)this.Color3Box).EndInit();
            ((ISupportInitialize)this.Color2Box).EndInit();
            ((ISupportInitialize)this.Color1Box).EndInit();
            this.tpPWB.ResumeLayout(false);
            this.tpPWB.PerformLayout();
            this.tabPageRRB.ResumeLayout(false);
            this.tabPageRRB.PerformLayout();
            this.RRBListViewContextMenu.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadColorBoxes()
        {
            int red = int.Parse(this.Color1BoxR.Text);
            this.Color1Box.BackColor = System.Drawing.Color.FromArgb(red, int.Parse(this.Color1BoxG.Text), int.Parse(this.Color1BoxB.Text));
            red = int.Parse(this.Color2BoxR.Text);
            this.Color2Box.BackColor = System.Drawing.Color.FromArgb(red, int.Parse(this.Color2BoxG.Text), int.Parse(this.Color2BoxB.Text));
            red = int.Parse(this.Color3BoxR.Text);
            this.Color3Box.BackColor = System.Drawing.Color.FromArgb(red, int.Parse(this.Color3BoxG.Text), int.Parse(this.Color3BoxB.Text));
        }

        private void LoadColorInputBoxes()
        {
            System.Drawing.Color backColor = this.Color1Box.BackColor;
            System.Drawing.Color color2 = this.Color2Box.BackColor;
            System.Drawing.Color color3 = this.Color3Box.BackColor;
            this.Color1BoxR.Text = backColor.R.ToString();
            this.Color1BoxG.Text = backColor.G.ToString();
            this.Color1BoxB.Text = backColor.B.ToString();
            this.Color2BoxR.Text = color2.R.ToString();
            this.Color2BoxG.Text = color2.G.ToString();
            this.Color2BoxB.Text = color2.B.ToString();
            this.Color3BoxR.Text = color3.R.ToString();
            this.Color3BoxG.Text = color3.G.ToString();
            this.Color3BoxB.Text = color3.B.ToString();
        }

        private void LoadGradient(SKB_Gradient grad)
        {
            this.Color1Box.BackColor = System.Drawing.Color.FromArgb(grad.Color3.R, grad.Color3.G, grad.Color3.B);
            this.Color2Box.BackColor = System.Drawing.Color.FromArgb(grad.Color2.R, grad.Color2.G, grad.Color2.B);
            this.Color3Box.BackColor = System.Drawing.Color.FromArgb(grad.Color1.R, grad.Color1.G, grad.Color1.B);
            if (grad.UnknownInt == null)
            {
                this.SkyboxIntCheckbox.Checked = false;
            }
            else
            {
                this.SkyboxIntCheckbox.Checked = true;
                this.SkyboxIntBox.Text = grad.UnknownInt.Value.ToString();
            }
            this.LoadColorInputBoxes();
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            if (this.OpenWarning())
            {
                Utils.OpenFileDialog(1);
            }
        }

        public bool OpenWarning() =>
            (this.edits.Count <= 0) || (System.Windows.Forms.MessageBox.Show("You have made changes in: " + string.Join(", ", this.edits) + "\n Do you want to discard these changes and load a new file anyway?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes);

        public void ApplyRuntimeSettings()
        {
            this.game.RefreshProjectionSettings();
        }

        private void tsmiOptions_Click(object sender, EventArgs e)
        {
            OptionsForm objA = new OptionsForm(this);
            try
            {
                objA.ShowDialog();
            }
            finally
            {
                if (objA is object)
                {
                    objA.Dispose();
                }
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (!(this.pictureBox1.Focused || !base.ContainsFocus))
            {
                this.pictureBox1.Focus();
                this.SelectBricks(this.game.SelectedBrickIndices.ToArray(), this.game.SelectedBricksColored.ToArray());
            }
        }

        private void PWBListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
            {
                int index = 0;
                while (true)
                {
                    if (index >= this.PWBListBox.Items.Count)
                    {
                        break;
                    }
                    this.PWBListBox.SelectedIndices.Add(index);
                    index++;
                }
            }
        }

        private void PWBListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Brick current;
            bool flag2;
            IDisposable disposable;
            List<int> list = new List<int>();
            List<bool> list2 = new List<bool>();
            IEnumerator enumerator = this.PWBListBox.SelectedItems.GetEnumerator();
            try
            {
                while (true)
                {
                    flag2 = enumerator.MoveNext();
                    if (!flag2)
                    {
                        break;
                    }
                    current = (Brick)enumerator.Current;
                    list.Add(current.index);
                    list2.Add(current.colored);
                }
            }
            finally
            {
                disposable = enumerator as IDisposable;
                if (disposable is object)
                {
                    disposable.Dispose();
                }
            }
            this.game.SelectedBrickIndices = list;
            this.game.SelectedBricksColored = list2;
            this.RefreshSelectedBrickFields();
        }

        private void PWBSaveAs()
        {
            if (this.game.pwb is null)
            {
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Powerups|*.PWB",
                FileName = "POWERUP.PWB",
                AddExtension = true,
                OverwritePrompt = true
            };
            if (!string.IsNullOrWhiteSpace(this.game.currentPWBfile))
            {
                string initialDirectory = Path.GetDirectoryName(this.game.currentPWBfile);
                if (!string.IsNullOrWhiteSpace(initialDirectory) && Directory.Exists(initialDirectory))
                {
                    dialog.InitialDirectory = initialDirectory;
                }
                dialog.FileName = Path.GetFileName(this.game.currentPWBfile);
            }

            if (Utils.STAShowDialog(dialog) == DialogResult.OK)
            {
                if (File.Exists(dialog.FileName))
                {
                    File.Delete(dialog.FileName);
                }
                this.game.pwb.Save(dialog.FileName);
                this.game.currentPWBfile = dialog.FileName;
                Utils.WriteLine("Saved PWB " + dialog.FileName, ConsoleColor.White);
                this.edits.Remove("PWB");
            }
        }

        public void refreshPWB(bool reselect = false)
        {
            if (this.game.pwb is null)
            {
                this.PWBListBox.Items.Clear();
                this.PWBListBox.Items.Add("No PWB Loaded");
                this.PWBListBox.Enabled = false;
            }
            else
            {
                int[] destination = new int[this.PWBListBox.SelectedIndices.Count];
                this.PWBListBox.SelectedIndices.CopyTo(destination, 0);
                this.PWBListBox.Items.Clear();
                this.PWBListBox.Enabled = true;
                int num = 0;
                while (true)
                {
                    Brick brick;
                    bool flag = num < this.game.pwb.ColorBricks.Count;
                    if (!flag)
                    {
                        num = 0;
                        while (true)
                        {
                            flag = num < this.game.pwb.WhiteBricks.Count;
                            if (!flag)
                            {
                                if (reselect)
                                {
                                    int[] numArray2 = destination;
                                    int index = 0;
                                    while (true)
                                    {
                                        flag = index < numArray2.Length;
                                        if (!flag)
                                        {
                                            break;
                                        }
                                        int num2 = numArray2[index];
                                        this.PWBListBox.SelectedIndices.Add(num2);
                                        index++;
                                    }
                                }
                                break;
                            }
                            brick = new Brick
                            {
                                Position = this.game.pwb.WhiteBricks[num].Position,
                                colored = false,
                                Color = "White",
                                index = num
                            };
                            this.PWBListBox.Items.Add(brick);
                            num++;
                        }
                        break;
                    }
                    brick = new Brick
                    {
                        Position = this.game.pwb.ColorBricks[num].Position,
                        colored = true,
                        Color = this.game.pwb.ColorBricks[num].ColorString(),
                        index = num
                    };
                    this.PWBListBox.Items.Add(brick);
                    num++;
                }
            }
        }

        public void refreshRRB()
        {
            this.RRBListView.Items.Clear();
            this.imageList1.Images.Clear();
            int num = 0;
            while (num < this.game.rrbs.Count)
            {
                ListViewItem item = new ListViewItem(this.game.rrbs[num].getname())
                {
                    Tag = num
                };
                if (this.game.rrbs[num].display)
                {
                    item.Checked = true;
                }
                Bitmap bitmap = new Bitmap(0x10, 0x10, PixelFormat.Format24bppRgb);
                Microsoft.Xna.Framework.Color color = this.game.rrbs[num].color;
                System.Drawing.Color color2 = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
                int x = 0;
                while (true)
                {
                    if (x >= 0x10)
                    {
                        this.imageList1.Images.Add(bitmap);
                        item.ImageIndex = this.imageList1.Images.Count - 1;
                        this.RRBListView.Items.Add(item);
                        num++;
                        break;
                    }
                    int y = 0;
                    while (true)
                    {
                        if (y >= 0x10)
                        {
                            x++;
                            break;
                        }
                        if (((x == 0) || ((x == 15) || (y == 0))) || (y == 15))
                        {
                            bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                        }
                        else
                        {
                            bitmap.SetPixel(x, y, color2);
                        }
                        y++;
                    }
                }
            }
        }

        public void refreshSKB()
        {
            this.SkyboxSelector.Items.Clear();
            this.DefaultSkyboxSelector.Items.Clear();
            using (Dictionary<string, SKB_Gradient>.Enumerator enumerator = this.game.skb.Gradients.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    KeyValuePair<string, SKB_Gradient> current = enumerator.Current;
                    this.SkyboxSelector.Items.Add(current.Key);
                    this.DefaultSkyboxSelector.Items.Add(current.Key);
                }
            }
            string str = this.game.skb.Default;
            this.DefaultSkyboxSelector.SelectedItem = str;
            this.SkyboxSelector.SelectedItem = str;
            this.LoadGradient(this.game.skb.Gradients[str]);
            if (this.game.skb.Unknownfloat == null)
            {
                this.SkyboxFloatCheckbox.Checked = false;
            }
            else
            {
                this.SkyboxFloatCheckbox.Checked = true;
                this.SkyboxFloatBox.Text = this.game.skb.Unknownfloat.Value.ToString(ci);
            }
        }

        private void AnimatedObjectListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.AnimatedObjectAnimationComboBox.Items.Clear();
            this.AnimatedObjectAnimationComboBox.Enabled = false;
            this.AnimatedObjectRunButton.Enabled = false;
            if (!(this.AnimatedObjectListBox.SelectedItem is AnimatedObjectEntry selectedEntry))
            {
                this.AnimatedObjectStatusLabel.Text = "No animated object selected.";
                return;
            }

            foreach (MabAnimationDefinition animation in selectedEntry.Animations)
            {
                this.AnimatedObjectAnimationComboBox.Items.Add(animation);
            }

            this.AnimatedObjectAnimationComboBox.DisplayMember = "DisplayName";
            this.AnimatedObjectAnimationComboBox.Enabled = this.AnimatedObjectAnimationComboBox.Items.Count > 0;
            this.AnimatedObjectRunButton.Enabled = this.AnimatedObjectAnimationComboBox.Items.Count > 0;
            if (this.AnimatedObjectAnimationComboBox.Items.Count > 0)
            {
                this.AnimatedObjectAnimationComboBox.SelectedIndex = 0;
            }

            this.AnimatedObjectStatusLabel.Text =
                selectedEntry.ObjectName + Environment.NewLine +
                "Model: " + selectedEntry.ModelName + Environment.NewLine +
                "Type: " + selectedEntry.ObjectType + Environment.NewLine +
                "Animations: " + selectedEntry.Animations.Count.ToString(ci);
        }

        private void AnimatedObjectRunButton_Click(object sender, EventArgs e)
        {
            if (!(this.AnimatedObjectListBox.SelectedItem is AnimatedObjectEntry selectedEntry) ||
                !(this.AnimatedObjectAnimationComboBox.SelectedItem is MabAnimationDefinition selectedAnimation))
            {
                return;
            }

            this.game.PlayAnimatedObject(selectedEntry, selectedAnimation, this.AnimatedObjectLoopCheckBox.Checked);
            if (selectedEntry.ObjectType == AnimatedObjectType.AnimatedModel)
            {
                this.tsmiAnimatedObjects.Checked = true;
            }
            else
            {
                this.tsmiStaticObjects.Checked = true;
            }
            this.AnimatedObjectStatusLabel.Text =
                "Running " + selectedAnimation.DisplayName + Environment.NewLine +
                "Object: " + selectedEntry.ObjectName + Environment.NewLine +
                "Loop: " + (this.AnimatedObjectLoopCheckBox.Checked ? "Yes" : "No");
        }

        public void refreshStaticObjects()
        {
            this.StaticObjectListBox.Items.Clear();
            List<string> items = new List<string>();
            List<WDB> scenes = new List<WDB>();
            if (this.game.wdb != null)
            {
                scenes.Add(this.game.wdb);
            }
            scenes.AddRange(this.game.extraWdbScenes.Where(scene => scene != null));

            foreach (WDB scene in scenes)
            {
                string scenePath = this.game.scenePaths.ContainsKey(scene) ? this.game.scenePaths[scene] : string.Empty;
                string sceneName = string.IsNullOrWhiteSpace(scenePath) ? "Scene" : Path.GetFileNameWithoutExtension(scenePath);
                foreach (KeyValuePair<string, WDB_StaticModel> current in scene.StaticModels.OrderBy(item => item.Key, StringComparer.InvariantCultureIgnoreCase))
                {
                    string modelName = "<none>";
                    if (current.Value?.ModelRef != null &&
                        current.Value.ModelRef.IndexGDB >= 0 &&
                        current.Value.ModelRef.IndexGDB < scene.GDBs.Length)
                    {
                        modelName = scene.GDBs[current.Value.ModelRef.IndexGDB];
                    }
                    items.Add(sceneName + " :: " + current.Key + " [" + modelName + "]");
                }

                foreach (KeyValuePair<string, WDB_BDBModel> current in scene.BDBModels.OrderBy(item => item.Key, StringComparer.InvariantCultureIgnoreCase))
                {
                    string modelName = "<none>";
                    if (current.Value?.ModelRef != null &&
                        current.Value.ModelRef.IndexGDB >= 0 &&
                        current.Value.ModelRef.IndexGDB < scene.GDBs.Length)
                    {
                        modelName = scene.GDBs[current.Value.ModelRef.IndexGDB];
                    }
                    items.Add(sceneName + " :: " + current.Key + " [" + modelName + "]");
                }
            }

            foreach (string item in items)
            {
                this.StaticObjectListBox.Items.Add(item);
            }

            this.StaticObjectStatusLabel.Text = items.Count == 0
                ? "No static objects loaded."
                : items.Count.ToString(ci) + " static objects loaded.";
        }

        public void refreshWDB()
        {
            this.refreshStaticObjects();
            this.game.RebuildAnimatedObjects();
            this.AnimatedObjectListBox.Items.Clear();
            this.AnimatedObjectAnimationComboBox.Items.Clear();
            this.AnimatedObjectAnimationComboBox.Enabled = false;
            this.AnimatedObjectRunButton.Enabled = false;

            foreach (AnimatedObjectEntry entry in this.game.animatedObjects.OrderBy(item => item.SceneName, StringComparer.InvariantCultureIgnoreCase).ThenBy(item => item.ObjectName, StringComparer.InvariantCultureIgnoreCase))
            {
                this.AnimatedObjectListBox.Items.Add(entry);
            }

            if (this.game.animatedObjects.Count == 0)
            {
                this.AnimatedObjectStatusLabel.Text = "No animatable objects loaded.";
                return;
            }

            this.AnimatedObjectStatusLabel.Text = this.game.animatedObjects.Count.ToString(ci) + " animated objects loaded.";
            this.AnimatedObjectListBox.SelectedIndex = 0;
        }

        private void tsmiReload_Click(object sender, EventArgs e)
        {
            this.game.Reload();
        }

        public void resetDefaultSize()
        {
            base.Size = !(this.currentDPI == 120f) ? new Size(0x44c, 0x2a4) : new Size(0x483, 0x2ab);
            this.FormEditor_ResizeEnd(null, null);
        }

        private void rightclickmelabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.RRBListViewContextMenu.Show(this.rightclickmelabel, e.Location);
            }
        }

        private void RRBListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            this.game.rrbs[(int)e.Item.Tag].display = e.Item.Checked;
        }

        private void RRBListViewContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ToolStripItem[] itemArray;
            this.RRBListViewContextMenu.Items.Clear();
            if (this.RRBListView.SelectedIndices.Count == 0)
            {
                itemArray = new ToolStripItem[] { this.addNewContextMenuItem, this.tsmiUnloadAll };
                this.RRBListViewContextMenu.Items.AddRange(itemArray);
            }
            else
            {
                itemArray = new ToolStripItem[] { this.addNewContextMenuItem, this.editColorContextMenuItem, this.saveContextMenuItem, this.unloadSelectedContextMenuItem, this.tsmiUnloadAll };
                this.RRBListViewContextMenu.Items.AddRange(itemArray);
            }
        }

        private void saveContextMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void tsmiSavePowerupsAs_Click(object sender, EventArgs e)
        {
            this.PWBSaveAs();
        }

        private void tsmiSavePowerups_Click(object sender, EventArgs e)
        {
            if (this.edits.Contains("PWB"))
            {
                if (!File.Exists(this.game.currentPWBfile))
                {
                    this.PWBSaveAs();
                }
                else
                {
                    File.Delete(this.game.currentPWBfile);
                    this.game.pwb.Save(this.game.currentPWBfile);
                    Utils.WriteLine("Saved PWB " + this.game.currentPWBfile, ConsoleColor.White);
                    this.edits.Remove("PWB");
                }
            }
        }

        private void tsmiSaveSkyboxAs_Click(object sender, EventArgs e)
        {
            this.SKBSaveAs();
        }

        private void tsmiSaveSkybox_Click(object sender, EventArgs e)
        {
            if (this.edits.Contains("SKB"))
            {
                string str2;
                if (!File.Exists(str2 = Path.Combine(Path.GetDirectoryName(this.game.currentGDBfile), "BACKGRND.SKB")))
                {
                    this.SKBSaveAs();
                }
                else
                {
                    File.Delete(str2);
                    this.game.skb.Save(str2);
                    Utils.WriteLine("Saved SKB " + str2, ConsoleColor.White);
                    this.edits.Remove("SKB");
                }
            }
        }

        private void ScaleControls()
        {
            if (this.currentDPI == 96f)
            {
                Console.WriteLine("Scaling controls for 96 DPI ...");
                base.Size = new Size(0x44c, 0x2a4);
                this.MinimumSize = new Size(600, 300);
                this.pictureBox1.Location = new System.Drawing.Point(9, 0x1a);
                this.pictureBox1.Size = new Size(0x322, 0x25a);
                this.menuStrip1.Location = new System.Drawing.Point(0, 0);
                this.menuStrip1.Size = new Size(0x43c, 0x18);
                this.tsmiFile.Size = new Size(0x25, 20);
                this.tsmiOpen.Size = new Size(180, 0x16);
                this.tssFile1.Size = new Size(0xb1, 6);
                this.tsmiReload.Size = new Size(180, 0x16);
                this.tssFile3.Size = new Size(0xb1, 6);
                this.tsmiSave.Size = new Size(180, 0x16);
                this.tsmiSaveSkybox.Size = new Size(0x99, 0x16);
                this.tsmiSavePowerups.Size = new Size(0x99, 0x16);
                this.tsmiSaveObjects.Size = new Size(0x99, 0x16);
                this.tsmiSaveAs.Size = new Size(180, 0x16);
                this.tsmiSaveSkyboxAs.Size = new Size(0xb2, 0x16);
                this.tsmiSavePowerupsAs.Size = new Size(0xb2, 0x16);
                this.tsmiSaveObjectsAs.Size = new Size(0xb2, 0x16);
                this.tssFile2.Size = new Size(0xb1, 6);
                this.tsmiExit.Size = new Size(180, 0x16);
                this.tsmiEdit.Size = new Size(0x27, 20);
                this.tsmiView.Size = new Size(0x2c, 20);
                this.tsmiSkybox.Size = new Size(0x9b, 0x16);
                this.tsmiTextures.Size = new Size(0x9b, 0x16);
                this.tsmiVertexColors.Size = new Size(0x9b, 0x16);
                this.tssView1.Size = new Size(0x98, 6);
                this.tsmiPowerupBricks.Size = new Size(0x9b, 0x16);
                this.tsmiAIPaths.Size = new Size(0x9b, 0x16);
                this.tsmiStaticObjects.Size = new Size(0x9b, 0x16);
                this.tsmiAnimatedObjects.Size = new Size(0x9b, 0x16);
                this.tsmiCollisionGeometry.Size = new Size(0x9b, 0x16);
                this.tsmiOptions.Size = new Size(70, 20);
                this.tsmiAbout.Size = new Size(0x3d, 20);
                this.button1.Location = new System.Drawing.Point(0x331, 0x264);
                this.button1.Size = new Size(0x4b, 0x17);
                this.tabControl1.Location = new System.Drawing.Point(0x331, 0x1a);
                this.tabControl1.Size = new Size(0x10b, 0x19b);
                this.tpSKB.Location = new System.Drawing.Point(4, 0x16);
                this.tpSKB.Size = new Size(0x103, 0x181);
                this.DefaultSkyboxSelector.Location = new System.Drawing.Point(15, 0xeb);
                this.DefaultSkyboxSelector.Size = new Size(0x79, 0x15);
                this.label13.Location = new System.Drawing.Point(12, 0xdb);
                this.label13.Size = new Size(0x59, 13);
                this.SkyboxFloatBox.Location = new System.Drawing.Point(0x73, 0x10d);
                this.SkyboxFloatBox.Size = new Size(100, 20);
                this.SkyboxFloatCheckbox.Location = new System.Drawing.Point(11, 0x10f);
                this.SkyboxFloatCheckbox.Size = new Size(0x5f, 0x11);
                this.SkyboxApplybutton.Location = new System.Drawing.Point(0x5b, 0x133);
                this.SkyboxApplybutton.Size = new Size(0x4b, 0x17);
                this.groupBox1.Location = new System.Drawing.Point(6, 0x21);
                this.groupBox1.Size = new Size(0xe5, 0xb7);
                this.SkyboxIntBox.Location = new System.Drawing.Point(0x6f, 150);
                this.SkyboxIntBox.Size = new Size(100, 20);
                this.SkyboxIntCheckbox.Location = new System.Drawing.Point(5, 0x99);
                this.SkyboxIntCheckbox.Size = new Size(0x56, 0x11);
                this.label10.Location = new System.Drawing.Point(0x3d, 0x77);
                this.label10.Size = new Size(8, 13);
                this.label11.Location = new System.Drawing.Point(0x66, 0x77);
                this.label11.Size = new Size(9, 13);
                this.label12.Location = new System.Drawing.Point(0x8f, 0x77);
                this.label12.Size = new Size(8, 13);
                this.Color3BoxB.Location = new System.Drawing.Point(0x97, 0x74);
                this.Color3BoxB.Size = new Size(0x1a, 20);
                this.Color3BoxG.Location = new System.Drawing.Point(0x6f, 0x74);
                this.Color3BoxG.Size = new Size(0x1a, 20);
                this.Color3BoxR.Location = new System.Drawing.Point(70, 0x74);
                this.Color3BoxR.Size = new Size(0x1a, 20);
                this.label7.Location = new System.Drawing.Point(0x3d, 0x48);
                this.label7.Size = new Size(8, 13);
                this.label8.Location = new System.Drawing.Point(0x66, 0x48);
                this.label8.Size = new Size(9, 13);
                this.label9.Location = new System.Drawing.Point(0x8f, 0x48);
                this.label9.Size = new Size(8, 13);
                this.Color2BoxB.Location = new System.Drawing.Point(0x97, 0x45);
                this.Color2BoxB.Size = new Size(0x1a, 20);
                this.Color2BoxG.Location = new System.Drawing.Point(0x6f, 0x45);
                this.Color2BoxG.Size = new Size(0x1a, 20);
                this.Color2BoxR.Location = new System.Drawing.Point(70, 0x45);
                this.Color2BoxR.Size = new Size(0x1a, 20);
                this.label5.Location = new System.Drawing.Point(0x3d, 0x19);
                this.label5.Size = new Size(8, 13);
                this.label4.Location = new System.Drawing.Point(0x66, 0x19);
                this.label4.Size = new Size(9, 13);
                this.label6.Location = new System.Drawing.Point(0x8f, 0x19);
                this.label6.Size = new Size(8, 13);
                this.Color1BoxB.Location = new System.Drawing.Point(0x97, 0x16);
                this.Color1BoxB.Size = new Size(0x1a, 20);
                this.Color1BoxG.Location = new System.Drawing.Point(0x6f, 0x16);
                this.Color1BoxG.Size = new Size(0x1a, 20);
                this.Color1BoxR.Location = new System.Drawing.Point(70, 0x16);
                this.Color1BoxR.Size = new Size(0x1a, 20);
                this.Color3Box.Location = new System.Drawing.Point(0xbf, 0x70);
                this.Color3Box.Size = new Size(0x20, 0x20);
                this.Color2Box.Location = new System.Drawing.Point(0xbf, 0x40);
                this.Color2Box.Size = new Size(0x20, 0x20);
                this.label1.Location = new System.Drawing.Point(2, 0x19);
                this.label1.Size = new Size(40, 13);
                this.Color1Box.Location = new System.Drawing.Point(0xbf, 0x10);
                this.Color1Box.Size = new Size(0x20, 0x20);
                this.label3.Location = new System.Drawing.Point(2, 0x77);
                this.label3.Size = new Size(40, 13);
                this.label2.Location = new System.Drawing.Point(2, 0x48);
                this.label2.Size = new Size(40, 13);
                this.SkyboxSelector.Location = new System.Drawing.Point(0x44, 6);
                this.SkyboxSelector.Size = new Size(0x79, 0x15);
                this.tpPWB.Location = new System.Drawing.Point(4, 0x16);
                this.tpPWB.Size = new Size(0x103, 0x181);
                this.button3.Location = new System.Drawing.Point(0x9f, 0x129);
                this.button3.Size = new Size(0x4b, 0x24);
                this.label31.Location = new System.Drawing.Point(3, 0xfe);
                this.label31.Size = new Size(0x22, 13);
                this.label30.Location = new System.Drawing.Point(20, 0x14f);
                this.label30.Size = new Size(0x11, 13);
                this.label29.Location = new System.Drawing.Point(20, 0x135);
                this.label29.Size = new Size(0x11, 13);
                this.label28.Location = new System.Drawing.Point(20, 0x11b);
                this.label28.Size = new Size(0x11, 13);
                this.BrickZtextBox.Location = new System.Drawing.Point(0x2b, 0x14c);
                this.BrickZtextBox.Size = new Size(100, 20);
                this.BrickYtextBox.Location = new System.Drawing.Point(0x2b, 0x132);
                this.BrickYtextBox.Size = new Size(100, 20);
                this.BrickXtextBox.Location = new System.Drawing.Point(0x2b, 280);
                this.BrickXtextBox.Size = new Size(100, 20);
                this.button2.Location = new System.Drawing.Point(0x8f, 0x164);
                this.button2.Size = new Size(0x5c, 0x17);
                this.BrickColorComboBox.Location = new System.Drawing.Point(0x2b, 250);
                this.BrickColorComboBox.Size = new Size(100, 0x15);
                this.BrickDeleteButton.Location = new System.Drawing.Point(0x16, 0x164);
                this.BrickDeleteButton.Size = new Size(0x5c, 0x17);
                this.PWBListBox.Location = new System.Drawing.Point(6, 6);
                this.PWBListBox.Size = new Size(0xe4, 0xee);
                this.tabPageRRB.Location = new System.Drawing.Point(4, 0x16);
                this.tabPageRRB.Size = new Size(0x103, 0x181);
                this.RRBEditCheckBox.Location = new System.Drawing.Point(6, 140);
                this.RRBEditCheckBox.Size = new Size(0x36, 0x15);
                this.RRBListView.Location = new System.Drawing.Point(6, 0x16);
                this.RRBListView.Size = new Size(0xe4, 0x70);
                this.RRBListViewContextMenu.Size = new Size(0x86, 0x72);
                this.addNewContextMenuItem.Size = new Size(0x85, 0x16);
                this.editColorContextMenuItem.Size = new Size(0x85, 0x16);
                this.saveContextMenuItem.Size = new Size(0x85, 0x16);
                this.unloadSelectedContextMenuItem.Size = new Size(0x85, 0x16);
                this.tsmiUnloadAll.Size = new Size(0x85, 0x16);
                this.imageList1.ImageSize = new Size(0x10, 0x10);
                this.RRBNodeListBox.Location = new System.Drawing.Point(6, 0xa9);
                this.RRBNodeListBox.Size = new Size(0xe4, 160);
                this.label33.Location = new System.Drawing.Point(4, 0x8f);
                this.label33.Size = new Size(0x80, 13);
                this.label32.Location = new System.Drawing.Point(4, 6);
                this.label32.Size = new Size(0x58, 13);
                this.tabPage3.Location = new System.Drawing.Point(4, 0x16);
                this.tabPage3.Size = new Size(0x103, 0x181);
                this.StaticObjectListBox.Location = new System.Drawing.Point(6, 0x16);
                this.StaticObjectListBox.Size = new Size(0xe4, 0x10a);
                this.StaticObjectStatusLabel.Location = new System.Drawing.Point(6, 0x125);
                this.StaticObjectStatusLabel.Size = new Size(0xe4, 0x55);
                this.tabPage4.Location = new System.Drawing.Point(4, 0x16);
                this.tabPage4.Size = new Size(0x103, 0x181);
                this.AnimatedObjectListBox.Location = new System.Drawing.Point(6, 0x16);
                this.AnimatedObjectListBox.Size = new Size(0xe4, 0xc0);
                this.AnimatedObjectAnimationComboBox.Location = new System.Drawing.Point(9, 0xe9);
                this.AnimatedObjectAnimationComboBox.Size = new Size(0xe1, 0x15);
                this.AnimatedObjectLoopCheckBox.Location = new System.Drawing.Point(9, 260);
                this.AnimatedObjectRunButton.Location = new System.Drawing.Point(9, 0x119);
                this.AnimatedObjectRunButton.Size = new Size(0x63, 0x17);
                this.AnimatedObjectStatusLabel.Location = new System.Drawing.Point(6, 0x138);
                this.AnimatedObjectStatusLabel.Size = new Size(0xe4, 0x40);
                this.tabPage1.Location = new System.Drawing.Point(4, 0x16);
                this.tabPage1.Size = new Size(0x103, 0x181);
                this.DebugButton2.Location = new System.Drawing.Point(0xb0, 0xf6);
                this.DebugButton2.Size = new Size(0x4b, 0x23);
                this.DebugButton1.Location = new System.Drawing.Point(15, 0xf6);
                this.DebugButton1.Size = new Size(0x4b, 0x17);
                this.DebugListView.Location = new System.Drawing.Point(15, 12);
                this.DebugListView.Size = new Size(0xec, 0xe4);
                this.BrickplaceStopButton.Location = new System.Drawing.Point(0x38a, 0xb3);
                this.BrickplaceStopButton.Size = new Size(0x4b, 0x24);
                this.Brickplacelabel.Location = new System.Drawing.Point(880, 160);
                this.Brickplacelabel.Size = new Size(0x81, 13);
                this.label14.Location = new System.Drawing.Point(0, 0);
                this.label14.Size = new Size(0x30, 13);
                this.label15.Location = new System.Drawing.Point(0, 0x0B);
                this.label15.Size = new Size(0x7b, 13);
                this.label16.Location = new System.Drawing.Point(0, 0x23);
                this.label16.Size = new Size(0x9e, 13);
                this.label17.Location = new System.Drawing.Point(0, 0x3B);
                this.label17.Size = new Size(0xaf, 13);
                this.label19.Location = new System.Drawing.Point(0, 0x17);
                this.label19.Size = new Size(0x66, 13);
                this.label20.Location = new System.Drawing.Point(0, 0x2F);
                this.label20.Size = new Size(0x5f, 13);
                this.label21.Location = new System.Drawing.Point(0, 0x47);
                this.label21.Size = new Size(0xa7, 13);
                this.label22.Location = new System.Drawing.Point(0, 0x6B);
                this.label22.Size = new Size(0x60, 13);
                this.label23.Location = new System.Drawing.Point(0, 0x53);
                this.label23.Size = new Size(0x7a, 13);
                this.label24.Location = new System.Drawing.Point(0, 0x5F);
                this.label24.Size = new Size(140, 13);
                this.label25.Location = new System.Drawing.Point(0, 0x77);
                this.label25.Size = new Size(0x66, 13);
                this.label26.Location = new System.Drawing.Point(0, 0x83);
                this.label26.Size = new Size(0x6b, 13);
                this.panel1.Location = new System.Drawing.Point(0x331, 0x1D2);
                this.panel1.Size = new Size(0x10b, 0x90);
                this.label27.Location = new System.Drawing.Point(0x331, 0x1B7);
                this.label27.Size = new Size(0x42, 13);
                this.CameraPositionLabel.Location = new System.Drawing.Point(0x38a, 0x1B7);
                this.CameraPositionLabel.Size = new Size(0x2d, 13);
                this.CameraSpeedHeaderLabel.Location = new System.Drawing.Point(0x331, 0x1C4);
                this.CameraSpeedHeaderLabel.Size = new Size(0x42, 13);
                this.CameraSpeedLabel.Location = new System.Drawing.Point(0x38a, 0x1C4);
                this.CameraSpeedLabel.Size = new Size(0x2d, 13);
            }
            else if (this.currentDPI == 120f)
            {
                Console.WriteLine("Scaling controls for 120 DPI ...");
                base.Size = new Size(0x483, 0x2ab);
                this.MinimumSize = new Size(600, 300);
                this.pictureBox1.Location = new System.Drawing.Point(9, 0x1a);
                this.pictureBox1.Size = new Size(0x322, 0x25a);
                this.menuStrip1.ImageScalingSize = new Size(20, 20);
                this.menuStrip1.Location = new System.Drawing.Point(0, 0);
                this.menuStrip1.Size = new Size(0x46c, 0x1c);
                this.tsmiFile.Size = new Size(0x2c, 0x18);
                this.tsmiOpen.Size = new Size(0xdb, 0x1a);
                this.tssFile1.Size = new Size(0xd8, 6);
                this.tsmiReload.Size = new Size(0xdb, 0x1a);
                this.tssFile3.Size = new Size(0xd8, 6);
                this.tsmiSave.Size = new Size(0xdb, 0x1a);
                this.tsmiSaveSkybox.Size = new Size(0xb7, 0x1a);
                this.tsmiSavePowerups.Size = new Size(0xb7, 0x1a);
                this.tsmiSaveObjects.Size = new Size(0xb7, 0x1a);
                this.tsmiSaveAs.Size = new Size(0xdb, 0x1a);
                this.tsmiSaveSkyboxAs.Size = new Size(0xd4, 0x1a);
                this.tsmiSavePowerupsAs.Size = new Size(0xd4, 0x1a);
                this.tsmiSaveObjectsAs.Size = new Size(0xd4, 0x1a);
                this.tssFile2.Size = new Size(0xd8, 6);
                this.tsmiExit.Size = new Size(0xdb, 0x1a);
                this.tsmiEdit.Size = new Size(0x2f, 0x18);
                this.tsmiView.Size = new Size(0x35, 0x18);
                this.tsmiSkybox.Size = new Size(0xb8, 0x1a);
                this.tsmiTextures.Size = new Size(0xb8, 0x1a);
                this.tsmiVertexColors.Size = new Size(0xb8, 0x1a);
                this.tssView1.Size = new Size(0xb5, 6);
                this.tsmiPowerupBricks.Size = new Size(0xb8, 0x1a);
                this.tsmiAIPaths.Size = new Size(0xb8, 0x1a);
                this.tsmiStaticObjects.Size = new Size(0xb8, 0x1a);
                this.tsmiAnimatedObjects.Size = new Size(0xb8, 0x1a);
                this.tsmiCollisionGeometry.Size = new Size(0xb8, 0x1a);
                this.tsmiOptions.Size = new Size(0x52, 0x18);
                this.tsmiAbout.Size = new Size(0x47, 0x18);
                this.button1.Location = new System.Drawing.Point(0x331, 0x267);
                this.button1.Size = new Size(0x6a, 0x1b);
                this.tabControl1.Location = new System.Drawing.Point(0x331, 0x1a);
                this.tabControl1.Size = new Size(0x13b, 0x1a3);
                this.tpSKB.Location = new System.Drawing.Point(4, 0x19);
                this.tpSKB.Size = new Size(0x133, 390);
                this.DefaultSkyboxSelector.Location = new System.Drawing.Point(14, 0xef);
                this.DefaultSkyboxSelector.Size = new Size(0x79, 0x18);
                this.BrickplaceStopButton.Location = new System.Drawing.Point(0x3a6, 0xb1);
                this.BrickplaceStopButton.Size = new Size(0x4b, 0x19);
                this.Brickplacelabel.Location = new System.Drawing.Point(890, 0x9d);
                this.Brickplacelabel.Size = new Size(0xac, 0x11);
                this.label13.Location = new System.Drawing.Point(12, 0xdb);
                this.label13.Size = new Size(0x76, 0x11);
                this.SkyboxFloatBox.Location = new System.Drawing.Point(0x88, 270);
                this.SkyboxFloatBox.Size = new Size(0x87, 0x16);
                this.SkyboxFloatCheckbox.Location = new System.Drawing.Point(11, 0x10f);
                this.SkyboxFloatCheckbox.Size = new Size(0x77, 0x15);
                this.SkyboxApplybutton.Location = new System.Drawing.Point(0x71, 0x133);
                this.SkyboxApplybutton.Size = new Size(0x4b, 0x19);
                this.groupBox1.Location = new System.Drawing.Point(6, 0x21);
                this.groupBox1.Size = new Size(0x110, 0xb7);
                this.SkyboxIntBox.Location = new System.Drawing.Point(0x76, 0x99);
                this.SkyboxIntBox.Size = new Size(0x93, 0x16);
                this.SkyboxIntCheckbox.Location = new System.Drawing.Point(5, 0x99);
                this.SkyboxIntCheckbox.Size = new Size(0x6b, 0x15);
                this.label10.Location = new System.Drawing.Point(0x48, 0x76);
                this.label10.Size = new Size(12, 15);
                this.label11.Location = new System.Drawing.Point(0x7f, 0x76);
                this.label11.Size = new Size(13, 14);
                this.label12.Location = new System.Drawing.Point(0xbb, 0x76);
                this.label12.Size = new Size(10, 15);
                this.Color3BoxB.Location = new System.Drawing.Point(0xc6, 0x74);
                this.Color3BoxB.Size = new Size(0x1c, 0x16);
                this.Color3BoxG.Location = new System.Drawing.Point(0x90, 0x74);
                this.Color3BoxG.Size = new Size(0x1c, 0x16);
                this.Color3BoxR.Location = new System.Drawing.Point(0x57, 0x74);
                this.Color3BoxR.Size = new Size(0x1c, 0x16);
                this.label7.Location = new System.Drawing.Point(0x48, 0x45);
                this.label7.Size = new Size(12, 15);
                this.label8.Location = new System.Drawing.Point(0x7f, 0x45);
                this.label8.Size = new Size(13, 14);
                this.label9.Location = new System.Drawing.Point(0xbb, 0x45);
                this.label9.Size = new Size(10, 15);
                this.Color2BoxB.Location = new System.Drawing.Point(0xc6, 0x43);
                this.Color2BoxB.Size = new Size(0x1c, 0x16);
                this.Color2BoxG.Location = new System.Drawing.Point(0x90, 0x44);
                this.Color2BoxG.Size = new Size(0x1c, 0x16);
                this.Color2BoxR.Location = new System.Drawing.Point(0x57, 0x43);
                this.Color2BoxR.Size = new Size(0x1c, 0x16);
                this.label5.Location = new System.Drawing.Point(0x48, 0x15);
                this.label5.Size = new Size(12, 15);
                this.label4.Location = new System.Drawing.Point(0x7f, 0x15);
                this.label4.Size = new Size(13, 14);
                this.label6.Location = new System.Drawing.Point(0xbb, 0x15);
                this.label6.Size = new Size(10, 15);
                this.Color1BoxB.Location = new System.Drawing.Point(0xc6, 0x13);
                this.Color1BoxB.Size = new Size(0x1c, 0x16);
                this.Color1BoxG.Location = new System.Drawing.Point(0x8f, 0x13);
                this.Color1BoxG.Size = new Size(0x1c, 0x16);
                this.Color1BoxR.Location = new System.Drawing.Point(0x57, 0x13);
                this.Color1BoxR.Size = new Size(0x1c, 0x16);
                this.Color3Box.Location = new System.Drawing.Point(0xea, 110);
                this.Color3Box.Size = new Size(0x20, 0x20);
                this.Color2Box.Location = new System.Drawing.Point(0xea, 0x3e);
                this.Color2Box.Size = new Size(0x20, 0x20);
                this.label1.Location = new System.Drawing.Point(10, 0x15);
                this.label1.Size = new Size(0x35, 0x11);
                this.Color1Box.Location = new System.Drawing.Point(0xea, 14);
                this.Color1Box.Size = new Size(0x20, 0x20);
                this.label3.Location = new System.Drawing.Point(10, 0x76);
                this.label3.Size = new Size(0x35, 0x11);
                this.label2.Location = new System.Drawing.Point(10, 0x45);
                this.label2.Size = new Size(0x35, 0x11);
                this.SkyboxSelector.Location = new System.Drawing.Point(0x4e, 6);
                this.SkyboxSelector.Size = new Size(0x91, 0x18);
                this.tpPWB.Location = new System.Drawing.Point(4, 0x19);
                this.tpPWB.Size = new Size(0x133, 390);
                this.button3.Location = new System.Drawing.Point(0xcb, 0x11e);
                this.button3.Size = new Size(0x4b, 0x19);
                this.label31.Location = new System.Drawing.Point(9, 0xf8);
                this.label31.Size = new Size(0x2d, 0x11);
                this.label30.Location = new System.Drawing.Point(0x21, 0x149);
                this.label30.Size = new Size(0x15, 0x11);
                this.label29.Location = new System.Drawing.Point(0x21, 0x12f);
                this.label29.Size = new Size(0x15, 0x11);
                this.label28.Location = new System.Drawing.Point(0x21, 0x115);
                this.label28.Size = new Size(0x15, 0x11);
                this.BrickZtextBox.Location = new System.Drawing.Point(0x38, 0x146);
                this.BrickZtextBox.Size = new Size(0x86, 0x16);
                this.BrickYtextBox.Location = new System.Drawing.Point(0x38, 300);
                this.BrickYtextBox.Size = new Size(0x86, 0x16);
                this.BrickXtextBox.Location = new System.Drawing.Point(0x38, 0x112);
                this.BrickXtextBox.Size = new Size(0x86, 0x16);
                this.button2.Location = new System.Drawing.Point(0xba, 0x164);
                this.button2.Size = new Size(0x5c, 0x19);
                this.BrickColorComboBox.Location = new System.Drawing.Point(0x38, 0xf5);
                this.BrickColorComboBox.Size = new Size(0x86, 0x18);
                this.BrickDeleteButton.Location = new System.Drawing.Point(0x16, 0x164);
                this.BrickDeleteButton.Size = new Size(0x5c, 0x19);
                this.PWBListBox.Location = new System.Drawing.Point(6, 6);
                this.PWBListBox.Size = new Size(0x110, 0xe4);
                this.tabPageRRB.Location = new System.Drawing.Point(4, 0x19);
                this.tabPageRRB.Size = new Size(0x133, 390);
                this.RRBEditCheckBox.Location = new System.Drawing.Point(6, 140);
                this.RRBEditCheckBox.Size = new Size(0x36, 0x15);
                this.RRBListView.Location = new System.Drawing.Point(6, 0x16);
                this.RRBListView.Size = new Size(0x110, 0x70);
                this.RRBListViewContextMenu.ImageScalingSize = new Size(20, 20);
                this.RRBListViewContextMenu.Size = new Size(0xa1, 0x86);
                this.addNewContextMenuItem.Size = new Size(160, 0x1a);
                this.editColorContextMenuItem.Size = new Size(160, 0x1a);
                this.saveContextMenuItem.Size = new Size(160, 0x1a);
                this.unloadSelectedContextMenuItem.Size = new Size(160, 0x1a);
                this.tsmiUnloadAll.Size = new Size(160, 0x1a);
                this.imageList1.ImageSize = new Size(0x10, 0x10);
                this.RRBNodeListBox.Location = new System.Drawing.Point(6, 0xb8);
                this.RRBNodeListBox.Size = new Size(0xe4, 0xa4);
                this.label33.Location = new System.Drawing.Point(4, 0xa4);
                this.label33.Size = new Size(0xa8, 0x11);
                this.label32.Location = new System.Drawing.Point(4, 6);
                this.label32.Size = new Size(0x73, 0x11);
                this.tabPage3.Location = new System.Drawing.Point(4, 0x19);
                this.tabPage3.Size = new Size(0x133, 390);
                this.StaticObjectListBox.Location = new System.Drawing.Point(6, 0x16);
                this.StaticObjectListBox.Size = new Size(0x110, 0x120);
                this.StaticObjectStatusLabel.Location = new System.Drawing.Point(6, 0x13d);
                this.StaticObjectStatusLabel.Size = new Size(0x110, 0x40);
                this.tabPage4.Location = new System.Drawing.Point(4, 0x19);
                this.tabPage4.Size = new Size(0x133, 390);
                this.AnimatedObjectListBox.Location = new System.Drawing.Point(6, 0x16);
                this.AnimatedObjectListBox.Size = new Size(0x110, 0xc6);
                this.AnimatedObjectAnimationComboBox.Location = new System.Drawing.Point(9, 0xea);
                this.AnimatedObjectAnimationComboBox.Size = new Size(0x10d, 0x18);
                this.AnimatedObjectLoopCheckBox.Location = new System.Drawing.Point(9, 0x107);
                this.AnimatedObjectRunButton.Location = new System.Drawing.Point(9, 0x123);
                this.AnimatedObjectRunButton.Size = new Size(0x76, 0x19);
                this.AnimatedObjectStatusLabel.Location = new System.Drawing.Point(6, 0x142);
                this.AnimatedObjectStatusLabel.Size = new Size(0x110, 0x3b);
                this.tabPage1.Location = new System.Drawing.Point(4, 0x19);
                this.tabPage1.Size = new Size(0x133, 390);
                this.DebugButton2.Location = new System.Drawing.Point(0xb0, 0xf6);
                this.DebugButton2.Size = new Size(0x4b, 0x23);
                this.DebugButton1.Location = new System.Drawing.Point(15, 0xf6);
                this.DebugButton1.Size = new Size(0x4b, 0x17);
                this.DebugListView.Location = new System.Drawing.Point(15, 12);
                this.DebugListView.Size = new Size(0xec, 0xe4);
                this.label14.Location = new System.Drawing.Point(0, 0);
                this.label14.Size = new Size(0x40, 0x11);
                this.label15.Location = new System.Drawing.Point(0, 0x0E);
                this.label15.Size = new Size(0x9f, 0x11);
                this.label16.Location = new System.Drawing.Point(0, 0x2A);
                this.label16.Size = new Size(210, 0x11);
                this.label17.Location = new System.Drawing.Point(0, 0x46);
                this.label17.Size = new Size(0xec, 0x11);
                this.label19.Location = new System.Drawing.Point(0, 0x1C);
                this.label19.Size = new Size(0x85, 0x11);
                this.label20.Location = new System.Drawing.Point(0, 0x38);
                this.label20.Size = new Size(0x7d, 0x11);
                this.label21.Location = new System.Drawing.Point(0, 0x54);
                this.label21.Size = new Size(0xde, 0x11);
                this.label22.Location = new System.Drawing.Point(0, 0x7E);
                this.label22.Size = new Size(0x80, 0x11);
                this.label23.Location = new System.Drawing.Point(0, 0x62);
                this.label23.Size = new Size(0xa1, 0x11);
                this.label24.Location = new System.Drawing.Point(0, 0x70);
                this.label24.Size = new Size(0xbc, 0x11);
                this.label25.Location = new System.Drawing.Point(0, 0x8C);
                this.label25.Size = new Size(0x89, 0x11);
                this.label26.Location = new System.Drawing.Point(0, 0x9A);
                this.label26.Size = new Size(0x8f, 0x11);
                this.panel1.Location = new System.Drawing.Point(0x32e, 0x1C0);
                this.panel1.Size = new Size(0x137, 0xA5);
                this.label27.Location = new System.Drawing.Point(0x32e, 0x19E);
                this.label27.Size = new Size(0x58, 0x11);
                this.CameraPositionLabel.Location = new System.Drawing.Point(0x38a, 0x19E);
                this.CameraPositionLabel.Size = new Size(0x3a, 0x11);
                this.CameraSpeedHeaderLabel.Location = new System.Drawing.Point(0x32e, 0x1AF);
                this.CameraSpeedHeaderLabel.Size = new Size(0x58, 0x11);
                this.CameraSpeedLabel.Location = new System.Drawing.Point(0x38a, 0x1AF);
                this.CameraSpeedLabel.Size = new Size(0x3a, 0x11);
            }
        }

        public void SelectBricks(int[] indices, bool[] colored)
        {
            this.PWBListBox.SelectedIndices.Clear();
            for (int i = 0; i < indices.Length; i++)
            {
                int index = indices[i];
                if (!colored[i])
                {
                    index += this.game.pwb.ColorBricks.Count;
                }
                this.PWBListBox.SelectedIndices.Add(index);
            }
        }

        public void SetTabControlEnabled(bool value)
        {
            this.tabControl1.Enabled = value;
        }

        private void SKBSaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Skybox|*.SKB",
                FileName = "BACKGRND.SKB",
                AddExtension = true,
                OverwritePrompt = true
            };
            if (Utils.STAShowDialog(dialog) == DialogResult.OK)
            {
                if (File.Exists(dialog.FileName))
                {
                    File.Delete(dialog.FileName);
                }
                this.game.skb.Save(dialog.FileName);
                Utils.WriteLine("Saved SKB " + dialog.FileName, ConsoleColor.White);
                this.edits.Remove("SKB");
            }
        }

        private void SkyboxApplybutton_Click(object sender, EventArgs e)
        {
            LR1TrackEditor.SKB objA = this.game.skb;
            if (objA is null)
            {
                System.Windows.Forms.MessageBox.Show("Please open an SKB file before editing it", "Error");
            }
            else
            {
                SKB_Gradient gradient = new SKB_Gradient
                {
                    Color3 = new LRColor(this.Color1Box.BackColor.R, this.Color1Box.BackColor.G, this.Color1Box.BackColor.B, this.Color1Box.BackColor.A),
                    Color2 = new LRColor(this.Color2Box.BackColor.R, this.Color2Box.BackColor.G, this.Color2Box.BackColor.B, this.Color2Box.BackColor.A),
                    Color1 = new LRColor(this.Color3Box.BackColor.R, this.Color3Box.BackColor.G, this.Color3Box.BackColor.B, this.Color3Box.BackColor.A)
                };
                if (this.SkyboxIntCheckbox.Checked)
                {
                    if (int.TryParse(this.SkyboxIntBox.Text, out int num))
                    {
                        gradient.UnknownInt = new int?(num);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Unknown int has an invalid value, please fixerino", "Error");
                        return;
                    }
                }
                this.game.skb.Gradients[(string)this.SkyboxSelector.SelectedItem] = gradient;
                this.game.skb.Default = (string)this.DefaultSkyboxSelector.SelectedItem;
                gradient = objA.Gradients[this.game.skb.Default];
                this.game.skbmesh = Utils.GenerateSKBMesh(new Microsoft.Xna.Framework.Color(gradient.Color1.R, gradient.Color1.G, gradient.Color1.B), new Microsoft.Xna.Framework.Color(gradient.Color2.R, gradient.Color2.G, gradient.Color2.B), new Microsoft.Xna.Framework.Color(gradient.Color3.R, gradient.Color3.G, gradient.Color3.B));
                if (!this.edits.Contains("SKB"))
                {
                    this.edits.Add("SKB");
                }
            }
        }

        private void SkyboxFloatCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.SkyboxFloatBox.Visible = this.SkyboxFloatCheckbox.Checked;
        }

        private void SkyboxIntCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.SkyboxIntBox.Visible = this.SkyboxIntCheckbox.Checked;
        }

        private void SkyboxSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = (string)this.SkyboxSelector.SelectedItem;
            this.LoadGradient(this.game.skb.Gradients[selectedItem]);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.game.editmode = this.tabControl1.SelectedIndex;
            this.game.placing = false;
            this.DeselectBricks();
            this.game.ClearViewerSelection();
        }

        private void tsmiUnloadAll_Click(object sender, EventArgs e)
        {
            this.game.rrbs.Clear();
            this.refreshRRB();
        }

        private void unloadSelectedContextMenuItem_Click(object sender, EventArgs e)
        {
            this.game.rrbs.RemoveAt((int)this.RRBListView.SelectedItems[0].Tag);
            this.refreshRRB();
        }

        public void updateCameraPosition(Vector3 position)
        {
            object[] args = new object[] { position.X, position.Y, position.Z };
            this.CameraPositionLabel.Text = string.Format(ci, "({0}, {1}, {2})", args);
            this.CameraSpeedLabel.Text = string.Format(ci, "{0:F3}", InputHandler.flyspeed);
        }

        public bool PWBToolStripItemChecked
        {
            get =>
                this.tsmiPowerupBricks.Checked;
            set =>
                this.tsmiPowerupBricks.Checked = value;
        }

        public bool staticObjectsToolStripItemChecked
        {
            get =>
                this.tsmiStaticObjects.Checked;
            set =>
                this.tsmiStaticObjects.Checked = value;
        }

        public bool TabControlFocused =>
            this.tabControl1.ContainsFocus;
    }
}

