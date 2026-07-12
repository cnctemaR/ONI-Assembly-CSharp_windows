using System;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionType : ReflectionMember
	{
		public ReflectionType(Type type)
		{
			Assumes.NotNull<Type>(type);
			this._type = type;
		}

		public override MemberInfo UnderlyingMember
		{
			get
			{
				return this._type;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool RequiresInstance
		{
			get
			{
				return true;
			}
		}

		public override Type ReturnType
		{
			get
			{
				return this._type;
			}
		}

		public override ReflectionItemType ItemType
		{
			get
			{
				return ReflectionItemType.Type;
			}
		}

		public override object GetValue(object instance)
		{
			return instance;
		}

		private Type _type;
	}
}
