using System;
using System.IO;
using System.Text;

public class KBinaryReader : BinaryReader, IReader
{
	public KBinaryReader(Stream stream)
		: base(stream)
	{
	}

	public void SkipBytes(int length)
	{
		this.ReadBytes(length);
	}

	public bool IsFinished
	{
		get
		{
			return this.BaseStream.Position == this.BaseStream.Length;
		}
	}

	public int Position
	{
		get
		{
			return (int)this.BaseStream.Position;
		}
	}

	public string ReadKleiString()
	{
		string text = null;
		int num = this.ReadInt32();
		if (num >= 0)
		{
			byte[] array = this.ReadBytes(num);
			text = Encoding.UTF8.GetString(array, 0, num);
		}
		return text;
	}

	public byte[] RawBytes()
	{
		long position = this.BaseStream.Position;
		this.BaseStream.Position = 0L;
		byte[] array = this.ReadBytes((int)this.BaseStream.Length);
		this.BaseStream.Position = position;
		return array;
	}
}
