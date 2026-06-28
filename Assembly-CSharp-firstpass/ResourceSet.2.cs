using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class ResourceSet<T> : ResourceSet where T : Resource
{
	public ResourceSet()
	{
	}

	public ResourceSet(string id, ResourceSet parent)
		: base(id, parent)
	{
	}

	public IEnumerator<T> GetEnumerator()
	{
		return this.resources.GetEnumerator();
	}

	public T this[int idx]
	{
		get
		{
			return this.resources[idx];
		}
	}

	public override int Count
	{
		get
		{
			return this.resources.Count;
		}
	}

	public override Resource GetResource(int idx)
	{
		return this.resources[idx];
	}

	public override void Initialize()
	{
		foreach (T t in this.resources)
		{
			Resource resource = t;
			resource.Initialize();
		}
	}

	public bool Exists(string id)
	{
		foreach (T t in this.resources)
		{
			if (t.Id == id)
			{
				return true;
			}
		}
		return false;
	}

	public T TryGet(string id)
	{
		foreach (T t in this.resources)
		{
			if (t.Id == id)
			{
				return t;
			}
		}
		return (T)((object)null);
	}

	public T Get(string id)
	{
		foreach (T t in this.resources)
		{
			if (t.Id == id)
			{
				return t;
			}
		}
		Debug.LogError("Could not find " + typeof(T).ToString() + ": " + id, null);
		return (T)((object)null);
	}

	public override Resource Add(Resource resource)
	{
		T t = resource as T;
		if (t == null)
		{
			Debug.LogError("Resource type mismatch: " + resource.GetType().Name + " does not match " + typeof(T).Name, null);
		}
		this.Add(t);
		return resource;
	}

	public T Add(T resource)
	{
		if (resource == null)
		{
			Output.LogError(new object[] { "Tried to add a null to the resource set" });
			return (T)((object)null);
		}
		this.resources.Add(resource);
		return resource;
	}

	public void ResolveReferences()
	{
		Type type = base.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType.IsSubclassOf(typeof(Resource)))
			{
				if (fieldInfo.GetValue(this) == null)
				{
					Resource resource = this.Get(fieldInfo.Name);
					if (resource != null)
					{
						fieldInfo.SetValue(this, resource);
					}
				}
			}
		}
	}

	public Resource[] ToArray()
	{
		List<Resource> list = new List<Resource>();
		foreach (T t in this)
		{
			Resource resource = t;
			list.Add(resource);
		}
		return list.ToArray();
	}

	public List<T> resources = new List<T>();
}
