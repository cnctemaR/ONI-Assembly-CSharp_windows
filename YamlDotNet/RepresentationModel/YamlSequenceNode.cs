using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[DebuggerDisplay("Count = {children.Count}")]
	[Serializable]
	public class YamlSequenceNode : YamlNode, IEnumerable<YamlNode>, IEnumerable
	{
		public IList<YamlNode> Children
		{
			get
			{
				return this.children;
			}
		}

		public SequenceStyle Style { get; set; }

		internal YamlSequenceNode(EventReader events, DocumentLoadingState state)
		{
			SequenceStart sequenceStart = events.Expect<SequenceStart>();
			base.Load(sequenceStart, state);
			bool flag = false;
			while (!events.Accept<SequenceEnd>())
			{
				YamlNode yamlNode = YamlNode.ParseNode(events, state);
				this.children.Add(yamlNode);
				flag |= yamlNode is YamlAliasNode;
			}
			if (flag)
			{
				state.AddNodeWithUnresolvedAliases(this);
			}
			events.Expect<SequenceEnd>();
		}

		public YamlSequenceNode()
		{
		}

		public YamlSequenceNode(params YamlNode[] children)
			: this((IEnumerable<YamlNode>)children)
		{
		}

		public YamlSequenceNode(IEnumerable<YamlNode> children)
		{
			foreach (YamlNode yamlNode in children)
			{
				this.children.Add(yamlNode);
			}
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

		public override bool Equals(object other)
		{
			YamlSequenceNode yamlSequenceNode = other as YamlSequenceNode;
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

		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
				foreach (YamlNode child in this.children)
				{
					foreach (YamlNode node in child.AllNodes)
					{
						yield return node;
					}
				}
				yield break;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("[ ");
			foreach (YamlNode yamlNode in this.children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(yamlNode);
			}
			stringBuilder.Append(" ]");
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

		private readonly IList<YamlNode> children = new List<YamlNode>();
	}
}
