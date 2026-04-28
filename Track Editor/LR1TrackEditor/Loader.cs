namespace LR1TrackEditor
{
    using LibLR1;
    using LibLR1.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public static class Loader
    {
        public const byte PROPERTY_MATERIAL_ID = 0x27;
        public const byte PROPERTY_INDICES_META = 0x2d;
        public const byte PROPERTY_VERTEX_META = 0x31;
        public const byte PROPERTY_BONE_ID = 50;
        public static CultureInfo ci = CultureInfo.InvariantCulture;
        private static readonly Regex NpcPathFileNameRegex = new Regex(@"^R(?<racer>[0-5])_[A-Z]_(?<path>\d+)\.RRB$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static List<Block> getBlocks(GDB_Meta[] input)
        {
            ushort length;
            List<Block> list = new List<Block>();
            ushort matid = 0;
            ushort boneid = 0xffff;
            ushort vstart = length = 0;
            byte vertexoffset = 0;
            if (input == null || input.Length == 0)
            {
                return list;
            }
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
            foreach (KeyValuePair<string, MDB_Material> current in new MDB(mdbpath).Materials)
            {
                MDB_Material material = current.Value;
                Material material2 = new Material
                {
                    name = current.Key
                };
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
                if (!string.IsNullOrWhiteSpace(material.TextureName) && tdb.Textures.ContainsKey(material.TextureName))
                {
                    TDB_Texture texture = tdb.Textures[material.TextureName];
                    material2.textureName = material.TextureName;
                    material2.textureDirectory = directoryName;
                    if (texture.HasColor2C)
                    {
                        material2.hasTransparentColor = true;
                        material2.transparentColor = System.Drawing.Color.FromArgb(texture.Color2C.R, texture.Color2C.G, texture.Color2C.B);
                    }
                    if (!texture.IsBitmap)
                    {
                        Utils.WriteLine("Not loading TGA: " + Path.Combine(directoryName, material.TextureName + ".TGA"), ConsoleColor.Red);
                    }
                    else
                    {
                        material2.texture = LoadTextureFromPath(Path.Combine(directoryName, material.TextureName + ".BMP"), gd, material2, true);
                    }
                }
                dictionary.Add(current.Key, material2);
            }
            return dictionary;
        }

        private static Texture2D LoadTextureFromPath(string path, GraphicsDevice gd, Material material, bool logLoad)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            Bitmap bitmap;
            Image image;
            MemoryStream stream;
            BMP bmp = null;
            bool isLegoBitmap = true;
            try
            {
                bmp = new BMP(path);
            }
            catch (InvalidDataException)
            {
                isLegoBitmap = false;
            }

            if (!isLegoBitmap)
            {
                if (logLoad)
                {
                    Utils.WriteLine("Loading Win BMP: " + path, ConsoleColor.DarkMagenta);
                }
                bitmap = new Bitmap(path);
                if (material.hasTransparentColor)
                {
                    bitmap.MakeTransparent(material.transparentColor);
                    material.semitransparent = true;
                }
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                image = bitmap;
                stream = new MemoryStream();
                try
                {
                    image.Save(stream, ImageFormat.Png);
                    return Texture2D.FromStream(gd, stream);
                }
                finally
                {
                    stream?.Dispose();
                    image.Dispose();
                }
            }

            if (logLoad)
            {
                Utils.WriteLine("Loading LR BMP: " + path, ConsoleColor.DarkMagenta);
            }
            bitmap = new Bitmap(bmp.Width, bmp.Height);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    BitmapColor pixel = bmp.GetPixel(x, y);
                    bitmap.SetPixel(x, (bmp.Height - y) - 1, System.Drawing.Color.FromArgb(pixel.r, pixel.g, pixel.b));
                }
            }

            if (material.hasTransparentColor)
            {
                bitmap.MakeTransparent(material.transparentColor);
                material.semitransparent = true;
            }

            image = bitmap;
            stream = new MemoryStream();
            try
            {
                image.Save(stream, ImageFormat.Png);
                return Texture2D.FromStream(gd, stream);
            }
            finally
            {
                stream?.Dispose();
                image.Dispose();
            }
        }

        private static IEnumerable<string> GetTextureFrameNameCandidates(string textureName, int frameIndex)
        {
            HashSet<string> candidates = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (string.IsNullOrWhiteSpace(textureName))
            {
                yield break;
            }

            string trimmed = textureName.Trim();
            if (frameIndex <= 0)
            {
                yield return trimmed;
                yield break;
            }

            Match trailingDigits = Regex.Match(trimmed, @"^(.*?)(\d+)$");
            if (trailingDigits.Success)
            {
                string prefix = trailingDigits.Groups[1].Value;
                int width = trailingDigits.Groups[2].Value.Length;
                string replaced = prefix + frameIndex.ToString(new string('0', width), CultureInfo.InvariantCulture);
                if (candidates.Add(replaced))
                {
                    yield return replaced;
                }
            }

            string appended = trimmed + frameIndex.ToString(CultureInfo.InvariantCulture);
            if (candidates.Add(appended))
            {
                yield return appended;
            }

            for (int width = 2; width <= 4; width++)
            {
                string padded = trimmed + frameIndex.ToString(new string('0', width), CultureInfo.InvariantCulture);
                if (candidates.Add(padded))
                {
                    yield return padded;
                }
            }
        }

        public static Material ResolveAnimatedMaterialFrame(Material source, GraphicsDevice gd, int frameIndex)
        {
            if (source == null || source.texture == null || frameIndex <= 0)
            {
                return source;
            }

            if (source.animationFrameTextures.TryGetValue(frameIndex, out Texture2D cachedTexture))
            {
                return cachedTexture == null ? source : source.CloneWithTexture(cachedTexture);
            }

            foreach (string candidate in GetTextureFrameNameCandidates(source.textureName, frameIndex))
            {
                string path = Path.Combine(source.textureDirectory, candidate + ".BMP");
                Texture2D texture = LoadTextureFromPath(path, gd, source, false);
                if (texture != null)
                {
                    source.animationFrameTextures[frameIndex] = texture;
                    return source.CloneWithTexture(texture);
                }
            }

            source.animationFrameTextures[frameIndex] = null;
            return source;
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
                if (list.Count == 0)
                {
                    Utils.WriteLine("Skipping empty GDB: " + modelpath, ConsoleColor.Yellow);
                    model.indices = Array.Empty<ushort>();
                    model.verticesC = Array.Empty<VertexPTC>();
                    model.verticesN = Array.Empty<VertexPTN>();
                    model.numvertices = 0;
                    model.boundingbox = new BoundingBox(Vector3.Zero, Vector3.Zero);
                    return model;
                }
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
            if (model.numvertices > 0 && model.indices.Length > 0)
            {
                model.CreateBuffers(game.GraphicsDevice);
            }
            model.generateBoundingBox(rotatingboundingbox);
            return model;
        }

        private static LR1TrackEditor.Model loadCollisionBVB(GameView game, string bvbPath)
        {
            Utils.WriteLine("Loading BVB: " + bvbPath, ConsoleColor.DarkYellow);
            BVB bvb = new BVB(bvbPath);
            LR1TrackEditor.Model model = new LR1TrackEditor.Model
            {
                normals = false,
                scale = 1f
            };

            Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(255, 160, 64);
            model.verticesC = bvb.Vertices.Select(vertex => new VertexPTC(vertex.toXNAVector(), color)).ToArray();
            model.numvertices = model.verticesC.Length;
            model.indices = new ushort[bvb.Polygons.Length * 3];

            for (int i = 0; i < bvb.Polygons.Length; i++)
            {
                BVB_Polygon polygon = bvb.Polygons[i];
                model.indices[(i * 3)] = (ushort)polygon.V0;
                model.indices[(i * 3) + 1] = (ushort)polygon.V1;
                model.indices[(i * 3) + 2] = (ushort)polygon.V2;
            }

            model.parts.Add(new ModelPart
            {
                material = null,
                indexstart = 0,
                vertexstart = 0,
                numvertices = model.numvertices,
                numindices = bvb.Polygons.Length
            });
            model.CreateBuffers(game.GraphicsDevice);
            model.generateBoundingBox(false);
            return model;
        }

        private static Matrix CreateWorldMatrix(LRVector3 position, LRVector3 rotationFwd, LRVector3 rotationUp)
        {
            Vector3 right = Vector3.Cross(rotationUp.toXNAVector(), rotationFwd.toXNAVector());
            return new Matrix(rotationFwd.X, rotationFwd.Y, rotationFwd.Z, 0f, right.X, right.Y, right.Z, 0f, rotationUp.X, rotationUp.Y, rotationUp.Z, 0f, position.X, position.Y, position.Z, 1f);
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

        public static RRBFile loadRRBFile(string rrbpath)
        {
            RRBFile file = new RRBFile(loadRRB(rrbpath), rrbpath)
            {
                color = GetRRBColor(rrbpath)
            };
            file.generatePoints();
            return file;
        }

        public static LR1TrackEditor.SKB loadSKB(string skbpath)
        {
            Utils.WriteLine("Loading SKB: " + skbpath, ConsoleColor.White);
            return new LR1TrackEditor.SKB(skbpath);
        }

        public static SPB loadSPB(string spbpath)
        {
            Utils.WriteLine("Loading SPB: " + spbpath, ConsoleColor.Green);
            return new SPB(spbpath);
        }

        public static CPB loadCPB(string cpbpath)
        {
            Utils.WriteLine("Loading CPB: " + cpbpath, ConsoleColor.Yellow);
            return new CPB(cpbpath);
        }

        public static HZB loadHZB(string hzbpath)
        {
            Utils.WriteLine("Loading HZB: " + hzbpath, ConsoleColor.Red);
            return new HZB(hzbpath);
        }

        public static EMB loadEMB(string embpath)
        {
            Utils.WriteLine("Loading EMB: " + embpath, ConsoleColor.DarkCyan);
            return new EMB(embpath);
        }

        /// <summary>
        /// Resolves a RAB file reference to a real file path.
        /// RAB references may have explicit extensions or need common ones tried.
        /// </summary>
        private static string ResolveRABPath(string directory, string reference, params string[] fallbackExtensions)
        {
            if (reference == null) return null;

            // Try exact path first
            string path = Path.Combine(directory, reference);
            if (File.Exists(path)) return path;

            // Try without extension + fallback extensions
            string baseName = Path.GetFileNameWithoutExtension(reference);
            foreach (string ext in fallbackExtensions)
            {
                path = Path.Combine(directory, baseName + ext);
                if (File.Exists(path)) return path;
            }

            return null;
        }

        private static bool TryParseNpcRRBFileName(string filepath, out int racerIndex, out int pathIndex)
        {
            Match match = NpcPathFileNameRegex.Match(Path.GetFileName(filepath));
            if (!match.Success)
            {
                racerIndex = 0;
                pathIndex = 0;
                return false;
            }

            racerIndex = int.Parse(match.Groups["racer"].Value, CultureInfo.InvariantCulture);
            pathIndex = int.Parse(match.Groups["path"].Value, CultureInfo.InvariantCulture);
            return true;
        }

        private static Microsoft.Xna.Framework.Color LightenColor(Microsoft.Xna.Framework.Color color, float amount)
        {
            byte Lighten(byte channel) => (byte)Math.Clamp((int)Math.Round(channel + ((255 - channel) * amount)), 0, 255);
            return new Microsoft.Xna.Framework.Color(Lighten(color.R), Lighten(color.G), Lighten(color.B));
        }

        private static Microsoft.Xna.Framework.Color DarkenColor(Microsoft.Xna.Framework.Color color, float amount)
        {
            byte Darken(byte channel) => (byte)Math.Clamp((int)Math.Round(channel * (1f - amount)), 0, 255);
            return new Microsoft.Xna.Framework.Color(Darken(color.R), Darken(color.G), Darken(color.B));
        }

        private static Microsoft.Xna.Framework.Color GetNpcBaseColor(int racerIndex)
        {
            switch (racerIndex)
            {
                case 1:
                    return new Microsoft.Xna.Framework.Color(160, 160, 160);

                case 2:
                    return new Microsoft.Xna.Framework.Color(224, 64, 64);

                case 3:
                    return new Microsoft.Xna.Framework.Color(255, 220, 0);

                case 4:
                    return new Microsoft.Xna.Framework.Color(48, 192, 96);

                case 5:
                    return new Microsoft.Xna.Framework.Color(64, 128, 255);
            }
            return Microsoft.Xna.Framework.Color.White;
        }

        private static Microsoft.Xna.Framework.Color GetRRBColor(string rrbpath)
        {
            if (!TryParseNpcRRBFileName(rrbpath, out int racerIndex, out int pathIndex))
            {
                return Microsoft.Xna.Framework.Color.White;
            }

            Microsoft.Xna.Framework.Color baseColor = GetNpcBaseColor(racerIndex);
            switch (pathIndex)
            {
                case 1:
                    return LightenColor(baseColor, 0.35f);

                case 2:
                    return DarkenColor(baseColor, 0.35f);

                default:
                    return baseColor;
            }
        }

        private static IEnumerable<string> GetRABTrackStringValues(RAB_Track track)
        {
            foreach (FieldInfo field in typeof(RAB_Track).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.FieldType == typeof(string))
                {
                    string value = field.GetValue(track) as string;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        yield return value;
                    }
                }
                else if (field.FieldType == typeof(string[]))
                {
                    string[] values = field.GetValue(track) as string[];
                    if (values == null)
                    {
                        continue;
                    }

                    foreach (string value in values)
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            yield return value;
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> FindNpcRRBPaths(string directory, RAB_Track track)
        {
            HashSet<string> paths = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            List<string> orderedPaths = new List<string>();

            foreach (string reference in GetRABTrackStringValues(track))
            {
                string resolvedPath = ResolveRABPath(directory, reference, ".RRB");
                if ((resolvedPath != null) &&
                    Path.GetExtension(resolvedPath).Equals(".RRB", StringComparison.InvariantCultureIgnoreCase) &&
                    TryParseNpcRRBFileName(resolvedPath, out int racerIndex, out _) &&
                    (racerIndex >= 1) &&
                    (racerIndex <= 5) &&
                    paths.Add(resolvedPath))
                {
                    orderedPaths.Add(resolvedPath);
                }
            }

            foreach (string path in Directory.EnumerateFiles(directory, "*.RRB"))
            {
                if (TryParseNpcRRBFileName(path, out int racerIndex, out _) &&
                    (racerIndex >= 1) &&
                    (racerIndex <= 5) &&
                    paths.Add(path))
                {
                    orderedPaths.Add(path);
                }
            }

            foreach (string path in orderedPaths
                .OrderBy(path =>
                {
                    TryParseNpcRRBFileName(path, out int racerIndex, out int pathIndex);
                    return racerIndex;
                })
                .ThenBy(path =>
                {
                    TryParseNpcRRBFileName(path, out int racerIndex, out int pathIndex);
                    return pathIndex;
                })
                .ThenBy(path => Path.GetFileName(path), StringComparer.InvariantCultureIgnoreCase))
            {
                yield return path;
            }
        }

        private static void LoadCollisionReference(GameView game, string directory, string collisionRef, HashSet<string> loadedPaths)
        {
            if (string.IsNullOrWhiteSpace(collisionRef))
            {
                return;
            }

            string wdbPath = ResolveRABPath(directory, collisionRef, ".WDB", ".WDF");
            if (wdbPath != null && loadedPaths.Add(wdbPath))
            {
                WDB collisionScene = new WDB(wdbPath);
                Dictionary<int, LR1TrackEditor.Model> gdbModels = new Dictionary<int, LR1TrackEditor.Model>();
                for (int i = 0; i < (collisionScene.GDBs ?? Array.Empty<string>()).Length; i++)
                {
                    string gdbName = collisionScene.GDBs[i];
                    string gdbPath = ResolveRABPath(directory, gdbName, ".GDB");
                    if (gdbPath != null && loadedPaths.Add(gdbPath))
                    {
                        gdbModels[i] = loadmodel(game, gdbPath, false);
                    }
                }

                HashSet<int> referencedGdbIndices = new HashSet<int>();
                foreach (KeyValuePair<string, WDB_StaticModel> current in collisionScene.StaticModels)
                {
                    if (current.Value?.ModelRef == null || !gdbModels.ContainsKey(current.Value.ModelRef.IndexGDB))
                    {
                        continue;
                    }

                    referencedGdbIndices.Add(current.Value.ModelRef.IndexGDB);
                    game.collisionModels.Add(new CollisionModelInstance
                    {
                        Model = gdbModels[current.Value.ModelRef.IndexGDB],
                        Transform = CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp),
                        SourcePath = wdbPath
                    });
                }

                for (int i = 0; i < (collisionScene.GDBs ?? Array.Empty<string>()).Length; i++)
                {
                    if (gdbModels.ContainsKey(i) && !referencedGdbIndices.Contains(i))
                    {
                        game.collisionModels.Add(new CollisionModelInstance
                        {
                            Model = gdbModels[i],
                            Transform = Matrix.Identity,
                            SourcePath = wdbPath
                        });
                    }
                }

                Dictionary<int, LR1TrackEditor.Model> bvbModels = new Dictionary<int, LR1TrackEditor.Model>();
                for (int i = 0; i < (collisionScene.BVBs ?? Array.Empty<string>()).Length; i++)
                {
                    string bvbName = collisionScene.BVBs[i];
                    string bvbPath = ResolveRABPath(directory, bvbName, ".BVB");
                    if (bvbPath != null && loadedPaths.Add(bvbPath))
                    {
                        bvbModels[i] = loadCollisionBVB(game, bvbPath);
                    }
                }

                HashSet<int> referencedBvbIndices = new HashSet<int>();
                foreach (KeyValuePair<string, WDB_BVBModel> current in collisionScene.BVBModels)
                {
                    if (current.Value == null || !bvbModels.ContainsKey(current.Value.ModelRef))
                    {
                        continue;
                    }

                    referencedBvbIndices.Add(current.Value.ModelRef);
                    game.collisionModels.Add(new CollisionModelInstance
                    {
                        Model = bvbModels[current.Value.ModelRef],
                        Transform = CreateWorldMatrix(current.Value.Position, current.Value.RotationFwd, current.Value.RotationUp),
                        SourcePath = wdbPath
                    });
                }

                for (int i = 0; i < (collisionScene.BVBs ?? Array.Empty<string>()).Length; i++)
                {
                    if (bvbModels.ContainsKey(i) && !referencedBvbIndices.Contains(i))
                    {
                        game.collisionModels.Add(new CollisionModelInstance
                        {
                            Model = bvbModels[i],
                            Transform = Matrix.Identity,
                            SourcePath = wdbPath
                        });
                    }
                }
                return;
            }

            string bvbDirectPath = ResolveRABPath(directory, collisionRef, ".BVB");
            if (bvbDirectPath != null && loadedPaths.Add(bvbDirectPath))
            {
                game.collisionModels.Add(new CollisionModelInstance
                {
                    Model = loadCollisionBVB(game, bvbDirectPath),
                    Transform = Matrix.Identity,
                    SourcePath = bvbDirectPath
                });
                return;
            }

            string gdbDirectPath = ResolveRABPath(directory, collisionRef, ".GDB");
            if (gdbDirectPath != null && loadedPaths.Add(gdbDirectPath))
            {
                game.collisionModels.Add(new CollisionModelInstance
                {
                    Model = loadmodel(game, gdbDirectPath, false),
                    Transform = Matrix.Identity,
                    SourcePath = gdbDirectPath
                });
            }
        }

        public static void ensureGamedir(GameView game, string filepath)
        {
            if (game.gamedir == "")
            {
                game.gamedir = Utils.getGamedir(filepath);
                if (game.gamedir != "")
                {
                    string commonDir = Path.Combine(game.gamedir, "GAMEDATA", "COMMON");
                    string pubricky = Path.Combine(commonDir, "PUBRICKY.GDB");
                    if (File.Exists(pubricky))
                    {
                        Console.WriteLine("Gamedir found at " + game.gamedir);
                        Console.WriteLine("Loading powerup models/textures");
                        game.pupbrick = loadmodel(game, pubricky, false);
                        game.enhabrick = loadmodel(game, Path.Combine(commonDir, "ENHABRIK.GDB"), false);
                        foreach (var kvp in loadmaterials(Path.Combine(commonDir, "POWERUP.MDB"), game.GraphicsDevice))
                        {
                            game.corematerials[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }
        }

        private static void DisposeSceneModels(GameView game)
        {
            foreach (LR1TrackEditor.Model model in game.models.Values)
            {
                model?.vertexbuffer?.Dispose();
                model?.indexbuffer?.Dispose();
            }
        }

        private static void MergeMaterials(GameView game, string mdbPath)
        {
            foreach (KeyValuePair<string, Material> kvp in loadmaterials(mdbPath, game.GraphicsDevice))
            {
                game.materials[kvp.Key] = kvp.Value;
            }
            game.currentmatfile = mdbPath;
        }

        private static void LoadSceneMaterials(GameView game, string wdbPath, bool clearExisting)
        {
            string directory = Path.GetDirectoryName(wdbPath);
            string stem = Path.GetFileNameWithoutExtension(wdbPath);
            string sceneMaterialPath = Path.Combine(directory, stem + ".MDB");
            string sceneTextureDbPath = Path.Combine(directory, stem + ".TDB");
            string combinedMaterialPath = Path.Combine(directory, "COMBINED.MDB");
            string combinedTextureDbPath = Path.Combine(directory, "COMBINED.TDB");

            if (clearExisting)
            {
                game.materials.Clear();
            }

            if (File.Exists(sceneMaterialPath) && File.Exists(sceneTextureDbPath))
            {
                MergeMaterials(game, sceneMaterialPath);
                return;
            }

            if (File.Exists(combinedMaterialPath) && File.Exists(combinedTextureDbPath))
            {
                if (!string.Equals(game.currentmatfile, combinedMaterialPath, StringComparison.InvariantCultureIgnoreCase) || clearExisting)
                {
                    MergeMaterials(game, combinedMaterialPath);
                }
            }
        }

        private static List<LoadedMabDefinition> LoadSceneMabs(string wdbPath, WDB scene)
        {
            List<LoadedMabDefinition> loadedMabs = new List<LoadedMabDefinition>();
            if (scene?.MABs == null || scene.MABs.Length == 0)
            {
                return loadedMabs;
            }

            string directory = Path.GetDirectoryName(wdbPath);
            foreach (string mabRef in scene.MABs)
            {
                string mabPath = ResolveRABPath(directory, mabRef, ".MAB", ".MAF");
                if (mabPath == null)
                {
                    continue;
                }

                try
                {
                    Utils.WriteLine("Loading MAB: " + mabPath, ConsoleColor.DarkGreen);
                    MAB mab = new MAB(mabPath);
                    LoadedMabDefinition loaded = new LoadedMabDefinition
                    {
                        SourcePath = mabPath,
                        DisplayName = Path.GetFileNameWithoutExtension(mabPath)
                    };

                    MAB_MaterialFrame[] materialFrames = mab.MaterialFrames ?? Array.Empty<MAB_MaterialFrame>();
                    MAB_Animation[] animations = mab.Animations ?? Array.Empty<MAB_Animation>();
                    for (int animationIndex = 0; animationIndex < animations.Length; animationIndex++)
                    {
                        MAB_Animation sourceAnimation = animations[animationIndex];
                        MabAnimationDefinition animation = new MabAnimationDefinition
                        {
                            Id = loaded.DisplayName + "@Animation" + animationIndex.ToString(CultureInfo.InvariantCulture),
                            DisplayName = loaded.DisplayName + " Animation " + animationIndex.ToString(CultureInfo.InvariantCulture),
                            SourceName = loaded.DisplayName,
                            SourceIndex = animationIndex,
                            Speed = sourceAnimation?.Speed ?? 0,
                            LogicalFrameCount = Math.Max(sourceAnimation?.Frames ?? 0, 1)
                        };

                        if (sourceAnimation != null)
                        {
                            for (int sequenceIndex = 0; sequenceIndex < sourceAnimation.AnimationLength; sequenceIndex++)
                            {
                                int sourceFrameIndex = sourceAnimation.AnimationOffset + sequenceIndex;
                                if (sourceFrameIndex < 0 || sourceFrameIndex >= materialFrames.Length)
                                {
                                    continue;
                                }

                                MAB_MaterialFrame sourceFrame = materialFrames[sourceFrameIndex];
                                if (sourceFrame == null)
                                {
                                    continue;
                                }

                                MabFrameDefinition frame = new MabFrameDefinition
                                {
                                    MaterialName = sourceFrame.Material ?? string.Empty,
                                    FrameIndex = sourceFrame.Frame
                                };
                                animation.SequenceFrames.Add(frame);
                                if (!string.IsNullOrWhiteSpace(frame.MaterialName))
                                {
                                    animation.ReferencedMaterials.Add(frame.MaterialName);
                                }
                            }
                        }

                        loaded.Animations.Add(animation);
                    }

                    loadedMabs.Add(loaded);
                }
                catch (Exception ex)
                {
                    Utils.WriteLine("Failed to load MAB: " + ex.Message, ConsoleColor.Red);
                }
            }

            return loadedMabs;
        }

        private static IEnumerable<string> GetAdditionalSceneReferences(RAB_Track track)
        {
            if (!string.IsNullOrWhiteSpace(track.Unknown27))
            {
                yield return track.Unknown27;
            }

            if (!string.IsNullOrWhiteSpace(track.Unknown34))
            {
                yield return track.Unknown34;
            }

            if (!string.IsNullOrWhiteSpace(track.Unknown3B))
            {
                yield return track.Unknown3B;
            }

            if (!string.IsNullOrWhiteSpace(track.Unknown40))
            {
                yield return track.Unknown40;
            }

            if (!string.IsNullOrWhiteSpace(track.Unknown45))
            {
                yield return track.Unknown45;
            }
        }

        public static void loadRAB(GameView game, string rabpath)
        {
            Utils.WriteLine("Loading RAB: " + rabpath, ConsoleColor.White);
            RAB rab = new RAB(rabpath);
            game.rab = rab;
            game.currentRABfile = rabpath;

            string dir = Path.GetDirectoryName(rabpath);
            string gamedir = Utils.getGamedir(rabpath);
            string commonDir = (gamedir != "") ? Path.Combine(gamedir, "GAMEDATA", "COMMON") : null;
            RAB_Track track = rab.Track;
            bool loadTrackSkybox = Settings.Default.TrackLoadSkybox;
            bool loadTrackPowerups = Settings.Default.TrackLoadPowerups;
            bool loadTrackStartPositions = Settings.Default.TrackLoadStartPositions;
            bool loadTrackCheckpoints = Settings.Default.TrackLoadCheckpoints;
            bool loadTrackHazards = Settings.Default.TrackLoadHazards;
            bool loadTrackEmitters = Settings.Default.TrackLoadEmitters;
            bool loadTrackRacerPaths = Settings.Default.TrackLoadRacerPaths;
            bool loadTrackCollisionGeometry = Settings.Default.TrackLoadCollisionGeometry;

            // Load materials and WDB scene
            if (track.MaybeTrackScene != null)
            {
                string wdbPath = ResolveRABPath(dir, track.MaybeTrackScene, ".WDB", ".WDF");
                if (wdbPath != null)
                {
                    try
                    {
                        LoadSceneMaterials(game, wdbPath, true);
                        game.wdb = loadWDB(game, wdbPath, true);
                        game.currentWDBfile = wdbPath;
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load WDB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            HashSet<string> loadedScenePaths = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (!string.IsNullOrWhiteSpace(game.currentWDBfile))
            {
                loadedScenePaths.Add(game.currentWDBfile);
            }

            foreach (string sceneReference in GetAdditionalSceneReferences(track))
            {
                string extraWdbPath = ResolveRABPath(dir, sceneReference, ".WDB", ".WDF");
                if (extraWdbPath == null || !loadedScenePaths.Add(extraWdbPath))
                {
                    continue;
                }

                try
                {
                    LoadSceneMaterials(game, extraWdbPath, false);
                    game.extraWdbScenes.Add(loadWDB(game, extraWdbPath, false));
                }
                catch (Exception ex)
                {
                    Utils.WriteLine("Failed to load extra WDB scene: " + ex.Message, ConsoleColor.Red);
                }
            }

            // Load collision geometry
            if (loadTrackCollisionGeometry && track.MaybeCollisionMeshes != null)
            {
                HashSet<string> loadedCollisionPaths = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (string collisionRef in track.MaybeCollisionMeshes)
                {
                    try
                    {
                        LoadCollisionReference(game, dir, collisionRef, loadedCollisionPaths);
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load collision geometry: " + ex.Message, ConsoleColor.Red);
                    }
                }
                Utils.WriteLine("Loaded collision meshes: " + game.collisionModels.Count.ToString(CultureInfo.InvariantCulture), ConsoleColor.DarkYellow);
            }
            else if (!loadTrackCollisionGeometry && track.MaybeCollisionMeshes != null && track.MaybeCollisionMeshes.Length > 0)
            {
                Utils.WriteLine("Skipping collision geometry load because it is disabled in Options.", ConsoleColor.Yellow);
            }

            // Load skybox
            if (loadTrackSkybox && track.SkyBoxFile != null)
            {
                string skbPath = ResolveRABPath(dir, track.SkyBoxFile, ".SKB");
                if (skbPath != null)
                {
                    try
                    {
                        game.skb = loadSKB(skbPath);
                        SKB_Gradient gradient = game.skb.Gradients[game.skb.Default];
                        game.skbmesh = Utils.GenerateSKBMesh(
                            new Microsoft.Xna.Framework.Color(gradient.Color1.R, gradient.Color1.G, gradient.Color1.B),
                            new Microsoft.Xna.Framework.Color(gradient.Color2.R, gradient.Color2.G, gradient.Color2.B),
                            new Microsoft.Xna.Framework.Color(gradient.Color3.R, gradient.Color3.G, gradient.Color3.B));
                        game.form.refreshSKB();
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load SKB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            // Load powerups (need core models first)
            if (loadTrackPowerups && track.PowerupFiles != null && track.PowerupFiles.Length > 0)
            {
                string pwbPath = ResolveRABPath(dir, track.PowerupFiles[0], ".PWB", ".PWF");
                if (pwbPath != null)
                {
                    try
                    {
                        game.pwb = loadPWB(game, pwbPath);
                        if (game.pwb != null)
                        {
                            game.form.PWBToolStripItemChecked = true;
                            game.form.refreshPWB(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load PWB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            // Load start positions
            if (loadTrackStartPositions && track.StartPosFile != null)
            {
                string spbPath = ResolveRABPath(dir, track.StartPosFile, ".SPB", ".SPF");
                if (spbPath != null)
                {
                    try
                    {
                        game.spb = loadSPB(spbPath);
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load SPB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            // Load checkpoints
            if (loadTrackCheckpoints && track.CheckpointFiles != null && track.CheckpointFiles.Length > 0)
            {
                string cpbPath = ResolveRABPath(dir, track.CheckpointFiles[0], ".CPB", ".CPF");
                if (cpbPath != null)
                {
                    try
                    {
                        game.cpb = loadCPB(cpbPath);
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load CPB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            // Load hazards
            if (loadTrackHazards && track.HazardFile != null)
            {
                string hzbPath = ResolveRABPath(dir, track.HazardFile, ".HZB", ".HZF");
                if (hzbPath != null)
                {
                    try
                    {
                        game.hzb = loadHZB(hzbPath);
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load HZB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            // Load emitters
            game.embs.Clear();
            if (loadTrackEmitters && track.EmitterFiles != null)
            {
                foreach (string emRef in track.EmitterFiles)
                {
                    if (emRef == null) continue;
                    // Track-specific emitters are in level dir, global ones in COMMON
                    string embPath = ResolveRABPath(dir, emRef, ".EMB", ".EMT");
                    if (embPath == null && commonDir != null)
                    {
                        embPath = ResolveRABPath(commonDir, emRef, ".EMB", ".EMT");
                    }
                    if (embPath != null)
                    {
                        try
                        {
                            game.embs.Add(loadEMB(embPath));
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteLine("Failed to load EMB: " + ex.Message, ConsoleColor.Red);
                        }
                    }
                }
            }

            int importedNpcPaths = 0;
            if (loadTrackRacerPaths)
            {
                foreach (string rrbPath in FindNpcRRBPaths(dir, track))
                {
                    if (game.rrbs.Any(x => string.Equals(x.filepath, rrbPath, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        continue;
                    }

                    try
                    {
                        game.rrbs.Add(loadRRBFile(rrbPath));
                        importedNpcPaths++;
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load NPC RRB: " + ex.Message, ConsoleColor.Red);
                    }
                }
            }

            if (importedNpcPaths > 0)
            {
                Utils.WriteLine("Loaded NPC paths: " + importedNpcPaths, ConsoleColor.Yellow);
            }

            game.form.refreshRRB();
        }

        public static WDB loadWDB(GameView game, string wdbpath, bool clearExistingModels = true)
        {
            WDB wdb = new WDB(wdbpath);
            Utils.WriteLine("Loading WDB: " + wdbpath, ConsoleColor.Magenta);
            if (clearExistingModels)
            {
                DisposeSceneModels(game);
                game.models.Clear();
            }
            string directoryName = Path.GetDirectoryName(wdbpath);
            Utils.WriteLine("WDB GDBs[" + wdb.GDBs.Length + "]: " + string.Join(", ", wdb.GDBs), ConsoleColor.Magenta);
            Utils.WriteLine("WDB GDB2s[" + wdb.GDB2s.Length + "]: " + string.Join(", ", wdb.GDB2s), ConsoleColor.Magenta);
            foreach (string str2 in wdb.GDBs)
            {
                if (!game.models.ContainsKey(str2))
                {
                    game.models[str2] = loadmodel(game, Path.Combine(directoryName, str2 + ".gdb"), false);
                }
            }
            foreach (string str2 in wdb.GDB2s)
            {
                if (!game.models.ContainsKey(str2))
                {
                    game.models[str2] = loadmodel(game, Path.Combine(directoryName, str2 + ".gdb"), false);
                }
            }
            foreach (var kvp in wdb.StaticModels)
            {
                string refGdb = (kvp.Value.ModelRef != null && kvp.Value.ModelRef.IndexGDB >= 0 && kvp.Value.ModelRef.IndexGDB < wdb.GDBs.Length)
                    ? wdb.GDBs[kvp.Value.ModelRef.IndexGDB] : "null";
                Utils.WriteLine("  Static: " + kvp.Key + " -> GDB index " + (kvp.Value.ModelRef?.IndexGDB.ToString() ?? "null") + " = " + refGdb, ConsoleColor.DarkMagenta);
            }
            foreach (var kvp in wdb.BDBModels)
            {
                string refGdb = (kvp.Value.ModelRef != null && kvp.Value.ModelRef.IndexGDB >= 0 && kvp.Value.ModelRef.IndexGDB < wdb.GDBs.Length)
                    ? wdb.GDBs[kvp.Value.ModelRef.IndexGDB] : "null";
                string refBdb = (kvp.Value.ModelRef != null && kvp.Value.ModelRef.IndexBDB >= 0 && kvp.Value.ModelRef.IndexBDB < wdb.BDBs.Length)
                    ? wdb.BDBs[kvp.Value.ModelRef.IndexBDB] : "null";
                Utils.WriteLine("  BDB: " + kvp.Key + " -> GDB index " + (kvp.Value.ModelRef?.IndexGDB.ToString() ?? "null") + " = " + refGdb + ", BDB = " + refBdb, ConsoleColor.DarkMagenta);
            }
            game.RegisterSceneResources(wdb, wdbpath, LoadSceneMabs(wdbpath, wdb), clearExistingModels);
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

