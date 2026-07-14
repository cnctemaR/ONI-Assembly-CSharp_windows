using System;
using System.Runtime.CompilerServices;

namespace VYaml.Emitter
{
	public class YamlEmitOptions
	{
		public int IndentWidth { get; set; } = 2;

		public ScalarStyle StringQuoteStyle
		{
			get
			{
				return this.stringQuoteStyle;
			}
			set
			{
				if (value != ScalarStyle.SingleQuoted && value != ScalarStyle.DoubleQuoted)
				{
					throw new InvalidOperationException("Invalid scalar style");
				}
				this.stringQuoteStyle = value;
			}
		}

		[Nullable(1)]
		public static readonly YamlEmitOptions Default = new YamlEmitOptions();

		private ScalarStyle stringQuoteStyle = ScalarStyle.DoubleQuoted;
	}
}
