using System;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionField : ReflectionWritableMember
	{
		public ReflectionField(FieldInfo field)
		{
			Assumes.NotNull<FieldInfo>(field);
			this._field = field;
		}

		public FieldInfo UndelyingField
		{
			get
			{
				return this._field;
			}
		}

		public override MemberInfo UnderlyingMember
		{
			get
			{
				return this.UndelyingField;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.UndelyingField.IsInitOnly;
			}
		}

		public override bool RequiresInstance
		{
			get
			{
				return !this.UndelyingField.IsStatic;
			}
		}

		public override Type ReturnType
		{
			get
			{
				return this.UndelyingField.FieldType;
			}
		}

		public override ReflectionItemType ItemType
		{
			get
			{
				return ReflectionItemType.Field;
			}
		}

		public override object GetValue(object instance)
		{
			return this.UndelyingField.SafeGetValue(instance);
		}

		public override void SetValue(object instance, object value)
		{
			this.UndelyingField.SafeSetValue(instance, value);
		}

		private readonly FieldInfo _field;
	}
}
