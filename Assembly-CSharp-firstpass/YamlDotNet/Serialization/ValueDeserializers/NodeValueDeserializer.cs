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
				Func<IParser, Type, object> <>9__0;
				foreach (INodeDeserializer nodeDeserializer in this.deserializers)
				{
					Type type = typeFromEvent;
					Func<IParser, Type, object> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (IParser r, Type t) => nestedObjectDeserializer.DeserializeValue(r, t, state, nestedObjectDeserializer));
					}
					object obj;
					if (nodeDeserializer.Deserialize(parser, type, func, out obj))
					{
						return obj;
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
			using (IEnumerator<INodeTypeResolver> enumerator = this.typeResolvers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Resolve(nodeEvent, ref currentType))
					{
						break;
					}
				}
			}
			return currentType;
		}

		private readonly IList<INodeDeserializer> deserializers;

		private readonly IList<INodeTypeResolver> typeResolvers;
	}
}
