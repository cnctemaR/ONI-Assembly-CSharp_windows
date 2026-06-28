using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct StructByteArray<StructType>
{
	public StructByteArray(int size_in_structs)
	{
		int num = size_in_structs * Marshal.SizeOf(typeof(StructType));
		this.structs = null;
		this.bytes = new byte[num];
	}

	public int SizeInStructs
	{
		get
		{
			return this.bytes.Length / Marshal.SizeOf(typeof(StructType));
		}
	}

	public int StructSizeInBytes
	{
		get
		{
			return Marshal.SizeOf(typeof(StructType));
		}
	}

	public void Resize(int size_in_structs)
	{
		byte[] array = this.bytes;
		this.bytes = new byte[size_in_structs * Marshal.SizeOf(typeof(StructType))];
		Buffer.BlockCopy(array, 0, this.bytes, 0, array.Length);
	}

	[FieldOffset(0)]
	public byte[] bytes;

	[FieldOffset(0)]
	public StructType[] structs;
}
