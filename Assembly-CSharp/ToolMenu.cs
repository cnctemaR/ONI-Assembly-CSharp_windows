using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolMenu : KScreen
{
	public PriorityScreen PriorityScreen
	{
		get
		{
			return this.priorityScreen;
		}
	}

	public override float GetSortKey()
	{
		return 5f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ToolMenu.Instance = this;
		this.priorityScreen = Util.KInstantiateUI<PriorityScreen>(this.Prefab_priorityScreen.gameObject, base.gameObject, false);
		this.priorityScreen.InstantiateButtons(new Action<PrioritySetting>(this.OnPriorityClicked), false);
		this.priorityScreen.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		this.activateOnSpawn = true;
		base.OnSpawn();
		this.SetData();
		this.rows.ForEach(delegate(ToolMenu.ToolCollection[] row)
		{
			this.InstantiateCollectionsUI(row);
		});
		this.rows.ForEach(delegate(ToolMenu.ToolCollection[] row)
		{
			this.BuildRowToggles(row);
		});
		this.rows.ForEach(delegate(ToolMenu.ToolCollection[] row)
		{
			this.BuildToolToggles(row);
		});
		this.ChooseCollection(null, true);
		this.priorityScreen.gameObject.SetActive(false);
		this.ToggleSandboxUI(null);
		Game.Instance.Subscribe(-1948169901, new Action<object>(this.ToggleSandboxUI));
		this.ResetToolDisplayPlane();
	}

	private void ResetToolDisplayPlane()
	{
		this.toolEffectDisplayPlane = this.CreateToolDisplayPlane("Overlay", World.Instance.transform);
		this.toolEffectDisplayPlaneTexture = this.CreatePlaneTexture(out this.toolEffectDisplayBytes, Grid.WidthInCells, Grid.HeightInCells);
		this.toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial = this.toolEffectDisplayMaterial;
		this.toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.toolEffectDisplayPlaneTexture;
		this.toolEffectDisplayPlane.transform.SetLocalPosition(new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -6f));
		this.RefreshToolDisplayPlaneColor();
	}

	private GameObject CreateToolDisplayPlane(string layer, Transform parent)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "toolEffectDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		global::UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
		if (parent != null)
		{
			gameObject.transform.SetParent(parent);
		}
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.transform.localScale = new Vector3(Grid.WidthInMeters / -10f, 1f, Grid.HeightInMeters / -10f);
		gameObject.transform.eulerAngles = new Vector3(270f, 0f, 0f);
		gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		return gameObject;
	}

	private Texture2D CreatePlaneTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		return new Texture2D(width, height, TextureFormat.RGBA32, false)
		{
			name = "toolEffectDisplayPlane",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	private void Update()
	{
		this.RefreshToolDisplayPlaneColor();
	}

	private void RefreshToolDisplayPlaneColor()
	{
		if (PlayerController.Instance.ActiveTool == null || PlayerController.Instance.ActiveTool == SelectTool.Instance)
		{
			this.toolEffectDisplayPlane.SetActive(false);
		}
		else
		{
			PlayerController.Instance.ActiveTool.GetOverlayColorData(out this.colors);
			Array.Clear(this.toolEffectDisplayBytes, 0, this.toolEffectDisplayBytes.Length);
			if (this.colors != null)
			{
				foreach (ToolMenu.CellColorData cellColorData in this.colors)
				{
					if (Grid.IsValidCell(cellColorData.cell))
					{
						int num = cellColorData.cell * 4;
						if (num >= 0)
						{
							this.toolEffectDisplayBytes[num] = (byte)(Mathf.Min(cellColorData.color.r, 1f) * 255f);
							this.toolEffectDisplayBytes[num + 1] = (byte)(Mathf.Min(cellColorData.color.g, 1f) * 255f);
							this.toolEffectDisplayBytes[num + 2] = (byte)(Mathf.Min(cellColorData.color.b, 1f) * 255f);
							this.toolEffectDisplayBytes[num + 3] = (byte)(Mathf.Min(cellColorData.color.a, 1f) * 255f);
						}
					}
				}
			}
			if (!this.toolEffectDisplayPlane.activeSelf)
			{
				this.toolEffectDisplayPlane.SetActive(true);
			}
			this.toolEffectDisplayPlaneTexture.LoadRawTextureData(this.toolEffectDisplayBytes);
			this.toolEffectDisplayPlaneTexture.Apply();
		}
	}

	public void ToggleSandboxUI(object data = null)
	{
		this.ClearSelection();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		this.rowSandboxTools[0].toggle.transform.parent.gameObject.SetActive(Game.Instance.SandboxModeActive);
	}

	private void SetData()
	{
		ToolMenu.ToolCollection toolCollection = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", global::Action.SandboxBrush, "SandboxBrushTool", toolCollection, UI.SANDBOXTOOLS.SETTINGS.BRUSH.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection2 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", global::Action.SandboxSprinkle, "SandboxSprinkleTool", toolCollection2, UI.SANDBOXTOOLS.SETTINGS.SPRINKLE.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection3 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", global::Action.SandboxFlood, "SandboxFloodTool", toolCollection3, UI.SANDBOXTOOLS.SETTINGS.FLOOD.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection4 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", global::Action.SandboxSample, "SandboxSampleTool", toolCollection4, UI.SANDBOXTOOLS.SETTINGS.SAMPLE.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection5 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.HEATGUN.NAME, "brush", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.HEATGUN.NAME, "brush", global::Action.SandboxHeatGun, "SandboxHeatTool", toolCollection5, UI.SANDBOXTOOLS.SETTINGS.HEATGUN.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection6 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", global::Action.SandboxHeatGun, "SandboxSpawnerTool", toolCollection6, UI.SANDBOXTOOLS.SETTINGS.SPAWNER.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection7 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", global::Action.SandboxClearFloor, "SandboxClearFloorTool", toolCollection7, UI.SANDBOXTOOLS.SETTINGS.CLEAR_FLOOR.TOOLTIP, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection8 = new ToolMenu.ToolCollection(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", string.Empty, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", global::Action.SandboxDestroy, "SandboxDestroyerTool", toolCollection8, UI.SANDBOXTOOLS.SETTINGS.DESTROY.TOOLTIP, SimViewMode.None, false, null, null);
		this.rowSandboxTools = new ToolMenu.ToolCollection[] { toolCollection, toolCollection2, toolCollection3, toolCollection4, toolCollection5, toolCollection6, toolCollection7, toolCollection8 };
		ToolMenu.ToolCollection toolCollection9 = new ToolMenu.ToolCollection(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", UI.TOOLTIPS.DECONSTRUCTBUTTON, false, global::Action.BuildingDeconstruct);
		new ToolMenu.ToolInfo(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", global::Action.BuildingDeconstruct, "DeconstructTool", toolCollection9, UI.TOOLTIPS.DECONSTRUCTBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection10 = new ToolMenu.ToolCollection(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", UI.TOOLTIPS.CANCELBUTTON, false, global::Action.BuildingCancel);
		new ToolMenu.ToolInfo(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", global::Action.BuildingCancel, "CancelTool", toolCollection10, UI.TOOLTIPS.CANCELBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection11 = new ToolMenu.ToolCollection(UI.TOOLS.DIG.NAME, "icon_action_dig", string.Empty, false, global::Action.Dig);
		new ToolMenu.ToolInfo(UI.TOOLS.DIG.NAME, "icon_action_dig", global::Action.Dig, "DigTool", toolCollection11, UI.TOOLTIPS.DIGBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection12 = new ToolMenu.ToolCollection(UI.TOOLS.PRIORITIESCATEGORY.NAME, "icon_action_prioritize", UI.TOOLTIPS.PRIORITIZEMAINBUTTON, false, global::Action.AccessPrioritizeCollection);
		new ToolMenu.ToolInfo(UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", global::Action.Prioritize, "PrioritizeTool", toolCollection12, UI.TOOLTIPS.PRIORITIZEBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection13 = new ToolMenu.ToolCollection(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", UI.TOOLTIPS.CLEARBUTTON, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", global::Action.Clear, "ClearTool", toolCollection13, UI.TOOLTIPS.CLEARBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection14 = new ToolMenu.ToolCollection(UI.TOOLS.MOP.NAME, "icon_action_mop", UI.TOOLTIPS.MOPBUTTON, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.MOP.NAME, "icon_action_mop", global::Action.Mop, "MopTool", toolCollection14, UI.TOOLTIPS.MOPBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection15 = new ToolMenu.ToolCollection(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", UI.TOOLTIPS.DISINFECTBUTTON, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", global::Action.Disinfect, "DisinfectTool", toolCollection15, UI.TOOLTIPS.DISINFECTBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection16 = new ToolMenu.ToolCollection(UI.TOOLS.ATTACK.NAME, "icon_action_attack", string.Empty, false, global::Action.Attack);
		new ToolMenu.ToolInfo(UI.TOOLS.ATTACK.NAME, "icon_action_attack", global::Action.Attack, "AttackTool", toolCollection16, UI.TOOLTIPS.ATTACKBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection17 = new ToolMenu.ToolCollection(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", string.Empty, false, global::Action.Capture);
		new ToolMenu.ToolInfo(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", global::Action.Capture, "CaptureTool", toolCollection17, UI.TOOLTIPS.CAPTUREBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection18 = new ToolMenu.ToolCollection(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", string.Empty, false, global::Action.Harvest);
		new ToolMenu.ToolInfo(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", global::Action.Harvest, "HarvestTool", toolCollection18, UI.TOOLTIPS.HARVESTBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection19 = new ToolMenu.ToolCollection(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", string.Empty, false, global::Action.Harvest);
		new ToolMenu.ToolInfo(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", global::Action.EmptyPipe, "EmptyPipeTool", toolCollection19, UI.TOOLS.EMPTY_PIPE.TOOLTIP, SimViewMode.None, false, null, null);
		this.rowBasicTools = new ToolMenu.ToolCollection[]
		{
			toolCollection11, toolCollection17, toolCollection18, toolCollection19, toolCollection12, toolCollection13, toolCollection14, toolCollection15, toolCollection9, toolCollection16,
			toolCollection10
		};
		this.rows.Add(this.rowSandboxTools);
		this.rows.Add(this.rowBasicTools);
	}

	private void InstantiateCollectionsUI(ToolMenu.ToolCollection[] collections)
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefabToolRow, base.gameObject, true);
		for (int i = 0; i < collections.Length; i++)
		{
			ToolMenu.ToolCollection tc = collections[i];
			tc.toggle = Util.KInstantiateUI((collections[i].tools.Count <= 1) ? ((collections != this.rowSandboxTools) ? this.toolIconPrefab : this.sandboxToolIconPrefab) : this.collectionIconPrefab, gameObject, true);
			KToggle component = tc.toggle.GetComponent<KToggle>();
			component.soundPlayer.Enabled = false;
			component.onClick += delegate
			{
				if (this.currentlySelectedCollection == tc && tc.tools.Count >= 1)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false));
				}
				this.ChooseCollection(tc, true);
			};
			if (tc.tools != null)
			{
				GameObject gameObject2;
				if (tc.tools.Count < this.smallCollectionMax)
				{
					gameObject2 = Util.KInstantiateUI(this.Prefab_collectionContainer, gameObject, true);
					gameObject2.transform.SetSiblingIndex(gameObject2.transform.GetSiblingIndex() - 1);
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.rectTransform().sizeDelta = new Vector2((float)(tc.tools.Count * 75), 50f);
					tc.MaskContainer = gameObject2.GetComponentInChildren<Mask>().gameObject;
					gameObject2.SetActive(false);
				}
				else
				{
					gameObject2 = Util.KInstantiateUI(this.Prefab_collectionContainerWindow, gameObject, true);
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
					tc.MaskContainer = gameObject2.GetComponentInChildren<GridLayoutGroup>().gameObject;
					gameObject2.SetActive(false);
				}
				tc.UIMenuDisplay = gameObject2;
				for (int j = 0; j < tc.tools.Count; j++)
				{
					ToolMenu.ToolInfo ti = tc.tools[j];
					GameObject gameObject3 = Util.KInstantiateUI((collections != this.rowSandboxTools) ? this.toolIconPrefab : this.sandboxToolIconPrefab, tc.MaskContainer, true);
					gameObject3.name = ti.text;
					ti.toggle = gameObject3.GetComponent<KToggle>();
					if (ti.collection.tools.Count > 1)
					{
						RectTransform rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
						if (gameObject3.name.Length > 12)
						{
							rectTransform.GetComponent<SetTextStyleSetting>().SetStyle(this.CategoryLabelTextStyle_LeftAlign);
							rectTransform.anchoredPosition = new Vector2(16f, rectTransform.anchoredPosition.y);
						}
					}
					ti.toggle.onClick += delegate
					{
						this.ChooseTool(ti);
					};
					tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate(object s)
					{
						this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
						tc.UIMenuDisplay.SetActive(false);
					});
				}
			}
		}
	}

	private void ChooseTool(ToolMenu.ToolInfo tool)
	{
		if (this.currentlySelectedTool == tool)
		{
			return;
		}
		if (this.currentlySelectedTool != tool)
		{
			this.currentlySelectedTool = tool;
			if (this.currentlySelectedTool != null && this.currentlySelectedTool.onSelectCallback != null)
			{
				this.currentlySelectedTool.onSelectCallback(this.currentlySelectedTool);
			}
		}
		if (this.currentlySelectedTool != null)
		{
			this.currentlySelectedCollection = this.currentlySelectedTool.collection;
			foreach (InterfaceTool interfaceTool in PlayerController.Instance.tools)
			{
				if (this.currentlySelectedTool.toolName == interfaceTool.name)
				{
					UISounds.PlaySound(UISounds.Sound.ClickObject);
					PlayerController.Instance.ActivateTool(interfaceTool);
					if (tool.forceViewMode && OverlayScreen.Instance.GetMode() != tool.viewMode)
					{
						EventSystem.Trigger(Game.Instance.gameObject, 1248612973, tool.viewMode);
					}
					break;
				}
			}
		}
		else
		{
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
		}
		this.rows.ForEach(delegate(ToolMenu.ToolCollection[] row)
		{
			this.RefreshRowDisplay(row);
		});
	}

	private void RefreshRowDisplay(ToolMenu.ToolCollection[] row)
	{
		for (int i = 0; i < row.Length; i++)
		{
			ToolMenu.ToolCollection tc = row[i];
			if (this.currentlySelectedTool != null && this.currentlySelectedTool.collection == tc)
			{
				if (!tc.UIMenuDisplay.activeSelf || tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
				{
					if (tc.tools.Count > 1)
					{
						tc.UIMenuDisplay.SetActive(true);
						if (tc.tools.Count < this.smallCollectionMax)
						{
							float num = Mathf.Clamp(1f - (float)tc.tools.Count * 0.15f, 0.5f, 1f);
							tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().speedScale = num;
						}
						tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Expand(delegate(object s)
						{
							this.SetToggleState(tc.toggle.GetComponent<KToggle>(), true);
						});
					}
					else
					{
						this.currentlySelectedTool = tc.tools[0];
					}
				}
			}
			else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing && tc.tools.Count > 0)
			{
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate(object s)
				{
					this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
					tc.UIMenuDisplay.SetActive(false);
				});
			}
			for (int j = 0; j < tc.tools.Count; j++)
			{
				if (tc.tools[j] == this.currentlySelectedTool)
				{
					this.SetToggleState(tc.tools[j].toggle, true);
				}
				else
				{
					this.SetToggleState(tc.tools[j].toggle, false);
				}
			}
		}
	}

	public void TurnLargeCollectionOff()
	{
		if (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Count > this.smallCollectionMax)
		{
			this.ChooseCollection(null, true);
		}
	}

	private void ChooseCollection(ToolMenu.ToolCollection collection, bool autoSelectTool = true)
	{
		if (collection == this.currentlySelectedCollection)
		{
			if (collection != null && collection.tools.Count > 1)
			{
				this.currentlySelectedCollection = null;
				if (this.currentlySelectedTool != null)
				{
					this.ChooseTool(null);
				}
			}
			else if (this.currentlySelectedTool != null && this.currentlySelectedCollection.tools.Contains(this.currentlySelectedTool) && this.currentlySelectedCollection.tools.Count == 1)
			{
				this.currentlySelectedCollection = null;
				this.ChooseTool(null);
			}
		}
		else
		{
			this.currentlySelectedCollection = collection;
		}
		this.rows.ForEach(delegate(ToolMenu.ToolCollection[] row)
		{
			this.OpenOrCloseCollectionsInRow(row, true);
		});
	}

	private void OpenOrCloseCollectionsInRow(ToolMenu.ToolCollection[] row, bool autoSelectTool = true)
	{
		for (int i = 0; i < row.Length; i++)
		{
			ToolMenu.ToolCollection tc = row[i];
			if (this.currentlySelectedCollection == tc)
			{
				if ((this.currentlySelectedCollection.tools != null && this.currentlySelectedCollection.tools.Count == 1) || autoSelectTool)
				{
					this.ChooseTool(this.currentlySelectedCollection.tools[0]);
				}
			}
			else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
			{
				tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(delegate(object s)
				{
					this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
					tc.UIMenuDisplay.SetActive(false);
				});
			}
			this.SetToggleState(tc.toggle.GetComponent<KToggle>(), this.currentlySelectedCollection == tc);
		}
	}

	private IEnumerator CloseCollection(ToolMenu.ToolCollection tc)
	{
		Animator anim = tc.UIMenuDisplay.GetComponent<Animator>();
		float speedMultiplier = 1f;
		float speedAdjustmentPerTool = 0.125f;
		anim.speed = speedMultiplier;
		anim.speed = 1f - speedAdjustmentPerTool * (float)(tc.tools.Count - 1);
		anim.Play("Close");
		float length = anim.GetCurrentAnimatorStateInfo(0).length + 0.05f;
		for (float remaining = length; remaining >= 0f; remaining -= Time.unscaledDeltaTime)
		{
			yield return null;
		}
		this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
		tc.UIMenuDisplay.SetActive(false);
		yield break;
	}

	private void SetToggleState(KToggle toggle, bool state)
	{
		if (state)
		{
			toggle.Select();
			toggle.isOn = true;
		}
		else
		{
			toggle.Deselect();
			toggle.isOn = false;
		}
	}

	public void ClearSelection()
	{
		if (this.currentlySelectedCollection != null)
		{
			this.ChooseCollection(null, true);
		}
		if (this.currentlySelectedTool != null)
		{
			this.ChooseTool(null);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (SaveGame.Instance.sandboxEnabled && e.IsAction(global::Action.ToggleSandboxTools))
			{
				Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
			}
			foreach (ToolMenu.ToolCollection[] array in this.rows)
			{
				if (array != this.rowSandboxTools || Game.Instance.SandboxModeActive)
				{
					for (int i = 0; i < array.Length; i++)
					{
						global::Action toolHotkey = array[i].hotkey;
						if (toolHotkey != global::Action.NumActions && e.IsAction(toolHotkey) && (this.currentlySelectedCollection == null || (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Find((ToolMenu.ToolInfo t) => GameInputMapping.CompareActionKeyCodes(t.hotkey, toolHotkey)) == null)))
						{
							if (this.currentlySelectedCollection != array[i])
							{
								this.ChooseCollection(array[i], false);
								this.ChooseTool(array[i].tools[0]);
							}
							else if (this.currentlySelectedCollection.tools.Count > 1)
							{
								e.Consumed = true;
								this.ChooseCollection(null, true);
								this.ChooseTool(null);
								string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
								if (sound != null)
								{
									KMonoBehaviour.PlaySound(sound);
								}
							}
							break;
						}
						for (int j = 0; j < array[i].tools.Count; j++)
						{
							if ((this.currentlySelectedCollection == null && array[i].tools.Count == 1) || this.currentlySelectedCollection == array[i] || (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Count == 1 && array[i].tools.Count == 1))
							{
								global::Action hotkey = array[i].tools[j].hotkey;
								if (e.IsAction(hotkey) && e.TryConsume(hotkey))
								{
									if (array[i].tools.Count == 1 && this.currentlySelectedCollection != array[i])
									{
										this.ChooseCollection(array[i], false);
									}
									else if (this.currentlySelectedTool != array[i].tools[j])
									{
										this.ChooseTool(array[i].tools[j]);
									}
								}
								else if (GameInputMapping.CompareActionKeyCodes(e.GetAction(), hotkey))
								{
									e.Consumed = true;
								}
							}
						}
					}
				}
			}
			if ((this.currentlySelectedTool != null || this.currentlySelectedCollection != null) && !e.Consumed)
			{
				if (e.TryConsume(global::Action.Escape))
				{
					string sound2 = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
					if (sound2 != null)
					{
						KMonoBehaviour.PlaySound(sound2);
					}
					if (this.currentlySelectedCollection != null)
					{
						this.ChooseCollection(null, true);
					}
					if (this.currentlySelectedTool != null)
					{
						this.ChooseTool(null);
					}
					SelectTool.Instance.Activate();
				}
			}
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && e.TryConsume(global::Action.Escape))
			{
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if ((this.currentlySelectedTool != null || this.currentlySelectedCollection != null) && !e.Consumed)
			{
				if (PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
				{
					string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
					if (sound != null)
					{
						KMonoBehaviour.PlaySound(sound);
					}
					if (this.currentlySelectedCollection != null)
					{
						this.ChooseCollection(null, true);
					}
					if (this.currentlySelectedTool != null)
					{
						this.ChooseTool(null);
					}
					SelectTool.Instance.Activate();
				}
			}
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
			{
				SelectTool.Instance.Activate();
				string sound2 = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false);
				if (sound2 != null)
				{
					KMonoBehaviour.PlaySound(sound2);
				}
			}
		}
		base.OnKeyUp(e);
	}

	protected void BuildRowToggles(ToolMenu.ToolCollection[] row)
	{
		for (int i = 0; i < row.Length; i++)
		{
			ToolMenu.ToolCollection toolCollection = row[i];
			if (!(toolCollection.toggle == null))
			{
				GameObject toggle = toolCollection.toggle;
				foreach (Sprite sprite in this.icons)
				{
					if (sprite != null && sprite.name == toolCollection.icon)
					{
						Image component = toggle.transform.Find("FG").GetComponent<Image>();
						component.sprite = sprite;
						break;
					}
				}
				Transform transform = toggle.transform.Find("Text");
				if (transform != null)
				{
					LocText component2 = transform.GetComponent<LocText>();
					if (component2 != null)
					{
						component2.text = toolCollection.text;
					}
				}
				ToolTip component3 = toggle.GetComponent<ToolTip>();
				if (component3)
				{
					if (row[i].tools.Count == 1)
					{
						string hotkeyString = GameUtil.GetHotkeyString(row[i].tools[0].hotkey);
						component3.AddMultiStringTooltip(row[i].tools[0].tooltip + " " + hotkeyString, this.ToggleToolTipTextStyleSetting);
					}
					else
					{
						string text = row[i].tooltip;
						if (row[i].hotkey != global::Action.NumActions)
						{
							text = text + " " + GameUtil.GetHotkeyString(row[i].hotkey);
						}
						component3.AddMultiStringTooltip(text, this.ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	protected void BuildToolToggles(ToolMenu.ToolCollection[] row)
	{
		foreach (ToolMenu.ToolCollection toolCollection in row)
		{
			if (!(toolCollection.toggle == null))
			{
				for (int j = 0; j < toolCollection.tools.Count; j++)
				{
					GameObject gameObject = toolCollection.tools[j].toggle.gameObject;
					foreach (Sprite sprite in this.icons)
					{
						if (sprite != null && sprite.name == toolCollection.tools[j].icon)
						{
							Image component = gameObject.transform.Find("FG").GetComponent<Image>();
							component.sprite = sprite;
							break;
						}
					}
					Transform transform = gameObject.transform.Find("Text");
					if (transform != null)
					{
						LocText component2 = transform.GetComponent<LocText>();
						if (component2 != null)
						{
							component2.text = toolCollection.tools[j].text;
						}
					}
					ToolTip component3 = gameObject.GetComponent<ToolTip>();
					if (component3)
					{
						string text = ((toolCollection.tools.Count <= 1) ? GameUtil.GetHotkeyString(toolCollection.tools[j].hotkey) : (GameUtil.GetHotkeyString(toolCollection.hotkey) + "+ " + GameUtil.GetHotkeyString(toolCollection.tools[j].hotkey)));
						component3.AddMultiStringTooltip(toolCollection.tools[j].tooltip + " " + text, this.ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	public bool HasUniqueKeyBindings()
	{
		bool flag = true;
		this.boundRootActions.Clear();
		foreach (ToolMenu.ToolCollection[] array in this.rows)
		{
			foreach (ToolMenu.ToolCollection toolCollection in array)
			{
				if (this.boundRootActions.Contains(toolCollection.hotkey))
				{
					flag = false;
					break;
				}
				this.boundRootActions.Add(toolCollection.hotkey);
				this.boundSubgroupActions.Clear();
				foreach (ToolMenu.ToolInfo toolInfo in toolCollection.tools)
				{
					if (this.boundSubgroupActions.Contains(toolInfo.hotkey))
					{
						flag = false;
						break;
					}
					this.boundSubgroupActions.Add(toolInfo.hotkey);
				}
			}
		}
		return flag;
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		this.priorityScreen.SetScreenPriority(priority, false);
	}

	public static ToolMenu Instance;

	public GameObject Prefab_collectionContainer;

	public GameObject Prefab_collectionContainerWindow;

	public PriorityScreen Prefab_priorityScreen;

	public GameObject toolIconPrefab;

	public GameObject sandboxToolIconPrefab;

	public GameObject collectionIconPrefab;

	public GameObject prefabToolRow;

	[SerializeField]
	private Sprite[] icons;

	private PriorityScreen priorityScreen;

	public ToolParameterMenu toolParameterMenu;

	public GameObject sandboxToolParameterMenu;

	private GameObject toolEffectDisplayPlane;

	private Texture2D toolEffectDisplayPlaneTexture;

	public Material toolEffectDisplayMaterial;

	private byte[] toolEffectDisplayBytes;

	private List<ToolMenu.ToolCollection[]> rows = new List<ToolMenu.ToolCollection[]>();

	public ToolMenu.ToolCollection[] rowBasicTools;

	public ToolMenu.ToolCollection[] rowSandboxTools;

	public ToolMenu.ToolCollection currentlySelectedCollection;

	public ToolMenu.ToolInfo currentlySelectedTool;

	private Coroutine activeOpenAnimationRoutine;

	private Coroutine activeCloseAnimationRoutine;

	private HashSet<global::Action> boundRootActions = new HashSet<global::Action>();

	private HashSet<global::Action> boundSubgroupActions = new HashSet<global::Action>();

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting CategoryLabelTextStyle_LeftAlign;

	private int smallCollectionMax = 5;

	private int startY;

	private int rowRange = 128;

	private HashSet<ToolMenu.CellColorData> colors = new HashSet<ToolMenu.CellColorData>();

	public class ToolInfo
	{
		public ToolInfo(string text, string icon_name, global::Action hotkey, string ToolName, ToolMenu.ToolCollection toolCollection, string tooltip = "", SimViewMode associatedViewMode = SimViewMode.None, bool forceViewMode = false, Action<object> onSelectCallback = null, object toolData = null)
		{
			this.text = text;
			this.icon = icon_name;
			this.hotkey = hotkey;
			this.toolName = ToolName;
			this.collection = toolCollection;
			toolCollection.tools.Add(this);
			this.tooltip = tooltip;
			this.viewMode = associatedViewMode;
			this.forceViewMode = forceViewMode;
			this.onSelectCallback = onSelectCallback;
			this.toolData = toolData;
		}

		public string text;

		public string icon;

		public global::Action hotkey;

		public string toolName;

		public ToolMenu.ToolCollection collection;

		public string tooltip;

		public SimViewMode viewMode;

		public bool forceViewMode;

		public KToggle toggle;

		public Action<object> onSelectCallback;

		public object toolData;
	}

	public class ToolCollection
	{
		public ToolCollection(string text, string icon_name, string tooltip = "", bool useInfoMenu = false, global::Action hotkey = global::Action.NumActions)
		{
			this.text = text;
			this.icon = icon_name;
			this.tooltip = tooltip;
			this.useInfoMenu = useInfoMenu;
			this.hotkey = hotkey;
		}

		public string text;

		public string icon;

		public string tooltip;

		public bool useInfoMenu;

		public GameObject toggle;

		public List<ToolMenu.ToolInfo> tools = new List<ToolMenu.ToolInfo>();

		public GameObject UIMenuDisplay;

		public GameObject MaskContainer;

		public global::Action hotkey;
	}

	public struct CellColorData
	{
		public CellColorData(int cell, Color color)
		{
			this.cell = cell;
			this.color = color;
		}

		public int cell;

		public Color color;
	}
}
