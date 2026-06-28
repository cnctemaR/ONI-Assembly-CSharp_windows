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
			}
			else
			{
				this._fullName = applicationIdentityFullName;
			}
		}

		[MonoTODO("Missing serialization")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
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

		private string _fullName;

		private string _codeBase;
	}
}
