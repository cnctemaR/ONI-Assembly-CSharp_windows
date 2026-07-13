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
		base.OnCleanUp();
	}
}
