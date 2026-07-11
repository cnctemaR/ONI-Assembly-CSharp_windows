using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public abstract class YamlNode
	{
		public string Anchor { get; set; }

		public string Tag { get; set; }

		public Mark Start { get; private set; }

		public Mark End { get; private set; }

		internal void Load(NodeEvent yamlEvent, DocumentLoadingState state)
		{
			this.Tag = yamlEvent.Tag;
			if (yamlEvent.Anchor != null)
			{
				this.Anchor = yamlEvent.Anchor;
				state.AddAnchor(this);
			}
			this.Start = yamlEvent.Start;
			this.End = yamlEvent.End;
		}

		internal static YamlNode ParseNode(IParser parser, DocumentLoadingState state)
		{
			if (parser.Accept<Scalar>())
			{
				return new YamlScalarNode(parser, state);
			}
			if (parser.Accept<SequenceStart>())
			{
				return new YamlSequenceNode(parser, state);
			}
			if (parser.Accept<MappingStart>())
			{
				return new YamlMappingNode(parser, state);
			}
			if (parser.Accept<AnchorAlias>())
			{
				AnchorAlias anchorAlias = parser.Expect<AnchorAlias>();
				return state.GetNode(anchorAlias.Value, false, anchorAlias.Start, anchorAlias.End) ?? new YamlAliasNode(anchorAlias.Value);
			}
			throw new ArgumentException("The current event is of an unsupported type.", "events");
		}

		internal abstract void ResolveAliases(DocumentLoadingState state);

		internal void Save(IEmitter emitter, EmitterState state)
		{
			if (!string.IsNullOrEmpty(this.Anchor) && !state.EmittedAnchors.Add(this.Anchor))
			{
				emitter.Emit(new AnchorAlias(this.Anchor));
				return;
			}
			this.Emit(emitter, state);
		}

		internal abstract void Emit(IEmitter emitter, EmitterState state);

		public abstract void Accept(IYamlVisitor visitor);

		protected bool Equals(YamlNode other)
		{
			return YamlNode.SafeEquals(this.Tag, other.Tag);
		}

		protected static bool SafeEquals(object first, object second)
		{
			if (first != null)
			{
				return first.Equals(second);
			}
			return second == null || second.Equals(first);
		}

		public override int GetHashCode()
		{
			return YamlNode.GetHashCode(this.Tag);
		}

		protected static int GetHashCode(object value)
		{
			if (value != null)
			{
				return value.GetHashCode();
			}
			return 0;
		}

		protected static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}

		public override string ToString()
		{
			RecursionLevel recursionLevel = new RecursionLevel(1000);
			return this.ToString(recursionLevel);
		}

		internal abstract string ToString(RecursionLevel level);

		public IEnumerable<YamlNode> AllNodes
		{
			get
			{
				RecursionLevel recursionLevel = new RecursionLevel(1000);
				return this.SafeAllNodes(recursionLevel);
			}
		}

		internal abstract IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level);

		public abstract YamlNodeType NodeType { get; }

		public static implicit operator YamlNode(string value)
		{
			return new YamlScalarNode(value);
		}

		public static implicit operator YamlNode(string[] sequence)
		{
			return new YamlSequenceNode(sequence.Select<string, YamlNode>((string i) => i));
		}

		public static explicit operator string(YamlNode scalar)
		{
			return ((YamlScalarNode)scalar).Value;
		}

		public YamlNode this[int index]
		{
			get
			{
				return ((YamlSequenceNode)this).Children[index];
			}
		}

		public YamlNode this[YamlNode key]
		{
			get
			{
				return ((YamlMappingNode)this).Children[key];
			}
		}

		private const int MaximumRecursionLevel = 1000;

		internal const string MaximumRecursionLevelReachedToStringValue = "WARNING! INFINITE RECURSION!";
	}
}
