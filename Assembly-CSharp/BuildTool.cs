using System;
using System.Collections.Generic;
using FMOD.Studio;
using Rendering;
using UnityEngine;

public class BuildTool : DragTool
{
	protected override void OnPrefabInit()
	{
		BuildTool.Instance = this;
		this.tooltip = base.GetComponent<ToolTip>();
		this.buildingCount = global::UnityEngine.Random.Range(1, 14);
	}

	protected override void OnActivateTool()
	{
		this.active = true;
		base.OnActivateTool();
		this.buildingOrientation = Orientation.Neutral;
		this.placementPivot = this.def.placementPivot;
		GameObject buildingPreview = this.def.BuildingPreview;
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Ore;
		Folder folder = Folder.Placers;
		int num = LayerMask.NameToLayer("Place");
		this.visualizer = GameUtil.KInstantiate(buildingPreview, sceneLayer, folder, null, num);
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
		this.visualizer.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Placers).transform;
		this.visualizer.gameObject.SetActive(false);
		this.visualizer.gameObject.SetActive(true);
		BuildToolHoverTextCard component2 = base.GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = this.def;
		component2.ConfigureHoverScreen();
		component2.UpdateHoverElements(null);
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
		this.active = false;
		GridCompositor.Instance.ToggleMajor(false);
		this.buildingOrientation = Orientation.Neutral;
		this.HideToolTip();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		this.ClearTilePreview();
		global::UnityEngine.Object.Destroy(this.visualizer);
		Game.Instance.Trigger(-1190690038, null);
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
		if (Grid.IsValidCell(this.lastCell) && this.def.IsTilePiece)
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
		int num = Grid.PosToCell(cursorPos);
		bool flag = this.def.IsValidPlaceLocation(this.visualizer, cursorPos, this.buildingOrientation);
		bool flag2 = this.def.IsValidReplaceLocation(cursorPos, this.buildingOrientation, this.def.ReplacementLayer, this.def.ObjectLayer);
		flag = flag || flag2;
		if (this.visualizer != null)
		{
			Color color = Color.white;
			float num2 = 0f;
			if (!flag)
			{
				color = Color.red;
				num2 = 1f;
			}
			this.SetColor(this.visualizer, color, num2);
		}
		if (this.def != null)
		{
			Vector3 position = this.visualizer.transform.position;
			position.z = Grid.CellToPosCCC(0, this.def.SceneLayer).z;
			this.visualizer.transform.SetPosition(position);
			base.transform.SetPosition(position - Vector3.up * 0.5f);
			if (this.def.IsTilePiece)
			{
				this.ClearTilePreview();
				if (Grid.IsValidCell(num))
				{
					GameObject gameObject = Grid.Objects[num, (int)this.def.TileLayer];
					if (gameObject == null)
					{
						Grid.Objects[num, (int)this.def.TileLayer] = this.visualizer;
					}
					if (this.def.isKAnimTile)
					{
						GameObject gameObject2 = null;
						if (this.def.ReplacementLayer != ObjectLayer.NumLayers)
						{
							gameObject2 = Grid.Objects[num, 11];
						}
						if (gameObject == null || (gameObject.GetComponent<Constructable>() == null && gameObject2 == null))
						{
							TileVisualizer.RefreshCell(num, this.def.TileLayer);
							if (this.def.BlockTileAtlas != null)
							{
								int num3 = LayerMask.NameToLayer("Overlay");
								BlockTileRenderer blockTileRenderer = World.Instance.blockTileRenderer;
								blockTileRenderer.SetInvalidPlaceCell(num, !flag);
								if (this.lastCell != num)
								{
									blockTileRenderer.SetInvalidPlaceCell(this.lastCell, false);
								}
								blockTileRenderer.AddBlock(num3, this.def, SimHashes.Void, num);
							}
						}
					}
				}
			}
			if (this.lastCell != num)
			{
				this.lastCell = num;
				BuildToolHoverTextCard component = base.GetComponent<BuildToolHoverTextCard>();
				component.UpdateHoverElements(null);
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
		this.ClearTilePreview();
		Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
		GameObject gameObject = null;
		if (DebugHandler.InstantBuildMode)
		{
			if (this.def.IsValidBuildLocation(this.visualizer, vector, this.buildingOrientation) && this.def.IsValidPlaceLocation(this.visualizer, vector, this.buildingOrientation))
			{
				gameObject = this.def.Build(cell, this.buildingOrientation, null, this.selectedElements, 293.15f, false, true);
				if (this.source != null)
				{
					this.source.DeleteObject();
				}
			}
		}
		else
		{
			gameObject = this.def.TryPlace(vector, this.buildingOrientation, this.selectedElements, 0, false);
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
						gameObject = this.def.Instantiate(vector, this.buildingOrientation, this.selectedElements, 0, false);
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
					component3.SetMasterPriority(BuildMenuPriorityScreen.Instance.GetScreenPriority());
				}
				if (this.source != null)
				{
					this.source.Trigger(2121280625, gameObject);
				}
			}
		}
		if (gameObject != null)
		{
			BuildToolHoverTextCard component4 = base.GetComponent<BuildToolHoverTextCard>();
			component4.UpdateHoverElements(null);
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
				Rotatable component5 = gameObject.GetComponent<Rotatable>();
				if (component5 != null)
				{
					component5.SetOrientation(this.buildingOrientation);
				}
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

	public override void Update()
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

	private IList<Element> selectedElements;

	private BuildingDef def;

	private Orientation buildingOrientation;

	private GameObject source;

	private ToolTip tooltip;

	public static BuildTool Instance;

	private bool active;

	private int buildingCount;
}
