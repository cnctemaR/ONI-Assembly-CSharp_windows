using System;
using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	public abstract class YamlVisitor : IYamlVisitor
	{
		protected virtual void Visit(YamlStream stream)
		{
		}

		protected virtual void Visited(YamlStream stream)
		{
		}

		protected virtual void Visit(YamlDocument document)
		{
		}

		protected virtual void Visited(YamlDocument document)
		{
		}

		protected virtual void Visit(YamlScalarNode scalar)
		{
		}

		protected virtual void Visited(YamlScalarNode scalar)
		{
		}

		protected virtual void Visit(YamlSequenceNode sequence)
		{
		}

		protected virtual void Visited(YamlSequenceNode sequence)
		{
		}

		protected virtual void Visit(YamlMappingNode mapping)
		{
		}

		protected virtual void Visited(YamlMappingNode mapping)
		{
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
				keyValuePair.Key.Accept(this);
				keyValuePair.Value.Accept(this);
			}
		}

		void IYamlVisitor.Visit(YamlStream stream)
		{
			this.Visit(stream);
			this.VisitChildren(stream);
			this.Visited(stream);
		}

		void IYamlVisitor.Visit(YamlDocument document)
		{
			this.Visit(document);
			this.VisitChildren(document);
			this.Visited(document);
		}

		void IYamlVisitor.Visit(YamlScalarNode scalar)
		{
			this.Visit(scalar);
			this.Visited(scalar);
		}

		void IYamlVisitor.Visit(YamlSequenceNode sequence)
		{
			this.Visit(sequence);
			this.VisitChildren(sequence);
			this.Visited(sequence);
		}

		void IYamlVisitor.Visit(YamlMappingNode mapping)
		{
			this.Visit(mapping);
			this.VisitChildren(mapping);
			this.Visited(mapping);
		}
	}
}
