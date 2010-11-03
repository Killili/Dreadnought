using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
	class Thruster {
		public enum ThrustDirection { Forward, Backward, FrontLeft, FrontRight, BackLeft, BackRight, FrontUp, FrontDown, BackUp, BackDown }
		private Dictionary<ThrustDirection, TimeSpan> fireDuration;
		private TimeSpan lastUpdate;

		public Thruster(Ship ship) {
			fireDuration = new Dictionary<ThrustDirection, TimeSpan>();
			ship.Thrust += FireThruster;
			Game.GameTimeUpdate += Update;
		}
		private void FireThruster(ThrustDirection dir) {
			fireDuration[dir] = lastUpdate + TimeSpan.FromSeconds(0.01);
		}

		public void Update(GameTime gameTime) {
			lastUpdate = gameTime.TotalGameTime;
		}

		public bool IsActive(ModelMesh mesh) {
			if(mesh.Tag == null) {
				ThrustDirection temp;
				Enum.TryParse<ThrustDirection>(mesh.Name, true, out temp);
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

	class BasicFlightHelper : GameComponent {
		private TimeSpan lastUpdate;
		private TimeSpan timeout;
		private Ship ship;
		private bool ignoreEvents;
		private double desiredSpeed;

		public BasicFlightHelper(Game game,Ship ship):base(game) {
			this.ship = ship;
			Game.Components.Add(this);
			ship.Thrust += thrustEvent;
			((Game)Game).UI.Orders += OrderHandler;
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
				if(ship.Speed.Z > desiredSpeed && ship.Speed.Z < 0 ) {
					ignoreEvents = true;
					//ship.accelerate();
					ignoreEvents = false;
				} else if(ship.Speed.Z < desiredSpeed-1 || ship.Speed.Z > 0) {
					ignoreEvents = true;
					//ship.decelerate();
					ignoreEvents = false;
				}
			}
		}
		
		private void OrderHandler(object sender, DreadnoughtUI.OrderEventArgs order) {
			Console.WriteLine("AyAy");
			desiredSpeed = order.Speed * -1;
		}

		public void thrustEvent(Thruster.ThrustDirection dir) {
			if(!ignoreEvents) {
				if(dir != Thruster.ThrustDirection.Forward) {
					timeout = lastUpdate + TimeSpan.FromSeconds(0.1);
				}
			}
		}

	}

	class Ship : Microsoft.Xna.Framework.DrawableGameComponent {
		#region Fileds and Constructors
		Model model;
		Texture2D texture;
		Matrix[] transforms;

		private Vector3 moment;
		private Vector3 position;
		public Vector3 Speed;

		public Quaternion Orientation;
		private Vector3 rotation;

		private float speedLimit;
		private double turnConst = Math.PI / (360*100) ;

		private Thruster thrusters;
		private List<ModelMesh> shipModel;
		private List<ModelMesh> thrusterModel;

		public delegate void ThrusterEvent(Thruster.ThrustDirection dir);
		public event ThrusterEvent Thrust;

		public Vector3 Position { get { return position; } set { position = value; } }

		public Ship(Game game)
			: base(game) {
			Enabled = true;
			
		}

		#endregion

		protected override void LoadContent() {
			model = Game.Content.Load<Model>("Ship");
			thrusters = new Thruster(this);
			//texture = content.Load<Texture2D>("ShipTexture");
			transforms = new Matrix[model.Bones.Count];
			moment = new Vector3(0.0f);
			position = new Vector3(0.0f);
			speedLimit = 0.2f;

			rotation = new Vector3(0.0f);
			Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0);

			shipModel = walkModelTree(model.Bones["Ship"]);
			thrusterModel = walkModelTree(model.Bones["Thruster"]);

			new BasicFlightHelper((Game)Game,this);
		}

		public override void Update(GameTime gameTime) {
			position += moment;
			Orientation = Quaternion.Concatenate(Orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Up, (float)(rotation.Y*turnConst)));
			Orientation = Quaternion.Concatenate(Orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Right, (float)(rotation.X*turnConst)));
			Orientation = Quaternion.Concatenate(Orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Forward, (float)(rotation.Z * turnConst)));
			Orientation.Normalize();
			Speed = Vector3.Transform(moment,Orientation);


			model.Root.Transform = Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(position);
			model.Bones["Radar"].Transform *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(1.5f));

			model.CopyAbsoluteBoneTransformsTo(transforms);

			Matrix slot = transforms[model.Bones["Slot"].Index];


			Vector3 tt = Vector3.Transform(Vector3.Zero, Matrix.Invert(slot));
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

		private Vector3 faceDir(Vector3 to) {

			Vector3 lf = Vector3.Transform(to, Quaternion.Conjugate(Orientation));
			Vector3 lu = Vector3.Up;// Vector3.Transform(Vector3.Up, orientation);
			lf.Normalize();
			var r = Math.Sqrt((lf.X * lf.X) + (lf.Y * lf.Y) + (lf.Z * lf.Z)); //sollte 1 sein
			var e = Math.Acos(lf.Z / r);
			var p = Math.Atan2(lf.Y, lf.X);
			lu.Normalize();
			//lf.Y *= -1;
			//lf.X *= -1;
			//lf.Z *= -1;
			Vector3 lr = Vector3.Cross(lu, lf);
			lr.Normalize();
			Vector3 up = Vector3.Cross(lf, lr);
			up.Normalize();
			//((Game)Game).Camera.AddDebugVector(to * 400);
			//((Game)Game).Camera.AddDebugVector(lf * 800);
			//((Game)Game).Camera.AddDebugVector(Vector3.Transform(lr * 400, Orientation));
			//((Game)Game).Camera.AddDebugVector(Vector3.Transform(up * 400, Orientation));


			//lf.Normalize();
			//lu.Normalize();
			//lf.Z *= -1;

			Matrix m = Matrix.CreateWorld(Vector3.Zero, lf, up);

			//((Game)Game).Camera.AddDebugStar(Matrix.CreateFromQuaternion(Orientation) * m * Matrix.CreateWorld(Position, Vector3.Forward, Vector3.Up));
			((Game)Game).Camera.AddDebugStar(((Game)Game).World * Matrix.CreateTranslation(-400, 5, 0));

			//Vector3 fo = Vector3.Transform(Vector3.Forward, Quaternion.Conjugate(orientation));
			//Vector3 u = Vector3.Transform(Vector3.Up, Quaternion.Conjugate(orientation));
			//Matrix m = Matrix.CreateWorld(Vector3.Zero, fo, u);
			Vector3 v = Vector3.Zero;

			if(m.M21 > 0.998) { // singularity at north pole
				v.X = (Single)Math.Atan2(m.M13, m.M33);
				v.Y = (Single)(Math.PI / 2);
				v.Z = 0;
			} else if(m.M21 < -0.998) { // singularity at south pole
				v.X = (Single)Math.Atan2(m.M13, m.M33);
				v.Y = (Single)(-Math.PI / 2);
				v.Z = 0;
			} else {
				v.X = (Single)Math.Atan2(-m.M31, m.M11) * -1.0f;
				v.Y = (Single)Math.Atan2(-m.M23, m.M22) * -1.0f;
				v.Z = (Single)Math.Asin(m.M21) * -1.0f;
			}
			var epsilon = 0.0005;
			if(Math.Abs(v.X) < epsilon)
				v.X = 0;
			if(Math.Abs(v.Y) < epsilon)
				v.Y = 0;
			if(Math.Abs(v.Z) < epsilon)
				v.Z = 0;
			Console.WriteLine(v);
			//Console.WriteLine(r);
			//Console.WriteLine(e);
			//Console.WriteLine(p);
			return v;
		}


		#region Draw
		private List<ModelMesh> walkModelTree(ModelBone modelBone) {
			List<ModelMesh> list = new List<ModelMesh>();
			return walkModelTree(modelBone, list);
		}

		private List<ModelMesh> walkModelTree(ModelBone start, List<ModelMesh> list) {
			foreach(ModelBone bone in start.Children) {
				if(bone.Children.Count > 0) walkModelTree(bone, list);
				foreach(ModelMesh mesh in model.Meshes) {
					if(mesh.ParentBone == bone) {
						list.Add(mesh);
					}
				}
			}
			return list;
		}



		public override void Draw(GameTime gameTime) {
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
				effect.View = ((Dreadnought.Game)Game).Camera.View;
				effect.Projection = ((Dreadnought.Game)Game).Camera.Projection;
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
				effect.View = ((Dreadnought.Game)Game).Camera.View;
				effect.Projection = ((Dreadnought.Game)Game).Camera.Projection;
				effect.LightingEnabled = false;
				effect.EmissiveColor = new Vector3(1f, 0.5f, 0f);
			}
			mesh.Draw();
		}

		#endregion
		Dictionary<String, List<KeyValuePair<int, float>>> fancyDebug = new Dictionary<String, List<KeyValuePair<int, float>>>();
		private void addDebugPoint(string line, float value) {
			if(!fancyDebug.Keys.Contains(line)) {
				fancyDebug[line] = new List<KeyValuePair<int, float>>();
			}
			fancyDebug[line].Add(new KeyValuePair<int, float>(fancyDebug[line].Count + 1, value));
		}
		public void PushDebugPoints() {
			if(((Game)Game).UI.Graph != null) {
				foreach(var line in fancyDebug.ToList()) {
					((Game)Game).UI.Graph.AddLine(line.Key, line.Value.ToArray());
				}
			}
			fancyDebug.Clear();
		}

		private int requiredTurnAction(float diff, int motionSteps) {
			int diffSteps = (int)(Math.Round(diff / turnConst));
			if(Math.Abs(diff) == 0) {
				if(Math.Abs(motionSteps) == 0) return 0;
				if(motionSteps > 0) return -1;
				if(motionSteps < 0) return 1;
			}
			var gd = MathHelper.Clamp(MathHelper.ToDegrees(diff), -180, 180);
			int ret = 0;
	
			int requiredSteps = 0;
			double kenn = 0;

			if(diff > 0) ret = 1;
			if(diff < 0) ret = -1;

			if(Math.Abs(motionSteps) > 0) {
				requiredSteps = motionSteps != 0 ? diffSteps / motionSteps : diffSteps;
				kenn = ((double)Math.Abs(diffSteps) / (0.5*((double)Math.Abs(motionSteps)*(double)Math.Abs(motionSteps))));
				if(kenn <= 1 && kenn > 0 && Math.Sign(motionSteps) == Math.Sign(diff)) {
					ret *= -1;
				}
			}


			
			//addDebugPoint("gradDiff", gd);
			//addDebugPoint("diffSteps", MathHelper.Clamp(diffSteps, -cl, cl));
			//addDebugPoint("motionSteps", motionSteps);
			//addDebugPoint("kennzahl", MathHelper.Clamp((float)kenn, -2, 2) * 10);
			//addDebugPoint("requiredSteps", MathHelper.Clamp(requiredSteps, -cl, cl));
			return ret;
		}
		public int turnToFace(Vector3 dir) {
			Vector3 ypr = faceDir(dir);
			int ret = 0;
			//Console.WriteLine(rotation);
			//Console.WriteLine(ypr);
			switch(requiredTurnAction(ypr.X, (int)rotation.Y)) {
				case 1:
					turnLeft();
					break;
				case -1:
					turnRight();
					break;
				case 0:
					ret += 1;
					break;
			}
			switch(requiredTurnAction(ypr.Y, (int)rotation.X)) {
				case 1:
					turnUp();
					break;
				case -1:
					turnDown();
					break;
				case 0:
					ret += 1;
					break;
			}
			if(ret >= 1) {
				switch(requiredTurnAction(ypr.Z, (int)rotation.Z)) {
					case 1:
						rollLeft();
						break;
					case -1:
						rollRight();
						break;
					case 0:
						ret += 1;
						break;
				}
			}
			return ret;
		}

		public void counterRotation() {
			counterRotationY();
			counterRotationX();
			counterRotationZ();
		}

		private void counterRotationZ() {
			if(rotation.Z > 0) {
				rollRight();
			} else if(rotation.Z < 0) {
				rollLeft();
			}
		}

		private void counterRotationX() {
			if(rotation.X > 0) {
				turnDown();
			} else if(rotation.X < 0) {
				turnUp();
			}
		}

		private void counterRotationY() {
			if(rotation.Y > 0) {
				turnRight();
			} else if(rotation.Y < 0) {
				turnLeft();
			}
		}

		public void counterMoment() {
			Vector3 temp = Vector3.Transform(moment, Quaternion.Inverse(Orientation));
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
			moment += Vector3.Transform(Vector3.Forward, Orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.Forward);
		}

		public void decelerate() {
			moment += Vector3.Transform(Vector3.Backward, Orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.Backward);
		}

		public void turnLeft() {
			rotation.Y += 1;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontRight);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackLeft);
		}

		public void turnRight() {
			rotation.Y -= 1;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontLeft);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackRight);
		}

		public void turnUp() {
			rotation.X += 1;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontDown);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackUp);
		}

		public void turnDown() {
			rotation.X -= 1;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontUp);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackDown);
		}

		public void rise() {
			moment += Vector3.Transform(Vector3.Up, Orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontDown);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackDown);
		}

		public void sink() {
			moment += Vector3.Transform(Vector3.Down, Orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontUp);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackUp);
		}

		internal void rollLeft() {
			rotation.Z += 1;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackUp);
		}

		internal void rollRight() {
			rotation.Z -= 1;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontDown);
		}

		internal void strafeLeft() {
			moment += Vector3.Transform(Vector3.Left, Orientation) * speedLimit;
			if(Thrust != null) Thrust(Thruster.ThrustDirection.FrontRight);
			if(Thrust != null) Thrust(Thruster.ThrustDirection.BackRight);
		}

		internal void strafeRight() {
			moment += Vector3.Transform(Vector3.Right, Orientation) * speedLimit;
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
