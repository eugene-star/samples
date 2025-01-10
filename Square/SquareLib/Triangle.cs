using System;

namespace SquareLib
{
	public class Triangle : IShape
	{
		decimal _a, _b, _c;

		public decimal A => _a;
		public decimal B => _b; 
		public decimal C => _c;

		public Triangle SetSides(decimal a, decimal b, decimal c)
		{
			if (a < 0 || b < 0 || c < 0)
				throw new ArgumentException("All sides must be non-negative.");

			if (a > b + c || b > a + c || c > b + a)
				throw new ArgumentException("Sides doesn't match to each other.");

			_a = a;
			_b = b;
			_c = c;

			return this;
		}

		public double Square()
		{
			// Heron's formula
			var p = (_a + _b + _c) / 2;
			return Math.Sqrt((double)(p * (p - _a) * (p - _b) * (p - _c)));
		}

		public bool IsRight() =>
			// Pythagorean theorem
			_a * _a == _b * _b + _c * _c ||
			_b * _b == _a * _a + _c * _c ||
			_c * _c == _a * _a + _b * _b;
	}
}
