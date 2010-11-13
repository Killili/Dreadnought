using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought.Base {
	public class Entity{
		public Dreadnought.Game Game;
		public GraphicsDevice GraphicsDevice;
		public UniversalCoordinate Position;

		public Entity():base(){
			Game = Dreadnought.Game.GameInstance;
			GraphicsDevice = Game.GameInstance.GraphicsDevice;
		}
		public virtual void Update(GameTime gameTime) { throw new NotImplementedException(); }
		public virtual void Draw(GameTime gameTime) { throw new NotImplementedException(); }
		public virtual void PreDraw(GameTime gameTime) { throw new NotImplementedException(); }
		public virtual void DrawOverlay(GameTime gameTime) { throw new NotImplementedException(); }
		public virtual void KeyboardAction(GameTime gametime, KeyboardState ks) { throw new NotImplementedException(); }
		public virtual void MouseAction(GameTime gametime, MouseState ms) { throw new NotImplementedException(); }
	}
}
