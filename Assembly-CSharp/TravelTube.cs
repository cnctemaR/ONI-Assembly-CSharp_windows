using System;
using System.Collections;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/TravelTube")]
public class TravelTube : KMonoBehaviour, IFirstFrameCallback, ITravelTubePiece, IHaveUtilityNetworkMgr
{
	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.travelTubeSystem;
	}

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
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.travelTubeSystem.AddToNetworks(num, this, false);
		base.Subscribe<TravelTube>(-1041684577, TravelTube.OnConnectionsChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.travelTubeSystem.RemoveFromNetworks(num, this, false);
		}
		base.Unsubscribe(-1041684577);
		Grid.HasTube[Grid.PosToCell(this)] = false;
		Components.ITravelTubePieces.Remove(this);
		GameScenePartitioner.Instance.Free(ref this.dirtyNavCellUpdatedEntry);
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
		if (enable && !this.dirtyNavCellUpdatedEntry.IsValid())
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			this.dirtyNavCellUpdatedEntry = GameScenePartitioner.Instance.Add("TravelTube.OnDirtyNavCellUpdated", this, num, GameScenePartitioner.Instance.dirtyNavCellUpdateLayer, new Action<object>(this.OnDirtyNavCellUpdated));
			this.OnDirtyNavCellUpdated(null);
			return;
		}
		if (!enable && this.dirtyNavCellUpdatedEntry.IsValid())
		{
			GameScenePartitioner.Instance.Free(ref this.dirtyNavCellUpdatedEntry);
		}
	}

	private void OnDirtyNavCellUpdated(object data)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		int num2 = num * navGrid.maxLinksPerCell;
		bool flag = false;
		if (this.isExitTube)
		{
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
			return;
		}
		if (this.connectedStatus == Guid.Empty)
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

	private HandleVector<int>.Handle dirtyNavCellUpdatedEntry;

	private bool isExitTube;

	private bool hasValidExitTransitions;

	private UtilityConnections connections;

	private static readonly EventSystem.IntraObjectHandler<TravelTube> OnConnectionsChangedDelegate = new EventSystem.IntraObjectHandler<TravelTube>(delegate(TravelTube component, object data)
	{
		component.OnConnectionsChanged(data);
	});

	private Guid connectedStatus;

	private global::System.Action firstFrameCallback;
}
