﻿using System;
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

		protected override void Establish_context ()
		{
			base.Establish_context ();
			m_App = App.CreateDefaultInstance();
		}
	}

	public class when_GildedRoseApp_is_instantiated : GildedRoseSpecification
	{
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

		[Test]
		public void then_SellIn_should_decrease ()
		{
			Assert.AreEqual (m_InitialItemSellIn - m_NumTimesCalled, m_Item.SellIn);
		}
	}

	public class when_UpdateQuality_is_called_with_normal_item_with_nonzero_Quality_and_positive_SellIn : when_UpdateQuality_is_called
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_InitialItemQuality = 10;
			m_InitialItemSellIn = 10;
			m_Item = new Item
				               {
					               Name = "Normal item",
								   Quality = m_InitialItemQuality,
								   SellIn = m_InitialItemSellIn,
				               };

			m_App.Items.Clear();
			m_App.Items.Add (m_Item);
		}

		[Test]
		public void then_Quality_should_decrease_by_1 ()
		{
			Assert.AreEqual (m_InitialItemQuality - 1, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_normal_item_with_zero_Quality_and_positive_SellIn : when_UpdateQuality_is_called
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_InitialItemQuality = 0;
			m_InitialItemSellIn = 10;
			m_Item = new Item
				               {
					               Name = "Normal item",
								   Quality = m_InitialItemQuality,
								   SellIn = m_InitialItemSellIn,
				               };

			m_App.Items.Clear();
			m_App.Items.Add (m_Item);
		}

		[Test]
		public void then_Quality_should_equal_0 ()
		{
			Assert.AreEqual (0, m_Item.Quality);
		}
	}

	public class when_UpdateQuality_is_called_with_normal_item_with_nonzero_Quality_and_zero_SellIn : when_UpdateQuality_is_called
	{
		protected override void Establish_context ()
		{
			base.Establish_context ();

			m_InitialItemQuality = 10;
			m_InitialItemSellIn = 0;
			m_Item = new Item
				               {
					               Name = "Normal item",
								   Quality = m_InitialItemQuality,
								   SellIn = m_InitialItemSellIn,
				               };

			m_App.Items.Clear();
			m_App.Items.Add (m_Item);
		}

		[Test]
		public void then_Quality_should_decrease_by_2 ()
		{
			Assert.AreEqual (m_InitialItemQuality - 2, m_Item.Quality);
		}
	}
}