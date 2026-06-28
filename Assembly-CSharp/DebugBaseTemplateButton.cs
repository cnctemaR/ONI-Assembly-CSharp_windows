using System;
using System.Collections.Generic;
using BaseTemplateClasses;
using UnityEngine;
using UnityEngine.UI;

public class DebugBaseTemplateButton : KScreen
{
	public static DebugBaseTemplateButton Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DebugBaseTemplateButton.Instance = this;
		base.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.saveBaseButton != null)
		{
			this.saveBaseButton.onClick -= this.OnClickSaveBase;
			this.saveBaseButton.onClick += this.OnClickSaveBase;
			if (!Application.isEditor)
			{
				this.saveBaseButton.gameObject.SetActive(false);
			}
		}
		if (this.clearButton != null)
		{
			this.clearButton.onClick -= this.OnClickClear;
			this.clearButton.onClick += this.OnClickClear;
			if (!Application.isEditor)
			{
				this.clearButton.gameObject.SetActive(false);
			}
		}
		if (this.AddSelectionButton != null)
		{
			this.AddSelectionButton.onClick -= this.OnClickAddSelection;
			this.AddSelectionButton.onClick += this.OnClickAddSelection;
			if (!Application.isEditor)
			{
				this.AddSelectionButton.gameObject.SetActive(false);
			}
		}
		if (this.RemoveSelectionButton != null)
		{
			this.RemoveSelectionButton.onClick -= this.OnClickRemoveSelection;
			this.RemoveSelectionButton.onClick += this.OnClickRemoveSelection;
			if (!Application.isEditor)
			{
				this.RemoveSelectionButton.gameObject.SetActive(false);
			}
		}
		if (this.clearSelectionButton != null)
		{
			this.clearSelectionButton.onClick -= this.OnClickClearSelection;
			this.clearSelectionButton.onClick += this.OnClickClearSelection;
			if (!Application.isEditor)
			{
				this.clearSelectionButton.gameObject.SetActive(false);
			}
		}
		if (this.MoveButton != null)
		{
			this.MoveButton.onClick -= this.OnClickMove;
			this.MoveButton.onClick += this.OnClickMove;
			if (!Application.isEditor)
			{
				this.RemoveSelectionButton.gameObject.SetActive(false);
			}
		}
		if (this.pasteStartingBaseButton != null)
		{
			this.pasteStartingBaseButton.onClick -= this.OnClickPasteStartingBase;
			this.pasteStartingBaseButton.onClick += this.OnClickPasteStartingBase;
			if (!Application.isEditor)
			{
				this.pasteStartingBaseButton.gameObject.SetActive(false);
			}
		}
		if (this.DestroyButton != null)
		{
			this.DestroyButton.onClick -= this.OnClickDestroySelection;
			this.DestroyButton.onClick += this.OnClickDestroySelection;
			if (!Application.isEditor)
			{
				this.DestroyButton.gameObject.SetActive(false);
			}
		}
		if (this.DeconstructButton != null)
		{
			this.DeconstructButton.onClick -= this.OnClickDeconstructSelection;
			this.DeconstructButton.onClick += this.OnClickDeconstructSelection;
			if (!Application.isEditor)
			{
				this.DeconstructButton.gameObject.SetActive(false);
			}
		}
	}

	private void OnClickPasteStartingBase()
	{
		DebugTool.Instance.DeactivateTool(null);
		this.pasteAndSelectAsset = null;
		this.pasteAndSelectAsset = Assets.GetBaseTemplate();
		if (this.pasteAndSelectAsset == null)
		{
			return;
		}
		this.ClearSelection();
		StampTool.Instance.Activate(this.pasteAndSelectAsset, true, false);
	}

	private void OnClickDestroySelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Destroy);
	}

	private void OnClickDeconstructSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Deconstruct);
	}

	private void OnClickMove()
	{
		DebugTool.Instance.DeactivateTool(null);
		this.moveAsset = this.GetSelectionAsAsset();
		StampTool.Instance.Activate(this.moveAsset, false, false);
	}

	private void OnClickAddSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.AddSelection);
	}

	private void OnClickRemoveSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.RemoveSelection);
	}

	private void OnClickClearSelection()
	{
		this.ClearSelection();
	}

	private void OnClickClear()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Clear);
	}

	protected override void OnDeactivate()
	{
		if (DebugTool.Instance != null)
		{
			DebugTool.Instance.DeactivateTool(null);
		}
		base.OnDeactivate();
	}

	private void OnDisable()
	{
		if (DebugTool.Instance != null)
		{
			DebugTool.Instance.DeactivateTool(null);
		}
	}

	private BaseTemplate GetSelectionAsAsset()
	{
		List<BaseTemplateCellInfo> list = new List<BaseTemplateCellInfo>();
		List<BaseTemplatePrefabInfo> list2 = new List<BaseTemplatePrefabInfo>();
		List<BaseTemplatePrefabInfo> list3 = new List<BaseTemplatePrefabInfo>();
		List<BaseTemplatePrefabInfo> list4 = new List<BaseTemplatePrefabInfo>();
		float num = 0f;
		float num2 = 0f;
		foreach (int num3 in this.SelectedCells)
		{
			num += (float)Grid.CellToXY(num3).x;
			num2 += (float)Grid.CellToXY(num3).y;
		}
		float num4 = num / (float)this.SelectedCells.Count;
		float num5;
		num2 = (num5 = num2 / (float)this.SelectedCells.Count);
		int num6 = Grid.PosToCell(new Vector3(num4, num5, 0f));
		int num7;
		int num8;
		Grid.CellToXY(num6, out num7, out num8);
		if (this.AutoSaveRoomContents)
		{
			for (int i = 0; i < Game.Instance.roomProber.rooms.Count; i++)
			{
				foreach (int num9 in Game.Instance.roomProber.rooms[i].cells)
				{
					Sim.Cell cell = Grid.Cell[num9];
					int num10;
					int num11;
					Grid.CellToXY(num9, out num10, out num11);
					Element element = ElementLoader.elements[(int)Grid.Cell[num9].elementIdx];
					list.Add(new BaseTemplateCellInfo(num10 - num7, num11 - num8, element.id, cell.temperature, cell.mass));
				}
			}
		}
		for (int j = 0; j < this.SelectedCells.Count; j++)
		{
			Sim.Cell cell2 = Grid.Cell[this.SelectedCells[j]];
			int num12;
			int num13;
			Grid.CellToXY(this.SelectedCells[j], out num12, out num13);
			Element element2 = ElementLoader.elements[(int)Grid.Cell[this.SelectedCells[j]].elementIdx];
			list.Add(new BaseTemplateCellInfo(num12 - num7, num13 - num8, element2.id, cell2.temperature, cell2.mass));
		}
		BaseTemplateConduitConnection[] array = new BaseTemplateConduitConnection[Components.BuildingCompletes.Count];
		for (int k = 0; k < Components.BuildingCompletes.Count; k++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[k];
			int num14;
			int num15;
			Grid.CellToXY(Grid.PosToCell(buildingComplete), out num14, out num15);
			if (this.SaveAllBuildings || this.SelectedCells.Contains(Grid.PosToCell(buildingComplete)))
			{
				foreach (int num16 in buildingComplete.PlacementCells)
				{
					Sim.Cell cell3 = Grid.Cell[num16];
					int num17;
					int num18;
					Grid.CellToXY(num16, out num17, out num18);
					Element element3 = ElementLoader.elements[(int)Grid.Cell[num16].elementIdx];
					list.Add(new BaseTemplateCellInfo(num17 - num7, num18 - num8, element3.id, cell3.temperature, cell3.mass));
				}
				Orientation orientation = Orientation.Up;
				Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
				if (component != null)
				{
					orientation = component.GetOrientation();
				}
				float num19 = 280f;
				StructureTemperature component2 = buildingComplete.gameObject.GetComponent<StructureTemperature>();
				if (component2)
				{
					num19 = buildingComplete.gameObject.GetComponent<StructureTemperature>().Temperature;
				}
				string name = buildingComplete.PrefabID().Name;
				if (name == null)
				{
					goto IL_0436;
				}
				if (DebugBaseTemplateButton.<>f__switch$map1 == null)
				{
					DebugBaseTemplateButton.<>f__switch$map1 = new Dictionary<string, int>(4)
					{
						{ "Wire", 0 },
						{ "InsulatedWire", 1 },
						{ "GasConduit", 2 },
						{ "LiquidConduit", 3 }
					};
				}
				int num20;
				if (DebugBaseTemplateButton.<>f__switch$map1.TryGetValue(name, out num20))
				{
					switch (num20)
					{
					case 0:
						array[k] = new BaseTemplateConduitConnection(BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Electrical, Game.Instance.electricalConduitSystem.GetConnections(Grid.PosToCell(buildingComplete), true));
						goto IL_04E3;
					case 1:
						array[k] = new BaseTemplateConduitConnection(BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Electrical, Game.Instance.electricalConduitSystem.GetConnections(Grid.PosToCell(buildingComplete), true));
						goto IL_04E3;
					case 2:
						array[k] = new BaseTemplateConduitConnection(BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Gas, Game.Instance.gasConduitSystem.GetConnections(Grid.PosToCell(buildingComplete), true));
						goto IL_04E3;
					case 3:
						array[k] = new BaseTemplateConduitConnection(BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Liquid, Game.Instance.liquidConduitSystem.GetConnections(Grid.PosToCell(buildingComplete), true));
						goto IL_04E3;
					}
					goto IL_0436;
				}
				goto IL_0436;
				IL_04E3:
				num14 -= num7;
				num15 -= num8;
				num19 = Mathf.Clamp(num19, 1f, 99999f);
				BaseTemplatePrefabInfo baseTemplatePrefabInfo = new BaseTemplatePrefabInfo(buildingComplete.PrefabID().Name, num14, num15, buildingComplete.GetComponent<PrimaryElement>().ElementID, num19, 0f, orientation);
				Storage component3 = buildingComplete.gameObject.GetComponent<Storage>();
				if (component3 != null)
				{
					foreach (GameObject gameObject in component3.items)
					{
						float num21 = 0f;
						SimHashes simHashes = SimHashes.Vacuum;
						float num22 = 280f;
						bool flag = false;
						PrimaryElement component4 = gameObject.GetComponent<PrimaryElement>();
						if (component4 != null)
						{
							num21 = component4.Mass;
							simHashes = component4.ElementID;
							num22 = component4.Temperature;
						}
						ElementChunk component5 = gameObject.GetComponent<ElementChunk>();
						int num23 = 0;
						Edible component6 = gameObject.gameObject.GetComponent<Edible>();
						if (component6 != null && component4 != null)
						{
							num23 = Mathf.RoundToInt(component4.Units);
						}
						float num24 = 0f;
						Rottable.Instance smi = gameObject.gameObject.GetSMI<Rottable.Instance>();
						if (smi != null)
						{
							num24 = smi.RotAmount;
						}
						if (component4 != null && component6 == null && smi == null)
						{
							flag = true;
							if (component5 == null)
							{
								Debug.LogWarning("Template saver is unsure what this stored pickup object is: " + gameObject.GetProperName() + ". Unlikely to load correctly.");
							}
						}
						BaseTemplateStorageItem baseTemplateStorageItem = new BaseTemplateStorageItem(gameObject.PrefabID().Name, num21, num22, simHashes, flag);
						if (component6)
						{
							baseTemplateStorageItem.rations.rations = num23;
						}
						if (smi != null)
						{
							baseTemplateStorageItem.rottable.rotAmount = num24;
						}
						baseTemplatePrefabInfo.AssignStorage(baseTemplateStorageItem);
					}
				}
				list2.Add(baseTemplatePrefabInfo);
				goto IL_06E4;
				IL_0436:
				array[k] = new BaseTemplateConduitConnection(BaseTemplateConduitConnection.BaseTemplateConduitSystemType.None, (UtilityConnections)0);
				goto IL_04E3;
			}
			IL_06E4:;
		}
		for (int m = 0; m < Components.Pickupables.Count; m++)
		{
			if (Components.Pickupables[m].gameObject.activeSelf)
			{
				Pickupable pickupable = Components.Pickupables[m];
				int num25 = Grid.PosToCell(pickupable);
				if (this.SaveAllPickups || this.SelectedCells.Contains(num25))
				{
					if (!Components.Pickupables[m].gameObject.GetComponent<MinionBrain>())
					{
						int num26;
						int num27;
						Grid.CellToXY(num25, out num26, out num27);
						num26 -= num7;
						num27 -= num8;
						float num28 = 280f;
						float num29 = 1f;
						ElementChunk component7 = pickupable.gameObject.GetComponent<ElementChunk>();
						float num30 = 0f;
						Edible component8 = pickupable.gameObject.GetComponent<Edible>();
						if (component8 != null)
						{
							num30 = component8.rations;
						}
						float num31 = 0f;
						Rottable.Instance smi2 = pickupable.gameObject.GetSMI<Rottable.Instance>();
						if (smi2 != null)
						{
							num31 = smi2.RotAmount;
						}
						PrimaryElement component9 = pickupable.gameObject.GetComponent<PrimaryElement>();
						if (component9 != null)
						{
							num29 = component9.Mass;
							num28 = component9.Temperature;
							if (component7 == null && component8 == null && smi2 == null)
							{
								Debug.LogWarning("Template saver is unsure what this pickup object is: " + pickupable.GetProperName() + ". Unlikely to load correctly.");
							}
						}
						PrimaryElement component10 = pickupable.gameObject.GetComponent<PrimaryElement>();
						if (component10 != null && component9 != null && component8 == null)
						{
							BaseTemplatePrefabInfo baseTemplatePrefabInfo2 = new BaseTemplatePrefabInfo(pickupable.PrefabID().Name, num26, num27, component10.ElementID, num28, num29, Orientation.Up);
							list4.Add(baseTemplatePrefabInfo2);
						}
						else
						{
							BaseTemplatePrefabInfo baseTemplatePrefabInfo2 = new BaseTemplatePrefabInfo(pickupable.PrefabID().Name, num26, num27);
							baseTemplatePrefabInfo2.rations.rations = num30;
							baseTemplatePrefabInfo2.rottable.rotAmount = num31;
							list3.Add(baseTemplatePrefabInfo2);
						}
					}
				}
			}
		}
		BaseTemplate baseTemplate = ScriptableObject.CreateInstance<BaseTemplate>();
		baseTemplate.Init(list, list2, list3, list4, array);
		return baseTemplate;
	}

	private void OnClickSaveBase()
	{
		BaseTemplate selectionAsAsset = this.GetSelectionAsAsset();
	}

	public void ClearSelection()
	{
		for (int i = this.SelectedCells.Count - 1; i >= 0; i--)
		{
			this.RemoveFromSelection(this.SelectedCells[i]);
		}
	}

	public void DestroySelection()
	{
	}

	public void DeconstructSelection()
	{
	}

	public void AddToSelection(int cell)
	{
		if (!this.SelectedCells.Contains(cell))
		{
			GameObject gameObject = Util.KInstantiate(this.Placer, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
			Grid.Objects[cell, 0] = gameObject;
			Vector3 vector = Grid.CellToPosCBC(cell, this.visualizerLayer);
			float depthBias = InterfaceTool.DepthBias;
			vector.z += depthBias;
			gameObject.transform.SetPosition(vector);
			this.SelectedCells.Add(cell);
		}
	}

	public void RemoveFromSelection(int cell)
	{
		if (this.SelectedCells.Contains(cell))
		{
			GameObject gameObject = Grid.Objects[cell, 0];
			if (gameObject != null)
			{
				gameObject.DeleteObject();
			}
			this.SelectedCells.Remove(cell);
		}
	}

	private bool AutoSaveRoomContents;

	private bool SaveAllBuildings;

	private bool SaveAllPickups;

	public KButton saveBaseButton;

	public KButton clearButton;

	public KButton pasteStartingBaseButton;

	private BaseTemplate pasteAndSelectAsset;

	public KButton AddSelectionButton;

	public KButton RemoveSelectionButton;

	public KButton clearSelectionButton;

	public KButton DestroyButton;

	public KButton DeconstructButton;

	public KButton MoveButton;

	public BaseTemplate moveAsset;

	public InputField nameField;

	public string SaveLocation = "Assets/tuning/Bases";

	private string SaveName = "startingBase";

	public GameObject Placer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	private List<int> SelectedCells = new List<int>();
}
