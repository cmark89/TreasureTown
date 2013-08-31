using System;
using LuaInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreasureTown
{
	public class GraphicElement
	{
		Texture2D texture;

		Vector2 position;
		bool lerpingPosition;
		Vector2 positionLerpStart;
		Vector2 positionLerpEnd;
		float positionLerpTime;
		float positionLerpDuration;

		Color color = Color.White;
		bool lerpingColor;
		Color colorLerpStart;
		Color colorLerpEnd;
		float colorLerpTime;
		float colorLerpDuration;

		float rotation;
		bool lerpingRotation;
		float rotationLerpStart;
		float rotationLerpEnd;
		float rotationLerpTime;
		float rotationLerpDuration;

		bool clickable;
		LuaFunction onClick;

		public GraphicElement (string textureName, int positionX = 0, int positionY = 0)
		{
			texture = TreasureTown.StaticContent.Load<Texture2D>(textureName);
			SetPosition(positionX, positionY);
		}

		public void SetPosition(float x, float y)
		{
			position = new Vector2(x, y); 
		}

		public void SetColor(float r, float g, float b, float a)
		{
			color = new Color(r,g,b,a);
		}

		public void SetRotation(float degrees)
		{
			rotation = DegreesToRadians(degrees);
		}

		public float DegreesToRadians(float degrees)
		{
			return degrees * ((float)Math.PI / 180f);
		}

		#region LERP FUNCTIONS

		public void LerpPosition (int targetX, int targetY, float duration)
		{
			lerpingPosition = true;
			positionLerpStart = position;
			positionLerpEnd = new Vector2(targetX, targetY);
			positionLerpTime = 0;
			positionLerpDuration = duration;
		}

		public void LerpColor (float r, float g, float b, float a, float duration)
		{
			lerpingColor = true;
			colorLerpStart = color;
			colorLerpEnd = new Color(r,g,b,a);
			colorLerpTime = 0;
			colorLerpDuration = duration;
		}

		public void LerpRotation (float deltaRotation, float duration)
		{
			lerpingRotation = true;
			rotationLerpStart = rotation;
			rotationLerpEnd = rotation + DegreesToRadians(deltaRotation);
			rotationLerpTime = 0;
			rotationLerpDuration = duration;
		}

		public void LerpUpdate (GameTime gameTime)
		{
			if (lerpingPosition)
			{
				positionLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				LerpUpdatePosition ();
			}
			if (lerpingColor)
			{
				colorLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				LerpUpdateColor ();
			}
			if (lerpingRotation)
			{
				rotationLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				LerpUpdateRotation ();
			}
		}

		public void LerpUpdatePosition ()
		{
			if (positionLerpTime >= positionLerpDuration)
			{
				lerpingPosition = false;
				position = positionLerpEnd;
			}
			else
			{
				float x = LerpFloat (positionLerpStart.X, positionLerpEnd.X, positionLerpTime / positionLerpDuration);
				float y = LerpFloat (positionLerpStart.Y, positionLerpEnd.Y, positionLerpTime / positionLerpDuration);
				
				position = new Vector2(x,y);
			}
		}

		public void LerpUpdateRotation ()
		{
			if (rotationLerpTime >= rotationLerpDuration)
			{
				lerpingRotation = false;
				rotation = rotationLerpEnd;
			}
			else
			{				
				rotation = LerpFloat (rotationLerpStart, rotationLerpEnd, rotationLerpTime / rotationLerpDuration);
			}
		}

		public void LerpUpdateColor ()
		{
			if (colorLerpTime >= colorLerpDuration)
			{
				lerpingColor = false;
				color = colorLerpEnd;
			}
			else
			{
				color = Color.Lerp (colorLerpStart, colorLerpEnd, colorLerpTime / colorLerpDuration);
			}
		}

		public void LerpFloat(float start, float end, float amount)
		{
			return start + ((end - start) * amount);
		}

		#endregion

	}
}

