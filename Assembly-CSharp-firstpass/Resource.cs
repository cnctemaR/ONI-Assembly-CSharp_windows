using System;
using System.Diagnostics;

[DebuggerDisplay("{Id}")]
public class Resource
{
	public Resource()
	{
	}

	public Resource(string id, ResourceSet parent = null, string name = null)
	{
		this.Id = id;
		this.Guid = new ResourceGuid(id, parent);
		if (parent != null)
		{
			parent.Add(this);
		}
		if (name != null)
		{
			this.Name = name;
		}
		else
		{
			this.Name = id;
		}
	}

	public Resource(string id, string name)
	{
		this.Guid = new ResourceGuid(id, null);
		this.Id = id;
		this.Name = name;
	}

	public ResourceGuid Guid { get; private set; }

	public virtual void Initialize()
	{
	}

	public string Name;

	public string Id;

	public bool Disabled;
}
