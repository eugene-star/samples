using System;

namespace VendingMachine
{
    class Coin
    {
        // сделаем предположение, что все суммы в рублях будут целочисленными
        // если мы хотим считать с копейками, есть 2 варианта: считать в целых копейках
        // или использовать тип decimal, который в данном случае чересчур избыточен
        private readonly int _value;

        public const int MinimumValue = 1;

        public int Value
        {
            get
            {
                return _value;
            }
        }

        public bool IsUser { get; set; }

        public Coin(int value)
        {
            if (value < MinimumValue)
                throw new Exception("Invalid coin value");
            _value = value;
        }
    }
}
