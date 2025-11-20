

using System.Runtime.InteropServices;
using System.Text;

namespace Common;

public interface IDataStreamWriter
{
    void Write(PropertyInfomation info, bool value);
    void Write(PropertyInfomation info, int value);
    void Write(PropertyInfomation info, long value);
    void Write(PropertyInfomation info, string value);
    void Write(PropertyInfomation info, MemoryStream stream);

    bool Begin(PropertyInfomation info);

    void End() { }
}

public interface IDataStreamReader
{
    int ReadInt(PropertyInfomation info);
    long ReadLong(PropertyInfomation info);
    bool ReadBool(PropertyInfomation info);
    string ReadString(PropertyInfomation info);
    MemoryStream ReadMemoryStream(PropertyInfomation info);
}

public class MemoryStreamDataStream : IDataStreamWriter
{
    public MemoryStream stream;
    PropertyFlag flag;

    public MemoryStreamDataStream(PropertyFlag flag = PropertyFlag.All)
    {
        this.stream = new MemoryStream();
        this.flag = flag;
    }

    public bool Begin(PropertyInfomation info)
    {
        return (info.flag & this.flag) != 0;
    }

    public void Write(PropertyInfomation info, bool value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        stream.Write(BitConverter.GetBytes(value));
    }

    public void Write(PropertyInfomation info, int value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }
        stream.Write(BitConverter.GetBytes(value));
    }

    public void Write(PropertyInfomation info, long value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        stream.Write(BitConverter.GetBytes(value));
    }

    public void Write(PropertyInfomation info, string value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        var rawBytes = MemoryMarshal.AsBytes(value.AsSpan());
        stream.Write(BitConverter.GetBytes((int)rawBytes.Length));
        stream.Write(rawBytes);
    }

    public void Write(PropertyInfomation info, MemoryStream msg)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        stream.Write(msg.GetBuffer(), 0, (int)msg.Length);
    }
}

public class MemoryStreamDataStreamReader : IDataStreamReader
{
    BinaryReader reader;
    PropertyFlag flag;
    public MemoryStreamDataStreamReader(MemoryStream stream, PropertyFlag flag = PropertyFlag.All)
    {
        reader = new BinaryReader(stream);
        this.flag = flag;
    }

    public int ReadInt(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return default;
        }

        return reader.ReadInt32();
    }

    public long ReadLong(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return default;
        }

        return reader.ReadInt64();
    }

    public bool ReadBool(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return default;
        }

        return reader.ReadBoolean();
    }

    public string ReadString(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return "";
        }

        int len = reader.ReadInt32();
        byte[] data = reader.ReadBytes(len);

        return Encoding.Unicode.GetString(data);
    }

    public MemoryStream ReadMemoryStream(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return new MemoryStream();
        }

        long size = reader.BaseStream.Length - reader.BaseStream.Position;
        return new MemoryStream(reader.ReadBytes((int)size));
    }
}

public class DictDataStream : IDataStreamWriter
{
    public Dictionary<string, object> root;
    Dictionary<string, object> datas;
    PropertyFlag flag;

    Stack<Dictionary<string, object>> dataStack = new Stack<Dictionary<string, object>>();

    public DictDataStream(PropertyFlag flag = PropertyFlag.All)
    {
        datas = new Dictionary<string, object>();
        root = datas;

        this.flag = flag;
    }

    public bool Begin(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return false;
        }

        dataStack.Push(datas);

        var newData = new Dictionary<string, object>();
        datas[info.name] = newData;

        datas = newData;

        return true;
    }

    public void Write(PropertyInfomation info, bool value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        datas.Add(info.name, value);
    }

    public void Write(PropertyInfomation info, int value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        datas.Add(info.name, value);
    }

    public void Write(PropertyInfomation info, long value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        datas.Add(info.name, value);
    }

    public void Write(PropertyInfomation info, string value)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        datas.Add(info.name, value);
    }

    public void Write(PropertyInfomation info, MemoryStream stream)
    {
        if ((flag & info.flag) == 0)
        {
            return;
        }

        datas.Add(info.name, stream);
    }
}

public class DictDataStreamReader : IDataStreamReader
{
    Dictionary<string, object> datas;
    PropertyFlag flag;

    public DictDataStreamReader(Dictionary<string, object> datas, PropertyFlag flag = PropertyFlag.All)
    {
        this.datas = datas;
        this.flag = flag;
    }

    public int ReadInt(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return default;
        }

        return (int)datas[info.name];
    }

    public long ReadLong(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return default;
        }

        return (long)datas[info.name];
    }

    public bool ReadBool(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return default;
        }

        return (bool)datas[info.name];
    }

    public string ReadString(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return "";
        }

        return (string)datas[info.name];
    }

    public MemoryStream ReadMemoryStream(PropertyInfomation info)
    {
        if ((flag & info.flag) == 0)
        {
            return new MemoryStream();
        }

        return (MemoryStream)datas[info.name];
    }
}
