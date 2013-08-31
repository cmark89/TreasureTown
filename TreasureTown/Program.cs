#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TreasureTown
{
	static class Program
	{
		private static TreasureTown game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			game = new TreasureTown ();
			game.Run ();
		}
	}
}
