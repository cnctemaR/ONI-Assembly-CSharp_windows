using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[DefaultEvent("CollectionChanged")]
	[Editor("Microsoft.VSDesigner.Data.Design.ConstraintsCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class ConstraintCollection : InternalDataCollectionBase
	{
		internal ConstraintCollection(DataTable table)
		{
			this.table = table;
		}

		public event CollectionChangeEventHandler CollectionChanged;

		internal DataTable Table
		{
			get
			{
				return this.table;
			}
		}

		public Constraint this[string name]
		{
			get
			{
				int num = this.IndexOf(name);
				return (num != -1) ? ((Constraint)this.List[num]) : null;
			}
		}

		public Constraint this[int index]
		{
			get
			{
				if (index < 0 || index >= this.List.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return (Constraint)this.List[index];
			}
		}

		private void _handleBeforeConstraintNameChange(object sender, string newName)
		{
			if (newName == null || newName == string.Empty)
			{
				throw new ArgumentException("ConstraintName cannot be set to null or empty after adding it to a ConstraintCollection.");
			}
			if (this._isDuplicateConstraintName(newName, (Constraint)sender))
			{
				throw new DuplicateNameException("Constraint name already exists.");
			}
		}

		private bool _isDuplicateConstraintName(string constraintName, Constraint excludeFromComparison)
		{
			foreach (object obj in this.List)
			{
				Constraint constraint = (Constraint)obj;
				if (constraint != excludeFromComparison)
				{
					if (string.Compare(constraintName, constraint.ConstraintName, false, this.Table.Locale) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private string _createNewConstraintName()
		{
			int num = 1;
			string text;
			for (;;)
			{
				text = "Constraint" + num;
				if (this.IndexOf(text) == -1)
				{
					break;
				}
				num++;
			}
			return text;
		}

		public void Add(Constraint constraint)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException("Can not add null.");
			}
			if (constraint.InitInProgress)
			{
				throw new ArgumentException("Hmm .. Failed to Add to collection");
			}
			if (this == constraint.ConstraintCollection)
			{
				throw new ArgumentException("Constraint already belongs to this collection.");
			}
			if (constraint.ConstraintCollection != null)
			{
				throw new ArgumentException("Constraint already belongs to another collection.");
			}
			foreach (object obj in this)
			{
				Constraint constraint2 = (Constraint)obj;
				if (constraint2.Equals(constraint))
				{
					throw new DataException("Constraint matches contraint named '" + constraint2.ConstraintName + "' already in collection");
				}
			}
			if (this._isDuplicateConstraintName(constraint.ConstraintName, null))
			{
				throw new DuplicateNameException("Constraint name already exists.");
			}
			constraint.AddToConstraintCollectionSetup(this);
			if (constraint.ConstraintName == null || constraint.ConstraintName == string.Empty)
			{
				constraint.ConstraintName = this._createNewConstraintName();
			}
			constraint.BeforeConstraintNameChange += this._handleBeforeConstraintNameChange;
			constraint.ConstraintCollection = this;
			this.List.Add(constraint);
			if (constraint is UniqueConstraint && ((UniqueConstraint)constraint).IsPrimaryKey)
			{
				this.table.PrimaryKey = ((UniqueConstraint)constraint).Columns;
			}
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, this));
		}

		public Constraint Add(string name, DataColumn column, bool primaryKey)
		{
			UniqueConstraint uniqueConstraint = new UniqueConstraint(name, column, primaryKey);
			this.Add(uniqueConstraint);
			return uniqueConstraint;
		}

		public Constraint Add(string name, DataColumn primaryKeyColumn, DataColumn foreignKeyColumn)
		{
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(name, primaryKeyColumn, foreignKeyColumn);
			this.Add(foreignKeyConstraint);
			return foreignKeyConstraint;
		}

		public Constraint Add(string name, DataColumn[] columns, bool primaryKey)
		{
			UniqueConstraint uniqueConstraint = new UniqueConstraint(name, columns, primaryKey);
			this.Add(uniqueConstraint);
			return uniqueConstraint;
		}

		public Constraint Add(string name, DataColumn[] primaryKeyColumns, DataColumn[] foreignKeyColumns)
		{
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(name, primaryKeyColumns, foreignKeyColumns);
			this.Add(foreignKeyConstraint);
			return foreignKeyConstraint;
		}

		public void AddRange(Constraint[] constraints)
		{
			if (this.Table.InitInProgress)
			{
				this._mostRecentConstraints = constraints;
				return;
			}
			if (constraints == null)
			{
				return;
			}
			for (int i = 0; i < constraints.Length; i++)
			{
				if (constraints[i] != null)
				{
					this.Add(constraints[i]);
				}
			}
		}

		internal void PostAddRange()
		{
			if (this._mostRecentConstraints == null)
			{
				return;
			}
			for (int i = 0; i < this._mostRecentConstraints.Length; i++)
			{
				Constraint constraint = this._mostRecentConstraints[i];
				if (constraint != null)
				{
					if (constraint.InitInProgress)
					{
						constraint.FinishInit(this.Table);
					}
					this.Add(constraint);
				}
			}
			this._mostRecentConstraints = null;
		}

		public bool CanRemove(Constraint constraint)
		{
			return constraint.CanRemoveFromCollection(this, false);
		}

		public void Clear()
		{
			this.Table.PrimaryKey = null;
			foreach (object obj in this.List)
			{
				Constraint constraint = (Constraint)obj;
				constraint.ConstraintCollection = null;
				constraint.BeforeConstraintNameChange -= this._handleBeforeConstraintNameChange;
			}
			this.List.Clear();
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
		}

		public bool Contains(string name)
		{
			return -1 != this.IndexOf(name);
		}

		public int IndexOf(Constraint constraint)
		{
			int num = 0;
			foreach (object obj in this)
			{
				Constraint constraint2 = (Constraint)obj;
				if (constraint2 == constraint)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public int IndexOf(string constraintName)
		{
			int num = 0;
			foreach (object obj in this.List)
			{
				Constraint constraint = (Constraint)obj;
				if (string.Compare(constraintName, constraint.ConstraintName, !this.Table.CaseSensitive, this.Table.Locale) == 0)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public void Remove(Constraint constraint)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException();
			}
			if (!constraint.CanRemoveFromCollection(this, true))
			{
				return;
			}
			constraint.RemoveFromConstraintCollectionCleanup(this);
			constraint.ConstraintCollection = null;
			this.List.Remove(constraint);
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, this));
		}

		public void Remove(string name)
		{
			int num = this.IndexOf(name);
			if (num == -1)
			{
				throw new ArgumentException("Constraint '" + name + "' does not belong to this DataTable.");
			}
			this.Remove(this[num]);
		}

		public void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		protected override ArrayList List
		{
			get
			{
				return base.List;
			}
		}

		internal void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		public void CopyTo(Constraint[] array, int index)
		{
			base.CopyTo(array, index);
		}

		private DataTable table;

		private Constraint[] _mostRecentConstraints;
	}
}
