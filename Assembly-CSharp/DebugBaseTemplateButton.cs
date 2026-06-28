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
			int num9 = this.SelectedCells[i];
			int num10;
			int num11;
			Grid.CellToXY(this.SelectedCells[i], out num10, out num11);
			Element element = ElementLoader.elements[(int)Grid.ElementIdx[num9]];
			string text = ((Grid.DiseaseIdx[num9] == byte.MaxValue) ? null : Db.Get().Diseases[(int)Grid.DiseaseIdx[num9]].Id);
			list.Add(new Cell(num10 - num7, num11 - num8, element.id, Grid.Temperature[num9], Grid.Mass[num9], text, Grid.DiseaseCount[num9], Grid.PreventFogOfWarReveal[this.SelectedCells[i]]));
		}
		for (int j = 0; j < Components.BuildingCompletes.Count; j++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[j];
			if (!hashSet.Contains(buildingComplete.gameObject))
			{
				int num12;
				int num13;
				Grid.CellToXY(Grid.PosToCell(buildingComplete), out num12, out num13);
				if (this.SaveAllBuildings || this.SelectedCells.Contains(Grid.PosToCell(buildingComplete)))
				{
					string text2;
					foreach (int num14 in buildingComplete.PlacementCells)
					{
						int num15;
						int num16;
						Grid.CellToXY(num14, out num15, out num16);
						text2 = ((Grid.DiseaseIdx[num14] == byte.MaxValue) ? null : Db.Get().Diseases[(int)Grid.DiseaseIdx[num14]].Id);
						list.Add(new Cell(num15 - num7, num16 - num8, Grid.Element[num14].id, Grid.Temperature[num14], Grid.Mass[num14], text2, Grid.DiseaseCount[num14], false));
					}
					Orientation orientation = Orientation.Neutral;
					Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
					if (component != null)
					{
						orientation = component.GetOrientation();
					}
					SimHashes simHashes = SimHashes.Void;
					float num17 = 280f;
					text2 = null;
					int num18 = 0;
					PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
					if (component2 != null)
					{
						simHashes = component2.ElementID;
						num17 = component2.Temperature;
						text2 = ((component2.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component2.DiseaseIdx].Id);
						num18 = component2.DiseaseCount;
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
						float num19 = (float)((!component4.facingRight) ? 0 : 1);
						list7.Add(new Prefab.template_amount_value("sealedDoorDirection", num19));
					}
					LogicSwitch component5 = buildingComplete.GetComponent<LogicSwitch>();
					if (component5 != null)
					{
						float num20 = (float)((!component5.IsSwitchedOn) ? 0 : 1);
						list7.Add(new Prefab.template_amount_value("switchSetting", num20));
					}
					num12 -= num7;
					num13 -= num8;
					num17 = Mathf.Clamp(num17, 1f, 99999f);
					Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, num12, num13, simHashes, num17, 0f, text2, num18, orientation, list6.ToArray(), list7.ToArray(), 0);
					Storage component6 = buildingComplete.gameObject.GetComponent<Storage>();
					if (component6 != null)
					{
						foreach (GameObject gameObject in component6.items)
						{
							float num21 = 0f;
							SimHashes simHashes2 = SimHashes.Vacuum;
							float num22 = 280f;
							string text3 = null;
							int num23 = 0;
							bool flag = false;
							PrimaryElement component7 = gameObject.GetComponent<PrimaryElement>();
							if (component7 != null)
							{
								num21 = component7.Units;
								simHashes2 = component7.ElementID;
								num22 = component7.Temperature;
								text3 = ((component7.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component7.DiseaseIdx].Id);
								num23 = component7.DiseaseCount;
							}
							float num24 = 0f;
							global::Rottable.Instance smi = gameObject.gameObject.GetSMI<global::Rottable.Instance>();
							if (smi != null)
							{
								num24 = smi.RotValue;
							}
							ElementChunk component8 = gameObject.GetComponent<ElementChunk>();
							if (component8 != null)
							{
								flag = true;
							}
							StorageItem storageItem = new StorageItem(gameObject.PrefabID().Name, num21, num22, simHashes2, text3, num23, flag);
							if (smi != null)
							{
								storageItem.rottable.rotAmount = num24;
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
			int num25 = prefab2.location_x + num7;
			int num26 = prefab2.location_y + num8;
			int num27 = Grid.XYToCell(num25, num26);
			string id = prefab2.id;
			if (id == null)
			{
				goto IL_07AF;
			}
			if (DebugBaseTemplateButton.<>f__switch$map1 == null)
			{
				DebugBaseTemplateButton.<>f__switch$map1 = new Dictionary<string, int>(8)
				{
					{ "Wire", 1 },
					{ "InsulatedWire", 1 },
					{ "HighWattageWire", 1 },
					{ "GasConduit", 2 },
					{ "InsulatedGasConduit", 2 },
					{ "LiquidConduit", 3 },
					{ "InsulatedLiquidConduit", 3 },
					{ "LogicWire", 4 }
				};
			}
			int num28;
			if (!DebugBaseTemplateButton.<>f__switch$map1.TryGetValue(id, out num28))
			{
				goto IL_07AF;
			}
			switch (num28)
			{
			default:
				goto IL_07AF;
			case 1:
				prefab2.connections = (int)Game.Instance.electricalConduitSystem.GetConnections(num27, true);
				break;
			case 2:
				prefab2.connections = (int)Game.Instance.gasConduitSystem.GetConnections(num27, true);
				break;
			case 3:
				prefab2.connections = (int)Game.Instance.liquidConduitSystem.GetConnections(num27, true);
				break;
			case 4:
				prefab2.connections = (int)Game.Instance.logicCircuitSystem.GetConnections(num27, true);
				break;
			}
			IL_0834:
			l++;
			continue;
			IL_07AF:
			prefab2.connections = 0;
			goto IL_0834;
		}
		for (int m = 0; m < Components.Pickupables.Count; m++)
		{
			if (Components.Pickupables[m].gameObject.activeSelf)
			{
				Pickupable pickupable = Components.Pickupables[m];
				if (!hashSet.Contains(pickupable.gameObject))
				{
					int num29 = Grid.PosToCell(pickupable);
					if (this.SaveAllPickups || this.SelectedCells.Contains(num29))
					{
						if (!Components.Pickupables[m].gameObject.GetComponent<MinionBrain>())
						{
							int num30;
							int num31;
							Grid.CellToXY(num29, out num30, out num31);
							num30 -= num7;
							num31 -= num8;
							SimHashes simHashes3 = SimHashes.Void;
							float num32 = 280f;
							float num33 = 1f;
							string text4 = null;
							int num34 = 0;
							float num35 = 0f;
							global::Rottable.Instance smi2 = pickupable.gameObject.GetSMI<global::Rottable.Instance>();
							if (smi2 != null)
							{
								num35 = smi2.RotValue;
							}
							PrimaryElement component9 = pickupable.gameObject.GetComponent<PrimaryElement>();
							if (component9 != null)
							{
								simHashes3 = component9.ElementID;
								num33 = component9.Units;
								num32 = component9.Temperature;
								text4 = ((component9.DiseaseIdx == byte.MaxValue) ? null : Db.Get().Diseases[(int)component9.DiseaseIdx].Id);
								num34 = component9.DiseaseCount;
							}
							ElementChunk component10 = pickupable.gameObject.GetComponent<ElementChunk>();
							if (component10 != null)
							{
								Prefab prefab3 = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, num30, num31, simHashes3, num32, num33, text4, num34, Orientation.Neutral, null, null, 0);
								list4.Add(prefab3);
							}
							else
							{
								list3.Add(new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, num30, num31, simHashes3, num32, num33, text4, num34, Orientation.Neutral, null, null, 0)
								{
									rottable = new global::TemplateClasses.Rottable(),
									rottable = 
									{
										rotAmount = num35
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
								string text2 = (t as KMonoBehaviour).PrefabID().Name;
								Prefab.Type type = Prefab.Type.Ore;
								int num7 = num2;
								int num8 = num3;
								SimHashes simHashes2 = simHashes;
								float num9 = num4;
								float num10 = num5;
								string text3 = text;
								int num11 = num6;
								Prefab.template_amount_value[] array = list.ToArray();
								Prefab prefab = new Prefab(text2, type, num7, num8, simHashes2, num9, num10, text3, num11, Orientation.Neutral, array, null, 0);
								_primaryElementOres.Add(prefab);
								_excludeEntities.Add((t as KMonoBehaviour).gameObject);
							}
							else
							{
								string text3 = (t as KMonoBehaviour).PrefabID().Name;
								Prefab.Type type = Prefab.Type.Other;
								int num11 = num2;
								int num8 = num3;
								SimHashes simHashes2 = simHashes;
								float num10 = num4;
								float num9 = num5;
								string text2 = text;
								int num7 = num6;
								Prefab.template_amount_value[] array = list.ToArray();
								Prefab prefab = new Prefab(text3, type, num11, num8, simHashes2, num10, num9, text2, num7, Orientation.Neutral, array, null, 0);
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
