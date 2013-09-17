using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LuaInterface;

namespace TreasureTown
{
	public class Map
	{
		Dictionary<int, Navnode> navmesh;

		public Map (LuaTable table)
		{
			// Parse the luatable to fill out the list of nodes
			navmesh = ParseNavmesh(table);
		}

		public Dictionary<int, Navnode> ParseNavmesh(LuaTable table)
		{
			Dictionary<int, Navnode> nodes = new Dictionary<int, Navnode>();
			int nodeCount = table.Keys.Count + 1;
			LuaTable currentTable;
			Navnode thisNode;

			for(int i = 1; i < nodeCount; i++)
			{
				if(table[i] != null)
				{
					// Cache this table
					currentTable = (LuaTable)table[i];

					// Create the new node
					thisNode = new Navnode(new Point((int)(double)currentTable["Position.X"], (int)(double)currentTable["Position.Y"]), i);
					thisNode.IsBuildingSpawn = (bool)currentTable["BuildingSpawn"];

					nodes.Add (i, thisNode);
				}
			}

			// Now that the list is populated, add all connections.
			for(int i = 1; i < nodeCount; i++)
			{
				if(table[i] != null)
				{
					// Cache this table
					currentTable = (LuaTable)table[i];

					// Set the neighbors for the current node
					currentTable = (LuaTable)currentTable["Connections"];
					if(currentTable["Up"] != null) nodes[i].Neighbors[MapDirection.Up] = nodes[(int)(double)currentTable["Up"]];
					if(currentTable["Down"] != null) nodes[i].Neighbors[MapDirection.Down] = nodes[(int)(double)currentTable["Down"]];
					if(currentTable["Left"] != null) nodes[i].Neighbors[MapDirection.Left] = nodes[(int)(double)currentTable["Left"]];
					if(currentTable["Right"] != null) nodes[i].Neighbors[MapDirection.Right] = nodes[(int)(double)currentTable["Right"]];
				}
			}
			return nodes;
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

