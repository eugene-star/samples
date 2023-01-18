using NUnit.Framework;
using SquareLib;
using System;

namespace Tests
{
	[TestFixture]
	public class TriangleTests
	{
		[Test]
		public void TriangleTest1()
		{
			Assert.Throws<ArgumentException>(() => { new Triangle().SetSides(-1, 0, 0); });
		}		
		
		[Test]
		public void TriangleTest2()
		{
			Assert.Throws<ArgumentException>(() => { new Triangle().SetSides(1, 1, 3); }); 
		}

		[Test]
		public void TriangleTest3()
		{
			Assert.AreEqual((new Triangle()).Square(), 0);
		}

		[Test]
		public void TriangleTest4()
		{
			Assert.AreEqual((new Triangle().SetSides(2, 2, 2)).Square(), Math.Sqrt(3));
		}

		[Test]
		public void TriangleTest5()
		{
			Assert.IsFalse((new Triangle().SetSides(2, 2, 2)).IsRight());
		}

		[Test]
		public void TriangleTest6()
		{
			Assert.IsTrue((new Triangle().SetSides(3, 4, 5)).IsRight());
		}
	}
}