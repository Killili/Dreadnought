using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreadnoughtOvermind.Common;
using Microsoft.Xna.Framework;

namespace DreadnoughtOvermind.Simulation {
	class Ship:SimulationEntity {
		Vector3 velocity = Vector3.Zero;
		public Vector3 WantedVelocity = Vector3.Zero;
		public Quaternion WantedOrientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, 0);
		public UniversalCoordinate Position = new UniversalCoordinate();
		public Quaternion Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward,0);

		public override bool Simulate() {
			Timer.Stop();

			#region Position Change
			Position.Local += velocity * Timer.ElapsedMilliseconds;
			#endregion

			#region Oriantation Change
			Quaternion tempDir;
			float dist;
			Quaternion delta = Quaternion.Conjugate(Orientation) * WantedOrientation;
			if(Quaternion.Dot(Orientation, WantedOrientation) < 0f) {
				tempDir = Quaternion.Negate(WantedOrientation);
				dist = 2f * (float)Math.Acos(MathHelper.Clamp(delta.W * -1, -1, 1));
			} else {
				tempDir = WantedOrientation;
				dist = 2f * (float)Math.Acos(MathHelper.Clamp(delta.W, -1, 1));
			}
			float steps = dist / (float)(Math.PI / 200); // TODO Zeitbasiert!?
			if(steps <= 1f) {
				Orientation = WantedOrientation;
			} else {
				Orientation = Quaternion.Slerp(Orientation, WantedOrientation, 1f / steps);
			}
			Orientation.Normalize();
			#endregion

			return base.Simulate();
		}
	}
}

