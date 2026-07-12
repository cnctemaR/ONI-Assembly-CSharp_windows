using System;
using System.Collections;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public class StampTool : InterfaceTool
{
	public static void DestroyInstance()
	{
		StampTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		StampTool.Instance = this;
	}

	private void Update()
	{
		this.RefreshPreview(Grid.PosToCell(this.GetCursorPos()));
	}

	public void Activate(TemplateContainer template, bool SelectAffected = false, bool DeactivateOnStamp = false)
	{
		this.selectAffected = SelectAffected;
		this.deactivateOnStamp = DeactivateOnStamp;
		if (this.stampTemplate == template)
		{
			return;
		}
		this.stampTemplate = template;
		PlayerController.Instance.ActivateTool(this);
		base.StartCoroutine(this.InitializePlacementVisual());
	}

	private Vector3 GetCursorPos()
	{
		return PlayerController.GetCursorPos(KInputManager.GetMousePos());
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		this.Stamp(cursor_pos);
	}

	private void Stamp(Vector2 pos)
	{
		if (!this.ready)
		{
			return;
		}
		int num = Grid.OffsetCell(Grid.PosToCell(pos), Mathf.FloorToInt(-this.stampTemplate.info.size.X / 2f), 0);
		int num2 = Grid.OffsetCell(Grid.PosToCell(pos), Mathf.FloorToInt(this.stampTemplate.info.size.X / 2f), 0);
		int num3 = Grid.OffsetCell(Grid.PosToCell(pos), 0, 1 + Mathf.FloorToInt(-this.stampTemplate.info.size.Y / 2f));
		int num4 = Grid.OffsetCell(Grid.PosToCell(pos), 0, 1 + Mathf.FloorToInt(this.stampTemplate.info.size.Y / 2f));
		if (!Grid.IsValidBuildingCell(num) || !Grid.IsValidBuildingCell(num2) || !Grid.IsValidBuildingCell(num4) || !Grid.IsValidBuildingCell(num3))
		{
			return;
		}
		this.ready = false;
		bool pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(true);
		}
		if (this.stampTemplate.cells != null)
		{
			for (int i = 0; i < this.buildingPreviews.Count; i++)
			{
				StampTool.ClearTilePreview(this.buildingPreviews[i]);
			}
			List<GameObject> list = new List<GameObject>();
			for (int j = 0; j < this.stampTemplate.cells.Count; j++)
			{
				for (int k = 0; k < 34; k++)
				{
					GameObject gameObject = Grid.Objects[Grid.XYToCell((int)(pos.x + (float)this.stampTemplate.cells[j].location_x), (int)(pos.y + (float)this.stampTemplate.cells[j].location_y)), k];
					if (gameObject != null && !list.Contains(gameObject))
					{
						list.Add(gameObject);
					}
				}
			}
			if (list != null)
			{
				foreach (GameObject gameObject2 in list)
				{
					if (gameObject2 != null)
					{
						Util.KDestroyGameObject(gameObject2);
					}
				}
			}
		}
		TemplateLoader.Stamp(this.stampTemplate, pos, delegate
		{
			this.CompleteStamp(pauseOnComplete);
		});
		if (this.selectAffected)
		{
			DebugBaseTemplateButton.Instance.ClearSelection();
			if (this.stampTemplate.cells != null)
			{
				for (int l = 0; l < this.stampTemplate.cells.Count; l++)
				{
					DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell((int)(pos.x + (float)this.stampTemplate.cells[l].location_x), (int)(pos.y + (float)this.stampTemplate.cells[l].location_y)));
				}
			}
		}
		if (this.deactivateOnStamp)
		{
			base.DeactivateTool(null);
		}
	}

	private void CompleteStamp(bool pause)
	{
		if (pause)
		{
			SpeedControlScreen.Instance.Pause(true, false);
		}
		this.ready = true;
		this.OnDeactivateTool(null);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.ReleasePlacementVisual();
		this.placementCell = Grid.InvalidCell;
		this.stampTemplate = null;
	}

	private IEnumerator InitializePlacementVisual()
	{
		this.ReleasePlacementVisual();
		this.rootCellPlacer = StampTool.placerPool.GetInstance();
		for (int i = 0; i < this.stampTemplate.cells.Count; i++)
		{
			Cell cell = this.stampTemplate.cells[i];
			if (cell.location_x != 0 || cell.location_y != 0)
			{
				GameObject instance = StampTool.placerPool.GetInstance();
				instance.transform.SetParent(this.rootCellPlacer.transform);
				instance.transform.localPosition = new Vector3((float)cell.location_x, (float)cell.location_y);
				instance.SetActive(true);
				this.childCellPlacers.Add(instance);
			}
		}
		for (int j = 0; j < this.stampTemplate.buildings.Count; j++)
		{
			Prefab prefab = this.stampTemplate.buildings[j];
			Building instance2 = StampTool.previewPool.GetInstance(prefab.id);
			Rotatable component = instance2.GetComponent<Rotatable>();
			if (component != null)
			{
				component.SetOrientation(prefab.rotationOrientation);
			}
			instance2.transform.SetParent(this.rootCellPlacer.transform);
			instance2.transform.SetLocalPosition(new Vector2((float)prefab.location_x, (float)prefab.location_y));
			instance2.gameObject.SetActive(true);
			this.buildingPreviews.Add(instance2);
		}
		yield return null;
		for (int k = 0; k < this.stampTemplate.buildings.Count; k++)
		{
			Prefab prefab2 = this.stampTemplate.buildings[k];
			Building building = this.buildingPreviews[k];
			string text = "";
			if ((prefab2.connections & 1) != 0)
			{
				text += "L";
			}
			if ((prefab2.connections & 2) != 0)
			{
				text += "R";
			}
			if ((prefab2.connections & 4) != 0)
			{
				text += "U";
			}
			if ((prefab2.connections & 8) != 0)
			{
				text += "D";
			}
			if (text == "")
			{
				text = "None";
			}
			KBatchedAnimController component2 = building.GetComponent<KBatchedAnimController>();
			if (component2 != null && component2.HasAnimation(text))
			{
				string text2 = text + "_place";
				bool flag = component2.HasAnimation(text2);
				component2.Play(flag ? text2 : text, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}
		yield break;
	}

	private void ReleasePlacementVisual()
	{
		if (this.rootCellPlacer == null)
		{
			return;
		}
		this.rootCellPlacer.SetActive(false);
		for (int i = this.childCellPlacers.Count - 1; i >= 0; i--)
		{
			GameObject gameObject = this.childCellPlacers[i];
			gameObject.transform.SetParent(StampTool.placerPoolTransform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.SetActive(false);
			StampTool.placerPool.ReleaseInstance(gameObject);
			this.childCellPlacers.RemoveAt(i);
		}
		for (int j = this.buildingPreviews.Count - 1; j >= 0; j--)
		{
			Building building = this.buildingPreviews[j];
			StampTool.ClearTilePreview(building);
			building.transform.SetParent(StampTool.previewPoolTransform);
			building.transform.localPosition = Vector3.zero;
			building.gameObject.SetActive(false);
			StampTool.previewPool.ReleaseInstance(building.Def.PrefabID, building);
			this.buildingPreviews.RemoveAt(j);
		}
		this.rootCellPlacer.transform.SetParent(StampTool.placerPoolTransform);
		this.rootCellPlacer.transform.position = Vector3.zero;
		StampTool.placerPool.ReleaseInstance(this.rootCellPlacer);
		this.rootCellPlacer = null;
	}

	private static void ClearTilePreview(Building b)
	{
		int num = Grid.PosToCell(b.transform.position);
		if (!b.Def.IsTilePiece || !Grid.IsValidBuildingCell(num))
		{
			return;
		}
		if (b.gameObject == Grid.Objects[num, (int)b.Def.TileLayer])
		{
			Grid.Objects[num, (int)b.Def.TileLayer] = null;
		}
		if (!b.Def.isKAnimTile)
		{
			return;
		}
		if (b.Def.BlockTileAtlas != null)
		{
			World.Instance.blockTileRenderer.RemoveBlock(b.Def, false, SimHashes.Void, num);
		}
		TileVisualizer.RefreshCell(num, b.Def.TileLayer, ObjectLayer.NumLayers);
	}

	private static void UpdateTileRendering(int newCell, Building b)
	{
		StampTool.ClearTilePreview(b);
		if (!b.Def.IsTilePiece || !Grid.IsValidBuildingCell(newCell))
		{
			return;
		}
		if (Grid.Objects[newCell, (int)b.Def.TileLayer] == null)
		{
			Grid.Objects[newCell, (int)b.Def.TileLayer] = b.gameObject;
		}
		if (!b.Def.isKAnimTile)
		{
			return;
		}
		if (b.Def.BlockTileAtlas != null)
		{
			World.Instance.blockTileRenderer.AddBlock(b.gameObject.layer, b.Def, false, SimHashes.Void, newCell);
		}
		TileVisualizer.RefreshCell(newCell, b.Def.TileLayer, ObjectLayer.NumLayers);
	}

	public void RefreshPreview(int new_placement_cell)
	{
		if (Grid.IsValidCell(new_placement_cell) && new_placement_cell != this.placementCell)
		{
			for (int i = 0; i < this.buildingPreviews.Count; i++)
			{
				Building building = this.buildingPreviews[i];
				Vector3 localPosition = building.transform.localPosition;
				StampTool.UpdateTileRendering(Grid.OffsetCell(new_placement_cell, (int)localPosition.x, (int)localPosition.y), building);
			}
			this.placementCell = new_placement_cell;
			this.rootCellPlacer.transform.SetPosition(Grid.CellToPosCBC(this.placementCell, this.visualizerLayer));
			this.rootCellPlacer.SetActive(true);
		}
	}

	private static Building InstantiatePreview(Tag previewId)
	{
		GameObject gameObject = Assets.TryGetPrefab(previewId);
		if (gameObject == null)
		{
			return null;
		}
		Building component = gameObject.GetComponent<Building>();
		if (component == null)
		{
			return null;
		}
		if (StampTool.previewPoolTransform == null)
		{
			StampTool.previewPoolTransform = new GameObject("Preview Pool").transform;
		}
		GameObject gameObject2 = component.Def.BuildingPreview;
		if (gameObject2 == null)
		{
			gameObject2 = BuildingLoader.Instance.CreateBuildingPreview(component.Def);
		}
		int num = LayerMask.NameToLayer("Place");
		Building component2 = GameUtil.KInstantiate(gameObject2, Vector3.zero, Grid.SceneLayer.Ore, null, num).GetComponent<Building>();
		KBatchedAnimController component3 = component2.GetComponent<KBatchedAnimController>();
		if (component3 != null)
		{
			component3.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component3.isMovable = true;
			component3.Offset = component.Def.GetVisualizerOffset();
			component3.name = component3.GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
			component3.TintColour = Color.white;
			component3.SetLayer(num);
		}
		component2.transform.SetParent(StampTool.previewPoolTransform);
		component2.gameObject.SetActive(false);
		return component2;
	}

	private static GameObject InstantiatePlacer()
	{
		if (StampTool.placerPoolTransform == null)
		{
			StampTool.placerPoolTransform = new GameObject("Stamp Placer Pool").transform;
		}
		GameObject gameObject = Util.KInstantiate(StampTool.Instance.PlacerPrefab, StampTool.placerPoolTransform.gameObject, null);
		gameObject.SetActive(false);
		return gameObject;
	}

	public static StampTool Instance;

	private static HashMapObjectPool<Tag, Building> previewPool = new HashMapObjectPool<Tag, Building>(new Func<Tag, Building>(StampTool.InstantiatePreview), 0);

	private static GameObjectPool placerPool = new GameObjectPool(new Func<GameObject>(StampTool.InstantiatePlacer), 0);

	private static Transform previewPoolTransform = null;

	private static Transform placerPoolTransform = null;

	public TemplateContainer stampTemplate;

	public GameObject PlacerPrefab;

	private bool ready = true;

	private int placementCell = Grid.InvalidCell;

	private bool selectAffected;

	private bool deactivateOnStamp;

	private GameObject rootCellPlacer;

	private List<GameObject> childCellPlacers = new List<GameObject>();

	private List<Building> buildingPreviews = new List<Building>();
}
