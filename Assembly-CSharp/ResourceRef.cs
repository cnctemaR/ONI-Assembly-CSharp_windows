using System;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ResourceRef<ResourceType> : ISaveLoadableJson where ResourceType : Resource
{
	public ResourceRef(ResourceType resource)
	{
		this.Set(resource);
	}

	public ResourceRef()
	{
	}

	public ResourceType Get()
	{
		return this.resource;
	}

	public void Set(ResourceType resource)
	{
		this.resource = resource;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (this.resource == null)
		{
			this.guid = null;
		}
		else
		{
			this.guid = this.resource.Guid;
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.guid != null)
		{
			this.resource = Db.Get().GetResource<ResourceType>(this.guid);
			this.guid = null;
		}
	}

	[Serialize]
	private ResourceGuid guid;

	private ResourceType resource;
}
