using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class BaseUtilityBuildTool : DragTool
{
	protected override void OnPrefabInit()
	{
		this.buildingCount = global::UnityEngine.Random.Range(1, 14);
		this.canChangeDragAxis = false;
	}

	private void Play(GameObject go, string anim)
	{
		go.GetComponent<KBatchedAnimController>().Play(anim, KAnim.PlayMode.Once, 1f, 0f);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		Vector3 cursorPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
		this.visualizer = GameUtil.KInstantiate(this.def.BuildingPreview, cursorPos, Grid.SceneLayer.Ore, null, LayerMask.NameToLayer("Place"));
		KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
			component.SetDirty();
		}
		this.visualizer.SetActive(true);
		this.Play(this.visualizer, "None_Place");
		base.GetComponent<BuildToolHoverTextCard>().currentDef = this.def;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(this.visualizer);
		IHaveUtilityNetworkMgr component2 = this.def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
		this.conduitMgr = component2.GetNetworkManager();
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

	public void Activate(BuildingDef def, IList<Tag> selected_elements)
	{
		this.selectedElements = selected_elements;
		this.def = def;
		this.viewMode = def.ViewMode;
		PlayerController.Instance.ActivateTool(this);
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (this.path.Count == 0 || this.path[this.path.Count - 1].cell == cell)
		{
			return;
		}
		this.placeSound = GlobalAssets.GetSound("Place_building_" + this.def.AudioSize, false);
		Vector3 vector = Grid.CellToPos(cell);
		EventInstance eventInstance = SoundEvent.BeginOneShot(this.placeSound, vector, 1f, false);
		if (this.path.Count > 1 && cell == this.path[this.path.Count - 2].cell)
		{
			if (this.previousCellConnection != null)
			{
				this.previousCellConnection.ConnectedEvent(this.previousCell);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("OutletDisconnected", false));
				this.previousCellConnection = null;
			}
			this.previousCell = cell;
			this.CheckForConnection(cell, this.def.PrefabID, "", ref this.previousCellConnection, false);
			global::UnityEngine.Object.Destroy(this.path[this.path.Count - 1].visualizer);
			TileVisualizer.RefreshCell(this.path[this.path.Count - 1].cell, this.def.TileLayer, this.def.ReplacementLayer);
			this.path.RemoveAt(this.path.Count - 1);
			this.buildingCount = ((this.buildingCount == 1) ? (this.buildingCount = 14) : (this.buildingCount - 1));
			eventInstance.setParameterValue("tileCount", (float)this.buildingCount);
			SoundEvent.EndOneShot(eventInstance);
		}
		else if (!this.path.Exists((BaseUtilityBuildTool.PathNode n) => n.cell == cell))
		{
			bool flag = this.CheckValidPathPiece(cell);
			this.path.Add(new BaseUtilityBuildTool.PathNode
			{
				cell = cell,
				visualizer = null,
				valid = flag
			});
			this.CheckForConnection(cell, this.def.PrefabID, "OutletConnected", ref this.previousCellConnection, true);
			this.buildingCount = this.buildingCount % 14 + 1;
			eventInstance.setParameterValue("tileCount", (float)this.buildingCount);
			SoundEvent.EndOneShot(eventInstance);
		}
		this.visualizer.SetActive(this.path.Count < 2);
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(this.path.Count);
	}

	protected override int GetDragLength()
	{
		return this.path.Count;
	}

	private bool CheckValidPathPiece(int cell)
	{
		if (this.def.BuildLocationRule == BuildLocationRule.NotInTiles)
		{
			if (Grid.Objects[cell, 9] != null)
			{
				return false;
			}
			if (Grid.HasDoor[cell])
			{
				return false;
			}
		}
		GameObject gameObject = Grid.Objects[cell, (int)this.def.ObjectLayer];
		if (gameObject != null && gameObject.GetComponent<KAnimGraphTileVisualizer>() == null)
		{
			return false;
		}
		GameObject gameObject2 = Grid.Objects[cell, (int)this.def.TileLayer];
		return !(gameObject2 != null) || !(gameObject2.GetComponent<KAnimGraphTileVisualizer>() == null);
	}

	private bool CheckForConnection(int cell, string defName, string soundName, ref BuildingCellVisualizer outBcv, bool fireEvents = true)
	{
		outBcv = null;
		DebugUtil.Assert(defName != null, "defName was null");
		Building building = this.GetBuilding(cell);
		if (!building)
		{
			return false;
		}
		DebugUtil.Assert(building.gameObject, "targetBuilding.gameObject was null");
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		if (defName.Contains("LogicWire"))
		{
			LogicPorts component = building.gameObject.GetComponent<LogicPorts>();
			if (!(component != null))
			{
				goto IL_022C;
			}
			if (component.inputPorts != null)
			{
				foreach (ILogicUIElement logicUIElement in component.inputPorts)
				{
					DebugUtil.Assert(logicUIElement != null, "input port was null");
					if (logicUIElement.GetLogicUICell() == cell)
					{
						num = cell;
						break;
					}
				}
			}
			if (num != -1 || component.outputPorts == null)
			{
				goto IL_022C;
			}
			using (List<ILogicUIElement>.Enumerator enumerator = component.outputPorts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ILogicUIElement logicUIElement2 = enumerator.Current;
					DebugUtil.Assert(logicUIElement2 != null, "output port was null");
					if (logicUIElement2.GetLogicUICell() == cell)
					{
						num2 = cell;
						break;
					}
				}
				goto IL_022C;
			}
		}
		if (defName.Contains("Wire"))
		{
			num = building.GetPowerInputCell();
			num2 = building.GetPowerOutputCell();
		}
		else if (defName.Contains("Liquid"))
		{
			if (building.Def.InputConduitType == ConduitType.Liquid)
			{
				num = building.GetUtilityInputCell();
			}
			if (building.Def.OutputConduitType == ConduitType.Liquid)
			{
				num2 = building.GetUtilityOutputCell();
			}
			ElementFilter component2 = building.GetComponent<ElementFilter>();
			if (component2 != null)
			{
				DebugUtil.Assert(component2.portInfo != null, "elementFilter.portInfo was null A");
				if (component2.portInfo.conduitType == ConduitType.Liquid)
				{
					num3 = component2.GetFilteredCell();
				}
			}
		}
		else if (defName.Contains("Gas"))
		{
			if (building.Def.InputConduitType == ConduitType.Gas)
			{
				num = building.GetUtilityInputCell();
			}
			if (building.Def.OutputConduitType == ConduitType.Gas)
			{
				num2 = building.GetUtilityOutputCell();
			}
			ElementFilter component3 = building.GetComponent<ElementFilter>();
			if (component3 != null)
			{
				DebugUtil.Assert(component3.portInfo != null, "elementFilter.portInfo was null B");
				if (component3.portInfo.conduitType == ConduitType.Gas)
				{
					num3 = component3.GetFilteredCell();
				}
			}
		}
		IL_022C:
		if (cell == num || cell == num2 || cell == num3)
		{
			BuildingCellVisualizer component4 = building.gameObject.GetComponent<BuildingCellVisualizer>();
			outBcv = component4;
			if (component4 != null && true)
			{
				if (fireEvents)
				{
					component4.ConnectedEvent(cell);
					string sound = GlobalAssets.GetSound(soundName, false);
					if (sound != null)
					{
						KMonoBehaviour.PlaySound(sound);
					}
				}
				return true;
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

	protected override DragTool.Mode GetMode()
	{
		return DragTool.Mode.Brush;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		if (this.visualizer == null)
		{
			return;
		}
		this.path.Clear();
		int num = Grid.PosToCell(cursor_pos);
		if (Grid.IsValidCell(num) && Grid.IsVisible(num))
		{
			bool flag = this.CheckValidPathPiece(num);
			this.path.Add(new BaseUtilityBuildTool.PathNode
			{
				cell = num,
				visualizer = null,
				valid = flag
			});
			this.CheckForConnection(num, this.def.PrefabID, "OutletConnected", ref this.previousCellConnection, true);
		}
		this.visUpdater = base.StartCoroutine(this.VisUpdater());
		this.visualizer.GetComponent<KBatchedAnimController>().StopAndClear();
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(1);
		this.placeSound = GlobalAssets.GetSound("Place_building_" + this.def.AudioSize, false);
		if (this.placeSound != null)
		{
			this.buildingCount = this.buildingCount % 14 + 1;
			Vector3 vector = Grid.CellToPos(num);
			EventInstance eventInstance = SoundEvent.BeginOneShot(this.placeSound, vector, 1f, false);
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
		if (this.visualizer == null)
		{
			return;
		}
		this.BuildPath();
		this.StopVisUpdater();
		this.Play(this.visualizer, "None_Place");
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
		base.OnLeftClickUp(cursor_pos);
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		int num = Grid.PosToCell(cursorPos);
		if (this.lastCell != num)
		{
			this.lastCell = num;
		}
		if (this.visualizer != null)
		{
			Color color = Color.white;
			float num2 = 0f;
			string text;
			if (!this.def.IsValidPlaceLocation(this.visualizer, num, Orientation.Neutral, out text))
			{
				color = Color.red;
				num2 = 1f;
			}
			this.SetColor(this.visualizer, color, num2);
		}
	}

	private void SetColor(GameObject root, Color c, float strength)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.TintColour = c;
		}
	}

	protected virtual void ApplyPathToConduitSystem()
	{
		DebugUtil.Assert(false, "I don't think this function ever runs");
	}

	private IEnumerator VisUpdater()
	{
		for (;;)
		{
			this.conduitMgr.StashVisualGrids();
			if (this.path.Count == 1)
			{
				BaseUtilityBuildTool.PathNode pathNode = this.path[0];
				this.path[0] = this.CreateVisualizer(pathNode);
			}
			this.ApplyPathToConduitSystem();
			for (int i = 0; i < this.path.Count; i++)
			{
				BaseUtilityBuildTool.PathNode pathNode2 = this.path[i];
				pathNode2 = this.CreateVisualizer(pathNode2);
				this.path[i] = pathNode2;
				string text = this.conduitMgr.GetVisualizerString(pathNode2.cell) + "_place";
				KBatchedAnimController component = pathNode2.visualizer.GetComponent<KBatchedAnimController>();
				if (component.HasAnimation(text))
				{
					pathNode2.Play(text);
				}
				else
				{
					pathNode2.Play(this.conduitMgr.GetVisualizerString(pathNode2.cell));
				}
				string text2;
				component.TintColour = (this.def.IsValidBuildLocation(null, pathNode2.cell, Orientation.Neutral, out text2) ? Color.white : Color.red);
				TileVisualizer.RefreshCell(pathNode2.cell, this.def.TileLayer, this.def.ReplacementLayer);
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
		bool flag = false;
		for (int i = 0; i < this.path.Count; i++)
		{
			BaseUtilityBuildTool.PathNode pathNode = this.path[i];
			Vector3 vector = Grid.CellToPosCBC(pathNode.cell, Grid.SceneLayer.Building);
			UtilityConnections utilityConnections = (UtilityConnections)0;
			GameObject gameObject = Grid.Objects[pathNode.cell, (int)this.def.TileLayer];
			if (gameObject == null)
			{
				utilityConnections = this.conduitMgr.GetConnections(pathNode.cell, false);
				string text;
				if ((DebugHandler.InstantBuildMode || (Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild)) && this.def.IsValidBuildLocation(this.visualizer, vector, Orientation.Neutral) && this.def.IsValidPlaceLocation(this.visualizer, vector, Orientation.Neutral, out text))
				{
					gameObject = this.def.Build(pathNode.cell, Orientation.Neutral, null, this.selectedElements, 293.15f, true, GameClock.Instance.GetTime());
				}
				else
				{
					gameObject = this.def.TryPlace(null, vector, Orientation.Neutral, this.selectedElements, 0);
					if (gameObject != null)
					{
						if (!this.def.MaterialsAvailable(this.selectedElements) && !DebugHandler.InstantBuildMode)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector, 1.5f, false, false);
						}
						Constructable component = gameObject.GetComponent<Constructable>();
						if (component.IconConnectionAnimation(0.1f * (float)num, num, "Wire", "OutletConnected_release") || component.IconConnectionAnimation(0.1f * (float)num, num, "Pipe", "OutletConnected_release"))
						{
							num++;
						}
						flag = true;
					}
				}
			}
			else
			{
				IUtilityItem component2 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component2 != null)
				{
					utilityConnections = component2.Connections;
				}
				utilityConnections |= this.conduitMgr.GetConnections(pathNode.cell, false);
				if (gameObject.GetComponent<BuildingComplete>() != null)
				{
					component2.UpdateConnections(utilityConnections);
				}
			}
			if (this.def.ReplacementLayer != ObjectLayer.NumLayers && !DebugHandler.InstantBuildMode && (!Game.Instance.SandboxModeActive || !SandboxToolParameterMenu.instance.settings.InstantBuild) && this.def.IsValidBuildLocation(null, vector, Orientation.Neutral))
			{
				GameObject gameObject2 = Grid.Objects[pathNode.cell, (int)this.def.TileLayer];
				GameObject gameObject3 = Grid.Objects[pathNode.cell, (int)this.def.ReplacementLayer];
				if (gameObject2 != null && gameObject3 == null)
				{
					BuildingComplete component3 = gameObject2.GetComponent<BuildingComplete>();
					bool flag2 = gameObject2.GetComponent<PrimaryElement>().Element.tag != this.selectedElements[0];
					if (component3 != null && (component3.Def != this.def || flag2))
					{
						Constructable component4 = this.def.BuildingUnderConstruction.GetComponent<Constructable>();
						component4.IsReplacementTile = true;
						gameObject = this.def.Instantiate(vector, Orientation.Neutral, this.selectedElements, 0);
						component4.IsReplacementTile = false;
						if (!this.def.MaterialsAvailable(this.selectedElements) && !DebugHandler.InstantBuildMode)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector, 1.5f, false, false);
						}
						Grid.Objects[pathNode.cell, (int)this.def.ReplacementLayer] = gameObject;
						IUtilityItem component5 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
						if (component5 != null)
						{
							utilityConnections = component5.Connections;
						}
						utilityConnections |= this.conduitMgr.GetConnections(pathNode.cell, false);
						if (gameObject.GetComponent<BuildingComplete>() != null)
						{
							component5.UpdateConnections(utilityConnections);
						}
						string visualizerString = this.conduitMgr.GetVisualizerString(utilityConnections);
						string text2 = visualizerString;
						if (gameObject.GetComponent<KBatchedAnimController>().HasAnimation(visualizerString + "_place"))
						{
							text2 += "_place";
						}
						this.Play(gameObject, text2);
						flag = true;
					}
				}
			}
			if (gameObject != null)
			{
				if (flag)
				{
					Prioritizable component6 = gameObject.GetComponent<Prioritizable>();
					if (component6 != null)
					{
						if (BuildMenu.Instance != null)
						{
							component6.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
						}
						if (PlanScreen.Instance != null)
						{
							component6.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
						}
					}
				}
				IUtilityItem component7 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component7 != null)
				{
					component7.Connections = utilityConnections;
				}
			}
			TileVisualizer.RefreshCell(pathNode.cell, this.def.TileLayer, this.def.ReplacementLayer);
		}
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
	}

	private BaseUtilityBuildTool.PathNode CreateVisualizer(BaseUtilityBuildTool.PathNode node)
	{
		if (node.visualizer == null)
		{
			Vector3 vector = Grid.CellToPosCBC(node.cell, this.def.SceneLayer);
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.def.BuildingPreview, vector, Quaternion.identity);
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

	private IList<Tag> selectedElements;

	private BuildingDef def;

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
			this.visualizer.GetComponent<KBatchedAnimController>().Play(anim, KAnim.PlayMode.Once, 1f, 0f);
		}

		public int cell;

		public bool valid;

		public GameObject visualizer;
	}
}
