using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[Serializable]
	public class LicenseException : SystemException
	{
		public LicenseException(Type type)
			: this(type, null)
		{
		}

		public LicenseException(Type type, object instance)
		{
			this.type = type;
		}

		public LicenseException(Type type, object instance, string message)
			: this(type, instance, message, null)
		{
		}

		public LicenseException(Type type, object instance, string message, Exception innerException)
			: base(message, innerException)
		{
			this.type = type;
		}

		protected LicenseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.type = (Type)info.GetValue("LicensedType", typeof(Type));
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("LicensedType", this.type);
			base.GetObjectData(info, context);
		}

		public Type LicensedType
		{
			get
			{
				return this.type;
			}
		}

		private Type type;
	}
}
