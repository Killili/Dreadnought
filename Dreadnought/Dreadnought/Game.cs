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
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game : Microsoft.Xna.Framework.Game {
		GraphicsDeviceManager graphics;
        
		Ship ship;
		private bool followMouse;
        private int height = 768;
        private int width = 1280;
		public Camera Camera { get; private set; }
		public Matrix World { get; private set; }
        //FormHandle windowFormHandle;

		public Game() {
			graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            graphics.ApplyChanges();
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			// TODO: Add your initialization logic here

			base.Initialize();
            Viewport v = GraphicsDevice.Viewport;
			v.Width = Window.ClientBounds.Width - 200;
			v.X = 200;
			GraphicsDevice.Viewport = v;
			ElementHost host = new ElementHost();
			Sidemenu c = new Sidemenu();
			host.Child = c;
			c.MouseEnter += new System.Windows.Input.MouseEventHandler(mouseEnteredMenu);
			c.MouseLeave += new System.Windows.Input.MouseEventHandler(mouseLeftMenu);
			c.Output = "TEst3";
			host.Location = new System.Drawing.Point(0, 0);
			host.Size = new System.Drawing.Size(200, Window.ClientBounds.Height);
			host.BackColorTransparent = true;
			System.Windows.Forms.Control.FromHandle(Window.Handle).Controls.Add(host);
			followMouse = true;
            //windowFormHandle = System.Windows.Forms.Control.FromHandle(Window.Handle);
            System.Windows.Forms.Control.FromHandle(Window.Handle).Width = width;
            System.Windows.Forms.Control.FromHandle(Window.Handle).Height = height;
            
		}

		void mouseLeftMenu(object sender, System.Windows.Input.MouseEventArgs e) {
			followMouse = true;
		}

		void mouseEnteredMenu(object sender, System.Windows.Input.MouseEventArgs e) {
			followMouse = false;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Camera = new Common.Camera(this);
			Camera.Load(Content);
			Camera.Position = new Vector3(1, 1000, 1);
			ship = new Ship(this);
			ship.Load(Content);



			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			// Allows the game to exit
			KeyboardState ks = Keyboard.GetState(PlayerIndex.One);
			MouseState ms = Mouse.GetState();

			if(ks.IsKeyDown(Keys.Escape)) {
				this.Exit();
			}

			if(ks.IsKeyDown(Keys.Up)) {
				ship.accelerate();
			} else if(ks.IsKeyDown(Keys.Down)) {
				ship.decelerate();
			}
			if(ks.IsKeyDown(Keys.A)) {
				ship.turnLeft();
			} else if(ks.IsKeyDown(Keys.D)) {
				ship.turnRight();
			}

			if(ks.IsKeyDown(Keys.X)){
				ship.counterRotation();
			}
			if(ks.IsKeyDown(Keys.C)) {
				ship.counterMoment();
			}

			if(ks.IsKeyDown(Keys.S)) {
				ship.turnUp();
			} else if(ks.IsKeyDown(Keys.W)) {
				ship.turnDown();
			}

			if(ks.IsKeyDown(Keys.R)) {
				ship.rise();
			} else if(ks.IsKeyDown(Keys.F)) {
				ship.sink();
			}

			if(ks.IsKeyDown(Keys.Q)) {
				ship.rollLeft();
			} else if(ks.IsKeyDown(Keys.E)) {
				ship.rollRight();
			}


			if(followMouse) {
				Camera.Position = new Vector3(1, 1000+ms.ScrollWheelValue , 1);
			}
			//Camera.Position = ship.Position - new Vector3(0f, -500f, -500f);
			Camera.LookAt = ship.Position;

			//Camera.Position = Vector3.Transform(Camera.Position, Matrix.CreateTranslation(Vector3.Up));
			//World *= Matrix.CreateRotationY(MathHelper.ToRadians(1f));
			// TODO: Add your update logic here
			Camera.Update(gameTime);
			ship.Update(gameTime);
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);



			ship.Draw();
			Camera.Draw();

			base.Draw(gameTime);
		}
	}
}
