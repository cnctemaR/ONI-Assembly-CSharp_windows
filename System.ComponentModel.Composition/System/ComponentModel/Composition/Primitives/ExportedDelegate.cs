using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	public class ExportedDelegate
	{
		protected ExportedDelegate()
		{
		}

		public ExportedDelegate(object instance, MethodInfo method)
		{
			Requires.NotNull<MethodInfo>(method, "method");
			this._instance = instance;
			this._method = method;
		}

		public virtual Delegate CreateDelegate(Type delegateType)
		{
			Requires.NotNull<Type>(delegateType, "delegateType");
			if (delegateType == typeof(Delegate) || delegateType == typeof(MulticastDelegate))
			{
				delegateType = this.CreateStandardDelegateType();
			}
			return Delegate.CreateDelegate(delegateType, this._instance, this._method, false);
		}

		private Type CreateStandardDelegateType()
		{
			ParameterInfo[] parameters = this._method.GetParameters();
			Type[] array = new Type[parameters.Length + 1];
			array[parameters.Length] = this._method.ReturnType;
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			return Expression.GetDelegateType(array);
		}

		private object _instance;

		private MethodInfo _method;
	}
}
