using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;

namespace System.Data
{
	[TypeConverter(typeof(ConstraintConverter))]
	[DefaultProperty("ConstraintName")]
	public abstract class Constraint
	{
		[DefaultValue("")]
		public virtual string ConstraintName
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (string.IsNullOrEmpty(value) && this.Table != null && this.InCollection)
				{
					throw ExceptionBuilder.NoConstraintName();
				}
				CultureInfo cultureInfo = ((this.Table != null) ? this.Table.Locale : CultureInfo.CurrentCulture);
				if (string.Compare(this._name, value, true, cultureInfo) != 0)
				{
					if (this.Table != null && this.InCollection)
					{
						this.Table.Constraints.RegisterName(value);
						if (this._name.Length != 0)
						{
							this.Table.Constraints.UnregisterName(this._name);
						}
					}
					this._name = value;
					return;
				}
				if (string.Compare(this._name, value, false, cultureInfo) != 0)
				{
					this._name = value;
				}
			}
		}

		internal string SchemaName
		{
			get
			{
				if (!string.IsNullOrEmpty(this._schemaName))
				{
					return this._schemaName;
				}
				return this.ConstraintName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._schemaName = value;
				}
			}
		}

		internal virtual bool InCollection
		{
			get
			{
				return this._inCollection;
			}
			set
			{
				this._inCollection = value;
				this._dataSet = (value ? this.Table.DataSet : null);
			}
		}

		public abstract DataTable Table { get; }

		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				PropertyCollection propertyCollection;
				if ((propertyCollection = this._extendedProperties) == null)
				{
					propertyCollection = (this._extendedProperties = new PropertyCollection());
				}
				return propertyCollection;
			}
		}

		internal abstract bool ContainsColumn(DataColumn column);

		internal abstract bool CanEnableConstraint();

		internal abstract Constraint Clone(DataSet destination);

		internal abstract Constraint Clone(DataSet destination, bool ignoreNSforTableLookup);

		internal void CheckConstraint()
		{
			if (!this.CanEnableConstraint())
			{
				throw ExceptionBuilder.ConstraintViolation(this.ConstraintName);
			}
		}

		internal abstract void CheckCanAddToCollection(ConstraintCollection constraint);

		internal abstract bool CanBeRemovedFromCollection(ConstraintCollection constraint, bool fThrowException);

		internal abstract void CheckConstraint(DataRow row, DataRowAction action);

		internal abstract void CheckState();

		protected void CheckStateForProperty()
		{
			try
			{
				this.CheckState();
			}
			catch (Exception ex) when (ADP.IsCatchableExceptionType(ex))
			{
				throw ExceptionBuilder.BadObjectPropertyAccess(ex.Message);
			}
		}

		[CLSCompliant(false)]
		protected virtual DataSet _DataSet
		{
			get
			{
				return this._dataSet;
			}
		}

		protected internal void SetDataSet(DataSet dataSet)
		{
			this._dataSet = dataSet;
		}

		internal abstract bool IsConstraintViolated();

		public override string ToString()
		{
			return this.ConstraintName;
		}

		private string _schemaName = string.Empty;

		private bool _inCollection;

		private DataSet _dataSet;

		internal string _name = string.Empty;

		internal PropertyCollection _extendedProperties;
	}
}
