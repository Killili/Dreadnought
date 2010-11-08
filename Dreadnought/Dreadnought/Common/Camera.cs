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
	public class Camera : GameComponent {

		public Vector3 Position = Vector3.Forward;
		public Quaternion Orientation = Quaternion.CreateFromAxisAngle(Vector3.Left, 40f);
		public Matrix World;
		public Matrix View;
		public Matrix Projection;

		public Vector3 Up = Vector3.Up;
		
		private Vector3 _lookAt;
		public Vector3 LookAt {
			get { return _lookAt; }
			set {
				_lookAt = value;
				updateMatrices();
			}
		}

		private float _zoom;
		public float Zoom {
			get { return _zoom; }
			set {
				_zoom = 1000 + value;
				updateMatrices();
			}
		}

		public Camera(Game game)
			: base(game) {
			resetLook();
		}

		internal void TurnRight(int x) {
			addRotation(Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(x / 4f)));
		}

		internal void TurnLeft(int x) {
			addRotation(Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.ToRadians(x / 4f)));
		}

		internal void TurnUp(int y) {
			if(Vector3.Dot(Vector3.Up, Vector3.Normalize(Position)) < 0.980f) {    // fix northpole dilemma
				addRotation(Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(Vector3.Up, Position)), MathHelper.ToRadians(y / 2f)));
			}
		}

		internal void TurnDown(int y) {
			if(Vector3.Dot(Vector3.Up, Vector3.Normalize(Position)) > -0.980f) {   // fix southpole dilemma
				addRotation(Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(Position, Vector3.Up)), MathHelper.ToRadians(y / 2f)));
			}
		}

		internal void resetLook() {
			Orientation = Quaternion.CreateFromAxisAngle(Vector3.Right, 45f);
			_zoom = 1000f;
			updateMatrices();
		}

		private void addRotation(Quaternion rot) {
			Quaternion.Concatenate(ref Orientation, ref rot, out Orientation);
			Orientation.Normalize();
			updateMatrices();
		}

		private void updateMatrices() {
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ((Game)Game).GraphicsDevice.Viewport.AspectRatio, 1, 200000);
			Position = Vector3.Transform(Vector3.Backward * _zoom, Orientation);
			View = Matrix.CreateLookAt(Position + _lookAt, _lookAt, Up);
			World = Matrix.CreateWorld(LookAt, Vector3.Forward, Vector3.Up);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
		}
	}
}
