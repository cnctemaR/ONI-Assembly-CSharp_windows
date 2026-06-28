using System;
using System.Globalization;
using System.IO;
using System.Threading;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LaunchInitializer : MonoBehaviour
{
	public static LaunchInitializer.SavePathIssue savePathState { get; private set; }

	private void Awake()
	{
		GraphicsOptionsScreen.SetResolutionFromPrefs();
		Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		global::Debug.Log("Development Build: TB-" + 247630U.ToString(), null);
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.CheckForSavePathIssue();
		if (LaunchInitializer.savePathState == LaunchInitializer.SavePathIssue.Ok)
		{
			KPlayerPrefs.instance.Load();
			for (int i = 0; i < this.SpawnPrefabs.Length; i++)
			{
				if (this.SpawnPrefabs[i] != null)
				{
					Util.KInstantiate(this.SpawnPrefabs[i], base.gameObject, null);
				}
			}
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
			global::Debug.Log("Cannot intialise filesystem. [" + LaunchInitializer.savePathState.ToString() + "]", null);
			Localization.Initialize(true);
			for (int j = 0; j < this.ErrorSpawnPrefabs.Length; j++)
			{
				if (this.ErrorSpawnPrefabs[j] != null)
				{
					Util.KInstantiate(this.ErrorSpawnPrefabs[j], base.gameObject, null);
				}
			}
			this.ShowFileErrorDialogs();
		}
	}

	private void Quit()
	{
		global::Debug.Log("Quitting...", null);
		Application.Quit();
	}

	private void ShowFileErrorDialogs()
	{
		if (LaunchInitializer.savePathState == LaunchInitializer.SavePathIssue.WriteTestFail)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(this.confirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.imageGO.GetComponent<Image>().sprite = this.sadDupe;
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, SaveLoader.GetSavePrefix()), new global::System.Action(this.Quit), null, null, null, null, null, null);
		}
		else if (LaunchInitializer.savePathState == LaunchInitializer.SavePathIssue.SpaceTestFail)
		{
			ConfirmDialogScreen confirmDialogScreen2 = Util.KInstantiateUI<ConfirmDialogScreen>(this.confirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen2.imageGO.GetComponent<Image>().sprite = this.sadDupe;
			confirmDialogScreen2.PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, SaveLoader.GetSavePrefix()), new global::System.Action(this.Quit), null, null, null, null, null, null);
		}
	}

	private void CheckForSavePathIssue()
	{
		bool flag = true;
		string savePrefix = SaveLoader.GetSavePrefix();
		LaunchInitializer.savePathState = LaunchInitializer.SavePathIssue.Ok;
		try
		{
			FileStream fileStream = File.Open(savePrefix + LaunchInitializer.testFile, FileMode.Create, FileAccess.Write);
			new BinaryWriter(fileStream);
			fileStream.Close();
			flag = false;
		}
		catch
		{
			flag = true;
			LaunchInitializer.savePathState = LaunchInitializer.SavePathIssue.WriteTestFail;
		}
		if (!flag)
		{
			FileStream fileStream2 = File.Open(savePrefix + LaunchInitializer.testSave, FileMode.Create, FileAccess.Write);
			try
			{
				fileStream2.SetLength(15000000L);
				new BinaryWriter(fileStream2);
				fileStream2.Close();
			}
			catch
			{
				fileStream2.Close();
				LaunchInitializer.savePathState = LaunchInitializer.SavePathIssue.SpaceTestFail;
			}
		}
		if (File.Exists(savePrefix + LaunchInitializer.testFile))
		{
			File.Delete(savePrefix + LaunchInitializer.testFile);
		}
		if (File.Exists(savePrefix + LaunchInitializer.testSave))
		{
			File.Delete(savePrefix + LaunchInitializer.testSave);
		}
	}

	public const string BUILD_PREFIX = "TB";

	private static readonly string testFile = "testfile";

	private static readonly string testSave = "testsavefile";

	public GameObject[] SpawnPrefabs;

	public GameObject[] ErrorSpawnPrefabs;

	public ConfirmDialogScreen confirmDialogScreen;

	public Sprite sadDupe;

	public enum SavePathIssue
	{
		Ok,
		WriteTestFail,
		SpaceTestFail
	}
}
