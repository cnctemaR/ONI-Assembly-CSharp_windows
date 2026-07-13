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
		MyAttributes.OnAwake(this);
		if (!KMonoBehaviour.isPoolPreInit)
		{
			try
			{
				this.OnPrefabInit();
			}
			catch (Exception ex)
			{
				string text = string.Concat(new string[]
				{
					"Error in ",
					base.name,
					".",
					base.GetType().Name,
					".OnPrefabInit at ",
					this.transform.position.ToString()
				});
				DebugUtil.LogExceptionCallstack(this, text, ex.ToString(), ex);
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

	protected virtual void OnDisable()
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
		if (KObjectManager.Instance != null && !base.gameObject.activeSelf)
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
			global::Debug.LogError(base.name + "." + base.GetType().Name + " is not initialized.");
			return;
		}
		this.isSpawned = true;
		if (this.autoRegisterSimRender)
		{
			SimAndRenderScheduler.instance.Add(this, this.simRenderLoadBalance);
		}
		MyAttributes.OnStart(this);
		try
		{
			this.OnSpawn();
		}
		catch (Exception ex)
		{
			string text = string.Concat(new string[]
			{
				"Error in ",
				base.name,
				".",
				base.GetType().Name,
				".OnSpawn at ",
				this.transform.position.ToString()
			});
			DebugUtil.LogExceptionCallstack(this, text, ex.ToString(), ex);
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

	public int Subscribe(int hash, Action<object> handler)
	{
		return this.obj.GetOrCreateEventSystem().Subscribe(hash, handler);
	}

	public int Subscribe(int hash, Action<object, object> handler, object handlerData)
	{
		return this.obj.GetOrCreateEventSystem().Subscribe(hash, handler, handlerData);
	}

	public int Subscribe(GameObject target, int hash, Action<object> handler)
	{
		return this.obj.GetOrCreateEventSystem().Subscribe(target, hash, handler);
	}

	public int Subscribe(GameObject target, int hash, Action<object, object> handler, object handlerData)
	{
		return this.obj.GetOrCreateEventSystem().Subscribe(target, hash, handler, handlerData);
	}

	public int Subscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler) where ComponentType : Component
	{
		return this.obj.GetOrCreateEventSystem().Subscribe<ComponentType>(hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Unsubscribe(hash, handler);
		}
	}

	public void Unsubscribe(int id)
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Unsubscribe(id);
		}
	}

	public void Unsubscribe(ref int id)
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Unsubscribe(id);
		}
		id = -1;
	}

	public void Unsubscribe(GameObject target, int hash, Action<object> handler)
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Unsubscribe(target, hash, handler);
		}
	}

	public void Unsubscribe(GameObject target, int id)
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Unsubscribe(target, id);
		}
	}

	public void Unsubscribe(GameObject target, ref int id)
	{
		this.Unsubscribe(target, id);
		id = -1;
	}

	public void Unsubscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings = false) where ComponentType : Component
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Unsubscribe<ComponentType>(hash, handler, suppressWarnings);
		}
	}

	public void Trigger(int hash, object data = null)
	{
		EventSystem eventSystem;
		if (this.obj != null && this.obj.GetEventSystem(out eventSystem))
		{
			eventSystem.Trigger(base.gameObject, hash, data);
		}
	}

	[Obsolete("Use BoxingTrigger to avoid sended boxing object to garbage collection, be careful to unbox parameter in any handlers")]
	public void Trigger<T>(int hash, T data) where T : struct
	{
		this.Trigger(hash, data);
	}

	public void BoxingTrigger(int hash, bool data)
	{
		this.Trigger(hash, BoxedBools.Box(data));
	}

	public void BoxingTrigger<T>(int hash, T data) where T : struct
	{
		Boxed<T> boxed = Boxed<T>.Get(data);
		this.Trigger(hash, boxed);
		boxed.Release();
	}

	public static void PlaySound(string sound)
	{
		if (sound != null)
		{
			try
			{
				if (SoundListenerController.Instance == null)
				{
					KFMOD.PlayUISound(sound);
				}
				else
				{
					KFMOD.PlayOneShot(sound, SoundListenerController.Instance.transform.GetPosition(), 1f);
				}
			}
			catch
			{
				DebugUtil.LogWarningArgs(new object[] { "AUDIOERROR: Missing [" + sound + "]" });
			}
		}
	}

	public static void PlaySound3DAtLocation(string sound, Vector3 location)
	{
		if (SoundListenerController.Instance != null)
		{
			try
			{
				KFMOD.PlayOneShot(sound, location, 1f);
			}
			catch
			{
				DebugUtil.LogWarningArgs(new object[] { "AUDIOERROR: Missing [" + sound + "]" });
			}
		}
	}

	public void PlaySound3D(string asset)
	{
		try
		{
			KFMOD.PlayOneShot(asset, this.transform.GetPosition(), 1f);
		}
		catch
		{
			DebugUtil.LogWarningArgs(new object[] { "AUDIOERROR: Missing [" + asset + "]" });
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
