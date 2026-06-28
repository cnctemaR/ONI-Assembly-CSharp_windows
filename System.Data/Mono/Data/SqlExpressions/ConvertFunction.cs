using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class ConvertFunction : UnaryExpression
	{
		public ConvertFunction(IExpression e, string targetType)
			: base(e)
		{
			try
			{
				this.targetType = Type.GetType(targetType, true);
			}
			catch (TypeLoadException)
			{
				throw new EvaluateException(string.Format("Invalid type name '{0}'.", targetType));
			}
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is ConvertFunction))
			{
				return false;
			}
			ConvertFunction convertFunction = (ConvertFunction)obj;
			return convertFunction.targetType == this.targetType;
		}

		public override int GetHashCode()
		{
			return this.targetType.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = this.expr.Eval(row);
			if (obj == null)
			{
				return DBNull.Value;
			}
			if (obj == DBNull.Value || obj.GetType() == this.targetType)
			{
				return obj;
			}
			if (this.targetType == typeof(string))
			{
				return obj.ToString();
			}
			if (this.targetType == typeof(TimeSpan))
			{
				if (obj is string)
				{
					return TimeSpan.Parse((string)obj);
				}
				this.ThrowInvalidCastException(obj);
			}
			if (obj is TimeSpan)
			{
				this.ThrowInvalidCastException(obj);
			}
			if (obj is char && this.targetType != typeof(int) && this.targetType != typeof(uint))
			{
				this.ThrowInvalidCastException(obj);
			}
			if (this.targetType == typeof(char) && !(obj is int) && !(obj is uint))
			{
				this.ThrowInvalidCastException(obj);
			}
			if (obj is bool && (this.targetType == typeof(float) || this.targetType == typeof(double) || this.targetType == typeof(decimal)))
			{
				this.ThrowInvalidCastException(obj);
			}
			if (this.targetType == typeof(bool) && (obj is float || obj is double || obj is decimal))
			{
				this.ThrowInvalidCastException(obj);
			}
			return Convert.ChangeType(obj, this.targetType);
		}

		private void ThrowInvalidCastException(object val)
		{
			throw new InvalidCastException(string.Format("Type '{0}' cannot be converted to '{1}'.", val.GetType(), this.targetType));
		}

		private Type targetType;
	}
}
