using System;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachine
{
    class Snack
    {
        public string Code;
        public int Price;
    }

    class Store
    {
        private List<Snack> _assortment = new List<Snack>();
        private Dictionary<string, int> _stock = new Dictionary<string, int>();

        public void AddSnack(Snack snack, int n)
        {
            if (_stock.ContainsKey(snack.Code))
                _stock[snack.Code] += n;
            else
            {
                _assortment.Add(snack);
                _stock.Add(snack.Code, n);
            }
        }

        public int GetCount(string code)
        {
            return _stock.ContainsKey(code) ? _stock[code] : 0;
        }

        public int GetPrice(string code)
        {
            var snack = _assortment.FirstOrDefault(x => x.Code == code);
            if (snack == null)
                throw new Exception("Unknown code " + code);
            return snack.Price;
        }

        public Snack Issue(string code)
        {
            var snack = _assortment.FirstOrDefault(x => x.Code == code);
            if (snack == null)
                throw new Exception("Unknown code " + code);
            if (_stock.ContainsKey(snack.Code))
            {
                if (_stock[snack.Code] > 0)
                    _stock[snack.Code] -= 1;
                else
                    throw new Exception("Unsufficient " + code);
            }
            else 
                throw new Exception("Unknown stock for " + code);

            return snack;
        }
    }
}