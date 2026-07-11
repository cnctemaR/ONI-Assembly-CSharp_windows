using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{id}")]
public class Ref<ReferenceType> : ISaveLoadable where ReferenceType : KMonoBehaviour
{
	public Ref(ReferenceType obj)
	{
		this.Set(obj);
	}

	public Ref()
	{
	}

	private void UpdateID()
	{
		ReferenceType referenceType = this.Get();
		if (referenceType)
		{
			this.id = this.obj.GetComponent<KPrefabID>().InstanceID;
		}
		else
		{
			this.id = -1;
		}
	}

	[OnSerializing]
	public void OnSerializing()
	{
		this.UpdateID();
	}

	public int GetId()
	{
		this.UpdateID();
		return this.id;
	}

	public ComponentType Get<ComponentType>() where ComponentType : MonoBehaviour
	{
		ReferenceType referenceType = this.Get();
		if (referenceType == null)
		{
			return (ComponentType)((object)null);
		}
		return referenceType.GetComponent<ComponentType>();
	}

	public ReferenceType Get()
	{
		if (this.obj == null && this.id != -1)
		{
			KPrefabID instance = KPrefabIDTracker.Get().GetInstance(this.id);
			if (instance != null)
			{
				this.obj = instance.GetComponent<ReferenceType>();
				if (this.obj == null)
				{
					this.id = -1;
					global::Debug.LogWarning(string.Concat(new object[]
					{
						"Missing ",
						typeof(ReferenceType).Name,
						" reference: ",
						this.id
					}), null);
				}
			}
			else
			{
				global::Debug.LogWarning("Missing KPrefabID reference: " + this.id, null);
				this.id = -1;
			}
		}
		return this.obj;
	}

	public void Set(ReferenceType obj)
	{
		if (obj == null)
		{
			this.id = -1;
		}
		else
		{
			this.id = obj.GetComponent<KPrefabID>().InstanceID;
		}
		this.obj = obj;
	}

	[Serialize]
	private int id = -1;

	private ReferenceType obj;
}
