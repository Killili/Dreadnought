using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Dreadnought.Base;

namespace Dreadnought.Common {
    class SkySphere:GameEntity {
        Model model;
        TextureCube texture;
        Effect effect;

        public SkySphere() {
				Game.RegisterDraw(this);
				Game.RegisterUpdate(this);
				Load();
        }

        void Load() {
            model = Game.Content.Load<Model>("Sphere");
				texture = Game.Content.Load<TextureCube>("SunInSpace");
				effect = Game.Content.Load<Effect>("SkySphere");

            effect.Parameters["SurfaceTexture"].SetValue(texture);

            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = effect;
                }
            }
        }

        public override void Update(GameTime gameTime) {
                effect.Parameters["EyePosition"].SetValue( Game.Camera.Position.Local );
                effect.Parameters["World"].SetValue( Matrix.CreateScale(1000) * Game.Camera.World );
                effect.Parameters["View"].SetValue( Game.Camera.View );
                effect.Parameters["Projection"].SetValue( Game.Camera.Projection );
        }

		  public override void Draw(GameTime gameTime) {

            Game.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (ModelMesh mesh in model.Meshes) {
                mesh.Draw();
            }
            
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

    }
}
