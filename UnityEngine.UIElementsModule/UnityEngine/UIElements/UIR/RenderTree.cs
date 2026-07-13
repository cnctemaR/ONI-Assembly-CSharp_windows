using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderTree
	{
		internal RenderTreeManager renderTreeManager
		{
			get
			{
				return this.m_RenderTreeManager;
			}
		}

		internal RenderData rootRenderData
		{
			get
			{
				return this.m_RootRenderData;
			}
		}

		internal ref DepthOrderedDirtyTracking dirtyTracker
		{
			get
			{
				return ref this.m_DirtyTracker;
			}
		}

		internal RenderChainCommand firstCommand
		{
			get
			{
				return this.m_FirstCommand;
			}
		}

		internal bool isRootRenderTree
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.rootRenderData.owner.parent == null && !this.rootRenderData.isNestedRenderTreeRoot;
			}
		}

		public void Init(RenderTreeManager renderTreeManager, RenderData rootRenderData)
		{
			this.m_RenderTreeManager = renderTreeManager;
			this.m_RootRenderData = rootRenderData;
			this.m_DirtyTracker.owner = this;
			this.quadTextureId = TextureId.invalid;
			this.parent = null;
			this.firstChild = null;
			this.nextSibling = null;
			this.m_DirtyTracker.heads = new List<RenderData>(8);
			this.m_DirtyTracker.tails = new List<RenderData>(8);
			this.m_DirtyTracker.minDepths = new int[5];
			this.m_DirtyTracker.maxDepths = new int[5];
			this.m_DirtyTracker.Reset();
		}

		public void Reset()
		{
			this.m_RenderTreeManager = null;
			this.m_RootRenderData = null;
			this.parent = null;
			this.firstChild = null;
			this.nextSibling = null;
		}

		public void Dispose()
		{
			bool flag = this.m_RootRenderData != null;
			if (flag)
			{
				this.DepthFirstResetTextures(this.m_RootRenderData);
			}
		}

		private void DepthFirstResetTextures(RenderData renderData)
		{
			this.m_GCHandlePool.ReturnAll();
			this.m_RenderTreeManager.ResetGraphicEntries(renderData);
			for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
			{
				this.DepthFirstResetTextures(renderData2);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRenderDataClippingChanged(RenderData renderData, bool hierarchical)
		{
			Debug.Assert((this.m_AllowedDirtyClasses & RenderTree.AllowedClasses.Clipping) > (RenderTree.AllowedClasses)0);
			this.m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.Clipping | (hierarchical ? RenderDataDirtyTypes.ClippingHierarchy : RenderDataDirtyTypes.None), RenderDataDirtyTypeClasses.Clipping);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRenderDataOpacityChanged(RenderData renderData, bool hierarchical = false)
		{
			Debug.Assert((this.m_AllowedDirtyClasses & RenderTree.AllowedClasses.Opacity) > (RenderTree.AllowedClasses)0);
			this.m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.Opacity | (hierarchical ? RenderDataDirtyTypes.OpacityHierarchy : RenderDataDirtyTypes.None), RenderDataDirtyTypeClasses.Opacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRenderDataColorChanged(RenderData renderData)
		{
			Debug.Assert((this.m_AllowedDirtyClasses & RenderTree.AllowedClasses.Color) > (RenderTree.AllowedClasses)0);
			this.m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.Color, RenderDataDirtyTypeClasses.Color);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRenderDataTransformOrSizeChanged(RenderData renderData, bool transformChanged, bool clipRectSizeChanged)
		{
			Debug.Assert((this.m_AllowedDirtyClasses & RenderTree.AllowedClasses.TransformSize) > (RenderTree.AllowedClasses)0);
			RenderDataDirtyTypes renderDataDirtyTypes = (transformChanged ? RenderDataDirtyTypes.Transform : RenderDataDirtyTypes.None) | (clipRectSizeChanged ? RenderDataDirtyTypes.ClipRectSize : RenderDataDirtyTypes.None);
			this.m_DirtyTracker.RegisterDirty(renderData, renderDataDirtyTypes, RenderDataDirtyTypeClasses.TransformSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRenderDataOpacityIdChanged(RenderData renderData)
		{
			Debug.Assert((this.m_AllowedDirtyClasses & RenderTree.AllowedClasses.Visuals) > (RenderTree.AllowedClasses)0);
			this.m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.VisualsOpacityId, RenderDataDirtyTypeClasses.Visuals);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnRenderDataVisualsChanged(RenderData renderData, bool hierarchical)
		{
			Debug.Assert((this.m_AllowedDirtyClasses & RenderTree.AllowedClasses.Visuals) > (RenderTree.AllowedClasses)0);
			this.m_DirtyTracker.RegisterDirty(renderData, RenderDataDirtyTypes.Visuals | (hierarchical ? RenderDataDirtyTypes.VisualsHierarchy : RenderDataDirtyTypes.None), RenderDataDirtyTypeClasses.Visuals);
		}

		public void ProcessChanges(ref ChainBuilderStats stats)
		{
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			int num = 0;
			RenderDataDirtyTypes renderDataDirtyTypes = RenderDataDirtyTypes.Clipping | RenderDataDirtyTypes.ClippingHierarchy;
			RenderDataDirtyTypes renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			this.m_AllowedDirtyClasses &= ~RenderTree.AllowedClasses.Clipping;
			for (int i = this.m_DirtyTracker.minDepths[num]; i <= this.m_DirtyTracker.maxDepths[num]; i++)
			{
				RenderData renderData = this.m_DirtyTracker.heads[i];
				while (renderData != null)
				{
					RenderData nextDirty = renderData.nextDirty;
					bool flag = (renderData.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag)
					{
						bool flag2 = renderData.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag2)
						{
							RenderEvents.ProcessOnClippingChanged(this.m_RenderTreeManager, renderData, this.m_DirtyTracker.dirtyID, ref stats);
						}
						this.m_DirtyTracker.ClearDirty(renderData, renderDataDirtyTypes2);
					}
					renderData = nextDirty;
					stats.dirtyProcessed += 1U;
				}
			}
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 1;
			renderDataDirtyTypes = RenderDataDirtyTypes.Opacity | RenderDataDirtyTypes.OpacityHierarchy;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			this.m_AllowedDirtyClasses &= ~RenderTree.AllowedClasses.Opacity;
			for (int j = this.m_DirtyTracker.minDepths[num]; j <= this.m_DirtyTracker.maxDepths[num]; j++)
			{
				RenderData renderData2 = this.m_DirtyTracker.heads[j];
				while (renderData2 != null)
				{
					RenderData nextDirty2 = renderData2.nextDirty;
					bool flag3 = (renderData2.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag3)
					{
						bool flag4 = renderData2.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag4)
						{
							RenderEvents.ProcessOnOpacityChanged(this.m_RenderTreeManager, renderData2, this.m_DirtyTracker.dirtyID, ref stats);
						}
						this.m_DirtyTracker.ClearDirty(renderData2, renderDataDirtyTypes2);
					}
					renderData2 = nextDirty2;
					stats.dirtyProcessed += 1U;
				}
			}
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 2;
			renderDataDirtyTypes = RenderDataDirtyTypes.Color;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			this.m_AllowedDirtyClasses &= ~RenderTree.AllowedClasses.Color;
			for (int k = this.m_DirtyTracker.minDepths[num]; k <= this.m_DirtyTracker.maxDepths[num]; k++)
			{
				RenderData renderData3 = this.m_DirtyTracker.heads[k];
				while (renderData3 != null)
				{
					RenderData nextDirty3 = renderData3.nextDirty;
					bool flag5 = (renderData3.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag5)
					{
						bool flag6 = renderData3 != null && renderData3.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag6)
						{
							RenderEvents.ProcessOnColorChanged(this.m_RenderTreeManager, renderData3, this.m_DirtyTracker.dirtyID, ref stats);
						}
						this.m_DirtyTracker.ClearDirty(renderData3, renderDataDirtyTypes2);
					}
					renderData3 = nextDirty3;
					stats.dirtyProcessed += 1U;
				}
			}
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 3;
			renderDataDirtyTypes = RenderDataDirtyTypes.Transform | RenderDataDirtyTypes.ClipRectSize;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			this.m_AllowedDirtyClasses &= ~RenderTree.AllowedClasses.TransformSize;
			for (int l = this.m_DirtyTracker.minDepths[num]; l <= this.m_DirtyTracker.maxDepths[num]; l++)
			{
				RenderData renderData4 = this.m_DirtyTracker.heads[l];
				while (renderData4 != null)
				{
					RenderData nextDirty4 = renderData4.nextDirty;
					bool flag7 = (renderData4.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag7)
					{
						bool flag8 = renderData4.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag8)
						{
							RenderEvents.ProcessOnTransformOrSizeChanged(this.m_RenderTreeManager, renderData4, this.m_DirtyTracker.dirtyID, ref stats);
						}
						this.m_DirtyTracker.ClearDirty(renderData4, renderDataDirtyTypes2);
					}
					renderData4 = nextDirty4;
					stats.dirtyProcessed += 1U;
				}
			}
			this.m_RenderTreeManager.jobManager.CompleteNudgeJobs();
			this.m_DirtyTracker.dirtyID = this.m_DirtyTracker.dirtyID + 1U;
			num = 4;
			renderDataDirtyTypes = RenderDataDirtyTypes.AllVisuals;
			renderDataDirtyTypes2 = ~renderDataDirtyTypes;
			this.m_AllowedDirtyClasses &= ~RenderTree.AllowedClasses.Visuals;
			for (int m = this.m_DirtyTracker.minDepths[num]; m <= this.m_DirtyTracker.maxDepths[num]; m++)
			{
				RenderData renderData5 = this.m_DirtyTracker.heads[m];
				while (renderData5 != null)
				{
					RenderData nextDirty5 = renderData5.nextDirty;
					bool flag9 = (renderData5.dirtiedValues & renderDataDirtyTypes) > RenderDataDirtyTypes.None;
					if (flag9)
					{
						bool flag10 = renderData5.dirtyID != this.m_DirtyTracker.dirtyID;
						if (flag10)
						{
							this.m_RenderTreeManager.visualChangesProcessor.ProcessOnVisualsChanged(renderData5, this.m_DirtyTracker.dirtyID, ref stats);
						}
						this.m_DirtyTracker.ClearDirty(renderData5, renderDataDirtyTypes2);
					}
					renderData5 = nextDirty5;
					stats.dirtyProcessed += 1U;
				}
			}
			this.m_RenderTreeManager.meshGenerationDeferrer.ProcessDeferredWork(this.m_RenderTreeManager.visualChangesProcessor.meshGenerationContext);
			this.m_RenderTreeManager.visualChangesProcessor.ScheduleMeshGenerationJobs();
			this.m_RenderTreeManager.meshGenerationDeferrer.ProcessDeferredWork(this.m_RenderTreeManager.visualChangesProcessor.meshGenerationContext);
			this.m_RenderTreeManager.visualChangesProcessor.ConvertEntriesToCommands(ref stats);
			this.m_RenderTreeManager.jobManager.CompleteConvertMeshJobs();
			this.m_RenderTreeManager.jobManager.CompleteCopyMeshJobs();
			this.m_RenderTreeManager.opacityIdAccelerator.CompleteJobs();
			this.m_DirtyTracker.Reset();
			this.m_AllowedDirtyClasses = RenderTree.AllowedClasses.All;
		}

		internal void OnRenderCommandAdded(RenderChainCommand command)
		{
			bool flag = command.prev == null;
			if (flag)
			{
				this.m_FirstCommand = command;
			}
		}

		internal void OnRenderCommandsRemoved(RenderChainCommand firstCommand, RenderChainCommand lastCommand)
		{
			bool flag = firstCommand.prev == null;
			if (flag)
			{
				this.m_FirstCommand = lastCommand.next;
			}
		}

		internal void ChildWillBeRemoved(RenderData renderData)
		{
			bool flag = renderData.dirtiedValues > RenderDataDirtyTypes.None;
			if (flag)
			{
				this.m_DirtyTracker.ClearDirty(renderData, ~renderData.dirtiedValues);
			}
			Debug.Assert(renderData.dirtiedValues == RenderDataDirtyTypes.None);
			Debug.Assert(renderData.prevDirty == null);
			Debug.Assert(renderData.nextDirty == null);
		}

		private RenderTreeManager m_RenderTreeManager;

		private DepthOrderedDirtyTracking m_DirtyTracker;

		private RenderChainCommand m_FirstCommand;

		private RenderData m_RootRenderData;

		public TextureId quadTextureId;

		public RectInt quadRect;

		public Rect quadUVRect;

		public GCHandlePool m_GCHandlePool = new GCHandlePool(256, 64);

		internal RenderTree parent;

		internal RenderTree firstChild;

		internal RenderTree nextSibling;

		private static readonly ProfilerMarker k_MarkerClipProcessing = new ProfilerMarker("RenderTree.UpdateClips");

		private static readonly ProfilerMarker k_MarkerOpacityProcessing = new ProfilerMarker("RenderTree.UpdateOpacity");

		private static readonly ProfilerMarker k_MarkerColorsProcessing = new ProfilerMarker("RenderTree.UpdateColors");

		private static readonly ProfilerMarker k_MarkerTransformProcessing = new ProfilerMarker("RenderTree.UpdateTransforms");

		private static readonly ProfilerMarker k_MarkerVisualsProcessing = new ProfilerMarker("RenderTree.UpdateVisuals");

		private RenderTree.AllowedClasses m_AllowedDirtyClasses = RenderTree.AllowedClasses.All;

		[Flags]
		internal enum AllowedClasses
		{
			Clipping = 1,
			Opacity = 2,
			Color = 4,
			TransformSize = 8,
			Visuals = 16,
			All = 31
		}
	}
}
