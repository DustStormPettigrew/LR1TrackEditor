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
                foreach (string gdbName in collisionScene.GDBs ?? Array.Empty<string>())
                {
                    string gdbPath = ResolveRABPath(directory, gdbName, ".GDB");
                    if (gdbPath != null && loadedPaths.Add(gdbPath))
                    {
                        game.collisionModels.Add(loadmodel(game, gdbPath, false));
                    }
                }

                foreach (string bvbName in collisionScene.BVBs ?? Array.Empty<string>())
                {
                    string bvbPath = ResolveRABPath(directory, bvbName, ".BVB");
                    if (bvbPath != null && loadedPaths.Add(bvbPath))
                    {
                        game.collisionModels.Add(loadCollisionBVB(game, bvbPath));
                    }
                }
                return;
            }

            string bvbDirectPath = ResolveRABPath(directory, collisionRef, ".BVB");
            if (bvbDirectPath != null && loadedPaths.Add(bvbDirectPath))
            {
                game.collisionModels.Add(loadCollisionBVB(game, bvbDirectPath));
                return;
            }

            string gdbDirectPath = ResolveRABPath(directory, collisionRef, ".GDB");
            if (gdbDirectPath != null && loadedPaths.Add(gdbDirectPath))
            {
                game.collisionModels.Add(loadmodel(game, gdbDirectPath, false));
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
                    // Load materials first (same name as WDB)
                    string mdbName = Path.GetFileNameWithoutExtension(wdbPath) + ".MDB";
                    string mdbPath = Path.Combine(dir, mdbName);
                    string tdbPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(wdbPath) + ".TDB");
                    if (File.Exists(mdbPath) && File.Exists(tdbPath))
                    {
                        game.materials.Clear();
                        foreach (var kvp in loadmaterials(mdbPath, game.GraphicsDevice))
                        {
                            game.materials[kvp.Key] = kvp.Value;
                        }
                        game.currentmatfile = mdbPath;
                    }
                    else
                    {
                        // Try COMBINED.MDB
                        mdbPath = Path.Combine(dir, "COMBINED.MDB");
                        tdbPath = Path.Combine(dir, "COMBINED.TDB");
                        if (File.Exists(mdbPath) && File.Exists(tdbPath))
                        {
                            game.materials.Clear();
                            foreach (var kvp in loadmaterials(mdbPath, game.GraphicsDevice))
                            {
                                game.materials[kvp.Key] = kvp.Value;
                            }
                            game.currentmatfile = mdbPath;
                        }
                    }

                    try
                    {
                        game.wdb = loadWDB(game, wdbPath);
                        game.currentWDBfile = wdbPath;
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLine("Failed to load WDB: " + ex.Message, ConsoleColor.Red);
                    }
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

        public static WDB loadWDB(GameView game, string wdbpath)
        {
            WDB wdb = new WDB(wdbpath);
            Utils.WriteLine("Loading WDB: " + wdbpath, ConsoleColor.Magenta);
            game.models.Clear();
            string directoryName = Path.GetDirectoryName(wdbpath);
            Utils.WriteLine("WDB GDBs[" + wdb.GDBs.Length + "]: " + string.Join(", ", wdb.GDBs), ConsoleColor.Magenta);
            Utils.WriteLine("WDB GDB2s[" + wdb.GDB2s.Length + "]: " + string.Join(", ", wdb.GDB2s), ConsoleColor.Magenta);
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
            foreach (var kvp in wdb.StaticModels)
            {
                string refGdb = (kvp.Value.ModelRef != null && kvp.Value.ModelRef.IndexGDB >= 0 && kvp.Value.ModelRef.IndexGDB < wdb.GDBs.Length)
                    ? wdb.GDBs[kvp.Value.ModelRef.IndexGDB] : "null";
                Utils.WriteLine("  Static: " + kvp.Key + " -> GDB index " + (kvp.Value.ModelRef?.IndexGDB.ToString() ?? "null") + " = " + refGdb, ConsoleColor.DarkMagenta);
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

