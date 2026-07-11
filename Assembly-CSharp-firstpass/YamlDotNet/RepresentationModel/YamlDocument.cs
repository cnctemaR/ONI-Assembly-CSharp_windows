using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlDocument
	{
		public YamlDocument(YamlNode rootNode)
		{
			this.RootNode = rootNode;
		}

		public YamlDocument(string rootNode)
		{
			this.RootNode = new YamlScalarNode(rootNode);
		}

		internal YamlDocument(IParser parser)
		{
			DocumentLoadingState documentLoadingState = new DocumentLoadingState();
			parser.Expect<DocumentStart>();
			while (!parser.Accept<DocumentEnd>())
			{
				Debug.Assert(this.RootNode == null);
				this.RootNode = YamlNode.ParseNode(parser, documentLoadingState);
				if (this.RootNode is YamlAliasNode)
				{
					throw new YamlException();
				}
			}
			documentLoadingState.ResolveAliases();
			parser.Expect<DocumentEnd>();
		}

		public YamlNode RootNode { get; private set; }

		private void AssignAnchors()
		{
			YamlDocument.AnchorAssigningVisitor anchorAssigningVisitor = new YamlDocument.AnchorAssigningVisitor();
			anchorAssigningVisitor.AssignAnchors(this);
		}

		internal void Save(IEmitter emitter, bool assignAnchors = true)
		{
			if (assignAnchors)
			{
				this.AssignAnchors();
			}
			emitter.Emit(new DocumentStart());
			this.RootNode.Save(emitter, new EmitterState());
			emitter.Emit(new DocumentEnd(false));
		}

		public void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public IEnumerable<YamlNode> AllNodes
		{
			get
			{
				return this.RootNode.AllNodes;
			}
		}

		private class AnchorAssigningVisitor : YamlVisitorBase
		{
			public void AssignAnchors(YamlDocument document)
			{
				this.existingAnchors.Clear();
				this.visitedNodes.Clear();
				document.Accept(this);
				Random random = new Random();
				foreach (KeyValuePair<YamlNode, bool> keyValuePair in this.visitedNodes)
				{
					if (keyValuePair.Value)
					{
						string text;
						if (!string.IsNullOrEmpty(keyValuePair.Key.Anchor) && !this.existingAnchors.Contains(keyValuePair.Key.Anchor))
						{
							text = keyValuePair.Key.Anchor;
						}
						else
						{
							do
							{
								text = random.Next().ToString(CultureInfo.InvariantCulture);
							}
							while (this.existingAnchors.Contains(text));
						}
						this.existingAnchors.Add(text);
						keyValuePair.Key.Anchor = text;
					}
				}
			}

			private bool VisitNodeAndFindDuplicates(YamlNode node)
			{
				bool flag;
				if (this.visitedNodes.TryGetValue(node, out flag))
				{
					if (!flag)
					{
						this.visitedNodes[node] = true;
					}
					return !flag;
				}
				this.visitedNodes.Add(node, false);
				return false;
			}

			public override void Visit(YamlScalarNode scalar)
			{
				this.VisitNodeAndFindDuplicates(scalar);
			}

			public override void Visit(YamlMappingNode mapping)
			{
				if (!this.VisitNodeAndFindDuplicates(mapping))
				{
					base.Visit(mapping);
				}
			}

			public override void Visit(YamlSequenceNode sequence)
			{
				if (!this.VisitNodeAndFindDuplicates(sequence))
				{
					base.Visit(sequence);
				}
			}

			private readonly HashSet<string> existingAnchors = new HashSet<string>();

			private readonly Dictionary<YamlNode, bool> visitedNodes = new Dictionary<YamlNode, bool>();
		}
	}
}
