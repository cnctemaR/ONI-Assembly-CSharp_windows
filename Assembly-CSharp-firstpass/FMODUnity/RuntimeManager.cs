using System;
using System.Collections.Generic;
using System.Text;
using FMOD;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("")]
	public class RuntimeManager : MonoBehaviour
	{
		public bool initializedSuccessfully { get; private set; }

		private static RuntimeManager Instance
		{
			get
			{
				if (RuntimeManager.initException != null)
				{
					throw RuntimeManager.initException;
				}
				if (RuntimeManager.isQuitting)
				{
					throw new Exception("FMOD Studio attempted access by script to RuntimeManager while application is quitting");
				}
				if (RuntimeManager.instance == null)
				{
					RESULT result = RESULT.OK;
					RuntimeManager runtimeManager = global::UnityEngine.Object.FindObjectOfType(typeof(RuntimeManager)) as RuntimeManager;
					if (runtimeManager != null && runtimeManager.cachedPointers[0] != 0L)
					{
						RuntimeManager.instance = runtimeManager;
						RuntimeManager.instance.studioSystem.handle = (IntPtr)RuntimeManager.instance.cachedPointers[0];
						RuntimeManager.instance.lowlevelSystem.handle = (IntPtr)RuntimeManager.instance.cachedPointers[1];
						return RuntimeManager.instance;
					}
					GameObject gameObject = new GameObject("FMOD.UnityIntegration.RuntimeManager");
					RuntimeManager.instance = gameObject.AddComponent<RuntimeManager>();
					global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					try
					{
						RuntimeUtils.EnforceLibraryOrder();
						result = RuntimeManager.instance.Initialize();
					}
					catch (Exception ex)
					{
						RuntimeManager.initException = ex as SystemNotInitializedException;
						if (RuntimeManager.initException == null)
						{
							RuntimeManager.initException = new SystemNotInitializedException(ex);
						}
						throw RuntimeManager.initException;
					}
					if (result != RESULT.OK)
					{
						throw new SystemNotInitializedException(result, "Output forced to NO SOUND mode");
					}
				}
				return RuntimeManager.instance;
			}
		}

		public static global::FMOD.Studio.System StudioSystem
		{
			get
			{
				return RuntimeManager.Instance.studioSystem;
			}
		}

		public static global::FMOD.System LowlevelSystem
		{
			get
			{
				return RuntimeManager.Instance.lowlevelSystem;
			}
		}

		private void CheckInitResult(RESULT result, string cause)
		{
			if (result != RESULT.OK)
			{
				if (this.studioSystem.isValid())
				{
					this.studioSystem.release();
					this.studioSystem.clearHandle();
				}
				throw new SystemNotInitializedException(result, cause);
			}
		}

		private RESULT Initialize()
		{
			this.initializedSuccessfully = false;
			RESULT result = RESULT.OK;
			Settings settings = Settings.Instance;
			this.fmodPlatform = RuntimeUtils.GetCurrentPlatform();
			int sampleRate = settings.GetSampleRate(this.fmodPlatform);
			int num = Math.Min(settings.GetRealChannels(this.fmodPlatform), 256);
			int virtualChannels = settings.GetVirtualChannels(this.fmodPlatform);
			SPEAKERMODE speakerMode = (SPEAKERMODE)settings.GetSpeakerMode(this.fmodPlatform);
			OUTPUTTYPE outputtype = OUTPUTTYPE.AUTODETECT;
			global::FMOD.ADVANCEDSETTINGS advancedsettings = default(global::FMOD.ADVANCEDSETTINGS);
			advancedsettings.randomSeed = (uint)DateTime.Now.Ticks;
			advancedsettings.maxVorbisCodecs = num;
			global::FMOD.Studio.INITFLAGS initflags = global::FMOD.Studio.INITFLAGS.DEFERRED_CALLBACKS;
			if (settings.IsLiveUpdateEnabled(this.fmodPlatform))
			{
				initflags |= global::FMOD.Studio.INITFLAGS.LIVEUPDATE;
			}
			RESULT result2;
			for (;;)
			{
				result2 = global::FMOD.Studio.System.create(out this.studioSystem);
				this.CheckInitResult(result2, "FMOD.Studio.System.create");
				result2 = this.studioSystem.getLowLevelSystem(out this.lowlevelSystem);
				this.CheckInitResult(result2, "FMOD.Studio.System.getLowLevelSystem");
				result2 = this.lowlevelSystem.setOutput(outputtype);
				this.CheckInitResult(result2, "FMOD.System.setOutput");
				result2 = this.lowlevelSystem.setSoftwareChannels(num);
				this.CheckInitResult(result2, "FMOD.System.setSoftwareChannels");
				result2 = this.lowlevelSystem.setSoftwareFormat(sampleRate, speakerMode, 0);
				this.CheckInitResult(result2, "FMOD.System.setSoftwareFormat");
				result2 = this.lowlevelSystem.setAdvancedSettings(ref advancedsettings);
				this.CheckInitResult(result2, "FMOD.System.setAdvancedSettings");
				result2 = this.studioSystem.initialize(virtualChannels, initflags, global::FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
				if (result2 != RESULT.OK && result == RESULT.OK)
				{
					result = result2;
					outputtype = OUTPUTTYPE.NOSOUND;
					global::Debug.LogWarningFormat("FMOD Studio: Studio::System::initialize returned {0}, defaulting to no-sound mode.", new object[] { result2.ToString() });
				}
				else
				{
					this.CheckInitResult(result2, "Studio::System::initialize");
					if ((initflags & global::FMOD.Studio.INITFLAGS.LIVEUPDATE) == global::FMOD.Studio.INITFLAGS.NORMAL)
					{
						break;
					}
					this.studioSystem.flushCommands();
					result2 = this.studioSystem.update();
					if (result2 != RESULT.ERR_NET_SOCKET_ERROR)
					{
						break;
					}
					initflags &= ~global::FMOD.Studio.INITFLAGS.LIVEUPDATE;
					global::Debug.LogWarning("FMOD Studio: Cannot open network port for Live Update (in-use), restarting with Live Update disabled.");
					result2 = this.studioSystem.release();
					this.CheckInitResult(result2, "FMOD.Studio.System.Release");
				}
			}
			this.LoadPlugins(settings);
			this.LoadBanks(settings);
			this.initializedSuccessfully = result2 == RESULT.OK;
			return result;
		}

		private void Update()
		{
			if (this.studioSystem.isValid() && RuntimeManager.IsInitialized)
			{
				this.studioSystem.update();
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				for (int i = 7; i >= 0; i--)
				{
					if (!flag && RuntimeManager.HasListener[i])
					{
						num = i + 1;
						flag = true;
						flag2 = true;
					}
					if (!RuntimeManager.HasListener[i] && flag)
					{
						flag2 = false;
					}
				}
				if (flag)
				{
					this.studioSystem.setNumListeners(num);
				}
				if (!flag2 && !this.listenerWarningIssued)
				{
					this.listenerWarningIssued = true;
				}
				for (int j = 0; j < this.attachedInstances.Count; j++)
				{
					PLAYBACK_STATE playback_STATE = PLAYBACK_STATE.STOPPED;
					this.attachedInstances[j].instance.getPlaybackState(out playback_STATE);
					if (!this.attachedInstances[j].instance.isValid() || playback_STATE == PLAYBACK_STATE.STOPPED || this.attachedInstances[j].transform == null)
					{
						this.attachedInstances.RemoveAt(j);
						j--;
					}
					else if (this.attachedInstances[j].rigidBody)
					{
						this.attachedInstances[j].instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.attachedInstances[j].transform, this.attachedInstances[j].rigidBody));
					}
					else
					{
						this.attachedInstances[j].instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.attachedInstances[j].transform, this.attachedInstances[j].rigidBody2D));
					}
				}
			}
		}

		public static void AttachInstanceToGameObject(EventInstance instance, Transform transform, Rigidbody rigidBody)
		{
			RuntimeManager.AttachedInstance attachedInstance = new RuntimeManager.AttachedInstance();
			attachedInstance.transform = transform;
			attachedInstance.instance = instance;
			attachedInstance.rigidBody = rigidBody;
			RuntimeManager.Instance.attachedInstances.Add(attachedInstance);
		}

		public static void AttachInstanceToGameObject(EventInstance instance, Transform transform, Rigidbody2D rigidBody2D)
		{
			RuntimeManager.AttachedInstance attachedInstance = new RuntimeManager.AttachedInstance();
			attachedInstance.transform = transform;
			attachedInstance.instance = instance;
			attachedInstance.rigidBody2D = rigidBody2D;
			attachedInstance.rigidBody = null;
			RuntimeManager.Instance.attachedInstances.Add(attachedInstance);
		}

		public static void DetachInstanceFromGameObject(EventInstance instance)
		{
			RuntimeManager runtimeManager = RuntimeManager.Instance;
			for (int i = 0; i < runtimeManager.attachedInstances.Count; i++)
			{
				if (runtimeManager.attachedInstances[i].instance.handle == instance.handle)
				{
					runtimeManager.attachedInstances.RemoveAt(i);
					return;
				}
			}
		}

		private void DrawDebugOverlay(int windowID)
		{
			if (this.lastDebugUpdate + 0.25f < Time.unscaledTime)
			{
				if (RuntimeManager.initException != null)
				{
					this.lastDebugText = RuntimeManager.initException.Message;
				}
				else
				{
					if (!this.mixerHead.hasHandle())
					{
						ChannelGroup channelGroup;
						this.lowlevelSystem.getMasterChannelGroup(out channelGroup);
						channelGroup.getDSP(0, out this.mixerHead);
						this.mixerHead.setMeteringEnabled(false, true);
					}
					StringBuilder stringBuilder = new StringBuilder();
					CPU_USAGE cpu_USAGE;
					this.studioSystem.getCPUUsage(out cpu_USAGE);
					stringBuilder.AppendFormat("CPU: dsp = {0:F1}%, studio = {1:F1}%\n", cpu_USAGE.dspusage, cpu_USAGE.studiousage);
					int num;
					int num2;
					Memory.GetStats(out num, out num2);
					stringBuilder.AppendFormat("MEMORY: cur = {0}MB, max = {1}MB\n", num >> 20, num2 >> 20);
					int num3;
					int num4;
					this.lowlevelSystem.getChannelsPlaying(out num3, out num4);
					stringBuilder.AppendFormat("CHANNELS: real = {0}, total = {1}\n", num4, num3);
					DSP_METERING_INFO dsp_METERING_INFO;
					this.mixerHead.getMeteringInfo(IntPtr.Zero, out dsp_METERING_INFO);
					float num5 = 0f;
					for (int i = 0; i < (int)dsp_METERING_INFO.numchannels; i++)
					{
						num5 += dsp_METERING_INFO.rmslevel[i] * dsp_METERING_INFO.rmslevel[i];
					}
					num5 = Mathf.Sqrt(num5 / (float)dsp_METERING_INFO.numchannels);
					float num6 = ((num5 > 0f) ? (20f * Mathf.Log10(num5 * Mathf.Sqrt(2f))) : (-80f));
					if (num6 > 10f)
					{
						num6 = 10f;
					}
					stringBuilder.AppendFormat("VOLUME: RMS = {0:f2}db\n", num6);
					this.lastDebugText = stringBuilder.ToString();
					this.lastDebugUpdate = Time.unscaledTime;
				}
			}
			GUI.Label(new Rect(10f, 20f, 290f, 100f), this.lastDebugText);
			GUI.DragWindow();
		}

		private void OnDisable()
		{
			this.cachedPointers[0] = (long)this.studioSystem.handle;
			this.cachedPointers[1] = (long)this.lowlevelSystem.handle;
		}

		private void OnDestroy()
		{
			if (this.studioSystem.isValid())
			{
				this.studioSystem.release();
				this.studioSystem.clearHandle();
			}
			RuntimeManager.initException = null;
			RuntimeManager.instance = null;
			RuntimeManager.isQuitting = true;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (this.studioSystem.isValid())
			{
				if (this.loadedBanks.Count > 1)
				{
					RuntimeManager.PauseAllEvents(pauseStatus);
				}
				if (pauseStatus)
				{
					this.lowlevelSystem.mixerSuspend();
					return;
				}
				this.lowlevelSystem.mixerResume();
			}
		}

		private void loadedBankRegister(RuntimeManager.LoadedBank loadedBank, string bankPath, string bankName, bool loadSamples, RESULT loadResult)
		{
			if (loadResult == RESULT.OK)
			{
				loadedBank.RefCount = 1;
				if (loadSamples)
				{
					loadedBank.Bank.loadSampleData();
				}
				RuntimeManager.Instance.loadedBanks.Add(bankName, loadedBank);
				return;
			}
			if (loadResult == RESULT.ERR_EVENT_ALREADY_LOADED)
			{
				loadedBank.RefCount = 2;
				RuntimeManager.Instance.loadedBanks.Add(bankName, loadedBank);
				return;
			}
			throw new BankLoadException(bankPath, loadResult);
		}

		public static void LoadBank(string bankName, bool loadSamples = false)
		{
			if (RuntimeManager.Instance.loadedBanks.ContainsKey(bankName))
			{
				RuntimeManager.LoadedBank loadedBank = RuntimeManager.Instance.loadedBanks[bankName];
				loadedBank.RefCount++;
				if (loadSamples)
				{
					loadedBank.Bank.loadSampleData();
				}
				RuntimeManager.Instance.loadedBanks[bankName] = loadedBank;
				return;
			}
			string bankPath = RuntimeUtils.GetBankPath(bankName);
			RuntimeManager.LoadedBank loadedBank2 = default(RuntimeManager.LoadedBank);
			RESULT result = RuntimeManager.Instance.studioSystem.loadBankFile(bankPath, LOAD_BANK_FLAGS.NORMAL, out loadedBank2.Bank);
			RuntimeManager.Instance.loadedBankRegister(loadedBank2, bankPath, bankName, loadSamples, result);
		}

		public static void LoadBank(TextAsset asset, bool loadSamples = false)
		{
			string name = asset.name;
			if (RuntimeManager.Instance.loadedBanks.ContainsKey(name))
			{
				RuntimeManager.LoadedBank loadedBank = RuntimeManager.Instance.loadedBanks[name];
				loadedBank.RefCount++;
				if (loadSamples)
				{
					loadedBank.Bank.loadSampleData();
					return;
				}
			}
			else
			{
				RuntimeManager.LoadedBank loadedBank2 = default(RuntimeManager.LoadedBank);
				RESULT result = RuntimeManager.Instance.studioSystem.loadBankMemory(asset.bytes, LOAD_BANK_FLAGS.NORMAL, out loadedBank2.Bank);
				if (result == RESULT.OK)
				{
					loadedBank2.RefCount = 1;
					RuntimeManager.Instance.loadedBanks.Add(name, loadedBank2);
					if (loadSamples)
					{
						loadedBank2.Bank.loadSampleData();
						return;
					}
				}
				else
				{
					if (result == RESULT.ERR_EVENT_ALREADY_LOADED)
					{
						loadedBank2.RefCount = 2;
						RuntimeManager.Instance.loadedBanks.Add(name, loadedBank2);
						return;
					}
					throw new BankLoadException(name, result);
				}
			}
		}

		private void LoadBanks(Settings fmodSettings)
		{
			if (fmodSettings.ImportType == ImportType.StreamingAssets)
			{
				try
				{
					RuntimeManager.LoadBank(fmodSettings.MasterBank + ".strings", fmodSettings.AutomaticSampleLoading);
					if (fmodSettings.AutomaticEventLoading)
					{
						RuntimeManager.LoadBank(fmodSettings.MasterBank, fmodSettings.AutomaticSampleLoading);
						foreach (string text in fmodSettings.Banks)
						{
							RuntimeManager.LoadBank(text, fmodSettings.AutomaticSampleLoading);
						}
						RuntimeManager.WaitForAllLoads();
					}
				}
				catch (BankLoadException ex)
				{
					global::Debug.LogException(ex);
				}
			}
		}

		public static void UnloadBank(string bankName)
		{
			RuntimeManager.LoadedBank loadedBank;
			if (RuntimeManager.Instance.loadedBanks.TryGetValue(bankName, out loadedBank))
			{
				loadedBank.RefCount--;
				if (loadedBank.RefCount == 0)
				{
					loadedBank.Bank.unload();
					RuntimeManager.Instance.loadedBanks.Remove(bankName);
					return;
				}
				RuntimeManager.Instance.loadedBanks[bankName] = loadedBank;
			}
		}

		public static bool AnyBankLoading()
		{
			bool flag = false;
			foreach (RuntimeManager.LoadedBank loadedBank in RuntimeManager.Instance.loadedBanks.Values)
			{
				Bank bank = loadedBank.Bank;
				LOADING_STATE loading_STATE;
				bank.getSampleLoadingState(out loading_STATE);
				flag |= loading_STATE == LOADING_STATE.LOADING;
			}
			return flag;
		}

		public static void WaitForAllLoads()
		{
			RuntimeManager.Instance.studioSystem.flushSampleLoading();
		}

		public static Guid PathToGUID(string path)
		{
			Guid empty = Guid.Empty;
			if (path.StartsWith("{"))
			{
				global::FMOD.Studio.Util.ParseID(path, out empty);
			}
			else if (RuntimeManager.Instance.studioSystem.lookupID(path, out empty) == RESULT.ERR_EVENT_NOTFOUND)
			{
				throw new EventNotFoundException(path);
			}
			return empty;
		}

		public static EventInstance CreateInstance(string path)
		{
			EventInstance eventInstance;
			try
			{
				eventInstance = RuntimeManager.CreateInstance(RuntimeManager.PathToGUID(path));
			}
			catch (EventNotFoundException)
			{
				throw new EventNotFoundException(path);
			}
			return eventInstance;
		}

		public static EventInstance CreateInstance(Guid guid)
		{
			EventInstance eventInstance;
			RuntimeManager.GetEventDescription(guid).createInstance(out eventInstance);
			return eventInstance;
		}

		public static void PlayOneShot(string path, Vector3 position = default(Vector3))
		{
			try
			{
				RuntimeManager.PlayOneShot(RuntimeManager.PathToGUID(path), position);
			}
			catch (EventNotFoundException)
			{
				global::Debug.LogWarning("FMOD Event not found: " + path);
			}
		}

		public static void PlayOneShot(Guid guid, Vector3 position = default(Vector3))
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(guid);
			eventInstance.set3DAttributes(position.To3DAttributes());
			eventInstance.start();
			eventInstance.release();
		}

		public static void PlayOneShotAttached(string path, GameObject gameObject)
		{
			try
			{
				RuntimeManager.PlayOneShotAttached(RuntimeManager.PathToGUID(path), gameObject);
			}
			catch (EventNotFoundException)
			{
				global::Debug.LogWarning("FMOD Event not found: " + path);
			}
		}

		public static void PlayOneShotAttached(Guid guid, GameObject gameObject)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(guid);
			RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject.transform, gameObject.GetComponent<Rigidbody>());
			eventInstance.start();
			eventInstance.release();
		}

		public static EventDescription GetEventDescription(string path)
		{
			EventDescription eventDescription;
			try
			{
				eventDescription = RuntimeManager.GetEventDescription(RuntimeManager.PathToGUID(path));
			}
			catch (EventNotFoundException)
			{
				throw new EventNotFoundException(path);
			}
			return eventDescription;
		}

		public static EventDescription GetEventDescription(Guid guid)
		{
			EventDescription eventDescription;
			if (RuntimeManager.Instance.cachedDescriptions.ContainsKey(guid) && RuntimeManager.Instance.cachedDescriptions[guid].isValid())
			{
				eventDescription = RuntimeManager.Instance.cachedDescriptions[guid];
			}
			else
			{
				if (RuntimeManager.Instance.studioSystem.getEventByID(guid, out eventDescription) != RESULT.OK)
				{
					throw new EventNotFoundException(guid);
				}
				if (eventDescription.isValid())
				{
					RuntimeManager.Instance.cachedDescriptions[guid] = eventDescription;
				}
			}
			return eventDescription;
		}

		public static void SetListenerLocation(GameObject gameObject, Rigidbody rigidBody = null)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(0, RuntimeUtils.To3DAttributes(gameObject, rigidBody));
		}

		public static void SetListenerLocation(GameObject gameObject, Rigidbody2D rigidBody2D)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(0, RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
		}

		public static void SetListenerLocation(Transform transform)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(0, transform.To3DAttributes());
		}

		public static void SetListenerLocation(int listenerIndex, GameObject gameObject, Rigidbody rigidBody = null)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, RuntimeUtils.To3DAttributes(gameObject, rigidBody));
		}

		public static void SetListenerLocation(int listenerIndex, GameObject gameObject, Rigidbody2D rigidBody2D)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
		}

		public static void SetListenerLocation(int listenerIndex, Transform transform)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, transform.To3DAttributes());
		}

		public static Bus GetBus(string path)
		{
			Bus bus;
			if (RuntimeManager.StudioSystem.getBus(path, out bus) != RESULT.OK)
			{
				throw new BusNotFoundException(path);
			}
			return bus;
		}

		public static VCA GetVCA(string path)
		{
			VCA vca;
			if (RuntimeManager.StudioSystem.getVCA(path, out vca) != RESULT.OK)
			{
				throw new VCANotFoundException(path);
			}
			return vca;
		}

		public static void PauseAllEvents(bool paused)
		{
			RuntimeManager.GetBus("bus:/").setPaused(paused);
		}

		public static void MuteAllEvents(bool muted)
		{
			RuntimeManager.GetBus("bus:/").setMute(muted);
		}

		public static bool IsInitialized
		{
			get
			{
				return RuntimeManager.instance != null && RuntimeManager.instance.studioSystem.isValid() && RuntimeManager.instance.initializedSuccessfully;
			}
		}

		public static bool HasBanksLoaded
		{
			get
			{
				return RuntimeManager.Instance.loadedBanks.Count > 1;
			}
		}

		public static bool HasBankLoaded(string loadedBank)
		{
			return RuntimeManager.instance.loadedBanks.ContainsKey(loadedBank);
		}

		private void LoadPlugins(Settings fmodSettings)
		{
			foreach (string text in fmodSettings.Plugins)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string pluginPath = RuntimeUtils.GetPluginPath(text);
					uint num;
					RESULT result = this.lowlevelSystem.loadPlugin(pluginPath, out num);
					if (result == RESULT.ERR_FILE_BAD || result == RESULT.ERR_FILE_NOTFOUND)
					{
						string pluginPath2 = RuntimeUtils.GetPluginPath(text + "64");
						result = this.lowlevelSystem.loadPlugin(pluginPath2, out num);
					}
					this.CheckInitResult(result, string.Format("Loading plugin '{0}' from '{1}'", text, pluginPath));
					this.loadedPlugins.Add(text, num);
				}
			}
		}

		private static SystemNotInitializedException initException = null;

		private static RuntimeManager instance;

		private static bool isQuitting = false;

		[SerializeField]
		private FMODPlatform fmodPlatform;

		private global::FMOD.Studio.System studioSystem;

		private global::FMOD.System lowlevelSystem;

		private DSP mixerHead;

		[SerializeField]
		private long[] cachedPointers = new long[2];

		private Dictionary<string, RuntimeManager.LoadedBank> loadedBanks = new Dictionary<string, RuntimeManager.LoadedBank>();

		private Dictionary<string, uint> loadedPlugins = new Dictionary<string, uint>();

		private Dictionary<Guid, EventDescription> cachedDescriptions = new Dictionary<Guid, EventDescription>(new RuntimeManager.GuidComparer());

		private List<RuntimeManager.AttachedInstance> attachedInstances = new List<RuntimeManager.AttachedInstance>(128);

		private bool listenerWarningIssued;

		private string lastDebugText;

		private float lastDebugUpdate;

		public static bool[] HasListener = new bool[8];

		private struct LoadedBank
		{
			public Bank Bank;

			public int RefCount;
		}

		private class GuidComparer : IEqualityComparer<Guid>
		{
			bool IEqualityComparer<Guid>.Equals(Guid x, Guid y)
			{
				return x.Equals(y);
			}

			int IEqualityComparer<Guid>.GetHashCode(Guid obj)
			{
				return obj.GetHashCode();
			}
		}

		private class AttachedInstance
		{
			public EventInstance instance;

			public Transform transform;

			public Rigidbody rigidBody;

			public Rigidbody2D rigidBody2D;
		}
	}
}
