namespace LR1TrackEditor
{
    using System;

    public class ModelPart
    {
        public string material;
        public ushort boneid = ushort.MaxValue;
        public int vertexstart;
        public int indexstart;
        public int numvertices;
        public int numindices;
        public bool visible = true;
    }
}

