using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
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

	protected override void OnDisable()
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
			list.Add(new Cell(num7 - rootX, num8 - rootY, num6));
		}
		for (int j = 0; j < Components.BuildingCompletes.Count; j++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[j];
			if (!hashSet.Contains(buildingComplete.gameObject))
			{
				int num9 = Grid.PosToCell(buildingComplete);
				int num10;
				int num11;
				Grid.CellToXY(num9, out num10, out num11);
				if (this.SaveAllBuildings || this.SelectedCells.Contains(num9))
				{
					int[] placementCells = buildingComplete.PlacementCells;
					string text;
					for (int k = 0; k < placementCells.Length; k++)
					{
						int num12 = placementCells[k];
						int xplace;
						int yplace;
						Grid.CellToXY(num12, out xplace, out yplace);
						text = ((Grid.DiseaseIdx[num12] != byte.MaxValue) ? Db.Get().Diseases[(int)Grid.DiseaseIdx[num12]].Id : null);
						if (list.Find((Cell c) => c.location_x == xplace - rootX && c.location_y == yplace - rootY) == null)
						{
							list.Add(new Cell(xplace - rootX, yplace - rootY, Grid.Element[num12].id, Grid.Temperature[num12], Grid.Mass[num12], text, Grid.DiseaseCount[num12], false, SimHashes.Vacuum, 0f, 0f));
						}
					}
					Orientation orientation = Orientation.Neutral;
					Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
					if (component != null)
					{
						orientation = component.GetOrientation();
					}
					SimHashes simHashes = SimHashes.Void;
					float num13 = 280f;
					text = null;
					int num14 = 0;
					PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
					buildingComplete.GetComponent<KPrefabID>();
					if (component2 != null)
					{
						simHashes = component2.ElementID;
						num13 = component2.Temperature;
						text = ((component2.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component2.DiseaseIdx].Id : null);
						num14 = component2.DiseaseCount;
					}
					List<Prefab.template_amount_value> list6 = new List<Prefab.template_amount_value>();
					List<Prefab.template_amount_value> list7 = new List<Prefab.template_amount_value>();
					foreach (AmountInstance amountInstance in buildingComplete.gameObject.GetAmounts().ModifierList)
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
						float num15 = (float)(component4.facingRight ? 1 : 0);
						list7.Add(new Prefab.template_amount_value("sealedDoorDirection", num15));
					}
					LogicSwitch component5 = buildingComplete.GetComponent<LogicSwitch>();
					if (component5 != null)
					{
						float num16 = (float)(component5.IsSwitchedOn ? 1 : 0);
						list7.Add(new Prefab.template_amount_value("switchSetting", num16));
					}
					int num17 = 0;
					IHaveUtilityNetworkMgr component6 = buildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
					if (component6 != null)
					{
						num17 = (int)component6.GetNetworkManager().GetConnections(num9, true);
					}
					string text2 = null;
					BuildingFacade component7 = buildingComplete.GetComponent<BuildingFacade>();
					if (component7 != null)
					{
						text2 = component7.CurrentFacade;
					}
					num10 -= rootX;
					num11 -= rootY;
					num13 = Mathf.Clamp(num13, 1f, 99999f);
					Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, num10, num11, simHashes, num13, 0f, text, num14, orientation, list6.ToArray(), list7.ToArray(), num17, text2);
					LoreBearer component8 = buildingComplete.GetComponent<LoreBearer>();
					if (component8 != null && !string.IsNullOrEmpty(component8.poiOverrideLoreUnlockId))
					{
						prefab.loreUnlockId = component8.poiOverrideLoreUnlockId;
						prefab.loreDisplayText = component8.poiOverrideLoreDisplayText;
						prefab.loreNextCollectionId = component8.poiOverrideNextCollectionId;
					}
					Storage component9 = buildingComplete.gameObject.GetComponent<Storage>();
					if (component9 != null)
					{
						foreach (GameObject gameObject in component9.items)
						{
							float num18 = 0f;
							SimHashes simHashes2 = SimHashes.Vacuum;
							float num19 = 280f;
							string text3 = null;
							int num20 = 0;
							bool flag = false;
							PrimaryElement component10 = gameObject.GetComponent<PrimaryElement>();
							if (component10 != null)
							{
								num18 = component10.Units;
								simHashes2 = component10.ElementID;
								num19 = component10.Temperature;
								text3 = ((component10.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component10.DiseaseIdx].Id : null);
								num20 = component10.DiseaseCount;
							}
							global::Rottable.Instance smi = gameObject.gameObject.GetSMI<global::Rottable.Instance>();
							if (gameObject.GetComponent<ElementChunk>() != null)
							{
								flag = true;
							}
							StorageItem storageItem = new StorageItem(gameObject.PrefabID().Name, num18, num19, simHashes2, text3, num20, flag);
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
					int num21 = Grid.PosToCell(pickupable);
					if ((this.SaveAllPickups || this.SelectedCells.Contains(num21)) && !Components.Pickupables[l].gameObject.GetComponent<MinionBrain>())
					{
						int num22;
						int num23;
						Grid.CellToXY(num21, out num22, out num23);
						num22 -= rootX;
						num23 -= rootY;
						SimHashes simHashes3 = SimHashes.Void;
						float num24 = 280f;
						float num25 = 1f;
						string text4 = null;
						int num26 = 0;
						float num27 = 0f;
						global::Rottable.Instance smi2 = pickupable.gameObject.GetSMI<global::Rottable.Instance>();
						if (smi2 != null)
						{
							num27 = smi2.RotValue;
						}
						PrimaryElement component11 = pickupable.gameObject.GetComponent<PrimaryElement>();
						if (component11 != null)
						{
							simHashes3 = component11.ElementID;
							num25 = component11.Units;
							num24 = component11.Temperature;
							text4 = ((component11.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component11.DiseaseIdx].Id : null);
							num26 = component11.DiseaseCount;
						}
						if (pickupable.gameObject.GetComponent<ElementChunk>() != null)
						{
							Prefab prefab2 = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, num22, num23, simHashes3, num24, num25, text4, num26, Orientation.Neutral, null, null, 0, null);
							list4.Add(prefab2);
						}
						else
						{
							list3.Add(new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, num22, num23, simHashes3, num24, num25, text4, num26, Orientation.Neutral, null, null, 0, null)
							{
								rottable = new global::TemplateClasses.Rottable(),
								rottable = 
								{
									rotAmount = num27
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
		list5.RemoveAll((Prefab x) => Assets.GetPrefab(x.id).HasTag(GameTags.ExcludeFromTemplate));
		TemplateContainer templateContainer = new TemplateContainer();
		templateContainer.Init(list, list2, list3, list4, list5);
		return templateContainer;
	}

	private void GetEntities<T>(int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		object[] array = global::UnityEngine.Object.FindObjectsByType(typeof(T), FindObjectsSortMode.InstanceID);
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
					Orientation orientation = Orientation.Neutral;
					Rotatable component = (t as KMonoBehaviour).GetComponent<Rotatable>();
					if (component != null)
					{
						orientation = component.Orientation;
					}
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
					PrimaryElement component2 = (t as KMonoBehaviour).gameObject.GetComponent<PrimaryElement>();
					if (component2 != null)
					{
						simHashes = component2.ElementID;
						num5 = component2.Units;
						num4 = component2.Temperature;
						text = ((component2.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)component2.DiseaseIdx].Id : null);
						num6 = component2.DiseaseCount;
					}
					List<Prefab.template_amount_value> list = new List<Prefab.template_amount_value>();
					if ((t as KMonoBehaviour).gameObject.GetAmounts() != null)
					{
						foreach (AmountInstance amountInstance in (t as KMonoBehaviour).gameObject.GetAmounts().ModifierList)
						{
							list.Add(new Prefab.template_amount_value(amountInstance.amount.Id, amountInstance.value));
						}
					}
					if ((t as KMonoBehaviour).gameObject.GetComponent<ElementChunk>() != null)
					{
						string name = (t as KMonoBehaviour).PrefabID().Name;
						Prefab.Type type = Prefab.Type.Ore;
						int num7 = num2;
						int num8 = num3;
						SimHashes simHashes2 = simHashes;
						float num9 = num4;
						float num10 = num5;
						string text2 = text;
						int num11 = num6;
						Prefab.template_amount_value[] array = list.ToArray();
						Prefab prefab = new Prefab(name, type, num7, num8, simHashes2, num9, num10, text2, num11, orientation, array, null, 0, null);
						_primaryElementOres.Add(prefab);
						_excludeEntities.Add((t as KMonoBehaviour).gameObject);
					}
					else
					{
						string name2 = (t as KMonoBehaviour).PrefabID().Name;
						Prefab.Type type2 = Prefab.Type.Other;
						int num12 = num2;
						int num13 = num3;
						SimHashes simHashes3 = simHashes;
						float num14 = num4;
						float num15 = num5;
						string text3 = text;
						int num16 = num6;
						Prefab.template_amount_value[] array = list.ToArray();
						Prefab prefab = new Prefab(name2, type2, num12, num13, simHashes3, num14, num15, text3, num16, orientation, array, null, 0, null);
						LoreBearer component3 = (t as KMonoBehaviour).gameObject.GetComponent<LoreBearer>();
						if (component3 != null && !string.IsNullOrEmpty(component3.poiOverrideLoreUnlockId))
						{
							prefab.loreUnlockId = component3.poiOverrideLoreUnlockId;
							prefab.loreDisplayText = component3.poiOverrideLoreDisplayText;
							prefab.loreNextCollectionId = component3.poiOverrideNextCollectionId;
						}
						_otherEntities.Add(prefab);
						_excludeEntities.Add((t as KMonoBehaviour).gameObject);
					}
				}
			}
		}
	}

	private static int CountLoreOverrides(TemplateContainer template)
	{
		int num = 0;
		if (template.buildings != null)
		{
			using (List<Prefab>.Enumerator enumerator = template.buildings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!string.IsNullOrEmpty(enumerator.Current.loreUnlockId))
					{
						num++;
					}
				}
			}
		}
		if (template.otherEntities != null)
		{
			using (List<Prefab>.Enumerator enumerator = template.otherEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!string.IsNullOrEmpty(enumerator.Current.loreUnlockId))
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private void OnClickSaveBase()
	{
		TemplateContainer asset = this.GetSelectionAsAsset();
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
		if (TemplateCache.TemplateExists(this.SaveName))
		{
			int num = DebugBaseTemplateButton.CountLoreOverrides(TemplateCache.GetTemplate(this.SaveName));
			int num2 = DebugBaseTemplateButton.CountLoreOverrides(asset);
			if (num2 < num)
			{
				Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true).PopupConfirmDialog(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.LORE_OVERRIDE_WARNING.Replace("{newCount}", num2.ToString()).Replace("{oldCount}", num.ToString()), delegate
				{
					this.DoSaveTemplate(asset);
				}, delegate
				{
				}, null, null, null, null, null, null);
				return;
			}
		}
		this.DoSaveTemplate(asset);
	}

	private void DoSaveTemplate(TemplateContainer asset)
	{
		asset.SaveToYaml(this.SaveName);
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
