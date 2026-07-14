using System;
using System.Collections.Generic;

public class DiscoverResources : KMonoBehaviour
{
	public void Add(Tag prefabId, Tag categoryTag)
	{
		if (this.resourcesToDiscover == null)
		{
			this.resourcesToDiscover = new List<DiscoverResources.Resource>();
		}
		this.resourcesToDiscover.Add(new DiscoverResources.Resource
		{
			prefabId = prefabId,
			categoryTag = categoryTag
		});
	}

	protected override void OnPrefabInit()
	{
		if (this.resourcesToDiscover == null)
		{
			return;
		}
		foreach (DiscoverResources.Resource resource in this.resourcesToDiscover)
		{
			DiscoveredResources.Instance.Discover(resource.prefabId, resource.categoryTag);
		}
	}

	public List<DiscoverResources.Resource> resourcesToDiscover;

	[Serializable]
	public struct Resource
	{
		public Tag prefabId;

		public Tag categoryTag;
	}
}
