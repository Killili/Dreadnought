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
		public Camera Camera { get; private set; }
		public Matrix World { get; private set; }


		public Game() {
			graphics = new GraphicsDeviceManager(this);

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
			c.MouseEnter += new System.Windows.Input.MouseEventHandler(c_MouseEnter);
			c.MouseLeave += new System.Windows.Input.MouseEventHandler(c_MouseLeave);
			c.Output = "Test";
			host.Location = new System.Drawing.Point(0, 0);
			host.Size = new System.Drawing.Size(200, Window.ClientBounds.Height);
			host.BackColorTransparent = true;
			System.Windows.Forms.Control.FromHandle(Window.Handle).Controls.Add(host);
			followMouse = true;
		}

		void c_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			followMouse = true;
		}

		void c_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
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
			if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Q)) {
				this.Exit();
			}

			if(followMouse) {
				MouseState ms = Mouse.GetState();
				Camera.Position = new Vector3(ms.X, ms.Y, ms.ScrollWheelValue);
			}

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
