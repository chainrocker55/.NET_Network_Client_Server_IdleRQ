using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Idle_RQ
{
    class PackageData
    {
        public int NUMBER { get; set; }
        public string DATA { get; set; }
        public string MD5 { get; set; }

        public PackageData(int _id, string _name, string _message)
        {
            this.NUMBER = _id;
            this.DATA = _name;
            this.MD5 = _message;
        }
        public PackageData() { }
        public PackageData(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            BinaryReader br = new BinaryReader(ms);
            NUMBER = br.ReadInt32();
            DATA = br.ReadString();
            MD5 = br.ReadString();
        }
        public byte[] ToByteArray()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(NUMBER);
                bw.Write(DATA);
                bw.Write(MD5);

                return ms.ToArray();
            }
        }

        public static PackageData FromBytes(byte[] buffer)
        {
            PackageData retVal = new PackageData();

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryReader br = new BinaryReader(ms);
                retVal.NUMBER = br.ReadInt32();
                retVal.DATA = br.ReadString();
                retVal.MD5 = br.ReadString();
            }

            return retVal;
        }
    }
}
