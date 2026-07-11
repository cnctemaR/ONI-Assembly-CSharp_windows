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
				return this.wrap_non_exception_throws;
			}
			set
			{
				this.wrap_non_exception_throws = value;
			}
		}

		private bool wrap_non_exception_throws;
	}
}
