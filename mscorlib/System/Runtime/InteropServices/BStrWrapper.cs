using System;

namespace System.Runtime.InteropServices
{
	public sealed class BStrWrapper
	{
		public BStrWrapper(string value)
		{
			this.m_WrappedObject = value;
		}

		public BStrWrapper(object value)
		{
			this.m_WrappedObject = (string)value;
		}

		public string WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private string m_WrappedObject;
	}
}
