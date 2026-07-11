using System;
using System.Diagnostics;
using System.Reflection;

namespace UnityEngine.Events
{
	internal class InvokableCall : BaseInvokableCall
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event UnityAction Delegate;

		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate += (UnityAction)global::System.Delegate.CreateDelegate(typeof(UnityAction), target, theFunction);
		}

		public InvokableCall(UnityAction action)
		{
			this.Delegate += action;
		}

		public override void Invoke(object[] args)
		{
			bool flag = BaseInvokableCall.AllowInvoke(this.Delegate);
			if (flag)
			{
				this.Delegate();
			}
		}

		public void Invoke()
		{
			bool flag = BaseInvokableCall.AllowInvoke(this.Delegate);
			if (flag)
			{
				this.Delegate();
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.Method.Equals(method);
		}
	}
}
