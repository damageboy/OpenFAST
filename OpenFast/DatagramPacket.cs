using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFAST
{
    public sealed class DatagramPacket : SupportClass.PacketSupport
    {
        public DatagramPacket(byte[] array, int length):base(array,length)
        {
            
        }
    }
}
