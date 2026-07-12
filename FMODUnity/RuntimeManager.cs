using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AOT;
using FMOD;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("")]
	public class RuntimeManager : MonoBehaviour
	{
		public static bool IsMuted
		{
			get
			{
				return RuntimeManager.Instance.isMuted;
			}
		}

		[MonoPInvokeCallback(typeof(DEBUG_CALLBACK))]
		private static RESULT DEBUG_CALLBACK(DEBUG_FLAGS flags, IntPtr filePtr, int line, IntPtr funcPtr, IntPtr messagePtr)
		{
			new StringWrapper(filePtr);
			StringWrapper stringWrapper = new StringWrapper(funcPtr);
			StringWrapper stringWrapper2 = new StringWrapper(messagePtr);
			if (flags == DEBUG_FLAGS.ERROR)
			{
				global::UnityEngine.Debug.LogError(string.Format("[FMOD] {0} : {1}", stringWrapper, stringWrapper2));
			}
			else if (flags == DEBUG_FLAGS.WARNING)
			{
				global::UnityEngine.Debug.LogWarning(string.Format("[FMOD] {0} : {1}", stringWrapper, stringWrapper2));
			}
			else if (flags == DEBUG_FLAGS.LOG)
			{
				global::UnityEngine.Debug.Log(string.Format("[FMOD] {0} : {1}", stringWrapper, stringWrapper2));
			}
			return RESULT.OK;
		}

		[MonoPInvokeCallback(typeof(global::FMOD.SYSTEM_CALLBACK))]
		private static RESULT ERROR_CALLBACK(IntPtr system, global::FMOD.SYSTEM_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2, IntPtr userdata)
		{
			ERRORCALLBACK_INFO errorcallback_INFO = (ERRORCALLBACK_INFO)MarshalHelper.PtrToStructure(commanddata1, typeof(ERRORCALLBACK_INFO));
			if ((errorcallback_INFO.instancetype == ERRORCALLBACK_INSTANCETYPE.CHANNEL || errorcallback_INFO.instancetype == ERRORCALLBACK_INSTANCETYPE.CHANNELCONTROL) && errorcallback_INFO.result == RESULT.ERR_INVALID_HANDLE)
			{
				return RESULT.OK;
			}
			global::UnityEngine.Debug.LogError(string.Format("[FMOD] {0}({1}) returned {2} for {3} (0x{4}).", new object[]
			{
				errorcallback_INFO.functionname,
				errorcallback_INFO.functionparams,
				errorcallback_INFO.result,
				errorcallback_INFO.instancetype,
				errorcallback_INFO.instance.ToString("X")
			}));
			return RESULT.OK;
		}

		private static RuntimeManager Instance
		{
			get
			{
				if (RuntimeManager.initException != null)
				{
					throw RuntimeManager.initException;
				}
				if (RuntimeManager.instance == null)
				{
					RESULT result = RESULT.OK;
					RuntimeManager[] array = Resources.FindObjectsOfTypeAll<RuntimeManager>();
					for (int i = 0; i < array.Length; i++)
					{
						global::UnityEngine.Object.DestroyImmediate(array[i].gameObject);
					}
					GameObject gameObject = new GameObject("FMOD.UnityIntegration.RuntimeManager");
					RuntimeManager.instance = gameObject.AddComponent<RuntimeManager>();
					if (Application.isPlaying)
					{
						global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
					gameObject.hideFlags = HideFlags.HideAndDontSave;
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

		public static global::FMOD.System CoreSystem
		{
			get
			{
				return RuntimeManager.Instance.coreSystem;
			}
		}

		private void CheckInitResult(RESULT result, string cause)
		{
			if (result != RESULT.OK)
			{
				this.ReleaseStudioSystem();
				throw new SystemNotInitializedException(result, cause);
			}
		}

		private void ReleaseStudioSystem()
		{
			if (this.studioSystem.isValid())
			{
				this.studioSystem.release();
				this.studioSystem.clearHandle();
			}
		}

		private RESULT Initialize()
		{
			RESULT result = RESULT.OK;
			Settings settings = Settings.Instance;
			this.currentPlatform = settings.FindCurrentPlatform();
			int sampleRate = this.currentPlatform.SampleRate;
			int num = Math.Min(this.currentPlatform.RealChannelCount, 256);
			int virtualChannelCount = this.currentPlatform.VirtualChannelCount;
			uint dspbufferLength = (uint)this.currentPlatform.DSPBufferLength;
			int dspbufferCount = this.currentPlatform.DSPBufferCount;
			SPEAKERMODE speakerMode = this.currentPlatform.SpeakerMode;
			OUTPUTTYPE outputtype = this.currentPlatform.GetOutputType();
			global::FMOD.ADVANCEDSETTINGS advancedsettings = default(global::FMOD.ADVANCEDSETTINGS);
			advancedsettings.randomSeed = (uint)DateTime.UtcNow.Ticks;
			advancedsettings.maxVorbisCodecs = num;
			RuntimeManager.SetThreadAffinities(this.currentPlatform);
			this.currentPlatform.PreSystemCreate(new Action<RESULT, string>(this.CheckInitResult));
			global::FMOD.Studio.INITFLAGS initflags = global::FMOD.Studio.INITFLAGS.DEFERRED_CALLBACKS;
			if (this.currentPlatform.IsLiveUpdateEnabled)
			{
				initflags |= global::FMOD.Studio.INITFLAGS.LIVEUPDATE;
				advancedsettings.profilePort = (ushort)this.currentPlatform.LiveUpdatePort;
			}
			for (;;)
			{
				RESULT result2 = global::FMOD.Studio.System.create(out this.studioSystem);
				this.CheckInitResult(result2, "FMOD.Studio.System.create");
				result2 = this.studioSystem.getCoreSystem(out this.coreSystem);
				this.CheckInitResult(result2, "FMOD.Studio.System.getCoreSystem");
				result2 = this.coreSystem.setOutput(outputtype);
				this.CheckInitResult(result2, "FMOD.System.setOutput");
				result2 = this.coreSystem.setSoftwareChannels(num);
				this.CheckInitResult(result2, "FMOD.System.setSoftwareChannels");
				result2 = this.coreSystem.setSoftwareFormat(sampleRate, speakerMode, 0);
				this.CheckInitResult(result2, "FMOD.System.setSoftwareFormat");
				if (dspbufferLength > 0U && dspbufferCount > 0)
				{
					result2 = this.coreSystem.setDSPBufferSize(dspbufferLength, dspbufferCount);
					this.CheckInitResult(result2, "FMOD.System.setDSPBufferSize");
				}
				result2 = this.coreSystem.setAdvancedSettings(ref advancedsettings);
				this.CheckInitResult(result2, "FMOD.System.setAdvancedSettings");
				if (settings.EnableErrorCallback)
				{
					this.errorCallback = new global::FMOD.SYSTEM_CALLBACK(RuntimeManager.ERROR_CALLBACK);
					result2 = this.coreSystem.setCallback(this.errorCallback, global::FMOD.SYSTEM_CALLBACK_TYPE.ERROR);
					this.CheckInitResult(result2, "FMOD.System.setCallback");
				}
				if (!string.IsNullOrEmpty(settings.EncryptionKey))
				{
					result2 = this.studioSystem.setAdvancedSettings(default(global::FMOD.Studio.ADVANCEDSETTINGS), Settings.Instance.EncryptionKey);
					this.CheckInitResult(result2, "FMOD.Studio.System.setAdvancedSettings");
				}
				if (settings.EnableMemoryTracking)
				{
					initflags |= global::FMOD.Studio.INITFLAGS.MEMORY_TRACKING;
				}
				this.currentPlatform.PreInitialize(this.studioSystem);
				PlatformCallbackHandler callbackHandler = this.currentPlatform.CallbackHandler;
				if (callbackHandler != null)
				{
					callbackHandler.PreInitialize(this.studioSystem, new Action<RESULT, string>(this.CheckInitResult));
				}
				result2 = this.studioSystem.initialize(virtualChannelCount, initflags, global::FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
				if (result2 != RESULT.OK && result == RESULT.OK)
				{
					result = result2;
					outputtype = OUTPUTTYPE.NOSOUND;
					global::UnityEngine.Debug.LogErrorFormat("[FMOD] Studio::System::initialize returned {0}, defaulting to no-sound mode.", new object[] { result2.ToString() });
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
					global::UnityEngine.Debug.LogWarning("[FMOD] Cannot open network port for Live Update (in-use), restarting with Live Update disabled.");
					result2 = this.studioSystem.release();
					this.CheckInitResult(result2, "FMOD.Studio.System.Release");
				}
			}
			this.currentPlatform.LoadPlugins(this.coreSystem, new Action<RESULT, string>(this.CheckInitResult));
			this.LoadBanks(settings);
			return result;
		}

		private static void SetThreadAffinities(Platform platform)
		{
			foreach (ThreadAffinityGroup threadAffinityGroup in platform.ThreadAffinities)
			{
				foreach (ThreadType threadType in threadAffinityGroup.threads)
				{
					THREAD_TYPE thread_TYPE = RuntimeUtils.ToFMODThreadType(threadType);
					THREAD_AFFINITY thread_AFFINITY = RuntimeUtils.ToFMODThreadAffinity(threadAffinityGroup.affinity);
					Thread.SetAttributes(thread_TYPE, thread_AFFINITY, THREAD_PRIORITY.DEFAULT, THREAD_STACK_SIZE.DEFAULT);
				}
			}
		}

		public static int AddListener(StudioListener listener)
		{
			for (int i = 0; i < RuntimeManager.Listeners.Count; i++)
			{
				if (RuntimeManager.Listeners[i] != null && listener.gameObject == RuntimeManager.Listeners[i].gameObject)
				{
					global::UnityEngine.Debug.LogWarning(string.Format("[FMOD] Listener has already been added at index {0}.", i));
					return i;
				}
			}
			if (RuntimeManager.numListeners >= 8)
			{
				global::UnityEngine.Debug.LogWarning(string.Format("[FMOD] Max number of listeners reached : {0}.", 8));
			}
			if (RuntimeManager.Listeners.Count <= RuntimeManager.numListeners)
			{
				RuntimeManager.Listeners.Add(listener);
			}
			else
			{
				RuntimeManager.Listeners[RuntimeManager.numListeners] = listener;
			}
			RuntimeManager.numListeners++;
			int num = Mathf.Min(RuntimeManager.numListeners, 8);
			RuntimeManager.StudioSystem.setNumListeners(num);
			return RuntimeManager.numListeners - 1;
		}

		public static bool RemoveListener(StudioListener listener)
		{
			int listenerNumber = listener.ListenerNumber;
			if (listenerNumber != -1)
			{
				RuntimeManager.Listeners[listenerNumber] = null;
				if (RuntimeManager.numListeners - 1 > listenerNumber)
				{
					for (int i = listenerNumber; i < RuntimeManager.Listeners.Count; i++)
					{
						if (i == RuntimeManager.Listeners.Count - 1)
						{
							RuntimeManager.Listeners[i] = null;
						}
						else
						{
							RuntimeManager.Listeners[i] = RuntimeManager.Listeners[i + 1];
							if (RuntimeManager.Listeners[i])
							{
								RuntimeManager.Listeners[i].ListenerNumber = i;
							}
						}
					}
				}
				RuntimeManager.numListeners--;
				int num = Mathf.Min(Mathf.Max(RuntimeManager.numListeners, 1), 8);
				RuntimeManager.StudioSystem.setNumListeners(num);
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (this.studioSystem.isValid())
			{
				if (RuntimeManager.numListeners <= 0 && !this.listenerWarningIssued)
				{
					this.listenerWarningIssued = true;
				}
				for (int i = 0; i < this.activeEmitters.Count; i++)
				{
					RuntimeManager.UpdateActiveEmitter(this.activeEmitters[i], false);
				}
				for (int j = 0; j < this.attachedInstances.Count; j++)
				{
					PLAYBACK_STATE playback_STATE = PLAYBACK_STATE.STOPPED;
					if (this.attachedInstances[j].instance.isValid())
					{
						this.attachedInstances[j].instance.getPlaybackState(out playback_STATE);
					}
					if (playback_STATE == PLAYBACK_STATE.STOPPED || this.attachedInstances[j].transform == null)
					{
						this.attachedInstances[j] = this.attachedInstances[this.attachedInstances.Count - 1];
						this.attachedInstances.RemoveAt(this.attachedInstances.Count - 1);
						j--;
					}
					else if (this.attachedInstances[j].rigidBody)
					{
						this.attachedInstances[j].instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.attachedInstances[j].transform, this.attachedInstances[j].rigidBody));
					}
					else if (this.attachedInstances[j].rigidBody2D)
					{
						this.attachedInstances[j].instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.attachedInstances[j].transform, this.attachedInstances[j].rigidBody2D));
					}
					else
					{
						this.attachedInstances[j].instance.set3DAttributes(this.attachedInstances[j].transform.To3DAttributes());
					}
				}
				if (this.isOverlayEnabled)
				{
					if (!this.overlayDrawer)
					{
						this.overlayDrawer = RuntimeManager.Instance.gameObject.AddComponent<FMODRuntimeManagerOnGUIHelper>();
						this.overlayDrawer.TargetRuntimeManager = this;
					}
					else
					{
						this.overlayDrawer.gameObject.SetActive(true);
					}
				}
				else if (this.overlayDrawer != null && this.overlayDrawer.gameObject.activeSelf)
				{
					this.overlayDrawer.gameObject.SetActive(false);
				}
				this.studioSystem.update();
			}
		}

		public static void RegisterActiveEmitter(StudioEventEmitter emitter)
		{
			if (!RuntimeManager.Instance.activeEmitters.Contains(emitter))
			{
				RuntimeManager.Instance.activeEmitters.Add(emitter);
			}
		}

		public static void DeregisterActiveEmitter(StudioEventEmitter emitter)
		{
			RuntimeManager.Instance.activeEmitters.Remove(emitter);
		}

		public static void UpdateActiveEmitter(StudioEventEmitter emitter, bool force = false)
		{
			bool flag = false;
			for (int i = 0; i < RuntimeManager.Listeners.Count; i++)
			{
				if (Vector3.Distance(emitter.transform.position, RuntimeManager.Listeners[i].transform.position) <= emitter.MaxDistance)
				{
					flag = true;
					break;
				}
			}
			if (force || flag != emitter.IsPlaying())
			{
				if (flag)
				{
					emitter.PlayInstance();
					return;
				}
				emitter.StopInstance();
			}
		}

		public static void AttachInstanceToGameObject(EventInstance instance, Transform transform)
		{
			RuntimeManager.AttachedInstance attachedInstance = RuntimeManager.Instance.attachedInstances.Find((RuntimeManager.AttachedInstance x) => x.instance.handle == instance.handle);
			if (attachedInstance == null)
			{
				attachedInstance = new RuntimeManager.AttachedInstance();
				RuntimeManager.Instance.attachedInstances.Add(attachedInstance);
			}
			instance.set3DAttributes(transform.To3DAttributes());
			attachedInstance.transform = transform;
			attachedInstance.instance = instance;
		}

		public static void AttachInstanceToGameObject(EventInstance instance, Transform transform, Rigidbody rigidBody)
		{
			RuntimeManager.AttachedInstance attachedInstance = RuntimeManager.Instance.attachedInstances.Find((RuntimeManager.AttachedInstance x) => x.instance.handle == instance.handle);
			if (attachedInstance == null)
			{
				attachedInstance = new RuntimeManager.AttachedInstance();
				RuntimeManager.Instance.attachedInstances.Add(attachedInstance);
			}
			instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform, rigidBody));
			attachedInstance.transform = transform;
			attachedInstance.instance = instance;
			attachedInstance.rigidBody = rigidBody;
		}

		public static void AttachInstanceToGameObject(EventInstance instance, Transform transform, Rigidbody2D rigidBody2D)
		{
			RuntimeManager.AttachedInstance attachedInstance = RuntimeManager.Instance.attachedInstances.Find((RuntimeManager.AttachedInstance x) => x.instance.handle == instance.handle);
			if (attachedInstance == null)
			{
				attachedInstance = new RuntimeManager.AttachedInstance();
				RuntimeManager.Instance.attachedInstances.Add(attachedInstance);
			}
			instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform, rigidBody2D));
			attachedInstance.transform = transform;
			attachedInstance.instance = instance;
			attachedInstance.rigidBody2D = rigidBody2D;
		}

		public static void DetachInstanceFromGameObject(EventInstance instance)
		{
			RuntimeManager runtimeManager = RuntimeManager.Instance;
			for (int i = 0; i < runtimeManager.attachedInstances.Count; i++)
			{
				if (runtimeManager.attachedInstances[i].instance.handle == instance.handle)
				{
					runtimeManager.attachedInstances[i] = runtimeManager.attachedInstances[runtimeManager.attachedInstances.Count - 1];
					runtimeManager.attachedInstances.RemoveAt(runtimeManager.attachedInstances.Count - 1);
					return;
				}
			}
		}

		public void ExecuteOnGUI()
		{
			if (this.studioSystem.isValid() && this.isOverlayEnabled)
			{
				this.windowRect = GUI.Window(base.GetInstanceID(), this.windowRect, new GUI.WindowFunction(this.DrawDebugOverlay), "FMOD Studio Debug");
			}
		}

		private void Start()
		{
			this.isOverlayEnabled = this.currentPlatform.IsOverlayEnabled;
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
						this.coreSystem.getMasterChannelGroup(out channelGroup);
						channelGroup.getDSP(0, out this.mixerHead);
						this.mixerHead.setMeteringEnabled(false, true);
					}
					StringBuilder stringBuilder = new StringBuilder();
					CPU_USAGE cpu_USAGE;
					this.studioSystem.getCPUUsage(out cpu_USAGE);
					stringBuilder.AppendFormat("CPU: dsp = {0:F1}%, studio = {1:F1}%\n", cpu_USAGE.dspusage, cpu_USAGE.studiousage);
					int num;
					int num2;
					Memory.GetStats(out num, out num2, true);
					stringBuilder.AppendFormat("MEMORY: cur = {0}MB, max = {1}MB\n", num >> 20, num2 >> 20);
					int num3;
					int num4;
					this.coreSystem.getChannelsPlaying(out num3, out num4);
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

		private void OnDestroy()
		{
			this.coreSystem.setCallback(null, (global::FMOD.SYSTEM_CALLBACK_TYPE)0U);
			this.ReleaseStudioSystem();
			RuntimeManager.initException = null;
			RuntimeManager.instance = null;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (this.studioSystem.isValid())
			{
				RuntimeManager.PauseAllEvents(pauseStatus);
				if (pauseStatus)
				{
					this.coreSystem.mixerSuspend();
					return;
				}
				this.coreSystem.mixerResume();
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
			}
			else
			{
				if (loadResult != RESULT.ERR_EVENT_ALREADY_LOADED)
				{
					throw new BankLoadException(bankPath, loadResult);
				}
				global::UnityEngine.Debug.LogWarningFormat("[FMOD] Unable to load {0} - bank already loaded. This may occur when attempting to load another localized bank before the first is unloaded, or if a bank has been loaded via the API.", new object[] { bankName });
			}
			this.ExecuteSampleLoadRequestsIfReady();
		}

		private void ExecuteSampleLoadRequestsIfReady()
		{
			if (this.sampleLoadRequests.Count > 0)
			{
				foreach (string text in this.sampleLoadRequests)
				{
					if (!this.loadedBanks.ContainsKey(text))
					{
						return;
					}
				}
				foreach (string text2 in this.sampleLoadRequests)
				{
					this.CheckInitResult(this.loadedBanks[text2].Bank.loadSampleData(), string.Format("Loading sample data for bank: {0}", text2));
				}
				this.sampleLoadRequests.Clear();
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
				RuntimeManager.Instance.loadedBanks[bankName] = loadedBank;
				return;
			}
			string text = RuntimeManager.Instance.currentPlatform.GetBankFolder();
			if (!string.IsNullOrEmpty(Settings.Instance.TargetSubFolder))
			{
				text = RuntimeUtils.GetCommonPlatformPath(Path.Combine(text, Settings.Instance.TargetSubFolder));
			}
			string text2;
			if (Path.GetExtension(bankName) != ".bank")
			{
				text2 = string.Format("{0}/{1}{2}", text, bankName, ".bank");
			}
			else
			{
				text2 = string.Format("{0}/{1}", text, bankName);
			}
			RuntimeManager.LoadedBank loadedBank2 = default(RuntimeManager.LoadedBank);
			RESULT result = RuntimeManager.Instance.studioSystem.loadBankFile(text2, LOAD_BANK_FLAGS.NORMAL, out loadedBank2.Bank);
			RuntimeManager.Instance.loadedBankRegister(loadedBank2, text2, bankName, loadSamples, result);
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
						global::UnityEngine.Debug.LogWarningFormat("[FMOD] Unable to load {0} - bank already loaded. This may occur when attempting to load another localized bank before the first is unloaded, or if a bank has been loaded via the API.", new object[] { name });
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
				if (fmodSettings.AutomaticSampleLoading)
				{
					this.sampleLoadRequests.AddRange(this.BanksToLoad(fmodSettings));
				}
				try
				{
					foreach (string text in this.BanksToLoad(fmodSettings))
					{
						RuntimeManager.LoadBank(text, false);
					}
					RuntimeManager.WaitForAllLoads();
				}
				catch (BankLoadException ex)
				{
					global::UnityEngine.Debug.LogException(ex);
				}
			}
		}

		private IEnumerable<string> BanksToLoad(Settings fmodSettings)
		{
			switch (fmodSettings.BankLoadType)
			{
			case BankLoadType.All:
			{
				foreach (string masterBankFileName in fmodSettings.MasterBanks)
				{
					yield return masterBankFileName + ".strings";
					yield return masterBankFileName;
					masterBankFileName = null;
				}
				List<string>.Enumerator enumerator = default(List<string>.Enumerator);
				foreach (string text in fmodSettings.Banks)
				{
					yield return text;
				}
				enumerator = default(List<string>.Enumerator);
				break;
			}
			case BankLoadType.Specified:
			{
				foreach (string text2 in fmodSettings.BanksToLoad)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						yield return text2;
					}
				}
				List<string>.Enumerator enumerator = default(List<string>.Enumerator);
				break;
			}
			}
			yield break;
			yield break;
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
					RuntimeManager.Instance.sampleLoadRequests.Remove(bankName);
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
				Util.parseID(path, out empty);
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
				global::UnityEngine.Debug.LogWarning("[FMOD] Event not found: " + path);
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
				global::UnityEngine.Debug.LogWarning("[FMOD] Event not found: " + path);
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

		public static void SetListenerLocation(GameObject gameObject, Rigidbody rigidBody, GameObject attenuationObject = null)
		{
			RuntimeManager.SetListenerLocation(0, gameObject, rigidBody, attenuationObject);
		}

		public static void SetListenerLocation(int listenerIndex, GameObject gameObject, Rigidbody rigidBody, GameObject attenuationObject = null)
		{
			if (attenuationObject)
			{
				RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody), attenuationObject.transform.position.ToFMODVector());
				return;
			}
			RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody));
		}

		public static void SetListenerLocation(GameObject gameObject, Rigidbody2D rigidBody2D, GameObject attenuationObject = null)
		{
			RuntimeManager.SetListenerLocation(0, gameObject, rigidBody2D, attenuationObject);
		}

		public static void SetListenerLocation(int listenerIndex, GameObject gameObject, Rigidbody2D rigidBody2D, GameObject attenuationObject = null)
		{
			if (attenuationObject)
			{
				RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody2D), attenuationObject.transform.position.ToFMODVector());
				return;
			}
			RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, RuntimeUtils.To3DAttributes(gameObject.transform, rigidBody2D));
		}

		public static void SetListenerLocation(GameObject gameObject, GameObject attenuationObject = null)
		{
			RuntimeManager.SetListenerLocation(0, gameObject, attenuationObject);
		}

		public static void SetListenerLocation(int listenerIndex, GameObject gameObject, GameObject attenuationObject = null)
		{
			if (attenuationObject)
			{
				RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, gameObject.transform.To3DAttributes(), attenuationObject.transform.position.ToFMODVector());
				return;
			}
			RuntimeManager.Instance.studioSystem.setListenerAttributes(listenerIndex, gameObject.transform.To3DAttributes());
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
			Bus bus;
			if (RuntimeManager.HasBanksLoaded && RuntimeManager.StudioSystem.getBus("bus:/", out bus) == RESULT.OK)
			{
				bus.setPaused(paused);
			}
		}

		public static void MuteAllEvents(bool muted)
		{
			RuntimeManager.Instance.isMuted = muted;
			RuntimeManager.ApplyMuteState();
		}

		private static void ApplyMuteState()
		{
			Bus bus;
			if (RuntimeManager.HasBanksLoaded && RuntimeManager.StudioSystem.getBus("bus:/", out bus) == RESULT.OK)
			{
				bus.setMute(RuntimeManager.Instance.isMuted);
			}
		}

		public static bool IsInitialized
		{
			get
			{
				return RuntimeManager.instance != null && RuntimeManager.instance.studioSystem.isValid();
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
			return RuntimeManager.Instance.loadedBanks.ContainsKey(loadedBank);
		}

		private static SystemNotInitializedException initException = null;

		private static RuntimeManager instance;

		private Platform currentPlatform;

		private DEBUG_CALLBACK debugCallback;

		private global::FMOD.SYSTEM_CALLBACK errorCallback;

		private bool isMuted;

		private global::FMOD.Studio.System studioSystem;

		private global::FMOD.System coreSystem;

		private DSP mixerHead;

		private Dictionary<string, RuntimeManager.LoadedBank> loadedBanks = new Dictionary<string, RuntimeManager.LoadedBank>();

		private List<string> sampleLoadRequests = new List<string>();

		private Dictionary<Guid, EventDescription> cachedDescriptions = new Dictionary<Guid, EventDescription>(new RuntimeManager.GuidComparer());

		private List<StudioEventEmitter> activeEmitters = new List<StudioEventEmitter>();

		private List<RuntimeManager.AttachedInstance> attachedInstances = new List<RuntimeManager.AttachedInstance>(128);

		private bool listenerWarningIssued;

		protected bool isOverlayEnabled;

		private FMODRuntimeManagerOnGUIHelper overlayDrawer;

		private Rect windowRect = new Rect(10f, 10f, 300f, 100f);

		private string lastDebugText;

		private float lastDebugUpdate;

		public const string BankStubPrefix = "bank stub:";

		public static List<StudioListener> Listeners = new List<StudioListener>();

		private static int numListeners = 0;

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
