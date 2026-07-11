using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace System.Runtime.CompilerServices
{
	public abstract class CallSiteBinder
	{
		public static LabelTarget UpdateLabel { get; } = Expression.Label("CallSiteBinder.UpdateLabel");

		public abstract Expression Bind(object[] args, ReadOnlyCollection<ParameterExpression> parameters, LabelTarget returnLabel);

		public virtual T BindDelegate<T>(CallSite<T> site, object[] args) where T : class
		{
			return default(T);
		}

		internal T BindCore<T>(CallSite<T> site, object[] args) where T : class
		{
			T t = this.BindDelegate<T>(site, args);
			if (t != null)
			{
				return t;
			}
			CallSiteBinder.LambdaSignature<T> instance = CallSiteBinder.LambdaSignature<T>.Instance;
			Expression expression = this.Bind(args, instance.Parameters, instance.ReturnLabel);
			if (expression == null)
			{
				throw Error.NoOrInvalidRuleProduced();
			}
			T t2 = CallSiteBinder.Stitch<T>(expression, instance).Compile();
			this.CacheTarget<T>(t2);
			return t2;
		}

		protected void CacheTarget<T>(T target) where T : class
		{
			this.GetRuleCache<T>().AddRule(target);
		}

		private static Expression<T> Stitch<T>(Expression binding, CallSiteBinder.LambdaSignature<T> signature) where T : class
		{
			Type typeFromHandle = typeof(CallSite<T>);
			ReadOnlyCollectionBuilder<Expression> readOnlyCollectionBuilder = new ReadOnlyCollectionBuilder<Expression>(3);
			readOnlyCollectionBuilder.Add(binding);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(CallSite), "$site");
			TrueReadOnlyCollection<ParameterExpression> trueReadOnlyCollection = signature.Parameters.AddFirst(parameterExpression);
			Expression expression = Expression.Label(CallSiteBinder.UpdateLabel);
			readOnlyCollectionBuilder.Add(expression);
			readOnlyCollectionBuilder.Add(Expression.Label(signature.ReturnLabel, Expression.Condition(Expression.Call(CachedReflectionInfo.CallSiteOps_SetNotMatched, parameterExpression), Expression.Default(signature.ReturnLabel.Type), Expression.Invoke(Expression.Property(Expression.Convert(parameterExpression, typeFromHandle), typeof(CallSite<T>).GetProperty("Update")), trueReadOnlyCollection))));
			return Expression.Lambda<T>(Expression.Block(readOnlyCollectionBuilder), "CallSite.Target", true, trueReadOnlyCollection);
		}

		internal RuleCache<T> GetRuleCache<T>() where T : class
		{
			if (this.Cache == null)
			{
				Interlocked.CompareExchange<Dictionary<Type, object>>(ref this.Cache, new Dictionary<Type, object>(), null);
			}
			Dictionary<Type, object> cache = this.Cache;
			Dictionary<Type, object> dictionary = cache;
			object obj;
			lock (dictionary)
			{
				if (!cache.TryGetValue(typeof(T), out obj))
				{
					obj = (cache[typeof(T)] = new RuleCache<T>());
				}
			}
			return obj as RuleCache<T>;
		}

		internal Dictionary<Type, object> Cache;

		private sealed class LambdaSignature<T> where T : class
		{
			internal static CallSiteBinder.LambdaSignature<T> Instance
			{
				get
				{
					if (CallSiteBinder.LambdaSignature<T>.s_instance == null)
					{
						CallSiteBinder.LambdaSignature<T>.s_instance = new CallSiteBinder.LambdaSignature<T>();
					}
					return CallSiteBinder.LambdaSignature<T>.s_instance;
				}
			}

			private LambdaSignature()
			{
				Type typeFromHandle = typeof(T);
				if (!typeFromHandle.IsSubclassOf(typeof(MulticastDelegate)))
				{
					throw Error.TypeParameterIsNotDelegate(typeFromHandle);
				}
				MethodInfo invokeMethod = typeFromHandle.GetInvokeMethod();
				ParameterInfo[] parametersCached = invokeMethod.GetParametersCached();
				if (parametersCached[0].ParameterType != typeof(CallSite))
				{
					throw Error.FirstArgumentMustBeCallSite();
				}
				ParameterExpression[] array = new ParameterExpression[parametersCached.Length - 1];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Expression.Parameter(parametersCached[i + 1].ParameterType, "$arg" + i);
				}
				this.Parameters = new TrueReadOnlyCollection<ParameterExpression>(array);
				this.ReturnLabel = Expression.Label(invokeMethod.GetReturnType());
			}

			private static CallSiteBinder.LambdaSignature<T> s_instance;

			internal readonly ReadOnlyCollection<ParameterExpression> Parameters;

			internal readonly LabelTarget ReturnLabel;
		}
	}
}
