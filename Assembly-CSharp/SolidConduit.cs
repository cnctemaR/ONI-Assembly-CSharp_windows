using System;
using System.Collections;
using UnityEngine;

[SkipSaveFileSerialization]
public class SolidConduit : KMonoBehaviour, IHaveUtilityNetworkMgr
{
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

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.solidConduitSystem;
	}

	public UtilityNetwork GetNetwork()
	{
		return this.GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
	}

	public static SolidConduitFlow GetFlowManager()
	{
		return Game.Instance.solidConduitFlow;
	}

	public Vector3 Position
	{
		get
		{
			return base.transform.GetPosition();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Conveyor, this);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(this);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			this.GetNetworkManager().RemoveFromNetworks(num, this, false);
			SolidConduit.GetFlowManager().EmptyConduit(num);
		}
		base.OnCleanUp();
	}

	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	private global::System.Action firstFrameCallback;
}
