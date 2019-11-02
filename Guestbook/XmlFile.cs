using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Guestbook
{
    public class XmlFile : IMessagePersistor
    {
        private const string c_xmlFileName = "messages.xml";

        private XmlDocument _xml;

        public IEnumerable<Message> GetMessages()
        {
            List<Message> messageList = new List<Message>();

            _xml = new XmlDocument();
            try
            {
                _xml.Load(c_xmlFileName);
                foreach (XmlNode node in _xml.SelectNodes("guestbook/message"))
                {
                    messageList.Add(new Message()
                    {
                        User = node.Attributes["user"].Value,
                        Text = node.InnerText
                    });
                }
            }
            catch(FileNotFoundException)
            {
                _xml.AppendChild(_xml.CreateElement("guestbook"));
            }

            return messageList;
        }

        public bool SaveMessage(Message m)
        {
            try
            {
                // добавим узел, текст в секции CDATA
                XmlNode node = _xml.CreateElement("message");
                node.AppendChild(_xml.CreateCDataSection(m.Text));

                // имя пользователя в атрибуте узла
                XmlAttribute attr = node.Attributes.Append(_xml.CreateAttribute("user"));
                attr.Value = m.User;

                _xml.DocumentElement.AppendChild(node);

                // сохраним в файл
                _xml.Save(c_xmlFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
