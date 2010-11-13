﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Dreadnought.Base;

namespace Dreadnought {
    class Grid : Entity {

        private Effect effect;
        private List<VertexPositionColor> pointList;
        private List<short> pointOrder;
			public float Scale;
        public int Size;

        public Grid()
            : base() {
            pointList = new List<VertexPositionColor>();
            pointOrder = new List<short>();
            Scale = 5000f;
            Size = 10;
			  Position = new UniversalCoordinate();

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
				LoadContent();
				Game.RegisterDraw(this);
        }

        private void LoadContent() {
            effect = Game.Content.Load<Effect>( "GridEffect" );
				effect.Parameters["Near"].SetValue(5000f);
				effect.Parameters["Far"].SetValue(15000f);
				effect.Parameters["Color"].SetValue(new Vector4(0f, 0f, 0.6f, 0.3f));
        }

        public override void Draw( GameTime gameTime ) {
			  GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			  Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateWorld(Position.CameraSpace(Game.Camera), Vector3.Forward, Vector3.Up) * Matrix.CreateTranslation(new Vector3(-((Size / 2f) * Scale)));
			  effect.Parameters["BlendPoint"].SetValue(Vector3.Zero);
			  effect.Parameters["gWVP"].SetValue(world * Game.Camera.View * Game.Camera.Projection);
			  effect.Parameters["gWorld"].SetValue(world);
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
				GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
