using System;

namespace System.IO.Ports
{
	internal static class InternalResources
	{
		internal static void EndOfFile()
		{
			throw new EndOfStreamException(global::SR.GetString("Unable to read beyond the end of the stream."));
		}

		internal static string GetMessage(int errorCode)
		{
			return global::SR.GetString("Unknown Error '{0}'.", new object[] { errorCode });
		}

		internal static void FileNotOpen()
		{
			throw new ObjectDisposedException(null, global::SR.GetString("The port is closed."));
		}

		internal static void WrongAsyncResult()
		{
			throw new ArgumentException(global::SR.GetString("IAsyncResult object did not come from the corresponding async method on this type."));
		}

		internal static void EndReadCalledTwice()
		{
			throw new ArgumentException(global::SR.GetString("EndRead can only be called once for each asynchronous operation."));
		}

		internal static void EndWriteCalledTwice()
		{
			throw new ArgumentException(global::SR.GetString("EndWrite can only be called once for each asynchronous operation."));
		}

		internal static void WinIOError(int errorCode, string str)
		{
			if (errorCode <= 5)
			{
				if (errorCode - 2 > 1)
				{
					if (errorCode == 5)
					{
						if (str.Length == 0)
						{
							throw new UnauthorizedAccessException(global::SR.GetString("Access to the port is denied."));
						}
						throw new UnauthorizedAccessException(global::SR.GetString("Access to the port '{0}' is denied.", new object[] { str }));
					}
				}
				else
				{
					if (str.Length == 0)
					{
						throw new IOException(global::SR.GetString("The specified port does not exist."));
					}
					throw new IOException(global::SR.GetString("The port '{0}' does not exist.", new object[] { str }));
				}
			}
			else if (errorCode != 32)
			{
				if (errorCode == 206)
				{
					throw new PathTooLongException(global::SR.GetString("The specified port name is too long.  The port name must be less than 260 characters."));
				}
			}
			else
			{
				if (str.Length == 0)
				{
					throw new IOException(global::SR.GetString("The process cannot access the port because it is being used by another process."));
				}
				throw new IOException(global::SR.GetString("The process cannot access the port '{0}' because it is being used by another process.", new object[] { str }));
			}
			throw new IOException(InternalResources.GetMessage(errorCode), InternalResources.MakeHRFromErrorCode(errorCode));
		}

		internal static int MakeHRFromErrorCode(int errorCode)
		{
			return -2147024896 | errorCode;
		}
	}
}
