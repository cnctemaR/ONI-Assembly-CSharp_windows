using System;
using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization
{
	public sealed class InstanceDescriptor
	{
		public InstanceDescriptor(MemberInfo member, ICollection arguments)
			: this(member, arguments, true)
		{
		}

		public InstanceDescriptor(MemberInfo member, ICollection arguments, bool isComplete)
		{
			this.member = member;
			this.isComplete = isComplete;
			if (arguments == null)
			{
				this.arguments = new object[0];
			}
			else
			{
				object[] array = new object[arguments.Count];
				arguments.CopyTo(array, 0);
				this.arguments = array;
			}
			if (member is FieldInfo)
			{
				if (!((FieldInfo)member).IsStatic)
				{
					throw new ArgumentException(global::SR.GetString("Parameter must be static."));
				}
				if (this.arguments.Count != 0)
				{
					throw new ArgumentException(global::SR.GetString("Length mismatch."));
				}
			}
			else if (member is ConstructorInfo)
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)member;
				if (constructorInfo.IsStatic)
				{
					throw new ArgumentException(global::SR.GetString("Parameter cannot be static."));
				}
				if (this.arguments.Count != constructorInfo.GetParameters().Length)
				{
					throw new ArgumentException(global::SR.GetString("Length mismatch."));
				}
			}
			else if (member is MethodInfo)
			{
				MethodInfo methodInfo = (MethodInfo)member;
				if (!methodInfo.IsStatic)
				{
					throw new ArgumentException(global::SR.GetString("Parameter must be static."));
				}
				if (this.arguments.Count != methodInfo.GetParameters().Length)
				{
					throw new ArgumentException(global::SR.GetString("Length mismatch."));
				}
			}
			else if (member is PropertyInfo)
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				if (!propertyInfo.CanRead)
				{
					throw new ArgumentException(global::SR.GetString("Parameter must be readable."));
				}
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				if (getMethod != null && !getMethod.IsStatic)
				{
					throw new ArgumentException(global::SR.GetString("Parameter must be static."));
				}
			}
		}

		public ICollection Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public bool IsComplete
		{
			get
			{
				return this.isComplete;
			}
		}

		public MemberInfo MemberInfo
		{
			get
			{
				return this.member;
			}
		}

		public object Invoke()
		{
			object[] array = new object[this.arguments.Count];
			this.arguments.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is InstanceDescriptor)
				{
					array[i] = ((InstanceDescriptor)array[i]).Invoke();
				}
			}
			if (this.member is ConstructorInfo)
			{
				return ((ConstructorInfo)this.member).Invoke(array);
			}
			if (this.member is MethodInfo)
			{
				return ((MethodInfo)this.member).Invoke(null, array);
			}
			if (this.member is PropertyInfo)
			{
				return ((PropertyInfo)this.member).GetValue(null, array);
			}
			if (this.member is FieldInfo)
			{
				return ((FieldInfo)this.member).GetValue(null);
			}
			return null;
		}

		private MemberInfo member;

		private ICollection arguments;

		private bool isComplete;
	}
}
