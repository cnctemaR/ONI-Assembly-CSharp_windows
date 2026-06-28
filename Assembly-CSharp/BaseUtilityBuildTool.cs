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
		Vector3 cursorPos = PlayerController.GetCursorPos(Input.mousePosition);
		GameObject buildingPreview = this.def.BuildingPreview;
		Vector3 vector = cursorPos;
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Ore;
		Folder folder = Folder.Placers;
		int num = LayerMask.NameToLayer("Place");
		this.visualizer = GameUtil.KInstantiate(buildingPreview, vector, sceneLayer, folder, null, num);
		KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
			component.SetDirty();
		}
		this.visualizer.SetActive(true);
		this.Play(this.visualizer, "None_Place");
		BuildToolHoverTextCard component2 = base.GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = this.def;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(this.visualizer);
		IHaveUtilityNetworkMgr component3 = this.def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
		this.conduitMgr = component3.GetNetworkManager();
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
			bool flag = this.CheckValidPathPiece(cell);
			this.path.Add(new BaseUtilityBuildTool.PathNode
			{
				cell = cell,
				visualizer = null,
				valid = flag
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

	private bool CheckValidPathPiece(int cell)
	{
		if (this.def.BuildLocationRule == BuildLocationRule.NotInTiles && Grid.Objects[cell, 9] != null)
		{
			return false;
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
						if (flag && component.RequiresPower)
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
		if (Grid.Visible[num] > 0 || PropertyTextures.FogOfWarScale == 1f)
		{
			bool flag = this.CheckValidPathPiece(num);
			this.path.Add(new BaseUtilityBuildTool.PathNode
			{
				cell = num,
				visualizer = null,
				valid = flag
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
				component.TintColour = ((!this.def.IsValidBuildLocation(null, pathNode2.cell, Orientation.Neutral, out text2)) ? Color.red : Color.white);
				TileVisualizer.RefreshCell(pathNode2.cell, this.def.TileLayer);
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
				string text;
				if (DebugHandler.InstantBuildMode && this.def.IsValidBuildLocation(this.visualizer, vector, Orientation.Neutral) && this.def.IsValidPlaceLocation(this.visualizer, vector, Orientation.Neutral, out text))
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
							if (BuildMenu.Instance != null)
							{
								component2.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
							}
							if (PlanScreen.Instance != null)
							{
								component2.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
							}
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
						string text2 = visualizerString;
						if (gameObject.GetComponent<KBatchedAnimController>().HasAnimation(visualizerString + "_place"))
						{
							text2 += "_place";
						}
						this.Play(gameObject, text2);
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

	private IList<Element> selectedElements;

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
			KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
			component.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
		}

		public int cell;

		public bool valid;

		public GameObject visualizer;
	}
}
