using System;
using System.Collections.Generic;

public interface IFrameBuffer
{
    int CurtTickInServer { get; }
    int NextTickToCheck { get; }
    int MaxServerTickInBuffer { get; }
    int MaxContinueServerTick { get; }
    public bool IsNeedRollback { get; }

    void PushServerFrame(Msg_FrameInfo frame);
    void PushServerFrame(Msg_FrameInfo[] frames);
    void PushLocalFrame(Msg_FrameInfo frame);
    Msg_FrameInfo GetServerFrame(int tick);
    Msg_FrameInfo GetLocalFrame(int tick);
    void FrameCheck(float deltaTime);
}

public class FrameBuffer : IFrameBuffer
{
    public int CurtTickInServer { get; private set; }
    public int NextTickToCheck { get; private set; }
    public int MaxServerTickInBuffer { get; private set; }
    public int MaxContinueServerTick { get; private set; }
    public bool IsNeedRollback { get; private set; }

    private int m_maxServerOverFrameCount;//最大帧缓存
    private int m_bufferSize;
    private Msg_FrameInfo[] m_serverBuffer;//服务器帧
    private Msg_FrameInfo[] m_clientBuffer;//本地帧
    private World m_world;

    public FrameBuffer(World world,int buffSize) 
    {
        m_world = world;
        m_bufferSize = buffSize;
        m_maxServerOverFrameCount = buffSize;
        m_serverBuffer = new Msg_FrameInfo[m_bufferSize];
        m_clientBuffer = new Msg_FrameInfo[m_bufferSize];
    }

    private int GetFrameIndex(int tick) 
    {
        return tick % m_bufferSize;
    }

    public void PushServerFrame(Msg_FrameInfo frame)
    {
        if (frame.Tick < NextTickToCheck)
            return;
        if(frame.Tick > CurtTickInServer)
            CurtTickInServer = frame.Tick;

        if (frame.Tick >= NextTickToCheck + m_maxServerOverFrameCount - 1)//超过最大缓存
            return;

        if(frame.Tick > MaxServerTickInBuffer)
            MaxServerTickInBuffer = frame.Tick;
        int index = GetFrameIndex(frame.Tick);
        if (m_serverBuffer[index] == null || m_serverBuffer[index].Tick != frame.Tick) 
        {
            m_serverBuffer[index] = frame;
        }
    }

    public void PushServerFrame(Msg_FrameInfo[] frames)
    {
        for (int i = 0; i < frames.Length; i++) 
        {
            PushServerFrame(frames[i]);
        }
    }

    public void FrameCheck(float deltaTime)
    {
        IsNeedRollback = false;
        while (NextTickToCheck <= MaxServerTickInBuffer && NextTickToCheck < m_world.Tick) 
        {
            var index = GetFrameIndex(NextTickToCheck);
            var cFrame = m_clientBuffer[index];
            var sFrame = m_serverBuffer[index];
            if (cFrame == null || cFrame.Tick != NextTickToCheck || sFrame == null || sFrame.Tick != NextTickToCheck)
                break;
            if (object.ReferenceEquals(cFrame, sFrame) || cFrame.Equals(sFrame))
            {
                NextTickToCheck++;
            }
            else 
            {
                IsNeedRollback = true;
                break;
            }
        }

        int tick = NextTickToCheck;
        for (; tick < MaxServerTickInBuffer; tick++) 
        {
            var frame = m_serverBuffer[GetFrameIndex(tick)];
            if (frame == null || frame.Tick != tick)
                break;
        }

        MaxContinueServerTick = tick - 1;
        //TODO miss的帧数请求
    }

    public void PushLocalFrame(Msg_FrameInfo frame)
    {
        var index = GetFrameIndex(frame.Tick);
        m_clientBuffer[index] = frame;
    }

    public Msg_FrameInfo GetServerFrame(int tick)
    {
        if (tick > MaxServerTickInBuffer)
            return null;
        return GetFrame(m_serverBuffer, tick);
    }

    public Msg_FrameInfo GetLocalFrame(int tick)
    {
        if (tick > NextTickToCheck)
            return null;
        return GetFrame(m_clientBuffer, tick);
    }

    private Msg_FrameInfo GetFrame(Msg_FrameInfo[] buffer, int tick) 
    {
        var index = GetFrameIndex(tick);
        var frame = buffer[index];
        if(frame == null)return null;
        if (frame.Tick != tick) return null;
        return frame;
    }
}
