using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
	[DebuggerDisplay("{Value}")]
	[Serializable]
	public sealed class YamlScalarNode : YamlNode, IYamlConvertible
	{
		public string Value { get; set; }

		public ScalarStyle Style { get; set; }

		internal YamlScalarNode(IParser parser, DocumentLoadingState state)
		{
			this.Load(parser, state);
		}

		private void Load(IParser parser, DocumentLoadingState state)
		{
			Scalar scalar = parser.Expect<Scalar>();
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
			emitter.Emit(new Scalar(base.Anchor, base.Tag, this.Value, this.Style, base.Tag == null, false));
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object obj)
		{
			YamlScalarNode yamlScalarNode = obj as YamlScalarNode;
			return yamlScalarNode != null && base.Equals(yamlScalarNode) && YamlNode.SafeEquals(this.Value, yamlScalarNode.Value);
		}

		public override int GetHashCode()
		{
			return YamlNode.CombineHashCodes(base.GetHashCode(), YamlNode.GetHashCode(this.Value));
		}

		public static explicit operator string(YamlScalarNode value)
		{
			return value.Value;
		}

		internal override string ToString(RecursionLevel level)
		{
			return this.Value;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			yield return this;
			yield break;
		}

		public override YamlNodeType NodeType
		{
			get
			{
				return YamlNodeType.Scalar;
			}
		}

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			this.Load(parser, new DocumentLoadingState());
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			this.Emit(emitter, new EmitterState());
		}
	}
}
