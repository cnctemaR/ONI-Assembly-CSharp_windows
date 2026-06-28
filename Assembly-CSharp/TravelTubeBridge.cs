using System;
using UnityEngine;

[SkipSaveFileSerialization]
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
		base.Subscribe(774203113, new Action<object>(this.OnBuildingBroken));
		base.Subscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(774203113, new Action<object>(this.OnBuildingBroken));
		base.Unsubscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
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
}
