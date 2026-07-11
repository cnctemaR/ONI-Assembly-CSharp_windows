using System;
using UnityEngine;

public class EntityPrefabs : KMonoBehaviour
{
	public static EntityPrefabs Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		EntityPrefabs.Instance = this;
	}

	public GameObject SelectMarker;

	public GameObject ForegroundLayer;
}
