using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreasureTown
{
	public class ScoreChanger
	{
		Vector2 position;
		Team team;
		float time;
		float duration;
		public float totalChange { get; private set; }
		float currentPoints;
		float targetPointValue;
		float startPoints;
		bool scrolling;
		public bool finished { get; private set; }
		Color drawColor;
		char leadingChar;

		float pointChange
		{
			get
			{
				return totalChange / duration;
			}
		}

		public ScoreChanger (Team thisTeam, Vector2 pos, int change, float dur)
		{
			scrolling = false;

			startPoints = thisTeam.Points;
			team = thisTeam;
			position = pos;
			totalChange = change;
			currentPoints = totalChange;
			duration = dur;

			targetPointValue = thisTeam.Points + change;

			if (change > 0)
			{
				leadingChar = '+';
				drawColor = Color.LightGreen;
			}
			else
			{
				leadingChar = ' ';
				drawColor = Color.Red;
			}

			time = 0f;
		}

		public void Update (GameTime gameTime)
		{
			if (scrolling)
			{
				float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
				time += deltaTime;
				currentPoints = MathHelper.Lerp (totalChange, 0, time / duration);
				team.Points = MathHelper.Lerp (startPoints, targetPointValue, time / duration);
				team.Points += pointChange * deltaTime;
				
				if(time >= duration)
				{
					team.Points = targetPointValue;
					scrolling = false;
					finished = true;
					EventManager.SendSignal("Points changed");
					// Destroy
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(!finished)
				spriteBatch.DrawString (GameScene.Instance.scoreFont, String.Format ("{0}{1}", leadingChar.ToString (), (int)currentPoints), position, drawColor);
		}

		public void BeginScrolling()
		{
			scrolling = true;
		}
	}
}

