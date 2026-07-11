using System;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting
{
	internal class ClientIdentity : Identity
	{
		public ClientIdentity(string objectUri, ObjRef objRef)
			: base(objectUri)
		{
			this._objRef = objRef;
			IMessageSink messageSink;
			if (this._objRef.EnvoyInfo != null)
			{
				IMessageSink envoySinks = this._objRef.EnvoyInfo.EnvoySinks;
				messageSink = envoySinks;
			}
			else
			{
				messageSink = null;
			}
			this._envoySink = messageSink;
		}

		public MarshalByRefObject ClientProxy
		{
			get
			{
				return (MarshalByRefObject)this._proxyReference.Target;
			}
			set
			{
				this._proxyReference = new WeakReference(value);
			}
		}

		public override ObjRef CreateObjRef(Type requestedType)
		{
			return this._objRef;
		}

		public string TargetUri
		{
			get
			{
				return this._objRef.URI;
			}
		}

		private WeakReference _proxyReference;
	}
}
