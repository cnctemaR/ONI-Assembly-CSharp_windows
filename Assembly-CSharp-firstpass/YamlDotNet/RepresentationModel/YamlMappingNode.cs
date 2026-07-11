using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public sealed class YamlMappingNode : YamlNode, IEnumerable<KeyValuePair<YamlNode, YamlNode>>, IEnumerable, IYamlConvertible
	{
		public IDictionary<YamlNode, YamlNode> Children
		{
			get
			{
				return this.children;
			}
		}

		public MappingStyle Style { get; set; }

		internal YamlMappingNode(IParser parser, DocumentLoadingState state)
		{
			this.Load(parser, state);
		}

		private void Load(IParser parser, DocumentLoadingState state)
		{
			MappingStart mappingStart = parser.Expect<MappingStart>();
			base.Load(mappingStart, state);
			this.Style = mappingStart.Style;
			bool flag = false;
			while (!parser.Accept<MappingEnd>())
			{
				YamlNode yamlNode = YamlNode.ParseNode(parser, state);
				YamlNode yamlNode2 = YamlNode.ParseNode(parser, state);
				try
				{
					this.children.Add(yamlNode, yamlNode2);
				}
				catch (ArgumentException ex)
				{
					throw new YamlException(yamlNode.Start, yamlNode.End, "Duplicate key", ex);
				}
				flag |= yamlNode is YamlAliasNode || yamlNode2 is YamlAliasNode;
			}
			if (flag)
			{
				state.AddNodeWithUnresolvedAliases(this);
			}
			parser.Expect<MappingEnd>();
		}

		public YamlMappingNode()
		{
		}

		public YamlMappingNode(int dummy)
		{
		}

		public YamlMappingNode(params KeyValuePair<YamlNode, YamlNode>[] children)
			: this(children)
		{
		}

		public YamlMappingNode(IEnumerable<KeyValuePair<YamlNode, YamlNode>> children)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in children)
			{
				this.children.Add(keyValuePair);
			}
		}

		public YamlMappingNode(params YamlNode[] children)
			: this(children)
		{
		}

		public YamlMappingNode(IEnumerable<YamlNode> children)
		{
			using (IEnumerator<YamlNode> enumerator = children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					YamlNode yamlNode = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						throw new ArgumentException("When constructing a mapping node with a sequence, the number of elements of the sequence must be even.");
					}
					this.Add(yamlNode, enumerator.Current);
				}
			}
		}

		public void Add(YamlNode key, YamlNode value)
		{
			this.children.Add(key, value);
		}

		public void Add(string key, YamlNode value)
		{
			this.children.Add(new YamlScalarNode(key), value);
		}

		public void Add(YamlNode key, string value)
		{
			this.children.Add(key, new YamlScalarNode(value));
		}

		public void Add(string key, string value)
		{
			this.children.Add(new YamlScalarNode(key), new YamlScalarNode(value));
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			Dictionary<YamlNode, YamlNode> dictionary = null;
			Dictionary<YamlNode, YamlNode> dictionary2 = null;
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in this.children)
			{
				if (keyValuePair.Key is YamlAliasNode)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<YamlNode, YamlNode>();
					}
					dictionary.Add(keyValuePair.Key, state.GetNode(keyValuePair.Key.Anchor, true, keyValuePair.Key.Start, keyValuePair.Key.End));
				}
				if (keyValuePair.Value is YamlAliasNode)
				{
					if (dictionary2 == null)
					{
						dictionary2 = new Dictionary<YamlNode, YamlNode>();
					}
					dictionary2.Add(keyValuePair.Key, state.GetNode(keyValuePair.Value.Anchor, true, keyValuePair.Value.Start, keyValuePair.Value.End));
				}
			}
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair2 in dictionary2)
				{
					this.children[keyValuePair2.Key] = keyValuePair2.Value;
				}
			}
			if (dictionary != null)
			{
				foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair3 in dictionary)
				{
					YamlNode yamlNode = this.children[keyValuePair3.Key];
					this.children.Remove(keyValuePair3.Key);
					this.children.Add(keyValuePair3.Value, yamlNode);
				}
			}
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			emitter.Emit(new MappingStart(base.Anchor, base.Tag, true, this.Style));
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in this.children)
			{
				keyValuePair.Key.Save(emitter, state);
				keyValuePair.Value.Save(emitter, state);
			}
			emitter.Emit(new MappingEnd());
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object obj)
		{
			YamlMappingNode yamlMappingNode = obj as YamlMappingNode;
			if (yamlMappingNode == null || !base.Equals(yamlMappingNode) || this.children.Count != yamlMappingNode.children.Count)
			{
				return false;
			}
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in this.children)
			{
				YamlNode yamlNode;
				if (!yamlMappingNode.children.TryGetValue(keyValuePair.Key, out yamlNode) || !YamlNode.SafeEquals(keyValuePair.Value, yamlNode))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in this.children)
			{
				num = YamlNode.CombineHashCodes(num, YamlNode.GetHashCode(keyValuePair.Key));
				num = YamlNode.CombineHashCodes(num, YamlNode.GetHashCode(keyValuePair.Value));
			}
			return num;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			level.Increment();
			yield return this;
			foreach (KeyValuePair<YamlNode, YamlNode> child in this.children)
			{
				foreach (YamlNode yamlNode in child.Key.SafeAllNodes(level))
				{
					yield return yamlNode;
				}
				IEnumerator<YamlNode> enumerator2 = null;
				foreach (YamlNode yamlNode2 in child.Value.SafeAllNodes(level))
				{
					yield return yamlNode2;
				}
				enumerator2 = null;
				child = default(KeyValuePair<YamlNode, YamlNode>);
			}
			IEnumerator<KeyValuePair<YamlNode, YamlNode>> enumerator = null;
			level.Decrement();
			yield break;
			yield break;
		}

		public override YamlNodeType NodeType
		{
			get
			{
				return YamlNodeType.Mapping;
			}
		}

		internal override string ToString(RecursionLevel level)
		{
			if (!level.TryIncrement())
			{
				return "WARNING! INFINITE RECURSION!";
			}
			StringBuilder stringBuilder = new StringBuilder("{ ");
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in this.children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("{ ").Append(keyValuePair.Key.ToString(level)).Append(", ")
					.Append(keyValuePair.Value.ToString(level))
					.Append(" }");
			}
			stringBuilder.Append(" }");
			level.Decrement();
			return stringBuilder.ToString();
		}

		public IEnumerator<KeyValuePair<YamlNode, YamlNode>> GetEnumerator()
		{
			return this.children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			this.Load(parser, new DocumentLoadingState());
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			this.Emit(emitter, new EmitterState());
		}

		public static YamlMappingNode FromObject(object mapping)
		{
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			YamlMappingNode yamlMappingNode = new YamlMappingNode(0);
			foreach (PropertyInfo propertyInfo in mapping.GetType().GetPublicProperties())
			{
				if (propertyInfo.CanRead && propertyInfo.GetGetMethod().GetParameters().Length == 0)
				{
					object value = propertyInfo.GetValue(mapping, null);
					YamlNode yamlNode = (value as YamlNode) ?? Convert.ToString(value);
					yamlMappingNode.Add(propertyInfo.Name, yamlNode);
				}
			}
			return yamlMappingNode;
		}

		private readonly IDictionary<YamlNode, YamlNode> children = new Dictionary<YamlNode, YamlNode>();
	}
}
