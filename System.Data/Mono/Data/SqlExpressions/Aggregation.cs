using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Aggregation : BaseExpression
	{
		public Aggregation(bool cacheResults, DataRow[] rows, AggregationFunction function, ColumnReference column)
		{
			this.cacheResults = cacheResults;
			this.rows = rows;
			this.column = column;
			this.function = function;
			this.result = null;
			if (cacheResults)
			{
				this.RowChangeHandler = new DataRowChangeEventHandler(this.InvalidateCache);
			}
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is Aggregation))
			{
				return false;
			}
			Aggregation aggregation = (Aggregation)obj;
			if (!aggregation.function.Equals(this.function))
			{
				return false;
			}
			if (!aggregation.column.Equals(this.column))
			{
				return false;
			}
			if (aggregation.rows != null && this.rows != null)
			{
				if (aggregation.rows.Length != this.rows.Length)
				{
					return false;
				}
				for (int i = 0; i < this.rows.Length; i++)
				{
					if (aggregation.rows[i] != this.rows[i])
					{
						return false;
					}
				}
			}
			else if (aggregation.rows != null || this.rows != null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num ^= this.function.GetHashCode();
			num ^= this.column.GetHashCode();
			for (int i = 0; i < this.rows.Length; i++)
			{
				num ^= this.rows[i].GetHashCode();
			}
			return num;
		}

		public override object Eval(DataRow row)
		{
			if (this.cacheResults && this.result != null && this.column.ReferencedTable == ReferencedTable.Self)
			{
				return this.result;
			}
			this.count = 0;
			this.result = null;
			object[] array;
			if (this.rows == null)
			{
				array = this.column.GetValues(this.column.GetReferencedRows(row));
			}
			else
			{
				array = this.column.GetValues(this.rows);
			}
			foreach (object obj in array)
			{
				if (obj != null)
				{
					this.count++;
					this.Aggregate((IConvertible)obj);
				}
			}
			switch (this.function)
			{
			case AggregationFunction.Count:
				this.result = this.count;
				break;
			case AggregationFunction.Avg:
			{
				IConvertible convertible;
				if (this.count == 0)
				{
					IConvertible value = DBNull.Value;
					convertible = value;
				}
				else
				{
					convertible = Numeric.Divide(this.result, this.count);
				}
				this.result = convertible;
				break;
			}
			case AggregationFunction.StDev:
			case AggregationFunction.Var:
				this.result = this.CalcStatisticalFunction(array);
				break;
			}
			if (this.result == null)
			{
				this.result = DBNull.Value;
			}
			if (this.cacheResults && this.column.ReferencedTable == ReferencedTable.Self)
			{
				this.table = row.Table;
				row.Table.RowChanged += this.RowChangeHandler;
			}
			return this.result;
		}

		public override bool DependsOn(DataColumn other)
		{
			return this.column.DependsOn(other);
		}

		private void Aggregate(IConvertible val)
		{
			switch (this.function)
			{
			case AggregationFunction.Sum:
			case AggregationFunction.Avg:
			case AggregationFunction.StDev:
			case AggregationFunction.Var:
			{
				IConvertible convertible2;
				if (this.result != null)
				{
					IConvertible convertible = Numeric.Add(this.result, val);
					convertible2 = convertible;
				}
				else
				{
					convertible2 = val;
				}
				this.result = convertible2;
				return;
			}
			case AggregationFunction.Min:
			{
				IConvertible convertible3;
				if (this.result != null)
				{
					IConvertible convertible = Numeric.Min(this.result, val);
					convertible3 = convertible;
				}
				else
				{
					convertible3 = val;
				}
				this.result = convertible3;
				return;
			}
			case AggregationFunction.Max:
			{
				IConvertible convertible4;
				if (this.result != null)
				{
					IConvertible convertible = Numeric.Max(this.result, val);
					convertible4 = convertible;
				}
				else
				{
					convertible4 = val;
				}
				this.result = convertible4;
				return;
			}
			default:
				return;
			}
		}

		private IConvertible CalcStatisticalFunction(object[] values)
		{
			if (this.count < 2)
			{
				return DBNull.Value;
			}
			double num = (double)Convert.ChangeType(this.result, TypeCode.Double) / (double)this.count;
			double num2 = 0.0;
			foreach (object obj in values)
			{
				if (obj != null)
				{
					double num3 = num - (double)Convert.ChangeType(obj, TypeCode.Double);
					num2 += Math.Pow(num3, 2.0);
				}
			}
			num2 /= (double)(this.count - 1);
			if (this.function == AggregationFunction.StDev)
			{
				num2 = Math.Sqrt(num2);
			}
			return num2;
		}

		public override void ResetExpression()
		{
			if (this.table != null)
			{
				this.InvalidateCache(this.table, null);
			}
		}

		private void InvalidateCache(object sender, DataRowChangeEventArgs args)
		{
			this.result = null;
			((DataTable)sender).RowChanged -= this.RowChangeHandler;
		}

		private bool cacheResults;

		private DataRow[] rows;

		private ColumnReference column;

		private AggregationFunction function;

		private int count;

		private IConvertible result;

		private DataRowChangeEventHandler RowChangeHandler;

		private DataTable table;
	}
}
