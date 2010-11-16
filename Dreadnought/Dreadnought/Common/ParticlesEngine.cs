using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dreadnought.Common {

    class ParticlesEngine :Microsoft.Xna.Framework.DrawableGameComponent {

        enum ParticleType {
            Explosion,
            Smoke,
            Fire
        };

        private KeyboardState currentKeyboardState;
        private KeyboardState lastKeyboardState;

        private BasicEffect effect;
        private List<VertexPositionColor> particlesPool;
        private List<Vector3> particlesForce;
        private List<short> particlesDrawOrder;
        private short numParticles;
        private Random ran;
        public Vector3 Position;

        public ParticlesEngine( Game game, ParticleType particleType )
            : base( game ) {
            
        }

        protected override void LoadContent() {
            effect = new BasicEffect( Game.GraphicsDevice );
        }

        public override void Update( GameTime gameTime ) {
            
            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime ) {

        }
    }
}
