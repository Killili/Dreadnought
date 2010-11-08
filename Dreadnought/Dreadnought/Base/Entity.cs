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
		public UniversalPosition Position;

		public Entity():base(){
			Game = Dreadnought.Game.GameInstance;
			GraphicsDevice = Game.GameInstance.GraphicsDevice;
		}
		public virtual void Update(GameTime gameTime) { }
		public virtual void Draw(GameTime gameTime) { }
		public virtual void  PreDraw( GameTime gameTime ){}
		public virtual void KeyboardAction(GameTime gametime,KeyboardState ks) {}
		public virtual void MouseAction(GameTime gametime,MouseState ms) {}
	}
}
