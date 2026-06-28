using System;
using UnityEngine;

public class EntityPrefabs : KMonoBehaviour
{
	public static EntityPrefabs Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		EntityPrefabs.Instance = this;
	}

	public GameObject MinionPrefab;

	public GameObject Portrait;

	public GameObject DigPlacer;

	public GameObject MoveTarget;

	public GameObject MinionSelectPreview;

	public GameObject Exclamation;

	public GameObject LiquidSource;

	public GameObject LiquidChunk;

	public GameObject GasSource;

	public GameObject GasChunk;

	public GameObject SelectMarker;

	public GameObject ForegroundLayer;

	public GameObject Bones;

	public GameObject Spawner;

	public GameObject GenericBuildingPackage;
}
