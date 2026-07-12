using System;
using System.Data.Common;
using System.Threading;

namespace System.Data.SqlClient
{
	internal sealed class TdsParserStaticMethods
	{
		internal static byte[] ObfuscatePassword(string password)
		{
			byte[] array = new byte[password.Length << 1];
			for (int i = 0; i < password.Length; i++)
			{
				char c = password[i];
				byte b = (byte)(c & 'ÿ');
				byte b2 = (byte)((c >> 8) & 'ÿ');
				array[i << 1] = (byte)((((int)(b & 15) << 4) | (b >> 4)) ^ 165);
				array[(i << 1) + 1] = (byte)((((int)(b2 & 15) << 4) | (b2 >> 4)) ^ 165);
			}
			return array;
		}

		internal static byte[] ObfuscatePassword(byte[] password)
		{
			for (int i = 0; i < password.Length; i++)
			{
				byte b = password[i] & 15;
				byte b2 = password[i] & 240;
				password[i] = (byte)(((b2 >> 4) | ((int)b << 4)) ^ 165);
			}
			return password;
		}

		internal static int GetCurrentProcessIdForTdsLoginOnly()
		{
			if (TdsParserStaticMethods.s_currentProcessId == -1)
			{
				int num = new Random().Next();
				Interlocked.CompareExchange(ref TdsParserStaticMethods.s_currentProcessId, num, -1);
			}
			return TdsParserStaticMethods.s_currentProcessId;
		}

		internal static int GetCurrentThreadIdForTdsLoginOnly()
		{
			return Environment.CurrentManagedThreadId;
		}

		internal static byte[] GetNetworkPhysicalAddressForTdsLoginOnly()
		{
			if (TdsParserStaticMethods.s_nicAddress == null)
			{
				byte[] array = new byte[6];
				new Random().NextBytes(array);
				Interlocked.CompareExchange<byte[]>(ref TdsParserStaticMethods.s_nicAddress, array, null);
			}
			return TdsParserStaticMethods.s_nicAddress;
		}

		internal static int GetTimeoutMilliseconds(long timeoutTime)
		{
			if (9223372036854775807L == timeoutTime)
			{
				return -1;
			}
			long num = ADP.TimerRemainingMilliseconds(timeoutTime);
			if (num < 0L)
			{
				return 0;
			}
			if (num > 2147483647L)
			{
				return int.MaxValue;
			}
			return (int)num;
		}

		internal static long GetTimeout(long timeoutMilliseconds)
		{
			long num;
			if (timeoutMilliseconds <= 0L)
			{
				num = long.MaxValue;
			}
			else
			{
				try
				{
					num = checked(ADP.TimerCurrent() + ADP.TimerFromMilliseconds(timeoutMilliseconds));
				}
				catch (OverflowException)
				{
					num = long.MaxValue;
				}
			}
			return num;
		}

		internal static bool TimeoutHasExpired(long timeoutTime)
		{
			bool flag = false;
			if (timeoutTime != 0L && 9223372036854775807L != timeoutTime)
			{
				flag = ADP.TimerHasExpired(timeoutTime);
			}
			return flag;
		}

		internal static int NullAwareStringLength(string str)
		{
			if (str == null)
			{
				return 0;
			}
			return str.Length;
		}

		internal static int GetRemainingTimeout(int timeout, long start)
		{
			if (timeout <= 0)
			{
				return timeout;
			}
			long num = ADP.TimerRemainingSeconds(start + ADP.TimerFromSeconds(timeout));
			if (num <= 0L)
			{
				return 1;
			}
			return checked((int)num);
		}

		private const int NoProcessId = -1;

		private static int s_currentProcessId = -1;

		private static byte[] s_nicAddress = null;
	}
}
