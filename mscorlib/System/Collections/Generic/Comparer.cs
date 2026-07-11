using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Collections.Generic
{
	[TypeDependency("System.Collections.Generic.ObjectComparer`1")]
	[Serializable]
	public abstract class Comparer<T> : IComparer, IComparer<T>
	{
		public static Comparer<T> Default
		{
			get
			{
				Comparer<T> comparer = Comparer<T>.defaultComparer;
				if (comparer == null)
				{
					comparer = Comparer<T>.CreateComparer();
					Comparer<T>.defaultComparer = comparer;
				}
				return comparer;
			}
		}

		public static Comparer<T> Create(Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			return new ComparisonComparer<T>(comparison);
		}

		[SecuritySafeCritical]
		private static Comparer<T> CreateComparer()
		{
			RuntimeType runtimeType = (RuntimeType)typeof(T);
			if (typeof(IComparable<T>).IsAssignableFrom(runtimeType))
			{
				return (Comparer<T>)RuntimeType.CreateInstanceForAnotherGenericParameter(typeof(GenericComparer<>), runtimeType);
			}
			if (runtimeType.IsGenericType && runtimeType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				RuntimeType runtimeType2 = (RuntimeType)runtimeType.GetGenericArguments()[0];
				if (typeof(IComparable<>).MakeGenericType(new Type[] { runtimeType2 }).IsAssignableFrom(runtimeType2))
				{
					return (Comparer<T>)RuntimeType.CreateInstanceForAnotherGenericParameter(typeof(NullableComparer<>), runtimeType2);
				}
			}
			return new ObjectComparer<T>();
		}

		public abstract int Compare(T x, T y);

		int IComparer.Compare(object x, object y)
		{
			if (x == null)
			{
				if (y != null)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (y == null)
				{
					return 1;
				}
				if (x is T && y is T)
				{
					return this.Compare((T)((object)x), (T)((object)y));
				}
				ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
				return 0;
			}
		}

		private static volatile Comparer<T> defaultComparer;
	}
}
