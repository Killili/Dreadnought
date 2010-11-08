using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dreadnought.Common;

namespace Dreadnought.Base {
	public class UniversalPosition {
		const float	StellarGridSize = 1000f;
		const float StellarGridWidth = StellarGridSize*2;
		public struct StellarPosition {
			public Int64 X;
			public Int64 Y;
			public Int64 Z;
		}
		public StellarPosition Stellar = new StellarPosition();

		private Vector3 localPos = Vector3.Zero;
		public Vector3 Local{
			get { return localPos; }
			set {
				this.X = value.X;
				this.Y = value.Y;
				this.Z = value.Z;
			}
		}

		public float X{ 
			get{ return localPos.X; }
			set{
				if(Math.Abs(value) > StellarGridSize) {
					if(Math.Sign(value) < 0) {
						localPos.X = 1000 + (value % StellarGridWidth);
					} else {
						localPos.X = value % StellarGridWidth;
					}
					Stellar.X += (int)(value / StellarGridWidth);
				} else {
					localPos.X = value;
				}
			}
		}
		public float Y {
			get { return localPos.Y; }
			set {
				if(Math.Abs(value) > StellarGridWidth) {
					if(Math.Sign(value) < 0) {
						localPos.Y = 1000 + (value % StellarGridWidth);
					} else {
						localPos.Y = value % StellarGridWidth;
					}
					Stellar.Y += (int)(value / StellarGridWidth);
				} else {
					localPos.Y = value;
				}
			}
		}

		public float Z {
			get { return localPos.Z; }
			set {
				if(Math.Abs(value) > StellarGridWidth) {
					if(Math.Sign(value) < 0) {
						localPos.Z = 1000 + (value % StellarGridWidth);
					} else {
						localPos.Z = value % StellarGridWidth;
					}
					Stellar.Z += (int)(value / StellarGridWidth);
				} else {
					localPos.Z = value;
				}
			}
		}
		
		public Vector3 CameraSpace(Camera camera){
			Vector3 ret;
			ret.X = camera.LookAt.Stellar.X + Stellar.X;
			ret.Y = camera.LookAt.Stellar.Y + Stellar.Y;
			ret.Z = camera.LookAt.Stellar.Z + Stellar.Z;
			ret *= StellarGridSize;
			ret += localPos + camera.LookAt.localPos;
			return ret;
		}
	}
}
