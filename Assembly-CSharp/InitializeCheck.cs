using System;
using System.IO;
using ProcGenGame;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class InitializeCheck : MonoBehaviour
{
	public static InitializeCheck.SavePathIssue savePathState { get; private set; }

	private void Awake()
	{
		this.CheckForSavePathIssue();
		if (InitializeCheck.savePathState == InitializeCheck.SavePathIssue.Ok)
		{
			AudioMixer.Create();
			App.LoadScene("frontend");
		}
		else
		{
			Canvas canvas = base.gameObject.AddComponent<Canvas>();
			canvas.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
			canvas.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500f);
			Camera camera = base.gameObject.AddComponent<Camera>();
			camera.orthographic = true;
			camera.orthographicSize = 200f;
			camera.backgroundColor = Color.black;
			camera.clearFlags = CameraClearFlags.Color;
			camera.nearClipPlane = 0f;
			global::Debug.Log("Cannot initialize filesystem. [" + InitializeCheck.savePathState.ToString() + "]", null);
			Localization.Initialize(true);
			this.ShowFileErrorDialogs();
		}
	}

	private GameObject CreateUIRoot()
	{
		return Util.KInstantiate(this.rootCanvasPrefab, null, "CanvasRoot");
	}

	private void ShowFileErrorDialogs()
	{
		string text = null;
		InitializeCheck.SavePathIssue savePathState = InitializeCheck.savePathState;
		if (savePathState != InitializeCheck.SavePathIssue.WriteTestFail)
		{
			if (savePathState != InitializeCheck.SavePathIssue.SpaceTestFail)
			{
				if (savePathState == InitializeCheck.SavePathIssue.WorldGenFilesFail)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FILES, WorldGen.WORLDGEN_SAVE_FILENAME + "\n" + WorldGen.SIM_SAVE_FILENAME);
				}
			}
			else
			{
				text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, SaveLoader.GetSavePrefix());
			}
		}
		else
		{
			text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, SaveLoader.GetSavePrefix());
		}
		if (text != null)
		{
			GameObject gameObject = this.CreateUIRoot();
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(this.confirmDialogScreen.gameObject, gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = this.sadDupe;
			confirmDialogScreen.PopupConfirmDialog(text, new global::System.Action(this.Quit), null, null, null, null, null, null);
		}
	}

	private void CheckForSavePathIssue()
	{
		string savePrefix = SaveLoader.GetSavePrefix();
		InitializeCheck.savePathState = InitializeCheck.SavePathIssue.Ok;
		try
		{
			SaveLoader.GetSavePrefixAndCreateFolder();
			using (FileStream fileStream = File.Open(savePrefix + InitializeCheck.testFile, FileMode.Create, FileAccess.Write))
			{
				new BinaryWriter(fileStream);
				fileStream.Close();
			}
		}
		catch
		{
			InitializeCheck.savePathState = InitializeCheck.SavePathIssue.WriteTestFail;
			goto IL_00FA;
		}
		using (FileStream fileStream2 = File.Open(savePrefix + InitializeCheck.testSave, FileMode.Create, FileAccess.Write))
		{
			try
			{
				fileStream2.SetLength(15000000L);
				new BinaryWriter(fileStream2);
				fileStream2.Close();
			}
			catch
			{
				fileStream2.Close();
				InitializeCheck.savePathState = InitializeCheck.SavePathIssue.SpaceTestFail;
				goto IL_00FA;
			}
		}
		try
		{
			using (File.Open(WorldGen.WORLDGEN_SAVE_FILENAME, FileMode.Append))
			{
			}
			using (File.Open(WorldGen.SIM_SAVE_FILENAME, FileMode.Append))
			{
			}
		}
		catch
		{
			InitializeCheck.savePathState = InitializeCheck.SavePathIssue.WorldGenFilesFail;
		}
		try
		{
			IL_00FA:
			if (File.Exists(savePrefix + InitializeCheck.testFile))
			{
				File.Delete(savePrefix + InitializeCheck.testFile);
			}
			if (File.Exists(savePrefix + InitializeCheck.testSave))
			{
				File.Delete(savePrefix + InitializeCheck.testSave);
			}
		}
		catch
		{
		}
	}

	private void Quit()
	{
		global::Debug.Log("Quitting...", null);
		Application.Quit();
	}

	private static readonly string testFile = "testfile";

	private static readonly string testSave = "testsavefile";

	public Canvas rootCanvasPrefab;

	public ConfirmDialogScreen confirmDialogScreen;

	public Sprite sadDupe;

	public enum SavePathIssue
	{
		Ok,
		WriteTestFail,
		SpaceTestFail,
		WorldGenFilesFail
	}
}
