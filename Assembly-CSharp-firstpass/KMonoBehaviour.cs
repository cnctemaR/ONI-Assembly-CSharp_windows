using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KMonoBehaviour : MonoBehaviour, ISaveLoadable, IStateMachineTarget
{
	public bool isSpawned { get; private set; }

	public new Transform transform
	{
		get
		{
			return base.transform;
		}
	}

	public bool isNull
	{
		get
		{
			return this == null;
		}
	}

	public void Awake()
	{
		if (App.IsExiting)
		{
			return;
		}
		this.InitializeComponent();
	}

	public void InitializeComponent()
	{
		if (this.isInitialized)
		{
			return;
		}
		if (Application.isPlaying && KMonoBehaviour.lastGameObject != base.gameObject)
		{
			KMonoBehaviour.lastGameObject = base.gameObject;
			KMonoBehaviour.lastObj = KObjectManager.Instance.GetOrCreateObject(base.gameObject);
		}
		this.obj = KMonoBehaviour.lastObj;
		this.isInitialized = true;
		MyCmp.OnAwake(this);
		if (!KMonoBehaviour.isPoolPreInit)
		{
			try
			{
				this.OnPrefabInit();
			}
			catch (Exception ex)
			{
				Output.LogError(new object[] { string.Concat(new string[]
				{
					"Error in: ",
					base.name,
					".",
					base.GetType().Name,
					".OnPrefabInit\n",
					ex.ToString()
				}) });
			}
		}
	}

	private void OnEnable()
	{
		if (App.IsExiting)
		{
			return;
		}
		this.OnCmpEnable();
		if (UpdateManager.instance != null && !this.simUpdateRegistered && this.isSpawned)
		{
			this.simUpdateRegistered = true;
			UpdateManager.AddSimUpdater(this);
		}
	}

	private void OnDisable()
	{
		if (App.IsExiting || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		this.OnCmpDisable();
		if (this.simUpdateRegistered)
		{
			this.simUpdateRegistered = false;
			UpdateManager.RemoveSimUpdater(this);
		}
	}

	public bool IsInitialized()
	{
		return this.isInitialized;
	}

	public void OnDestroy()
	{
		this.OnForcedCleanUp();
		if (App.IsExiting)
		{
			return;
		}
		if (KMonoBehaviour.isLoadingScene)
		{
			this.OnLoadLevel();
			return;
		}
		if (KObjectManager.Instance != null)
		{
			KObjectManager.Instance.QueueDestroy(this.obj);
		}
		this.OnCleanUp();
	}

	public void Start()
	{
		if (App.IsExiting)
		{
			return;
		}
		this.Spawn();
	}

	public void Spawn()
	{
		if (this.isSpawned)
		{
			return;
		}
		string text = base.GetType().Name;
		if (text == "LoopingSounds")
		{
			text = "LS";
		}
		if (text == "Sequenceable")
		{
			text = "S";
		}
		if (text == "StructureTemperature")
		{
			text = "ST";
		}
		if (!this.isInitialized)
		{
			global::Debug.LogError(base.name + "." + text + " is not initialized.", null);
			return;
		}
		this.isSpawned = true;
		MyCmp.OnStart(this);
		try
		{
			this.OnSpawn();
		}
		catch (Exception ex)
		{
			Output.LogError(new object[] { string.Concat(new string[]
			{
				"Error in: ",
				base.name,
				".",
				text,
				".OnSpawn\n",
				ex.ToString()
			}) });
		}
		if (UpdateManager.instance != null && !this.simUpdateRegistered && base.enabled)
		{
			this.simUpdateRegistered = true;
			UpdateManager.AddSimUpdater(this);
		}
	}

	protected virtual void OnPrefabInit()
	{
	}

	protected virtual void OnSpawn()
	{
	}

	protected virtual void OnCmpEnable()
	{
	}

	protected virtual void OnCmpDisable()
	{
	}

	protected virtual void OnCleanUp()
	{
	}

	protected virtual void OnForcedCleanUp()
	{
	}

	protected virtual void OnLoadLevel()
	{
	}

	public T FindOrAdd<T>() where T : KMonoBehaviour
	{
		return this.FindOrAddComponent<T>();
	}

	public void FindOrAdd<T>(ref T c) where T : KMonoBehaviour
	{
		c = this.FindOrAdd<T>();
	}

	public T Require<T>() where T : Component
	{
		return this.RequireComponent<T>();
	}

	public global::Logger GetEventLog()
	{
		return this.obj.GetEventSystem().GetLog();
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		return this.obj.GetEventSystem().Subscribe(hash, handler);
	}

	public void Subscribe(GameObject target, int hash, Action<object> handler)
	{
		this.obj.GetEventSystem().Subscribe(target, hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		this.obj.GetEventSystem().Unsubscribe(hash, handler);
	}

	public void Unsubscribe(int id)
	{
		this.obj.GetEventSystem().Unsubscribe(id);
	}

	public void Unsubscribe(GameObject target, int hash, Action<object> handler)
	{
		this.obj.GetEventSystem().Unsubscribe(target, hash, handler);
	}

	public void Trigger(int hash, object data = null)
	{
		if (this.obj != null)
		{
			this.obj.GetEventSystem().Trigger(hash, data);
		}
	}

	public static void PlaySound(string sound)
	{
		if (sound != null)
		{
			try
			{
				if (SoundListenerController.Instance == null)
				{
					KFMOD.PlayOneShot(sound);
				}
				else
				{
					KFMOD.PlayOneShot(sound, SoundListenerController.Instance.transform.position);
				}
			}
			catch
			{
				Output.LogWarning(new object[] { "AUDIOERROR: Missing [" + sound + "]" });
			}
		}
	}

	public static void PlaySound3DAtLocation(string sound, Vector3 location)
	{
		if (SoundListenerController.Instance != null)
		{
			try
			{
				KFMOD.PlayOneShot(sound, location);
			}
			catch
			{
				Output.LogWarning(new object[] { "AUDIOERROR: Missing [" + sound + "]" });
			}
		}
	}

	public void PlaySound3D(string asset)
	{
		try
		{
			KFMOD.PlayOneShot(asset, this.transform.position);
		}
		catch
		{
			Output.LogWarning(new object[] { "AUDIOERROR: Missing [" + asset + "]" });
		}
	}

	virtual ComponentType IStateMachineTarget.GetComponent<ComponentType>()
	{
		return base.GetComponent<ComponentType>();
	}

	virtual GameObject IStateMachineTarget.get_gameObject()
	{
		return base.gameObject;
	}

	virtual string IStateMachineTarget.get_name()
	{
		return base.name;
	}

	public static GameObject lastGameObject;

	public static KObject lastObj;

	public static bool isPoolPreInit;

	public static bool isLoadingScene;

	private KObject obj;

	private bool isInitialized;

	private bool simUpdateRegistered;
}
