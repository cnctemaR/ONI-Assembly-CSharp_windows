using System;
using System.Collections.Generic;
using System.IO;
using STRINGS;
using UnityEngine;

public class LoadScreen : KModalScreen
{
	public static LoadScreen Instance { get; private set; }

	public static void DestroyInstance()
	{
		LoadScreen.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		global::Debug.Assert(LoadScreen.Instance == null);
		LoadScreen.Instance = this;
		base.OnPrefabInit();
		this.savenameRowPool = new UIPool<HierarchyReferences>(this.saveButtonPrefab);
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		if (this.onClick == null)
		{
			this.onClick = new Action<string>(this.SetSelectedGame);
		}
		if (this.closeButton != null)
		{
			this.closeButton.onClick += delegate
			{
				this.Deactivate();
			};
		}
		if (this.loadButton != null)
		{
			this.loadButton.onClick += this.Load;
		}
		if (this.deleteButton != null)
		{
			this.deleteButton.onClick += this.Delete;
			this.deleteButton.isInteractable = false;
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.RefreshFiles();
	}

	private void GetFilesList()
	{
		this.saveFiles = new Dictionary<string, List<LoadScreen.SaveGameFileDetails>>();
		List<string> allFiles = SaveLoader.GetAllFiles();
		if (allFiles.Count > 0)
		{
			for (int i = 0; i < allFiles.Count; i++)
			{
				bool flag = this.IsFileValid(allFiles[i]);
				if (flag)
				{
					Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = this.GetFileInfo(allFiles[i]);
					SaveGame.Header first = fileInfo.first;
					SaveGame.GameInfo second = fileInfo.second;
					global::System.DateTime lastWriteTime = File.GetLastWriteTime(allFiles[i]);
					string text = ((!(second.originalSaveName != string.Empty)) ? allFiles[i] : second.originalSaveName);
					text = Path.GetFileNameWithoutExtension(text);
					LoadScreen.SaveGameFileDetails saveGameFileDetails = default(LoadScreen.SaveGameFileDetails);
					saveGameFileDetails.BaseName = second.baseName;
					saveGameFileDetails.FileName = allFiles[i];
					saveGameFileDetails.FileDate = lastWriteTime;
					saveGameFileDetails.FileHeader = first;
					saveGameFileDetails.FileInfo = second;
					if (!this.saveFiles.ContainsKey(text))
					{
						this.saveFiles.Add(text, new List<LoadScreen.SaveGameFileDetails>());
					}
					this.saveFiles[text].Add(saveGameFileDetails);
				}
			}
		}
	}

	private bool IsFileValid(string filename)
	{
		bool flag = false;
		try
		{
			SaveGame.Header header;
			flag = SaveLoader.LoadHeader(filename, out header).saveMajorVersion >= 7;
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("Corrupted save file: " + filename + "\n" + ex.ToString());
		}
		return flag;
	}

	private Tuple<SaveGame.Header, SaveGame.GameInfo> GetFileInfo(string filename)
	{
		try
		{
			SaveGame.Header header;
			SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
			if (gameInfo.saveMajorVersion >= 7)
			{
				return new Tuple<SaveGame.Header, SaveGame.GameInfo>(header, gameInfo);
			}
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(ex);
			this.InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.CORRUPTEDSAVE, filename);
		}
		return null;
	}

	private void RefreshFiles()
	{
		if (this.savenameRowPool != null)
		{
			this.savenameRowPool.ClearAll();
		}
		if (this.fileButtonMap != null)
		{
			this.fileButtonMap.Clear();
		}
		this.GetFilesList();
		if (this.saveFiles.Count > 0)
		{
			foreach (KeyValuePair<string, List<LoadScreen.SaveGameFileDetails>> keyValuePair in this.saveFiles)
			{
				this.AddExistingSaveFile(keyValuePair.Key, keyValuePair.Value);
			}
		}
		this.InfoText.text = string.Empty;
		this.CyclesSurvivedValue.text = "-";
		this.DuplicantsAliveValue.text = "-";
		this.deleteButton.isInteractable = false;
		this.loadButton.isInteractable = false;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		this.RefreshFiles();
	}

	protected override void OnDeactivate()
	{
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		this.selectedFileName = null;
		base.OnDeactivate();
	}

	private void AddExistingSaveFile(string savename, List<LoadScreen.SaveGameFileDetails> fileDetailsList)
	{
		HierarchyReferences savenameRow = this.savenameRowPool.GetFreeElement(this.saveButtonRoot, true);
		KButton component = savenameRow.GetReference<RectTransform>("Button").GetComponent<KButton>();
		component.ClearOnClick();
		LocText headerTitle = savenameRow.GetReference<RectTransform>("HeaderTitle").GetComponent<LocText>();
		LocText component2 = savenameRow.GetReference<RectTransform>("HeaderDate").GetComponent<LocText>();
		RectTransform saveDetailsRow = savenameRow.GetReference<RectTransform>("SaveDetailsRow");
		LocText component3 = savenameRow.GetReference<RectTransform>("SaveDetailsBaseName").GetComponent<LocText>();
		RectTransform savefileRowTemplate = savenameRow.GetReference<RectTransform>("SavefileRowTemplate");
		fileDetailsList.Sort((LoadScreen.SaveGameFileDetails x, LoadScreen.SaveGameFileDetails y) => y.FileDate.CompareTo(x.FileDate));
		headerTitle.text = savename;
		component2.text = string.Format("{0:H:mm:ss} " + Localization.GetFileDateFormat(0), fileDetailsList[0].FileDate);
		component3.text = string.Format("Base Name: {0}", fileDetailsList[0].BaseName);
		for (int i = 0; i < savenameRow.transform.childCount; i++)
		{
			GameObject gameObject = savenameRow.transform.GetChild(i).gameObject;
			if (gameObject != null && gameObject.name.Contains("Clone"))
			{
				global::UnityEngine.Object.Destroy(gameObject);
			}
		}
		bool flag = true;
		using (List<LoadScreen.SaveGameFileDetails>.Enumerator enumerator = fileDetailsList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				LoadScreen.SaveGameFileDetails fileDetails = enumerator.Current;
				RectTransform rectTransform = global::UnityEngine.Object.Instantiate<RectTransform>(savefileRowTemplate, savenameRow.transform);
				HierarchyReferences component4 = rectTransform.GetComponent<HierarchyReferences>();
				KButton component5 = rectTransform.GetComponent<KButton>();
				RectTransform reference = component4.GetReference<RectTransform>("NewestLabel");
				RectTransform reference2 = component4.GetReference<RectTransform>("AutoLabel");
				LocText component6 = component4.GetReference<RectTransform>("SaveText").GetComponent<LocText>();
				LocText component7 = component4.GetReference<RectTransform>("DateText").GetComponent<LocText>();
				reference.gameObject.SetActive(flag);
				flag = false;
				reference2.gameObject.SetActive(fileDetails.FileInfo.isAutoSave);
				component6.text = Path.GetFileNameWithoutExtension(fileDetails.FileName);
				component7.text = string.Format("{0:H:mm:ss} " + Localization.GetFileDateFormat(0), fileDetails.FileDate);
				component5.onClick += delegate
				{
					this.onClick(fileDetails.FileName);
				};
				component5.onDoubleClick += delegate
				{
					this.onClick(fileDetails.FileName);
					this.Load();
				};
				this.fileButtonMap.Add(fileDetails.FileName, component5);
			}
		}
		component.onClick += delegate
		{
			bool activeSelf = saveDetailsRow.gameObject.activeSelf;
			for (int j = 0; j < savenameRow.transform.childCount; j++)
			{
				GameObject gameObject2 = savenameRow.transform.GetChild(j).gameObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(!activeSelf);
				}
			}
			headerTitle.transform.parent.gameObject.SetActive(true);
			savefileRowTemplate.gameObject.SetActive(false);
			if (!activeSelf)
			{
				this.onClick(fileDetailsList[0].FileName);
			}
		};
		component.onDoubleClick += delegate
		{
			this.onClick(fileDetailsList[0].FileName);
			LoadingOverlay.Load(new global::System.Action(this.DoLoad));
		};
		savenameRow.transform.SetAsLastSibling();
	}

	public static void ForceStopGame()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.SetIsLoading();
		Grid.CellCount = 0;
		Sim.Shutdown();
	}

	private static bool IsSaveFileFromUnsupportedFutureBuild(SaveGame.Header header)
	{
		return header.buildVersion > 326232U;
	}

	private void SetSelectedGame(string filename)
	{
		if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
		{
			global::Debug.LogError("The filename provided is not valid.");
			this.deleteButton.isInteractable = false;
			return;
		}
		this.deleteButton.isInteractable = true;
		KButton kbutton = ((this.selectedFileName == null) ? null : this.fileButtonMap[this.selectedFileName]);
		if (kbutton != null)
		{
			kbutton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
		this.selectedFileName = filename;
		this.FileName.text = Path.GetFileName(this.selectedFileName);
		kbutton = this.fileButtonMap[this.selectedFileName];
		kbutton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		try
		{
			SaveGame.Header header;
			SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
			string text = Path.GetFileName(filename);
			if (gameInfo.isAutoSave)
			{
				text = text + "\n" + UI.FRONTEND.LOADSCREEN.AUTOSAVEWARNING;
			}
			this.CyclesSurvivedValue.text = gameInfo.numberOfCycles.ToString();
			this.DuplicantsAliveValue.text = gameInfo.numberOfDuplicants.ToString();
			this.InfoText.text = string.Empty;
			if (LoadScreen.IsSaveFileFromUnsupportedFutureBuild(header))
			{
				this.InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_TOO_NEW, filename, header.buildVersion, 326232U);
				this.loadButton.isInteractable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
			else if (gameInfo.saveMajorVersion < 7)
			{
				this.InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.UNSUPPORTED_SAVE_VERSION, new object[] { filename, gameInfo.saveMajorVersion, gameInfo.saveMinorVersion, 7, 8 });
				this.loadButton.isInteractable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
			else if (!this.loadButton.isInteractable)
			{
				this.loadButton.isInteractable = true;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			}
			if (this.InfoText.text == string.Empty && gameInfo.isAutoSave)
			{
				this.InfoText.text = UI.FRONTEND.LOADSCREEN.AUTOSAVEWARNING;
			}
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(ex);
			this.InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.CORRUPTEDSAVE, filename);
			if (this.loadButton.isInteractable)
			{
				this.loadButton.isInteractable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
			this.deleteButton.isInteractable = false;
		}
	}

	private void Load()
	{
		LoadingOverlay.Load(new global::System.Action(this.DoLoad));
	}

	private void DoLoad()
	{
		LoadScreen.DoLoad(this.selectedFileName);
		this.Deactivate();
	}

	private static void DoLoad(string filename)
	{
		ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
		bool flag = true;
		SaveGame.Header header;
		SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
		string text = null;
		string text2 = null;
		if (header.buildVersion > 326232U)
		{
			text = header.buildVersion.ToString();
			text2 = 326232U.ToString();
		}
		else if (gameInfo.saveMajorVersion < 7)
		{
			text = string.Format("v{0}.{1}", gameInfo.saveMajorVersion, gameInfo.saveMinorVersion);
			text2 = string.Format("v{0}.{1}", 7, 8);
		}
		if (!flag)
		{
			GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.LOADFAILED, "Version Mismatch", text, text2), null, null, null, null, null, null, null, null);
			return;
		}
		if (Game.Instance != null)
		{
			LoadScreen.ForceStopGame();
		}
		SaveLoader.SetActiveSaveFilePath(filename);
		Time.timeScale = 0f;
		App.LoadScene("backend");
	}

	private void MoreInfo()
	{
		Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
	}

	private void Delete()
	{
		if (string.IsNullOrEmpty(this.selectedFileName))
		{
			global::Debug.LogError("The path provided is not valid and cannot be deleted.");
			return;
		}
		this.ConfirmDoAction(string.Format(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, Path.GetFileName(this.selectedFileName)), delegate
		{
			this.fileButtonMap[this.selectedFileName].GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.fileButtonMap[this.selectedFileName].isInteractable = true;
			File.Delete(this.selectedFileName);
			this.selectedFileName = null;
			this.RefreshFiles();
		});
	}

	private void ConfirmDoAction(string message, global::System.Action action)
	{
		if (this.confirmScreen == null)
		{
			this.confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, false);
			this.confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			}, null, null, null, null, null, null);
			this.confirmScreen.gameObject.SetActive(true);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			this.Deactivate();
		}
		base.OnKeyUp(e);
	}

	private InspectSaveScreen inspectScreenInstance;

	[SerializeField]
	private HierarchyReferences saveButtonPrefab;

	[SerializeField]
	private GameObject saveButtonRoot;

	[SerializeField]
	private LocText saveDetails;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton loadButton;

	[SerializeField]
	private KButton deleteButton;

	[SerializeField]
	private ColorStyleSetting validSaveFileStyle;

	[SerializeField]
	private ColorStyleSetting invalidSaveFileStyle;

	public LocText FileName;

	public LocText CyclesSurvivedValue;

	public LocText DuplicantsAliveValue;

	public LocText InfoText;

	public Action<string> onClick;

	public bool requireConfirmation = true;

	private UIPool<HierarchyReferences> savenameRowPool;

	private Dictionary<string, KButton> fileButtonMap = new Dictionary<string, KButton>();

	private ConfirmDialogScreen confirmScreen;

	private string selectedFileName;

	private Dictionary<string, List<LoadScreen.SaveGameFileDetails>> saveFiles;

	private struct SaveGameFileDetails
	{
		public string BaseName;

		public string FileName;

		public global::System.DateTime FileDate;

		public SaveGame.Header FileHeader;

		public SaveGame.GameInfo FileInfo;
	}
}
