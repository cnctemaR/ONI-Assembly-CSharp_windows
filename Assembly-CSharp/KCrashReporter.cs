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
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> onCrashReported;

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
					string text3 = Path.Combine(KCrashReporter.dataRoot, key);
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
		if (Application.isEditor && !GenericGameSettings.instance.enableEditorCrashReporting)
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
				canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
				gameObject.AddComponent<GraphicRaycaster>();
			}
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(this.reportErrorPrefab, Vector3.zero, Quaternion.identity);
			gameObject2.transform.SetParent(gameObject.transform, false);
			this.errorDialog = gameObject2.GetComponentInChildren<ReportErrorDialog>();
			this.errorDialog.PopupConfirmDialog("ERROR OCCURRED!\nDo you want to report this error?", delegate
			{
				string text = null;
				if (KCrashReporter.MOST_RECENT_SAVEFILE != null)
				{
					text = KCrashReporter.UploadSaveFile(KCrashReporter.MOST_RECENT_SAVEFILE, local_stack_trace, null);
				}
				KCrashReporter.ReportError(local_msg, local_stack_trace, text, this.confirmDialogPrefab, this.errorDialog.UserMessage());
			}, delegate
			{
				this.OnQuitToDesktop();
			}, delegate
			{
				this.OnCloseErrorDialog();
			});
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

	private static string UploadSaveFile(string save_file, string stack_trace, Dictionary<string, string> metadata = null)
	{
		global::Debug.Log(string.Format("Save_file: {0}", save_file), null);
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			return string.Empty;
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
				string text2 = string.Empty;
				string text3;
				using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
				{
					text3 = BitConverter.ToString(sha1CryptoServiceProvider.ComputeHash(array)).Replace("-", string.Empty);
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
					global::Debug.Log(ex, null);
					return string.Empty;
				}
			}
		}
		return string.Empty;
	}

	private static string GetUserID()
	{
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
		return Environment.UserName;
	}

	private static string GetLogContents()
	{
		string text = string.Empty;
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/Editor/Editor.log");
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../LocalLow/Klei/Oxygen Not Included/output_log.txt");
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library/Logs/Unity/Editor.log");
		}
		else if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library/Logs/Unity/Player.log");
		}
		else
		{
			if (Application.platform != RuntimePlatform.LinuxPlayer)
			{
				return string.Empty;
			}
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "unity3d/Klei/Oxygen Not Included/Player.log");
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
		if (KCrashReporter.ignoreAll)
		{
			return;
		}
		global::Debug.Log("Reporting error.\n", null);
		if (msg != null)
		{
			global::Debug.Log(msg, null);
		}
		if (stack_trace != null)
		{
			global::Debug.Log(stack_trace, null);
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
				stack_trace = string.Format("No stack trace.\n\n{0}", msg);
			}
			string text3 = string.Empty;
			string[] array = new string[] { "Debug:LogError", "UnityEngine.Debug:LogError", "UnityEngine.Debug:Assert(Boolean, String)", "Output:LogError(String)", "Output:LogErrorWithObj(Object, String)", "Output:LogErrorWithObj(Object, Object[])", "DebugUtil:Assert(Boolean, String)", "KCrashReporter.Assert(Boolean condition, System.String message)", "No stack trace." };
			foreach (string text4 in stack_trace.Split(new char[] { '\n' }))
			{
				if (!string.IsNullOrEmpty(text4))
				{
					bool flag = false;
					foreach (string text5 in array)
					{
						if (text4.StartsWith(text5))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						text3 = text3 + text4 + "\n";
					}
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
			if (KCrashReporter.debugWasUsed)
			{
				msg = "Debug tools were used in this game.\n\n" + msg;
			}
			error.fullstack = msg;
			error.build = 300458;
			error.log = KCrashReporter.GetLogContents();
			error.summaryline = msg;
			error.user_message = userMessage;
			if (!string.IsNullOrEmpty(save_file_hash))
			{
				error.save_hash = save_file_hash;
			}
			if (DistributionPlatform.Initialized)
			{
				error.steam64_verified = DistributionPlatform.Inst.LocalUser.Id.ToInt64();
			}
			string text6 = JsonConvert.SerializeObject(error);
			string empty = string.Empty;
			Uri uri = new Uri("http://crashes.klei.ca/submitCrash");
			global::Debug.Log("Submitting crash:", null);
			try
			{
				webClient.UploadStringAsync(uri, text6);
			}
			catch (Exception ex)
			{
				global::Debug.Log(ex, null);
			}
			if (confirm_prefab != null)
			{
				ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, null);
				confirmDialogScreen.PopupConfirmDialog("Reported Error", null, null, null, null, null, null, null, null);
			}
			text7 = empty;
		}
		if (KCrashReporter.onCrashReported != null)
		{
			KCrashReporter.onCrashReported(text7);
		}
	}

	public static void ReportBug(string msg, string save_file)
	{
		string text = "Bug Report From: " + KCrashReporter.GetUserID() + " at " + global::System.DateTime.Now.ToString();
		string text2 = KCrashReporter.UploadSaveFile(save_file, text, new Dictionary<string, string> { 
		{
			"user",
			KCrashReporter.GetUserID()
		} });
		KCrashReporter.ReportError(msg, text, text2, ScreenPrefabs.Instance.ConfirmDialogScreen, string.Empty);
	}

	public static void Assert(bool condition, string message)
	{
		if (!condition && !KCrashReporter.hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(1, true);
			KCrashReporter.ReportError("ASSERT: " + message, stackTrace.ToString(), null, null, string.Empty);
		}
	}

	public static void ReportDLLCrash(string msg, string stack_trace, string dmp_filename)
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
		KCrashReporter.ReportError(msg, stack_trace, text, null, string.Empty);
		if (dmp_filename != null)
		{
			File.Move(text3, text2);
		}
	}

	public static string MOST_RECENT_SAVEFILE = null;

	public const string CRASH_REPORTER_SERVER = "http://crashes.klei.ca";

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static string error_canvas_name = "ErrorCanvas";

	private static bool disableDeduping = false;

	private static bool hasReportedError;

	private static readonly Regex failedToLoadModuleRegEx = new Regex("^Failed to load '(.*?)' with error (.*)", RegexOptions.Multiline);

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private GameObject reportErrorPrefab;

	[SerializeField]
	private ConfirmDialogScreen confirmDialogPrefab;

	private ReportErrorDialog errorDialog;

	public static bool terminateOnError = true;

	private static string dataRoot;

	private static readonly string[] IgnoreStrings = new string[] { "Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!", "The profiler has run out of samples for this frame. This frame will be skipped. Increase the sample limit using Profiler.maxNumberOfSamplesPerFrame", "Trying to add Text (LocText) for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported." };

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
