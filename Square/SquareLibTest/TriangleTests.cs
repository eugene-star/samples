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
			var triangle = new Triangle();
			Assert.AreEqual(triangle.Square(), 0);
		}

		[Test]
		public void TriangleTest2()
		{
			var triangle = new Triangle() { SideA = 2, SideB = 2, SideC = 2 };
			Assert.AreEqual(triangle.Square(), Math.Sqrt(3));
		}

		[Test]
		public void TriangleTest3()
		{
			var triangle = new Triangle() { SideA = 2, SideB = 2, SideC = 2 };
			Assert.AreEqual(triangle.IsRight(), false);
		}

		[Test]
		public void TriangleTest4()
		{
			var triangle = new Triangle() { SideA = 3, SideB = 4, SideC = 5 };
			Assert.AreEqual(triangle.IsRight(), true);
		}

		[Test]
		public void TriangleTest5()
		{
			var triangle = new Triangle() { SideA = -1 };

			try
			{
				triangle.Square();
			}
			catch (ArgumentException)
			{
				Assert.Pass();
			}

			Assert.Fail();
		}

		[Test]
		public void TriangleTest6()
		{
			var triangle = new Triangle() { SideA = 3, SideB = 4, SideC = 8 };

			try
			{
				triangle.Square();
			}
			catch (ArgumentException)
			{
				Assert.Pass();
			}

			Assert.Fail();
		}
	}
}