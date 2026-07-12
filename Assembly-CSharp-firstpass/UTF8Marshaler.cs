using System;
using System.Runtime.InteropServices;
using System.Text;

public class UTF8Marshaler : ICustomMarshaler
{
	public IntPtr MarshalManagedToNative(object obj)
	{
		if (obj == null)
		{
			return IntPtr.Zero;
		}
		if (!(obj is string))
		{
			throw new MarshalDirectiveException("Invalid obj in UTF8Marshaler.");
		}
		byte[] bytes = Encoding.UTF8.GetBytes((string)obj);
		IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		Marshal.WriteByte((IntPtr)((long)intPtr + (long)bytes.Length), 0);
		return intPtr;
	}

	public object MarshalNativeToManaged(IntPtr data)
	{
		return UTF8Marshaler.MarshalNativeToString(data);
	}

	public void CleanUpNativeData(IntPtr data)
	{
		Marshal.FreeHGlobal(data);
	}

	public void CleanUpManagedData(object obj)
	{
	}

	public int GetNativeDataSize()
	{
		return -1;
	}

	public static ICustomMarshaler GetInstance(string cookie)
	{
		if (UTF8Marshaler.instance_ == null)
		{
			return UTF8Marshaler.instance_ = new UTF8Marshaler();
		}
		return UTF8Marshaler.instance_;
	}

	public static string MarshalNativeToString(IntPtr data)
	{
		int num = 0;
		while (Marshal.ReadByte(data, num) != 0)
		{
			num++;
		}
		if (num == 0)
		{
			return string.Empty;
		}
		byte[] array = new byte[num];
		Marshal.Copy(data, array, 0, num);
		return Encoding.UTF8.GetString(array);
	}

	private static UTF8Marshaler instance_;
}
