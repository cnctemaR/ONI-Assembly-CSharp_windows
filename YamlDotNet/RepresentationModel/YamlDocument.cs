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
		public YamlNode RootNode { get; private set; }

		public YamlDocument(YamlNode rootNode)
		{
			this.RootNode = rootNode;
		}

		public YamlDocument(string rootNode)
		{
			this.RootNode = new YamlScalarNode(rootNode);
		}

		internal YamlDocument(EventReader events)
		{
			DocumentLoadingState documentLoadingState = new DocumentLoadingState();
			events.Expect<DocumentStart>();
			while (!events.Accept<DocumentEnd>())
			{
				this.RootNode = YamlNode.ParseNode(events, documentLoadingState);
				if (this.RootNode is YamlAliasNode)
				{
					throw new YamlException();
				}
			}
			documentLoadingState.ResolveAliases();
			events.Expect<DocumentEnd>();
		}

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

		private class AnchorAssigningVisitor : YamlVisitor
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
						do
						{
							text = random.Next().ToString(CultureInfo.InvariantCulture);
						}
						while (this.existingAnchors.Contains(text));
						this.existingAnchors.Add(text);
						keyValuePair.Key.Anchor = text;
					}
				}
			}

			private void VisitNode(YamlNode node)
			{
				if (string.IsNullOrEmpty(node.Anchor))
				{
					bool flag;
					if (!this.visitedNodes.TryGetValue(node, out flag))
					{
						this.visitedNodes.Add(node, false);
						return;
					}
					if (!flag)
					{
						this.visitedNodes[node] = true;
						return;
					}
				}
				else
				{
					this.existingAnchors.Add(node.Anchor);
				}
			}

			protected override void Visit(YamlScalarNode scalar)
			{
				this.VisitNode(scalar);
			}

			protected override void Visit(YamlMappingNode mapping)
			{
				this.VisitNode(mapping);
			}

			protected override void Visit(YamlSequenceNode sequence)
			{
				this.VisitNode(sequence);
			}

			private readonly HashSet<string> existingAnchors = new HashSet<string>();

			private readonly Dictionary<YamlNode, bool> visitedNodes = new Dictionary<YamlNode, bool>();
		}
	}
}
