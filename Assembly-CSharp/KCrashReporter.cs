using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
			bool flag = local_msg != null && local_msg.ToLower().Contains("simdll.dll");
			this.errorDialog.PopupConfirmDialog("ERROR OCCURRED!\nDo you want to report this error?", delegate
			{
				string text = null;
				if (KCrashReporter.MOST_RECENT_SAVEFILE != null)
				{
					text = KCrashReporter.UploadSaveFile(KCrashReporter.MOST_RECENT_SAVEFILE, local_stack_trace);
				}
				KCrashReporter.ReportError(local_msg, local_stack_trace, text, this.confirmDialogPrefab, this.errorDialog.UserMessage());
			}, delegate
			{
				this.OnQuitToDesktop();
			}, delegate
			{
				this.OnCloseErrorDialog();
			}, flag);
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
		global::Debug.Log(string.Format("Save_file: {0}", save_file), null);
		if (save_file != null && File.Exists(save_file))
		{
			using (WebClient webClient = new WebClient())
			{
				byte[] array = File.ReadAllBytes(save_file);
				string text = "----" + global::System.DateTime.Now.Ticks.ToString("x");
				webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + text);
				string @string = webClient.Encoding.GetString(array);
				string text2 = string.Empty;
				string text3;
				using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
				{
					text3 = BitConverter.ToString(sha1CryptoServiceProvider.ComputeHash(array)).Replace("-", string.Empty);
				}
				text2 += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "hash", text3);
				text2 += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"save\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}", new object[] { text, save_file, "application/x-spss-sav", @string });
				text2 += string.Format("\r\n--{0}--\r\n", text);
				byte[] bytes = webClient.Encoding.GetBytes(text2);
				Uri uri = new Uri("http://crashes.klei.ca/submitSave");
				try
				{
					webClient.UploadData(uri, "POST", bytes);
					return text3;
				}
				catch (Exception ex)
				{
					global::Debug.Log(ex, null);
					return string.Empty;
				}
			}
		}
		return string.Empty;
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

	public static void ReportError(string msg, string stack_trace, string save_file_hash, ConfirmDialogScreen confirm_prefab, string userMessage = "")
	{
		if (KCrashReporter.debugWasUsed)
		{
			global::Debug.Log("Ignoring crash because debug was used.", null);
			return;
		}
		global::Debug.Log("Reporting error.", null);
		KCrashReporter.hasReportedError = true;
		string text6;
		using (WebClient webClient = new WebClient())
		{
			webClient.Encoding = Encoding.UTF8;
			if (string.IsNullOrEmpty(msg))
			{
				msg = "No message";
			}
			string text = save_file_hash;
			if (string.IsNullOrEmpty(save_file_hash))
			{
				text = "No save file uploaded";
			}
			msg = string.Format("{0}\n\nSave File: {1}", msg, text);
			if (string.IsNullOrEmpty(stack_trace))
			{
				stack_trace = string.Format("No stack trace.\n\n{0}", msg);
			}
			int num = stack_trace.IndexOf('\n');
			string text2 = stack_trace;
			if (num > 0)
			{
				text2 = stack_trace.Substring(0, num);
			}
			while (text2 == string.Empty || text2.StartsWith("UnityEngine.Debug:LogError(Object)") || text2.StartsWith("UnityEngine.Debug:LogError(Object, Object)") || text2.StartsWith("UnityEngine.Debug:Assert(Boolean, String)") || text2.StartsWith("Output:LogError(String)") || text2.StartsWith("Output:LogErrorWithObj(Object, String)") || text2.StartsWith("Output:LogErrorWithObj(Object, Object[])") || text2.StartsWith("DebugUtil:Assert(Boolean, String)") || text2.StartsWith("KCrashReporter.Assert(Boolean condition, System.String message)") || text2.StartsWith("No stack trace."))
			{
				int num2 = num + 1;
				bool flag = false;
				if (num2 < stack_trace.Length)
				{
					num = stack_trace.IndexOf('\n', num2);
					if (num < stack_trace.Length)
					{
						text2 = stack_trace.Substring(num2, num - num2);
						flag = true;
					}
				}
				if (!flag)
				{
					text2 = string.Empty;
					break;
				}
			}
			Match match = KCrashReporter.failedToLoadModuleRegEx.Match(text2);
			if (match.Success)
			{
				string text3 = match.Groups[1].ToString();
				string text4 = match.Groups[2].ToString();
				string fileName = Path.GetFileName(text3);
				text2 = string.Concat(new string[] { "Failed to load '", fileName, "' with error '", text4, "'." });
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
			error.build = 218235;
			error.log = KCrashReporter.GetLogContents();
			error.summaryline = text2;
			error.user_message = userMessage;
			if (!string.IsNullOrEmpty(save_file_hash))
			{
				error.save_hash = save_file_hash;
			}
			if (DistributionPlatform.Initialized)
			{
				error.steam64_verified = DistributionPlatform.Inst.LocalUser.Id.ToInt64();
			}
			string text5 = JsonConvert.SerializeObject(error);
			string empty = string.Empty;
			Uri uri = new Uri("http://crashes.klei.ca/submitCrash");
			global::Debug.Log("Submitting crash:", null);
			try
			{
				webClient.UploadStringAsync(uri, text5);
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
			text6 = empty;
		}
		if (KCrashReporter.onCrashReported != null)
		{
			KCrashReporter.onCrashReported(text6);
		}
	}

	public static void ReportBug(string msg, string save_file)
	{
		string text = "Bug Report From: " + KCrashReporter.GetUserID() + " at " + global::System.DateTime.Now.ToString();
		string text2 = KCrashReporter.UploadSaveFile(save_file, text);
		KCrashReporter.ReportError(msg, text, text2, ScreenPrefabs.Instance.ConfirmDialogScreen, string.Empty);
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

	public const string CRASH_REPORTER_SERVER = "http://crashes.klei.ca";

	public static string MOST_RECENT_SAVEFILE = null;

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static string error_canvas_name = "ErrorCanvas";

	private static bool disableDeduping = false;

	private static bool hasReportedError;

	private static readonly Regex failedToLoadModuleRegEx = new Regex("^Failed to load '(.*?)' with error '(.*?)'.$");

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

		public string save_hash = string.Empty;
	}
}
