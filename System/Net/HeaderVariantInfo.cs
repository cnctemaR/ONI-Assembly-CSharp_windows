using System;

namespace System.Net
{
	internal struct HeaderVariantInfo
	{
		internal HeaderVariantInfo(string name, CookieVariant variant)
		{
			this.m_name = name;
			this.m_variant = variant;
		}

		internal string Name
		{
			get
			{
				return this.m_name;
			}
		}

		internal CookieVariant Variant
		{
			get
			{
				return this.m_variant;
			}
		}

		private string m_name;

		private CookieVariant m_variant;
	}
}
