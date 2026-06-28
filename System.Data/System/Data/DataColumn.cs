using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	[DefaultProperty("ColumnName")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataColumnEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DesignTimeVisible(false)]
	[ToolboxItem(false)]
	public class DataColumn : MarshalByValueComponent
	{
		public DataColumn()
			: this(string.Empty, typeof(string), string.Empty, MappingType.Element)
		{
		}

		public DataColumn(string columnName)
			: this(columnName, typeof(string), string.Empty, MappingType.Element)
		{
		}

		public DataColumn(string columnName, Type dataType)
			: this(columnName, dataType, string.Empty, MappingType.Element)
		{
		}

		public DataColumn(string columnName, Type dataType, string expr)
			: this(columnName, dataType, expr, MappingType.Element)
		{
		}

		public DataColumn(string columnName, Type dataType, string expr, MappingType type)
		{
			this.ColumnName = ((columnName != null) ? columnName : string.Empty);
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			this.DataType = dataType;
			this.Expression = ((expr != null) ? expr : string.Empty);
			this.ColumnMapping = type;
		}

		internal event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				this._eventHandlers.AddHandler(DataColumn._propertyChangedKey, value);
			}
			remove
			{
				this._eventHandlers.RemoveHandler(DataColumn._propertyChangedKey, value);
			}
		}

		internal object this[int index]
		{
			get
			{
				return this.DataContainer[index];
			}
			set
			{
				if (value == null)
				{
					if (this.AutoIncrement)
					{
						goto IL_0064;
					}
				}
				try
				{
					this.DataContainer[index] = value;
				}
				catch (Exception ex)
				{
					throw new ArgumentException(string.Format("{0}. Couldn't store <{1}> in Column named '{2}'. Expected type is {3}.", new object[]
					{
						ex.Message,
						value,
						this.ColumnName,
						this.DataType.Name
					}), ex);
				}
				IL_0064:
				if (this.AutoIncrement && !this.DataContainer.IsNull(index))
				{
					long num = Convert.ToInt64(value);
					this.UpdateAutoIncrementValue(num);
				}
			}
		}

		[DefaultValue(DataSetDateTime.UnspecifiedLocal)]
		[RefreshProperties(RefreshProperties.All)]
		public DataSetDateTime DateTimeMode
		{
			get
			{
				return this._datetimeMode;
			}
			set
			{
				if (this.DataType != typeof(DateTime))
				{
					throw new InvalidOperationException("The DateTimeMode can be set only on DataColumns of type DateTime.");
				}
				if (!Enum.IsDefined(typeof(DataSetDateTime), value))
				{
					throw new InvalidEnumArgumentException(string.Format(CultureInfo.InvariantCulture, "The {0} enumeration value, {1}, is invalid", new object[]
					{
						typeof(DataSetDateTime).Name,
						value
					}));
				}
				if (this._datetimeMode == value)
				{
					return;
				}
				if (this._table == null || this._table.Rows.Count == 0)
				{
					this._datetimeMode = value;
					return;
				}
				if ((this._datetimeMode == DataSetDateTime.Unspecified || this._datetimeMode == DataSetDateTime.UnspecifiedLocal) && (value == DataSetDateTime.Unspecified || value == DataSetDateTime.UnspecifiedLocal))
				{
					this._datetimeMode = value;
					return;
				}
				throw new InvalidOperationException(string.Format("Cannot change DateTimeMode from '{0}' to '{1}' once the table has data.", this._datetimeMode, value));
			}
		}

		[DefaultValue(true)]
		[DataCategory("Data")]
		public bool AllowDBNull
		{
			get
			{
				return this._allowDBNull;
			}
			set
			{
				if (!value && this._table != null)
				{
					for (int i = 0; i < this._table.Rows.Count; i++)
					{
						DataRow dataRow = this._table.Rows[i];
						DataRowVersion dataRowVersion = ((!dataRow.HasVersion(DataRowVersion.Default)) ? DataRowVersion.Original : DataRowVersion.Default);
						if (dataRow.IsNull(this, dataRowVersion))
						{
							throw new DataException("Column '" + this.ColumnName + "' has null values in it.");
						}
					}
				}
				this._allowDBNull = value;
			}
		}

		[DefaultValue(false)]
		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		public bool AutoIncrement
		{
			get
			{
				return this._autoIncrement;
			}
			set
			{
				if (value)
				{
					if (this.Expression != string.Empty)
					{
						throw new ArgumentException("Can not Auto Increment a computed column.");
					}
					if (this.DefaultValue != DBNull.Value)
					{
						throw new ArgumentException("Can not set AutoIncrement while default value exists for this column.");
					}
					if (!DataColumn.CanAutoIncrement(this.DataType))
					{
						this.DataType = typeof(int);
					}
				}
				if (this._table != null)
				{
					this._table.Columns.UpdateAutoIncrement(this, value);
				}
				this._autoIncrement = value;
			}
		}

		[DefaultValue(0)]
		[DataCategory("Data")]
		public long AutoIncrementSeed
		{
			get
			{
				return this._autoIncrementSeed;
			}
			set
			{
				this._autoIncrementSeed = value;
				this._nextAutoIncrementValue = this._autoIncrementSeed;
			}
		}

		[DefaultValue(1)]
		[DataCategory("Data")]
		public long AutoIncrementStep
		{
			get
			{
				return this._autoIncrementStep;
			}
			set
			{
				this._autoIncrementStep = value;
			}
		}

		internal void UpdateAutoIncrementValue(long value64)
		{
			if (this._autoIncrementStep > 0L)
			{
				if (value64 >= this._nextAutoIncrementValue)
				{
					this._nextAutoIncrementValue = value64;
					this.AutoIncrementValue();
				}
			}
			else if (value64 <= this._nextAutoIncrementValue)
			{
				this.AutoIncrementValue();
			}
		}

		internal long AutoIncrementValue()
		{
			long nextAutoIncrementValue = this._nextAutoIncrementValue;
			this._nextAutoIncrementValue += this.AutoIncrementStep;
			return nextAutoIncrementValue;
		}

		internal long GetAutoIncrementValue()
		{
			return this._nextAutoIncrementValue;
		}

		internal void SetDefaultValue(int index)
		{
			if (this.AutoIncrement)
			{
				this[index] = this._nextAutoIncrementValue;
			}
			else
			{
				this.DataContainer.CopyValue(this.Table.DefaultValuesRowIndex, index);
			}
		}

		[DataCategory("Data")]
		public string Caption
		{
			get
			{
				return (this._caption != null) ? this._caption : this.ColumnName;
			}
			set
			{
				this._caption = ((value != null) ? value : string.Empty);
			}
		}

		[DefaultValue(MappingType.Element)]
		public virtual MappingType ColumnMapping
		{
			get
			{
				return this._columnMapping;
			}
			set
			{
				this._columnMapping = value;
			}
		}

		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public string ColumnName
		{
			get
			{
				return this._columnName;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				CultureInfo cultureInfo = ((this.Table == null) ? CultureInfo.CurrentCulture : this.Table.Locale);
				if (string.Compare(value, this._columnName, true, cultureInfo) != 0)
				{
					if (this.Table != null)
					{
						if (value.Length == 0)
						{
							throw new ArgumentException("ColumnName is required when it is part of a DataTable.");
						}
						this.Table.Columns.RegisterName(value, this);
						if (this._columnName.Length > 0)
						{
							this.Table.Columns.UnregisterName(this._columnName);
						}
					}
					this.RaisePropertyChanging("ColumnName");
					this._columnName = value;
					if (this.Table != null)
					{
						this.Table.ResetPropertyDescriptorsCache();
					}
				}
				else if (string.Compare(value, this._columnName, false, cultureInfo) != 0)
				{
					this.RaisePropertyChanging("ColumnName");
					this._columnName = value;
					if (this.Table != null)
					{
						this.Table.ResetPropertyDescriptorsCache();
					}
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ColumnTypeConverter))]
		[DataCategory("Data")]
		[DefaultValue(typeof(string))]
		public Type DataType
		{
			get
			{
				return this.DataContainer.Type;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				if (this._dataContainer != null)
				{
					if (value == this._dataContainer.Type)
					{
						return;
					}
					if (this._dataContainer.Capacity > 0)
					{
						throw new ArgumentException("The column already has data stored.");
					}
				}
				if (this.GetParentRelation() != null || this.GetChildRelation() != null)
				{
					throw new InvalidConstraintException("Cannot change datatype when column is part of a relation");
				}
				Type type = ((this._dataContainer == null) ? null : this._dataContainer.Type);
				if (this._dataContainer != null && this._dataContainer.Type == typeof(DateTime))
				{
					this._datetimeMode = DataSetDateTime.UnspecifiedLocal;
				}
				this._dataContainer = DataContainer.Create(value, this);
				if (this.AutoIncrement && !DataColumn.CanAutoIncrement(value))
				{
					this.AutoIncrement = false;
				}
				if (this.DefaultValue != DataColumn.GetDefaultValueForType(type))
				{
					this.SetDefaultValue(this.DefaultValue, true);
				}
				else
				{
					this._defaultValue = DataColumn.GetDefaultValueForType(this.DataType);
				}
			}
		}

		[DataCategory("Data")]
		[TypeConverter(typeof(DefaultValueTypeConverter))]
		public object DefaultValue
		{
			get
			{
				return this._defaultValue;
			}
			set
			{
				if (this.AutoIncrement)
				{
					throw new ArgumentException("Can not set default value while AutoIncrement is true on this column.");
				}
				this.SetDefaultValue(value, false);
			}
		}

		private void SetDefaultValue(object value, bool forcedTypeCheck)
		{
			if (forcedTypeCheck || !this._defaultValue.Equals(value))
			{
				if (value == null || value == DBNull.Value)
				{
					this._defaultValue = DataColumn.GetDefaultValueForType(this.DataType);
				}
				else if (this.DataType.IsInstanceOfType(value))
				{
					this._defaultValue = value;
				}
				else
				{
					try
					{
						this._defaultValue = Convert.ChangeType(value, this.DataType);
					}
					catch (InvalidCastException)
					{
						string text = string.Format("Default Value of type '{0}' is not compatible with column type '{1}'", value.GetType(), this.DataType);
						throw new DataException(text);
					}
				}
			}
			if (this.Table != null && this.Table.DefaultValuesRowIndex != -1)
			{
				this.DataContainer[this.Table.DefaultValuesRowIndex] = this._defaultValue;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		[DataCategory("Data")]
		public string Expression
		{
			get
			{
				return this._expression;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value != string.Empty)
				{
					if (this.AutoIncrement || this.Unique)
					{
						throw new ArgumentException("Cannot create an expression on a column that has AutoIncrement or Unique.");
					}
					if (this.Table != null)
					{
						for (int i = 0; i < this.Table.Constraints.Count; i++)
						{
							if (this.Table.Constraints[i].IsColumnContained(this))
							{
								throw new ArgumentException(string.Format("Cannot set Expression property on column {0}, because it is a part of a constraint.", this.ColumnName));
							}
						}
					}
					Parser parser = new Parser();
					IExpression expression = parser.Compile(value);
					if (this.Table != null)
					{
						if (expression.DependsOn(this))
						{
							throw new ArgumentException("Cannot set Expression property due to circular reference in the expression.");
						}
						if (this.Table.Rows.Count == 0)
						{
							expression.Eval(this.Table.NewRow());
						}
						else
						{
							expression.Eval(this.Table.Rows[0]);
						}
					}
					this.ReadOnly = true;
					this._compiledExpression = expression;
				}
				else
				{
					this._compiledExpression = null;
					if (this.Table != null)
					{
						int defaultValuesRowIndex = this.Table.DefaultValuesRowIndex;
						if (defaultValuesRowIndex != -1)
						{
							this.DataContainer.FillValues(defaultValuesRowIndex);
						}
					}
				}
				this._expression = value;
			}
		}

		internal IExpression CompiledExpression
		{
			get
			{
				return this._compiledExpression;
			}
		}

		[DataCategory("Data")]
		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return this._extendedProperties;
			}
			internal set
			{
				this._extendedProperties = value;
			}
		}

		[DefaultValue(-1)]
		[DataCategory("Data")]
		public int MaxLength
		{
			get
			{
				return this._maxLength;
			}
			set
			{
				if (value >= 0 && this._columnMapping == MappingType.SimpleContent)
				{
					throw new ArgumentException(string.Format("Cannot set MaxLength property on '{0}' column which is mapped to SimpleContent.", this.ColumnName));
				}
				this._maxLength = value;
			}
		}

		[DataCategory("Data")]
		public string Namespace
		{
			get
			{
				if (this._nameSpace != null)
				{
					return this._nameSpace;
				}
				if (this.Table != null && this._columnMapping != MappingType.Attribute)
				{
					return this.Table.Namespace;
				}
				return string.Empty;
			}
			set
			{
				this._nameSpace = value;
			}
		}

		[Browsable(false)]
		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Ordinal
		{
			get
			{
				return this._ordinal;
			}
			internal set
			{
				this._ordinal = value;
			}
		}

		public void SetOrdinal(int ordinal)
		{
			if (this._ordinal == -1)
			{
				throw new ArgumentException("Column must belong to a table.");
			}
			this._table.Columns.MoveColumn(this._ordinal, ordinal);
			this._ordinal = ordinal;
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				return this._prefix;
			}
			set
			{
				this._prefix = ((value != null) ? value : string.Empty);
			}
		}

		[DataCategory("Data")]
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get
			{
				return this._readOnly;
			}
			set
			{
				this._readOnly = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DataCategory("Data")]
		public DataTable Table
		{
			get
			{
				return this._table;
			}
			internal set
			{
				this._table = value;
			}
		}

		[DefaultValue(false)]
		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Unique
		{
			get
			{
				return this._unique;
			}
			set
			{
				if (this._unique == value)
				{
					return;
				}
				this._unique = value;
				if (this._table == null)
				{
					return;
				}
				try
				{
					if (value)
					{
						if (this.Expression != null && this.Expression != string.Empty)
						{
							throw new ArgumentException("Cannot change Unique property for the expression column.");
						}
						this._table.Constraints.Add(null, this, false);
					}
					else
					{
						UniqueConstraint uniqueConstraintForColumnSet = UniqueConstraint.GetUniqueConstraintForColumnSet(this._table.Constraints, new DataColumn[] { this });
						this._table.Constraints.Remove(uniqueConstraintForColumnSet);
					}
				}
				catch (Exception ex)
				{
					this._unique = !value;
					throw ex;
				}
			}
		}

		internal DataContainer DataContainer
		{
			get
			{
				return this._dataContainer;
			}
		}

		internal static bool CanAutoIncrement(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode)
			{
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
				break;
			default:
				if (typeCode != TypeCode.Decimal)
				{
					return false;
				}
				break;
			}
			return true;
		}

		[MonoTODO]
		internal DataColumn Clone()
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn._allowDBNull = this._allowDBNull;
			dataColumn._autoIncrement = this._autoIncrement;
			dataColumn._autoIncrementSeed = this._autoIncrementSeed;
			dataColumn._autoIncrementStep = this._autoIncrementStep;
			dataColumn._caption = this._caption;
			dataColumn._columnMapping = this._columnMapping;
			dataColumn._columnName = this._columnName;
			dataColumn.DataType = this.DataType;
			dataColumn._defaultValue = this._defaultValue;
			dataColumn.Expression = this._expression;
			dataColumn._maxLength = this._maxLength;
			dataColumn._nameSpace = this._nameSpace;
			dataColumn._prefix = this._prefix;
			dataColumn._readOnly = this._readOnly;
			if (this.DataType == typeof(DateTime))
			{
				dataColumn.DateTimeMode = this._datetimeMode;
			}
			dataColumn._extendedProperties = this._extendedProperties;
			return dataColumn;
		}

		internal void SetUnique()
		{
			this._unique = true;
		}

		[MonoTODO]
		internal void AssertCanAddToCollection()
		{
		}

		[MonoTODO]
		protected internal void CheckNotAllowNull()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected void CheckUnique()
		{
			throw new NotImplementedException();
		}

		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this._eventHandlers[DataColumn._propertyChangedKey] as PropertyChangedEventHandler;
			if (propertyChangedEventHandler != null)
			{
				propertyChangedEventHandler(this, pcevent);
			}
		}

		protected internal void RaisePropertyChanging(string name)
		{
			PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
			this.OnPropertyChanging(e);
		}

		public override string ToString()
		{
			if (this._expression != string.Empty)
			{
				return this.ColumnName + " + " + this._expression;
			}
			return this.ColumnName;
		}

		internal void SetTable(DataTable table)
		{
			if (this._table != null)
			{
				throw new ArgumentException("The column already belongs to a different table");
			}
			this._table = table;
			if (this._unique)
			{
				UniqueConstraint uniqueConstraint = new UniqueConstraint(this);
				this._table.Constraints.Add(uniqueConstraint);
			}
			this.DataContainer.Capacity = this._table.RecordCache.CurrentCapacity;
			int defaultValuesRowIndex = this._table.DefaultValuesRowIndex;
			if (defaultValuesRowIndex != -1)
			{
				this.DataContainer[defaultValuesRowIndex] = this._defaultValue;
				this.DataContainer.FillValues(defaultValuesRowIndex);
			}
		}

		internal static bool AreColumnSetsTheSame(DataColumn[] columnSet, DataColumn[] compareSet)
		{
			if (columnSet == null && compareSet == null)
			{
				return true;
			}
			if (columnSet == null || compareSet == null)
			{
				return false;
			}
			if (columnSet.Length != compareSet.Length)
			{
				return false;
			}
			foreach (DataColumn dataColumn in columnSet)
			{
				bool flag = false;
				foreach (DataColumn dataColumn2 in compareSet)
				{
					if (dataColumn == dataColumn2)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		internal int CompareValues(int index1, int index2)
		{
			return this.DataContainer.CompareValues(index1, index2);
		}

		private DataRelation GetParentRelation()
		{
			if (this._table == null)
			{
				return null;
			}
			foreach (object obj in this._table.ParentRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Contains(this))
				{
					return dataRelation;
				}
			}
			return null;
		}

		private DataRelation GetChildRelation()
		{
			if (this._table == null)
			{
				return null;
			}
			foreach (object obj in this._table.ChildRelations)
			{
				DataRelation dataRelation = (DataRelation)obj;
				if (dataRelation.Contains(this))
				{
					return dataRelation;
				}
			}
			return null;
		}

		internal void ResetColumnInfo()
		{
			this._ordinal = -1;
			this._table = null;
			if (this._compiledExpression != null)
			{
				this._compiledExpression.ResetExpression();
			}
		}

		internal bool DataTypeMatches(DataColumn col)
		{
			return this.DataType == col.DataType && (this.DataType != typeof(DateTime) || this.DateTimeMode == col.DateTimeMode || (this.DateTimeMode != DataSetDateTime.Local && this.DateTimeMode != DataSetDateTime.Utc && col.DateTimeMode != DataSetDateTime.Local && col.DateTimeMode != DataSetDateTime.Utc));
		}

		internal static object GetDefaultValueForType(Type type)
		{
			if (type == null)
			{
				return DBNull.Value;
			}
			if (type.Namespace == "System.Data.SqlTypes" && type.Assembly == typeof(DataColumn).Assembly)
			{
				if (type == typeof(SqlBinary))
				{
					return SqlBinary.Null;
				}
				if (type == typeof(SqlBoolean))
				{
					return SqlBoolean.Null;
				}
				if (type == typeof(SqlByte))
				{
					return SqlByte.Null;
				}
				if (type == typeof(SqlBytes))
				{
					return SqlBytes.Null;
				}
				if (type == typeof(SqlChars))
				{
					return SqlChars.Null;
				}
				if (type == typeof(SqlDateTime))
				{
					return SqlDateTime.Null;
				}
				if (type == typeof(SqlDecimal))
				{
					return SqlDecimal.Null;
				}
				if (type == typeof(SqlDouble))
				{
					return SqlDouble.Null;
				}
				if (type == typeof(SqlGuid))
				{
					return SqlGuid.Null;
				}
				if (type == typeof(SqlInt16))
				{
					return SqlInt16.Null;
				}
				if (type == typeof(SqlInt32))
				{
					return SqlInt32.Null;
				}
				if (type == typeof(SqlInt64))
				{
					return SqlInt64.Null;
				}
				if (type == typeof(SqlMoney))
				{
					return SqlMoney.Null;
				}
				if (type == typeof(SqlSingle))
				{
					return SqlSingle.Null;
				}
				if (type == typeof(SqlString))
				{
					return SqlString.Null;
				}
				if (type == typeof(SqlXml))
				{
					return SqlXml.Null;
				}
			}
			return DBNull.Value;
		}

		private EventHandlerList _eventHandlers = new EventHandlerList();

		private static readonly object _propertyChangedKey = new object();

		private bool _allowDBNull = true;

		private bool _autoIncrement;

		private long _autoIncrementSeed;

		private long _autoIncrementStep = 1L;

		private long _nextAutoIncrementValue;

		private string _caption;

		private MappingType _columnMapping;

		private string _columnName = string.Empty;

		private object _defaultValue = DataColumn.GetDefaultValueForType(null);

		private string _expression;

		private IExpression _compiledExpression;

		private PropertyCollection _extendedProperties = new PropertyCollection();

		private int _maxLength = -1;

		private string _nameSpace;

		private int _ordinal = -1;

		private string _prefix = string.Empty;

		private bool _readOnly;

		private DataTable _table;

		private bool _unique;

		private DataContainer _dataContainer;

		private DataSetDateTime _datetimeMode = DataSetDateTime.UnspecifiedLocal;
	}
}
