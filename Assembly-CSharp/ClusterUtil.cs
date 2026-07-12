using System;
using System.Collections.Generic;
using UnityEngine;

public static class ClusterUtil
{
	public static WorldContainer GetMyWorld(this StateMachine.Instance smi)
	{
		return smi.GetComponent<StateMachineController>().GetMyWorld();
	}

	public static WorldContainer GetMyWorld(this KMonoBehaviour component)
	{
		return component.gameObject.GetMyWorld();
	}

	public static WorldContainer GetMyWorld(this GameObject gameObject)
	{
		int num = Grid.PosToCell(gameObject);
		if (Grid.IsValidCell(num) && Grid.WorldIdx[num] != ClusterManager.INVALID_WORLD_IDX)
		{
			return ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[num]);
		}
		return null;
	}

	public static int GetMyWorldId(this StateMachine.Instance smi)
	{
		return smi.GetComponent<StateMachineController>().GetMyWorldId();
	}

	public static int GetMyWorldId(this KMonoBehaviour component)
	{
		return component.gameObject.GetMyWorldId();
	}

	public static int GetMyWorldId(this GameObject gameObject)
	{
		int num = Grid.PosToCell(gameObject);
		if (Grid.IsValidCell(num) && Grid.WorldIdx[num] != ClusterManager.INVALID_WORLD_IDX)
		{
			return (int)Grid.WorldIdx[num];
		}
		return -1;
	}

	public static AxialI GetMyWorldLocation(this StateMachine.Instance smi)
	{
		return smi.GetComponent<StateMachineController>().GetMyWorldLocation();
	}

	public static AxialI GetMyWorldLocation(this KMonoBehaviour component)
	{
		return component.gameObject.GetMyWorldLocation();
	}

	public static AxialI GetMyWorldLocation(this GameObject gameObject)
	{
		ClusterGridEntity component = gameObject.GetComponent<ClusterGridEntity>();
		if (component != null)
		{
			return component.Location;
		}
		WorldContainer myWorld = gameObject.GetMyWorld();
		DebugUtil.DevAssertArgs(myWorld != null, new object[] { "GetMyWorldLocation called on object with no world", gameObject });
		return myWorld.GetComponent<ClusterGridEntity>().Location;
	}

	public static bool IsMyWorld(this GameObject go, GameObject otherGo)
	{
		int num = Grid.PosToCell(go);
		int num2 = Grid.PosToCell(otherGo);
		return Grid.IsValidCell(num) && Grid.IsValidCell(num2) && Grid.WorldIdx[num] == Grid.WorldIdx[num2];
	}

	public static bool IsMyParentWorld(this GameObject go, GameObject otherGo)
	{
		int num = Grid.PosToCell(go);
		int num2 = Grid.PosToCell(otherGo);
		if (Grid.IsValidCell(num) && Grid.IsValidCell(num2))
		{
			if (Grid.WorldIdx[num] == Grid.WorldIdx[num2])
			{
				return true;
			}
			WorldContainer world = ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[num]);
			WorldContainer world2 = ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[num2]);
			if (world == null)
			{
				DebugUtil.DevLogError(string.Format("{0} at {1} has a valid cell but no world", go, num));
			}
			if (world2 == null)
			{
				DebugUtil.DevLogError(string.Format("{0} at {1} has a valid cell but no world", otherGo, num2));
			}
			if (world != null && world2 != null && world.ParentWorldId == world2.ParentWorldId)
			{
				return true;
			}
		}
		return false;
	}

	public static int GetAsteroidWorldIdAtLocation(AxialI location)
	{
		foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
		{
			if (clusterGridEntity.Layer == EntityLayer.Asteroid)
			{
				WorldContainer component = clusterGridEntity.GetComponent<WorldContainer>();
				if (component != null)
				{
					return component.id;
				}
			}
		}
		return -1;
	}

	public static bool ActiveWorldIsRocketInterior()
	{
		return ClusterManager.Instance.activeWorld.IsModuleInterior;
	}

	public static bool ActiveWorldHasPrinter()
	{
		return ClusterManager.Instance.activeWorld.IsModuleInterior || Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId, false).Count > 0;
	}

	public static float GetAmountFromRelatedWorlds(WorldInventory worldInventory, Tag element)
	{
		WorldContainer worldContainer = worldInventory.WorldContainer;
		float num = 0f;
		int parentWorldId = worldContainer.ParentWorldId;
		foreach (WorldContainer worldContainer2 in ClusterManager.Instance.WorldContainers)
		{
			if (worldContainer2.ParentWorldId == parentWorldId)
			{
				num += worldContainer2.worldInventory.GetAmount(element, false);
			}
		}
		return num;
	}

	public static List<Pickupable> GetPickupablesFromRelatedWorlds(WorldInventory worldInventory, Tag tag)
	{
		List<Pickupable> list = new List<Pickupable>();
		int parentWorldId = worldInventory.GetComponent<WorldContainer>().ParentWorldId;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (worldContainer.ParentWorldId == parentWorldId)
			{
				ICollection<Pickupable> pickupables = worldContainer.worldInventory.GetPickupables(tag, false);
				if (pickupables != null)
				{
					list.AddRange(pickupables);
				}
			}
		}
		return list;
	}

	public static string DebugGetMyWorldName(this GameObject gameObject)
	{
		WorldContainer myWorld = gameObject.GetMyWorld();
		if (myWorld != null)
		{
			return myWorld.worldName;
		}
		return string.Format("InvalidWorld(pos={0})", gameObject.transform.GetPosition());
	}

	public static ClusterGridEntity ClosestVisibleAsteroidToLocation(AxialI location)
	{
		foreach (AxialI axialI in AxialUtil.SpiralOut(location, ClusterGrid.Instance.numRings))
		{
			if (ClusterGrid.Instance.IsValidCell(axialI) && ClusterGrid.Instance.IsCellVisible(axialI))
			{
				ClusterGridEntity asteroidAtCell = ClusterGrid.Instance.GetAsteroidAtCell(axialI);
				if (asteroidAtCell != null)
				{
					return asteroidAtCell;
				}
			}
		}
		return null;
	}
}
