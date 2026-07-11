using System;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SubjectIdentifier
	{
		internal SubjectIdentifier(SubjectIdentifierType type, object value)
		{
			this._type = type;
			this._value = value;
		}

		public SubjectIdentifierType Type
		{
			get
			{
				return this._type;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
		}

		internal SubjectIdentifier()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private SubjectIdentifierType _type;

		private object _value;
	}
}
