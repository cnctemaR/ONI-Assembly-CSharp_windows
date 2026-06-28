using System;
using System.Text;

public class FastReader : IReader
{
	public FastReader(byte[] bytes)
	{
		this.bytes = bytes;
	}

	public unsafe byte ReadByte()
	{
		byte b;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			b = *ptr;
		}
		this.idx++;
		return b;
	}

	public unsafe sbyte ReadSByte()
	{
		sbyte b;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			b = *(sbyte*)ptr;
		}
		this.idx++;
		return b;
	}

	public unsafe ushort ReadUInt16()
	{
		ushort num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(ushort*)ptr;
		}
		this.idx += 2;
		return num;
	}

	public unsafe short ReadInt16()
	{
		short num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(short*)ptr;
		}
		this.idx += 2;
		return num;
	}

	public unsafe uint ReadUInt32()
	{
		uint num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(uint*)ptr;
		}
		this.idx += 4;
		return num;
	}

	public unsafe int ReadInt32()
	{
		int num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(int*)ptr;
		}
		this.idx += 4;
		return num;
	}

	public unsafe ulong ReadUInt64()
	{
		ulong num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = (ulong)(*(long*)ptr);
		}
		this.idx += 8;
		return num;
	}

	public unsafe long ReadInt64()
	{
		long num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(long*)ptr;
		}
		this.idx += 8;
		return num;
	}

	public unsafe float ReadSingle()
	{
		float num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(float*)ptr;
		}
		this.idx += 4;
		return num;
	}

	public unsafe double ReadDouble()
	{
		double num;
		fixed (byte* ptr = &this.bytes[this.idx])
		{
			num = *(double*)ptr;
		}
		this.idx += 8;
		return num;
	}

	public char[] ReadChars(int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = (char)this.bytes[this.idx + i];
		}
		this.idx += length;
		return array;
	}

	public byte[] ReadBytes(int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = this.bytes[this.idx + i];
		}
		this.idx += length;
		return array;
	}

	public string ReadKleiString()
	{
		int num = this.ReadInt32();
		string text = null;
		if (num >= 0)
		{
			text = Encoding.UTF8.GetString(this.bytes, this.idx, num);
			this.idx += num;
		}
		return text;
	}

	public void SkipBytes(int length)
	{
		this.idx += length;
	}

	public bool IsFinished
	{
		get
		{
			return this.bytes == null || this.idx == this.bytes.Length;
		}
	}

	public byte[] RawBytes()
	{
		return this.bytes;
	}

	public int Position
	{
		get
		{
			return this.idx;
		}
		set
		{
			this.idx = value;
		}
	}

	private int idx;

	private byte[] bytes;
}
