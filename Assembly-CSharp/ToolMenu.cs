using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : KScreen
{
	public override float GetSortKey()
	{
		return 5f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ToolMenu.Instance = this;
	}

	protected override void OnSpawn()
	{
		this.activateOnSpawn = true;
		base.OnSpawn();
		this.SetData();
		this.Setup();
		this.BuildCollectionToggles();
		this.BuildToolToggles();
		this.ChooseCollection(null, true);
	}

	private void SetData()
	{
		ToolMenu.ToolCollection toolCollection = new ToolMenu.ToolCollection(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", UI.TOOLTIPS.DECONSTRUCTBUTTON, false, global::Action.BuildingDeconstruct);
		new ToolMenu.ToolInfo(UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", global::Action.BuildingDeconstruct, "DeconstructTool", toolCollection, UI.TOOLTIPS.DECONSTRUCTBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection2 = new ToolMenu.ToolCollection(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", UI.TOOLTIPS.CANCELBUTTON, false, global::Action.BuildingCancel);
		new ToolMenu.ToolInfo(UI.TOOLS.CANCEL.NAME, "icon_action_cancel", global::Action.BuildingCancel, "CancelTool", toolCollection2, UI.TOOLTIPS.CANCELBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection3 = new ToolMenu.ToolCollection(UI.TOOLS.DIG.NAME, "icon_action_dig", string.Empty, false, global::Action.Dig);
		new ToolMenu.ToolInfo(UI.TOOLS.DIG.NAME, "icon_action_dig", global::Action.Dig, "DigTool", toolCollection3, UI.TOOLTIPS.DIGBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection4 = new ToolMenu.ToolCollection(UI.TOOLS.PRIORITIESCATEGORY.NAME, "icon_action_prioritize", UI.TOOLTIPS.PRIORITIZEMAINBUTTON, false, global::Action.AccessPrioritizeCollection);
		new ToolMenu.ToolInfo(UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", global::Action.Prioritize, "PrioritizeTool", toolCollection4, UI.TOOLTIPS.PRIORITIZEBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection5 = new ToolMenu.ToolCollection(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", UI.TOOLTIPS.CLEARBUTTON, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", global::Action.Clear, "ClearTool", toolCollection5, UI.TOOLTIPS.CLEARBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection6 = new ToolMenu.ToolCollection(UI.TOOLS.MOP.NAME, "icon_action_mop", UI.TOOLTIPS.MOPBUTTON, false, global::Action.NumActions);
		new ToolMenu.ToolInfo(UI.TOOLS.MOP.NAME, "icon_action_mop", global::Action.Mop, "MopTool", toolCollection6, UI.TOOLTIPS.MOPBUTTON, SimViewMode.None, false, null, null);
		ToolMenu.ToolCollection toolCollection7 = new ToolMenu.ToolCollection(UI.TOOLS.ATTACK.NAME, "icon_action_attack", string.Empty, false, global::Action.Attack);
		new ToolMenu.ToolInfo(UI.TOOLS.ATTACK.NAME, "icon_action_attack", global::Action.Attack, "AttackTool", toolCollection7, UI.TOOLTIPS.ATTACKBUTTON, SimViewMode.None, false, null, null);
		this.toolCollections = new ToolMenu.ToolCollection[] { toolCollection3, toolCollection7, toolCollection4, toolCollection5, toolCollection6, toolCollection, toolCollection2 };
	}

	private void SetupRegionTools(ToolMenu.ToolCollection Collection_StorageRegions)
	{
		new ToolMenu.ToolInfo(UI.TOOLS.ERASEREGION.NAME, "icon_action_cancel", global::Action.EraseRegion, "EraseRegionTool", Collection_StorageRegions, UI.TOOLTIPS.ERASEREGIONBUTTON, SimViewMode.None, false, null, null);
		List<Region> regionPrefabs = Game.Instance.RegionManager.regionPrefabs;
		foreach (Region region in regionPrefabs)
		{
			Action<object> action = new Action<object>(this.RegionToolCallback);
			string regionName = region.RegionName;
			new ToolMenu.ToolInfo(region.RegionName, region.IconName, region.Action, "RegionTool", Collection_StorageRegions, region.ButtonStr, SimViewMode.None, false, action, regionName);
		}
	}

	private void RegionToolCallback(object data)
	{
		ToolMenu.ToolInfo toolInfo = (ToolMenu.ToolInfo)data;
		string text = (string)toolInfo.toolData;
		Game.Instance.RegionManager.SelectRegionPrefab(text);
	}

	private void Setup()
	{
		for (int i = 0; i < this.toolCollections.Length; i++)
		{
			ToolMenu.ToolCollection tc = this.toolCollections[i];
			tc.toggle = Util.KInstantiateUI((this.toolCollections[i].tools.Count <= 1) ? this.toolIconPrefab : this.collectionIconPrefab, base.gameObject, true);
			tc.toggle.GetComponent<KToggle>().onClick += delegate
			{
				if (this.currentlySelectedCollection == tc && tc.tools.Count >= 1)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound(), false));
				}
				this.ChooseCollection(tc, true);
			};
			if (tc.tools != null)
			{
				GameObject gameObject;
				if (tc.tools.Count < this.smallCollectionMax)
				{
					gameObject = Util.KInstantiateUI(this.Prefab_collectionContainer, base.gameObject, true);
					gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() - 1);
					gameObject.transform.localScale = Vector3.one;
					gameObject.rectTransform().sizeDelta = new Vector2((float)(tc.tools.Count * 75), 50f);
					tc.MaskContainer = gameObject.GetComponentInChildren<Mask>().gameObject;
				}
				else
				{
					gameObject = Util.KInstantiateUI(this.Prefab_collectionContainerWindow, base.gameObject, true);
					gameObject.transform.localScale = Vector3.one;
					gameObject.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
					tc.MaskContainer = gameObject.GetComponentInChildren<GridLayoutGroup>().gameObject;
				}
				tc.UIMenuDisplay = gameObject;
				for (int j = 0; j < tc.tools.Count; j++)
				{
					ToolMenu.ToolInfo ti = tc.tools[j];
					GameObject gameObject2 = Util.KInstantiateUI(this.toolIconPrefab, tc.MaskContainer, true);
					gameObject2.name = ti.text;
					ti.toggle = gameObject2.GetComponent<KToggle>();
					if (ti.collection.tools.Count > 1)
					{
						RectTransform rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
						if (gameObject2.name.Length > 12)
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
		for (int j = 0; j < this.toolCollections.Length; j++)
		{
			ToolMenu.ToolCollection tc = this.toolCollections[j];
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
			for (int k = 0; k < tc.tools.Count; k++)
			{
				if (tc.tools[k] == this.currentlySelectedTool)
				{
					this.SetToggleState(tc.tools[k].toggle, true);
				}
				else
				{
					this.SetToggleState(tc.tools[k].toggle, false);
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
		for (int i = 0; i < this.toolCollections.Length; i++)
		{
			ToolMenu.ToolCollection tc = this.toolCollections[i];
			if (this.currentlySelectedCollection == tc)
			{
				if (this.currentlySelectedTool != null && this.currentlySelectedTool.collection != tc)
				{
					this.ChooseTool(null);
				}
				if (this.currentlySelectedCollection.tools != null && this.currentlySelectedCollection.tools.Count == 1)
				{
					this.ChooseTool(this.currentlySelectedCollection.tools[0]);
				}
				else if (autoSelectTool)
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
			if (this.currentlySelectedCollection == tc)
			{
				this.SetToggleState(tc.toggle.GetComponent<KToggle>(), true);
			}
			else
			{
				this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
			}
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
		ImageToggleState component = toggle.GetComponent<ImageToggleState>();
		if (state)
		{
			toggle.Select();
			toggle.isOn = true;
			toggle.ActivateFlourish(true);
			if (component)
			{
				component.SetActive();
			}
		}
		else
		{
			toggle.Deselect();
			toggle.isOn = false;
			toggle.ActivateFlourish(false);
			if (component)
			{
				component.SetInactive();
			}
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
			for (int i = 0; i < this.toolCollections.Length; i++)
			{
				global::Action toolHotkey = this.toolCollections[i].hotkey;
				if (toolHotkey != global::Action.NumActions && e.IsAction(toolHotkey) && (this.currentlySelectedCollection == null || (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Find((ToolMenu.ToolInfo t) => GameInputMapping.CompareActionKeyCodes(t.hotkey, toolHotkey)) == null)))
				{
					if (this.currentlySelectedCollection != this.toolCollections[i])
					{
						this.ChooseCollection(this.toolCollections[i], false);
						this.ChooseTool(this.toolCollections[i].tools[0]);
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
				for (int j = 0; j < this.toolCollections[i].tools.Count; j++)
				{
					if ((this.currentlySelectedCollection == null && this.toolCollections[i].tools.Count == 1) || this.currentlySelectedCollection == this.toolCollections[i] || (this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Count == 1 && this.toolCollections[i].tools.Count == 1))
					{
						global::Action hotkey = this.toolCollections[i].tools[j].hotkey;
						if (e.IsAction(hotkey) && e.TryConsume(hotkey))
						{
							if (this.toolCollections[i].tools.Count == 1 && this.currentlySelectedCollection != this.toolCollections[i])
							{
								this.ChooseCollection(this.toolCollections[i], false);
							}
							else if (this.currentlySelectedTool != this.toolCollections[i].tools[j])
							{
								this.ChooseTool(this.toolCollections[i].tools[j]);
							}
						}
						else if (GameInputMapping.CompareActionKeyCodes(e.GetAction(), hotkey))
						{
							e.Consumed = true;
						}
					}
				}
			}
			if ((this.currentlySelectedTool != null || this.currentlySelectedCollection != null) && !e.Consumed)
			{
				if (e.TryConsume(global::Action.MouseRight) || e.TryConsume(global::Action.Escape))
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
			else if (!PlayerController.Instance.IsUsingDefaultTool() && !e.Consumed && (e.TryConsume(global::Action.MouseRight) || e.TryConsume(global::Action.Escape)))
			{
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	protected void BuildCollectionToggles()
	{
		for (int i = 0; i < this.toolCollections.Length; i++)
		{
			ToolMenu.ToolCollection toolCollection = this.toolCollections[i];
			if (!(toolCollection.toggle == null))
			{
				GameObject toggle = toolCollection.toggle;
				foreach (Sprite sprite in this.icons)
				{
					if (sprite != null && sprite.name == toolCollection.icon)
					{
						Image component = toggle.transform.FindChild("FG").GetComponent<Image>();
						component.sprite = sprite;
						break;
					}
				}
				Transform transform = toggle.transform.FindChild("Text");
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
					if (this.toolCollections[i].tools.Count == 1)
					{
						string hotkeyString = GameUtil.GetHotkeyString(this.toolCollections[i].tools[0].hotkey);
						component3.AddMultiStringTooltip(this.toolCollections[i].tools[0].tooltip + " " + hotkeyString, this.ToggleToolTipTextStyleSetting);
					}
					else
					{
						string text = this.toolCollections[i].tooltip;
						if (this.toolCollections[i].hotkey != global::Action.NumActions)
						{
							text = text + " " + GameUtil.GetHotkeyString(this.toolCollections[i].hotkey);
						}
						component3.AddMultiStringTooltip(text, this.ToggleToolTipTextStyleSetting);
					}
				}
			}
		}
	}

	protected void BuildToolToggles()
	{
		for (int i = 0; i < this.toolCollections.Length; i++)
		{
			ToolMenu.ToolCollection toolCollection = this.toolCollections[i];
			if (!(toolCollection.toggle == null))
			{
				for (int j = 0; j < toolCollection.tools.Count; j++)
				{
					GameObject gameObject = toolCollection.tools[j].toggle.gameObject;
					foreach (Sprite sprite in this.icons)
					{
						if (sprite != null && sprite.name == toolCollection.tools[j].icon)
						{
							Image component = gameObject.transform.FindChild("FG").GetComponent<Image>();
							component.sprite = sprite;
							break;
						}
					}
					Transform transform = gameObject.transform.FindChild("Text");
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
		foreach (ToolMenu.ToolCollection toolCollection in this.toolCollections)
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
		return flag;
	}

	public static ToolMenu Instance;

	public ToolMenu.ToolCollection[] toolCollections;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting CategoryLabelTextStyle_LeftAlign;

	public ToolMenu.ToolCollection currentlySelectedCollection;

	public ToolMenu.ToolInfo currentlySelectedTool;

	public GameObject toolIconPrefab;

	public GameObject collectionIconPrefab;

	[SerializeField]
	private Sprite[] icons;

	public GameObject Prefab_collectionContainer;

	public GameObject Prefab_collectionContainerWindow;

	private Coroutine activeOpenAnimationRoutine;

	private Coroutine activeCloseAnimationRoutine;

	private int smallCollectionMax = 5;

	public ToolParameterMenu toolParameterMenu;

	private HashSet<global::Action> boundRootActions = new HashSet<global::Action>();

	private HashSet<global::Action> boundSubgroupActions = new HashSet<global::Action>();

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
}
