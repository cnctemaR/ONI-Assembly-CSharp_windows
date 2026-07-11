using System;
using System.Globalization;

namespace YamlDotNet
{
	internal sealed class CultureInfoAdapter : CultureInfo
	{
		public CultureInfoAdapter(CultureInfo baseCulture, IFormatProvider provider)
			: base(baseCulture.LCID)
		{
			this._provider = provider;
		}

		public override object GetFormat(Type formatType)
		{
			return this._provider.GetFormat(formatType);
		}

		private readonly IFormatProvider _provider;
	}
}
