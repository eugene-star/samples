using System;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachine
{
    class Purse
    {
        enum WithdrawalMode
        {
            Buy,
            Change,
            Flush
        }

        private const string ErrorStr = "Can't withdraw {0}";

        private List<Coin> _coins = new List<Coin>();

        public void AddCoins(int value, bool isUser, int n)
        {
            for (int i = 0; i < n; i++)
                _coins.Add(new Coin(value) { IsUser = isUser });
        }

        public int Sum(bool isUser)
        {
            return _coins.Where(x => x.IsUser == isUser).Sum(x => x.Value);
        }

        public int Count(int value, bool isUser)
        {
            return _coins.Count(x => x.IsUser == isUser && x.Value == value);
        }

        public IEnumerable<Coin> Buy(int sum)
        {
            return Withdraw(sum, WithdrawalMode.Buy);
        }

        public IEnumerable<Coin> Flush()
        {
            return Withdraw(Sum(true), WithdrawalMode.Flush);
        }

        private IEnumerable<Coin> Withdraw(int sum, WithdrawalMode mode)
        {
            var result = new List<Coin>();

            while (sum > 0)
            {
                Coin coin;

                switch (mode)
                {
                    case WithdrawalMode.Buy:
                        coin = GetCoinForBuy(sum);
                        coin.IsUser = false;
                        break;

                    case WithdrawalMode.Change:
                        coin = GetCoinForChange(sum);
                        coin.IsUser = true;
                        break;

                    case WithdrawalMode.Flush:
                        coin = GetCoinForFlush(sum);
                        _coins.Remove(coin);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                result.Add(coin);
                sum -= coin.Value;
            }

            return result;
        }

        private Coin GetCoinForBuy(int sum)
        {
            var values = _coins.Where(x => x.IsUser).GroupBy(x => x.Value).OrderBy(x => x.Key).Reverse();
            foreach (var value in values)
                if (sum >= value.Key)
                    return value.First();

            Change(values.Last().First(), true);
            return GetCoinForBuy(sum);
        }

        private Coin GetCoinForFlush(int sum)
        {
            var values = _coins.Where(x => x.Value <= sum)
                .GroupBy(x => x.Value).OrderBy(x => x.Key).Last();

            var coin = values.FirstOrDefault(x => x.IsUser);
            if (coin == null)
            {
                coin = values.First();
                Change(coin, false);
            }

            return coin;
        }

        private Coin GetCoinForChange(int sum)
        {
            Coin coin;

            if (sum == Coin.MinimumValue)
                coin = _coins.Where(x => !x.IsUser && x.Value == Coin.MinimumValue).First();
            else
            {
                var values = _coins.Where(x => x.Value < sum && !x.IsUser)
                    .GroupBy(x => x.Value).OrderBy(x => x.Key).LastOrDefault();
                if (values == null)
                    throw new Exception(string.Format(ErrorStr, sum));
                coin = values.FirstOrDefault();
            }

            if (coin == null)
                throw new Exception(string.Format(ErrorStr, sum));

            return coin;
        }

        private void Change(Coin coin, bool buyMode)
        {
            var coinsToChange = Withdraw(coin.Value, buyMode ? WithdrawalMode.Change : WithdrawalMode.Buy);
            foreach (var c in coinsToChange)
                c.IsUser = buyMode;
            coin.IsUser = !buyMode;
        }
    }
}