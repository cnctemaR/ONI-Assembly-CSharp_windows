using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	public sealed class SharedProperty
	{
		internal SharedProperty(ISharedProperty property)
		{
			this.property = property;
		}

		public object Value
		{
			get
			{
				return this.property.Value;
			}
			set
			{
				this.property.Value = value;
			}
		}

		internal SharedProperty()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ISharedProperty property;
	}
}
