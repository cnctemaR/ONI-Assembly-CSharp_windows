using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/TravelTubeBridge")]
public class TravelTubeBridge : KMonoBehaviour, ITravelTubePiece
{
	public Vector3 Position
	{
		get
		{
			return base.transform.GetPosition();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Grid.HasTube[Grid.PosToCell(this)] = true;
		Components.ITravelTubePieces.Add(this);
		base.Subscribe<TravelTubeBridge>(774203113, TravelTubeBridge.OnBuildingBrokenDelegate);
		base.Subscribe<TravelTubeBridge>(-1735440190, TravelTubeBridge.OnBuildingFullyRepairedDelegate);
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe<TravelTubeBridge>(774203113, TravelTubeBridge.OnBuildingBrokenDelegate, false);
		base.Unsubscribe<TravelTubeBridge>(-1735440190, TravelTubeBridge.OnBuildingFullyRepairedDelegate, false);
		Grid.HasTube[Grid.PosToCell(this)] = false;
		Components.ITravelTubePieces.Remove(this);
		base.OnCleanUp();
	}

	private void OnBuildingBroken(object data)
	{
	}

	private void OnBuildingFullyRepaired(object data)
	{
	}

	private static readonly EventSystem.IntraObjectHandler<TravelTubeBridge> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<TravelTubeBridge>(delegate(TravelTubeBridge component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<TravelTubeBridge> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<TravelTubeBridge>(delegate(TravelTubeBridge component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});
}
