using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
	public sealed class NodeValueDeserializer : IValueDeserializer
	{
		public NodeValueDeserializer(IList<INodeDeserializer> deserializers, IList<INodeTypeResolver> typeResolvers)
		{
			if (deserializers == null)
			{
				throw new ArgumentNullException("deserializers");
			}
			this.deserializers = deserializers;
			if (typeResolvers == null)
			{
				throw new ArgumentNullException("typeResolvers");
			}
			this.typeResolvers = typeResolvers;
		}

		public object DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
		{
			NodeEvent nodeEvent = parser.Peek<NodeEvent>();
			Type typeFromEvent = this.GetTypeFromEvent(nodeEvent, expectedType);
			try
			{
				foreach (INodeDeserializer nodeDeserializer in this.deserializers)
				{
					object obj;
					if (nodeDeserializer.Deserialize(parser, typeFromEvent, (IParser r, Type t) => nestedObjectDeserializer.DeserializeValue(r, t, state, nestedObjectDeserializer), out obj))
					{
						return TypeConverter.ChangeType(obj, expectedType);
					}
				}
			}
			catch (YamlException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new YamlException(nodeEvent.Start, nodeEvent.End, "Exception during deserialization", ex);
			}
			throw new YamlException(nodeEvent.Start, nodeEvent.End, string.Format("No node deserializer was able to deserialize the node into type {0}", expectedType.AssemblyQualifiedName));
		}

		private Type GetTypeFromEvent(NodeEvent nodeEvent, Type currentType)
		{
			foreach (INodeTypeResolver nodeTypeResolver in this.typeResolvers)
			{
				if (nodeTypeResolver.Resolve(nodeEvent, ref currentType))
				{
					break;
				}
			}
			return currentType;
		}

		private readonly IList<INodeDeserializer> deserializers;

		private readonly IList<INodeTypeResolver> typeResolvers;
	}
}
