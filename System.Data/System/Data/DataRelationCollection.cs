using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataRelationCollectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultEvent("CollectionChanged")]
	[DefaultProperty("Table")]
	public abstract class DataRelationCollection : InternalDataCollectionBase
	{
		protected DataRelationCollection()
		{
			this.inTransition = null;
		}

		[ResDescription("Occurs whenever this collection's membership changes.")]
		public event CollectionChangeEventHandler CollectionChanged;

		public abstract DataRelation this[string name] { get; }

		public abstract DataRelation this[int index] { get; }

		private string GetNextDefaultRelationName()
		{
			int num = 1;
			string text = "Relation" + num;
			while (this.Contains(text))
			{
				text = "Relation" + num;
				num++;
			}
			return text;
		}

		public void Add(DataRelation relation)
		{
			if (this.inTransition == relation)
			{
				return;
			}
			this.inTransition = relation;
			try
			{
				CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Add, this);
				this.OnCollectionChanging(e);
				this.AddCore(relation);
				if (relation.RelationName == string.Empty)
				{
					relation.RelationName = this.GenerateRelationName();
				}
				relation.ParentTable.ResetPropertyDescriptorsCache();
				relation.ChildTable.ResetPropertyDescriptorsCache();
				e = new CollectionChangeEventArgs(CollectionChangeAction.Add, this);
				this.OnCollectionChanged(e);
			}
			finally
			{
				this.inTransition = null;
			}
		}

		private string GenerateRelationName()
		{
			this.index++;
			return "Relation" + this.index;
		}

		public virtual DataRelation Add(DataColumn parentColumn, DataColumn childColumn)
		{
			DataRelation dataRelation = new DataRelation(this.GetNextDefaultRelationName(), parentColumn, childColumn);
			this.Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			DataRelation dataRelation = new DataRelation(this.GetNextDefaultRelationName(), parentColumns, childColumns);
			this.Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn parentColumn, DataColumn childColumn)
		{
			if (name == null || name == string.Empty)
			{
				name = this.GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumn, childColumn);
			this.Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			if (name == null || name == string.Empty)
			{
				name = this.GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumns, childColumns);
			this.Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn parentColumn, DataColumn childColumn, bool createConstraints)
		{
			if (name == null || name == string.Empty)
			{
				name = this.GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumn, childColumn, createConstraints);
			this.Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn[] parentColumns, DataColumn[] childColumns, bool createConstraints)
		{
			if (name == null || name == string.Empty)
			{
				name = this.GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumns, childColumns, createConstraints);
			this.Add(dataRelation);
			return dataRelation;
		}

		protected virtual void AddCore(DataRelation relation)
		{
			if (relation == null)
			{
				throw new ArgumentNullException();
			}
			if (this.List.IndexOf(relation) != -1)
			{
				throw new ArgumentException();
			}
			int num = this.IndexOf(relation.RelationName);
			if (num != -1 && relation.RelationName == this[num].RelationName)
			{
				throw new DuplicateNameException("A DataRelation named '" + relation.RelationName + "' already belongs to this DataSet.");
			}
			foreach (object obj in this)
			{
				DataRelation dataRelation = (DataRelation)obj;
				bool flag = false;
				foreach (DataColumn dataColumn in relation.ChildColumns)
				{
					bool flag2 = false;
					foreach (DataColumn dataColumn2 in dataRelation.ChildColumns)
					{
						if (dataColumn2 == dataColumn)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					flag = false;
					foreach (DataColumn dataColumn3 in relation.ParentColumns)
					{
						bool flag3 = false;
						foreach (DataColumn dataColumn4 in dataRelation.ParentColumns)
						{
							if (dataColumn4 == dataColumn3)
							{
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new ArgumentException("A relation already exists for these child columns");
					}
				}
			}
			this.List.Add(relation);
		}

		public virtual void AddRange(DataRelation[] relations)
		{
			if (relations == null)
			{
				return;
			}
			foreach (DataRelation dataRelation in relations)
			{
				this.Add(dataRelation);
			}
		}

		internal virtual void PostAddRange()
		{
		}

		public virtual bool CanRemove(DataRelation relation)
		{
			if (relation == null || !this.GetDataSet().Equals(relation.DataSet))
			{
				return false;
			}
			int num = this.IndexOf(relation.RelationName);
			return num != -1 && relation.RelationName == this[num].RelationName;
		}

		public virtual void Clear()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this.Remove(this[i]);
			}
			this.List.Clear();
		}

		public virtual bool Contains(string name)
		{
			DataSet dataSet = this.GetDataSet();
			if (dataSet != null)
			{
				DataRelation dataRelation = dataSet.Relations[name];
				if (dataRelation != null)
				{
					return true;
				}
			}
			return -1 != this.IndexOf(name, false);
		}

		private CollectionChangeEventArgs CreateCollectionChangeEvent(CollectionChangeAction action)
		{
			return new CollectionChangeEventArgs(action, this);
		}

		protected abstract DataSet GetDataSet();

		public virtual int IndexOf(DataRelation relation)
		{
			return this.List.IndexOf(relation);
		}

		public virtual int IndexOf(string relationName)
		{
			return this.IndexOf(relationName, false);
		}

		private int IndexOf(string name, bool error)
		{
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < this.List.Count; i++)
			{
				string relationName = ((DataRelation)this.List[i]).RelationName;
				if (string.Compare(name, relationName, true) == 0)
				{
					if (string.Compare(name, relationName, false) == 0)
					{
						return i;
					}
					num2 = i;
					num++;
				}
			}
			if (num == 1)
			{
				return num2;
			}
			if (num > 1 && error)
			{
				throw new ArgumentException("There is no match for the name in the same case and there are multiple matches in different case.");
			}
			return -1;
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		protected virtual void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
		}

		public void Remove(DataRelation relation)
		{
			if (this.inTransition == relation)
			{
				return;
			}
			this.inTransition = relation;
			if (relation == null)
			{
				return;
			}
			try
			{
				if (!this.List.Contains(relation))
				{
					throw new ArgumentException("Relation doesnot belong to this Collection.");
				}
				this.OnCollectionChanging(this.CreateCollectionChangeEvent(CollectionChangeAction.Remove));
				this.RemoveCore(relation);
				string text = "Relation" + this.index;
				if (relation.RelationName == text)
				{
					this.index--;
				}
				this.OnCollectionChanged(this.CreateCollectionChangeEvent(CollectionChangeAction.Remove));
			}
			finally
			{
				this.inTransition = null;
			}
		}

		public void Remove(string name)
		{
			DataRelation dataRelation = this[name];
			if (dataRelation == null)
			{
				throw new ArgumentException("Relation doesnot belong to this Collection.");
			}
			this.Remove(dataRelation);
		}

		public void RemoveAt(int index)
		{
			DataRelation dataRelation = this[index];
			if (dataRelation == null)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot find relation {0}", index));
			}
			this.Remove(dataRelation);
		}

		protected virtual void RemoveCore(DataRelation relation)
		{
			this.List.Remove(relation);
		}

		public void CopyTo(DataRelation[] array, int index)
		{
			this.CopyTo(array, index);
		}

		internal void BinarySerialize(SerializationInfo si)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this.Count; i++)
			{
				DataRelation dataRelation = (DataRelation)this.List[i];
				ArrayList arrayList2 = new ArrayList();
				arrayList2.Add(dataRelation.RelationName);
				int[] array = new int[2];
				DataTable dataTable = dataRelation.ParentTable;
				array[0] = dataTable.DataSet.Tables.IndexOf(dataTable);
				array[1] = dataTable.Columns.IndexOf(dataRelation.ParentColumns[0]);
				arrayList2.Add(array);
				array = new int[2];
				dataTable = dataRelation.ChildTable;
				array[0] = dataTable.DataSet.Tables.IndexOf(dataTable);
				array[1] = dataTable.Columns.IndexOf(dataRelation.ChildColumns[0]);
				arrayList2.Add(array);
				arrayList2.Add(false);
				arrayList2.Add(null);
				arrayList.Add(arrayList2);
			}
			si.AddValue("DataSet.Relations", arrayList, typeof(ArrayList));
		}

		private DataRelation inTransition;

		private int index;

		internal class DataSetRelationCollection : DataRelationCollection
		{
			internal DataSetRelationCollection(DataSet dataSet)
			{
				this.dataSet = dataSet;
			}

			protected override DataSet GetDataSet()
			{
				return this.dataSet;
			}

			protected override void AddCore(DataRelation relation)
			{
				if (relation.ChildTable.DataSet != this.dataSet || relation.ParentTable.DataSet != this.dataSet)
				{
					throw new DataException();
				}
				base.AddCore(relation);
				relation.ParentTable.ChildRelations.Add(relation);
				relation.ChildTable.ParentRelations.Add(relation);
				relation.SetDataSet(this.dataSet);
				relation.UpdateConstraints();
			}

			protected override void RemoveCore(DataRelation relation)
			{
				base.RemoveCore(relation);
				relation.SetDataSet(null);
				relation.ParentTable.ChildRelations.Remove(relation);
				relation.ChildTable.ParentRelations.Remove(relation);
				relation.SetParentKeyConstraint(null);
				relation.SetChildKeyConstraint(null);
			}

			public override void AddRange(DataRelation[] relations)
			{
				if (relations == null)
				{
					return;
				}
				if (this.dataSet != null && this.dataSet.InitInProgress)
				{
					this.mostRecentRelations = relations;
					return;
				}
				foreach (DataRelation dataRelation in relations)
				{
					if (dataRelation != null)
					{
						base.Add(dataRelation);
					}
				}
			}

			internal override void PostAddRange()
			{
				if (this.mostRecentRelations == null)
				{
					return;
				}
				foreach (DataRelation dataRelation in this.mostRecentRelations)
				{
					if (dataRelation != null)
					{
						if (dataRelation.InitInProgress)
						{
							dataRelation.FinishInit(this.dataSet);
						}
						base.Add(dataRelation);
					}
				}
				this.mostRecentRelations = null;
			}

			protected override ArrayList List
			{
				get
				{
					return base.List;
				}
			}

			public override DataRelation this[string name]
			{
				get
				{
					int num = base.IndexOf(name, true);
					return (num >= 0) ? ((DataRelation)this.List[num]) : null;
				}
			}

			public override DataRelation this[int index]
			{
				get
				{
					if (index < 0 || index >= this.List.Count)
					{
						throw new IndexOutOfRangeException(string.Format("Cannot find relation {0}.", index));
					}
					return (DataRelation)this.List[index];
				}
			}

			private DataSet dataSet;

			private DataRelation[] mostRecentRelations;
		}

		internal class DataTableRelationCollection : DataRelationCollection
		{
			internal DataTableRelationCollection(DataTable dataTable)
			{
				this.dataTable = dataTable;
			}

			protected override DataSet GetDataSet()
			{
				return this.dataTable.DataSet;
			}

			public override DataRelation this[string name]
			{
				get
				{
					int num = base.IndexOf(name, true);
					return (num >= 0) ? ((DataRelation)this.List[num]) : null;
				}
			}

			public override DataRelation this[int index]
			{
				get
				{
					if (index < 0 || index >= this.List.Count)
					{
						throw new IndexOutOfRangeException(string.Format("Cannot find relation {0}.", index));
					}
					return (DataRelation)this.List[index];
				}
			}

			protected override void AddCore(DataRelation relation)
			{
				if (this.dataTable.ParentRelations == this && relation.ChildTable != this.dataTable)
				{
					throw new ArgumentException("Cannot add a relation to this table's ParentRelations where this table is not the Child table.");
				}
				if (this.dataTable.ChildRelations == this && relation.ParentTable != this.dataTable)
				{
					throw new ArgumentException("Cannot add a relation to this table's ChildRelations where this table is not the Parent table.");
				}
				this.dataTable.DataSet.Relations.Add(relation);
				base.AddCore(relation);
			}

			protected override void RemoveCore(DataRelation relation)
			{
				relation.DataSet.Relations.Remove(relation);
				base.RemoveCore(relation);
			}

			protected override ArrayList List
			{
				get
				{
					return base.List;
				}
			}

			private DataTable dataTable;
		}
	}
}
