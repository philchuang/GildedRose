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
			return new App ()
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
		}

		public IList<Item> Items;

		private readonly Dictionary<ItemType, IItemQualityUpdater> m_ItemTypeToUpdaterMap;

		public App ()
		{
			m_ItemTypeToUpdaterMap = new Dictionary<ItemType, IItemQualityUpdater> ();
			//m_ItemTypeToUpdaterMap[ItemType.Unknown] = new NormalItemQualityUpdater ();
			//m_ItemTypeToUpdaterMap[ItemType.Conjured] = new ConjuredItemQualityUpdater ();
			m_ItemTypeToUpdaterMap[ItemType.Legendary] = new LegendaryItemQualityUpdater ();
			m_ItemTypeToUpdaterMap[ItemType.Unique] = new UniqueItemsQualityUpdater (
				//new AgedBrieItemQualityUpdater (),
				new BackstagePassesItemQualityUpdater ());
		}

		public ItemType GetItemType (Item item)
		{
			//if (item.Name == "Aged Brie")
			//	return ItemType.Unique;
			if (item.Name == "Backstage passes to a TAFKAL80ETC concert")
				return ItemType.Unique;
			if (item.Name == "Sulfuras, Hand of Ragnaros")
				return ItemType.Legendary;

			return ItemType.Unknown;
		}
		
		public void UpdateQuality ()
        {
            for (var i = 0; i < Items.Count; i++)
            {
	            var itemType = GetItemType (Items[i]);
				if (itemType != ItemType.Unknown)
				{
					m_ItemTypeToUpdaterMap[itemType].Update (Items[i]);
					continue;
				}

                if (Items[i].Name != "Aged Brie" && Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                {
                    if (Items[i].Quality > 0)
                    {
                        if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                        {
                            Items[i].Quality = Items[i].Quality - 1;
                        }
                    }
                }
                else
                {
                    if (Items[i].Quality < 50)
                    {
                        Items[i].Quality = Items[i].Quality + 1;

                        if (Items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (Items[i].SellIn < 11)
                            {
                                if (Items[i].Quality < 50)
                                {
                                    Items[i].Quality = Items[i].Quality + 1;
                                }
                            }

                            if (Items[i].SellIn < 6)
                            {
                                if (Items[i].Quality < 50)
                                {
                                    Items[i].Quality = Items[i].Quality + 1;
                                }
                            }
                        }
                    }
                }

                if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                {
                    Items[i].SellIn = Items[i].SellIn - 1;
                }

                if (Items[i].SellIn < 0)
                {
                    if (Items[i].Name != "Aged Brie")
                    {
                        if (Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (Items[i].Quality > 0)
                            {
                                if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                                {
                                    Items[i].Quality = Items[i].Quality - 1;
                                }
                            }
                        }
                        else
                        {
                            Items[i].Quality = Items[i].Quality - Items[i].Quality;
                        }
                    }
                    else
                    {
                        if (Items[i].Quality < 50)
                        {
                            Items[i].Quality = Items[i].Quality + 1;
                        }
                    }
                }
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
		public void Update (Item item)
		{
			item.SellIn--;
			if (item.Quality == 0) return;

			item.Quality += item.SellIn >= 0 ? -1 : -2;

			if (item.Quality < 0)
				item.Quality = 0;
		}
	}

	public class ConjuredItemQualityUpdater : IItemQualityUpdater
	{
		public void Update (Item item)
		{
			// TODO implement
		}
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
			// TODO implement
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
