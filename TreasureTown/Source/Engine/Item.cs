using System;
using LuaInterface;

namespace TreasureTown
{
	public class Item
	{
		public ItemType Type { get; private set; }
		LuaFunction onActivate;
		Building thisBuilding;
		public int value { get; private set; }

		public Item (Building building, ItemType thisType, int pointValue = 0)
		{
			thisBuilding = building;
			building.SetItem (this);
			Type = thisType;
			onActivate = (LuaFunction)TreasureTown.MainLua["find" + Type.ToString ()];
			value = pointValue;
		}

		public void Activate ()
		{
			if (onActivate != null)
			{
				if (Type == ItemType.Points)
					onActivate.Call (value);
				else
					onActivate.Call ();
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

