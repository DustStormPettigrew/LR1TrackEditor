namespace LR1TrackEditor
{
    using Microsoft.Xna.Framework;

    public class CollisionModelInstance
    {
        public LR1TrackEditor.Model Model { get; set; }
        public Matrix Transform { get; set; } = Matrix.Identity;
        public string SourcePath { get; set; } = string.Empty;
    }
}
