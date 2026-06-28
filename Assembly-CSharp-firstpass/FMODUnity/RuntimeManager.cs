using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using FMOD;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("")]
	public class RuntimeManager : MonoBehaviour
	{
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
					RuntimeManager runtimeManager = global::UnityEngine.Object.FindObjectOfType(typeof(RuntimeManager)) as RuntimeManager;
					if (runtimeManager != null && runtimeManager.cachedPointers[0] != 0L)
					{
						RuntimeManager.instance = runtimeManager;
						RuntimeManager.instance.studioSystem = new global::FMOD.Studio.System((IntPtr)RuntimeManager.instance.cachedPointers[0]);
						RuntimeManager.instance.lowlevelSystem = new global::FMOD.System((IntPtr)RuntimeManager.instance.cachedPointers[1]);
						RuntimeManager.instance.mixerHead = new DSP((IntPtr)RuntimeManager.instance.cachedPointers[2]);
						return RuntimeManager.instance;
					}
					GameObject gameObject = new GameObject("FMOD.UnityItegration.RuntimeManager");
					RuntimeManager.instance = gameObject.AddComponent<RuntimeManager>();
					global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					try
					{
						RuntimeUtils.EnforceLibraryOrder();
						RuntimeManager.instance.Initialiase(false);
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
				if (this.studioSystem != null)
				{
					this.studioSystem.release();
					this.studioSystem = null;
				}
				throw new SystemNotInitializedException(result, cause);
			}
		}

		private void Initialiase(bool forceNoNetwork)
		{
			global::UnityEngine.Debug.Log("FMOD Studio: Creating runtime system instance");
			RESULT result = global::FMOD.Studio.System.create(out this.studioSystem);
			this.CheckInitResult(result, "Creating System Object");
			this.studioSystem.getLowLevelSystem(out this.lowlevelSystem);
			Settings settings = Settings.Instance;
			this.fmodPlatform = RuntimeUtils.GetCurrentPlatform();
			int realChannels = settings.GetRealChannels(this.fmodPlatform);
			result = this.lowlevelSystem.setSoftwareChannels(realChannels);
			this.CheckInitResult(result, "Set software channels");
			result = this.lowlevelSystem.setSoftwareFormat(settings.GetSampleRate(this.fmodPlatform), (SPEAKERMODE)settings.GetSpeakerMode(this.fmodPlatform), 0);
			this.CheckInitResult(result, "Set software format");
			global::FMOD.ADVANCEDSETTINGS advancedsettings = default(global::FMOD.ADVANCEDSETTINGS);
			advancedsettings.maxVorbisCodecs = realChannels;
			advancedsettings.randomSeed = (uint)DateTime.Now.Ticks;
			result = this.lowlevelSystem.setAdvancedSettings(ref advancedsettings);
			this.CheckInitResult(result, "Set advanced settings");
			global::FMOD.INITFLAGS initflags = global::FMOD.INITFLAGS.NORMAL;
			global::FMOD.Studio.INITFLAGS initflags2 = global::FMOD.Studio.INITFLAGS.DEFERRED_CALLBACKS;
			if (settings.IsLiveUpdateEnabled(this.fmodPlatform) && !forceNoNetwork)
			{
				initflags2 |= global::FMOD.Studio.INITFLAGS.LIVEUPDATE;
			}
			RESULT result2 = this.studioSystem.initialize(settings.GetVirtualChannels(this.fmodPlatform), initflags2, initflags, IntPtr.Zero);
			this.CheckInitResult(result2, "Calling initialize");
			this.studioSystem.flushCommands();
			RESULT result3 = this.studioSystem.update();
			if (result3 == RESULT.ERR_NET_SOCKET_ERROR)
			{
				this.studioSystem.release();
				global::UnityEngine.Debug.LogWarning("FMOD Studio: Cannot open network port for Live Update, restarting with Live Update disabled. Check for other applications that are running FMOD Studio");
				this.Initialiase(true);
			}
			else
			{
				foreach (string text in settings.Plugins)
				{
					string text2 = RuntimeUtils.GetPluginPath(text);
					uint num;
					result = this.lowlevelSystem.loadPlugin(text2, out num);
					if (result == RESULT.ERR_FILE_BAD || result == RESULT.ERR_FILE_NOTFOUND)
					{
						text2 = RuntimeUtils.GetPluginPath(text + "64");
						result = this.lowlevelSystem.loadPlugin(text2, out num);
					}
					this.CheckInitResult(result, string.Format("Loading plugin '{0}' from '{1}'", text, text2));
					this.loadedPlugins.Add(text, num);
				}
				try
				{
					RuntimeManager.LoadBank(settings.MasterBank + ".strings", settings.AutomaticSampleLoading);
				}
				catch (BankLoadException ex)
				{
					global::UnityEngine.Debug.LogException(ex);
				}
				if (settings.AutomaticEventLoading)
				{
					try
					{
						RuntimeManager.LoadBank(settings.MasterBank, settings.AutomaticSampleLoading);
					}
					catch (BankLoadException ex2)
					{
						global::UnityEngine.Debug.LogException(ex2);
					}
					foreach (string text3 in settings.Banks)
					{
						try
						{
							RuntimeManager.LoadBank(text3, settings.AutomaticSampleLoading);
						}
						catch (BankLoadException ex3)
						{
							global::UnityEngine.Debug.LogException(ex3);
						}
					}
				}
			}
			ChannelGroup channelGroup;
			this.lowlevelSystem.getMasterChannelGroup(out channelGroup);
			channelGroup.getDSP(0, out this.mixerHead);
			this.mixerHead.setMeteringEnabled(false, true);
		}

		private void Update()
		{
			if (this.studioSystem != null)
			{
				this.studioSystem.update();
				if (!RuntimeManager.hasListener && !this.listenerWarningIssued)
				{
					this.listenerWarningIssued = true;
				}
				for (int i = 0; i < this.attachedInstances.Count; i++)
				{
					PLAYBACK_STATE playback_STATE = PLAYBACK_STATE.STOPPED;
					this.attachedInstances[i].instance.getPlaybackState(out playback_STATE);
					if (!this.attachedInstances[i].instance.isValid() || playback_STATE == PLAYBACK_STATE.STOPPED || this.attachedInstances[i].transform == null)
					{
						this.attachedInstances.RemoveAt(i);
						i--;
					}
					else
					{
						this.attachedInstances[i].instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.attachedInstances[i].transform, this.attachedInstances[i].rigidBody));
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

		public static void DetachInstanceFromGameObject(EventInstance instance)
		{
			RuntimeManager runtimeManager = RuntimeManager.Instance;
			for (int i = 0; i < runtimeManager.attachedInstances.Count; i++)
			{
				if (runtimeManager.attachedInstances[i].instance == instance)
				{
					runtimeManager.attachedInstances.RemoveAt(i);
					return;
				}
			}
		}

		private void OnGUI()
		{
			if (this.studioSystem != null && Settings.Instance.IsOverlayEnabled(this.fmodPlatform))
			{
				this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.DrawDebugOverlay), "FMOD Studio Debug");
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
					StringBuilder stringBuilder = new StringBuilder();
					CPU_USAGE cpu_USAGE;
					this.studioSystem.getCPUUsage(out cpu_USAGE);
					stringBuilder.AppendFormat("CPU: dsp = {0:F1}%, studio = {1:F1}%\n", cpu_USAGE.dspUsage, cpu_USAGE.studioUsage);
					int num;
					int num2;
					Memory.GetStats(out num, out num2);
					stringBuilder.AppendFormat("MEMORY: cur = {0}MB, max = {1}MB\n", num >> 20, num2 >> 20);
					int num3;
					int num4;
					this.lowlevelSystem.getChannelsPlaying(out num3, out num4);
					stringBuilder.AppendFormat("CHANNELS: real = {0}, total = {1}\n", num4, num3);
					DSP_METERING_INFO dsp_METERING_INFO = new DSP_METERING_INFO();
					this.mixerHead.getMeteringInfo(null, dsp_METERING_INFO);
					float num5 = 0f;
					for (int i = 0; i < (int)dsp_METERING_INFO.numchannels; i++)
					{
						num5 += dsp_METERING_INFO.rmslevel[i] * dsp_METERING_INFO.rmslevel[i];
					}
					num5 = Mathf.Sqrt(num5 / (float)dsp_METERING_INFO.numchannels);
					float num6 = ((num5 <= 0f) ? (-80f) : (20f * Mathf.Log10(num5 * Mathf.Sqrt(2f))));
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
			this.cachedPointers[0] = (long)this.studioSystem.getRaw();
			this.cachedPointers[1] = (long)this.lowlevelSystem.getRaw();
			this.cachedPointers[2] = (long)this.mixerHead.getRaw();
		}

		private void OnDestroy()
		{
			if (this.studioSystem != null)
			{
				global::UnityEngine.Debug.Log("FMOD Studio: Destroying runtime system instance");
				this.studioSystem.release();
				this.studioSystem = null;
			}
			RuntimeManager.initException = null;
			RuntimeManager.instance = null;
			RuntimeManager.isQuitting = true;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (this.studioSystem != null && this.studioSystem.isValid())
			{
				if (pauseStatus)
				{
					this.lowlevelSystem.mixerSuspend();
				}
				else
				{
					this.lowlevelSystem.mixerResume();
				}
			}
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
			}
			else
			{
				RuntimeManager.LoadedBank loadedBank2 = default(RuntimeManager.LoadedBank);
				string bankPath = RuntimeUtils.GetBankPath(bankName);
				RESULT result = RuntimeManager.Instance.studioSystem.loadBankFile(bankPath, LOAD_BANK_FLAGS.NORMAL, out loadedBank2.Bank);
				if (result == RESULT.OK)
				{
					loadedBank2.RefCount = 1;
					RuntimeManager.Instance.loadedBanks.Add(bankName, loadedBank2);
					if (loadSamples)
					{
						loadedBank2.Bank.loadSampleData();
					}
				}
				else
				{
					if (result != RESULT.ERR_EVENT_ALREADY_LOADED)
					{
						throw new BankLoadException(bankPath, result);
					}
					loadedBank2.RefCount = 2;
					RuntimeManager.Instance.loadedBanks.Add(bankName, loadedBank2);
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
				}
			}
		}

		public static Guid PathToGUID(string path)
		{
			Guid empty = Guid.Empty;
			if (path.StartsWith("{"))
			{
				global::FMOD.Studio.Util.ParseID(path, out empty);
			}
			else
			{
				RESULT result = RuntimeManager.Instance.studioSystem.lookupID(path, out empty);
				if (result == RESULT.ERR_EVENT_NOTFOUND)
				{
					throw new EventNotFoundException(path);
				}
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
			EventDescription eventDescription = RuntimeManager.GetEventDescription(guid);
			EventInstance eventInstance;
			eventDescription.createInstance(out eventInstance);
			return eventInstance;
		}

		public static void PlayOneShot(string path, [Optional] Vector3 position)
		{
			try
			{
				RuntimeManager.PlayOneShot(RuntimeManager.PathToGUID(path), position);
			}
			catch (EventNotFoundException)
			{
				throw new EventNotFoundException(path);
			}
		}

		public static void PlayOneShot(Guid guid, [Optional] Vector3 position)
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
				throw new EventNotFoundException(path);
			}
		}

		public static void PlayOneShotAttached(Guid guid, GameObject gameObject)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(guid);
			RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject.transform, gameObject.GetComponent<Rigidbody>());
			eventInstance.start();
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
			EventDescription eventDescription = null;
			if (RuntimeManager.Instance.cachedDescriptions.ContainsKey(guid) && RuntimeManager.Instance.cachedDescriptions[guid].isValid())
			{
				eventDescription = RuntimeManager.Instance.cachedDescriptions[guid];
			}
			else
			{
				RESULT eventByID = RuntimeManager.Instance.studioSystem.getEventByID(guid, out eventDescription);
				if (eventByID != RESULT.OK)
				{
					throw new EventNotFoundException(guid);
				}
				if (eventDescription != null && eventDescription.isValid())
				{
					RuntimeManager.Instance.cachedDescriptions[guid] = eventDescription;
				}
			}
			return eventDescription;
		}

		public static bool HasListener
		{
			get
			{
				return RuntimeManager.hasListener;
			}
			set
			{
				RuntimeManager.hasListener = value;
			}
		}

		public static void SetListenerLocation(GameObject gameObject, Rigidbody rigidBody = null)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(0, RuntimeUtils.To3DAttributes(gameObject, rigidBody));
		}

		public static void SetListenerLocation(Transform transform)
		{
			RuntimeManager.Instance.studioSystem.setListenerAttributes(0, transform.To3DAttributes());
		}

		public static Bus GetBus(string path)
		{
			Bus bus2;
			RESULT bus = RuntimeManager.StudioSystem.getBus(path, out bus2);
			if (bus != RESULT.OK)
			{
			}
			return bus2;
		}

		public static VCA GetVCA(string path)
		{
			VCA vca2;
			RESULT vca = RuntimeManager.StudioSystem.getVCA(path, out vca2);
			if (vca != RESULT.OK)
			{
			}
			return vca2;
		}

		private static SystemNotInitializedException initException;

		private static RuntimeManager instance;

		private static bool isQuitting;

		[SerializeField]
		private FMODPlatform fmodPlatform;

		private global::FMOD.Studio.System studioSystem;

		private global::FMOD.System lowlevelSystem;

		private DSP mixerHead;

		[SerializeField]
		private long[] cachedPointers = new long[3];

		private Dictionary<string, RuntimeManager.LoadedBank> loadedBanks = new Dictionary<string, RuntimeManager.LoadedBank>();

		private Dictionary<string, uint> loadedPlugins = new Dictionary<string, uint>();

		private Dictionary<Guid, EventDescription> cachedDescriptions = new Dictionary<Guid, EventDescription>(new RuntimeManager.GuidComparer());

		private List<RuntimeManager.AttachedInstance> attachedInstances = new List<RuntimeManager.AttachedInstance>(128);

		private bool listenerWarningIssued;

		private Rect windowRect = new Rect(10f, 10f, 300f, 100f);

		private string lastDebugText;

		private float lastDebugUpdate;

		private static bool hasListener;

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
		}
	}
}
