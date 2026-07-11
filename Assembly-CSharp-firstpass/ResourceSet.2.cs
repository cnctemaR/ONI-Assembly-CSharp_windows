using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class ResourceSet<T> : ResourceSet where T : Resource
{
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

	public ResourceSet()
	{
	}

	public ResourceSet(string id, ResourceSet parent)
		: base(id, parent)
	{
	}

	public override void Initialize()
	{
		foreach (T t in this.resources)
		{
			t.Initialize();
		}
	}

	public bool Exists(string id)
	{
		using (List<T>.Enumerator enumerator = this.resources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Id == id)
				{
					return true;
				}
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
		return default(T);
	}

	public T Get(HashedString id)
	{
		foreach (T t in this.resources)
		{
			if (new HashedString(t.Id) == id)
			{
				return t;
			}
		}
		Debug.LogError(string.Concat(new object[]
		{
			"Could not find ",
			typeof(T).ToString(),
			": ",
			id
		}));
		return default(T);
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
		Debug.LogError("Could not find " + typeof(T).ToString() + ": " + id);
		return default(T);
	}

	public override Resource Add(Resource resource)
	{
		T t = resource as T;
		if (t == null)
		{
			Debug.LogError("Resource type mismatch: " + resource.GetType().Name + " does not match " + typeof(T).Name);
		}
		this.Add(t);
		return resource;
	}

	public T Add(T resource)
	{
		if (resource == null)
		{
			Debug.LogError("Tried to add a null to the resource set");
			return default(T);
		}
		this.resources.Add(resource);
		return resource;
	}

	public void ResolveReferences()
	{
		foreach (FieldInfo fieldInfo in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
		{
			if (fieldInfo.FieldType.IsSubclassOf(typeof(Resource)) && fieldInfo.GetValue(this) == null)
			{
				Resource resource = this.Get(fieldInfo.Name);
				if (resource != null)
				{
					fieldInfo.SetValue(this, resource);
				}
			}
		}
	}

	public List<T> resources = new List<T>();
}
