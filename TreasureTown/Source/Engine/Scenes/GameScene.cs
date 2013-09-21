using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SchedulerTest;

namespace TreasureTown
{
	public class GameScene : Scene
	{

		private static GameScene _instance;
		public static GameScene Instance 
		{
			get { return _instance; }
		}

		public List<GraphicElement> objects;
		public List<Building> buildings;
		public List<ScoreChanger> scoreChanges;
		Navnode startPoint;
		Map map;
		TownGenerator townGenerator;

		public int TeamCount;
		public List<Team> Teams;
		public int CurrentTeam = -1;

		public int ChangeCounter = 0;
		public const int MaxChange = 12;

		public float RemainingTime;
		public float TeamTime;
		public float RemainingTeamTime;

		public bool setupCompleted = false;

		public bool AvatarShown = false;
		public bool CanMove = false;
		private bool teamCountDown = false;
		public Navnode CurrentPosition;
		public float CurrentRotation;
		public MapDirection CurrentDirection;
		public bool GameOver = false;

		private readonly Dictionary<MapDirection, float> Compass;
		public Dictionary<string, Texture2D> Graphics { get; private set; }
		
		Color highlightColor;
		public SpriteFont scoreFont { get; private set; }
		public SpriteFont timeFont { get; private set; }

		public ChangeMode Mode { get; private set; }
		private Texture2D _MapTexture;
		public Texture2D MapTexture 
		{
			get 
			{
				if(_MapTexture == null) 
					return GetMapTexture();
				else
					return _MapTexture;
			}
		}
		public static SpriteBatch MapSpriteBatch;
		public static RenderTarget2D MapTarget;
		public bool MapUsedThisTurn;

		public bool FinalTurn { get; private set; }


		public GameScene (int minutes, int teams = 2, int turnTime = 30)
		{
			_instance = this;

			TreasureTown.SetBackgroundColor(Color.Black);
			objects = new List<GraphicElement>();
			buildings = new List<Building>();
			Teams = new List<Team>();
			scoreChanges = new List<ScoreChanger>();
			Mode = ChangeMode.Normal;

			TeamTime = turnTime;
			TeamCount = teams;

			RemainingTime = minutes * 60;

			Compass = new Dictionary<MapDirection, float>()
			{
				{MapDirection.Right, 0f},
				{MapDirection.Down, (float)Math.PI/2},
				{MapDirection.Left, (float)Math.PI},
				{MapDirection.Up, ((float)Math.PI/2) * 3}
			};
		}

		public override void Initialize()
		{
			map = new Map();
			townGenerator = new TownGenerator(map);

			// Now generate the town!  --REFACTOR THIS!
			GenerateTown();
			objects.Add (new GraphicElement("Graphics/townbase", 0, 0, false, Color.White, 0f, "MAIN"));

			Graphics = new Dictionary<string, Texture2D>();
			Graphics.Add ("Team1Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team1"));
			Graphics.Add ("Team2Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team2"));
			Graphics.Add ("Team3Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team3"));
			Graphics.Add ("Team4Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team4"));
			Graphics.Add ("Team5Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team5"));
			Graphics.Add ("Team6Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team6"));
			Graphics.Add ("TeamHighlight", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/TeamHighlight"));
			Graphics.Add ("MapIcon", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/mapIcon"));
			Graphics.Add ("ChangeBar", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/changebar"));

			Graphics.Add ("MapBase", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/MapBase"));
			Graphics.Add ("BombMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/BombMark"));
			Graphics.Add ("EmptyMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/EmptyMark"));
			Graphics.Add ("MapMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/MapMark"));
			Graphics.Add ("MoneyMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/MoneyMark"));
			Graphics.Add ("SmallMoneyMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/SmallMoneyMark"));
			Graphics.Add ("TreasureMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/TreasureMark"));
			Graphics.Add ("ThiefMark", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Map/ThiefMark"));

			Graphics.Add ("Explosion", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Explosion"));


			scoreFont = TreasureTown.StaticContent.Load<SpriteFont>("Fonts/Dustismo24B");
			timeFont = TreasureTown.StaticContent.Load<SpriteFont>("Fonts/Dustismo18");


			// Setup
			SetupTeams ();
			EventManager.KillMusic();
			EventManager.PlaySong ("TreasureHunt");
			Scheduler.Execute(Scripts.nextTeam);
		}

		public override void Update (GameTime gameTime)
		{
			if(!setupCompleted)
				return;

			float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (!GameOver)
			{
				RemainingTime -= deltaTime;
				if (RemainingTime <= 0)
					EndGame ();
			}

			if (teamCountDown)
			{
				RemainingTeamTime -= deltaTime;
				if (RemainingTeamTime <= 0)
					TimeOver ();
			}

			// Check for the end of the game or turn here.
			if (RemainingTime <= 0)
			{
				FinalTurn = true;
			}

			float alpha = (float)(Math.Sin (gameTime.TotalGameTime.TotalSeconds * 7) + 1) / 2;
			highlightColor = new Color (alpha, alpha, alpha, alpha);

			foreach (GraphicElement g in objects)
			{
				g.Update (gameTime);
			}

			scoreChanges.RemoveAll (x => x.finished);

			foreach (ScoreChanger s in scoreChanges)
			{
				s.Update (gameTime);
			}

			if(AvatarShown && CanMove)
				InputUpdate();
		}

		public void InputUpdate ()
		{
			if (KeyboardManager.ButtonPressUp (Microsoft.Xna.Framework.Input.Keys.Up))
			{
				Scheduler.Execute(Scripts.goStraight);
			}
			else if (KeyboardManager.ButtonPressUp (Microsoft.Xna.Framework.Input.Keys.Left))
			{
				Scheduler.Execute(Scripts.turnLeft);
			}
			else if (KeyboardManager.ButtonPressUp (Microsoft.Xna.Framework.Input.Keys.Right))
			{
				Scheduler.Execute(Scripts.turnRight);
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			if(!setupCompleted)
				return;

			foreach (GraphicElement g in objects)
			{
				g.Draw (spriteBatch);
			}

			foreach (Building b in buildings)
			{
				b.Draw (spriteBatch);
			}

			foreach (ScoreChanger s in scoreChanges)
			{
				s.Draw (spriteBatch);
			}

			for (int i = Teams.Count; i > 0; i--)
			{
				spriteBatch.Draw (Graphics ["Team" + i + "Name"], position: new Vector2 (613, 518 - 65 * (Teams.Count - i)), color: Teams [i - 1].TeamColor, depth: .5f);
				if (Teams [i - 1].HasMap)
					spriteBatch.Draw (Graphics ["MapIcon"], position: new Vector2 (585, 548 - 65 * (Teams.Count - i)), color: Color.White);
				
				spriteBatch.DrawString (scoreFont, String.Format ("{0:n0}", (int)Teams [i - 1].Points), new Vector2 (630, 548 - 65 * (Teams.Count - i)), Color.Lerp (Color.White, Teams [i - 1].TeamColor, .2f));

				if(FinalTurn)
				{
					spriteBatch.DrawString (timeFont, String.Format ("00:00", (int)(RemainingTime/60), (int)(RemainingTime % 60)), new Vector2(683,56), Color.Red);
				}
				else
				{
					spriteBatch.DrawString (timeFont, String.Format ("{0:00}:{1:00}", (int)(RemainingTime/60), (int)(RemainingTime % 60)), new Vector2(683,56), Color.White);
				}



				if (Teams [i - 1] == Teams [CurrentTeam])
					spriteBatch.Draw (Graphics ["TeamHighlight"], position: new Vector2 (613, 518 - 65 * (Teams.Count - i)), color: highlightColor, depth: .4f);
			}

			if(teamCountDown)
				spriteBatch.DrawString (scoreFont, String.Format ("{0:00.0}", RemainingTeamTime), new Vector2 (673,14), Color.White);

			if (ChangeCounter > 0)
			{
				spriteBatch.Draw (Graphics["ChangeBar"], position: new Vector2(622, 111), color: Color.White, sourceRectangle: GetChangeRect());
			}

		}

		public void GenerateTown()
		{
			// Take into account the cool stuff too
			townGenerator.GenerateTown (ref buildings, ref startPoint);
		}


		public void SetupTeams ()
		{
			for (int i = 0; i < TeamCount; i++)
			{
				Teams.Add (new Team());
			}
		}

		public void GetNextTeam ()
		{
			CurrentTeam++;
			if (CurrentTeam >= Teams.Count)
			{
				CurrentTeam = 0;
			}

			if(!setupCompleted)
				setupCompleted = true;
		}


		public void DestroyGraphic (string name)
		{
			objects.RemoveAll (x=>x.Name == name);
		}

		public void EndTurn ()
		{
			// First, destroy the player avatar if it is still displayed.
			
			if (AvatarShown)
			{
				AvatarShown = false;
				DestroyGraphic ("PLAYER");
			}

			// A map was used this turn, so add some Change and then shuffle the remaining items about
			if (MapUsedThisTurn)
			{
				MapUsedThisTurn = false;
				EventManager.AddChange (2);

				townGenerator.ShuffleRemainingItems(ref buildings);
			}
		}

		public void StartTurn ()
		{
			CurrentPosition = startPoint;
			GraphicElement player = new GraphicElement("Graphics/player", CurrentPosition.Position.X, CurrentPosition.Position.Y, false, Teams[CurrentTeam].TeamColor, .75f, "PLAYER");
			CurrentDirection = CurrentPosition.GetExitDirection();
			player.SetRotation (Compass[CurrentDirection]);
			player.SetOrigin (new Vector2(14, 31));
			player.SetScale (new Vector2(.7f, .7f));
			objects.Add (player);

			AvatarShown = true;
			RemainingTeamTime = TeamTime;
			teamCountDown = true;
			CanMove = true;
		}


		public void GoStraight ()
		{
			if (CurrentPosition.Neighbors.ContainsKey (CurrentDirection))
			{
				objects.Find (x => x.Name == "PLAYER").LerpPosition (CurrentPosition.GetNeighborPosition(CurrentDirection), 1f);
				CurrentPosition = CurrentPosition.Neighbors [CurrentDirection];
			}
			else
			{
				Console.WriteLine ("No node found!");
				// Error beeping!

				// Raise the signal to keep the lua side moving forward
				EventManager.SendSignal("PLAYER position lerped");
			}
		}

		public void TurnLeft()
		{
			objects.Find (x=>x.Name == "PLAYER").LerpChangeRotation (-(float)Math.PI/2, .5f);
			ModifyDirection (-1);
		}


		public void TurnRight()
		{
			objects.Find (x=>x.Name == "PLAYER").LerpChangeRotation ((float)Math.PI/2, .5f);
			ModifyDirection (1);
		}


		public void ModifyDirection (int change)
		{
			int dir = ((int)CurrentDirection) + change;
			if (dir < 0)
			{
				dir = 3;
			}
			else if (dir > 3)
			{
				dir = 0;
			}
			CurrentDirection = (MapDirection)dir;
		}

		public void Explore ()
		{
			CurrentPosition.Building.Explore ();
		}

		public void ChangePoints(Team team, int change)
		{
			Vector2 pos = new Vector2(710, 548 - 65*((Teams.Count - 1) - GetTeamIndex (team)));
			scoreChanges.Add (new ScoreChanger(team, pos, change, 2f));
			Scheduler.Execute(Scripts.delayPointScroll);
		}

		public int GetTeamIndex(Team team)
		{
			Console.WriteLine ("TEAM INDEX IS" + team);
			for(int i = 0; i < Teams.Count; i++)
			{
				if(Teams[i] == team) return i;
			}


			return -1;
		}

		public void BeginScrollingPointChangers ()
		{
			// Get the net change in points
			int total = 0;
			foreach (ScoreChanger s in scoreChanges)
			{
				s.BeginScrolling ();
				total += (int)s.totalChange;
			}

			if (total >= 0)
			{
				EventManager.PlaySoundEffect("gainpoints", 1f);
			}
			else
			{
				EventManager.PlaySoundEffect("losepoints", 1f);
			}
		}

		public GraphicElement CreateGraphic(string textureName, int positionX, int positionY, string name)
		{
			GraphicElement element = new GraphicElement(textureName, positionX, positionY, false, Color.Transparent, 1f, name);
			objects.Add (element);
			return element;
		}

		public void CreateGraphic(Texture2D texture, int positionX, int positionY, string name)
		{
			objects.Add (new GraphicElement(texture, positionX, positionY, false, Color.Transparent, 1f, name));
		}

		public GraphicElement GetGraphic(string name)
		{
			return objects.Find (x => x.Name == name);
		}

		public Rectangle GetChangeRect ()
		{
			int units;
			int width;
			int height;

			height = Graphics ["ChangeBar"].Height;
			if (ChangeCounter < MaxChange)
			{
				units = Math.Min (MaxChange, ChangeCounter);
				width = Graphics ["ChangeBar"].Width / MaxChange;
				width *= units;
			}
			else
			{
				width = Graphics ["ChangeBar"].Width;
			}

			return new Rectangle(0,0, width, height);
		}

		public void AddChange (int change)
		{
			ChangeCounter += change;
			if (ChangeCounter > MaxChange)
				ChangeCounter = MaxChange;
		}

		public void ResetChange()
		{
			ChangeCounter = 0;
		}

		public string GetChangeMode()
		{
			Console.WriteLine ("CHANGE MODE INVOKED!");

			Mode = (ChangeMode)TreasureTown.StaticRandom.Next (0, Enum.GetValues (typeof(ChangeMode)).Length);
			return Mode.ToString ();
		}

		public bool AllBuildingsExplored ()
		{
			foreach (Building b in buildings)
			{
				if(!b.explored)
					return false;
			}

			return true;
		}

		public Texture2D GetMapTexture ()
		{
			MapSpriteBatch.GraphicsDevice.SetRenderTarget (MapTarget);

			MapSpriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);

			MapSpriteBatch.Draw (Graphics ["MapBase"], position: Vector2.Zero, color: Color.White);

			Vector2 pos = Vector2.Zero;
			Texture2D mark = null;
			foreach (Building b in buildings)
			{
				pos = b.Position - new Vector2(20,64);
				if(b.item == null)
					mark = Graphics["EmptyMark"];
				else
				{
					switch(b.item.Type)
					{
					case ItemType.Points:
						if(b.item.value >= 1500)
							mark = Graphics["TreasureMark"];
						else if(b.item.value >= 1001)
							mark = Graphics["MoneyMark"];
						else
							mark = Graphics["SmallMoneyMark"];
						break;
					case ItemType.Bomb:
						mark = Graphics["BombMark"];
						break;
					case ItemType.Thief:
						mark = Graphics["ThiefMark"];
						break;
					case ItemType.Map:
						mark = Graphics["MapMark"];
						break;
					}
				}

				MapSpriteBatch.Draw (mark, position: pos, color: Color.White);
			}


			MapSpriteBatch.End ();

			MapSpriteBatch.GraphicsDevice.SetRenderTarget (null);

			_MapTexture = (Texture2D)MapTarget;
			return _MapTexture;
		}

		public void TimeOver()
		{
			PauseTeamTime();
			Scheduler.Execute(Scripts.endTurn);
		}

		public void PauseTeamTime()
		{
			teamCountDown = false;
		}


		public void EndGame()
		{
			GameOver = true;
			// Call the whistle and end the game!
		}

		public List<Team> RankTeams ()
		{
			Teams.Sort((x, y) => y.Points.CompareTo(x.Points));
			return Teams;
		}
	}

	public enum ChangeMode
	{
		Normal,
		Danger,
		Greed,
		CrimeWave
	}
}

