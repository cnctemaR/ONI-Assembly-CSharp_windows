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
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.transform.position);
		Game.Instance.travelTubeSystem.AddToNetworks(num, this, false);
		base.Subscribe(-1041684577, new Action<object>(this.OnConnectionsChanged));
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(base.transform.position);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.travelTubeSystem.RemoveFromNetworks(num, this, false);
		}
		base.Unsubscribe(-1041684577);
		Grid.HasTube[Grid.PosToCell(this)] = false;
		Components.ITravelTubePieces.Remove(this);
		if (this.dirtyNavCellUpdatedEntry != null)
		{
			this.dirtyNavCellUpdatedEntry.Release();
			this.dirtyNavCellUpdatedEntry = null;
		}
		base.OnCleanUp();
	}

	private void OnConnectionsChanged(object data)
	{
		this.connections = (UtilityConnections)data;
		bool flag = this.connections == UtilityConnections.Up || this.connections == UtilityConnections.Down || this.connections == UtilityConnections.Left || this.connections == UtilityConnections.Right;
		if (flag != this.isExitTube)
		{
			this.isExitTube = flag;
			this.UpdateExitListener(this.isExitTube);
			this.UpdateExitStatus();
		}
	}

	private void UpdateExitListener(bool enable)
	{
		if (enable && this.dirtyNavCellUpdatedEntry == null)
		{
			int num = Grid.PosToCell(base.transform.position);
			this.dirtyNavCellUpdatedEntry = GameScenePartitioner.Instance.Add("TravelTube.OnDirtyNavCellUpdated", this, num, GameScenePartitioner.Instance.dirtyNavCellUpdateLayer, new Action<object>(this.OnDirtyNavCellUpdated));
			this.OnDirtyNavCellUpdated(null);
		}
		else if (!enable && this.dirtyNavCellUpdatedEntry != null)
		{
			this.dirtyNavCellUpdatedEntry.Release();
			this.dirtyNavCellUpdatedEntry = null;
		}
	}

	private void OnDirtyNavCellUpdated(object data)
	{
		int num = Grid.PosToCell(base.transform.position);
		int num2 = num * NavGrid.MaxLinksPerCell;
		bool flag = false;
		if (this.isExitTube)
		{
			NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
			NavGrid.Link link = navGrid.Links[num2];
			while (link.link != PathFinder.InvalidHandle)
			{
				if (link.startNavType == NavType.Tube)
				{
					if (link.endNavType != NavType.Tube)
					{
						flag = true;
						break;
					}
					UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(link.link, num);
					if (this.connections == utilityConnections)
					{
						flag = true;
						break;
					}
				}
				num2++;
				link = navGrid.Links[num2];
			}
		}
		if (flag != this.hasValidExitTransitions)
		{
			this.hasValidExitTransitions = flag;
			this.UpdateExitStatus();
		}
	}

	private void UpdateExitStatus()
	{
		if (!this.isExitTube || this.hasValidExitTransitions)
		{
			this.connectedStatus = this.selectable.RemoveStatusItem(this.connectedStatus, false);
		}
		else if (this.connectedStatus == Guid.Empty)
		{
			this.connectedStatus = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NoTubeExits, null);
		}
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

	[MyCmpReq]
	private KSelectable selectable;

	private GameScenePartitionerEntry dirtyNavCellUpdatedEntry;

	private bool isExitTube;

	private bool hasValidExitTransitions;

	private UtilityConnections connections;

	private Guid connectedStatus;

	private global::System.Action firstFrameCallback;
}
