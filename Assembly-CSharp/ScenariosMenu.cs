using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosMenu : KModalScreen, SteamUGCService.IClient
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
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
		ListPool<SteamUGCService.Mod, ScenariosMenu>.PooledList pooledList = ListPool<SteamUGCService.Mod, ScenariosMenu>.Allocate();
		bool flag = pooledList.Count > 0;
		this.noScenariosText.gameObject.SetActive(!flag);
		this.contentRoot.gameObject.SetActive(flag);
		bool flag2 = true;
		if (pooledList.Count != 0)
		{
			for (int i = 0; i < pooledList.Count; i++)
			{
				GameObject gameObject = Util.KInstantiateUI(this.ugcButtonPrefab, this.ugcContainer, false);
				gameObject.name = pooledList[i].title + "_button";
				gameObject.gameObject.SetActive(true);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset fontForLangage = LanguageOptionsScreen.GetFontForLangage(pooledList[i].fileId);
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(pooledList[i].title);
				reference.font = fontForLangage;
				Texture2D previewImage = pooledList[i].previewImage;
				if (previewImage != null)
				{
					component.GetReference<Image>("Image").sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float)previewImage.width, (float)previewImage.height)), Vector2.one * 0.5f);
				}
				KButton component2 = gameObject.GetComponent<KButton>();
				int num = i;
				PublishedFileId_t item = pooledList[num].fileId;
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
		pooledList.Recycle();
	}

	private void LoadScenario(PublishedFileId_t item)
	{
		ulong num;
		string text;
		uint num2;
		SteamUGC.GetItemInstallInfo(item, out num, out text, 1024U, out num2);
		DebugUtil.LogArgs(new object[] { "LoadScenario", text, num, num2 });
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
		KScreen component = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void ShowDetails(PublishedFileId_t item)
	{
		this.activeItem = item;
		SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(item);
		if (mod != null)
		{
			this.scenarioTitle.text = mod.title;
			this.scenarioDetails.text = mod.description;
		}
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
		SteamUGCService.Instance.AddClient(this);
		this.HideDetails();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		SteamUGCService.Instance.RemoveClient(this);
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=scenario");
	}

	public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
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
