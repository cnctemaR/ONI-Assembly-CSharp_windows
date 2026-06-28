using System;
using System.ComponentModel;

namespace System.Data
{
	internal class DataRelationPropertyDescriptor : PropertyDescriptor
	{
		internal DataRelationPropertyDescriptor(DataRelation relation)
			: base(relation.RelationName, null)
		{
			this._relation = relation;
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(DataRowView);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(IBindingList);
			}
		}

		public DataRelation Relation
		{
			get
			{
				return this._relation;
			}
		}

		public override bool CanResetValue(object obj)
		{
			return false;
		}

		public override bool Equals(object obj)
		{
			DataRelationPropertyDescriptor dataRelationPropertyDescriptor = obj as DataRelationPropertyDescriptor;
			return dataRelationPropertyDescriptor != null && this.Relation == dataRelationPropertyDescriptor.Relation;
		}

		public override int GetHashCode()
		{
			return this._relation.GetHashCode();
		}

		public override object GetValue(object obj)
		{
			DataRowView dataRowView = (DataRowView)obj;
			return dataRowView.CreateChildView(this.Relation);
		}

		public override void ResetValue(object obj)
		{
		}

		public override void SetValue(object obj, object val)
		{
		}

		public override bool ShouldSerializeValue(object obj)
		{
			return false;
		}

		private DataRelation _relation;
	}
}
