using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dreadnought.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Dreadnought.Helper {
	class Pilot :Entity {
		private Ship ship;
		private Vector3 faceDir;
		public Pilot(Ship ship):base() {
			this.ship = ship;
			//Game.RegisterUpdate(this);
			Game.RegisterMouseAction(this);
		}
		public override void Update(GameTime gameTime) {
			if(faceDir != null) {
				if(ship.turnToFace((Vector3)faceDir) == 3) {
					//faceDir = null;
				}
			}
		}

		public override void MouseAction(GameTime gametime, Microsoft.Xna.Framework.Input.MouseState ms) {
			if(ms.LeftButton == ButtonState.Pressed && GraphicsDevice.Viewport.Bounds.Contains(ms.X, ms.Y)) {
				Vector3 pos1 = GraphicsDevice.Viewport.Unproject(new Vector3(ms.X, ms.Y, 0), Game.Camera.Projection, Game.Camera.View, Game.Camera.World);
				Vector3 pos2 = GraphicsDevice.Viewport.Unproject(new Vector3(ms.X, ms.Y, 1), Game.Camera.Projection, Game.Camera.View, Game.Camera.World);
				faceDir = Vector3.Normalize(pos2 - ship.Position.Local);
				Vector3 right = Vector3.Cross(faceDir,Vector3.Transform(Vector3.Up,ship.Orientation));
				//Vector3 right = Vector3.Cross(faceDir, Vector3.Up);
				Vector3 up = Vector3.Cross( right,faceDir);
				Matrix test = Matrix.CreateWorld(Vector3.Zero, faceDir, up);
				ship.WantedOrientation = Quaternion.CreateFromRotationMatrix(test);
			}
		}
	}
}
