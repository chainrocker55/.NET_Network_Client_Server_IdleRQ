using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    public class ClientData
    {
        int NUMBER;
        string DATA;
        string CHECKSUM;
        public ClientData(int _id, string _name, string _message)
        {
            this.NUMBER = _id;
            this.DATA = _name;
            this.CHECKSUM = _message;
        }
        public ClientData() { }
        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(NUMBER);
                bw.Write(DATA);
                bw.Write(CHECKSUM);

                return ms.ToArray();
            }
        }

        public static ClientData FromBytes(byte[] buffer)
        {
            ClientData retVal = new ClientData();

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryReader br = new BinaryReader(ms);
                retVal.NUMBER = br.ReadInt32();
                retVal.DATA = br.ReadString();
                retVal.CHECKSUM = br.ReadString();
            }

            return retVal;
        }
    }
}
