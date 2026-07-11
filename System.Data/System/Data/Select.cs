using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace System.Data
{
	internal sealed class Select
	{
		public Select(DataTable table, string filterExpression, string sort, DataViewRowState recordStates)
		{
			this._table = table;
			this._indexFields = table.ParseSortString(sort);
			if (filterExpression != null && filterExpression.Length > 0)
			{
				this._rowFilter = new DataExpression(this._table, filterExpression);
				this._expression = this._rowFilter.ExpressionNode;
			}
			this._recordStates = recordStates;
		}

		private bool IsSupportedOperator(int op)
		{
			return (op >= 7 && op <= 11) || op == 13 || op == 39;
		}

		private void AnalyzeExpression(BinaryNode expr)
		{
			if (this._linearExpression == this._expression)
			{
				return;
			}
			if (expr._op == 27)
			{
				this._linearExpression = this._expression;
				return;
			}
			if (expr._op != 26)
			{
				if (this.IsSupportedOperator(expr._op))
				{
					if (expr._left is NameNode && expr._right is ConstNode)
					{
						Select.ColumnInfo columnInfo = this._candidateColumns[((NameNode)expr._left)._column.Ordinal];
						columnInfo.expr = ((columnInfo.expr == null) ? expr : new BinaryNode(this._table, 26, expr, columnInfo.expr));
						if (expr._op == 7)
						{
							columnInfo.equalsOperator = true;
						}
						this._candidatesForBinarySearch = true;
						return;
					}
					if (expr._right is NameNode && expr._left is ConstNode)
					{
						ExpressionNode left = expr._left;
						expr._left = expr._right;
						expr._right = left;
						switch (expr._op)
						{
						case 8:
							expr._op = 9;
							break;
						case 9:
							expr._op = 8;
							break;
						case 10:
							expr._op = 11;
							break;
						case 11:
							expr._op = 10;
							break;
						}
						Select.ColumnInfo columnInfo2 = this._candidateColumns[((NameNode)expr._left)._column.Ordinal];
						columnInfo2.expr = ((columnInfo2.expr == null) ? expr : new BinaryNode(this._table, 26, expr, columnInfo2.expr));
						if (expr._op == 7)
						{
							columnInfo2.equalsOperator = true;
						}
						this._candidatesForBinarySearch = true;
						return;
					}
				}
				this._linearExpression = ((this._linearExpression == null) ? expr : new BinaryNode(this._table, 26, expr, this._linearExpression));
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (expr._left is BinaryNode)
			{
				this.AnalyzeExpression((BinaryNode)expr._left);
				if (this._linearExpression == this._expression)
				{
					return;
				}
				flag = true;
			}
			else
			{
				UnaryNode unaryNode = expr._left as UnaryNode;
				if (unaryNode != null)
				{
					while (unaryNode._op == 0 && unaryNode._right is UnaryNode && ((UnaryNode)unaryNode._right)._op == 0)
					{
						unaryNode = (UnaryNode)unaryNode._right;
					}
					if (unaryNode._op == 0 && unaryNode._right is BinaryNode)
					{
						this.AnalyzeExpression((BinaryNode)unaryNode._right);
						if (this._linearExpression == this._expression)
						{
							return;
						}
						flag = true;
					}
				}
			}
			if (expr._right is BinaryNode)
			{
				this.AnalyzeExpression((BinaryNode)expr._right);
				if (this._linearExpression == this._expression)
				{
					return;
				}
				flag2 = true;
			}
			else
			{
				UnaryNode unaryNode2 = expr._right as UnaryNode;
				if (unaryNode2 != null)
				{
					while (unaryNode2._op == 0 && unaryNode2._right is UnaryNode && ((UnaryNode)unaryNode2._right)._op == 0)
					{
						unaryNode2 = (UnaryNode)unaryNode2._right;
					}
					if (unaryNode2._op == 0 && unaryNode2._right is BinaryNode)
					{
						this.AnalyzeExpression((BinaryNode)unaryNode2._right);
						if (this._linearExpression == this._expression)
						{
							return;
						}
						flag2 = true;
					}
				}
			}
			if (flag && flag2)
			{
				return;
			}
			ExpressionNode expressionNode = (flag ? expr._right : expr._left);
			this._linearExpression = ((this._linearExpression == null) ? expressionNode : new BinaryNode(this._table, 26, expressionNode, this._linearExpression));
		}

		private bool CompareSortIndexDesc(IndexField[] fields)
		{
			if (fields.Length < this._indexFields.Length)
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			while (num2 < fields.Length && num < this._indexFields.Length)
			{
				if (fields[num2] == this._indexFields[num])
				{
					num++;
				}
				else
				{
					Select.ColumnInfo columnInfo = this._candidateColumns[fields[num2].Column.Ordinal];
					if (columnInfo == null || !columnInfo.equalsOperator)
					{
						return false;
					}
				}
				num2++;
			}
			return num == this._indexFields.Length;
		}

		private bool FindSortIndex()
		{
			this._index = null;
			this._table._indexesLock.EnterUpgradeableReadLock();
			try
			{
				int count = this._table._indexes.Count;
				int count2 = this._table.Rows.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._table._indexes[i];
					if (index.RecordStates == this._recordStates && index.IsSharable && this.CompareSortIndexDesc(index._indexFields))
					{
						this._index = index;
						return true;
					}
				}
			}
			finally
			{
				this._table._indexesLock.ExitUpgradeableReadLock();
			}
			return false;
		}

		private int CompareClosestCandidateIndexDesc(IndexField[] fields)
		{
			int num = ((fields.Length < this._nCandidates) ? fields.Length : this._nCandidates);
			int i;
			for (i = 0; i < num; i++)
			{
				Select.ColumnInfo columnInfo = this._candidateColumns[fields[i].Column.Ordinal];
				if (columnInfo == null || columnInfo.expr == null)
				{
					break;
				}
				if (!columnInfo.equalsOperator)
				{
					return i + 1;
				}
			}
			return i;
		}

		private bool FindClosestCandidateIndex()
		{
			this._index = null;
			this._matchedCandidates = 0;
			bool flag = true;
			this._table._indexesLock.EnterUpgradeableReadLock();
			try
			{
				int count = this._table._indexes.Count;
				int count2 = this._table.Rows.Count;
				for (int i = 0; i < count; i++)
				{
					Index index = this._table._indexes[i];
					if (index.RecordStates == this._recordStates && index.IsSharable)
					{
						int num = this.CompareClosestCandidateIndexDesc(index._indexFields);
						if (num > this._matchedCandidates || (num == this._matchedCandidates && !flag))
						{
							this._matchedCandidates = num;
							this._index = index;
							flag = this.CompareSortIndexDesc(index._indexFields);
							if (this._matchedCandidates == this._nCandidates && flag)
							{
								return true;
							}
						}
					}
				}
			}
			finally
			{
				this._table._indexesLock.ExitUpgradeableReadLock();
			}
			return this._index != null && flag;
		}

		private void InitCandidateColumns()
		{
			this._nCandidates = 0;
			this._candidateColumns = new Select.ColumnInfo[this._table.Columns.Count];
			if (this._rowFilter == null)
			{
				return;
			}
			DataColumn[] dependency = this._rowFilter.GetDependency();
			for (int i = 0; i < dependency.Length; i++)
			{
				if (dependency[i].Table == this._table)
				{
					this._candidateColumns[dependency[i].Ordinal] = new Select.ColumnInfo();
					this._nCandidates++;
				}
			}
		}

		private void CreateIndex()
		{
			if (this._index == null)
			{
				if (this._nCandidates == 0)
				{
					this._index = new Index(this._table, this._indexFields, this._recordStates, null);
					this._index.AddRef();
					return;
				}
				int num = this._candidateColumns.Length;
				int num2 = this._indexFields.Length;
				bool flag = true;
				int i;
				for (i = 0; i < num; i++)
				{
					if (this._candidateColumns[i] != null && !this._candidateColumns[i].equalsOperator)
					{
						flag = false;
						break;
					}
				}
				int num3 = 0;
				for (i = 0; i < num2; i++)
				{
					Select.ColumnInfo columnInfo = this._candidateColumns[this._indexFields[i].Column.Ordinal];
					if (columnInfo != null)
					{
						columnInfo.flag = true;
						num3++;
					}
				}
				int num4 = num2 - num3;
				int nCandidates = this._nCandidates;
				IndexField[] array = new IndexField[this._nCandidates + num4];
				if (flag)
				{
					num3 = 0;
					for (i = 0; i < num; i++)
					{
						if (this._candidateColumns[i] != null)
						{
							array[num3++] = new IndexField(this._table.Columns[i], false);
							this._candidateColumns[i].flag = false;
						}
					}
					for (i = 0; i < num2; i++)
					{
						Select.ColumnInfo columnInfo2 = this._candidateColumns[this._indexFields[i].Column.Ordinal];
						if (columnInfo2 == null || columnInfo2.flag)
						{
							array[num3++] = this._indexFields[i];
							if (columnInfo2 != null)
							{
								columnInfo2.flag = false;
							}
						}
					}
					for (i = 0; i < this._candidateColumns.Length; i++)
					{
						if (this._candidateColumns[i] != null)
						{
							this._candidateColumns[i].flag = false;
						}
					}
					this._index = new Index(this._table, array, this._recordStates, null);
					if (!this.IsOperatorIn(this._expression))
					{
						this._index.AddRef();
					}
					this._matchedCandidates = this._nCandidates;
					return;
				}
				for (i = 0; i < num2; i++)
				{
					array[i] = this._indexFields[i];
					Select.ColumnInfo columnInfo3 = this._candidateColumns[this._indexFields[i].Column.Ordinal];
					if (columnInfo3 != null)
					{
						columnInfo3.flag = true;
					}
				}
				num3 = i;
				for (i = 0; i < num; i++)
				{
					if (this._candidateColumns[i] != null)
					{
						if (!this._candidateColumns[i].flag)
						{
							array[num3++] = new IndexField(this._table.Columns[i], false);
						}
						else
						{
							this._candidateColumns[i].flag = false;
						}
					}
				}
				this._index = new Index(this._table, array, this._recordStates, null);
				this._matchedCandidates = 0;
				if (this._linearExpression != this._expression)
				{
					IndexField[] indexFields = this._index._indexFields;
					while (this._matchedCandidates < num3)
					{
						Select.ColumnInfo columnInfo4 = this._candidateColumns[indexFields[this._matchedCandidates].Column.Ordinal];
						if (columnInfo4 == null || columnInfo4.expr == null)
						{
							break;
						}
						this._matchedCandidates++;
						if (!columnInfo4.equalsOperator)
						{
							break;
						}
					}
				}
				for (i = 0; i < this._candidateColumns.Length; i++)
				{
					if (this._candidateColumns[i] != null)
					{
						this._candidateColumns[i].flag = false;
					}
				}
			}
		}

		private bool IsOperatorIn(ExpressionNode enode)
		{
			BinaryNode binaryNode = enode as BinaryNode;
			return binaryNode != null && (5 == binaryNode._op || this.IsOperatorIn(binaryNode._right) || this.IsOperatorIn(binaryNode._left));
		}

		private void BuildLinearExpression()
		{
			IndexField[] indexFields = this._index._indexFields;
			int num = indexFields.Length;
			for (int i = 0; i < this._matchedCandidates; i++)
			{
				this._candidateColumns[indexFields[i].Column.Ordinal].flag = true;
			}
			int num2 = this._candidateColumns.Length;
			for (int i = 0; i < num2; i++)
			{
				if (this._candidateColumns[i] != null)
				{
					if (!this._candidateColumns[i].flag)
					{
						if (this._candidateColumns[i].expr != null)
						{
							this._linearExpression = ((this._linearExpression == null) ? this._candidateColumns[i].expr : new BinaryNode(this._table, 26, this._candidateColumns[i].expr, this._linearExpression));
						}
					}
					else
					{
						this._candidateColumns[i].flag = false;
					}
				}
			}
		}

		public DataRow[] SelectRows()
		{
			bool flag = true;
			this.InitCandidateColumns();
			if (this._expression is BinaryNode)
			{
				this.AnalyzeExpression((BinaryNode)this._expression);
				if (!this._candidatesForBinarySearch)
				{
					this._linearExpression = this._expression;
				}
				if (this._linearExpression == this._expression)
				{
					for (int i = 0; i < this._candidateColumns.Length; i++)
					{
						if (this._candidateColumns[i] != null)
						{
							this._candidateColumns[i].equalsOperator = false;
							this._candidateColumns[i].expr = null;
						}
					}
				}
				else
				{
					flag = !this.FindClosestCandidateIndex();
				}
			}
			else
			{
				this._linearExpression = this._expression;
			}
			if (this._index == null && (this._indexFields.Length != 0 || this._linearExpression == this._expression))
			{
				flag = !this.FindSortIndex();
			}
			if (this._index == null)
			{
				this.CreateIndex();
				flag = false;
			}
			if (this._index.RecordCount == 0)
			{
				return this._table.NewRowArray(0);
			}
			Range binaryFilteredRecords;
			if (this._matchedCandidates == 0)
			{
				binaryFilteredRecords = new Range(0, this._index.RecordCount - 1);
				this._linearExpression = this._expression;
				return this.GetLinearFilteredRows(binaryFilteredRecords);
			}
			binaryFilteredRecords = this.GetBinaryFilteredRecords();
			if (binaryFilteredRecords.Count == 0)
			{
				return this._table.NewRowArray(0);
			}
			if (this._matchedCandidates < this._nCandidates)
			{
				this.BuildLinearExpression();
			}
			if (!flag)
			{
				return this.GetLinearFilteredRows(binaryFilteredRecords);
			}
			this._records = this.GetLinearFilteredRecords(binaryFilteredRecords);
			this._recordCount = this._records.Length;
			if (this._recordCount == 0)
			{
				return this._table.NewRowArray(0);
			}
			this.Sort(0, this._recordCount - 1);
			return this.GetRows();
		}

		public DataRow[] GetRows()
		{
			DataRow[] array = this._table.NewRowArray(this._recordCount);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this._table._recordManager[this._records[i]];
			}
			return array;
		}

		private bool AcceptRecord(int record)
		{
			DataRow dataRow = this._table._recordManager[record];
			if (dataRow == null)
			{
				return true;
			}
			DataRowVersion dataRowVersion = DataRowVersion.Default;
			if (dataRow._oldRecord == record)
			{
				dataRowVersion = DataRowVersion.Original;
			}
			else if (dataRow._newRecord == record)
			{
				dataRowVersion = DataRowVersion.Current;
			}
			else if (dataRow._tempRecord == record)
			{
				dataRowVersion = DataRowVersion.Proposed;
			}
			object obj = this._linearExpression.Eval(dataRow, dataRowVersion);
			bool flag;
			try
			{
				flag = DataExpression.ToBoolean(obj);
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				throw ExprException.FilterConvertion(this._rowFilter.Expression);
			}
			return flag;
		}

		private int Eval(BinaryNode expr, DataRow row, DataRowVersion version)
		{
			if (expr._op != 26)
			{
				long num = 0L;
				object obj = expr._left.Eval(row, version);
				if (expr._op != 13 && expr._op != 39)
				{
					object obj2 = expr._right.Eval(row, version);
					bool flag = expr._left is ConstNode;
					bool flag2 = expr._right is ConstNode;
					if (obj == DBNull.Value || (expr._left.IsSqlColumn && DataStorage.IsObjectSqlNull(obj)))
					{
						return -1;
					}
					if (obj2 == DBNull.Value || (expr._right.IsSqlColumn && DataStorage.IsObjectSqlNull(obj2)))
					{
						return 1;
					}
					StorageType storageType = DataStorage.GetStorageType(obj.GetType());
					if (StorageType.Char == storageType)
					{
						if (flag2 || !expr._right.IsSqlColumn)
						{
							obj2 = Convert.ToChar(obj2, this._table.FormatProvider);
						}
						else
						{
							obj2 = SqlConvert.ChangeType2(obj2, StorageType.Char, typeof(char), this._table.FormatProvider);
						}
					}
					StorageType storageType2 = DataStorage.GetStorageType(obj2.GetType());
					StorageType storageType3;
					if (expr._left.IsSqlColumn || expr._right.IsSqlColumn)
					{
						storageType3 = expr.ResultSqlType(storageType, storageType2, flag, flag2, expr._op);
					}
					else
					{
						storageType3 = expr.ResultType(storageType, storageType2, flag, flag2, expr._op);
					}
					if (storageType3 == StorageType.Empty)
					{
						expr.SetTypeMismatchError(expr._op, obj.GetType(), obj2.GetType());
					}
					NameNode nameNode;
					CompareInfo compareInfo = (((flag && !flag2 && storageType == StorageType.String && storageType2 == StorageType.Guid && (nameNode = expr._right as NameNode) != null && nameNode._column.DataType == typeof(Guid)) || (flag2 && !flag && storageType2 == StorageType.String && storageType == StorageType.Guid && (nameNode = expr._left as NameNode) != null && nameNode._column.DataType == typeof(Guid))) ? CultureInfo.InvariantCulture.CompareInfo : null);
					num = (long)expr.BinaryCompare(obj, obj2, storageType3, expr._op, compareInfo);
				}
				int op = expr._op;
				switch (op)
				{
				case 7:
					num = ((num == 0L) ? 0L : ((num < 0L) ? (-1L) : 1L));
					break;
				case 8:
					num = ((num > 0L) ? 0L : (-1L));
					break;
				case 9:
					num = ((num < 0L) ? 0L : 1L);
					break;
				case 10:
					num = ((num >= 0L) ? 0L : (-1L));
					break;
				case 11:
					num = ((num <= 0L) ? 0L : 1L);
					break;
				case 12:
					break;
				case 13:
					num = ((obj == DBNull.Value) ? 0L : (-1L));
					break;
				default:
					if (op == 39)
					{
						num = ((obj != DBNull.Value) ? 0L : 1L);
					}
					break;
				}
				return (int)num;
			}
			int num2 = this.Eval((BinaryNode)expr._left, row, version);
			if (num2 != 0)
			{
				return num2;
			}
			int num3 = this.Eval((BinaryNode)expr._right, row, version);
			if (num3 != 0)
			{
				return num3;
			}
			return 0;
		}

		private int Evaluate(int record)
		{
			DataRow dataRow = this._table._recordManager[record];
			if (dataRow == null)
			{
				return 0;
			}
			DataRowVersion dataRowVersion = DataRowVersion.Default;
			if (dataRow._oldRecord == record)
			{
				dataRowVersion = DataRowVersion.Original;
			}
			else if (dataRow._newRecord == record)
			{
				dataRowVersion = DataRowVersion.Current;
			}
			else if (dataRow._tempRecord == record)
			{
				dataRowVersion = DataRowVersion.Proposed;
			}
			IndexField[] indexFields = this._index._indexFields;
			int i = 0;
			while (i < this._matchedCandidates)
			{
				int ordinal = indexFields[i].Column.Ordinal;
				int num = this.Eval(this._candidateColumns[ordinal].expr, dataRow, dataRowVersion);
				if (num != 0)
				{
					if (!indexFields[i].IsDescending)
					{
						return num;
					}
					return -num;
				}
				else
				{
					i++;
				}
			}
			return 0;
		}

		private int FindFirstMatchingRecord()
		{
			int num = -1;
			int i = 0;
			int num2 = this._index.RecordCount - 1;
			while (i <= num2)
			{
				int num3 = i + num2 >> 1;
				int record = this._index.GetRecord(num3);
				int num4 = this.Evaluate(record);
				if (num4 == 0)
				{
					num = num3;
				}
				if (num4 < 0)
				{
					i = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			return num;
		}

		private int FindLastMatchingRecord(int lo)
		{
			int num = -1;
			int num2 = this._index.RecordCount - 1;
			while (lo <= num2)
			{
				int num3 = lo + num2 >> 1;
				int record = this._index.GetRecord(num3);
				int num4 = this.Evaluate(record);
				if (num4 == 0)
				{
					num = num3;
				}
				if (num4 <= 0)
				{
					lo = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			return num;
		}

		private Range GetBinaryFilteredRecords()
		{
			if (this._matchedCandidates == 0)
			{
				return new Range(0, this._index.RecordCount - 1);
			}
			int num = this.FindFirstMatchingRecord();
			if (num == -1)
			{
				return default(Range);
			}
			int num2 = this.FindLastMatchingRecord(num);
			return new Range(num, num2);
		}

		private int[] GetLinearFilteredRecords(Range range)
		{
			if (this._linearExpression == null)
			{
				int[] array = new int[range.Count];
				RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(range.Min);
				int num = 0;
				while (num < range.Count && enumerator.MoveNext())
				{
					array[num] = enumerator.Current;
					num++;
				}
				return array;
			}
			List<int> list = new List<int>();
			RBTree<int>.RBTreeEnumerator enumerator2 = this._index.GetEnumerator(range.Min);
			int num2 = 0;
			while (num2 < range.Count && enumerator2.MoveNext())
			{
				if (this.AcceptRecord(enumerator2.Current))
				{
					list.Add(enumerator2.Current);
				}
				num2++;
			}
			return list.ToArray();
		}

		private DataRow[] GetLinearFilteredRows(Range range)
		{
			if (this._linearExpression == null)
			{
				return this._index.GetRows(range);
			}
			List<DataRow> list = new List<DataRow>();
			RBTree<int>.RBTreeEnumerator enumerator = this._index.GetEnumerator(range.Min);
			int num = 0;
			while (num < range.Count && enumerator.MoveNext())
			{
				if (this.AcceptRecord(enumerator.Current))
				{
					list.Add(this._table._recordManager[enumerator.Current]);
				}
				num++;
			}
			DataRow[] array = this._table.NewRowArray(list.Count);
			list.CopyTo(array);
			return array;
		}

		private int CompareRecords(int record1, int record2)
		{
			int num = this._indexFields.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = this._indexFields[i].Column.Compare(record1, record2);
				if (num2 != 0)
				{
					if (this._indexFields[i].IsDescending)
					{
						num2 = -num2;
					}
					return num2;
				}
			}
			long num3 = ((this._table._recordManager[record1] == null) ? 0L : this._table._recordManager[record1].rowID);
			long num4 = ((this._table._recordManager[record2] == null) ? 0L : this._table._recordManager[record2].rowID);
			int num5 = ((num3 < num4) ? (-1) : ((num4 < num3) ? 1 : 0));
			if (num5 == 0 && record1 != record2 && this._table._recordManager[record1] != null && this._table._recordManager[record2] != null)
			{
				num3 = (long)this._table._recordManager[record1].GetRecordState(record1);
				num4 = (long)this._table._recordManager[record2].GetRecordState(record2);
				num5 = ((num3 < num4) ? (-1) : ((num4 < num3) ? 1 : 0));
			}
			return num5;
		}

		private void Sort(int left, int right)
		{
			int num;
			do
			{
				num = left;
				int num2 = right;
				int num3 = this._records[num + num2 >> 1];
				for (;;)
				{
					if (this.CompareRecords(this._records[num], num3) >= 0)
					{
						while (this.CompareRecords(this._records[num2], num3) > 0)
						{
							num2--;
						}
						if (num <= num2)
						{
							int num4 = this._records[num];
							this._records[num] = this._records[num2];
							this._records[num2] = num4;
							num++;
							num2--;
						}
						if (num > num2)
						{
							break;
						}
					}
					else
					{
						num++;
					}
				}
				if (left < num2)
				{
					this.Sort(left, num2);
				}
				left = num;
			}
			while (num < right);
		}

		private readonly DataTable _table;

		private readonly IndexField[] _indexFields;

		private DataViewRowState _recordStates;

		private DataExpression _rowFilter;

		private ExpressionNode _expression;

		private Index _index;

		private int[] _records;

		private int _recordCount;

		private ExpressionNode _linearExpression;

		private bool _candidatesForBinarySearch;

		private Select.ColumnInfo[] _candidateColumns;

		private int _nCandidates;

		private int _matchedCandidates;

		private sealed class ColumnInfo
		{
			public bool flag;

			public bool equalsOperator;

			public BinaryNode expr;
		}
	}
}
