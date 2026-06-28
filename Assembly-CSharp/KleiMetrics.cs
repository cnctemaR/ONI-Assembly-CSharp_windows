using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using UnityEngine;

public class KleiMetrics : ThreadedHttps<KleiMetrics>, KleiMetricsInterface
{
	public KleiMetrics()
	{
		this.LIVE_ENDPOINT = "metric.kleientertainment.com/write";
		this.serviceName = "KleiMetrics";
		this.CLIENT_KEY = DistributionPlatform.Inst.MetricsClientKey;
		this.PlatformUserIDFieldName = DistributionPlatform.Inst.MetricsUserIDField;
		KleiMetrics.sessionID = -1;
		this.enabled = PlayerPrefs.GetInt("ENABLE_METRICS", 1) == 1;
	}

	public bool enabled { get; private set; }

	public void SetEnabled(bool enabled)
	{
		PlayerPrefs.SetInt("ENABLE_METRICS", (!enabled) ? 0 : 1);
		this.enabled = enabled;
	}

	private string PostMetricData(Dictionary<string, object> data)
	{
		KleiMetrics.PostData postData = new KleiMetrics.PostData(this.CLIENT_KEY, data);
		string text = JsonConvert.SerializeObject(postData);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		base.PutPacket(bytes, false);
		return "OK";
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
		PlayerPrefs.SetInt("SESSION_ID", KleiMetrics.sessionID);
	}

	public static int SessionID()
	{
		if (KleiMetrics.sessionID == -1)
		{
			KleiMetrics.sessionID = PlayerPrefs.GetInt("SESSION_ID", -1);
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
			KleiMetrics.gameID = PlayerPrefs.GetInt("GAME_ID", -1);
		}
		return KleiMetrics.gameID;
	}

	public static void SetGameID(int id)
	{
		PlayerPrefs.SetInt("GAME_ID", id);
	}

	public static string GetInstallTimeStamp()
	{
		if (KleiMetrics.installTimeStamp == null)
		{
			KleiMetrics.installTimeStamp = PlayerPrefs.GetString("INSTALL_TIMESTAMP", null);
			if (KleiMetrics.installTimeStamp == null || KleiMetrics.installTimeStamp == string.Empty)
			{
				KleiMetrics.installTimeStamp = global::System.DateTime.UtcNow.Ticks.ToString();
				PlayerPrefs.SetString("INSTALL_TIMESTAMP", KleiMetrics.installTimeStamp);
			}
		}
		return KleiMetrics.installTimeStamp;
	}

	public static string CurrentLevel()
	{
		return null;
	}

	public void SetLastUserAction(long lastUserActionTick)
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.sessionStarted)
		{
			return;
		}
		this.currentSessionTicks = global::System.DateTime.Now.Ticks;
		if (this.shouldEndSession)
		{
			this.EndSession(false);
			this.shouldEndSession = false;
			this.shouldStartSession = true;
		}
		else if (this.shouldStartSession && lastUserActionTick > this.lastHeartBeatTick)
		{
			this.StartSession();
			this.shouldStartSession = false;
		}
		this.timeSinceLastUserAction = (float)TimeSpan.FromTicks(this.currentSessionTicks - lastUserActionTick).TotalSeconds;
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
		this.lastHeartBeatTick = global::System.DateTime.Now.Ticks;
	}

	private int GetSessionTime()
	{
		return (int)TimeSpan.FromTicks(this.currentSessionTicks - this.startTime).TotalSeconds;
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
		Dictionary<string, object> userSession = this.GetUserSession();
		userSession.Add("LastUA", (int)this.timeSinceLastUserAction);
		if (this.timeSinceLastUserAction > (float)this.HeartBeatTimeOutInSeconds)
		{
			userSession.Add("HeartBeatTimeOut", true);
			KleiMetrics.heartbeatTimer.Stop();
			this.shouldEndSession = true;
		}
		long num = global::System.DateTime.Now.Ticks - this.lastHeartBeatTick;
		userSession.Add("HeartBeat", (int)TimeSpan.FromTicks(num).TotalSeconds);
		this.PostMetricData(userSession);
		this.lastHeartBeatTick = global::System.DateTime.Now.Ticks;
	}

	private void StartThread()
	{
		if (!this.hasStarted)
		{
			base.Start();
			this.hasStarted = true;
		}
	}

	private void EndThread()
	{
		if (this.hasStarted)
		{
			base.End();
			this.hasStarted = false;
		}
	}

	private Dictionary<string, object> GetUserSession()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (!this.sessionStarted)
		{
			return dictionary;
		}
		dictionary.Add("InstallTimeStamp", KleiMetrics.GetInstallTimeStamp());
		dictionary.Add("user", KleiMetrics.UserID());
		dictionary.Add("SessionID", KleiMetrics.SessionID());
		dictionary.Add("SessionStartTimeStamp", this.startTime.ToString());
		dictionary.Add("SessionTimeSeconds", this.GetSessionTime());
		string text = KleiMetrics.PlatformUserID();
		if (text != null)
		{
		}
		if (KleiAccount.KleiUserID != null)
		{
			dictionary.Add("KU", KleiAccount.KleiUserID);
		}
		int num = KleiMetrics.GameID();
		if (num != -1)
		{
			dictionary.Add("GameID", KleiMetrics.GameID());
		}
		if (Game.Instance != null && GameClock.Instance != null)
		{
			dictionary.Add("GameTimeSeconds", (int)GameClock.Instance.GetTime());
		}
		string text2 = KleiMetrics.CurrentLevel();
		if (text2 != null)
		{
			dictionary.Add("Level", text2);
		}
		dictionary.Add("Branch", "release");
		dictionary.Add("Build", 217844U);
		return dictionary;
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
		this.sessionStarted = true;
		this.startTime = global::System.DateTime.Now.Ticks;
		this.currentSessionTicks = this.startTime;
		this.IncrementSessionCount();
		Dictionary<string, object> userSession = this.GetUserSession();
		userSession.Add("StartSession", true);
		string text = KleiMetrics.PlatformUserID();
		if (text != null)
		{
			userSession.Add(this.PlatformUserIDFieldName, text);
		}
		userSession.Add("UserName", Environment.UserName);
		if (this.shouldStartSession)
		{
			userSession.Add("HeartBeatTimeOut", false);
		}
		Dictionary<string, object> hardwareStats = KleiMetrics.GetHardwareStats();
		foreach (KeyValuePair<string, object> keyValuePair in hardwareStats)
		{
			userSession.Add(keyValuePair.Key, keyValuePair.Value);
		}
		this.PostMetricData(userSession);
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
		Dictionary<string, object> userSession = this.GetUserSession();
		userSession.Add("EndSession", true);
		if (crashed)
		{
			userSession.Add("EndSessionCrashed", true);
		}
		if (this.shouldEndSession)
		{
			userSession.Add("HeartBeatTimeOut", true);
		}
		this.PostMetricData(userSession);
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
		Dictionary<string, object> userSession = this.GetUserSession();
		userSession.Add("NewGame", true);
		this.PostMetricData(userSession);
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
		Dictionary<string, object> userSession = this.GetUserSession();
		userSession.Add("EndGame", true);
		this.PostMetricData(userSession);
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
		Dictionary<string, object> userSession = this.GetUserSession();
		foreach (KeyValuePair<string, object> keyValuePair in eventData)
		{
			userSession.Add(keyValuePair.Key, keyValuePair.Value);
		}
		this.PostMetricData(userSession);
	}

	public bool SendProfileStats()
	{
		if (!this.enabled)
		{
			return false;
		}
		Dictionary<string, object> userSession = this.GetUserSession();
		return ThreadedHttps<KleiMetrics>.Instance.PostMetricData(userSession) == "OK";
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
				"GPUsupportsRenderTextures",
				SystemInfo.supportsRenderTextures
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
			},
			{
				"GPUsupportsStencil",
				SystemInfo.supportsStencil
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

	private const string GameTimeFieldName = "GameTimeSeconds";

	private const string LevelFieldName = "Level";

	private const string BuildBranchName = "Branch";

	private const string BuildFieldName = "Build";

	private const int EDITOR_BUILD_ID = -1;

	private const string HeartBeatFieldName = "HeartBeat";

	private const string HeartBeatTimeOutFieldName = "HeartBeatTimeOut";

	private const string LastUserActionFieldName = "LastUA";

	private string PlatformUserIDFieldName;

	private static int sessionID = -1;

	private static int gameID = -1;

	private static string installTimeStamp;

	private static global::System.Timers.Timer heartbeatTimer;

	private int HeartBeatInSeconds = 120;

	private int HeartBeatTimeOutInSeconds = 1200;

	private long currentSessionTicks;

	private float timeSinceLastUserAction;

	private long lastHeartBeatTick;

	private bool shouldEndSession;

	private bool shouldStartSession;

	private long startTime;

	private bool hasStarted;

	private bool sessionStarted;

	public struct PostData
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
