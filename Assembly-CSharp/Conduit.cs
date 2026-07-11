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
		base.OnPrefabInit();
		base.Subscribe<Conduit>(-1201923725, Conduit.OnHighlightedDelegate);
		base.Subscribe<Conduit>(-700727624, Conduit.OnConduitFrozenDelegate);
		base.Subscribe<Conduit>(-1152799878, Conduit.OnConduitBoilingDelegate);
		base.Subscribe<Conduit>(-1555603773, Conduit.OnStructureTemperatureRegisteredDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Conduit>(774203113, Conduit.OnBuildingBrokenDelegate);
		base.Subscribe<Conduit>(-1735440190, Conduit.OnBuildingFullyRepairedDelegate);
	}

	private void OnStructureTemperatureRegistered(object data)
	{
		int num = Grid.PosToCell(this);
		this.GetNetworkManager().AddToNetworks(num, this, false);
		this.Connect();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, this);
		BuildingDef def = base.GetComponent<Building>().Def;
		if (def != null && def.ThermalConductivity != 1f)
		{
			this.GetFlowVisualizer().AddThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
		}
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe<Conduit>(774203113, Conduit.OnBuildingBrokenDelegate, false);
		base.Unsubscribe<Conduit>(-1735440190, Conduit.OnBuildingFullyRepairedDelegate, false);
		BuildingDef def = base.GetComponent<Building>().Def;
		if (def != null && def.ThermalConductivity != 1f)
		{
			this.GetFlowVisualizer().RemoveThermalConductivity(Grid.PosToCell(base.transform.GetPosition()), def.ThermalConductivity);
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
		if (this.type != ConduitType.Gas)
		{
			return Game.Instance.liquidFlowVisualizer;
		}
		return Game.Instance.gasFlowVisualizer;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		if (this.type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitSystem;
		}
		return Game.Instance.gasConduitSystem;
	}

	public ConduitFlow GetFlowManager()
	{
		if (this.type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitFlow;
		}
		return Game.Instance.gasConduitFlow;
	}

	public static ConduitFlow GetFlowManager(ConduitType type)
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitFlow;
		}
		return Game.Instance.gasConduitFlow;
	}

	public static IUtilityNetworkMgr GetNetworkManager(ConduitType type)
	{
		if (type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitSystem;
		}
		return Game.Instance.gasConduitSystem;
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
		int num = (((bool)data) ? Grid.PosToCell(base.transform.GetPosition()) : (-1));
		this.GetFlowVisualizer().SetHighlightedCell(num);
	}

	private void OnConduitFrozen(object data)
	{
		base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_FROZE,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_FROZE,
			takeDamageEffect = ((this.ConduitType == ConduitType.Gas) ? SpawnFXHashes.BuildingLeakLiquid : SpawnFXHashes.BuildingFreeze),
			fullDamageEffectName = ((this.ConduitType == ConduitType.Gas) ? "water_damage_kanim" : "ice_damage_kanim")
		});
		this.GetFlowManager().EmptyConduit(Grid.PosToCell(base.transform.GetPosition()));
	}

	private void OnConduitBoiling(object data)
	{
		base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_BOILED,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_BOILED,
			takeDamageEffect = SpawnFXHashes.BuildingLeakGas,
			fullDamageEffectName = "gas_damage_kanim"
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
			return this.type;
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

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnHighlightedDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnHighlighted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitFrozenDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnConduitFrozen(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitBoilingDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnConduitBoiling(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnStructureTemperatureRegisteredDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnStructureTemperatureRegistered(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Conduit>(delegate(Conduit component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});
}
