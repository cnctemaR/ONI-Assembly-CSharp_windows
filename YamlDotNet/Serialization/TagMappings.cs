using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization
{
	public sealed class TagMappings
	{
		public TagMappings()
		{
			this.mappings = new Dictionary<string, Type>();
		}

		public TagMappings(IDictionary<string, Type> mappings)
		{
			this.mappings = new Dictionary<string, Type>(mappings);
		}

		public void Add(string tag, Type mapping)
		{
			this.mappings.Add(tag, mapping);
		}

		internal Type GetMapping(string tag)
		{
			Type type;
			if (this.mappings.TryGetValue(tag, out type))
			{
				return type;
			}
			return null;
		}

		private readonly IDictionary<string, Type> mappings;
	}
}
