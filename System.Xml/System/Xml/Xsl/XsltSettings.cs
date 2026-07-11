using System;

namespace System.Xml.Xsl
{
	public sealed class XsltSettings
	{
		public XsltSettings()
		{
		}

		public XsltSettings(bool enableDocumentFunction, bool enableScript)
		{
			this.enableDocument = enableDocumentFunction;
			this.enableScript = enableScript;
		}

		private XsltSettings(bool readOnly)
		{
			this.readOnly = readOnly;
		}

		static XsltSettings()
		{
			XsltSettings.trustedXslt.enableDocument = true;
			XsltSettings.trustedXslt.enableScript = true;
		}

		public static XsltSettings Default
		{
			get
			{
				return XsltSettings.defaultSettings;
			}
		}

		public static XsltSettings TrustedXslt
		{
			get
			{
				return XsltSettings.trustedXslt;
			}
		}

		public bool EnableDocumentFunction
		{
			get
			{
				return this.enableDocument;
			}
			set
			{
				if (!this.readOnly)
				{
					this.enableDocument = value;
				}
			}
		}

		public bool EnableScript
		{
			get
			{
				return this.enableScript;
			}
			set
			{
				if (!this.readOnly)
				{
					this.enableScript = value;
				}
			}
		}

		private static readonly XsltSettings defaultSettings = new XsltSettings(true);

		private static readonly XsltSettings trustedXslt = new XsltSettings(true);

		private bool readOnly;

		private bool enableDocument;

		private bool enableScript;
	}
}
