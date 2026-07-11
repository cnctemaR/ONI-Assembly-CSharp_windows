using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	public sealed class WebUtilityElement : ConfigurationElement
	{
		public WebUtilityElement()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public UnicodeDecodingConformance UnicodeDecodingConformance
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return UnicodeDecodingConformance.Auto;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public UnicodeEncodingConformance UnicodeEncodingConformance
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return UnicodeEncodingConformance.Auto;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}
	}
}
