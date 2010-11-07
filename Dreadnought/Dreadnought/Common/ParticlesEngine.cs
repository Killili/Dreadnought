using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought.Common {

    class ParticlesEngine :Microsoft.Xna.Framework.DrawableGameComponent {

        private BasicEffect effect;
        private List<VertexPositionColor> particlesPool;
        private List<Vector3> particlesForce;
        private List<short> particlesDrawOrder;
        private short numParticles;
        private Random ran;
        public Vector3 Position;

        public ParticlesEngine( Game game )
            : base( game ) {
            particlesPool = new List<VertexPositionColor>();
            particlesForce = new List<Vector3>();
            particlesDrawOrder = new List<short>();
            numParticles = 100;
            Position = new Vector3( 0f, 0f, 0f );

            /***** this is the init loop for the particles pool *****/
            ran = new Random();
            for(short i = 0; i < numParticles; i++) {
                particlesPool.Add( new VertexPositionColor( new Vector3( 10f, 10f, 0f ), Color.White ) );
                particlesForce.Add( new Vector3( (float)ran.NextDouble(), (float)ran.NextDouble(), (float)ran.NextDouble() ) );
            }

            /***** fill draworder *****/
            for(short i = 0; i < numParticles; i++) {
                particlesDrawOrder.Add( i );
            }
        }

        protected override void LoadContent() {
            effect = new BasicEffect( Game.GraphicsDevice );
        }

        public override void Update( GameTime gameTime ) {
            effect.View = ((Game)Game).Camera.View;
            effect.Projection = ((Game)Game).Camera.Projection;
            effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            ran = new Random();
            for(short i = 0; i < numParticles; i++) {
                particlesPool.Add( new VertexPositionColor( particlesPool[0].Position + new Vector3( particlesForce[i].X, particlesForce[i].Y, 0f ), Color.White ) );
                particlesPool.RemoveAt( 0 );
            }
            if( ran.Next(10) < 7 )
                for(short i = 0; i < numParticles; i++) {
                    particlesPool.Add( new VertexPositionColor( particlesPool[0].Position + new Vector3( -0.7f, -0.7f, 0f ), Color.White ) );
                    particlesPool.RemoveAt( 0 );
                }
            particlesForce.Clear();
            for(short i = 0; i < numParticles; i++) {
                particlesForce.Add( new Vector3( (float)ran.NextDouble(), (float)ran.NextDouble(), (float)ran.NextDouble() ) );
            }

            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime ) {

            foreach(EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    particlesPool.ToArray(),
                    0,
                    particlesPool.Count,
                    particlesDrawOrder.ToArray(),
                    0,
                    particlesDrawOrder.Count / 2 );
            }
        }
    }
}
