using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntityPrefabs")]
public class EntityPrefabs : KMonoBehaviour
{
	public static EntityPrefabs Instance { get; private set; }

	public static void DestroyInstance()
	{
		EntityPrefabs.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		EntityPrefabs.Instance = this;
	}

	public GameObject SelectMarker;

	public GameObject ForegroundLayer;
}
