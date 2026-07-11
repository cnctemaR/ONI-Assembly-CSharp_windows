using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class BStrWrapper
	{
		public BStrWrapper(string value)
		{
			this._value = value;
		}

		public string WrappedObject
		{
			get
			{
				return this._value;
			}
		}

		private string _value;
	}
}
