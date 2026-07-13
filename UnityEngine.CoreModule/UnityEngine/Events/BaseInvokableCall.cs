using System;
using System.Reflection;

namespace UnityEngine.Events
{
	internal abstract class BaseInvokableCall
	{
		protected BaseInvokableCall()
		{
		}

		protected BaseInvokableCall(object target, MethodInfo function)
		{
			bool flag = function == null;
			if (flag)
			{
				throw new ArgumentNullException("function");
			}
			bool isStatic = function.IsStatic;
			if (isStatic)
			{
				bool flag2 = target != null;
				if (flag2)
				{
					throw new ArgumentException("target must be null");
				}
			}
			else
			{
				bool flag3 = target == null;
				if (flag3)
				{
					throw new ArgumentNullException("target");
				}
			}
		}

		public abstract void Invoke(object[] args);

		protected static void ThrowOnInvalidArg<T>(object arg)
		{
			bool flag = arg != null && !(arg is T);
			if (flag)
			{
				throw new ArgumentException(string.Format("Passed argument 'args[0]' is of the wrong type. Type:{0} Expected:{1}", arg.GetType(), typeof(T)));
			}
		}

		protected static bool AllowInvoke(Delegate @delegate)
		{
			object target = @delegate.Target;
			bool flag = target == null;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				Object @object = target as Object;
				bool flag3 = @object != null;
				flag2 = !flag3 || @object != null;
			}
			return flag2;
		}

		public abstract bool Find(object targetObj, MethodInfo method);
	}
}
