using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TreasureTown
{
	public static class MouseManager
	{
		private static MouseState _lastFrameState;
		private static MouseState _thisFrameState;


		public static void Initialize ()
		{
		}


		public static void Update(GameTime gameTime)
		{
			_lastFrameState = _thisFrameState;
			_thisFrameState = Mouse.GetState();

			if(LeftClickUp)
				Console.WriteLine("Clicked at " + Position.X + ", " + Position.Y);
		}

		public static float X 
		{ 
			get { return _thisFrameState.X; } 
		}

		public static float Y 
		{ 
			get { return _thisFrameState.Y; } 
		}

		public static Vector2 Position 
		{ 
			get 
			{ 
				float x = ((float)_thisFrameState.X / (float)TreasureTown.FullscreenRect.Width) * 800;
				float y = ((float)_thisFrameState.Y / (float)TreasureTown.FullscreenRect.Height) * 600;
				return new Vector2(x, y); 
			} 
		}

		public static bool LeftClickDown
		{
			get { return (_lastFrameState.LeftButton ==  ButtonState.Released && _thisFrameState.LeftButton == ButtonState.Pressed); }
		}

		public static bool LeftClickUp
		{
			get { return (_lastFrameState.LeftButton ==  ButtonState.Pressed && _thisFrameState.LeftButton == ButtonState.Released); }
		}

		public static bool LeftButtonIsDown
		{
			get { return (_thisFrameState.LeftButton == ButtonState.Pressed); }
		}

		public static bool RightClickDown
		{
			get { return (_lastFrameState.RightButton ==  ButtonState.Released && _thisFrameState.RightButton == ButtonState.Pressed); }
		}

		public static bool RightClickUp
		{
			get { return (_lastFrameState.RightButton ==  ButtonState.Pressed && _thisFrameState.RightButton == ButtonState.Released); }
		}

		public static bool RightButtonIsDown
		{
			get { return (_thisFrameState.RightButton == ButtonState.Pressed); }
		}

		public static bool MouseInRect(Rectangle rect)
		{
			return(Position.X >= rect.X && 
			       Position.X <= rect.X + rect.Width && 
			       Position.Y >= rect.Y && 
			       Position.Y <= rect.Y + rect.Height);
		}
	}
}

