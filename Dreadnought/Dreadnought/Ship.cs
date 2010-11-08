using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DreadnoughtUI;
using Dreadnought.Common;
using Dreadnought.Base;

namespace Dreadnought {
	class Ship : Entity {
		#region Fileds and Constructors
		
		//internal stuff
		private long gameTime;
		private Vector3 moment = new Vector3(0.0f);
		private Vector3 rotation = new Vector3(0.0f);
		private List<ModelMesh> shipModel;
		private Model model;
		private Texture2D texture;
		private Matrix[] transforms;
		private Dictionary<ThrustDirection, long> thrusterLastActive = new Dictionary<ThrustDirection, long>();
		private Dictionary<ThrustDirection, ModelMesh> thrusterModels = new Dictionary<ThrustDirection, ModelMesh>();
	
		//events
		public event EventHandler Thrust;

		//external stuff
		public enum ThrustDirection { Forward, Backward, FrontLeft, FrontRight, BackLeft, BackRight, FrontUp, FrontDown, BackUp, BackDown, CenterLeftUp, CenterLeftDown, CenterRightUp, CenterRightDown }
		public Vector3 Speed = new Vector3(0.0f);
		public Quaternion Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0);
		public BoundingBox BoundingBox = new BoundingBox(new Vector3(-8, -10, 37), new Vector3(8, 10, -37));

		//constant stuff
		private static double turnConst = Math.PI / (360*100) ;
		private static float speedLimit = 0.2f;
		private static long thrusterBurnTime = TimeSpan.FromSeconds(1.0 / 60.0).Ticks;
		private Shadow shadow;
		
		public Ship(){
			Position = new UniversalPosition();
			Game.RegisterPreDraw(this);
			Game.RegisterUpdate(this);
			Game.RegisterDraw(this);
			LoadContent();
		}
		#endregion

		#region GameComponent
		private void LoadContent() {
			model = Game.Content.Load<Model>("Ship");
			//texture = content.Load<Texture2D>("ShipTexture");
			transforms = new Matrix[model.Bones.Count];
			shipModel = walkModelTree(model.Bones["Ship"]);
			shadow = new Shadow((Game)Game, this);
			shipModel.Each( mesh => mesh.MeshParts.Each( part => part.Effect = shadow.Effect ));
			initThrusters();
		}

		public override void Update(GameTime gt) {
			gameTime = gt.TotalGameTime.Ticks;
			Position.Local += moment;
			Orientation = Quaternion.Concatenate(Orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Up, (float)(rotation.Y * turnConst)));
			Orientation = Quaternion.Concatenate(Orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Right, (float)(rotation.X * turnConst)));
			Orientation = Quaternion.Concatenate(Orientation, Quaternion.CreateFromAxisAngle(model.Root.Transform.Forward, (float)(rotation.Z * turnConst)));
			Orientation.Normalize();
			Speed = Vector3.Transform(moment, Orientation);


			model.Root.Transform =  Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position.Local);
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
		#endregion

		#region Thrusters
		private void initThrusters() {
			foreach(int dir in Enum.GetValues(typeof(ThrustDirection))) {
				thrusterLastActive[(ThrustDirection)dir] = 0;
			}
			model.Bones["Thrusters"].Children.Each(bone => {
				ThrustDirection dir;
				Enum.TryParse<ThrustDirection>(bone.Name, true, out dir);
				thrusterModels.Add(
					dir,
					model.Meshes[bone.Name]
				);
			});
		}

		private void fireThruster(ThrustDirection dir) {
			thrusterLastActive[dir] = gameTime;
			if(Thrust != null) Thrust(this, new EventArgs());
		}

		private bool thrusterActive(ThrustDirection dir) {
			return thrusterLastActive[dir] == gameTime;
		}

		private bool thrusterBurning(ThrustDirection dir) {
			return thrusterLastActive[dir]+thrusterBurnTime >= gameTime;
		}

		private void drawThrusters() {
			thrusterLastActive.Each((dir, time) => {
				if( thrusterBurning(dir) ) {
					if(thrusterModels.ContainsKey(dir)) {
						drawThruster(thrusterModels[dir]);
					}
				}
			});
		}
		#endregion

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


		public override void PreDraw(GameTime gt) {
			shadow.CreateShadowMap();
		}
		public override void Draw(GameTime gt) {
			shadow.DrawWithShadowMap();
			drawThrusters();
		}

		public void DrawGeometry() {
			foreach(ModelMesh mesh in shipModel) {
				shadow.Effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index]);
				mesh.Draw();
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

		private void drawThruster(ModelMesh mesh) {
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

		#region Movement
		#region TurnToFace
		private Vector3 radToFaceDirection(Vector3 to) {
			Vector3 lf = Vector3.Transform(to, Quaternion.Conjugate(Orientation));
			Vector3 lu = Vector3.Up;
			lf.Normalize();
			lu.Normalize();

			Vector3 lr = Vector3.Cross(lu, lf);
			lr.Normalize();
			Vector3 up = Vector3.Cross(lf, lr);
			up.Normalize();

			Matrix m = Matrix.CreateWorld(Vector3.Zero, lf, up);

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
			//Console.WriteLine(v);
			//Console.WriteLine(r);
			//Console.WriteLine(e);
			//Console.WriteLine(p);
			return v;
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
			return ret;
		}

		public int turnToFace(Vector3 dir) {
			Vector3 ypr = radToFaceDirection(dir);
			int ret = 0;
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
						rollRight();
						break;
					case -1:
						rollLeft();
						break;
					case 0:
						ret += 1;
						break;
				}
			}
			return ret;
		}
		#endregion
		public void counterRotation() {
			counterRotationY();
			counterRotationX();
			counterRotationZ();
		}

		private void counterRotationZ() {
			if(rotation.Z > 0) {
				rollLeft();
			} else if(rotation.Z < 0) {
				rollRight();
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

		public void accelerate() {
			if(!thrusterActive(ThrustDirection.Backward)) {
				fireThruster(ThrustDirection.Backward);
				moment += Vector3.Transform(Vector3.Forward, Orientation) * speedLimit;
			}
		}

		public void decelerate() {
			if(!thrusterActive(ThrustDirection.Forward)) {
				fireThruster(ThrustDirection.Forward);
				moment += Vector3.Transform(Vector3.Backward, Orientation) * speedLimit;
			}
		}

		public void turnLeft() {
			if(!thrusterActive(ThrustDirection.FrontRight) && !thrusterActive(ThrustDirection.BackLeft)) {
				fireThruster(ThrustDirection.FrontRight);
				fireThruster(ThrustDirection.BackLeft);
				rotation.Y += 1;
			}
		}
		
		public void turnRight() {
			if(!thrusterActive(ThrustDirection.FrontLeft) && !thrusterActive(ThrustDirection.BackRight)) {
				fireThruster(ThrustDirection.FrontLeft);
				fireThruster(ThrustDirection.BackRight);
				rotation.Y -= 1;
			}
		}
	
		public void turnUp() {
			if(!thrusterActive(ThrustDirection.FrontDown) && !thrusterActive(ThrustDirection.BackUp)) {
				fireThruster(ThrustDirection.FrontDown);
				fireThruster(ThrustDirection.BackUp);
				rotation.X += 1;
			}
		}

		public void turnDown() {
			if(!thrusterActive(ThrustDirection.FrontUp) && !thrusterActive(ThrustDirection.BackDown)) {
				fireThruster(ThrustDirection.FrontUp);
				fireThruster(ThrustDirection.BackDown);
				rotation.X -= 1;
			}
		}

		public void rise() {
			if(!thrusterActive(ThrustDirection.FrontDown) && !thrusterActive(ThrustDirection.BackDown)) {
				fireThruster(ThrustDirection.FrontDown);
				fireThruster(ThrustDirection.BackDown);
				moment += Vector3.Transform(Vector3.Up, Orientation) * speedLimit;
			}
		}

		public void sink() {
			if(!thrusterActive(ThrustDirection.FrontUp) && !thrusterActive(ThrustDirection.BackUp)) {
				fireThruster(ThrustDirection.FrontUp);
				fireThruster(ThrustDirection.BackUp);
				moment += Vector3.Transform(Vector3.Down, Orientation) * speedLimit;
			}
		}

		internal void rollLeft() {
			if(!thrusterActive(ThrustDirection.CenterLeftUp) && !thrusterActive(ThrustDirection.CenterRightDown)) {
				fireThruster(ThrustDirection.CenterLeftUp);
				fireThruster(ThrustDirection.CenterRightDown);
				rotation.Z -= 1;
			}
		}

		internal void rollRight() {
			if(!thrusterActive(ThrustDirection.CenterLeftDown) && !thrusterActive(ThrustDirection.CenterRightUp)) {
				fireThruster(ThrustDirection.CenterLeftDown);
				fireThruster(ThrustDirection.CenterRightUp);
				rotation.Z += 1;
			}
		}

		internal void strafeLeft() {
			if(!thrusterActive(ThrustDirection.FrontRight) && !thrusterActive(ThrustDirection.BackRight)) {
				fireThruster(ThrustDirection.FrontRight);
				fireThruster(ThrustDirection.BackRight);
				moment += Vector3.Transform(Vector3.Left, Orientation) * speedLimit;
			}
		}

		internal void strafeRight() {
			if(!thrusterActive(ThrustDirection.FrontLeft) && !thrusterActive(ThrustDirection.BackLeft)) {
				fireThruster(ThrustDirection.FrontLeft);
				fireThruster(ThrustDirection.BackLeft);
				moment += Vector3.Transform(Vector3.Right, Orientation) * speedLimit;
			}
		}

		internal bool IsRotating() {
			if(rotation.Length() > 0) return true;
			return false;
		}

		internal bool IsMoving() {
			if(moment.Length() > 0) return true;
			return false;
		}
		#endregion
	}
}
