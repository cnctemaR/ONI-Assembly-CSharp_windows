using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class YamlDeserializationContext
	{
		public YamlSerializerOptions Options { get; set; }

		public IYamlFormatterResolver Resolver { get; set; }

		public YamlDeserializationContext(YamlSerializerOptions options)
		{
			this.Options = options;
			this.Resolver = options.Resolver;
		}

		public void Reset()
		{
			this.aliases.Clear();
		}

		public T DeserializeWithAlias<[Nullable(2)] T>(ref YamlParser parser)
		{
			IYamlFormatter<T> formatterWithVerify = this.Resolver.GetFormatterWithVerify<T>();
			return this.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
		}

		public T DeserializeWithAlias<[Nullable(2)] T>(IYamlFormatter<T> innerFormatter, ref YamlParser parser)
		{
			T t;
			if (this.TryResolveCurrentAlias<T>(ref parser, out t))
			{
				return t;
			}
			Anchor anchor;
			bool flag = parser.TryGetCurrentAnchor(out anchor);
			T t2 = innerFormatter.Deserialize(ref parser, this);
			if (flag)
			{
				this.RegisterAnchor(anchor, t2);
			}
			return t2;
		}

		private void RegisterAnchor(Anchor anchor, [Nullable(2)] object value)
		{
			this.aliases[anchor] = value;
		}

		[NullableContext(2)]
		private bool TryResolveCurrentAlias<T>(ref YamlParser parser, out T aliasValue)
		{
			if (parser.CurrentEventType != ParseEventType.Alias)
			{
				aliasValue = default(T);
				return false;
			}
			Anchor anchor;
			if (!parser.TryGetCurrentAnchor(out anchor))
			{
				aliasValue = default(T);
				return false;
			}
			parser.Read();
			object obj;
			if (!this.aliases.TryGetValue(anchor, out obj))
			{
				throw new YamlSerializerException(string.Format("Could not found an alias value of anchor: {0}", anchor));
			}
			if (obj == null)
			{
				aliasValue = default(T);
				return true;
			}
			if (obj is T)
			{
				T t = (T)((object)obj);
				aliasValue = t;
				return true;
			}
			throw new YamlSerializerException("The alias value is not a type of " + typeof(T).Name);
		}

		[Nullable(new byte[] { 1, 1, 2 })]
		private readonly Dictionary<Anchor, object> aliases = new Dictionary<Anchor, object>();
	}
}
