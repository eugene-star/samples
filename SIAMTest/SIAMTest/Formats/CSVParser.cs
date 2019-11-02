using SIAMTest.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace SIAMTest.Formats
{
    /// <summary>
    /// Класс для извлечения объектов из CSV
    /// </summary>
    public class CSVParser : StreamParser
    {
        /// <summary>
        /// Извлечение объектов типа Order
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override IEnumerable<Order> GetObjects(Stream stream)
        {
            Dictionary<int,Objects.Order> orders = new Dictionary<int,Order>();

            try
            {
                using (var sr = new StreamReader(stream))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(';');
                        int orderId = Int32.Parse(values[0]),
                            customerId = Int32.Parse(values[1]),
                            productId = Int32.Parse(values[2]),
                            amount = Int32.Parse(values[3]);

                        Order order;
                        if (!orders.TryGetValue(orderId, out order))
                        {
                            order = new Order() { Id = orderId, CustomerId = customerId };
                            orders.Add(orderId, order);
                        }
                        order.AddDetail(new OrderDetail() { ProductId = productId, Amount = amount }); 
                    }

                    return orders.Values;
                }
            }
            catch (Exception ex)
            {
                throw new StreamParserException("Ошибка в структуре файла CSV.", ex);
            }
        }
    }
}
