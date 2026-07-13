using System;

public class ClustercraftInteriorDoor : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.ClusterCraftInteriorDoors.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.ClusterCraftInteriorDoors.Remove(this);
		foreach (int num in base.GetComponent<OccupyArea>().GetOccupiedGridCells())
		{
			Grid.HasDoor[num] = false;
		}
		base.OnCleanUp();
	}
}
