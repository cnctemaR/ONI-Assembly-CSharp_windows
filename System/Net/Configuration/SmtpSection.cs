using System;
using System.Configuration;
using System.Net.Mail;

namespace System.Net.Configuration
{
	public sealed class SmtpSection : ConfigurationSection
	{
		[ConfigurationProperty("deliveryMethod", DefaultValue = "Network")]
		public global::System.Net.Mail.SmtpDeliveryMethod DeliveryMethod
		{
			get
			{
				return (global::System.Net.Mail.SmtpDeliveryMethod)((int)base["deliveryMethod"]);
			}
			set
			{
				base["deliveryMethod"] = value;
			}
		}

		[ConfigurationProperty("from")]
		public string From
		{
			get
			{
				return (string)base["from"];
			}
			set
			{
				base["from"] = value;
			}
		}

		[ConfigurationProperty("network")]
		public SmtpNetworkElement Network
		{
			get
			{
				return (SmtpNetworkElement)base["network"];
			}
		}

		[ConfigurationProperty("specifiedPickupDirectory")]
		public SmtpSpecifiedPickupDirectoryElement SpecifiedPickupDirectory
		{
			get
			{
				return (SmtpSpecifiedPickupDirectoryElement)base["specifiedPickupDirectory"];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return base.Properties;
			}
		}
	}
}
