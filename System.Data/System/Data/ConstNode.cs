using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Data
{
	internal sealed class ConstNode : ExpressionNode
	{
		internal ConstNode(DataTable table, ValueType type, object constant)
			: this(table, type, constant, true)
		{
		}

		internal ConstNode(DataTable table, ValueType type, object constant, bool fParseQuotes)
			: base(table)
		{
			switch (type)
			{
			case ValueType.Null:
				this._val = DBNull.Value;
				return;
			case ValueType.Bool:
				this._val = Convert.ToBoolean(constant, CultureInfo.InvariantCulture);
				return;
			case ValueType.Numeric:
				this._val = this.SmallestNumeric(constant);
				return;
			case ValueType.Str:
				if (fParseQuotes)
				{
					this._val = ((string)constant).Replace("''", "'");
					return;
				}
				this._val = (string)constant;
				return;
			case ValueType.Float:
				this._val = Convert.ToDouble(constant, NumberFormatInfo.InvariantInfo);
				return;
			case ValueType.Decimal:
				this._val = this.SmallestDecimal(constant);
				return;
			case ValueType.Date:
				this._val = DateTime.Parse((string)constant, CultureInfo.InvariantCulture);
				return;
			}
			this._val = constant;
		}

		internal override void Bind(DataTable table, List<DataColumn> list)
		{
			base.BindTable(table);
		}

		internal override object Eval()
		{
			return this._val;
		}

		internal override object Eval(DataRow row, DataRowVersion version)
		{
			return this.Eval();
		}

		internal override object Eval(int[] recordNos)
		{
			return this.Eval();
		}

		internal override bool IsConstant()
		{
			return true;
		}

		internal override bool IsTableConstant()
		{
			return true;
		}

		internal override bool HasLocalAggregate()
		{
			return false;
		}

		internal override bool HasRemoteAggregate()
		{
			return false;
		}

		internal override ExpressionNode Optimize()
		{
			return this;
		}

		private object SmallestDecimal(object constant)
		{
			if (constant == null)
			{
				return 0.0;
			}
			string text = constant as string;
			if (text != null)
			{
				decimal num;
				if (decimal.TryParse(text, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out num))
				{
					return num;
				}
				double num2;
				if (double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num2))
				{
					return num2;
				}
			}
			else
			{
				IConvertible convertible = constant as IConvertible;
				if (convertible != null)
				{
					try
					{
						return convertible.ToDecimal(NumberFormatInfo.InvariantInfo);
					}
					catch (ArgumentException ex)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
					}
					catch (FormatException ex2)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex2);
					}
					catch (InvalidCastException ex3)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex3);
					}
					catch (OverflowException ex4)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex4);
					}
					try
					{
						return convertible.ToDouble(NumberFormatInfo.InvariantInfo);
					}
					catch (ArgumentException ex5)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex5);
					}
					catch (FormatException ex6)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex6);
					}
					catch (InvalidCastException ex7)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex7);
					}
					catch (OverflowException ex8)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex8);
					}
					return constant;
				}
			}
			return constant;
		}

		private object SmallestNumeric(object constant)
		{
			if (constant == null)
			{
				return 0;
			}
			string text = constant as string;
			if (text != null)
			{
				int num;
				if (int.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num))
				{
					return num;
				}
				long num2;
				if (long.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2))
				{
					return num2;
				}
				double num3;
				if (double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num3))
				{
					return num3;
				}
			}
			else
			{
				IConvertible convertible = constant as IConvertible;
				if (convertible != null)
				{
					try
					{
						return convertible.ToInt32(NumberFormatInfo.InvariantInfo);
					}
					catch (ArgumentException ex)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex);
					}
					catch (FormatException ex2)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex2);
					}
					catch (InvalidCastException ex3)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex3);
					}
					catch (OverflowException ex4)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex4);
					}
					try
					{
						return convertible.ToInt64(NumberFormatInfo.InvariantInfo);
					}
					catch (ArgumentException ex5)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex5);
					}
					catch (FormatException ex6)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex6);
					}
					catch (InvalidCastException ex7)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex7);
					}
					catch (OverflowException ex8)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex8);
					}
					try
					{
						return convertible.ToDouble(NumberFormatInfo.InvariantInfo);
					}
					catch (ArgumentException ex9)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex9);
					}
					catch (FormatException ex10)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex10);
					}
					catch (InvalidCastException ex11)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex11);
					}
					catch (OverflowException ex12)
					{
						ExceptionBuilder.TraceExceptionWithoutRethrow(ex12);
					}
					return constant;
				}
			}
			return constant;
		}

		internal readonly object _val;
	}
}
