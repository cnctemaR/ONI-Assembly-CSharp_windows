using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public abstract class BaseChannelWithProperties : BaseChannelObjectWithProperties
	{
		public override IDictionary Properties
		{
			get
			{
				if (this.SinksWithProperties == null || this.SinksWithProperties.Properties == null)
				{
					return base.Properties;
				}
				IDictionary[] array = new IDictionary[]
				{
					base.Properties,
					this.SinksWithProperties.Properties
				};
				return new AggregateDictionary(array);
			}
		}

		protected IChannelSinkBase SinksWithProperties;
	}
}
