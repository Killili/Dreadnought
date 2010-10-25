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

		private Vector3 moment;
		private Vector3 position;
		
		private Quaternion orientation;
		private Vector3 rotation;
		
		private float speedLimit;
		private float turnLimit;
		private float helpSpeed;

		

		public Vector3 Position { get{ return position; }  set{ position = value;} }

		public Ship(Game game) {
			this.game = game;
		}

		public void Load(ContentManager content) {
			model = content.Load<Model>("Ship");
			//texture = content.Load<Texture2D>("ShipTexture");
			transforms = new Matrix[model.Bones.Count];
			moment = new Vector3(0.0f);
			position = new Vector3(0.0f);
			speedLimit = 0.02f;
			turnLimit = 0.0002f;
			helpSpeed = 0.01f;
			rotation = new Vector3(0.0f);
			orientation = new Quaternion(Vector3.Up,0f);
		}

		public void Update(GameTime gameTime) {
			position += moment;
			orientation = Quaternion.Concatenate( orientation , Quaternion.CreateFromAxisAngle(model.Root.Transform.Up, rotation.Y));
			orientation = Quaternion.Concatenate( orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Right, rotation.X));
			orientation.Normalize();
			//orientation = Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, 0f);
			
			
			model.Root.Transform = Matrix.CreateFromQuaternion(orientation) * Matrix.CreateTranslation(position);
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



		
		public void accelerate() {
			moment += Vector3.Transform( Vector3.Forward , orientation ) * speedLimit; 
		}

		public void decelerate() {
			moment -= Vector3.Transform(Vector3.Forward, orientation) * speedLimit;
		}

		public void turnLeft() {
			//direction = Vector3.Lerp(direction, Vector3.Cross(direction,Vector3.Up), turnLimit);
			rotation.Y += turnLimit;
			//rotationY.Normalize();
			//direction.Normalize();
		}

		public void turnRight() {
			//direction = Vector3.Lerp(direction, Vector3.Cross(direction, Vector3.Up), -turnLimit);
			rotation.Y -= turnLimit;
			//rotationY.Normalize();
			//direction.Normalize();
		}



		public void counterRotation() {
			if(Math.Abs(rotation.Y) <= turnLimit) {
				rotation.Y = 0;
			} else if(rotation.Y > 0) {
				rotation.Y -= turnLimit;
			} else if(rotation.Y < 0) {
				rotation.Y += turnLimit;
			}
			if(Math.Abs(rotation.X) <= turnLimit) {
				rotation.X = 0;
			} else if(rotation.X > 0) {
				rotation.X-= turnLimit;
			} else if(rotation.X < 0) {
				rotation.X += turnLimit;
			}
		}

		public void counterMoment() {
			moment -= Vector3.Normalize(moment) * speedLimit;
		}

		public void turnUp() {
			rotation.X += turnLimit;
		}

		public void turnDown() {
			rotation.X -= turnLimit;
		}

		public void rise() {
			moment += Vector3.Transform(Vector3.Up, orientation) * speedLimit; 
		}

		public void sink() {
			moment -= Vector3.Transform(Vector3.Up, orientation) * speedLimit; 
		}

		internal void rollLeft() {
			throw new NotImplementedException();
		}

		internal void rollRight() {
			throw new NotImplementedException();
		}
	}
}
