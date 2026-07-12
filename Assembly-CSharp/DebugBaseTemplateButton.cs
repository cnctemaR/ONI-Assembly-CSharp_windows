using System;
using System.Collections.Generic;
using Klei.AI;
using TemplateClasses;
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
		base.ConsumeMouseScroll = true;
		KInputTextField kinputTextField = this.nameField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(delegate
		{
			base.isEditing = true;
		}));
		this.nameField.onEndEdit.AddListener(delegate
		{
			base.isEditing = false;
		});
		this.nameField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(this.nameField, true, false);
		});
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		base.ConsumeMouseScroll = true;
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
		int rootX;
		int rootY;
		Grid.CellToXY(Grid.PosToCell(new Vector3(num4, num5, 0f)), out rootX, out rootY);
		for (int i = 0; i < this.SelectedCells.Count; i++)
		{
			int num6 = this.SelectedCells[i];
			int num7;
			int num8;
			Grid.CellToXY(this.SelectedCells[i], out num7, out num8);
			Element element = ElementLoader.elements[(int)Grid.ElementIdx[num6]];
			string text = ((Grid.DiseaseIdx[num6] != byte.MaxValue) ? Db.Get().Diseases[(int)Grid.DiseaseIdx[num6]].Id : null);
			int num9 = Grid.DiseaseCount[num6];
			if (num9 <= 0)
			{
				num9 = 0;
				text = null;
			}
			list.Add(new Cell(num7 - rootX, num8 - rootY, element.id, Grid.Temperature[num6], Grid.Mass[num6], text, num9, Grid.PreventFogOfWarReveal[this.SelectedCells[i]]));
		}
		for (int j = 0; j < Components.BuildingCompletes.Count; j++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[j];
			if (!hashSet.Contains(buildingComplete.gameObject))
			{
				int num10 = Grid.PosToCell(buildingComplete);
				int num11;
				int num12;
				Grid.CellToXY(num10, out num11, out num12);
				if (this.SaveAllBuildings || this.SelectedCells.Contains(num10))
				{
					int[] placementCells = buildingComplete.PlacementCells;
					string text2;
					for (int k = 0; k < placementCells.Length; k++)
					{
						int num13 = placementCells[k];
						int xplace;
						int yplace;
						Grid.CellToXY(num13, out xplace, out yplace);
						text2 = ((Grid.DiseaseIdx[num13] != byte.MaxValue) ? Db.Get().Diseases[(int)Grid.DiseaseIdx[num13]].Id : null);
						if (list.Find((Cell c) => c.location_x == xplace - rootX && c.location_y == yplace - rootY) == null)
						{
							list.Add(new Cell(xplace - rootX, yplace - rootY, Grid.Element[num13].id, Grid.Temperature[num13], Grid.Mass[num13], text2, Grid.DiseaseCount[num13], false));
						}
					}
					Orientation orientation = Orientation.Neutral;
					Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
					if (component != null)
					{
						orientation = component.GetOrientation();
					}
					SimHashes simHashes = SimHashes.Void;
					float num14 = 280f;
					text2 = null;
					int num15 = 0;
					PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
					if (component2 != null)
					{
						simHashes = component2.ElementID;
						num14 = component2.Temperature;
						text2 = ((component2.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component2.DiseaseIdx].Id : null);
						num15 = component2.DiseaseCount;
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
						float num16 = (float)(component4.facingRight ? 1 : 0);
						list7.Add(new Prefab.template_amount_value("sealedDoorDirection", num16));
					}
					LogicSwitch component5 = buildingComplete.GetComponent<LogicSwitch>();
					if (component5 != null)
					{
						float num17 = (float)(component5.IsSwitchedOn ? 1 : 0);
						list7.Add(new Prefab.template_amount_value("switchSetting", num17));
					}
					int num18 = 0;
					IHaveUtilityNetworkMgr component6 = buildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
					if (component6 != null)
					{
						num18 = (int)component6.GetNetworkManager().GetConnections(num10, true);
					}
					num11 -= rootX;
					num12 -= rootY;
					num14 = Mathf.Clamp(num14, 1f, 99999f);
					Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, num11, num12, simHashes, num14, 0f, text2, num15, orientation, list6.ToArray(), list7.ToArray(), num18);
					Storage component7 = buildingComplete.gameObject.GetComponent<Storage>();
					if (component7 != null)
					{
						foreach (GameObject gameObject in component7.items)
						{
							float num19 = 0f;
							SimHashes simHashes2 = SimHashes.Vacuum;
							float num20 = 280f;
							string text3 = null;
							int num21 = 0;
							bool flag = false;
							PrimaryElement component8 = gameObject.GetComponent<PrimaryElement>();
							if (component8 != null)
							{
								num19 = component8.Units;
								simHashes2 = component8.ElementID;
								num20 = component8.Temperature;
								text3 = ((component8.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component8.DiseaseIdx].Id : null);
								num21 = component8.DiseaseCount;
							}
							global::Rottable.Instance smi = gameObject.gameObject.GetSMI<global::Rottable.Instance>();
							if (gameObject.GetComponent<ElementChunk>() != null)
							{
								flag = true;
							}
							StorageItem storageItem = new StorageItem(gameObject.PrefabID().Name, num19, num20, simHashes2, text3, num21, flag);
							if (smi != null)
							{
								storageItem.rottable.rotAmount = smi.RotValue;
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
		for (int l = 0; l < Components.Pickupables.Count; l++)
		{
			if (Components.Pickupables[l].gameObject.activeSelf)
			{
				Pickupable pickupable = Components.Pickupables[l];
				if (!hashSet.Contains(pickupable.gameObject))
				{
					int num22 = Grid.PosToCell(pickupable);
					if ((this.SaveAllPickups || this.SelectedCells.Contains(num22)) && !Components.Pickupables[l].gameObject.GetComponent<MinionBrain>())
					{
						int num23;
						int num24;
						Grid.CellToXY(num22, out num23, out num24);
						num23 -= rootX;
						num24 -= rootY;
						SimHashes simHashes3 = SimHashes.Void;
						float num25 = 280f;
						float num26 = 1f;
						string text4 = null;
						int num27 = 0;
						float num28 = 0f;
						global::Rottable.Instance smi2 = pickupable.gameObject.GetSMI<global::Rottable.Instance>();
						if (smi2 != null)
						{
							num28 = smi2.RotValue;
						}
						PrimaryElement component9 = pickupable.gameObject.GetComponent<PrimaryElement>();
						if (component9 != null)
						{
							simHashes3 = component9.ElementID;
							num26 = component9.Units;
							num25 = component9.Temperature;
							text4 = ((component9.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component9.DiseaseIdx].Id : null);
							num27 = component9.DiseaseCount;
						}
						if (pickupable.gameObject.GetComponent<ElementChunk>() != null)
						{
							Prefab prefab2 = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, num23, num24, simHashes3, num25, num26, text4, num27, Orientation.Neutral, null, null, 0);
							list4.Add(prefab2);
						}
						else
						{
							list3.Add(new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, num23, num24, simHashes3, num25, num26, text4, num27, Orientation.Neutral, null, null, 0)
							{
								rottable = new global::TemplateClasses.Rottable(),
								rottable = 
								{
									rotAmount = num28
								}
							});
						}
						hashSet.Add(pickupable.gameObject);
					}
				}
			}
		}
		this.GetEntities<Crop>(Components.Crops.Items, rootX, rootY, ref list4, ref list5, ref hashSet);
		this.GetEntities<Health>(Components.Health.Items, rootX, rootY, ref list4, ref list5, ref hashSet);
		this.GetEntities<Harvestable>(Components.Harvestables.Items, rootX, rootY, ref list4, ref list5, ref hashSet);
		this.GetEntities<Edible>(Components.Edibles.Items, rootX, rootY, ref list4, ref list5, ref hashSet);
		this.GetEntities<Geyser>(rootX, rootY, ref list4, ref list5, ref hashSet);
		this.GetEntities<OccupyArea>(rootX, rootY, ref list4, ref list5, ref hashSet);
		this.GetEntities<FogOfWarMask>(rootX, rootY, ref list4, ref list5, ref hashSet);
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
		TemplateCache.Clear();
		TemplateCache.Init();
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

	public KInputTextField nameField;

	private string SaveName = "enter_template_name";

	public GameObject Placer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public List<int> SelectedCells = new List<int>();
}
