using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderTreeCompositor : IDisposable
	{
		public RenderTreeCompositor(RenderTreeManager owner)
		{
			this.m_RenderTreeManager = owner;
		}

		public void Update(RenderTree rootRenderTree)
		{
			this.CleanupOperationTree();
			bool flag = rootRenderTree == null;
			if (!flag)
			{
				this.BuildDrawOperationTree(rootRenderTree);
				this.UpdateDrawBounds_PostOrder(this.m_RootOperation);
				this.AssignTextureIds_DepthFirst(this.m_RootOperation);
			}
		}

		private void BuildDrawOperationTree(RenderTree rootRenderTree)
		{
			this.m_RootOperation = this.m_DrawOperationPool.Get();
			this.m_RootOperation.Init(rootRenderTree);
			for (RenderTree renderTree = rootRenderTree.firstChild; renderTree != null; renderTree = renderTree.nextSibling)
			{
				this.AddChildrenOperations_DepthFirst(this.m_RootOperation, renderTree);
			}
		}

		private void AddChildrenOperations_DepthFirst(RenderTreeCompositor.DrawOperation parentOperation, RenderTree renderTree)
		{
			VisualElement owner = renderTree.rootRenderData.owner;
			List<FilterFunction> list = owner.resolvedStyle.filter as List<FilterFunction>;
			bool flag = list == null;
			if (flag)
			{
				throw new InvalidOperationException("Filter IEnumerable is not a List<FilterFunction>");
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				FilterFunctionDefinition definition = list[i].GetDefinition();
				bool flag2 = ((definition != null) ? definition.passes : null) == null;
				if (!flag2)
				{
					for (int j = definition.passes.Length - 1; j >= 0; j--)
					{
						PostProcessingPass postProcessingPass = definition.passes[j];
						bool flag3 = postProcessingPass.material == null;
						if (!flag3)
						{
							RenderTreeCompositor.DrawOperation drawOperation = this.m_DrawOperationPool.Get();
							drawOperation.Init(owner, in postProcessingPass, j, list[i]);
							parentOperation.AddChild(drawOperation);
							parentOperation = drawOperation;
						}
					}
				}
			}
			RenderTreeCompositor.DrawOperation drawOperation2 = this.m_DrawOperationPool.Get();
			drawOperation2.Init(renderTree);
			parentOperation.AddChild(drawOperation2);
			for (RenderTree renderTree2 = renderTree.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
			{
				this.AddChildrenOperations_DepthFirst(drawOperation2, renderTree2);
			}
		}

		private static PostProcessingMargins GetReadMargins(PostProcessingPass effect, FilterFunction func)
		{
			bool flag = effect.computeRequiredReadMarginsCallback != null;
			PostProcessingMargins postProcessingMargins;
			if (flag)
			{
				postProcessingMargins = effect.computeRequiredReadMarginsCallback(func);
			}
			else
			{
				postProcessingMargins = effect.readMargins;
			}
			return postProcessingMargins;
		}

		private static PostProcessingMargins GetWriteMargins(PostProcessingPass effect, FilterFunction func)
		{
			bool flag = effect.computeRequiredWriteMarginsCallback != null;
			PostProcessingMargins postProcessingMargins;
			if (flag)
			{
				postProcessingMargins = effect.computeRequiredWriteMarginsCallback(func);
			}
			else
			{
				postProcessingMargins = effect.writeMargins;
			}
			return postProcessingMargins;
		}

		private void UpdateDrawBounds_PostOrder(RenderTreeCompositor.DrawOperation op)
		{
			Rect? rect = null;
			RenderTreeCompositor.DrawOperationType type = op.type;
			RenderTreeCompositor.DrawOperationType drawOperationType = type;
			if (drawOperationType != RenderTreeCompositor.DrawOperationType.RenderTree)
			{
				if (drawOperationType != RenderTreeCompositor.DrawOperationType.Effect)
				{
					throw new NotImplementedException();
				}
				RenderTreeCompositor.DrawOperation firstChild = op.firstChild;
				bool flag = firstChild != null;
				if (flag)
				{
					Debug.Assert(firstChild.nextSibling == null);
					this.UpdateDrawBounds_PostOrder(firstChild);
					bool flag2 = UIRUtility.RectHasArea(op.drawSourceBounds);
					if (flag2)
					{
						rect = new Rect?(UIRUtility.CastToRect(op.drawSourceBounds));
					}
				}
			}
			else
			{
				for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
				{
					this.UpdateDrawBounds_PostOrder(drawOperation);
					bool flag3 = UIRUtility.RectHasArea(drawOperation.bounds);
					if (flag3)
					{
						Matrix4x4 matrix4x;
						UIRUtility.ComputeMatrixRelativeToRenderTree(drawOperation.visualElement.renderData, out matrix4x);
						Rect rect2 = VisualElement.CalculateConservativeRect(ref matrix4x, UIRUtility.CastToRect(drawOperation.bounds));
						rect = new Rect?((rect == null) ? rect2 : UIRUtility.Encapsulate(rect.Value, rect2));
					}
				}
				Rect boundingBox = op.renderTree.rootRenderData.owner.boundingBox;
				bool flag4 = UIRUtility.RectHasArea(boundingBox);
				if (flag4)
				{
					rect = new Rect?((rect == null) ? boundingBox : UIRUtility.Encapsulate(rect.Value, boundingBox));
				}
				else
				{
					Debug.Assert(rect == null);
				}
			}
			bool flag5 = rect != null;
			if (flag5)
			{
				Rect value = rect.Value;
				PostProcessingMargins postProcessingMargins = default(PostProcessingMargins);
				PostProcessingMargins postProcessingMargins2 = default(PostProcessingMargins);
				RenderTreeCompositor.DrawOperation parent = op.parent;
				bool flag6 = parent != null && parent.type == RenderTreeCompositor.DrawOperationType.Effect;
				RectInt rectInt;
				if (flag6)
				{
					postProcessingMargins = RenderTreeCompositor.GetReadMargins(parent.FilterPass, parent.filter);
					postProcessingMargins2 = RenderTreeCompositor.GetWriteMargins(parent.FilterPass, parent.filter);
					Rect rect3 = UIRUtility.InflateByMargins(UIRUtility.InflateByMargins(value, postProcessingMargins), postProcessingMargins2);
					rectInt = UIRUtility.CastToRectInt(rect3);
					Rect rect4 = value;
					rect4 = UIRUtility.InflateByMargins(rect4, postProcessingMargins2);
					op.parent.drawSourceBounds = UIRUtility.CastToRectInt(rect4);
					op.parent.drawSourceTexOffsets = new Vector4(postProcessingMargins.left, postProcessingMargins.top, postProcessingMargins.right, postProcessingMargins.bottom);
				}
				else
				{
					rectInt = UIRUtility.CastToRectInt(value);
				}
				op.bounds = rectInt;
			}
			else
			{
				op.bounds = RectInt.zero;
			}
			bool flag7 = op.parent != null;
			if (flag7)
			{
				RenderTreeAtlas.AtlasBlock atlasBlock;
				bool flag8 = RenderTreeAtlas.ReserveSize(op.bounds.width, op.bounds.height, out atlasBlock);
				if (flag8)
				{
					op.dstAtlasBlock = atlasBlock;
					bool flag9 = op.parent.type == RenderTreeCompositor.DrawOperationType.RenderTree;
					if (flag9)
					{
						op.renderTree.quadRect = op.bounds;
						op.renderTree.quadUVRect = atlasBlock.uvRect;
					}
				}
			}
		}

		private void AssignTextureIds_DepthFirst(RenderTreeCompositor.DrawOperation op)
		{
			RenderTreeCompositor.DrawOperation parent = op.parent;
			bool flag = parent != null && parent.type == RenderTreeCompositor.DrawOperationType.RenderTree;
			if (flag)
			{
				Debug.Assert(!op.renderTree.quadTextureId.IsValid());
				TextureId textureId = this.m_RenderTreeManager.textureRegistry.AllocAndAcquireDynamic();
				op.dstTextureId = textureId;
				op.renderTree.quadTextureId = textureId;
				op.parent.renderTree.OnRenderDataVisualsChanged(op.visualElement.renderData, false);
			}
			else
			{
				Debug.Assert(!op.dstTextureId.IsValid());
			}
			for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				this.AssignTextureIds_DepthFirst(drawOperation);
			}
		}

		public void RenderNestedPasses()
		{
			this.ExecuteDrawOperation_PostOrder(this.m_RootOperation);
		}

		private void ExecuteDrawOperation_PostOrder(RenderTreeCompositor.DrawOperation op)
		{
			for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				this.ExecuteDrawOperation_PostOrder(drawOperation);
			}
			bool flag = op.parent == null;
			if (!flag)
			{
				RectInt bounds = op.bounds;
				bool flag2 = bounds.width <= 0;
				if (!flag2)
				{
					Debug.Assert(bounds.height > 0);
					bool forceGammaRendering = this.m_RenderTreeManager.forceGammaRendering;
					RenderTreeCompositor.DrawOperation parent = op.parent;
					bool flag3 = parent != null && parent.type == RenderTreeCompositor.DrawOperationType.RenderTree;
					bool flag5;
					bool flag4 = RenderTreeAtlas.CreateTextureForAtlasBlock(ref op.dstAtlasBlock, forceGammaRendering && !flag3, out flag5);
					if (flag4)
					{
						bool flag6 = flag5;
						if (flag6)
						{
							this.m_AllocatedTextures.Add(op.dstAtlasBlock.texture);
						}
						bool flag7 = op.dstTextureId.IsValid();
						if (flag7)
						{
							this.m_RenderTreeManager.textureRegistry.UpdateDynamic(op.dstTextureId, op.dstAtlasBlock.texture);
						}
						RenderTreeCompositor.DrawOperationType type = op.type;
						RenderTreeCompositor.DrawOperationType drawOperationType = type;
						if (drawOperationType != RenderTreeCompositor.DrawOperationType.RenderTree)
						{
							if (drawOperationType != RenderTreeCompositor.DrawOperationType.Effect)
							{
								throw new NotImplementedException();
							}
							try
							{
								Debug.Assert(op.firstChild != null, "An effect draw operation must have at least one child operation to render from.");
								RenderTexture active = RenderTexture.active;
								RenderTexture texture = op.dstAtlasBlock.texture;
								RenderTexture.active = texture;
								RectInt rect = op.dstAtlasBlock.rect;
								RenderTreeAtlas.AtlasBlock dstAtlasBlock = op.firstChild.dstAtlasBlock;
								Rect uvRect = dstAtlasBlock.uvRect;
								Material material = op.FilterPass.material;
								bool flag8 = forceGammaRendering && flag3;
								if (flag8)
								{
									material.EnableKeyword("_UIE_OUTPUT_LINEAR");
								}
								else
								{
									material.DisableKeyword("_UIE_OUTPUT_LINEAR");
								}
								material.SetPass(op.FilterPass.passIndex);
								this.m_Block.SetTexture("_MainTex", dstAtlasBlock.texture);
								RenderTreeCompositor.s_UVRects[0] = new Vector4(uvRect.x, uvRect.y, uvRect.width, uvRect.height);
								this.m_Block.SetVectorArray("unity_uie_UVRect", RenderTreeCompositor.s_UVRects);
								bool flag9 = QualitySettings.activeColorSpace == ColorSpace.Gamma || forceGammaRendering;
								bool flag10 = op.FilterPass.prepareMaterialPropertyBlockCallback != null || op.FilterPass.applySettingsCallback != null;
								if (flag10)
								{
									bool flag11 = op.FilterPass.prepareMaterialPropertyBlockCallback != null;
									if (flag11)
									{
										op.FilterPass.prepareMaterialPropertyBlockCallback(this.m_Block, op.filter);
									}
									bool flag12 = op.FilterPass.applySettingsCallback != null;
									if (flag12)
									{
										op.FilterPass.applySettingsCallback(this.m_Block, new FilterPassContext
										{
											filterFunction = op.filter,
											filterPassIndex = op.FilterPassIndex,
											readsGamma = flag9,
											writesGamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma || (forceGammaRendering && flag3))
										});
									}
								}
								else
								{
									this.ApplyEffectParameters(op.FilterPass, op.filter, op.visualElement, flag9);
								}
								Utility.SetPropertyBlock(this.m_Block);
								Matrix4x4 matrix4x = ProjectionUtils.Ortho((float)bounds.xMin, (float)bounds.xMax, (float)bounds.yMax, (float)bounds.yMin, 0f, 1f);
								GL.LoadProjectionMatrix(matrix4x);
								GL.modelview = Matrix4x4.identity;
								RectInt drawSourceBounds = op.drawSourceBounds;
								Vector4 drawSourceTexOffsets = op.drawSourceTexOffsets;
								float num = (float)dstAtlasBlock.texture.width;
								float num2 = (float)dstAtlasBlock.texture.height;
								Rect rect2 = new Rect(uvRect.x + drawSourceTexOffsets.x / num, uvRect.y + drawSourceTexOffsets.y / num2, uvRect.width - (drawSourceTexOffsets.x + drawSourceTexOffsets.z) / num, uvRect.height - (drawSourceTexOffsets.y + drawSourceTexOffsets.w) / num2);
								GL.Viewport(new Rect((float)rect.xMin, (float)rect.yMin, (float)rect.width, (float)rect.height));
								GL.Begin(7);
								GL.TexCoord2(rect2.xMin, rect2.yMin);
								GL.MultiTexCoord2(1, 0f, 0f);
								GL.Vertex3((float)drawSourceBounds.xMin, (float)drawSourceBounds.yMax, 0.5f);
								GL.TexCoord2(rect2.xMin, rect2.yMax);
								GL.MultiTexCoord2(1, 0f, 0f);
								GL.Vertex3((float)drawSourceBounds.xMin, (float)drawSourceBounds.yMin, 0.5f);
								GL.TexCoord2(rect2.xMax, rect2.yMax);
								GL.MultiTexCoord2(1, 0f, 0f);
								GL.Vertex3((float)drawSourceBounds.xMax, (float)drawSourceBounds.yMin, 0.5f);
								GL.TexCoord2(rect2.xMax, rect2.yMin);
								GL.MultiTexCoord2(1, 0f, 0f);
								GL.Vertex3((float)drawSourceBounds.xMax, (float)drawSourceBounds.yMax, 0.5f);
								GL.End();
								RenderTexture.active = active;
							}
							catch
							{
							}
						}
						else
						{
							this.m_RenderTreeManager.RenderSingleTree(op.renderTree, op.dstAtlasBlock.texture, op.dstAtlasBlock.rect, UIRUtility.CastToRect(bounds));
						}
					}
					else
					{
						Debug.LogError(string.Format("Failed to create a texture for draw operation with bounds {0}.", bounds));
					}
				}
			}
		}

		private unsafe void ApplyEffectParameters(PostProcessingPass effect, FilterFunction filter, VisualElement source, bool readsGamma)
		{
			bool flag = effect.parameterBindings == null;
			if (!flag)
			{
				FixedBuffer4<FilterParameter> parameters = filter.parameters;
				int parameterCount = filter.parameterCount;
				for (int i = 0; i < effect.parameterBindings.Length; i++)
				{
					bool flag2 = i >= parameterCount;
					if (flag2)
					{
						break;
					}
					ParameterBinding parameterBinding = effect.parameterBindings[i];
					FilterParameter filterParameter = *parameters[i];
					bool flag3 = filterParameter.type == FilterParameterType.Float;
					if (flag3)
					{
						this.m_Block.SetFloat(parameterBinding.name, filterParameter.floatValue);
					}
					else
					{
						bool flag4 = filterParameter.type == FilterParameterType.Color;
						if (flag4)
						{
							this.m_Block.SetVector(parameterBinding.name, readsGamma ? filterParameter.colorValue : filterParameter.colorValue.linear);
						}
					}
				}
			}
		}

		private void CleanupOperationTree()
		{
			bool flag = this.m_RootOperation != null;
			if (flag)
			{
				this.CleanupOperation_PostOrder(this.m_RootOperation);
				this.m_RootOperation = null;
			}
			foreach (RenderTexture renderTexture in this.m_AllocatedTextures)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			this.m_AllocatedTextures.Clear();
		}

		private void CleanupOperation_PostOrder(RenderTreeCompositor.DrawOperation op)
		{
			for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				this.CleanupOperation_PostOrder(drawOperation);
			}
			bool flag = op.dstTextureId.IsValid();
			if (flag)
			{
				this.m_RenderTreeManager.textureRegistry.Release(op.dstTextureId);
				op.dstTextureId = TextureId.invalid;
				op.renderTree.quadTextureId = TextureId.invalid;
			}
			op.Reset();
			this.m_DrawOperationPool.Release(op);
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
					this.CleanupOperationTree();
				}
				this.disposed = true;
			}
		}

		private readonly RenderTreeManager m_RenderTreeManager;

		private RenderTreeCompositor.DrawOperation m_RootOperation;

		private List<RenderTexture> m_AllocatedTextures = new List<RenderTexture>();

		private MaterialPropertyBlock m_Block = new MaterialPropertyBlock();

		private ObjectPool<RenderTreeCompositor.DrawOperation> m_DrawOperationPool = new ObjectPool<RenderTreeCompositor.DrawOperation>(() => new RenderTreeCompositor.DrawOperation(), 100);

		private static Vector4[] s_UVRects = new Vector4[1];

		private enum DrawOperationType
		{
			Undefined,
			RenderTree,
			Effect
		}

		private class DrawOperation
		{
			public RenderTreeCompositor.DrawOperationType type
			{
				get
				{
					return this.m_Type;
				}
			}

			public VisualElement visualElement
			{
				get
				{
					return this.m_VisualElement;
				}
			}

			public RenderTree renderTree
			{
				get
				{
					return this.m_RenderTree;
				}
			}

			public PostProcessingPass FilterPass
			{
				get
				{
					return this.m_FilterPass;
				}
			}

			public int FilterPassIndex
			{
				get
				{
					return this.m_FilterPassIndex;
				}
			}

			public FilterFunction filter
			{
				get
				{
					return this.m_Filter;
				}
			}

			public void Init(VisualElement ve, in PostProcessingPass filterPass, int filterPassIndex, FilterFunction filter)
			{
				this.m_Type = RenderTreeCompositor.DrawOperationType.Effect;
				this.m_VisualElement = ve;
				this.m_FilterPass = filterPass;
				this.m_FilterPassIndex = filterPassIndex;
				this.m_Filter = filter;
				this.m_RenderTree = ve.nestedRenderData.renderTree;
				this.InitPointers();
			}

			public void Init(RenderTree renderTree)
			{
				this.m_Type = RenderTreeCompositor.DrawOperationType.RenderTree;
				this.m_VisualElement = renderTree.rootRenderData.owner;
				this.m_RenderTree = renderTree;
				this.InitPointers();
			}

			private void InitPointers()
			{
				this.parent = null;
				this.firstChild = null;
				this.lastChild = null;
				this.prevSibling = null;
				this.nextSibling = null;
			}

			public void Reset()
			{
				this.m_Type = RenderTreeCompositor.DrawOperationType.Undefined;
				this.m_VisualElement = null;
				this.m_RenderTree = null;
				this.m_FilterPass = default(PostProcessingPass);
				this.m_Filter = default(FilterFunction);
				this.dstAtlasBlock = default(RenderTreeAtlas.AtlasBlock);
				this.dstTextureId = TextureId.invalid;
			}

			public void AddChild(RenderTreeCompositor.DrawOperation op)
			{
				Debug.Assert(op.prevSibling == null);
				op.parent = this;
				op.nextSibling = this.firstChild;
				bool flag = this.firstChild != null;
				if (flag)
				{
					this.firstChild.prevSibling = op;
				}
				this.firstChild = op;
			}

			private RenderTreeCompositor.DrawOperationType m_Type;

			private VisualElement m_VisualElement;

			private RenderTree m_RenderTree;

			private PostProcessingPass m_FilterPass;

			private int m_FilterPassIndex;

			private FilterFunction m_Filter;

			public RectInt bounds;

			public RectInt drawSourceBounds;

			public Vector4 drawSourceTexOffsets;

			public RenderTreeAtlas.AtlasBlock dstAtlasBlock;

			public TextureId dstTextureId;

			public RenderTreeCompositor.DrawOperation parent;

			public RenderTreeCompositor.DrawOperation firstChild;

			public RenderTreeCompositor.DrawOperation lastChild;

			public RenderTreeCompositor.DrawOperation prevSibling;

			public RenderTreeCompositor.DrawOperation nextSibling;
		}
	}
}
