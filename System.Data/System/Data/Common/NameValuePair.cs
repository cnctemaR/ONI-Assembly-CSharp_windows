using System;

namespace System.Data.Common
{
	[Serializable]
	internal sealed class NameValuePair
	{
		internal NameValuePair(string name, string value, int length)
		{
			this._name = name;
			this._value = value;
			this._length = length;
		}

		internal int Length
		{
			get
			{
				return this._length;
			}
		}

		internal string Name
		{
			get
			{
				return this._name;
			}
		}

		internal string Value
		{
			get
			{
				return this._value;
			}
		}

		internal NameValuePair Next
		{
			get
			{
				return this._next;
			}
			set
			{
				if (this._next != null || value == null)
				{
					throw ADP.InternalError(ADP.InternalErrorCode.NameValuePairNext);
				}
				this._next = value;
			}
		}

		private readonly string _name;

		private readonly string _value;

		private readonly int _length;

		private NameValuePair _next;
	}
}
