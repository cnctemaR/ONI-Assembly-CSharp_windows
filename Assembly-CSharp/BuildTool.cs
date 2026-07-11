using System;
using System.Collections.Generic;
using FMOD.Studio;
using Rendering;
using STRINGS;
using UnityEngine;

public class BuildTool : DragTool
{
	public static void DestroyInstance()
	{
		BuildTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		BuildTool.Instance = this;
		this.tooltip = base.GetComponent<ToolTip>();
		this.buildingCount = global::UnityEngine.Random.Range(1, 14);
		this.canChangeDragAxis = false;
	}

	protected override void OnActivateTool()
	{
		this.lastDragCell = -1;
		if (this.visualizer != null)
		{
			this.ClearTilePreview();
			global::UnityEngine.Object.Destroy(this.visualizer);
		}
		this.active = true;
		base.OnActivateTool();
		this.buildingOrientation = Orientation.Neutral;
		this.placementPivot = this.def.placementPivot;
		Vector3 cursorPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
		GameObject buildingPreview = this.def.BuildingPreview;
		Vector3 vector = cursorPos;
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Ore;
		int num = LayerMask.NameToLayer("Place");
		this.visualizer = GameUtil.KInstantiate(buildingPreview, vector, sceneLayer, null, num);
		KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
			component.Offset = this.def.GetVisualizerOffset();
			component.Offset += this.def.placementPivot;
			component.name = component.GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
		}
		this.visualizer.SetActive(true);
		this.UpdateVis(cursorPos);
		BuildToolHoverTextCard component2 = base.GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = this.def;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(this.visualizer);
		if (component == null)
		{
			this.visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
		}
		else
		{
			component.SetLayer(LayerMask.NameToLayer("Place"));
		}
		GridCompositor.Instance.ToggleMajor(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		this.lastDragCell = -1;
		if (!this.active)
		{
			return;
		}
		this.active = false;
		GridCompositor.Instance.ToggleMajor(false);
		this.buildingOrientation = Orientation.Neutral;
		this.HideToolTip();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		this.ClearTilePreview();
		global::UnityEngine.Object.Destroy(this.visualizer);
		if (new_tool == SelectTool.Instance)
		{
			Game.Instance.Trigger(-1190690038, null);
		}
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(BuildingDef def, IList<Element> selected_elements, GameObject source = null)
	{
		this.selectedElements = selected_elements;
		this.def = def;
		this.source = source;
		this.viewMode = def.ViewMode;
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
		PlayerController.Instance.ActivateTool(this);
		this.OnActivateTool();
	}

	public void Deactivate()
	{
		this.selectedElements = null;
		SelectTool.Instance.Activate();
		this.def = null;
		this.source = null;
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
	}

	public int GetLastCell
	{
		get
		{
			return this.lastCell;
		}
	}

	public Orientation GetBuildingOrientation
	{
		get
		{
			return this.buildingOrientation;
		}
	}

	private void ClearTilePreview()
	{
		if (Grid.IsValidBuildingCell(this.lastCell) && this.def.IsTilePiece)
		{
			GameObject gameObject = Grid.Objects[this.lastCell, (int)this.def.TileLayer];
			if (this.visualizer == gameObject)
			{
				Grid.Objects[this.lastCell, (int)this.def.TileLayer] = null;
			}
			if (this.def.isKAnimTile)
			{
				GameObject gameObject2 = null;
				if (this.def.ReplacementLayer != ObjectLayer.NumLayers)
				{
					gameObject2 = Grid.Objects[this.lastCell, (int)this.def.ReplacementLayer];
				}
				if ((gameObject == null || gameObject.GetComponent<Constructable>() == null) && (gameObject2 == null || gameObject2 == this.visualizer))
				{
					World.Instance.blockTileRenderer.RemoveBlock(this.def, SimHashes.Void, this.lastCell);
					TileVisualizer.RefreshCell(this.lastCell, this.def.TileLayer);
				}
			}
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		cursorPos -= this.placementPivot;
		base.OnMouseMove(cursorPos);
		this.UpdateVis(cursorPos);
	}

	private void UpdateVis(Vector3 pos)
	{
		string text;
		bool flag = this.def.IsValidPlaceLocation(this.visualizer, pos, this.buildingOrientation, out text);
		bool flag2 = this.def.IsValidReplaceLocation(pos, this.buildingOrientation, this.def.ReplacementLayer, this.def.ObjectLayer);
		flag = flag || flag2;
		if (this.visualizer != null)
		{
			Color color = Color.white;
			float num = 0f;
			if (!flag)
			{
				color = Color.red;
				num = 1f;
			}
			this.SetColor(this.visualizer, color, num);
		}
		int num2 = Grid.PosToCell(pos);
		if (this.def != null)
		{
			Vector3 vector = Grid.CellToPosCBC(num2, this.def.SceneLayer);
			this.visualizer.transform.SetPosition(vector);
			base.transform.SetPosition(vector - Vector3.up * 0.5f);
			if (this.def.IsTilePiece)
			{
				this.ClearTilePreview();
				if (Grid.IsValidBuildingCell(num2))
				{
					GameObject gameObject = Grid.Objects[num2, (int)this.def.TileLayer];
					if (gameObject == null)
					{
						Grid.Objects[num2, (int)this.def.TileLayer] = this.visualizer;
					}
					if (this.def.isKAnimTile)
					{
						GameObject gameObject2 = null;
						if (this.def.ReplacementLayer != ObjectLayer.NumLayers)
						{
							gameObject2 = Grid.Objects[num2, 11];
						}
						if (gameObject == null || (gameObject.GetComponent<Constructable>() == null && gameObject2 == null))
						{
							TileVisualizer.RefreshCell(num2, this.def.TileLayer);
							if (this.def.BlockTileAtlas != null)
							{
								int num3 = LayerMask.NameToLayer("Overlay");
								BlockTileRenderer blockTileRenderer = World.Instance.blockTileRenderer;
								blockTileRenderer.SetInvalidPlaceCell(num2, !flag);
								if (this.lastCell != num2)
								{
									blockTileRenderer.SetInvalidPlaceCell(this.lastCell, false);
								}
								blockTileRenderer.AddBlock(num3, this.def, SimHashes.Void, num2);
							}
						}
					}
				}
			}
			if (this.lastCell != num2)
			{
				this.lastCell = num2;
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.RotateBuilding))
		{
			if (this.visualizer != null)
			{
				Rotatable component = this.visualizer.GetComponent<Rotatable>();
				if (component != null)
				{
					KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Rotate", false));
					this.buildingOrientation = component.Rotate();
					if (Grid.IsValidBuildingCell(this.lastCell))
					{
						Vector3 vector = Grid.CellToPosCCC(this.lastCell, Grid.SceneLayer.Building);
						this.UpdateVis(vector);
					}
				}
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (this.visualizer == null)
		{
			return;
		}
		if (cell == this.lastDragCell)
		{
			return;
		}
		this.lastDragCell = cell;
		this.ClearTilePreview();
		Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
		GameObject gameObject = null;
		if (DebugHandler.InstantBuildMode || (Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild))
		{
			string text;
			if (this.def.IsValidBuildLocation(this.visualizer, vector, this.buildingOrientation) && this.def.IsValidPlaceLocation(this.visualizer, vector, this.buildingOrientation, out text))
			{
				gameObject = this.def.Build(cell, this.buildingOrientation, null, this.selectedElements, 293.15f, false);
				if (this.source != null)
				{
					this.source.DeleteObject();
				}
			}
		}
		else
		{
			gameObject = this.def.TryPlace(this.visualizer, vector, this.buildingOrientation, this.selectedElements, 0);
			if (gameObject == null && this.def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				if (!Grid.ObjectLayers[(int)this.def.TileLayer].ContainsKey(cell))
				{
					return;
				}
				GameObject gameObject2 = Grid.ObjectLayers[(int)this.def.TileLayer][cell];
				if (gameObject2 != null && Grid.Objects[cell, (int)this.def.ReplacementLayer] == null)
				{
					BuildingComplete component = gameObject2.GetComponent<BuildingComplete>();
					if (component != null && component.Def.Replaceable && component.Def.IsFoundation && component.Def.isKAnimTile && (component.Def != this.def || this.selectedElements[0] != gameObject2.GetComponent<PrimaryElement>().Element))
					{
						Constructable component2 = this.def.BuildingUnderConstruction.GetComponent<Constructable>();
						component2.IsReplacementTile = true;
						gameObject = this.def.Instantiate(vector, this.buildingOrientation, this.selectedElements, 0);
						component2.IsReplacementTile = false;
						Grid.Objects[cell, (int)this.def.ReplacementLayer] = gameObject;
					}
				}
			}
			if (gameObject != null)
			{
				ObjectLayer objectLayer = ((!gameObject.GetComponent<Constructable>().IsReplacementTile) ? this.def.ObjectLayer : this.def.ReplacementLayer);
				this.def.MarkArea(cell, this.buildingOrientation, objectLayer, gameObject);
				Prioritizable component3 = gameObject.GetComponent<Prioritizable>();
				if (component3 != null)
				{
					if (BuildMenu.Instance != null)
					{
						component3.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
					}
					if (PlanScreen.Instance != null)
					{
						component3.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
					}
				}
				if (this.source != null)
				{
					this.source.Trigger(2121280625, gameObject);
				}
			}
		}
		if (gameObject != null)
		{
			if (this.def.MaterialsAvailable(this.selectedElements) || DebugHandler.InstantBuildMode)
			{
				this.placeSound = GlobalAssets.GetSound("Place_Building_" + this.def.AudioSize, false);
				if (this.placeSound != null)
				{
					this.buildingCount = this.buildingCount % 14 + 1;
					EventInstance eventInstance = SoundEvent.BeginOneShot(this.placeSound, vector);
					if (this.def.AudioSize == "small")
					{
						eventInstance.setParameterValue("tileCount", (float)this.buildingCount);
					}
					SoundEvent.EndOneShot(eventInstance);
				}
			}
			else
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector, 1.5f, false, false);
			}
			Rotatable component4 = gameObject.GetComponent<Rotatable>();
			if (component4 != null)
			{
				component4.SetOrientation(this.buildingOrientation);
			}
		}
	}

	protected override DragTool.Mode GetMode()
	{
		return DragTool.Mode.Brush;
	}

	private void SetColor(GameObject root, Color c, float strength)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.TintColour = c;
		}
	}

	private void ShowToolTip()
	{
		ToolTipScreen.Instance.SetToolTip(this.tooltip);
	}

	private void HideToolTip()
	{
		ToolTipScreen.Instance.ClearToolTip(this.tooltip);
	}

	public void Update()
	{
		if (this.active)
		{
			KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.SetLayer(LayerMask.NameToLayer("Place"));
			}
		}
	}

	public override string GetDeactivateSound()
	{
		return "HUD_Click_Deselect";
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
	}

	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private int lastCell = -1;

	private int lastDragCell = -1;

	private IList<Element> selectedElements;

	private BuildingDef def;

	private Orientation buildingOrientation;

	private GameObject source;

	private ToolTip tooltip;

	public static BuildTool Instance;

	private bool active;

	private int buildingCount;
}
