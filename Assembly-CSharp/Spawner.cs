using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Spawner : KMonoBehaviour, ISaveLoadable
{
	protected override void OnSpawn()
	{
		Components.Spawners.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.Spawners.Remove(this);
	}

	public void SetPrefabTag(Tag prefabTag)
	{
		this.prefabTag = prefabTag;
		KPrefabID component = base.gameObject.GetComponent<KPrefabID>();
		component.PrefabTag = GameTags.Spawner;
		component.SaveLoadTag = GameTags.Spawner;
	}

	public void DoSpawn()
	{
		GameObject prefab = Assets.GetPrefab(this.prefabTag);
		if (prefab == null)
		{
			Debug.LogWarning("Spawner could not find prefab with tag: " + this.prefabTag);
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.NoLayer;
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			sceneLayer = component.sceneLayer;
		}
		GameObject gameObject = GameUtil.KInstantiate(prefab, sceneLayer, Folder.Creatures, null, 0);
		gameObject.transform.SetParent(this.transform.parent);
		int num = Grid.PosToCell(this.transform.position);
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		if (component2 != null)
		{
			gameObject.transform.position = Grid.CellToPos(num, component2.defaultSpawnOffset, sceneLayer);
		}
		else
		{
			gameObject.transform.localPosition = Grid.CellToPosCBC(num, sceneLayer);
		}
		PrimaryElement component3 = gameObject.GetComponent<PrimaryElement>();
		if (this.units > 1 && component3 != null)
		{
			component3.Units = (float)this.units;
		}
		gameObject.SetActive(true);
		gameObject.Trigger(1119167081, null);
		Util.KDestroyGameObject(base.gameObject);
	}

	public void SetUnits(int units)
	{
		this.units = units;
	}

	[Serialize]
	public Tag prefabTag;

	[Serialize]
	public int units = 1;
}
