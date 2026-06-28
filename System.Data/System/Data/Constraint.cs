using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	[TypeConverter(typeof(ConstraintConverter))]
	[DefaultProperty("ConstraintName")]
	public abstract class Constraint
	{
		protected Constraint()
		{
			this.dataSet = null;
			this._properties = new PropertyCollection();
		}

		internal event DelegateConstraintNameChange BeforeConstraintNameChange
		{
			add
			{
				this.events.AddHandler(Constraint.beforeConstraintNameChange, value);
			}
			remove
			{
				this.events.RemoveHandler(Constraint.beforeConstraintNameChange, value);
			}
		}

		[CLSCompliant(false)]
		protected internal virtual DataSet _DataSet
		{
			get
			{
				return this.dataSet;
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public virtual string ConstraintName
		{
			get
			{
				return (this._constraintName != null) ? this._constraintName : string.Empty;
			}
			set
			{
				this._onConstraintNameChange(value);
				this._constraintName = value;
			}
		}

		[Browsable(false)]
		[DataCategory("Data")]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return this._properties;
			}
		}

		public abstract DataTable Table { get; }

		internal ConstraintCollection ConstraintCollection
		{
			get
			{
				return this._constraintCollection;
			}
			set
			{
				this._constraintCollection = value;
			}
		}

		private void _onConstraintNameChange(string newName)
		{
			DelegateConstraintNameChange delegateConstraintNameChange = this.events[Constraint.beforeConstraintNameChange] as DelegateConstraintNameChange;
			if (delegateConstraintNameChange != null)
			{
				delegateConstraintNameChange(this, newName);
			}
		}

		internal abstract void AddToConstraintCollectionSetup(ConstraintCollection collection);

		internal abstract bool IsConstraintViolated();

		internal static void ThrowConstraintException()
		{
			throw new ConstraintException("Failed to enable constraints. One or more rows contain values violating non-null, unique, or foreign-key constraints.");
		}

		internal virtual bool InitInProgress
		{
			get
			{
				return this.initInProgress;
			}
			set
			{
				this.initInProgress = value;
			}
		}

		internal virtual void FinishInit(DataTable table)
		{
		}

		internal void AssertConstraint()
		{
			if (!this.IsConstraintViolated())
			{
				return;
			}
			if (this.Table._duringDataLoad || (this.Table.DataSet != null && !this.Table.DataSet.EnforceConstraints))
			{
				return;
			}
			Constraint.ThrowConstraintException();
		}

		internal abstract void AssertConstraint(DataRow row);

		internal virtual void RollbackAssert(DataRow row)
		{
		}

		internal abstract void RemoveFromConstraintCollectionCleanup(ConstraintCollection collection);

		[MonoTODO]
		protected void CheckStateForProperty()
		{
			throw new NotImplementedException();
		}

		protected internal void SetDataSet(DataSet dataSet)
		{
			this.dataSet = dataSet;
		}

		internal void SetExtendedProperties(PropertyCollection properties)
		{
			this._properties = properties;
		}

		internal Index Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (this._index != null)
				{
					this._index.RemoveRef();
					this.Table.DropIndex(this._index);
				}
				this._index = value;
				if (this._index != null)
				{
					this._index.AddRef();
				}
			}
		}

		internal abstract bool IsColumnContained(DataColumn column);

		internal abstract bool CanRemoveFromCollection(ConstraintCollection col, bool shouldThrow);

		public override string ToString()
		{
			return (this._constraintName != null) ? this._constraintName : string.Empty;
		}

		private static readonly object beforeConstraintNameChange = new object();

		private EventHandlerList events = new EventHandlerList();

		private string _constraintName;

		private PropertyCollection _properties;

		private Index _index;

		private ConstraintCollection _constraintCollection;

		private DataSet dataSet;

		private bool initInProgress;
	}
}
