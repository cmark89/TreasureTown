using System;
using System.Collections.Generic;
using SchedulerTest;

namespace TreasureTown
{
	public class Item
	{
		public ItemType Type { get; private set; }
		ScriptWithArgs<int> onActivate;
		Building thisBuilding;
		public int value { get; private set; }

		public Item (Building building, ItemType thisType, int pointValue = 0)
		{
			thisBuilding = building;
			building.SetItem (this);
			Type = thisType;
			SetOnActivate();
			value = pointValue;
		}

		public void SetOnActivate ()
		{
			switch (Type)
			{
			case(ItemType.Points):
				onActivate = Scripts.findPoints;
				break;
			case(ItemType.Thief):
				onActivate = Scripts.findThief;
				break;
			case(ItemType.Bomb):
				onActivate = Scripts.findBomb;
				break;
			case(ItemType.Map):
				onActivate = Scripts.findMap;
				break;
			}
		}

		public void Activate ()
		{
			if (onActivate != null)
			{
				Console.WriteLine("Executing " + Type.ToString());
				Scheduler.ExecuteWithArgs<int>(onActivate, value);
			}

			thisBuilding.FinishExploring ();
		}
	}

	public enum ItemType
	{
		Points,
		Bomb,
		Thief,
		Map
	}
}

