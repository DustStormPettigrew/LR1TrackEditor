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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public enum ViewerSelectionType
    {
        None,
        PowerupBrick,
        StartPosition,
        Checkpoint
    }

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
        public bool doDrawCollision = false;
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
        public EVB evb = null;
        public List<EMB> embs = new List<EMB>();
        public List<LoadedEmitterDefinition> emitterDefinitions = new List<LoadedEmitterDefinition>();
        public List<CollisionModelInstance> collisionModels = new List<CollisionModelInstance>();
        public List<WDB> extraWdbScenes = new List<WDB>();
        public Dictionary<WDB, string> scenePaths = new Dictionary<WDB, string>();
        public Dictionary<WDB, List<LoadedMabDefinition>> sceneMabs = new Dictionary<WDB, List<LoadedMabDefinition>>();
        public Dictionary<WDB, List<LoadedAdbDefinition>> sceneAdbs = new Dictionary<WDB, List<LoadedAdbDefinition>>();
        public Dictionary<WDB, List<LoadedSdbDefinition>> sceneSdbs = new Dictionary<WDB, List<LoadedSdbDefinition>>();
        private readonly HashSet<string> animationDiagnosticKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        public List<AnimatedObjectEntry> animatedObjects = new List<AnimatedObjectEntry>();
        public Dictionary<string, AnimatedObjectPlayback> animatedObjectPlaybacks = new Dictionary<string, AnimatedObjectPlayback>(StringComparer.InvariantCultureIgnoreCase);
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
        public ViewerSelectionType selectedViewerObject = ViewerSelectionType.None;
        public int selectedStartPositionKey = -1;
        public int selectedCheckpointIndex = -1;
        private float totalElapsedSeconds = 0f;

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
            this.ClearViewerSelection();
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
                                            Matrix matrix = Matrix.CreateTranslation(brick.Position.X, brick.Position.Y, brick.Position.Z);
                                            this.pupbrick.DrawBoundingBox(this, this.basicEffect, matrix);
                                        }
                                        else
                                        {
                                            PWB_WhiteBrick brick2 = this.pwb.WhiteBricks[this.SelectedBrickIndices[index]];
                                            Matrix matrix = Matrix.CreateTranslation(brick2.Position.X, brick2.Position.Y, brick2.Position.Z);
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
                this.DrawScene(this.wdb);
                foreach (WDB extraScene in this.extraWdbScenes)
                {
                    this.DrawScene(extraScene);
                }
            }
            if (this.collisionModels.Count > 0 && this.doDrawCollision)
            {
                RasterizerState previousRasterizerState = base.GraphicsDevice.RasterizerState;
                DepthStencilState previousDepthStencilState = base.GraphicsDevice.DepthStencilState;
                Material collisionMaterial = new Material
                {
                    ambientcolor = new Microsoft.Xna.Framework.Color(255, 160, 64),
                    diffusecolor = new Microsoft.Xna.Framework.Color(255, 160, 64),
                    alpha = 255
                };
                Material checkpointCollisionMaterial = new Material
                {
                    ambientcolor = new Microsoft.Xna.Framework.Color(64, 224, 255),
                    diffusecolor = new Microsoft.Xna.Framework.Color(64, 224, 255),
                    alpha = 255
                };
                RasterizerState collisionRasterizerState = new RasterizerState
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.WireFrame
                };
                base.GraphicsDevice.RasterizerState = collisionRasterizerState;
                base.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                foreach (CollisionModelInstance collisionModel in this.collisionModels)
                {
                    Material resolvedCollisionMaterial = collisionModel?.IsCheckpointTrigger == true
                        ? checkpointCollisionMaterial
                        : collisionMaterial;
                    collisionModel.Model?.Draw(this, this.basicEffect, collisionModel.Transform, resolvedCollisionMaterial);
                }
                base.GraphicsDevice.DepthStencilState = previousDepthStencilState;
                base.GraphicsDevice.RasterizerState = previousRasterizerState;
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

            // Draw EMB emitter positions and textured billboards when supported.
            if (this.embs.Count > 0 && this.doDrawEMB)
            {
                List<Vector3> positions = new List<Vector3>();
                foreach (LoadedEmitterDefinition emitterDefinition in this.emitterDefinitions)
                {
                    if (emitterDefinition?.Source?.Emitters == null)
                    {
                        continue;
                    }

                    foreach (var kvp in emitterDefinition.Source.Emitters)
                    {
                        EMB_Emitter em = kvp.Value;
                        Vector3 anchorPosition = this.GetEmitterAnchorPosition(kvp.Key);
                        if (em?.Positions != null)
                        {
                            foreach (LRVector3 pos2 in em.Positions)
                            {
                                Vector3 worldPosition = anchorPosition + new Vector3(pos2.X, pos2.Y, pos2.Z);
                                positions.Add(worldPosition);
                                this.DrawEmitterBillboard(worldPosition, em, emitterDefinition);
                            }
                        }
                    }
                }
                DrawPositionMarkers(positions, new Microsoft.Xna.Framework.Color(0, 200, 255), 4f);
            }

            Vector3? selectedViewerPosition = this.GetSelectedViewerPosition();
            if (selectedViewerPosition != null)
            {
                Microsoft.Xna.Framework.Color selectionColor = this.selectedViewerObject == ViewerSelectionType.StartPosition
                    ? new Microsoft.Xna.Framework.Color(0, 255, 0)
                    : new Microsoft.Xna.Framework.Color(255, 255, 0);
                this.DrawPositionMarkers(new List<Vector3> { selectedViewerPosition.Value }, selectionColor, 12f);
                base.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                this.DrawMoveArrows(selectedViewerPosition.Value);
                base.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
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

        private Vector3 GetEmitterAnchorPosition(string emitterName)
        {
            if (string.IsNullOrWhiteSpace(emitterName) || this.evb?.Models == null)
            {
                return Vector3.Zero;
            }

            foreach (KeyValuePair<int, EVB_Model> modelEntry in this.evb.Models)
            {
                EVB_Model model = modelEntry.Value;
                if (model == null ||
                    !model.HasPosition ||
                    !string.Equals(model.Name, emitterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                return model.Position.toXNAVector();
            }

            return Vector3.Zero;
        }

        private void DrawEmitterBillboard(Vector3 center, EMB_Emitter emitter, LoadedEmitterDefinition emitterDefinition)
        {
            Material material = this.ResolveEmitterMaterial(emitter, emitterDefinition);
            if (material?.texture == null)
            {
                return;
            }

            float size = Math.Max(emitter?.Size ?? 0f, 1f);
            Vector3 worldUp = new Vector3(0f, 0f, 1f);
            Vector3 toCamera = this.cameraPosition - center;
            if (toCamera.LengthSquared() <= 0.0001f)
            {
                return;
            }

            toCamera.Normalize();
            Vector3 right = Vector3.Cross(worldUp, toCamera);
            if (right.LengthSquared() <= 0.0001f)
            {
                right = Vector3.Right;
            }
            else
            {
                right.Normalize();
            }

            Vector3 up = Vector3.Cross(toCamera, right);
            if (up.LengthSquared() <= 0.0001f)
            {
                up = worldUp;
            }
            else
            {
                up.Normalize();
            }

            float halfWidth = size * 0.5f;
            float halfHeight = size * 0.5f;
            Vector3 topLeft = center - (right * halfWidth) + (up * halfHeight);
            Vector3 topRight = center + (right * halfWidth) + (up * halfHeight);
            Vector3 bottomLeft = center - (right * halfWidth) - (up * halfHeight);
            Vector3 bottomRight = center + (right * halfWidth) - (up * halfHeight);
            Microsoft.Xna.Framework.Color tint = material.diffusecolor == default ? Microsoft.Xna.Framework.Color.White : material.diffusecolor;
            VertexPTC[] vertices = new VertexPTC[]
            {
                new VertexPTC(topLeft, new Vector2(0f, 0f), tint),
                new VertexPTC(bottomLeft, new Vector2(0f, 1f), tint),
                new VertexPTC(topRight, new Vector2(1f, 0f), tint),
                new VertexPTC(topRight, new Vector2(1f, 0f), tint),
                new VertexPTC(bottomLeft, new Vector2(0f, 1f), tint),
                new VertexPTC(bottomRight, new Vector2(1f, 1f), tint)
            };

            this.basicEffect.World = Matrix.Identity;
            this.basicEffect.TextureEnabled = this.doTextures;
            this.basicEffect.Texture = material.texture;
            this.basicEffect.VertexColorEnabled = true;
            this.basicEffect.Alpha = ((float)material.alpha) / 255f;
            this.basicEffect.AmbientLightColor = Utils.vectorfromcolor(material.ambientcolor);
            this.basicEffect.DiffuseColor = Vector3.One;
            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                base.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.TriangleList, vertices, 0, 2, VertexPTC.VertexDeclaration);
            }
        }

        private Material ResolveEmitterMaterial(EMB_Emitter emitter, LoadedEmitterDefinition emitterDefinition)
        {
            if (emitterDefinition == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(emitter?.Texture))
            {
                if (emitterDefinition.Materials.TryGetValue(emitter.Texture, out Material explicitMaterial))
                {
                    return explicitMaterial;
                }

                foreach (Material candidate in emitterDefinition.Materials.Values)
                {
                    if (string.Equals(candidate.textureName, emitter.Texture, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return candidate;
                    }
                }
            }

            if (emitterDefinition.MaterialAnimations.Count == 1)
            {
                MabAnimationDefinition animation = emitterDefinition.MaterialAnimations[0].Animations.FirstOrDefault();
                if (animation != null && animation.SequenceFrames.Count > 0)
                {
                    int frameIndex = (int)(this.totalElapsedSeconds * Math.Max(animation.Speed, 1));
                    foreach (MabFrameDefinition frame in animation.GetPlaybackFrame(frameIndex))
                    {
                        if (string.IsNullOrWhiteSpace(frame.MaterialName))
                        {
                            continue;
                        }

                        if (emitterDefinition.Materials.TryGetValue(frame.MaterialName, out Material animatedMaterial))
                        {
                            return animatedMaterial;
                        }

                        foreach (Material candidate in emitterDefinition.Materials.Values)
                        {
                            if (string.Equals(candidate.textureName, frame.MaterialName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                return candidate;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(emitter?.Texture) &&
                            emitterDefinition.Materials.TryGetValue(emitter.Texture, out Material baseAnimatedMaterial))
                        {
                            Material resolvedFrameMaterial = Loader.ResolveAnimatedMaterialFrame(baseAnimatedMaterial, base.GraphicsDevice, frame.FrameIndex);
                            if (resolvedFrameMaterial != null)
                            {
                                return resolvedFrameMaterial;
                            }
                        }
                    }
                }
            }

            return null;
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
            this.basicEffect = new BasicEffect(base.GraphicsDevice);
            this.RefreshProjectionSettings();
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
            this.ClearTrackData();
            DisposeModelBuffers(this.loadedmodel);
            this.loadedmodel = null;
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
                    this.form.MarkPwbEdited();
                }
                if (!keepplacing)
                {
                    this.form.BrickplaceStopButton_Click(null, null);
                }
            }
        }

        public void Reload()
        {
            if (this.currentRABfile != "")
            {
                string rabpath = this.currentRABfile;
                this.ClearTrackData();
                this.form.refreshPWB(false);
                this.form.refreshRRB();
                Loader.loadRAB(this, rabpath);
                this.track = true;
                this.form.refreshSKB();
                this.form.refreshWDB();
                this.form.PWBToolStripItemChecked = this.pwb != null;
                this.form.staticObjectsToolStripItemChecked = this.wdb != null;
                this.form.SetTabControlEnabled(true);
                return;
            }
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
                        current.generatePoints();
                    }
                }
            }
        }

        private static void DisposeModelBuffers(LR1TrackEditor.Model model)
        {
            model?.vertexbuffer?.Dispose();
            model?.indexbuffer?.Dispose();
        }

        public void RefreshProjectionSettings()
        {
            if (this.basicEffect == null || this.height <= 0 || this.width <= 0)
            {
                return;
            }

            float nearPlane = 1f;
            float farPlane = Math.Max(LR1TrackEditor.Settings.Default.RenderDistance, nearPlane + 0.01f);
            float aspectRatio = ((float)this.width) / ((float)this.height);
            this.basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(LR1TrackEditor.Settings.Default.FoV), aspectRatio, nearPlane, farPlane);
        }

        public void RegisterSceneResources(WDB scene, string scenePath, List<LoadedMabDefinition> loadedMabs, List<LoadedAdbDefinition> loadedAdbs, List<LoadedSdbDefinition> loadedSdbs, bool clearExisting)
        {
            if (clearExisting)
            {
                this.scenePaths.Clear();
                this.sceneMabs.Clear();
                this.sceneAdbs.Clear();
                this.sceneSdbs.Clear();
                this.animatedObjects.Clear();
                this.animatedObjectPlaybacks.Clear();
            }

            if (scene == null)
            {
                return;
            }

            this.scenePaths[scene] = scenePath ?? string.Empty;
            this.sceneMabs[scene] = loadedMabs ?? new List<LoadedMabDefinition>();
            this.sceneAdbs[scene] = loadedAdbs ?? new List<LoadedAdbDefinition>();
            this.sceneSdbs[scene] = loadedSdbs ?? new List<LoadedSdbDefinition>();
        }

        public void ClearViewerSelection()
        {
            this.selectedViewerObject = ViewerSelectionType.None;
            this.selectedStartPositionKey = -1;
            this.selectedCheckpointIndex = -1;
        }

        public bool HasDraggableSelection()
        {
            return ((this.editmode == 1) && (this.SelectedBrickIndices.Count > 0)) ||
                   (this.selectedViewerObject != ViewerSelectionType.None);
        }

        public Vector3? GetSelectedViewerPosition()
        {
            if (this.selectedViewerObject == ViewerSelectionType.StartPosition &&
                this.spb?.StartPositions != null &&
                this.spb.StartPositions.ContainsKey(this.selectedStartPositionKey))
            {
                LRVector3 position = this.spb.StartPositions[this.selectedStartPositionKey].Position;
                return new Vector3(position.X, position.Y, position.Z);
            }

            if (this.selectedViewerObject == ViewerSelectionType.Checkpoint &&
                this.cpb?.Checkpoints != null &&
                this.selectedCheckpointIndex >= 0 &&
                this.selectedCheckpointIndex < this.cpb.Checkpoints.Length)
            {
                LRVector3 position = this.cpb.Checkpoints[this.selectedCheckpointIndex].Location;
                return new Vector3(position.X, position.Y, position.Z);
            }

            return null;
        }

        public void ApplyViewerDragDelta(int axis, float delta)
        {
            if (this.selectedViewerObject == ViewerSelectionType.StartPosition &&
                this.spb?.StartPositions != null &&
                this.spb.StartPositions.ContainsKey(this.selectedStartPositionKey))
            {
                LRVector3 position = this.spb.StartPositions[this.selectedStartPositionKey].Position;
                if (axis == 1)
                {
                    position.X += delta;
                }
                else if (axis == 2)
                {
                    position.Y += delta;
                }
                else if (axis == 3)
                {
                    position.Z += delta;
                }
                this.spb.StartPositions[this.selectedStartPositionKey].Position = position;
            }
            else if (this.selectedViewerObject == ViewerSelectionType.Checkpoint &&
                     this.cpb?.Checkpoints != null &&
                     this.selectedCheckpointIndex >= 0 &&
                     this.selectedCheckpointIndex < this.cpb.Checkpoints.Length)
            {
                LRVector3 position = this.cpb.Checkpoints[this.selectedCheckpointIndex].Location;
                if (axis == 1)
                {
                    position.X += delta;
                }
                else if (axis == 2)
                {
                    position.Y += delta;
                }
                else if (axis == 3)
                {
                    position.Z += delta;
                }
                this.cpb.Checkpoints[this.selectedCheckpointIndex].Location = position;
            }
        }

        private string GetScenePath(WDB scene)
        {
            return scene != null && this.scenePaths.ContainsKey(scene) ? this.scenePaths[scene] : string.Empty;
        }

        private string GetSceneName(WDB scene)
        {
            string scenePath = this.GetScenePath(scene);
            return string.IsNullOrWhiteSpace(scenePath) ? "Scene" : Path.GetFileNameWithoutExtension(scenePath);
        }

        private string GetAnimatedObjectId(WDB scene, string objectName)
        {
            string scenePath = this.GetScenePath(scene);
            string sceneName = this.GetSceneName(scene);
            return (scenePath ?? sceneName) + "::" + objectName;
        }

        private List<string> GetModelMaterialNames(string modelName)
        {
            if (string.IsNullOrWhiteSpace(modelName) || !this.models.ContainsKey(modelName))
            {
                return new List<string>();
            }

            return this.models[modelName].parts
                .Where(part => !string.IsNullOrWhiteSpace(part.material))
                .Select(part => part.material)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToList();
        }

        private List<MabAnimationDefinition> GetAnimationsForMaterials(List<LoadedMabDefinition> loadedMabs, IEnumerable<string> materialNames)
        {
            HashSet<string> materialSet = new HashSet<string>(materialNames ?? Enumerable.Empty<string>(), StringComparer.InvariantCultureIgnoreCase);
            List<MabAnimationDefinition> matches = new List<MabAnimationDefinition>();
            if (loadedMabs == null || loadedMabs.Count == 0)
            {
                return matches;
            }

            foreach (LoadedMabDefinition mab in loadedMabs)
            {
                foreach (MabAnimationDefinition animation in mab.Animations)
                {
                    if (animation == null)
                    {
                        continue;
                    }

                    if (animation.ReferencedMaterials.Count == 0 || animation.ReferencedMaterials.Overlaps(materialSet))
                    {
                        matches.Add(animation);
                    }
                }
            }

            return matches;
        }

        private LoadedAdbDefinition GetLoadedAdb(List<LoadedAdbDefinition> loadedAdbs, int? adbIndex)
        {
            if (!adbIndex.HasValue || loadedAdbs == null)
            {
                return null;
            }

            int index = adbIndex.Value;
            return index >= 0 && index < loadedAdbs.Count ? loadedAdbs[index] : null;
        }

        private LoadedSdbDefinition GetLoadedSdb(List<LoadedSdbDefinition> loadedSdbs, int? sdbIndex)
        {
            if (!sdbIndex.HasValue || loadedSdbs == null)
            {
                return null;
            }

            int index = sdbIndex.Value;
            return index >= 0 && index < loadedSdbs.Count ? loadedSdbs[index] : null;
        }

        public int GetAnimatedSceneModelCount()
        {
            int count = 0;
            if (this.wdb?.AnimatedModels != null)
            {
                count += this.wdb.AnimatedModels.Count;
            }

            foreach (WDB scene in this.extraWdbScenes.Where(scene => scene?.AnimatedModels != null))
            {
                count += scene.AnimatedModels.Count;
            }

            return count;
        }

        public void RebuildAnimatedObjects()
        {
            this.animatedObjects.Clear();
            List<WDB> scenes = new List<WDB>();
            if (this.wdb != null)
            {
                scenes.Add(this.wdb);
            }
            scenes.AddRange(this.extraWdbScenes.Where(scene => scene != null));

            foreach (WDB scene in scenes)
            {
                string scenePath = this.GetScenePath(scene);
                string sceneName = this.GetSceneName(scene);
                List<LoadedMabDefinition> loadedMabs = this.sceneMabs.ContainsKey(scene) ? this.sceneMabs[scene] : new List<LoadedMabDefinition>();
                List<LoadedAdbDefinition> loadedAdbs = this.sceneAdbs.ContainsKey(scene) ? this.sceneAdbs[scene] : new List<LoadedAdbDefinition>();
                List<LoadedSdbDefinition> loadedSdbs = this.sceneSdbs.ContainsKey(scene) ? this.sceneSdbs[scene] : new List<LoadedSdbDefinition>();

                void AddEntry(string objectName, string modelName, Matrix worldMatrix, AnimatedObjectType objectType, int? adbIndex = null, int? sdbIndex = null)
                {
                    if (string.IsNullOrWhiteSpace(objectName) || string.IsNullOrWhiteSpace(modelName) || !this.models.ContainsKey(modelName))
                    {
                        return;
                    }

                    List<string> materialNames = this.GetModelMaterialNames(modelName);
                    List<MabAnimationDefinition> matchingAnimations = this.GetAnimationsForMaterials(loadedMabs, materialNames);
                    LoadedAdbDefinition linkedAdb = this.GetLoadedAdb(loadedAdbs, adbIndex);
                    LoadedSdbDefinition linkedSdb = this.GetLoadedSdb(loadedSdbs, sdbIndex);

                    AnimatedObjectEntry entry = new AnimatedObjectEntry
                    {
                        Id = this.GetAnimatedObjectId(scene, objectName),
                        ObjectName = objectName,
                        SceneName = sceneName,
                        ModelName = modelName,
                        SourcePath = scenePath ?? string.Empty,
                        SceneKey = scenePath ?? sceneName,
                        ObjectType = objectType,
                        WorldMatrix = worldMatrix,
                        Scene = scene,
                        AdbIndex = adbIndex,
                        SdbIndex = sdbIndex,
                        AdbDefinition = linkedAdb,
                        SdbDefinition = linkedSdb
                    };

                    foreach (string materialName in materialNames)
                    {
                        entry.MaterialNames.Add(materialName);
                    }

                    foreach (MabAnimationDefinition animation in matchingAnimations)
                    {
                        entry.Animations.Add(animation);
                        entry.AvailableAnimations.Add(new AnimatedObjectAnimationOption
                        {
                            Id = entry.Id + "::mat::" + animation.Id,
                            DisplayName = "Material: " + animation.DisplayName,
                            Kind = AnimatedObjectAnimationKind.Material,
                            MaterialAnimation = animation
                        });
                    }

                    if (linkedAdb?.Animations != null)
                    {
                        foreach (AdbAnimationDefinition animation in linkedAdb.Animations)
                        {
                            entry.AvailableAnimations.Add(new AnimatedObjectAnimationOption
                            {
                                Id = entry.Id + "::adb::" + animation.Id,
                                DisplayName = "Transform: " + animation.DisplayName,
                                Kind = AnimatedObjectAnimationKind.Transform,
                                TransformAnimation = animation
                            });
                        }
                    }

                    if (entry.AvailableAnimations.Count == 0)
                    {
                        return;
                    }

                    this.animatedObjects.Add(entry);
                }

                foreach (KeyValuePair<string, WDB_StaticModel> current in scene.StaticModels)
                {
                    if (current.Value?.ModelRef == null)
                    {
                        continue;
                    }

                    int gdbIndex = current.Value.ModelRef.IndexGDB;
                    if (gdbIndex < 0 || gdbIndex >= scene.GDBs.Length)
                    {
                        continue;
                    }

                    AddEntry(
                        current.Key,
                        scene.GDBs[gdbIndex],
                        CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp),
                        AnimatedObjectType.StaticModel);
                }

                foreach (KeyValuePair<string, WDB_BDBModel> current in scene.BDBModels)
                {
                    if (current.Value?.ModelRef == null)
                    {
                        continue;
                    }

                    int gdbIndex = current.Value.ModelRef.IndexGDB;
                    if (gdbIndex < 0 || gdbIndex >= scene.GDBs.Length)
                    {
                        continue;
                    }

                    AddEntry(
                        current.Key,
                        scene.GDBs[gdbIndex],
                        CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp),
                        AnimatedObjectType.BdbModel);
                }

                foreach (KeyValuePair<string, WDB_AnimatedModel> current in scene.AnimatedModels)
                {
                    if (current.Value?.ModelRef == null || !current.Value.ModelRef.IndexGDB.HasValue)
                    {
                        continue;
                    }

                    int gdbIndex = current.Value.ModelRef.IndexGDB.Value;
                    if (gdbIndex < 0 || gdbIndex >= scene.GDBs.Length)
                    {
                        continue;
                    }

                    AddEntry(
                        current.Key,
                        scene.GDBs[gdbIndex],
                        CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp),
                        AnimatedObjectType.AnimatedModel,
                        current.Value.ModelRef.IndexADB,
                        current.Value.ModelRef.IndexSDB);
                }
            }

            HashSet<string> validIds = new HashSet<string>(this.animatedObjects.Select(entry => entry.Id), StringComparer.InvariantCultureIgnoreCase);
            foreach (string key in this.animatedObjectPlaybacks.Keys.Where(key => !validIds.Contains(key)).ToList())
            {
                this.animatedObjectPlaybacks.Remove(key);
            }
        }

        public void PlayAnimatedObject(AnimatedObjectEntry entry, AnimatedObjectAnimationOption animationOption, bool loop)
        {
            if (entry == null || animationOption == null)
            {
                return;
            }

            this.animatedObjectPlaybacks[entry.Id] = new AnimatedObjectPlayback
            {
                Entry = entry,
                AnimationOption = animationOption,
                Loop = loop,
                ElapsedSeconds = 0f
            };
        }

        private int GetAnimationSequenceStep(AnimatedObjectPlayback playback)
        {
            MabAnimationDefinition animation = playback?.MaterialAnimation;
            if (animation == null || animation.SequenceFrames.Count == 0)
            {
                return 0;
            }

            int logicalFrameCount = Math.Max(animation.LogicalFrameCount, 1);
            int currentFrame = playback.GetCurrentFrameIndex();
            if (logicalFrameCount <= 1 || animation.SequenceFrames.Count == 1)
            {
                return 0;
            }

            return Math.Min((currentFrame * animation.SequenceFrames.Count) / logicalFrameCount, animation.SequenceFrames.Count - 1);
        }

        private Material ResolveAnimatedOverrideMaterial(string baseMaterialName, MabFrameDefinition frame)
        {
            if (frame == null)
            {
                return null;
            }

            Material baseMaterial = null;
            if (!string.IsNullOrWhiteSpace(baseMaterialName))
            {
                this.materials.TryGetValue(baseMaterialName, out baseMaterial);
            }

            if (baseMaterial != null &&
                string.Equals(baseMaterialName, frame.MaterialName, StringComparison.InvariantCultureIgnoreCase))
            {
                return Loader.ResolveAnimatedMaterialFrame(baseMaterial, base.GraphicsDevice, frame.FrameIndex);
            }

            if (!string.IsNullOrWhiteSpace(frame.MaterialName) &&
                this.materials.TryGetValue(frame.MaterialName, out Material swappedMaterial))
            {
                return swappedMaterial;
            }

            return baseMaterial == null
                ? null
                : Loader.ResolveAnimatedMaterialFrame(baseMaterial, base.GraphicsDevice, frame.FrameIndex);
        }

        private Dictionary<string, Material> GetAnimatedMaterialOverrides(AnimatedObjectEntry entry)
        {
            if (entry == null || !this.animatedObjectPlaybacks.TryGetValue(entry.Id, out AnimatedObjectPlayback playback))
            {
                return null;
            }

            MabAnimationDefinition animation = playback.MaterialAnimation;
            if (animation == null)
            {
                return null;
            }

            List<string> animatedMaterials = entry.MaterialNames
                .Where(name => !string.IsNullOrWhiteSpace(name) && animation.ReferencedMaterials.Contains(name))
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            if (animatedMaterials.Count == 0)
            {
                animatedMaterials = animation.SequenceFrames
                    .Select(frame => frame.MaterialName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .ToList();
            }

            if (animatedMaterials.Count == 0 || animation.SequenceFrames.Count == 0)
            {
                return null;
            }

            Dictionary<string, Material> overrides = new Dictionary<string, Material>(StringComparer.InvariantCultureIgnoreCase);
            int animationStep = this.GetAnimationSequenceStep(playback);
            for (int materialIndex = 0; materialIndex < animatedMaterials.Count; materialIndex++)
            {
                MabFrameDefinition frame = animation.SequenceFrames[(animationStep + materialIndex) % animation.SequenceFrames.Count];
                string baseMaterialName = animatedMaterials[materialIndex];
                Material resolvedMaterial = this.ResolveAnimatedOverrideMaterial(baseMaterialName, frame);
                if (resolvedMaterial == null)
                {
                    continue;
                }

                overrides[baseMaterialName] = resolvedMaterial;
            }

            return overrides.Count == 0 ? null : overrides;
        }

        private static Quaternion ToXnaAnimationQuaternion(LRQuaternion rotation)
        {
            if (rotation == null)
            {
                return Quaternion.Identity;
            }

            Quaternion quaternion = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            if (quaternion.LengthSquared() <= 0f)
            {
                return Quaternion.Identity;
            }

            // LR1 quaternion winding is inverted relative to the XNA transform usage here.
            return Quaternion.Conjugate(Quaternion.Normalize(quaternion));
        }

        private static Quaternion ToXnaBindQuaternion(LRQuaternion rotation)
        {
            if (rotation == null)
            {
                return Quaternion.Identity;
            }

            Quaternion quaternion = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            return quaternion.LengthSquared() <= 0f ? Quaternion.Identity : Quaternion.Normalize(quaternion);
        }

        private static float NormalizeAnimationFrameTime(float frameTime, int logicalFrameCount)
        {
            int frameCount = Math.Max(logicalFrameCount, 1);
            if (frameCount <= 1)
            {
                return 0f;
            }

            float normalized = frameTime % frameCount;
            if (normalized < 0f)
            {
                normalized += frameCount;
            }

            return normalized;
        }

        private static bool TryGetAdbSampleRange(int[] timeOffsets, int startOffset, int sampleLength, float frameTime, int logicalFrameCount, bool loop, out int lowerIndex, out int upperIndex, out float interpolationAmount)
        {
            lowerIndex = 0;
            upperIndex = 0;
            interpolationAmount = 0f;

            if (sampleLength <= 0)
            {
                return false;
            }

            if (sampleLength == 1)
            {
                return true;
            }

            float normalizedFrameTime = loop
                ? NormalizeAnimationFrameTime(frameTime, logicalFrameCount)
                : MathHelper.Clamp(frameTime, 0f, Math.Max(logicalFrameCount - 1, 0));

            if (timeOffsets == null || startOffset < 0 || startOffset >= timeOffsets.Length)
            {
                float clampedSampleTime = loop
                    ? NormalizeAnimationFrameTime(frameTime, sampleLength)
                    : MathHelper.Clamp(frameTime, 0f, sampleLength - 1);
                lowerIndex = Math.Min(Math.Max((int)Math.Floor(clampedSampleTime), 0), sampleLength - 1);
                upperIndex = loop
                    ? (lowerIndex + 1) % sampleLength
                    : Math.Min(lowerIndex + 1, sampleLength - 1);
                interpolationAmount = MathHelper.Clamp(clampedSampleTime - lowerIndex, 0f, 1f);
                if (upperIndex == lowerIndex)
                {
                    interpolationAmount = 0f;
                }

                return true;
            }

            int clampedLength = Math.Min(sampleLength, timeOffsets.Length - startOffset);
            if (clampedLength <= 0)
            {
                return false;
            }

            if (clampedLength == 1)
            {
                return true;
            }

            int firstTime = timeOffsets[startOffset];
            if (normalizedFrameTime <= firstTime)
            {
                if (!loop || normalizedFrameTime == firstTime)
                {
                    return true;
                }

                int lastIndex = clampedLength - 1;
                int lastTime = timeOffsets[startOffset + lastIndex];
                float wrappedTime = normalizedFrameTime + logicalFrameCount;
                float span = (firstTime + logicalFrameCount) - lastTime;
                if (span <= 0f)
                {
                    lowerIndex = lastIndex;
                    upperIndex = 0;
                    return true;
                }

                lowerIndex = lastIndex;
                upperIndex = 0;
                interpolationAmount = MathHelper.Clamp((wrappedTime - lastTime) / span, 0f, 1f);
                return true;
            }

            for (int i = 0; i < clampedLength - 1; i++)
            {
                int currentTime = timeOffsets[startOffset + i];
                int nextTime = timeOffsets[startOffset + i + 1];
                if (normalizedFrameTime > nextTime)
                {
                    continue;
                }

                lowerIndex = i;
                upperIndex = i + 1;
                int span = nextTime - currentTime;
                if (span > 0)
                {
                    interpolationAmount = MathHelper.Clamp((normalizedFrameTime - currentTime) / span, 0f, 1f);
                }

                return true;
            }

            lowerIndex = clampedLength - 1;
            upperIndex = lowerIndex;
            interpolationAmount = 0f;

            if (loop)
            {
                int lastTime = timeOffsets[startOffset + lowerIndex];
                float span = (firstTime + logicalFrameCount) - lastTime;
                if (span > 0f)
                {
                    upperIndex = 0;
                    float wrappedTime = normalizedFrameTime < lastTime
                        ? normalizedFrameTime + logicalFrameCount
                        : normalizedFrameTime;
                    interpolationAmount = MathHelper.Clamp((wrappedTime - lastTime) / span, 0f, 1f);
                }
            }

            return true;
        }

        private static bool TrySampleAdbPosition(ADB adb, ADB_Pointer pointer, float frameTime, int logicalFrameCount, bool loop, out Vector3 sampledPosition)
        {
            sampledPosition = Vector3.Zero;
            if (adb?.Data?.PositionOffsets == null || pointer.PositionLength <= 0)
            {
                return false;
            }

            if (!TryGetAdbSampleRange(adb.Data.TimeOffsets, pointer.PositionTimeOffset, pointer.PositionLength, frameTime, logicalFrameCount, loop, out int lowerIndex, out int upperIndex, out float interpolationAmount))
            {
                return false;
            }

            int lowerDataIndex = pointer.PositionOffset + lowerIndex;
            int upperDataIndex = pointer.PositionOffset + upperIndex;
            if (lowerDataIndex < 0 || lowerDataIndex >= adb.Data.PositionOffsets.Length ||
                upperDataIndex < 0 || upperDataIndex >= adb.Data.PositionOffsets.Length)
            {
                return false;
            }

            Vector3 lowerPosition = adb.Data.PositionOffsets[lowerDataIndex].toXNAVector();
            Vector3 upperPosition = adb.Data.PositionOffsets[upperDataIndex].toXNAVector();
            sampledPosition = lowerIndex == upperIndex
                ? lowerPosition
                : Vector3.Lerp(lowerPosition, upperPosition, interpolationAmount);
            return true;
        }

        private static bool TrySampleAdbRotation(ADB adb, ADB_Pointer pointer, float frameTime, int logicalFrameCount, bool loop, out Quaternion sampledRotation)
        {
            sampledRotation = Quaternion.Identity;
            if (adb?.Data?.Transforms == null || pointer.TransformLength <= 0)
            {
                return false;
            }

            if (!TryGetAdbSampleRange(adb.Data.TimeOffsets, pointer.TransformTimeOffset, pointer.TransformLength, frameTime, logicalFrameCount, loop, out int lowerIndex, out int upperIndex, out float interpolationAmount))
            {
                return false;
            }

            int lowerDataIndex = pointer.TransformOffset + lowerIndex;
            int upperDataIndex = pointer.TransformOffset + upperIndex;
            if (lowerDataIndex < 0 || lowerDataIndex >= adb.Data.Transforms.Length ||
                upperDataIndex < 0 || upperDataIndex >= adb.Data.Transforms.Length)
            {
                return false;
            }

            Quaternion lowerRotation = ToXnaAnimationQuaternion(adb.Data.Transforms[lowerDataIndex]);
            Quaternion upperRotation = ToXnaAnimationQuaternion(adb.Data.Transforms[upperDataIndex]);
            sampledRotation = lowerIndex == upperIndex
                ? lowerRotation
                : Quaternion.Normalize(Quaternion.Slerp(lowerRotation, upperRotation, interpolationAmount));
            return true;
        }

        private bool TryGetAnimatedObjectTransform(AnimatedObjectEntry entry, out AnimatedObjectPlayback playback, out ADB adb, out ADB_Meta meta, out float frameTime)
        {
            playback = null;
            adb = null;
            meta = null;
            frameTime = 0f;

            if (entry == null ||
                !this.animatedObjectPlaybacks.TryGetValue(entry.Id, out playback) ||
                playback.TransformAnimation?.SourceDefinition?.Source == null ||
                playback.TransformAnimation.Meta == null)
            {
                return false;
            }

            adb = playback.TransformAnimation.SourceDefinition.Source;
            meta = playback.TransformAnimation.Meta;
            if (adb.Pointers == null || meta.PointerTableOffset < 0 || meta.PointerTableOffset >= adb.Pointers.Length)
            {
                return false;
            }

            frameTime = playback.GetCurrentFrameTime();
            return true;
        }

        private Matrix GetAnimatedObjectMatrix(AnimatedObjectEntry entry)
        {
            if (!this.TryGetAnimatedObjectTransform(entry, out AnimatedObjectPlayback playback, out ADB adb, out ADB_Meta meta, out float frameTime))
            {
                return entry?.WorldMatrix ?? Matrix.Identity;
            }

            ADB_Pointer rootPointer = adb.Pointers[meta.PointerTableOffset];
            int logicalFrameCount = Math.Max(meta.Length, 1);

            Vector3 animatedPosition = meta.InitialPosition?.toXNAVector() ?? Vector3.Zero;
            if (TrySampleAdbPosition(adb, rootPointer, frameTime, logicalFrameCount, playback.Loop, out Vector3 sampledPosition))
            {
                animatedPosition += sampledPosition;
            }

            Quaternion animatedRotation = ToXnaAnimationQuaternion(meta.InitialQuaternion);
            if (TrySampleAdbRotation(adb, rootPointer, frameTime, logicalFrameCount, playback.Loop, out Quaternion sampledRotation))
            {
                animatedRotation = Quaternion.Normalize(animatedRotation * sampledRotation);
            }

            Matrix animationMatrix = Matrix.CreateFromQuaternion(animatedRotation) * Matrix.CreateTranslation(animatedPosition);
            return animationMatrix * entry.WorldMatrix;
        }

        private Dictionary<ushort, Matrix> GetAnimatedBoneTransforms(AnimatedObjectEntry entry)
        {
            if (!this.TryGetAnimatedObjectTransform(entry, out AnimatedObjectPlayback playback, out ADB adb, out ADB_Meta meta, out float frameTime) ||
                entry?.SdbDefinition?.Source?.Bones == null ||
                entry.SdbDefinition.Source.Bones.Count == 0)
            {
                return null;
            }

            List<KeyValuePair<string, SDB_Bone>> orderedBones = entry.SdbDefinition.Source.Bones.ToList();
            Dictionary<string, ushort> boneIndices = new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);
            for (ushort boneIndex = 0; boneIndex < orderedBones.Count; boneIndex++)
            {
                boneIndices[orderedBones[boneIndex].Key] = boneIndex;
            }

            Dictionary<ushort, Matrix> bindWorldTransforms = new Dictionary<ushort, Matrix>();
            Dictionary<ushort, Matrix> animatedWorldTransforms = new Dictionary<ushort, Matrix>();
            Dictionary<ushort, Matrix> boneTransforms = new Dictionary<ushort, Matrix>();
            int logicalFrameCount = Math.Max(meta.Length, 1);
            for (ushort boneIndex = 0; boneIndex < orderedBones.Count; boneIndex++)
            {
                KeyValuePair<string, SDB_Bone> boneEntry = orderedBones[boneIndex];
                SDB_Bone bone = boneEntry.Value;
                Quaternion bindLocalRotation = ToXnaBindQuaternion(bone?.Transform);
                Vector3 bindLocalPosition = bone?.Position?.toXNAVector() ?? Vector3.Zero;
                Quaternion animatedLocalRotation = bindLocalRotation;
                Vector3 animatedLocalPosition = bindLocalPosition;

                int pointerIndex = meta.PointerTableOffset + boneIndex;
                if (boneIndex != 0 && pointerIndex >= 0 && pointerIndex < adb.Pointers.Length)
                {
                    ADB_Pointer pointer = adb.Pointers[pointerIndex];
                    if (TrySampleAdbPosition(adb, pointer, frameTime, logicalFrameCount, playback.Loop, out Vector3 sampledPosition))
                    {
                        animatedLocalPosition += sampledPosition;
                    }

                    if (TrySampleAdbRotation(adb, pointer, frameTime, logicalFrameCount, playback.Loop, out Quaternion sampledRotation))
                    {
                        animatedLocalRotation = Quaternion.Normalize(bindLocalRotation * sampledRotation);
                    }
                }

                Matrix bindLocalTransform = Matrix.CreateFromQuaternion(bindLocalRotation) * Matrix.CreateTranslation(bindLocalPosition);
                Matrix animatedLocalTransform = Matrix.CreateFromQuaternion(animatedLocalRotation) * Matrix.CreateTranslation(animatedLocalPosition);
                Matrix parentBindTransform = Matrix.Identity;
                Matrix parentAnimatedTransform = Matrix.Identity;
                if (bone != null &&
                    bone.HasParent &&
                    !string.IsNullOrWhiteSpace(bone.ParentBone) &&
                    boneIndices.TryGetValue(bone.ParentBone, out ushort parentBoneIndex) &&
                    bindWorldTransforms.TryGetValue(parentBoneIndex, out Matrix resolvedParentBindTransform) &&
                    animatedWorldTransforms.TryGetValue(parentBoneIndex, out Matrix resolvedParentAnimatedTransform))
                {
                    parentBindTransform = resolvedParentBindTransform;
                    parentAnimatedTransform = resolvedParentAnimatedTransform;
                }

                Matrix bindWorldTransform = bindLocalTransform * parentBindTransform;
                Matrix animatedWorldTransform = animatedLocalTransform * parentAnimatedTransform;
                bindWorldTransforms[boneIndex] = bindWorldTransform;
                animatedWorldTransforms[boneIndex] = animatedWorldTransform;

                Matrix deltaTransform = Matrix.Invert(bindWorldTransform) * animatedWorldTransform;
                if (boneIndex != 0)
                {
                    boneTransforms[boneIndex] = animatedWorldTransform * this.GetAnimatedObjectMatrix(entry);
                }
                this.LogPlanePropDiagnosticsOnce(
                    entry,
                    boneIndex,
                    boneEntry.Key,
                    bone,
                    pointerIndex >= 0 && pointerIndex < adb.Pointers.Length ? adb.Pointers[pointerIndex] : null,
                    bindLocalPosition,
                    bindLocalRotation,
                    animatedLocalPosition,
                    animatedLocalRotation,
                    bindLocalTransform,
                    animatedLocalTransform,
                    bindWorldTransform,
                    animatedWorldTransform,
                    deltaTransform);
            }

            return boneTransforms.Count == 0 ? null : boneTransforms;
        }

        private void LogPlanePropDiagnosticsOnce(
            AnimatedObjectEntry entry,
            ushort boneIndex,
            string boneName,
            SDB_Bone bone,
            ADB_Pointer pointer,
            Vector3 bindLocalPosition,
            Quaternion bindLocalRotation,
            Vector3 animatedLocalPosition,
            Quaternion animatedLocalRotation,
            Matrix bindLocalTransform,
            Matrix animatedLocalTransform,
            Matrix bindWorldTransform,
            Matrix animatedWorldTransform,
            Matrix deltaTransform)
        {
            if (entry == null ||
                !string.Equals(entry.ModelName, "aa_plane", StringComparison.InvariantCultureIgnoreCase) ||
                !string.Equals(boneName, "prop", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            string diagnosticKey = entry.Id + "::" + boneIndex.ToString(CultureInfo.InvariantCulture);
            if (!this.animationDiagnosticKeys.Add(diagnosticKey))
            {
                return;
            }

            List<string> partMappings = new List<string>();
            if (!string.IsNullOrWhiteSpace(entry.ModelName) &&
                this.models.TryGetValue(entry.ModelName, out LR1TrackEditor.Model model) &&
                model?.parts != null)
            {
                for (int partIndex = 0; partIndex < model.parts.Count; partIndex++)
                {
                    ModelPart part = model.parts[partIndex];
                    if (part != null && part.boneid == boneIndex)
                    {
                        partMappings.Add(
                            "#" + partIndex.ToString(CultureInfo.InvariantCulture) +
                            " material=" + (part.material ?? "<null>") +
                            " vertexStart=" + part.vertexstart.ToString(CultureInfo.InvariantCulture) +
                            " vertexCount=" + part.numvertices.ToString(CultureInfo.InvariantCulture) +
                            " indexStart=" + part.indexstart.ToString(CultureInfo.InvariantCulture) +
                            " indexCount=" + part.numindices.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            Utils.WriteLine("AA_PLANE prop diagnostics", ConsoleColor.Cyan);
            Utils.WriteLine("  Entry: " + entry.Id + " object=" + entry.ObjectName + " scene=" + entry.SceneName, ConsoleColor.Cyan);
            Utils.WriteLine("  Bone: index=" + boneIndex.ToString(CultureInfo.InvariantCulture) + " name=" + boneName + " parent=" + (bone?.ParentBone ?? "<none>"), ConsoleColor.Cyan);
            Utils.WriteLine(
                "  Pointer: posOffset=" + (pointer?.PositionOffset.ToString(CultureInfo.InvariantCulture) ?? "<null>") +
                " posLength=" + (pointer?.PositionLength.ToString(CultureInfo.InvariantCulture) ?? "<null>") +
                " rotOffset=" + (pointer?.TransformOffset.ToString(CultureInfo.InvariantCulture) ?? "<null>") +
                " rotLength=" + (pointer?.TransformLength.ToString(CultureInfo.InvariantCulture) ?? "<null>"),
                ConsoleColor.Cyan);
            Utils.WriteLine("  Bind local pos=" + FormatVector3(bindLocalPosition) + " rot=" + FormatQuaternion(bindLocalRotation), ConsoleColor.Cyan);
            Utils.WriteLine("  Animated local pos=" + FormatVector3(animatedLocalPosition) + " rot=" + FormatQuaternion(animatedLocalRotation), ConsoleColor.Cyan);
            Utils.WriteLine("  Bind local matrix=" + FormatMatrix(bindLocalTransform), ConsoleColor.Cyan);
            Utils.WriteLine("  Animated local matrix=" + FormatMatrix(animatedLocalTransform), ConsoleColor.Cyan);
            Utils.WriteLine("  Bind world matrix=" + FormatMatrix(bindWorldTransform), ConsoleColor.Cyan);
            Utils.WriteLine("  Animated world matrix=" + FormatMatrix(animatedWorldTransform), ConsoleColor.Cyan);
            Utils.WriteLine("  Delta matrix=" + FormatMatrix(deltaTransform), ConsoleColor.Cyan);
            Utils.WriteLine("  Model parts on bone=" + (partMappings.Count == 0 ? "<none>" : string.Join(" | ", partMappings)), ConsoleColor.Cyan);
        }

        private static string FormatVector3(Vector3 value)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "({0:0.######}, {1:0.######}, {2:0.######})",
                value.X,
                value.Y,
                value.Z);
        }

        private static string FormatQuaternion(Quaternion value)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "({0:0.######}, {1:0.######}, {2:0.######}, {3:0.######})",
                value.X,
                value.Y,
                value.Z,
                value.W);
        }

        private static string FormatMatrix(Matrix value)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "[[{0:0.######}, {1:0.######}, {2:0.######}, {3:0.######}], [{4:0.######}, {5:0.######}, {6:0.######}, {7:0.######}], [{8:0.######}, {9:0.######}, {10:0.######}, {11:0.######}], [{12:0.######}, {13:0.######}, {14:0.######}, {15:0.######}]]",
                value.M11, value.M12, value.M13, value.M14,
                value.M21, value.M22, value.M23, value.M24,
                value.M31, value.M32, value.M33, value.M34,
                value.M41, value.M42, value.M43, value.M44);
        }

        private void DrawScene(WDB scene)
        {
            if (scene == null)
            {
                return;
            }

            if (this.doDrawStaticObj)
            {
                HashSet<int> referencedGdbIndices = new HashSet<int>();
                foreach (KeyValuePair<string, WDB_StaticModel> current in scene.StaticModels)
                {
                    if (current.Value?.ModelRef == null)
                    {
                        continue;
                    }

                    int gdbIndex = current.Value.ModelRef.IndexGDB;
                    if (gdbIndex < 0 || gdbIndex >= scene.GDBs.Length)
                    {
                        continue;
                    }

                    string gdbName = scene.GDBs[gdbIndex];
                    if (!this.models.ContainsKey(gdbName))
                    {
                        continue;
                    }

                    referencedGdbIndices.Add(gdbIndex);
                    AnimatedObjectEntry entry = this.animatedObjects.FirstOrDefault(item => string.Equals(item.Id, this.GetAnimatedObjectId(scene, current.Key), StringComparison.InvariantCultureIgnoreCase));
                    this.models[gdbName].Draw(this, this.basicEffect, entry?.WorldMatrix ?? CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp), null, this.GetAnimatedMaterialOverrides(entry));
                }

                foreach (KeyValuePair<string, WDB_BDBModel> current in scene.BDBModels)
                {
                    if (current.Value?.ModelRef == null)
                    {
                        continue;
                    }

                    int gdbIndex = current.Value.ModelRef.IndexGDB;
                    if (gdbIndex < 0 || gdbIndex >= scene.GDBs.Length)
                    {
                        continue;
                    }

                    string gdbName = scene.GDBs[gdbIndex];
                    if (!this.models.ContainsKey(gdbName))
                    {
                        continue;
                    }

                    referencedGdbIndices.Add(gdbIndex);
                    AnimatedObjectEntry entry = this.animatedObjects.FirstOrDefault(item => string.Equals(item.Id, this.GetAnimatedObjectId(scene, current.Key), StringComparison.InvariantCultureIgnoreCase));
                    this.models[gdbName].Draw(this, this.basicEffect, entry?.WorldMatrix ?? CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp), null, this.GetAnimatedMaterialOverrides(entry));
                }

                for (int i = 0; i < scene.GDBs.Length; i++)
                {
                    if (!referencedGdbIndices.Contains(i) && this.models.ContainsKey(scene.GDBs[i]))
                    {
                        this.models[scene.GDBs[i]].Draw(this, this.basicEffect);
                    }
                }
            }

            if (this.doDrawAnimObj)
            {
                foreach (KeyValuePair<string, WDB_AnimatedModel> current in scene.AnimatedModels)
                {
                    if (current.Value?.ModelRef == null || !current.Value.ModelRef.IndexGDB.HasValue)
                    {
                        continue;
                    }

                    int gdbIndex = current.Value.ModelRef.IndexGDB.Value;
                    if (gdbIndex < 0 || gdbIndex >= scene.GDBs.Length)
                    {
                        continue;
                    }

                    string gdbName = scene.GDBs[gdbIndex];
                    if (!this.models.ContainsKey(gdbName))
                    {
                        continue;
                    }

                    AnimatedObjectEntry entry = this.animatedObjects.FirstOrDefault(item => string.Equals(item.Id, this.GetAnimatedObjectId(scene, current.Key), StringComparison.InvariantCultureIgnoreCase));
                    Matrix worldMatrix = entry == null
                        ? CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp)
                        : this.GetAnimatedObjectMatrix(entry);
                    Dictionary<ushort, Matrix> boneTransforms = this.GetAnimatedBoneTransforms(entry);
                    this.models[gdbName].Draw(this, this.basicEffect, worldMatrix, null, this.GetAnimatedMaterialOverrides(entry), boneTransforms);
                }
            }
        }

        private static Matrix CreateWorldMatrix(LRVector3 position, LRVector3 rotationFwd, LRVector3 rotationUp)
        {
            Vector3 right = Vector3.Cross(rotationUp.toXNAVector(), rotationFwd.toXNAVector());
            return new Matrix(rotationFwd.X, rotationFwd.Y, rotationFwd.Z, 0f, right.X, right.Y, right.Z, 0f, rotationUp.X, rotationUp.Y, rotationUp.Z, 0f, position.X, position.Y, position.Z, 1f);
        }

        public void ClearTrackData()
        {
            this.track = false;
            this.rab = null;
            this.pwb = null;
            this.wdb = null;
            this.extraWdbScenes.Clear();
            this.scenePaths.Clear();
            this.sceneMabs.Clear();
            this.sceneAdbs.Clear();
            this.sceneSdbs.Clear();
            this.animatedObjects.Clear();
            this.animatedObjectPlaybacks.Clear();
            this.spb = null;
            this.cpb = null;
            this.hzb = null;
            this.evb = null;
            this.skb = null;
            this.skbmesh = null;
            this.embs.Clear();
            this.emitterDefinitions.Clear();
            this.rrbs.Clear();
            this.animationDiagnosticKeys.Clear();
            this.totalElapsedSeconds = 0f;
            foreach (LR1TrackEditor.Model model in this.models.Values)
            {
                DisposeModelBuffers(model);
            }
            this.models.Clear();
            this.materials.Clear();
            foreach (CollisionModelInstance collisionModel in this.collisionModels)
            {
                DisposeModelBuffers(collisionModel?.Model);
            }
            this.collisionModels.Clear();
            this.currentPWBfile = "";
            this.currentWDBfile = "";
            this.currentRABfile = "";
            this.ClearViewerSelection();
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
                ViewerSelectionType selectedType = ViewerSelectionType.None;
                int selectedIndex = -1;
                if (this.spb?.StartPositions != null)
                {
                    foreach (KeyValuePair<int, SPB_StartPosition> current in this.spb.StartPositions)
                    {
                        Vector3 markerPosition = current.Value.Position.toXNAVector();
                        BoundingBox box = new BoundingBox(markerPosition - new Vector3(8f), markerPosition + new Vector3(8f));
                        float? hit = input.Intersects(box);
                        if (hit != null && hit.Value < maxValue)
                        {
                            maxValue = hit.Value;
                            selectedType = ViewerSelectionType.StartPosition;
                            selectedIndex = current.Key;
                        }
                    }
                }

                if (this.cpb?.Checkpoints != null)
                {
                    for (int i = 0; i < this.cpb.Checkpoints.Length; i++)
                    {
                        Vector3 markerPosition = this.cpb.Checkpoints[i].Location.toXNAVector();
                        BoundingBox box = new BoundingBox(markerPosition - new Vector3(10f), markerPosition + new Vector3(10f));
                        float? hit = input.Intersects(box);
                        if (hit != null && hit.Value < maxValue)
                        {
                            maxValue = hit.Value;
                            selectedType = ViewerSelectionType.Checkpoint;
                            selectedIndex = i;
                        }
                    }
                }

                if (selectedType == ViewerSelectionType.StartPosition)
                {
                    this.selectedViewerObject = ViewerSelectionType.StartPosition;
                    this.selectedStartPositionKey = selectedIndex;
                    this.selectedCheckpointIndex = -1;
                }
                else if (selectedType == ViewerSelectionType.Checkpoint)
                {
                    this.selectedViewerObject = ViewerSelectionType.Checkpoint;
                    this.selectedCheckpointIndex = selectedIndex;
                    this.selectedStartPositionKey = -1;
                }
                else
                {
                    this.ClearViewerSelection();
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
                this.RefreshProjectionSettings();
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
            this.RefreshProjectionSettings();
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
            this.totalElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.animatedObjectPlaybacks.Count > 0)
            {
                float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (KeyValuePair<string, AnimatedObjectPlayback> current in this.animatedObjectPlaybacks.ToList())
                {
                    current.Value.ElapsedSeconds += elapsedSeconds;
                    if (current.Value.IsFinished())
                    {
                        this.animatedObjectPlaybacks.Remove(current.Key);
                    }
                }
            }
            base.Update(gameTime);
        }

        public bool gameFormFocused =>
            this.gameform.ContainsFocus;
    }
}

