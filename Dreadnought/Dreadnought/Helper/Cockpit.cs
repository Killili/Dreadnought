using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Dreadnought;

namespace Dreadnought.Helper {
	class Cockpit : Base.GameEntity {
		private Ship ship;
		public Cockpit(Ship ship)
			: base() {
				this.ship = ship;
				Game.RegisterKeyboardAction(this);
		}
		public override void KeyboardAction(GameTime gametime, KeyboardState ks) {
			if(ks.IsKeyDown(Keys.W)) {
				ship.accelerate();
			} else if(ks.IsKeyDown(Keys.S)) {
				ship.decelerate();
			}
			if(ks.IsKeyDown(Keys.Left)) {
				ship.turnLeft();
			} else if(ks.IsKeyDown(Keys.Right)) {
				ship.turnRight();
			}

			if(ks.IsKeyDown(Keys.X)) {
				ship.counterRotation();
			}
			if(ks.IsKeyDown(Keys.C)) {
				ship.counterMoment();
			}

			if(ks.IsKeyDown(Keys.Down)) {
				ship.turnUp();
			} else if(ks.IsKeyDown(Keys.Up)) {
				ship.turnDown();
			}

			if(ks.IsKeyDown(Keys.R)) {
				ship.rise();
			} else if(ks.IsKeyDown(Keys.F)) {
				ship.sink();
			}

			if(ks.IsKeyDown(Keys.Q)) {
				ship.rollLeft();
			} else if(ks.IsKeyDown(Keys.E)) {
				ship.rollRight();
			}

			if(ks.IsKeyDown(Keys.A)) {
				ship.strafeLeft();
			} else if(ks.IsKeyDown(Keys.D)) {
				ship.strafeRight();
			}

			if(ks.IsKeyDown(Keys.L)) {
				ship.turnToFace(Vector3.Right);
			} else if(ks.IsKeyDown(Keys.J)) {
				ship.turnToFace(Vector3.Left);
			}
		}
	}
}
