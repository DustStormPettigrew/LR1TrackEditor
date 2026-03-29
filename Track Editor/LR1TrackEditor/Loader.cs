namespace LR1TrackEditor
{
    using LibLR1;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

    public static class Loader
    {
        public const byte PROPERTY_MATERIAL_ID = 0x27;
        public const byte PROPERTY_INDICES_META = 0x2d;
        public const byte PROPERTY_VERTEX_META = 0x31;
        public const byte PROPERTY_BONE_ID = 50;
        public static CultureInfo ci = CultureInfo.InvariantCulture;

        public static List<Block> getBlocks(GDB_Meta[] input)
        {
            ushort length;
            List<Block> list = new List<Block>();
            ushort matid = 0;
            ushort boneid = 0xffff;
            ushort vstart = length = 0;
            byte vertexoffset = 0;
            foreach (GDB_Meta meta in input)
            {
                if (meta.Type == 0x27)
                {
                    matid = ((GDB_Meta_Material)meta).MaterialId;
                }
                else if (meta.Type == 50)
                {
                    boneid = ((GDB_Meta_Bone)meta).BoneId;
                }
                else if (meta.Type == 0x31)
                {
                    GDB_Meta_Vertices vertices = (GDB_Meta_Vertices)meta;
                    vstart = vertices.Offset;
                    length = vertices.Length;
                    vertexoffset = vertices.UnknownByte;
                }
                else if (meta.Type == 0x2d)
                {
                    GDB_Meta_Indices indices = (GDB_Meta_Indices)meta;
                    list.Add(new Block(matid, boneid, vertexoffset, vstart, length, indices.Offset, indices.Length));
                }
            }
            return list;
        }

        public static Dictionary<string, Material> loadmaterials(string mdbpath, GraphicsDevice gd)
        {
            Utils.WriteLine("Loading MDB: " + mdbpath, ConsoleColor.Cyan);
            Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
            string directoryName = Path.GetDirectoryName(mdbpath);
            string str2 = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(mdbpath) + ".TDB");
            Utils.WriteLine("Loading TDB: " + str2, ConsoleColor.DarkCyan);
            TDB tdb = new TDB(str2);
            using (Dictionary<string, MDB_Material>.Enumerator enumerator = new MDB(mdbpath).Materials.GetEnumerator())
            {
                KeyValuePair<string, MDB_Material> current;
                Material material2;
                goto TR_0031;
            TR_0005:
                dictionary.Add(current.Key, material2);
            TR_0031:
                while (true)
                {
                    bool flag2 = enumerator.MoveNext();
                    if (flag2)
                    {
                        current = enumerator.Current;
                        MDB_Material material = current.Value;
                        material2 = new Material();
                        if (material.Alpha != null)
                        {
                            material2.alpha = (byte)material.Alpha.Value;
                        }
                        if (material.AmbientColor is object)
                        {
                            material2.ambientcolor = new Microsoft.Xna.Framework.Color(material.AmbientColor.R, material.AmbientColor.G, material.AmbientColor.B, material.AmbientColor.A);
                        }
                        if (material.DiffuseColor is object)
                        {
                            material2.diffusecolor = new Microsoft.Xna.Framework.Color(material.DiffuseColor.R, material.DiffuseColor.G, material.DiffuseColor.B, material.DiffuseColor.A);
                        }
                        if (material.TextureName is object)
                        {
                            TDB_Texture texture = tdb.Textures[material.TextureName];
                            if (!texture.IsBitmap)
                            {
                                Utils.WriteLine("Not loading TGA: " + Path.Combine(directoryName, material.TextureName + ".TGA"), ConsoleColor.Red);
                            }
                            else
                            {
                                string path = Path.Combine(directoryName, material.TextureName + ".BMP");
                                if (File.Exists(path))
                                {
                                    Bitmap bitmap;
                                    Image image;
                                    MemoryStream stream;
                                    BMP bmp = null;
                                    bool flag = true;
                                    try
                                    {
                                        bmp = new BMP(path);
                                    }
                                    catch (InvalidDataException)
                                    {
                                        flag = false;
                                    }
                                    if (!flag)
                                    {
                                        Utils.WriteLine("Loading Win BMP: " + path, ConsoleColor.DarkMagenta);
                                        bitmap = new Bitmap(path);
                                        if (texture.HasColor2C)
                                        {
                                            bitmap.MakeTransparent(System.Drawing.Color.FromArgb(texture.Color2C.R, texture.Color2C.G, texture.Color2C.B));
                                            material2.semitransparent = true;
                                        }
                                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                                        image = bitmap;
                                        stream = new MemoryStream();
                                        try
                                        {
                                            image.Save(stream, ImageFormat.Png);
                                            material2.texture = Texture2D.FromStream(gd, stream);
                                        }
                                        finally
                                        {
                                            if (stream is object)
                                            {
                                                stream.Dispose();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Utils.WriteLine("Loading LR BMP: " + path, ConsoleColor.DarkMagenta);
                                        bitmap = new Bitmap(bmp.Width, bmp.Height);
                                        int num = 0;
                                        while (true)
                                        {
                                            if (num >= bitmap.Width)
                                            {
                                                if (texture.HasColor2C)
                                                {
                                                    bitmap.MakeTransparent(System.Drawing.Color.FromArgb(texture.Color2C.R, texture.Color2C.G, texture.Color2C.B));
                                                    material2.semitransparent = true;
                                                }
                                                image = bitmap;
                                                stream = new MemoryStream();
                                                try
                                                {
                                                    image.Save(stream, ImageFormat.Png);
                                                    material2.texture = Texture2D.FromStream(gd, stream);
                                                }
                                                finally
                                                {
                                                    if (stream is object)
                                                    {
                                                        stream.Dispose();
                                                    }
                                                }
                                                break;
                                            }
                                            int num2 = 0;
                                            while (true)
                                            {
                                                flag2 = num2 < bitmap.Height;
                                                if (!flag2)
                                                {
                                                    num++;
                                                    break;
                                                }
                                                BitmapColor pixel = bmp.GetPixel(num, num2);
                                                bitmap.SetPixel(num, (bmp.Height - num2) - 1, System.Drawing.Color.FromArgb(pixel.r, pixel.g, pixel.b));
                                                num2++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return dictionary;
                    }
                    break;
                }
                goto TR_0005;
            }
        }

        public static LR1TrackEditor.Model loadmodel(GameView game, string modelpath, bool rotatingboundingbox = false)
        {
            Utils.WriteLine("Loading GDB: " + modelpath, ConsoleColor.DarkYellow);
            LR1TrackEditor.Model model = new LR1TrackEditor.Model();
            if (Path.GetExtension(modelpath).ToLower() == ".gdb")
            {
                GDB gdb = new GDB(modelpath);
                model.normals = gdb.VertexColors.Length == 0;
                model.scale = gdb.Scale;
                List<Block> list = getBlocks(gdb.Meta);
                List<ushort> list2 = new List<ushort>();
                List<GDB_Vertex_Color> input = new List<GDB_Vertex_Color>();
                List<GDB_Vertex_Normal> list4 = new List<GDB_Vertex_Normal>();
                int num = 0;
                while (true)
                {
                    Block current;
                    bool flag = num < list.Count;
                    if (!flag)
                    {
                        model.indices = list2.ToArray();
                        flag = !model.normals;
                        if (!flag)
                        {
                            model.verticesN = toPTNarray(list4);
                            model.numvertices = model.verticesN.Length;
                        }
                        else
                        {
                            model.verticesC = toPTCarray(input);
                            model.numvertices = model.verticesC.Length;
                        }
                        int vertexstart = 0;
                        int num7 = 0;
                        int vertexlength = 0;
                        int indexlength = 0;
                        int materialid = list[0].materialid;
                        ModelPart item = new ModelPart();
                        using (List<Block>.Enumerator enumerator = list.GetEnumerator())
                        {
                            while (true)
                            {
                                flag = enumerator.MoveNext();
                                if (!flag)
                                {
                                    break;
                                }
                                current = enumerator.Current;
                                if (current.materialid == materialid)
                                {
                                    vertexlength += current.vertexlength;
                                    indexlength += current.indexlength;
                                    continue;
                                }
                                item.material = gdb.Materials[materialid];
                                item.indexstart = num7;
                                item.vertexstart = vertexstart;
                                item.numvertices = vertexlength;
                                item.numindices = indexlength;
                                model.parts.Add(item);
                                item = new ModelPart();
                                vertexlength = current.vertexlength;
                                indexlength = current.indexlength;
                                vertexstart = current.vertexstart;
                                num7 = current.indexstart - current.vertexoffset;
                                materialid = current.materialid;
                            }
                        }
                        item.material = gdb.Materials[materialid];
                        item.indexstart = num7;
                        item.vertexstart = vertexstart;
                        item.numvertices = vertexlength;
                        item.numindices = indexlength;
                        model.parts.Add(item);
                        break;
                    }
                    current = list[num];
                    Block block2 = null;
                    if (num > 0)
                    {
                        block2 = list[num - 1];
                    }
                    if (model.normals)
                    {
                        list4.AddRange(gdb.VertexNormals.ToList<GDB_Vertex_Normal>().GetRange(current.vertexstart, current.vertexlength));
                    }
                    else
                    {
                        input.AddRange(gdb.VertexColors.ToList<GDB_Vertex_Color>().GetRange(current.vertexstart, current.vertexlength));
                    }
                    int num2 = 0;
                    while (true)
                    {
                        if (num2 >= current.indexlength)
                        {
                            num++;
                            break;
                        }
                        GDB_Polygon polygon = gdb.Polygons[num2 + current.indexstart];
                        int[] numArray = new int[] { polygon.V0, polygon.V1, polygon.V2 };
                        int index = 0;
                        while (true)
                        {
                            flag = index < 3;
                            if (!flag)
                            {
                                num2++;
                                break;
                            }
                            int num4 = numArray[index];
                            int num5 = num4 - current.vertexoffset;
                            if (num5 >= current.vertexlength)
                            {
                                list2.Add((ushort)(block2.vertexstart + (num4 - block2.vertexoffset)));
                            }
                            else if (num5 < 0)
                            {
                                list2.Add((ushort)(block2.vertexstart + num4));
                            }
                            else
                            {
                                list2.Add((ushort)(current.vertexstart + num5));
                            }
                            index++;
                        }
                    }
                }
            }
            model.parts = (from part in model.parts
                           orderby ((part.material != null) && game.materials.ContainsKey(part.material)) && game.materials[part.material].semitransparent
                           select part).ToList<ModelPart>();
            model.CreateBuffers(game.GraphicsDevice);
            model.generateBoundingBox(rotatingboundingbox);
            return model;
        }

        public static PWB loadPWB(GameView game, string pwbpath)
        {
            Action<KeyValuePair<string, Material>> action = null;
            PWB pwb;
            game.form.ClearEdits("PWB");
            game.SelectedBrickIndices.Clear();
            game.currentPWBfile = pwbpath;
            Utils.WriteLine("Loading PWB " + pwbpath, ConsoleColor.DarkGreen);
            if (game.pupbrick is object)
            {
                pwb = new PWB(pwbpath);
            }
            else
            {
                string str2;
                string str = Utils.getGamedir(pwbpath);
                if (!((str != "") && File.Exists(str2 = Path.Combine(str, @"GAMEDATA\COMMON\PUBRICKY.GDB"))))
                {
                    Utils.WriteLine("ERROR: could not find coremodels, please read coremodels\readme.txt", ConsoleColor.Red);
                    pwb = null;
                }
                else
                {
                    Console.WriteLine("Gamedir found at " + str);
                    Console.WriteLine(@"Loading powerup models\textures");
                    game.pupbrick = loadmodel(game, str2, false);
                    game.puptrail = loadmodel(game, Path.Combine(Path.GetDirectoryName(str2), "PUTRAILY.GDB"), false);
                    game.enhabrick = loadmodel(game, Path.Combine(Path.GetDirectoryName(str2), "ENHABRIK.GDB"), false);
                    game.enhatrail = loadmodel(game, Path.Combine(Path.GetDirectoryName(str2), "ENHANER.GDB"), false);
                    if (action == null)
                    {
                        action = delegate (KeyValuePair<string, Material> x)
                        {
                            game.corematerials[x.Key] = x.Value;
                        };
                    }
                    loadmaterials(Path.Combine(str, @"GAMEDATA\COMMON\POWERUP.MDB"), game.GraphicsDevice).ToList<KeyValuePair<string, Material>>().ForEach(action);
                    pwb = new PWB(pwbpath);
                }
            }
            return pwb;
        }

        public static RRB loadRRB(string rrbpath)
        {
            Utils.WriteLine("Loading RRB: " + rrbpath, ConsoleColor.Yellow);
            return new RRB(rrbpath);
        }

        public static LR1TrackEditor.SKB loadSKB(string skbpath)
        {
            Utils.WriteLine("Loading SKB: " + skbpath, ConsoleColor.White);
            return new LR1TrackEditor.SKB(skbpath);
        }

        public static WDB loadWDB(GameView game, string wdbpath)
        {
            WDB wdb = new WDB(wdbpath);
            Utils.WriteLine("Loading WDB: " + wdbpath, ConsoleColor.Magenta);
            game.models.Clear();
            string directoryName = Path.GetDirectoryName(wdbpath);
            foreach (string str2 in wdb.GDBs)
            {
                game.models[str2] = loadmodel(game, Path.Combine(directoryName, str2 + ".gdb"), false);
            }
            foreach (string str2 in wdb.GDB2s)
            {
                if (!game.models.ContainsKey(str2))
                {
                    game.models[str2] = loadmodel(game, Path.Combine(directoryName, str2 + ".gdb"), false);
                }
            }
            return wdb;
        }

        private static VertexPTC[] toPTCarray(List<GDB_Vertex_Color> input)
        {
            VertexPTC[] xptcArray = new VertexPTC[input.Count];
            for (int i = 0; i < input.Count; i++)
            {
                xptcArray[i] = new VertexPTC(input[i].Position.toXNAVector(), input[i].TexCoords.toXNAVector(), new Microsoft.Xna.Framework.Color(input[i].Color.R, input[i].Color.G, input[i].Color.B, input[i].Color.A));
            }
            return xptcArray;
        }

        private static VertexPTN[] toPTNarray(List<GDB_Vertex_Normal> input)
        {
            VertexPTN[] xptnArray = new VertexPTN[input.Count];
            for (int i = 0; i < input.Count; i++)
            {
                xptnArray[i] = new VertexPTN(input[i].Position.toXNAVector(), input[i].TexCoords.toXNAVector(), input[i].Normal.toXNAVector());
            }
            return xptnArray;
        }

        public class Block
        {
            public ushort materialid;
            public ushort boneid;
            public byte vertexoffset;
            public ushort vertexstart;
            public ushort vertexlength;
            public ushort indexstart;
            public ushort indexlength;

            public Block(ushort matid, ushort boneid, byte vertexoffset, ushort vstart, ushort vlength, ushort istart, ushort ilength)
            {
                this.materialid = matid;
                this.boneid = boneid;
                this.vertexoffset = vertexoffset;
                this.vertexstart = vstart;
                this.vertexlength = vlength;
                this.indexstart = istart;
                this.indexlength = ilength;
            }

            public override string ToString()
            {
                string[] strArray = new string[] { this.materialid.ToString(), this.boneid.ToString(), this.vertexoffset.ToString(), this.vertexstart.ToString(), this.vertexlength.ToString(), this.indexstart.ToString(), this.indexlength.ToString(), "" };
                return string.Join(Environment.NewLine, strArray);
            }
        }
    }
}

