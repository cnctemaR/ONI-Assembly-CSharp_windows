using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public sealed class CLSCompliantAttribute : Attribute
	{
		public CLSCompliantAttribute(bool isCompliant)
		{
			this.m_compliant = isCompliant;
		}

		public bool IsCompliant
		{
			get
			{
				return this.m_compliant;
			}
		}

		private bool m_compliant;
	}
}
