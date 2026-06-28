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
			BuildingAttachPoint component = gameObject.GetComponent<BuildingAttachPoint>();
			if (component != null)
			{
				this.attachPoint = new Ref<BuildingAttachPoint>(component);
			}
		}
	}

	public Tag attachableTag;

	private Ref<BuildingAttachPoint> attachPoint;
}
