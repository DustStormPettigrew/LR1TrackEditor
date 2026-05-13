namespace LR1TrackEditor
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPTN
    {
        public Vector3 Position;
        public Vector2 TextureCoordinates;
        public Vector3 Normals;
        public static Microsoft.Xna.Framework.Graphics.VertexDeclaration VertexDeclaration;
        public VertexPTN(Vector3 Position, Vector2 TextureCoordinates, Vector3 Normals)
        {
            this.Normals = Normals;
            this.Position = Position;
            this.TextureCoordinates = TextureCoordinates;
        }

        static VertexPTN()
        {
            VertexElement[] elements = new VertexElement[] { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0) };
            VertexDeclaration = new Microsoft.Xna.Framework.Graphics.VertexDeclaration(elements);
        }
    }
}

