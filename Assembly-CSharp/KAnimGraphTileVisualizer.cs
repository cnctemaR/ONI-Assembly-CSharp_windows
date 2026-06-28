using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KAnimGraphTileVisualizer : KMonoBehaviour, ISaveLoadable, IUtilityItem
{
	public UtilityConnections Connections
	{
		get
		{
			return this._connections;
		}
		set
		{
			this._connections = value;
		}
	}

	public IUtilityNetworkMgr ConnectionManager
	{
		get
		{
			IUtilityNetworkMgr utilityNetworkMgr;
			switch (this.connectionSource)
			{
			case KAnimGraphTileVisualizer.ConnectionSource.Gas:
				utilityNetworkMgr = Game.Instance.gasConduitSystem;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Liquid:
				utilityNetworkMgr = Game.Instance.liquidConduitSystem;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Electrical:
				utilityNetworkMgr = Game.Instance.electricalConduitSystem;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Logic:
				utilityNetworkMgr = Game.Instance.logicCircuitSystem;
				break;
			default:
				utilityNetworkMgr = null;
				break;
			}
			return utilityNetworkMgr;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.connectionManager = this.ConnectionManager;
		int num = Grid.PosToCell(base.transform.position);
		this.connectionManager.SetConnections(this.Connections, num, this.isPhysicalBuilding);
		Building component = base.GetComponent<Building>();
		TileVisualizer.RefreshCell(num, component.Def.TileLayer);
	}

	protected override void OnCleanUp()
	{
		if (this.connectionManager != null && !this.skipCleanup)
		{
			this.skipRefresh = true;
			int num = Grid.PosToCell(base.transform.position);
			this.connectionManager.ClearCell(num, this.isPhysicalBuilding);
			Building component = base.GetComponent<Building>();
			TileVisualizer.RefreshCell(num, component.Def.TileLayer);
		}
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (this.connectionManager != null && !this.skipRefresh)
		{
			int num = Grid.PosToCell(base.transform.position);
			this.Connections = this.connectionManager.GetConnections(num, this.isPhysicalBuilding);
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				string text = this.connectionManager.GetVisualizerString(num);
				BuildingUnderConstruction component2 = base.GetComponent<BuildingUnderConstruction>();
				if (component2 != null && component.HasAnimation(text + "_place"))
				{
					text += "_place";
				}
				if (text != null && text != "")
				{
					component.Play(text, KAnim.PlayMode.Once, 1f, 0f);
				}
			}
		}
	}

	public int GetNetworkID()
	{
		UtilityNetwork network = this.GetNetwork();
		return (network == null) ? (-1) : network.id;
	}

	private UtilityNetwork GetNetwork()
	{
		int num = Grid.PosToCell(base.transform.position);
		return this.connectionManager.GetNetworkForDirection(num, Direction.None);
	}

	public UtilityNetwork GetNetworkForDirection(Direction d)
	{
		int num = Grid.PosToCell(base.transform.position);
		return this.connectionManager.GetNetworkForDirection(num, d);
	}

	public void UpdateConnections(UtilityConnections new_connections)
	{
		this._connections = new_connections;
		int num = Grid.PosToCell(base.transform.position);
		this.connectionManager.SetConnections(new_connections, num, this.isPhysicalBuilding);
		base.Trigger(-1041684577, new_connections);
	}

	public KAnimGraphTileVisualizer GetNeighbour(Direction d)
	{
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = null;
		Vector2I vector2I;
		Grid.PosToXY(base.transform.position, out vector2I);
		int num = -1;
		switch (d)
		{
		case Direction.Up:
			if (vector2I.y < Grid.HeightInCells - 1)
			{
				num = Grid.XYToCell(vector2I.x, vector2I.y + 1);
			}
			break;
		case Direction.Right:
			if (vector2I.x < Grid.WidthInCells - 1)
			{
				num = Grid.XYToCell(vector2I.x + 1, vector2I.y);
			}
			break;
		case Direction.Down:
			if (vector2I.y > 0)
			{
				num = Grid.XYToCell(vector2I.x, vector2I.y - 1);
			}
			break;
		case Direction.Left:
			if (vector2I.x > 0)
			{
				num = Grid.XYToCell(vector2I.x - 1, vector2I.y);
			}
			break;
		}
		if (num != -1)
		{
			ObjectLayer objectLayer;
			switch (this.connectionSource)
			{
			case KAnimGraphTileVisualizer.ConnectionSource.Gas:
				objectLayer = ObjectLayer.GasConduitTile;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Liquid:
				objectLayer = ObjectLayer.LiquidConduitTile;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Electrical:
				objectLayer = ObjectLayer.WireTile;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Logic:
				objectLayer = ObjectLayer.LogicWiresTiling;
				break;
			default:
				throw new ArgumentNullException("wtf");
			}
			GameObject gameObject = Grid.Objects[num, (int)objectLayer];
			if (gameObject != null)
			{
				kanimGraphTileVisualizer = gameObject.GetComponent<KAnimGraphTileVisualizer>();
			}
		}
		return kanimGraphTileVisualizer;
	}

	[Serialize]
	private UtilityConnections _connections = (UtilityConnections)0;

	public bool isPhysicalBuilding;

	public bool skipCleanup = false;

	public bool skipRefresh = false;

	public KAnimGraphTileVisualizer.ConnectionSource connectionSource;

	[NonSerialized]
	public IUtilityNetworkMgr connectionManager = null;

	public enum ConnectionSource
	{
		Gas,
		Liquid,
		Electrical,
		Logic
	}
}
