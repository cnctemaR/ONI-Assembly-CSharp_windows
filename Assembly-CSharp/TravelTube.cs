using System;
using System.Collections;
using UnityEngine;

[SkipSaveFileSerialization]
public class TravelTube : KMonoBehaviour, IFirstFrameCallback, ITravelTubePiece
{
	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.travelTubeSystem;
	}

	public Vector3 Position
	{
		get
		{
			return base.transform.position;
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.transform.position);
		Game.Instance.travelTubeSystem.AddToNetworks(num, this, false);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(base.transform.position);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.travelTubeSystem.RemoveFromNetworks(num, this, false);
		}
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

	public void SetFirstFrameCallback(global::System.Action ffCb)
	{
		this.firstFrameCallback = ffCb;
		base.StartCoroutine(this.RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return null;
		if (this.firstFrameCallback != null)
		{
			this.firstFrameCallback();
			this.firstFrameCallback = null;
		}
		yield return null;
		yield break;
	}

	private global::System.Action firstFrameCallback;
}
