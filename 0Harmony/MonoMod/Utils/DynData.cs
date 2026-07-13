using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DynData<TTarget> : IDisposable where TTarget : class
	{
		[Nullable(new byte[] { 2, 1, 1, 2 })]
		[field: Nullable(new byte[] { 2, 1, 1, 2 })]
		public static event Action<DynData<TTarget>, TTarget> OnInitialize;

		[Nullable(new byte[] { 1, 1, 1, 1, 2 })]
		public Dictionary<string, Func<TTarget, object>> Getters
		{
			[return: Nullable(new byte[] { 1, 1, 1, 1, 2 })]
			get
			{
				return this._Data.Getters;
			}
		}

		[Nullable(new byte[] { 1, 1, 1, 1, 2 })]
		public Dictionary<string, Action<TTarget, object>> Setters
		{
			[return: Nullable(new byte[] { 1, 1, 1, 1, 2 })]
			get
			{
				return this._Data.Setters;
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

		static DynData()
		{
			FieldInfo[] fields = typeof(TTarget).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				string name = field.Name;
				DynData<TTarget>._SpecialGetters[name] = (TTarget obj) => field.GetValue(obj);
				DynData<TTarget>._SpecialSetters[name] = delegate(TTarget obj, [Nullable(2)] object value)
				{
					field.SetValue(obj, value);
				};
			}
			PropertyInfo[] properties = typeof(TTarget).GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				string name2 = propertyInfo.Name;
				MethodInfo get = propertyInfo.GetGetMethod(true);
				if (get != null)
				{
					DynData<TTarget>._SpecialGetters[name2] = (TTarget obj) => get.Invoke(obj, ArrayEx.Empty<object>());
				}
				MethodInfo set = propertyInfo.GetSetMethod(true);
				if (set != null)
				{
					DynData<TTarget>._SpecialSetters[name2] = delegate(TTarget obj, [Nullable(2)] object value)
					{
						set.Invoke(obj, new object[] { value });
					};
				}
			}
		}

		public bool IsAlive
		{
			get
			{
				return this.Weak == null || this.Weak.SafeGetIsAlive();
			}
		}

		public TTarget Target
		{
			get
			{
				WeakReference weak = this.Weak;
				return (TTarget)((object)((weak != null) ? weak.SafeGetTarget() : null));
			}
		}

		[Nullable(2)]
		public object this[string name]
		{
			[return: Nullable(2)]
			get
			{
				Func<TTarget, object> func;
				if (DynData<TTarget>._SpecialGetters.TryGetValue(name, out func) || this.Getters.TryGetValue(name, out func))
				{
					return func(this.Target);
				}
				object obj;
				if (this.Data.TryGetValue(name, out obj))
				{
					return obj;
				}
				return null;
			}
			[param: Nullable(2)]
			set
			{
				Action<TTarget, object> action;
				if (DynData<TTarget>._SpecialSetters.TryGetValue(name, out action) || this.Setters.TryGetValue(name, out action))
				{
					action(this.Target, value);
					return;
				}
				object obj;
				if (this._Data.Disposable.Contains(name) && (obj = this[name]) != null)
				{
					IDisposable disposable = obj as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				this.Data[name] = value;
			}
		}

		public DynData()
			: this(default(TTarget), false)
		{
		}

		[NullableContext(2)]
		public DynData(TTarget obj)
			: this(obj, true)
		{
		}

		[NullableContext(2)]
		public DynData(TTarget obj, bool keepAlive)
		{
			if (obj != null)
			{
				WeakReference weakReference = new WeakReference(obj);
				object obj2 = obj;
				DynData<TTarget>._Data_ data_;
				if (!DynData<TTarget>._DataMap.TryGetValue(obj2, out data_))
				{
					data_ = new DynData<TTarget>._Data_();
					DynData<TTarget>._DataMap.Add(obj2, data_);
				}
				this._Data = data_;
				this.Weak = weakReference;
				if (keepAlive)
				{
					this.KeepAlive = obj;
				}
			}
			else
			{
				this._Data = DynData<TTarget>._DataStatic;
			}
			Action<DynData<TTarget>, TTarget> onInitialize = DynData<TTarget>.OnInitialize;
			if (onInitialize == null)
			{
				return;
			}
			onInitialize(this, obj);
		}

		[NullableContext(2)]
		public T Get<T>([Nullable(1)] string name)
		{
			return (T)((object)this[name]);
		}

		public void Set<[Nullable(2)] T>(string name, T value)
		{
			this[name] = value;
		}

		public void RegisterProperty(string name, [Nullable(new byte[] { 1, 1, 2 })] Func<TTarget, object> getter, [Nullable(new byte[] { 1, 1, 2 })] Action<TTarget, object> setter)
		{
			this.Getters[name] = getter;
			this.Setters[name] = setter;
		}

		public void UnregisterProperty(string name)
		{
			this.Getters.Remove(name);
			this.Setters.Remove(name);
		}

		private void Dispose(bool disposing)
		{
			this.KeepAlive = default(TTarget);
			if (disposing)
			{
				this._Data.Dispose();
			}
		}

		~DynData()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[Nullable(new byte[] { 1, 0 })]
		private static readonly DynData<TTarget>._Data_ _DataStatic = new DynData<TTarget>._Data_();

		[Nullable(new byte[] { 1, 1, 1, 0 })]
		private static readonly ConditionalWeakTable<object, DynData<TTarget>._Data_> _DataMap = new ConditionalWeakTable<object, DynData<TTarget>._Data_>();

		[Nullable(new byte[] { 1, 1, 1, 1, 2 })]
		private static readonly Dictionary<string, Func<TTarget, object>> _SpecialGetters = new Dictionary<string, Func<TTarget, object>>();

		[Nullable(new byte[] { 1, 1, 1, 1, 2 })]
		private static readonly Dictionary<string, Action<TTarget, object>> _SpecialSetters = new Dictionary<string, Action<TTarget, object>>();

		[Nullable(2)]
		private readonly WeakReference Weak;

		[Nullable(2)]
		private TTarget KeepAlive;

		[Nullable(new byte[] { 1, 0 })]
		private readonly DynData<TTarget>._Data_ _Data;

		[NullableContext(0)]
		private class _Data_ : IDisposable
		{
			~_Data_()
			{
				this.Dispose();
			}

			public void Dispose()
			{
				Dictionary<string, object> data = this.Data;
				lock (data)
				{
					if (this.Data.Count == 0)
					{
						return;
					}
					foreach (string text in this.Disposable)
					{
						object obj;
						if (this.Data.TryGetValue(text, out obj))
						{
							IDisposable disposable = obj as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					this.Disposable.Clear();
					this.Data.Clear();
				}
				GC.SuppressFinalize(this);
			}

			[Nullable(new byte[] { 1, 1, 1, 1, 2 })]
			public readonly Dictionary<string, Func<TTarget, object>> Getters = new Dictionary<string, Func<TTarget, object>>();

			[Nullable(new byte[] { 1, 1, 1, 1, 2 })]
			public readonly Dictionary<string, Action<TTarget, object>> Setters = new Dictionary<string, Action<TTarget, object>>();

			[Nullable(new byte[] { 1, 1, 2 })]
			public readonly Dictionary<string, object> Data = new Dictionary<string, object>();

			[Nullable(1)]
			public readonly HashSet<string> Disposable = new HashSet<string>();
		}
	}
}
