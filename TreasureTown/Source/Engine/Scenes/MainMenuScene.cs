using System;
using System.Collections.Generic;
using Eglantine.Engine;
using LuaInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eglantine
{
	public class MainMenuScene : Scene
	{
		private static MainMenuScene _instance;
		public static MainMenuScene Instance 
		{
			get { return _instance; }
		}

		Lua lua;
		Dictionary<string, TitleElement> graphics;
		MainMenuPhase phase;
		bool menuShown = false;
		int buttonWidth;
		int buttonHeight;

		Vector2 origin;
		Rectangle newGameRect;
		Rectangle loadRect;
		Rectangle exitRect;

		Texture2D newGameTexture;
		Texture2D loadTexture;
		Texture2D exitTexture;

		public bool MenuInput = true;

		public MainMenuScene ()
		{
			Initialize();
		}



		// Whether just loading the game or returning to it...
		public override void Initialize()
		{
			phase = MainMenuPhase.Loading;
			_instance = this;
			
			lua = new Lua();
			lua.DoFile ("Data/scheduler.lua");
			lua.DoFile ("Data/Events/MainMenu_events.lua");

			// Load the graphics to be used by the menu
			LoadContent();

			origin = new Vector2(Eglantine.GAME_WIDTH / 2, Eglantine.GAME_HEIGHT /2);

			newGameRect = new Rectangle((int)(origin.X - (buttonWidth / 2)), (int)origin.Y, buttonWidth, buttonHeight);
			loadRect = new Rectangle((int)(int)(origin.X - (buttonWidth / 2)), (int)(origin.Y + buttonHeight + 10), buttonWidth, buttonHeight);
			exitRect = new Rectangle((int)(origin.X - (buttonWidth / 2)), (int)(origin.Y + (buttonHeight + 10) * 2), buttonWidth, buttonHeight);

			NextPhase ();
		}

		public void LoadContent ()
		{
			if (graphics == null)
			{
				graphics = new Dictionary<string, TitleElement>();
				Texture2D texture;

				texture = ContentLoader.Instance.Load<Texture2D>("Graphics/Client/ObjectivelyRadicalSplash");
				graphics.Add ("splash", new TitleElement(texture, CenterElement (texture)));

				texture = ContentLoader.Instance.Load<Texture2D>("Graphics/Client/TitleBackground");
				graphics.Add ("background", new TitleElement(texture));

				texture = ContentLoader.Instance.Load<Texture2D>("Graphics/Client/TitleText");
				graphics.Add ("title", new TitleElement(texture));

				// Add graphics for the menu
				newGameTexture = ContentLoader.Instance.Load<Texture2D>("Graphics/Client/NewGameButton");
				loadTexture = ContentLoader.Instance.Load<Texture2D>("Graphics/Client/LoadButton");
				exitTexture = ContentLoader.Instance.Load<Texture2D>("Graphics/Client/ExitButton");

				buttonWidth = newGameTexture.Width;
				buttonHeight = newGameTexture.Height;
			}
		}

		public override void Update (GameTime gameTime)
		{
			lua.DoString ("updateCoroutines(" + gameTime.ElapsedGameTime.TotalSeconds + ")");
			foreach (KeyValuePair<string, TitleElement> k in graphics)
			{
				k.Value.Update (gameTime);
			}

			if ((int)phase > 0 && (int)phase < 3 && (KeyboardManager.ButtonPressUp (Microsoft.Xna.Framework.Input.Keys.Enter) || (KeyboardManager.ButtonPressUp (Microsoft.Xna.Framework.Input.Keys.Escape))))
			{
				NextPhase ();
			}

			if (menuShown && MenuInput)
			{
				// Update menu
				if(newGameRect.Contains ((int)MouseManager.X, (int)MouseManager.Y))
				{
					MouseManager.MouseMode = MouseInteractMode.Hot;
					if(MouseManager.LeftClickUp)
					{
						NewGame ();
					}
				}

				if(loadRect.Contains ((int)MouseManager.X, (int)MouseManager.Y))
				{
					MouseManager.MouseMode = MouseInteractMode.Hot;
					if(MouseManager.LeftClickUp)
					{
						LoadGame ();
					}
				}

				if(exitRect.Contains ((int)MouseManager.X, (int)MouseManager.Y))
				{
					MouseManager.MouseMode = MouseInteractMode.Hot;
					if(MouseManager.LeftClickUp)
					{
						Exit ();
					}
				}
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			foreach (KeyValuePair<string, TitleElement> k in graphics)
			{
				k.Value.Draw (spriteBatch);
			}

			// Draw menu and text and whatnot here
			if (menuShown && MenuInput)
			{
				// Draw menu
				Color drawColor = Color.Gray;

				if (newGameRect.Contains ((int)MouseManager.X, (int)MouseManager.Y))
					drawColor = Color.White;
				else
					drawColor = Color.Gray;
				spriteBatch.Draw (newGameTexture, drawRectangle: newGameRect, color: drawColor);

				if (loadRect.Contains ((int)MouseManager.X, (int)MouseManager.Y))
					drawColor = Color.White;
				else
					drawColor = Color.Gray;
				spriteBatch.Draw (loadTexture, drawRectangle: loadRect, color: drawColor);

				if (exitRect.Contains ((int)MouseManager.X, (int)MouseManager.Y))
					drawColor = Color.White;
				else
					drawColor = Color.Gray;
				spriteBatch.Draw (exitTexture, drawRectangle: exitRect, color: drawColor);
			}
			else if(menuShown)
			{
				spriteBatch.Draw (newGameTexture, drawRectangle: newGameRect, color: graphics["background"].currentColor);
				spriteBatch.Draw (loadTexture, drawRectangle: loadRect, color: graphics["background"].currentColor);
				spriteBatch.Draw (exitTexture, drawRectangle: exitRect, color: graphics["background"].currentColor);
			}
		}

		public override void Unload()
		{
		}

		public Vector2 CenterElement(Texture2D tex)
		{
			float X = (Eglantine.GAME_WIDTH - tex.Width) / 2;
			float Y = (Eglantine.GAME_HEIGHT - tex.Height) / 2;

			return new Vector2(X, Y);
		}

		public void NextPhase()
		{
			phase = (MainMenuPhase)((int)phase + 1);
			lua.DoString ("abortAllCoroutines();");
			lua.DoString ("menuPhase = " + (int)phase);
			lua.DoString ("menuEvents[menuPhase]()");

			Console.WriteLine ("Begin phase " + (int)phase);
		}

		public void FadeInElement(string name, float time)
		{
			if(graphics.ContainsKey (name))
				graphics[name].LerpColor (Color.White, time);
		}

		public void FadeOutElement(string name, float time)
		{
			if(graphics.ContainsKey (name))
				graphics[name].LerpColor (Color.Transparent, time);
		}

		public void HideElement (string name)
		{
			if(graphics.ContainsKey (name))
				graphics[name].Hide ();
		}

		public void ShowMenu ()
		{
			menuShown = true;
		}

		public class TitleElement
		{
			Texture2D texture;
			bool lerpingColor;
			public Color currentColor;
			Vector2 position;
			Color startColor;
			Color targetColor;
			float time;
			float lerpDuration;

			public TitleElement(Texture2D tex, Vector2? pos = null)
			{
				texture = tex;

				if(pos == null) position = Vector2.Zero;
				else position = (Vector2)pos;
			}

			public void Update (GameTime gameTime)
			{
				if (lerpingColor)
				{
					time += (float)gameTime.ElapsedGameTime.TotalSeconds;

					currentColor = Color.Lerp (startColor, targetColor, time/lerpDuration);

					if(time > lerpDuration)
					{
						lerpingColor = false;
						currentColor = targetColor;
					}
				}
			}

			public void Draw(SpriteBatch spriteBatch)
			{
				spriteBatch.Draw(texture, position: position, color: currentColor);
			}

			public void LerpColor(Color color, float duration)
			{
				startColor = currentColor;
				targetColor = color;
				lerpDuration = duration;
				time = 0;

				lerpingColor = true;
			}

			public void Hide ()
			{
				lerpingColor = false;
				currentColor = Color.Transparent;
			}
		}

		private enum MainMenuPhase
		{
			Loading,
			SplashScreen,
			Intro,
			Menu
		}

		private void NewGame()
		{
			menuShown = false;
			Console.WriteLine ("NEW GAME");
			lua.DoString ("onStartNewGame()");
		}

		private void LoadGame()
		{
			MenuInput = false;
			menuShown = false;
			SaveManager.ToggleLoadScreen(LoadWindowClose);
		}

		private void LoadWindowClose()
		{
			MenuInput = true;
			menuShown = true;
		}

		private void Exit()
		{
			Eglantine.ExitGame ();
		}
	}
}

