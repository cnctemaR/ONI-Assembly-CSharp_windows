using System;
using System.Collections.Generic;
using KMod;
using STRINGS;
using UnityEngine;

public class ModsScreen : KModalScreen
{
	protected override void OnActivate()
	{
		base.OnActivate();
		this.closeButtonTitle.onClick += this.Exit;
		this.closeButton.onClick += this.Exit;
		global::System.Action action = delegate
		{
			Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140");
		};
		this.workshopButton.onClick += action;
		Global.Instance.modManager.Sanitize(base.gameObject);
		this.mod_footprint.Clear();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.enabled)
			{
				this.mod_footprint.Add(mod.label);
				if ((byte)(mod.loaded_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) == (byte)(mod.available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)))
				{
					mod.Uncrash();
				}
			}
		}
		this.BuildDisplay();
		Manager modManager = Global.Instance.modManager;
		modManager.on_update = (Manager.OnUpdate)Delegate.Combine(modManager.on_update, new Manager.OnUpdate(this.RebuildDisplay));
	}

	protected override void OnDeactivate()
	{
		Manager modManager = Global.Instance.modManager;
		modManager.on_update = (Manager.OnUpdate)Delegate.Remove(modManager.on_update, new Manager.OnUpdate(this.RebuildDisplay));
		base.OnDeactivate();
	}

	private void Exit()
	{
		Global.Instance.modManager.Save();
		if (!Global.Instance.modManager.MatchFootprint(this.mod_footprint, Content.Strings | Content.DLL | Content.Translation | Content.Animation))
		{
			Global.Instance.modManager.RestartDialog(UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.TITLE, UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.MESSAGE, new global::System.Action(this.Deactivate), true, base.gameObject, null);
		}
		else
		{
			this.Deactivate();
		}
		Global.Instance.modManager.events.Clear();
	}

	private void RebuildDisplay(object change_source)
	{
		if (!object.ReferenceEquals(change_source, this))
		{
			this.BuildDisplay();
		}
	}

	private void BuildDisplay()
	{
		foreach (ModsScreen.DisplayedMod displayedMod in this.displayedMods)
		{
			if (displayedMod.rect_transform != null)
			{
				global::UnityEngine.Object.Destroy(displayedMod.rect_transform.gameObject);
			}
		}
		this.displayedMods.Clear();
		ModsScreen.ModOrderingDragListener modOrderingDragListener = new ModsScreen.ModOrderingDragListener(this, this.displayedMods);
		for (int num = 0; num != Global.Instance.modManager.mods.Count; num++)
		{
			Mod mod = Global.Instance.modManager.mods[num];
			if (mod.status != Mod.Status.NotInstalled && mod.status != Mod.Status.UninstallPending && mod.HasAnyContent(Content.LayerableFiles | Content.Strings | Content.DLL))
			{
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.entryPrefab, this.entryParent.gameObject, false);
				this.displayedMods.Add(new ModsScreen.DisplayedMod
				{
					rect_transform = hierarchyReferences.gameObject.GetComponent<RectTransform>(),
					mod_index = num
				});
				DragMe component = hierarchyReferences.GetComponent<DragMe>();
				component.listener = modOrderingDragListener;
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				reference.text = mod.title;
				ToolTip reference2 = hierarchyReferences.GetReference<ToolTip>("Description");
				reference2.toolTip = mod.description;
				if (mod.crash_count != 0)
				{
					reference.color = Color.Lerp(Color.white, Color.red, (float)mod.crash_count / 3f);
				}
				KButton reference3 = hierarchyReferences.GetReference<KButton>("ManageButton");
				reference3.isInteractable = mod.is_managed;
				if (reference3.isInteractable)
				{
					reference3.GetComponent<ToolTip>().toolTip = mod.manage_tooltip;
					reference3.onClick += mod.on_managed;
				}
				MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
				toggle.ChangeState((!mod.enabled) ? 0 : 1);
				MultiToggle toggle2 = toggle;
				toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
				{
					this.OnToggleClicked(toggle, mod.label);
				}));
				toggle.GetComponent<ToolTip>().OnToolTip = () => (!mod.enabled) ? UI.FRONTEND.MODS.TOOLTIPS.DISABLED : UI.FRONTEND.MODS.TOOLTIPS.ENABLED;
				hierarchyReferences.gameObject.SetActive(true);
			}
		}
		foreach (ModsScreen.DisplayedMod displayedMod2 in this.displayedMods)
		{
			displayedMod2.rect_transform.gameObject.SetActive(true);
		}
		if (this.displayedMods.Count == 0)
		{
		}
	}

	private void OnToggleClicked(MultiToggle toggle, Label mod)
	{
		Manager modManager = Global.Instance.modManager;
		bool flag = modManager.IsModEnabled(mod);
		flag = !flag;
		toggle.ChangeState((!flag) ? 0 : 1);
		modManager.EnableMod(mod, flag, this);
	}

	[SerializeField]
	private KButton closeButtonTitle;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton workshopButton;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private Transform entryParent;

	private List<ModsScreen.DisplayedMod> displayedMods = new List<ModsScreen.DisplayedMod>();

	private List<Label> mod_footprint = new List<Label>();

	private struct DisplayedMod
	{
		public RectTransform rect_transform;

		public int mod_index;
	}

	private class ModOrderingDragListener : DragMe.IDragListener
	{
		public ModOrderingDragListener(ModsScreen screen, List<ModsScreen.DisplayedMod> mods)
		{
			this.screen = screen;
			this.mods = mods;
		}

		public void OnBeginDrag(Vector2 pos)
		{
			this.startDragIdx = this.GetDragIdx(pos);
		}

		public void OnEndDrag(Vector2 pos)
		{
			if (this.startDragIdx < 0)
			{
				return;
			}
			int dragIdx = this.GetDragIdx(pos);
			int num = ((dragIdx < 0 || dragIdx == this.startDragIdx) ? Global.Instance.modManager.mods.Count : this.mods[dragIdx].mod_index);
			Global.Instance.modManager.Reinsert(this.mods[this.startDragIdx].mod_index, num, this);
			this.screen.BuildDisplay();
		}

		private int GetDragIdx(Vector2 pos)
		{
			int num = -1;
			for (int i = 0; i < this.mods.Count; i++)
			{
				if (RectTransformUtility.RectangleContainsScreenPoint(this.mods[i].rect_transform, pos))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		private List<ModsScreen.DisplayedMod> mods;

		private ModsScreen screen;

		private int startDragIdx = -1;
	}
}
