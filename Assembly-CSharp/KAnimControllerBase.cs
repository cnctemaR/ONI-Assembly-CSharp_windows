using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
		this.visible = true;
	}

	public abstract KAnim.Anim GetAnim(int index);

	public string debugName { get; private set; }

	public Color32 TintColour
	{
		get
		{
			return this.tintColour;
		}
		set
		{
			int num = ((int)this.TintColour.r << 24) | ((int)this.TintColour.g << 16) | ((int)this.TintColour.b << 8) | (int)this.TintColour.a;
			int num2 = ((int)value.r << 24) | ((int)value.g << 16) | ((int)value.b << 8) | (int)value.a;
			if (num != num2)
			{
				this.SetDirty();
				this.tintColour = value;
				this.SuspendUpdates(false);
				if (this.OnTintChanged != null)
				{
					this.OnTintChanged(this.tintColour);
				}
			}
		}
	}

	public Color32 HighlightColour
	{
		get
		{
			return this.highlightColour;
		}
		set
		{
			int num = ((int)this.HighlightColour.r << 24) | ((int)this.HighlightColour.g << 16) | ((int)this.HighlightColour.b << 8) | (int)this.HighlightColour.a;
			int num2 = ((int)value.r << 24) | ((int)value.g << 16) | ((int)value.b << 8) | (int)value.a;
			if (num != num2)
			{
				this.SetDirty();
				this.highlightColour = value;
				this.SuspendUpdates(false);
				if (this.OnHighlightChanged != null)
				{
					this.OnHighlightChanged(this.highlightColour);
				}
			}
		}
	}

	public Color32 OverlayColour
	{
		get
		{
			return this.overlayColour;
		}
		set
		{
			int num = ((int)this.OverlayColour.r << 24) | ((int)this.OverlayColour.g << 16) | ((int)this.OverlayColour.b << 8) | (int)this.OverlayColour.a;
			int num2 = ((int)value.r << 24) | ((int)value.g << 16) | ((int)value.b << 8) | (int)value.a;
			if (num != num2)
			{
				this.overlayColour = value;
				this.SetDirty();
				this.SuspendUpdates(false);
				if (this.onOverlayColourChanged != null)
				{
					this.onOverlayColourChanged(value);
				}
			}
		}
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event KAnimControllerBase.KAnimEvent onAnimEnter;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event KAnimControllerBase.KAnimEvent onAnimComplete;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Color32> onOverlayColourChanged;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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

	public float PlayTime
	{
		get
		{
			return (this.curAnim == null) ? (-1f) : this.elapsedTime;
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

	public ReadOnlyCollection<KAnimControllerBase.OverrideAnimFileData> OverrideAnimFiles
	{
		get
		{
			return (this.overrideAnimFiles != null) ? this.overrideAnimFiles.AsReadOnly() : null;
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

	public KAnimBatchGroup.MaterialType GetMaterialType()
	{
		return this.materialType;
	}

	public Vector3 GetWorldPivot()
	{
		Vector3 position = base.transform.position;
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
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

	public KAnimFile[] GetAnims()
	{
		return this.animFiles;
	}

	public KAnimHashedString GetBuildHash()
	{
		KAnimHashedString kanimHashedString;
		if (this.curBuild == null)
		{
			kanimHashedString = KAnimBatchManager.NO_BATCH;
		}
		else
		{
			kanimHashedString = this.curBuild.fileHash;
		}
		return kanimHashedString;
	}

	protected float GetDuration()
	{
		float num;
		if (this.curAnim != null)
		{
			num = (float)this.curAnim.numFrames / this.curAnim.frameRate;
		}
		else
		{
			num = 0f;
		}
		return num;
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
			num = this.curAnim.GetFrameIdx(this.mode, time) + ((!absolute) ? 0 : this.curAnim.firstFrameIdx);
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
		string text;
		if (mode != KAnim.PlayMode.Once)
		{
			if (mode != KAnim.PlayMode.Loop)
			{
				if (mode != KAnim.PlayMode.Paused)
				{
					text = "Unknown";
				}
				else
				{
					text = "Paused";
				}
			}
			else
			{
				text = "Loop";
			}
		}
		else
		{
			text = "Once";
		}
		return text;
	}

	public float GetPlaySpeed()
	{
		return this.playSpeed;
	}

	public float GetElapsedTime()
	{
		return this.elapsedTime;
	}

	protected abstract void SuspendUpdates(bool suspend);

	protected abstract void OnStartQueuedAnim();

	public abstract void SetDirty();

	protected abstract void OnAwake();

	protected abstract void OnStart();

	protected abstract void OnStop();

	protected abstract void RebuildBatchGroupInstance();

	protected abstract void ApplySymbolOverride(KAnimHashedString overridden_symbol_name, HashedString batchSource, KAnim.Build.Symbol new_symbol, bool is_perminent);

	protected abstract void ApplyRemoveSingleFrameOverride(KAnimHashedString symbol_name);

	protected abstract void ApplyClearOverrides();

	protected abstract void UpdateFrame(float t);

	public abstract Matrix4x4 GetTransformMatrix();

	public abstract Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible);

	public abstract void UpdateHidden(bool reset = true);

	public abstract void AddAnims(params KAnimFile[] addedAnims);

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
		Vector3 position = base.transform.position;
		if (flag)
		{
			position = new Vector3(symbolTransform[0, 3], symbolTransform[1, 3], symbolTransform[2, 3]);
		}
		return position;
	}

	protected virtual Matrix4x4 GetRootMatrix()
	{
		return Matrix4x4.identity;
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
		this.curAnimFile = null;
		this.animQueue = null;
		this.overrideAnims = null;
		this.anims = null;
		this.baseHiddenSymbols = null;
		this.hiddenSymbols = null;
		this.visibleSymbols = null;
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

	public void MarkDirty()
	{
		this.SetDirty();
		this.dirtyBuild = true;
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
		this.Queue(anim_names[anim_names.Length - 1], mode, 1f, 0f);
	}

	public void Queue(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (this.curBuild == null)
		{
			if (this.animFiles != null && this.animFiles.Length > 0)
			{
				for (int i = 0; i < this.animFiles.Length; i++)
				{
					if (!(this.animFiles[i] == null))
					{
						KAnimFileData data = this.animFiles[i].GetData();
						if (data != null && data.build != null)
						{
							this.curBuild = data.build;
							break;
						}
					}
				}
			}
			if (this.curBuild == null)
			{
				global::Debug.LogWarning(string.Concat(new object[]
				{
					"[",
					base.gameObject.name,
					"] Missing build while trying to play anim [",
					anim_name,
					"]"
				}), base.gameObject);
				return;
			}
		}
		this.animQueue.Enqueue(new KAnimControllerBase.AnimData
		{
			anim = anim_name,
			mode = mode,
			speed = speed,
			timeOffset = time_offset
		});
		this.mode = ((mode != KAnim.PlayMode.Paused) ? KAnim.PlayMode.Once : KAnim.PlayMode.Paused);
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
			global::Debug.LogWarning(string.Concat(new object[]
			{
				"[",
				base.gameObject.name,
				"] Missing build while trying to play anim [",
				anim_name,
				"]"
			}), base.gameObject);
		}
		else
		{
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
				if (!string.IsNullOrEmpty(this.missingAnim))
				{
					animData.anim = new HashedString(this.missingAnim);
					flag = !this.anims.TryGetValue(animData.anim, out animLookupData);
					if (this.showWhenMissing != null)
					{
						this.showWhenMissing.SetActive(true);
					}
				}
				if (flag)
				{
					return;
				}
			}
			else if (this.showWhenMissing != null)
			{
				this.showWhenMissing.SetActive(false);
			}
		}
		this.curAnim = this.GetAnim(animLookupData.animIndex);
		this.curAnimFile = this.curAnim.animFile;
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
		this.elapsedTime = (float)num / this.curAnim.frameRate + animData.timeOffset;
		this.synchronizer.Sync();
		this.StartAnimEventSequence();
		this.AnimEnter(animData.anim);
	}

	public abstract void AddSymbolOverride(KAnimHashedString overridden_symbol_name, HashedString batchSource, KAnim.Build.Symbol new_symbol, bool is_perminent = false);

	public void RemoveSymbolOverride(KAnimHashedString symbol_name)
	{
		this.ApplyRemoveSingleFrameOverride(symbol_name);
		this.dirtyBuild = true;
	}

	public void AddBuildOverride(KAnimFile override_file, bool enable_symbols, bool is_perminent = false)
	{
		if (!(override_file == null))
		{
			KAnimFileData data = override_file.GetData();
			this.AddBuildOverride(data, enable_symbols, is_perminent);
		}
	}

	protected void ReApplyTempBuildOverrides()
	{
		if (this.temporaryBuildOverrides != null)
		{
			foreach (KAnimFileData kanimFileData in this.temporaryBuildOverrides)
			{
				this.ApplyBuildOverride(kanimFileData.build, true, false);
			}
		}
	}

	private void ApplyBuildOverride(KAnim.Build build, bool enable_symbols, bool is_perminent)
	{
		for (int i = 0; i < build.symbols.Length; i++)
		{
			this.AddSymbolOverride(build.symbols[i].hash, build.batchTag, build.symbols[i], is_perminent);
			if (enable_symbols)
			{
				this.ShowSymbol(build.symbols[i].hash);
			}
		}
	}

	public void AddBuildOverride(KAnimFileData kafd, bool enable_symbols, bool is_perminent)
	{
		this.ApplyBuildOverride(kafd.build, enable_symbols, is_perminent);
		if (!is_perminent)
		{
			if (this.temporaryBuildOverrides == null)
			{
				this.temporaryBuildOverrides = new HashSet<KAnimFileData>();
			}
			this.temporaryBuildOverrides.Add(kafd);
		}
	}

	public void ClearBuildOverride(KAnimFile override_file, bool enable_symbols)
	{
		if (!(override_file == null))
		{
			KAnimFileData data = override_file.GetData();
			this.ClearBuildOverride(data, enable_symbols);
		}
	}

	public void ClearBuildOverride(KAnimFileData kafd, bool enable_symbols)
	{
		for (int i = 0; i < kafd.build.symbols.Length; i++)
		{
			this.RemoveSymbolOverride(kafd.build.symbols[i].hash);
			if (enable_symbols)
			{
				this.RemoveVisibleSymbol(kafd.build.symbols[i].hash);
			}
		}
		if (this.temporaryBuildOverrides != null)
		{
			this.temporaryBuildOverrides.Remove(kafd);
		}
	}

	public void HideSymbol(bool hide, KAnimHashedString symbol)
	{
		if (hide)
		{
			this.HideSymbol(symbol, true);
		}
		else
		{
			this.StopHidingSymbol(symbol, true);
		}
	}

	public void HideSymbols(bool hide, KAnimHashedString[] symbol_names)
	{
		for (int i = 0; i < symbol_names.Length; i++)
		{
			this.HideSymbol(hide, symbol_names[i]);
		}
	}

	public bool HideSymbol(KAnimHashedString symbol, bool update_hidden = true)
	{
		if (this.baseHiddenSymbols == null)
		{
			this.baseHiddenSymbols = new List<KAnimHashedString>();
		}
		bool flag;
		if (!this.baseHiddenSymbols.Contains(symbol))
		{
			this.baseHiddenSymbols.Add(symbol);
			this.SetDirty();
			if (update_hidden)
			{
				this.UpdateHidden(true);
			}
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public bool StopHidingSymbol(KAnimHashedString symbol, bool update_hidden = true)
	{
		bool flag;
		if (this.baseHiddenSymbols == null)
		{
			flag = false;
		}
		else if (this.baseHiddenSymbols.Contains(symbol))
		{
			this.baseHiddenSymbols.Remove(symbol);
			this.SetDirty();
			if (update_hidden)
			{
				this.UpdateHidden(true);
			}
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public void ShowSymbol(KAnimHashedString symbol)
	{
		if (!this.visibleSymbols.Contains(symbol))
		{
			this.visibleSymbols.Add(symbol);
			this.SetDirty();
			this.UpdateHidden(true);
		}
	}

	public void RemoveVisibleSymbol(KAnimHashedString symbol)
	{
		if (this.visibleSymbols.Remove(symbol))
		{
			this.UpdateHidden(true);
			this.dirtyBuild = true;
		}
	}

	public void AddAnimOverrides(KAnimFile kanim_file, float priority = 0f)
	{
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
		for (int i = 0; i < this.overrideAnimFiles.Count; i++)
		{
			if (this.overrideAnimFiles[i].file == kanim_file)
			{
				this.overrideAnimFiles.RemoveAt(i);
				break;
			}
		}
		if (this.temporaryBuildOverrides != null && this.temporaryBuildOverrides.Contains(kanim_file.GetData()))
		{
			this.RemoveSymbolOverride(kanim_file);
			this.temporaryBuildOverrides.Remove(kanim_file.GetData());
		}
		this.RebuildOverrides(kanim_file);
	}

	protected abstract void RemoveSymbolOverride(KAnimFile kanim_file);

	private void RebuildOverrides(KAnimFile kanim_file)
	{
		bool flag = false;
		this.overrideAnims.Clear();
		for (int i = 0; i < this.overrideAnimFiles.Count; i++)
		{
			KAnimControllerBase.OverrideAnimFileData overrideAnimFileData = this.overrideAnimFiles[i];
			KAnimFileData data = overrideAnimFileData.file.GetData();
			if (data.buildIndex != -1)
			{
				this.AddBuildOverride(data, true, false);
			}
			for (int j = 0; j < data.animCount; j++)
			{
				KAnim.Anim anim = data.GetAnim(j);
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
		this.dirtyBuild = true;
		if (flag)
		{
			this.Restart(this.curAnim.name, this.mode, this.playSpeed, 0f);
		}
	}

	private void HideSnapToSymbols(KAnim.Build build)
	{
		if (build != null && build.symbols != null)
		{
			for (int i = 0; i < build.symbols.Length; i++)
			{
				KAnim.Build.Symbol symbol = build.symbols[i];
				if (symbol.HasFlag(KAnim.SymbolFlags.SnapTo))
				{
					this.hiddenSymbols.Add(symbol.hash);
				}
			}
		}
	}

	public void UpdateSymbolLookups()
	{
		if (this.dirtyBuild)
		{
			if (this.layering != null)
			{
				this.layering.HideSymbols();
			}
			this.dirtyBuild = false;
			if (this.curBuild != null)
			{
				this.hiddenSymbols.Clear();
				if (this.doHideSnapTo)
				{
					this.HideSnapToSymbols(this.curBuild);
					for (int i = 0; i < this.overrideAnimFiles.Count; i++)
					{
						KAnimFile file = this.overrideAnimFiles[i].file;
						KAnim.Build build = file.GetData().build;
						if (build != null)
						{
							this.HideSnapToSymbols(build);
						}
					}
				}
				this.hiddenSymbols.RemoveAll((KAnimHashedString x) => this.visibleSymbols.Contains(x));
				this.UpdateHidden(true);
			}
		}
	}

	public bool HasAnimation(string animName)
	{
		HashedString hashedString = new HashedString(animName);
		KAnimControllerBase.AnimLookupData animLookupData;
		return this.anims.TryGetValue(hashedString, out animLookupData);
	}

	public void AddAnims(KAnimFileData animFile)
	{
		if (animFile == null)
		{
			global::Debug.LogError("AddAnims() Null animfile data", null);
		}
		else
		{
			this.maxSymbols = Mathf.Max(this.maxSymbols, animFile.maxVisSymbolFrames);
			for (int i = 0; i < animFile.animCount; i++)
			{
				KAnim.Anim anim = animFile.GetAnim(i);
				this.anims[anim.hash] = new KAnimControllerBase.AnimLookupData
				{
					animIndex = anim.index
				};
			}
			if (animFile.buildIndex != -1 && animFile.build.symbols != null && animFile.build.symbols.Length > 0)
			{
				if (this.curBuild == null)
				{
					this.curBuild = animFile.build;
					this.dirtyBuild = true;
				}
			}
		}
	}

	public void ClearAnims()
	{
		this.curBuild = null;
		this.anims.Clear();
		this.overrideAnims.Clear();
		this.animFiles = new KAnimFile[0];
	}

	public IList<KAnimFile> AnimFiles
	{
		get
		{
			return this.animFiles;
		}
		set
		{
			this.animFiles = new KAnimFile[value.Count];
			for (int i = 0; i < value.Count; i++)
			{
				this.animFiles[i] = value[i];
			}
		}
	}

	public void SetAnims(KAnimFile[] setAnims, bool dobind = true)
	{
		this.dirtyBuild = true;
		this.animFiles = new KAnimFile[0];
		this.AddAnims(setAnims);
		if (dobind)
		{
			this.UpdateSymbolLookups();
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
			this.onAnimComplete((this.curAnim != null) ? this.curAnim.hash : HashedString.Invalid);
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

	public void SetPositionPercent(float percent)
	{
		if (this.curAnim != null)
		{
			this.elapsedTime = (float)this.curAnim.numFrames / this.curAnim.frameRate * percent;
			int frameIdx = this.curAnim.GetFrameIdx(this.mode, this.elapsedTime);
			if (this.currentFrame != frameIdx)
			{
				this.SetDirty();
				this.UpdateAnimEventSequenceTime();
				this.SuspendUpdates(false);
			}
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
				this.elapsedTime = this.aem.GetElapsedTime(this.eventManagerHandle);
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
		}
		else
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	[NonSerialized]
	public GameObject showWhenMissing;

	[SerializeField]
	public KAnimBatchGroup.MaterialType materialType = KAnimBatchGroup.MaterialType.Default;

	[SerializeField]
	public string initialAnim;

	[SerializeField]
	public KAnim.PlayMode initialMode = KAnim.PlayMode.Once;

	[SerializeField]
	protected KAnimFile[] animFiles = new KAnimFile[0];

	[SerializeField]
	protected string missingAnim;

	[SerializeField]
	protected Vector3 offset;

	[SerializeField]
	protected Vector3 pivot;

	[SerializeField]
	protected float rotation;

	[SerializeField]
	public bool destroyOnAnimComplete;

	[SerializeField]
	public bool inactiveDisable = false;

	[SerializeField]
	protected bool flipX = false;

	[SerializeField]
	protected bool flipY = false;

	protected KAnimFileData curAnimFile;

	protected KAnim.Anim curAnim;

	protected int curAnimFrameIdx = KAnim.Anim.Frame.InvalidFrame.idx;

	protected int prevAnimFrame = KAnim.Anim.Frame.InvalidFrame.idx;

	protected KAnim.Build curBuild;

	protected HandleVector<int>.Handle eventManagerHandle = HandleVector<int>.InvalidHandle;

	protected List<KAnimControllerBase.OverrideAnimFileData> overrideAnimFiles = new List<KAnimControllerBase.OverrideAnimFileData>();

	protected DeepProfiler DeepProfiler = new DeepProfiler(false);

	public bool randomiseLoopedOffset = false;

	protected float elapsedTime = 0f;

	protected float playSpeed = 1f;

	protected KAnim.PlayMode mode = KAnim.PlayMode.Once;

	protected bool stopped = true;

	protected bool dirtyBuild = false;

	public float animHeight = 1f;

	public float animWidth = 1f;

	protected bool visible;

	public bool doHideSnapTo = true;

	public bool warnOnMissinAnim = true;

	protected Bounds bounds;

	public Action<Bounds> OnUpdateBounds;

	public Action<Color32> OnTintChanged;

	public Action<Color32> OnHighlightChanged;

	private KAnimSynchronizer synchronizer;

	protected KAnimLayering layering;

	protected Color32 tintColour = Color.white;

	protected Color32 highlightColour = Color.black;

	protected Color32 overlayColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	[NonSerialized]
	public KAnimControllerBase.VisibilityType visibilityType = KAnimControllerBase.VisibilityType.Default;

	public Action<GameObject> onDestroySelf;

	protected List<KAnimHashedString> baseHiddenSymbols;

	protected List<KAnimHashedString> hiddenSymbols = new List<KAnimHashedString>();

	protected List<KAnimHashedString> visibleSymbols = new List<KAnimHashedString>();

	protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> anims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();

	protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> overrideAnims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();

	protected Queue<KAnimControllerBase.AnimData> animQueue = new Queue<KAnimControllerBase.AnimData>();

	protected int maxSymbols = 0;

	public Grid.SceneLayer fgLayer = Grid.SceneLayer.NoLayer;

	protected AnimEventManager aem = null;

	private static HashedString snaptoPivot = new HashedString("snapTo_pivot");

	private HashSet<KAnimFileData> temporaryBuildOverrides = null;

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
		Always
	}

	public delegate void KAnimEvent(HashedString name);
}
