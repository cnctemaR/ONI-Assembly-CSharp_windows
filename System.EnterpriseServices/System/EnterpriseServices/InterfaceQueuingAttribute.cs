using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	[ComVisible(false)]
	public sealed class InterfaceQueuingAttribute : Attribute
	{
		public InterfaceQueuingAttribute()
			: this(true)
		{
		}

		public InterfaceQueuingAttribute(bool enabled)
		{
			this.enabled = enabled;
			this.interfaceName = null;
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public string Interface
		{
			get
			{
				return this.interfaceName;
			}
			set
			{
				this.interfaceName = value;
			}
		}

		private bool enabled;

		private string interfaceName;
	}
}
