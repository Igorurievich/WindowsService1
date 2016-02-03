using System;
using System.Linq;
using System.Text;

namespace WindowsService
{
    public struct Message
    {
        public DateTime DTime;
        public long IMEI;
        public int Latitude;
        public int Longitude;
        public int MessageCount;
        public int OneRecordLenth;
        public int Speed;

        public Message(long IMEI_, int OneRecordLenth_, int MessageCount_, int Lingitude_, int Latitude_, short Speed_,
            DateTime DTime_)
        {
            IMEI = IMEI_;
            OneRecordLenth = OneRecordLenth_;
            MessageCount = MessageCount_;
            Longitude = Lingitude_;
            Latitude = Latitude_;
            Speed = Speed_;
            DTime = DTime_;
        }

        public DateTime ByteToDateTime(int index, byte[] Buff)
        {
            var tempBuff = new byte[sizeof (long)];
            Buffer.BlockCopy(Buff, index, tempBuff, 0, 8);
            tempBuff = tempBuff.Reverse().ToArray();
            var result = BitConverter.ToInt64(tempBuff, 0);
            //var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            //epoch = epoch.Add(new TimeSpan(result * 10000 + 10800000L * 10000L));
            var dt = new DateTime(1970, 1, 1, 2, 0, 0).Add(TimeSpan.FromMilliseconds(result));
            return dt;
        }

        public int ByteToInt(int index, byte[] Buff)
        {
            var tempBuff = new byte[sizeof (int)];
            Buffer.BlockCopy(Buff, index, tempBuff, 0, 4);
            tempBuff = tempBuff.Reverse().ToArray();
            var result = BitConverter.ToInt32(tempBuff, 0);
            return result;
        }

        public short ByteToShort(int index, byte[] Buff)
        {
            var tempBuff = new byte[sizeof (short)];
            Buffer.BlockCopy(Buff, index, tempBuff, 0, 2);
            tempBuff = tempBuff.Reverse().ToArray();
            var result = BitConverter.ToInt16(tempBuff, 0);
            return result;
        }

        public long GetIMEI(int BytesCount, byte[] Buff)
        {
            long IMEI = 0;
            var Request = Encoding.ASCII.GetString(Buff, 0, BytesCount);
            Request = Request.Substring(2);
            IMEI = Convert.ToInt64(Request);
            return IMEI;
        }

        public int GetOffset()
        {
            var value = MessageCount*OneRecordLenth;
            return value;
        }

        public void InitComponents(byte[] Buff)
        {
            OneRecordLenth = 32;
            MessageCount = Buff[9];
        }
    }
}