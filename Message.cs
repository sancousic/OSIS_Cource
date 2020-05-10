using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSIS_Cource
{
    class Message
    {
        public int IdPoint { get; set; }
        public int IdSource { get; set; }
        public int Counter { get; set; }
        public bool IsLast { get; set; }
        public MessageType MessageType { get; set; }
        public byte[] Key { get; set; } = new byte[7];
        public byte[] Data { get; set; } = new byte[0];
        public int Length { get => sizeof(int) * 4 + Key.Length + 1 + Data.Length; }
        public static Message FromByteArray(byte[] source, int len)
        {
            var intSize = sizeof(int) / sizeof(byte);
            return new Message()
            {
                IdPoint = BitConverter.ToInt32(source, 0),
                IdSource = BitConverter.ToInt32(source, intSize),
                Counter = BitConverter.ToInt32(source, intSize * 2),
                MessageType = (MessageType)BitConverter.ToInt32(source, intSize * 3),
                IsLast = BitConverter.ToBoolean(source, intSize * 4),
                Key = source.Skip(intSize * 4 + 1).Take(7).ToArray(),
                Data = source.Skip(intSize * 4 + 7 + 1).Take(len - (intSize * 4 + 7 + 1)).ToArray()
            };
        }
        public byte[] ToByteArray()
        {
            var intSize = sizeof(int) / sizeof(byte);
            var res = new byte[intSize * 4 + 1 + Key.Length + Data.Length];
            BitConverter.GetBytes(IdPoint).CopyTo(res, 0);
            BitConverter.GetBytes(IdSource).CopyTo(res, intSize);
            BitConverter.GetBytes(Counter).CopyTo(res, intSize * 2);
            BitConverter.GetBytes((int)this.MessageType).CopyTo(res, intSize * 3);
            BitConverter.GetBytes(IsLast).CopyTo(res, intSize * 4);
            Key.CopyTo(res, intSize * 4 + 1);
            Data.CopyTo(res, intSize * 4 + Key.Length + 1);
            return res;
        }
    }
}
