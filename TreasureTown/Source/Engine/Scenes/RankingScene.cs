using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TreasureTown
{
	public class RankingScene : Scene
	{
		int teamCount;
		List<RankedTeam> teams;
		Dictionary<string, Texture2D> graphics;
		SpriteFont scoreFont;

		float time;
		float timeBeforeEscapable = 5f;
		float timeBeforeMainMenu = 30f;

		float yPadding;


		public RankingScene (List<Team> teamsToRank)
		{
			teams = MakeRankedList(teamsToRank);
			teamCount = teams.Count;

			yPadding = 600 / (teamsToRank.Count + 1);


		}

		public override void Initialize()
		{
			LoadContent();
		}

		private void LoadContent()
		{
			graphics = new Dictionary<string, Texture2D>()
			{
				{ "Team1Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team1") },
				{ "Team2Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team2") },
				{ "Team3Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team3") },
				{ "Team4Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team4") },
				{ "Team5Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team5") },
				{ "Team6Name", TreasureTown.StaticContent.Load<Texture2D>("Graphics/GUI/Team6") },

				{ "Rank1", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/1stPlace") },
				{ "Rank2", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/2ndPlace") },
				{ "Rank3", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/3rdPlace") },
				{ "Rank4", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/4thPlace") },
				{ "Rank5", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/5thPlace") },
				{ "Rank6", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/6thPlace") },

				{ "Ranking", TreasureTown.StaticContent.Load<Texture2D>("Graphics/Client/ranking") }
			};

			scoreFont = TreasureTown.StaticContent.Load<SpriteFont>("Fonts/Dustismo24B");
		}

		public override void Update (GameTime gameTime)
		{
			time += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (time > timeBeforeEscapable)
			{
				// Check for input to go back to the main menu
				if(KeyboardManager.ButtonPressUp(Microsoft.Xna.Framework.Input.Keys.Space) || KeyboardManager.ButtonPressUp(Microsoft.Xna.Framework.Input.Keys.Enter))
				{
					TreasureTown.SetScene(new MenuScene());
				}
			}
			if (time > timeBeforeMainMenu)
			{
				TreasureTown.SetScene(new MenuScene());
			}
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(graphics["Ranking"], position: new Vector2(100, 0), color: Color.White);
			float thisY;
			for(int i = 0; i < teams.Count; i++)
			{
				thisY = (i + 1) * yPadding;
				spriteBatch.Draw (graphics["Rank"+(i+1)], position: new Vector2(175, thisY), color: Color.White);
				spriteBatch.Draw (graphics["Team" + teams[i].teamnumber + "Name"], position: new Vector2(250, thisY + 10), color: teams[i].team.TeamColor);

				spriteBatch.DrawString (scoreFont, String.Format ("{0:n0}", teams[i].team.Points), new Vector2(450, thisY + 5), Color.Lerp (Color.White, teams[i].team.TeamColor, .3f));
			}
		}

		public List<RankedTeam> MakeRankedList (List<Team> teamList)
		{
			List<RankedTeam> newTeams = new List<RankedTeam>();
			for(int i = 0; i < teamList.Count; i++)
			{
				newTeams.Add (new RankedTeam(teamList[i], i+1));
			}

			return newTeams;
		}

		public class RankedTeam
		{
			public Team team { get; private set; }
			public int rank { get; private set; }
			public int teamnumber 
			{ 
				get { return team.ID + 1; } 
			}

			public RankedTeam(Team t, int r)
			{
				team = t;
				rank = r;
			}
		}
	}
}

