using System;
using System.Collections.Generic;
using Klei.AI;
using TemplateClasses;
using TMPro;
using UnityEngine;

public class DebugBaseTemplateButton : KScreen
{
	public static DebugBaseTemplateButton Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DebugBaseTemplateButton.Instance = this;
		base.gameObject.SetActive(false);
		this.SetupLocText();
		this.ConsumeMouseScroll = true;
		TMP_InputField tmp_InputField = this.nameField;
		tmp_InputField.onFocus = (global::System.Action)Delegate.Combine(tmp_InputField.onFocus, new global::System.Action(delegate
		{
			this.editing = true;
		}));
		this.nameField.onEndEdit.AddListener(delegate
		{
			this.editing = false;
		});
		this.nameField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(this.nameField, true);
		});
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.ConsumeMouseScroll = true;
	}

	public override float GetSortKey()
	{
		return 10f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.editing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.saveBaseButton != null)
		{
			this.saveBaseButton.onClick -= this.OnClickSaveBase;
			this.saveBaseButton.onClick += this.OnClickSaveBase;
		}
		if (this.clearButton != null)
		{
			this.clearButton.onClick -= this.OnClickClear;
			this.clearButton.onClick += this.OnClickClear;
		}
		if (this.AddSelectionButton != null)
		{
			this.AddSelectionButton.onClick -= this.OnClickAddSelection;
			this.AddSelectionButton.onClick += this.OnClickAddSelection;
		}
		if (this.RemoveSelectionButton != null)
		{
			this.RemoveSelectionButton.onClick -= this.OnClickRemoveSelection;
			this.RemoveSelectionButton.onClick += this.OnClickRemoveSelection;
		}
		if (this.clearSelectionButton != null)
		{
			this.clearSelectionButton.onClick -= this.OnClickClearSelection;
			this.clearSelectionButton.onClick += this.OnClickClearSelection;
		}
		if (this.MoveButton != null)
		{
			this.MoveButton.onClick -= this.OnClickMove;
			this.MoveButton.onClick += this.OnClickMove;
		}
		if (this.DestroyButton != null)
		{
			this.DestroyButton.onClick -= this.OnClickDestroySelection;
			this.DestroyButton.onClick += this.OnClickDestroySelection;
		}
		if (this.DeconstructButton != null)
		{
			this.DeconstructButton.onClick -= this.OnClickDeconstructSelection;
			this.DeconstructButton.onClick += this.OnClickDeconstructSelection;
		}
	}

	private void SetupLocText()
	{
	}

	private void OnClickPasteStartingBase()
	{
		DebugTool.Instance.DeactivateTool(null);
		this.pasteAndSelectAsset = null;
		this.pasteAndSelectAsset = TemplateCache.GetBaseStartingTemplate();
		if (this.pasteAndSelectAsset == null)
		{
			return;
		}
		this.ClearSelection();
		StampTool.Instance.Activate(this.pasteAndSelectAsset, true, false);
		this.nameField.text = this.pasteAndSelectAsset.name;
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
		this.nameField.text = string.Empty;
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

	private TemplateContainer GetSelectionAsAsset()
	{
		List<Cell> list = new List<Cell>();
		List<Prefab> list2 = new List<Prefab>();
		List<Prefab> list3 = new List<Prefab>();
		List<Prefab> list4 = new List<Prefab>();
		List<Prefab> list5 = new List<Prefab>();
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
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
		for (int i = 0; i < this.SelectedCells.Count; i++)
		{
			Sim.Cell cell = Grid.Cell[this.SelectedCells[i]];
			Sim.DiseaseCell diseaseCell = Grid.Disease[this.SelectedCells[i]];
			int num9;
			int num10;
			Grid.CellToXY(this.SelectedCells[i], out num9, out num10);
			Element element = ElementLoader.elements[(int)Grid.Cell[this.SelectedCells[i]].elementIdx];
			string text = ((diseaseCell.diseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)diseaseCell.diseaseIdx].Id);
			list.Add(new Cell(num9 - num7, num10 - num8, element.id, cell.temperature, cell.mass, text, diseaseCell.elementCount, Grid.PreventFogOfWarReveal[this.SelectedCells[i]]));
		}
		for (int j = 0; j < Components.BuildingCompletes.Count; j++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[j];
			if (!hashSet.Contains(buildingComplete.gameObject))
			{
				int num11;
				int num12;
				Grid.CellToXY(Grid.PosToCell(buildingComplete), out num11, out num12);
				if (this.SaveAllBuildings || this.SelectedCells.Contains(Grid.PosToCell(buildingComplete)))
				{
					string text2;
					foreach (int num13 in buildingComplete.PlacementCells)
					{
						Sim.Cell cell2 = Grid.Cell[num13];
						Sim.DiseaseCell diseaseCell2 = Grid.Disease[num13];
						int num14;
						int num15;
						Grid.CellToXY(num13, out num14, out num15);
						text2 = ((diseaseCell2.diseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)diseaseCell2.diseaseIdx].Id);
						list.Add(new Cell(num14 - num7, num15 - num8, Grid.Element[num13].id, cell2.temperature, cell2.mass, text2, diseaseCell2.elementCount, false));
					}
					Orientation orientation = Orientation.Neutral;
					Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
					if (component != null)
					{
						orientation = component.GetOrientation();
					}
					SimHashes simHashes = SimHashes.Void;
					float num16 = 280f;
					text2 = null;
					int num17 = 0;
					PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
					if (component2 != null)
					{
						simHashes = component2.ElementID;
						num16 = component2.Temperature;
						text2 = ((component2.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component2.DiseaseIdx].Id);
						num17 = component2.DiseaseCount;
					}
					List<Prefab.template_amount_value> list6 = new List<Prefab.template_amount_value>();
					List<Prefab.template_amount_value> list7 = new List<Prefab.template_amount_value>();
					foreach (AmountInstance amountInstance in buildingComplete.gameObject.GetAmounts())
					{
						list6.Add(new Prefab.template_amount_value(amountInstance.amount.Id, amountInstance.value));
					}
					Battery component3 = buildingComplete.GetComponent<Battery>();
					if (component3 != null)
					{
						float joulesAvailable = component3.JoulesAvailable;
						list7.Add(new Prefab.template_amount_value("joulesAvailable", joulesAvailable));
					}
					Unsealable component4 = buildingComplete.GetComponent<Unsealable>();
					if (component4 != null)
					{
						float num18 = (float)((!component4.facingRight) ? 0 : 1);
						list7.Add(new Prefab.template_amount_value("sealedDoorDirection", num18));
					}
					num11 -= num7;
					num12 -= num8;
					num16 = Mathf.Clamp(num16, 1f, 99999f);
					Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, num11, num12, simHashes, num16, 0f, text2, num17, orientation, list6.ToArray(), list7.ToArray(), 0);
					Storage component5 = buildingComplete.gameObject.GetComponent<Storage>();
					if (component5 != null)
					{
						foreach (GameObject gameObject in component5.items)
						{
							float num19 = 0f;
							SimHashes simHashes2 = SimHashes.Vacuum;
							float num20 = 280f;
							string text3 = null;
							int num21 = 0;
							bool flag = false;
							PrimaryElement component6 = gameObject.GetComponent<PrimaryElement>();
							if (component6 != null)
							{
								num19 = component6.Units;
								simHashes2 = component6.ElementID;
								num20 = component6.Temperature;
								text3 = ((component6.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component6.DiseaseIdx].Id);
								num21 = component6.DiseaseCount;
							}
							float num22 = 0f;
							global::Rottable.Instance smi = gameObject.gameObject.GetSMI<global::Rottable.Instance>();
							if (smi != null)
							{
								num22 = smi.RotValue;
							}
							ElementChunk component7 = gameObject.GetComponent<ElementChunk>();
							if (component7 != null)
							{
								flag = true;
							}
							StorageItem storageItem = new StorageItem(gameObject.PrefabID().Name, num19, num20, simHashes2, text3, num21, flag);
							if (smi != null)
							{
								storageItem.rottable.rotAmount = num22;
							}
							prefab.AssignStorage(storageItem);
							hashSet.Add(gameObject);
						}
					}
					list2.Add(prefab);
					hashSet.Add(buildingComplete.gameObject);
				}
			}
		}
		int l = 0;
		while (l < list2.Count)
		{
			Prefab prefab2 = list2[l];
			int num23 = prefab2.location_x + num7;
			int num24 = prefab2.location_y + num8;
			int num25 = Grid.XYToCell(num23, num24);
			string id = prefab2.id;
			if (id == null)
			{
				goto IL_0771;
			}
			if (DebugBaseTemplateButton.<>f__switch$map3 == null)
			{
				DebugBaseTemplateButton.<>f__switch$map3 = new Dictionary<string, int>(7)
				{
					{ "Wire", 0 },
					{ "InsulatedWire", 0 },
					{ "HighWattageWire", 0 },
					{ "GasConduit", 1 },
					{ "InsulatedGasConduit", 1 },
					{ "LiquidConduit", 2 },
					{ "InsulatedLiquidConduit", 2 }
				};
			}
			int num26;
			if (DebugBaseTemplateButton.<>f__switch$map3.TryGetValue(id, out num26))
			{
				switch (num26)
				{
				case 0:
					prefab2.connections = (int)Game.Instance.electricalConduitSystem.GetConnections(num25, true);
					goto IL_07D8;
				case 1:
					prefab2.connections = (int)Game.Instance.gasConduitSystem.GetConnections(num25, true);
					goto IL_07D8;
				case 2:
					prefab2.connections = (int)Game.Instance.liquidConduitSystem.GetConnections(num25, true);
					goto IL_07D8;
				}
				goto IL_0771;
			}
			goto IL_0771;
			IL_07D8:
			l++;
			continue;
			IL_0771:
			prefab2.connections = 0;
			goto IL_07D8;
		}
		for (int m = 0; m < Components.Pickupables.Count; m++)
		{
			if (Components.Pickupables[m].gameObject.activeSelf)
			{
				Pickupable pickupable = Components.Pickupables[m];
				if (!hashSet.Contains(pickupable.gameObject))
				{
					int num27 = Grid.PosToCell(pickupable);
					if (this.SaveAllPickups || this.SelectedCells.Contains(num27))
					{
						if (!Components.Pickupables[m].gameObject.GetComponent<MinionBrain>())
						{
							int num28;
							int num29;
							Grid.CellToXY(num27, out num28, out num29);
							num28 -= num7;
							num29 -= num8;
							SimHashes simHashes3 = SimHashes.Void;
							float num30 = 280f;
							float num31 = 1f;
							string text4 = null;
							int num32 = 0;
							float num33 = 0f;
							global::Rottable.Instance smi2 = pickupable.gameObject.GetSMI<global::Rottable.Instance>();
							if (smi2 != null)
							{
								num33 = smi2.RotValue;
							}
							PrimaryElement component8 = pickupable.gameObject.GetComponent<PrimaryElement>();
							if (component8 != null)
							{
								simHashes3 = component8.ElementID;
								num31 = component8.Units;
								num30 = component8.Temperature;
								text4 = ((component8.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component8.DiseaseIdx].Id);
								num32 = component8.DiseaseCount;
							}
							ElementChunk component9 = pickupable.gameObject.GetComponent<ElementChunk>();
							if (component9 != null)
							{
								Prefab prefab3 = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, num28, num29, simHashes3, num30, num31, text4, num32, Orientation.Neutral, null, null, 0);
								list4.Add(prefab3);
							}
							else
							{
								list3.Add(new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, num28, num29, simHashes3, num30, num31, text4, num32, Orientation.Neutral, null, null, 0)
								{
									rottable = new global::TemplateClasses.Rottable(),
									rottable = 
									{
										rotAmount = num33
									}
								});
							}
							hashSet.Add(pickupable.gameObject);
						}
					}
				}
			}
		}
		this.GetEntities<Crop>(Components.Crops, num7, num8, ref list4, ref list5, ref hashSet);
		this.GetEntities<Health>(Components.Health, num7, num8, ref list4, ref list5, ref hashSet);
		this.GetEntities<Harvestable>(Components.Harvestables, num7, num8, ref list4, ref list5, ref hashSet);
		this.GetEntities<Edible>(Components.Edibles, num7, num8, ref list4, ref list5, ref hashSet);
		this.GetEntities<Geyser>(num7, num8, ref list4, ref list5, ref hashSet);
		this.GetEntities<OccupyArea>(num7, num8, ref list4, ref list5, ref hashSet);
		this.GetEntities<FogOfWarMask>(num7, num8, ref list4, ref list5, ref hashSet);
		TemplateContainer templateContainer = new TemplateContainer();
		templateContainer.Init(list, list2, list3, list4, list5);
		return templateContainer;
	}

	private void GetEntities<T>(int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		object[] array = global::UnityEngine.Object.FindObjectsOfType(typeof(T));
		this.GetEntities<object>(array, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
	}

	private void GetEntities<T>(IEnumerable<T> component_collection, int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		foreach (T t in component_collection)
		{
			if (!_excludeEntities.Contains((t as KMonoBehaviour).gameObject))
			{
				if ((t as KMonoBehaviour).gameObject.activeSelf)
				{
					int num = Grid.PosToCell(t as KMonoBehaviour);
					if (this.SelectedCells.Contains(num))
					{
						if (!(t as KMonoBehaviour).gameObject.GetComponent<MinionBrain>())
						{
							int num2;
							int num3;
							Grid.CellToXY(num, out num2, out num3);
							num2 -= rootX;
							num3 -= rootY;
							SimHashes simHashes = SimHashes.Void;
							float num4 = 280f;
							float num5 = 1f;
							string text = null;
							int num6 = 0;
							PrimaryElement component = (t as KMonoBehaviour).gameObject.GetComponent<PrimaryElement>();
							if (component != null)
							{
								simHashes = component.ElementID;
								num5 = component.Units;
								num4 = component.Temperature;
								text = ((component.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component.DiseaseIdx].Id);
								num6 = component.DiseaseCount;
							}
							List<Prefab.template_amount_value> list = new List<Prefab.template_amount_value>();
							if ((t as KMonoBehaviour).gameObject.GetAmounts() != null)
							{
								foreach (AmountInstance amountInstance in (t as KMonoBehaviour).gameObject.GetAmounts())
								{
									list.Add(new Prefab.template_amount_value(amountInstance.amount.Id, amountInstance.value));
								}
							}
							ElementChunk component2 = (t as KMonoBehaviour).gameObject.GetComponent<ElementChunk>();
							if (component2 != null)
							{
								Prefab.template_amount_value[] array = list.ToArray();
								Prefab prefab = new Prefab((t as KMonoBehaviour).PrefabID().Name, Prefab.Type.Ore, num2, num3, simHashes, num4, num5, text, num6, Orientation.Neutral, array, null, 0);
								_primaryElementOres.Add(prefab);
								_excludeEntities.Add((t as KMonoBehaviour).gameObject);
							}
							else
							{
								Prefab.template_amount_value[] array = list.ToArray();
								Prefab prefab = new Prefab((t as KMonoBehaviour).PrefabID().Name, Prefab.Type.Other, num2, num3, simHashes, num4, num5, text, num6, Orientation.Neutral, array, null, 0);
								_otherEntities.Add(prefab);
								_excludeEntities.Add((t as KMonoBehaviour).gameObject);
							}
						}
					}
				}
			}
		}
	}

	private void OnClickSaveBase()
	{
		TemplateContainer selectionAsAsset = this.GetSelectionAsAsset();
		if (this.SelectedCells.Count <= 0)
		{
			global::Debug.LogWarning("No cells selected. Use buttons above to select the area you want to save.", null);
			return;
		}
		this.SaveName = this.nameField.text;
		if (this.SaveName == null || this.SaveName == string.Empty)
		{
			global::Debug.LogWarning("Invalid save name. Please enter a name in the input field.", null);
			return;
		}
		selectionAsAsset.SaveToYaml(this.SaveName);
		PasteBaseTemplateScreen.Instance.RefreshStampButtons();
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
			Grid.Objects[cell, 7] = gameObject;
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
			GameObject gameObject = Grid.Objects[cell, 7];
			if (gameObject != null)
			{
				gameObject.DeleteObject();
			}
			this.SelectedCells.Remove(cell);
		}
	}

	private bool SaveAllBuildings;

	private bool SaveAllPickups;

	public KButton saveBaseButton;

	public KButton clearButton;

	private TemplateContainer pasteAndSelectAsset;

	public KButton AddSelectionButton;

	public KButton RemoveSelectionButton;

	public KButton clearSelectionButton;

	public KButton DestroyButton;

	public KButton DeconstructButton;

	public KButton MoveButton;

	public TemplateContainer moveAsset;

	public TMP_InputField nameField;

	private bool editing;

	private string SaveName = "enter_template_name";

	public GameObject Placer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public List<int> SelectedCells = new List<int>();
}
