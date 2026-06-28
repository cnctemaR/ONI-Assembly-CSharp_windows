using System;
using System.Configuration;

namespace System.Net.Configuration
{
	public sealed class MailSettingsSectionGroup : ConfigurationSectionGroup
	{
		public SmtpSection Smtp
		{
			get
			{
				return (SmtpSection)base.Sections["smtp"];
			}
		}
	}
}
