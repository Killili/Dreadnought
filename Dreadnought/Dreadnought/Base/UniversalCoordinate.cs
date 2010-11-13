using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dreadnought.Common;

namespace Dreadnought.Base {
	public class UniversalCoordinate {
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
					localPos.X = value % StellarGridSize;
					Stellar.X += (int)(value / StellarGridSize);
				} else {
					localPos.X = value;
				}
			}
		}
		public float Y {
			get { return localPos.Y; }
			set {
				if(Math.Abs(value) > StellarGridSize) {
					localPos.Y = value % StellarGridSize;
					Stellar.Y += (int)(value / StellarGridSize);
				} else {
					localPos.Y = value;
				}
			}
		}

		public float Z {
			get { return localPos.Z; }
			set {
				if(Math.Abs(value) > StellarGridSize) {
					localPos.Z = value % StellarGridSize;
					Stellar.Z += (int)(value / StellarGridSize);
				} else {
					localPos.Z = value;
				}
			}
		}

		public UniversalCoordinate(UniversalCoordinate p2) {
			this.Stellar.X = p2.Stellar.X;
			this.Stellar.Y = p2.Stellar.Y;
			this.Stellar.Z = p2.Stellar.Z;
			this.localPos = p2.localPos;
		}
		public UniversalCoordinate() {
			this.Stellar.X = 0;
			this.Stellar.Y = 0;
			this.Stellar.Z = 0;
			this.localPos = Vector3.Zero;
		}

		public Vector3 CameraSpace(Camera camera){
			Vector3 ret;
			ret.X = Stellar.X - camera.LookAt.Stellar.X;
			ret.Y = Stellar.Y - camera.LookAt.Stellar.Y;
			ret.Z = Stellar.Z - camera.LookAt.Stellar.Z;
			ret *= StellarGridSize;
			ret += localPos;// - camera.LookAt.Local;
			return ret;
		}

		public double DistanceTo(UniversalCoordinate p2) {
			double sd = Math.Sqrt(
				(Stellar.X - p2.Stellar.X) * (Stellar.X - p2.Stellar.X) +
				(Stellar.Y - p2.Stellar.Y) * (Stellar.Y - p2.Stellar.Y) +
				(Stellar.Z - p2.Stellar.Z) * (Stellar.Z - p2.Stellar.Z));
			sd = sd * StellarGridSize;
			sd += Vector3.Distance( localPos,p2.Local);
			return sd;
		}

		public override string ToString() {
			return '{' + Stellar.X.ToString() + '|' + localPos.X.ToString() + ';' + Stellar.Y.ToString() + '|' + localPos.Y.ToString() + ';' + Stellar.Z.ToString() + '|' + localPos.Z.ToString() + '}';
		}
	}
}
