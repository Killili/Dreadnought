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
using Dreadnought.Common;
using System.Windows.Forms.Integration;
using DreadnoughtUI;

namespace Dreadnought {
	public class Game : Microsoft.Xna.Framework.Game {
		GraphicsDeviceManager graphics;
		internal Ship Ship;
		private Grid grid;
		public Sidemenu UI;
		private Nullable<Vector3> faceDir;

		public Camera Camera { get; private set; }
		public Matrix World { get; private set; }
		public Vector3 SunDirection = Vector3.Right;

		#region Config and Windows
		private void config() {
#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += (s, e) => {
				Exception ex = (Exception)e.ExceptionObject;
				System.Windows.MessageBox.Show(ex.Message);
			};
#endif
			graphics = new GraphicsDeviceManager(this);
			
			graphics.PreferredBackBufferHeight = 768;
			graphics.PreferredBackBufferWidth = 200 + 1024;
			graphics.ApplyChanges();
			IsMouseVisible = true;
			Content.RootDirectory = "Content";
			
			ElementHost host = new ElementHost();
			UI = new Sidemenu();
			host.Child = UI;

			host.Location = new System.Drawing.Point(0, 0);
			host.Size = new System.Drawing.Size(200, Window.ClientBounds.Height);
			host.BackColorTransparent = true;
			System.Windows.Forms.Control.FromHandle(Window.Handle).Controls.Add(host);
		}
		private void init() {
			
		}
		#endregion

		public Game() {
			config();
		}

		protected override void Initialize() {
			init();
			// init fps counter
			var fpsCntr = new FPSCounter(this);
			fpsCntr.Updated += delegate { this.Window.Title = "Dreadnought  (FPS: " + fpsCntr.FPS.ToString() + " )"; };
			Components.Add(fpsCntr);
			// add ship
			Ship = new Ship(this);
			Components.Add(Ship);
			// add Copilot
			Components.Add(new BasicFlightHelper(this, Ship));
			// add grid
			grid = new Grid(this);
			Components.Add(grid);

			base.Initialize();
		}

		protected override void LoadContent() {
			World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Camera = new Common.Camera(this);
			Camera.Load(Content);
			Camera.Position = new Vector3(1, 1000, 1);

		}

		protected override void UnloadContent() {
		}

		protected override void Update(GameTime gameTime) {
			KeyboardState ks = Keyboard.GetState(PlayerIndex.One);
			MouseState ms = Mouse.GetState();

			if(ks.IsKeyDown(Keys.Escape)) {
				this.Exit();
			}

			if(ks.IsKeyDown(Keys.W)) {
				Ship.accelerate();
			} else if(ks.IsKeyDown(Keys.S)) {
				Ship.decelerate();
			}
			if(ks.IsKeyDown(Keys.Left)) {
				Ship.turnLeft();
			} else if(ks.IsKeyDown(Keys.Right)) {
				Ship.turnRight();
			}

			if(ks.IsKeyDown(Keys.X)) {
				Ship.counterRotation();
			}
			if(ks.IsKeyDown(Keys.C)) {
				Ship.counterMoment();
			}

			if(ks.IsKeyDown(Keys.Down)) {
				Ship.turnUp();
			} else if(ks.IsKeyDown(Keys.Up)) {
				Ship.turnDown();
			}

			if(ks.IsKeyDown(Keys.R)) {
				Ship.rise();
			} else if(ks.IsKeyDown(Keys.F)) {
				Ship.sink();
			}

			if(ks.IsKeyDown(Keys.Q)) {
				Ship.rollLeft();
			} else if(ks.IsKeyDown(Keys.E)) {
				Ship.rollRight();
			}

			if(ks.IsKeyDown(Keys.A)) {
				Ship.strafeLeft();
			} else if(ks.IsKeyDown(Keys.D)) {
				Ship.strafeRight();
			}

			if(ks.IsKeyDown(Keys.L)) {
				Ship.turnToFace(Vector3.Right);
			} else if(ks.IsKeyDown(Keys.J)) {
				Ship.turnToFace(Vector3.Left);
			}

			if(faceDir != null) {
				if(Ship.turnToFace(faceDir.Value) == 3) {
					faceDir = null;
				}
			}

			if(ms.LeftButton == ButtonState.Pressed && GraphicsDevice.Viewport.Bounds.Contains(ms.X,ms.Y)) {
				Vector3 pos1 = GraphicsDevice.Viewport.Unproject(new Vector3(ms.X, ms.Y, 0), Camera.Projection, Camera.View, Camera.World);
				Vector3 pos2 = GraphicsDevice.Viewport.Unproject(new Vector3(ms.X, ms.Y, 1), Camera.Projection, Camera.View, Camera.World);
				faceDir = Vector3.Normalize(pos2 - Ship.Position);
			}

			Camera.Up = Vector3.Up;
			//Camera.Position = ship.Position + (Vector3.Transform(Vector3.Backward, ship.Orientation) * 3000) + Vector3.Transform(Vector3.Up, ship.Orientation) * 500;
			Vector3 gp = new Vector3((float)Math.Round(Ship.Position.X / grid.Scale), (float)Math.Round(Ship.Position.Y / grid.Scale), (float)Math.Round(Ship.Position.Z / grid.Scale));
			grid.Position = new Vector3(-(grid.Size / 2f), -(grid.Size / 2f), -(grid.Size / 2f)) + gp;
			Camera.LookAt = Ship.Position * 500;

			Camera.Position = new Vector3(1,1,1) * 1000;
			//World *= Matrix.CreateRotationY(MathHelper.ToRadians(1f));
			Camera.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			
			
			Ship.DrawShadow();
			Viewport v = GraphicsDevice.Viewport;
			v.Width = Window.ClientBounds.Width - 200;
			v.X = 200;
			GraphicsDevice.Viewport = v;
			GraphicsDevice.Clear(Color.CornflowerBlue);
			Camera.Draw();
			base.Draw(gameTime);
			
		}
	}
}
