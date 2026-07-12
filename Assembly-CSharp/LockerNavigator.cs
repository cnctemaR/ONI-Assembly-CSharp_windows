using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LockerNavigator : KModalScreen
{
	public GameObject ContentSlot
	{
		get
		{
			return this.slot.gameObject;
		}
	}

	protected override void OnActivate()
	{
		LockerNavigator.Instance = this;
		this.Show(false);
		this.backButton.onClick += this.OnClickBack;
	}

	public override float GetSortKey()
	{
		return 41f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.PopScreen();
		}
		base.OnKeyDown(e);
	}

	public override void Show(bool show = true)
	{
		base.Show(show);
		if (!show && this.navigationHistory.Count > 0)
		{
			this.navigationHistory[this.navigationHistory.Count - 1].SetActive(false);
			this.navigationHistory.Clear();
		}
		StreamedTextures.SetBundlesLoaded(show);
	}

	private void OnClickBack()
	{
		this.PopScreen();
	}

	public void PushScreen(GameObject screen)
	{
		if (screen == null)
		{
			return;
		}
		if (this.navigationHistory.Count == 0)
		{
			this.Show(true);
			if (KPrivacyPrefs.instance.disableDataCollection)
			{
				LockerNavigator.MakeDataCollectionWarningPopup(base.gameObject.transform.parent.gameObject);
			}
		}
		if (this.navigationHistory.Count > 0 && screen == this.navigationHistory[this.navigationHistory.Count - 1])
		{
			return;
		}
		if (this.navigationHistory.Count > 0)
		{
			this.navigationHistory[this.navigationHistory.Count - 1].SetActive(false);
		}
		this.navigationHistory.Add(screen);
		this.navigationHistory[this.navigationHistory.Count - 1].SetActive(true);
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		this.RefreshButtons();
	}

	public bool PopScreen()
	{
		while (this.preventScreenPop.Count > 0)
		{
			int num = this.preventScreenPop.Count - 1;
			Func<bool> func = this.preventScreenPop[num];
			this.preventScreenPop.RemoveAt(num);
			if (func())
			{
				return true;
			}
		}
		this.navigationHistory[this.navigationHistory.Count - 1].SetActive(false);
		this.navigationHistory.RemoveAt(this.navigationHistory.Count - 1);
		if (this.navigationHistory.Count > 0)
		{
			this.navigationHistory[this.navigationHistory.Count - 1].SetActive(true);
			this.RefreshButtons();
			return true;
		}
		this.Show(false);
		return false;
	}

	public void PopAllScreens()
	{
		int num = 0;
		while (this.PopScreen())
		{
			if (num > 100)
			{
				DebugUtil.DevAssert(false, string.Format("Can't close all LockerNavigator screens, hit limit of trying to close {0} screens", 100), null);
				return;
			}
			num++;
		}
	}

	private void RefreshButtons()
	{
		this.backButton.isInteractable = true;
	}

	public void ShowDialogPopup(Action<InfoDialogScreen> configureDialogFn)
	{
		InfoDialogScreen dialog = Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, this.ContentSlot, false);
		configureDialogFn(dialog);
		dialog.Activate();
		dialog.gameObject.AddOrGet<LayoutElement>().ignoreLayout = true;
		dialog.gameObject.AddOrGet<RectTransform>().Fill();
		Func<bool> preventScreenPopFn = delegate
		{
			dialog.Deactivate();
			return true;
		};
		this.preventScreenPop.Add(preventScreenPopFn);
		InfoDialogScreen dialog2 = dialog;
		dialog2.onDeactivateFn = (global::System.Action)Delegate.Combine(dialog2.onDeactivateFn, new global::System.Action(delegate
		{
			this.preventScreenPop.Remove(preventScreenPopFn);
		}));
	}

	public static void MakeDataCollectionWarningPopup(GameObject fullscreenParent)
	{
		Action<InfoDialogScreen> <>9__2;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			InfoDialogScreen infoDialogScreen = dialog.SetHeader(UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.HEADER).AddPlainText(UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.BODY).AddOption(UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.BUTTON_OK, delegate(InfoDialogScreen d)
			{
				d.Deactivate();
			}, true);
			string text = UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.BUTTON_OPEN_SETTINGS;
			Action<InfoDialogScreen> action;
			if ((action = <>9__2) == null)
			{
				action = (<>9__2 = delegate(InfoDialogScreen d)
				{
					d.Deactivate();
					LockerNavigator.Instance.PopAllScreens();
					LockerMenuScreen.Instance.Show(false);
					Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, fullscreenParent, true).ShowMetricsScreen();
				});
			}
			infoDialogScreen.AddOption(text, action, false);
		});
	}

	public static LockerNavigator Instance;

	[SerializeField]
	private RectTransform slot;

	[SerializeField]
	private KButton backButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	public GameObject kleiInventoryScreen;

	[SerializeField]
	public GameObject duplicantCatalogueScreen;

	[SerializeField]
	public GameObject outfitDesignerScreen;

	[SerializeField]
	public GameObject outfitBrowserScreen;

	private List<GameObject> navigationHistory = new List<GameObject>();

	private Dictionary<string, GameObject> screens = new Dictionary<string, GameObject>();

	public List<Func<bool>> preventScreenPop = new List<Func<bool>>();
}
