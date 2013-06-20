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
		protected GildedRose.Console.App m_App;
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
	}

	public abstract class when_UpdateQuality_is_called : GildedRoseSpecification
	{
		protected int m_NumTimesCalled = 1;

		protected override void Establish_context ()
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
}
