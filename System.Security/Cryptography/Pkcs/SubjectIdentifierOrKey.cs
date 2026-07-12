using System;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SubjectIdentifierOrKey
	{
		internal SubjectIdentifierOrKey(SubjectIdentifierOrKeyType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}

		public SubjectIdentifierOrKeyType Type { get; }

		public object Value { get; }

		internal SubjectIdentifierOrKey()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
