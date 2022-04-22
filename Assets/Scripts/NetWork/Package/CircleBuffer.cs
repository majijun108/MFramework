using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lockstep.NetWork
{
    /// <summary>
    /// 循环写入和读取流 将数据分包 分次发送
    /// </summary>
    public class CircleBuffer
    {

        public long Length 
        { get 
            {
                int c = 0;
                if (bufferQueue.Count > 0)
                {
                    c = (this.bufferQueue.Count - 1) * ChunkSize + Top - Base;
                }
                return c;
            } 
        }

        public int Top { get; set; }//最后一个可用的位置
        public int Base { get; set; }

        public int ChunkSize = 8192;//每一个内存块的大小

        private readonly Queue<byte[]> bufferQueue = new Queue<byte[]>();
        private readonly Queue<byte[]> cacheQueue = new Queue<byte[]>();
        private byte[] writeBuffer;//当前写入流
        private byte[] readBuffer;//当前读取流

        public CircleBuffer() 
        {
            this.ApplyChunk();
            this.readBuffer = writeBuffer;
            this.Top = 0;
            this.Base = 0;
        }


        public CircleBuffer(int chunkSize) 
        {
            this.ChunkSize = chunkSize;
            this.ApplyChunk();
            this.readBuffer = writeBuffer;
            this.Top = 0;
            this.Base = 0;
        }

        //添加一个当前可用内存块
        void ApplyChunk() 
        {
            byte[] buffer;
            if (this.cacheQueue.Count > 0)
            {
                buffer = this.cacheQueue.Dequeue();
            }
            else 
            {
                buffer=new byte[this.ChunkSize];
            }
            this.bufferQueue.Enqueue(buffer);
            this.writeBuffer = buffer;
        }

        bool RecycleChunk() 
        {
            if (readBuffer == writeBuffer)
            {
                return false;
            }
            this.cacheQueue.Enqueue(this.bufferQueue.Dequeue());//回收最开始的
            readBuffer = this.bufferQueue.Peek();
            return true;
        }


        public void Write(byte[] buffer, int offset, int count)
        {
            int buffIndex = offset;
            int maxIndex = offset + count;
            while (buffIndex < maxIndex) 
            {
                int copuCount = maxIndex - buffIndex;
                if (ChunkSize - Top > copuCount) //内存足够 
                {
                    Array.Copy(buffer, buffIndex, writeBuffer, Top, copuCount);
                    Top += copuCount;
                    buffIndex += copuCount;
                }
                else 
                {
                    int n = ChunkSize - Top;
                    Array.Copy(buffer, buffIndex, writeBuffer, Top,n);
                    Top += n;
                    buffIndex += n;
                }

                if (Top == ChunkSize) //当前块已经用完 扩容
                {
                    ApplyChunk();
                    Top = 0;
                }
            }
        }

        public void Write(byte[] buffer) 
        {
            this.Write(buffer, 0, buffer.Length);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset + count) 
            {
                throw new Exception("buff leng < count");
            }
            long length = this.Length;
            if (length < count) 
            {
                count = (int)length;//限制最大写入量 不能超过buff长度
            }

            int startIndex = offset;
            int maxIndex = startIndex + count;
            while (startIndex < maxIndex)
            {
                int n = maxIndex - startIndex;
                if (ChunkSize - Base > n)
                {
                    Array.Copy(readBuffer, Base, buffer, startIndex, n);
                    Base += n;
                    startIndex += n;
                }
                else 
                {
                    n = ChunkSize - Base;
                    Array.Copy(readBuffer, Base, buffer, startIndex, n);
                    Base += n;
                    startIndex += n;
                }

                if (Base == ChunkSize) 
                {
                    RecycleChunk();
                    Base = 0;
                }
            }

            return count;
        }

        //UDP发送用 UDP每条消息都是完整的
        public int ReadMsg(byte[] buffer) 
        {
            if (Length < 2)//长度小于2 不可能是一条完整的消息
                throw new Exception("UDP Msg Error");
            this.Read(buffer,0,3);
            ushort length = BitConverter.ToUInt16(buffer, 0);
            int readLength = this.Read(buffer, 3, length - 0b11);
            if (readLength != length - 0b11) 
            {
                throw new Exception("UDP Msg Error,length error");
            }
            return length;
        }

        /// <summary>
        /// 读取到stream流中 主要是写入到网络流中
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task ReadToStreamAsync(Stream stream) 
        {
            int sendSize = ChunkSize - Base;
            if (sendSize > this.Length) 
            {
                sendSize = (int)this.Length;
            }

            await stream.WriteAsync(readBuffer,Base,sendSize);
            Base += sendSize;
            if (Base == ChunkSize) 
            {
                RecycleChunk();
                Base = 0;
            }
        }

        /// <summary>
        /// 从流中写入到内存
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<int> WriteFromStreamAsync(Stream stream) 
        {
            int size = ChunkSize - Top;
            int n = await stream.ReadAsync(writeBuffer, Top, size);
            if(n == 0)
                return 0;

            Top += n;
            if (Top == ChunkSize) 
            {
                ApplyChunk();
                Top = 0;
            }
            return n;
        }
    }
}
