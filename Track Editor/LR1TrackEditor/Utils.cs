namespace LR1TrackEditor
{
    using LibLR1.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public static class Utils
    {
        private const float EPSILON = 1E-07f;
        public static FormEditor form;
        public static GameView game;

        public static float? distanceToTriangle(Ray input)
        {
            ushort[] indices = game.loadedmodel.indices;
            List<Vector3> points = game.loadedmodel.GetPoints();
            float maxValue = float.MaxValue;
            int index = 0;
            while (true)
            {
                if (index >= indices.Length)
                {
                    float? nullable2;
                    if (maxValue != float.MaxValue)
                    {
                        nullable2 = new float?(maxValue);
                    }
                    else
                    {
                        nullable2 = null;
                    }
                    return nullable2;
                }
                float? nullable = RayIntersectTriangle(input, points[indices[index]], points[indices[index + 1]], points[indices[index + 2]]);
                if ((nullable != null) && (nullable.Value < maxValue))
                {
                    maxValue = nullable.Value;
                }
                index += 3;
            }
        }

        public static VertexPTC[] GenerateSKBMesh(Color color1, Color color2, Color color3) =>
            new VertexPTC[] { new VertexPTC(new Vector3(10f, 0f, -5f), color1), new VertexPTC(new Vector3(-10f, 0f, -5f), color1), new VertexPTC(new Vector3(10f, 0f, 2.5f), color2), new VertexPTC(new Vector3(-10f, 0f, 2.5f), color2), new VertexPTC(new Vector3(10f, 0f, 10f), color3), new VertexPTC(new Vector3(-10f, 0f, 10f), color3) };

        public static string getGamedir(string modelpath)
        {
            string pathRoot = Path.GetPathRoot(modelpath);
            while (true)
            {
                string directoryName;
                if (modelpath == pathRoot)
                {
                    directoryName = "";
                }
                else
                {
                    if ((Path.GetFileName(modelpath) != "MENUDATA") && (Path.GetFileName(modelpath) != "GAMEDATA"))
                    {
                        modelpath = Path.GetDirectoryName(modelpath);
                        continue;
                    }
                    directoryName = Path.GetDirectoryName(modelpath);
                }
                return directoryName;
            }
        }

        public static void OpenFileDialog(int filterindex)
        {
            Func<RRBFile, bool> predicate = null;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Track Files|*.RAB|3D Objects|*.GDB|Powerups|*.PWB|AI Paths|*.RRB|3D Scenes|*.WDB",
                FilterIndex = filterindex,
                Multiselect = false,
                CheckFileExists = true
            };
            game.IsMouseVisible = true;
            DialogResult result = STAShowDialog(ofd);
            if (game.mouselock)
            {
                MouseHelper.SetPosition(game.drawsurface, game.width / 2, game.height / 2);
                game.IsMouseVisible = false;
            }
            if (result == DialogResult.OK)
            {
                if (ofd.FileName.EndsWith("RAB", StringComparison.InvariantCultureIgnoreCase))
                {
                    game.ClearTrackData();
                    game.loadedmodel?.vertexbuffer.Dispose();
                    game.loadedmodel?.indexbuffer.Dispose();
                    game.loadedmodel = null;
                    form.refreshPWB(false);
                    form.refreshRRB();
                    form.ClearEdits(null);

                    // Set up gamedir and core powerup models if not already loaded
                    Loader.ensureGamedir(game, ofd.FileName);

                    // loadRAB handles everything: materials, WDB (with all GDBs), SKB, PWB, SPB, CPB, HZB, EMB
                    // The track mesh is a WDB static object, so enable static objects by default
                    Loader.loadRAB(game, ofd.FileName);

                    game.track = true;
                    form.refreshSKB();
                    form.refreshWDB();
                    form.PWBToolStripItemChecked = game.pwb != null;
                    form.staticObjectsToolStripItemChecked = game.wdb != null;
                    form.SetTabControlEnabled(true);
                }
                else if (ofd.FileName.EndsWith("GDB", StringComparison.InvariantCultureIgnoreCase))
                {
                    game.pwb = null;
                    form.refreshPWB(false);
                    game.loadModel(ofd.FileName);
                }
                else if (ofd.FileName.EndsWith("PWB", StringComparison.InvariantCultureIgnoreCase))
                {
                    game.pwb = Loader.loadPWB(game, ofd.FileName);
                    if (game.pwb is object)
                    {
                        form.PWBToolStripItemChecked = true;
                        form.refreshPWB(false);
                    }
                }
                else if (!ofd.FileName.EndsWith("RRB", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ofd.FileName.EndsWith("WDB", StringComparison.InvariantCultureIgnoreCase))
                    {
                        game.wdb = Loader.loadWDB(game, ofd.FileName);
                        form.staticObjectsToolStripItemChecked = true;
                        form.refreshWDB();
                    }
                }
                else
                {
                    if (predicate == null)
                    {
                        predicate = rrb => rrb.filepath == ofd.FileName;
                    }
                    if (game.rrbs.Where<RRBFile>(predicate).Count<RRBFile>() != 0)
                    {
                        System.Windows.Forms.MessageBox.Show("That RRB file is already loaded.", "Notice");
                    }
                    else
                    {
                        game.rrbs.Add(Loader.loadRRBFile(ofd.FileName));
                        form.refreshRRB();
                    }
                }
            }
        }

        private static float? RayIntersectTriangle(Ray ray, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float? nullable;
            float? nullable2;
            Vector3 vector = v2 - v1;
            Vector3 vector2 = v3 - v1;
            Vector3 vector3 = Vector3.Cross(ray.Direction, vector2);
            float num = Vector3.Dot(vector, vector3);
            if (InputHandler.cullmode != 1)
            {
                if (InputHandler.cullmode != 2)
                {
                    if ((num < 1E-07f) && (num > -1E-07f))
                    {
                        return null;
                    }
                }
                else if (num > -1E-07f)
                {
                    return null;
                }
            }
            else if (num < 1E-07f)
            {
                return null;
            }
            Vector3 vector4 = ray.Position - v1;
            float num2 = Vector3.Dot(vector4, vector3) / num;
            if ((num2 < 0f) || (num2 > 1f))
            {
                nullable2 = null;
                nullable = nullable2;
            }
            else
            {
                Vector3 vector5 = Vector3.Cross(vector4, vector);
                float num3 = Vector3.Dot(ray.Direction, vector5) / num;
                if ((num3 < 0f) || ((num2 + num3) > 1f))
                {
                    nullable2 = null;
                    nullable = nullable2;
                }
                else
                {
                    float num4 = Vector3.Dot(vector2, vector5) / num;
                    if (num4 > 1E-07f)
                    {
                        nullable = new float?(num4);
                    }
                    else
                    {
                        nullable2 = null;
                        nullable = nullable2;
                    }
                }
            }
            return nullable;
        }

        public static DialogResult STAShowDialog(FileDialog dialog)
        {
            Console.WriteLine("Opening FileDialog");
            DialogState state = new DialogState
            {
                dialog = dialog
            };
            Thread thread = new Thread(new ThreadStart(state.ThreadProcShowDialog));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            Console.WriteLine("DialogResult=" + state.result.ToString());
            return state.result;
        }

        public static Vector2 toXNAVector(this LRVector2 input) =>
            new Vector2(input.X, input.Y);

        public static Vector3 toXNAVector(this LRVector3 input) =>
            new Vector3(input.X, input.Y, input.Z);

        public static Vector3 vectorfromcolor(Color input) =>
            new Vector3
            {
                X = ((float)input.R) / 255f,
                Y = ((float)input.G) / 255f,
                Z = ((float)input.B) / 255f
            };

        public static void WriteLine(string text, ConsoleColor? col = new ConsoleColor?())
        {
            if (col == null)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.ForegroundColor = col.Value;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        private class DialogState
        {
            public DialogResult result;
            public FileDialog dialog;

            public void ThreadProcShowDialog()
            {
                this.result = this.dialog.ShowDialog();
            }
        }
    }
}

