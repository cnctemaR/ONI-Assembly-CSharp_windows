using System;
using UnityEngine;

namespace Database
{
	public class TechTreeTitles : ResourceSet<TechTreeTitle>
	{
		public TechTreeTitles(ResourceSet parent)
			: base("TreeTitles", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			foreach (ResourceTreeNode resourceTreeNode in resourceTreeLoader)
			{
				string text = resourceTreeNode.Id.Substring(0, 1);
				if (string.Equals(text, "_"))
				{
					new TechTreeTitle(resourceTreeNode.Id, this, Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + resourceTreeNode.Id.ToUpper()), resourceTreeNode);
				}
			}
		}
	}
}
