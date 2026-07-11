using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class UnknownWrapper
	{
		public UnknownWrapper(object obj)
		{
			this.m_WrappedObject = obj;
		}

		public object WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private object m_WrappedObject;
	}
}
