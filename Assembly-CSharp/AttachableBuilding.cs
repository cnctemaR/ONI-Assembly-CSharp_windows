using System;
using UnityEngine;

public class AttachableBuilding : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.gameObject);
		GameObject gameObject;
		if (!Grid.ObjectLayers[1].TryGetValue(num, out gameObject))
		{
			return;
		}
		if (gameObject != null)
		{
		}
	}

	public Tag attachableTag;
}
