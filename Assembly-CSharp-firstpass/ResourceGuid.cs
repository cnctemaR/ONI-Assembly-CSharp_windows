using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ResourceGuid : ISaveLoadableJson, IEquatable<ResourceGuid>
{
	public ResourceGuid(string id, Resource parent = null)
	{
		if (parent != null)
		{
			this.Guid = parent.Guid.Guid + "." + id;
		}
		else
		{
			this.Guid = id;
		}
	}

	public override int GetHashCode()
	{
		return this.Guid.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		ResourceGuid resourceGuid = (ResourceGuid)obj;
		return obj != null && this.Guid == resourceGuid.Guid;
	}

	public bool Equals(ResourceGuid other)
	{
		return this.Guid == other.Guid;
	}

	public override string ToString()
	{
		return this.Guid;
	}

	public static bool operator ==(ResourceGuid a, ResourceGuid b)
	{
		return a == b || (a != null && b != null && a.Guid == b.Guid);
	}

	public static bool operator !=(ResourceGuid a, ResourceGuid b)
	{
		return a != b && (a == null || b == null || a.Guid != b.Guid);
	}

	[Serialize]
	public string Guid;
}
