using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Dreadnought.Common {
    class SkySphere {
        Model model;
        TextureCube texture;
        Effect effect;
        Game game;

        public SkySphere(Game game) {
            this.game = game;
        }

        public void Load(ContentManager content) {
            model = content.Load<Model>("Sphere");
            texture = content.Load<TextureCube>("SunInSpace");
            effect = content.Load<Effect>("SkySphere");

            effect.Parameters["SurfaceTexture"].SetValue(texture);

            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = effect;
                }
            }
        }

        public void Update(GameTime gameTime) {
                effect.Parameters["EyePosition"].SetValue( game.Camera.Position.Local );
                effect.Parameters["World"].SetValue( Matrix.CreateScale(1000) * game.Camera.World );
                effect.Parameters["View"].SetValue( game.Camera.View );
                effect.Parameters["Projection"].SetValue( game.Camera.Projection );
        }

        public void Draw() {

            game.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (ModelMesh mesh in model.Meshes) {
                mesh.Draw();
            }
            
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

    }
}
