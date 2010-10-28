using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
    class Grid :Microsoft.Xna.Framework.DrawableGameComponent {
        Game game;
        private BasicEffect effect;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Matrix World { get; private set; }

        private static int points = 40;
        private VertexPositionColor[] pointList = new VertexPositionColor[points];
        private short[] lineListIndices;

        public Grid( Game game ) : base( game ) {
            this.game = game;
            for( int x = 0; x < points / 2; x++ ) {
                for( int y = 0; y < 2; y++ ) {
                    pointList[( x * 2 ) + y] = new VertexPositionColor(
                        // z is 490 because camera is set to 500
                        new Vector3( x * 100 , y * 100 , 490 ), Color.White );
                }
            }
            // Initialize an array of indices of type short.
            // Populate the array with references to indices in the vertex buffer
            lineListIndices = new short[( points * 2 ) - 2];
            for( int i = 0; i < points - 1; i++ ) {
                lineListIndices[i * 2] = (short)( i );
                lineListIndices[( i * 2 ) + 1] = (short)( i + 1 );
            }
        }

        public void Load() {
            effect = new BasicEffect( game.GraphicsDevice );
            effect.VertexColorEnabled = true;
            effect.World = game.World;
        }

        public override void Update( GameTime gameTime ) {

        }

        public override void Draw( GameTime gameTime ) {
            foreach( EffectPass pass in effect.CurrentTechnique.Passes ) {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>( 
                    PrimitiveType.LineList,
                    pointList.ToArray(),
                    0,
                    points,
                    lineListIndices,
                    0,
                    points-1 );
            }
        }
    }
}
