using System;
using System.Collections.Generic;
using System.IO;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : KModalScreen
{
	public static event global::System.Action OnFileDeleted;

	protected override void OnPrefabInit()
	{
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
		LoadScreen.Instance = this;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
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
		if (allFiles.Count == 0)
		{
			base.Show(false);
			return;
		}
		for (int i = 0; i < allFiles.Count; i++)
		{
			this.AddExistingSaveFile(allFiles[i]);
		}
		this.SetSelectedGame(allFiles[0]);
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
		LocText componentInChildren = freeElement.GetComponentInChildren<LocText>();
		global::System.DateTime lastWriteTime = File.GetLastWriteTime(filename);
		componentInChildren.text = string.Format("{0}\n{1:H:mm:ss}\n{1:dd / MMM / yyyy}", Path.GetFileNameWithoutExtension(filename), lastWriteTime);
		freeElement.ClearOnClick();
		freeElement.onClick += delegate
		{
			this.onClick(filename);
		};
		freeElement.onDoubleClick += delegate
		{
			this.onClick(filename);
			this.Load();
		};
		freeElement.transform.SetAsLastSibling();
		this.fileButtonMap.Add(filename, freeElement);
	}

	public static void ForceStopGame()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		UpdateManager.instance.enabled = false;
		Game.Instance.SetIsLoading();
		Grid.CellCount = 0;
		Sim.Shutdown();
	}

	private void SetSelectedGame(string filename)
	{
		if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
		{
			Debug.LogError("The filename provided is not valid.");
			return;
		}
		KButton kbutton = ((this.selectedFileName == null) ? null : this.fileButtonMap[this.selectedFileName]);
		if (kbutton != null)
		{
			kbutton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			kbutton.Select();
		}
		this.selectedFileName = filename;
		kbutton = this.fileButtonMap[this.selectedFileName];
		kbutton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		kbutton.Deselect();
		try
		{
			SaveGame.Header header;
			SaveGame.HeaderData headerData = SaveLoader.LoadHeader(filename, out header);
			string text = UI.FRONTEND.LOADSCREEN.SAVEDETAILS;
			string text2 = string.Format("{0:H:mm:ss}\n{0:dd / MMM / yyyy}", File.GetLastWriteTime(filename));
			string text3 = Path.GetFileName(filename);
			if (headerData.isAutoSave)
			{
				text3 += UI.FRONTEND.LOADSCREEN.AUTOSAVEWARNING;
			}
			string text4 = string.Format(text, new object[]
			{
				text3,
				text2,
				headerData.baseName,
				headerData.numberOfDuplicants.ToString(),
				headerData.numberOfCycles.ToString()
			});
			this.saveDetails.text = text4;
			if (208689U < header.buildVersion)
			{
				this.saveDetails.text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_TOO_NEW, filename, header.buildVersion, 208689U);
				if (this.loadButton.interactable)
				{
					this.loadButton.interactable = false;
					this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
				}
			}
			else if (!this.loadButton.interactable)
			{
				this.loadButton.interactable = true;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex);
			this.saveDetails.text = string.Format(UI.FRONTEND.LOADSCREEN.CORRUPTEDSAVE, filename);
			if (this.loadButton.interactable)
			{
				this.loadButton.interactable = false;
				this.loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
		}
	}

	private void Load()
	{
		if (Game.Instance != null)
		{
			LoadScreen.ForceStopGame();
		}
		MainMenu mainMenu = global::UnityEngine.Object.FindObjectOfType<MainMenu>();
		if (mainMenu != null)
		{
			mainMenu.ClearFileDeletedCallback();
		}
		SaveLoader.SetActiveSaveFilePath(this.selectedFileName);
		App.LoadScene("backend");
		this.Deactivate();
	}

	private void Delete()
	{
		if (string.IsNullOrEmpty(this.selectedFileName))
		{
			Debug.LogError("The path provided is not valid and cannot be deleted.");
			return;
		}
		this.ConfirmDoAction(string.Format(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, Path.GetFileName(this.selectedFileName)), delegate
		{
			this.fileButtonMap[this.selectedFileName].GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.fileButtonMap[this.selectedFileName].interactable = true;
			this.saveButtonPool.ClearElement(this.fileButtonMap[this.selectedFileName]);
			File.Delete(this.selectedFileName);
			this.selectedFileName = null;
			this.RefreshFiles();
			if (LoadScreen.OnFileDeleted != null)
			{
				LoadScreen.OnFileDeleted();
			}
		});
	}

	private void ConfirmDoAction(string message, global::System.Action action)
	{
		if (this.confirmScreen == null)
		{
			this.confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, false);
			this.confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			}, null, null);
			this.confirmScreen.GetComponent<LayoutElement>().ignoreLayout = true;
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

	public Action<string> onClick;

	public bool requireConfirmation = true;

	public static LoadScreen Instance;

	private UIPool<KButton> saveButtonPool;

	private Dictionary<string, KButton> fileButtonMap = new Dictionary<string, KButton>();

	private ConfirmDialogScreen confirmScreen;

	private string selectedFileName;
}
