using System;
using System.Configuration;

namespace System.Diagnostics
{
	internal class AssertSection : ConfigurationElement
	{
		static AssertSection()
		{
			AssertSection._properties.Add(AssertSection._propAssertUIEnabled);
			AssertSection._properties.Add(AssertSection._propLogFile);
		}

		[ConfigurationProperty("assertuienabled", DefaultValue = true)]
		public bool AssertUIEnabled
		{
			get
			{
				return (bool)base[AssertSection._propAssertUIEnabled];
			}
		}

		[ConfigurationProperty("logfilename", DefaultValue = "")]
		public string LogFileName
		{
			get
			{
				return (string)base[AssertSection._propLogFile];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AssertSection._properties;
			}
		}

		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propAssertUIEnabled = new ConfigurationProperty("assertuienabled", typeof(bool), true, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propLogFile = new ConfigurationProperty("logfilename", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
	}
}
