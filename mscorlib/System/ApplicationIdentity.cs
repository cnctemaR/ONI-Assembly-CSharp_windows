using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(false)]
	[Serializable]
	public sealed class ApplicationIdentity : ISerializable
	{
		public ApplicationIdentity(string applicationIdentityFullName)
		{
			if (applicationIdentityFullName == null)
			{
				throw new ArgumentNullException("applicationIdentityFullName");
			}
			if (applicationIdentityFullName.IndexOf(", Culture=") == -1)
			{
				this._fullName = applicationIdentityFullName + ", Culture=neutral";
				return;
			}
			this._fullName = applicationIdentityFullName;
		}

		public string CodeBase
		{
			get
			{
				return this._codeBase;
			}
		}

		public string FullName
		{
			get
			{
				return this._fullName;
			}
		}

		public override string ToString()
		{
			return this._fullName;
		}

		[MonoTODO("Missing serialization")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
		}

		private string _fullName;

		private string _codeBase;
	}
}
