using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[DebuggerDisplay("{name} visible={visible} suspendUpdates={suspendUpdates} moving={moving}")]
public class KBatchedAnimController : KAnimControllerBase, KAnimConverter.IAnimConverter
{
	public KBatchedAnimController()
	{
		this.batchInstanceData = new KBatchedAnimInstanceData(this);
	}

	public int GetCurrentFrameIndex()
	{
		return this.curAnimFrameIdx;
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

	public bool IsActive()
	{
		return base.isActiveAndEnabled;
	}

	public bool IsVisible()
	{
		return this.isVisible;
	}

	public void SetSymbolScale(KAnimHashedString symbol_name, float scale)
	{
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(this.GetBatchGroupID(false)).GetSymbol(symbol_name);
		if (symbol == null)
		{
			return;
		}
		base.symbolInstanceGpuData.SetSymbolScale(symbol.symbolIndexInSourceBuild, scale);
		this.SuspendUpdates(false);
		this.SetDirty();
	}

	public void SetSymbolTint(KAnimHashedString symbol_name, Color color)
	{
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(this.GetBatchGroupID(false)).GetSymbol(symbol_name);
		if (symbol == null)
		{
			return;
		}
		base.symbolInstanceGpuData.SetSymbolTint(symbol.symbolIndexInSourceBuild, color);
		this.SuspendUpdates(false);
		this.SetDirty();
	}

	public Vector2I GetCellXY()
	{
		if (Grid.CellSizeInMeters == 0f)
		{
			return new Vector2I((int)base.transform.GetPosition().x, (int)base.transform.GetPosition().y);
		}
		return Grid.PosToXY(base.transform.GetPosition());
	}

	public float GetZ()
	{
		return base.transform.GetPosition().z;
	}

	public string GetName()
	{
		return base.name;
	}

	public override KAnim.Anim GetAnim(int index)
	{
		if (!this.batchGroupID.IsValid || !(this.batchGroupID != KAnimBatchManager.NO_BATCH))
		{
			global::Debug.LogError(base.name + " batch not ready", null);
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.batchGroupID);
		return batchGroupData.GetAnim(index);
	}

	private void Initialize()
	{
		if (this.batchGroupID.IsValid && this.batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			this.DeRegister();
			this.Register();
		}
	}

	private void OnMovementStateChanged(bool is_moving)
	{
		if (is_moving == this.moving)
		{
			return;
		}
		this.moving = is_moving;
		this.SetDirty();
		this.ConfigureUpdateListener();
	}

	private static void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		KBatchedAnimController component = transform.GetComponent<KBatchedAnimController>();
		component.OnMovementStateChanged(is_moving);
	}

	private void SetBatchGroup(KAnimFileData kafd)
	{
		DebugUtil.Assert(!this.batchGroupID.IsValid, "Should only be setting the batch group once.");
		base.curBuild = kafd.build;
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(kafd.build.batchTag);
		HashedString hashedString = kafd.build.batchTag;
		if (group.renderType == KAnimBatchGroup.RendererType.DontRender || group.renderType == KAnimBatchGroup.RendererType.AnimOnly)
		{
			hashedString = group.swapTarget;
		}
		this.batchGroupID = hashedString;
		base.symbolInstanceGpuData = new SymbolInstanceGpuData(KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID).maxSymbolsPerBuild);
		base.symbolOverrideInfoGpuData = new SymbolOverrideInfoGpuData(KAnimBatchManager.instance.GetBatchGroupData(this.batchGroupID).symbolFrameInstances.Count);
		if (!this.batchGroupID.IsValid || this.batchGroupID == KAnimBatchManager.NO_BATCH)
		{
			global::Debug.LogError("Batch is not ready: " + base.name, null);
		}
	}

	public void LoadAnims()
	{
		if (!KAnimBatchManager.Instance().isReady)
		{
			global::Debug.LogError("KAnimBatchManager is not ready when loading anim:" + base.name, null);
		}
		if (this.animFiles.Length <= 0)
		{
			DebugUtil.Assert(false, "KBatchedAnimController has no anim files:" + base.name);
		}
		if (this.animFiles[0].buildFile == null)
		{
			Output.LogErrorWithObj(base.gameObject, new object[] { string.Format("First anim file needs to be the build file but {0} doesn't have an associated build", this.animFiles[0].animFile.name) });
		}
		this.overrideAnims.Clear();
		this.anims.Clear();
		this.SetBatchGroup(this.animFiles[0].GetData());
		for (int i = 0; i < this.animFiles.Length; i++)
		{
			base.AddAnims(this.animFiles[i]);
		}
		this.forceRebuild = true;
		if (this.layering != null)
		{
			this.layering.HideSymbols();
		}
		if (this.usingNewSymbolOverrideSystem)
		{
			DebugUtil.Assert(base.GetComponent<SymbolOverrideController>() != null, "Assert!");
		}
	}

	public void UpdateAnim(float dt)
	{
		if (this.batch != null && base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			if (this.batch != null && this.batch.group.maxGroupSize == 1 && this.lastPos.z != base.transform.GetPosition().z)
			{
				this.batch.OverrideZ(base.transform.GetPosition().z);
			}
			Vector3 position = base.transform.GetPosition();
			this.lastPos = position;
			if (this.visibilityType != KAnimControllerBase.VisibilityType.Always)
			{
				Vector2I vector2I = KAnimBatchManager.ControllerToChunkXY(this);
				if (vector2I != this.lastChunkXY && this.lastChunkXY != KBatchedAnimUpdater.INVALID_CHUNK_ID)
				{
					this.DeRegister();
					this.Register();
				}
			}
			this.SetDirty();
		}
		if (this.batchGroupID == KAnimBatchManager.NO_BATCH || !base.isActiveAndEnabled || (!this.isVisible && !this.forceRebuild))
		{
			return;
		}
		if (!this.forceRebuild && (this.mode == KAnim.PlayMode.Paused || this.stopped || this.curAnim == null || (this.mode == KAnim.PlayMode.Once && this.curAnim != null && (this.elapsedTime > this.curAnim.totalTime || this.curAnim.totalTime <= 0f) && this.animQueue.Count == 0)))
		{
			this.SuspendUpdates(true);
		}
		this.curAnimFrameIdx = base.GetFrameIdx(this.elapsedTime, true);
		if (this.eventManagerHandle.IsValid() && this.aem != null)
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
		this.forceRebuild = false;
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

	public override void TriggerStop()
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

	public override void UpdateHidden()
	{
		for (int i = 0; i < base.curBuild.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = base.curBuild.symbols[i];
			bool flag = !this.hiddenSymbols.Contains(symbol.hash);
			base.symbolInstanceGpuData.SetVisible(i, flag);
		}
		this.SetDirty();
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

	public void SetBatch(KAnimBatch new_batch)
	{
		this.batch = new_batch;
		if (this.materialType == KAnimBatchGroup.MaterialType.UI)
		{
			KBatchedAnimCanvasRenderer kbatchedAnimCanvasRenderer = base.GetComponent<KBatchedAnimCanvasRenderer>();
			if (kbatchedAnimCanvasRenderer == null && new_batch != null)
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

	public override Matrix2x3 GetTransformMatrix()
	{
		Vector3 vector = base.transform.GetPosition() + this.offset;
		vector.z = 0f;
		Vector2 vector2 = new Vector2(this.animScale * this.animWidth, -this.animScale * this.animHeight);
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
		Matrix2x3 matrix2x = Matrix2x3.Scale(vector2);
		Matrix2x3 matrix2x2 = Matrix2x3.Scale(new Vector2((!this.flipX) ? 1f : (-1f), (!this.flipY) ? 1f : (-1f)));
		Matrix2x3 matrix2x8;
		if (this.rotation != 0f)
		{
			Matrix2x3 matrix2x3 = Matrix2x3.Translate(-this.pivot);
			Matrix2x3 matrix2x4 = Matrix2x3.Rotate(this.rotation * 0.017453292f);
			Matrix2x3 matrix2x5 = Matrix2x3.Translate(this.pivot);
			Matrix2x3 matrix2x6 = matrix2x5 * matrix2x4 * matrix2x3;
			Matrix2x3 matrix2x7 = Matrix2x3.TRS(vector, base.transform.rotation, base.transform.localScale);
			matrix2x8 = matrix2x7 * matrix2x6 * matrix2x * this.navMatrix * matrix2x2;
		}
		else
		{
			Matrix2x3 matrix2x9 = Matrix2x3.TRS(vector, base.transform.rotation, base.transform.localScale);
			matrix2x8 = matrix2x9 * matrix2x * this.navMatrix * matrix2x2;
		}
		return matrix2x8;
	}

	public override Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
	{
		if (this.curAnimFrameIdx != -1 && this.batch != null)
		{
			Matrix2x3 symbolLocalTransform = this.GetSymbolLocalTransform(symbol, out symbolVisible);
			if (symbolVisible)
			{
				Matrix4x4 matrix4x = this.GetTransformMatrix();
				return matrix4x * symbolLocalTransform;
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

	public void SetBatchGroupRenderQueueOverride()
	{
		if (this.batch != null)
		{
			if (this.originalRenderQueue < 0)
			{
				this.originalRenderQueue = this.batch.group.GetMaterial(this.batch.materialType).renderQueue;
			}
			this.batch.group.GetMaterial(this.batch.materialType).renderQueue = this.renderQueueOverride;
		}
	}

	public override void SetLayer(int layer)
	{
		if (layer == base.gameObject.layer)
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
		this.LoadAnims();
		if (this.visibilityType == KAnimControllerBase.VisibilityType.Default)
		{
			this.visibilityType = ((this.materialType != KAnimBatchGroup.MaterialType.UI) ? this.visibilityType : KAnimControllerBase.VisibilityType.Always);
		}
		this.symbolOverrideController = base.GetComponent<SymbolOverrideController>();
		this.UpdateHidden();
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
		CellChangeMonitor.Instance.RegisterMovementStateChanged(base.transform, new Action<Transform, bool>(KBatchedAnimController.OnMovementStateChanged));
		this.moving = CellChangeMonitor.Instance.IsMoving(base.transform);
		this.symbolOverrideController = base.GetComponent<SymbolOverrideController>();
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
		if (!this.stopped && this.curAnim != null && this.mode != KAnim.PlayMode.Paused && !this.eventManagerHandle.IsValid())
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
		if (App.IsExiting)
		{
			return;
		}
		CellChangeMonitor.Instance.UnregisterMovementStateChanged(base.transform, new Action<Transform, bool>(KBatchedAnimController.OnMovementStateChanged));
		KBatchedAnimUpdater.instance.UpdateUnregister(this);
		this.isVisible = false;
		this.DeRegister();
		this.stopped = true;
		base.StopAnimEventSequence();
		this.batchInstanceData = null;
		this.batch = null;
		base.OnDestroy();
	}

	public void SetBlendValue(float value)
	{
		this.batchInstanceData.SetBlend(value);
		this.SetDirty();
	}

	public bool ApplySymbolOverrides()
	{
		if (this.symbolOverrideController != null)
		{
			if (this.symbolOverrideControllerVersion != this.symbolOverrideController.version)
			{
				this.symbolOverrideControllerVersion = this.symbolOverrideController.version;
				this.symbolOverrideController.ApplyOverrides();
			}
			this.symbolOverrideController.ApplyAtlases();
			return true;
		}
		return false;
	}

	public void SetSymbolOverride(int symbol_idx, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		DebugUtil.Assert(this.usingNewSymbolOverrideSystem, "KBatchedAnimController requires usingNewSymbolOverrideSystem to bet to true to enable symbol overrides.");
		base.symbolOverrideInfoGpuData.SetSymbolOverrideInfo(symbol_idx, symbol_frame_instance);
	}

	private void Register()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.batch != null)
		{
			return;
		}
		if (this.batchGroupID.IsValid && this.batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			this.lastChunkXY = KAnimBatchManager.ControllerToChunkXY(this);
			KAnimBatchManager.Instance().Register(this);
			this.forceRebuild = true;
			this.SetDirty();
		}
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
		bool flag = (base.isActiveAndEnabled && !this.suspendUpdates && this.isVisible) || this.moving || this.visibilityType == KAnimControllerBase.VisibilityType.Always;
		if (flag)
		{
			KBatchedAnimUpdater.instance.UpdateRegister(this);
		}
		else
		{
			KBatchedAnimUpdater.instance.UpdateUnregister(this);
		}
	}

	protected override void SuspendUpdates(bool suspend)
	{
		this.suspendUpdates = suspend;
		this.ConfigureUpdateListener();
	}

	public void SetVisiblity(bool is_visible)
	{
		if (is_visible != this.isVisible)
		{
			this.isVisible = is_visible;
			if (is_visible)
			{
				this.SuspendUpdates(false);
				this.SetDirty();
				base.UpdateAnimEventSequenceTime();
			}
			else
			{
				this.SuspendUpdates(true);
				this.SetDirty();
			}
		}
	}

	private void ConfigureVisibilityListener(bool enabled)
	{
		if (this.visibilityType == KAnimControllerBase.VisibilityType.Always)
		{
			return;
		}
		if (enabled)
		{
			this.RegisterVisibilityListener();
		}
		else
		{
			this.UnregisterVisibilityListener();
		}
	}

	private void RegisterVisibilityListener()
	{
		DebugUtil.Assert(!this.visibilityListenerRegistered, "Assert!");
		KBatchedAnimUpdater.instance.VisibilityRegister(this);
		this.visibilityListenerRegistered = true;
	}

	private void UnregisterVisibilityListener()
	{
		DebugUtil.Assert(this.visibilityListenerRegistered, "Assert!");
		KBatchedAnimUpdater.instance.VisibilityUnregister(this);
		this.visibilityListenerRegistered = false;
	}

	public void SetSceneLayer(Grid.SceneLayer layer)
	{
		float layerZ = Grid.GetLayerZ(layer);
		if (layerZ == base.transform.GetPosition().z)
		{
			return;
		}
		this.sceneLayer = layer;
		Vector3 position = base.transform.GetPosition();
		position.z = layerZ;
		base.transform.SetPosition(position);
		this.DeRegister();
		this.Register();
	}

	[NonSerialized]
	protected bool _forceRebuild;

	private Vector3 lastPos = Vector3.zero;

	private Vector2I lastChunkXY = KBatchedAnimUpdater.INVALID_CHUNK_ID;

	private KAnimBatch batch;

	public float animScale = 0.005f;

	private bool suspendUpdates;

	private bool visibilityListenerRegistered;

	private bool moving;

	private SymbolOverrideController symbolOverrideController;

	private int symbolOverrideControllerVersion;

	[NonSerialized]
	public KBatchedAnimUpdater.RegistrationState updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Unregistered;

	public int renderQueueOverride = -1;

	[NonSerialized]
	public int originalRenderQueue = -1;

	public Grid.SceneLayer sceneLayer;

	private RectTransform rt;

	private Vector3 screenOffset = new Vector3(0f, 0f, 0f);

	public Matrix2x3 navMatrix = Matrix2x3.identity;

	private CanvasScaler scaler;

	public bool setScaleFromAnim = true;

	public Vector2 animOverrideSize = Vector2.one;

	private Canvas rootCanvas;

	public bool isMovable;
}
