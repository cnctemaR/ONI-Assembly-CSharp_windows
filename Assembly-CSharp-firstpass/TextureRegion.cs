using System;
using Unity.Collections;

public struct TextureRegion
{
	public TextureRegion(int x, int y, int width, int height, TexturePage page, TextureBuffer buffer)
	{
		this.x = x;
		this.y = y;
		this.page = page;
		this.buffer = buffer;
		this.targetWidth = width;
		this.targetHeight = height;
		this.pageWidth = page.width;
		this.bytesPerPixel = TextureUtil.GetBytesPerPixel(page.format);
		this.bytes = page.texture.GetRawTextureData<byte>();
		this.floats = page.texture.GetRawTextureData<float>();
	}

	private int GetByteIdx(int x, int y)
	{
		int num = x - this.x;
		int num2 = y - this.y;
		return (num + num2 * this.pageWidth) * this.bytesPerPixel;
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

	public void SetBytes(int x, int y, float v0)
	{
		int num = this.GetByteIdx(x, y) / 4;
		this.floats[num] = v0;
	}

	public void SetBytes(int x, int y, float v0, float v1)
	{
		int num = this.GetByteIdx(x, y) / 4;
		this.floats[num] = v0;
		this.floats[num + 1] = v1;
	}

	public void Unlock()
	{
		this.buffer.Unlock(this);
	}

	public int x;

	public int y;

	public int bytesPerPixel;

	public NativeArray<byte> bytes;

	public NativeArray<float> floats;

	public int targetWidth;

	public int targetHeight;

	public int pageWidth;

	public TexturePage page;

	public TextureBuffer buffer;
}
