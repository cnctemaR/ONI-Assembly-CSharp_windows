using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Klei;
using Newtonsoft.Json;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
	public static event Action<string> onCrashReported;

	public static bool hasReportedError { get; private set; }

	private void OnEnable()
	{
		KCrashReporter.dataRoot = Application.dataPath;
		Application.logMessageReceived += this.HandleLog;
		KCrashReporter.ignoreAll = true;
		string text = Path.Combine(KCrashReporter.dataRoot, "hashes.json");
		if (File.Exists(text))
		{
			StringBuilder stringBuilder = new StringBuilder();
			MD5 md = MD5.Create();
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(text));
			if (dictionary.Count > 0)
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					string key = keyValuePair.Key;
					string value = keyValuePair.Value;
					stringBuilder.Length = 0;
					using (FileStream fileStream = new FileStream(Path.Combine(KCrashReporter.dataRoot, key), FileMode.Open, FileAccess.Read))
					{
						foreach (byte b in md.ComputeHash(fileStream))
						{
							stringBuilder.AppendFormat("{0:x2}", b);
						}
						if (stringBuilder.ToString() != value)
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
			global::Debug.Log("Ignoring crash due to mismatched hashes.json entries.");
		}
		if (File.Exists("ignorekcrashreporter.txt"))
		{
			KCrashReporter.ignoreAll = true;
			global::Debug.Log("Ignoring crash due to ignorekcrashreporter.txt");
		}
		if (Application.isEditor && !GenericGameSettings.instance.enableEditorCrashReporting)
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
		if ((KCrashReporter.logCount += 1U) == 10000000U)
		{
			DebugUtil.DevLogError("Turning off logging to avoid increasing the file to an unreasonable size, please review the logs as they probably contain spam");
			global::Debug.DisableLogging();
		}
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
		if (msg != null && msg.StartsWith("Failed to load cursor"))
		{
			return;
		}
		if (msg != null && msg.StartsWith("Failed to save a temporary cursor"))
		{
			return;
		}
		if (type == LogType.Exception)
		{
			RestartWarning.ShouldWarn = true;
		}
		if (this.errorScreen == null && (type == LogType.Exception || type == LogType.Error))
		{
			if (KCrashReporter.terminateOnError && KCrashReporter.hasCrash)
			{
				return;
			}
			if (SpeedControlScreen.Instance != null)
			{
				SpeedControlScreen.Instance.Pause(true, true);
			}
			string text = stack_trace;
			if (string.IsNullOrEmpty(text))
			{
				text = new StackTrace(5, true).ToString();
			}
			if (App.isLoading)
			{
				if (!SceneInitializerLoader.deferred_error.IsValid)
				{
					SceneInitializerLoader.deferred_error = new SceneInitializerLoader.DeferredError
					{
						msg = msg,
						stack_trace = text
					};
					return;
				}
			}
			else
			{
				this.ShowDialog(msg, text);
			}
		}
	}

	public bool ShowDialog(string error, string stack_trace)
	{
		if (this.errorScreen != null)
		{
			return false;
		}
		GameObject gameObject = GameObject.Find(KCrashReporter.error_canvas_name);
		if (gameObject == null)
		{
			gameObject = new GameObject();
			gameObject.name = KCrashReporter.error_canvas_name;
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
			canvas.sortingOrder = 32767;
			gameObject.AddComponent<GraphicRaycaster>();
		}
		this.errorScreen = global::UnityEngine.Object.Instantiate<GameObject>(this.reportErrorPrefab, Vector3.zero, Quaternion.identity);
		this.errorScreen.transform.SetParent(gameObject.transform, false);
		ReportErrorDialog errorDialog = this.errorScreen.GetComponentInChildren<ReportErrorDialog>();
		string text = error + "\n\n" + stack_trace;
		KCrashReporter.hasCrash = true;
		if (Global.Instance != null && Global.Instance.modManager != null && Global.Instance.modManager.HasCrashableMods())
		{
			Exception ex = DebugUtil.RetrieveLastExceptionLogged();
			StackTrace stackTrace = ((ex != null) ? new StackTrace(ex) : new StackTrace(5, true));
			Global.Instance.modManager.SearchForModsInStackTrace(stackTrace);
			Global.Instance.modManager.SearchForModsInStackTrace(stack_trace);
			errorDialog.PopupDisableModsDialog(text, new global::System.Action(this.OnQuitToDesktop), (Global.Instance.modManager.IsInDevMode() || !KCrashReporter.terminateOnError) ? new global::System.Action(this.OnCloseErrorDialog) : null);
		}
		else
		{
			errorDialog.PopupSubmitErrorDialog(text, delegate
			{
				string text2 = null;
				if (KCrashReporter.MOST_RECENT_SAVEFILE != null)
				{
					text2 = KCrashReporter.UploadSaveFile(KCrashReporter.MOST_RECENT_SAVEFILE, stack_trace, null);
				}
				KCrashReporter.ReportError(error, stack_trace, text2, this.confirmDialogPrefab, this.errorScreen, errorDialog.UserMessage());
			}, new global::System.Action(this.OnQuitToDesktop), KCrashReporter.terminateOnError ? null : new global::System.Action(this.OnCloseErrorDialog));
		}
		return true;
	}

	private void OnCloseErrorDialog()
	{
		global::UnityEngine.Object.Destroy(this.errorScreen);
		this.errorScreen = null;
		KCrashReporter.hasCrash = false;
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Unpause(true);
		}
	}

	private void OnQuitToDesktop()
	{
		App.Quit();
	}

	private static string UploadSaveFile(string save_file, string stack_trace, Dictionary<string, string> metadata = null)
	{
		global::Debug.Log(string.Format("Save_file: {0}", save_file));
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			return "";
		}
		if (save_file != null && File.Exists(save_file))
		{
			using (WebClient webClient = new WebClient())
			{
				Encoding utf = Encoding.UTF8;
				webClient.Encoding = utf;
				byte[] array = File.ReadAllBytes(save_file);
				string text = "----" + global::System.DateTime.Now.Ticks.ToString("x");
				webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + text);
				string text2 = "";
				string text3;
				using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
				{
					text3 = BitConverter.ToString(sha1CryptoServiceProvider.ComputeHash(array)).Replace("-", "");
				}
				text2 += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "hash", text3);
				if (metadata != null)
				{
					string text4 = JsonConvert.SerializeObject(metadata);
					text2 += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "metadata", text4);
				}
				text2 += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"save\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", new object[] { text, save_file, "application/x-spss-sav" });
				byte[] bytes = utf.GetBytes(text2);
				string text5 = string.Format("\r\n--{0}--\r\n", text);
				byte[] bytes2 = utf.GetBytes(text5);
				byte[] array2 = new byte[bytes.Length + array.Length + bytes2.Length];
				Buffer.BlockCopy(bytes, 0, array2, 0, bytes.Length);
				Buffer.BlockCopy(array, 0, array2, bytes.Length, array.Length);
				Buffer.BlockCopy(bytes2, 0, array2, bytes.Length + array.Length, bytes2.Length);
				Uri uri = new Uri("http://crashes.klei.ca/submitSave");
				try
				{
					webClient.UploadData(uri, "POST", array2);
					return text3;
				}
				catch (Exception ex)
				{
					global::Debug.Log(ex);
					return "";
				}
			}
		}
		return "";
	}

	private static string GetUserID()
	{
		if (DistributionPlatform.Initialized)
		{
			string[] array = new string[5];
			array[0] = DistributionPlatform.Inst.Name;
			array[1] = "ID_";
			array[2] = DistributionPlatform.Inst.LocalUser.Name;
			array[3] = "_";
			int num = 4;
			DistributionPlatform.UserId id = DistributionPlatform.Inst.LocalUser.Id;
			array[num] = ((id != null) ? id.ToString() : null);
			return string.Concat(array);
		}
		return "LocalUser";
	}

	private static string GetLogContents()
	{
		string text = Util.LogFilePath();
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
		return "";
	}

	public static void ReportErrorDevNotification(string notification_name, string stack_trace, string details = "")
	{
		if (KCrashReporter.previouslyReportedDevNotifications == null)
		{
			KCrashReporter.previouslyReportedDevNotifications = new HashSet<int>();
		}
		details = "DevNotification: " + notification_name + " - " + details;
		global::Debug.Log(details);
		int hashValue = new HashedString(notification_name).HashValue;
		bool hasReportedError = KCrashReporter.hasReportedError;
		if (!KCrashReporter.previouslyReportedDevNotifications.Contains(hashValue))
		{
			KCrashReporter.previouslyReportedDevNotifications.Add(hashValue);
			KCrashReporter.ReportError("DevNotification: " + notification_name, stack_trace, null, null, null, details);
		}
		KCrashReporter.hasReportedError = hasReportedError;
	}

	public static void ReportError(string msg, string stack_trace, string save_file_hash, ConfirmDialogScreen confirm_prefab, GameObject confirm_parent, string userMessage = "")
	{
		if (KCrashReporter.ignoreAll)
		{
			return;
		}
		global::Debug.Log("Reporting error.\n");
		if (msg != null)
		{
			global::Debug.Log(msg);
		}
		if (stack_trace != null)
		{
			global::Debug.Log(stack_trace);
		}
		KCrashReporter.hasReportedError = true;
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			return;
		}
		string text7;
		using (WebClient webClient = new WebClient())
		{
			webClient.Encoding = Encoding.UTF8;
			if (string.IsNullOrEmpty(msg))
			{
				msg = "No message";
			}
			Match match = KCrashReporter.failedToLoadModuleRegEx.Match(msg);
			if (match.Success)
			{
				string text = match.Groups[1].ToString();
				string text2 = match.Groups[2].ToString();
				string fileName = Path.GetFileName(text);
				msg = string.Concat(new string[] { "Failed to load '", fileName, "' with error '", text2, "'." });
			}
			if (string.IsNullOrEmpty(stack_trace))
			{
				string buildText = BuildWatermark.GetBuildText();
				stack_trace = string.Format("No stack trace {0}\n\n{1}", buildText, msg);
			}
			List<string> list = new List<string>();
			if (KCrashReporter.debugWasUsed)
			{
				list.Add("(Debug Used)");
			}
			if (KCrashReporter.haveActiveMods)
			{
				list.Add("(Mods Active)");
			}
			list.Add(msg);
			string[] array = new string[] { "Debug:LogError", "UnityEngine.Debug", "Output:LogError", "DebugUtil:Assert", "System.Array", "System.Collections", "KCrashReporter.Assert", "No stack trace." };
			foreach (string text3 in stack_trace.Split(new char[] { '\n' }))
			{
				if (list.Count >= 5)
				{
					break;
				}
				if (!string.IsNullOrEmpty(text3))
				{
					bool flag = false;
					foreach (string text4 in array)
					{
						if (text3.StartsWith(text4))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(text3);
					}
				}
			}
			if (userMessage == UI.CRASHSCREEN.BODY.text || userMessage.IsNullOrWhiteSpace())
			{
				userMessage = "";
			}
			else
			{
				userMessage = "[" + BuildWatermark.GetBuildText() + "]" + userMessage;
				if (!string.IsNullOrEmpty(save_file_hash))
				{
					userMessage = userMessage + "\nsave_hash: " + save_file_hash;
				}
			}
			KCrashReporter.Error error = new KCrashReporter.Error();
			error.user = KCrashReporter.GetUserID();
			error.callstack = stack_trace;
			if (KCrashReporter.disableDeduping)
			{
				error.callstack = error.callstack + "\n" + Guid.NewGuid().ToString();
			}
			error.fullstack = string.Format("{0}\n\n{1}", msg, stack_trace);
			error.build = 561558;
			error.log = KCrashReporter.GetLogContents();
			error.summaryline = string.Join("\n", list.ToArray());
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
			string text6 = "";
			Uri uri = new Uri("http://crashes.klei.ca/submitCrash");
			global::Debug.Log("Submitting crash:");
			try
			{
				webClient.UploadStringAsync(uri, text5);
			}
			catch (Exception ex)
			{
				global::Debug.Log(ex);
			}
			if (confirm_prefab != null && confirm_parent != null)
			{
				((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, confirm_parent)).PopupConfirmDialog(UI.CRASHSCREEN.REPORTEDERROR, null, null, null, null, null, null, null, null);
			}
			text7 = text6;
		}
		if (KCrashReporter.onCrashReported != null)
		{
			KCrashReporter.onCrashReported(text7);
		}
	}

	public static void ReportBug(string msg, string save_file, GameObject confirmParent)
	{
		string text = "Bug Report From: " + KCrashReporter.GetUserID() + " at " + global::System.DateTime.Now.ToString();
		string text2 = KCrashReporter.UploadSaveFile(save_file, text, new Dictionary<string, string> { 
		{
			"user",
			KCrashReporter.GetUserID()
		} });
		KCrashReporter.ReportError(msg, text, text2, ScreenPrefabs.Instance.ConfirmDialogScreen, confirmParent, "");
	}

	public static void Assert(bool condition, string message)
	{
		if (!condition && !KCrashReporter.hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(1, true);
			KCrashReporter.ReportError("ASSERT: " + message, stackTrace.ToString(), null, null, null, "");
		}
	}

	public static void ReportSimDLLCrash(string msg, string stack_trace, string dmp_filename)
	{
		if (KCrashReporter.hasReportedError)
		{
			return;
		}
		string text = null;
		string text2 = null;
		string text3 = null;
		if (dmp_filename != null)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dmp_filename);
			text2 = Path.Combine(Path.GetDirectoryName(KCrashReporter.dataRoot), dmp_filename);
			text3 = Path.Combine(Path.GetDirectoryName(KCrashReporter.dataRoot), fileNameWithoutExtension + ".sav");
			File.Move(text2, text3);
			text = KCrashReporter.UploadSaveFile(text3, stack_trace, new Dictionary<string, string> { 
			{
				"user",
				KCrashReporter.GetUserID()
			} });
		}
		KCrashReporter.ReportError(msg, stack_trace, text, null, null, "");
		if (dmp_filename != null)
		{
			File.Move(text3, text2);
		}
	}

	public static string MOST_RECENT_SAVEFILE = null;

	public const string CRASH_REPORTER_SERVER = "http://crashes.klei.ca";

	public const uint MAX_LOGS = 10000000U;

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static bool haveActiveMods = false;

	public static uint logCount = 0U;

	public static string error_canvas_name = "ErrorCanvas";

	private static bool disableDeduping = false;

	public static bool hasCrash = false;

	private static readonly Regex failedToLoadModuleRegEx = new Regex("^Failed to load '(.*?)' with error (.*)", RegexOptions.Multiline);

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private GameObject reportErrorPrefab;

	[SerializeField]
	private ConfirmDialogScreen confirmDialogPrefab;

	private GameObject errorScreen;

	public static bool terminateOnError = true;

	private static string dataRoot;

	private static readonly string[] IgnoreStrings = new string[] { "Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!", "The profiler has run out of samples for this frame. This frame will be skipped. Increase the sample limit using Profiler.maxNumberOfSamplesPerFrame", "Trying to add Text (LocText) for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported.", "Texture has out of range width / height", "<I> Failed to get cursor position:\r\nSuccess.\r\n" };

	private static HashSet<int> previouslyReportedDevNotifications;

	private class Error
	{
		public string game = "simgame";

		public int build = -1;

		public string platform = Environment.OSVersion.ToString();

		public string user = "unknown";

		public ulong steam64_verified;

		public string callstack = "";

		public string fullstack = "";

		public string log = "";

		public string summaryline = "";

		public string user_message = "";

		public bool is_server;

		public bool is_dedicated;

		public string save_hash = "";
	}
}
