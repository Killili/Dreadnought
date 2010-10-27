using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
	class Thruster {
		public enum ThrustDirection{ Forward,Backward,FrontLeft,FrontRight,BackLeft,BackRight,FrontUp,FrontDown,BackUp,BackDown }
		private Dictionary<ThrustDirection, TimeSpan> fireDuration;
		private TimeSpan lastUpdate;

		public Thruster(Ship ship) {
			fireDuration = new Dictionary<ThrustDirection,TimeSpan>();
			ship.Thrust += FireThruster;
			Game.GameTimeUpdate += Update;
		}
		private void FireThruster(ThrustDirection dir){
			fireDuration[dir] = lastUpdate + TimeSpan.FromSeconds(0.01);
		}

		public void Update(GameTime gameTime) {
			lastUpdate = gameTime.TotalGameTime;
		}

		public bool IsActive(ModelMesh mesh) {
			if(mesh.Tag == null) {
				ThrustDirection temp;
				Enum.TryParse<ThrustDirection>(mesh.Name, true, out temp );
				mesh.Tag = temp;
			}
			try {
				if(fireDuration[(ThrustDirection)mesh.Tag] > lastUpdate) {
					return true;
				}
			} catch(KeyNotFoundException) {
				fireDuration[(ThrustDirection)mesh.Tag] = TimeSpan.Zero;
				return false;
			}
			return false;
		}
	}

	class BasicFlightHelper {
		private TimeSpan lastUpdate;
		private TimeSpan timeout;
		private Ship ship;
		private bool ignoreEvents;
		public BasicFlightHelper(Ship s) {
			ship = s;
			Game.GameTimeUpdate += timerUpdate;
			ship.Thrust += thrustEvent;
		}
		public void timerUpdate(GameTime gameTime) {
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
		public void thrustEvent(Thruster.ThrustDirection dir) {
			if(!ignoreEvents) {
				if(dir != Thruster.ThrustDirection.Forward) {
					timeout = lastUpdate + TimeSpan.FromSeconds(0.1);
				}
			}
		}
	}

	class Ship {
		#region Fileds and Constructors
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
		private Thruster thrusters;
		private List<ModelMesh> shipModel;
		private List<ModelMesh> thrusterModel;

		public delegate void ThrusterEvent(Thruster.ThrustDirection dir);
		public event ThrusterEvent Thrust;
		
		public Vector3 Position { get{ return position; }  set{ position = value;} }

		public Ship(Game game) {
			this.game = game;
		}
		#endregion
		public void Load(ContentManager content) {
			model = content.Load<Model>("Ship");
			thrusters = new Thruster(this);
			//texture = content.Load<Texture2D>("ShipTexture");
			transforms = new Matrix[model.Bones.Count];
			moment = new Vector3(0.0f);
			position = new Vector3(0.0f);
			speedLimit = 0.02f;
			turnLimit = 0.0002f;
			helpSpeed = 0.01f;
			rotation = new Vector3(0.0f);
			orientation = new Quaternion(Vector3.Up,0f);

			shipModel = walkModelTree(model.Bones["Ship"]);
			thrusterModel = walkModelTree(model.Bones["Thruster"]);
			BasicFlightHelper copilot = new BasicFlightHelper(this);
		}

		public void Update(GameTime gameTime) {
			position += moment;
			orientation = Quaternion.Concatenate( orientation , Quaternion.CreateFromAxisAngle(model.Root.Transform.Up, rotation.Y));
			orientation = Quaternion.Concatenate( orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Right, rotation.X));
			orientation = Quaternion.Concatenate(orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Forward, rotation.Z));
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
			}
		}

		

		private List<ModelMesh> walkModelTree(ModelBone modelBone) {
			List<ModelMesh> list = new List<ModelMesh>();
			return walkModelTree(modelBone, list);
		}

		private List<ModelMesh> walkModelTree(ModelBone start, List<ModelMesh> list ) {
			foreach(ModelBone bone in start.Children) {
				if(bone.Children.Count > 0) walkModelTree(bone,list);
				foreach(ModelMesh mesh in model.Meshes) {
					if(mesh.ParentBone == bone) {
						list.Add(mesh);
					}
				}
			}
			return list;
		}


		#region Draw
		public void Draw() {
			foreach(ModelMesh mesh in shipModel) {
				drawMesh(mesh);
			}
			foreach(ModelMesh mesh in thrusterModel) {
				if(thrusters.IsActive(mesh)) {
					drawThrust(mesh);
				}
			}

		}

		private void drawMesh(ModelMesh mesh) {
			foreach(BasicEffect effect in mesh.Effects) {
				effect.World = transforms[mesh.ParentBone.Index];
				effect.View = game.Camera.View;
				effect.Projection = game.Camera.Projection;
				effect.DirectionalLight0.Direction = Vector3.Down;
				effect.LightingEnabled = true;
				effect.PreferPerPixelLighting = true;
				effect.EmissiveColor = Vector3.Zero;
				effect.DiffuseColor = new Vector3(0.75f);
			}
			mesh.Draw();
		}

		private void drawThrust(ModelMesh mesh) {
			foreach(BasicEffect effect in mesh.Effects) {
				effect.World = transforms[mesh.ParentBone.Index];
				effect.View = game.Camera.View;
				effect.Projection = game.Camera.Projection;
				effect.LightingEnabled = false;
				effect.EmissiveColor = new Vector3(1f, 0.5f, 0f);
			}
			mesh.Draw();
		}
		
		#endregion

		public void counterRotation() {
			if(Math.Abs(rotation.Y) <= turnLimit) {
				rotation.Y = 0;
			} else if(rotation.Y > 0) {
				turnRight();
			} else if(rotation.Y < 0) {
				turnLeft();
			}
			if(Math.Abs(rotation.X) <= turnLimit) {
				rotation.X = 0;
			} else if(rotation.X > 0) {
				turnDown();
			} else if(rotation.X < 0) {
				turnUp();
			}
			if(Math.Abs(rotation.Z) <= turnLimit) {
				rotation.Z = 0;
			} else if(rotation.Z > 0) {
				rollRight();
			} else if(rotation.Z < 0) {
				rollLeft();
			}
		}

		public void counterMoment() {
			Vector3 temp = Vector3.Transform(moment, Quaternion.Inverse(orientation));
			if(temp.Length() > speedLimit) {
				if(Math.Abs(temp.X) < speedLimit) temp.X = 0;
				if(temp.X < 0) strafeRight();
				if(temp.X > 0) strafeLeft();
				if(Math.Abs(temp.Z) < speedLimit) temp.Z = 0;
				if(temp.Z > 0) accelerate();
				//if(temp.Z < 0) decelerate();
				if(Math.Abs(temp.Y) < speedLimit) temp.Y = 0;
				if(temp.Y > 0) sink();
				if(temp.Y < 0) rise();
			}
		}

		#region Movment
		public void accelerate() {
			moment += Vector3.Transform(Vector3.Forward,orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.Forward);
		}

		public void decelerate() {
			moment += Vector3.Transform(Vector3.Backward, orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.Backward);
		}

		public void turnLeft() {
			rotation.Y += turnLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontRight);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackLeft);
		}

		public void turnRight() {
			rotation.Y -= turnLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontLeft);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackRight);
		}

		public void turnUp() {
			rotation.X += turnLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontDown);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackUp);
		}

		public void turnDown() {
			rotation.X -= turnLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontUp);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackDown);
		}

		public void rise() {
			moment += Vector3.Transform(Vector3.Up, orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontDown);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackDown);
		}

		public void sink() {
			moment += Vector3.Transform(Vector3.Down, orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontUp);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackUp);
		}

		internal void rollLeft() {
			rotation.Z += turnLimit;
		}

		internal void rollRight() {
			rotation.Z -= turnLimit;
		}

		internal void strafeLeft() {
			moment += Vector3.Transform(Vector3.Left, orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontRight);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackRight);
		}

		internal void strafeRight() {
			moment += Vector3.Transform(Vector3.Right, orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontLeft);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackLeft);
		}
		#endregion

		internal bool IsRotating() {
			if(rotation.Length() > 0) return true;
			return false;
		}

		internal bool IsMoving() {
			if(moment.Length() > 0) return true;
			return false;
		}
	}
}
