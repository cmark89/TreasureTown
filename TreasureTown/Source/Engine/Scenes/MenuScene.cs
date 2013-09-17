using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreasureTown
{
	public class MenuScene : Scene
	{
		// Phase = 0; Preload
		// Phase = 1; Splash screen
		// Phase = 2; Menu
		// Phase = 3; Choose number of teams
		// Phase = 4; Choose minutes

		private static MenuScene _instance;
		public static MenuScene Instance {
			get 
			{
				if(_instance != null) 
					return _instance;
				else 
				{
					Console.WriteLine("Attempting to get null MenuScene instance.");
					return null;
				}
			}
		}

		int phase = 0;		
		Dictionary<string, Texture2D> graphics;
		SpriteFont font;
		List<GraphicElement> elements;
		List<MenuButton> menuButtons;
		delegate void buttonclick(int i);
		int numberOfTeams;
		int minutes;
		bool finalCountdown = false;
		float gameStartInSeconds;
		bool clicked = false;

		public MenuScene ()
		{
			_instance = this;
		}

		public override void Initialize ()
		{
			LoadContent();
			phase = 1;
			elements = new List<GraphicElement>();
			menuButtons = new List<MenuButton>();
			ShowSplashScreen();
		}

		public void LoadContent()
		{
			TreasureTown.MainLua.DoFile ("Scripts/mainMenu.lua");

			graphics = new Dictionary<string, Texture2D>()
			{
				{ "ORGSplash", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/ObjectivelyRadicalSplash") },
				{ "Title", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/Title") },

				{ "2Teams", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/2teams") },
				{ "3Teams", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/3teams") },
				{ "4Teams", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/4teams") },
				{ "5Teams", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/5teams") },
				{ "6Teams", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/6teams") },

				{ "10Minutes", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/10minutes") },
				{ "15Minutes", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/15minutes") },
				{ "20Minutes", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/20minutes") },
				{ "30Minutes", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/30minutes") }
			};

			font = TreasureTown.StaticContent.Load<SpriteFont>("Fonts/Dustismo18");
		}

		public override void Update (GameTime gameTime)
		{
			if(clicked)
				clicked = false;

			foreach (GraphicElement e in elements)
				e.Update (gameTime);

			for (int i = 0; i < menuButtons.Count; i++)
			{
				if (menuButtons [i] != null)
					menuButtons [i].Update (gameTime);
			}

			InputUpdate ();

			if (finalCountdown)
			{
				gameStartInSeconds -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				if(gameStartInSeconds <= 0)
				{
					TreasureTown.SetScene(new GameScene(minutes, numberOfTeams, 45));
				}
			}
		}

		void InputUpdate()
		{
			if(KeyboardManager.ButtonPressUp(Microsoft.Xna.Framework.Input.Keys.Space) || KeyboardManager.ButtonPressUp(Microsoft.Xna.Framework.Input.Keys.Enter))
			{
				if(phase == 1)
				{
					TreasureTown.MainLua.DoString("cancelSplashScreen()");
				}
				else if(phase == 2)
				{
					EventManager.PlaySoundEffect("boxring");
					ShowTeamSelect();
				}
				// Receiving input, so let's go to the next phase!
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			foreach(GraphicElement e in elements)
				e.Draw(spriteBatch);
			foreach(MenuButton b in menuButtons)
				b.Draw(spriteBatch);
		}

		public void CreateGraphic(Texture2D texture, int positionX, int positionY, string name)
		{
			elements.Add (new GraphicElement(texture, positionX, positionY, false, Color.Transparent, 1f, name));
		}

		public void CreateGraphic(string textureName, int positionX, int positionY, string name)
		{
			elements.Add (new GraphicElement(TreasureTown.StaticContent.Load<Texture2D>(textureName), positionX, positionY, false, Color.Transparent, 1f, name));
		}

		public void DestroyGraphic(string s)
		{
			elements.RemoveAll(x=> x.Name == s);
		}

		public GraphicElement GetGraphic(string name)
		{
			return elements.Find (x => x.Name == name);
		}


		public void ShowSplashScreen ()
		{
			phase = 1;
			Console.WriteLine("ShowSplashScreen()");
			Texture2D splash = graphics["ORGSplash"];
			CreateGraphic(splash, (800 - splash.Width) /2, (600 - splash.Height) / 2, "SPLASH");
			TreasureTown.MainLua.DoString ("showSplash()");
		}

		public void ShowMenu()
		{
			phase = 2;
			CreateGraphic(graphics["Title"], 0, 0, "TITLE");
			EventManager.MenuLerpColor("TITLE", 1f,1f,1f,1f, 3f);
			EventManager.PlaySong("PointsOrDie");
		}

		public void ShowTeamSelect ()
		{
			int width = graphics["2Teams"].Width / 2;
			phase = 3;
			EventManager.MenuLerpColor("TITLE", .5f, .5f, .5f, .5f, 1f);
			GetGraphic("TITLE").SetDepth(.2f);

			menuButtons.Clear();
			menuButtons.Add(new MenuButton(graphics["2Teams"], Origin + new Vector2(-200 - width, 0), 2, SetTeams));
			menuButtons.Add(new MenuButton(graphics["3Teams"], Origin + new Vector2(-100 - width, 0), 3, SetTeams));
			menuButtons.Add(new MenuButton(graphics["4Teams"], Origin + new Vector2(0 - width, 0), 4, SetTeams));
			menuButtons.Add(new MenuButton(graphics["5Teams"], Origin + new Vector2(100 - width, 0), 5, SetTeams));
			menuButtons.Add(new MenuButton(graphics["6Teams"], Origin + new Vector2(200 - width, 0), 6, SetTeams));
		}

		public void ShowTimeSelect()
		{
			int width = graphics["2Teams"].Width / 2;
			phase = 4;

			// Adjust the hell out of this
			menuButtons.Clear();
			menuButtons.Add(new MenuButton(graphics["10Minutes"], Origin + new Vector2(-75-width, -50), 10, SetTime));
			menuButtons.Add(new MenuButton(graphics["15Minutes"], Origin + new Vector2(75-width, -50), 15, SetTime));
			menuButtons.Add(new MenuButton(graphics["20Minutes"], Origin + new Vector2(-75-width, 50), 20, SetTime));
			menuButtons.Add(new MenuButton(graphics["30Minutes"], Origin + new Vector2(75-width, 50), 30, SetTime));
		}

		public void SetTeams (int num)
		{
			EventManager.PlaySoundEffect("boxring");
			numberOfTeams = num;
			ShowTimeSelect();
		}

		public void SetTime (int num)
		{
			EventManager.PlaySoundEffect("boxring");
			minutes = num;
			FadeOutAllGraphics();
		}

		public void FadeOutAllGraphics ()
		{
			EventManager.PlaySoundEffect("gamestart");
			AudioManager.Instance.FadeMusic(0f, 2f);
			foreach (GraphicElement e in elements)
			{
				e.LerpColor(0,0,0,0,3);
			}
			foreach (MenuButton b in menuButtons)
			{
				b.SetColor(new Color(0f, 0f, 0f, 0f));
			}
			finalCountdown = true;
			gameStartInSeconds = 3f;
		}

		public Vector2 Origin 
		{
			get { return new Vector2 (800 / 2, 600 / 2); }
		}


		class MenuButton
		{
			Texture2D texture;
			Vector2 position;
			int value;
			buttonclick onClick;
			Color color;

			public MenuButton(Texture2D tex, Vector2 pos, int val, buttonclick clickevent)
			{
				texture = tex;
				position = pos;
				value = val;
				onClick = clickevent;
				color = Color.White;
			}

			public void Update (GameTime gameTime)
			{
				if(MenuScene.Instance.clicked)
					return;

				if (MouseManager.LeftClickUp && MouseManager.MouseInRect (new Rectangle ((int)position.X, (int)position.Y, texture.Width, texture.Height)))
				{
					MenuScene.Instance.clicked = true;
					onClick(value);
				}
			}

			public void Draw(SpriteBatch spriteBatch)
			{
				spriteBatch.Draw(texture, position: position, color: color, depth: 1f);
			}

			public void SetColor(Color newColor)
			{
				color = newColor;
			}
		}
	}
}

