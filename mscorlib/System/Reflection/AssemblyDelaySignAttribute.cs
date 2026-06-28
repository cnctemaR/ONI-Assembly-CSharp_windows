using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyDelaySignAttribute : Attribute
	{
		public AssemblyDelaySignAttribute(bool delaySign)
		{
			this.delay = delaySign;
		}

		public bool DelaySign
		{
			get
			{
				return this.delay;
			}
		}

		private bool delay;
	}
}
