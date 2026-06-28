using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using UnityEngine;

public class KleiMetrics : ThreadedHttps<KleiMetrics>
{
	public KleiMetrics()
	{
		this.LIVE_ENDPOINT = "metric.kleientertainment.com/write";
		this.serviceName = "KleiMetrics";
		this.CLIENT_KEY = DistributionPlatform.Inst.MetricsClientKey;
		this.PlatformUserIDFieldName = DistributionPlatform.Inst.MetricsUserIDField;
		KleiMetrics.sessionID = -1;
		this.enabled = KPlayerPrefs.GetInt("ENABLE_METRICS", 1) == 1;
		this.isMultiThreaded = true;
	}

	public bool isMultiThreaded { get; protected set; }

	public bool enabled { get; private set; }

	public void SetEnabled(bool enabled)
	{
		KPlayerPrefs.SetInt("ENABLE_METRICS", (!enabled) ? 0 : 1);
		this.enabled = enabled;
	}

	protected string PostMetricData(Dictionary<string, object> data)
	{
		KleiMetrics.PostData postData = new KleiMetrics.PostData(this.CLIENT_KEY, data);
		string text = JsonConvert.SerializeObject(postData);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		if (this.isMultiThreaded)
		{
			base.PutPacket(bytes, false);
			return "OK";
		}
		return base.Send(bytes, false);
	}

	public static string PlatformUserID()
	{
		DistributionPlatform.User localUser = DistributionPlatform.Inst.LocalUser;
		return (localUser == null) ? string.Empty : localUser.Id.ToString();
	}

	public static string UserID()
	{
		DistributionPlatform.User localUser = DistributionPlatform.Inst.LocalUser;
		return (localUser == null) ? string.Empty : localUser.Id.ToString();
	}

	private void IncrementSessionCount()
	{
		KleiMetrics.sessionID = KleiMetrics.SessionID() + 1;
		KPlayerPrefs.SetInt("SESSION_ID", KleiMetrics.sessionID);
	}

	public static int SessionID()
	{
		if (KleiMetrics.sessionID == -1)
		{
			KleiMetrics.sessionID = KPlayerPrefs.GetInt("SESSION_ID", -1);
		}
		return KleiMetrics.sessionID;
	}

	private void IncrementGameCount()
	{
		KleiMetrics.gameID = KleiMetrics.GameID() + 1;
		KleiMetrics.SetGameID(KleiMetrics.gameID);
	}

	public static int GameID()
	{
		if (KleiMetrics.gameID == -1)
		{
			KleiMetrics.gameID = KPlayerPrefs.GetInt("GAME_ID", -1);
		}
		return KleiMetrics.gameID;
	}

	public static void SetGameID(int id)
	{
		KPlayerPrefs.SetInt("GAME_ID", id);
	}

	public static string GetInstallTimeStamp()
	{
		if (KleiMetrics.installTimeStamp == null)
		{
			KleiMetrics.installTimeStamp = KPlayerPrefs.GetString("INSTALL_TIMESTAMP", null);
			if (KleiMetrics.installTimeStamp == null || KleiMetrics.installTimeStamp == string.Empty)
			{
				KleiMetrics.installTimeStamp = DateTime.UtcNow.Ticks.ToString();
				KPlayerPrefs.SetString("INSTALL_TIMESTAMP", KleiMetrics.installTimeStamp);
			}
		}
		return KleiMetrics.installTimeStamp;
	}

	public static string CurrentLevel()
	{
		return null;
	}

	public void SetLastUserAction(long lastUserActionTicks)
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			return;
		}
		this.currentSessionTicks = DateTime.Now.Ticks;
		if (this.shouldEndSession)
		{
			this.EndSession(false);
			this.shouldEndSession = false;
			this.shouldStartSession = true;
		}
		else if (this.shouldStartSession && lastUserActionTicks > this.lastHeartBeatTicks)
		{
			this.StartSession();
			this.shouldStartSession = false;
		}
		this.timeSinceLastUserAction = (float)TimeSpan.FromTicks(this.currentSessionTicks - lastUserActionTicks).TotalSeconds;
	}

	private void StopHeartBeat()
	{
		if (KleiMetrics.heartbeatTimer != null)
		{
			KleiMetrics.heartbeatTimer.Stop();
			KleiMetrics.heartbeatTimer.Dispose();
			KleiMetrics.heartbeatTimer = null;
		}
	}

	private void StartHeartBeat()
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			return;
		}
		this.StopHeartBeat();
		KleiMetrics.heartbeatTimer = new global::System.Timers.Timer((double)(this.HeartBeatInSeconds * 1000));
		KleiMetrics.heartbeatTimer.Elapsed += this.SendHeartBeat;
		KleiMetrics.heartbeatTimer.AutoReset = true;
		KleiMetrics.heartbeatTimer.Enabled = true;
		this.lastHeartBeatTicks = DateTime.Now.Ticks;
	}

	private uint GetSessionTime()
	{
		int num = (int)TimeSpan.FromTicks(this.currentSessionTicks - this.startTimeTicks).TotalSeconds;
		if (num < 0)
		{
			global::Debug.LogWarning("Session time is < 0", null);
		}
		return (uint)num;
	}

	private void SendHeartBeat(object source, ElapsedEventArgs e)
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			return;
		}
		Dictionary<string, object> dictionary = this.GetUserSession();
		dictionary.Add("LastUA", (int)this.timeSinceLastUserAction);
		if (this.timeSinceLastUserAction > (float)this.HeartBeatTimeOutInSeconds)
		{
			dictionary.Add("HeartBeatTimeOut", true);
			KleiMetrics.heartbeatTimer.Stop();
			this.shouldEndSession = true;
		}
		long num = DateTime.Now.Ticks - this.lastHeartBeatTicks;
		dictionary.Add("HeartBeat", (int)TimeSpan.FromTicks(num).TotalSeconds);
		this.PostMetricData(dictionary);
		this.lastHeartBeatTicks = DateTime.Now.Ticks;
	}

	private void StartThread()
	{
		if (!this.hasStarted)
		{
			if (this.isMultiThreaded)
			{
				base.Start();
			}
			this.hasStarted = true;
		}
	}

	private void EndThread()
	{
		if (this.hasStarted)
		{
			if (this.isMultiThreaded)
			{
				base.End();
			}
			this.hasStarted = false;
		}
	}

	public void SetStaticSessionVariable(string name, object var)
	{
		if (this.userSession.ContainsKey(name))
		{
			this.userSession[name] = var;
		}
		else
		{
			this.userSession.Add(name, var);
		}
	}

	public void RemoveStaticSessionVariable(string name)
	{
		if (this.userSession.ContainsKey(name))
		{
			this.userSession.Remove(name);
		}
	}

	public void AddDefaultSessionVariables()
	{
		this.userSession.Clear();
		this.SetStaticSessionVariable("InstallTimeStamp", KleiMetrics.GetInstallTimeStamp());
		this.SetStaticSessionVariable("user", KleiMetrics.UserID());
		this.SetStaticSessionVariable("SessionID", KleiMetrics.SessionID());
		this.SetStaticSessionVariable("SessionStartTimeStamp", this.sessionStartUtcTicks.ToString());
		if (KleiAccount.KleiUserID != null)
		{
			this.SetStaticSessionVariable("KU", KleiAccount.KleiUserID);
		}
	}

	private Dictionary<string, object> GetUserSession()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (!this.sessionStarted)
		{
			return dictionary;
		}
		foreach (KeyValuePair<string, object> keyValuePair in this.userSession)
		{
			dictionary.Add(keyValuePair.Key, keyValuePair.Value);
		}
		dictionary.Add("SessionTimeSeconds", this.GetSessionTime());
		int num = KleiMetrics.GameID();
		if (num != -1)
		{
			dictionary.Add("GameID", KleiMetrics.GameID());
		}
		string text = KleiMetrics.CurrentLevel();
		if (text != null)
		{
			dictionary.Add("Level", text);
		}
		if (this.SetDynamicSessionVariables != null)
		{
			try
			{
				this.SetDynamicSessionVariables(dictionary);
			}
			catch (Exception ex)
			{
				global::Debug.LogError("Dynamic session variables may be set from a thread. " + ex.Message + "\n" + ex.StackTrace, null);
			}
		}
		return dictionary;
	}

	public void SetCallBacks(global::System.Action setStaticSessionVariables, Action<Dictionary<string, object>> setDynamicSessionVariables)
	{
		this.SetDynamicSessionVariables = setDynamicSessionVariables;
		this.SetStaticSessionVariables = setStaticSessionVariables;
	}

	private void SetStartTime()
	{
		this.sessionStartUtcTicks = DateTime.UtcNow.Ticks;
		this.startTimeTicks = DateTime.Now.Ticks;
		this.currentSessionTicks = DateTime.Now.Ticks;
		this.sessionStarted = true;
	}

	public void StartSession()
	{
		if (!this.enabled)
		{
			return;
		}
		if (this.sessionStarted)
		{
			this.EndSession(false);
		}
		this.StartThread();
		this.SetStartTime();
		this.IncrementSessionCount();
		this.AddDefaultSessionVariables();
		if (this.SetStaticSessionVariables != null)
		{
			this.SetStaticSessionVariables();
		}
		Dictionary<string, object> dictionary = this.GetUserSession();
		dictionary.Add("StartSession", true);
		string text = KleiMetrics.PlatformUserID();
		if (text != null)
		{
			dictionary.Add(this.PlatformUserIDFieldName, text);
		}
		dictionary.Add("UserName", Environment.UserName);
		if (this.shouldStartSession)
		{
			dictionary.Add("HeartBeatTimeOut", false);
		}
		Dictionary<string, object> hardwareStats = KleiMetrics.GetHardwareStats();
		foreach (KeyValuePair<string, object> keyValuePair in hardwareStats)
		{
			dictionary.Add(keyValuePair.Key, keyValuePair.Value);
		}
		this.PostMetricData(dictionary);
		this.StartHeartBeat();
	}

	public void EndSession(bool crashed = false)
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			return;
		}
		Dictionary<string, object> dictionary = this.GetUserSession();
		dictionary.Add("EndSession", true);
		if (crashed)
		{
			dictionary.Add("EndSessionCrashed", true);
		}
		if (this.shouldEndSession)
		{
			dictionary.Add("HeartBeatTimeOut", true);
		}
		this.PostMetricData(dictionary);
		this.sessionStarted = false;
		this.StopHeartBeat();
		this.EndThread();
	}

	public void StartNewGame()
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			this.StartSession();
		}
		this.IncrementGameCount();
		Dictionary<string, object> dictionary = this.GetUserSession();
		dictionary.Add("NewGame", true);
		this.PostMetricData(dictionary);
	}

	public void EndGame()
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			return;
		}
		Dictionary<string, object> dictionary = this.GetUserSession();
		dictionary.Add("EndGame", true);
		this.PostMetricData(dictionary);
	}

	public void SendEvent(Dictionary<string, object> eventData)
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			this.StartSession();
		}
		Dictionary<string, object> dictionary = this.GetUserSession();
		foreach (KeyValuePair<string, object> keyValuePair in eventData)
		{
			dictionary.Add(keyValuePair.Key, keyValuePair.Value);
		}
		this.PostMetricData(dictionary);
	}

	public bool SendProfileStats()
	{
		if (!this.enabled)
		{
			return false;
		}
		Dictionary<string, object> dictionary = this.GetUserSession();
		return ThreadedHttps<KleiMetrics>.Instance.PostMetricData(dictionary) == "OK";
	}

	public static Dictionary<string, object> GetHardwareStats()
	{
		return new Dictionary<string, object>
		{
			{
				"Platform",
				Application.platform.ToString()
			},
			{
				"OSname",
				SystemInfo.operatingSystem
			},
			{
				"OSversion",
				Environment.OSVersion.Version.ToString()
			},
			{
				"CPUmodel",
				SystemInfo.deviceModel
			},
			{
				"CPUdeviceType",
				SystemInfo.deviceType.ToString()
			},
			{
				"CPUarch",
				Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
			},
			{
				"ProcBits",
				(IntPtr.Size != 4) ? 64 : 32
			},
			{
				"CPUcount",
				SystemInfo.processorCount
			},
			{
				"CPUtype",
				SystemInfo.processorType
			},
			{
				"SystemMemoryMegs",
				SystemInfo.systemMemorySize
			},
			{
				"GPUgraphicsDeviceID",
				SystemInfo.graphicsDeviceID
			},
			{
				"GPUname",
				SystemInfo.graphicsDeviceName
			},
			{
				"GPUgraphicsDeviceType",
				SystemInfo.graphicsDeviceType.ToString()
			},
			{
				"GPUgraphicsDeviceVendor",
				SystemInfo.graphicsDeviceVendor
			},
			{
				"GPUgraphicsDeviceVendorID",
				SystemInfo.graphicsDeviceVendorID
			},
			{
				"GPUgraphicsDeviceVersion",
				SystemInfo.graphicsDeviceVersion
			},
			{
				"GPUmemoryMegs",
				SystemInfo.graphicsMemorySize
			},
			{
				"GPUgraphicsMultiThreaded",
				SystemInfo.graphicsMultiThreaded
			},
			{
				"GPUgraphicsShaderLevel",
				SystemInfo.graphicsShaderLevel
			},
			{
				"GPUmaxTextureSize",
				SystemInfo.maxTextureSize
			},
			{
				"GPUnpotSupport",
				SystemInfo.npotSupport.ToString()
			},
			{
				"GPUsupportedRenderTargetCount",
				SystemInfo.supportedRenderTargetCount
			},
			{
				"GPUsupports3DTextures",
				SystemInfo.supports3DTextures
			},
			{
				"GPUsupportsComputeShaders",
				SystemInfo.supportsComputeShaders
			},
			{
				"GPUsupportsImageEffects",
				SystemInfo.supportsImageEffects
			},
			{
				"GPUsupportsInstancing",
				SystemInfo.supportsInstancing
			},
			{
				"GPUsupportsRenderToCubemap",
				SystemInfo.supportsRenderToCubemap
			},
			{
				"GPUsupportsShadows",
				SystemInfo.supportsShadows
			},
			{
				"GPUsupportsSparseTextures",
				SystemInfo.supportsSparseTextures
			}
		};
	}

	private const string EnableMetricsKey = "ENABLE_METRICS";

	private const string SessionIDKey = "SESSION_ID";

	private const string GameIDKey = "GAME_ID";

	private const string InstallTimeStampKey = "INSTALL_TIMESTAMP";

	private const string UserIDFieldName = "user";

	private const string SessionIDFieldName = "SessionID";

	private const string GameIDFieldName = "GameID";

	private const string InstallTimeStampFieldName = "InstallTimeStamp";

	private const string KleiUserFieldName = "KU";

	private const string StartSessionFieldName = "StartSession";

	private const string EndSessionFieldName = "EndSession";

	private const string EndSessionCrashedFieldName = "EndSessionCrashed";

	private const string SessionStartTimeStampFieldName = "SessionStartTimeStamp";

	private const string SessionTimeFieldName = "SessionTimeSeconds";

	private const string NewGameFieldName = "NewGame";

	private const string EndGameFieldName = "EndGame";

	public const string GameTimeFieldName = "GameTimeSeconds";

	private const string LevelFieldName = "Level";

	public const string BuildBranchName = "Branch";

	public const string BuildFieldName = "Build";

	private const int EDITOR_BUILD_ID = -1;

	private const string HeartBeatFieldName = "HeartBeat";

	private const string HeartBeatTimeOutFieldName = "HeartBeatTimeOut";

	private const string LastUserActionFieldName = "LastUA";

	private string PlatformUserIDFieldName;

	private static int sessionID = -1;

	private static int gameID = -1;

	private static string installTimeStamp;

	private static global::System.Timers.Timer heartbeatTimer;

	private int HeartBeatInSeconds = 180;

	private int HeartBeatTimeOutInSeconds = 1200;

	private long currentSessionTicks = DateTime.Now.Ticks;

	private float timeSinceLastUserAction;

	private long lastHeartBeatTicks = DateTime.Now.Ticks;

	private long startTimeTicks = DateTime.Now.Ticks;

	private bool shouldEndSession;

	private bool shouldStartSession;

	private bool hasStarted;

	private Dictionary<string, object> userSession = new Dictionary<string, object>();

	private Action<Dictionary<string, object>> SetDynamicSessionVariables;

	private global::System.Action SetStaticSessionVariables;

	private bool sessionStarted;

	private long sessionStartUtcTicks = DateTime.UtcNow.Ticks;

	protected struct PostData
	{
		public PostData(string key, Dictionary<string, object> data)
		{
			this.clientKey = key;
			this.metricData = data;
		}

		public string clientKey;

		public Dictionary<string, object> metricData;
	}
}
