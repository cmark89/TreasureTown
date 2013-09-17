using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreasureTown
{
	public class Building
	{
		static Dictionary<BuildingType, Texture2D> buildingGraphics;
		private Texture2D texture;
		public BuildingType Type { get; private set; }
		public Item item { get; private set; }
		public bool explored = false;
		Navnode parentNode;
		public Vector2 Position {
			get 
			{
				return new Vector2(parentNode.Position.X, parentNode.Position.Y);
			}
		}
		// Item item

		public static void LoadBuildingGraphics ()
		{
			buildingGraphics = new Dictionary<BuildingType, Texture2D>();

			string name;
			Texture2D tex;
			for(int i = 0; i < Enum.GetNames(typeof(BuildingType)).Length; i++)
			{
				name = ((BuildingType)i).ToString ();
				tex = TreasureTown.StaticContent.Load<Texture2D> ("Graphics/buildings/" + name);
				buildingGraphics.Add ((BuildingType)i, tex);
			}
		}

		public Building (Navnode node, BuildingType type)
		{
			Type = type;
			texture = buildingGraphics[Type];
			parentNode = node;

#if DEBUG
			Console.WriteLine("GENERATE " + Type.ToString ());
#endif
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(texture != null)
				spriteBatch.Draw (texture, position: new Vector2(parentNode.Position.X, parentNode.Position.Y), origin: GetOrigin(), color: DrawColor(), depth: .5f, scale: new Vector2(.75f, .75f));
		}

		private Color DrawColor()
		{
			if(explored) return Color.SlateGray;
			else return Color.White;
		}

		private Vector2 GetOrigin()
		{
			return new Vector2(texture.Width/2, texture.Height - 35);
		}

		public void SetItem(Item newItem)
		{
			item = newItem;
		}

		public void FinishExploring()
		{
			explored = true;
			item = null;
		}

		public void Explore ()
		{
			if (item != null)
			{
				item.Activate ();

				// If there's a bonus item
			}
			else
			{
				// Do something so that it's not a waste?
				TreasureTown.MainLua.DoString ("findEmpty()");
			}
		}

	}

	public enum BuildingType
	{
		TrainStation,
		SuperMarket,
		DepartmentStore,
		ConvenienceStore,
		Restaurant,
		Bookstore,
		FlowerShop,
		StationeryStore,
		ToyStore,
		BarberShop,
		Park,
		Hospital,
		PoliceStation,
		PoliceBox,
		FireStation,
		Bank,
		PostOffice,
		Castle
	}
}

