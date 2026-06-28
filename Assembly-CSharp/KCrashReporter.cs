using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
	public static event Action<string> onCrashReported;

	private void OnEnable()
	{
		Application.logMessageReceived += this.HandleLog;
		if (File.Exists("ignorekcrashreporter.txt"))
		{
			KCrashReporter.ignoreAll = true;
		}
		if (Application.isEditor)
		{
			KCrashReporter.terminateOnError = false;
		}
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= this.HandleLog;
	}

	private void HandleLog(string msg, string stack_trace, LogType type)
	{
		if (KCrashReporter.ignoreAll)
		{
			return;
		}
		if (msg == "Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!")
		{
			return;
		}
		if (msg != null && msg.StartsWith("<RI.Hid>"))
		{
			return;
		}
		if (type == LogType.Exception)
		{
			RestartWarning.ShouldWarn = true;
		}
		if (this.errorDialog == null && (type == LogType.Exception || type == LogType.Error))
		{
			if (KCrashReporter.terminateOnError && ReportErrorDialog.hasCrash)
			{
				return;
			}
			if (SpeedControlScreen.Instance != null)
			{
				SpeedControlScreen.Instance.Pause(true);
			}
			string local_msg = msg;
			string local_stack_trace = stack_trace;
			GameObject gameObject = GameObject.Find(KCrashReporter.error_canvas_name);
			if (gameObject == null)
			{
				gameObject = new GameObject();
				gameObject.name = KCrashReporter.error_canvas_name;
				Canvas canvas = gameObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				gameObject.AddComponent<GraphicRaycaster>();
			}
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate(this.reportErrorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.transform.SetParent(gameObject.transform, false);
			this.errorDialog = gameObject2.GetComponentInChildren<ReportErrorDialog>();
			string text = null;
			global::System.Action action = null;
			if (Application.isEditor && KScreenManager.Instance != null)
			{
				text = "Report & Upload Save";
				action = delegate
				{
					LoadScreen loadScreen = Util.KInstantiateUI<LoadScreen>(this.loadScreenPrefab.gameObject, GameScreenManager.Instance.ssCameraCanvas.gameObject, false);
					loadScreen.onClick = delegate(string save_path)
					{
						string[] files = Directory.GetFiles(save_path);
						string text2 = string.Empty;
						foreach (string text3 in files)
						{
							if (Path.GetExtension(text3) == ".sav")
							{
								text2 = text3;
								break;
							}
						}
						string text4 = KCrashReporter.UploadSaveFile(text2, local_stack_trace);
						KCrashReporter.ReportError(local_msg, local_stack_trace, text4, this.confirmDialogPrefab, string.Empty);
						this.OnCloseErrorDialog();
						loadScreen.Deactivate();
					};
				};
			}
			bool flag = local_msg != null && local_msg.ToLower().Contains("simdll.dll");
			this.errorDialog.PopupConfirmDialog("ERROR OCCURRED!\nDo you want to report this error?", delegate
			{
				KCrashReporter.ReportError(local_msg, local_stack_trace, null, this.confirmDialogPrefab, this.errorDialog.UserMessage());
			}, delegate
			{
				this.OnQuitToDesktop();
			}, delegate
			{
				this.OnCloseErrorDialog();
			}, text, action, flag);
		}
	}

	private void OnCloseErrorDialog()
	{
		global::UnityEngine.Object.Destroy(this.errorDialog.gameObject);
		this.errorDialog = null;
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Unpause(true);
		}
	}

	private void OnQuitToDesktop()
	{
		Application.Quit();
	}

	private static string UploadSaveFile(string save_file, string stack_trace)
	{
		string text = null;
		if (Application.isEditor && save_file != null && File.Exists(save_file))
		{
			string fileName = Path.GetFileName(save_file);
			string text2 = Path.Combine("\\\\files\\Klei - File Database\\Trough\\OxygenNotIncludedCrashes", ((uint)Hash.SDBMLower(stack_trace)).ToString("X"));
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.Combine(text2, fileName);
			File.Copy(save_file, text3);
			text = text3;
		}
		return text;
	}

	private static string GetUserID()
	{
		if (Application.isEditor)
		{
			return Environment.UserName;
		}
		if (SteamManager.Initialized)
		{
			return "SteamID_" + SteamFriends.GetPersonaName() + "_" + SteamUser.GetSteamID().ToString();
		}
		return "NO_STEAM";
	}

	private static string GetLogContents()
	{
		string text;
		if (Application.isEditor)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/Editor/Editor.log");
		}
		else
		{
			text = Path.Combine(Application.dataPath, "output_log.txt");
		}
		if (File.Exists(text))
		{
			using (FileStream fileStream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					return streamReader.ReadToEnd();
				}
			}
		}
		return string.Empty;
	}

	private static void ReportError(string msg, string stack_trace, string save_file_link, ConfirmDialogScreen confirm_prefab, string userMessage = "")
	{
		string text3;
		using (WebClient webClient = new WebClient())
		{
			webClient.Encoding = Encoding.UTF8;
			if (string.IsNullOrEmpty(msg))
			{
				msg = "No message";
			}
			if (string.IsNullOrEmpty(save_file_link))
			{
				save_file_link = "No save file uploaded";
			}
			msg = string.Format("{0}\n\nSave File: {1}", msg, save_file_link);
			if (string.IsNullOrEmpty(stack_trace))
			{
				stack_trace = string.Format("No stack trace.\n\n{0}", msg);
			}
			string text = JsonConvert.SerializeObject(new KCrashReporter.Error
			{
				user = KCrashReporter.GetUserID(),
				callstack = stack_trace,
				fullstack = "USER_MESSAGE:\n" + userMessage + "\n\nUNITY_OUTPUT:\n" + msg,
				build = 208689,
				log = KCrashReporter.GetLogContents()
			});
			string text2 = webClient.UploadString("http://crashes.klei.ca/submitCrash", text);
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, null);
			confirmDialogScreen.PopupConfirmDialog("Reported Error", null, null, null, null);
			text3 = text2;
		}
		if (KCrashReporter.onCrashReported != null)
		{
			KCrashReporter.onCrashReported(text3);
		}
	}

	public static void ReportBug(string msg, string save_file)
	{
		string text = "Bug Report From: " + KCrashReporter.GetUserID() + " at " + global::System.DateTime.Now.ToString();
		string text2 = KCrashReporter.UploadSaveFile(save_file, text);
		KCrashReporter.ReportError(string.Empty, text, text2, ScreenPrefabs.Instance.ConfirmDialogScreen, string.Empty);
	}

	public static bool ignoreAll;

	public static string error_canvas_name = "ErrorCanvas";

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private GameObject reportErrorPrefab;

	[SerializeField]
	private ConfirmDialogScreen confirmDialogPrefab;

	private ReportErrorDialog errorDialog;

	public static bool terminateOnError = true;

	private class Error
	{
		public string game = "simgame";

		public int build = -1;

		public string platform = Environment.OSVersion.ToString();

		public string user = "unknown";

		public string callstack = string.Empty;

		public string fullstack = string.Empty;

		public string log = string.Empty;

		public bool is_server;

		public bool is_dedicated;
	}
}
