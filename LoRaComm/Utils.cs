using System;
using System.Text;


    public static class Utils
    {
        public static string ToHexString(byte[] bytes, int index, int count)
        {
            var result = new StringBuilder(bytes.Length * 2);

            for (int i = index; i < index + count; i++)
            {
                result.AppendFormat("{0:x2}", bytes[i]);
            }
            return result.ToString();
        }

        public static uint RandomUint(uint min, uint max)
        {
            return (BitConverter.ToUInt32(RandomBytes(sizeof(uint)), 0) % (max - min)) + min;
        }

        public static ushort RandomUshort()
        {            
            return BitConverter.ToUInt16(RandomBytes(2), 0);
        }

        public static byte[] RandomBytes(int length)
        {
            byte[] rand = new byte[length];
            new Random().NextBytes(rand);
            return rand;
        }
    }
