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
using Dreadnought.Base;
using Dreadnought.Helper;

namespace Dreadnought {
	public class Game : Microsoft.Xna.Framework.Game {
		public static Game GameInstance;
		GraphicsDeviceManager graphics;
		internal Ship Ship;
		private Grid grid;
		public Sidemenu UI;
		private Nullable<Vector3> faceDir;
		private Point oldMousePos;

		private List<Entity> drawList = new List<Entity>();
		private List<Entity> preDrawList = new List<Entity>();
		private List<Entity> updateList = new List<Entity>();
		private List<Entity> keyboardActionList = new List<Entity>();
		private List<Entity> mouseActionList = new List<Entity>();

		public Camera Camera { get; private set; }
		public Matrix World { get; private set; }
		public Vector3 SunDirection = Vector3.Right;
		private int oldScrollWheelValue;

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
			GameInstance = this;
			config();
		}

		protected override void Initialize() {
			init();
			// Init Camera
			Camera = new Common.Camera();
			// init fps counter
			var fpsCntr = new FPSCounter(this);
			fpsCntr.Updated += delegate { this.Window.Title = "Dreadnought  (FPS: " + fpsCntr.FPS.ToString() + " )"; };
			Components.Add(fpsCntr);
			
			// add ship
			Ship = new Ship();
			new Cockpit(Ship);
			new Pilot(Ship);
			new FlightAssist(Ship);

			// add grid
			grid = new Grid(this);
			Components.Add(grid);

			base.Initialize();
		}

		protected override void LoadContent() {
			World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
		}

		protected override void UnloadContent() {
		}

		protected override void Update(GameTime gameTime) {
			KeyboardState ks = Keyboard.GetState(PlayerIndex.One);
			MouseState ms = Mouse.GetState();
			mouseActionList.Each( ent => ent.MouseAction(gameTime,ms));
			keyboardActionList.Each(ent => ent.KeyboardAction(gameTime, ks));
			updateList.Each(ent => ent.Update(gameTime));

			if(ks.IsKeyDown(Keys.Escape)) {
				this.Exit();
			}
			
			if(ks.IsKeyDown(Keys.NumPad0)) // this is the panic-camera-button
				Camera.resetLook();


			//Vector3 gp = Ship.Position.CameraSpace(Camera)/(int)grid.Scale;
			UniversalPosition gp = new UniversalPosition();
			gp.Local += new Vector3(-((grid.Scale*grid.Size) / 2));
			grid.Position = gp.CameraSpace(Camera);
			Camera.LookAt = Ship.Position;

			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {


			preDrawList.Each(ent => ent.PreDraw(gameTime));

			#region Setup Backbuffer
			Viewport v = GraphicsDevice.Viewport;
			v.Width = Window.ClientBounds.Width - 200;
			v.X = 200;
			GraphicsDevice.Viewport = v;
			GraphicsDevice.Clear(Color.CornflowerBlue);
			#endregion

			drawList.Each(ent => ent.Draw(gameTime));

			base.Draw(gameTime);
			
		}
		#region Register Methods
		internal void RegisterUpdate(Base.Entity entity) {
			updateList.Add(entity);
		}

		internal void RegisterPreDraw(Base.Entity entity) {
			preDrawList.Add(entity);
		}

		internal void RegisterDraw(Base.Entity entity) {
			drawList.Add(entity);
		}

		internal void RegisterMouseAction(Base.Entity entity) {
			mouseActionList.Add(entity);
		}
		internal void RegisterKeyboardAction(Base.Entity entity) {
			keyboardActionList.Add(entity);
		}
		#endregion
	}
}
