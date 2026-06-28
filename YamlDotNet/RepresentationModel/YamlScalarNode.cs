using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[DebuggerDisplay("{Value}")]
	[Serializable]
	public class YamlScalarNode : YamlNode
	{
		public string Value { get; set; }

		public ScalarStyle Style { get; set; }

		internal YamlScalarNode(EventReader events, DocumentLoadingState state)
		{
			Scalar scalar = events.Expect<Scalar>();
			base.Load(scalar, state);
			this.Value = scalar.Value;
			this.Style = scalar.Style;
		}

		public YamlScalarNode()
		{
		}

		public YamlScalarNode(string value)
		{
			this.Value = value;
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			throw new NotSupportedException("Resolving an alias on a scalar node does not make sense");
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			emitter.Emit(new Scalar(base.Anchor, base.Tag, this.Value, this.Style, true, false));
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object other)
		{
			YamlScalarNode yamlScalarNode = other as YamlScalarNode;
			return yamlScalarNode != null && base.Equals(yamlScalarNode) && YamlNode.SafeEquals(this.Value, yamlScalarNode.Value);
		}

		public override int GetHashCode()
		{
			return YamlNode.CombineHashCodes(base.GetHashCode(), YamlNode.GetHashCode(this.Value));
		}

		public static implicit operator YamlScalarNode(string value)
		{
			return new YamlScalarNode(value);
		}

		public static explicit operator string(YamlScalarNode value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return this.Value;
		}

		public override IEnumerable<YamlNode> AllNodes
		{
			get
			{
				yield return this;
				yield break;
			}
		}
	}
}
