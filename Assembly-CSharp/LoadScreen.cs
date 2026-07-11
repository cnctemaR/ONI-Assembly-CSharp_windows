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
		LoadScreen.Instance = this;
		base.OnPrefabInit();
		this.saveButtonPool = new UIPool<KButton>(this.saveButtonPrefab);
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
				base.Show(false);
			};
		}
		if (this.loadButton != null)
		{
			this.loadButton.onClick += this.Load;
		}
		if (this.deleteButton != null)
		{
			this.deleteButton.onClick += this.Delete;
		}
		if (this.moreInfoButton != null)
		{
			this.moreInfoButton.onClick += this.MoreInfo;
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.RefreshFiles();
	}

	private void RefreshFiles()
	{
		if (this.saveButtonPool != null)
		{
			this.saveButtonPool.ClearAll();
		}
		if (this.fileButtonMap != null)
		{
			this.fileButtonMap.Clear();
		}
		List<string> allFiles = SaveLoader.GetAllFiles();
		if (allFiles.Count > 0)
		{
			for (int i = 0; i < allFiles.Count; i++)
			{
				this.AddExistingSaveFile(allFiles[i]);
			}
			this.SetSelectedGame(allFiles[0]);
			this.deleteButton.isInteractable = true;
		}
		else
		{
			this.saveDetails.text = string.Empty;
			this.deleteButton.isInteractable = false;
			this.loadButton.isInteractable = false;
		}
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

	private void AddExistingSaveFile(string filename)
	{
		KButton freeElement = this.saveButtonPool.GetFreeElement(this.saveButtonRoot, true);
		freeElement.ClearOnClick();
		LocText componentInChildren = freeElement.GetComponentInChildren<LocText>();
		global::System.DateTime lastWriteTime = File.GetLastWriteTime(filename);
		componentInChildren.text = string.Format("{0}\n{1:H:mm:ss}\n" + Localization.GetFileDateFormat(1), Path.GetFileNameWithoutExtension(filename), lastWriteTime);
		freeElement.onClick += delegate
		{
			this.onClick(filename);
		};
		bool flag = false;
		try
		{
			SaveGame.Header header;
			flag = SaveLoader.LoadHeader(filename, out header).saveMajorVersion >= 7;
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning("Corrupted save file: " + filename + "\n" + ex.ToString(), null);
		}
		if (flag)
		{
			freeElement.onDoubleClick += delegate
			{
				this.onClick(filename);
				this.DoLoad();
			};
		}
		ImageToggleState component = freeElement.GetComponent<ImageToggleState>();
		component.colorStyleSetting = ((!flag) ? this.invalidSaveFileStyle : this.validSaveFileStyle);
		component.RefreshColorStyle();
		component.SetState(ImageToggleState.State.Inactive);
		component.ResetColor();
		freeElement.transform.SetAsLastSibling();
		this.fileButtonMap.Add(filename, freeElement);
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
		return header.buildVersion > 279457U;
	}

	private void SetSelectedGame(string filename)
	{
		if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
		{
			global::Debug.LogError("The filename provided is not valid.", null);
			return;
		}
		KButton kbutton = ((this.selectedFileName == null) ? null : this.fileButtonMap[this.selectedFileName]);
		if (kbutton != null)
		{
			kbutton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
		this.selectedFileName = filename;
		kbutton = this.fileButtonMap[this.selectedFileName];
		kbutton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		this.moreInfoButton.gameObject.SetActive(false);
		try
		{
			SaveGame.Header header;
			SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
			string text = UI.FRONTEND.LOADSCREEN.SAVEDETAILS;
			string text2 = string.Format("{0:H:mm:ss}\n" + Localization.GetFileDateFormat(0), File.GetLastWriteTime(filename));
			string text3 = Path.GetFileName(filename);
			if (gameInfo.isAutoSave)
			{
				text3 = text3 + "\n" + UI.FRONTEND.LOADSCREEN.AUTOSAVEWARNING;
			}
			string text4 = string.Format(text, new object[]
			{
				text3,
				text2,
				gameInfo.baseName,
				gameInfo.numberOfDuplicants.ToString(),
				gameInfo.numberOfCycles.ToString()
			});
			this.saveDetails.text = text4;
			if (LoadScreen.IsSaveFileFromUnsupportedFutureBuild(header))
			{
				this.saveDetails.text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_TOO_NEW, filename, header.buildVersion, 279457U);
				this.loadButton.isInteractable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
			else if (gameInfo.saveMajorVersion < 7)
			{
				this.saveDetails.text = string.Format(UI.FRONTEND.LOADSCREEN.UNSUPPORTED_SAVE_VERSION, new object[] { filename, gameInfo.saveMajorVersion, gameInfo.saveMinorVersion, 7, 4 });
				this.loadButton.isInteractable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
			else if (!this.loadButton.isInteractable)
			{
				this.loadButton.isInteractable = true;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			}
		}
		catch (Exception ex)
		{
			global::Debug.LogWarning(ex, null);
			this.saveDetails.text = string.Format(UI.FRONTEND.LOADSCREEN.CORRUPTEDSAVE, filename);
			if (this.loadButton.isInteractable)
			{
				this.loadButton.isInteractable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
		}
	}

	private void Load()
	{
		LoadingOverlay.Load(new global::System.Action(this.DoLoad));
	}

	private void DoLoad()
	{
		ReportErrorDialog.MOST_RECENT_SAVEFILE = this.selectedFileName;
		bool flag = true;
		SaveGame.Header header;
		SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(this.selectedFileName, out header);
		string text = null;
		string text2 = null;
		if (header.buildVersion > 279457U)
		{
			text = header.buildVersion.ToString();
			text2 = 279457U.ToString();
		}
		else if (gameInfo.saveMajorVersion < 7)
		{
			text = string.Format("v{0}.{1}", gameInfo.saveMajorVersion, gameInfo.saveMinorVersion);
			text2 = string.Format("v{0}.{1}", 7, 4);
		}
		if (!flag)
		{
			GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.LOADFAILED, "Version Mismatch", text, text2), null, null, null, null, null, null, null);
			return;
		}
		if (Game.Instance != null)
		{
			LoadScreen.ForceStopGame();
		}
		SaveLoader.SetActiveSaveFilePath(this.selectedFileName);
		Time.timeScale = 0f;
		App.LoadScene("backend");
		this.Deactivate();
	}

	private void MoreInfo()
	{
		Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
	}

	private void Delete()
	{
		if (string.IsNullOrEmpty(this.selectedFileName))
		{
			global::Debug.LogError("The path provided is not valid and cannot be deleted.", null);
			return;
		}
		this.ConfirmDoAction(string.Format(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, Path.GetFileName(this.selectedFileName)), delegate
		{
			this.fileButtonMap[this.selectedFileName].GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.fileButtonMap[this.selectedFileName].isInteractable = true;
			this.saveButtonPool.ClearElement(this.fileButtonMap[this.selectedFileName]);
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
			}, null, null, null, null, null);
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
	private KButton saveButtonPrefab;

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
	private KButton moreInfoButton;

	[SerializeField]
	private ColorStyleSetting validSaveFileStyle;

	[SerializeField]
	private ColorStyleSetting invalidSaveFileStyle;

	public Action<string> onClick;

	public bool requireConfirmation = true;

	private UIPool<KButton> saveButtonPool;

	private Dictionary<string, KButton> fileButtonMap = new Dictionary<string, KButton>();

	private ConfirmDialogScreen confirmScreen;

	private string selectedFileName;
}
