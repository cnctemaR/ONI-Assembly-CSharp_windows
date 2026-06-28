using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Wire : KMonoBehaviour, IDisconnectable
{
	public static float GetMaxWattageAsFloat(Wire.WattageRating rating)
	{
		switch (rating)
		{
		case Wire.WattageRating.Max500:
			return 500f;
		case Wire.WattageRating.Max1000:
			return 1000f;
		case Wire.WattageRating.Max20000:
			return 20000f;
		default:
			return 0f;
		}
	}

	public bool IsConnected
	{
		get
		{
			int num = Grid.PosToCell(this.transform.position);
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			return electricalUtilityNetwork != null;
		}
	}

	public ushort NetworkID
	{
		get
		{
			int num = Grid.PosToCell(this.transform.position);
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			if (electricalUtilityNetwork == null)
			{
				return ushort.MaxValue;
			}
			return (ushort)electricalUtilityNetwork.id;
		}
	}

	protected override void OnSpawn()
	{
		int num = Grid.PosToCell(this.transform.position);
		Game.Instance.electricalConduitSystem.AddToNetworks(num, this, false);
		this.InitializeSwitchState();
		this.Subscribe(774203113, new Action<object>(this.OnBuildingBroken));
		this.Subscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		base.GetComponent<KSelectable>().AddStatusItem(Wire.WireMaxWattageStatus, this);
		base.GetComponent<KSelectable>().AddStatusItem(Wire.WireCircuitStatus, this);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.HideSymbol(true, Wire.OutlineSymbol);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(this.transform.position);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.electricalConduitSystem.RemoveFromNetworks(num, this, false);
		}
		this.Unsubscribe(774203113, new Action<object>(this.OnBuildingBroken));
		this.Unsubscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		base.OnCleanUp();
	}

	public bool IsDisconnected()
	{
		return this.disconnected;
	}

	private void InitializeSwitchState()
	{
		int num = Grid.PosToCell(this.transform.position);
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

	public bool Connect()
	{
		BuildingHP component = base.GetComponent<BuildingHP>();
		if (component.HitPoints > 0)
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

	public UtilityConnections GetWireConnections()
	{
		int num = Grid.PosToCell(this.transform.position);
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

	public bool IsBroken
	{
		get
		{
			return this.buildingHP.IsBroken;
		}
	}

	[SerializeField]
	public Wire.WattageRating MaxWattageRating;

	[SerializeField]
	private bool disconnected = true;

	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	[MyCmpReq]
	private BuildingHP buildingHP;

	public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");

	private static StatusItem WireCircuitStatus = new StatusItem("WireCircuitStatus", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true).SetResolveStringCallback(delegate(string str, object data)
	{
		Wire wire = (Wire)data;
		int num = Grid.PosToCell(wire.transform.position);
		CircuitManager circuitManager = Game.Instance.circuitManager;
		ushort circuitID = circuitManager.GetCircuitID(num);
		float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitID);
		float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
		str = str.Replace("{CurrentLoad}", GameUtil.GetFormattedWattage(wattsUsedByCircuit, "F1"));
		str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(wattsNeededWhenActive, "F1"));
		return str;
	});

	private static StatusItem WireMaxWattageStatus = new StatusItem("WireMaxWattageStatus", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true).SetResolveStringCallback(delegate(string str, object data)
	{
		Wire wire2 = (Wire)data;
		str = str.Replace("{WireMaxWattage}", GameUtil.GetFormattedWattage(Wire.GetMaxWattageAsFloat(wire2.MaxWattageRating), "F1"));
		return str;
	});

	public enum WattageRating
	{
		Max500,
		Max1000,
		Max20000,
		NumRatings
	}
}
