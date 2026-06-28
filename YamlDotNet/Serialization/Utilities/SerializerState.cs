using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.Utilities
{
	public sealed class SerializerState : IDisposable
	{
		public T Get<T>() where T : class, new()
		{
			object obj;
			if (!this.items.TryGetValue(typeof(T), out obj))
			{
				obj = new T();
				this.items.Add(typeof(T), obj);
			}
			return (T)((object)obj);
		}

		public void OnDeserialization()
		{
			foreach (IPostDeserializationCallback postDeserializationCallback in this.items.Values.OfType<IPostDeserializationCallback>())
			{
				postDeserializationCallback.OnDeserialization();
			}
		}

		public void Dispose()
		{
			foreach (IDisposable disposable in this.items.Values.OfType<IDisposable>())
			{
				disposable.Dispose();
			}
		}

		private readonly IDictionary<Type, object> items = new Dictionary<Type, object>();
	}
}
