using System;

namespace SquareLib
{
	public class Triangle : IShape
	{
		public double SideA { get; set; }
		public double SideB { get; set; }
		public double SideC { get; set; }

		public double Square()
		{
			double a = SideA, b = SideB, c = SideC;
				
			if (IsInvalid(a, b, c))
				throw new ArgumentException("Invalid triangle");

			// Heron's formula
			double p = (a + b + c) / 2;
			return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
		}

		public bool IsRight()
		{
			double a = SideA, b = SideB, c = SideC;

			if (IsInvalid(a, b, c))
				throw new ArgumentException("Invalid triangle");

			// Pythagorean theorem
			return (Math.Pow(a, 2) == Math.Pow(b, 2) + Math.Pow(c, 2) ||
					Math.Pow(b, 2) == Math.Pow(a, 2) + Math.Pow(c, 2) ||
					Math.Pow(c, 2) == Math.Pow(b, 2) + Math.Pow(a, 2));
		}

		private static bool IsInvalid(double a, double b, double c)
		{
			return a < 0 || b < 0 || c < 0 ||
				a > b + c || b > a + c || c > b + a;
		}
	}
}
