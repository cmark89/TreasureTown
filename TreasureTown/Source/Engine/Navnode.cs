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
		private int[] neighborIDs;
		public bool IsBuildingSpawn = false;
		public Building Building;

		public Navnode (Point pos, int id, bool isBuilding, int[] neighbors = null)
		{
			Position = pos;
			IsBuildingSpawn = isBuilding;
			Neighbors = new Dictionary<MapDirection, Navnode>();
			ID = id;

			if(neighbors != null)
				neighborIDs = neighbors;
		}

		public void LinkToNeighbors (Map map)
		{
			// Ensure this is cleared
			Neighbors = new Dictionary<MapDirection, Navnode>();

			// For each ID, add the actual node represented by it 
			Navnode targetNode;
			MapDirection dir;
			foreach (int i in neighborIDs)
			{

				targetNode = map.navmesh[i];
				dir = GetDirectionTo(map.navmesh[i]);
				Neighbors[dir] = targetNode;
			}
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

		public MapDirection GetDirectionTo (Navnode other)
		{
			float xDist = 0;
			float yDist = 0;

			xDist = other.Position.X - Position.X;
			yDist = other.Position.Y - Position.Y;

			// Check to see which direction is the facing direction
			if (Math.Abs (xDist) > Math.Abs (yDist))
			{
				// Horizontal direction
				if(xDist > 0)
				{
					return MapDirection.Right;
				}
				else
				{
					return MapDirection.Left;
				}
			}
			else
			{
				// Vertical direction
				if(yDist > 0)
				{
					return MapDirection.Down;
				}
				else
				{
					return MapDirection.Up;
				}
			}
		}
	}
}

