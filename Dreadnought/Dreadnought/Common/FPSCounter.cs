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


namespace Dreadnought.Common {
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class FPSCounter : Microsoft.Xna.Framework.DrawableGameComponent {
		private float updateInterval = 0.5f;
		private float tLastUpdate = 1.0f;
		private float frameCounter = 0.0f;
		private int fps = 0;

		/// <summery>
		/// FPScounter updated event
		/// </summary>
		public event EventHandler<EventArgs> Updated;

		public float FPS { get { return fps; } }

		public FPSCounter(Game game)
			: base(game) {
			// TODO: Construct any child components here
				Enabled = true;
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize() {
			// TODO: Add your initialization code here

			base.Initialize();
		}

        public override void Update( GameTime gameTime ) {
             // just a dummy
        }
		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime) {
			// TODO: Add your update code here
			base.Update(gameTime);
			float tElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			frameCounter++;
			tLastUpdate += tElapsed;
			if(tLastUpdate > updateInterval) {
				fps = (int)(frameCounter / tLastUpdate);
				frameCounter = 0.0f;
				tLastUpdate -= updateInterval;
				if(Updated != null)
					Updated(this, new EventArgs());
			}
		}
	}
}
