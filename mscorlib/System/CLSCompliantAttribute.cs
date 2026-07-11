using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.All)]
	[Serializable]
	public sealed class CLSCompliantAttribute : Attribute
	{
		public CLSCompliantAttribute(bool isCompliant)
		{
			this.is_compliant = isCompliant;
		}

		public bool IsCompliant
		{
			get
			{
				return this.is_compliant;
			}
		}

		private bool is_compliant;
	}
}
