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
			Assert.Throws<ArgumentException>(() => new Circle() { Radius = -1 });
		}		
		[Test]
		public void CircleTest2()
		{
			Assert.AreEqual(new Circle().Square(), 0);
		}

		[Test]
		public void CircleTest3()
		{
			var circle = new Circle() { Radius = 2 };
			Assert.AreEqual(circle.Square(), Math.PI * 4);
		}
	}
}