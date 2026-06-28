using System;
using System.Reflection;

namespace UnityEngine.Events
{
	internal class CachedInvokableCall<T> : InvokableCall<T>
	{
		public CachedInvokableCall(Object target, MethodInfo theFunction, T argument)
			: base(target, theFunction)
		{
			this.m_Arg1[0] = argument;
		}

		public override void Invoke(object[] args)
		{
			base.Invoke(this.m_Arg1);
		}

		private readonly object[] m_Arg1 = new object[1];
	}
}
