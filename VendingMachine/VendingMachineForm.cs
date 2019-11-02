using System;
using System.Windows.Forms;

namespace VendingMachine
{
    public partial class VendingMachineForm : Form
    {
        public event EventHandler<AddCoinEventArgs> AddCoin;
        public event EventHandler<BuySnackEventArgs> BuySnack;
        public event EventHandler Flush;

        public VendingMachineForm()
        {
            InitializeComponent();
        }

        internal void SetData(Purse purse)
        {
            labelUser1R.Text = purse.Count(1, true).ToString();
            labelUser2R.Text = purse.Count(2, true).ToString();
            labelUser5R.Text = purse.Count(5, true).ToString();
            labelUser10R.Text = purse.Count(10, true).ToString();
            labelUserSum.Text = purse.Sum(true) + "₽";

            label1R.Text = purse.Count(1, false).ToString();
            label2R.Text = purse.Count(2, false).ToString();
            label5R.Text = purse.Count(5, false).ToString();
            label10R.Text = purse.Count(10, false).ToString();
            labelSum.Text = purse.Sum(false) + "₽";
        }

        internal void SetData(Store store)
        {
            labelTea.Text = store.GetCount("T").ToString();
            labelCoffee.Text = store.GetCount("C").ToString();
            labelCoffeeMilk.Text = store.GetCount("CM").ToString();
            labelJuice.Text = store.GetCount("J").ToString();
        }

        public void LogMessage(string msg)
        {
            textBoxLog.AppendText(msg + "\r\n");
        }

        #region Event handlers

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (AddCoin != null)
            {
                var button = sender as Button;
                int value = int.Parse(button.Tag.ToString());
                AddCoin(this, new AddCoinEventArgs() { Value = value });
            }
        }

        private void buttonBuy_Click(object sender, EventArgs e)
        {
            if (BuySnack != null)
            {
                var button = sender as Button;
                BuySnack(this, new BuySnackEventArgs() { Code = button.Tag as string });
            }
        }

        private void buttonFlush_Click(object sender, EventArgs e)
        {
            if (Flush != null)
                Flush(this, EventArgs.Empty);
        }

        #endregion
    }

    public class AddCoinEventArgs : EventArgs
    {
        public int Value;
    }

    public class BuySnackEventArgs : EventArgs
    {
        public string Code;
    }
}
