using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Models
{
    [Serializable]
    public class Message
    {
        public Guid Id { get; set; }
        public MessageType Type { get; set; }
        public string Description { get; set; }

        public Message()
        {
            Id = Guid.NewGuid();
        }

        public Message(byte[] param)
        {
            Deserialize(param);
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        private void Deserialize(byte[] param)
        {
            using (var stream = new MemoryStream(param))
            {
                IFormatter formatter = new BinaryFormatter();
                var temp = formatter.Deserialize(stream) as Message;
                if(temp == null) throw new NullReferenceException(nameof(temp));
                Id = temp.Id;
                Type = temp.Type;
                Description = temp.Description;
            }
        }
    }

    [Serializable]
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }
}
