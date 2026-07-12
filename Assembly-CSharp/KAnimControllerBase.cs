using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class KAnimControllerBase : MonoBehaviour
{
	protected KAnimControllerBase()
	{
		this.previousFrame = -1;
		this.currentFrame = -1;
		this.PlaySpeedMultiplier = 1f;
		this.synchronizer = new KAnimSynchronizer(this);
		this.layering = new KAnimLayering(this, this.fgLayer);
		this.isVisible = true;
	}

	public abstract KAnim.Anim GetAnim(int index);

	public string debugName { get; private set; }

	public KAnim.Build curBuild { get; protected set; }

	public event Action<Color32> OnOverlayColourChanged;

	public new bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
			if (!this.hasAwakeRun)
			{
				return;
			}
			if (this._enabled)
			{
				this.Enable();
				return;
			}
			this.Disable();
		}
	}

	public bool HasBatchInstanceData
	{
		get
		{
			return this.batchInstanceData != null;
		}
	}

	public SymbolInstanceGpuData symbolInstanceGpuData { get; protected set; }

	public SymbolOverrideInfoGpuData symbolOverrideInfoGpuData { get; protected set; }

	public Color32 TintColour
	{
		get
		{
			return this.batchInstanceData.GetTintColour();
		}
		set
		{
			if (this.batchInstanceData != null && this.batchInstanceData.SetTintColour(value))
			{
				this.SetDirty();
				this.SuspendUpdates(false);
				if (this.OnTintChanged != null)
				{
					this.OnTintChanged(value);
				}
			}
		}
	}

	public Color32 HighlightColour
	{
		get
		{
			return this.batchInstanceData.GetHighlightcolour();
		}
		set
		{
			if (this.batchInstanceData.SetHighlightColour(value))
			{
				this.SetDirty();
				this.SuspendUpdates(false);
				if (this.OnHighlightChanged != null)
				{
					this.OnHighlightChanged(value);
				}
			}
		}
	}

	public Color OverlayColour
	{
		get
		{
			return this.batchInstanceData.GetOverlayColour();
		}
		set
		{
			if (this.batchInstanceData.SetOverlayColour(value))
			{
				this.SetDirty();
				this.SuspendUpdates(false);
				if (this.OnOverlayColourChanged != null)
				{
					this.OnOverlayColourChanged(value);
				}
			}
		}
	}

	public event KAnimControllerBase.KAnimEvent onAnimEnter;

	public event KAnimControllerBase.KAnimEvent onAnimComplete;

	public event Action<int> onLayerChanged;

	public int previousFrame { get; protected set; }

	public int currentFrame { get; protected set; }

	public string currentAnim { get; protected set; }

	public string currentAnimFile { get; protected set; }

	public KAnimHashedString currentAnimFileHash { get; protected set; }

	public float PlaySpeedMultiplier { get; set; }

	public void SetFGLayer(Grid.SceneLayer layer)
	{
		this.fgLayer = layer;
		this.GetLayering();
		if (this.layering != null)
		{
			this.layering.SetLayer(this.fgLayer);
		}
	}

	public KAnim.PlayMode PlayMode
	{
		get
		{
			return this.mode;
		}
		set
		{
			this.mode = value;
		}
	}

	public bool FlipX
	{
		get
		{
			return this.flipX;
		}
		set
		{
			this.flipX = value;
			if (this.layering != null)
			{
				this.layering.Dirty();
			}
			this.SetDirty();
		}
	}

	public bool FlipY
	{
		get
		{
			return this.flipY;
		}
		set
		{
			this.flipY = value;
			if (this.layering != null)
			{
				this.layering.Dirty();
			}
			this.SetDirty();
		}
	}

	public Vector3 Offset
	{
		get
		{
			return this.offset;
		}
		set
		{
			this.offset = value;
			if (this.layering != null)
			{
				this.layering.Dirty();
			}
			this.DeRegister();
			this.Register();
			this.RefreshVisibilityListener();
			this.SetDirty();
		}
	}

	public float Rotation
	{
		get
		{
			return this.rotation;
		}
		set
		{
			this.rotation = value;
			if (this.layering != null)
			{
				this.layering.Dirty();
			}
			this.SetDirty();
		}
	}

	public Vector3 Pivot
	{
		get
		{
			return this.pivot;
		}
		set
		{
			this.pivot = value;
			if (this.layering != null)
			{
				this.layering.Dirty();
			}
			this.SetDirty();
		}
	}

	public Vector3 PositionIncludingOffset
	{
		get
		{
			return base.transform.GetPosition() + this.Offset;
		}
	}

	public KAnimBatchGroup.MaterialType GetMaterialType()
	{
		return this.materialType;
	}

	public Vector3 GetWorldPivot()
	{
		Vector3 position = base.transform.GetPosition();
		KBoxCollider2D component = base.GetComponent<KBoxCollider2D>();
		if (component != null)
		{
			position.x += component.offset.x;
			position.y += component.offset.y - component.size.y / 2f;
		}
		return position;
	}

	public KAnim.Anim GetCurrentAnim()
	{
		return this.curAnim;
	}

	public KAnimHashedString GetBuildHash()
	{
		if (this.curBuild == null)
		{
			return KAnimBatchManager.NO_BATCH;
		}
		return this.curBuild.fileHash;
	}

	protected float GetDuration()
	{
		if (this.curAnim != null)
		{
			return (float)this.curAnim.numFrames / this.curAnim.frameRate;
		}
		return 0f;
	}

	protected int GetFrameIdxFromOffset(int offset)
	{
		int num = -1;
		if (this.curAnim != null)
		{
			num = offset + this.curAnim.firstFrameIdx;
		}
		return num;
	}

	public int GetFrameIdx(float time, bool absolute)
	{
		int num = -1;
		if (this.curAnim != null)
		{
			num = this.curAnim.GetFrameIdx(this.mode, time) + (absolute ? this.curAnim.firstFrameIdx : 0);
		}
		return num;
	}

	public bool IsStopped()
	{
		return this.stopped;
	}

	public KAnim.Anim CurrentAnim
	{
		get
		{
			return this.curAnim;
		}
	}

	public KAnimSynchronizer GetSynchronizer()
	{
		return this.synchronizer;
	}

	public KAnimLayering GetLayering()
	{
		if (this.layering == null && this.fgLayer != Grid.SceneLayer.NoLayer)
		{
			this.layering = new KAnimLayering(this, this.fgLayer);
		}
		return this.layering;
	}

	public KAnim.PlayMode GetMode()
	{
		return this.mode;
	}

	public static string GetModeString(KAnim.PlayMode mode)
	{
		switch (mode)
		{
		case KAnim.PlayMode.Loop:
			return "Loop";
		case KAnim.PlayMode.Once:
			return "Once";
		case KAnim.PlayMode.Paused:
			return "Paused";
		default:
			return "Unknown";
		}
	}

	public float GetPlaySpeed()
	{
		return this.playSpeed;
	}

	public void SetElapsedTime(float value)
	{
		this.elapsedTime = value;
	}

	public float GetElapsedTime()
	{
		return this.elapsedTime;
	}

	protected abstract void SuspendUpdates(bool suspend);

	protected abstract void OnStartQueuedAnim();

	public abstract void SetDirty();

	protected abstract void RefreshVisibilityListener();

	protected abstract void DeRegister();

	protected abstract void Register();

	protected abstract void OnAwake();

	protected abstract void OnStart();

	protected abstract void OnStop();

	protected abstract void Enable();

	protected abstract void Disable();

	protected abstract void UpdateFrame(float t);

	public abstract Matrix2x3 GetTransformMatrix();

	public abstract Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible);

	public abstract void UpdateHidden();

	public abstract void TriggerStop();

	public virtual void SetLayer(int layer)
	{
		if (this.onLayerChanged != null)
		{
			this.onLayerChanged(layer);
		}
	}

	public Vector3 GetPivotSymbolPosition()
	{
		bool flag = false;
		Matrix4x4 symbolTransform = this.GetSymbolTransform(KAnimControllerBase.snaptoPivot, out flag);
		Vector3 position = base.transform.GetPosition();
		if (flag)
		{
			position = new Vector3(symbolTransform[0, 3], symbolTransform[1, 3], symbolTransform[2, 3]);
		}
		return position;
	}

	public virtual Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
	{
		symbolVisible = false;
		return Matrix4x4.identity;
	}

	private void Awake()
	{
		if (Global.Instance != null)
		{
			this.aem = Global.Instance.GetAnimEventManager();
		}
		this.debugName = base.name;
		this.SetFGLayer(this.fgLayer);
		this.OnAwake();
		if (!string.IsNullOrEmpty(this.initialAnim))
		{
			this.SetDirty();
			this.Play(this.initialAnim, this.initialMode, 1f, 0f);
		}
		this.hasAwakeRun = true;
	}

	private void Start()
	{
		this.OnStart();
	}

	protected virtual void OnDestroy()
	{
		this.animFiles = null;
		this.curAnim = null;
		this.curBuild = null;
		this.synchronizer = null;
		this.layering = null;
		this.animQueue = null;
		this.overrideAnims = null;
		this.anims = null;
		this.synchronizer = null;
		this.layering = null;
		this.overrideAnimFiles = null;
	}

	protected void AnimEnter(HashedString hashed_name)
	{
		if (this.onAnimEnter != null)
		{
			this.onAnimEnter(hashed_name);
		}
	}

	public void Play(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (!this.stopped)
		{
			this.Stop();
		}
		this.Queue(anim_name, mode, speed, time_offset);
	}

	public void Play(HashedString[] anim_names, KAnim.PlayMode mode = KAnim.PlayMode.Once)
	{
		if (!this.stopped)
		{
			this.Stop();
		}
		for (int i = 0; i < anim_names.Length - 1; i++)
		{
			this.Queue(anim_names[i], KAnim.PlayMode.Once, 1f, 0f);
		}
		global::Debug.Assert(anim_names.Length != 0, "Play was called with an empty anim array");
		this.Queue(anim_names[anim_names.Length - 1], mode, 1f, 0f);
	}

	public void Queue(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		this.animQueue.Enqueue(new KAnimControllerBase.AnimData
		{
			anim = anim_name,
			mode = mode,
			speed = speed,
			timeOffset = time_offset
		});
		this.mode = ((mode == KAnim.PlayMode.Paused) ? KAnim.PlayMode.Paused : KAnim.PlayMode.Once);
		if (this.aem != null)
		{
			this.aem.SetMode(this.eventManagerHandle, this.mode);
		}
		if (this.animQueue.Count == 1 && this.stopped)
		{
			this.StartQueuedAnim();
		}
	}

	public void ClearQueue()
	{
		this.animQueue.Clear();
	}

	private void Restart(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (this.curBuild == null)
		{
			string[] array = new string[5];
			array[0] = "[";
			array[1] = base.gameObject.name;
			array[2] = "] Missing build while trying to play anim [";
			int num = 3;
			HashedString hashedString = anim_name;
			array[num] = hashedString.ToString();
			array[4] = "]";
			global::Debug.LogWarning(string.Concat(array), base.gameObject);
			return;
		}
		Queue<KAnimControllerBase.AnimData> queue = new Queue<KAnimControllerBase.AnimData>();
		queue.Enqueue(new KAnimControllerBase.AnimData
		{
			anim = anim_name,
			mode = mode,
			speed = speed,
			timeOffset = time_offset
		});
		while (this.animQueue.Count > 0)
		{
			queue.Enqueue(this.animQueue.Dequeue());
		}
		this.animQueue = queue;
		if (this.animQueue.Count == 1 && this.stopped)
		{
			this.StartQueuedAnim();
		}
	}

	protected void StartQueuedAnim()
	{
		this.StopAnimEventSequence();
		this.previousFrame = -1;
		this.currentFrame = -1;
		this.SuspendUpdates(false);
		this.stopped = false;
		this.OnStartQueuedAnim();
		KAnimControllerBase.AnimData animData = this.animQueue.Dequeue();
		while (animData.mode == KAnim.PlayMode.Loop && this.animQueue.Count > 0)
		{
			animData = this.animQueue.Dequeue();
		}
		KAnimControllerBase.AnimLookupData animLookupData;
		if (this.overrideAnims == null || !this.overrideAnims.TryGetValue(animData.anim, out animLookupData))
		{
			if (!this.anims.TryGetValue(animData.anim, out animLookupData))
			{
				bool flag = true;
				if (this.showWhenMissing != null)
				{
					this.showWhenMissing.SetActive(true);
				}
				if (flag)
				{
					this.TriggerStop();
					return;
				}
			}
			else if (this.showWhenMissing != null)
			{
				this.showWhenMissing.SetActive(false);
			}
		}
		this.curAnim = this.GetAnim(animLookupData.animIndex);
		int num = 0;
		if (animData.mode == KAnim.PlayMode.Loop && this.randomiseLoopedOffset)
		{
			num = global::UnityEngine.Random.Range(0, this.curAnim.numFrames - 1);
		}
		this.prevAnimFrame = -1;
		this.curAnimFrameIdx = this.GetFrameIdxFromOffset(num);
		this.currentFrame = this.curAnimFrameIdx;
		this.mode = animData.mode;
		this.playSpeed = animData.speed * this.PlaySpeedMultiplier;
		this.SetElapsedTime((float)num / this.curAnim.frameRate + animData.timeOffset);
		this.synchronizer.Sync();
		this.StartAnimEventSequence();
		this.AnimEnter(animData.anim);
	}

	public bool GetSymbolVisiblity(KAnimHashedString symbol)
	{
		return !this.hiddenSymbols.Contains(symbol);
	}

	public void SetSymbolVisiblity(KAnimHashedString symbol, bool is_visible)
	{
		if (is_visible)
		{
			this.hiddenSymbols.Remove(symbol);
		}
		else if (!this.hiddenSymbols.Contains(symbol))
		{
			this.hiddenSymbols.Add(symbol);
		}
		if (this.curBuild != null)
		{
			this.UpdateHidden();
		}
	}

	public void AddAnimOverrides(KAnimFile kanim_file, float priority = 0f)
	{
		global::Debug.Assert(kanim_file != null);
		if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length != 0)
		{
			SymbolOverrideController component = base.GetComponent<SymbolOverrideController>();
			DebugUtil.Assert(component != null, "Anim overrides containing additional symbols require a symbol override controller.");
			component.AddBuildOverride(kanim_file.GetData(), 0);
		}
		this.overrideAnimFiles.Add(new KAnimControllerBase.OverrideAnimFileData
		{
			priority = priority,
			file = kanim_file
		});
		this.overrideAnimFiles.Sort((KAnimControllerBase.OverrideAnimFileData a, KAnimControllerBase.OverrideAnimFileData b) => b.priority.CompareTo(a.priority));
		this.RebuildOverrides(kanim_file);
	}

	public void RemoveAnimOverrides(KAnimFile kanim_file)
	{
		global::Debug.Assert(kanim_file != null);
		if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length != 0)
		{
			SymbolOverrideController component = base.GetComponent<SymbolOverrideController>();
			DebugUtil.Assert(component != null, "Anim overrides containing additional symbols require a symbol override controller.");
			component.TryRemoveBuildOverride(kanim_file.GetData(), 0);
		}
		for (int i = 0; i < this.overrideAnimFiles.Count; i++)
		{
			if (this.overrideAnimFiles[i].file == kanim_file)
			{
				this.overrideAnimFiles.RemoveAt(i);
				break;
			}
		}
		this.RebuildOverrides(kanim_file);
	}

	private void RebuildOverrides(KAnimFile kanim_file)
	{
		bool flag = false;
		this.overrideAnims.Clear();
		for (int i = 0; i < this.overrideAnimFiles.Count; i++)
		{
			KAnimControllerBase.OverrideAnimFileData overrideAnimFileData = this.overrideAnimFiles[i];
			KAnimFileData data = overrideAnimFileData.file.GetData();
			for (int j = 0; j < data.animCount; j++)
			{
				KAnim.Anim anim = data.GetAnim(j);
				if (anim.animFile.hashName != data.hashName)
				{
					global::Debug.LogError(string.Format("How did we get an anim from another file? [{0}] != [{1}] for anim [{2}]", data.name, anim.animFile.name, j));
				}
				KAnimControllerBase.AnimLookupData animLookupData = default(KAnimControllerBase.AnimLookupData);
				animLookupData.animIndex = anim.index;
				HashedString hashedString = new HashedString(anim.name);
				if (!this.overrideAnims.ContainsKey(hashedString))
				{
					this.overrideAnims[hashedString] = animLookupData;
				}
				if (this.curAnim != null && this.curAnim.hash == hashedString && overrideAnimFileData.file == kanim_file)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.Restart(this.curAnim.name, this.mode, this.playSpeed, 0f);
		}
	}

	public bool HasAnimation(HashedString anim_name)
	{
		return this.anims.ContainsKey(anim_name) || this.overrideAnims.ContainsKey(anim_name);
	}

	public void AddAnims(KAnimFile anim_file)
	{
		KAnimFileData data = anim_file.GetData();
		if (data == null)
		{
			global::Debug.LogError("AddAnims() Null animfile data");
			return;
		}
		this.maxSymbols = Mathf.Max(this.maxSymbols, data.maxVisSymbolFrames);
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.animFile.hashName != data.hashName)
			{
				global::Debug.LogErrorFormat("How did we get an anim from another file? [{0}] != [{1}] for anim [{2}]", new object[]
				{
					data.name,
					anim.animFile.name,
					i
				});
			}
			this.anims[anim.hash] = new KAnimControllerBase.AnimLookupData
			{
				animIndex = anim.index
			};
		}
		if (this.usingNewSymbolOverrideSystem && data.buildIndex != -1 && data.build.symbols != null && data.build.symbols.Length != 0)
		{
			base.GetComponent<SymbolOverrideController>().AddBuildOverride(anim_file.GetData(), -1);
		}
	}

	public KAnimFile[] AnimFiles
	{
		get
		{
			return this.animFiles;
		}
		set
		{
			DebugUtil.AssertArgs(value.Length != 0, new object[] { "Controller has no anim files.", base.gameObject });
			DebugUtil.AssertArgs(value[0] != null, new object[] { "First anim file needs to be non-null.", base.gameObject });
			DebugUtil.AssertArgs(value[0].IsBuildLoaded, new object[] { "First anim file needs to be the build file.", base.gameObject });
			for (int i = 0; i < value.Length; i++)
			{
				DebugUtil.AssertArgs(value[i] != null, new object[] { "Anim file is null", base.gameObject });
			}
			this.animFiles = new KAnimFile[value.Length];
			for (int j = 0; j < value.Length; j++)
			{
				this.animFiles[j] = value[j];
			}
		}
	}

	public void Stop()
	{
		if (this.curAnim != null)
		{
			this.StopAnimEventSequence();
		}
		this.animQueue.Clear();
		this.stopped = true;
		if (this.onAnimComplete != null)
		{
			this.onAnimComplete((this.curAnim == null) ? HashedString.Invalid : this.curAnim.hash);
		}
		this.OnStop();
	}

	public void StopAndClear()
	{
		if (!this.stopped)
		{
			this.Stop();
		}
		this.bounds.center = Vector3.zero;
		this.bounds.extents = Vector3.zero;
		if (this.OnUpdateBounds != null)
		{
			this.OnUpdateBounds(this.bounds);
		}
	}

	public float GetPositionPercent()
	{
		return this.GetElapsedTime() / this.GetDuration();
	}

	public void SetPositionPercent(float percent)
	{
		if (this.curAnim == null)
		{
			return;
		}
		this.SetElapsedTime((float)this.curAnim.numFrames / this.curAnim.frameRate * percent);
		int frameIdx = this.curAnim.GetFrameIdx(this.mode, this.elapsedTime);
		if (this.currentFrame != frameIdx)
		{
			this.SetDirty();
			this.UpdateAnimEventSequenceTime();
			this.SuspendUpdates(false);
		}
	}

	protected void StartAnimEventSequence()
	{
		if (!this.layering.GetIsForeground() && this.aem != null)
		{
			this.eventManagerHandle = this.aem.PlayAnim(this, this.curAnim, this.mode, this.elapsedTime, this.visibilityType == KAnimControllerBase.VisibilityType.Always);
		}
	}

	protected void UpdateAnimEventSequenceTime()
	{
		if (this.eventManagerHandle.IsValid() && this.aem != null)
		{
			this.aem.SetElapsedTime(this.eventManagerHandle, this.elapsedTime);
		}
	}

	protected void StopAnimEventSequence()
	{
		if (this.eventManagerHandle.IsValid() && this.aem != null)
		{
			if (!this.stopped && this.mode != KAnim.PlayMode.Paused)
			{
				this.SetElapsedTime(this.aem.GetElapsedTime(this.eventManagerHandle));
			}
			this.aem.StopAnim(this.eventManagerHandle);
			this.eventManagerHandle = HandleVector<int>.InvalidHandle;
		}
	}

	protected void DestroySelf()
	{
		if (this.onDestroySelf != null)
		{
			this.onDestroySelf(base.gameObject);
			return;
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	[NonSerialized]
	public GameObject showWhenMissing;

	[SerializeField]
	public KAnimBatchGroup.MaterialType materialType;

	[SerializeField]
	public string initialAnim;

	[SerializeField]
	public KAnim.PlayMode initialMode = KAnim.PlayMode.Once;

	[SerializeField]
	protected KAnimFile[] animFiles = new KAnimFile[0];

	[SerializeField]
	protected Vector3 offset;

	[SerializeField]
	protected Vector3 pivot;

	[SerializeField]
	protected float rotation;

	[SerializeField]
	public bool destroyOnAnimComplete;

	[SerializeField]
	public bool inactiveDisable;

	[SerializeField]
	protected bool flipX;

	[SerializeField]
	protected bool flipY;

	[SerializeField]
	public bool forceUseGameTime;

	public string defaultAnim;

	protected KAnim.Anim curAnim;

	protected int curAnimFrameIdx = KAnim.Anim.Frame.InvalidFrame.idx;

	protected int prevAnimFrame = KAnim.Anim.Frame.InvalidFrame.idx;

	public bool usingNewSymbolOverrideSystem;

	protected HandleVector<int>.Handle eventManagerHandle = HandleVector<int>.InvalidHandle;

	protected List<KAnimControllerBase.OverrideAnimFileData> overrideAnimFiles = new List<KAnimControllerBase.OverrideAnimFileData>();

	protected DeepProfiler DeepProfiler = new DeepProfiler(false);

	public bool randomiseLoopedOffset;

	protected float elapsedTime;

	protected float playSpeed = 1f;

	protected KAnim.PlayMode mode = KAnim.PlayMode.Once;

	protected bool stopped = true;

	public float animHeight = 1f;

	public float animWidth = 1f;

	protected bool isVisible;

	protected Bounds bounds;

	public Action<Bounds> OnUpdateBounds;

	public Action<Color> OnTintChanged;

	public Action<Color> OnHighlightChanged;

	protected KAnimSynchronizer synchronizer;

	protected KAnimLayering layering;

	[SerializeField]
	protected bool _enabled = true;

	protected bool hasEnableRun;

	protected bool hasAwakeRun;

	protected KBatchedAnimInstanceData batchInstanceData;

	public KAnimControllerBase.VisibilityType visibilityType;

	public Action<GameObject> onDestroySelf;

	[SerializeField]
	protected List<KAnimHashedString> hiddenSymbols = new List<KAnimHashedString>();

	protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> anims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();

	protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> overrideAnims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();

	protected Queue<KAnimControllerBase.AnimData> animQueue = new Queue<KAnimControllerBase.AnimData>();

	protected int maxSymbols;

	public Grid.SceneLayer fgLayer = Grid.SceneLayer.NoLayer;

	protected AnimEventManager aem;

	private static HashedString snaptoPivot = new HashedString("snapTo_pivot");

	public struct OverrideAnimFileData
	{
		public float priority;

		public KAnimFile file;
	}

	public struct AnimLookupData
	{
		public int animIndex;
	}

	public struct AnimData
	{
		public HashedString anim;

		public KAnim.PlayMode mode;

		public float speed;

		public float timeOffset;
	}

	public enum VisibilityType
	{
		Default,
		OffscreenUpdate,
		Always
	}

	public delegate void KAnimEvent(HashedString name);
}
