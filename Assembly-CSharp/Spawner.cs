using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Spawner")]
public class Spawner : KMonoBehaviour, ISaveLoadable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		SaveGame.Instance.worldGenSpawner.AddLegacySpawner(this.prefabTag, Grid.PosToCell(this));
		Util.KDestroyGameObject(base.gameObject);
	}

	[Serialize]
	public Tag prefabTag;

	[Serialize]
	public int units = 1;
}
