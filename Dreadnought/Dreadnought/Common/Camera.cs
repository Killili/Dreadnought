using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Dreadnought.Common {
	public class Camera {

		public Vector3 Position { get; set; }
		public Vector3 LookAt { get; set; }

		public Matrix View { get; private set; }
		public Matrix Projection { get; private set; }
		public Matrix World { get; private set; }

		Game game;
		SkySphere background;
		private Vector3 lookAt;
		private List<VertexPositionColor> pointList;
		private List<short> pointOrder;
		private BasicEffect effect;
		public Vector3 Up;

		public Camera(Game game) {
			this.game = game;
			Position = new Vector3(500, 500, 500);
			pointList = new List<VertexPositionColor>();
			pointOrder = new List<short>();
			LookAt = Vector3.Zero;
			Up = Vector3.Up;
		}


		public void Load(ContentManager content) {
			background = new SkySphere(game);
			background.Load(content);
			effect = new BasicEffect(game.GraphicsDevice);
			effect.VertexColorEnabled = true;
			effect.World = game.World;
		}

		public void Update(GameTime gameTime) {
			World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			View = Matrix.CreateLookAt(Position, LookAt, Up);
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 200000);

			effect.World = World;
			effect.View = View;
			effect.Projection = Projection;

			background.Update(gameTime);
		}

		public void AddDebugVector(Vector3 v) {
			AddDebugVector(Vector3.Zero, v);
		}

		public void AddDebugVector(Vector3 v1, Vector3 v2) {
			Vector3 v = Vector3.Normalize(v2 - v1);
			Color color = new Color(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));

			pointList.Add(new VertexPositionColor(v1, color));
			pointList.Add(new VertexPositionColor(v2, color));
			pointOrder.Add((short)(pointList.Count - 2));
			pointOrder.Add((short)(pointList.Count - 1));
		}

		public void AddDebugStar(Matrix m) {
			Vector3 c = Vector3.Transform(Vector3.Zero, m);
			AddDebugVector(c, c + (m.Right * 100));
			AddDebugVector(c, c + (m.Up * 100));
			AddDebugVector(c, c + (m.Forward * 100));
		}

		public void Draw() {
			background.Draw();
			if(pointOrder.Count >= 2) {
				foreach(EffectPass pass in effect.CurrentTechnique.Passes) {
					pass.Apply();

					game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
						 PrimitiveType.LineList,
						 pointList.ToArray(),
						 0,  // vertex buffer offset to add to each element of the index buffer
						 pointList.Count,  // number of vertices in pointList
						 pointOrder.ToArray(),  // the index buffer
						 0,  // first index element to read
						 pointOrder.Count / 2  // number of primitives to draw
					);
				}
				pointList.Clear();
				pointOrder.Clear();
			}
		}
	}
}
