using System;
using System.Collections.Generic;

public interface ISerializablePacket
{
    byte[] ToBytes();
    void FromBytes(byte[] bytes);
}

public interface ISerializable
{
    void Serialize(Serializer writer);

    void Deserialize(Deserializer reader);
}

public abstract class BaseFormater : ISerializable, ISerializablePacket
{
    public abstract void Deserialize(Deserializer reader);

    public abstract void Serialize(Serializer writer);

    public byte[] ToBytes() 
    {
        var write = new Serializer();
        Serialize(write);
        var bytes = write.CopyData();
        return bytes;
    }

    public void FromBytes(byte[] data) 
    {
        var bytes = data;
        var reader = new Deserializer(bytes);
        Deserialize(reader);
    }

    public void FromBytes(byte[] data, int offset, int size) 
    {
        var bytes = data;
        var reader = new Deserializer(data, offset, size);
        Deserialize(reader);
    }

    public static T FromBytes<T>(byte[] data) where T : BaseFormater, new() 
    {
        var ret = new T();
        ret.FromBytes(data);
        return ret;
    }

    public static T FromBytes<T>(byte[] data, int offset, int size) where T : BaseFormater, new()
    {
        var ret = new T();
        ret.FromBytes(data,offset,size);
        return ret;
    }
}

