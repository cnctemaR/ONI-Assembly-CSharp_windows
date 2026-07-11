using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using UnityEngine.UIElements.UIR.Implementation;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderChain : IDisposable
	{
		internal RenderChainCommand firstCommand
		{
			get
			{
				return this.m_FirstCommand;
			}
		}

		public RenderChain(IPanel panel, Shader standardShader)
		{
			UIRAtlasManager uiratlasManager = new UIRAtlasManager(RenderTextureFormat.ARGB32, FilterMode.Bilinear, 64, 64);
			VectorImageManager vectorImageManager = new VectorImageManager(uiratlasManager);
			this.Constructor(panel, new UIRenderDevice(RenderEvents.ResolveShader(standardShader), 0U, 0U, UIRenderDevice.DrawingModes.FlipY, 1024), uiratlasManager, vectorImageManager);
		}

		protected RenderChain(IPanel panel, UIRenderDevice device, UIRAtlasManager atlasManager, VectorImageManager vectorImageManager)
		{
			this.Constructor(panel, device, atlasManager, vectorImageManager);
		}

		private void Constructor(IPanel panelObj, UIRenderDevice deviceObj, UIRAtlasManager atlasMan, VectorImageManager vectorImageMan)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			this.m_DirtyTracker.heads = new List<VisualElement>(8);
			this.m_DirtyTracker.tails = new List<VisualElement>(8);
			this.m_DirtyTracker.minDepths = new int[4];
			this.m_DirtyTracker.maxDepths = new int[4];
			this.m_DirtyTracker.Reset();
			this.panel = panelObj;
			this.device = deviceObj;
			this.atlasManager = atlasMan;
			this.vectorImageManager = vectorImageMan;
			this.shaderInfoAllocator.Construct();
			this.painter = new UIRStylePainter(this);
			Font.textureRebuilt += this.OnFontReset;
		}

		private void Destructor()
		{
			Font.textureRebuilt -= this.OnFontReset;
			UIRStylePainter painter = this.painter;
			if (painter != null)
			{
				painter.Dispose();
			}
			UIRTextUpdatePainter textUpdatePainter = this.m_TextUpdatePainter;
			if (textUpdatePainter != null)
			{
				textUpdatePainter.Dispose();
			}
			UIRAtlasManager atlasManager = this.atlasManager;
			if (atlasManager != null)
			{
				atlasManager.Dispose();
			}
			VectorImageManager vectorImageManager = this.vectorImageManager;
			if (vectorImageManager != null)
			{
				vectorImageManager.Dispose();
			}
			this.shaderInfoAllocator.Dispose();
			UIRenderDevice device = this.device;
			if (device != null)
			{
				device.Dispose();
			}
			this.painter = null;
			this.m_TextUpdatePainter = null;
			this.atlasManager = null;
			this.shaderInfoAllocator = default(UIRVEShaderInfoAllocator);
			this.device = null;
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.Destructor();
				}
				this.disposed = true;
			}
		}

		internal ChainBuilderStats stats
		{
			get
			{
				return this.m_Stats;
			}
		}

		public void Render(Rect viewport, Matrix4x4 projection, PanelClearFlags clearFlags)
		{
			this.m_Stats = default(ChainBuilderStats);
			this.m_Stats.elementsAdded = this.m_Stats.elementsAdded + this.m_StatsElementsAdded;
			this.m_Stats.elementsRemoved = this.m_Stats.elementsRemoved + this.m_StatsElementsRemoved;
			this.m_StatsElementsAdded = (this.m_StatsElementsRemoved = 0U);
			bool isReleased = this.shaderInfoAllocator.isReleased;
			if (isReleased)
			{
				this.RecreateDevice();
			}
			bool flag = RenderChain.OnPreRender != null;
			if (flag)
			{
				RenderChain.OnPreRender();
			}
			bool flag2 = false;
			UIRAtlasManager atlasManager = this.atlasManager;
			bool flag3 = atlasManager != null && atlasManager.RequiresReset();
			if (flag3)
			{
				this.atlasManager.Reset();
				flag2 = true;
			}
			VectorImageManager vectorImageManager = this.vectorImageManager;
			bool flag4 = vectorImageManager != null && vectorImageManager.RequiresReset();
			if (flag4)
			{
				this.vectorImageManager.Reset();
				flag2 = true;
			}
			bool flag5 = flag2;
			if (flag5)
			{
				this.RepaintAtlassedElements();
			}
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			int num = 0;
			RenderDataDirtyTypes renderDataDirtyTypes = RenderDataDirtyTypes.Clipping | RenderDataDirtyTypes.ClippingHierarchy;
			RenderDataDirtyTypes renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			for (int i = this.m_DirtyTracker.minDepths[num]; i <= this.m_DirtyTracker.maxDepths[num]; i++)
			{
				VisualElement nextDirty;
				for (VisualElement visualElement = this.m_DirtyTracker.heads[i]; visualElement != null; visualElement = nextDirty)
				{
					nextDirty = visualElement.renderChainData.nextDirty;
					bool flag6 = (visualElement.renderChainData.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag6)
					{
						bool flag7 = visualElement.renderChainData.isInChain && visualElement.renderChainData.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag7)
						{
							RenderEvents.ProcessOnClippingChanged(this, visualElement, this.m_DirtyTracker.dirtyID, this.device, ref this.m_Stats);
						}
						this.m_DirtyTracker.ClearDirty(visualElement, renderDataDirtyTypes2);
					}
				}
			}
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 1;
			renderDataDirtyTypes = RenderDataDirtyTypes.Opacity;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			for (int j = this.m_DirtyTracker.minDepths[num]; j <= this.m_DirtyTracker.maxDepths[num]; j++)
			{
				VisualElement nextDirty2;
				for (VisualElement visualElement2 = this.m_DirtyTracker.heads[j]; visualElement2 != null; visualElement2 = nextDirty2)
				{
					nextDirty2 = visualElement2.renderChainData.nextDirty;
					bool flag8 = (visualElement2.renderChainData.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag8)
					{
						bool flag9 = visualElement2.renderChainData.isInChain && visualElement2.renderChainData.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag9)
						{
							RenderEvents.ProcessOnOpacityChanged(this, visualElement2, this.m_DirtyTracker.dirtyID, ref this.m_Stats);
						}
						this.m_DirtyTracker.ClearDirty(visualElement2, renderDataDirtyTypes2);
					}
				}
			}
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 2;
			renderDataDirtyTypes = RenderDataDirtyTypes.Transform | RenderDataDirtyTypes.ClipRectSize;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			for (int k = this.m_DirtyTracker.minDepths[num]; k <= this.m_DirtyTracker.maxDepths[num]; k++)
			{
				VisualElement nextDirty3;
				for (VisualElement visualElement3 = this.m_DirtyTracker.heads[k]; visualElement3 != null; visualElement3 = nextDirty3)
				{
					nextDirty3 = visualElement3.renderChainData.nextDirty;
					bool flag10 = (visualElement3.renderChainData.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag10)
					{
						bool flag11 = visualElement3.renderChainData.isInChain && visualElement3.renderChainData.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag11)
						{
							RenderEvents.ProcessOnTransformOrSizeChanged(this, visualElement3, this.m_DirtyTracker.dirtyID, this.device, ref this.m_Stats);
						}
						this.m_DirtyTracker.ClearDirty(visualElement3, renderDataDirtyTypes2);
					}
				}
			}
			this.m_BlockDirtyRegistration = true;
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 3;
			renderDataDirtyTypes = RenderDataDirtyTypes.Visuals | RenderDataDirtyTypes.VisualsHierarchy;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			for (int l = this.m_DirtyTracker.minDepths[num]; l <= this.m_DirtyTracker.maxDepths[num]; l++)
			{
				VisualElement nextDirty4;
				for (VisualElement visualElement4 = this.m_DirtyTracker.heads[l]; visualElement4 != null; visualElement4 = nextDirty4)
				{
					nextDirty4 = visualElement4.renderChainData.nextDirty;
					bool flag12 = (visualElement4.renderChainData.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag12)
					{
						bool flag13 = visualElement4.renderChainData.isInChain && visualElement4.renderChainData.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag13)
						{
							RenderEvents.ProcessOnVisualsChanged(this, visualElement4, this.m_DirtyTracker.dirtyID, ref this.m_Stats);
						}
						this.m_DirtyTracker.ClearDirty(visualElement4, renderDataDirtyTypes2);
					}
				}
			}
			this.m_BlockDirtyRegistration = false;
			this.m_DirtyTracker.Reset();
			this.ProcessTextRegen(true);
			bool fontWasReset = this.m_FontWasReset;
			if (fontWasReset)
			{
				for (int m = 0; m < 2; m++)
				{
					bool flag14 = !this.m_FontWasReset;
					if (flag14)
					{
						break;
					}
					this.m_FontWasReset = false;
					this.ProcessTextRegen(false);
				}
			}
			UIRAtlasManager atlasManager2 = this.atlasManager;
			if (atlasManager2 != null)
			{
				atlasManager2.Commit();
			}
			VectorImageManager vectorImageManager2 = this.vectorImageManager;
			if (vectorImageManager2 != null)
			{
				vectorImageManager2.Commit();
			}
			this.shaderInfoAllocator.IssuePendingAtlasBlits();
			bool flag15 = this.BeforeDrawChain != null;
			if (flag15)
			{
				this.BeforeDrawChain(this.device);
			}
			Exception ex = null;
			UIRenderDevice device = this.device;
			RenderChainCommand firstCommand = this.m_FirstCommand;
			UIRAtlasManager atlasManager3 = this.atlasManager;
			Texture texture = ((atlasManager3 != null) ? atlasManager3.atlas : null);
			VectorImageManager vectorImageManager3 = this.vectorImageManager;
			device.DrawChain(firstCommand, viewport, projection, clearFlags, texture, (vectorImageManager3 != null) ? vectorImageManager3.atlas : null, this.shaderInfoAllocator.atlas, (this.panel as BaseVisualElementPanel).scaledPixelsPerPoint, this.shaderInfoAllocator.transformConstants, this.shaderInfoAllocator.clipRectConstants, ref ex);
			bool flag16 = ex != null;
			if (!flag16)
			{
				bool drawStats = this.drawStats;
				if (drawStats)
				{
					this.DrawStats();
				}
				return;
			}
			bool flag17 = GUIUtility.IsExitGUIException(ex);
			if (flag17)
			{
				throw ex;
			}
			throw new ImmediateModeException(ex);
		}

		private void ProcessTextRegen(bool timeSliced)
		{
			bool flag = (timeSliced && this.m_DirtyTextRemaining == 0) || this.m_TextElementCount == 0;
			if (!flag)
			{
				bool flag2 = this.m_TextUpdatePainter == null;
				if (flag2)
				{
					this.m_TextUpdatePainter = new UIRTextUpdatePainter();
				}
				VisualElement visualElement = this.m_FirstTextElement;
				this.m_DirtyTextStartIndex = (timeSliced ? (this.m_DirtyTextStartIndex % this.m_TextElementCount) : 0);
				for (int i = 0; i < this.m_DirtyTextStartIndex; i++)
				{
					visualElement = visualElement.renderChainData.nextText;
				}
				bool flag3 = visualElement == null;
				if (flag3)
				{
					visualElement = this.m_FirstTextElement;
				}
				int num = (timeSliced ? Math.Min(50, this.m_DirtyTextRemaining) : this.m_TextElementCount);
				for (int j = 0; j < num; j++)
				{
					RenderEvents.ProcessRegenText(this, visualElement, this.m_TextUpdatePainter, this.device, ref this.m_Stats);
					visualElement = visualElement.renderChainData.nextText;
					this.m_DirtyTextStartIndex++;
					bool flag4 = visualElement == null;
					if (flag4)
					{
						visualElement = this.m_FirstTextElement;
						this.m_DirtyTextStartIndex = 0;
					}
				}
				this.m_DirtyTextRemaining = Math.Max(0, this.m_DirtyTextRemaining - num);
				bool flag5 = this.m_DirtyTextRemaining > 0;
				if (flag5)
				{
					BaseVisualElementPanel baseVisualElementPanel = this.panel as BaseVisualElementPanel;
					if (baseVisualElementPanel != null)
					{
						baseVisualElementPanel.OnVersionChanged(this.m_FirstTextElement, VersionChangeType.Transform);
					}
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<UIRenderDevice> BeforeDrawChain;

		public void UIEOnStandardShaderChanged(Shader standardShader)
		{
			this.device.standardShader = RenderEvents.ResolveShader(standardShader);
		}

		public void UIEOnChildAdded(VisualElement parent, VisualElement ve, int index)
		{
			bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
			if (blockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot be added to an active visual tree during generateVisualContent callback execution");
			}
			bool flag = parent != null && !parent.renderChainData.isInChain;
			if (!flag)
			{
				uint num = RenderEvents.DepthFirstOnChildAdded(this, parent, ve, index, true);
				Debug.Assert(ve.renderChainData.isInChain);
				Debug.Assert(ve.panel == this.panel);
				this.UIEOnClippingChanged(ve, true);
				this.UIEOnOpacityChanged(ve);
				this.UIEOnVisualsChanged(ve, true);
				this.m_StatsElementsAdded += num;
			}
		}

		public void UIEOnChildrenReordered(VisualElement ve)
		{
			bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
			if (blockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot be moved under an active visual tree during generateVisualContent callback execution");
			}
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				RenderEvents.DepthFirstOnChildRemoving(this, ve.hierarchy[i]);
			}
			for (int j = 0; j < childCount; j++)
			{
				RenderEvents.DepthFirstOnChildAdded(this, ve, ve.hierarchy[j], j, false);
			}
			this.UIEOnClippingChanged(ve, true);
			this.UIEOnVisualsChanged(ve, true);
		}

		public void UIEOnChildRemoving(VisualElement ve)
		{
			bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
			if (blockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot be removed from an active visual tree during generateVisualContent callback execution");
			}
			this.m_StatsElementsRemoved += RenderEvents.DepthFirstOnChildRemoving(this, ve);
			Debug.Assert(!ve.renderChainData.isInChain);
		}

		public void StopTrackingGroupTransformElement(VisualElement ve)
		{
			this.m_LastGroupTransformElementScale.Remove(ve);
		}

		public void UIEOnClippingChanged(VisualElement ve, bool hierarchical)
		{
			bool isInChain = ve.renderChainData.isInChain;
			if (isInChain)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("VisualElements cannot change clipping state under an active visual tree during generateVisualContent callback execution");
				}
				this.m_DirtyTracker.RegisterDirty(ve, RenderDataDirtyTypes.Clipping | (hierarchical ? RenderDataDirtyTypes.ClippingHierarchy : RenderDataDirtyTypes.None), 0);
			}
		}

		public void UIEOnOpacityChanged(VisualElement ve)
		{
			bool isInChain = ve.renderChainData.isInChain;
			if (isInChain)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("VisualElements cannot change opacity under an active visual tree during generateVisualContent callback execution");
				}
				this.m_DirtyTracker.RegisterDirty(ve, RenderDataDirtyTypes.Opacity, 1);
			}
		}

		public void UIEOnTransformOrSizeChanged(VisualElement ve, bool transformChanged, bool clipRectSizeChanged)
		{
			bool isInChain = ve.renderChainData.isInChain;
			if (isInChain)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("VisualElements cannot change size or transform under an active visual tree during generateVisualContent callback execution");
				}
				RenderDataDirtyTypes renderDataDirtyTypes = (transformChanged ? RenderDataDirtyTypes.Transform : RenderDataDirtyTypes.None) | (clipRectSizeChanged ? RenderDataDirtyTypes.ClipRectSize : RenderDataDirtyTypes.None);
				this.m_DirtyTracker.RegisterDirty(ve, renderDataDirtyTypes, 2);
			}
		}

		public void UIEOnVisualsChanged(VisualElement ve, bool hierarchical)
		{
			bool isInChain = ve.renderChainData.isInChain;
			if (isInChain)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("VisualElements cannot be marked for dirty repaint under an active visual tree during generateVisualContent callback execution");
				}
				this.m_DirtyTracker.RegisterDirty(ve, RenderDataDirtyTypes.Visuals | (hierarchical ? RenderDataDirtyTypes.VisualsHierarchy : RenderDataDirtyTypes.None), 3);
			}
		}

		internal IPanel panel { get; private set; }

		internal UIRenderDevice device { get; private set; }

		internal UIRAtlasManager atlasManager { get; private set; }

		internal VectorImageManager vectorImageManager { get; private set; }

		internal UIRStylePainter painter { get; private set; }

		internal bool drawStats { get; set; }

		internal void EnsureFitsDepth(int depth)
		{
			this.m_DirtyTracker.EnsureFits(depth);
		}

		internal void ChildWillBeRemoved(VisualElement ve)
		{
			bool flag = ve.renderChainData.dirtiedValues > RenderDataDirtyTypes.None;
			if (flag)
			{
				this.m_DirtyTracker.ClearDirty(ve, ~ve.renderChainData.dirtiedValues);
			}
			Debug.Assert(ve.renderChainData.dirtiedValues == RenderDataDirtyTypes.None);
			Debug.Assert(ve.renderChainData.prevDirty == null);
			Debug.Assert(ve.renderChainData.nextDirty == null);
		}

		internal RenderChainCommand AllocCommand()
		{
			RenderChainCommand renderChainCommand = this.m_CommandPool.Get();
			renderChainCommand.Reset();
			return renderChainCommand;
		}

		internal void FreeCommand(RenderChainCommand cmd)
		{
			cmd.Reset();
			this.m_CommandPool.Return(cmd);
		}

		internal void OnRenderCommandAdded(RenderChainCommand firstCommand)
		{
			bool flag = firstCommand.prev == null;
			if (flag)
			{
				this.m_FirstCommand = firstCommand;
			}
		}

		internal void OnRenderCommandRemoved(RenderChainCommand firstCommand, RenderChainCommand lastCommand)
		{
			bool flag = firstCommand.prev == null;
			if (flag)
			{
				this.m_FirstCommand = lastCommand.next;
			}
		}

		internal void AddTextElement(VisualElement ve)
		{
			bool flag = this.m_FirstTextElement != null;
			if (flag)
			{
				this.m_FirstTextElement.renderChainData.prevText = ve;
				ve.renderChainData.nextText = this.m_FirstTextElement;
			}
			this.m_FirstTextElement = ve;
			this.m_TextElementCount++;
		}

		internal void RemoveTextElement(VisualElement ve)
		{
			bool flag = ve.renderChainData.prevText != null;
			if (flag)
			{
				ve.renderChainData.prevText.renderChainData.nextText = ve.renderChainData.nextText;
			}
			bool flag2 = ve.renderChainData.nextText != null;
			if (flag2)
			{
				ve.renderChainData.nextText.renderChainData.prevText = ve.renderChainData.prevText;
			}
			bool flag3 = this.m_FirstTextElement == ve;
			if (flag3)
			{
				this.m_FirstTextElement = ve.renderChainData.nextText;
			}
			ve.renderChainData.prevText = (ve.renderChainData.nextText = null);
			this.m_TextElementCount--;
		}

		internal void OnGroupTransformElementChangedTransform(VisualElement ve)
		{
			Vector2 vector;
			bool flag = !this.m_LastGroupTransformElementScale.TryGetValue(ve, out vector) || ve.worldTransform.m00 != vector.x || ve.worldTransform.m11 != vector.y;
			if (flag)
			{
				this.m_DirtyTextRemaining = this.m_TextElementCount;
				this.m_LastGroupTransformElementScale[ve] = new Vector2(ve.worldTransform.m00, ve.worldTransform.m11);
			}
		}

		internal void BeforeRenderDeviceRelease()
		{
			Debug.Assert(this.device != null);
			Debug.Assert(this.m_RenderDeviceRestoreInfo.root == null);
			RenderChainCommand firstCommand = this.m_FirstCommand;
			this.m_RenderDeviceRestoreInfo.root = RenderChain.GetFirstElementInPanel((firstCommand != null) ? firstCommand.owner : null);
			this.m_RenderDeviceRestoreInfo.standardShader = this.device.standardShader;
			this.m_RenderDeviceRestoreInfo.hasAtlasMan = this.atlasManager != null;
			this.m_RenderDeviceRestoreInfo.hasVectorImageMan = this.vectorImageManager != null;
			this.UIEOnChildRemoving(this.m_RenderDeviceRestoreInfo.root);
			this.Destructor();
		}

		internal void AfterRenderDeviceRelease()
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			Debug.Assert(this.device == null);
			VisualElement root = this.m_RenderDeviceRestoreInfo.root;
			IPanel panel = root.panel;
			UIRenderDevice uirenderDevice = new UIRenderDevice(this.m_RenderDeviceRestoreInfo.standardShader, 0U, 0U, UIRenderDevice.DrawingModes.FlipY, 1024);
			UIRAtlasManager uiratlasManager = (this.m_RenderDeviceRestoreInfo.hasAtlasMan ? new UIRAtlasManager(RenderTextureFormat.ARGB32, FilterMode.Bilinear, 64, 64) : null);
			VectorImageManager vectorImageManager = (this.m_RenderDeviceRestoreInfo.hasVectorImageMan ? new VectorImageManager(uiratlasManager) : null);
			this.m_RenderDeviceRestoreInfo = default(RenderChain.RenderDeviceRestoreInfo);
			this.Constructor(panel, uirenderDevice, uiratlasManager, vectorImageManager);
			this.UIEOnChildAdded(root.parent, root, (root.hierarchy.parent == null) ? 0 : root.hierarchy.parent.IndexOf(this.panel.visualTree));
		}

		internal void RecreateDevice()
		{
			this.BeforeRenderDeviceRelease();
			this.AfterRenderDeviceRelease();
		}

		private void RepaintAtlassedElements()
		{
			RenderChainCommand firstCommand = this.m_FirstCommand;
			for (VisualElement visualElement = RenderChain.GetFirstElementInPanel((firstCommand != null) ? firstCommand.owner : null); visualElement != null; visualElement = visualElement.renderChainData.next)
			{
				bool usesAtlas = visualElement.renderChainData.usesAtlas;
				if (usesAtlas)
				{
					this.UIEOnVisualsChanged(visualElement, false);
				}
			}
			this.UIEOnOpacityChanged(this.panel.visualTree);
		}

		private void OnFontReset(Font font)
		{
			this.m_FontWasReset = true;
		}

		private void DrawStats()
		{
			bool flag = this.device != null;
			float num = 12f;
			Rect rect = new Rect(30f, 60f, 1000f, 100f);
			GUI.Box(new Rect(20f, 40f, 200f, (float)(flag ? 380 : 256)), "UIElements Draw Stats");
			GUI.Label(rect, "Elements added\t: " + this.m_Stats.elementsAdded);
			rect.y += num;
			GUI.Label(rect, "Elements removed\t: " + this.m_Stats.elementsRemoved);
			rect.y += num;
			GUI.Label(rect, "Mesh allocs allocated\t: " + this.m_Stats.newMeshAllocations);
			rect.y += num;
			GUI.Label(rect, "Mesh allocs updated\t: " + this.m_Stats.updatedMeshAllocations);
			rect.y += num;
			GUI.Label(rect, "Clip update roots\t: " + this.m_Stats.recursiveClipUpdates);
			rect.y += num;
			GUI.Label(rect, "Clip update total\t: " + this.m_Stats.recursiveClipUpdatesExpanded);
			rect.y += num;
			GUI.Label(rect, "Opacity update roots\t: " + this.m_Stats.recursiveOpacityUpdates);
			rect.y += num;
			GUI.Label(rect, "Opacity update total\t: " + this.m_Stats.recursiveOpacityUpdatesExpanded);
			rect.y += num;
			GUI.Label(rect, "Xform update roots\t: " + this.m_Stats.recursiveTransformUpdates);
			rect.y += num;
			GUI.Label(rect, "Xform update total\t: " + this.m_Stats.recursiveTransformUpdatesExpanded);
			rect.y += num;
			GUI.Label(rect, "Xformed by bone\t: " + this.m_Stats.boneTransformed);
			rect.y += num;
			GUI.Label(rect, "Xformed by skipping\t: " + this.m_Stats.skipTransformed);
			rect.y += num;
			GUI.Label(rect, "Xformed by nudging\t: " + this.m_Stats.nudgeTransformed);
			rect.y += num;
			GUI.Label(rect, "Xformed by repaint\t: " + this.m_Stats.visualUpdateTransformed);
			rect.y += num;
			GUI.Label(rect, "Visual update roots\t: " + this.m_Stats.recursiveVisualUpdates);
			rect.y += num;
			GUI.Label(rect, "Visual update total\t: " + this.m_Stats.recursiveVisualUpdatesExpanded);
			rect.y += num;
			GUI.Label(rect, "Visual update flats\t: " + this.m_Stats.nonRecursiveVisualUpdates);
			rect.y += num;
			GUI.Label(rect, "Group-xform updates\t: " + this.m_Stats.groupTransformElementsChanged);
			rect.y += num;
			GUI.Label(rect, "Text regens\t: " + this.m_Stats.textUpdates);
			rect.y += num;
			bool flag2 = !flag;
			if (!flag2)
			{
				rect.y += num;
				UIRenderDevice.DrawStatistics drawStatistics = this.device.GatherDrawStatistics();
				GUI.Label(rect, "Frame index\t: " + drawStatistics.currentFrameIndex);
				rect.y += num;
				GUI.Label(rect, "Command count\t: " + drawStatistics.commandCount);
				rect.y += num;
				GUI.Label(rect, "Draw commands\t: " + drawStatistics.drawCommandCount);
				rect.y += num;
				GUI.Label(rect, "Draw range start\t: " + drawStatistics.currentDrawRangeStart);
				rect.y += num;
				GUI.Label(rect, "Draw ranges\t: " + drawStatistics.drawRangeCount);
				rect.y += num;
				GUI.Label(rect, "Draw range calls\t: " + drawStatistics.drawRangeCallCount);
				rect.y += num;
				GUI.Label(rect, "Material sets\t: " + drawStatistics.materialSetCount);
				rect.y += num;
				GUI.Label(rect, "Immediate draws\t: " + drawStatistics.immediateDraws);
				rect.y += num;
				GUI.Label(rect, "Total triangles\t: " + drawStatistics.totalIndices / 3U);
				rect.y += num;
			}
		}

		private static VisualElement GetFirstElementInPanel(VisualElement ve)
		{
			for (;;)
			{
				bool flag;
				if (ve != null)
				{
					VisualElement prev = ve.renderChainData.prev;
					flag = prev != null && prev.renderChainData.isInChain;
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					break;
				}
				ve = ve.renderChainData.prev;
			}
			return ve;
		}

		private RenderChainCommand m_FirstCommand;

		private RenderChain.DepthOrderedDirtyTracking m_DirtyTracker;

		private Pool<RenderChainCommand> m_CommandPool = new Pool<RenderChainCommand>();

		private bool m_BlockDirtyRegistration;

		private ChainBuilderStats m_Stats;

		private uint m_StatsElementsAdded;

		private uint m_StatsElementsRemoved;

		private VisualElement m_FirstTextElement;

		private UIRTextUpdatePainter m_TextUpdatePainter;

		private int m_TextElementCount;

		private int m_DirtyTextStartIndex;

		private int m_DirtyTextRemaining;

		private bool m_FontWasReset;

		private Dictionary<VisualElement, Vector2> m_LastGroupTransformElementScale = new Dictionary<VisualElement, Vector2>();

		private static ProfilerMarker s_MarkerRender = new ProfilerMarker("RenderChain.Draw");

		private static ProfilerMarker s_MarkerClipProcessing = new ProfilerMarker("RenderChain.UpdateClips");

		private static ProfilerMarker s_MarkerOpacityProcessing = new ProfilerMarker("RenderChain.UpdateOpacity");

		private static ProfilerMarker s_MarkerTransformProcessing = new ProfilerMarker("RenderChain.UpdateTransforms");

		private static ProfilerMarker s_MarkerVisualsProcessing = new ProfilerMarker("RenderChain.UpdateVisuals");

		private static ProfilerMarker s_MarkerTextRegen = new ProfilerMarker("RenderChain.RegenText");

		internal static Action OnPreRender = null;

		internal UIRVEShaderInfoAllocator shaderInfoAllocator;

		private RenderChain.RenderDeviceRestoreInfo m_RenderDeviceRestoreInfo;

		private struct DepthOrderedDirtyTracking
		{
			public void EnsureFits(int maxDepth)
			{
				while (this.heads.Count <= maxDepth)
				{
					this.heads.Add(null);
					this.tails.Add(null);
				}
			}

			public void RegisterDirty(VisualElement ve, RenderDataDirtyTypes dirtyTypes, int dirtyTypeClassIndex)
			{
				Debug.Assert(dirtyTypes > RenderDataDirtyTypes.None);
				int hierarchyDepth = ve.renderChainData.hierarchyDepth;
				this.minDepths[dirtyTypeClassIndex] = ((hierarchyDepth < this.minDepths[dirtyTypeClassIndex]) ? hierarchyDepth : this.minDepths[dirtyTypeClassIndex]);
				this.maxDepths[dirtyTypeClassIndex] = ((hierarchyDepth > this.maxDepths[dirtyTypeClassIndex]) ? hierarchyDepth : this.maxDepths[dirtyTypeClassIndex]);
				bool flag = ve.renderChainData.dirtiedValues > RenderDataDirtyTypes.None;
				if (flag)
				{
					ve.renderChainData.dirtiedValues = ve.renderChainData.dirtiedValues | dirtyTypes;
				}
				else
				{
					ve.renderChainData.dirtiedValues = dirtyTypes;
					bool flag2 = this.tails[hierarchyDepth] != null;
					if (flag2)
					{
						this.tails[hierarchyDepth].renderChainData.nextDirty = ve;
						ve.renderChainData.prevDirty = this.tails[hierarchyDepth];
						this.tails[hierarchyDepth] = ve;
					}
					else
					{
						List<VisualElement> list = this.heads;
						int num = hierarchyDepth;
						this.tails[hierarchyDepth] = ve;
						list[num] = ve;
					}
				}
			}

			public void ClearDirty(VisualElement ve, RenderDataDirtyTypes dirtyTypesInverse)
			{
				Debug.Assert(ve.renderChainData.dirtiedValues > RenderDataDirtyTypes.None);
				ve.renderChainData.dirtiedValues = ve.renderChainData.dirtiedValues & dirtyTypesInverse;
				bool flag = ve.renderChainData.dirtiedValues == RenderDataDirtyTypes.None;
				if (flag)
				{
					bool flag2 = ve.renderChainData.prevDirty != null;
					if (flag2)
					{
						ve.renderChainData.prevDirty.renderChainData.nextDirty = ve.renderChainData.nextDirty;
					}
					bool flag3 = ve.renderChainData.nextDirty != null;
					if (flag3)
					{
						ve.renderChainData.nextDirty.renderChainData.prevDirty = ve.renderChainData.prevDirty;
					}
					bool flag4 = this.tails[ve.renderChainData.hierarchyDepth] == ve;
					if (flag4)
					{
						Debug.Assert(ve.renderChainData.nextDirty == null);
						this.tails[ve.renderChainData.hierarchyDepth] = ve.renderChainData.prevDirty;
					}
					bool flag5 = this.heads[ve.renderChainData.hierarchyDepth] == ve;
					if (flag5)
					{
						Debug.Assert(ve.renderChainData.prevDirty == null);
						this.heads[ve.renderChainData.hierarchyDepth] = ve.renderChainData.nextDirty;
					}
					ve.renderChainData.prevDirty = (ve.renderChainData.nextDirty = null);
				}
			}

			public void Reset()
			{
				for (int i = 0; i < this.minDepths.Length; i++)
				{
					this.minDepths[i] = int.MaxValue;
					this.maxDepths[i] = int.MinValue;
				}
			}

			public List<VisualElement> heads;

			public List<VisualElement> tails;

			public int[] minDepths;

			public int[] maxDepths;

			public uint dirtyID;
		}

		private struct RenderDeviceRestoreInfo
		{
			public VisualElement root;

			public Shader standardShader;

			public bool hasAtlasMan;

			public bool hasVectorImageMan;
		}
	}
}
