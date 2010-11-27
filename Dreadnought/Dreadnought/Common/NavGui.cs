using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dreadnought.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dreadnought.Common {
	class NavGui : Entity {
		private List<VertexPositionColor> vertexList;
		private VertexBuffer vertexBuffer;
		private BasicEffect effect;
		private List<Tuple<PrimitiveType, int, int>> parts;
		private enum states { NotVisible, ChooseXZ , PickedXZ , ChooseY , PickedY };
		private states state = states.NotVisible;
		private Vector3 selectedXZ = Vector3.Zero;
		private Vector3 selectedPoint = Vector3.Zero;
		public Action<object, Vector3> OnPointSet;

		private VertexPositionColor[] navTriangle = new VertexPositionColor[3];
		private short[] navTrianglePoints = {0,1,2,0};

		public NavGui() {
			Game.RegisterDraw(this);
			Game.RegisterUpdate(this);
			Game.RegisterMouseAction(this);
			Game.RegisterKeyboardAction(this);
			Load();
		}
		public void Load() {
			parts = new List<Tuple<PrimitiveType, int, int>>();
			vertexList = new List<VertexPositionColor>();
			effect = new BasicEffect(GraphicsDevice);

			effect.VertexColorEnabled = true;

			addCircle(Vector3.Forward * 10, Vector3.Up, 36, new Color(0, 0, 0.7f));
			for(int i = 0 ; i < 10 ; i++) {
				addCircle(Vector3.Forward * 250 * i, Vector3.Up, 36, new Color(0, 0, 0.7f));
			}

			navTriangle[0] = new VertexPositionColor(Vector3.Zero, Color.GreenYellow);
			navTriangle[1] = new VertexPositionColor(Vector3.Zero, Color.GreenYellow);
			navTriangle[2] = new VertexPositionColor(Vector3.Zero, Color.GreenYellow);

			vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertexList.Count, BufferUsage.WriteOnly);
			vertexBuffer.SetData<VertexPositionColor>(vertexList.ToArray());

		}
		public override void Update(GameTime gameTime) {
			effect.World = Game.Camera.World;
			effect.Projection = Game.Camera.Projection;
			effect.View = Game.Camera.View;
		}
		public override void KeyboardAction(GameTime gametime, KeyboardState ks) {
			if(ks.IsKeyDown(Keys.M) && state == states.NotVisible) {
				state = states.ChooseXZ;
				navTriangle[0] = new VertexPositionColor(Vector3.Zero, Color.GreenYellow);
				navTriangle[1] = new VertexPositionColor(Vector3.Zero, Color.GreenYellow);
				navTriangle[2] = new VertexPositionColor(Vector3.Zero, Color.GreenYellow);
			}
		}

		public override void MouseAction(GameTime gametime, MouseState ms) {
			if(ms.LeftButton == ButtonState.Pressed && state == states.ChooseXZ) {
				state = states.PickedXZ;
			}
			if(ms.LeftButton == ButtonState.Pressed && state == states.ChooseY) {
				state = states.PickedY;
			}
			if(ms.LeftButton == ButtonState.Released && state == states.PickedXZ) {
				state = states.ChooseY;
			}
			if(ms.LeftButton == ButtonState.Released && state == states.PickedY) {
				Console.WriteLine(selectedPoint);
				state = states.NotVisible;
				if(OnPointSet != null) OnPointSet(this, selectedPoint);
			}

			if(state != states.NotVisible && Game.IsMouseVisible == true && GraphicsDevice.Viewport.Bounds.Contains(ms.X, ms.Y)) {
				Vector3 pos1 = GraphicsDevice.Viewport.Unproject(new Vector3(ms.X, ms.Y, 0), Game.Camera.Projection, Game.Camera.View, Game.Camera.World);
				Vector3 pos2 = GraphicsDevice.Viewport.Unproject(new Vector3(ms.X, ms.Y, 1), Game.Camera.Projection, Game.Camera.View, Game.Camera.World);
				Vector3 dir = Vector3.Normalize(pos2 - pos1);
				Ray ray = new Ray(pos1, dir);
				float? dist;
				switch(state) {
					case states.ChooseXZ:
						dist = ray.Intersects(new Plane(Vector3.Up, 0));
						if(dist != null) {
							selectedXZ = ray.Position + (dir * (float)dist);
							selectedPoint = selectedXZ;
							navTriangle[1].Position = selectedXZ;
							Console.WriteLine(selectedXZ);
						}
						break;
					case states.ChooseY:
						Vector3 planeNorm = Vector3.Normalize(selectedXZ);
						planeNorm = Vector3.Negate(planeNorm);
						dist = ray.Intersects(new Plane(Vector3.Normalize(planeNorm), selectedXZ.Length()));
						if(dist != null) {
							Vector3 temp = ray.Position + (dir * (float)dist);
							selectedPoint.Y = temp.Y;
							navTriangle[2].Position = selectedPoint;
							Console.WriteLine(selectedPoint);
						}
						break;
				}

			}
		}

		private void addCircle(Vector3 radius, Vector3 plane, int segments, Color color) {
			int start = vertexList.Count;
			Quaternion rotation = Quaternion.CreateFromAxisAngle(plane, (float)(Math.PI * 2) / (float)(segments - 1));
			for(int i = 0 ; i < segments ; i++) {
				Vector3.Transform(ref radius, ref rotation, out radius);
				vertexList.Add(new VertexPositionColor(
					radius,
					color));

			}
			parts.Add(new Tuple<PrimitiveType, int, int>(PrimitiveType.LineStrip, start, vertexList.Count - start - 1));
		}

		public override void Draw(GameTime gameTime) {
			switch(state) {
				case states.ChooseXZ:
				case states.ChooseY:
					GraphicsDevice.SetVertexBuffer(vertexBuffer);
					foreach(EffectPass pass in effect.CurrentTechnique.Passes) {
						pass.Apply();
						foreach(var part in parts) {
							GraphicsDevice.DrawPrimitives(part.Item1, part.Item2, part.Item3);
						}
					}

					GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
						  PrimitiveType.LineStrip,
						  navTriangle,
						  0,
						  3,
						  navTrianglePoints,
						  0,
						  3);
					GraphicsDevice.SetVertexBuffer(vertexBuffer);
					effect.World *= Matrix.CreateTranslation(selectedPoint);
					foreach(EffectPass pass in effect.CurrentTechnique.Passes) {
						pass.Apply();
						var part = parts[0];
						GraphicsDevice.DrawPrimitives(part.Item1, part.Item2, part.Item3);
						
					}
					break;
			}
		}
	}
}
