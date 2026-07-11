using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;

namespace System.Data
{
	internal sealed class DataExpression : IFilter
	{
		internal DataExpression(DataTable table, string expression)
			: this(table, expression, null)
		{
		}

		internal DataExpression(DataTable table, string expression, Type type)
		{
			ExpressionParser expressionParser = new ExpressionParser(table);
			expressionParser.LoadExpression(expression);
			this._originalExpression = expression;
			this._expr = null;
			if (expression != null)
			{
				this._storageType = DataStorage.GetStorageType(type);
				if (this._storageType == StorageType.BigInteger)
				{
					throw ExprException.UnsupportedDataType(type);
				}
				this._dataType = type;
				this._expr = expressionParser.Parse();
				this._parsed = true;
				if (this._expr != null && table != null)
				{
					this.Bind(table);
					return;
				}
				this._bound = false;
			}
		}

		internal string Expression
		{
			get
			{
				if (this._originalExpression == null)
				{
					return "";
				}
				return this._originalExpression;
			}
		}

		internal ExpressionNode ExpressionNode
		{
			get
			{
				return this._expr;
			}
		}

		internal bool HasValue
		{
			get
			{
				return this._expr != null;
			}
		}

		internal void Bind(DataTable table)
		{
			this._table = table;
			if (table == null)
			{
				return;
			}
			if (this._expr != null)
			{
				List<DataColumn> list = new List<DataColumn>();
				this._expr.Bind(table, list);
				this._expr = this._expr.Optimize();
				this._table = table;
				this._bound = true;
				this._dependency = list.ToArray();
			}
		}

		internal bool DependsOn(DataColumn column)
		{
			return this._expr != null && this._expr.DependsOn(column);
		}

		internal object Evaluate()
		{
			return this.Evaluate(null, DataRowVersion.Default);
		}

		internal object Evaluate(DataRow row, DataRowVersion version)
		{
			if (!this._bound)
			{
				this.Bind(this._table);
			}
			object obj;
			if (this._expr != null)
			{
				obj = this._expr.Eval(row, version);
				if (obj == DBNull.Value && StorageType.Uri >= this._storageType)
				{
					return obj;
				}
				try
				{
					if (StorageType.Object != this._storageType)
					{
						obj = SqlConvert.ChangeType2(obj, this._storageType, this._dataType, this._table.FormatProvider);
					}
					return obj;
				}
				catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
				{
					ExceptionBuilder.TraceExceptionForCapture(ex);
					throw ExprException.DatavalueConvertion(obj, this._dataType, ex);
				}
			}
			obj = null;
			return obj;
		}

		internal object Evaluate(DataRow[] rows)
		{
			return this.Evaluate(rows, DataRowVersion.Default);
		}

		internal object Evaluate(DataRow[] rows, DataRowVersion version)
		{
			if (!this._bound)
			{
				this.Bind(this._table);
			}
			if (this._expr != null)
			{
				List<int> list = new List<int>();
				foreach (DataRow dataRow in rows)
				{
					if (dataRow.RowState != DataRowState.Deleted && (version != DataRowVersion.Original || dataRow._oldRecord != -1))
					{
						list.Add(dataRow.GetRecordFromVersion(version));
					}
				}
				int[] array = list.ToArray();
				return this._expr.Eval(array);
			}
			return DBNull.Value;
		}

		public bool Invoke(DataRow row, DataRowVersion version)
		{
			if (this._expr == null)
			{
				return true;
			}
			if (row == null)
			{
				throw ExprException.InvokeArgument();
			}
			object obj = this._expr.Eval(row, version);
			bool flag;
			try
			{
				flag = DataExpression.ToBoolean(obj);
			}
			catch (EvaluateException)
			{
				throw ExprException.FilterConvertion(this.Expression);
			}
			return flag;
		}

		internal DataColumn[] GetDependency()
		{
			return this._dependency;
		}

		internal bool IsTableAggregate()
		{
			return this._expr != null && this._expr.IsTableConstant();
		}

		internal static bool IsUnknown(object value)
		{
			return DataStorage.IsObjectNull(value);
		}

		internal bool HasLocalAggregate()
		{
			return this._expr != null && this._expr.HasLocalAggregate();
		}

		internal bool HasRemoteAggregate()
		{
			return this._expr != null && this._expr.HasRemoteAggregate();
		}

		internal static bool ToBoolean(object value)
		{
			if (DataExpression.IsUnknown(value))
			{
				return false;
			}
			if (value is bool)
			{
				return (bool)value;
			}
			if (value is SqlBoolean)
			{
				return ((SqlBoolean)value).IsTrue;
			}
			if (value is string)
			{
				try
				{
					return bool.Parse((string)value);
				}
				catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
				{
					ExceptionBuilder.TraceExceptionForCapture(ex);
					throw ExprException.DatavalueConvertion(value, typeof(bool), ex);
				}
			}
			throw ExprException.DatavalueConvertion(value, typeof(bool), null);
		}

		internal string _originalExpression;

		private bool _parsed;

		private bool _bound;

		private ExpressionNode _expr;

		private DataTable _table;

		private readonly StorageType _storageType;

		private readonly Type _dataType;

		private DataColumn[] _dependency = Array.Empty<DataColumn>();
	}
}
