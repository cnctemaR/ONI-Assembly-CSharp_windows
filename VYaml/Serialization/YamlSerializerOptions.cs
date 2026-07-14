using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;
using VYaml.Emitter;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class YamlSerializerOptions
	{
		public static YamlSerializerOptions Standard
		{
			get
			{
				return new YamlSerializerOptions
				{
					Resolver = StandardResolver.Instance
				};
			}
		}

		public IYamlFormatterResolver Resolver { get; set; } = StandardResolver.Instance;

		public NamingConvention NamingConvention { get; set; }

		public YamlEmitOptions EmitOptions { get; set; } = new YamlEmitOptions();

		public const NamingConvention DefaultNamingConvention = NamingConvention.LowerCamelCase;
	}
}
