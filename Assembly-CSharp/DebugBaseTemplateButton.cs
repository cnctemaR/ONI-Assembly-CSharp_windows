using System;
using System.Collections.Generic;
using Klei.AI;
using TemplateClasses;
using TMPro;
using UnityEngine;

public class DebugBaseTemplateButton : KScreen
{
	public static DebugBaseTemplateButton Instance { get; private set; }

	public static void DestroyInstance()
	{
		DebugBaseTemplateButton.Instance = null;
	}

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
			return;
		}
		base.OnKeyDown(e);
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
		this.nameField.text = "";
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
		int num6;
		int num7;
		Grid.CellToXY(Grid.PosToCell(new Vector3(num4, num5, 0f)), out num6, out num7);
		for (int i = 0; i < this.SelectedCells.Count; i++)
		{
			int num8 = this.SelectedCells[i];
			int num9;
			int num10;
			Grid.CellToXY(this.SelectedCells[i], out num9, out num10);
			Element element = ElementLoader.elements[(int)Grid.ElementIdx[num8]];
			string text = ((Grid.DiseaseIdx[num8] != byte.MaxValue) ? Db.Get().Diseases[(int)Grid.DiseaseIdx[num8]].Id : null);
			list.Add(new Cell(num9 - num6, num10 - num7, element.id, Grid.Temperature[num8], Grid.Mass[num8], text, Grid.DiseaseCount[num8], Grid.PreventFogOfWarReveal[this.SelectedCells[i]]));
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
						int num14;
						int num15;
						Grid.CellToXY(num13, out num14, out num15);
						text2 = ((Grid.DiseaseIdx[num13] != byte.MaxValue) ? Db.Get().Diseases[(int)Grid.DiseaseIdx[num13]].Id : null);
						list.Add(new Cell(num14 - num6, num15 - num7, Grid.Element[num13].id, Grid.Temperature[num13], Grid.Mass[num13], text2, Grid.DiseaseCount[num13], false));
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
						text2 = ((component2.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component2.DiseaseIdx].Id : null);
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
						float num18 = (float)(component4.facingRight ? 1 : 0);
						list7.Add(new Prefab.template_amount_value("sealedDoorDirection", num18));
					}
					LogicSwitch component5 = buildingComplete.GetComponent<LogicSwitch>();
					if (component5 != null)
					{
						float num19 = (float)(component5.IsSwitchedOn ? 1 : 0);
						list7.Add(new Prefab.template_amount_value("switchSetting", num19));
					}
					num11 -= num6;
					num12 -= num7;
					num16 = Mathf.Clamp(num16, 1f, 99999f);
					Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, num11, num12, simHashes, num16, 0f, text2, num17, orientation, list6.ToArray(), list7.ToArray(), 0);
					Storage component6 = buildingComplete.gameObject.GetComponent<Storage>();
					if (component6 != null)
					{
						foreach (GameObject gameObject in component6.items)
						{
							float num20 = 0f;
							SimHashes simHashes2 = SimHashes.Vacuum;
							float num21 = 280f;
							string text3 = null;
							int num22 = 0;
							bool flag = false;
							PrimaryElement component7 = gameObject.GetComponent<PrimaryElement>();
							if (component7 != null)
							{
								num20 = component7.Units;
								simHashes2 = component7.ElementID;
								num21 = component7.Temperature;
								text3 = ((component7.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component7.DiseaseIdx].Id : null);
								num22 = component7.DiseaseCount;
							}
							float num23 = 0f;
							global::Rottable.Instance smi = gameObject.gameObject.GetSMI<global::Rottable.Instance>();
							if (smi != null)
							{
								num23 = smi.RotValue;
							}
							if (gameObject.GetComponent<ElementChunk>() != null)
							{
								flag = true;
							}
							StorageItem storageItem = new StorageItem(gameObject.PrefabID().Name, num20, num21, simHashes2, text3, num22, flag);
							if (smi != null)
							{
								storageItem.rottable.rotAmount = num23;
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
			int num24 = prefab2.location_x + num6;
			int num25 = prefab2.location_y + num7;
			int num26 = Grid.XYToCell(num24, num25);
			string id = prefab2.id;
			uint num27 = <PrivateImplementationDetails>.ComputeStringHash(id);
			if (num27 <= 1827504487U)
			{
				if (num27 <= 609727380U)
				{
					if (num27 != 379600269U)
					{
						if (num27 != 609727380U)
						{
							goto IL_0783;
						}
						if (!(id == "GasConduit"))
						{
							goto IL_0783;
						}
					}
					else
					{
						if (!(id == "LiquidConduit"))
						{
							goto IL_0783;
						}
						goto IL_07C3;
					}
				}
				else if (num27 != 848332507U)
				{
					if (num27 != 1827504487U)
					{
						goto IL_0783;
					}
					if (!(id == "InsulatedWire"))
					{
						goto IL_0783;
					}
					goto IL_078D;
				}
				else if (!(id == "InsulatedGasConduit"))
				{
					goto IL_0783;
				}
				prefab2.connections = (int)Game.Instance.gasConduitSystem.GetConnections(num26, true);
			}
			else if (num27 <= 3228988836U)
			{
				if (num27 != 1938276536U)
				{
					if (num27 != 3228988836U)
					{
						goto IL_0783;
					}
					if (!(id == "LogicWire"))
					{
						goto IL_0783;
					}
					prefab2.connections = (int)Game.Instance.logicCircuitSystem.GetConnections(num26, true);
				}
				else
				{
					if (!(id == "Wire"))
					{
						goto IL_0783;
					}
					goto IL_078D;
				}
			}
			else if (num27 != 3711470516U)
			{
				if (num27 != 3716494409U)
				{
					goto IL_0783;
				}
				if (!(id == "HighWattageWire"))
				{
					goto IL_0783;
				}
				goto IL_078D;
			}
			else
			{
				if (!(id == "InsulatedLiquidConduit"))
				{
					goto IL_0783;
				}
				goto IL_07C3;
			}
			IL_07F7:
			l++;
			continue;
			IL_0783:
			prefab2.connections = 0;
			goto IL_07F7;
			IL_078D:
			prefab2.connections = (int)Game.Instance.electricalConduitSystem.GetConnections(num26, true);
			goto IL_07F7;
			IL_07C3:
			prefab2.connections = (int)Game.Instance.liquidConduitSystem.GetConnections(num26, true);
			goto IL_07F7;
		}
		for (int m = 0; m < Components.Pickupables.Count; m++)
		{
			if (Components.Pickupables[m].gameObject.activeSelf)
			{
				Pickupable pickupable = Components.Pickupables[m];
				if (!hashSet.Contains(pickupable.gameObject))
				{
					int num28 = Grid.PosToCell(pickupable);
					if ((this.SaveAllPickups || this.SelectedCells.Contains(num28)) && !Components.Pickupables[m].gameObject.GetComponent<MinionBrain>())
					{
						int num29;
						int num30;
						Grid.CellToXY(num28, out num29, out num30);
						num29 -= num6;
						num30 -= num7;
						SimHashes simHashes3 = SimHashes.Void;
						float num31 = 280f;
						float num32 = 1f;
						string text4 = null;
						int num33 = 0;
						float num34 = 0f;
						global::Rottable.Instance smi2 = pickupable.gameObject.GetSMI<global::Rottable.Instance>();
						if (smi2 != null)
						{
							num34 = smi2.RotValue;
						}
						PrimaryElement component8 = pickupable.gameObject.GetComponent<PrimaryElement>();
						if (component8 != null)
						{
							simHashes3 = component8.ElementID;
							num32 = component8.Units;
							num31 = component8.Temperature;
							text4 = ((component8.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component8.DiseaseIdx].Id : null);
							num33 = component8.DiseaseCount;
						}
						if (pickupable.gameObject.GetComponent<ElementChunk>() != null)
						{
							Prefab prefab3 = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, num29, num30, simHashes3, num31, num32, text4, num33, Orientation.Neutral, null, null, 0);
							list4.Add(prefab3);
						}
						else
						{
							list3.Add(new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, num29, num30, simHashes3, num31, num32, text4, num33, Orientation.Neutral, null, null, 0)
							{
								rottable = new global::TemplateClasses.Rottable(),
								rottable = 
								{
									rotAmount = num34
								}
							});
						}
						hashSet.Add(pickupable.gameObject);
					}
				}
			}
		}
		this.GetEntities<Crop>(Components.Crops.Items, num6, num7, ref list4, ref list5, ref hashSet);
		this.GetEntities<Health>(Components.Health.Items, num6, num7, ref list4, ref list5, ref hashSet);
		this.GetEntities<Harvestable>(Components.Harvestables.Items, num6, num7, ref list4, ref list5, ref hashSet);
		this.GetEntities<Edible>(Components.Edibles.Items, num6, num7, ref list4, ref list5, ref hashSet);
		this.GetEntities<Geyser>(num6, num7, ref list4, ref list5, ref hashSet);
		this.GetEntities<OccupyArea>(num6, num7, ref list4, ref list5, ref hashSet);
		this.GetEntities<FogOfWarMask>(num6, num7, ref list4, ref list5, ref hashSet);
		TemplateContainer templateContainer = new TemplateContainer();
		templateContainer.Init(list, list2, list3, list4, list5);
		return templateContainer;
	}

	private void GetEntities<T>(int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		object[] array = global::UnityEngine.Object.FindObjectsOfType(typeof(T));
		object[] array2 = array;
		this.GetEntities<object>(array2, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
	}

	private void GetEntities<T>(IEnumerable<T> component_collection, int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		foreach (T t in component_collection)
		{
			if (!_excludeEntities.Contains((t as KMonoBehaviour).gameObject) && (t as KMonoBehaviour).gameObject.activeSelf)
			{
				int num = Grid.PosToCell(t as KMonoBehaviour);
				if (this.SelectedCells.Contains(num) && !(t as KMonoBehaviour).gameObject.GetComponent<MinionBrain>())
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
						text = ((component.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component.DiseaseIdx].Id : null);
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
					if ((t as KMonoBehaviour).gameObject.GetComponent<ElementChunk>() != null)
					{
						Prefab prefab = new Prefab((t as KMonoBehaviour).PrefabID().Name, Prefab.Type.Ore, num2, num3, simHashes, num4, num5, text, num6, Orientation.Neutral, list.ToArray(), null, 0);
						_primaryElementOres.Add(prefab);
						_excludeEntities.Add((t as KMonoBehaviour).gameObject);
					}
					else
					{
						Prefab prefab = new Prefab((t as KMonoBehaviour).PrefabID().Name, Prefab.Type.Other, num2, num3, simHashes, num4, num5, text, num6, Orientation.Neutral, list.ToArray(), null, 0);
						_otherEntities.Add(prefab);
						_excludeEntities.Add((t as KMonoBehaviour).gameObject);
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
			global::Debug.LogWarning("No cells selected. Use buttons above to select the area you want to save.");
			return;
		}
		this.SaveName = this.nameField.text;
		if (this.SaveName == null || this.SaveName == "")
		{
			global::Debug.LogWarning("Invalid save name. Please enter a name in the input field.");
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
			GameObject gameObject = Util.KInstantiate(this.Placer, null, null);
			Grid.Objects[cell, 7] = gameObject;
			Vector3 vector = Grid.CellToPosCBC(cell, this.visualizerLayer);
			float num = -0.15f;
			vector.z += num;
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
