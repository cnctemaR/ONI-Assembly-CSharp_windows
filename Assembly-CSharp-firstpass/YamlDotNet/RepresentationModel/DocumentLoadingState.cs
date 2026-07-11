using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
	internal class DocumentLoadingState
	{
		public void AddAnchor(YamlNode node)
		{
			if (node.Anchor == null)
			{
				throw new ArgumentException("The specified node does not have an anchor");
			}
			if (this.anchors.ContainsKey(node.Anchor))
			{
				this.anchors[node.Anchor] = node;
				return;
			}
			this.anchors.Add(node.Anchor, node);
		}

		public YamlNode GetNode(string anchor, bool throwException, Mark start, Mark end)
		{
			YamlNode yamlNode;
			if (this.anchors.TryGetValue(anchor, out yamlNode))
			{
				return yamlNode;
			}
			if (throwException)
			{
				throw new AnchorNotFoundException(start, end, string.Format(CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", anchor));
			}
			return null;
		}

		public void AddNodeWithUnresolvedAliases(YamlNode node)
		{
			this.nodesWithUnresolvedAliases.Add(node);
		}

		public void ResolveAliases()
		{
			foreach (YamlNode yamlNode in this.nodesWithUnresolvedAliases)
			{
				yamlNode.ResolveAliases(this);
			}
		}

		private readonly IDictionary<string, YamlNode> anchors = new Dictionary<string, YamlNode>();

		private readonly IList<YamlNode> nodesWithUnresolvedAliases = new List<YamlNode>();
	}
}
