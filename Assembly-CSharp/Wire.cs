using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Wire : KMonoBehaviour, IDisconnectable, IFirstFrameCallback, IWattageRating, IHaveUtilityNetworkMgr, IUtilityNetworkItem, IBridgedNetworkItem
{
	public static float GetMaxWattageAsFloat(Wire.WattageRating rating)
	{
		switch (rating)
		{
		case Wire.WattageRating.Max500:
			return 500f;
		case Wire.WattageRating.Max1000:
			return 1000f;
		case Wire.WattageRating.Max2000:
			return 2000f;
		case Wire.WattageRating.Max20000:
			return 20000f;
		case Wire.WattageRating.Max50000:
			return 50000f;
		default:
			return 0f;
		}
	}

	public bool IsConnected
	{
		get
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			return electricalUtilityNetwork != null;
		}
	}

	public ushort NetworkID
	{
		get
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			return (electricalUtilityNetwork == null) ? ushort.MaxValue : ((ushort)electricalUtilityNetwork.id);
		}
	}

	protected override void OnSpawn()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.electricalConduitSystem.AddToNetworks(num, this, false);
		this.InitializeSwitchState();
		base.Subscribe<Wire>(774203113, Wire.OnBuildingBrokenDelegate);
		base.Subscribe<Wire>(-1735440190, Wire.OnBuildingFullyRepairedDelegate);
		base.GetComponent<KSelectable>().AddStatusItem(Wire.WireMaxWattageStatus, this);
		base.GetComponent<KSelectable>().AddStatusItem(Wire.WireCircuitStatus, this);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity(Wire.OutlineSymbol, false);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.electricalConduitSystem.RemoveFromNetworks(num, this, false);
		}
		base.Unsubscribe<Wire>(774203113, Wire.OnBuildingBrokenDelegate, false);
		base.Unsubscribe<Wire>(-1735440190, Wire.OnBuildingFullyRepairedDelegate, false);
		base.OnCleanUp();
	}

	private void InitializeSwitchState()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		bool flag = false;
		GameObject gameObject = Grid.Objects[num, 1];
		if (gameObject != null)
		{
			CircuitSwitch component = gameObject.GetComponent<CircuitSwitch>();
			if (component != null)
			{
				flag = true;
				component.AttachWire(this);
			}
		}
		if (!flag)
		{
			this.Connect();
		}
	}

	public UtilityConnections GetWireConnections()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Game.Instance.electricalConduitSystem.GetConnections(num, true);
	}

	public string GetWireConnectionsString()
	{
		UtilityConnections wireConnections = this.GetWireConnections();
		return Game.Instance.electricalConduitSystem.GetVisualizerString(wireConnections);
	}

	private void OnBuildingBroken(object data)
	{
		this.Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		this.InitializeSwitchState();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (Wire.WireCircuitStatus == null)
		{
			Wire.WireCircuitStatus = new StatusItem("WireCircuitStatus", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				Wire wire = (Wire)data;
				int num = Grid.PosToCell(wire.transform.GetPosition());
				CircuitManager circuitManager = Game.Instance.circuitManager;
				ushort circuitID = circuitManager.GetCircuitID(num);
				float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitID);
				float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
				GameUtil.WattageFormatterUnit wattageFormatterUnit = GameUtil.WattageFormatterUnit.Watts;
				if (wire.MaxWattageRating == Wire.WattageRating.Max20000)
				{
					wattageFormatterUnit = GameUtil.WattageFormatterUnit.Kilowatts;
				}
				str = str.Replace("{CurrentLoad}", GameUtil.GetFormattedWattage(wattsUsedByCircuit, wattageFormatterUnit));
				str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(wattsNeededWhenActive, wattageFormatterUnit));
				return str;
			});
		}
		if (Wire.WireMaxWattageStatus == null)
		{
			Wire.WireMaxWattageStatus = new StatusItem("WireMaxWattageStatus", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				Wire wire2 = (Wire)data;
				GameUtil.WattageFormatterUnit wattageFormatterUnit2 = GameUtil.WattageFormatterUnit.Watts;
				if (wire2.MaxWattageRating == Wire.WattageRating.Max20000)
				{
					wattageFormatterUnit2 = GameUtil.WattageFormatterUnit.Kilowatts;
				}
				str = str.Replace("{WireMaxWattage}", GameUtil.GetFormattedWattage(Wire.GetMaxWattageAsFloat(wire2.MaxWattageRating), wattageFormatterUnit2));
				return str;
			});
		}
	}

	public Wire.WattageRating GetMaxWattageRating()
	{
		return this.MaxWattageRating;
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
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
			Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
		}
		return !this.disconnected;
	}

	public void Disconnect()
	{
		this.disconnected = true;
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected, null);
		Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
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

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.electricalConduitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(num);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(num);
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}

	[SerializeField]
	public Wire.WattageRating MaxWattageRating;

	[SerializeField]
	private bool disconnected = true;

	public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");

	public float circuitOverloadTime;

	private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Wire>(delegate(Wire component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Wire>(delegate(Wire component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private static StatusItem WireCircuitStatus = null;

	private static StatusItem WireMaxWattageStatus = null;

	private global::System.Action firstFrameCallback;

	public enum WattageRating
	{
		Max500,
		Max1000,
		Max2000,
		Max20000,
		Max50000,
		NumRatings
	}
}
