namespace LR1TrackEditor
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class Model
    {
        public List<ModelPart> parts = new List<ModelPart>();
        public bool normals = false;
        public ushort[] indices;
        public int numvertices;
        public VertexPTC[] verticesC;
        public VertexPTN[] verticesN;
        public float scale = 1f;
        public VertexBuffer vertexbuffer;
        public IndexBuffer indexbuffer;
        public BoundingBox boundingbox;

        public void CreateBuffers(GraphicsDevice gd)
        {
            this.indexbuffer = new IndexBuffer(gd, IndexElementSize.SixteenBits, this.indices.Length, BufferUsage.None);
            this.indexbuffer.SetData<ushort>(this.indices);
            this.vertexbuffer = new VertexBuffer(gd, this.getVertexDeclaration(), this.numvertices, BufferUsage.None);
            if (this.normals)
            {
                this.vertexbuffer.SetData<VertexPTN>(this.verticesN);
            }
            else
            {
                this.vertexbuffer.SetData<VertexPTC>(this.verticesC);
            }
        }

        public void Draw(GameView game, BasicEffect basicEffect)
        {
            this.Draw(game, basicEffect, Matrix.Identity, null);
        }

        public void Draw(GameView game, BasicEffect basicEffect, Matrix transform, Material mat = null)
        {
            this.Draw(game, basicEffect, transform, mat, null);
        }

        public void Draw(GameView game, BasicEffect basicEffect, Matrix transform, Material mat, Dictionary<string, Material> materialOverrides)
        {
            if (this.vertexbuffer == null || this.indexbuffer == null || this.parts.Count == 0)
            {
                return;
            }

            basicEffect.World = Matrix.CreateScale(this.scale) * transform;
            basicEffect.VertexColorEnabled = !this.normals && game.doVertexColors;
            game.GraphicsDevice.Indices = this.indexbuffer;
            game.GraphicsDevice.SetVertexBuffer(this.vertexbuffer);
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            using (List<ModelPart>.Enumerator enumerator = this.parts.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    ModelPart current = enumerator.Current;
                    if (current.visible)
                    {
                        if (mat is object)
                        {
                            if ((mat.texture == null) || !game.doTextures)
                            {
                                basicEffect.TextureEnabled = false;
                            }
                            else
                            {
                                basicEffect.TextureEnabled = true;
                                basicEffect.Texture = mat.texture;
                            }
                            basicEffect.Alpha = ((float)mat.alpha) / 255f;
                            basicEffect.AmbientLightColor = Utils.vectorfromcolor(mat.ambientcolor);
                        }
                        else if ((current.material == null) || !game.materials.ContainsKey(current.material))
                        {
                            basicEffect.TextureEnabled = false;
                            basicEffect.Alpha = 1f;
                            basicEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
                            basicEffect.DiffuseColor = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            Material resolvedMaterial = game.materials[current.material];
                            if (materialOverrides != null && materialOverrides.ContainsKey(current.material) && materialOverrides[current.material] != null)
                            {
                                resolvedMaterial = materialOverrides[current.material];
                            }
                            if ((resolvedMaterial.texture == null) || !game.doTextures)
                            {
                                basicEffect.TextureEnabled = false;
                            }
                            else
                            {
                                basicEffect.TextureEnabled = true;
                                basicEffect.Texture = resolvedMaterial.texture;
                            }
                            basicEffect.Alpha = ((float)resolvedMaterial.alpha) / 255f;
                            basicEffect.AmbientLightColor = Utils.vectorfromcolor(resolvedMaterial.ambientcolor);
                        }
                        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, current.indexstart * 3, current.numindices);
                        }
                    }
                }
            }
            basicEffect.World = Matrix.Identity;
        }

        public void DrawBoundingBox(GameView game, BasicEffect basicEffect, Matrix transform)
        {
            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = true;
            List<VertexPTC> input = new List<VertexPTC>();
            Vector3[] corners = this.boundingbox.Transform(transform).GetCorners();
            input.Add(new VertexPTC(corners[0], Color.White));
            input.AddTwice(new VertexPTC(corners[1], Color.White));
            input.AddTwice(new VertexPTC(corners[2], Color.White));
            input.AddTwice(new VertexPTC(corners[3], Color.White));
            input.Add(new VertexPTC(corners[0], Color.White));
            input.Add(new VertexPTC(corners[4], Color.White));
            input.AddTwice(new VertexPTC(corners[5], Color.White));
            input.AddTwice(new VertexPTC(corners[6], Color.White));
            input.AddTwice(new VertexPTC(corners[7], Color.White));
            input.Add(new VertexPTC(corners[4], Color.White));
            input.Add(new VertexPTC(corners[0], Color.White));
            input.Add(new VertexPTC(corners[4], Color.White));
            input.Add(new VertexPTC(corners[1], Color.White));
            input.Add(new VertexPTC(corners[5], Color.White));
            input.Add(new VertexPTC(corners[2], Color.White));
            input.Add(new VertexPTC(corners[6], Color.White));
            input.Add(new VertexPTC(corners[3], Color.White));
            input.Add(new VertexPTC(corners[7], Color.White));
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives<VertexPTC>(PrimitiveType.LineList, input.ToArray(), 0, 12, VertexPTC.VertexDeclaration);
            }
        }

        public void DrawMultiple(GameView game, BasicEffect basicEffect, Matrix[] transforms, Material[] mats = null)
        {
            if (this.vertexbuffer == null || this.indexbuffer == null || this.parts.Count == 0)
            {
                return;
            }

            game.GraphicsDevice.Indices = this.indexbuffer;
            game.GraphicsDevice.SetVertexBuffer(this.vertexbuffer);
            basicEffect.VertexColorEnabled = !this.normals;
            Matrix matrix = Matrix.CreateScale(this.scale);
            int index = 0;
            while (true)
            {
                if (index >= transforms.Length)
                {
                    basicEffect.World = Matrix.Identity;
                    return;
                }
                basicEffect.World = matrix * transforms[index];
                using (List<ModelPart>.Enumerator enumerator = this.parts.GetEnumerator())
                {
                    while (true)
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                        ModelPart current = enumerator.Current;
                        if (mats != null)
                        {
                            if ((mats[index].texture == null) || !game.doTextures)
                            {
                                basicEffect.TextureEnabled = false;
                            }
                            else
                            {
                                basicEffect.TextureEnabled = true;
                                basicEffect.Texture = mats[index].texture;
                            }
                            basicEffect.Alpha = ((float)mats[index].alpha) / 255f;
                            basicEffect.AmbientLightColor = Utils.vectorfromcolor(mats[index].ambientcolor);
                        }
                        else if ((current.material == null) || !game.materials.ContainsKey(current.material))
                        {
                            basicEffect.TextureEnabled = false;
                            basicEffect.Alpha = 1f;
                            basicEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);
                            basicEffect.DiffuseColor = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            if ((game.materials[current.material].texture == null) || !game.doTextures)
                            {
                                basicEffect.TextureEnabled = false;
                            }
                            else
                            {
                                basicEffect.TextureEnabled = true;
                                basicEffect.Texture = game.materials[current.material].texture;
                            }
                            basicEffect.Alpha = ((float)game.materials[current.material].alpha) / 255f;
                            basicEffect.AmbientLightColor = Utils.vectorfromcolor(game.materials[current.material].ambientcolor);
                        }
                        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, current.indexstart * 3, current.numindices);
                        }
                    }
                    index++;
                    continue;
                }
            }
        }

        public void generateBoundingBox(bool rotating)
        {
            int num;
            Vector3 zero = Vector3.Zero;
            Vector3 min = Vector3.Zero;
            if (this.normals)
            {
                VertexPTN[] verticesN = this.verticesN;
                num = 0;
                while (true)
                {
                    if (num >= verticesN.Length)
                    {
                        break;
                    }
                    VertexPTN xptn = verticesN[num];
                    if (xptn.Position.X > zero.X)
                    {
                        zero.X = xptn.Position.X;
                    }
                    if (xptn.Position.Y > zero.Y)
                    {
                        zero.Y = xptn.Position.Y;
                    }
                    if (xptn.Position.Z > zero.Z)
                    {
                        zero.Z = xptn.Position.Z;
                    }
                    if (xptn.Position.X < min.X)
                    {
                        min.X = xptn.Position.X;
                    }
                    if (xptn.Position.Y < min.Y)
                    {
                        min.Y = xptn.Position.Y;
                    }
                    if (xptn.Position.Z < min.Z)
                    {
                        min.Z = xptn.Position.Z;
                    }
                    num++;
                }
            }
            else
            {
                VertexPTC[] verticesC = this.verticesC;
                num = 0;
                while (true)
                {
                    if (num >= verticesC.Length)
                    {
                        break;
                    }
                    VertexPTC xptc = verticesC[num];
                    if (xptc.Position.X > zero.X)
                    {
                        zero.X = xptc.Position.X;
                    }
                    if (xptc.Position.Y > zero.Y)
                    {
                        zero.Y = xptc.Position.Y;
                    }
                    if (xptc.Position.Z > zero.Z)
                    {
                        zero.Z = xptc.Position.Z;
                    }
                    if (xptc.Position.X < min.X)
                    {
                        min.X = xptc.Position.X;
                    }
                    if (xptc.Position.Y < min.Y)
                    {
                        min.Y = xptc.Position.Y;
                    }
                    if (xptc.Position.Z < min.Z)
                    {
                        min.Z = xptc.Position.Z;
                    }
                    num++;
                }
            }
            if (!rotating)
            {
                this.boundingbox = new BoundingBox(min, zero);
            }
            else
            {
                Vector3 vector3 = new Vector3(Math.Max(zero.X, Math.Abs(min.X)), Math.Max(zero.Y, Math.Abs(min.Y)), Math.Max(zero.Z, Math.Abs(min.Z)));
                this.boundingbox = new BoundingBox(vector3, -vector3);
            }
            this.boundingbox = this.boundingbox.Transform(Matrix.CreateScale(this.scale));
        }

        public List<Vector3> GetPoints()
        {
            int num;
            List<Vector3> list = new List<Vector3>();
            if (this.normals)
            {
                VertexPTN[] verticesN = this.verticesN;
                num = 0;
                while (true)
                {
                    if (num >= verticesN.Length)
                    {
                        break;
                    }
                    VertexPTN xptn = verticesN[num];
                    list.Add(xptn.Position);
                    num++;
                }
            }
            else
            {
                VertexPTC[] verticesC = this.verticesC;
                num = 0;
                while (true)
                {
                    if (num >= verticesC.Length)
                    {
                        break;
                    }
                    VertexPTC xptc = verticesC[num];
                    list.Add(xptc.Position);
                    num++;
                }
            }
            return list;
        }

        public VertexDeclaration getVertexDeclaration() =>
            !this.normals ? VertexPTC.VertexDeclaration : VertexPTN.VertexDeclaration;
    }
}

