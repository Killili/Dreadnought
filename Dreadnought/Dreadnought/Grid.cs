using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
	class Grid : Microsoft.Xna.Framework.DrawableGameComponent {

		private BasicEffect effect;
		private List<VertexPositionColor> pointList;
		private List<short> pointOrder;

		public Grid(Game game)
			: base(game) {
			pointList = new List<VertexPositionColor>();
			pointOrder = new List<short>();
			pointList.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color.White));
			pointList.Add(new VertexPositionColor(new Vector3(0, 0, 100), Color.White));
			pointList.Add(new VertexPositionColor(new Vector3(100, 0, 0), Color.White));
			pointList.Add(new VertexPositionColor(new Vector3(100, 0 , 100), Color.White));
			pointOrder.Add(0);
			pointOrder.Add(1);
			pointOrder.Add(0);
			pointOrder.Add(2);
			pointOrder.Add(3);
			pointOrder.Add(2);
			pointOrder.Add(3);
			pointOrder.Add(1);
		}

		protected override void LoadContent() {
			effect = new BasicEffect(Game.GraphicsDevice);
			effect.VertexColorEnabled = true;
			effect.LightingEnabled = false;
		}

		public override void Update(GameTime gameTime) {
			effect.View = ((Game)Game).Camera.View;
			effect.Projection = ((Game)Game).Camera.Projection;
			effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
			foreach(EffectPass pass in effect.CurrentTechnique.Passes) {
				pass.Apply();
				GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
					 PrimitiveType.LineList,
					 pointList.ToArray(),
					 0,  // vertex buffer offset to add to each element of the index buffer
					 pointList.Count,  // number of vertices in pointList
					 pointOrder.ToArray(),  // the index buffer
					 0,  // first index element to read
					 pointOrder.Count / 2  // number of primitives to draw
				);
			}
		}
	}
}
