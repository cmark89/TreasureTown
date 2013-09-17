using System;
using Microsoft.Xna.Framework;

namespace TreasureTown
{
	public class Team
	{
		static int next_ID = 0;
		public int ID { get; private set; }

		public readonly static Color[] TeamColors = {
			Color.Red,
			Color.CornflowerBlue,
			Color.Yellow,
			Color.LightGreen,
			Color.Orange,
			Color.Magenta
		};
		public Color TeamColor 
		{
			get { return TeamColors [ID]; }
		}
		public int TeamNumber 
		{
			get { return ID + 1; }
		}

		public float Points = 1000;
		public bool HasMap = false;

		public Team ()
		{
			ID = next_ID;
			next_ID++;
		}
	}
}

