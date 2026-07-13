using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DynamicData : DynamicObject, IDisposable, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		[Nullable(new byte[] { 2, 1, 1, 2 })]
		[field: Nullable(new byte[] { 2, 1, 1, 2 })]
		public static event Action<DynamicData, Type, object> OnInitialize;

		[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
		public Dictionary<string, Func<object, object>> Getters
		{
			[return: Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			get
			{
				return this._Data.Getters;
			}
		}

		[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
		public Dictionary<string, Action<object, object>> Setters
		{
			[return: Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			get
			{
				return this._Data.Setters;
			}
		}

		[Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
		public Dictionary<string, Func<object, object[], object>> Methods
		{
			[return: Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
			get
			{
				return this._Data.Methods;
			}
		}

		[Nullable(new byte[] { 1, 1, 2 })]
		public Dictionary<string, object> Data
		{
			[return: Nullable(new byte[] { 1, 1, 2 })]
			get
			{
				return this._Data.Data;
			}
		}

		public bool IsAlive
		{
			get
			{
				return this.Weak == null || this.Weak.SafeGetIsAlive();
			}
		}

		[Nullable(2)]
		public object Target
		{
			[NullableContext(2)]
			get
			{
				WeakReference weak = this.Weak;
				if (weak == null)
				{
					return null;
				}
				return weak.SafeGetTarget();
			}
		}

		public Type TargetType { get; private set; }

		public DynamicData(Type type)
			: this(type, null, false)
		{
		}

		public DynamicData(object obj)
			: this(Helpers.ThrowIfNull<object>(obj, "obj").GetType(), obj, true)
		{
		}

		public DynamicData(Type type, [Nullable(2)] object obj)
			: this(type, obj, true)
		{
		}

		public DynamicData(Type type, [Nullable(2)] object obj, bool keepAlive)
		{
			this.TargetType = type;
			Dictionary<Type, DynamicData._Cache_> cacheMap = DynamicData._CacheMap;
			lock (cacheMap)
			{
				DynamicData._Cache_ cache_;
				if (!DynamicData._CacheMap.TryGetValue(type, out cache_))
				{
					cache_ = new DynamicData._Cache_(type);
					DynamicData._CacheMap.Add(type, cache_);
				}
				this._Cache = cache_;
			}
			if (obj != null)
			{
				ConditionalWeakTable<object, DynamicData._Data_> dataMap = DynamicData._DataMap;
				lock (dataMap)
				{
					DynamicData._Data_ data_;
					if (!DynamicData._DataMap.TryGetValue(obj, out data_))
					{
						data_ = new DynamicData._Data_(type);
						DynamicData._DataMap.Add(obj, data_);
					}
					this._Data = data_;
				}
				this.Weak = new WeakReference(obj);
				if (keepAlive)
				{
					this.KeepAlive = obj;
				}
			}
			else
			{
				Dictionary<Type, DynamicData._Data_> dataStaticMap = DynamicData._DataStaticMap;
				lock (dataStaticMap)
				{
					DynamicData._Data_ data_2;
					if (!DynamicData._DataStaticMap.TryGetValue(type, out data_2))
					{
						data_2 = new DynamicData._Data_(type);
						DynamicData._DataStaticMap.Add(type, data_2);
					}
					this._Data = data_2;
				}
			}
			Action<DynamicData, Type, object> onInitialize = DynamicData.OnInitialize;
			if (onInitialize == null)
			{
				return;
			}
			onInitialize(this, type, obj);
		}

		public static DynamicData For(object obj)
		{
			ConditionalWeakTable<object, DynamicData> dynamicDataMap = DynamicData._DynamicDataMap;
			DynamicData dynamicData2;
			lock (dynamicDataMap)
			{
				DynamicData dynamicData;
				if (!DynamicData._DynamicDataMap.TryGetValue(obj, out dynamicData))
				{
					dynamicData = new DynamicData(obj);
					DynamicData._DynamicDataMap.Add(obj, dynamicData);
				}
				dynamicData2 = dynamicData;
			}
			return dynamicData2;
		}

		[return: Nullable(new byte[] { 1, 1, 2 })]
		public static Func<object, T> New<T>(params object[] args)
		{
			T target = (T)((object)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null));
			return (object other) => DynamicData.Set<T>(target, other);
		}

		[return: Nullable(new byte[] { 1, 1, 2 })]
		public static Func<object, object> New(Type type, params object[] args)
		{
			object target = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
			return (object other) => DynamicData.Set(target, other);
		}

		[return: Dynamic(new bool[] { false, false, true })]
		public static Func<object, dynamic> NewWrap<[Nullable(2)] T>(params object[] args)
		{
			T target = (T)((object)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null));
			return (object other) => DynamicData.Wrap(target, other);
		}

		[return: Dynamic(new bool[] { false, false, true })]
		public static Func<object, dynamic> NewWrap(Type type, params object[] args)
		{
			object target = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
			return (object other) => DynamicData.Wrap(target, other);
		}

		[return: Dynamic]
		public static dynamic Wrap(object target, [Nullable(2)] object other = null)
		{
			DynamicData dynamicData = new DynamicData(target);
			dynamicData.CopyFrom(other);
			return dynamicData;
		}

		[return: Nullable(2)]
		public static T Set<T>(T target, [Nullable(2)] object other = null)
		{
			return (T)((object)DynamicData.Set(target, other));
		}

		[NullableContext(2)]
		public static object Set([Nullable(1)] object target, object other = null)
		{
			object target2;
			using (DynamicData dynamicData = new DynamicData(target))
			{
				dynamicData.CopyFrom(other);
				target2 = dynamicData.Target;
			}
			return target2;
		}

		public void RegisterProperty(string name, [Nullable(new byte[] { 1, 2, 2 })] Func<object, object> getter, [Nullable(new byte[] { 1, 2, 2 })] Action<object, object> setter)
		{
			this.Getters[name] = getter;
			this.Setters[name] = setter;
		}

		public void UnregisterProperty(string name)
		{
			this.Getters.Remove(name);
			this.Setters.Remove(name);
		}

		public void RegisterMethod(string name, [Nullable(new byte[] { 1, 2, 2, 2, 2 })] Func<object, object[], object> cb)
		{
			this.Methods[name] = cb;
		}

		public void UnregisterMethod(string name)
		{
			this.Methods.Remove(name);
		}

		[NullableContext(2)]
		public void CopyFrom(object other)
		{
			if (other == null)
			{
				return;
			}
			foreach (PropertyInfo propertyInfo in other.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				this.Set(propertyInfo.Name, propertyInfo.GetValue(other, null));
			}
		}

		[return: Nullable(2)]
		public object Get(string name)
		{
			object obj;
			this.TryGet(name, out obj);
			return obj;
		}

		public bool TryGet(string name, [Nullable(2)] out object value)
		{
			object target = this.Target;
			Func<object, object> func;
			if (this._Data.Getters.TryGetValue(name, out func))
			{
				value = func(target);
				return true;
			}
			if (this._Cache.Getters.TryGetValue(name, out func))
			{
				value = func(target);
				return true;
			}
			return this._Data.Data.TryGetValue(name, out value);
		}

		[NullableContext(2)]
		public T Get<T>([Nullable(1)] string name)
		{
			return (T)((object)this.Get(name));
		}

		public bool TryGet<[Nullable(2)] T>(string name, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T value)
		{
			object obj;
			bool flag = this.TryGet(name, out obj);
			value = (T)((object)obj);
			return flag;
		}

		public void Set(string name, [Nullable(2)] object value)
		{
			object target = this.Target;
			Action<object, object> action;
			if (this._Data.Setters.TryGetValue(name, out action))
			{
				action(target, value);
				return;
			}
			if (this._Cache.Setters.TryGetValue(name, out action))
			{
				action(target, value);
				return;
			}
			this.Data[name] = value;
		}

		public void Add([Nullable(new byte[] { 0, 1, 1 })] KeyValuePair<string, object> kvp)
		{
			this.Set(kvp.Key, kvp.Value);
		}

		public void Add(string key, object value)
		{
			this.Set(key, value);
		}

		[return: Nullable(2)]
		public object Invoke(string name, params object[] args)
		{
			object obj;
			this.TryInvoke(name, args, out obj);
			return obj;
		}

		[NullableContext(2)]
		public bool TryInvoke([Nullable(1)] string name, object[] args, out object result)
		{
			Func<object, object[], object> func;
			if (this._Data.Methods.TryGetValue(name, out func))
			{
				result = func(this.Target, args);
				return true;
			}
			if (this._Cache.Methods.TryGetValue(name, out func))
			{
				result = func(this.Target, args);
				return true;
			}
			result = null;
			return false;
		}

		[return: Nullable(2)]
		public T Invoke<[Nullable(2)] T>(string name, params object[] args)
		{
			return (T)((object)this.Invoke(name, args));
		}

		public bool TryInvoke<[Nullable(2)] T>(string name, object[] args, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T result)
		{
			object obj;
			bool flag = this.TryInvoke(name, args, out obj);
			result = (T)((object)obj);
			return flag;
		}

		private void Dispose(bool disposing)
		{
			this.KeepAlive = null;
		}

		~DynamicData()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return this._Data.Data.Keys.Union<string>(this._Data.Getters.Keys).Union<string>(this._Data.Setters.Keys).Union<string>(this._Data.Methods.Keys)
				.Union<string>(this._Cache.Getters.Keys)
				.Union<string>(this._Cache.Setters.Keys)
				.Union<string>(this._Cache.Methods.Keys);
		}

		public override bool TryConvert(ConvertBinder binder, [Nullable(2)] out object result)
		{
			Helpers.ThrowIfArgumentNull<ConvertBinder>(binder, "binder");
			if (this.TargetType.IsCompatible(binder.Type) || this.TargetType.IsCompatible(binder.ReturnType) || binder.Type == typeof(object) || binder.ReturnType == typeof(object))
			{
				result = this.Target;
				return true;
			}
			if (typeof(DynamicData).IsCompatible(binder.Type) || typeof(DynamicData).IsCompatible(binder.ReturnType))
			{
				result = this;
				return true;
			}
			result = null;
			return false;
		}

		public override bool TryGetMember(GetMemberBinder binder, [Nullable(2)] out object result)
		{
			Helpers.ThrowIfArgumentNull<GetMemberBinder>(binder, "binder");
			if (this.Methods.ContainsKey(binder.Name))
			{
				result = null;
				return false;
			}
			result = this.Get(binder.Name);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, [Nullable(2)] object value)
		{
			Helpers.ThrowIfArgumentNull<SetMemberBinder>(binder, "binder");
			this.Set(binder.Name, value);
			return true;
		}

		[NullableContext(2)]
		public override bool TryInvokeMember([Nullable(1)] InvokeMemberBinder binder, object[] args, out object result)
		{
			Helpers.ThrowIfArgumentNull<InvokeMemberBinder>(binder, "binder");
			return this.TryInvoke(binder.Name, args, out result);
		}

		[return: Nullable(new byte[] { 1, 0, 1, 2 })]
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			DynamicData.<GetEnumerator>d__66 <GetEnumerator>d__ = new DynamicData.<GetEnumerator>d__66(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[Nullable(new byte[] { 1, 2 })]
		private static readonly object[] _NoArgs = ArrayEx.Empty<object>();

		private static readonly Dictionary<Type, DynamicData._Cache_> _CacheMap = new Dictionary<Type, DynamicData._Cache_>();

		private static readonly Dictionary<Type, DynamicData._Data_> _DataStaticMap = new Dictionary<Type, DynamicData._Data_>();

		private static readonly ConditionalWeakTable<object, DynamicData._Data_> _DataMap = new ConditionalWeakTable<object, DynamicData._Data_>();

		private static readonly ConditionalWeakTable<object, DynamicData> _DynamicDataMap = new ConditionalWeakTable<object, DynamicData>();

		[Nullable(2)]
		private readonly WeakReference Weak;

		[Nullable(2)]
		private object KeepAlive;

		private readonly DynamicData._Cache_ _Cache;

		private readonly DynamicData._Data_ _Data;

		[NullableContext(0)]
		private class _Cache_
		{
			[NullableContext(2)]
			public _Cache_(Type targetType)
			{
				bool flag = true;
				while (targetType != null && targetType != targetType.BaseType)
				{
					foreach (FieldInfo fieldInfo in targetType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						string name = fieldInfo.Name;
						if (!this.Getters.ContainsKey(name) && !this.Setters.ContainsKey(name))
						{
							try
							{
								FastReflectionHelper.FastInvoker fastInvoker2 = fieldInfo.GetFastInvoker();
								this.Getters[name] = (object obj) => fastInvoker2(obj, new object[0]);
								this.Setters[name] = delegate(object obj, object value)
								{
									fastInvoker2(obj, new object[] { value });
								};
							}
							catch
							{
								this.Getters[name] = new Func<object, object>(fieldInfo.GetValue);
								this.Setters[name] = new Action<object, object>(fieldInfo.SetValue);
							}
						}
					}
					PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					for (int i = 0; i < properties.Length; i++)
					{
						PropertyInfo propertyInfo = properties[i];
						string name2 = propertyInfo.Name;
						MethodInfo get = propertyInfo.GetGetMethod(true);
						if (get != null && !this.Getters.ContainsKey(name2))
						{
							try
							{
								FastReflectionHelper.FastInvoker fastInvoker3 = get.GetFastInvoker();
								this.Getters[name2] = (object obj) => fastInvoker3(obj, new object[0]);
							}
							catch
							{
								this.Getters[name2] = (object obj) => get.Invoke(obj, DynamicData._NoArgs);
							}
						}
						MethodInfo set = propertyInfo.GetSetMethod(true);
						if (set != null && !this.Setters.ContainsKey(name2))
						{
							try
							{
								FastReflectionHelper.FastInvoker fastInvoker = set.GetFastInvoker();
								this.Setters[name2] = delegate(object obj, object value)
								{
									fastInvoker(obj, new object[] { value });
								};
							}
							catch
							{
								this.Setters[name2] = delegate(object obj, object value)
								{
									set.Invoke(obj, new object[] { value });
								};
							}
						}
					}
					Dictionary<string, MethodInfo> dictionary = new Dictionary<string, MethodInfo>();
					foreach (MethodInfo methodInfo in targetType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						string name3 = methodInfo.Name;
						if (flag || !this.Methods.ContainsKey(name3))
						{
							if (dictionary.ContainsKey(name3))
							{
								dictionary[name3] = null;
							}
							else
							{
								dictionary[name3] = methodInfo;
							}
						}
					}
					foreach (KeyValuePair<string, MethodInfo> keyValuePair in dictionary)
					{
						if (!(keyValuePair.Value == null) && !keyValuePair.Value.IsGenericMethod)
						{
							try
							{
								FastReflectionHelper.FastInvoker cb = keyValuePair.Value.GetFastInvoker();
								this.Methods[keyValuePair.Key] = (object target, object[] args) => cb(target, args);
							}
							catch
							{
								this.Methods[keyValuePair.Key] = new Func<object, object[], object>(keyValuePair.Value.Invoke);
							}
						}
					}
					flag = false;
					targetType = targetType.BaseType;
				}
			}

			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Func<object, object>> Getters = new Dictionary<string, Func<object, object>>();

			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Action<object, object>> Setters = new Dictionary<string, Action<object, object>>();

			[Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
			public readonly Dictionary<string, Func<object, object[], object>> Methods = new Dictionary<string, Func<object, object[], object>>();
		}

		[NullableContext(0)]
		private class _Data_
		{
			[NullableContext(1)]
			public _Data_(Type type)
			{
				type == null;
			}

			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Func<object, object>> Getters = new Dictionary<string, Func<object, object>>();

			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Action<object, object>> Setters = new Dictionary<string, Action<object, object>>();

			[Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
			public readonly Dictionary<string, Func<object, object[], object>> Methods = new Dictionary<string, Func<object, object[], object>>();

			[Nullable(new byte[] { 1, 1, 2 })]
			public readonly Dictionary<string, object> Data = new Dictionary<string, object>();
		}
	}
}
