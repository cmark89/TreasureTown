using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TreasureTown
{
	public class TownGenerator
	{
		Map gameMap;

		public TownGenerator (Map map)
		{
			gameMap = map;
		}


		// Add options for the super mode
		public void GenerateTown(ref List<Building> buildings, ref Navnode startPoint)
		{
			// First, clear the list of buildings.  Presumably there will be some flashy effect as well
			buildings.Clear ();
			
			Navnode currentNode;
			List<Navnode> remainingNodes = gameMap.GetNodeList ().FindAll (x => x.IsBuildingSpawn);
			List<BuildingType> availableTypes = new List<BuildingType>();
			Building newBuilding;

			foreach(BuildingType b in Enum.GetValues(typeof(BuildingType)))
				availableTypes.Add (b);
			
			availableTypes.Remove (BuildingType.TrainStation);

			int index = 0;
			// Next, generate the station in a random location.
			index = TreasureTown.StaticRandom.Next (0, remainingNodes.Count);
			currentNode = remainingNodes[index];
			remainingNodes.RemoveAt (index);

			newBuilding = new Building(currentNode, BuildingType.TrainStation);
			currentNode.SetBuilding (newBuilding);
			buildings.Add (newBuilding);

			BuildingType newBuildingType;

			// Inform the game scene where the new start point will be
			startPoint = currentNode.GetAdjacentSpace();

			// Now that the start point has been settled, generate the rest of the town
			while(remainingNodes.Count > 0)
			{
				index = TreasureTown.StaticRandom.Next (0, remainingNodes.Count);
				currentNode = remainingNodes[index];
				remainingNodes.RemoveAt (index);
				newBuildingType = GetRandomBuildingType(availableTypes);
				newBuilding = new Building(currentNode, newBuildingType);
				currentNode.SetBuilding(newBuilding);
				buildings.Add (newBuilding);

				// If this building occurs twice in the town, remove it from the available types.
				if(buildings.FindAll (x=>x.Type == newBuildingType).Count >= 2)
					availableTypes.Remove (newBuildingType);

				// Now there are no more towns with 3 banks
			}

			GenerateItems (ref buildings);
		}

		public static BuildingType GetRandomBuildingType(List<BuildingType> availableTypes)
		{
			
			int roll = TreasureTown.StaticRandom.Next (0, availableTypes.Count);
			
			return availableTypes[roll];
		}

		//Add option for super mode
		public void GenerateItems (ref List<Building> buildings)
		{
			int maxBombs = 0;
			int maxThieves = 0;
			int maxMaps = 0;
			int maxHighPoints = 0;
			int roll;

			if (GameScene.Instance.Mode == ChangeMode.Danger)
			{
				maxBombs = 9;
				maxThieves = 1;
				maxMaps = 1;
				maxHighPoints = 2;

				foreach (Building b in buildings)
				{
					// Roll a d10
					roll = TreasureTown.StaticRandom.Next (1, 11);
					switch (roll)
					{
					case 1:
						if (maxMaps > 0)
						{
							maxMaps--;
							new Item (b, ItemType.Map);
							break;
						}
						goto case 2;
					case 2: 
						if (maxThieves > 0)
						{
							maxThieves--;
							new Item (b, ItemType.Thief);
							break;
						}
						goto case 3;
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
						if (maxBombs > 0)
						{
							maxBombs--;
							new Item (b, ItemType.Bomb);
							break;
						}
						goto case 9;
					case 9:
					case 10:
						new Item (b, ItemType.Points, GetPointValue (ref maxHighPoints));
						break;
					default:
						break;
					}
				}
			}
			else if (GameScene.Instance.Mode == ChangeMode.Greed)
			{
				maxBombs = 0;
				maxThieves = 3;
				maxMaps = 0;
				maxHighPoints = 5;
				
				foreach(Building b in buildings)
				{
					// Roll a d10
					roll = TreasureTown.StaticRandom.Next (1, 11);
					switch(roll)
					{
					case 1:
						if(maxMaps > 0)
						{
							maxMaps--;
							new Item(b, ItemType.Map);
							break;
						}
						goto case 2;
					case 2: 
						if(maxThieves > 0)
						{
							maxThieves--;
							new Item(b, ItemType.Thief);
							break;
						}
						goto case 3;
					case 3:
						if(maxBombs > 0)
						{
							maxBombs--;
							new Item(b, ItemType.Bomb);
							break;
						}
						goto case 4;
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
						new Item(b, ItemType.Points, GetPointValue(ref maxHighPoints));
						break;
					default:
						break;
					}
				}
			}
			else if (GameScene.Instance.Mode == ChangeMode.CrimeWave)
			{
				maxBombs = 3;
				maxThieves = 7;
				maxMaps = 1;
				maxHighPoints = 3;
				
				foreach(Building b in buildings)
				{
					// Roll a d10
					roll = TreasureTown.StaticRandom.Next (1, 11);
					switch(roll)
					{
					case 1:
						if(maxMaps > 0)
						{
							maxMaps--;
							new Item(b, ItemType.Map);
							break;
						}
						goto case 2;
					case 2:
						if(maxBombs > 0)
						{
							maxBombs--;
							new Item(b, ItemType.Bomb);
							break;
						}
						goto case 3;
					case 3:
					case 4: 
					case 5:
					case 6:
						if(maxThieves > 0)
						{
							maxThieves--;
							new Item(b, ItemType.Thief);
							break;
						}
						goto case 7;
					case 7:
					case 8:
					case 9:
					case 10:
						new Item(b, ItemType.Points, GetPointValue(ref maxHighPoints));
						break;
					default:
						break;
					}
				}
			}
			else
			{
				maxBombs = 4;
				maxThieves = 2;
				maxMaps = 1;
				maxHighPoints = 3;

				foreach(Building b in buildings)
				{
					// Roll a d10
					roll = TreasureTown.StaticRandom.Next (1, 11);
					switch(roll)
					{
					case 1:
						if(maxMaps > 0)
						{
							maxMaps--;
							new Item(b, ItemType.Map);
							break;
						}
						goto case 2;
					case 2: 
						if(maxThieves > 0)
						{
							maxThieves--;
							new Item(b, ItemType.Thief);
							break;
						}
						goto case 3;
					case 3:
					case 4:
						if(maxBombs > 0)
						{
							maxBombs--;
							new Item(b, ItemType.Bomb);
							break;
						}
						goto case 5;
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
						new Item(b, ItemType.Points, GetPointValue(ref maxHighPoints));
						break;
					default:
						break;
					}
				}
			}
		}

		public int GetPointValue (ref int maxHighPoints)
		{
			float multiplier = 1f;
			if (GameScene.Instance.Mode == ChangeMode.Greed)
				multiplier = 2f;

			bool isTreasure = false;
			// Check to see if this is a high value point item
			if (maxHighPoints > 0 && TreasureTown.StaticRandom.Next (0, 4) == 0)
			{
				maxHighPoints--;
				isTreasure = true;
			}

			int points;
			// Roll between 5 and 11
			if (!isTreasure)
			{
				points = TreasureTown.StaticRandom.Next (5, 14);

				// Base points: 500 - 1,300
				// 500 - 1000 = smallmoney
				// 1000+ = money
			}
			else
			{
				// Treasure always has a higher payout
				points = TreasureTown.StaticRandom.Next (15, 21);

				// Base points: 1,500 - 2,000
				// treasure
			}




			points *= (int)(100 * multiplier);

			return points;

			// Minimum point value: 500
			// Maximum point value: 1500

			// Minimum treasure value: 1000
			// Maximum treasure value: 3000
		}

		public void ShuffleRemainingItems (ref List<Building> buildings)
		{
			Console.WriteLine ("SHUFFLE REMAINING ITEMS");
			// Create a list of items in the remaining buildings, and a list of the buildings that are unexplored
			List<Item> remainingItems = new List<Item> ();

			foreach (Building b in buildings.FindAll(x=>!x.explored))
			{
				remainingItems.Add (b.item);
			}

			int i = 0;
			Item newItem = null;
			foreach (Building b in buildings)
			{
				if(!b.explored)
				{
					// Grab a random item
					i = TreasureTown.StaticRandom.Next (0, remainingItems.Count);
					newItem = remainingItems[i];
					remainingItems.RemoveAt (i);

					b.SetItem(newItem);
				}
			}
		}
	}
}

