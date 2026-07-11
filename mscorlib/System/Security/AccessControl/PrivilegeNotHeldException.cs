using System;
using System.Runtime.Serialization;

namespace System.Security.AccessControl
{
	[Serializable]
	public sealed class PrivilegeNotHeldException : UnauthorizedAccessException, ISerializable
	{
		public PrivilegeNotHeldException()
		{
		}

		public PrivilegeNotHeldException(string privilege)
			: base(privilege)
		{
		}

		public PrivilegeNotHeldException(string privilege, Exception inner)
			: base(privilege, inner)
		{
		}

		public string PrivilegeName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}
	}
}
