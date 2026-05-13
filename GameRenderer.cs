using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LR1TrackEditor
{
    public class GameRenderer
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly BasicEffect basicEffect;
        private readonly RasterizerState rasterizerState;

        // TODO: Add camera, model, and brick data

        public GameRenderer(GraphicsDevice device, BasicEffect effect, RasterizerState rasterizer)
        {
            graphicsDevice = device;
            basicEffect = effect;
            rasterizerState = rasterizer;

            InitializeScene();
        }

        private void InitializeScene()
        {
            // Load models, textures, etc.
        }

        public void Draw()
        {
            graphicsDevice.Clear(Color.CornflowerBlue);

            // Apply effect
            basicEffect.CurrentTechnique.Passes[0].Apply();

            // TODO: Add real draw logic here (vertex buffers, bricks, etc.)
        }
    }
}
