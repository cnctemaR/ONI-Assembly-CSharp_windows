using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class Global : MonoBehaviour
{
	public static Global Instance { get; private set; }

	private void Awake()
	{
		Global.Instance = this;
		this.mInputManager = new GameInputManager();
		this.mCoroutineManager = base.gameObject.AddComponent<CoroutineManager>();
		this.mAnimEventManager = new AnimEventManager();
		KBatchedAnimUpdater.CreateInstance();
		SystemScheduler.Initialize();
		if (DistributionPlatform.Initialized)
		{
			global::Debug.Log(string.Concat(new object[]
			{
				"Logged into ",
				DistributionPlatform.Inst.Name,
				" with ID:",
				DistributionPlatform.Inst.LocalUser.Id,
				", NAME:",
				DistributionPlatform.Inst.LocalUser.Name
			}), null);
			ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate(this.OnGetUserIdKey));
		}
		else
		{
			global::Debug.LogWarning("Can't init " + DistributionPlatform.Inst.Name + " distribution platform...", null);
			this.OnGetUserIdKey();
		}
	}

	private void Start()
	{
		base.StartCoroutine(this.mCoroutineManager.UpdateCoroutines());
	}

	public GameInputManager GetInputManager()
	{
		return this.mInputManager;
	}

	public CoroutineManager GetCoroutineManager()
	{
		return this.mCoroutineManager;
	}

	public AnimEventManager GetAnimEventManager()
	{
		return this.mAnimEventManager;
	}

	private void OnApplicationFocus(bool focus)
	{
		this.mInputManager.OnApplicationFocus(focus);
	}

	private void OnGetUserIdKey()
	{
		this.gotKleiUserID = true;
	}

	private void Update()
	{
		int num = KProfiler.BeginSampleI("Global.Update");
		this.mInputManager.Update();
		SystemScheduler.instance.Update();
		this.mAnimEventManager.Update();
		if (this.gotKleiUserID)
		{
			this.gotKleiUserID = false;
			ThreadedHttps<KleiMetrics>.Instance.SetCallBacks(new global::System.Action(this.SetONIStaticSessionVariables), new Action<Dictionary<string, object>>(this.SetONIDynamicSessionVariables));
			ThreadedHttps<KleiMetrics>.Instance.StartSession();
		}
		ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTicks);
		int num2 = KProfiler.EndSampleI();
	}

	private void SetONIStaticSessionVariables()
	{
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Branch", "release");
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Build", 219035U);
		if (PlayerPrefs.HasKey(UnitConfigurationScreen.MassUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.MassUnitKey, ((GameUtil.MassUnit)PlayerPrefs.GetInt(UnitConfigurationScreen.MassUnitKey)).ToString());
		}
		if (PlayerPrefs.HasKey(UnitConfigurationScreen.TemperatureUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.TemperatureUnitKey, ((GameUtil.TemperatureUnit)PlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey)).ToString());
		}
		PublishedFileId_t publishedFileId_t;
		string installedLanguageCode = SteamUGCService.Instance.GetInstalledLanguageCode(out publishedFileId_t);
		if (publishedFileId_t != PublishedFileId_t.Invalid)
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguagePackKey, publishedFileId_t.m_PublishedFileId);
		}
		if (installedLanguageCode != null && installedLanguageCode != string.Empty)
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguageCodeKey, installedLanguageCode);
		}
	}

	private void SetONIDynamicSessionVariables(Dictionary<string, object> data)
	{
		if (Game.Instance != null && GameClock.Instance != null)
		{
			data.Add("GameTimeSeconds", (int)GameClock.Instance.GetTime());
		}
	}

	private void LateUpdate()
	{
		if (Time.frameCount % 30 == 0)
		{
		}
		KBatchedAnimUpdater.instance.LateUpdate();
	}

	private void OnDestroy()
	{
		Global.Instance = null;
	}

	private void OnApplicationQuit()
	{
		KGlobalAnimParser.Destroy();
		ThreadedHttps<KleiMetrics>.Instance.EndSession(false);
	}

	private GameInputManager mInputManager;

	private CoroutineManager mCoroutineManager;

	private AnimEventManager mAnimEventManager;

	private bool gotKleiUserID;

	public static readonly string LanguagePackKey = "LanguagePack";

	public static readonly string LanguageCodeKey = "LanguageCode";
}
