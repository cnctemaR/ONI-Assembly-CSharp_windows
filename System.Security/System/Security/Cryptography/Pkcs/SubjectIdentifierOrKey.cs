using System;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SubjectIdentifierOrKey
	{
		internal SubjectIdentifierOrKey(SubjectIdentifierOrKeyType type, object value)
		{
			this._type = type;
			this._value = value;
		}

		public SubjectIdentifierOrKeyType Type
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

		internal SubjectIdentifierOrKey()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private SubjectIdentifierOrKeyType _type;

		private object _value;
	}
}
