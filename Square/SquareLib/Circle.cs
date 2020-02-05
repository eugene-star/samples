using System;

namespace SquareLib
{
	public class Circle : IShape
	{
		public double Radius { get; set; }

		public double Square()
		{
			if (Radius < 0)
				throw new ArgumentException("Radius can't be below zero");

			return Math.PI * Math.Pow(Radius, 2);
		}
	}
}
