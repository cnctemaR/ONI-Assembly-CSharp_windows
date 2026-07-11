using System;
using System.Runtime.InteropServices;

public struct TextureRegion
{
	public TextureRegion(int x, int y, TexturePage page, TextureBuffer buffer)
	{
		this.x = x;
		this.y = y;
		this.page = page;
		this.buffer = buffer;
		this.width = page.width;
		this.bytesPerPixel = TextureUtil.GetBytesPerPixel(page.format);
		this.bytes = page.bytes;
		this.floatConverter = new TextureRegion.ByteToFloatConverter
		{
			bytes = page.bytes
		};
	}

	private int GetByteIdx(int x, int y)
	{
		int num = x - this.x;
		int num2 = y - this.y;
		return (num + num2 * this.width) * this.bytesPerPixel;
	}

	public void SetBytes(int x, int y, byte b0)
	{
		int byteIdx = this.GetByteIdx(x, y);
		this.bytes[byteIdx] = b0;
	}

	public void SetBytes(int x, int y, byte b0, byte b1)
	{
		int byteIdx = this.GetByteIdx(x, y);
		this.bytes[byteIdx] = b0;
		this.bytes[byteIdx + 1] = b1;
	}

	public void SetBytes(int x, int y, byte b0, byte b1, byte b2)
	{
		int byteIdx = this.GetByteIdx(x, y);
		this.bytes[byteIdx] = b0;
		this.bytes[byteIdx + 1] = b1;
		this.bytes[byteIdx + 2] = b2;
	}

	public void SetBytes(int x, int y, byte b0, byte b1, byte b2, byte b3)
	{
		int byteIdx = this.GetByteIdx(x, y);
		this.bytes[byteIdx] = b0;
		this.bytes[byteIdx + 1] = b1;
		this.bytes[byteIdx + 2] = b2;
		this.bytes[byteIdx + 3] = b3;
	}

	public void SetBytes(int x, int y, float v0, float v1)
	{
		int num = this.GetByteIdx(x, y) / 4;
		this.floatConverter.floats[num] = v0;
		this.floatConverter.floats[num + 1] = v1;
	}

	public void Unlock()
	{
		this.buffer.Unlock(this);
	}

	public int x;

	public int y;

	public int bytesPerPixel;

	public byte[] bytes;

	public int width;

	public TexturePage page;

	public TextureBuffer buffer;

	public TextureRegion.ByteToFloatConverter floatConverter;

	[StructLayout(LayoutKind.Explicit)]
	public struct ByteToFloatConverter
	{
		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public float[] floats;
	}
}
