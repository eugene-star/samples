using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIAMTest.Objects
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public void AddDetail(OrderDetail od)
        {
            // STUB: здесь реализация добавления строки заказа 
        }
        #region STUB
        // Здесь остальная функциональность заказа
        #endregion
    }

    /// <summary>
    /// Строка заказа
    /// </summary>
    public class OrderDetail
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        #region STUB
        // Здесь остальная функциональность строки заказа
        #endregion
    }
}
