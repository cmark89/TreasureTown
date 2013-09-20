using System;
using System.Collections.Generic;
using SchedulerTest;

namespace TreasureTown
{
	public static class Scripts
	{
		private static PauseScript waitSeconds (float time)
		{
			return new PauseScript(time);
		}

		private static PauseScript waitUntil(string s)
		{
			return new PauseScript(s);
		}


		#region items.lua
		public static IEnumerator<PauseScript> findPoints(int value)
		{
			string graphicToShow = "";
			if(value < 1000)
				graphicToShow = "SmallMoney";
			else if(value < 1500)
				graphicToShow = "Money";
			else if(value >= 1500)
				graphicToShow = "Treasure";

			Scheduler.ExecuteWithArgs<string>(showGetGraphic, graphicToShow);

			int change = 0;

			if(value > 2500)
				change = 3;
			else if(value >= 1500)
				change = 2;
			else if(value >= 1000)
				change = 1;

			yield return waitSeconds(3.5f);
			EventManager.AddChange(change);
			EventManager.ChangePoints(EventManager.CurrentTeam(), value);
			yield return waitUntil("Points changed");
			Scheduler.Execute(endTurn);
		}

		public static IEnumerator<PauseScript>  findBomb(int value = 0)
		{
			Scheduler.ExecuteWithArgs<string>(showGetGraphic, "Bomb");
			yield return waitSeconds(3.5f);
			EventManager.ChangePoints(EventManager.CurrentTeam(), (int)EventManager.GetBombPenalty());
			yield return waitSeconds(0.5f);
			EventManager.AddChange(2);
			Scheduler.Execute(showExplosion);
			yield return waitUntil("Points changed");
			Scheduler.Execute(endTurn);
		}

		public static IEnumerator<PauseScript> findThief(int value = 0)
		{
			Scheduler.ExecuteWithArgs<string>(showGetGraphic, "Thief");
			yield return waitSeconds(3.5f);
			EventManager.Steal(EventManager.CurrentTeam(), EventManager.TeamWithHighestPoints());

			int change = 4;
			if(EventManager.CurrentMode() == "Greed")
				change = 0;
							
			EventManager.AddChange(change);
			yield return waitUntil("Points changed");
			Scheduler.Execute(endTurn);
		}

		public static IEnumerator<PauseScript> findMap(int value = 0)
		{
			Scheduler.ExecuteWithArgs<string>(showGetGraphic, "Map");
			yield return waitSeconds(3.5f);
			EventManager.SetCurrentTeamMap(true);
			Scheduler.Execute(endTurn);
		}

		public static IEnumerator<PauseScript> findEmpty()
		{
			Scheduler.Execute(endTurn);
			yield return null;
		}

		public static IEnumerator<PauseScript> showGetGraphic(string name)
		{
			EventManager.CreateGraphic("Graphics/GUI/get" + name, 0, 0, name);
			EventManager.LerpColor(name, 1, 1, 1, 1, 1);
			yield return waitSeconds(3);

			EventManager.LerpColor(name, 0,0,0,0,.5f);
			yield return waitSeconds(.5f);

			EventManager.DestroyGraphic(name);
			EventManager.SendSignal("Get graphic shown");
		}


		#endregion



		#region main.lua

		public static IEnumerator<PauseScript> goStraight()
		{
			EventManager.DisableInput();
			EventManager.GoStraight();
			yield return waitUntil("PLAYER position lerped");
			EventManager.CheckIfExploring();
		}

		public static IEnumerator<PauseScript> turnLeft()
		{
			EventManager.DisableInput();
			EventManager.TurnLeft();
			yield return waitUntil("PLAYER rotation lerped");
			EventManager.EnableInput();
		}

		public static IEnumerator<PauseScript> turnRight()
		{
			EventManager.DisableInput();
			EventManager.TurnRight();
			yield return waitUntil("PLAYER rotation lerped");
			EventManager.EnableInput();
		}

		public static IEnumerator<PauseScript> nextTeam()
		{
			EventManager.GetNextTeam();
				
			EventManager.CreateBannerGraphic("Graphics/GUI/team" + (EventManager.GetCurrentTeamIndex() + 1) + "banner", -600, 0, "TeamBanner");

			EventManager.PlaySoundEffect("whoosh", .7f);
			EventManager.LerpPosition("TeamBanner", 0, 0, .5f);
			yield return waitSeconds(2.25f);
			EventManager.LerpPosition("TeamBanner", 800, 0, .75f);
			yield return waitSeconds(.5f);
			EventManager.DestroyGraphic("TeamBanner");

			if (EventManager.CurrentTeamHasMap())
			{
				Scheduler.Execute(showMap);
				yield return waitUntil("MAP CLOSED");
			}

			EventManager.StartTurn();
		}

		public static IEnumerator<PauseScript> showMap()
		{
			EventManager.SetCurrentTeamMap(false);
			EventManager.CreateMapGraphic(0,0,"MAP");
			EventManager.PlaySoundEffect("openmap", 1);
			EventManager.LerpColor("MAP", 1,1,1,1, 2);
			yield return waitSeconds(6);

			EventManager.LerpColor("MAP", 0,0,0,0, 1.5f);
			EventManager.PlaySoundEffect("closemap", 1);
			yield return waitSeconds(1.5f);

			EventManager.DestroyGraphic("MAP");
			EventManager.SendSignal("MAP CLOSED");
		}

		public static IEnumerator<PauseScript> delayPointScroll()
		{
			yield return waitSeconds(1.5f);
			EventManager.ScrollPoints();
		}

		public static IEnumerator<PauseScript> endTurn()
		{
			EventManager.EndTurn();
			Scheduler.Execute(checkChange);
			yield return waitUntil("Change checked");

			EventManager.Write("Change checked raised!");
			if (!EventManager.CheckEndOfGame())
				Scheduler.Execute(nextTeam);
		}

		public static IEnumerator<PauseScript> checkChange ()
		{
			if (EventManager.ChangeFilled ())
			{
				Scheduler.Execute(runChange);
				yield return waitUntil("Change run");
			}

			EventManager.Write("leaving checkChange()");
			EventManager.SendSignal("Change checked");
		}

		public static IEnumerator<PauseScript> runChange()
		{
			// Yes we can
			//Randomize the change mode and set it in the game scene
			string type = EventManager.GetChangeMode();
			Scheduler.ExecuteWithArgs<string>(showChangeGraphic, type);

			yield return waitUntil("Change graphic shown");
			EventManager.ResetChange();
			EventManager.SendSignal("Change checked");
		}

		public static IEnumerator<PauseScript> showChangeGraphic (string type)
		{
			EventManager.FadeOutMusic (2);
				
			EventManager.Write ("run showChangeGraphic()");
			EventManager.PlaySoundEffect ("change", 1);
			EventManager.CreateGraphic ("Graphics/GUI/change", 0, 0, "CHANGE");
			EventManager.LerpColor ("CHANGE", 1, 1, 1, 1, .5f);
			yield return waitSeconds (2);
					
			EventManager.KillMusic ();

			// Do this here, for some reason
			EventManager.GenerateTown ();
							
			EventManager.LerpColor ("CHANGE", 0, 0, 0, 0, .5f);
			yield return waitSeconds (.5f);
			EventManager.DestroyGraphic ("CHANGE");

			string graphicName = "";
			bool notNormal = true;
			if (type == "Normal")
			{
				notNormal = false;
				EventManager.PlaySong ("TreasureHunt");
			}
			else if (type == "Danger")
			{
				graphicName = "BOMB";
				EventManager.CreateGraphic ("Graphics/GUI/DangerBanner", 0, 0, graphicName);
				EventManager.PlaySong ("BakugekiNights");
			}
			else if (type == "CrimeWave")
			{
				graphicName = "CRIME";
				EventManager.CreateGraphic ("Graphics/GUI/CrimeWaveBanner", 0, 0, graphicName);
				EventManager.PlaySong ("DorobonusRound");
			}
			else if (type == "Greed")
			{
				graphicName = "GREED";
				EventManager.CreateGraphic ("Graphics/GUI/GreedBanner", 0, 0, graphicName);
				EventManager.PlaySong ("PointsOrDie");
			}

			if (notNormal)
			{
				EventManager.LerpColor(graphicName, 1, 1, 1, 1, .5f);
				yield return waitSeconds(2.5f);
				EventManager.LerpColor(graphicName, 0, 0, 0, 0, .5f);
				yield return waitSeconds(.5f);
				EventManager.DestroyGraphic(graphicName);
			}
		}

		public static IEnumerator<PauseScript> showExplosion()
		{
			EventManager.PlaySoundEffect("bomb");
			EventManager.CreateExplosionGraphic();
			EventManager.LerpColor("EXPLOSION", 1, 1, 1, 1, .5f);
			EventManager.LerpScale("EXPLOSION", 1, .5f);
			yield return waitSeconds(.5f);
			EventManager.LerpScale("EXPLOSION", 1.35f, 2);
			EventManager.LerpColor("EXPLOSION", 0, 0, 0, 0, 2);
			yield return waitSeconds(2);
			EventManager.DestroyGraphic("EXPLOSION");
		}

		public static IEnumerator<PauseScript> endGame()
		{
			EventManager.FadeOutMusic(3);
			yield return waitSeconds(3);
			EventManager.CreateGraphic("Graphics/GUI/Finish", 0, 0, "FINISH");
			EventManager.LerpColor("FINISH", 1, 1, 1, 1, 1.5f);
			yield return waitSeconds(4);
			EventManager.GoToResultsScene();
		}

		#endregion



		#region mainMenu.lua
		public static IEnumerator<PauseScript> showSplash()
		{
			EventManager.MenuLerpColor("SPLASH", 1, 1, 1, 1, 2.5f);
			yield return waitSeconds(4);
			EventManager.MenuLerpColor("SPLASH", 0, 0, 0, 0, 1.5f);
			yield return waitSeconds(2.5f);
			EventManager.MenuDestroyGraphic("SPLASH");
			EventManager.MenuShowTitle();
		}

		public static IEnumerator<PauseScript> cancelSplashScreen()
		{
			Scheduler.AbortAllCoroutines();
			EventManager.MenuDestroyGraphic("SPLASH");
			EventManager.MenuShowTitle();
			yield return null;
		}
		#endregion
	
	
	}
}

