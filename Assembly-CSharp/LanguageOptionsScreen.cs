using System;
using System.Collections.Generic;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.TRANSLATIONS_SCREEN.TITLE);
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		LocText component = this.dismissButton.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.workshopButton.onClick += delegate
		{
			this.OnClickOpenWorkshop();
		};
		this.uninstallButton.onClick += delegate
		{
			this.OnClickUninstall();
		};
		this.RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (KeyValuePair<PublishedFileId_t, GameObject> keyValuePair in this.buttons)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		this.buttons.Clear();
		this.uninstallButton.isInteractable = SteamUGCService.HasInstalledLanguage();
		List<SteamUGCService.Subscibed> subs = SteamUGCService.Instance.GetSubscribed();
		if (subs.Count != 0)
		{
			Transform child = this.transform.GetChild(1).GetChild(0).GetChild(0);
			for (int i = 0; i < subs.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(this.buttonrefab, child.gameObject, false);
				gameObject.name = subs[i].title + "_button";
				TMP_FontAsset fontForLangage = SteamUGCService.GetFontForLangage(subs[i].fileId);
				LocText component = gameObject.transform.GetChild(1).GetComponent<LocText>();
				component.SetText(subs[i].title);
				component.font = fontForLangage;
				Texture2D previewImage = SteamUGCService.Instance.GetPreviewImage(subs[i].fileId);
				if (previewImage != null)
				{
					Image component2 = gameObject.transform.GetChild(0).GetComponent<Image>();
					component2.sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component3 = gameObject.GetComponent<KButton>();
				int index = i;
				component3.onClick += delegate
				{
					PublishedFileId_t fileId = subs[index].fileId;
					this.InstallLanguage(fileId);
				};
				this.buttons.Add(subs[i].fileId, gameObject);
			}
		}
	}

	private void InstallLanguage(PublishedFileId_t item)
	{
		Localization.SetLanguage(item);
		ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
		confirmDialogScreen.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null);
	}

	private void Uninstall()
	{
		ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
		confirmDialogScreen.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			ConfirmDialogScreen confirmDialogScreen2 = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen2.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
			{
				Application.Quit();
			}, delegate
			{
				App.LoadScene("frontend");
			}, null, null, null, null);
		}, delegate
		{
		}, null, null, null, null);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SteamUGCService.Instance.OnRefreshLanguage = delegate
		{
			this.RebuildScreen();
		};
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		SteamUGCService.Instance.OnRefreshLanguage = null;
	}

	private void OnClickUninstall()
	{
		this.Uninstall();
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");
	}

	public LocText title;

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton uninstallButton;

	public GameObject buttonrefab;

	private Dictionary<PublishedFileId_t, GameObject> buttons = new Dictionary<PublishedFileId_t, GameObject>();
}
