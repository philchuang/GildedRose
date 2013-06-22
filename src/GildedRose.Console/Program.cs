using System;
using System.Collections.Generic;

namespace GildedRose.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("OMGHAI!");

			var app = App.CreateDefaultInstance();

            app.UpdateQuality();

            System.Console.ReadKey();

        }
	}

	public class App
	{
		public static App CreateDefaultInstance ()
		{
			var app = new App ()
				          {
					          Items = new List<Item>
						                  {
							                  new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
							                  new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
							                  new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
							                  new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
							                  new Item
								                  {
									                  Name = "Backstage passes to a TAFKAL80ETC concert",
									                  SellIn = 15,
									                  Quality = 20
								                  },
							                  new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
						                  }
				          };

			app.AddItemTypeMapping ("Aged Brie", ItemType.Unique);
			app.AddItemTypeMapping ("Backstage passes to a TAFKAL80ETC concert", ItemType.Unique);
			app.AddItemTypeMapping ("Sulfuras, Hand of Ragnaros", ItemType.Legendary);
			app.AddItemTypeMapping ("Conjured Mana Cake", ItemType.Conjured);

			return app;
		}

		public IList<Item> Items;

		private readonly Dictionary<String, ItemType> m_ItemNameToItemTypeMap;
		private readonly Dictionary<ItemType, IItemQualityUpdater> m_ItemTypeToUpdaterMap;

		public App ()
		{
			m_ItemNameToItemTypeMap = new Dictionary<string, ItemType> ();
			m_ItemTypeToUpdaterMap = new Dictionary<ItemType, IItemQualityUpdater> ();
			m_ItemTypeToUpdaterMap[ItemType.Unknown] = new NormalItemQualityUpdater ();
			m_ItemTypeToUpdaterMap[ItemType.Conjured] = new ConjuredItemQualityUpdater ();
			m_ItemTypeToUpdaterMap[ItemType.Legendary] = new LegendaryItemQualityUpdater ();
			m_ItemTypeToUpdaterMap[ItemType.Unique] = new UniqueItemsQualityUpdater (
				new AgedBrieItemQualityUpdater (),
				new BackstagePassesItemQualityUpdater ());
		}

		public void AddItemTypeMapping (String itemName, ItemType type)
		{
			m_ItemNameToItemTypeMap[itemName] = type;
		}

		public ItemType GetItemType (Item item)
		{
			if (item == null) throw new ArgumentNullException ("item");

			ItemType type;
			return m_ItemNameToItemTypeMap.TryGetValue (item.Name, out type) ? type : ItemType.Unknown;
		}
		
		public void UpdateQuality ()
        {
            for (var i = 0; i < Items.Count; i++)
            {
	            var itemType = GetItemType (Items[i]);
				m_ItemTypeToUpdaterMap[itemType].Update (Items[i]);
            }
        }
    }

	public interface IItemQualityUpdater
	{
		void Update (Item item);
	}

	public interface IUniqueItemQualityUpdater : IItemQualityUpdater
	{
		string ItemName { get; }
	}

	public class UniqueItemsQualityUpdater : IItemQualityUpdater
	{
		private readonly Dictionary<string, IUniqueItemQualityUpdater> m_UniqueItemNameToUpdaterMap;

		public UniqueItemsQualityUpdater (params IUniqueItemQualityUpdater[] updaters)
		{
			m_UniqueItemNameToUpdaterMap = new Dictionary<string, IUniqueItemQualityUpdater> ();

			if (updaters == null) return;

			foreach (var updater in updaters)
				m_UniqueItemNameToUpdaterMap[updater.ItemName] = updater;
		}

		public void Update (Item item)
		{
			if (item == null) throw new ArgumentNullException ("item");

			m_UniqueItemNameToUpdaterMap[item.Name].Update (item);
		}
	}

	public class NormalItemQualityUpdater : IItemQualityUpdater
	{
		protected virtual int DecayFactor { get { return -1; } }

		public void Update (Item item)
		{
			item.Quality += item.SellIn > 0 ? DecayFactor : DecayFactor*2;

			item.SellIn--;

			if (item.Quality < 0)
				item.Quality = 0;
		}
	}

	public class ConjuredItemQualityUpdater : NormalItemQualityUpdater
	{
		protected override int DecayFactor { get { return -2; } }
	}

	public class LegendaryItemQualityUpdater : IItemQualityUpdater
	{
		public void Update (Item item)
		{
			// does nothing, Quality and SellIn do not change
		}
	}

	public class AgedBrieItemQualityUpdater : IUniqueItemQualityUpdater
	{
		public String ItemName { get { return "Aged Brie"; } }

		public void Update (Item item)
		{
			if (item.SellIn <= 0) item.Quality += 2;
			else item.Quality += 1;

			item.SellIn--;

			if (item.Quality > 50) item.Quality = 50;
			else if (item.Quality < 0) item.Quality = 0;
		}
	}

	public class BackstagePassesItemQualityUpdater : IUniqueItemQualityUpdater
	{
		public String ItemName { get { return "Backstage passes to a TAFKAL80ETC concert"; } }

		public void Update (Item item)
		{
			if (item.SellIn <= 0) item.Quality = 0;
			else if (item.SellIn <= 5) item.Quality += 3;
			else if (item.SellIn <= 10) item.Quality += 2;
			else item.Quality += 1;

			item.SellIn--;

			if (item.Quality > 50) item.Quality = 50;
			else if (item.Quality < 0) item.Quality = 0;
		}
	}

	public enum ItemType
	{
		Unknown,
		Conjured,
		Legendary,
		Unique,
	}

    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }
}
