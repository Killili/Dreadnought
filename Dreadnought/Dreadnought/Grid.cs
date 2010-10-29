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

        public Grid( Game game ) : base( game ) {
            pointList = new List<VertexPositionColor>();
            pointOrder = new List<short>();

            // fill list with vertices
            // dont try to put both inner for-llops into one
            // this would change the vertices order!
            for( int z = 0; z <= 5; z++ ) {
                for( int i = 0; i <= 5; i++ ) {
                    pointList.Add( new VertexPositionColor( new Vector3( i, 0, z ), Color.DarkBlue ) );
						  pointList.Add(new VertexPositionColor(new Vector3(i, 5, z), Color.DarkBlue));
                }
                for( int i = 0; i <= 5; i++ ) {
						 pointList.Add(new VertexPositionColor(new Vector3(0, i, z), Color.DarkBlue));
						 pointList.Add(new VertexPositionColor(new Vector3(5, i, z), Color.DarkBlue));
                }
            }

            // set vertices draw order
            for( short i = 0; i < pointList.Count; i++ ) {
                pointOrder.Add( i );
            }
        }

        protected override void LoadContent() {
            effect = new BasicEffect( Game.GraphicsDevice );
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
				effect.FogEnabled = true;
				effect.FogColor = new Vector3(0.0f);
				effect.FogStart = 5000f;
				effect.FogEnd = 7000f;


        }

        public override void Update( GameTime gameTime ) {
            effect.View = ( (Game)Game ).Camera.View;
            effect.Projection = ( (Game)Game ).Camera.Projection;
            effect.World = Matrix.CreateWorld( Vector3.Zero, Vector3.Forward, Vector3.Up )* Matrix.CreateTranslation(Position) * Matrix.CreateScale( 5000.0f );
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
