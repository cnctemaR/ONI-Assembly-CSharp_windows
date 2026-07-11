using System;
using System.ComponentModel;
using System.Configuration;

namespace System.Xml.XmlConfiguration
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XsltConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("prohibitDefaultResolver", DefaultValue = "false")]
		public string ProhibitDefaultResolverString
		{
			get
			{
				return (string)base["prohibitDefaultResolver"];
			}
			set
			{
				base["prohibitDefaultResolver"] = value;
			}
		}

		private bool _ProhibitDefaultResolver
		{
			get
			{
				bool flag;
				XmlConvert.TryToBoolean(this.ProhibitDefaultResolverString, out flag);
				return flag;
			}
		}

		private static bool s_ProhibitDefaultUrlResolver
		{
			get
			{
				XsltConfigSection xsltConfigSection = ConfigurationManager.GetSection(XmlConfigurationString.XsltSectionPath) as XsltConfigSection;
				return xsltConfigSection != null && xsltConfigSection._ProhibitDefaultResolver;
			}
		}

		internal static XmlResolver CreateDefaultResolver()
		{
			if (XsltConfigSection.s_ProhibitDefaultUrlResolver)
			{
				return XmlNullResolver.Singleton;
			}
			return new XmlUrlResolver();
		}

		[ConfigurationProperty("limitXPathComplexity", DefaultValue = "true")]
		internal string LimitXPathComplexityString
		{
			get
			{
				return (string)base["limitXPathComplexity"];
			}
			set
			{
				base["limitXPathComplexity"] = value;
			}
		}

		private bool _LimitXPathComplexity
		{
			get
			{
				string limitXPathComplexityString = this.LimitXPathComplexityString;
				bool flag = true;
				XmlConvert.TryToBoolean(limitXPathComplexityString, out flag);
				return flag;
			}
		}

		internal static bool LimitXPathComplexity
		{
			get
			{
				XsltConfigSection xsltConfigSection = ConfigurationManager.GetSection(XmlConfigurationString.XsltSectionPath) as XsltConfigSection;
				return xsltConfigSection == null || xsltConfigSection._LimitXPathComplexity;
			}
		}

		[ConfigurationProperty("enableMemberAccessForXslCompiledTransform", DefaultValue = "False")]
		internal string EnableMemberAccessForXslCompiledTransformString
		{
			get
			{
				return (string)base["enableMemberAccessForXslCompiledTransform"];
			}
			set
			{
				base["enableMemberAccessForXslCompiledTransform"] = value;
			}
		}

		private bool _EnableMemberAccessForXslCompiledTransform
		{
			get
			{
				string enableMemberAccessForXslCompiledTransformString = this.EnableMemberAccessForXslCompiledTransformString;
				bool flag = false;
				XmlConvert.TryToBoolean(enableMemberAccessForXslCompiledTransformString, out flag);
				return flag;
			}
		}

		internal static bool EnableMemberAccessForXslCompiledTransform
		{
			get
			{
				XsltConfigSection xsltConfigSection = ConfigurationManager.GetSection(XmlConfigurationString.XsltSectionPath) as XsltConfigSection;
				return xsltConfigSection != null && xsltConfigSection._EnableMemberAccessForXslCompiledTransform;
			}
		}
	}
}
