using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	public sealed class RuntimeWrappedException : Exception
	{
		private RuntimeWrappedException()
		{
		}

		public object WrappedException
		{
			get
			{
				return this.wrapped_exception;
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("WrappedException", this.wrapped_exception);
		}

		private object wrapped_exception;
	}
}
