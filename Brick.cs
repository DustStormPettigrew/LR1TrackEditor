namespace LR1TrackEditor
{
    using LibLR1.Utils;
    using System;

    public class Brick
    {
        public LRVector3 Position;
        public int index;
        public bool colored;
        public string Color;

        public string Description =>
            this.Color + "@" + this.Position.ToString();
    }
}

