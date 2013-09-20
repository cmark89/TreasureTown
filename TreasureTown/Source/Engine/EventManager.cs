using System;
using SchedulerTest;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TreasureTown
{
	public static class EventManager
	{
		public static void Initialize()
		{	
		}

		public static void GoStraight ()
		{
			Console.WriteLine ("EventManager:GoStraight()");
			GameScene.Instance.GoStraight();
		}

		public static void TurnLeft()
		{
			Console.WriteLine ("EventManager:TurnLeft()");
			GameScene.Instance.TurnLeft();
		}

		public static void TurnRight()
		{
			Console.WriteLine ("EventManager:TurnRight()");
			GameScene.Instance.TurnRight();
		}

		public static void SendSignal(string signal)
		{
			Console.WriteLine("Send signal: " + signal);

			if(GameScene.Instance != null)
				SchedulerTest.Scheduler.SendSignal(signal);
		}

		public static void GetNextTeam()
		{
			GameScene.Instance.GetNextTeam();
		}

		public static void DisableInput()
		{
			GameScene.Instance.CanMove = false;
		}

		public static void EnableInput()
		{
			GameScene.Instance.CanMove = true;
		}

		public static void CheckIfExploring ()
		{
			if (GameScene.Instance.CurrentPosition.IsBuildingSpawn)
			{
				GameScene.Instance.PauseTeamTime();
				GameScene.Instance.Explore();
			}
			else
			{
				EnableInput ();
			}
		}

		public static void StartTurn()
		{
			GameScene.Instance.StartTurn ();
		}

		public static void ChangePoints(Team team, int change)
		{
			GameScene.Instance.ChangePoints(team, change);
		}

		public static Team CurrentTeam()
		{
			return GameScene.Instance.Teams[GameScene.Instance.CurrentTeam];
		}

		public static Team TeamWithHighestPoints ()
		{
			Team highest = null;
			foreach (Team t in GameScene.Instance.Teams)
			{
				if(t == CurrentTeam ()) continue;

				if(highest == null || t.Points > highest.Points)
					highest = t;
			}

			return highest;
		}

		public static Team TeamWithLowestPoints ()
		{
			Team lowest = null;
			foreach (Team t in GameScene.Instance.Teams)
			{
				if(t == CurrentTeam ()) continue;
				
				if(lowest == null || t.Points < lowest.Points)
					lowest = t;
			}
			
			return lowest;
		}

		public static Team RandomOtherTeam()
		{
			return null;
		}

		public static float GetBombPenalty ()
		{
			int flatPenalty;
			float percent;
			if (GameScene.Instance.Mode == ChangeMode.Danger)
			{
				percent = .7f;
				flatPenalty = 400;
			}
			else
			{
				percent = .4f;
				flatPenalty = 150;
			}

			return -Math.Min ((CurrentTeam ().Points * percent) + flatPenalty, CurrentTeam ().Points);
		}

		public static void Steal (Team thief, Team victim)
		{
			if (GameScene.Instance.Mode == ChangeMode.CrimeWave)
			{
				SuperSteal();
				return;
			}
			float minSteal;
			float maxSteal;

			// If the victim has more points than the thief, the max steal is higher
			if (victim.Points > thief.Points)
			{
				if(victim.Points >= thief.Points * 3)
				{
					maxSteal = 1f;
					minSteal = .8f;
				}
				else if(victim.Points >= thief.Points * 2)
				{
					maxSteal = .9f;
					minSteal = .6f;
				}
				else
				{
					maxSteal = .75f;
					minSteal = .5f;
				}
			}
			else
			{
				if(victim.Points * 2 <= thief.Points)
				{
					maxSteal = .2f;
					minSteal = .05f;
				}
				else
				{
					maxSteal = .5f;
					minSteal = .25f;
				}
			}

			// Given those min and max values, find a random number rounded to the nearest .05
			float actualSteal = (float)TreasureTown.StaticRandom.NextDouble () * (maxSteal - minSteal) + minSteal;

			actualSteal = (float)Math.Ceiling (actualSteal * 20) / 20;
			actualSteal *= victim.Points;

			ChangePoints (thief, (int)actualSteal);
			ChangePoints (victim, -(int)actualSteal);
		}

		public static void ScrollPoints()
		{
			Console.WriteLine ("SCROLL POINTS");
			GameScene.Instance.BeginScrollingPointChangers();
		}

		public static GraphicElement CreateGraphic(string textureName, int positionX, int positionY, string name)
		{
			return GameScene.Instance.CreateGraphic(textureName, positionX, positionY, name);
		}

		public static void MenuCreateGraphic(string textureName, int positionX, int positionY, string name)
		{
			MenuScene.Instance.CreateGraphic(textureName, positionX, positionY, name);
		}

		public static void CreateBannerGraphic(string textureName, int positionX, int positionY, string name)
		{
			GameScene.Instance.CreateGraphic(textureName, positionX, positionY, name);
			GameScene.Instance.GetGraphic (name).SetColor (CurrentTeam ().TeamColor);
		}

		public static int GetCurrentTeamIndex ()
		{
			return GameScene.Instance.GetTeamIndex(CurrentTeam ());
		}

		public static Color GetCurrentTeamColor ()
		{
			return CurrentTeam ().TeamColor;
		}

		public static void DestroyGraphic(string textureName)
		{
			GameScene.Instance.DestroyGraphic(textureName);
		}

		public static void LerpColor(string item, float r, float g, float b, float a, float time)
		{
			GameScene.Instance.GetGraphic(item).LerpColor (r, g, b, a, time);
		}

		public static void LerpPosition(string item, float x, float y, float time)
		{
			GameScene.Instance.GetGraphic(item).LerpPosition (new Vector2(x, y), time);
		}

		public static void MenuDestroyGraphic(string textureName)
		{
			MenuScene.Instance.DestroyGraphic(textureName);
		}
		
		public static void MenuLerpColor(string item, float r, float g, float b, float a, float time)
		{

			MenuScene.Instance.GetGraphic(item).LerpColor(r,g,b,a,time);
		}
		
		public static void MenuLerpPosition(string item, float x, float y, float time)
		{
			MenuScene.Instance.GetGraphic(item).LerpPosition (new Vector2(x, y), time);
		}

		public static void SetCurrentTeamMap(bool hasMap)
		{
			CurrentTeam ().HasMap = hasMap;
		}

		public static void AddChange(int change)
		{
			GameScene.Instance.AddChange(change);
		}

		public static void ResetChange()
		{
			GameScene.Instance.ResetChange ();
		}

		public static bool ChangeFilled()
		{
			return (GameScene.Instance.ChangeCounter >= GameScene.MaxChange ||
			        GameScene.Instance.AllBuildingsExplored());
		}

		public static string GetChangeMode()
		{
			string s = GameScene.Instance.GetChangeMode();
			Console.WriteLine (s);
			return s;
		}

		public static void GenerateTown()
		{
			GameScene.Instance.GenerateTown ();
		}

		public static string CurrentMode()
		{
			return GameScene.Instance.Mode.ToString ();
		}

		public static void Write(string s)
		{
			Console.WriteLine (s);
		}

		public static void PlaySong(string songName)
		{
			AudioManager.Instance.PlaySong (songName, .75f, true);
		}

		public static void PlaySoundEffect(string sfxName, float volume = 1f)
		{
			AudioManager.Instance.PlaySoundEffect(sfxName, volume, 0f, 0f);
		}

		public static void FadeOutMusic(float time)
		{
			AudioManager.Instance.FadeMusic (0f, time);
		}

		public static void KillMusic()
		{
			AudioManager.Instance.StopMusic ();
		}

		public static void CreateMapGraphic(int x, int y, string name)
		{
			GameScene.Instance.MapUsedThisTurn = true;

			// First generate the map
			GameScene.Instance.GetMapTexture();
			GameScene.Instance.CreateGraphic(GameScene.Instance.MapTexture, x, y, name);
		}

		public static bool CurrentTeamHasMap()
		{
			return CurrentTeam ().HasMap;
		}

		public static void EndTurn()
		{
			GameScene.Instance.EndTurn ();
		}

		public static void CreateExplosionGraphic()
		{
			// Current position
			Point pos = GameScene.Instance.CurrentPosition.Position;
			Vector2 currentPos = new Vector2(pos.X, pos.Y);
			// Create the explosion at the curent position, offset by half the explosion texture.
			GraphicElement explosion = CreateGraphic ("Graphics/GUI/Explosion", (int)currentPos.X, (int)currentPos.Y, "EXPLOSION");
			explosion.SetOrigin(new Vector2(175,175));
			explosion.SetScale (.1f);
		}

		public static void LerpScale(string name, float toScale, float duration)
		{
			GameScene.Instance.GetGraphic (name).LerpScale (new Vector2(toScale, toScale), duration);
		}

		public static void MenuShowTitle ()
		{
			MenuScene.Instance.ShowMenu();
		}

		public static void StartGame (int teams, int time)
		{
			TreasureTown.SetScene(new GameScene(time, teams, 45));
		}

		public static bool CheckEndOfGame ()
		{
			if (GameScene.Instance.FinalTurn)
			{
				Scheduler.AbortAllCoroutines();
				Scheduler.Execute(Scripts.endGame);
				return true;
			}
			else
				return false;
		}

		public static void GoToResultsScene()
		{
			TreasureTown.SetScene(new RankingScene(GameScene.Instance.RankTeams()));
		}

		public static void SuperSteal ()
		{
			int roll = TreasureTown.StaticRandom.Next (1, 11);
			Team thief = CurrentTeam ();
			// First determine which version of the super steal to do...


			if (thief == TeamWithHighestPoints())
			{
				int totalToSteal = 0;
				// The highest point team has found the thief
				if(roll < 4)
				{
					// Steal an insignificant number of points from all teams
					foreach(Team t in GameScene.Instance.Teams)
					{
						if(t == CurrentTeam ()) continue;

						int steal = Math.Min (5, (int)t.Points);
						totalToSteal += steal;
						
						ChangePoints (t, -(int)steal);
					}
					ChangePoints (thief, totalToSteal);
				}
				else if(roll >= 4)
				{
					// Steal all points from the thief
					int pointTotal = (int)thief.Points;
					ChangePoints (thief, -pointTotal);

					if(roll >= 9 || pointTotal >= TeamWithLowestPoints().Points * 3)
					{
						// Split the points between all other teams
						foreach(Team t in GameScene.Instance.Teams)
						{
							if(t == thief) continue;

							ChangePoints (t, pointTotal / GameScene.Instance.Teams.Count - 1);
						}
					}
				}
			}
			else if (thief == TeamWithLowestPoints())
			{
				// The lowest point team has found the thief
				
				// A high chance to steal from everyone 
				if(roll < 8)
				{
					int totalToSteal = 0;
					foreach(Team t in GameScene.Instance.Teams)
					{
						if(t == CurrentTeam ()) continue;
						
						int steal = (int)(t.Points * .25f);
						totalToSteal += steal;
						
						ChangePoints (t, -(int)steal);
					}
					ChangePoints (thief, totalToSteal);
				}
				else
				{
					int totalToSteal = 0;
					// Steal huge points from the top 2 teams
					List<Team> victims = new List<Team>(GameScene.Instance.RankTeams());
					for(int i = 2; i < victims.Count; i++)
					{
						if(victims[i] != null)
							victims.RemoveAt(i);
					}

					if(roll < 10)
					{
						// Steal half of points
						foreach(Team t in victims)
						{
							if(t == thief) continue;
							totalToSteal += (int)(t.Points * .5f);
							ChangePoints(t, (int)(-t.Points * .5f));
						}

						ChangePoints (thief, totalToSteal);
					}
					else
					{
						// Steal ALL points
						foreach(Team t in victims)
						{
							if(t == thief) continue;
							totalToSteal += (int)t.Points;
							ChangePoints(t, -(int)t.Points);
						}

						ChangePoints (thief, totalToSteal);
					}
				}

				// Also give the thief a map if they don't have one, just to be super nice.  
				// Reversal victory time!
				if(!thief.HasMap)
					thief.HasMap = true;
			}
			else
			{
				// Someone else has found the thief.  Roll...
				if(roll < 6)
				{
					// Just steal regularly steal
					Steal (thief, TeamWithHighestPoints());
				}
				else
				{
					int totalToSteal = 0;

					// Steal a smaller portion of points from everyone!
					foreach(Team t in GameScene.Instance.Teams)
					{
						if(t == CurrentTeam ()) continue;

						float maxSteal = .2f;
						float minSteal = .1f;
				
						// Given those min and max values, find a random number rounded to the nearest .05
						float actualSteal = (float)TreasureTown.StaticRandom.NextDouble () * (maxSteal - minSteal) + minSteal;
					
						actualSteal = (float)Math.Ceiling (actualSteal * 20) / 20;
						actualSteal *= t.Points;
						totalToSteal += (int)actualSteal;

						ChangePoints (t, -(int)actualSteal);
					}

					ChangePoints (thief, totalToSteal);
				}
			}
		}
	}
}

