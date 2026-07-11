using System;

namespace System.Runtime.InteropServices
{
	[Serializable]
	public sealed class VariantWrapper
	{
		public VariantWrapper(object obj)
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
