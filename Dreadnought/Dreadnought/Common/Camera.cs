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
using Dreadnought.Base;


namespace Dreadnought.Common {
	public class Camera : Entity {

		public Quaternion Orientation;
		public Matrix World;
		public Matrix View;
		public Matrix Projection;

		public Vector3 Up = Vector3.Up;
		private Vector3 offset;
		private Point oldMousePos;
		private UniversalCoordinate _lookAt;
		public UniversalCoordinate LookAt {
			get { return _lookAt; }
			set {
				_lookAt = value;
				updateMatrices();
			}
		}

		private float _zoom;
		private int oldScrollWheelValue;
		public float Zoom {
			get { return _zoom; }
			set {
				_zoom = 1000 + value;
				updateMatrices();
			}
		}

		public Camera()
			: base() {
				_lookAt = new UniversalCoordinate();
			resetLook();
			Game.RegisterMouseAction(this);
		}

		public override void MouseAction(GameTime gametime, MouseState ms) {
			if(ms.RightButton == ButtonState.Pressed && GraphicsDevice.Viewport.Bounds.Contains(ms.X, ms.Y)) {

				int x = ms.X - GraphicsDevice.Viewport.Bounds.Center.X;
				int y = ms.Y - GraphicsDevice.Viewport.Bounds.Center.Y;
				if(Game.IsMouseVisible) {
					Game.IsMouseVisible = false;
					oldMousePos.X = ms.X;
					oldMousePos.Y = ms.Y;
					x = 0;
					y = 0;
				}
				Mouse.SetPosition(GraphicsDevice.Viewport.Bounds.Center.X, GraphicsDevice.Viewport.Bounds.Center.Y);
				if(x > 0) {
					TurnRight(-x);
				} else if(x < 0) {
					TurnLeft(x);
				}
				if(y < 0) {
					TurnUp(y);
				} else if(y > 0) {
					TurnDown(-y);
				}

			}
			if(ms.ScrollWheelValue != oldScrollWheelValue) {
				// TODO: Fix Scrollwheel
				Zoom = ms.ScrollWheelValue - oldScrollWheelValue;
			}

			if(ms.RightButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(ms.X, ms.Y)) {
				if(!Game.IsMouseVisible) {
					Mouse.SetPosition(oldMousePos.X, oldMousePos.Y);
					Game.IsMouseVisible = true;
				}
			}
		}

		internal void TurnRight(int x) {
			addRotation(Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(x / 4f)));
		}

		internal void TurnLeft(int x) {
			addRotation(Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.ToRadians(x / 4f)));
		}

		internal void TurnUp(int y) {
			if(Vector3.Dot(Vector3.Up, Vector3.Normalize(offset)) < 0.980f) {    // fix northpole dilemma
				addRotation(Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(Vector3.Up, offset)), MathHelper.ToRadians(y / 2f)));
			}
		}

		internal void TurnDown(int y) {
			if(Vector3.Dot(Vector3.Up, Vector3.Normalize(offset)) > -0.980f) {   // fix southpole dilemma
				addRotation(Quaternion.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(offset, Vector3.Up)), MathHelper.ToRadians(y / 2f)));
			}
		}

		internal void resetLook() {
			Orientation = Quaternion.CreateFromAxisAngle(Vector3.Left, MathHelper.ToRadians(45));
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
			offset = Vector3.Transform(Vector3.Backward * _zoom, Orientation);
			View = Matrix.CreateLookAt(offset + _lookAt.Local, _lookAt.Local, Up);
			World = Matrix.CreateWorld(LookAt.Local, Vector3.Forward, Vector3.Up);
		}

	}
}
