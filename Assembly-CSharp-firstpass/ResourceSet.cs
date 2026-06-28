using System;

[Serializable]
public abstract class ResourceSet : Resource
{
	public ResourceSet()
	{
	}

	public ResourceSet(string id, ResourceSet parent)
		: base(id, parent, null)
	{
	}

	public abstract Resource Add(Resource resource);

	public abstract int Count { get; }

	public abstract Resource GetResource(int idx);
}
