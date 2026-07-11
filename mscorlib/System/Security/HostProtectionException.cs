using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Security
{
	[ComVisible(true)]
	[MonoTODO("Not supported in the runtime")]
	[Serializable]
	public class HostProtectionException : SystemException
	{
		public HostProtectionException()
		{
		}

		public HostProtectionException(string message)
			: base(message)
		{
		}

		public HostProtectionException(string message, Exception e)
			: base(message, e)
		{
		}

		public HostProtectionException(string message, HostProtectionResource protectedResources, HostProtectionResource demandedResources)
			: base(message)
		{
			this._protected = protectedResources;
			this._demanded = demandedResources;
		}

		protected HostProtectionException(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		public HostProtectionResource DemandedResources
		{
			get
			{
				return this._demanded;
			}
		}

		public HostProtectionResource ProtectedResources
		{
			get
			{
				return this._protected;
			}
		}

		[MonoTODO]
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
		}

		[MonoTODO]
		public override string ToString()
		{
			return base.ToString();
		}

		private HostProtectionResource _protected;

		private HostProtectionResource _demanded;
	}
}
