using System;

namespace System.Data
{
	public static class DataRowComparer
	{
		public static DataRowComparer<DataRow> Default
		{
			get
			{
				return DataRowComparer<DataRow>.Default;
			}
		}

		internal static bool AreEqual(object a, object b)
		{
			return a == b || (a != null && a != DBNull.Value && b != null && b != DBNull.Value && (a.Equals(b) || (a.GetType().IsArray && DataRowComparer.CompareArray((Array)a, b as Array))));
		}

		private static bool AreElementEqual(object a, object b)
		{
			return a == b || (a != null && a != DBNull.Value && b != null && b != DBNull.Value && a.Equals(b));
		}

		private static bool CompareArray(Array a, Array b)
		{
			if (b == null || 1 != a.Rank || 1 != b.Rank || a.Length != b.Length)
			{
				return false;
			}
			int i = a.GetLowerBound(0);
			int num = b.GetLowerBound(0);
			if (a.GetType() == b.GetType() && i == 0 && num == 0)
			{
				TypeCode typeCode = Type.GetTypeCode(a.GetType().GetElementType());
				switch (typeCode)
				{
				case TypeCode.Byte:
					return DataRowComparer.CompareEquatableArray<byte>((byte[])a, (byte[])b);
				case TypeCode.Int16:
					return DataRowComparer.CompareEquatableArray<short>((short[])a, (short[])b);
				case TypeCode.UInt16:
				case TypeCode.UInt32:
					break;
				case TypeCode.Int32:
					return DataRowComparer.CompareEquatableArray<int>((int[])a, (int[])b);
				case TypeCode.Int64:
					return DataRowComparer.CompareEquatableArray<long>((long[])a, (long[])b);
				default:
					if (typeCode == TypeCode.String)
					{
						return DataRowComparer.CompareEquatableArray<string>((string[])a, (string[])b);
					}
					break;
				}
			}
			int num2 = i + a.Length;
			while (i < num2)
			{
				if (!DataRowComparer.AreElementEqual(a.GetValue(i), b.GetValue(num)))
				{
					return false;
				}
				i++;
				num++;
			}
			return true;
		}

		private static bool CompareEquatableArray<TElem>(TElem[] a, TElem[] b) where TElem : IEquatable<TElem>
		{
			for (int i = 0; i < a.Length; i++)
			{
				if (!a[i].Equals(b[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
