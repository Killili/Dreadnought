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
using DreadnoughtOvermind.Common;

namespace Dreadnought {
	public class Game : Microsoft.Xna.Framework.Game {
		public static Game GameInstance;
		GraphicsDeviceManager graphics;
		internal Uplink Uplink;
		private Grid grid;
		public Sidemenu UI;
		
		private List<GameEntity> drawList = new List<GameEntity>();
		private List<GameEntity> preDrawList = new List<GameEntity>();
		private List<GameEntity> drawOverlayList = new List<GameEntity>();
		private List<GameEntity> updateList = new List<GameEntity>();
		private List<GameEntity> keyboardActionList = new List<GameEntity>();
		private List<GameEntity> mouseActionList = new List<GameEntity>();

		public Camera Camera { get; private set; }
		public Matrix World { get; private set; }
		public Vector3 SunDirection = Vector3.Right;
		public SpriteBatch Overlay;
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
			Uplink = new Uplink("Test","Blub");
		}

		protected override void Initialize() {
			init();
			Overlay = new SpriteBatch(GraphicsDevice);
			// Init Camera
			Camera = new Common.Camera();
			// init fps counter
			var fpsCntr = new FPSCounter(this);
			fpsCntr.Updated += delegate { this.Window.Title = String.Format("Dreadnought  FPS: {0}  ID: {1}",fpsCntr.FPS,Uplink.ID); };
			Components.Add(fpsCntr);
			// add Sky
			SkySphere sk = new SkySphere();
			// add grid
			grid = new Grid();
			
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
			
			//grid.Position = gp.CameraSpace(Camera);
			if(grid.Position.DistanceTo(Camera.LookAt) >= grid.Scale) {
				//grid.Position = new UniversalCoordinate( Ship.Position );
			}
			//Camera.LookAt = Ship.Position;

			
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

			//Overlay.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
			//drawOverlayList.Each(ent => ent.DrawOverlay(gameTime));
			//Overlay.End();

			base.Draw(gameTime);
			
		}
		#region Register Methods
		internal void RegisterUpdate(Base.GameEntity entity) {
			updateList.Add(entity);
		}

		internal void RegisterPreDraw(Base.GameEntity entity) {
			preDrawList.Add(entity);
		}

		internal void RegisterDraw(Base.GameEntity entity) {
			drawList.Add(entity);
		}

		internal void RegisterOverlay(Base.GameEntity entity) {
			drawOverlayList.Add(entity);
		}

		internal void RegisterMouseAction(Base.GameEntity entity) {
			mouseActionList.Add(entity);
		}
		internal void RegisterKeyboardAction(Base.GameEntity entity) {
			keyboardActionList.Add(entity);
		}
		#endregion
	}
}
