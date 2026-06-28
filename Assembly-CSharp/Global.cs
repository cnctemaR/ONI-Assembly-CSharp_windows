using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

public class Global : MonoBehaviour
{
	public static Global Instance { get; private set; }

	public Dictionary<string, Dictionary<string, object>> SystemInfo
	{
		get
		{
			return this.sysInfo;
		}
	}

	private unsafe void Awake()
	{
		Global.Instance = this;
		this.mInputManager = new GameInputManager();
		this.mCoroutineManager = base.gameObject.AddComponent<CoroutineManager>();
		this.mAnimEventManager = new AnimEventManager();
		KBatchedAnimUpdater.CreateInstance();
		SystemScheduler.Initialize();
		char* ptr = Sim.SYSINFO_Acquire();
		if (ptr != null)
		{
			string text = Marshal.PtrToStringAnsi((IntPtr)((void*)ptr));
			Console.WriteLine(text);
			this.sysInfo = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(text);
		}
		Sim.SYSINFO_Release();
		if (SteamManager.Initialized)
		{
			CSteamID steamID = SteamUser.GetSteamID();
			Debug.Log("Logged into steam with ID: " + steamID);
			ThreadedHttps<KleiAccount>.Instance.SendSteamTicket(new KleiAccount.GetUserIDdelegate(this.OnGetUserIdKey));
		}
		else
		{
			Debug.LogWarning("Cant init steam...");
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
		this.mInputManager.Update();
		SystemScheduler.instance.Update();
		this.mAnimEventManager.Update();
		if (this.gotKleiUserID)
		{
			this.gotKleiUserID = false;
			ThreadedHttps<KleiMetrics>.Instance.StartSession();
		}
		ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTick);
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
		ThreadedHttps<KleiMetrics>.Instance.EndSession(false);
	}

	private GameInputManager mInputManager;

	private CoroutineManager mCoroutineManager;

	private AnimEventManager mAnimEventManager;

	private Dictionary<string, Dictionary<string, object>> sysInfo;

	private bool gotKleiUserID;
}
