using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
	[DebuggerDisplay("Count = {children.Count}")]
	[Serializable]
	public sealed class YamlSequenceNode : YamlNode, IEnumerable<YamlNode>, IYamlConvertible, IEnumerable
	{
		internal YamlSequenceNode(IParser parser, DocumentLoadingState state)
		{
			this.Load(parser, state);
		}

		public YamlSequenceNode()
		{
		}

		public YamlSequenceNode(params YamlNode[] children)
			: this(children)
		{
		}

		public YamlSequenceNode(IEnumerable<YamlNode> children)
		{
			foreach (YamlNode yamlNode in children)
			{
				this.children.Add(yamlNode);
			}
		}

		public IList<YamlNode> Children
		{
			get
			{
				return this.children;
			}
		}

		public SequenceStyle Style { get; set; }

		private void Load(IParser parser, DocumentLoadingState state)
		{
			SequenceStart sequenceStart = parser.Expect<SequenceStart>();
			base.Load(sequenceStart, state);
			this.Style = sequenceStart.Style;
			bool flag = false;
			while (!parser.Accept<SequenceEnd>())
			{
				YamlNode yamlNode = YamlNode.ParseNode(parser, state);
				this.children.Add(yamlNode);
				flag |= yamlNode is YamlAliasNode;
			}
			if (flag)
			{
				state.AddNodeWithUnresolvedAliases(this);
			}
			parser.Expect<SequenceEnd>();
		}

		public void Add(YamlNode child)
		{
			this.children.Add(child);
		}

		public void Add(string child)
		{
			this.children.Add(new YamlScalarNode(child));
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i] is YamlAliasNode)
				{
					this.children[i] = state.GetNode(this.children[i].Anchor, true, this.children[i].Start, this.children[i].End);
				}
			}
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			emitter.Emit(new SequenceStart(base.Anchor, base.Tag, true, this.Style));
			foreach (YamlNode yamlNode in this.children)
			{
				yamlNode.Save(emitter, state);
			}
			emitter.Emit(new SequenceEnd());
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object obj)
		{
			YamlSequenceNode yamlSequenceNode = obj as YamlSequenceNode;
			if (yamlSequenceNode == null || !base.Equals(yamlSequenceNode) || this.children.Count != yamlSequenceNode.children.Count)
			{
				return false;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				if (!YamlNode.SafeEquals(this.children[i], yamlSequenceNode.children[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			foreach (YamlNode yamlNode in this.children)
			{
				num = YamlNode.CombineHashCodes(num, YamlNode.GetHashCode(yamlNode));
			}
			return num;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			level.Increment();
			yield return this;
			foreach (YamlNode child in this.children)
			{
				foreach (YamlNode node in child.SafeAllNodes(level))
				{
					yield return node;
				}
			}
			level.Decrement();
			yield break;
		}

		public override YamlNodeType NodeType
		{
			get
			{
				return YamlNodeType.Sequence;
			}
		}

		internal override string ToString(RecursionLevel level)
		{
			if (!level.TryIncrement())
			{
				return "WARNING! INFINITE RECURSION!";
			}
			StringBuilder stringBuilder = new StringBuilder("[ ");
			foreach (YamlNode yamlNode in this.children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(yamlNode.ToString(level));
			}
			stringBuilder.Append(" ]");
			level.Decrement();
			return stringBuilder.ToString();
		}

		public IEnumerator<YamlNode> GetEnumerator()
		{
			return this.Children.GetEnumerator();
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

		private readonly IList<YamlNode> children = new List<YamlNode>();
	}
}
