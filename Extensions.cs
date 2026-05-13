namespace LR1TrackEditor
{
    using LibLR1;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class Extensions
    {
        private static readonly Dictionary<byte, string> colorstring;
        private static readonly Dictionary<string, byte> colorbyte;

        static Extensions()
        {
            Dictionary<byte, string> dictionary = new Dictionary<byte, string> {
                {
                    0x2a,
                    "Red"
                },
                {
                    0x2c,
                    "Blue"
                },
                {
                    0x2d,
                    "Green"
                },
                {
                    0x2b,
                    "Yellow"
                }
            };
            colorstring = dictionary;
            Dictionary<string, byte> dictionary2 = new Dictionary<string, byte> {
                {
                    "Red",
                    0x2a
                },
                {
                    "Blue",
                    0x2c
                },
                {
                    "Green",
                    0x2d
                },
                {
                    "Yellow",
                    0x2b
                }
            };
            colorbyte = dictionary2;
        }

        public static void AddTwice(this List<VertexPTC> input, VertexPTC item)
        {
            input.Add(item);
            input.Add(item);
        }

        public static byte ColorByte(this Brick brick) =>
            colorbyte[brick.Color];

        public static string ColorString(this PWB_ColorBrick brick) =>
            colorstring[brick.Color];

        public static float DistanceTo(this Plane plane, Vector3 point) =>
            Vector3.Dot(plane.Normal, point) - plane.D;

        public static float? Intersects(this Ray ray, Plane plane, bool culling)
        {
            float? nullable;
            if (culling)
            {
                nullable = ray.Intersects(plane);
            }
            else
            {
                float num = Vector3.Dot(plane.Normal, ray.Direction);
                if (!(num == 0f))
                {
                    nullable = new float?(-(Vector3.Dot(plane.Normal, ray.Position) + plane.D) / num);
                }
                else
                {
                    nullable = null;
                }
            }
            return nullable;
        }

        public static BoundingBox Transform(this BoundingBox input, Matrix transform) =>
            new BoundingBox
            {
                Min = Vector3.Transform(input.Min, transform),
                Max = Vector3.Transform(input.Max, transform)
            };
    }
}

