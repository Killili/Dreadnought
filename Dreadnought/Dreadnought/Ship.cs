using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
    class Ship {
        Model model;
        Texture2D texture;
        Matrix[] transforms;
        private Game game;

        public Ship(Game game) {
            this.game = game;
        }

        public void Load(ContentManager content) {
            model = content.Load<Model>("Ship");
            //texture = content.Load<Texture2D>("ShipTexture");
            transforms = new Matrix[model.Bones.Count];

        }

        public void Update(GameTime gameTime) {
            //model.Root.Transform *= Matrix.CreateFromAxisAngle( new Vector3(0f,1f,1f) , MathHelper.ToRadians(0.1f));
            model.Bones["Radar"].Transform *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(1.5f));

            model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix slot = transforms[model.Bones["Slot"].Index];


            Vector3 tt = Vector3.Transform(game.Camera.Position, Matrix.Invert(slot));
            tt.Normalize();
            Matrix turret = model.Bones["Turret"].Transform;

            Vector3 front = turret.Forward;
            

            if(Vector3.Dot(front, tt) > -0.99) {
                front = Vector3.Lerp(front, tt, 0.1f);
            } else {
                // Special case for if we are turning exactly 180 degrees.
                front = Vector3.Lerp(front, turret.Right, 0.1f);
            }

            Vector3 right = Vector3.Cross(front, Vector3.Up);
            Vector3 up = Vector3.Cross(right, front);

            front.Normalize();
            right.Normalize();
            up.Normalize();

            if(Vector3.Dot(front, Vector3.Up) > -0.1f) {

                model.Bones["Turret"].Transform = Matrix.CreateWorld(Vector3.Zero, front, up);
                model.CopyAbsoluteBoneTransformsTo(transforms);

                game.Camera.AddDebugStar(transforms[model.Bones["Slot"].Index]);
            }

        }

        public void Draw() {

            foreach(ModelMesh mesh in model.Meshes) {
                Matrix world = transforms[mesh.ParentBone.Index];
                foreach(BasicEffect effect in mesh.Effects) {

                    effect.World = world;
                    effect.View = game.Camera.View;
                    effect.Projection = game.Camera.Projection;
                    effect.DirectionalLight0.Direction = Vector3.Down;
                    effect.LightingEnabled = true;
                    effect.PreferPerPixelLighting = true;
                    //effect.TextureEnabled = true;
                    //effect.Texture = texture;

                    effect.DiffuseColor = new Vector3(0.75f);


                }
                mesh.Draw();

            }
        }
    }
}
