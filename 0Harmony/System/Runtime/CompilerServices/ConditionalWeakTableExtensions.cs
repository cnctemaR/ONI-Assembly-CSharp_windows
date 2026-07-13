using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ConditionalWeakTableExtensions
	{
		[return: Nullable(new byte[] { 1, 0, 1, 1 })]
		public static IEnumerable<KeyValuePair<TKey, TValue>> AsEnumerable<TKey, [Nullable(2)] TValue>(this ConditionalWeakTable<TKey, TValue> self) where TKey : class where TValue : class
		{
			ThrowHelper.ThrowIfArgumentNull(self, "self", null);
			IEnumerable<KeyValuePair<TKey, TValue>> enumerable = self as IEnumerable<KeyValuePair<TKey, TValue>>;
			if (enumerable != null)
			{
				return enumerable;
			}
			ICWTEnumerable<KeyValuePair<TKey, TValue>> icwtenumerable = self as ICWTEnumerable<KeyValuePair<TKey, TValue>>;
			if (icwtenumerable != null)
			{
				return icwtenumerable.SelfEnumerable;
			}
			return new CWTEnumerable<TKey, TValue>(self);
		}

		[return: Nullable(new byte[] { 1, 0, 1, 1 })]
		public static IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator<TKey, [Nullable(2)] TValue>(this ConditionalWeakTable<TKey, TValue> self) where TKey : class where TValue : class
		{
			ThrowHelper.ThrowIfArgumentNull(self, "self", null);
			IEnumerable<KeyValuePair<TKey, TValue>> enumerable = self as IEnumerable<KeyValuePair<TKey, TValue>>;
			if (enumerable != null)
			{
				return enumerable.GetEnumerator();
			}
			ICWTEnumerable<KeyValuePair<TKey, TValue>> icwtenumerable = self as ICWTEnumerable<KeyValuePair<TKey, TValue>>;
			if (icwtenumerable != null)
			{
				return icwtenumerable.GetEnumerator();
			}
			ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.GetKeys get_Keys = ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.get_Keys;
			if (get_Keys != null)
			{
				return ConditionalWeakTableExtensions.<GetEnumerator>g__Enumerate|2_0<TKey, TValue>(self, get_Keys(self));
			}
			throw new PlatformNotSupportedException("Could not get Keys property of ConditionalWeakTable to enumerate it");
		}

		public static void Clear<TKey, [Nullable(2)] TValue>(this ConditionalWeakTable<TKey, TValue> self) where TKey : class where TValue : class
		{
			ThrowHelper.ThrowIfArgumentNull(self, "self", null);
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = self.GetEnumerator<TKey, TValue>())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
					self.Remove(keyValuePair.Key);
				}
			}
		}

		public static bool TryAdd<TKey, [Nullable(2)] TValue>(this ConditionalWeakTable<TKey, TValue> self, TKey key, TValue value) where TKey : class where TValue : class
		{
			ThrowHelper.ThrowIfArgumentNull(self, "self", null);
			bool didAdd = false;
			self.GetValue(key, delegate([Nullable(1)] TKey _)
			{
				didAdd = true;
				return value;
			});
			return didAdd;
		}

		[CompilerGenerated]
		[return: Nullable(new byte[] { 1, 0, 1, 0 })]
		internal static IEnumerator<KeyValuePair<TKey, TValue>> <GetEnumerator>g__Enumerate|2_0<TKey, TValue>([Nullable(new byte[] { 1, 1, 0 })] ConditionalWeakTable<TKey, TValue> cwt, IEnumerable<TKey> keys) where TKey : class where TValue : class
		{
			foreach (TKey tkey in keys)
			{
				TValue tvalue;
				if (cwt.TryGetValue(tkey, out tvalue))
				{
					yield return new KeyValuePair<TKey, TValue>(tkey, tvalue);
				}
			}
			IEnumerator<TKey> enumerator = null;
			yield break;
			yield break;
		}

		[NullableContext(0)]
		private static class CWTInfoHolder<[Nullable(1)] TKey, [Nullable(2)] TValue> where TKey : class where TValue : class
		{
			static CWTInfoHolder()
			{
				PropertyInfo property = typeof(ConditionalWeakTable<TKey, TValue>).GetProperty("Keys", BindingFlags.Instance | BindingFlags.NonPublic);
				ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.get_KeysMethod = ((property != null) ? property.GetGetMethod(true) : null);
				if (ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.get_KeysMethod != null)
				{
					ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.get_Keys = (ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.GetKeys)Delegate.CreateDelegate(typeof(ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.GetKeys), ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.get_KeysMethod);
				}
			}

			[Nullable(2)]
			private static readonly MethodInfo get_KeysMethod;

			[Nullable(new byte[] { 2, 0, 0 })]
			public static readonly ConditionalWeakTableExtensions.CWTInfoHolder<TKey, TValue>.GetKeys get_Keys;

			public delegate IEnumerable<TKey> GetKeys(ConditionalWeakTable<TKey, TValue> cwt);
		}
	}
}
