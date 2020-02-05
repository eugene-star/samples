using NUnit.Framework;
using SquareLib;
using System;

namespace Tests
{
	[TestFixture]
	public class CircleTests
	{
		[Test]
		public void CircleTest1()
		{
			var circle = new Circle();
			Assert.AreEqual(circle.Square(), 0);
		}

		[Test]
		public void CircleTest2()
		{
			var circle = new Circle() { Radius = 2 };
			Assert.AreEqual(circle.Square(), Math.PI * 4);
		}

		[Test]
		public void CircleTest3()
		{
			var circle = new Circle() { Radius = -1 };

			try
			{
				circle.Square();
			}
			catch(ArgumentException)
			{
				Assert.Pass();
			}

			Assert.Fail();
		}
	}
}