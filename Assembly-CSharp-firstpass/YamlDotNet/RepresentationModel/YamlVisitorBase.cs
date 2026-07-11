using System;
using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	public abstract class YamlVisitorBase : IYamlVisitor
	{
		public virtual void Visit(YamlStream stream)
		{
			this.VisitChildren(stream);
		}

		public virtual void Visit(YamlDocument document)
		{
			this.VisitChildren(document);
		}

		public virtual void Visit(YamlScalarNode scalar)
		{
		}

		public virtual void Visit(YamlSequenceNode sequence)
		{
			this.VisitChildren(sequence);
		}

		public virtual void Visit(YamlMappingNode mapping)
		{
			this.VisitChildren(mapping);
		}

		protected virtual void VisitPair(YamlNode key, YamlNode value)
		{
			key.Accept(this);
			value.Accept(this);
		}

		protected virtual void VisitChildren(YamlStream stream)
		{
			foreach (YamlDocument yamlDocument in stream.Documents)
			{
				yamlDocument.Accept(this);
			}
		}

		protected virtual void VisitChildren(YamlDocument document)
		{
			if (document.RootNode != null)
			{
				document.RootNode.Accept(this);
			}
		}

		protected virtual void VisitChildren(YamlSequenceNode sequence)
		{
			foreach (YamlNode yamlNode in sequence.Children)
			{
				yamlNode.Accept(this);
			}
		}

		protected virtual void VisitChildren(YamlMappingNode mapping)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in mapping.Children)
			{
				this.VisitPair(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}
}
