using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class Conduit : KMonoBehaviour, IFirstFrameCallback, IHaveUtilityNetworkMgr, IBridgedNetworkItem, IDisconnectable, FlowUtilityNetwork.IItem
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

	protected override void OnPrefabInit()
	{
		base.Subscribe(-1201923725, new Action<object>(this.OnHighlighted));
		base.Subscribe(-700727624, new Action<object>(this.OnConduitFrozen));
		base.Subscribe(-1152799878, new Action<object>(this.OnConduitBoiling));
	}

	protected override void OnSpawn()
	{
		int num = Grid.PosToCell(this);
		this.GetNetworkManager().AddToNetworks(num, this, false);
		this.Connect();
		base.Subscribe(774203113, new Action<object>(this.OnBuildingBroken));
		base.Subscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, this);
		BuildingDef def = base.GetComponent<Building>().Def;
		if (def != null && def.ThermalConductivity != 1f)
		{
			ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
			flowVisualizer.AddThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(774203113, new Action<object>(this.OnBuildingBroken));
		base.Unsubscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		BuildingDef def = base.GetComponent<Building>().Def;
		if (def != null && def.ThermalConductivity != 1f)
		{
			ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
			flowVisualizer.RemoveThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		this.GetNetworkManager().RemoveFromNetworks(num, this, false);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			this.GetNetworkManager().RemoveFromNetworks(num, this, false);
			this.GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
		}
		base.OnCleanUp();
	}

	private ConduitFlowVisualizer GetFlowVisualizer()
	{
		return (this.type != ConduitType.Gas) ? Game.Instance.liquidFlowVisualizer : Game.Instance.gasFlowVisualizer;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return (this.type != ConduitType.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	public ConduitFlow GetFlowManager()
	{
		return (this.type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	public static ConduitFlow GetFlowManager(ConduitType type)
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	public static IUtilityNetworkMgr GetNetworkManager(ConduitType type)
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}

	private void OnHighlighted(object data)
	{
		bool flag = (bool)data;
		int num = ((!flag) ? (-1) : Grid.PosToCell(base.transform.GetPosition()));
		ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
		flowVisualizer.SetHighlightedCell(num);
	}

	private void OnConduitFrozen(object data)
	{
		base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_FROZE,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_FROZE
		});
		this.GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnConduitBoiling(object data)
	{
		base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_BOILED,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_BOILED
		});
		this.GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnBuildingBroken(object data)
	{
		this.Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		this.Connect();
	}

	public bool IsDisconnected()
	{
		return this.disconnected;
	}

	public bool Connect()
	{
		BuildingHP component = base.GetComponent<BuildingHP>();
		if (component == null || component.HitPoints > 0)
		{
			this.disconnected = false;
			this.GetNetworkManager().ForceRebuildNetworks();
		}
		return !this.disconnected;
	}

	public void Disconnect()
	{
		this.disconnected = true;
		this.GetNetworkManager().ForceRebuildNetworks();
	}

	public FlowUtilityNetwork Network
	{
		set
		{
		}
	}

	public int Cell
	{
		get
		{
			return Grid.PosToCell(this);
		}
	}

	public Endpoint EndpointType
	{
		get
		{
			return Endpoint.Conduit;
		}
	}

	public ConduitType ConduitType
	{
		get
		{
			return this.ConduitType;
		}
	}

	public GameObject GameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	[SerializeField]
	private bool disconnected = true;

	public ConduitType type;

	private global::System.Action firstFrameCallback;
}
