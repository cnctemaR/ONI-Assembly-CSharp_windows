using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/LogicWire")]
public class LogicWire : KMonoBehaviour, IFirstFrameCallback, IHaveUtilityNetworkMgr, IBridgedNetworkItem, IBitRating, IDisconnectable
{
	public static int GetBitDepthAsInt(LogicWire.BitDepth rating)
	{
		if (rating == LogicWire.BitDepth.OneBit)
		{
			return 1;
		}
		if (rating != LogicWire.BitDepth.FourBit)
		{
			return 0;
		}
		return 4;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.logicCircuitSystem.AddToNetworks(num, this, false);
		base.Subscribe<LogicWire>(774203113, LogicWire.OnBuildingBrokenDelegate);
		base.Subscribe<LogicWire>(-1735440190, LogicWire.OnBuildingFullyRepairedDelegate);
		this.Connect();
		base.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(LogicWire.OutlineSymbol, false);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			Game.Instance.logicCircuitSystem.RemoveFromNetworks(num, this, false);
		}
		base.Unsubscribe<LogicWire>(774203113, LogicWire.OnBuildingBrokenDelegate, false);
		base.Unsubscribe<LogicWire>(-1735440190, LogicWire.OnBuildingFullyRepairedDelegate, false);
		base.OnCleanUp();
	}

	public bool IsConnected
	{
		get
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			return Game.Instance.logicCircuitSystem.GetNetworkForCell(num) is LogicCircuitNetwork;
		}
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
			Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
		}
		return !this.disconnected;
	}

	public void Disconnect()
	{
		this.disconnected = true;
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected, null);
		Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
	}

	public UtilityConnections GetWireConnections()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Game.Instance.logicCircuitSystem.GetConnections(num, true);
	}

	public string GetWireConnectionsString()
	{
		UtilityConnections wireConnections = this.GetWireConnections();
		return Game.Instance.logicCircuitSystem.GetVisualizerString(wireConnections);
	}

	private void OnBuildingBroken(object data)
	{
		this.Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		this.Connect();
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

	public LogicWire.BitDepth GetMaxBitRating()
	{
		return this.MaxBitDepth;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.logicCircuitSystem;
	}

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(num);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(num);
		return networks.Contains(networkForCell);
	}

	public int GetNetworkCell()
	{
		return Grid.PosToCell(this);
	}

	[SerializeField]
	public LogicWire.BitDepth MaxBitDepth;

	[SerializeField]
	private bool disconnected = true;

	public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");

	private static readonly EventSystem.IntraObjectHandler<LogicWire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicWire>(delegate(LogicWire component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicWire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicWire>(delegate(LogicWire component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private global::System.Action firstFrameCallback;

	public enum BitDepth
	{
		OneBit,
		FourBit,
		NumRatings
	}
}
