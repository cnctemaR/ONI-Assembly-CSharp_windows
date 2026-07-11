using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimGraphTileVisualizer")]
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
			base.Trigger(-1041684577, this._connections);
		}
	}

	public IUtilityNetworkMgr ConnectionManager
	{
		get
		{
			switch (this.connectionSource)
			{
			case KAnimGraphTileVisualizer.ConnectionSource.Gas:
				return Game.Instance.gasConduitSystem;
			case KAnimGraphTileVisualizer.ConnectionSource.Liquid:
				return Game.Instance.liquidConduitSystem;
			case KAnimGraphTileVisualizer.ConnectionSource.Electrical:
				return Game.Instance.electricalConduitSystem;
			case KAnimGraphTileVisualizer.ConnectionSource.Logic:
				return Game.Instance.logicCircuitSystem;
			case KAnimGraphTileVisualizer.ConnectionSource.Tube:
				return Game.Instance.travelTubeSystem;
			case KAnimGraphTileVisualizer.ConnectionSource.Solid:
				return Game.Instance.solidConduitSystem;
			default:
				return null;
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.connectionManager = this.ConnectionManager;
		int num = Grid.PosToCell(base.transform.GetPosition());
		this.connectionManager.SetConnections(this.Connections, num, this.isPhysicalBuilding);
		Building component = base.GetComponent<Building>();
		TileVisualizer.RefreshCell(num, component.Def.TileLayer, component.Def.ReplacementLayer);
	}

	protected override void OnCleanUp()
	{
		if (this.connectionManager != null && !this.skipCleanup)
		{
			this.skipRefresh = true;
			int num = Grid.PosToCell(base.transform.GetPosition());
			this.connectionManager.ClearCell(num, this.isPhysicalBuilding);
			Building component = base.GetComponent<Building>();
			TileVisualizer.RefreshCell(num, component.Def.TileLayer, component.Def.ReplacementLayer);
		}
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (this.connectionManager == null || this.skipRefresh)
		{
			return;
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		this.Connections = this.connectionManager.GetConnections(num, this.isPhysicalBuilding);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			string text = this.connectionManager.GetVisualizerString(num);
			if (base.GetComponent<BuildingUnderConstruction>() != null && component.HasAnimation(text + "_place"))
			{
				text += "_place";
			}
			if (text != null && text != "")
			{
				component.Play(text, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	public int GetNetworkID()
	{
		UtilityNetwork network = this.GetNetwork();
		if (network == null)
		{
			return -1;
		}
		return network.id;
	}

	private UtilityNetwork GetNetwork()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return this.connectionManager.GetNetworkForDirection(num, Direction.None);
	}

	public UtilityNetwork GetNetworkForDirection(Direction d)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return this.connectionManager.GetNetworkForDirection(num, d);
	}

	public void UpdateConnections(UtilityConnections new_connections)
	{
		this._connections = new_connections;
		if (this.connectionManager != null)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			this.connectionManager.SetConnections(new_connections, num, this.isPhysicalBuilding);
		}
	}

	public KAnimGraphTileVisualizer GetNeighbour(Direction d)
	{
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = null;
		Vector2I vector2I;
		Grid.PosToXY(base.transform.GetPosition(), out vector2I);
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
				objectLayer = ObjectLayer.LogicWireTile;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Tube:
				objectLayer = ObjectLayer.TravelTubeTile;
				break;
			case KAnimGraphTileVisualizer.ConnectionSource.Solid:
				objectLayer = ObjectLayer.SolidConduitTile;
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
	private UtilityConnections _connections;

	public bool isPhysicalBuilding;

	public bool skipCleanup;

	public bool skipRefresh;

	public KAnimGraphTileVisualizer.ConnectionSource connectionSource;

	[NonSerialized]
	public IUtilityNetworkMgr connectionManager;

	public enum ConnectionSource
	{
		Gas,
		Liquid,
		Electrical,
		Logic,
		Tube,
		Solid
	}
}
