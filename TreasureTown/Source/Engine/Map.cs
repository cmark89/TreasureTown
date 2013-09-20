using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SchedulerTest;

namespace TreasureTown
{
	public partial class Map
	{
		public Dictionary<int, Navnode> navmesh { get; private set; }

		public Map ()
		{
			BuildNavmesh();
		}

		public Navnode GetNextNode (int start, MapDirection dir)
		{
			try
			{
				return navmesh[start].Neighbors[dir];
			}
			catch
			{
				Console.WriteLine ("Node " + start + " has no " + dir.ToString () + " neighbor!");
				return null;
			}
		}

		public List<Navnode> GetNodeList ()
		{
			List<Navnode> list = new List<Navnode> ();

			foreach (KeyValuePair<int, Navnode> k in navmesh)
			{
				list.Add (k.Value);
			}

			return list;
		}
	}

	public enum MapDirection
	{
		Left,
		Up,
		Right,
		Down
	}
}

