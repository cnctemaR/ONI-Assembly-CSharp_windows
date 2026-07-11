using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosMenu : KModalScreen, SteamUGCService.IUGCEventHandler
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
		this.RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (GameObject gameObject in this.buttons)
		{
			global::UnityEngine.Object.Destroy(gameObject);
		}
		this.buttons.Clear();
		this.RebuildUGCButtons();
	}

	private void RebuildUGCButtons()
	{
		List<SteamUGCService.Subscribed> subscribed = SteamUGCService.Instance.GetSubscribed("scenario");
		bool flag = subscribed.Count > 0;
		this.noScenariosText.gameObject.SetActive(!flag);
		this.contentRoot.gameObject.SetActive(flag);
		bool flag2 = true;
		if (subscribed.Count != 0)
		{
			for (int i = 0; i < subscribed.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(this.ugcButtonPrefab, this.ugcContainer, false);
				gameObject.name = subscribed[i].title + "_button";
				gameObject.gameObject.SetActive(true);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset fontForLangage = LanguageOptionsScreen.GetFontForLangage(subscribed[i].fileId);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(subscribed[i].title);
				reference.font = fontForLangage;
				Texture2D previewImage = SteamUGCService.Instance.GetPreviewImage(subscribed[i].fileId);
				if (previewImage != null)
				{
					Image reference2 = component.GetReference<Image>("Image");
					reference2.sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				int num = i;
				PublishedFileId_t item = subscribed[num].fileId;
				component2.onClick += delegate
				{
					this.ShowDetails(item);
				};
				component2.onDoubleClick += delegate
				{
					this.LoadScenario(item);
				};
				this.buttons.Add(gameObject);
				if (item == this.activeItem)
				{
					flag2 = false;
				}
			}
		}
		if (flag2)
		{
			this.HideDetails();
		}
	}

	private void LoadScenario(PublishedFileId_t item)
	{
		ulong num;
		string text;
		uint num2;
		SteamUGC.GetItemInstallInfo(item, out num, out text, 1024U, out num2);
		Output.Log(new object[] { "LoadScenario", text, num, num2 });
		global::System.DateTime dateTime;
		byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, new string[] { ".sav" }, out dateTime, false);
		string text2 = Path.Combine(SaveLoader.GetSavePrefix(), "scenario.sav");
		File.WriteAllBytes(text2, bytesFromZip);
		SaveLoader.SetActiveSaveFilePath(text2);
		Time.timeScale = 0f;
		App.LoadScene("backend");
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		GameObject gameObject = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void ShowDetails(PublishedFileId_t item)
	{
		this.activeItem = item;
		SteamUGCDetails_t details = SteamUGCService.Instance.GetDetails(item);
		this.scenarioTitle.text = details.m_rgchTitle;
		this.scenarioDetails.text = details.m_rgchDescription;
		this.loadScenarioButton.onClick += delegate
		{
			this.LoadScenario(item);
		};
		this.detailsRoot.gameObject.SetActive(true);
	}

	private void HideDetails()
	{
		this.detailsRoot.gameObject.SetActive(false);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SteamUGCService.Instance.ugcEventHandlers.Add(this);
		this.HideDetails();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		SteamUGCService.Instance.ugcEventHandlers.Remove(this);
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=scenario");
	}

	public void OnUGCItemInstalled(ItemInstalled_t pCallback)
	{
	}

	public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		this.RebuildScreen();
	}

	public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		this.RebuildScreen();
	}

	public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
	{
	}

	public void OnUGCRefresh()
	{
		this.RebuildScreen();
	}

	public const string TAG_SCENARIO = "scenario";

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton loadScenarioButton;

	[Space]
	public GameObject ugcContainer;

	public GameObject ugcButtonPrefab;

	public LocText noScenariosText;

	public RectTransform contentRoot;

	public RectTransform detailsRoot;

	public LocText scenarioTitle;

	public LocText scenarioDetails;

	private PublishedFileId_t activeItem;

	private List<GameObject> buttons = new List<GameObject>();
}
