using System;
using UnityEngine;

public class Meter : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = this.transform.position;
		if (this.offset == Meter.Offset.Behind)
		{
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
		}
		else
		{
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront);
		}
		this.transform.SetPosition(position);
	}

	public Meter.Offset offset = Meter.Offset.Behind;

	public enum Offset
	{
		Infront,
		Behind
	}
}
