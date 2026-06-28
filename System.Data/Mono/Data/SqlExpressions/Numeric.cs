using System;

namespace Mono.Data.SqlExpressions
{
	internal class Numeric
	{
		internal static bool IsNumeric(object o)
		{
			if (o is IConvertible)
			{
				TypeCode typeCode = ((IConvertible)o).GetTypeCode();
				if (TypeCode.Char < typeCode && typeCode <= TypeCode.Decimal)
				{
					return true;
				}
			}
			return false;
		}

		internal static IConvertible Unify(IConvertible o)
		{
			switch (o.GetTypeCode())
			{
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Int32);
			case TypeCode.UInt32:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Int64);
			case TypeCode.UInt64:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Decimal);
			case TypeCode.Single:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Double);
			}
			return o;
		}

		internal static TypeCode ToSameType(ref IConvertible o1, ref IConvertible o2)
		{
			TypeCode typeCode = o1.GetTypeCode();
			TypeCode typeCode2 = o2.GetTypeCode();
			if (typeCode == typeCode2)
			{
				return typeCode;
			}
			if (typeCode == TypeCode.DBNull || typeCode2 == TypeCode.DBNull)
			{
				return TypeCode.DBNull;
			}
			if (typeCode < typeCode2)
			{
				o1 = (IConvertible)Convert.ChangeType(o1, typeCode2);
				return typeCode2;
			}
			o2 = (IConvertible)Convert.ChangeType(o2, typeCode);
			return typeCode;
		}

		internal static IConvertible Add(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (long)((int)o1 + (int)o2);
			case TypeCode.Int64:
				return (long)o1 + (long)o2;
			case TypeCode.Double:
				return (double)o1 + (double)o2;
			case TypeCode.Decimal:
				return (decimal)o1 + (decimal)o2;
			}
			return DBNull.Value;
		}

		internal static IConvertible Subtract(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)o1 - (int)o2;
			case TypeCode.Int64:
				return (long)o1 - (long)o2;
			case TypeCode.Double:
				return (double)o1 - (double)o2;
			case TypeCode.Decimal:
				return (decimal)o1 - (decimal)o2;
			}
			return DBNull.Value;
		}

		internal static IConvertible Multiply(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)o1 * (int)o2;
			case TypeCode.Int64:
				return (long)o1 * (long)o2;
			case TypeCode.Double:
				return (double)o1 * (double)o2;
			case TypeCode.Decimal:
				return (decimal)o1 * (decimal)o2;
			}
			return DBNull.Value;
		}

		internal static IConvertible Divide(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)o1 / (int)o2;
			case TypeCode.Int64:
				return (long)o1 / (long)o2;
			case TypeCode.Double:
				return (double)o1 / (double)o2;
			case TypeCode.Decimal:
				return (decimal)o1 / (decimal)o2;
			}
			return DBNull.Value;
		}

		internal static IConvertible Modulo(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)o1 % (int)o2;
			case TypeCode.Int64:
				return (long)o1 % (long)o2;
			case TypeCode.Double:
				return (double)o1 % (double)o2;
			case TypeCode.Decimal:
				return (decimal)o1 % (decimal)o2;
			}
			return DBNull.Value;
		}

		internal static IConvertible Negative(IConvertible o)
		{
			switch (o.GetTypeCode())
			{
			case TypeCode.Int32:
				return -(int)o;
			case TypeCode.Int64:
				return -(long)o;
			case TypeCode.Double:
				return -(double)o;
			case TypeCode.Decimal:
				return -(decimal)o;
			}
			return DBNull.Value;
		}

		internal static IConvertible Min(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return Math.Min((int)o1, (int)o2);
			case TypeCode.Int64:
				return Math.Min((long)o1, (long)o2);
			case TypeCode.Double:
				return Math.Min((double)o1, (double)o2);
			case TypeCode.Decimal:
				return Math.Min((decimal)o1, (decimal)o2);
			case TypeCode.String:
			{
				int num = string.Compare((string)o1, (string)o2);
				if (num <= 0)
				{
					return o1;
				}
				return o2;
			}
			}
			return DBNull.Value;
		}

		internal static IConvertible Max(IConvertible o1, IConvertible o2)
		{
			switch (Numeric.ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return Math.Max((int)o1, (int)o2);
			case TypeCode.Int64:
				return Math.Max((long)o1, (long)o2);
			case TypeCode.Double:
				return Math.Max((double)o1, (double)o2);
			case TypeCode.Decimal:
				return Math.Max((decimal)o1, (decimal)o2);
			case TypeCode.String:
			{
				int num = string.Compare((string)o1, (string)o2);
				if (num >= 0)
				{
					return o1;
				}
				return o2;
			}
			}
			return DBNull.Value;
		}
	}
}
