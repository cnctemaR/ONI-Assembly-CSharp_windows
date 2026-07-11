using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class MarshalByRefObject
	{
		internal Identity GetObjectIdentity(MarshalByRefObject obj, out bool IsClient)
		{
			IsClient = false;
			Identity identity;
			if (RemotingServices.IsTransparentProxy(obj))
			{
				identity = RemotingServices.GetRealProxy(obj).ObjectIdentity;
				IsClient = true;
			}
			else
			{
				identity = obj.ObjectIdentity;
			}
			return identity;
		}

		internal ServerIdentity ObjectIdentity
		{
			get
			{
				return this._identity;
			}
			set
			{
				this._identity = value;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
		public virtual ObjRef CreateObjRef(Type requestedType)
		{
			if (this._identity == null)
			{
				throw new RemotingException(Locale.GetText("No remoting information was found for the object."));
			}
			return this._identity.CreateObjRef(requestedType);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
		public object GetLifetimeService()
		{
			if (this._identity == null)
			{
				return null;
			}
			return this._identity.Lease;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
		public virtual object InitializeLifetimeService()
		{
			if (this._identity != null && this._identity.Lease != null)
			{
				return this._identity.Lease;
			}
			return new Lease();
		}

		protected MarshalByRefObject MemberwiseClone(bool cloneIdentity)
		{
			MarshalByRefObject marshalByRefObject = (MarshalByRefObject)base.MemberwiseClone();
			if (!cloneIdentity)
			{
				marshalByRefObject._identity = null;
			}
			return marshalByRefObject;
		}

		[NonSerialized]
		private ServerIdentity _identity;
	}
}
