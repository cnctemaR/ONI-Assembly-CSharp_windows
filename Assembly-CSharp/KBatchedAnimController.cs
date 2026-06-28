using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[DebuggerDisplay("{name} visible={visible} suspendUpdates={suspendUpdates} moving={moving}")]
public class KBatchedAnimController : KAnimControllerBase, KAnimConverter.IAnimConverter
{
	public int GetCurrentFrameIndex()
	{
		return this.curAnimFrameIdx;
	}

	public Color32 GetHighlightColour()
	{
		return this.highlightColour;
	}

	public Color32 GetFirstTintColour()
	{
		return this.tintColour;
	}

	public Color32 GetSecondTintColour()
	{
		return this.secondTintColour;
	}

	public Color32 GetOverlayColour()
	{
		return this.overlayColour;
	}

	public int GetFirstTintIndex()
	{
		return this.firstTintIndex;
	}

	public int GetSecondTintIndex()
	{
		return this.secondTintIndex;
	}

	public int GetSymbolScaleIndex()
	{
		return this.symbolScaleIndex;
	}

	public float GetSymbolScale()
	{
		return this.symbolScale;
	}

	public KBatchedAnimInstanceData GetBatchInstanceData()
	{
		return this.batchInstanceData;
	}

	protected bool forceRebuild
	{
		get
		{
			return this._forceRebuild;
		}
		set
		{
			this._forceRebuild = value;
		}
	}

	public void SetRenderQueueOverride(int value)
	{
		this.renderQueueOverride = value;
	}

	public void UnsetRenderQueueOverride()
	{
		this.renderQueueOverride = this.originalRenderQueue;
	}

	public bool hasLoadedAnims { get; private set; }

	public bool IsActive()
	{
		return base.enabled;
	}

	public bool IsVisible()
	{
		return this.visible;
	}

	private int GetSymbolIndex(KAnimHashedString name)
	{
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.GetBatchGroupID(false), false);
		int symbolIndex = batchGroupData.GetSymbolIndex(name, this.curAnimFile.hashName);
		if (symbolIndex == -1)
		{
			global::Debug.LogWarning("Couldn't set tint for [" + name + "] - not found", null);
			return -1;
		}
		return symbolIndex;
	}

	public void SetSymbolTint(KBatchedAnimController.SymbolTintIndex stIdx, KAnimHashedString name, Color32 colour)
	{
		int symbolIndex = this.GetSymbolIndex(name);
		if (symbolIndex < 0)
		{
			return;
		}
		if (stIdx == KBatchedAnimController.SymbolTintIndex.First)
		{
			this.firstTintIndex = symbolIndex;
			this.tintColour = colour;
		}
		else
		{
			this.secondTintIndex = symbolIndex;
			this.secondTintColour = colour;
		}
		this.SuspendUpdates(false);
		this.SetDirty();
	}

	public void UnsetSymbolTint(KBatchedAnimController.SymbolTintIndex stIdx)
	{
		if (stIdx == KBatchedAnimController.SymbolTintIndex.First)
		{
			this.firstTintIndex = -1;
			this.tintColour = Color.white;
		}
		else
		{
			this.secondTintIndex = -1;
			this.secondTintColour = Color.white;
		}
		this.SuspendUpdates(false);
		this.SetDirty();
	}

	public void SetSymbolScale(KAnimHashedString name, float scale)
	{
		this.symbolScale = 1f;
		this.symbolScaleIndex = -1;
		int symbolIndex = this.GetSymbolIndex(name);
		if (symbolIndex < 0)
		{
			return;
		}
		this.symbolScaleIndex = symbolIndex;
		this.symbolScale = scale;
		this.SuspendUpdates(false);
		this.SetDirty();
	}

	public void UnsetSymbolScale()
	{
		if (this.symbolScaleIndex >= 0)
		{
			this.symbolScale = 1f;
			this.symbolScaleIndex = -1;
			this.SuspendUpdates(false);
			this.SetDirty();
		}
	}

	public Vector2I GetCellXY()
	{
		if (Grid.CellSizeInMeters == 0f)
		{
			return new Vector2I((int)base.transform.position.x, (int)base.transform.position.y);
		}
		return Grid.PosToXY(base.transform.position);
	}

	public float GetZ()
	{
		return base.transform.position.z;
	}

	public string GetName()
	{
		return base.name;
	}

	public override KAnim.Anim GetAnim(int index)
	{
		if (!this.batchGroupID.isValid || !(this.batchGroupID != KAnimBatchManager.NO_BATCH))
		{
			global::Debug.LogError(base.name + " batch not ready", null);
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.batchGroupID, false);
		return batchGroupData.GetAnim(index);
	}

	private void Initialize()
	{
		if (this.batchGroupID.isValid && this.batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			this.DeRegister();
			this.Register();
		}
	}

	public override void AddAnims(params KAnimFile[] addedAnims)
	{
		List<KAnimFile> list = new List<KAnimFile>(this.animFiles.Length + addedAnims.Length);
		list.AddRange(this.animFiles);
		for (int i = 0; i < addedAnims.Length; i++)
		{
			if (addedAnims[i] != null && !list.Contains(addedAnims[i]))
			{
				KAnimFileData data = addedAnims[i].GetData();
				if (data.buildIndex != -1)
				{
					this.SetBatchGroup(data);
				}
				list.Add(addedAnims[i]);
				base.AddAnims(data);
			}
		}
		this.animFiles = list.ToArray();
		this.dirtyBuild = true;
	}

	public void OnObjectMovementStateChanged(int new_state, int ignore_me)
	{
		bool flag = -97592435 == new_state;
		if (flag == this.moving)
		{
			return;
		}
		this.moving = flag;
		base.MarkDirty();
		this.ConfigureUpdateListener();
	}

	private void SetBatchGroup(KAnimFileData kafd)
	{
		if (!this.batchGroupID.isValid || this.batchGroupID == KAnimBatchManager.NO_BATCH)
		{
			KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(kafd.build.batchTag);
			if (group.renderType == KAnimBatchGroup.RendererType.DontRender || group.renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				this.batchGroupID = group.swapTarget;
				return;
			}
			this.batchGroupID = kafd.build.batchTag;
		}
	}

	public void LoadAnims()
	{
		this.overrideAnims.Clear();
		this.anims.Clear();
		if (this.animFiles != null)
		{
			List<KAnimFile> list = new List<KAnimFile>(this.animFiles);
			list = list.FindAll((KAnimFile f) => f != null);
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].buildFile != null)
					{
						KAnimFileData data = list[i].GetData();
						this.SetBatchGroup(data);
						base.AddAnims(data);
						list.RemoveAt(i);
						break;
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					base.AddAnims(list[j].GetData());
				}
				this.forceRebuild = true;
				this.dirtyBuild = true;
				base.UpdateSymbolLookups();
				this.hasLoadedAnims = true;
			}
			else
			{
				Output.LogWarning(new object[] { "Missing anim files for", base.name, base.gameObject });
			}
		}
	}

	public void UpdateAnim(float dt)
	{
		if (this.batch != null && this.isMovable && base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			if (this.batch != null && this.batch.group.maxGroupSize == 1 && this.lastPos.z != base.transform.position.z)
			{
				this.batch.OverrideZ(base.transform.position.z);
			}
			Vector3 position = base.transform.position;
			this.lastPos = position;
			if (this.visibilityType != KAnimControllerBase.VisibilityType.Always)
			{
				Vector2I vector2I = KBatchedAnimUpdater.PosToChunkXY(position);
				if (vector2I != this.lastChunkXY)
				{
					if (this.lastChunkXY != Vector2I.minusone)
					{
						this.UnregisterVisibilityListener(this.lastChunkXY);
						if (this.batch != null)
						{
							if (!this.batch.group.isMultiInstance)
							{
								this.DeRegister();
							}
							else if (this.lastChunkXY != Vector2I.zero)
							{
								this.MoveToNewChunk(vector2I);
							}
						}
					}
					this.RegisterVisibilityListener(vector2I);
				}
			}
			base.MarkDirty();
		}
		if (this.batchGroupID == KAnimBatchManager.NO_BATCH || !base.isActiveAndEnabled || (!this.visible && !this.forceRebuild))
		{
			return;
		}
		if (this.batch == null)
		{
			if (this.batchGroupInstance == null)
			{
				this.forceRebuild = true;
			}
			this.Register();
		}
		if (!this.forceRebuild && (this.mode == KAnim.PlayMode.Paused || this.stopped || this.curAnim == null || (this.mode == KAnim.PlayMode.Once && this.curAnim != null && (this.elapsedTime > this.curAnim.totalTime || this.curAnim.totalTime <= 0f) && this.animQueue.Count == 0)))
		{
			this.SuspendUpdates(true);
		}
		this.curAnimFrameIdx = base.GetFrameIdx(this.elapsedTime, true);
		if (this.eventManagerHandle != -1 && this.aem != null)
		{
			float elapsedTime = this.aem.GetElapsedTime(this.eventManagerHandle);
			if ((int)((this.elapsedTime - elapsedTime) * 100f) != 0)
			{
				base.UpdateAnimEventSequenceTime();
			}
		}
		this.UpdateFrame(this.elapsedTime);
		if (!this.stopped && this.mode != KAnim.PlayMode.Paused)
		{
			this.elapsedTime += dt * this.playSpeed;
		}
		if (this.forceRebuild && this.batch != null)
		{
			this.ReApplyOverrides();
			this.RebuildBatchGroupInstance();
			this.forceRebuild = false;
		}
	}

	protected override void UpdateFrame(float t)
	{
		base.previousFrame = base.currentFrame;
		if (!this.stopped || this.forceRebuild)
		{
			if (this.curAnim != null && (this.mode == KAnim.PlayMode.Loop || this.elapsedTime <= base.GetDuration() || this.forceRebuild))
			{
				base.currentFrame = this.curAnim.GetFrameIdx(this.mode, this.elapsedTime);
				if (base.currentFrame != base.previousFrame || this.forceRebuild)
				{
					this.SetDirty();
				}
			}
			else
			{
				this.TriggerStop();
			}
			if (!this.stopped && this.mode == KAnim.PlayMode.Loop && base.currentFrame == 0)
			{
				base.AnimEnter(this.curAnim.hash);
			}
		}
	}

	public void TriggerStop()
	{
		if (this.animQueue.Count > 0)
		{
			base.StartQueuedAnim();
		}
		else if (this.curAnim != null && this.mode == KAnim.PlayMode.Once)
		{
			base.currentFrame = this.curAnim.numFrames - 1;
			base.Stop();
			EventSystem.Trigger(base.gameObject, -1061186183, null);
			if (this.destroyOnAnimComplete)
			{
				base.DestroySelf();
			}
		}
	}

	public override void UpdateHidden(bool reset = true)
	{
		if (this.curBuild == null || this.batchInstanceData == null)
		{
			return;
		}
		if (this.hiddenSymbols == null && this.baseHiddenSymbols == null)
		{
			return;
		}
		if (reset)
		{
			this.batchInstanceData.ResetHidden();
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.GetBatchGroupID(false), false);
		if (batchGroupData == null || batchGroupData.firstSymbolIndex == null)
		{
			return;
		}
		KAnimHashedString buildHash = base.GetBuildHash();
		if (!batchGroupData.firstSymbolIndex.ContainsKey(buildHash))
		{
			return;
		}
		int num = batchGroupData.firstSymbolIndex[buildHash];
		int num2 = num + this.curBuild.symbols.Length;
		for (int i = num; i < num2; i++)
		{
			int num3 = i / 30;
			if (num3 >= 4)
			{
				break;
			}
			KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(i);
			if (this.visibleSymbols.Contains(symbol.hash))
			{
				this.batchInstanceData.UnsetHiddenBit(i);
			}
			else if ((this.hiddenSymbols != null && this.hiddenSymbols.Contains(symbol.hash)) || (this.baseHiddenSymbols != null && this.baseHiddenSymbols.Contains(symbol.hash)))
			{
				this.batchInstanceData.SetHiddenBit(i);
			}
		}
		this.SetDirty();
	}

	public void ForceHideAll()
	{
		if (this.batchInstanceData != null)
		{
			this.batchInstanceData.HideAll();
			this.SetDirty();
		}
	}

	public int GetMaxVisible()
	{
		return this.maxSymbols;
	}

	public HashedString batchGroupID { get; private set; }

	public HashedString GetBatchGroupID(bool isEditorWindow = false)
	{
		return this.batchGroupID;
	}

	public int GetLayer()
	{
		return base.gameObject.layer;
	}

	public KAnimBatch GetBatch()
	{
		return this.batch;
	}

	public void SetBatch(KAnimBatch newBatch)
	{
		this.batch = newBatch;
		if (newBatch != null && this.batchGroupInstance == null)
		{
			this.ReApplyOverrides();
			this.batchGroupInstance = newBatch.batchGroupInstance;
		}
		if (this.materialType == KAnimBatchGroup.MaterialType.UI)
		{
			KBatchedAnimCanvasRenderer kbatchedAnimCanvasRenderer = base.GetComponent<KBatchedAnimCanvasRenderer>();
			if (kbatchedAnimCanvasRenderer == null && newBatch != null)
			{
				kbatchedAnimCanvasRenderer = base.gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
			}
			if (kbatchedAnimCanvasRenderer != null)
			{
				kbatchedAnimCanvasRenderer.SetBatch(this);
			}
		}
		this.SetBatchGroupRenderQueueOverride();
	}

	public int GetCurrentNumFrames()
	{
		return (this.curAnim == null) ? 0 : this.curAnim.numFrames;
	}

	public int GetFirstFrameIndex()
	{
		return (this.curAnim == null) ? (-1) : this.curAnim.firstFrameIdx;
	}

	protected override Matrix4x4 GetRootMatrix()
	{
		return this.GetTransformMatrix();
	}

	private Canvas GetRootCanvas()
	{
		if (this.rt == null)
		{
			return null;
		}
		RectTransform rectTransform = this.rt.parent.GetComponent<RectTransform>();
		while (rectTransform != null)
		{
			Canvas component = rectTransform.GetComponent<Canvas>();
			if (component != null && component.isRootCanvas)
			{
				return component;
			}
			rectTransform = rectTransform.parent.GetComponent<RectTransform>();
		}
		return null;
	}

	public override Matrix4x4 GetTransformMatrix()
	{
		Vector3 vector = base.transform.position + this.offset;
		vector.z = 0f;
		Vector3 vector2 = new Vector3(this.animScale * this.animWidth, -this.animScale * this.animHeight, this.animScale);
		if (this.materialType == KAnimBatchGroup.MaterialType.UI)
		{
			this.rt = base.GetComponent<RectTransform>();
			if (this.rootCanvas == null)
			{
				this.rootCanvas = this.GetRootCanvas();
			}
			if (this.scaler == null && this.rootCanvas != null)
			{
				this.scaler = this.rootCanvas.GetComponent<CanvasScaler>();
			}
			if (this.rootCanvas == null)
			{
				this.screenOffset.x = (float)(Screen.width / 2);
				this.screenOffset.y = (float)(Screen.height / 2);
			}
			else
			{
				this.screenOffset.x = this.rootCanvas.rectTransform().rect.width / 2f;
				this.screenOffset.y = this.rootCanvas.rectTransform().rect.height / 2f;
			}
			float num = 1f;
			if (this.scaler != null)
			{
				num = 1f / this.scaler.scaleFactor;
			}
			vector = (this.rt.localToWorldMatrix.MultiplyPoint(this.rt.pivot) + this.offset) * num - this.screenOffset;
			float num2 = this.animWidth * this.animScale;
			float num3 = this.animHeight * this.animScale;
			if (this.setScaleFromAnim && this.curAnim != null)
			{
				num2 *= this.rt.rect.size.x / this.curAnim.unScaledSize.x;
				num3 *= this.rt.rect.size.y / this.curAnim.unScaledSize.y;
			}
			else
			{
				num2 *= this.rt.rect.size.x / this.animOverrideSize.x;
				num3 *= this.rt.rect.size.y / this.animOverrideSize.y;
			}
			vector2 = new Vector3(this.rt.lossyScale.x * num2, -this.rt.lossyScale.y * num3, this.rt.lossyScale.z);
			this.pivot = this.rt.pivot;
		}
		Matrix4x4 matrix4x = Matrix4x4.Scale(vector2);
		if (this.flipX || this.flipY)
		{
			Vector3 vector3 = new Vector3((float)((!this.flipX) ? 1 : (-1)), (float)((!this.flipY) ? 1 : (-1)), 1f);
			Matrix4x4 matrix4x2 = Matrix4x4.TRS(-this.pivot, Quaternion.identity, Vector3.one);
			Matrix4x4 matrix4x3 = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, vector3);
			Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one);
			matrix4x = matrix4x4 * matrix4x3 * matrix4x2 * matrix4x;
		}
		Matrix4x4 matrix4x9;
		if (this.rotation != 0f)
		{
			Quaternion quaternion = Quaternion.Euler(0f, 0f, this.rotation);
			Matrix4x4 matrix4x5 = Matrix4x4.TRS(-this.pivot, Quaternion.identity, Vector3.one);
			Matrix4x4 matrix4x6 = Matrix4x4.TRS(Vector3.zero, quaternion, Vector3.one);
			Matrix4x4 matrix4x7 = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one);
			this.rotationMatrix = matrix4x7 * matrix4x6 * matrix4x5;
			Matrix4x4 matrix4x8 = Matrix4x4.TRS(vector, base.transform.localRotation, base.transform.localScale);
			matrix4x9 = matrix4x8 * this.rotationMatrix * matrix4x;
		}
		else
		{
			Matrix4x4 matrix4x10 = Matrix4x4.TRS(vector, base.transform.localRotation, base.transform.localScale);
			matrix4x9 = matrix4x10 * matrix4x;
		}
		return matrix4x9;
	}

	public override Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
	{
		if (this.curAnimFrameIdx != -1 && this.batch != null)
		{
			Matrix2x3 symbolLocalTransform = this.GetSymbolLocalTransform(symbol, out symbolVisible);
			if (symbolVisible)
			{
				Matrix4x4 rootMatrix = this.GetRootMatrix();
				return rootMatrix * symbolLocalTransform;
			}
		}
		symbolVisible = false;
		return default(Matrix4x4);
	}

	public override Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible)
	{
		if (this.curAnimFrameIdx != -1 && this.batch != null)
		{
			KAnim.Anim.Frame frame = this.batch.group.data.GetFrame(this.curAnimFrameIdx);
			if (frame != KAnim.Anim.Frame.InvalidFrame)
			{
				for (int i = 0; i < frame.numElements; i++)
				{
					int num = frame.firstElementIdx + i;
					if (num < this.batch.group.data.frameElements.Count)
					{
						KAnim.Anim.FrameElement frameElement = this.batch.group.data.frameElements[num];
						if (frameElement.symbol == symbol)
						{
							symbolVisible = true;
							return frameElement.transform;
						}
					}
				}
			}
		}
		symbolVisible = false;
		return Matrix2x3.identity;
	}

	public void SetBuild(string build, KAnimHashedString symbolName)
	{
		for (int i = 0; i < this.animFiles.Length; i++)
		{
			if (this.animFiles[i].GetData().build.name == build)
			{
				this.curBuild = this.animFiles[i].GetData().build;
				KAnim.Build.Symbol symbol = this.curBuild.GetSymbol(symbolName);
				if (symbol == null)
				{
					symbol = this.curBuild.GetSymbolByIndex(0U);
				}
				this.curAnim = new KAnim.Anim(this.animFiles[i].GetData(), 0);
				this.curAnim.firstFrameIdx = symbol.firstFrameIdx;
				this.curAnim.numFrames = 1;
				this.curAnimFrameIdx = symbol.GetFrameIdx(0);
				this.dirtyBuild = true;
				this.Initialize();
				break;
			}
		}
		this.mode = KAnim.PlayMode.Paused;
	}

	public void SetBatchGroupRenderQueueOverride()
	{
		if (this.batch != null)
		{
			if (this.originalRenderQueue < 0)
			{
				this.originalRenderQueue = this.batch.group.material.renderQueue;
			}
			this.batch.group.material.renderQueue = this.renderQueueOverride;
		}
	}

	public override void SetLayer(int layer)
	{
		if (this.batch != null && this.batch.layer == layer)
		{
			return;
		}
		base.SetLayer(layer);
		this.DeRegister();
		base.gameObject.layer = layer;
		this.Register();
	}

	public override void SetDirty()
	{
		if (this.batch != null)
		{
			this.batch.SetDirty(this);
		}
	}

	protected override void OnStartQueuedAnim()
	{
		this.SuspendUpdates(false);
	}

	protected override void OnAwake()
	{
		this.batchInstanceData = new KBatchedAnimInstanceData(this);
		this.LoadAnims();
		if (this.visibilityType == KAnimControllerBase.VisibilityType.Default)
		{
			this.visibilityType = ((this.materialType != KAnimBatchGroup.MaterialType.UI) ? this.visibilityType : KAnimControllerBase.VisibilityType.Always);
		}
	}

	protected override void OnStart()
	{
		if (this.batch == null)
		{
			this.Initialize();
		}
		if (this.visibilityType == KAnimControllerBase.VisibilityType.Always)
		{
			this.ConfigureUpdateListener();
		}
		this.ConfigureVisibilityListener(true);
		if (this.isMovable)
		{
			CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnObjectMovementStateChanged), true);
		}
		this.SetDirty();
	}

	protected override void OnStop()
	{
		this.SetDirty();
	}

	private void OnEnable()
	{
		if (this.batch == null)
		{
			this.Initialize();
		}
		this.SetDirty();
		this.SuspendUpdates(false);
		this.ConfigureVisibilityListener(true);
		if (!this.stopped && this.curAnim != null && this.mode != KAnim.PlayMode.Paused && this.eventManagerHandle == -1)
		{
			base.StartAnimEventSequence();
		}
	}

	private void OnDisable()
	{
		if (App.IsExiting || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		this.SuspendUpdates(true);
		if (this.batch != null)
		{
			this.DeRegister();
		}
		this.ConfigureVisibilityListener(false);
		base.StopAnimEventSequence();
	}

	protected override void OnDestroy()
	{
		if (this.isMovable)
		{
			CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnObjectMovementStateChanged), true);
		}
		if (this.updateListenerRegistered)
		{
			KBatchedAnimUpdater.instance.UpdateUnregister(this);
			this.updateListenerRegistered = false;
		}
		if (this.visibilityType != KAnimControllerBase.VisibilityType.Always)
		{
			Vector2I vector2I = Grid.PosToXY(base.transform.position);
			KBatchedAnimUpdater.instance.VisibilityUnregister(vector2I, this);
		}
		this.visible = false;
		this.DeRegister();
		this.stopped = true;
		base.StopAnimEventSequence();
		this.batchInstanceData = null;
		this.batch = null;
		this.pendingSymbolOverrides = null;
		this.appliedSymbolOverrides = null;
		this.substituteFrames = null;
		base.OnDestroy();
	}

	public void SetBlendValue(float value)
	{
		this.batchInstanceData.SetBlend(value);
		this.SetDirty();
	}

	private void GetSubstituteFrames(KAnim.Build.Symbol substituteSymbol, List<KAnim.Build.SymbolFrameInstance> substituteFrames, List<Texture2D> textures)
	{
		KBatchGroupData batch_group = KAnimBatchManager.Instance().GetBatchGroupData(substituteSymbol.build.batchTag, false);
		for (int i = 0; i < substituteSymbol.numFrames; i++)
		{
			KAnim.Build.SymbolFrameInstance sfi = batch_group.symbolFrameInstances[substituteSymbol.firstFrameIdx + i];
			int num = textures.FindIndex((Texture2D t) => t == batch_group.textures[sfi.buildImageIdx]);
			if (num == -1)
			{
				num = textures.Count;
				textures.Add(batch_group.textures[sfi.buildImageIdx]);
			}
			sfi.buildImageIdx = num;
			substituteFrames.Add(sfi);
		}
	}

	protected override void ApplyClearOverrides()
	{
		if (this.batch != null && this.batch.batchGroupInstance != null)
		{
			this.batch.batchGroupInstance.ClearOverrides();
		}
	}

	protected override void ApplyRemoveSingleFrameOverride(KAnimHashedString symbol_name)
	{
		if (this.batch != null && this.batch.batchGroupInstance != null)
		{
			this.batch.batchGroupInstance.RemoveOverride(symbol_name);
		}
		this.forceRebuild = true;
	}

	public void ApplySingleFrameOverride(KAnimHashedString target_symbol, int target_frame, int override_frame)
	{
		if (this.batch != null && this.batch.batchGroupInstance != null)
		{
			this.batch.batchGroupInstance.SwapSymbolFrameInstance(target_symbol, target_frame, override_frame);
		}
	}

	public void RemoveSingleFrameOverride(KAnimHashedString target_symbol)
	{
		if (this.batch != null && this.batch.batchGroupInstance != null)
		{
			this.batch.batchGroupInstance.RemoveSwapSymbolFrameInstance(target_symbol);
		}
	}

	protected void ReApplySymbolOverrides()
	{
		if (this.pendingSymbolOverrides != null)
		{
			this.AcceptOverrides();
		}
		else if (this.appliedSymbolOverrides != null)
		{
			for (int i = 0; i < this.appliedSymbolOverrides.Count; i++)
			{
				if (!KBatchedAnimController.skipList.Contains(this.appliedSymbolOverrides[i].overridden_symbol_name.HashValue))
				{
					this.ApplySymbolOverride(this.appliedSymbolOverrides[i].overridden_symbol_name, this.appliedSymbolOverrides[i].batchSource, this.appliedSymbolOverrides[i].new_symbol, this.appliedSymbolOverrides[i].is_perminent);
				}
			}
			this.dirtyBuild = true;
			this.forceRebuild = true;
		}
	}

	protected void AcceptOverrides()
	{
		if (this.pendingSymbolOverrides != null)
		{
			for (int i = 0; i < this.pendingSymbolOverrides.Count; i++)
			{
				this.ApplySymbolOverride(this.pendingSymbolOverrides[i].overridden_symbol_name, this.pendingSymbolOverrides[i].batchSource, this.pendingSymbolOverrides[i].new_symbol, this.pendingSymbolOverrides[i].is_perminent);
			}
			this.appliedSymbolOverrides = this.pendingSymbolOverrides;
			this.pendingSymbolOverrides = null;
			this.dirtyBuild = true;
			this.forceRebuild = true;
		}
	}

	public override void AddSymbolOverride(KAnimHashedString overridden_symbol_name, HashedString batchSource, KAnim.Build.Symbol new_symbol, bool is_perminent = false)
	{
		if (this.pendingSymbolOverrides == null)
		{
			this.ApplySymbolOverride(overridden_symbol_name, batchSource, new_symbol, is_perminent);
		}
		else
		{
			KBatchedAnimController.PendingSymbolOverride pendingSymbolOverride;
			pendingSymbolOverride.overridden_symbol_name = overridden_symbol_name;
			pendingSymbolOverride.batchSource = batchSource;
			pendingSymbolOverride.new_symbol = new_symbol;
			pendingSymbolOverride.is_perminent = is_perminent;
			this.pendingSymbolOverrides.Add(pendingSymbolOverride);
		}
		this.dirtyBuild = true;
		this.forceRebuild = true;
	}

	protected override void RemoveSymbolOverride(KAnimFile kanim_file)
	{
		if (this.batch != null && this.batch.batchGroupInstance != null)
		{
			KAnimFileData data = kanim_file.GetData();
			if (data.buildIndex != -1)
			{
				for (int i = 0; i < data.build.symbols.Length; i++)
				{
					this.batch.batchGroupInstance.RemoveOverride(data.build.symbols[i].hash);
				}
			}
		}
	}

	protected override void ApplySymbolOverride(KAnimHashedString overridden_symbol_name, HashedString batchSource, KAnim.Build.Symbol new_symbol, bool is_perminent)
	{
		if (this.batch != null && this.batch.batchGroupInstance != null)
		{
			if (batchSource == this.batch.group.batchID)
			{
				return;
			}
			KAnim.Build.Symbol buildSymbol = this.batch.group.data.GetBuildSymbol(overridden_symbol_name);
			if (buildSymbol == null || buildSymbol.numFrames == 0)
			{
				return;
			}
			this.substituteFrames.Clear();
			List<Texture2D> list = new List<Texture2D>();
			this.GetSubstituteFrames(new_symbol, this.substituteFrames, list);
			if (this.substituteFrames.Count == 0)
			{
				global::Debug.LogWarning("substituteFrames == 0 for [" + overridden_symbol_name + "]", null);
				return;
			}
			List<int> list2 = new List<int>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < list.Count; i++)
			{
				int num = -1;
				bool flag = this.batch.batchGroupInstance.AddOverrideTexture(list[i], ref num);
				int num2 = this.batch.batchGroupInstance.textures.Count + this.batch.group.data.textures.Count;
				if (num >= KBatchedAnimCanvasRenderer.atlasNames.Length || num >= num2)
				{
					string text = string.Empty;
					int num3 = 0;
					for (int j = 0; j < this.batch.batchGroupInstance.group.data.textures.Count; j++)
					{
						if (this.batch.batchGroupInstance.group.data.textures[j] != null)
						{
							string text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"\n[",
								num3,
								"] [",
								this.batch.batchGroupInstance.group.data.textures[j].name,
								"]"
							});
						}
						else
						{
							string text2 = text;
							text = string.Concat(new object[] { text2, "\n[", num3, "] [NULL]" });
						}
						num3++;
					}
					for (int k = 0; k < this.batch.batchGroupInstance.textures.Count; k++)
					{
						if (this.batch.batchGroupInstance.textures[k] != null)
						{
							string text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"\n[",
								num3,
								"] [",
								this.batch.batchGroupInstance.textures[k].name,
								"]"
							});
						}
						else
						{
							string text2 = text;
							text = string.Concat(new object[] { text2, "\n[", num3, "] [NULL]" });
						}
						num3++;
					}
					global::Debug.LogError(string.Concat(new object[]
					{
						"[",
						this.batchGroupID.ToString(),
						"] Too many textures!  texIndex: [",
						num,
						"] tex count: [",
						num2,
						"] ",
						text
					}), null);
					return;
				}
				list2.Add(num);
				dictionary[i] = num;
				if (flag)
				{
					this.batch.matProperties.SetTexture(KBatchedAnimCanvasRenderer.atlasNames[num], list[i]);
				}
			}
			if (list2.Count > 0)
			{
				for (int l = 0; l < this.substituteFrames.Count; l++)
				{
					KAnim.Build.SymbolFrameInstance symbolFrameInstance = this.substituteFrames[l];
					symbolFrameInstance.buildImageIdx = dictionary[symbolFrameInstance.buildImageIdx];
					this.substituteFrames[l] = symbolFrameInstance;
				}
				this.batch.batchGroupInstance.AddOverride(overridden_symbol_name, batchSource, this.substituteFrames, list2, new_symbol.path, is_perminent);
			}
		}
	}

	protected override void RebuildBatchGroupInstance()
	{
		if (this.batch.batchGroupInstance.requiresRebuild)
		{
			try
			{
				this.batch.batchGroupInstance.Rebuild(this.batch.matProperties);
				if (this.materialType == KAnimBatchGroup.MaterialType.UI)
				{
					KBatchedAnimCanvasRenderer component = base.GetComponent<KBatchedAnimCanvasRenderer>();
					if (component != null)
					{
						component.SetBatch(this);
					}
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Output.LogErrorWithObj(base.gameObject, new object[] { string.Format("Updating symbol overrides on {0}: {1}\n{2}", this.GetName(), message, ex.StackTrace) });
			}
		}
	}

	public void ReApplyOverrides()
	{
		this.ReApplySymbolOverrides();
		base.ReApplyTempBuildOverrides();
	}

	private void Register()
	{
		if (this.batch != null)
		{
			return;
		}
		if (this.batchGroupID.isValid && this.batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			KAnimBatchManager.Instance().Register(this);
			this.forceRebuild = true;
			base.MarkDirty();
		}
	}

	private void MoveToNewChunk(Vector2I newChunkXY)
	{
		KAnimBatchManager.Instance().MoveChunk(this, this.lastChunkXY, newChunkXY);
	}

	private void DeRegister()
	{
		if (this.batch != null)
		{
			this.batch.Deregister(this);
		}
	}

	private void ConfigureUpdateListener()
	{
		if (this == null)
		{
			return;
		}
		bool flag = (base.isActiveAndEnabled && !this.suspendUpdates && this.visible) || this.moving || this.visibilityType == KAnimControllerBase.VisibilityType.Always;
		if (flag)
		{
			if (!this.updateListenerRegistered)
			{
				KBatchedAnimUpdater.instance.UpdateRegister(this);
				this.updateListenerRegistered = true;
			}
		}
		else if (this.updateListenerRegistered)
		{
			KBatchedAnimUpdater.instance.UpdateUnregister(this);
			this.updateListenerRegistered = false;
		}
	}

	protected override void SuspendUpdates(bool suspend)
	{
		this.suspendUpdates = suspend;
		this.ConfigureUpdateListener();
	}

	public void OnBecameInvisible()
	{
		this.visible = false;
		this.SuspendUpdates(true);
		this.SetDirty();
	}

	public void OnBecameVisible()
	{
		this.visible = true;
		this.SuspendUpdates(false);
		base.MarkDirty();
		base.UpdateAnimEventSequenceTime();
	}

	private void ConfigureVisibilityListener(bool enabled)
	{
		if (this.visibilityType == KAnimControllerBase.VisibilityType.Always)
		{
			return;
		}
		Vector2I vector2I = KBatchedAnimUpdater.PosToChunkXY(base.transform.position);
		if (enabled)
		{
			this.RegisterVisibilityListener(vector2I);
		}
		else
		{
			this.UnregisterVisibilityListener(vector2I);
		}
	}

	private void RegisterVisibilityListener(Vector2I chunk_xy)
	{
		if (!this.visibilityListenerRegistered)
		{
			KBatchedAnimUpdater.instance.VisibilityRegister(chunk_xy, this);
			this.lastChunkXY = chunk_xy;
			this.visibilityListenerRegistered = true;
		}
	}

	private void UnregisterVisibilityListener(Vector2I chunk_xy)
	{
		if (this.visibilityListenerRegistered)
		{
			KBatchedAnimUpdater.instance.VisibilityUnregister(chunk_xy, this);
			this.visibilityListenerRegistered = false;
		}
	}

	virtual KAnimFile[] KAnimConverter+IAnimConverter.GetAnims()
	{
		return base.GetAnims();
	}

	virtual KAnimBatchGroup.MaterialType KAnimConverter+IAnimConverter.GetMaterialType()
	{
		return base.GetMaterialType();
	}

	private BatchGroupInstance batchGroupInstance;

	[NonSerialized]
	protected bool _forceRebuild;

	private KBatchedAnimInstanceData batchInstanceData;

	private Vector3 lastPos = Vector3.zero;

	private Vector2I lastChunkXY = Vector2I.minusone;

	private Matrix4x4 rotationMatrix = Matrix4x4.identity;

	private KAnimBatch batch;

	public float animScale = 0.005f;

	private bool suspendUpdates;

	private bool updateListenerRegistered;

	private bool visibilityListenerRegistered;

	private bool moving;

	public int renderQueueOverride = -1;

	[NonSerialized]
	public int originalRenderQueue = -1;

	public Grid.SceneLayer sceneLayer;

	private RectTransform rt;

	private Vector3 screenOffset = new Vector3(0f, 0f, 0f);

	private CanvasScaler scaler;

	public bool setScaleFromAnim = true;

	public Vector2 animOverrideSize = Vector2.one;

	private Canvas rootCanvas;

	private List<KBatchedAnimController.PendingSymbolOverride> pendingSymbolOverrides = new List<KBatchedAnimController.PendingSymbolOverride>();

	private List<KBatchedAnimController.PendingSymbolOverride> appliedSymbolOverrides;

	private List<KAnim.Build.SymbolFrameInstance> substituteFrames = new List<KAnim.Build.SymbolFrameInstance>();

	public bool isMovable;

	protected Color32 secondTintColour = Color.white;

	private int firstTintIndex = -1;

	private int secondTintIndex = -1;

	private int symbolScaleIndex = -1;

	private float symbolScale = 1f;

	private static List<int> skipList = new List<int>(new int[]
	{
		KCompBuilder.snapTo_eyes.HashValue,
		KCompBuilder.snapTo_hair.HashValue,
		KCompBuilder.snapTo_headshape.HashValue,
		KCompBuilder.snapTo_mouth.HashValue
	});

	private struct PendingSymbolOverride
	{
		public KAnimHashedString overridden_symbol_name;

		public HashedString batchSource;

		public KAnim.Build.Symbol new_symbol;

		public bool is_perminent;
	}

	public enum SymbolTintIndex
	{
		First,
		Second
	}
}
