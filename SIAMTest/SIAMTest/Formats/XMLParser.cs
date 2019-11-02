using SIAMTest.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SIAMTest.Formats
{
    /// <summary>
    /// Класс для извлечения объектов из XML
    /// </summary>
    class XMLParser : StreamParser
    {
        /// <summary>
        /// Извлечение объектов типа Order
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override IEnumerable<Order> GetObjects(Stream stream)
        {
            XmlDocument doc;
            List<Objects.Order> orders;

            try
            {
                doc = new XmlDocument();
                doc.Load(stream);

                orders = new List<Objects.Order>();

                foreach (XmlNode orderNode in doc.SelectNodes("Orders/Order"))
                {
                    Objects.Order order = new Objects.Order()
                    { 
                        Id = Int32.Parse(orderNode.Attributes["id"].Value),
                        CustomerId = Int32.Parse(orderNode.SelectSingleNode("Customer").Attributes["id"].Value)
                    };

                    foreach (XmlNode detailNode in orderNode.SelectNodes("OrderDetails/Product"))
                    {
                        Objects.OrderDetail od = new Objects.OrderDetail()
                        {
                            ProductId = Int32.Parse(detailNode.Attributes["id"].Value),
                            Amount = Int32.Parse(detailNode.Attributes["amount"].Value)
                        };
                        order.AddDetail(od);
                    }

                    orders.Add(order);
                }
            }
            catch (XmlException ex)
            {
                throw new StreamParserException("Ошибка в структуре XML-файла", ex);
            }
            catch (Exception ex)
            {
                throw new StreamParserException("Ошибка загрузки XML", ex);
            }

            return orders;
        }
    }
}
