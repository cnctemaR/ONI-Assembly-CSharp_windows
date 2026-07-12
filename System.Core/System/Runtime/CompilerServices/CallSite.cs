using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using Unity;

namespace System.Runtime.CompilerServices
{
	public class CallSite
	{
		internal CallSite(CallSiteBinder binder)
		{
			this._binder = binder;
		}

		public CallSiteBinder Binder
		{
			get
			{
				return this._binder;
			}
		}

		public static CallSite Create(Type delegateType, CallSiteBinder binder)
		{
			ContractUtils.RequiresNotNull(delegateType, "delegateType");
			ContractUtils.RequiresNotNull(binder, "binder");
			if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
			{
				throw Error.TypeMustBeDerivedFromSystemDelegate();
			}
			CacheDict<Type, Func<CallSiteBinder, CallSite>> cacheDict = CallSite.s_siteCtors;
			if (cacheDict == null)
			{
				cacheDict = (CallSite.s_siteCtors = new CacheDict<Type, Func<CallSiteBinder, CallSite>>(100));
			}
			Func<CallSiteBinder, CallSite> func;
			if (!cacheDict.TryGetValue(delegateType, out func))
			{
				MethodInfo method = typeof(CallSite<>).MakeGenericType(new Type[] { delegateType }).GetMethod("Create");
				if (delegateType.IsCollectible)
				{
					return (CallSite)method.Invoke(null, new object[] { binder });
				}
				func = (Func<CallSiteBinder, CallSite>)method.CreateDelegate(typeof(Func<CallSiteBinder, CallSite>));
				cacheDict.Add(delegateType, func);
			}
			return func(binder);
		}

		internal CallSite()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal const string CallSiteTargetMethodName = "CallSite.Target";

		private static volatile CacheDict<Type, Func<CallSiteBinder, CallSite>> s_siteCtors;

		internal readonly CallSiteBinder _binder;

		internal bool _match;
	}
}
