namespace LR1TrackEditor
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPTC
    {
        public Vector3 Position;
        public Vector2 TextureCoordinates;
        public Microsoft.Xna.Framework.Color Color;
        public static Microsoft.Xna.Framework.Graphics.VertexDeclaration VertexDeclaration;
        public VertexPTC(Vector3 Position, Vector2 TextureCoordinates, Microsoft.Xna.Framework.Color Color)
        {
            this.Color = Color;
            this.Position = Position;
            this.TextureCoordinates = TextureCoordinates;
        }

        public VertexPTC(Vector3 Position, Microsoft.Xna.Framework.Color Color)
        {
            this.Color = Color;
            this.Position = Position;
            this.TextureCoordinates = Vector2.Zero;
        }

        static VertexPTC()
        {
            VertexElement[] elements = new VertexElement[] { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0) };
            VertexDeclaration = new Microsoft.Xna.Framework.Graphics.VertexDeclaration(elements);
        }
    }
}

