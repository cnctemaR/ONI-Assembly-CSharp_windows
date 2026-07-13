using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Klei;
using KMod;
using Newtonsoft.Json;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
	public static event Action<bool> onCrashReported;

	public static event Action<float> onCrashUploadProgress;

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
		if (msg != null && msg.StartsWith(DebugUtil.START_CALLSTACK))
		{
			string text = msg;
			msg = text.Substring(text.IndexOf(DebugUtil.END_CALLSTACK, StringComparison.Ordinal) + DebugUtil.END_CALLSTACK.Length);
			stack_trace = text.Substring(DebugUtil.START_CALLSTACK.Length, text.IndexOf(DebugUtil.END_CALLSTACK, StringComparison.Ordinal) - DebugUtil.START_CALLSTACK.Length);
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
			string text2 = msg;
			string text3 = stack_trace;
			if (string.IsNullOrEmpty(text3))
			{
				text3 = new StackTrace(5, true).ToString();
			}
			if (App.isLoading)
			{
				if (!SceneInitializerLoader.deferred_error.IsValid)
				{
					SceneInitializerLoader.deferred_error = new SceneInitializerLoader.DeferredError
					{
						msg = text2,
						stack_trace = text3
					};
					return;
				}
			}
			else
			{
				this.ShowDialog(text2, text3, true, null, null);
			}
		}
	}

	public bool ShowDialog(string error, string stack_trace, bool includeSaveFile = true, string[] extraCategories = null, string[] extraFiles = null)
	{
		GenericGameSettings instance = GenericGameSettings.instance;
		if (instance.devBootSmoke || !string.IsNullOrEmpty(instance.scriptedProfile.saveGame))
		{
			KCrashReporter.hasCrash = true;
			global::Debug.Log("Automated test resulted in a crash. Return exit code 1.");
			App.QuitCode(1);
			return false;
		}
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
		if (Global.Instance != null && Global.Instance.modManager != null && Global.Instance.modManager.HasCrashableMods())
		{
			Exception ex = DebugUtil.RetrieveLastExceptionLogged();
			StackTrace stackTrace = ((ex != null) ? new StackTrace(ex) : new StackTrace(5, true));
			Global.Instance.modManager.SearchForModsInStackTrace(stackTrace);
			Global.Instance.modManager.SearchForModsInStackTrace(stack_trace);
			errorDialog.PopupDisableModsDialog(text, new global::System.Action(this.OnQuitToDesktopCrashed), (Global.Instance.modManager.IsInDevMode() || !KCrashReporter.terminateOnError) ? new global::System.Action(this.OnCloseErrorDialog) : null);
		}
		else
		{
			errorDialog.PopupSubmitErrorDialog(text, delegate
			{
				KCrashReporter.ReportError(error, stack_trace, this.confirmDialogPrefab, this.errorScreen, errorDialog.UserMessage(), includeSaveFile, extraCategories, extraFiles);
			}, new global::System.Action(this.OnQuitToDesktopCrashed), KCrashReporter.terminateOnError ? null : new global::System.Action(this.OnCloseErrorDialog));
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

	private void OnQuitToDesktopCrashed()
	{
		App.QuitCode(1);
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
		return "LocalUser_" + Environment.UserName;
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

	public static void ReportDevNotification(string notification_name, string stack_trace, string details = "", bool includeSaveFile = false, string[] extraCategories = null)
	{
		if (KCrashReporter.previouslyReportedDevNotifications == null)
		{
			KCrashReporter.previouslyReportedDevNotifications = new HashSet<int>();
		}
		details = notification_name + " - " + details;
		global::Debug.Log(details);
		int hashValue = new HashedString(notification_name).HashValue;
		bool hasReportedError = KCrashReporter.hasReportedError;
		if (!KCrashReporter.previouslyReportedDevNotifications.Contains(hashValue))
		{
			KCrashReporter.previouslyReportedDevNotifications.Add(hashValue);
			if (extraCategories != null)
			{
				Array.Resize<string>(ref extraCategories, extraCategories.Length + 1);
				extraCategories[extraCategories.Length - 1] = KCrashReporter.CRASH_CATEGORY.DEVNOTIFICATION;
			}
			else
			{
				extraCategories = new string[] { KCrashReporter.CRASH_CATEGORY.DEVNOTIFICATION };
			}
			KCrashReporter.ReportError("DevNotification: " + notification_name, stack_trace, null, null, details, includeSaveFile, extraCategories, null);
		}
		KCrashReporter.hasReportedError = hasReportedError;
	}

	public static void ReportError(string msg, string stack_trace, ConfirmDialogScreen confirm_prefab, GameObject confirm_parent, string userMessage = "", bool includeSaveFile = true, string[] extraCategories = null, string[] extraFiles = null)
	{
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			return;
		}
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
		foreach (string text3 in stack_trace.Split('\n', StringSplitOptions.None))
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
			userMessage = "[" + BuildWatermark.GetBuildText() + "] " + userMessage;
		}
		userMessage = userMessage.Replace(stack_trace, "");
		KCrashReporter.Error error = new KCrashReporter.Error();
		if (extraCategories != null)
		{
			error.categories.AddRange(extraCategories);
		}
		error.callstack = stack_trace;
		if (KCrashReporter.disableDeduping)
		{
			error.callstack = error.callstack + "\n" + Guid.NewGuid().ToString();
		}
		error.fullstack = string.Format("{0}\n\n{1}", msg, stack_trace);
		error.summaryline = string.Join("\n", list.ToArray());
		error.userMessage = userMessage;
		List<string> list2 = new List<string>();
		if (includeSaveFile && KCrashReporter.MOST_RECENT_SAVEFILE != null)
		{
			list2.Add(KCrashReporter.MOST_RECENT_SAVEFILE);
			error.saveFilename = Path.GetFileName(KCrashReporter.MOST_RECENT_SAVEFILE);
		}
		if (extraFiles != null)
		{
			foreach (string text5 in extraFiles)
			{
				list2.Add(text5);
				error.extraFilenames.Add(Path.GetFileName(text5));
			}
		}
		string text6 = JsonConvert.SerializeObject(error);
		byte[] array4 = KCrashReporter.CreateArchiveZip(KCrashReporter.GetLogContents(), list2);
		global::System.Action action = delegate
		{
			if (confirm_prefab != null && confirm_parent != null)
			{
				((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, confirm_parent)).PopupConfirmDialog(UI.CRASHSCREEN.REPORTEDERROR_SUCCESS, null, null, null, null, null, null, null, null);
			}
		};
		Action<long> action2 = delegate(long errorCode)
		{
			if (confirm_prefab != null && confirm_parent != null)
			{
				string text7 = ((errorCode == 413L) ? UI.CRASHSCREEN.REPORTEDERROR_FAILURE_TOO_LARGE : UI.CRASHSCREEN.REPORTEDERROR_FAILURE);
				((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, confirm_parent)).PopupConfirmDialog(text7, null, null, null, null, null, null, null, null);
			}
		};
		KCrashReporter.pendingCrash = new KCrashReporter.PendingCrash
		{
			jsonString = text6,
			archiveData = array4,
			successCallback = action,
			failureCallback = action2
		};
	}

	private static IEnumerator SubmitCrashAsync(string jsonString, byte[] archiveData, global::System.Action successCallback, Action<long> failureCallback)
	{
		bool success = false;
		Uri uri = new Uri("https://games-feedback.klei.com/submit");
		List<IMultipartFormSection> list = new List<IMultipartFormSection>
		{
			new MultipartFormDataSection("metadata", jsonString),
			new MultipartFormFileSection("archiveFile", archiveData, "Archive.zip", "application/octet-stream")
		};
		if (KleiAccount.KleiToken != null)
		{
			list.Add(new MultipartFormDataSection("loginToken", KleiAccount.KleiToken));
		}
		using (UnityWebRequest w = UnityWebRequest.Post(uri, list))
		{
			w.SendWebRequest();
			while (!w.isDone)
			{
				yield return null;
				if (KCrashReporter.onCrashUploadProgress != null)
				{
					KCrashReporter.onCrashUploadProgress(w.uploadProgress);
				}
			}
			if (w.result == UnityWebRequest.Result.Success)
			{
				global::UnityEngine.Debug.Log("Submitted crash!");
				if (successCallback != null)
				{
					successCallback();
				}
				success = true;
			}
			else
			{
				global::UnityEngine.Debug.Log("CrashReporter: Could not submit crash " + w.result.ToString());
				if (failureCallback != null)
				{
					failureCallback(w.responseCode);
				}
			}
		}
		UnityWebRequest w = null;
		if (KCrashReporter.onCrashReported != null)
		{
			KCrashReporter.onCrashReported(success);
		}
		yield break;
		yield break;
	}

	public static void ReportBug(string msg, GameObject confirmParent)
	{
		string text = "Bug Report From: " + KCrashReporter.GetUserID() + " at " + global::System.DateTime.Now.ToString();
		KCrashReporter.ReportError(msg, text, ScreenPrefabs.Instance.ConfirmDialogScreen, confirmParent, "", true, null, null);
	}

	public static void Assert(bool condition, string message, string[] extraCategories = null)
	{
		if (!condition && !KCrashReporter.hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(1, true);
			KCrashReporter.ReportError("ASSERT: " + message, stackTrace.ToString(), null, null, null, true, extraCategories, null);
		}
	}

	public static void ReportSimDLLCrash(string msg, string stack_trace, string dmp_filename)
	{
		if (KCrashReporter.hasReportedError)
		{
			return;
		}
		KCrashReporter.pendingReport = new KCrashReporter.PendingReport(msg, stack_trace, dmp_filename);
	}

	private static byte[] CreateArchiveZip(string log, List<string> files)
	{
		byte[] array2;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (ZipArchive zipArchive = new ZipArchive(memoryStream, 1, true))
			{
				if (files != null)
				{
					foreach (string text in files)
					{
						try
						{
							if (!File.Exists(text))
							{
								global::UnityEngine.Debug.Log("CrashReporter: file does not exist to include: " + text);
							}
							else
							{
								using (Stream stream = zipArchive.CreateEntry(Path.GetFileName(text), global::System.IO.Compression.CompressionLevel.Fastest).Open())
								{
									byte[] array = File.ReadAllBytes(text);
									stream.Write(array, 0, array.Length);
								}
							}
						}
						catch (Exception ex)
						{
							string text2 = "CrashReporter: Could not add file '";
							string text3 = text;
							string text4 = "' to archive: ";
							Exception ex2 = ex;
							global::UnityEngine.Debug.Log(text2 + text3 + text4 + ((ex2 != null) ? ex2.ToString() : null));
						}
					}
					using (Stream stream2 = zipArchive.CreateEntry("Player.log", global::System.IO.Compression.CompressionLevel.Fastest).Open())
					{
						byte[] bytes = Encoding.UTF8.GetBytes(log);
						stream2.Write(bytes, 0, bytes.Length);
					}
				}
			}
			array2 = memoryStream.ToArray();
		}
		return array2;
	}

	private void Update()
	{
		if (KCrashReporter.pendingReport != null)
		{
			KCrashReporter.PendingReport pendingReport = KCrashReporter.pendingReport;
			KCrashReporter.pendingReport = null;
			if (KCrashReporter.hasReportedError)
			{
				return;
			}
			KCrashReporter component = Global.Instance.GetComponent<KCrashReporter>();
			if (component != null)
			{
				component.ShowDialog(pendingReport.message, pendingReport.stack_trace, true, new string[] { KCrashReporter.CRASH_CATEGORY.SIM }, new string[] { pendingReport.additional_filename });
			}
			else
			{
				KCrashReporter.ReportError(pendingReport.message, pendingReport.stack_trace, null, null, "", true, new string[] { KCrashReporter.CRASH_CATEGORY.SIM }, new string[] { pendingReport.additional_filename });
			}
		}
		if (KCrashReporter.pendingCrash != null)
		{
			KCrashReporter.PendingCrash pendingCrash = KCrashReporter.pendingCrash;
			KCrashReporter.pendingCrash = null;
			global::Debug.Log("Submitting crash...");
			base.StartCoroutine(KCrashReporter.SubmitCrashAsync(pendingCrash.jsonString, pendingCrash.archiveData, pendingCrash.successCallback, pendingCrash.failureCallback));
		}
	}

	public static string MOST_RECENT_SAVEFILE = null;

	public const string CRASH_REPORTER_SERVER = "https://games-feedback.klei.com";

	public const uint MAX_LOGS = 10000000U;

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static bool haveActiveMods = false;

	public static uint logCount = 0U;

	public static string error_canvas_name = "ErrorCanvas";

	public static bool disableDeduping = false;

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

	private static KCrashReporter.PendingReport pendingReport;

	private static KCrashReporter.PendingCrash pendingCrash;

	public class CRASH_CATEGORY
	{
		public static string DEVNOTIFICATION = "DevNotification";

		public static string VANILLA = "Vanilla";

		public static string SPACEDOUT = "SpacedOut";

		public static string MODDED = "Modded";

		public static string DEBUGUSED = "DebugUsed";

		public static string SANDBOX = "Sandbox";

		public static string STEAMDECK = "SteamDeck";

		public static string SIM = "SimDll";

		public static string FILEIO = "FileIO";

		public static string MODSYSTEM = "ModSystem";

		public static string WORLDGENFAILURE = "WorldgenFailure";
	}

	private class Error
	{
		public Error()
		{
			this.userName = KCrashReporter.GetUserID();
			this.platform = Util.GetOperatingSystem();
			this.InitDefaultCategories();
			this.InitSku();
			this.InitSlackSummary();
			if (DistributionPlatform.Inst.Initialized)
			{
				string text;
				bool flag = !SteamApps.GetCurrentBetaName(out text, 100);
				this.branch = text;
				if (text == "public_playtest")
				{
					this.branch = "public_testing";
				}
				if (flag || (text == "public_testing" && !global::UnityEngine.Debug.isDebugBuild))
				{
					this.branch = "default";
				}
			}
		}

		private void InitDefaultCategories()
		{
			if (DlcManager.IsPureVanilla())
			{
				this.categories.Add(KCrashReporter.CRASH_CATEGORY.VANILLA);
			}
			if (DlcManager.IsExpansion1Active())
			{
				this.categories.Add(KCrashReporter.CRASH_CATEGORY.SPACEDOUT);
			}
			foreach (string text in DlcManager.GetActiveDLCIds())
			{
				if (!(text == "EXPANSION1_ID"))
				{
					this.categories.Add(text);
				}
			}
			if (KCrashReporter.debugWasUsed)
			{
				this.categories.Add(KCrashReporter.CRASH_CATEGORY.DEBUGUSED);
			}
			if (KCrashReporter.haveActiveMods)
			{
				this.categories.Add(KCrashReporter.CRASH_CATEGORY.MODDED);
			}
			if (SaveGame.Instance != null && SaveGame.Instance.sandboxEnabled)
			{
				this.categories.Add(KCrashReporter.CRASH_CATEGORY.SANDBOX);
			}
			if (DistributionPlatform.Inst.Initialized && SteamUtils.IsSteamRunningOnSteamDeck())
			{
				this.categories.Add(KCrashReporter.CRASH_CATEGORY.STEAMDECK);
			}
		}

		private void InitSku()
		{
			this.sku = "steam";
			if (DistributionPlatform.Inst.Initialized)
			{
				string text;
				bool flag = !SteamApps.GetCurrentBetaName(out text, 100);
				if (text == "public_testing" || text == "preview" || text == "public_playtest" || text == "playtest")
				{
					if (global::UnityEngine.Debug.isDebugBuild)
					{
						this.sku = "steam-public-testing";
					}
					else
					{
						this.sku = "steam-release";
					}
				}
				if (flag || text == "release")
				{
					this.sku = "steam-release";
				}
			}
		}

		private void InitSlackSummary()
		{
			string buildText = BuildWatermark.GetBuildText();
			string text = ((GameClock.Instance != null) ? string.Format(" - Cycle {0}", GameClock.Instance.GetCycle() + 1) : "");
			int num;
			if (!(Global.Instance != null) || Global.Instance.modManager == null)
			{
				num = 0;
			}
			else
			{
				num = Global.Instance.modManager.mods.Count<Mod>((Mod x) => x.IsEnabledForActiveDlc());
			}
			int num2 = num;
			string text2 = ((num2 > 0) ? string.Format(" - {0} active mods", num2) : "");
			this.slackSummary = string.Concat(new string[] { buildText, " ", this.platform, text, text2 });
		}

		public string game = "ONI";

		public string userName;

		public string platform;

		public string version = LaunchInitializer.BuildPrefix();

		public string branch = "default";

		public string sku = "";

		public int build = 707956;

		public string callstack = "";

		public string fullstack = "";

		public string summaryline = "";

		public string userMessage = "";

		public List<string> categories = new List<string>();

		public string slackSummary;

		public string logFilename = "Player.log";

		public string saveFilename = "";

		public string screenshotFilename = "";

		public List<string> extraFilenames = new List<string>();

		public string title = "";

		public bool isServer;

		public bool isDedicated;

		public bool isError = true;

		public string emote = "";
	}

	public class PendingCrash
	{
		public string jsonString;

		public byte[] archiveData;

		public global::System.Action successCallback;

		public Action<long> failureCallback;
	}

	public class PendingReport
	{
		public PendingReport(string msg, string stack_trace, string filename)
		{
			this.message = msg;
			this.stack_trace = stack_trace;
			this.additional_filename = filename;
		}

		public string message;

		public string stack_trace;

		public string additional_filename;
	}
}
