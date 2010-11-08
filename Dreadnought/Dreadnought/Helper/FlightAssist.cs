using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Dreadnought;
using Dreadnought.Base;

namespace Dreadnought.Helper {
	class FlightAssist:Entity {
		private TimeSpan lastUpdate;
		private TimeSpan timeout;
		private Ship ship;
		private bool ignoreEvents;
		private double desiredSpeed;

		public FlightAssist(Ship ship):base() {
			this.ship = ship;
			ship.Thrust += thrustEvent;
			Game.RegisterUpdate(this);
		}

		public override void Update(GameTime gameTime) {
			lastUpdate = gameTime.TotalGameTime;
			if(timeout < lastUpdate) {
				if(ship.IsRotating()) {
					ignoreEvents = true;
					ship.counterRotation();
					ignoreEvents = false;
				} else if(ship.IsMoving()) {
					ignoreEvents = true;
					ship.counterMoment();
					ignoreEvents = false;
				}
			}
		}
		
		public void thrustEvent(object sender,EventArgs args) {
			if(!ignoreEvents) {
				//if(dir != Thruster.ThrustDirection.Forward) {
					timeout = lastUpdate + TimeSpan.FromSeconds(0.1);
				//}
			}
		}
	}
}
