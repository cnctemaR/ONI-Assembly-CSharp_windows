using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MessageTarget : ISaveLoadable
{
	public MessageTarget(KPrefabID prefab_id)
	{
		this.prefabId.Set(prefab_id);
		this.position = prefab_id.transform.GetPosition();
		this.name = "Unknown";
		KSelectable component = prefab_id.GetComponent<KSelectable>();
		if (component != null)
		{
			this.name = component.GetName();
		}
		prefab_id.Subscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
	}

	public Vector3 GetPosition()
	{
		if (this.prefabId.Get() != null)
		{
			return this.prefabId.Get().transform.GetPosition();
		}
		return this.position;
	}

	public KSelectable GetSelectable()
	{
		if (this.prefabId.Get() != null)
		{
			return this.prefabId.Get().transform.GetComponent<KSelectable>();
		}
		return null;
	}

	public string GetName()
	{
		return this.name;
	}

	private void OnAbsorbedBy(object data)
	{
		if (this.prefabId.Get() != null)
		{
			this.prefabId.Get().Unsubscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
		}
		KPrefabID component = ((GameObject)data).GetComponent<KPrefabID>();
		component.Subscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
		this.prefabId.Set(component);
	}

	public void OnCleanUp()
	{
		if (this.prefabId.Get() != null)
		{
			this.prefabId.Get().Unsubscribe(-1940207677, new Action<object>(this.OnAbsorbedBy));
			this.prefabId.Set(null);
		}
	}

	[Serialize]
	private Ref<KPrefabID> prefabId = new Ref<KPrefabID>();

	[Serialize]
	private Vector3 position;

	[Serialize]
	private string name;
}
