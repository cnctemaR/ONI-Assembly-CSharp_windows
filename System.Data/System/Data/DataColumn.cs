using System;
using System.ComponentModel;

namespace System.Data
{
	[DefaultProperty("ColumnName")]
	[DesignTimeVisible(false)]
	[Editor("Microsoft.VSDesigner.Data.Design.DataColumnEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ToolboxItem(false)]
	public class DataColumn : MarshalByValueComponent
	{
		public DataColumn()
		{
		}

		public DataColumn(string columnName)
		{
		}

		public DataColumn(string columnName, Type dataType)
		{
		}

		public DataColumn(string columnName, Type dataType, string expr)
		{
		}

		public DataColumn(string columnName, Type dataType, string expr, MappingType type)
		{
		}

		[DefaultValue(true)]
		public bool AllowDBNull
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool AutoIncrement
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(0)]
		public long AutoIncrementSeed
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(1)]
		public long AutoIncrementStep
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string Caption
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(MappingType.Element)]
		public virtual MappingType ColumnMapping
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public string ColumnName
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(typeof(string))]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.ColumnTypeConverter")]
		public Type DataType
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(DataSetDateTime.UnspecifiedLocal)]
		[RefreshProperties(RefreshProperties.All)]
		public DataSetDateTime DateTimeMode
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[TypeConverter("System.Data.DefaultValueTypeConverter")]
		public object DefaultValue
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public string Expression
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue(-1)]
		public int MaxLength
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public string Namespace
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Ordinal
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(false)]
		public bool ReadOnly
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTable Table
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Unique
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[MonoTODO]
		protected internal void CheckNotAllowNull()
		{
		}

		[MonoTODO]
		protected void CheckUnique()
		{
		}

		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
		}

		protected internal void RaisePropertyChanging(string name)
		{
		}

		public void SetOrdinal(int ordinal)
		{
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
