using System;
using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class Resource
{
	public Resource()
	{
	}

	public Resource(string id, ResourceSet parent = null, string name = null)
	{
		global::Debug.Assert(id != null);
		this.Id = id;
		this.IdHash = new HashedString(this.Id);
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
		global::Debug.Assert(id != null);
		this.Guid = new ResourceGuid(id, null);
		this.Id = id;
		this.IdHash = new HashedString(this.Id);
		this.Name = name;
	}

	public ResourceGuid Guid { get; private set; }

	public virtual void Initialize()
	{
	}

	public string Name;

	public string Id;

	public HashedString IdHash;

	public bool Disabled;
}
