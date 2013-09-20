using System;
using SchedulerTest;
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

		bool lerpingScale;
		Vector2 scaleLerpStart;
		Vector2 scaleLerpEnd;
		float scaleLerpTime;
		float scaleLerpDuration;

		Vector2 origin = Vector2.Zero;
		Vector2 scale = new Vector2(1f,1f);

		float drawDepth;
		public string Name { get; private set; }

		public GraphicElement (string textureName, int positionX = 0, int positionY = 0, bool isClickable = false, Color? newColor = null, float depth = 1f, string objectName = "")
		{
			texture = TreasureTown.StaticContent.Load<Texture2D>(textureName);
			SetPosition(positionX, positionY);
			if(newColor != null) color = (Color)newColor;
			drawDepth = depth;
			Name = objectName;
		}

		public GraphicElement (Texture2D tex, int positionX = 0, int positionY = 0, bool isClickable = false, Color? newColor = null, float depth = 1f, string objectName = "")
		{
			texture = tex;
			SetPosition(positionX, positionY);
			if(newColor != null) color = (Color)newColor;
			drawDepth = depth;
			Name = objectName;
		}

		public void SetPosition(float x, float y)
		{
			position = new Vector2(x, y); 
		}

		public void SetDepth (float depth)
		{
			drawDepth = depth;
		}

		public void SetColor(float r, float g, float b, float a)
		{
			color = new Color(r,g,b,a);
		}

		public void SetColor(Color c)
		{
			color = c;
		}

		public void SetScale(Vector2 newScale)
		{
			scale = newScale;
		}

		public void SetScale(float newScale)
		{
			scale = new Vector2(newScale, newScale);
		}

		public void SetOrigin(Vector2 newOrigin)
		{
			origin = newOrigin;
		}

		// Because of the naive way that clicking is handled, clickable objects cannot be rotated.
		public void SetRotation(float targetRotation)
		{
			rotation = targetRotation;
		}

		#region LERP FUNCTIONS

		public void LerpPosition (Vector2 target, float duration)
		{
			lerpingPosition = true;
			positionLerpStart = position;
			positionLerpEnd = new Vector2(target.X, target.Y);
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

		public void LerpScale (Vector2 toScale, float duration)
		{
			lerpingScale = true;
			scaleLerpStart = new Vector2(scale.X, scale.Y);
			scaleLerpEnd = toScale;
			scaleLerpTime = 0;
			scaleLerpDuration = duration;
		}

		public void LerpRotation (float targetRotation, float duration)
		{
			lerpingRotation = true;
			rotationLerpStart = rotation;
			rotationLerpEnd = targetRotation;
			rotationLerpTime = 0;
			rotationLerpDuration = duration;
		}

		public void LerpChangeRotation (float rotationChange, float duration)
		{
			lerpingRotation = true;
			rotationLerpStart = rotation;
			rotationLerpEnd = rotation + rotationChange;
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
			if (lerpingScale)
			{
				scaleLerpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				LerpUpdateScale ();
			}
		}

		public void LerpUpdatePosition ()
		{
			if (positionLerpTime >= positionLerpDuration)
			{
				lerpingPosition = false;
				position = positionLerpEnd;
				EventManager.SendSignal (Name + " position lerped");
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
				EventManager.SendSignal (Name + " rotation lerped");
			}
			else
			{				
				rotation = LerpFloat (rotationLerpStart, rotationLerpEnd, rotationLerpTime / rotationLerpDuration);
			}
		}

		public void LerpUpdateScale ()
		{
			if (scaleLerpTime >= scaleLerpDuration)
			{
				lerpingScale = false;
				scale = scaleLerpEnd;
				EventManager.SendSignal (Name + " scale lerped");
			}
			else
			{				
				scale.X = LerpFloat (scaleLerpStart.X, scaleLerpEnd.X, scaleLerpTime / scaleLerpDuration);
				scale.Y = LerpFloat (scaleLerpStart.Y, scaleLerpEnd.Y, scaleLerpTime / scaleLerpDuration);
			}
		}

		public void LerpUpdateColor ()
		{
			if (colorLerpTime >= colorLerpDuration)
			{
				lerpingColor = false;
				color = colorLerpEnd;
				EventManager.SendSignal (Name + " color lerped");
			}
			else
			{
				color = Color.Lerp (colorLerpStart, colorLerpEnd, colorLerpTime / colorLerpDuration);
			}
		}

		public float LerpFloat(float start, float end, float amount)
		{
			return start + ((end - start) * amount);
		}

		#endregion

		public void Update (GameTime gameTime)
		{
			LerpUpdate (gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (texture, position: position, rotation: rotation, color: color, depth: drawDepth, origin: origin, scale: scale);
		}
	}
}

