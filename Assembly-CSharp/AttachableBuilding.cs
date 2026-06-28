using System;
using UnityEngine;

public class AttachableBuilding : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.gameObject);
		GameObject gameObject = Grid.ObjectLayers[1][num];
		if (gameObject != null)
		{
		}
	}

	public Tag attachableTag;
}
