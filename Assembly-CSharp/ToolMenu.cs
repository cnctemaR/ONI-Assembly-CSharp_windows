using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolMenu : KScreen
{
	public static void DestroyInstance()
	{
		ToolMenu.Instance = null;
	}

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
	}

	protected override void OnSpawn()
	{
		this.activateOnSpawn = true;
		base.OnSpawn();
		this.CreateSandBoxTools();
		this.CreateBasicTools();
		this.rows.Add(this.sandboxTools);
		this.rows.Add(this.basicTools);
		this.rows.ForEach(delegate(List<ToolMenu.ToolCollection> row)
		{
			this.InstantiateCollectionsUI(row);
		});
		this.rows.ForEach(delegate(List<ToolMenu.ToolCollection> row)
		{
			this.BuildRowToggles(row);
		});
		this.rows.ForEach(delegate(List<ToolMenu.ToolCollection> row)
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
		return new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat(TextureFormat.RGBA32), TextureCreationFlags.None)
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
		this.sandboxTools[0].toggle.transform.parent.transform.parent.gameObject.SetActive(Game.Instance.SandboxModeActive);
	}

	public static ToolMenu.ToolCollection CreateToolCollection(LocString collection_name, string icon_name, global::Action hotkey, string tool_name, LocString tooltip, bool largeIcon)
	{
		string text = collection_name;
		ToolMenu.ToolCollection toolCollection = new ToolMenu.ToolCollection(text, icon_name, string.Empty, false, global::Action.NumActions, largeIcon);
		new ToolMenu.ToolInfo(collection_name, icon_name, hotkey, tool_name, toolCollection, tooltip, false, null, null);
		return toolCollection;
	}

	private void CreateSandBoxTools()
	{
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", global::Action.SandboxBrush, "SandboxBrushTool", UI.SANDBOXTOOLS.SETTINGS.BRUSH.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", global::Action.SandboxSprinkle, "SandboxSprinkleTool", UI.SANDBOXTOOLS.SETTINGS.SPRINKLE.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", global::Action.SandboxFlood, "SandboxFloodTool", UI.SANDBOXTOOLS.SETTINGS.FLOOD.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", global::Action.SandboxSample, "SandboxSampleTool", UI.SANDBOXTOOLS.SETTINGS.SAMPLE.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.HEATGUN.NAME, "brush", global::Action.SandboxHeatGun, "SandboxHeatTool", UI.SANDBOXTOOLS.SETTINGS.HEATGUN.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", global::Action.SandboxSpawnEntity, "SandboxSpawnerTool", UI.SANDBOXTOOLS.SETTINGS.SPAWNER.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", global::Action.SandboxClearFloor, "SandboxClearFloorTool", UI.SANDBOXTOOLS.SETTINGS.CLEAR_FLOOR.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", global::Action.SandboxDestroy, "SandboxDestroyerTool", UI.SANDBOXTOOLS.SETTINGS.DESTROY.TOOLTIP, false));
		this.sandboxTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.SANDBOX.FOW.NAME, "brush", global::Action.SandboxReveal, "SandboxFOWTool", UI.SANDBOXTOOLS.SETTINGS.FOW.TOOLTIP, false));
	}

	private void CreateBasicTools()
	{
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.DIG.NAME, "icon_action_dig", global::Action.Dig, "DigTool", UI.TOOLTIPS.DIGBUTTON, true));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", global::Action.BuildingCancel, "CancelTool", UI.TOOLTIPS.CANCELBUTTON, true));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", global::Action.BuildingDeconstruct, "DeconstructTool", UI.TOOLTIPS.DECONSTRUCTBUTTON, true));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", global::Action.Prioritize, "PrioritizeTool", UI.TOOLTIPS.PRIORITIZEBUTTON, true));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", global::Action.Disinfect, "DisinfectTool", UI.TOOLTIPS.DISINFECTBUTTON, false));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", global::Action.Clear, "ClearTool", UI.TOOLTIPS.CLEARBUTTON, false));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.ATTACK.NAME, "icon_action_attack", global::Action.Attack, "AttackTool", UI.TOOLTIPS.ATTACKBUTTON, false));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.MOP.NAME, "icon_action_mop", global::Action.Mop, "MopTool", UI.TOOLTIPS.MOPBUTTON, false));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.CAPTURE.NAME, "icon_action_capture", global::Action.Capture, "CaptureTool", UI.TOOLTIPS.CAPTUREBUTTON, false));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.HARVEST.NAME, "icon_action_harvest", global::Action.Harvest, "HarvestTool", UI.TOOLTIPS.HARVESTBUTTON, false));
		this.basicTools.Add(ToolMenu.CreateToolCollection(UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", global::Action.EmptyPipe, "EmptyPipeTool", UI.TOOLS.EMPTY_PIPE.TOOLTIP, false));
	}

	private void InstantiateCollectionsUI(IList<ToolMenu.ToolCollection> collections)
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefabToolRow, base.gameObject, true);
		GameObject gameObject2 = Util.KInstantiateUI(this.largeToolSet, gameObject, true);
		GameObject gameObject3 = Util.KInstantiateUI(this.smallToolSet, gameObject, true);
		GameObject gameObject4 = Util.KInstantiateUI(this.smallToolBottomRow, gameObject3, true);
		GameObject gameObject5 = Util.KInstantiateUI(this.smallToolTopRow, gameObject3, true);
		GameObject gameObject6 = Util.KInstantiateUI(this.sandboxToolSet, gameObject, true);
		bool flag = true;
		for (int i = 0; i < collections.Count; i++)
		{
			GameObject gameObject7;
			if (collections == this.sandboxTools)
			{
				gameObject7 = gameObject6;
			}
			else if (collections[i].largeIcon)
			{
				gameObject7 = gameObject2;
			}
			else
			{
				gameObject7 = ((!flag) ? gameObject4 : gameObject5);
				flag = !flag;
			}
			ToolMenu.ToolCollection tc = collections[i];
			tc.toggle = Util.KInstantiateUI((collections[i].tools.Count <= 1) ? ((collections != this.sandboxTools) ? ((!collections[i].largeIcon) ? this.toolIconPrefab : this.toolIconLargePrefab) : this.sandboxToolIconPrefab) : this.collectionIconPrefab, gameObject7, true);
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
				GameObject gameObject8;
				if (tc.tools.Count < this.smallCollectionMax)
				{
					gameObject8 = Util.KInstantiateUI(this.Prefab_collectionContainer, gameObject7, true);
					gameObject8.transform.SetSiblingIndex(gameObject8.transform.GetSiblingIndex() - 1);
					gameObject8.transform.localScale = Vector3.one;
					gameObject8.rectTransform().sizeDelta = new Vector2((float)(tc.tools.Count * 75), 50f);
					tc.MaskContainer = gameObject8.GetComponentInChildren<Mask>().gameObject;
					gameObject8.SetActive(false);
				}
				else
				{
					gameObject8 = Util.KInstantiateUI(this.Prefab_collectionContainerWindow, gameObject7, true);
					gameObject8.transform.localScale = Vector3.one;
					gameObject8.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
					tc.MaskContainer = gameObject8.GetComponentInChildren<GridLayoutGroup>().gameObject;
					gameObject8.SetActive(false);
				}
				tc.UIMenuDisplay = gameObject8;
				for (int j = 0; j < tc.tools.Count; j++)
				{
					ToolMenu.ToolInfo ti = tc.tools[j];
					GameObject gameObject9 = Util.KInstantiateUI((collections != this.sandboxTools) ? ((!collections[i].largeIcon) ? this.toolIconPrefab : this.toolIconLargePrefab) : this.sandboxToolIconPrefab, tc.MaskContainer, true);
					gameObject9.name = ti.text;
					ti.toggle = gameObject9.GetComponent<KToggle>();
					if (ti.collection.tools.Count > 1)
					{
						RectTransform rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
						if (gameObject9.name.Length > 12)
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
		if (gameObject2.transform.childCount == 0)
		{
			global::UnityEngine.Object.Destroy(gameObject2);
		}
		if (gameObject4.transform.childCount == 0 && gameObject5.transform.childCount == 0)
		{
			global::UnityEngine.Object.Destroy(gameObject3);
		}
		if (gameObject6.transform.childCount == 0)
		{
			global::UnityEngine.Object.Destroy(gameObject6);
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
						Game.Instance.gameObject.Trigger(1248612973, tool.viewMode);
					}
					break;
				}
			}
		}
		else
		{
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
		}
		this.rows.ForEach(delegate(List<ToolMenu.ToolCollection> row)
		{
			this.RefreshRowDisplay(row);
		});
	}

	private void RefreshRowDisplay(IList<ToolMenu.ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
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
		this.rows.ForEach(delegate(List<ToolMenu.ToolCollection> row)
		{
			this.OpenOrCloseCollectionsInRow(row, true);
		});
	}

	private void OpenOrCloseCollectionsInRow(IList<ToolMenu.ToolCollection> row, bool autoSelectTool = true)
	{
		for (int i = 0; i < row.Count; i++)
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
			if (e.IsAction(global::Action.ToggleSandboxTools))
			{
				if (Application.isEditor)
				{
					Output.Log(new object[] { "Force-enabling sandbox mode because we're in editor." });
					SaveGame.Instance.sandboxEnabled = true;
				}
				if (SaveGame.Instance.sandboxEnabled)
				{
					Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
				}
			}
			foreach (List<ToolMenu.ToolCollection> list in this.rows)
			{
				if (list != this.sandboxTools || Game.Instance.SandboxModeActive)
				{
					for (int i = 0; i < list.Count; i++)
					{
						global::Action toolHotkey = list[i].hotkey;
						if (toolHotkey != global::Action.NumActions && e.IsAction(toolHotkey) && (this.currentlySelectedCollection == null || (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Find((ToolMenu.ToolInfo t) => GameInputMapping.CompareActionKeyCodes(t.hotkey, toolHotkey)) == null)))
						{
							if (this.currentlySelectedCollection != list[i])
							{
								this.ChooseCollection(list[i], false);
								this.ChooseTool(list[i].tools[0]);
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
						for (int j = 0; j < list[i].tools.Count; j++)
						{
							if ((this.currentlySelectedCollection == null && list[i].tools.Count == 1) || this.currentlySelectedCollection == list[i] || (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Count == 1 && list[i].tools.Count == 1))
							{
								global::Action hotkey = list[i].tools[j].hotkey;
								if (e.IsAction(hotkey) && e.TryConsume(hotkey))
								{
									if (list[i].tools.Count == 1 && this.currentlySelectedCollection != list[i])
									{
										this.ChooseCollection(list[i], false);
									}
									else if (this.currentlySelectedTool != list[i].tools[j])
									{
										this.ChooseTool(list[i].tools[j]);
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

	protected void BuildRowToggles(IList<ToolMenu.ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
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

	protected void BuildToolToggles(IList<ToolMenu.ToolCollection> row)
	{
		for (int i = 0; i < row.Count; i++)
		{
			ToolMenu.ToolCollection toolCollection = row[i];
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
		foreach (List<ToolMenu.ToolCollection> list in this.rows)
		{
			foreach (ToolMenu.ToolCollection toolCollection in list)
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

	public GameObject toolIconLargePrefab;

	public GameObject sandboxToolIconPrefab;

	public GameObject collectionIconPrefab;

	public GameObject prefabToolRow;

	public GameObject largeToolSet;

	public GameObject smallToolSet;

	public GameObject smallToolBottomRow;

	public GameObject smallToolTopRow;

	public GameObject sandboxToolSet;

	[SerializeField]
	private List<Sprite> icons = new List<Sprite>();

	private PriorityScreen priorityScreen;

	public ToolParameterMenu toolParameterMenu;

	public GameObject sandboxToolParameterMenu;

	private GameObject toolEffectDisplayPlane;

	private Texture2D toolEffectDisplayPlaneTexture;

	public Material toolEffectDisplayMaterial;

	private byte[] toolEffectDisplayBytes;

	private List<List<ToolMenu.ToolCollection>> rows = new List<List<ToolMenu.ToolCollection>>();

	public List<ToolMenu.ToolCollection> basicTools = new List<ToolMenu.ToolCollection>();

	public List<ToolMenu.ToolCollection> sandboxTools = new List<ToolMenu.ToolCollection>();

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

	private HashSet<ToolMenu.CellColorData> colors = new HashSet<ToolMenu.CellColorData>();

	public class ToolInfo
	{
		public ToolInfo(string text, string icon_name, global::Action hotkey, string ToolName, ToolMenu.ToolCollection toolCollection, string tooltip = "", bool forceViewMode = false, Action<object> onSelectCallback = null, object toolData = null)
		{
			this.text = text;
			this.icon = icon_name;
			this.hotkey = hotkey;
			this.toolName = ToolName;
			this.collection = toolCollection;
			toolCollection.tools.Add(this);
			this.tooltip = tooltip;
			this.forceViewMode = forceViewMode;
			this.onSelectCallback = onSelectCallback;
			this.toolData = toolData;
			this.viewMode = OverlayModes.None.ID;
		}

		public string text;

		public string icon;

		public global::Action hotkey;

		public string toolName;

		public ToolMenu.ToolCollection collection;

		public string tooltip;

		public HashedString viewMode;

		public bool forceViewMode;

		public KToggle toggle;

		public Action<object> onSelectCallback;

		public object toolData;
	}

	public class ToolCollection
	{
		public ToolCollection(string text, string icon_name, string tooltip = "", bool useInfoMenu = false, global::Action hotkey = global::Action.NumActions, bool largeIcon = false)
		{
			this.text = text;
			this.icon = icon_name;
			this.tooltip = tooltip;
			this.useInfoMenu = useInfoMenu;
			this.hotkey = hotkey;
			this.largeIcon = largeIcon;
		}

		public string text;

		public string icon;

		public string tooltip;

		public bool useInfoMenu;

		public bool largeIcon;

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
