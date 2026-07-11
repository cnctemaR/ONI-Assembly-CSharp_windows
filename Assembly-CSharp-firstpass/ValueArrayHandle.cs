using System;

public struct ValueArrayHandle
{
	public ValueArrayHandle(int handle)
	{
		this.handle = handle;
	}

	public bool IsValid()
	{
		return this.handle >= 0;
	}

	public int handle;

	public static readonly ValueArrayHandle Invalid = new ValueArrayHandle(-1);
}
