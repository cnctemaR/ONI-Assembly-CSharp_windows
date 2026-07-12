using System;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionProperty : ReflectionWritableMember
	{
		public ReflectionProperty(MethodInfo getMethod, MethodInfo setMethod)
		{
			Assumes.IsTrue(getMethod != null || setMethod != null);
			this._getMethod = getMethod;
			this._setMethod = setMethod;
		}

		public override MemberInfo UnderlyingMember
		{
			get
			{
				return this.UnderlyingGetMethod ?? this.UnderlyingSetMethod;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.UnderlyingGetMethod != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.UnderlyingSetMethod != null;
			}
		}

		public MethodInfo UnderlyingGetMethod
		{
			get
			{
				return this._getMethod;
			}
		}

		public MethodInfo UnderlyingSetMethod
		{
			get
			{
				return this._setMethod;
			}
		}

		public override string Name
		{
			get
			{
				string name = (this.UnderlyingGetMethod ?? this.UnderlyingSetMethod).Name;
				Assumes.IsTrue(name.Length > 4);
				return name.Substring(4);
			}
		}

		public override string GetDisplayName()
		{
			return ReflectionServices.GetDisplayName(base.DeclaringType, this.Name);
		}

		public override bool RequiresInstance
		{
			get
			{
				return !(this.UnderlyingGetMethod ?? this.UnderlyingSetMethod).IsStatic;
			}
		}

		public override Type ReturnType
		{
			get
			{
				if (this.UnderlyingGetMethod != null)
				{
					return this.UnderlyingGetMethod.ReturnType;
				}
				ParameterInfo[] parameters = this.UnderlyingSetMethod.GetParameters();
				Assumes.IsTrue(parameters.Length != 0);
				return parameters[parameters.Length - 1].ParameterType;
			}
		}

		public override ReflectionItemType ItemType
		{
			get
			{
				return ReflectionItemType.Property;
			}
		}

		public override object GetValue(object instance)
		{
			Assumes.NotNull<MethodInfo>(this._getMethod);
			return this.UnderlyingGetMethod.SafeInvoke(instance, Array.Empty<object>());
		}

		public override void SetValue(object instance, object value)
		{
			Assumes.NotNull<MethodInfo>(this._setMethod);
			this.UnderlyingSetMethod.SafeInvoke(instance, new object[] { value });
		}

		private readonly MethodInfo _getMethod;

		private readonly MethodInfo _setMethod;
	}
}
