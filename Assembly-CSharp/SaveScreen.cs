using System;
using System.Collections.Generic;
using System.IO;
using STRINGS;
using UnityEngine;

public class SaveScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.oldSaveButtonPrefab.gameObject.SetActive(false);
		this.newSaveButton.onClick += this.OnClickNewSave;
		this.closeButton.onClick += this.Deactivate;
	}

	protected override void OnCmpEnable()
	{
		List<string> allFiles = SaveLoader.GetAllFiles();
		foreach (string text in allFiles)
		{
			this.AddExistingSaveFile(text);
		}
		SpeedControlScreen.Instance.Pause(true);
	}

	protected override void OnDeactivate()
	{
		SpeedControlScreen.Instance.Unpause(true);
		base.OnDeactivate();
	}

	private void AddExistingSaveFile(string filename)
	{
		KButton kbutton = Util.KInstantiateUI<KButton>(this.oldSaveButtonPrefab.gameObject, this.oldSavesRoot.gameObject, true);
		HierarchyReferences component = kbutton.GetComponent<HierarchyReferences>();
		LocText component2 = component.GetReference<RectTransform>("Title").GetComponent<LocText>();
		LocText component3 = component.GetReference<RectTransform>("Date").GetComponent<LocText>();
		global::System.DateTime lastWriteTime = File.GetLastWriteTime(filename);
		component2.text = string.Format("{0}", Path.GetFileNameWithoutExtension(filename));
		component3.text = string.Format("{0:H:mm:ss}" + Localization.GetFileDateFormat(0), lastWriteTime);
		kbutton.onClick += delegate
		{
			this.Save(filename);
		};
	}

	public static string GetValidSaveFilename(string filename)
	{
		string text = ".sav";
		string text2 = Path.GetExtension(filename).ToLower();
		if (text2 != text)
		{
			filename += text;
		}
		return filename;
	}

	public void Save(string filename)
	{
		filename = SaveScreen.GetValidSaveFilename(filename);
		if (File.Exists(filename))
		{
			ScreenPrefabs.Instance.ConfirmDoAction(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, Path.GetFileNameWithoutExtension(filename)), delegate
			{
				this.DoSave(filename);
			}, base.transform.parent);
		}
		else
		{
			this.DoSave(filename);
		}
	}

	private void DoSave(string filename)
	{
		ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
		try
		{
			SaveLoader.Instance.Save(filename, false, true);
			this.Deactivate();
		}
		catch (IOException ex)
		{
			IOException ex2 = ex;
			IOException e = ex2;
			SaveScreen $this = this;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.IO_ERROR, e.ToString()), delegate
			{
				$this.Deactivate();
			}, null, UI.FRONTEND.SAVESCREEN.REPORT_BUG, delegate
			{
				KCrashReporter.ReportError(e.Message, e.StackTrace.ToString(), null, null, string.Empty);
			}, null, null, null, null);
		}
	}

	public void OnClickNewSave()
	{
		FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, base.transform.parent.gameObject);
		fileNameDialog.onConfirm = delegate(string filename)
		{
			filename = Path.Combine(SaveLoader.GetSavePrefixAndCreateFolder(), filename);
			this.Save(filename);
		};
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			this.Deactivate();
		}
		e.Consumed = true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		e.Consumed = true;
	}

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton newSaveButton;

	[SerializeField]
	private KButton oldSaveButtonPrefab;

	[SerializeField]
	private Transform oldSavesRoot;
}
