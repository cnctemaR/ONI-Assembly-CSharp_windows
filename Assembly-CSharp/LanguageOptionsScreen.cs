using System;
using System.Collections.Generic;
using System.IO;
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
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		LocText reference = this.dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title");
		reference.SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
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
		foreach (GameObject gameObject in this.buttons)
		{
			global::UnityEngine.Object.Destroy(gameObject);
		}
		this.buttons.Clear();
		this.uninstallButton.isInteractable = KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString()) != Localization.SelectedLanguageType.None.ToString();
		this.RebuildPreinstalledButtons();
		this.RebuildUGCButtons();
	}

	private void RebuildPreinstalledButtons()
	{
		foreach (string text in Localization.PreinstalledLanguages)
		{
			if (!(text != Localization.DEFAULT_LANGUAGE_CODE) || File.Exists(Localization.GetPreinstalledLocalizationFilePath(text)))
			{
				GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.preinstalledLanguagesContainer, false);
				gameObject.name = text + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset fontForLocale = Localization.GetFontForLocale(text);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(Localization.GetPreinstalledLocalizationTitle(text));
				reference.font = fontForLocale;
				Texture2D preinstalledLocalizationImage = Localization.GetPreinstalledLocalizationImage(text);
				if (preinstalledLocalizationImage != null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(preinstalledLocalizationImage, new Rect(Vector2.zero, new Vector2((float)preinstalledLocalizationImage.width, (float)preinstalledLocalizationImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				string _code = text;
				component2.onClick += delegate
				{
					this.ActivatePreinstalledLanguage(_code);
				};
				this.buttons.Add(gameObject);
			}
		}
	}

	private void RebuildUGCButtons()
	{
		List<SteamUGCService.Subscibed> subs = SteamUGCService.Instance.GetSubscribed();
		if (subs.Count != 0)
		{
			for (int i = 0; i < subs.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.ugcLanguagesContainer, false);
				gameObject.name = subs[i].title + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset fontForLangage = SteamUGCService.GetFontForLangage(subs[i].fileId);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, subs[i].title));
				reference.font = fontForLangage;
				Texture2D previewImage = SteamUGCService.Instance.GetPreviewImage(subs[i].fileId);
				if (previewImage != null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				int index = i;
				component2.onClick += delegate
				{
					PublishedFileId_t fileId = subs[index].fileId;
					this.InstallLanguage(fileId);
				};
				this.buttons.Add(gameObject);
			}
		}
	}

	private void ActivatePreinstalledLanguage(string code)
	{
		Localization.LoadPreinstalledTranslation(code);
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null, null);
	}

	private void InstallLanguage(PublishedFileId_t item)
	{
		Localization.SetLanguage(item);
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
		{
			Application.Quit();
		}, delegate
		{
			App.LoadScene("frontend");
		}, null, null, null, null, null);
	}

	private void Uninstall()
	{
		ConfirmDialogScreen confirmDialog = this.GetConfirmDialog();
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			ConfirmDialogScreen confirmDialog2 = this.GetConfirmDialog();
			confirmDialog2.PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
			{
				Application.Quit();
			}, delegate
			{
				App.LoadScene("frontend");
			}, null, null, null, null, null);
		}, delegate
		{
		}, null, null, null, null, null);
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		GameObject gameObject = KScreenManager.AddChild(this.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
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

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton uninstallButton;

	[Space]
	public GameObject languageButtonPrefab;

	public GameObject preinstalledLanguagesTitle;

	public GameObject preinstalledLanguagesContainer;

	public GameObject ugcLanguagesTitle;

	public GameObject ugcLanguagesContainer;

	private List<GameObject> buttons = new List<GameObject>();
}
