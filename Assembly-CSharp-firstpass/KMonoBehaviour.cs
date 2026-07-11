using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KMonoBehaviour : MonoBehaviour, IStateMachineTarget, ISaveLoadable, IUniformGridObject
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
		if (!KMonoBehaviour.isPoolPreInit && Application.isPlaying && KMonoBehaviour.lastGameObject != base.gameObject)
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
				Output.LogError(string.Concat(new string[]
				{
					"Error in: ",
					base.name,
					".",
					base.GetType().Name,
					".OnPrefabInit\n",
					ex.ToString()
				}));
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
	}

	private void OnDisable()
	{
		if (App.IsExiting || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		this.OnCmpDisable();
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
		SimAndRenderScheduler.instance.Remove(this);
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
		if (!this.isInitialized)
		{
			global::Debug.LogError(base.name + "." + base.GetType().Name + " is not initialized.", null);
			return;
		}
		this.isSpawned = true;
		if (this.autoRegisterSimRender)
		{
			SimAndRenderScheduler.instance.Add(this, this.simRenderLoadBalance);
		}
		MyCmp.OnStart(this);
		try
		{
			this.OnSpawn();
		}
		catch (Exception ex)
		{
			Output.LogError(string.Concat(new string[]
			{
				"Error in: ",
				base.name,
				".",
				base.GetType().Name,
				".OnSpawn\n",
				ex.ToString()
			}));
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

	public virtual void CreateDef()
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

	public int Subscribe(int hash, Action<object> handler)
	{
		return this.obj.GetEventSystem().Subscribe(hash, handler);
	}

	public void Subscribe(GameObject target, int hash, Action<object> handler)
	{
		this.obj.GetEventSystem().Subscribe(target, hash, handler);
	}

	public void Subscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler)
	{
		this.obj.GetEventSystem().Subscribe<ComponentType>(hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		if (this.obj != null)
		{
			this.obj.GetEventSystem().Unsubscribe(hash, handler);
		}
	}

	public void Unsubscribe(int id)
	{
		this.obj.GetEventSystem().Unsubscribe(id);
	}

	public void Unsubscribe(GameObject target, int hash, Action<object> handler)
	{
		this.obj.GetEventSystem().Unsubscribe(target, hash, handler);
	}

	public void Unsubscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings = false)
	{
		if (this.obj != null)
		{
			this.obj.GetEventSystem().Unsubscribe<ComponentType>(hash, handler, suppressWarnings);
		}
	}

	public void Trigger(int hash, object data = null)
	{
		if (this.obj != null && this.obj.hasEventSystem)
		{
			this.obj.GetEventSystem().Trigger(base.gameObject, hash, data);
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
					KFMOD.PlayOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
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
			KFMOD.PlayOneShot(asset, this.transform.GetPosition());
		}
		catch
		{
			Output.LogWarning(new object[] { "AUDIOERROR: Missing [" + asset + "]" });
		}
	}

	public virtual Vector2 PosMin()
	{
		return this.transform.GetPosition();
	}

	public virtual Vector2 PosMax()
	{
		return this.transform.GetPosition();
	}

	ComponentType IStateMachineTarget.GetComponent<ComponentType>()
	{
		return base.GetComponent<ComponentType>();
	}

	GameObject IStateMachineTarget.get_gameObject()
	{
		return base.gameObject;
	}

	string IStateMachineTarget.get_name()
	{
		return base.name;
	}

	public static GameObject lastGameObject;

	public static KObject lastObj;

	public static bool isPoolPreInit;

	public static bool isLoadingScene;

	private KObject obj;

	private bool isInitialized;

	protected bool autoRegisterSimRender = true;

	protected bool simRenderLoadBalance;
}
