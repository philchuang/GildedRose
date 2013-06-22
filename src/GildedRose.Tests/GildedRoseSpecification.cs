using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GildedRose.Console;
using NBehave.Spec.NUnit;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace GildedRose.Tests
{
	public abstract class GildedRoseSpecification : SpecBase
	{
		protected App m_App;
		protected Exception m_Because_of_Exception;
		protected bool? m_Because_of_Exception_Expected;

		protected override void Establish_context ()
		{
			base.Establish_context ();
			m_Because_of_Exception_Expected = false;
			m_App = App.CreateDefaultInstance();
		}

		[Test]
		public void then_unexpected_exception_should_not_be_thrown ()
		{
			if (m_Because_of_Exception_Expected == null) return;

			if (m_Because_of_Exception_Expected.Value)
				Assert.IsNotNull (m_Because_of_Exception);
			else
				Assert.IsNull (m_Because_of_Exception);
		}
	}

	public class when_GildedRoseApp_is_instantiated : SpecBase
	{
		protected App m_App;
		protected Exception m_Because_of_Exception;

		protected override void Because_of ()
		{
			try
			{
				m_App = App.CreateDefaultInstance ();
			}
			catch (Exception ex)
			{
				m_Because_of_Exception = ex;
			}
		}

		[Test]
		public void then_no_exceptions_should_be_thrown ()
		{
			Assert.IsNull (m_Because_of_Exception);
		}

		[Test]
		public void then_app_should_have_items ()
		{
			Assert.IsNotNull (m_App.Items);
			Assert.IsTrue (m_App.Items.Any ());
		}
	}

	public abstract class when_UpdateQuality_is_called : GildedRoseSpecification
	{
		protected Item m_Item;
		protected int m_InitialItemQuality;
		protected int m_InitialItemSellIn;
		protected int m_NumTimesCalled = 1;

		protected override void Because_of ()
		{
			try
			{
				for (var i = 0; i < m_NumTimesCalled; i++)
					m_App.UpdateQuality ();
			}
			catch (Exception ex)
			{
				m_Because_of_Exception = ex;
			}
		}

	}

	public abstract class when_UpdateQuality_is_called_on_non_legendary_items : when_UpdateQuality_is_called
	{
		[Test]
		public void then_SellIn_should_decrease ()
		{
			Assert.AreEqual (m_InitialItemSellIn - m_NumTimesCalled, m_Item.SellIn);
		}
	}

	public abstract class when_UpdateQuality_is_called_with_normal_item : when_UpdateQuality_is_called_on_non_legendary_items
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_Item = new Item
				         {
					         Name = "Normal item",
					         Quality = m_InitialItemQuality,
					         SellIn = m_InitialItemSellIn,
				         };

			m_App.Items.Clear();
			m_App.Items.Add (m_Item);
		}
	}

	public class when_UpdateQuality_is_called_with_normal_item_with_nonzero_Quality_and_positive_SellIn : when_UpdateQuality_is_called_with_normal_item
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 10;
			m_InitialItemSellIn = 10;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_decrease_by_1 ()
		{
			Assert.AreEqual (m_InitialItemQuality - 1, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_normal_item_with_0_Quality : when_UpdateQuality_is_called_with_normal_item
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 10;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_equal_0 ()
		{
			Assert.AreEqual (0, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_normal_item_with_nonzero_Quality_and_0_SellIn : when_UpdateQuality_is_called_with_normal_item
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 10;
			m_InitialItemSellIn = 0;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_decrease_by_2 ()
		{
			Assert.AreEqual (m_InitialItemQuality - 2, m_Item.Quality);
		}
	}

	public abstract class when_UpdateQuality_is_called_with_AgedBrie : when_UpdateQuality_is_called_on_non_legendary_items
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_Item = new Item
				               {
					               Name = "Aged Brie",
								   Quality = m_InitialItemQuality,
								   SellIn = m_InitialItemSellIn,
				               };

			m_App.Items.Clear();
			m_App.Items.Add (m_Item);
		}
	}

	public class when_UpdateQuality_is_called_with_AgedBrie_with_0_Quality_and_positive_SellIn : when_UpdateQuality_is_called_with_AgedBrie
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 1;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_increase_by_1 ()
		{
			Assert.AreEqual (m_InitialItemQuality + 1, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_AgedBrie_with_50_Quality : when_UpdateQuality_is_called_with_AgedBrie
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 50;
			m_InitialItemSellIn = 0;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_equal_50 ()
		{
			Assert.AreEqual (50, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_AgedBrie_with_lte_48_quality_and_0_SellIn : when_UpdateQuality_is_called_with_AgedBrie
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 0;

			base.Establish_context ();
		}

		/// <summary>
		/// This rule was not explicitly stated, but found via testing
		/// </summary>
		[Test]
		public void then_Quality_should_increase_by_2 ()
		{
			Assert.AreEqual (m_InitialItemQuality + 2, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_AgedBrie_with_49_quality_and_0_SellIn : when_UpdateQuality_is_called_with_AgedBrie
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 49;
			m_InitialItemSellIn = 0;

			base.Establish_context ();
		}

		/// <summary>
		/// This rule was not explicitly stated, but found via testing
		/// </summary>
		[Test]
		public void then_Quality_should_be_50 ()
		{
			Assert.AreEqual (50, m_Item.Quality);
		}
	}

	public abstract class when_UpdateQuality_is_called_on_legendary_item : when_UpdateQuality_is_called
	{
		[Test]
		public void then_Quality_should_not_change ()
		{
			Assert.AreEqual (m_InitialItemQuality, m_Item.Quality);
		}

		[Test]
		public void then_SellIn_should_not_change ()
		{
			Assert.AreEqual (m_InitialItemSellIn, m_Item.SellIn);
		}
	}

	public abstract class when_UpdateQuality_is_called_with_Sulfuras : when_UpdateQuality_is_called_on_legendary_item
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_Item = new Item
			{
				Name = "Sulfuras, Hand of Ragnaros",
				Quality = m_InitialItemQuality,
				SellIn = m_InitialItemSellIn,
			};

			m_App.Items.Clear ();
			m_App.Items.Add (m_Item);
		}

	}

	public class when_UpdateQuality_is_called_with_Sulfuras_with_positive_SellIn: when_UpdateQuality_is_called_with_Sulfuras
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 80;
			m_InitialItemSellIn = 1;

			base.Establish_context ();
		}
	}

	public class when_UpdateQuality_is_called_with_Sulfuras_with_0_SellIn: when_UpdateQuality_is_called_with_Sulfuras
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 80;
			m_InitialItemSellIn = 0;

			base.Establish_context ();
		}
	}

	public abstract class when_UpdateQuality_is_called_with_BackstagePasses : when_UpdateQuality_is_called_on_non_legendary_items
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_Item = new Item
			{
				Name = "Backstage passes to a TAFKAL80ETC concert",
				Quality = m_InitialItemQuality,
				SellIn = m_InitialItemSellIn,
			};

			m_App.Items.Clear ();
			m_App.Items.Add (m_Item);
		}
	}

	public class when_UpdateQuality_is_called_with_BackstagePasses_with_gt_10_SellIn : when_UpdateQuality_is_called_with_BackstagePasses
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 11;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_increase_by_1 ()
		{
			Assert.AreEqual (m_InitialItemQuality + 1, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_BackstagePasses_with_lte_10_SellIn : when_UpdateQuality_is_called_with_BackstagePasses
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 10;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_increase_by_2 ()
		{
			Assert.AreEqual (m_InitialItemQuality + 2, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_BackstagePasses_with_lte_5_SellIn : when_UpdateQuality_is_called_with_BackstagePasses
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 5;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_increase_by_3 ()
		{
			Assert.AreEqual (m_InitialItemQuality + 3, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_BackstagePasses_with_lte_0_SellIn : when_UpdateQuality_is_called_with_BackstagePasses
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 50;
			m_InitialItemSellIn = 0;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_be_0 ()
		{
			Assert.AreEqual (0, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_BackstagePasses_with_0_Quality_and_40_SellIn_over_40_days : when_UpdateQuality_is_called_with_BackstagePasses
	{
		protected override void Establish_context ()
		{
			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 40;
			m_NumTimesCalled = 40;

			base.Establish_context ();
		}

		[Test]
		public void then_Quality_should_be_50 ()
		{
			Assert.AreEqual (50, m_Item.Quality);
		}
	}

	public abstract class when_GetItemType_is_called : GildedRoseSpecification
	{
		protected String m_ItemName;
		protected ItemType m_ExpectedItemType;

		protected Item m_Item;
		protected ItemType? m_ReturnedItemType;

		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_Item = new Item {Name = m_ItemName};
		}

		protected override void Because_of ()
		{
			try
			{
				m_ReturnedItemType = m_App.GetItemType (m_Item);
			}
			catch (Exception ex)
			{
				m_Because_of_Exception = ex;
			}
		}

		[Test]
		public void then_expected_itemtype_should_be_returned ()
		{
			Assert.IsNotNull (m_ReturnedItemType);
			Assert.AreEqual (m_ExpectedItemType, m_ReturnedItemType.Value);
		}
	}

	//public class when_GetItemType_is_called_with_AgedBrie : when_GetItemType_is_called
	//{
	//	protected override void Establish_context ()
	//	{
	//		m_ItemName = "Aged Brie";
	//		m_ExpectedItemType = ItemType.Unique;

	//		base.Establish_context ();
	//	}
	//}

	public class when_GetItemType_is_called_with_BackstagePasses : when_GetItemType_is_called
	{
		protected override void Establish_context ()
		{
			m_ItemName = "Backstage passes to a TAFKAL80ETC concert";
			m_ExpectedItemType = ItemType.Unique;

			base.Establish_context ();
		}
	}

	public class when_GetItemType_is_called_with_Sulfuras : when_GetItemType_is_called
	{
		protected override void Establish_context ()
		{
			m_ItemName = "Sulfuras, Hand of Ragnaros";
			m_ExpectedItemType = ItemType.Legendary;

			base.Establish_context ();
		}
	}
}