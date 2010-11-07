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
    public class Game :Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;

        internal Ship ship;
        private Grid grid;
        private ParticlesEngine particlesBLOB;

        public Sidemenu UI;
        private Nullable<Vector3> faceDir;

        private int msPosX, msPosY;

        public Camera Camera { get; private set; }
        public Matrix World { get; private set; }

        #region Config and Windows
        private void config() {
#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += (s, e) => {
				Exception ex = (Exception)e.ExceptionObject;
				System.Windows.MessageBox.Show(ex.Message);
			};
#endif
            graphics = new GraphicsDeviceManager( this );
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 200 + 1024;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            ElementHost host = new ElementHost();
            UI = new Sidemenu();
            host.Child = UI;

            host.Location = new System.Drawing.Point( 0, 0 );
            host.Size = new System.Drawing.Size( 200, Window.ClientBounds.Height );
            host.BackColorTransparent = true;
            System.Windows.Forms.Control.FromHandle( Window.Handle ).Controls.Add( host );
        }
        private void init() {
            Viewport v = GraphicsDevice.Viewport;
            v.Width = Window.ClientBounds.Width - 200;
            v.X = 200;
            GraphicsDevice.Viewport = v;
        }
        #endregion

        public Game() {
            config();
        }

        protected override void Initialize() {
            init();
            // init fps counter
            var fpsCntr = new FPSCounter( this );
            fpsCntr.Updated += delegate { this.Window.Title = "Dreadnought  (FPS: " + fpsCntr.FPS.ToString() + " )"; };
            Components.Add( fpsCntr );

            // add particlesBLOB
            particlesBLOB = new ParticlesEngine( this );
            Components.Add( particlesBLOB );

            base.Initialize();
        }

        protected override void LoadContent() {
            World = Matrix.CreateWorld( Vector3.Zero, Vector3.Forward, Vector3.Up );
            Camera = new Common.Camera( this );
        }

        protected override void UnloadContent() {
        }

        protected override void Update( GameTime gameTime ) {
            KeyboardState ks = Keyboard.GetState( PlayerIndex.One );
            MouseState ms = Mouse.GetState();

            if(ks.IsKeyDown( Keys.Escape )) {
                this.Exit();
            }

            //World *= Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            Camera.Update( gameTime );
            particlesBLOB.Update( gameTime );

            base.Update( gameTime );
        }

        protected override void Draw( GameTime gameTime ) {
            GraphicsDevice.Clear( Color.Black );

            base.Draw( gameTime );
        }
    }
}
