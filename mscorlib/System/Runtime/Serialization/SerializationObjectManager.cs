using System;
using System.Collections;

namespace System.Runtime.Serialization
{
	public sealed class SerializationObjectManager
	{
		public SerializationObjectManager(StreamingContext context)
		{
			this.context = context;
		}

		private event SerializationCallbacks.CallbackHandler callbacks;

		public void RegisterObject(object obj)
		{
			if (this.seen.Contains(obj))
			{
				return;
			}
			SerializationCallbacks sc = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
			this.seen[obj] = 1;
			sc.RaiseOnSerializing(obj, this.context);
			if (sc.HasSerializedCallbacks)
			{
				this.callbacks = (SerializationCallbacks.CallbackHandler)Delegate.Combine(this.callbacks, new SerializationCallbacks.CallbackHandler(delegate(StreamingContext ctx)
				{
					sc.RaiseOnSerialized(obj, ctx);
				}));
			}
		}

		public void RaiseOnSerializedEvent()
		{
			if (this.callbacks != null)
			{
				this.callbacks(this.context);
			}
		}

		private readonly StreamingContext context;

		private readonly Hashtable seen = new Hashtable();
	}
}
