using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class NullNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			value = null;
			NodeEvent nodeEvent = reader.Peek<NodeEvent>();
			bool flag = nodeEvent != null && this.NodeIsNull(nodeEvent);
			if (flag)
			{
				reader.SkipThisAndNestedEvents();
			}
			return flag;
		}

		private bool NodeIsNull(NodeEvent nodeEvent)
		{
			if (nodeEvent.Tag == "tag:yaml.org,2002:null")
			{
				return true;
			}
			Scalar scalar = nodeEvent as Scalar;
			if (scalar == null || scalar.Style != ScalarStyle.Plain)
			{
				return false;
			}
			string value = scalar.Value;
			return value == "" || value == "~" || value == "null" || value == "Null" || value == "NULL";
		}
	}
}
