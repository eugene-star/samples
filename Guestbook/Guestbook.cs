/* Реализация гостевой книги с различными вариантами хранения сообщений
*/

using System.Collections.Generic;
using System.Linq;

namespace Guestbook
{
    public class Message
    {
        public string User { get; set; }
        public string Text { get; set; }
    }

    interface IMessagePersistor
    {
        IEnumerable<Message> GetMessages();
        bool SaveMessage(Message m);
    }

    class Guestbook : IMessagePersistor
    {
        private List<Message> _messageList;
        private IMessagePersistor _persistor;

        /// <summary>
        /// Конструктор. Загружаются добавленные ранее сообщения
        /// </summary>
        public Guestbook(IMessagePersistor persistor)
        {
            _persistor = persistor;
            _messageList = persistor.GetMessages().ToList();
        }

        public IEnumerable<Message> GetMessages()
        {
            return _messageList;
        }

        public bool SaveMessage(Message m)
        {
            _messageList.Add(m);
            return _persistor.SaveMessage(m);
        }
    }
}
