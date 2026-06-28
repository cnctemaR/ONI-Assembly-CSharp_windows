using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlMappingNode : YamlNode, IEnumerable<KeyValuePair<YamlNode, YamlNode>>, IEnumerable
	{
		public IDictionary<YamlNode, YamlNode> Children
		{
			get
			{
				return this.children;
			}
		}

		public MappingStyle Style { get; set; }

		internal YamlMappingNode(EventReader events, DocumentLoadingState state)
		{
			MappingStart mappingStart = events.Expect<MappingStart>();
			base.Load(mappingStart, state);
			bool flag = false;
			while (!events.Accept<MappingEnd>())
			{
				YamlNode yamlNode = YamlNode.ParseNode(events, state);
				YamlNode yamlNode2 = YamlNode.ParseNode(events, state);
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
			events.Expect<MappingEnd>();
		}

		public YamlMappingNode()
		{
		}

		public YamlMappingNode(params KeyValuePair<YamlNode, YamlNode>[] children)
			: this((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)children)
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
			: this((IEnumerable<YamlNode>)children)
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

		public override bool Equals(object other)
		{
			YamlMappingNode yamlMappingNode = other as YamlMappingNode;
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

		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
				foreach (KeyValuePair<YamlNode, YamlNode> child in this.children)
				{
					KeyValuePair<YamlNode, YamlNode> keyValuePair = child;
					foreach (YamlNode node in keyValuePair.Key.AllNodes)
					{
						yield return node;
					}
					KeyValuePair<YamlNode, YamlNode> keyValuePair2 = child;
					foreach (YamlNode node2 in keyValuePair2.Value.AllNodes)
					{
						yield return node2;
					}
				}
				yield break;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("{ ");
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in this.children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("{ ").Append(keyValuePair.Key).Append(", ")
					.Append(keyValuePair.Value)
					.Append(" }");
			}
			stringBuilder.Append(" }");
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

		private readonly IDictionary<YamlNode, YamlNode> children = new Dictionary<YamlNode, YamlNode>();
	}
}
