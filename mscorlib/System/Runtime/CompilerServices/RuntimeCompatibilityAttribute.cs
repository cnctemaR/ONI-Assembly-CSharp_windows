using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	[Serializable]
	public sealed class RuntimeCompatibilityAttribute : Attribute
	{
		public bool WrapNonExceptionThrows
		{
			get
			{
				return this.m_wrapNonExceptionThrows;
			}
			set
			{
				this.m_wrapNonExceptionThrows = value;
			}
		}

		private bool m_wrapNonExceptionThrows;
	}
}
