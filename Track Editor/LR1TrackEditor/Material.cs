namespace LR1TrackEditor
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    public class Material
    {
        public Texture2D texture;
        public Color diffusecolor = Color.White;
        public Color ambientcolor = Color.White;
        public byte alpha = 0xff;
        public bool semitransparent = false;
        public string name = string.Empty;
        public string textureName = string.Empty;
        public string textureDirectory = string.Empty;
        public bool hasTransparentColor = false;
        public System.Drawing.Color transparentColor = System.Drawing.Color.Empty;
        public Dictionary<int, Texture2D> animationFrameTextures = new Dictionary<int, Texture2D>();

        public Material CloneWithTexture(Texture2D overrideTexture)
        {
            return new Material
            {
                texture = overrideTexture,
                diffusecolor = this.diffusecolor,
                ambientcolor = this.ambientcolor,
                alpha = this.alpha,
                semitransparent = this.semitransparent,
                name = this.name,
                textureName = this.textureName,
                textureDirectory = this.textureDirectory,
                hasTransparentColor = this.hasTransparentColor,
                transparentColor = this.transparentColor,
                animationFrameTextures = this.animationFrameTextures
            };
        }
    }
}

