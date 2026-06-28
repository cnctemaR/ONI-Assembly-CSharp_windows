using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;

namespace FileHelpers
{
	internal sealed class RecordOperations
	{
		public IRecordInfo RecordInfo { get; private set; }

		public RecordOperations(IRecordInfo recordInfo)
		{
			this.RecordInfo = recordInfo;
		}

		public object StringToRecord(LineInfo line, object[] values)
		{
			if (this.MustIgnoreLine(line.mLineStr))
			{
				return null;
			}
			for (int i = 0; i < this.RecordInfo.FieldCount; i++)
			{
				values[i] = this.RecordInfo.Fields[i].ExtractFieldValue(line);
			}
			object obj;
			try
			{
				obj = this.CreateHandler(values);
			}
			catch (InvalidCastException ex)
			{
				for (int j = 0; j < this.RecordInfo.FieldCount; j++)
				{
					if (values[j] != null && !this.RecordInfo.Fields[j].FieldTypeInternal.IsInstanceOfType(values[j]))
					{
						throw new ConvertException(null, this.RecordInfo.Fields[j].FieldTypeInternal, this.RecordInfo.Fields[j].FieldInfo.Name, line.mReader.LineNumber, -1, Messages.Errors.WrongConverter.FieldName(this.RecordInfo.Fields[j].FieldInfo.Name).ConverterReturnedType(values[j].GetType().Name).FieldType(this.RecordInfo.Fields[j].FieldInfo.FieldType.Name)
							.Text, ex);
					}
				}
				obj = null;
			}
			return obj;
		}

		public bool StringToRecord(object record, LineInfo line, object[] values)
		{
			if (this.MustIgnoreLine(line.mLineStr))
			{
				return false;
			}
			for (int i = 0; i < this.RecordInfo.FieldCount; i++)
			{
				values[i] = this.RecordInfo.Fields[i].ExtractFieldValue(line);
			}
			bool flag;
			try
			{
				this.AssignHandler(record, values);
				flag = true;
			}
			catch (InvalidCastException ex)
			{
				for (int j = 0; j < this.RecordInfo.FieldCount; j++)
				{
					if (values[j] != null && !this.RecordInfo.Fields[j].FieldTypeInternal.IsInstanceOfType(values[j]))
					{
						throw new ConvertException(null, this.RecordInfo.Fields[j].FieldTypeInternal, this.RecordInfo.Fields[j].FieldInfo.Name, line.mReader.LineNumber, -1, Messages.Errors.WrongConverter.FieldName(this.RecordInfo.Fields[j].FieldInfo.Name).ConverterReturnedType(values[j].GetType().Name).FieldType(this.RecordInfo.Fields[j].FieldInfo.FieldType.Name)
							.Text, ex);
					}
				}
				throw;
			}
			return flag;
		}

		private bool MustIgnoreLine(string line)
		{
			if (this.RecordInfo.IgnoreEmptyLines && ((this.RecordInfo.IgnoreEmptySpaces && StringHelper.IsNullOrWhiteSpace(line)) || line.Length == 0))
			{
				return true;
			}
			if (!string.IsNullOrEmpty(this.RecordInfo.CommentMarker) && ((this.RecordInfo.CommentAnyPlace && StringHelper.StartsWithIgnoringWhiteSpaces(line, this.RecordInfo.CommentMarker, StringComparison.Ordinal)) || line.StartsWith(this.RecordInfo.CommentMarker, StringComparison.Ordinal)))
			{
				return true;
			}
			if (this.RecordInfo.RecordCondition != RecordCondition.None)
			{
				switch (this.RecordInfo.RecordCondition)
				{
				case RecordCondition.IncludeIfContains:
					return !ConditionHelper.Contains(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.IncludeIfBegins:
					return !ConditionHelper.BeginsWith(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.IncludeIfEnds:
					return !ConditionHelper.EndsWith(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.IncludeIfEnclosed:
					return !ConditionHelper.Enclosed(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.IncludeIfMatchRegex:
					return !this.RecordInfo.RecordConditionRegEx.IsMatch(line);
				case RecordCondition.ExcludeIfContains:
					return ConditionHelper.Contains(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.ExcludeIfBegins:
					return ConditionHelper.BeginsWith(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.ExcludeIfEnds:
					return ConditionHelper.EndsWith(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.ExcludeIfEnclosed:
					return ConditionHelper.Enclosed(line, this.RecordInfo.RecordConditionSelector);
				case RecordCondition.ExcludeIfMatchRegex:
					return this.RecordInfo.RecordConditionRegEx.IsMatch(line);
				}
			}
			return false;
		}

		public string RecordToString(object record)
		{
			StringBuilder stringBuilder = new StringBuilder(this.RecordInfo.SizeHint);
			object[] array = this.ObjectToValuesHandler(record);
			for (int i = 0; i < this.RecordInfo.FieldCount; i++)
			{
				this.RecordInfo.Fields[i].AssignToString(stringBuilder, array[i]);
			}
			return stringBuilder.ToString();
		}

		public string RecordValuesToString(object[] recordValues)
		{
			StringBuilder stringBuilder = new StringBuilder(this.RecordInfo.SizeHint);
			for (int i = 0; i < this.RecordInfo.FieldCount; i++)
			{
				this.RecordInfo.Fields[i].AssignToString(stringBuilder, recordValues[i]);
			}
			return stringBuilder.ToString();
		}

		public object ValuesToRecord(object[] values)
		{
			for (int i = 0; i < this.RecordInfo.FieldCount; i++)
			{
				values[i] = this.RecordInfo.Fields[i].CreateValueForField(values[i]);
			}
			return this.CreateHandler(values);
		}

		public object[] RecordToValues(object record)
		{
			return this.ObjectToValuesHandler(record);
		}

		public DataTable RecordsToDataTable(ICollection records)
		{
			return this.RecordsToDataTable(records, -1);
		}

		public DataTable RecordsToDataTable(ICollection records, int maxRecords)
		{
			DataTable dataTable = this.CreateEmptyDataTable();
			dataTable.BeginLoadData();
			dataTable.MinimumCapacity = records.Count;
			if (maxRecords == -1)
			{
				using (IEnumerator enumerator = records.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						dataTable.Rows.Add(this.RecordToValues(obj));
					}
					goto IL_00B1;
				}
			}
			int num = 0;
			foreach (object obj2 in records)
			{
				if (num == maxRecords)
				{
					break;
				}
				dataTable.Rows.Add(this.RecordToValues(obj2));
				num++;
			}
			IL_00B1:
			dataTable.EndLoadData();
			return dataTable;
		}

		public DataTable CreateEmptyDataTable()
		{
			DataTable dataTable = new DataTable();
			foreach (FieldBase fieldBase in this.RecordInfo.Fields)
			{
				DataColumn dataColumn;
				if (fieldBase.IsNullableType)
				{
					dataColumn = dataTable.Columns.Add(fieldBase.FieldInfo.Name, Nullable.GetUnderlyingType(fieldBase.FieldInfo.FieldType));
					dataColumn.AllowDBNull = true;
				}
				else
				{
					dataColumn = dataTable.Columns.Add(fieldBase.FieldInfo.Name, fieldBase.FieldInfo.FieldType);
				}
				dataColumn.ReadOnly = true;
			}
			return dataTable;
		}

		private ObjectToValuesDelegate ObjectToValuesHandler
		{
			get
			{
				ObjectToValuesDelegate objectToValuesDelegate;
				if ((objectToValuesDelegate = this.mObjectToValuesHandler) == null)
				{
					objectToValuesDelegate = (this.mObjectToValuesHandler = ReflectionHelper.ObjectToValuesMethod(this.RecordInfo.RecordType, this.GetFieldInfoArray()));
				}
				return objectToValuesDelegate;
			}
		}

		private CreateAndAssignDelegate CreateHandler
		{
			get
			{
				if (this.mCreateHandler == null)
				{
					this.mCreateHandler = ReflectionHelper.CreateAndAssignValuesMethod(this.RecordInfo.RecordType, this.GetFieldInfoArray());
				}
				return this.mCreateHandler;
			}
		}

		private AssignDelegate AssignHandler
		{
			get
			{
				if (this.mAssignHandler == null)
				{
					this.mAssignHandler = ReflectionHelper.AssignValuesMethod(this.RecordInfo.RecordType, this.GetFieldInfoArray());
				}
				return this.mAssignHandler;
			}
		}

		internal CreateObjectDelegate CreateRecordHandler
		{
			get
			{
				if (this.mFastConstructor == null)
				{
					this.mFastConstructor = ReflectionHelper.CreateFastConstructor(this.RecordInfo.RecordType);
				}
				return this.mFastConstructor;
			}
		}

		private FieldInfo[] GetFieldInfoArray()
		{
			FieldInfo[] array = new FieldInfo[this.RecordInfo.Fields.Length];
			for (int i = 0; i < this.RecordInfo.Fields.Length; i++)
			{
				array[i] = this.RecordInfo.Fields[i].FieldInfo;
			}
			return array;
		}

		public RecordOperations Clone(RecordInfo ri)
		{
			return new RecordOperations(ri)
			{
				mCreateHandler = this.mCreateHandler,
				mFastConstructor = this.mFastConstructor,
				mObjectToValuesHandler = this.mObjectToValuesHandler
			};
		}

		private CreateAndAssignDelegate mCreateHandler;

		private AssignDelegate mAssignHandler;

		private CreateObjectDelegate mFastConstructor;

		private ObjectToValuesDelegate mObjectToValuesHandler;
	}
}
