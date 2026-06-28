using System;
using System.Collections.Generic;
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

		internal static YamlNode ParseNode(EventReader events, DocumentLoadingState state)
		{
			if (events.Accept<Scalar>())
			{
				return new YamlScalarNode(events, state);
			}
			if (events.Accept<SequenceStart>())
			{
				return new YamlSequenceNode(events, state);
			}
			if (events.Accept<MappingStart>())
			{
				return new YamlMappingNode(events, state);
			}
			if (events.Accept<AnchorAlias>())
			{
				AnchorAlias anchorAlias = events.Expect<AnchorAlias>();
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

		public abstract IEnumerable<YamlNode> AllNodes { get; }
	}
}
