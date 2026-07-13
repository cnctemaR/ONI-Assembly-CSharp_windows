using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderTreeManager : IDisposable
	{
		internal TextureRegistry textureRegistry
		{
			get
			{
				return this.m_TextureRegistry;
			}
		}

		internal RenderTreeManager.VisualChangesProcessor visualChangesProcessor
		{
			get
			{
				return this.m_VisualChangesProcessor;
			}
		}

		public OpacityIdAccelerator opacityIdAccelerator { get; private set; }

		private bool blockDirtyRegistration { get; set; }

		public TextureSlotCount textureSlotCount { get; set; } = TextureSlotCount.Eight;

		internal RenderData GetPooledRenderData()
		{
			RenderData renderData = this.m_RenderDataPool.Get();
			renderData.Init();
			return renderData;
		}

		internal void ReturnPoolRenderData(RenderData data)
		{
			bool flag = data != null;
			if (flag)
			{
				data.Reset();
				this.m_RenderDataPool.Release(data);
			}
		}

		internal RenderTree GetPooledRenderTree(RenderTreeManager renderTreeManager, RenderData rootRenderData)
		{
			RenderTree renderTree = this.m_RenderTreePool.Get();
			renderTree.Init(renderTreeManager, rootRenderData);
			return renderTree;
		}

		internal void ReturnPoolRenderTree(RenderTree tree)
		{
			bool flag = tree != null;
			if (flag)
			{
				tree.Reset();
				this.m_RenderTreePool.Release(tree);
			}
		}

		public RenderTreeManager(BaseVisualElementPanel panel)
		{
			this.panel = panel;
			this.atlas = panel.atlas;
			this.vectorImageManager = new VectorImageManager(this.atlas);
			this.m_Compositor = new RenderTreeCompositor(this);
			this.tempMeshAllocator = new TempMeshAllocatorImpl();
			this.jobManager = new JobManager();
			this.opacityIdAccelerator = new OpacityIdAccelerator();
			this.meshGenerationNodeManager = new MeshGenerationNodeManager(this.entryRecorder);
			this.m_VisualChangesProcessor = new RenderTreeManager.VisualChangesProcessor(this);
			ColorSpace activeColorSpace = QualitySettings.activeColorSpace;
			this.m_DefaultMat = Shaders.defaultMaterial;
			bool flag = panel.contextType == ContextType.Player;
			if (flag)
			{
				BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)panel;
				this.drawInCameras = baseRuntimePanel.drawsInCameras;
				bool flag2 = !this.drawInCameras && activeColorSpace == ColorSpace.Linear;
				if (flag2)
				{
					this.forceGammaRendering = panel.panelRenderer.forceGammaRendering;
				}
			}
			else
			{
				bool flag3 = activeColorSpace == ColorSpace.Linear;
				if (flag3)
				{
					this.forceGammaRendering = true;
				}
			}
			this.isFlat = panel.isFlat;
			this.device = new UIRenderDevice(panel.panelRenderer.vertexBudget, 0U, this.isFlat, this.forceGammaRendering);
			Shaders.Acquire();
			this.shaderInfoAllocator = new UIRVEShaderInfoAllocator(this.forceGammaRendering ? ColorSpace.Gamma : activeColorSpace);
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
					Shaders.Release();
					RenderTreeManager.ReverseDepthFirstDisposeRenderTrees(this.m_RootRenderTree);
					this.m_RootRenderTree = null;
					this.tempMeshAllocator.Dispose();
					this.tempMeshAllocator = null;
					this.jobManager.Dispose();
					this.jobManager = null;
					VectorImageManager vectorImageManager = this.vectorImageManager;
					if (vectorImageManager != null)
					{
						vectorImageManager.Dispose();
					}
					this.vectorImageManager = null;
					this.shaderInfoAllocator.Dispose();
					this.shaderInfoAllocator = null;
					UIRenderDevice device = this.device;
					if (device != null)
					{
						device.Dispose();
					}
					this.device = null;
					OpacityIdAccelerator opacityIdAccelerator = this.opacityIdAccelerator;
					if (opacityIdAccelerator != null)
					{
						opacityIdAccelerator.Dispose();
					}
					this.opacityIdAccelerator = null;
					RenderTreeManager.VisualChangesProcessor visualChangesProcessor = this.m_VisualChangesProcessor;
					if (visualChangesProcessor != null)
					{
						visualChangesProcessor.Dispose();
					}
					this.m_VisualChangesProcessor = null;
					MeshGenerationDeferrer meshGenerationDeferrer = this.m_MeshGenerationDeferrer;
					if (meshGenerationDeferrer != null)
					{
						meshGenerationDeferrer.Dispose();
					}
					this.m_MeshGenerationDeferrer = null;
					this.meshGenerationNodeManager.Dispose();
					this.meshGenerationNodeManager = null;
					this.m_Compositor.Dispose();
					this.m_Compositor = null;
					this.m_RenderDataPool.Clear();
					this.m_RenderDataPool = null;
					foreach (RenderTreeManager.ElementInsertionData elementInsertionData in this.m_InsertionList)
					{
						elementInsertionData.element.insertionIndex = -1;
					}
					this.m_InsertionList.Clear();
					this.atlas = null;
				}
				this.disposed = true;
			}
		}

		private static void ReverseDepthFirstDisposeRenderTrees(RenderTree renderTree)
		{
			for (RenderTree renderTree2 = ((renderTree != null) ? renderTree.firstChild : null); renderTree2 != null; renderTree2 = renderTree2.nextSibling)
			{
				RenderTreeManager.ReverseDepthFirstDisposeRenderTrees(renderTree2);
			}
			if (renderTree != null)
			{
				renderTree.Dispose();
			}
		}

		internal ChainBuilderStats stats
		{
			get
			{
				return this.m_Stats;
			}
		}

		internal ref ChainBuilderStats statsByRef
		{
			get
			{
				return ref this.m_Stats;
			}
		}

		internal RenderTree rootRenderTree
		{
			get
			{
				return this.m_RootRenderTree;
			}
			set
			{
				Debug.Assert(this.m_RootRenderTree == null);
				this.m_RootRenderTree = value;
			}
		}

		private void DepthFirstProcessChanges(RenderTree renderTree)
		{
			renderTree.ProcessChanges(ref this.m_Stats);
			for (RenderTree renderTree2 = renderTree.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
			{
				this.DepthFirstProcessChanges(renderTree2);
			}
		}

		public void ProcessChanges()
		{
			this.m_Stats = default(ChainBuilderStats);
			this.m_Stats.elementsAdded = this.m_Stats.elementsAdded + this.m_StatsElementsAdded;
			this.m_Stats.elementsRemoved = this.m_Stats.elementsRemoved + this.m_StatsElementsRemoved;
			this.m_StatsElementsAdded = (this.m_StatsElementsRemoved = 0U);
			for (int i = 0; i < this.m_InsertionList.Count; i++)
			{
				RenderTreeManager.ElementInsertionData elementInsertionData = this.m_InsertionList[i];
				bool flag = !elementInsertionData.canceled;
				if (flag)
				{
					elementInsertionData.element.insertionIndex = -1;
					this.ProcessChildAdded(elementInsertionData.element);
				}
			}
			this.m_InsertionList.Clear();
			this.m_BlockDirtyRegistration = true;
			this.m_Compositor.Update(this.m_RootRenderTree);
			this.device.AdvanceFrame();
			this.DepthFirstProcessChanges(this.m_RootRenderTree);
			this.m_BlockDirtyRegistration = false;
			this.meshGenerationNodeManager.ResetAll();
			this.tempMeshAllocator.Clear();
			this.meshWriteDataPool.ReturnAll();
			this.entryPool.ReturnAll();
			AtlasBase atlas = this.atlas;
			if (atlas != null)
			{
				atlas.InvokeUpdateDynamicTextures(this.panel);
			}
			VectorImageManager vectorImageManager = this.vectorImageManager;
			if (vectorImageManager != null)
			{
				vectorImageManager.Commit();
			}
			this.shaderInfoAllocator.IssuePendingStorageChanges();
			this.device.OnFrameRenderingBegin();
			this.RenderNestedTrees();
			bool drawInCameras = this.drawInCameras;
			if (drawInCameras)
			{
				this.SerializeRootTreeCommands();
			}
		}

		private void SerializeRootTreeCommands()
		{
			Debug.Assert(this.drawInCameras);
			RenderTree rootRenderTree = this.m_RootRenderTree;
			bool flag = ((rootRenderTree != null) ? rootRenderTree.firstCommand : null) == null;
			if (!flag)
			{
				Exception ex = null;
				this.m_BlockDirtyRegistration = true;
				UIRenderDevice device = this.device;
				RenderChainCommand firstCommand = this.m_RootRenderTree.firstCommand;
				Material defaultMat = this.m_DefaultMat;
				VectorImageManager vectorImageManager = this.vectorImageManager;
				device.EvaluateChain(firstCommand, defaultMat, (vectorImageManager != null) ? vectorImageManager.atlas : null, this.shaderInfoAllocator.atlas, null, this.panel.scaledPixelsPerPoint, true, this.textureSlotCount, false, ref ex);
				this.m_BlockDirtyRegistration = false;
				Debug.Assert(ex == null);
			}
		}

		public void RenderRootTree()
		{
			Debug.Assert(!this.drawInCameras);
			PanelClearSettings clearSettings = this.panel.clearSettings;
			bool flag = clearSettings.clearColor || clearSettings.clearDepthStencil;
			if (flag)
			{
				Color color = clearSettings.color;
				color = color.RGBMultiplied(color.a);
				GL.Clear(clearSettings.clearDepthStencil, clearSettings.clearColor, color, 0.99f);
			}
			this.RenderSingleTree(this.m_RootRenderTree, null, RectInt.zero, Rect.zero);
			bool drawStats = this.drawStats;
			if (drawStats)
			{
				this.DrawStats();
			}
		}

		private void RenderNestedTrees()
		{
			this.m_Compositor.RenderNestedPasses();
		}

		public void RenderSingleTree(RenderTree renderTree, RenderTexture nestedTreeRT, RectInt nestedTreeViewport, Rect bounds)
		{
			Debug.Assert(!this.drawInCameras || renderTree != this.m_RootRenderTree);
			bool flag = renderTree.firstCommand == null;
			if (!flag)
			{
				Exception ex = null;
				bool flag2 = false;
				RenderTexture renderTexture = null;
				float num = this.panel.scaledPixelsPerPoint;
				bool flag3 = renderTree == this.m_RootRenderTree;
				Rect rect;
				if (flag3)
				{
					Debug.Assert(nestedTreeRT == null);
					Rect layout = this.panel.visualTree.layout;
					rect = new Rect(0f, 0f, layout.width, layout.height);
					bounds = layout;
				}
				else
				{
					Debug.Assert(nestedTreeRT != null);
					renderTexture = RenderTexture.active;
					Camera.SetupCurrent(null);
					RenderTexture.active = nestedTreeRT;
					flag2 = true;
					num = 1f;
					Rect rect2 = UIRUtility.CastToRect(nestedTreeViewport);
					rect = rect2;
					rect.y = rect.height - rect.yMax;
					GL.Viewport(rect2);
				}
				Matrix4x4 matrix4x = ProjectionUtils.Ortho(bounds.xMin, bounds.xMax, bounds.yMax, bounds.yMin, -0.001f, 1.001f);
				GL.LoadProjectionMatrix(matrix4x);
				GL.modelview = Matrix4x4.identity;
				this.m_BlockDirtyRegistration = this.drawInCameras;
				UIRenderDevice device = this.device;
				RenderChainCommand firstCommand = renderTree.firstCommand;
				Material defaultMat = this.m_DefaultMat;
				VectorImageManager vectorImageManager = this.vectorImageManager;
				device.EvaluateChain(firstCommand, defaultMat, (vectorImageManager != null) ? vectorImageManager.atlas : null, this.shaderInfoAllocator.atlas, new Rect?(rect), num, false, this.textureSlotCount, nestedTreeRT != null, ref ex);
				this.m_BlockDirtyRegistration = false;
				Utility.DisableScissor();
				bool flag4 = flag2;
				if (flag4)
				{
					RenderTexture.active = renderTexture;
				}
				bool flag5 = ex != null;
				if (flag5)
				{
					Debug.Assert(!this.drawInCameras);
					bool flag6 = GUIUtility.IsExitGUIException(ex);
					if (flag6)
					{
						throw ex;
					}
					throw new ImmediateModeException(ex);
				}
			}
		}

		public void CancelInsertion(VisualElement ve)
		{
			int insertionIndex = ve.insertionIndex;
			Debug.Assert(insertionIndex >= 0 && insertionIndex < this.m_InsertionList.Count);
			RenderTreeManager.ElementInsertionData elementInsertionData = this.m_InsertionList[insertionIndex];
			elementInsertionData.canceled = true;
			this.m_InsertionList[insertionIndex] = elementInsertionData;
			ve.insertionIndex = -1;
		}

		public void UIEOnChildAdded(VisualElement ve)
		{
			ve.insertionIndex = this.m_InsertionList.Count;
			this.m_InsertionList.Add(new RenderTreeManager.ElementInsertionData
			{
				element = ve,
				canceled = false
			});
		}

		private void ProcessChildAdded(VisualElement ve)
		{
			VisualElement parent = ve.hierarchy.parent;
			int num = ((parent != null) ? parent.hierarchy.IndexOf(ve) : 0);
			bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
			if (blockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot be added to an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
			}
			bool flag = parent != null && parent.renderData == null;
			if (!flag)
			{
				uint num2 = RenderEvents.DepthFirstOnChildAdded(this, parent, ve, num);
				Debug.Assert(ve.renderData != null);
				Debug.Assert(ve.panel == this.panel);
				this.UIEOnClippingChanged(ve, true);
				this.UIEOnOpacityChanged(ve, false);
				this.UIEOnTransformOrSizeChanged(ve, true, true);
				this.UIEOnVisualsChanged(ve, true);
				ve.MarkRenderHintsClean();
				this.m_StatsElementsAdded += num2;
			}
		}

		public void UIEOnChildrenReordered(VisualElement ve)
		{
			bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
			if (blockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot be moved under an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
			}
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				RenderEvents.DepthFirstOnElementRemoving(this, ve.hierarchy[i]);
			}
			for (int j = 0; j < childCount; j++)
			{
				this.UIEOnChildAdded(ve.hierarchy[j]);
			}
			this.UIEOnClippingChanged(ve, true);
			this.UIEOnOpacityChanged(ve, true);
			this.UIEOnVisualsChanged(ve, true);
		}

		public void UIEOnChildRemoving(VisualElement ve)
		{
			bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
			if (blockDirtyRegistration)
			{
				throw new InvalidOperationException("VisualElements cannot be removed from an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
			}
			this.m_StatsElementsRemoved += RenderEvents.DepthFirstOnElementRemoving(this, ve);
			Debug.Assert(ve.renderData == null);
		}

		public void UIEOnRenderHintsChanged(VisualElement ve)
		{
			bool flag = ve.renderData != null;
			if (flag)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("Render Hints cannot change under an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
				}
				bool flag2 = (ve.renderHints & RenderHints.DirtyAll) == RenderHints.DirtyDynamicColor;
				bool flag3 = flag2;
				if (flag3)
				{
					this.UIEOnVisualsChanged(ve, false);
				}
				else
				{
					this.UIEOnChildRemoving(ve);
					this.UIEOnChildAdded(ve);
				}
				ve.MarkRenderHintsClean();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RegisterDirty(VisualElement ve, RenderDataDirtyTypes dirtyTypes, RenderDataDirtyTypeClasses dirtyClasses)
		{
			RenderData renderData = ve.renderData;
			bool flag = renderData != null;
			if (flag)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("VisualElements cannot change their render data under an active visual tree during generateVisualContent callback execution nor during visual tree rendering");
				}
				renderData.renderTree.dirtyTracker.RegisterDirty(renderData, dirtyTypes, dirtyClasses);
				bool flag2 = ve.nestedRenderData != null;
				if (flag2)
				{
					ve.nestedRenderData.renderTree.dirtyTracker.RegisterDirty(ve.nestedRenderData, dirtyTypes, dirtyClasses);
				}
			}
		}

		public void UIEOnClippingChanged(VisualElement ve, bool hierarchical)
		{
			this.RegisterDirty(ve, RenderDataDirtyTypes.Clipping | (hierarchical ? RenderDataDirtyTypes.ClippingHierarchy : RenderDataDirtyTypes.None), RenderDataDirtyTypeClasses.Clipping);
		}

		public void UIEOnOpacityChanged(VisualElement ve, bool hierarchical = false)
		{
			this.RegisterDirty(ve, RenderDataDirtyTypes.Opacity | (hierarchical ? RenderDataDirtyTypes.OpacityHierarchy : RenderDataDirtyTypes.None), RenderDataDirtyTypeClasses.Opacity);
		}

		public void UIEOnColorChanged(VisualElement ve)
		{
			this.RegisterDirty(ve, RenderDataDirtyTypes.Color, RenderDataDirtyTypeClasses.Color);
		}

		public void UIEOnTransformOrSizeChanged(VisualElement ve, bool transformChanged, bool clipRectSizeChanged)
		{
			RenderDataDirtyTypes renderDataDirtyTypes = (transformChanged ? RenderDataDirtyTypes.Transform : RenderDataDirtyTypes.None) | (clipRectSizeChanged ? RenderDataDirtyTypes.ClipRectSize : RenderDataDirtyTypes.None);
			this.RegisterDirty(ve, renderDataDirtyTypes, RenderDataDirtyTypeClasses.TransformSize);
		}

		public void UIEOnVisualsChanged(VisualElement ve, bool hierarchical)
		{
			this.RegisterDirty(ve, RenderDataDirtyTypes.Visuals | (hierarchical ? RenderDataDirtyTypes.VisualsHierarchy : RenderDataDirtyTypes.None), RenderDataDirtyTypeClasses.Visuals);
		}

		public void UIEOnOpacityIdChanged(VisualElement ve)
		{
			this.RegisterDirty(ve, RenderDataDirtyTypes.VisualsOpacityId, RenderDataDirtyTypeClasses.Visuals);
		}

		public void UIEOnDisableRenderingChanged(VisualElement ve)
		{
			bool flag = ve.renderData != null;
			if (flag)
			{
				bool blockDirtyRegistration = this.m_BlockDirtyRegistration;
				if (blockDirtyRegistration)
				{
					throw new InvalidOperationException("VisualElements cannot change their display style during generateVisualContent callback execution nor during visual tree rendering");
				}
				CommandManipulator.DisableElementRendering(this, ve, ve.disableRendering);
			}
		}

		internal BaseVisualElementPanel panel { get; private set; }

		internal UIRenderDevice device { get; private set; }

		public BaseElementBuilder elementBuilder
		{
			get
			{
				return this.m_VisualChangesProcessor.elementBuilder;
			}
		}

		internal AtlasBase atlas { get; private set; }

		internal VectorImageManager vectorImageManager { get; private set; }

		internal TempMeshAllocatorImpl tempMeshAllocator { get; private set; }

		internal MeshWriteDataPool meshWriteDataPool { get; } = new MeshWriteDataPool();

		internal EntryPool entryPool
		{
			get
			{
				return RenderTreeManager.s_SharedEntryPool;
			}
		}

		public MeshGenerationDeferrer meshGenerationDeferrer
		{
			get
			{
				return this.m_MeshGenerationDeferrer;
			}
		}

		public MeshGenerationNodeManager meshGenerationNodeManager { get; private set; }

		internal JobManager jobManager { get; private set; }

		internal bool drawStats { get; set; }

		internal bool drawInCameras { get; }

		internal bool isFlat { get; }

		public bool forceGammaRendering { get; }

		internal RenderChainCommand AllocCommand()
		{
			return this.m_CommandPool.Get();
		}

		internal void FreeCommand(RenderChainCommand cmd)
		{
			cmd.Reset();
			this.m_CommandPool.Return(cmd);
		}

		internal void RepaintTexturedElements()
		{
			bool flag = this.m_RootRenderTree != null;
			if (flag)
			{
				this.DepthFirstRepaintTextured(this.m_RootRenderTree);
			}
		}

		private void DepthFirstRepaintTextured(RenderTree renderTree)
		{
			RenderData rootRenderData = renderTree.rootRenderData;
			bool flag = rootRenderData != null;
			if (flag)
			{
				this.DepthFirstRepaintTextured(rootRenderData);
			}
			for (RenderTree renderTree2 = renderTree.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
			{
				this.DepthFirstRepaintTextured(renderTree2);
			}
		}

		private void DepthFirstRepaintTextured(RenderData renderData)
		{
			bool flag = renderData.graphicEntries != null;
			if (flag)
			{
				this.UIEOnVisualsChanged(renderData.owner, false);
			}
			for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
			{
				this.DepthFirstRepaintTextured(renderData2);
			}
		}

		public ExtraRenderData GetOrAddExtraData(RenderData renderData)
		{
			ExtraRenderData extraRenderData;
			bool flag = !this.m_ExtraData.TryGetValue(renderData, out extraRenderData);
			if (flag)
			{
				extraRenderData = this.m_ExtraDataPool.Get();
				this.m_ExtraData.Add(renderData, extraRenderData);
				renderData.flags |= RenderDataFlags.HasExtraData;
			}
			return extraRenderData;
		}

		public void FreeExtraData(RenderData renderData)
		{
			Debug.Assert(renderData.hasExtraData);
			Debug.Assert(!renderData.hasExtraMeshes);
			ExtraRenderData extraRenderData;
			this.m_ExtraData.Remove(renderData, out extraRenderData);
			this.m_ExtraDataPool.Return(extraRenderData);
			renderData.flags &= ~RenderDataFlags.HasExtraData;
		}

		public void InsertExtraMesh(RenderData renderData, MeshHandle mesh)
		{
			ExtraRenderData orAddExtraData = this.GetOrAddExtraData(renderData);
			BasicNode<MeshHandle> basicNode = this.m_MeshHandleNodePool.Get();
			basicNode.data = mesh;
			basicNode.InsertFirst(ref orAddExtraData.extraMesh);
			renderData.flags |= RenderDataFlags.HasExtraMeshes;
		}

		public void FreeExtraMeshes(RenderData renderData)
		{
			bool flag = !renderData.hasExtraMeshes;
			if (!flag)
			{
				ExtraRenderData extraRenderData = this.m_ExtraData[renderData];
				BasicNode<MeshHandle> basicNode = extraRenderData.extraMesh;
				extraRenderData.extraMesh = null;
				while (basicNode != null)
				{
					this.device.Free(basicNode.data);
					BasicNode<MeshHandle> next = basicNode.next;
					basicNode.data = null;
					basicNode.next = null;
					this.m_MeshHandleNodePool.Return(basicNode);
					basicNode = next;
				}
				renderData.flags &= ~RenderDataFlags.HasExtraMeshes;
			}
		}

		public void InsertTexture(RenderData renderData, Texture src, TextureId id, bool isAtlas)
		{
			BasicNode<GraphicEntry> basicNode = this.m_GraphicEntryPool.Get();
			basicNode.data.source = src;
			basicNode.data.actual = id;
			basicNode.data.replaced = isAtlas;
			basicNode.InsertFirst(ref renderData.graphicEntries);
		}

		public void InsertVectorImage(RenderData renderData, VectorImage vi)
		{
			BasicNode<GraphicEntry> basicNode = this.m_GraphicEntryPool.Get();
			basicNode.data.vectorImage = vi;
			basicNode.InsertFirst(ref renderData.graphicEntries);
		}

		public void ResetGraphicEntries(RenderData renderData)
		{
			AtlasBase atlas = this.atlas;
			TextureRegistry textureRegistry = this.m_TextureRegistry;
			BasicNodePool<GraphicEntry> graphicEntryPool = this.m_GraphicEntryPool;
			BasicNode<GraphicEntry> basicNode = renderData.graphicEntries;
			renderData.graphicEntries = null;
			while (basicNode != null)
			{
				BasicNode<GraphicEntry> next = basicNode.next;
				bool flag = basicNode.data.vectorImage != null;
				if (flag)
				{
					this.vectorImageManager.RemoveUser(basicNode.data.vectorImage);
					basicNode.data.vectorImage = null;
				}
				else
				{
					bool replaced = basicNode.data.replaced;
					if (replaced)
					{
						atlas.ReturnAtlas(renderData.owner, basicNode.data.source as Texture2D, basicNode.data.actual);
					}
					else
					{
						textureRegistry.Release(basicNode.data.actual);
					}
					basicNode.data.source = null;
				}
				graphicEntryPool.Return(basicNode);
				basicNode = next;
			}
		}

		private void DrawStats()
		{
			bool flag = this.device != null;
			float num = 12f;
			Rect rect = new Rect(30f, 60f, 1000f, 100f);
			GUI.Box(new Rect(20f, 40f, 200f, (float)(flag ? 380 : 256)), "UI Toolkit Draw Stats");
			GUI.Label(rect, "Elements added\t: " + this.m_Stats.elementsAdded.ToString());
			rect.y += num;
			GUI.Label(rect, "Elements removed\t: " + this.m_Stats.elementsRemoved.ToString());
			rect.y += num;
			GUI.Label(rect, "Mesh allocs allocated\t: " + this.m_Stats.newMeshAllocations.ToString());
			rect.y += num;
			GUI.Label(rect, "Mesh allocs updated\t: " + this.m_Stats.updatedMeshAllocations.ToString());
			rect.y += num;
			GUI.Label(rect, "Clip update roots\t: " + this.m_Stats.recursiveClipUpdates.ToString());
			rect.y += num;
			GUI.Label(rect, "Clip update total\t: " + this.m_Stats.recursiveClipUpdatesExpanded.ToString());
			rect.y += num;
			GUI.Label(rect, "Opacity update roots\t: " + this.m_Stats.recursiveOpacityUpdates.ToString());
			rect.y += num;
			GUI.Label(rect, "Opacity update total\t: " + this.m_Stats.recursiveOpacityUpdatesExpanded.ToString());
			rect.y += num;
			GUI.Label(rect, "Opacity ID update\t: " + this.m_Stats.opacityIdUpdates.ToString());
			rect.y += num;
			GUI.Label(rect, "Xform update roots\t: " + this.m_Stats.recursiveTransformUpdates.ToString());
			rect.y += num;
			GUI.Label(rect, "Xform update total\t: " + this.m_Stats.recursiveTransformUpdatesExpanded.ToString());
			rect.y += num;
			GUI.Label(rect, "Xformed by bone\t: " + this.m_Stats.boneTransformed.ToString());
			rect.y += num;
			GUI.Label(rect, "Xformed by skipping\t: " + this.m_Stats.skipTransformed.ToString());
			rect.y += num;
			GUI.Label(rect, "Xformed by nudging\t: " + this.m_Stats.nudgeTransformed.ToString());
			rect.y += num;
			GUI.Label(rect, "Xformed by repaint\t: " + this.m_Stats.visualUpdateTransformed.ToString());
			rect.y += num;
			GUI.Label(rect, "Visual update roots\t: " + this.m_Stats.recursiveVisualUpdates.ToString());
			rect.y += num;
			GUI.Label(rect, "Visual update total\t: " + this.m_Stats.recursiveVisualUpdatesExpanded.ToString());
			rect.y += num;
			GUI.Label(rect, "Visual update flats\t: " + this.m_Stats.nonRecursiveVisualUpdates.ToString());
			rect.y += num;
			GUI.Label(rect, "Dirty processed\t: " + this.m_Stats.dirtyProcessed.ToString());
			rect.y += num;
			GUI.Label(rect, "Group-xform updates\t: " + this.m_Stats.groupTransformElementsChanged.ToString());
			rect.y += num;
			bool flag2 = !flag;
			if (!flag2)
			{
				rect.y += num;
				UIRenderDevice.DrawStatistics drawStatistics = this.device.GatherDrawStatistics();
				GUI.Label(rect, "Frame index\t: " + drawStatistics.currentFrameIndex.ToString());
				rect.y += num;
				GUI.Label(rect, "Command count\t: " + drawStatistics.commandCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Skip cmd counts\t: " + drawStatistics.skippedCommandCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Draw commands\t: " + drawStatistics.drawCommandCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Disable commands\t: " + drawStatistics.disableCommandCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Draw ranges\t: " + drawStatistics.drawRangeCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Draw range calls\t: " + drawStatistics.drawRangeCallCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Material sets\t: " + drawStatistics.materialSetCount.ToString());
				rect.y += num;
				GUI.Label(rect, "Stencil changes\t: " + drawStatistics.stencilRefChanges.ToString());
				rect.y += num;
				GUI.Label(rect, "Immediate draws\t: " + drawStatistics.immediateDraws.ToString());
				rect.y += num;
				GUI.Label(rect, "Total triangles\t: " + (drawStatistics.totalIndices / 3U).ToString());
				rect.y += num;
			}
		}

		private RenderTreeCompositor m_Compositor;

		private RenderTreeManager.VisualChangesProcessor m_VisualChangesProcessor;

		private LinkedPool<RenderChainCommand> m_CommandPool = new LinkedPool<RenderChainCommand>(() => new RenderChainCommand(), delegate(RenderChainCommand cmd)
		{
			cmd.Reset();
		}, 10000);

		private LinkedPool<ExtraRenderData> m_ExtraDataPool = new LinkedPool<ExtraRenderData>(() => new ExtraRenderData(), null, 10000);

		private BasicNodePool<MeshHandle> m_MeshHandleNodePool = new BasicNodePool<MeshHandle>();

		private BasicNodePool<GraphicEntry> m_GraphicEntryPool = new BasicNodePool<GraphicEntry>();

		private Dictionary<RenderData, ExtraRenderData> m_ExtraData = new Dictionary<RenderData, ExtraRenderData>();

		internal List<RenderTreeManager.ElementInsertionData> m_InsertionList = new List<RenderTreeManager.ElementInsertionData>(1024);

		private MeshGenerationDeferrer m_MeshGenerationDeferrer = new MeshGenerationDeferrer();

		private Material m_DefaultMat;

		private bool m_BlockDirtyRegistration;

		private ChainBuilderStats m_Stats;

		private uint m_StatsElementsAdded;

		private uint m_StatsElementsRemoved;

		private TextureRegistry m_TextureRegistry = TextureRegistry.instance;

		private ObjectPool<RenderData> m_RenderDataPool = new ObjectPool<RenderData>(() => new RenderData(), null, null, null, false, 256, 1024);

		private ObjectPool<RenderTree> m_RenderTreePool = new ObjectPool<RenderTree>(() => new RenderTree(), null, null, null, false, 8, 128);

		private static EntryPool s_SharedEntryPool = new EntryPool(10000);

		private static readonly ProfilerMarker k_MarkerProcess = new ProfilerMarker("RenderTreeManager.Process");

		private static readonly ProfilerMarker k_MarkerSerialize = new ProfilerMarker("RenderChain.Serialize");

		private RenderTree m_RootRenderTree;

		public EntryRecorder entryRecorder = new EntryRecorder(RenderTreeManager.s_SharedEntryPool);

		internal UIRVEShaderInfoAllocator shaderInfoAllocator;

		internal struct ElementInsertionData
		{
			public VisualElement element;

			public bool canceled;
		}

		internal class VisualChangesProcessor : IDisposable
		{
			public BaseElementBuilder elementBuilder
			{
				get
				{
					return this.m_ElementBuilder;
				}
			}

			public MeshGenerationContext meshGenerationContext
			{
				get
				{
					return this.m_MeshGenerationContext;
				}
			}

			public VisualChangesProcessor(RenderTreeManager renderTreeManager)
			{
				this.m_RenderTreeManager = renderTreeManager;
				this.m_MeshGenerationContext = new MeshGenerationContext(this.m_RenderTreeManager.meshWriteDataPool, this.m_RenderTreeManager.entryRecorder, this.m_RenderTreeManager.tempMeshAllocator, this.m_RenderTreeManager.meshGenerationDeferrer, this.m_RenderTreeManager.meshGenerationNodeManager);
				this.m_ElementBuilder = new DefaultElementBuilder(this.m_RenderTreeManager);
				this.m_EntryProcessingList = new List<RenderTreeManager.VisualChangesProcessor.EntryProcessingInfo>();
				this.m_Processors = new List<EntryProcessor>(4);
			}

			public void ScheduleMeshGenerationJobs()
			{
				this.m_ElementBuilder.ScheduleMeshGenerationJobs(this.m_MeshGenerationContext);
			}

			public void ProcessOnVisualsChanged(RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
			{
				bool flag = renderData.pendingHierarchicalRepaint || (renderData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) > RenderDataDirtyTypes.None;
				bool flag2 = flag;
				if (flag2)
				{
					stats.recursiveVisualUpdates += 1U;
				}
				else
				{
					stats.nonRecursiveVisualUpdates += 1U;
				}
				this.DepthFirstOnVisualsChanged(renderData, dirtyID, flag, ref stats);
			}

			private void DepthFirstOnVisualsChanged(RenderData renderData, uint dirtyID, bool hierarchical, ref ChainBuilderStats stats)
			{
				bool flag = dirtyID == renderData.dirtyID;
				if (!flag)
				{
					renderData.dirtyID = dirtyID;
					if (hierarchical)
					{
						stats.recursiveVisualUpdatesExpanded += 1U;
					}
					bool flag2 = !renderData.owner.areAncestorsAndSelfDisplayed;
					if (flag2)
					{
						if (hierarchical)
						{
							renderData.pendingHierarchicalRepaint = true;
						}
						else
						{
							renderData.pendingRepaint = true;
						}
					}
					else
					{
						renderData.pendingHierarchicalRepaint = false;
						renderData.pendingRepaint = false;
						bool flag3 = !hierarchical && (renderData.dirtiedValues & RenderDataDirtyTypes.AllVisuals) == RenderDataDirtyTypes.VisualsOpacityId;
						if (flag3)
						{
							stats.opacityIdUpdates += 1U;
							RenderTreeManager.VisualChangesProcessor.UpdateOpacityId(renderData, this.m_RenderTreeManager);
						}
						else
						{
							RenderTreeManager.VisualChangesProcessor.UpdateWorldFlipsWinding(renderData);
							Debug.Assert(renderData.clipMethod > ClipMethod.Undetermined);
							Debug.Assert(RenderData.AllocatesID(renderData.transformID) || renderData.parent == null || renderData.transformID.Equals(renderData.parent.transformID) || renderData.isGroupTransform);
							bool flag4 = renderData.owner is TextElement;
							if (flag4)
							{
								RenderEvents.UpdateTextCoreSettings(this.m_RenderTreeManager, renderData.owner);
							}
							bool flag5 = (renderData.owner.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor;
							if (flag5)
							{
								RenderEvents.SetColorValues(this.m_RenderTreeManager, renderData.owner);
							}
							Entry entry = this.m_RenderTreeManager.entryPool.Get();
							entry.type = EntryType.DedicatedPlaceholder;
							this.m_EntryProcessingList.Add(new RenderTreeManager.VisualChangesProcessor.EntryProcessingInfo
							{
								type = RenderTreeManager.VisualChangesProcessor.VisualsProcessingType.Head,
								renderData = renderData,
								rootEntry = entry
							});
							this.m_MeshGenerationContext.Begin(entry, renderData.owner, renderData);
							this.m_ElementBuilder.Build(this.m_MeshGenerationContext);
							this.m_MeshGenerationContext.End();
							if (hierarchical)
							{
								for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
								{
									this.DepthFirstOnVisualsChanged(renderData2, dirtyID, true, ref stats);
								}
							}
							this.m_EntryProcessingList.Add(new RenderTreeManager.VisualChangesProcessor.EntryProcessingInfo
							{
								type = RenderTreeManager.VisualChangesProcessor.VisualsProcessingType.Tail,
								renderData = renderData,
								rootEntry = entry
							});
						}
					}
				}
			}

			private static void UpdateWorldFlipsWinding(RenderData renderData)
			{
				bool localFlipsWinding = renderData.localFlipsWinding;
				RenderData parent = renderData.parent;
				bool flag = parent != null && parent.worldFlipsWinding;
				renderData.worldFlipsWinding = flag ^ localFlipsWinding;
			}

			public void ConvertEntriesToCommands(ref ChainBuilderStats stats)
			{
				int num = 0;
				for (int i = 0; i < this.m_EntryProcessingList.Count; i++)
				{
					RenderTreeManager.VisualChangesProcessor.EntryProcessingInfo entryProcessingInfo = this.m_EntryProcessingList[i];
					bool flag = entryProcessingInfo.type == RenderTreeManager.VisualChangesProcessor.VisualsProcessingType.Head;
					if (flag)
					{
						bool flag2 = num < this.m_Processors.Count;
						EntryProcessor entryProcessor;
						if (flag2)
						{
							entryProcessor = this.m_Processors[num];
						}
						else
						{
							entryProcessor = new EntryProcessor();
							this.m_Processors.Add(entryProcessor);
						}
						num++;
						entryProcessor.Init(entryProcessingInfo.rootEntry, this.m_RenderTreeManager, entryProcessingInfo.renderData);
						entryProcessor.ProcessHead();
						CommandManipulator.ReplaceHeadCommands(this.m_RenderTreeManager, entryProcessingInfo.renderData, entryProcessor);
					}
					else
					{
						num--;
						EntryProcessor entryProcessor2 = this.m_Processors[num];
						entryProcessor2.ProcessTail();
						CommandManipulator.ReplaceTailCommands(this.m_RenderTreeManager, entryProcessingInfo.renderData, entryProcessor2);
					}
				}
				this.m_EntryProcessingList.Clear();
				for (int j = 0; j < this.m_Processors.Count; j++)
				{
					this.m_Processors[j].ClearReferences();
				}
			}

			public static void UpdateOpacityId(RenderData renderData, RenderTreeManager renderTreeManager)
			{
				bool flag = renderData.headMesh != null;
				if (flag)
				{
					RenderTreeManager.VisualChangesProcessor.DoUpdateOpacityId(renderData, renderTreeManager, renderData.headMesh);
				}
				bool flag2 = renderData.tailMesh != null;
				if (flag2)
				{
					RenderTreeManager.VisualChangesProcessor.DoUpdateOpacityId(renderData, renderTreeManager, renderData.tailMesh);
				}
				bool hasExtraMeshes = renderData.hasExtraMeshes;
				if (hasExtraMeshes)
				{
					ExtraRenderData orAddExtraData = renderTreeManager.GetOrAddExtraData(renderData);
					for (BasicNode<MeshHandle> basicNode = orAddExtraData.extraMesh; basicNode != null; basicNode = basicNode.next)
					{
						RenderTreeManager.VisualChangesProcessor.DoUpdateOpacityId(renderData, renderTreeManager, basicNode.data);
					}
				}
			}

			private static void DoUpdateOpacityId(RenderData renderData, RenderTreeManager renderTreeManager, MeshHandle mesh)
			{
				int size = (int)mesh.allocVerts.size;
				NativeSlice<Vertex> nativeSlice = mesh.allocPage.vertices.cpuData.Slice<Vertex>((int)mesh.allocVerts.start, size);
				NativeSlice<Vertex> nativeSlice2;
				renderTreeManager.device.Update(mesh, (uint)size, out nativeSlice2);
				Color32 color = renderTreeManager.shaderInfoAllocator.OpacityAllocToVertexData(renderData.opacityID);
				renderTreeManager.opacityIdAccelerator.CreateJob(nativeSlice, nativeSlice2, color, size);
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
						this.m_MeshGenerationContext.Dispose();
						this.m_MeshGenerationContext = null;
					}
					this.disposed = true;
				}
			}

			private static readonly ProfilerMarker k_GenerateEntriesMarker = new ProfilerMarker("UIR.GenerateEntries");

			private static readonly ProfilerMarker k_ConvertEntriesToCommandsMarker = new ProfilerMarker("UIR.ConvertEntriesToCommands");

			private static readonly ProfilerMarker k_UpdateOpacityIdMarker = new ProfilerMarker("UIR.UpdateOpacityId");

			private RenderTreeManager m_RenderTreeManager;

			private MeshGenerationContext m_MeshGenerationContext;

			private BaseElementBuilder m_ElementBuilder;

			private List<RenderTreeManager.VisualChangesProcessor.EntryProcessingInfo> m_EntryProcessingList;

			private List<EntryProcessor> m_Processors;

			private enum VisualsProcessingType
			{
				Head,
				Tail
			}

			private struct EntryProcessingInfo
			{
				public RenderData renderData;

				public RenderTreeManager.VisualChangesProcessor.VisualsProcessingType type;

				public Entry rootEntry;
			}
		}
	}
}
