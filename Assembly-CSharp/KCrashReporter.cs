using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
	public static event Action<string> onCrashReported;

	private void OnEnable()
	{
		Application.logMessageReceived += this.HandleLog;
		KCrashReporter.ignoreAll = true;
		string dataPath = Application.dataPath;
		string text = Path.Combine(dataPath, "hashes.json");
		if (File.Exists(text))
		{
			StringBuilder stringBuilder = new StringBuilder();
			MD5 md = MD5.Create();
			string text2 = File.ReadAllText(text);
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text2);
			if (dictionary.Count > 0)
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					string key = keyValuePair.Key;
					string value = keyValuePair.Value;
					stringBuilder.Length = 0;
					string text3 = Path.Combine(dataPath, key);
					using (FileStream fileStream = new FileStream(text3, FileMode.Open, FileAccess.Read))
					{
						byte[] array = md.ComputeHash(fileStream);
						foreach (byte b in array)
						{
							stringBuilder.AppendFormat("{0:x2}", b);
						}
						string text4 = stringBuilder.ToString();
						if (text4 != value)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					KCrashReporter.ignoreAll = false;
				}
			}
			else
			{
				KCrashReporter.ignoreAll = false;
			}
		}
		else
		{
			KCrashReporter.ignoreAll = false;
		}
		if (KCrashReporter.ignoreAll)
		{
			global::Debug.Log("Ignoring crash due to mismatched hashes.json entries.", null);
		}
		if (File.Exists("ignorekcrashreporter.txt"))
		{
			KCrashReporter.ignoreAll = true;
			global::Debug.Log("Ignoring crash due to ignorekcrashreporter.txt", null);
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
		if (Array.IndexOf<string>(KCrashReporter.IgnoreStrings, msg) != -1)
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
		if (DistributionPlatform.Initialized)
		{
			return string.Concat(new object[]
			{
				DistributionPlatform.Inst.Name,
				"ID_",
				DistributionPlatform.Inst.LocalUser.Name,
				"_",
				DistributionPlatform.Inst.LocalUser.Id
			});
		}
		return "NO_PLATFORM";
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

	public static void ReportError(string msg, string stack_trace, string save_file_link, ConfirmDialogScreen confirm_prefab, string userMessage = "")
	{
		if (KCrashReporter.debugWasUsed)
		{
			global::Debug.Log("Ignoring crash because debug was used.", null);
			return;
		}
		global::Debug.Log("Reporting error.", null);
		KCrashReporter.hasReportedError = true;
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
			int num = stack_trace.IndexOf('\n');
			string text = stack_trace;
			if (num > 0)
			{
				text = stack_trace.Substring(0, num);
			}
			while (text == string.Empty || text.StartsWith("UnityEngine.Debug:LogError(Object)") || text.StartsWith("UnityEngine.Debug:LogError(Object, Object)") || text.StartsWith("UnityEngine.Debug:Assert(Boolean, String)") || text.StartsWith("Output:LogError(String)") || text.StartsWith("Output:LogErrorWithObj(Object, String)") || text.StartsWith("Output:LogErrorWithObj(Object, Object[])") || text.StartsWith("DebugUtil:Assert(Boolean, String)") || text.StartsWith("KCrashReporter.Assert(Boolean condition, System.String message)") || text.StartsWith("No stack trace."))
			{
				int num2 = num + 1;
				bool flag = false;
				if (num2 < stack_trace.Length)
				{
					num = stack_trace.IndexOf('\n', num2);
					if (num < stack_trace.Length)
					{
						text = stack_trace.Substring(num2, num - num2);
						flag = true;
					}
				}
				if (!flag)
				{
					text = string.Empty;
					break;
				}
			}
			if (userMessage == UI.CRASHSCREEN.BODY.text)
			{
				userMessage = string.Empty;
			}
			KCrashReporter.Error error = new KCrashReporter.Error();
			error.user = KCrashReporter.GetUserID();
			error.callstack = stack_trace;
			if (KCrashReporter.disableDeduping)
			{
				error.callstack = error.callstack + "\n" + Guid.NewGuid().ToString();
			}
			error.fullstack = "UNITY_OUTPUT:\n" + msg;
			error.build = 217844;
			error.log = KCrashReporter.GetLogContents();
			error.summaryline = text;
			error.user_message = userMessage;
			if (DistributionPlatform.Initialized)
			{
				error.steam64_verified = DistributionPlatform.Inst.LocalUser.Id.ToInt64();
			}
			string text2 = JsonConvert.SerializeObject(error);
			string empty = string.Empty;
			Uri uri = new Uri("http://crashes.klei.ca/submitCrash");
			global::Debug.Log("Submitting crash:", null);
			try
			{
				webClient.UploadStringAsync(uri, text2);
			}
			catch (Exception ex)
			{
				global::Debug.Log(ex, null);
			}
			if (confirm_prefab != null)
			{
				ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, null);
				confirmDialogScreen.PopupConfirmDialog("Reported Error", null, null, null, null);
			}
			text3 = empty;
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

	public static void Assert(bool condition, string message)
	{
		if (!condition && !KCrashReporter.hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(0, true);
			KCrashReporter.ReportError(message, stackTrace.ToString(), null, null, string.Empty);
		}
	}

	public static void Assert(bool condition)
	{
		if (!condition && !KCrashReporter.hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(0, true);
			KCrashReporter.ReportError("Assertion failed", stackTrace.ToString(), null, null, string.Empty);
		}
	}

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static string error_canvas_name = "ErrorCanvas";

	private static bool disableDeduping = false;

	private static bool hasReportedError;

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private GameObject reportErrorPrefab;

	[SerializeField]
	private ConfirmDialogScreen confirmDialogPrefab;

	private ReportErrorDialog errorDialog;

	public static bool terminateOnError = true;

	private static readonly string[] IgnoreStrings = new string[] { "Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!", "The profiler has run out of samples for this frame. This frame will be skipped. Increase the sample limit using Profiler.maxNumberOfSamplesPerFrame" };

	private class Error
	{
		public string game = "simgame";

		public int build = -1;

		public string platform = Environment.OSVersion.ToString();

		public string user = "unknown";

		public ulong steam64_verified;

		public string callstack = string.Empty;

		public string fullstack = string.Empty;

		public string log = string.Empty;

		public string summaryline = string.Empty;

		public string user_message = string.Empty;

		public bool is_server;

		public bool is_dedicated;
	}
}
