using System;
using System.Text;

namespace VendingMachine
{
    class VendingMachine
    {
        private VendingMachineForm _view;
        private Purse _purse = new Purse();
        private Store _store = new Store();

        public VendingMachine(VendingMachineForm view)
        {
            _view = view;
            _view.Load += _view_Load;
            _view.AddCoin += _view_AddCoin;
            _view.BuySnack += _view_BuySnack;
            _view.Flush += _view_Flush;

            _purse.AddCoins(1, true, 10);
            _purse.AddCoins(2, true, 30);
            _purse.AddCoins(5, true, 20);
            _purse.AddCoins(10, true, 15);

            _purse.AddCoins(1, false, 100);
            _purse.AddCoins(2, false, 100);
            _purse.AddCoins(5, false, 100);
            _purse.AddCoins(10, false, 100);

            _store.AddSnack(new Snack() { Code = "T", Price = 13 }, 10);
            _store.AddSnack(new Snack() { Code = "C", Price = 18 }, 20);
            _store.AddSnack(new Snack() { Code = "CM", Price = 21 }, 20);
            _store.AddSnack(new Snack() { Code = "J", Price = 35 }, 15);
        }

        void _view_Flush(object sender, EventArgs e)
        {
            var coinsWdr = _purse.Flush();
            var sb = new StringBuilder("Выданные монеты: ");
            foreach (var coin in coinsWdr)
                sb.AppendFormat("{0}₽, ", coin.Value);
            _view.LogMessage(sb.ToString().TrimEnd(' ', ','));
            _view.SetData(_purse);
        }

        void _view_BuySnack(object sender, BuySnackEventArgs e)
        {
            int cnt = _store.GetCount(e.Code);
            if (cnt == 0)
                _view.LogMessage("Не хватает товара");
            else
            {
                int price = _store.GetPrice(e.Code);
                if (price > _purse.Sum(true))
                    _view.LogMessage("Недостаточно внесённой суммы");
                else
                {
                    var coinsWdr = _purse.Buy(price);
                    _store.Issue(e.Code);
                    var sb = new StringBuilder("Спасибо! Использованные монеты: ");
                    foreach (var coin in coinsWdr)
                        sb.AppendFormat("{0}₽, ", coin.Value);
                    _view.LogMessage(sb.ToString().TrimEnd(' ', ','));
                    _view.SetData(_purse);
                    _view.SetData(_store);
                }
            }
        }

        void _view_AddCoin(object sender, AddCoinEventArgs e)
        {
            _purse.AddCoins(e.Value, true, 1);
            _view.SetData(_purse);
        }

        void _view_Load(object sender, EventArgs e)
        {
            _view.SetData(_purse);
            _view.SetData(_store);
        }
    }
}