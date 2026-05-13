namespace LR1TrackEditor
{
    using LibLR1;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class RRBFile
    {
        public string filepath;
        public bool display;
        public RRB rrbfile;
        public List<VertexPTC> points;
        public Color color;

        public RRBFile(RRB rrbfile, string filepath)
        {
            this.rrbfile = rrbfile;
            this.filepath = filepath;
            this.display = true;
            this.points = new List<VertexPTC>();
            this.color = Color.White;
            this.generatePoints();
        }

        public unsafe void generatePoints()
        {
            this.points.Clear();
            Vector3 position = new Vector3(this.rrbfile.Unknown29.X, this.rrbfile.Unknown29.Y, this.rrbfile.Unknown29.Z);
            this.points.Add(new VertexPTC(position, this.color));
            foreach (RRB_Node node in this.rrbfile.Nodes)
            {
                Vector3* vectorPtr1 = &position;
                vectorPtr1->X += node.DeltaX.AsFloat;
                Vector3* vectorPtr2 = &position;
                vectorPtr2->Y += node.DeltaY.AsFloat;
                Vector3* vectorPtr3 = &position;
                vectorPtr3->Z += node.DeltaZ.AsFloat;
                this.points.Add(new VertexPTC(position, this.color));
            }
        }

        public string getname() =>
            Path.GetFileName(this.filepath);
    }
}

