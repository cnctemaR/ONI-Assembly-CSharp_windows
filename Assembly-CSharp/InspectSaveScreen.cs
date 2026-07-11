using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class InspectSaveScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.closeButton.onClick += this.CloseScreen;
		this.deleteSaveBtn.onClick += this.DeleteSave;
	}

	private void CloseScreen()
	{
		LoadScreen.Instance.Show(true);
		base.Show(false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			this.buttonPool.ClearAll();
			this.buttonFileMap.Clear();
		}
	}

	public void SetTarget(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			global::Debug.LogError("The directory path provided is empty.");
			base.Show(false);
			return;
		}
		if (!Directory.Exists(path))
		{
			global::Debug.LogError("The directory provided does not exist.");
			base.Show(false);
			return;
		}
		if (this.buttonPool == null)
		{
			this.buttonPool = new UIPool<KButton>(this.backupBtnPrefab);
		}
		this.currentPath = path;
		List<string> list = (from filename in Directory.GetFiles(path)
			where Path.GetExtension(filename).ToLower() == ".sav"
			orderby File.GetLastWriteTime(filename) descending
			select filename).ToList<string>();
		string text = list[0];
		if (File.Exists(text))
		{
			this.mainSaveBtn.gameObject.SetActive(true);
			this.AddNewSave(this.mainSaveBtn, text);
		}
		else
		{
			this.mainSaveBtn.gameObject.SetActive(false);
		}
		if (list.Count > 1)
		{
			for (int i = 1; i < list.Count; i++)
			{
				this.AddNewSave(this.buttonPool.GetFreeElement(this.buttonGroup, true), list[i]);
			}
		}
		base.Show(true);
	}

	private void ConfirmDoAction(string message, global::System.Action action)
	{
		if (this.confirmScreen == null)
		{
			this.confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, false);
			this.confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			}, null, null, null, null, null, null);
			this.confirmScreen.GetComponent<LayoutElement>().ignoreLayout = true;
			this.confirmScreen.gameObject.SetActive(true);
		}
	}

	private void DeleteSave()
	{
		if (string.IsNullOrEmpty(this.currentPath))
		{
			global::Debug.LogError("The path provided is not valid and cannot be deleted.");
			return;
		}
		this.ConfirmDoAction(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, delegate
		{
			string[] files = Directory.GetFiles(this.currentPath);
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
			Directory.Delete(this.currentPath);
			this.CloseScreen();
		});
	}

	private void AddNewSave(KButton btn, string file)
	{
	}

	private void ButtonClicked(KButton btn)
	{
		LoadingOverlay.Load(delegate
		{
			this.Load(this.buttonFileMap[btn]);
		});
	}

	private void Load(string filename)
	{
		if (Game.Instance != null)
		{
			LoadScreen.ForceStopGame();
		}
		SaveLoader.SetActiveSaveFilePath(filename);
		App.LoadScene("backend");
		this.Deactivate();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.CloseScreen();
			return;
		}
		base.OnKeyDown(e);
	}

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton mainSaveBtn;

	[SerializeField]
	private KButton backupBtnPrefab;

	[SerializeField]
	private KButton deleteSaveBtn;

	[SerializeField]
	private GameObject buttonGroup;

	private UIPool<KButton> buttonPool;

	private Dictionary<KButton, string> buttonFileMap = new Dictionary<KButton, string>();

	private ConfirmDialogScreen confirmScreen;

	private string currentPath = "";
}
