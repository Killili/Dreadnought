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
    public class Camera :GameComponent {

        public Vector3 Position = Vector3.Forward;
        public Vector3 LookAt = Vector3.Zero;
        public Quaternion Orientation = Quaternion.CreateFromAxisAngle( Vector3.Left, 40f );

        public Matrix View;
        public Matrix Projection;

        public Vector3 Up = Vector3.Up;
        public float Zoom { get; set; }
        
        private bool reset = true;
        private Vector3 shipPos;

        public Camera( Game game ) : base( game ) {
            Zoom = 1000;
        }

        internal void TurnRight( int x ) {
            reset = false;
            shipPos = ( (Game)Game ).ship.Position;
            Quaternion rotation = Quaternion.CreateFromAxisAngle( Vector3.Up, MathHelper.ToRadians( x / 4f ) );
            Quaternion.Concatenate( ref Orientation, ref rotation, out Orientation );
            Orientation.Normalize();
            LookAt = shipPos;
        }

        internal void TurnLeft( int x ) {
            reset = false;
            shipPos = ( (Game)Game ).ship.Position;
            Quaternion rotation = Quaternion.CreateFromAxisAngle( Vector3.Down, MathHelper.ToRadians( x / 4f ) );
            Quaternion.Concatenate( ref Orientation, ref rotation, out Orientation );
            Orientation.Normalize();
            LookAt = shipPos;
        }

        internal void TurnUp( int y ) {
            reset = false;
            shipPos = ( (Game)Game ).ship.Position;
            Quaternion rotation = Quaternion.CreateFromAxisAngle( Vector3.Left, MathHelper.ToRadians( y / 2f ) );
            Quaternion.Concatenate( ref Orientation, ref rotation, out Orientation );
            Orientation.Normalize();
            LookAt = shipPos;
        }

        internal void TurnDown( int y ) {
            reset = false;
            shipPos = ( (Game)Game ).ship.Position;
            Quaternion rotation = Quaternion.CreateFromAxisAngle( Vector3.Right, MathHelper.ToRadians( y / 2f ) );
            Quaternion.Concatenate( ref Orientation, ref rotation, out Orientation );
            Orientation.Normalize();
            LookAt = shipPos;
        }

        internal void resetLook() {
            reset = true;
        }

        public override void Update( GameTime gameTime ) {
            shipPos = ( (Game)Game ).ship.Position;
            LookAt = shipPos;
            Projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.PiOver4, ( (Game)Game ).GraphicsDevice.Viewport.AspectRatio, 1, 200000 );
            if(reset) {
                Orientation = Quaternion.CreateFromAxisAngle( Vector3.Right, 45f );
                Position = shipPos + new Vector3( 0f, 1000f, 1500f );
                View = Matrix.CreateLookAt( Position, LookAt, Up );
            } else {
                Position = shipPos + Vector3.Transform( Vector3.Backward * Zoom, Orientation );
                View = Matrix.CreateLookAt( Position, LookAt, Up );
            }
            base.Update( gameTime );
        }
    }
}
