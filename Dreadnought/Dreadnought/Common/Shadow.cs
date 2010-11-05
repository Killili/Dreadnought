using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dreadnought;

namespace Dreadnought.Common {
	class Shadow {
		public RenderTarget2D Map;
		public Matrix View;
		public Matrix Projection;
		Game game;
		Ship ship;
		private SpriteBatch spriteBatch;
		public Effect Effect;

		public Shadow(Game game,Ship ship){
			this.game = game;
			this.ship = ship;
			Map = new RenderTarget2D(game.GraphicsDevice,
																	 1024,
																	 1024,
																	 false,
																	 SurfaceFormat.Single,
																	 DepthFormat.Depth24);
			spriteBatch = new SpriteBatch(game.GraphicsDevice);
			Effect = game.Content.Load<Effect>("ShadowMap");
		}

		private void recalc() {
			Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero, -game.SunDirection, Vector3.Up);
			Vector3[] frustumCorners = ship.BoundingBox.GetCorners();

			for(int i = 0 ; i < frustumCorners.Length ; i++) {
				frustumCorners[i] = Vector3.Transform(frustumCorners[i], ship.Orientation);
				frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
			}
			// Find the smallest box around the points
			BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

			Vector3 boxSize = lightBox.Max - lightBox.Min;
			Vector3 halfBoxSize = boxSize * 0.5f;

			// The position of the light should be in the center of the back
			// pannel of the box. 
			Vector3 lightPosition = lightBox.Min + halfBoxSize;
			lightPosition.Z = lightBox.Min.Z;

			// We need the position back in world coordinates so we transform 
			// the light position by the inverse of the lights rotation
			lightPosition = Vector3.Transform(lightPosition,
														 Matrix.Invert(lightRotation));
			lightPosition += ship.Position;
			// Create the view matrix for the light
			Matrix lightView = Matrix.CreateLookAt(lightPosition,
																lightPosition - game.SunDirection,
																Vector3.Up);

			// Create the projection matrix for the light
			// The projection is orthographic since we are using a directional light
			Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
																				-boxSize.Z, boxSize.Z);
			View = lightView;
			Projection = lightProjection;
		}
		public void CreateShadowMap() {
			recalc();
			game.GraphicsDevice.SetRenderTarget(Map);
			// Clear the render target to white or all 1's
			// We set the clear to white since that represents the 
			// furthest the object could be away
			game.GraphicsDevice.Clear(Color.White);
			//game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			Effect.CurrentTechnique = Effect.Techniques["CreateShadowMap"];
			Effect.Parameters["World"].SetValue(game.World);
			Effect.Parameters["LightDirection"].SetValue(game.SunDirection);
			Effect.Parameters["GameViewProj"].SetValue(game.Camera.View * game.Camera.Projection);
			Effect.Parameters["LightViewProj"].SetValue(View * Projection);
			

			ship.DrawGeometry();

			// Set render target back to the back buffer
			game.GraphicsDevice.SetRenderTarget(null);
			//game.GraphicsDevice.Clear(Color.White);
			//game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		public void DrawWithShadowMap() {
			Effect.CurrentTechnique = Effect.Techniques["DrawWithShadowMap"];
			Effect.Parameters["World"].SetValue(game.World);
			Effect.Parameters["LightDirection"].SetValue(game.SunDirection);
			Effect.Parameters["GameViewProj"].SetValue(game.Camera.View * game.Camera.Projection);
			Effect.Parameters["LightViewProj"].SetValue(View * Projection);
			Effect.Parameters["ShadowMap"].SetValue(Map);
			game.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			ship.DrawGeometry();
			drawShadowMapToScreen();
		}

		void drawShadowMapToScreen() {
			spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
			spriteBatch.Draw(Map, new Rectangle(500, 0, 128, 128), Color.White);
			spriteBatch.End();
			game.GraphicsDevice.BlendState = BlendState.Opaque;
			game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			game.GraphicsDevice.Textures[0] = null;
			game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		}
	}
}
