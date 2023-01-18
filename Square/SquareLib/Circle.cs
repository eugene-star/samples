using System;

namespace SquareLib
{
	public class Circle : IShape
	{
		decimal _radius;

		public decimal Radius
		{
			get => _radius;

			set
			{
				if (value < 0)
					throw new ArgumentException("Radius can't be below zero.");

				_radius = value;
			}
		}

		public double Square() => Math.PI * (double)(_radius * _radius);
	}
}
