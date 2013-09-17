using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TreasureTown
{
	public class Navnode
	{
		public int ID { get; private set; }
		public Point Position { get; private set; }
		public Dictionary<MapDirection, Navnode> Neighbors;
		public bool IsBuildingSpawn = false;
		public Building Building;

		public Navnode (Point pos, int id)
		{
			Position = pos;
			Neighbors = new Dictionary<MapDirection, Navnode>();
			ID = id;
		}


		// This returns a random adjacent node, which is only useful for like one thing ever
		public Navnode GetAdjacentSpace ()
		{
			List<Navnode> neighbors = new List<Navnode> ();
			foreach (KeyValuePair<MapDirection, Navnode> k in Neighbors)
			{
				neighbors.Add (k.Value);
			}

			return neighbors[TreasureTown.StaticRandom.Next (0, neighbors.Count)];
		}

		public MapDirection GetExitDirection()
		{
			List<MapDirection> validDirections = new List<MapDirection>();
			foreach (KeyValuePair<MapDirection, Navnode> k in Neighbors)
			{
				if(k.Value.IsBuildingSpawn)
					continue;
				else
					validDirections.Add (k.Key);
			}

			return validDirections[TreasureTown.StaticRandom.Next(0, validDirections.Count)];
		}

		public Vector2 GetNeighborPosition (MapDirection dir)
		{
			if (Neighbors [dir] != null)
			{
				return new Vector2(Neighbors[dir].Position.X, Neighbors[dir].Position.Y);
			}
			else
			{
				return Vector2.Zero;
			}
		}

		public void SetBuilding (Building building)
		{
			if (IsBuildingSpawn)
			{
				Building = building;
			}
		}
	}
}

