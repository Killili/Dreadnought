using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
    class Grid :Microsoft.Xna.Framework.DrawableGameComponent {

        private BasicEffect effect;
        private List<VertexPositionColor> pointList;
        private List<short> pointOrder;
        public Vector3 Position;

        public Grid( Game game )
            : base( game ) {
            pointList = new List<VertexPositionColor>();
            pointOrder = new List<short>();

            // fill list with vertices
            // dont try to put both inner for-llops into one
            // this would change the vertices order!
            
            /***** this is the init loop for the grid; maybe this is not neccessary *****/
/*            // add lines from top to bottom
            for( int z = 0; z <= 10; z++ ) {
                for( int i = 0; i <= 10; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( i, 0, z ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( i, 10, z ), Color.DarkBlue ) );
                }
                for( int i = 0; i <= 10; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( 0, i, z ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( 10, i, z ), Color.DarkBlue ) );
                }
            }
            // set vertices draw order
            for( short i = 0; i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }
            for( short i = 0; i < pointList.Count - 44; i++ ) {
                pointOrder.Add( i );
                pointOrder.Add( (short)( i + 44 ) );
            }

            // add lines from near to far
            for( int x = 0; x <= 10; x++ ) {
                for( int y = 0; y <= 10; y++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( x, y, 0 ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( x, y, 10 ), Color.DarkBlue ) );
                }
            }
            // 484 is 11*44; see first for-loops, 0..10 * (2*(2*0..10))
            for( short i = 484; i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }
*/
        }

        protected override void LoadContent() {
            effect = new BasicEffect( Game.GraphicsDevice );
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
            effect.FogEnabled = true;
            effect.FogColor = new Vector3( 0.0f );
            effect.FogStart = 10000.0f;
            effect.FogEnd = 20000.0f;
        }

        public override void Update( GameTime gameTime ) {
            /***** this is the update loop for the grid *****/
            pointList.Clear();
            pointOrder.Clear();
            // add lines from top to bottom
            for( int z = 0; z <= 10; z++ ) {
                for( int i = 0; i <= 10; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( i, 0, z ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( i, 10, z ), Color.DarkBlue ) );
                }
                for( int i = 0; i <= 10; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( 0, i, z ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( 10, i, z ), Color.DarkBlue ) );
                }
            }
            // set vertices draw order
            for( short i = 0; i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }
            for( short i = 0; i < pointList.Count - 44; i++ ) {
                pointOrder.Add( i );
                pointOrder.Add( (short)( i + 44 ) );
            }

            // add lines from near to far
            for( int x = 0; x <= 10; x++ ) {
                for( int y = 0; y <= 10; y++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( x, y, 0 ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( x, y, 10 ), Color.DarkBlue ) );
                }
            }
            // 484 is 11*44; see first for-loops, 0..10 * (2*(2*0..10))
            for( short i = 484; i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }

            effect.View = ( (Game)Game ).Camera.View;
            effect.Projection = ( (Game)Game ).Camera.Projection;
            effect.World = Matrix.CreateWorld( Vector3.Zero, Vector3.Forward, Vector3.Up ) * Matrix.CreateTranslation( Position ) * Matrix.CreateScale( 5000.0f );
            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime ) {
            foreach( EffectPass pass in effect.CurrentTechnique.Passes ) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    pointList.ToArray(),
                    0,
                    pointList.Count,
                    pointOrder.ToArray(),
                    0,
                    pointOrder.Count / 2 );
            }
        }
    }
}
