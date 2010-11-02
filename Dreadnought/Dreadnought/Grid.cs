using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dreadnought {
    class Grid :Microsoft.Xna.Framework.DrawableGameComponent {

        private Effect effect;
        private List<VertexPositionColor> pointList;
        private List<short> pointOrder;
        public Vector3 Position;
        public float Scale;
        public int Size;

        Vector3 v;

        public Grid( Game game )
            : base( game ) {
            pointList = new List<VertexPositionColor>();
            pointOrder = new List<short>();
            Scale = 5000f;
            Size = 10;

            // fill list with vertices
            // dont try to put both inner for-llops into one
            // this would change the vertices order!

            /***** this is the init loop for the grid *****/
            // add lines from top to bottom
            for( int z = 0; z <= Size; z++ ) {
                for( int i = 0; i <= Size; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( i , 0, z  ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( i, Size, z ), Color.DarkBlue ) );
                }
                for( int i = 0; i <= Size; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( 0, i , z  ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( Size, i, z ), Color.DarkBlue ) );
                }
            }
            // set vertices draw order
            for( short i = 0; i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }
            for( short i = 0; i < pointList.Count - ((Size+1)*4); i++ ) {
                pointOrder.Add( i );
                pointOrder.Add( (short)( i + ( ( Size + 1 ) * 4 ) ) );
            }

            // add lines from near to far
            for( int x = 0; x <= Size; x++ ) {
                for( int y = 0; y <= Size; y++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( x , y , 0 ), Color.DarkBlue ) );
                    pointList.Add( new VertexPositionColor( new Vector3( x, y, Size ), Color.DarkBlue ) );
                }
            }
            for( short i = (short)( Size+1 * ( ( ( Size + 1 ) * 4 ) ) ); i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }

        }

        protected override void LoadContent() {
            effect = ( (Game)Game ).Content.Load<Effect>( "GridEffect" );
        }

        public override void Update( GameTime gameTime ) {
            v = ( (Game)Game ).Camera.Position;
            Matrix World = Matrix.CreateWorld( Position, Vector3.Forward, Vector3.Up ) * Matrix.CreateScale( Scale );
            effect.Parameters["BlendPoint"].SetValue( v );
            effect.Parameters["Near"].SetValue( 10000f );
            effect.Parameters["Far"].SetValue( 15000f );
            effect.Parameters["gWVP"].SetValue( World * ( (Game)Game ).Camera.View * ( (Game)Game ).Camera.Projection );
            effect.Parameters["gWorld"].SetValue( World );
            effect.Parameters["Color"].SetValue( new Vector4( 0f, 0f, 0.6f, 1f ) );
            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime ) {
            ( (Game)Game ).GraphicsDevice.RasterizerState = RasterizerState.CullNone;
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
