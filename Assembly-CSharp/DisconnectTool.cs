using System;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectTool : FilteredDragTool
{
	public static void DestroyInstance()
	{
		DisconnectTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DisconnectTool.Instance = this;
		this.disconnectVisPool = new GameObjectPool(new Func<GameObject>(this.InstantiateDisconnectVis), this.singleDisconnectMode ? 1 : 10);
		if (this.singleDisconnectMode)
		{
			this.lineModeMaxLength = 2;
		}
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override DragTool.Mode GetMode()
	{
		if (!this.singleDisconnectMode)
		{
			return DragTool.Mode.Box;
		}
		return DragTool.Mode.Line;
	}

	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		if (this.singleDisconnectMode)
		{
			upPos = base.SnapToLine(upPos);
		}
		this.RunOnRegion(downPos, upPos, new Action<int, GameObject, IHaveUtilityNetworkMgr, UtilityConnections>(this.DisconnectCellsAction));
		this.ClearVisualizers();
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		this.lastRefreshedCell = -1;
	}

	private void DisconnectCellsAction(int cell, GameObject objectOnCell, IHaveUtilityNetworkMgr utilityComponent, UtilityConnections removeConnections)
	{
		Building component = objectOnCell.GetComponent<Building>();
		KAnimGraphTileVisualizer component2 = objectOnCell.GetComponent<KAnimGraphTileVisualizer>();
		if (component2 != null)
		{
			UtilityConnections utilityConnections = utilityComponent.GetNetworkManager().GetConnections(cell, false) & ~removeConnections;
			component2.UpdateConnections(utilityConnections);
			component2.Refresh();
		}
		TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
		utilityComponent.GetNetworkManager().ForceRebuildNetworks();
	}

	private void RunOnRegion(Vector3 pos1, Vector3 pos2, Action<int, GameObject, IHaveUtilityNetworkMgr, UtilityConnections> action)
	{
		Vector2 regularizedPos = base.GetRegularizedPos(Vector2.Min(pos1, pos2), true);
		Vector2 regularizedPos2 = base.GetRegularizedPos(Vector2.Max(pos1, pos2), false);
		Vector2I vector2I = new Vector2I((int)regularizedPos.x, (int)regularizedPos.y);
		Vector2I vector2I2 = new Vector2I((int)regularizedPos2.x, (int)regularizedPos2.y);
		for (int i = vector2I.x; i < vector2I2.x; i++)
		{
			for (int j = vector2I.y; j < vector2I2.y; j++)
			{
				int num = Grid.XYToCell(i, j);
				if (Grid.IsVisible(num))
				{
					for (int k = 0; k < 45; k++)
					{
						GameObject gameObject = Grid.Objects[num, k];
						if (!(gameObject == null))
						{
							string filterLayerFromGameObject = this.GetFilterLayerFromGameObject(gameObject);
							if (base.IsActiveLayer(filterLayerFromGameObject))
							{
								Building component = gameObject.GetComponent<Building>();
								if (!(component == null))
								{
									IHaveUtilityNetworkMgr component2 = component.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
									if (!component2.IsNullOrDestroyed())
									{
										UtilityConnections connections = component2.GetNetworkManager().GetConnections(num, false);
										UtilityConnections utilityConnections = (UtilityConnections)0;
										if ((connections & UtilityConnections.Left) > (UtilityConnections)0 && this.IsInsideRegion(vector2I, vector2I2, num, -1, 0))
										{
											utilityConnections |= UtilityConnections.Left;
										}
										if ((connections & UtilityConnections.Right) > (UtilityConnections)0 && this.IsInsideRegion(vector2I, vector2I2, num, 1, 0))
										{
											utilityConnections |= UtilityConnections.Right;
										}
										if ((connections & UtilityConnections.Up) > (UtilityConnections)0 && this.IsInsideRegion(vector2I, vector2I2, num, 0, 1))
										{
											utilityConnections |= UtilityConnections.Up;
										}
										if ((connections & UtilityConnections.Down) > (UtilityConnections)0 && this.IsInsideRegion(vector2I, vector2I2, num, 0, -1))
										{
											utilityConnections |= UtilityConnections.Down;
										}
										if (utilityConnections > (UtilityConnections)0)
										{
											action(num, gameObject, component2, utilityConnections);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private bool IsInsideRegion(Vector2I min, Vector2I max, int cell, int xoff, int yoff)
	{
		int num;
		int num2;
		Grid.CellToXY(Grid.OffsetCell(cell, xoff, yoff), out num, out num2);
		return num >= min.x && num < max.x && num2 >= min.y && num2 < max.y;
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		if (!base.Dragging)
		{
			return;
		}
		cursorPos = base.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		if (this.singleDisconnectMode)
		{
			cursorPos = base.SnapToLine(cursorPos);
		}
		int num = Grid.PosToCell(cursorPos);
		if (this.lastRefreshedCell == num)
		{
			return;
		}
		this.lastRefreshedCell = num;
		this.ClearVisualizers();
		this.RunOnRegion(this.downPos, cursorPos, new Action<int, GameObject, IHaveUtilityNetworkMgr, UtilityConnections>(this.VisualizeAction));
	}

	private GameObject InstantiateDisconnectVis()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.singleDisconnectMode ? this.disconnectVisSingleModePrefab : this.disconnectVisMultiModePrefab, Grid.SceneLayer.FXFront, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	private void VisualizeAction(int cell, GameObject objectOnCell, IHaveUtilityNetworkMgr utilityComponent, UtilityConnections removeConnections)
	{
		if ((removeConnections & UtilityConnections.Down) != (UtilityConnections)0)
		{
			this.CreateVisualizer(cell, Grid.CellBelow(cell), true);
		}
		if ((removeConnections & UtilityConnections.Right) != (UtilityConnections)0)
		{
			this.CreateVisualizer(cell, Grid.CellRight(cell), false);
		}
	}

	private void CreateVisualizer(int cell1, int cell2, bool rotate)
	{
		foreach (DisconnectTool.VisData visData in this.visualizersInUse)
		{
			if (visData.Equals(cell1, cell2))
			{
				return;
			}
		}
		Vector3 vector = Grid.CellToPosCCC(cell1, Grid.SceneLayer.FXFront);
		Vector3 vector2 = Grid.CellToPosCCC(cell2, Grid.SceneLayer.FXFront);
		GameObject instance = this.disconnectVisPool.GetInstance();
		instance.transform.rotation = Quaternion.Euler(0f, 0f, (float)(rotate ? 90 : 0));
		instance.transform.SetPosition(Vector3.Lerp(vector, vector2, 0.5f));
		instance.SetActive(true);
		this.visualizersInUse.Add(new DisconnectTool.VisData(cell1, cell2, instance));
	}

	private void ClearVisualizers()
	{
		foreach (DisconnectTool.VisData visData in this.visualizersInUse)
		{
			visData.go.SetActive(false);
			this.disconnectVisPool.ReleaseInstance(visData.go);
		}
		this.visualizersInUse.Clear();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		this.ClearVisualizers();
	}

	protected override string GetConfirmSound()
	{
		return "OutletDisconnected";
	}

	protected override string GetDragSound()
	{
		return "Tile_Drag_NegativeTool";
	}

	protected override void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.WIRES, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.BUILDINGS, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LOGIC, ToolParameterMenu.ToggleState.Off);
	}

	[SerializeField]
	private GameObject disconnectVisSingleModePrefab;

	[SerializeField]
	private GameObject disconnectVisMultiModePrefab;

	private GameObjectPool disconnectVisPool;

	private List<DisconnectTool.VisData> visualizersInUse = new List<DisconnectTool.VisData>();

	private int lastRefreshedCell;

	private bool singleDisconnectMode = true;

	public static DisconnectTool Instance;

	public struct VisData
	{
		public VisData(int cell1, int cell2, GameObject go)
		{
			this.cell1 = cell1;
			this.cell2 = cell2;
			this.go = go;
		}

		public bool Equals(int cell1, int cell2)
		{
			return (this.cell1 == cell1 && this.cell2 == cell2) || (this.cell1 == cell2 && this.cell2 == cell1);
		}

		public readonly int cell1;

		public readonly int cell2;

		public GameObject go;
	}
}
