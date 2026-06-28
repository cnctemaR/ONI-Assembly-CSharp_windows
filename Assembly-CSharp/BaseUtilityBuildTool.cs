using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class BaseUtilityBuildTool : DragTool
{
	protected override void OnPrefabInit()
	{
		this.buildingCount = global::UnityEngine.Random.Range(1, 14);
	}

	private void Play(GameObject go, string anim)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		int num = LayerMask.NameToLayer("Place");
		this.visualizer = GameUtil.KInstantiate(this.def.BuildingPreview, Grid.SceneLayer.Use, Folder.Placers, null, num);
		KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
		}
		this.visualizer.SetActive(true);
		this.Play(this.visualizer, "None_Place");
		BuildToolHoverTextCard component2 = base.GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = this.def;
		component2.ConfigureHoverScreen();
		component2.UpdateHoverElements(null);
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(this.visualizer);
		Wire component3 = this.def.BuildingComplete.GetComponent<Wire>();
		if (component3 != null)
		{
			this.conduitMgr = Game.Instance.electricalConduitSystem;
		}
		else
		{
			this.conduit = this.def.BuildingComplete.GetComponent<Conduit>();
			this.conduitMgr = this.conduit.GetNetworkManager();
		}
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		this.StopVisUpdater();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		if (this.visualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.visualizer);
		}
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(BuildingDef def, IList<Element> selected_elements)
	{
		this.selectedElements = selected_elements;
		this.def = def;
		this.viewMode = def.ViewMode;
		PlayerController.Instance.ActivateTool(this);
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
		base.GetComponent<BuildToolHoverTextCard>().UpdateHoverElements(null);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (this.path.Count == 0 || this.path[this.path.Count - 1].cell == cell)
		{
			return;
		}
		this.placeSound = GlobalAssets.GetSound("Place_building_" + this.def.AudioSize, false);
		Vector3 vector = Grid.CellToPos(cell);
		EventInstance eventInstance = SoundEvent.BeginOneShot(this.placeSound, vector);
		if (this.path.Count > 1 && cell == this.path[this.path.Count - 2].cell)
		{
			if (this.previousCellConnection != null)
			{
				this.previousCellConnection.ConnectedEvent(this.previousCell);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("OutletDisconnected", false));
				this.previousCellConnection = null;
			}
			this.previousCell = cell;
			if (!this.CheckForConnection(cell, "Wire", string.Empty, ref this.previousCellConnection, false))
			{
				this.CheckForConnection(cell, "Pipe", string.Empty, ref this.previousCellConnection, false);
			}
			global::UnityEngine.Object.Destroy(this.path[this.path.Count - 1].visualizer);
			TileVisualizer.RefreshCell(this.path[this.path.Count - 1].cell, this.def.TileLayer);
			this.path.RemoveAt(this.path.Count - 1);
			this.buildingCount = ((this.buildingCount != 1) ? (this.buildingCount - 1) : (this.buildingCount = 14));
			eventInstance.setParameterValue("tileCount", (float)this.buildingCount);
			SoundEvent.EndOneShot(eventInstance);
		}
		else if (!this.path.Exists((BaseUtilityBuildTool.PathNode n) => n.cell == cell))
		{
			this.path.Add(new BaseUtilityBuildTool.PathNode
			{
				cell = cell,
				visualizer = null
			});
			if (!this.CheckForConnection(cell, "Wire", "OutletConnected", ref this.previousCellConnection, true))
			{
				this.CheckForConnection(cell, "Pipe", "OutletConnected", ref this.previousCellConnection, true);
			}
			else
			{
				this.previousCell = cell;
			}
			this.buildingCount = this.buildingCount % 14 + 1;
			eventInstance.setParameterValue("tileCount", (float)this.buildingCount);
			SoundEvent.EndOneShot(eventInstance);
		}
		this.visualizer.SetActive(this.path.Count < 2);
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(this.path.Count);
	}

	private bool CheckForConnection(int cell, string defName, string soundName, ref BuildingCellVisualizer outBcv, bool fireEvents = true)
	{
		if (this.def.Name.Contains(defName))
		{
			Building building = this.GetBuilding(cell);
			if (building != null)
			{
				bool flag = defName.Contains("Wire");
				int num = ((!flag) ? building.GetUtilityInputCell() : building.GetPowerInputCell());
				int num2 = ((!flag) ? building.GetUtilityOutputCell() : num);
				if (cell == num || cell == num2)
				{
					BuildingCellVisualizer component = building.gameObject.GetComponent<BuildingCellVisualizer>();
					outBcv = component;
					if (component != null)
					{
						bool flag2 = false;
						if ((flag && component.RequiresPower) || (this.conduit != null && ((component.RequiresGas && this.conduit.type == ConduitType.Gas) || (component.RequiresLiquid && this.conduit.type == ConduitType.Liquid))))
						{
							flag2 = true;
						}
						if (flag2)
						{
							if (fireEvents)
							{
								component.ConnectedEvent(cell);
								string sound = GlobalAssets.GetSound(soundName, false);
								if (sound != null)
								{
									KMonoBehaviour.PlaySound(sound);
								}
							}
							return true;
						}
					}
				}
			}
		}
		outBcv = null;
		return false;
	}

	private Building GetBuilding(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			return gameObject.GetComponent<Building>();
		}
		return null;
	}

	private UtilityConnections GetConnectionDirection(int prevCell, int cell)
	{
		if (prevCell + 1 == cell)
		{
			return UtilityConnections.Right;
		}
		if (prevCell - 1 == cell)
		{
			return UtilityConnections.Left;
		}
		if (prevCell + Grid.WidthInCells == cell)
		{
			return UtilityConnections.Up;
		}
		if (prevCell - Grid.WidthInCells == cell)
		{
			return UtilityConnections.Down;
		}
		return (UtilityConnections)0;
	}

	protected override DragTool.Mode GetMode()
	{
		return DragTool.Mode.Brush;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		this.path.Clear();
		int num = Grid.PosToCell(cursor_pos);
		if (Grid.Visible[num] > 0 || PropertyTextures.FogOfWarScale == 1f)
		{
			this.path.Add(new BaseUtilityBuildTool.PathNode
			{
				cell = num,
				visualizer = null
			});
			if (!this.CheckForConnection(num, "Wire", "OutletConnected", ref this.previousCellConnection, true))
			{
				this.CheckForConnection(num, "Pipe", "OutletConnected", ref this.previousCellConnection, true);
			}
		}
		this.visUpdater = base.StartCoroutine(this.VisUpdater());
		this.visualizer.GetComponent<KBatchedAnimController>().StopAndClear();
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(1);
		this.placeSound = GlobalAssets.GetSound("Place_building_" + this.def.AudioSize, false);
		if (this.placeSound != null)
		{
			this.buildingCount = this.buildingCount % 14 + 1;
			Vector3 vector = Grid.CellToPos(num);
			EventInstance eventInstance = SoundEvent.BeginOneShot(this.placeSound, vector);
			if (this.def.AudioSize == "small")
			{
				eventInstance.setParameterValue("tileCount", (float)this.buildingCount);
			}
			SoundEvent.EndOneShot(eventInstance);
		}
		base.OnLeftClickDown(cursor_pos);
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		this.BuildPath();
		this.StopVisUpdater();
		this.Play(this.visualizer, "None_Place");
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
		base.OnLeftClickUp(cursor_pos);
	}

	protected UtilityConnections GetDirection(int start_cell, int end_cell)
	{
		if (end_cell == start_cell - 1)
		{
			return UtilityConnections.Left;
		}
		if (end_cell == start_cell + 1)
		{
			return UtilityConnections.Right;
		}
		if (end_cell == start_cell + Grid.WidthInCells)
		{
			return UtilityConnections.Up;
		}
		if (end_cell == start_cell - Grid.WidthInCells)
		{
			return UtilityConnections.Down;
		}
		return (UtilityConnections)0;
	}

	protected UtilityConnections GetOppositeDirection(UtilityConnections dir)
	{
		switch (dir)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Right;
		case UtilityConnections.Right:
			return UtilityConnections.Left;
		case UtilityConnections.Up:
			return UtilityConnections.Down;
		case UtilityConnections.Down:
			return UtilityConnections.Up;
		}
		return (UtilityConnections)0;
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		int num = Grid.PosToCell(cursorPos);
		if (this.lastCell != num)
		{
			this.lastCell = num;
		}
	}

	protected virtual void ApplyPathToConduitSystem()
	{
		if (this.path.Count < 2)
		{
			return;
		}
		for (int i = 1; i < this.path.Count; i++)
		{
			int cell = this.path[i - 1].cell;
			int cell2 = this.path[i].cell;
			UtilityConnections direction = this.GetDirection(cell, this.path[i].cell);
			UtilityConnections oppositeDirection = this.GetOppositeDirection(direction);
			UtilityConnections connections = this.conduitMgr.GetConnections(cell, false);
			UtilityConnections connections2 = this.conduitMgr.GetConnections(cell2, false);
			if (this.ShouldLink(direction, connections) && this.ShouldLink(oppositeDirection, connections2))
			{
				this.conduitMgr.AddConnection(direction, cell, false);
				this.conduitMgr.AddConnection(oppositeDirection, cell2, false);
			}
		}
	}

	private bool ShouldLink(UtilityConnections direction, UtilityConnections existing_connections)
	{
		UtilityConnections utilityConnections = UtilityConnections.Left | UtilityConnections.Right;
		UtilityConnections utilityConnections2 = UtilityConnections.Up | UtilityConnections.Down;
		existing_connections |= direction;
		bool flag = ((existing_connections & utilityConnections) == utilityConnections && (existing_connections & utilityConnections2) != (UtilityConnections)0) || ((existing_connections & utilityConnections2) == utilityConnections2 && (existing_connections & utilityConnections) != (UtilityConnections)0);
		return !flag;
	}

	private IEnumerator VisUpdater()
	{
		for (;;)
		{
			this.conduitMgr.StashVisualGrids();
			if (this.path.Count == 1)
			{
				BaseUtilityBuildTool.PathNode node = this.path[0];
				this.path[0] = this.CreateVisualizer(node);
			}
			this.ApplyPathToConduitSystem();
			for (int i = 0; i < this.path.Count; i++)
			{
				BaseUtilityBuildTool.PathNode node2 = this.path[i];
				node2 = this.CreateVisualizer(node2);
				this.path[i] = node2;
				string vis_string = this.conduitMgr.GetVisualizerString(node2.cell) + "_place";
				KBatchedAnimController kbac = node2.visualizer.GetComponent<KBatchedAnimController>();
				if (kbac.HasAnimation(vis_string))
				{
					node2.Play(vis_string);
				}
				else
				{
					node2.Play(this.conduitMgr.GetVisualizerString(node2.cell));
				}
				string reason;
				kbac.TintColour = ((!this.def.IsValidBuildLocation(null, node2.cell, Orientation.Neutral, out reason)) ? Color.red : Color.white);
				TileVisualizer.RefreshCell(node2.cell, this.def.TileLayer);
			}
			this.conduitMgr.UnstashVisualGrids();
			yield return null;
		}
		yield break;
	}

	private void BuildPath()
	{
		this.ApplyPathToConduitSystem();
		int num = 0;
		for (int i = 0; i < this.path.Count; i++)
		{
			BaseUtilityBuildTool.PathNode pathNode = this.path[i];
			Vector3 vector = Grid.CellToPosCBC(pathNode.cell, Grid.SceneLayer.Building);
			UtilityConnections utilityConnections = (UtilityConnections)0;
			GameObject gameObject = Grid.Objects[pathNode.cell, (int)this.def.TileLayer];
			if (gameObject == null)
			{
				utilityConnections = this.conduitMgr.GetConnections(pathNode.cell, false);
				if (DebugHandler.InstantBuildMode && this.def.IsValidBuildLocation(this.visualizer, vector, Orientation.Neutral) && this.def.IsValidPlaceLocation(this.visualizer, vector, Orientation.Neutral))
				{
					gameObject = this.def.Build(pathNode.cell, Orientation.Neutral, null, this.selectedElements, 293.15f, false, true);
				}
				else
				{
					gameObject = this.def.TryPlace(vector, Orientation.Neutral, this.selectedElements, 0, false);
					if (gameObject != null)
					{
						Constructable component = gameObject.GetComponent<Constructable>();
						if (component.IconConnectionAnimation(0.1f * (float)num, num, "Wire", "OutletConnected_release") || component.IconConnectionAnimation(0.1f * (float)num, num, "Pipe", "OutletConnected_release"))
						{
							num++;
						}
						Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
						if (component2 != null)
						{
							component2.SetMasterPriority(BuildMenuPriorityScreen.Instance.GetScreenPriority());
						}
					}
				}
			}
			else
			{
				IUtilityItem component3 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component3 != null)
				{
					utilityConnections = component3.Connections;
				}
				utilityConnections |= this.conduitMgr.GetConnections(pathNode.cell, false);
				if (gameObject.GetComponent<BuildingComplete>() != null)
				{
					component3.UpdateConnections(utilityConnections);
					if (DebugHandler.InstantBuildMode)
					{
						this.conduitMgr.SetConnections(utilityConnections, pathNode.cell, true);
					}
				}
			}
			if (this.def.ReplacementLayer != ObjectLayer.NumLayers && !DebugHandler.InstantBuildMode && this.def.IsValidBuildLocation(null, vector, Orientation.Neutral))
			{
				GameObject gameObject2 = Grid.Objects[pathNode.cell, (int)this.def.TileLayer];
				GameObject gameObject3 = Grid.Objects[pathNode.cell, (int)this.def.ReplacementLayer];
				if (gameObject2 != null && gameObject3 == null)
				{
					BuildingComplete component4 = gameObject2.GetComponent<BuildingComplete>();
					if (component4 != null && component4.Def != this.def)
					{
						Constructable component5 = this.def.BuildingUnderConstruction.GetComponent<Constructable>();
						component5.IsReplacementTile = true;
						gameObject = this.def.Instantiate(vector, Orientation.Neutral, this.selectedElements, 0, false);
						component5.IsReplacementTile = false;
						Grid.Objects[pathNode.cell, (int)this.def.ReplacementLayer] = gameObject;
						IUtilityItem component6 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
						if (component6 != null)
						{
							utilityConnections = component6.Connections;
						}
						utilityConnections |= this.conduitMgr.GetConnections(pathNode.cell, false);
						if (gameObject.GetComponent<BuildingComplete>() != null)
						{
							component6.UpdateConnections(utilityConnections);
						}
						string visualizerString = this.conduitMgr.GetVisualizerString(utilityConnections);
						string text = visualizerString;
						if (gameObject.GetComponent<KBatchedAnimController>().HasAnimation(visualizerString + "_place"))
						{
							text += "_place";
						}
						this.Play(gameObject, text);
					}
				}
			}
			if (gameObject != null)
			{
				IUtilityItem component7 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component7 != null)
				{
					component7.Connections = utilityConnections;
				}
			}
			TileVisualizer.RefreshCell(pathNode.cell, this.def.TileLayer);
		}
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
	}

	private BaseUtilityBuildTool.PathNode CreateVisualizer(BaseUtilityBuildTool.PathNode node)
	{
		if (node.visualizer == null)
		{
			Vector3 vector = Grid.CellToPosCBC(node.cell, this.def.SceneLayer);
			GameObject gameObject = global::UnityEngine.Object.Instantiate(this.def.BuildingPreview, vector, Quaternion.identity) as GameObject;
			gameObject.SetActive(true);
			node.visualizer = gameObject;
		}
		return node;
	}

	private void StopVisUpdater()
	{
		for (int i = 0; i < this.path.Count; i++)
		{
			global::UnityEngine.Object.Destroy(this.path[i].visualizer);
		}
		this.path.Clear();
		if (this.visUpdater != null)
		{
			base.StopCoroutine(this.visUpdater);
			this.visUpdater = null;
		}
	}

	private IList<Element> selectedElements;

	private BuildingDef def;

	private Conduit conduit;

	protected List<BaseUtilityBuildTool.PathNode> path = new List<BaseUtilityBuildTool.PathNode>();

	protected IUtilityNetworkMgr conduitMgr;

	private Coroutine visUpdater;

	private int buildingCount;

	private int lastCell = -1;

	private BuildingCellVisualizer previousCellConnection;

	private int previousCell;

	protected struct PathNode
	{
		public void Play(string anim)
		{
			KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
			component.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
		}

		public int cell;

		public GameObject visualizer;
	}
}
