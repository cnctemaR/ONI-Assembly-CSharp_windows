using System;

namespace System.Runtime.Remoting
{
	internal class ClientIdentity : Identity
	{
		public ClientIdentity(string objectUri, ObjRef objRef)
			: base(objectUri)
		{
			this._objRef = objRef;
			this._envoySink = ((this._objRef.EnvoyInfo != null) ? this._objRef.EnvoyInfo.EnvoySinks : null);
		}

		public MarshalByRefObject ClientProxy
		{
			get
			{
				WeakReference proxyReference = this._proxyReference;
				return (MarshalByRefObject)((proxyReference != null) ? proxyReference.Target : null);
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
