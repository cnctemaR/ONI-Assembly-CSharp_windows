using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class RemotingSurrogateSelector : ISurrogateSelector
	{
		public MessageSurrogateFilter Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this._filter = value;
			}
		}

		[SecurityCritical]
		public virtual void ChainSelector(ISurrogateSelector selector)
		{
			if (this._next != null)
			{
				selector.ChainSelector(this._next);
			}
			this._next = selector;
		}

		[SecurityCritical]
		public virtual ISurrogateSelector GetNextSelector()
		{
			return this._next;
		}

		public object GetRootObject()
		{
			return this._rootObj;
		}

		[SecurityCritical]
		public virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector ssout)
		{
			if (type.IsMarshalByRef)
			{
				ssout = this;
				return RemotingSurrogateSelector._objRemotingSurrogate;
			}
			if (RemotingSurrogateSelector.s_cachedTypeObjRef.IsAssignableFrom(type))
			{
				ssout = this;
				return RemotingSurrogateSelector._objRefSurrogate;
			}
			if (this._next != null)
			{
				return this._next.GetSurrogate(type, context, out ssout);
			}
			ssout = null;
			return null;
		}

		public void SetRootObject(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			this._rootObj = obj;
		}

		[MonoTODO]
		public virtual void UseSoapFormat()
		{
			throw new NotImplementedException();
		}

		private static Type s_cachedTypeObjRef = typeof(ObjRef);

		private static ObjRefSurrogate _objRefSurrogate = new ObjRefSurrogate();

		private static RemotingSurrogate _objRemotingSurrogate = new RemotingSurrogate();

		private object _rootObj;

		private MessageSurrogateFilter _filter;

		private ISurrogateSelector _next;
	}
}
