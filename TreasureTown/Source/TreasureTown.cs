#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using LuaInterface;

#endregion

namespace TreasureTown
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class TreasureTown : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		RenderTarget2D renderTarget;
		static Scene currentScene;
		public static Color bgColor { get; private set; }
		public static ContentManager StaticContent { get; private set; }
		public static Lua MainLua { get; private set; }
		public static Rectangle FullscreenRect { get; private set; }
		public static Random StaticRandom;

		public TreasureTown ()
		{
			StaticContent = Content;
			MainLua = new Lua();
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	  
			StaticRandom = new Random();
			IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// This must be set here instead of in the constructor
			graphics.IsFullScreen = true;

			// Load any lua scripts required at startup

			// TODO: Check to make sure this works...
			FullscreenRect = new Rectangle(0,0,graphics.GraphicsDevice.DisplayMode.Width, graphics.GraphicsDevice.DisplayMode.Height);
			renderTarget = new RenderTarget2D(graphics.GraphicsDevice, 800, 600);

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			GameScene.MapSpriteBatch = new SpriteBatch (GraphicsDevice);
			GameScene.MapTarget = new RenderTarget2D(GraphicsDevice, 600, 600);

			// Load all lua content
			MainLua.DoFile ("Scripts/scheduler.lua");
			MainLua.DoFile ("Scripts/audio_assets.lua");
			MainLua.DoFile ("Scripts/main.lua");
			MainLua.DoFile ("Scripts/items.lua");
			MainLua.DoFile ("Scripts/mapdata.lua");
			AudioManager.Instance.Initialize ();


			Building.LoadBuildingGraphics ();


			// Now set the initial scene!
			SetScene(new MenuScene());
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			AudioManager.Instance.Update (gameTime);
			KeyboardManager.Update (gameTime);
			MouseManager.Update(gameTime);
			MainLua.DoString ("updateCoroutines(" + gameTime.ElapsedGameTime.TotalSeconds + ")");

			if (currentScene != null)
			{
				currentScene.Update (gameTime);
			}

			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.SetRenderTarget(renderTarget);
			spriteBatch.Begin (SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			graphics.GraphicsDevice.Clear (bgColor);


			// Draw the stuff here to the render target
			if(currentScene != null)
				currentScene.Draw (spriteBatch);

			spriteBatch.End ();

			// Now draw that render target to the whole screen.
			GraphicsDevice.SetRenderTarget (null);
			graphics.GraphicsDevice.Clear (bgColor);
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);
			spriteBatch.Draw (renderTarget, drawRectangle: FullscreenRect, color: Color.White);
			spriteBatch.End ();
            
			base.Draw (gameTime);
		}

		public static void SetBackgroundColor(Color newColor)
		{
			bgColor = newColor;
		}

		public static void SetScene(Scene newScene)
		{
			currentScene = newScene;
			currentScene.Initialize();
		}
	}
}

