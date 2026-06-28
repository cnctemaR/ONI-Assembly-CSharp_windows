using System;
using System.Collections.Generic;
using System.IO;
using STRINGS;
using UnityEngine;

public class SaveScreen : KScreen
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
		LocText componentInChildren = kbutton.GetComponentInChildren<LocText>();
		global::System.DateTime lastWriteTime = File.GetLastWriteTime(filename);
		componentInChildren.text = string.Format("{0}\n{1:H:mm:ss}\n" + Localization.GetFileDateFormat(1), Path.GetFileNameWithoutExtension(filename), lastWriteTime);
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
			}, this.transform.parent);
		}
		else
		{
			this.DoSave(filename);
		}
	}

	private void DoSave(string filename)
	{
		SaveLoader.Instance.Save(filename, false, true);
		this.Deactivate();
	}

	public void OnClickNewSave()
	{
		FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, this.transform.parent.gameObject);
		fileNameDialog.onConfirm = delegate(string filename)
		{
			filename = Path.Combine(SaveLoader.GetSavePrefix(), filename);
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
