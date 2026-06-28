using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class KAnimControllerBase : MonoBehaviour
{
	protected KAnimControllerBase()
	{
		this.previousFrame = -1;
		this.currentFrame = -1;
		this.PlaySpeedMultiplier = 1f;
		this.MovementSpeedMultiplier = 1f;
		this.log = new LoggerFSSS("KAnimController");
		this.synchronizer = new KAnimSynchronizer(this);
		this.layering = new KAnimLayering(this, this.fgLayer);
		this.visible = true;
	}

	public event KAnimControllerBase.KAnimEvent onAnimEnter;

	public event KAnimControllerBase.KAnimEvent onAnimComplete;

	public event Action<Color32> onTemperatureColorChanged;

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
				this.forceRebuild = true;
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
				this.forceRebuild = true;
				this.highlightColour = value;
				this.SuspendUpdates(false);
				if (this.OnHighlightChanged != null)
				{
					this.OnHighlightChanged(this.highlightColour);
				}
			}
		}
	}

	public Color32 TemperatureColour
	{
		get
		{
			return this.temperatureColour;
		}
		set
		{
			int num = ((int)this.TemperatureColour.r << 24) | ((int)this.TemperatureColour.g << 16) | ((int)this.TemperatureColour.b << 8) | (int)this.TemperatureColour.a;
			int num2 = ((int)value.r << 24) | ((int)value.g << 16) | ((int)value.b << 8) | (int)value.a;
			if (num != num2)
			{
				this.temperatureColour = value;
				this.forceRebuild = true;
				this.SuspendUpdates(false);
				this.OnTemperatureChanged();
				if (this.onTemperatureColorChanged != null)
				{
					this.onTemperatureColorChanged(value);
				}
			}
		}
	}

	public int previousFrame { get; protected set; }

	public int currentFrame { get; protected set; }

	public string currentAnim
	{
		get
		{
			if (this.curAnim != null)
			{
				return this.curAnim.name;
			}
			return null;
		}
	}

	public string currentAnimFile
	{
		get
		{
			if (this.curAnimFile != null)
			{
				return this.curAnimFile.name;
			}
			return null;
		}
	}

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

	public float MovementSpeedMultiplier
	{
		get
		{
			return this.movementSpeedMultiplier;
		}
		set
		{
			this.movementSpeedMultiplier = value / 1f;
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

	public bool Flip
	{
		get
		{
			return this.flip;
		}
		set
		{
			this.flip = value;
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
			this.forceRebuild = true;
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
			this.forceRebuild = true;
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
			this.forceRebuild = true;
		}
	}

	public KAnimBatchGroup.MaterialType GetMaterialType()
	{
		return this.materialType;
	}

	public global::Logger GetLog()
	{
		return this.log;
	}

	public Vector3 GetWorldPivot()
	{
		Vector3 position = base.transform.position;
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		position.x += component.offset.x;
		position.y += component.offset.y - component.size.y / 2f;
		return position;
	}

	public KAnim.Anim GetCurrentAnim()
	{
		return this.curAnim;
	}

	public List<KAnimHashedString> GetHiddenSymbols()
	{
		return this.baseHiddenSymbols;
	}

	public KAnimFile[] GetAnims()
	{
		return this.animFiles;
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

	protected KAnim.Anim.Frame GetFrame(float time)
	{
		return (this.curAnimFile == null || this.curAnim == null) ? KAnim.Anim.Frame.InvalidFrame : this.curAnim.GetFrame(this.curAnimFile, this.mode, time);
	}

	protected int GetFrameIdx(int offset)
	{
		int num = -1;
		if (this.curAnim != null)
		{
			num = offset + this.curAnim.firstFrameIdx;
		}
		return num;
	}

	protected int GetFrameIdx(float time)
	{
		int num = -1;
		if (this.curAnim != null)
		{
			num = this.curAnim.GetFrameIdx(this.mode, time) + this.curAnim.firstFrameIdx;
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

	public KAnimControllerBase.AnimLookupData GetAnimData(string name)
	{
		return this.GetAnimData(new HashedString(name));
	}

	public KAnimControllerBase.AnimLookupData GetAnimData(HashedString hashed_name)
	{
		KAnimControllerBase.AnimLookupData animLookupData = default(KAnimControllerBase.AnimLookupData);
		if (!this.overrideAnims.TryGetValue(hashed_name, out animLookupData))
		{
			this.anims.TryGetValue(hashed_name, out animLookupData);
		}
		return animLookupData;
	}

	public int GetFrameIdx()
	{
		return this.curAnimFrameIdx;
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

	public float GetElapsedTime()
	{
		return this.elapsedTime;
	}

	protected abstract void SuspendUpdates(bool suspend);

	protected abstract void OnStartQueuedAnim();

	protected virtual void OnTemperatureChanged()
	{
	}

	protected virtual void OnAwake()
	{
	}

	protected virtual void OnStart()
	{
	}

	protected virtual void OnStop()
	{
	}

	protected virtual void Rebuild()
	{
	}

	protected virtual void UpdateSymbolOverrides(bool setBatch = true)
	{
	}

	protected virtual void ApplySymbolOverride(KAnimHashedString overridden_symbol_name, HashedString batchSource, KAnim.Build.Symbol new_symbol)
	{
	}

	protected virtual void ApplyRemoveOverride(KAnimHashedString symbol_name)
	{
	}

	protected virtual void ApplyClearOverrides()
	{
	}

	protected virtual void BindAnims()
	{
	}

	protected virtual void UpdateFrame(float t)
	{
	}

	public virtual void UpdateHidden(bool reset = true)
	{
	}

	public virtual void SetLayer(int layer)
	{
	}

	public Vector3 GetPivotSymbolPosition()
	{
		bool flag = false;
		Matrix4x4 symbolTransform = base.GetComponent<KBatchedAnimController>().GetSymbolTransform(KAnimControllerBase.snaptoPivot, out flag);
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
		this.SetFGLayer(this.fgLayer);
		this.OnAwake();
		if (this.initialAnim != null && this.initialAnim != string.Empty)
		{
			this.forceRebuild = true;
			this.Play(this.initialAnim, this.initialMode, 1f, 0f);
		}
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
			component.AddLog(this.log);
		}
	}

	private void Start()
	{
		this.OnStart();
	}

	protected void TemperatureColorChanged(Color32 value)
	{
		if (this.onTemperatureColorChanged != null)
		{
			this.onTemperatureColorChanged(value);
		}
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
		this.forceRebuild = true;
		this.dirtyBuild = true;
	}

	public void Play(string anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		this.forceRebuild = true;
		if (!this.stopped)
		{
			this.Stop();
		}
		this.Queue(anim_name, mode, speed, time_offset);
	}

	public void Play(AnimRef[] playAnims, KAnim.PlayMode mode = KAnim.PlayMode.Once)
	{
		if (!this.stopped)
		{
			this.Stop();
		}
		for (int i = 0; i < playAnims.Length - 1; i++)
		{
			if (playAnims[i] != null)
			{
				this.Queue(playAnims[i].name, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		if (playAnims[playAnims.Length - 1] != null)
		{
			this.Queue(playAnims[playAnims.Length - 1].name, mode, 1f, 0f);
		}
	}

	public void Play(string[] anim_names, KAnim.PlayMode mode = KAnim.PlayMode.Once)
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

	public void Queue(string anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
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
				Debug.LogWarning(string.Concat(new string[]
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
			anim = new HashedString(anim_name),
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

	private void Restart(string anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (this.curBuild == null)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"[",
				base.gameObject.name,
				"] Missing build while trying to play anim [",
				anim_name,
				"]"
			}), base.gameObject);
			return;
		}
		Queue<KAnimControllerBase.AnimData> queue = new Queue<KAnimControllerBase.AnimData>();
		queue.Enqueue(new KAnimControllerBase.AnimData
		{
			anim = new HashedString(anim_name),
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
		this.forceRebuild = true;
	}

	protected void StartQueuedAnim()
	{
		this.StopAnimEventSequence();
		this.previousFrame = -1;
		this.currentFrame = -1;
		this.SuspendUpdates(false);
		this.stopped = false;
		this.forceRebuild = true;
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
				if (this.missingAnim != null && this.missingAnim != string.Empty)
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
		if (this.curAnimFile != null && animLookupData.animFile != this.curAnimFile && this.curAnimFile.build != null)
		{
			this.ClearBuildOverride(this.curAnimFile, false);
		}
		this.curAnimFile = animLookupData.animFile;
		this.curAnim = animLookupData.anim;
		int num = 0;
		if (animData.mode == KAnim.PlayMode.Loop && this.randomiseLoopedOffset)
		{
			num = global::UnityEngine.Random.Range(0, this.curAnim.numFrames - 1);
		}
		this.prevAnimFrame = -1;
		this.curAnimFrameIdx = this.GetFrameIdx(num);
		this.currentFrame = this.curAnimFrameIdx;
		this.mode = animData.mode;
		this.playSpeed = animData.speed * this.PlaySpeedMultiplier * this.MovementSpeedMultiplier;
		this.elapsedTime = (float)num / this.curAnim.frameRate + animData.timeOffset;
		this.synchronizer.Sync();
		this.StartAnimEventSequence();
		this.AnimEnter(animData.anim);
		this.Rebuild();
	}

	protected void ReApplyOverrides()
	{
		if (this.pendingOverrides != null)
		{
			this.AcceptOverrides();
		}
		else if (this.appliedOverrides != null)
		{
			for (int i = 0; i < this.appliedOverrides.Count; i++)
			{
				this.ApplySymbolOverride(this.appliedOverrides[i].overridden_symbol_name, this.appliedOverrides[i].batchSource, this.appliedOverrides[i].new_symbol);
			}
			this.dirtyBuild = true;
			this.forceRebuild = true;
			this.UpdateSymbolOverrides(false);
		}
	}

	protected void AcceptOverrides()
	{
		if (this.pendingOverrides != null)
		{
			for (int i = 0; i < this.pendingOverrides.Count; i++)
			{
				this.ApplySymbolOverride(this.pendingOverrides[i].overridden_symbol_name, this.pendingOverrides[i].batchSource, this.pendingOverrides[i].new_symbol);
			}
			this.appliedOverrides = this.pendingOverrides;
			this.pendingOverrides = null;
			this.dirtyBuild = true;
			this.forceRebuild = true;
		}
	}

	public void AddSymbolOverride(KAnimHashedString overridden_symbol_name, HashedString batchSource, KAnim.Build.Symbol new_symbol)
	{
		if (this.pendingOverrides == null)
		{
			this.ApplySymbolOverride(overridden_symbol_name, batchSource, new_symbol);
			this.dirtyBuild = true;
			this.forceRebuild = true;
		}
		else
		{
			KAnimControllerBase.PendingOverride pendingOverride;
			pendingOverride.overridden_symbol_name = overridden_symbol_name;
			pendingOverride.batchSource = batchSource;
			pendingOverride.new_symbol = new_symbol;
			this.pendingOverrides.Add(pendingOverride);
		}
	}

	public void RemoveSymbolOverride(KAnimHashedString symbol_name)
	{
		this.ApplyRemoveOverride(symbol_name);
		this.dirtyBuild = true;
		this.forceRebuild = true;
	}

	public void ClearOverrides()
	{
		this.ApplyClearOverrides();
		this.visibleSymbols.Clear();
		this.dirtyBuild = true;
		this.forceRebuild = true;
	}

	public void AddBuildOverride(KAnimFile override_file, bool enable_symbols)
	{
		if (override_file == null)
		{
			return;
		}
		KAnimFileData data = override_file.GetData();
		this.AddBuildOverride(data, enable_symbols);
	}

	public void AddBuildOverride(KAnimFileData kafd, bool enable_symbols)
	{
		for (int i = 0; i < kafd.build.symbols.Length; i++)
		{
			this.AddSymbolOverride(kafd.build.symbols[i].hash, kafd.build.batchTag, kafd.build.symbols[i]);
			if (enable_symbols)
			{
				this.ShowSymbol(kafd.build.symbols[i].hash);
			}
		}
	}

	public void ClearBuildOverride(KAnimFile override_file, bool enable_symbols)
	{
		if (override_file == null)
		{
			return;
		}
		KAnimFileData data = override_file.GetData();
		this.ClearBuildOverride(data, enable_symbols);
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
		if (!this.baseHiddenSymbols.Contains(symbol))
		{
			this.baseHiddenSymbols.Add(symbol);
			this.forceRebuild = true;
			this.dirtyBuild = true;
			if (update_hidden)
			{
				this.UpdateHidden(true);
			}
			return true;
		}
		return false;
	}

	public bool StopHidingSymbol(KAnimHashedString symbol, bool update_hidden = true)
	{
		if (this.baseHiddenSymbols == null)
		{
			return false;
		}
		if (this.baseHiddenSymbols.Contains(symbol))
		{
			this.baseHiddenSymbols.Remove(symbol);
			this.forceRebuild = true;
			this.dirtyBuild = true;
			if (update_hidden)
			{
				this.UpdateHidden(true);
			}
			return true;
		}
		return false;
	}

	public void ShowSymbol(KAnimHashedString symbol)
	{
		if (!this.visibleSymbols.Contains(symbol))
		{
			this.visibleSymbols.Add(symbol);
			this.forceRebuild = true;
			this.dirtyBuild = true;
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
		if (kanim_file == null)
		{
			return;
		}
		for (int i = 0; i < this.overrideAnimFiles.Count; i++)
		{
			if (this.overrideAnimFiles[i].file == kanim_file)
			{
				Debug.LogWarning("Duplicate animation file [" + kanim_file.name + "] being overridden");
			}
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
		if (kanim_file == null)
		{
			return;
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
			if (data.build != null)
			{
				this.AddBuildOverride(data, true);
			}
			for (int j = 0; j < data.anims.Length; j++)
			{
				KAnimControllerBase.AnimLookupData animLookupData = default(KAnimControllerBase.AnimLookupData);
				animLookupData.animFile = data;
				animLookupData.anim = data.anims[j];
				HashedString hashedString = new HashedString(data.anims[j].name);
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
		if (build == null || build.symbols == null)
		{
			return;
		}
		for (int i = 0; i < build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = build.symbols[i];
			if (symbol.HasFlag(KAnim.SymbolFlags.SnapTo))
			{
				this.hiddenSymbols.Add(symbol.hash);
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
			this.forceRebuild = true;
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
		this.Rebuild();
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
			Debug.LogError("AddAnims() Null animfile data");
			return;
		}
		this.maxSymbols = Mathf.Max(this.maxSymbols, animFile.maxVisSymbolFrames);
		if (animFile.anims != null)
		{
			for (int i = 0; i < animFile.anims.Length; i++)
			{
				KAnim.Anim anim = animFile.anims[i];
				this.anims[anim.hash] = new KAnimControllerBase.AnimLookupData
				{
					animFile = animFile,
					anim = anim
				};
			}
		}
		if (animFile.build != null && animFile.build.symbols != null && animFile.build.symbols.Length > 0 && this.curBuild == null)
		{
			this.curBuild = animFile.build;
			this.dirtyBuild = true;
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

	public void AddAnims(params KAnimFile[] addedAnims)
	{
		List<KAnimFile> list = new List<KAnimFile>(this.animFiles.Length + addedAnims.Length);
		list.AddRange(this.animFiles);
		for (int i = 0; i < addedAnims.Length; i++)
		{
			if (addedAnims[i] != null && !list.Contains(addedAnims[i]))
			{
				list.Add(addedAnims[i]);
				this.AddAnims(addedAnims[i].GetData());
			}
		}
		this.animFiles = list.ToArray();
		this.dirtyBuild = true;
	}

	public void SetAnims(KAnimFile[] setAnims, bool dobind = true)
	{
		this.dirtyBuild = true;
		this.animFiles = new KAnimFile[0];
		this.AddAnims(setAnims);
		if (dobind)
		{
			this.BindAnims();
		}
	}

	public void Stop()
	{
		if (this.curAnim != null && Global.Instance != null && Global.Instance.GetAnimEventManager() != null)
		{
			this.eventManagerHandle = Global.Instance.GetAnimEventManager().StopAnim(this.eventManagerHandle);
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
		this.UpdateBounds();
	}

	private void UpdateBounds()
	{
		if (this.OnUpdateBounds != null)
		{
			this.OnUpdateBounds(this.bounds);
		}
	}

	public void SetPositionPercent(float percent)
	{
		if (this.curAnim == null)
		{
			return;
		}
		this.elapsedTime = (float)this.curAnim.numFrames / this.curAnim.frameRate * percent;
		this.forceRebuild = true;
		this.SuspendUpdates(false);
	}

	protected void StartAnimEventSequence()
	{
		if (Global.Instance != null && Global.Instance.GetAnimEventManager() != null && !this.layering.GetIsForeground())
		{
			this.eventManagerHandle = Global.Instance.GetAnimEventManager().PlayAnim(this, this.curAnim, this.mode, this.elapsedTime, this.visibilityType == KAnimControllerBase.VisibilityType.Always);
		}
	}

	protected void StopAnimEventSequence()
	{
		if (this.eventManagerHandle != -1 && Global.Instance != null && Global.Instance.GetAnimEventManager() != null)
		{
			if (!this.stopped && this.mode != KAnim.PlayMode.Paused)
			{
				this.elapsedTime = Global.Instance.GetAnimEventManager().GetElapsedTime(this.eventManagerHandle);
			}
			this.eventManagerHandle = Global.Instance.GetAnimEventManager().StopAnim(this.eventManagerHandle);
		}
	}

	public void DestroySelf()
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
	public KAnimBatchGroup.MaterialType materialType;

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
	public bool inactiveDisable;

	protected KAnimFileData curAnimFile;

	protected KAnim.Anim curAnim;

	protected int curAnimFrameIdx = KAnim.Anim.Frame.InvalidFrame.idx;

	protected int prevAnimFrame = KAnim.Anim.Frame.InvalidFrame.idx;

	protected KAnim.Build curBuild;

	protected int eventManagerHandle = -1;

	protected List<KAnimControllerBase.OverrideAnimFileData> overrideAnimFiles = new List<KAnimControllerBase.OverrideAnimFileData>();

	protected DeepProfiler DeepProfiler = new DeepProfiler(false);

	public bool randomiseLoopedOffset;

	protected float elapsedTime;

	protected float playSpeed = 1f;

	protected KAnim.PlayMode mode = KAnim.PlayMode.Once;

	protected bool stopped = true;

	protected bool flip;

	protected bool dirtyBuild;

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

	protected Color32 temperatureColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	[NonSerialized]
	public KAnimControllerBase.VisibilityType visibilityType;

	public Action<GameObject> onDestroySelf;

	[NonSerialized]
	protected bool forceRebuild;

	protected LoggerFSSS log;

	protected List<KAnimHashedString> baseHiddenSymbols;

	protected List<KAnimHashedString> hiddenSymbols = new List<KAnimHashedString>();

	protected List<KAnimHashedString> visibleSymbols = new List<KAnimHashedString>();

	protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> anims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();

	protected Dictionary<HashedString, KAnimControllerBase.AnimLookupData> overrideAnims = new Dictionary<HashedString, KAnimControllerBase.AnimLookupData>();

	protected Queue<KAnimControllerBase.AnimData> animQueue = new Queue<KAnimControllerBase.AnimData>();

	protected int maxSymbols;

	public Grid.SceneLayer fgLayer = Grid.SceneLayer.NoLayer;

	private float movementSpeedMultiplier = 1f;

	private static HashedString snaptoPivot = new HashedString("snapTo_pivot");

	private List<KAnimControllerBase.PendingOverride> pendingOverrides = new List<KAnimControllerBase.PendingOverride>();

	private List<KAnimControllerBase.PendingOverride> appliedOverrides;

	public struct OverrideAnimFileData
	{
		public float priority;

		public KAnimFile file;
	}

	public struct AnimLookupData
	{
		public KAnimFileData animFile;

		public KAnim.Anim anim;
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

	private struct PendingOverride
	{
		public KAnimHashedString overridden_symbol_name;

		public HashedString batchSource;

		public KAnim.Build.Symbol new_symbol;
	}

	public delegate void KAnimEvent(HashedString name);
}
