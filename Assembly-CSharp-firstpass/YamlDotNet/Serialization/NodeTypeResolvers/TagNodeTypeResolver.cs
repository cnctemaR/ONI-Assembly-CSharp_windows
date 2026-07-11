using System;
using System.Collections.Generic;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeTypeResolvers
{
	public sealed class TagNodeTypeResolver : INodeTypeResolver
	{
		public TagNodeTypeResolver(IDictionary<string, Type> tagMappings)
		{
			if (tagMappings == null)
			{
				throw new ArgumentNullException("tagMappings");
			}
			this.tagMappings = tagMappings;
		}

		bool INodeTypeResolver.Resolve(NodeEvent nodeEvent, ref Type currentType)
		{
			Type type;
			if (!string.IsNullOrEmpty(nodeEvent.Tag) && this.tagMappings.TryGetValue(nodeEvent.Tag, out type))
			{
				currentType = type;
				return true;
			}
			return false;
		}

		private readonly IDictionary<string, Type> tagMappings;
	}
}
