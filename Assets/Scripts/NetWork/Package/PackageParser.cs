using System;
using System.Collections.Generic;

namespace Lockstep.NetWork
{
    //数据包 结构 大小2个字节 opcode 1个字节 data
    public struct Packet 
    {
        public byte[] Bytes { get; }

        public static int LengthIndex = 0;
        public static int OpCodeIndex = 2;
        public static int DataIndex = 3;
        public Packet(int length)
        {
            this.Bytes = new byte[length];
        }

        //public Packet(byte opcode, byte[] data) 
        //{
        //    if (data.Length + 3 > ushort.MaxValue) 
        //    {
        //        throw new Exception("packe is too large");
        //    }
        //    ushort length = (ushort)(data.Length + 3);
        //    this.Bytes = new byte[length];

        //}

        public ushort Length 
        {
            get 
            {
                ushort size = BitConverter.ToUInt16(Bytes, LengthIndex);
                return size;
            }
        }

        //数据大小
        public ushort Size 
        {
            get 
            {
                return (ushort)(Length - 3);
            }
        }

        public byte OpCode
        {
            get 
            {
                return Bytes[OpCodeIndex];
            }
        }
    }
    
    public class PackageParser
    {
        enum ParserState 
        {
            PacketSize,
            PacketBody
        }

        private readonly CircleBuffer _circleBuffer;

        private bool _isOK;
        private ParserState _state = ParserState.PacketBody;
        private ushort _packetLength;
        private Packet _packet = new Packet(ushort.MaxValue);

        public PackageParser(CircleBuffer buff) 
        {
            _circleBuffer = buff;
        }

        public bool Parse() 
        {
            if(this._isOK)
                return true;

            bool finish = false;
            while (!finish) 
            {
                switch (this._state) 
                {
                    case ParserState.PacketBody:
                        if (this._circleBuffer.Length < 2)
                            finish = true;
                        else 
                        {
                            this._circleBuffer.Read(this._packet.Bytes, 0, 2);
                            this._packetLength = this._packet.Length;//包的总大小
                            this._state = ParserState.PacketBody;
                        }
                        break;
                    case ParserState.PacketSize:
                        if (this._circleBuffer.Length < this._packetLength)//还未接受完成
                            finish = true;
                        else 
                        {
                            this._circleBuffer.Read(this._packet.Bytes, Packet.OpCodeIndex,1);//1个字节的opcode
                            this._circleBuffer.Read(this._packet.Bytes, Packet.DataIndex, this._packet.Size);
                            this._isOK = true;
                            this._state = ParserState.PacketSize;
                            finish = true;
                        }
                        break;
                }
            }

            return _isOK;
        }

        public Packet GetPacket() 
        {
            this._isOK = false;
            return this._packet;
        }
    }
}
