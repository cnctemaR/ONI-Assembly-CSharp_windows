using System;

public interface IReader
{
	byte ReadByte();

	sbyte ReadSByte();

	short ReadInt16();

	ushort ReadUInt16();

	int ReadInt32();

	uint ReadUInt32();

	long ReadInt64();

	ulong ReadUInt64();

	float ReadSingle();

	double ReadDouble();

	char[] ReadChars(int length);

	byte[] ReadBytes(int length);

	void SkipBytes(int length);

	string ReadKleiString();

	byte[] RawBytes();

	int Position { get; }

	bool IsFinished { get; }
}
