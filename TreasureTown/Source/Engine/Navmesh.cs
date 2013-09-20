using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TreasureTown
{
	public partial class Map
	{
		// Build the map.  Hardcoded because Lua.
		public void BuildNavmesh ()
		{
			navmesh = new Dictionary<int, Navnode> ();

			navmesh.Add (1, new Navnode (new Point (100, 67), 1, true, new int[] { 4 }));
			navmesh.Add (2, new Navnode (new Point (280, 62), 2, true, new int[] { 6 }));
			navmesh.Add (3, new Navnode (new Point (445, 71), 3, true, new int[] { 7 }));

			navmesh.Add (4, new Navnode (new Point (100, 111), 4, false, new int[] { 1, 5 }));
			navmesh.Add (5, new Navnode (new Point (192, 114), 5, false, new int[] { 4, 6, 13 }));
			navmesh.Add (6, new Navnode (new Point (280, 110), 6, false, new int[] { 5, 2 }));
			navmesh.Add (7, new Navnode (new Point (445, 102), 7, false, new int[] { 3, 8 }));
			navmesh.Add (8, new Navnode (new Point (444, 162), 8, false, new int[] { 7, 9, 15 }));

			navmesh.Add (9, new Navnode (new Point (510, 162), 9, true, new int[] { 8 }));
			navmesh.Add (10, new Navnode (new Point (106, 172), 10, true, new int[] { 12 }));
			navmesh.Add (11, new Navnode (new Point (282, 179), 11, true, new int[] { 14 }));

			navmesh.Add (12, new Navnode (new Point (106, 223), 12, false, new int[] { 10, 13 }));
			navmesh.Add (13, new Navnode (new Point (192, 225), 13, false, new int[] { 12, 5, 19 }));
			navmesh.Add (14, new Navnode (new Point (285, 220), 14, false, new int[] { 11, 15, 20 }));
			navmesh.Add (15, new Navnode (new Point (444, 214), 15, false, new int[] { 14, 8 }));

			navmesh.Add (16, new Navnode (new Point (104, 291), 16, true, new int[] { 18 }));
			navmesh.Add (17, new Navnode (new Point (462, 289), 17, true, new int[] { 22 }));

			navmesh.Add (18, new Navnode (new Point (104, 334), 18, false, new int[] { 16, 19, 25 }));
			navmesh.Add (19, new Navnode (new Point (192, 331), 19, false, new int[] { 13, 18, 20 }));
			navmesh.Add (20, new Navnode (new Point (285, 331), 20, false, new int[] { 14, 19, 21 }));
			navmesh.Add (21, new Navnode (new Point (364, 333), 21, false, new int[] { 20, 22, 27 }));
			navmesh.Add (22, new Navnode (new Point (462, 332), 22, false, new int[] { 17, 21 }));

			navmesh.Add (23, new Navnode (new Point (286, 420), 23, true, new int[] { 26 }));
			navmesh.Add (24, new Navnode (new Point (450, 421), 24, true, new int[] { 28 }));

			navmesh.Add (25, new Navnode (new Point (102, 461), 25, false, new int[] { 18, 26, 30 }));
			navmesh.Add (26, new Navnode (new Point (286, 462), 26, false, new int[] { 25, 23, 27, 32 }));
			navmesh.Add (27, new Navnode (new Point (364, 462), 27, false, new int[] { 26, 21, 28 }));
			navmesh.Add (28, new Navnode (new Point (448, 461), 28, false, new int[] { 27, 24 }));

			navmesh.Add (29, new Navnode (new Point (189, 523), 29, true, new int[] { 31 }));

			navmesh.Add (30, new Navnode (new Point (104, 573), 30, false, new int[] { 25, 31 }));
			navmesh.Add (31, new Navnode (new Point (191, 572), 31, false, new int[] { 29, 30, 32 }));
			navmesh.Add (32, new Navnode (new Point (286, 571), 32, false, new int[] { 26, 31, 33 }));
			navmesh.Add (33, new Navnode (new Point (372, 570), 33, false, new int[] { 32, 34 }));

			navmesh.Add (34, new Navnode (new Point (456, 570), 34, true, new int[] { 33 }));




			foreach (KeyValuePair<int, Navnode> kv in navmesh)
			{
				kv.Value.LinkToNeighbors(this);
			}
		}

	}
}

