namespace LR1TrackEditor
{
    using LibLR1;
    using LibLR1.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class GameView : Game
    {
        private readonly GraphicsDeviceManager graphics;
        public BasicEffect basicEffect;
        private BasicEffect backgrnd;
        public RasterizerState rasterizerstate;
        public Vector3 cameraPosition;
        public VertexPTC[] skbmesh;
        public LR1TrackEditor.Model pupbrick = null;
        public LR1TrackEditor.Model puptrail = null;
        public LR1TrackEditor.Model enhabrick = null;
        public LR1TrackEditor.Model enhatrail = null;
        public float brickrotation = 0f;
        public Dictionary<string, Material> corematerials = new Dictionary<string, Material>();
        public bool mouselock = false;
        public bool fullscreen = false;
        public bool track = false;
        public bool doTextures;
        public bool doVertexColors;
        public bool doDrawPWB = true;
        public bool doDrawRRB = true;
        public bool doDrawSKB = true;
        public bool doDrawStaticObj = false;
        public bool doDrawAnimObj = false;
        public bool doDrawSPB = true;
        public bool doDrawCPB = true;
        public bool doDrawHZB = true;
        public bool doDrawEMB = true;
        public LR1TrackEditor.Model loadedmodel = null;
        public string currentGDBfile = "";
        public string currentPWBfile = "";
        public string currentWDBfile = "";
        public string currentRABfile = "";
        public Dictionary<string, Material> materials = new Dictionary<string, Material>();
        public Dictionary<string, LR1TrackEditor.Model> models = new Dictionary<string, LR1TrackEditor.Model>();
        public string currentmatfile = "";
        public float Pitch = 0f;
        public float Yaw = 0f;
        public int height = 0;
        public int width = 0;
        public PWB pwb = null;
        public LR1TrackEditor.SKB skb = null;
        public WDB wdb = null;
        public RAB rab = null;
        public SPB spb = null;
        public CPB cpb = null;
        public HZB hzb = null;
        public List<EMB> embs = new List<EMB>();
        public List<RRBFile> rrbs = new List<RRBFile>();
        public int editingRRBindex = -1;
        public string gamedir = "";
        public IntPtr drawsurface = IntPtr.Zero;
        private IntPtr pctdrawsurface = IntPtr.Zero;
        private Size pctsize;
        private Size surfacesize;
        private readonly Form gameform;
        public FormEditor form;
        private readonly Dictionary<byte, string> brickcolors;
        public int editmode;
        public List<bool> SelectedBricksColored;
        public List<int> SelectedBrickIndices;
        public bool placing;
        public int placed;
        public LR1TrackEditor.Model placingmodel;
        public Vector3? placingposition;
        public BoundingBox[] dragarrowhitboxes;

        public GameView()
        {
            Dictionary<byte, string> dictionary = new Dictionary<byte, string> {
                {
                    0x2a,
                    "pbrickP"
                },
                {
                    0x2c,
                    "pbrickS"
                },
                {
                    0x2d,
                    "pbrickT"
                },
                {
                    0x2b,
                    "pbrickM"
                }
            };
            this.brickcolors = dictionary;
            this.editmode = 0;
            List<bool> list = new List<bool> { false };
            this.SelectedBricksColored = list;
            this.SelectedBrickIndices = new List<int>();
            this.placing = false;
            this.placed = 0;
            this.placingmodel = null;
            this.placingposition = null;
            this.dragarrowhitboxes = new BoundingBox[3];
            if (LR1TrackEditor.Settings.Default.NeedsUpdate)
            {
                LR1TrackEditor.Settings.Default.Upgrade();
                LR1TrackEditor.Settings.Default.NeedsUpdate = false;
                LR1TrackEditor.Settings.Default.Save();
            }
            this.gameform = (Form)Control.FromHandle(base.Window.Handle);
            base.IsMouseVisible = true;
            this.graphics = new GraphicsDeviceManager(this);
            base.Content.RootDirectory = "Content";
            this.doTextures = LR1TrackEditor.Settings.Default.doTextures;
            this.doVertexColors = LR1TrackEditor.Settings.Default.doVertexColors;
            this.doDrawSKB = LR1TrackEditor.Settings.Default.doSkybox;
            this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(this.graphics_PreparingDeviceSettings);
            this.gameform.VisibleChanged += new EventHandler(this.gameform_VisibleChanged);
        }

        protected override void Draw(GameTime gameTime)
        {
            Matrix matrix;
            LRVector3 position;
            this.basicEffect.World = Matrix.Identity;
            base.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color((int)LR1TrackEditor.Settings.Default.BackgroundColor.R, (int)LR1TrackEditor.Settings.Default.BackgroundColor.G, (int)LR1TrackEditor.Settings.Default.BackgroundColor.B);
            if ((this.skbmesh != null) && this.doDrawSKB)
            {
                color = this.skbmesh[0].Color;
            }
            base.GraphicsDevice.Clear(color);
            bool flag = (this.skbmesh == null) || !this.doDrawSKB;
            if (!flag)
            {
                base.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                foreach (EffectPass pass in this.backgrnd.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.TriangleStrip, this.skbmesh, 0, this.skbmesh.Length - 2, VertexPTC.VertexDeclaration);
                }
            }
            base.GraphicsDevice.RasterizerState = this.rasterizerstate;
            this.loadedmodel?.Draw(this, this.basicEffect);
            if ((this.pwb != null) && this.doDrawPWB)
            {
                Matrix[] transforms = new Matrix[this.pwb.ColorBricks.Count];
                Material[] mats = new Material[this.pwb.ColorBricks.Count];
                int index = 0;
                while (true)
                {
                    flag = index < this.pwb.ColorBricks.Count;
                    if (!flag)
                    {
                        this.pupbrick.DrawMultiple(this, this.basicEffect, transforms, mats);
                        transforms = new Matrix[this.pwb.WhiteBricks.Count];
                        mats = new Material[this.pwb.WhiteBricks.Count];
                        index = 0;
                        while (true)
                        {
                            flag = index < this.pwb.WhiteBricks.Count;
                            if (!flag)
                            {
                                this.enhabrick.DrawMultiple(this, this.basicEffect, transforms, mats);
                                if (this.editmode == 1)
                                {
                                    if ((this.placing && LR1TrackEditor.Settings.Default.GhostPlacing) && (this.placingposition != null))
                                    {
                                        Material mat = new Material
                                        {
                                            ambientcolor = this.corematerials["pbrickP"].ambientcolor,
                                            diffusecolor = this.corematerials["pbrickP"].diffusecolor,
                                            texture = this.corematerials["pbrickP"].texture,
                                            alpha = 100
                                        };
                                        this.placingmodel.Draw(this, this.basicEffect, Matrix.CreateTranslation(this.placingposition.Value), mat);
                                    }
                                    index = 0;
                                    while (true)
                                    {
                                        flag = index < this.SelectedBrickIndices.Count;
                                        if (!flag)
                                        {
                                            break;
                                        }
                                        if (this.SelectedBricksColored[index])
                                        {
                                            PWB_ColorBrick brick = this.pwb.ColorBricks[this.SelectedBrickIndices[index]];
                                            matrix = Matrix.CreateTranslation(brick.Position.X, brick.Position.Y, brick.Position.Z);
                                            this.pupbrick.DrawBoundingBox(this, this.basicEffect, matrix);
                                        }
                                        else
                                        {
                                            PWB_WhiteBrick brick2 = this.pwb.WhiteBricks[this.SelectedBrickIndices[index]];
                                            matrix = Matrix.CreateTranslation(brick2.Position.X, brick2.Position.Y, brick2.Position.Z);
                                            this.enhabrick.DrawBoundingBox(this, this.basicEffect, matrix);
                                        }
                                        if (index == (this.SelectedBrickIndices.Count - 1))
                                        {
                                            position = !this.SelectedBricksColored[index] ? this.pwb.WhiteBricks[this.SelectedBrickIndices[index]].Position : this.pwb.ColorBricks[this.SelectedBrickIndices[index]].Position;
                                            base.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                                            this.DrawMoveArrows(position.toXNAVector());
                                            base.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                                        }
                                        index++;
                                    }
                                }
                                break;
                            }
                            transforms[index] = Matrix.CreateFromYawPitchRoll(0f, 0f, this.brickrotation * 3.141593f) * Matrix.CreateTranslation(this.pwb.WhiteBricks[index].Position.X, this.pwb.WhiteBricks[index].Position.Y, this.pwb.WhiteBricks[index].Position.Z);
                            mats[index] = this.corematerials["pubrickw"];
                            index++;
                        }
                        break;
                    }
                    transforms[index] = Matrix.CreateFromYawPitchRoll(0f, 0f, this.brickrotation * 3.141593f) * Matrix.CreateTranslation(this.pwb.ColorBricks[index].Position.X, this.pwb.ColorBricks[index].Position.Y, this.pwb.ColorBricks[index].Position.Z);
                    mats[index] = this.corematerials[this.brickcolors[this.pwb.ColorBricks[index].Color]];
                    index++;
                }
            }
            flag = (this.rrbs.Count <= 0) || !this.doDrawRRB;
            if (!flag)
            {
                this.basicEffect.TextureEnabled = false;
                this.basicEffect.VertexColorEnabled = true;
                using (List<RRBFile>.Enumerator enumerator2 = this.rrbs.GetEnumerator())
                {
                    while (true)
                    {
                        flag = enumerator2.MoveNext();
                        if (!flag)
                        {
                            break;
                        }
                        RRBFile current = enumerator2.Current;
                        flag = !current.display;
                        if (!flag)
                        {
                            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
                            {
                                pass.Apply();
                                base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.LineStrip, current.points.ToArray(), 0, current.points.Count - 1, VertexPTC.VertexDeclaration);
                            }
                        }
                    }
                }
            }
            if (!(this.wdb is null))
            {
                flag = !this.doDrawStaticObj;
                if (!flag)
                {
                    // Track which GDB indices are referenced by static models
                    HashSet<int> referencedGDBIndices = new HashSet<int>();
                    using (Dictionary<string, WDB_StaticModel>.Enumerator enumerator3 = this.wdb.StaticModels.GetEnumerator())
                    {
                        while (true)
                        {
                            flag = enumerator3.MoveNext();
                            if (!flag)
                            {
                                break;
                            }
                            KeyValuePair<string, WDB_StaticModel> current = enumerator3.Current;
                            string gdbName = null;
                            if (current.Value.ModelRef != null && current.Value.ModelRef.IndexGDB >= 0 && current.Value.ModelRef.IndexGDB < this.wdb.GDBs.Length)
                            {
                                gdbName = this.wdb.GDBs[current.Value.ModelRef.IndexGDB];
                                referencedGDBIndices.Add(current.Value.ModelRef.IndexGDB);
                            }
                            if (gdbName != null && this.models.ContainsKey(gdbName))
                            {
                                position = current.Value.Position;
                                LRVector3 rotationFwd = current.Value.RotationFwd;
                                LRVector3 rotationUp = current.Value.RotationUp;
                                Vector3 vector4 = Vector3.Cross(rotationUp.toXNAVector(), rotationFwd.toXNAVector());
                                matrix = new Matrix(rotationFwd.X, rotationFwd.Y, rotationFwd.Z, 0f, vector4.X, vector4.Y, vector4.Z, 0f, rotationUp.X, rotationUp.Y, rotationUp.Z, 0f, position.X, position.Y, position.Z, 1f);
                                this.models[gdbName].Draw(this, this.basicEffect, matrix, null);
                            }
                        }
                    }
                    // Draw GDBs not referenced by any static model at origin (e.g. track mesh)
                    for (int i = 0; i < this.wdb.GDBs.Length; i++)
                    {
                        if (!referencedGDBIndices.Contains(i) && this.models.ContainsKey(this.wdb.GDBs[i]))
                        {
                            this.models[this.wdb.GDBs[i]].Draw(this, this.basicEffect);
                        }
                    }
                }
                flag = !this.doDrawAnimObj;
                if (!flag)
                {
                    using (Dictionary<string, WDB_AnimatedModel>.Enumerator enumerator4 = this.wdb.AnimatedModels.GetEnumerator())
                    {
                        while (true)
                        {
                            flag = enumerator4.MoveNext();
                            if (!flag)
                            {
                                break;
                            }
                            KeyValuePair<string, WDB_AnimatedModel> current = enumerator4.Current;
                            if (current.Value.ModelRef != null && current.Value.ModelRef.IndexGDB.HasValue && this.wdb.GDB2s.Length > current.Value.ModelRef.IndexGDB.Value)
                            {
                                string gdbName = this.wdb.GDB2s[current.Value.ModelRef.IndexGDB.Value];
                                if (this.models.ContainsKey(gdbName))
                                {
                                    position = current.Value.Position;
                                    LRVector3 rotationFwd = current.Value.RotationFwd;
                                    LRVector3 rotationUp = current.Value.RotationUp;
                                    Vector3 vector4 = Vector3.Cross(rotationUp.toXNAVector(), rotationFwd.toXNAVector());
                                    matrix = new Matrix(rotationFwd.X, rotationFwd.Y, rotationFwd.Z, 0f, vector4.X, vector4.Y, vector4.Z, 0f, rotationUp.X, rotationUp.Y, rotationUp.Z, 0f, position.X, position.Y, position.Z, 1f);
                                    this.models[gdbName].Draw(this, this.basicEffect, matrix, null);
                                }
                            }
                        }
                    }
                }
            }
            // Draw SPB start positions
            if (this.spb != null && this.doDrawSPB && this.spb.StartPositions != null)
            {
                List<Vector3> positions = new List<Vector3>();
                List<Vector3> directions = new List<Vector3>();
                foreach (var kvp in this.spb.StartPositions)
                {
                    SPB_StartPosition sp = kvp.Value;
                    positions.Add(new Vector3(sp.Position.X, sp.Position.Y, sp.Position.Z));
                    if (sp.Orientation != null && sp.Orientation.Length >= 3)
                    {
                        directions.Add(new Vector3(sp.Orientation[0], sp.Orientation[1], sp.Orientation[2]));
                    }
                    else
                    {
                        directions.Add(Vector3.Zero);
                    }
                }
                DrawPositionMarkers(positions, new Microsoft.Xna.Framework.Color(0, 255, 0), 5f);
                // Draw direction lines from start positions
                if (directions.Count == positions.Count)
                {
                    this.basicEffect.TextureEnabled = false;
                    this.basicEffect.VertexColorEnabled = true;
                    List<VertexPTC> dirLines = new List<VertexPTC>();
                    for (int i = 0; i < positions.Count; i++)
                    {
                        if (directions[i] != Vector3.Zero)
                        {
                            dirLines.Add(new VertexPTC(positions[i], new Microsoft.Xna.Framework.Color(0, 200, 0)));
                            dirLines.Add(new VertexPTC(positions[i] + directions[i] * 15f, new Microsoft.Xna.Framework.Color(0, 200, 0)));
                        }
                    }
                    if (dirLines.Count >= 2)
                    {
                        foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.LineList, dirLines.ToArray(), 0, dirLines.Count / 2, VertexPTC.VertexDeclaration);
                        }
                    }
                }
            }

            // Draw CPB checkpoints
            if (this.cpb != null && this.doDrawCPB && this.cpb.Checkpoints != null)
            {
                List<Vector3> positions = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                foreach (CPB_Checkpoint cp in this.cpb.Checkpoints)
                {
                    positions.Add(new Vector3(cp.Location.X, cp.Location.Y, cp.Location.Z));
                    if (cp.Direction != null && cp.Direction.Normal != null)
                    {
                        normals.Add(new Vector3(cp.Direction.Normal.X, cp.Direction.Normal.Y, cp.Direction.Normal.Z));
                    }
                    else
                    {
                        normals.Add(Vector3.Zero);
                    }
                }
                DrawPositionMarkers(positions, new Microsoft.Xna.Framework.Color(255, 255, 0), 8f);
                // Draw checkpoint direction normals
                this.basicEffect.TextureEnabled = false;
                this.basicEffect.VertexColorEnabled = true;
                List<VertexPTC> normLines = new List<VertexPTC>();
                for (int i = 0; i < positions.Count; i++)
                {
                    if (normals[i] != Vector3.Zero)
                    {
                        normLines.Add(new VertexPTC(positions[i], new Microsoft.Xna.Framework.Color(200, 200, 0)));
                        normLines.Add(new VertexPTC(positions[i] + normals[i] * 20f, new Microsoft.Xna.Framework.Color(200, 200, 0)));
                    }
                }
                if (normLines.Count >= 2)
                {
                    foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.LineList, normLines.ToArray(), 0, normLines.Count / 2, VertexPTC.VertexDeclaration);
                    }
                }
            }

            // Draw HZB hazard positions
            if (this.hzb != null && this.doDrawHZB && this.hzb.Entries != null)
            {
                List<Vector3> positions = new List<Vector3>();
                foreach (HZB_Entry entry in this.hzb.Entries)
                {
                    if (entry.PathData != null)
                    {
                        if (entry.PathData.Position1 != null)
                            positions.Add(new Vector3(entry.PathData.Position1.X, entry.PathData.Position1.Y, entry.PathData.Position1.Z));
                        if (entry.PathData.HasPosition2)
                            positions.Add(new Vector3(entry.PathData.Position2.X, entry.PathData.Position2.Y, entry.PathData.Position2.Z));
                        if (entry.PathData.HasPosition3)
                            positions.Add(new Vector3(entry.PathData.Position3.X, entry.PathData.Position3.Y, entry.PathData.Position3.Z));
                    }
                    if (entry.SpinningData != null && entry.SpinningData.HasPosition)
                    {
                        positions.Add(new Vector3(entry.SpinningData.Position.X, entry.SpinningData.Position.Y, entry.SpinningData.Position.Z));
                    }
                    if (entry.WaterZoneData != null && entry.WaterZoneData.Path != null && entry.WaterZoneData.Path.Position1 != null)
                    {
                        positions.Add(new Vector3(entry.WaterZoneData.Path.Position1.X, entry.WaterZoneData.Path.Position1.Y, entry.WaterZoneData.Path.Position1.Z));
                    }
                }
                DrawPositionMarkers(positions, new Microsoft.Xna.Framework.Color(255, 50, 50), 6f);
            }

            // Draw EMB emitter positions
            if (this.embs.Count > 0 && this.doDrawEMB)
            {
                List<Vector3> positions = new List<Vector3>();
                foreach (EMB emb in this.embs)
                {
                    if (emb.Emitters == null) continue;
                    foreach (var kvp in emb.Emitters)
                    {
                        EMB_Emitter em = kvp.Value;
                        if (em.Positions != null)
                        {
                            foreach (LRVector3 pos2 in em.Positions)
                            {
                                positions.Add(new Vector3(pos2.X, pos2.Y, pos2.Z));
                            }
                        }
                    }
                }
                DrawPositionMarkers(positions, new Microsoft.Xna.Framework.Color(0, 200, 255), 4f);
            }

            base.Draw(gameTime);
        }

        private void DrawPositionMarkers(List<Vector3> positions, Microsoft.Xna.Framework.Color color, float size)
        {
            if (positions.Count == 0) return;
            this.basicEffect.TextureEnabled = false;
            this.basicEffect.VertexColorEnabled = true;
            List<VertexPTC> lines = new List<VertexPTC>();
            foreach (Vector3 pos in positions)
            {
                lines.Add(new VertexPTC(pos + new Vector3(-size, 0, 0), color));
                lines.Add(new VertexPTC(pos + new Vector3(size, 0, 0), color));
                lines.Add(new VertexPTC(pos + new Vector3(0, -size, 0), color));
                lines.Add(new VertexPTC(pos + new Vector3(0, size, 0), color));
                lines.Add(new VertexPTC(pos + new Vector3(0, 0, -size), color));
                lines.Add(new VertexPTC(pos + new Vector3(0, 0, size), color));
            }
            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.LineList, lines.ToArray(), 0, lines.Count / 2, VertexPTC.VertexDeclaration);
            }
        }

        private void DrawMoveArrows(Vector3 pos)
        {
            Vector3 vector = base.GraphicsDevice.Viewport.Unproject(new Vector3(((float)this.width) / 2f, ((float)this.height) / 2f, 0f), this.basicEffect.Projection, this.basicEffect.View, Matrix.Identity);
            float x = MathHelper.Min((pos - vector).Length(), 250f) * 0.2f;
            List<VertexPTC> list = new List<VertexPTC> {
                new VertexPTC(pos, Microsoft.Xna.Framework.Color.Orange),
                new VertexPTC(pos + new Vector3(x, 0f, 0f), Microsoft.Xna.Framework.Color.Orange),
                new VertexPTC(pos, Microsoft.Xna.Framework.Color.Orange),
                new VertexPTC(pos + new Vector3(0f, x, 0f), Microsoft.Xna.Framework.Color.Orange),
                new VertexPTC(pos, Microsoft.Xna.Framework.Color.Orange),
                new VertexPTC(pos + new Vector3(0f, 0f, x), Microsoft.Xna.Framework.Color.Orange)
            };
            base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.LineList, list.ToArray(), 0, list.Count / 2, VertexPTC.VertexDeclaration);
            float y = x * 0.05f;
            list.Clear();
            list.Add(new VertexPTC(pos + new Vector3(x, y, -y), new Microsoft.Xna.Framework.Color(150, 0, 0)));
            list.Add(new VertexPTC(pos + new Vector3(x, -y, -y), new Microsoft.Xna.Framework.Color(180, 0, 0)));
            list.Add(new VertexPTC(pos + new Vector3(x, 0f, y), new Microsoft.Xna.Framework.Color(240, 0, 0)));
            list.Add(new VertexPTC(pos + new Vector3(x + (3f * y), 0f, 0f), new Microsoft.Xna.Framework.Color(0xff, 5, 5)));
            this.dragarrowhitboxes[0] = new BoundingBox(pos + new Vector3(x, -y, -y), pos + new Vector3(x + (3f * y), y, y));
            list.Add(new VertexPTC(pos + new Vector3(y, x, -y), new Microsoft.Xna.Framework.Color(0, 0, 150)));
            list.Add(new VertexPTC(pos + new Vector3(-y, x, -y), new Microsoft.Xna.Framework.Color(0, 0, 180)));
            list.Add(new VertexPTC(pos + new Vector3(0f, x, y), new Microsoft.Xna.Framework.Color(0, 0, 240)));
            list.Add(new VertexPTC(pos + new Vector3(0f, x + (3f * y), 0f), new Microsoft.Xna.Framework.Color(5, 5, 0xff)));
            this.dragarrowhitboxes[1] = new BoundingBox(pos + new Vector3(-y, x, -y), pos + new Vector3(y, x + (3f * y), y));
            list.Add(new VertexPTC(pos + new Vector3(0f, y, x), new Microsoft.Xna.Framework.Color(0, 150, 0)));
            list.Add(new VertexPTC(pos + new Vector3(y, 0f, x), new Microsoft.Xna.Framework.Color(0, 180, 0)));
            list.Add(new VertexPTC(pos + new Vector3(-y, -y, x), new Microsoft.Xna.Framework.Color(0, 240, 0)));
            list.Add(new VertexPTC(pos + new Vector3(0f, 0f, x + (3f * y)), new Microsoft.Xna.Framework.Color(5, 0xff, 5)));
            this.dragarrowhitboxes[2] = new BoundingBox(pos + new Vector3(-y, -y, x), pos + new Vector3(y, y, x + (3f * y)));
            short[] indexData = new short[] {
                0, 1, 2, 0, 3, 1, 1, 3, 2, 2, 3, 0, 4, 5, 6, 4,
                7, 5, 5, 7, 6, 6, 7, 4, 8, 9, 10, 8, 11, 9, 9, 11,
                10, 10, 11, 8
            };
            base.GraphicsDevice.DrawUserIndexedPrimitives<VertexPTC>(PrimitiveType.TriangleList, list.ToArray(), 0, list.Count, indexData, 0, indexData.Length / 3, VertexPTC.VertexDeclaration);
        }

        private void gameform_VisibleChanged(object sender, EventArgs e)
        {
            if (!(!this.gameform.Visible || this.fullscreen))
            {
                this.gameform.Visible = false;
            }
            else if (this.gameform.Visible)
            {
                Console.WriteLine(Screen.PrimaryScreen.Bounds.Size);
                this.gameform.FormBorderStyle = FormBorderStyle.None;
                this.gameform.Size = Screen.PrimaryScreen.Bounds.Size;
                this.gameform.Location = Screen.PrimaryScreen.Bounds.Location;
                this.gameform.TopLevel = true;
            }
        }

        public GraphicsDeviceManager getGraphics() =>
            this.graphics;

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = this.drawsurface;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = this.surfacesize.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = this.surfacesize.Height;
        }

        protected override void Initialize()
        {
            this.height = base.GraphicsDevice.Viewport.Height;
            this.width = base.GraphicsDevice.Viewport.Width;
            this.rasterizerstate = RasterizerState.CullClockwise;
            this.basicEffect = new BasicEffect(base.GraphicsDevice)
            {
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(LR1TrackEditor.Settings.Default.FoV), ((float)this.width) / ((float)this.height), 1f, LR1TrackEditor.Settings.Default.RenderDistance)
            };
            this.cameraPosition = new Vector3(200f, -200f, 20f);
            this.backgrnd = new BasicEffect(base.GraphicsDevice)
            {
                View = Matrix.CreateLookAt(new Vector3(0f, 4f, 2.5f), new Vector3(0f, 1f, 2.5f), new Vector3(0f, 0f, 1f)),
                Projection = Matrix.CreatePerspective(5f, 3.7f, 1f, 4f),
                VertexColorEnabled = true,
                TextureEnabled = false,
                LightingEnabled = false
            };
            base.Initialize();
        }

        public void loadModel(string modelpath)
        {
            string str4;
            Action<KeyValuePair<string, Material>> action = null;
            Action<KeyValuePair<string, Material>> action2 = null;
            Action<KeyValuePair<string, Material>> action3 = null;
            this.form.ClearEdits(null);
            this.pwb = null;
            this.wdb = null;
            this.rrbs.Clear();
            this.loadedmodel?.vertexbuffer.Dispose();
            if (this.gamedir == "")
            {
                string str;
                this.gamedir = Utils.getGamedir(modelpath);
                if ((this.gamedir != "") && File.Exists(str = Path.Combine(this.gamedir, @"GAMEDATA\COMMON\PUBRICKY.GDB")))
                {
                    Console.WriteLine("Gamedir found at " + this.gamedir);
                    Console.WriteLine(@"Loading powerup models\textures");
                    this.pupbrick = Loader.loadmodel(this, str, false);
                    this.enhabrick = Loader.loadmodel(this, Path.Combine(Path.GetDirectoryName(str), "ENHABRIK.GDB"), false);
                    if (action == null)
                    {
                        action = x => this.corematerials[x.Key] = x.Value;
                    }
                    Loader.loadmaterials(Path.Combine(this.gamedir, @"GAMEDATA\COMMON\POWERUP.MDB"), base.GraphicsDevice).ToList<KeyValuePair<string, Material>>().ForEach(action);
                }
            }
            this.track = true;
            string directoryName = Path.GetDirectoryName(modelpath);
            Console.WriteLine("Loading model");
            string path = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(modelpath) + ".MDB");
            if (File.Exists(path) && File.Exists(path.Substring(0, path.Length - 3) + "TDB"))
            {
                if (this.currentmatfile != path)
                {
                    this.materials.Clear();
                    if (action2 == null)
                    {
                        action2 = x => this.materials[x.Key] = x.Value;
                    }
                    Loader.loadmaterials(Path.Combine(directoryName, Path.GetFileNameWithoutExtension(modelpath) + ".MDB"), base.GraphicsDevice).ToList<KeyValuePair<string, Material>>().ForEach(action2);
                    this.currentmatfile = path;
                }
            }
            else
            {
                path = Path.Combine(directoryName, "COMBINED.MDB");
                if ((File.Exists(path) && File.Exists(Path.Combine(directoryName, "COMBINED.TDB"))) && (this.currentmatfile != path))
                {
                    this.materials.Clear();
                    if (action3 == null)
                    {
                        action3 = x => this.materials[x.Key] = x.Value;
                    }
                    Loader.loadmaterials(path, base.GraphicsDevice).ToList<KeyValuePair<string, Material>>().ForEach(action3);
                    this.currentmatfile = path;
                }
            }
            this.loadedmodel = Loader.loadmodel(this, modelpath, false);
            HashSet<string> set = new HashSet<string>();
            using (List<ModelPart>.Enumerator enumerator = this.loadedmodel.parts.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    ModelPart current = enumerator.Current;
                    if (!(this.materials.ContainsKey(current.material) || set.Contains(current.material)))
                    {
                        set.Add(current.material);
                        Utils.WriteLine("Material " + current.material + " not found", ConsoleColor.Yellow);
                    }
                }
            }
            this.currentGDBfile = modelpath;
            if (File.Exists(Path.Combine(directoryName, "BACKGRND.SKB")))
            {
                this.skb = Loader.loadSKB(Path.Combine(directoryName, "BACKGRND.SKB"));
                SKB_Gradient gradient = this.skb.Gradients[this.skb.Default];
                this.skbmesh = Utils.GenerateSKBMesh(new Microsoft.Xna.Framework.Color(gradient.Color1.R, gradient.Color1.G, gradient.Color1.B), new Microsoft.Xna.Framework.Color(gradient.Color2.R, gradient.Color2.G, gradient.Color2.B), new Microsoft.Xna.Framework.Color(gradient.Color3.R, gradient.Color3.G, gradient.Color3.B));
                this.form.refreshSKB();
            }
            if (!LR1TrackEditor.Settings.Default.AutoloadPowerup)
            {
                this.form.PWBToolStripItemChecked = false;
            }
            else
            {
                str4 = Path.Combine(Path.GetDirectoryName(this.currentGDBfile), "POWERUP.PWB");
                if (File.Exists(str4))
                {
                    this.pwb = Loader.loadPWB(this, str4);
                    this.form.refreshPWB(false);
                }
            }
            if (!LR1TrackEditor.Settings.Default.AutoloadObject)
            {
                this.form.staticObjectsToolStripItemChecked = false;
            }
            else
            {
                str4 = Path.Combine(Path.GetDirectoryName(this.currentGDBfile), "TEST.WDB");
                if (File.Exists(str4))
                {
                    try
                    {
                        this.wdb = Loader.loadWDB(this, str4);
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load WDB: " + ex.Message, ConsoleColor.Red);
                        this.wdb = null;
                        this.form.staticObjectsToolStripItemChecked = false;
                    }
                }
            }
            this.form.SetTabControlEnabled(true);
        }

        public void Place(bool keepplacing = false)
        {
            if (this.placingposition != null)
            {
                this.placed++;
                LRVector3 position = new LRVector3(this.placingposition.Value.X, this.placingposition.Value.Y, this.placingposition.Value.Z);
                if (this.editmode == 1)
                {
                    PWB_ColorBrick item = new PWB_ColorBrick(position, 0x2a);
                    this.pwb.ColorBricks.Add(item);
                }
                if (!keepplacing)
                {
                    this.form.BrickplaceStopButton_Click(null, null);
                }
            }
        }

        public void Reload()
        {
            if (this.currentGDBfile != "")
            {
                this.currentmatfile = "";
                this.loadModel(this.currentGDBfile);
            }
            if (!((this.currentPWBfile == "") || LR1TrackEditor.Settings.Default.AutoloadPowerup))
            {
                this.pwb = Loader.loadPWB(this, this.currentPWBfile);
            }
            bool flag = this.rrbs.Count <= 0;
            if (!flag)
            {
                using (List<RRBFile>.Enumerator enumerator = this.rrbs.GetEnumerator())
                {
                    while (true)
                    {
                        flag = enumerator.MoveNext();
                        if (!flag)
                        {
                            break;
                        }
                        RRBFile current = enumerator.Current;
                        current.rrbfile = Loader.loadRRB(current.filepath);
                    }
                }
            }
        }

        public void Select(int mousex, int mousey, bool multiselect)
        {
            Vector3 position = base.GraphicsDevice.Viewport.Unproject(new Vector3((float)mousex, (float)mousey, 0f), this.basicEffect.Projection, this.basicEffect.View, Matrix.Identity);
            Vector3 direction = base.GraphicsDevice.Viewport.Unproject(new Vector3((float)mousex, (float)mousey, 1f), this.basicEffect.Projection, this.basicEffect.View, Matrix.Identity) - position;
            direction.Normalize();
            Ray input = new Ray(position, direction);
            float maxValue = float.MaxValue;
            if ((this.editmode != 1) || (this.pwb is null))
            {
                if (this.editmode == 2)
                {
                }
            }
            else
            {
                int item = -1;
                bool flag = false;
                int num3 = 0;
                while (true)
                {
                    BoundingBox box;
                    float? nullable;
                    bool flag2 = num3 < this.pwb.ColorBricks.Count;
                    if (!flag2)
                    {
                        num3 = 0;
                        while (true)
                        {
                            flag2 = num3 < this.pwb.WhiteBricks.Count;
                            if (!flag2)
                            {
                                bool flag1;
                                if (maxValue == float.MaxValue)
                                {
                                    flag1 = true;
                                }
                                else if (InputHandler.fillmode != 0)
                                {
                                    flag1 = false;
                                }
                                else
                                {
                                    float? nullable2 = Utils.distanceToTriangle(input);
                                    float num5 = maxValue;
                                    flag1 = (nullable2.GetValueOrDefault() < num5) && (nullable2 != null);
                                }
                                if (flag1)
                                {
                                    if (!multiselect)
                                    {
                                        this.form.DeselectBricks();
                                    }
                                }
                                else if (!multiselect)
                                {
                                    this.form.SelectBricks(new int[] { item }, new bool[] { flag });
                                }
                                else
                                {
                                    int index = -1;
                                    num3 = 0;
                                    while (true)
                                    {
                                        flag2 = num3 < this.SelectedBrickIndices.Count;
                                        if (flag2)
                                        {
                                            if ((this.SelectedBrickIndices[num3] != item) || (this.SelectedBricksColored[num3] != flag))
                                            {
                                                num3++;
                                                continue;
                                            }
                                            index = num3;
                                        }
                                        if (index != -1)
                                        {
                                            this.SelectedBrickIndices.RemoveAt(index);
                                            this.SelectedBricksColored.RemoveAt(index);
                                        }
                                        else
                                        {
                                            this.SelectedBrickIndices.Add(item);
                                            this.SelectedBricksColored.Add(flag);
                                        }
                                        this.form.SelectBricks(this.SelectedBrickIndices.ToArray(), this.SelectedBricksColored.ToArray());
                                        break;
                                    }
                                }
                                break;
                            }
                            PWB_WhiteBrick brick2 = this.pwb.WhiteBricks[num3];
                            Vector3 vector4 = brick2.Position.toXNAVector();
                            box = this.enhabrick.boundingbox.Transform(Matrix.CreateTranslation(vector4));
                            nullable = input.Intersects(box);
                            if ((nullable != null) && (nullable.Value < maxValue))
                            {
                                maxValue = nullable.Value;
                                flag = false;
                                item = num3;
                            }
                            num3++;
                        }
                        break;
                    }
                    PWB_ColorBrick brick = this.pwb.ColorBricks[num3];
                    Matrix transform = Matrix.CreateTranslation(brick.Position.X, brick.Position.Y, brick.Position.Z);
                    box = this.pupbrick.boundingbox.Transform(transform);
                    nullable = input.Intersects(box);
                    if ((nullable != null) && (nullable.Value < maxValue))
                    {
                        maxValue = nullable.Value;
                        flag = true;
                        item = num3;
                    }
                    num3++;
                }
            }
        }

        public void setOriginalSurface()
        {
            this.drawsurface = this.gameform.Handle;
            this.surfacesize = this.gameform.Size;
        }

        public void setSurface(IntPtr drawSurface, Size size, bool reset = false)
        {
            this.drawsurface = this.pctdrawsurface = drawSurface;
            this.surfacesize = this.pctsize = size;
            if (reset)
            {
                PresentationParameters presentationParameters = base.GraphicsDevice.PresentationParameters;
                presentationParameters.DeviceWindowHandle = this.drawsurface;
                this.width = presentationParameters.BackBufferWidth = this.surfacesize.Width;
                this.height = presentationParameters.BackBufferHeight = this.surfacesize.Height;
                this.graphics.GraphicsDevice.Reset(presentationParameters);
                this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(LR1TrackEditor.Settings.Default.FoV), ((float)this.width) / ((float)this.height), 1f, LR1TrackEditor.Settings.Default.RenderDistance);
            }
        }

        public void ToggleFullscreen()
        {
            PresentationParameters presentationParameters;
            Console.WriteLine("Toggling fullscreen");
            this.fullscreen = !this.fullscreen;
            if (!this.fullscreen)
            {
                presentationParameters = base.GraphicsDevice.PresentationParameters;
                presentationParameters.DeviceWindowHandle = this.pctdrawsurface;
                this.width = presentationParameters.BackBufferWidth = this.pctsize.Width;
                this.height = presentationParameters.BackBufferHeight = this.pctsize.Height;
                this.graphics.GraphicsDevice.Reset(presentationParameters);
                this.drawsurface = this.pctdrawsurface;
                this.mouselock = false;
                base.IsMouseVisible = true;
                this.gameform.Hide();
            }
            else
            {
                this.gameform.Show();
                presentationParameters = base.GraphicsDevice.PresentationParameters;
                presentationParameters.DeviceWindowHandle = this.gameform.Handle;
                this.width = presentationParameters.BackBufferWidth = this.gameform.Size.Width;
                this.height = presentationParameters.BackBufferHeight = this.gameform.Size.Height;
                this.graphics.GraphicsDevice.Reset(presentationParameters);
                this.drawsurface = this.gameform.Handle;
                MouseHelper.SetPosition(this.drawsurface, this.width / 2, this.height / 2);
                base.IsMouseVisible = false;
                this.mouselock = true;
            }
            this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(LR1TrackEditor.Settings.Default.FoV), ((float)this.width) / ((float)this.height), 1f, LR1TrackEditor.Settings.Default.RenderDistance);
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler.handleinput(this);
            this.form.updateCameraPosition(this.cameraPosition);
            Vector3 vector = new Vector3((float)(Math.Sin(3.1415926535897931 * this.Yaw) * Math.Cos(3.1415926535897931 * this.Pitch)), (float)(Math.Cos(3.1415926535897931 * this.Yaw) * Math.Cos(3.1415926535897931 * this.Pitch)), (float)Math.Sin(3.1415926535897931 * this.Pitch));
            this.basicEffect.View = Matrix.CreateLookAt(this.cameraPosition, this.cameraPosition + vector, new Vector3(0f, 0f, 1f));
            Vector3 vector2 = new Vector3(0f, 0f, -5f + (27f * ((float)Math.Min((double)this.Pitch, 0.18))));
            this.backgrnd.View = Matrix.CreateLookAt(new Vector3(0f, 4f, 2.5f) + vector2, new Vector3(0f, 1f, 2.5f) + vector2, new Vector3(0f, 0f, 1f));
            this.brickrotation += 0.04f;
            if (this.brickrotation >= 2f)
            {
                this.brickrotation -= 2f;
            }
            base.Update(gameTime);
        }

        public bool gameFormFocused =>
            this.gameform.ContainsFocus;
    }
}

