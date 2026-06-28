using System;
using System.Collections;

namespace System.Data.Common
{
	internal class DBComparerFactory
	{
		public static IComparer GetComparer(Type type, bool ignoreCase)
		{
			if (type == typeof(string))
			{
				if (ignoreCase)
				{
					return DBComparerFactory.ignoreCaseComparer;
				}
				return DBComparerFactory.caseComparer;
			}
			else
			{
				if (DBComparerFactory.icomparerType.IsAssignableFrom(type))
				{
					return DBComparerFactory.comparableComparer;
				}
				if (type == typeof(byte[]))
				{
					return DBComparerFactory.byteArrayComparer;
				}
				return null;
			}
		}

		private static IComparer comparableComparer = new DBComparerFactory.ComparebleComparer();

		private static IComparer ignoreCaseComparer = new DBComparerFactory.IgnoreCaseComparer();

		private static IComparer caseComparer = new DBComparerFactory.CaseComparer();

		private static IComparer byteArrayComparer = new DBComparerFactory.ByteArrayComparer();

		private static Type icomparerType = typeof(IComparable);

		private class ComparebleComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (y == DBNull.Value)
					{
						return 1;
					}
					return ((IComparable)x).CompareTo(y);
				}
			}
		}

		private class CaseComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (y == DBNull.Value)
					{
						return 1;
					}
					return string.Compare((string)x, (string)y, false);
				}
			}
		}

		private class IgnoreCaseComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (y == DBNull.Value)
					{
						return 1;
					}
					return string.Compare((string)x, (string)y, true);
				}
			}
		}

		private class ByteArrayComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (y == DBNull.Value)
					{
						return 1;
					}
					byte[] array = (byte[])x;
					byte[] array2 = (byte[])y;
					int num = array.Length;
					int num2 = array2.Length;
					int num3 = 0;
					for (;;)
					{
						int num4 = 0;
						int num5 = 0;
						if (num3 < num)
						{
							num4 = (int)array[num3];
						}
						else if (num3 >= num2)
						{
							break;
						}
						if (num3 < num2)
						{
							num5 = (int)array2[num3];
						}
						if (num4 > num5)
						{
							return 1;
						}
						if (num5 > num4)
						{
							return -1;
						}
						num3++;
					}
					return 0;
				}
			}
		}
	}
}
