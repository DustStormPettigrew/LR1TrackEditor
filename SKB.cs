namespace LR1TrackEditor
{
    using LibLR1;
    using LibLR1.IO;
    using LibLR1.Utils;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SKB
    {
        private const byte ID_GRADIENTS = 0x2c;
        private const byte ID_GRADIENTBLOCK = 0x27;
        private const byte PROPERTY_DEFAULTGRADIENT = 0x2d;
        private const byte PROPERTY_UNKNOWNFLOAT = 0x2e;
        public Dictionary<string, SKB_Gradient> Gradients = new Dictionary<string, SKB_Gradient>();
        public string Default;
        public float? Unknownfloat;

        public SKB(string path)
        {
            LibLR1.SKB skb = new LibLR1.SKB(path);
            if (skb.Gradients != null)
            {
                foreach (Dictionary<string, SKB_Gradient> set in skb.Gradients)
                {
                    foreach (KeyValuePair<string, SKB_Gradient> kvp in set)
                    {
                        this.Gradients[kvp.Key] = kvp.Value;
                    }
                }
            }
            this.Default = skb.PreferredSet;
            this.Unknownfloat = skb.UnknownFloat;
        }

        public void Save(LRBinaryWriter writer)
        {
            if ((this.Gradients != null) && (this.Gradients.Count != 0))
            {
                writer.WriteByte(ID_GRADIENTS);
                writer.WriteArrayBlock<KeyValuePair<string, SKB_Gradient>>(new System.Action<LRBinaryWriter, KeyValuePair<string, SKB_Gradient>>(SKB.WriteGradientBlock), this.Gradients.ToArray<KeyValuePair<string, SKB_Gradient>>());
            }
            writer.WriteByte(PROPERTY_DEFAULTGRADIENT);
            writer.WriteStringWithHeader(this.Default);
            if (this.Unknownfloat != null)
            {
                writer.WriteByte(PROPERTY_UNKNOWNFLOAT);
                writer.WriteFloatWithHeader(this.Unknownfloat.Value);
            }
        }

        public void Save(string path)
        {
            LRBinaryWriter writer = new LRBinaryWriter(File.OpenWrite(path), true);
            try
            {
                this.Save(writer);
            }
            finally
            {
                writer?.Dispose();
            }
        }

        private static void WriteGradientBlock(LRBinaryWriter w, KeyValuePair<string, SKB_Gradient> gradient)
        {
            w.WriteByte(ID_GRADIENTBLOCK);
            w.WriteToken(Token.LeftBracket);
            w.WriteIntWithHeader(1);
            w.WriteToken(Token.RightBracket);
            w.WriteStringWithHeader(gradient.Key);
            w.WriteToken(Token.LeftCurly);
            w.WriteByte(ID_GRADIENTBLOCK);
            w.WriteStruct<SKB_Gradient>(new System.Action<LRBinaryWriter, SKB_Gradient>(SKB_Gradient.Write), gradient.Value);
            w.WriteToken(Token.RightCurly);
        }
    }
}
