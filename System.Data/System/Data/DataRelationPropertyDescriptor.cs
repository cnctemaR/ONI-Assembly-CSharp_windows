using System;
using System.ComponentModel;

namespace System.Data
{
	internal sealed class DataRelationPropertyDescriptor : PropertyDescriptor
	{
		internal DataRelationPropertyDescriptor(DataRelation dataRelation)
			: base(dataRelation.RelationName, null)
		{
			this.Relation = dataRelation;
		}

		internal DataRelation Relation { get; }

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

		public override bool Equals(object other)
		{
			return other is DataRelationPropertyDescriptor && ((DataRelationPropertyDescriptor)other).Relation == this.Relation;
		}

		public override int GetHashCode()
		{
			return this.Relation.GetHashCode();
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return ((DataRowView)component).CreateChildView(this.Relation);
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
