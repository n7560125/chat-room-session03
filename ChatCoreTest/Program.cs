using System;
using System.Text;

namespace ChatCoreTest
{
  internal class Program
  {
    private static byte[] m_PacketData;
    private static uint m_Pos;
        private static byte[] m_PackInt;
        private static byte[] m_PackFloat;
        private static byte[] m_PackString;
        private static byte[] m_PackLength;
    public static void Main(string[] args)
    {
      m_PacketData = new byte[1024];
      m_Pos = 0;

      Write(109);
      Write(109.99f);
      Write("Hello!");
            //add length to array
            AddLength(m_PacketData, m_Pos);
      Console.Write($"Output Byte array(length:{m_Pos}): ");
      
            
            for (var i = 0; i < m_Pos+4; i++)
      
            {
        Console.Write(m_PacketData[i] + ", ");
      
            }
            //unpack package to four packages;
            m_PackLength = new byte[4];
            m_PackInt = new byte[4];
            m_PackFloat = new byte[4];
            m_PackString = new byte[12];
            for(int i = 0; i < m_Pos+4; i++)
            {
                if (i < 4)
                {
                    m_PackLength[i] = m_PacketData[i];
                    continue;
                }
                if (i < 8)
                {
                    m_PackInt[i-4] = m_PacketData[i];
                    continue;
                }
                else if (i < 12)
                {
                    m_PackFloat[i-8] = m_PacketData[i];
                    continue;
                }
                else if (i>15)
                {
                    m_PackString[i-16] = m_PacketData[i];
                    continue;
                }
            }
            //reverse
            LittleEndian(m_PackFloat);
            LittleEndian(m_PackInt);
            LittleEndian(m_PackString);
            LittleEndian(m_PackLength);

            //wrirte;
            Console.WriteLine("");
            Console.WriteLine(m_Pos);
            Console.WriteLine(BitConverter.ToInt32(m_PackInt,0));
            Console.WriteLine(BitConverter.ToSingle(m_PackFloat,0));
            Console.WriteLine( System.Text.Encoding.Unicode.GetString(m_PackString));
            
    }

    // write an integer into a byte array
    private static bool Write(int i)
    {
      // convert int to byte array
      var bytes = BitConverter.GetBytes(i);
            //Console.WriteLine(BitConverter.ToInt32(bytes,0));
      _Write(bytes);
      return true;
    }

    // write a float into a byte array
    private static bool Write(float f)
    {
      // convert int to byte array
      var bytes = BitConverter.GetBytes(f);
      _Write(bytes);
      return true;
    }

    // write a string into a byte array
    private static bool Write(string s)
    {
      // convert string to byte array
      var bytes = Encoding.Unicode.GetBytes(s);

      // write byte array length to packet's byte array
      if (Write(bytes.Length) == false)
      {
        return false;
      }

      _Write(bytes);
      return true;
    }

    // write a byte array into packet's byte array
    private static void _Write(byte[] byteData)
    {
            //converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }
            byteData.CopyTo(m_PacketData, m_Pos);
      m_Pos += (uint)byteData.Length;
    }
        /// <summary>
        /// reverse func.
        /// </summary>
        /// <param name="byteData"></param>
        private static void LittleEndian(byte[] byteData)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }
        }
        /// <summary>
        /// add length func.
        /// </summary>
        /// <param name="byteData"></param>
        /// <param name="length"></param>
        private static void AddLength(byte[] byteData, uint length)
        {
            var data = new byte[1024];
            for(int x = 0; x<length; x++)
            {
                data[x] = byteData[x];
            }
            var bytes = BitConverter.GetBytes(length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            for(int i = 0; i < 4; i++)
            {
                byteData[i] = bytes[i];
            }
            for(int j = 0; j < length; j++)
            {
                byteData[j + 4] = data[j];
            }
        }
    }
    
}
