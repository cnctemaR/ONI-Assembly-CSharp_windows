using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal abstract class BaseElementBuilder
	{
		public abstract bool RequiresStencilMask(VisualElement ve);

		public void Build(MeshGenerationContext mgc)
		{
			bool isSubTreeQuad = mgc.renderData.isSubTreeQuad;
			if (isSubTreeQuad)
			{
				this.BuildRenderTreeQuadElement(mgc);
			}
			else
			{
				this.BuildStandardElement(mgc);
			}
		}

		private void BuildRenderTreeQuadElement(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderTree renderTree = visualElement.nestedRenderData.renderTree;
			RectInt quadRect = renderTree.quadRect;
			Rect quadUVRect = renderTree.quadUVRect;
			bool flag = quadRect != RectInt.zero;
			if (flag)
			{
				Color white = Color.white;
				NativeSlice<Vertex> nativeSlice;
				NativeSlice<ushort> nativeSlice2;
				mgc.AllocateTempMesh(4, 6, out nativeSlice, out nativeSlice2);
				nativeSlice[0] = new Vertex
				{
					position = new Vector3((float)quadRect.xMin, (float)quadRect.yMax, Vertex.nearZ),
					tint = white,
					uv = new Vector2(quadUVRect.xMin, quadUVRect.yMin)
				};
				nativeSlice[1] = new Vertex
				{
					position = new Vector3((float)quadRect.xMin, (float)quadRect.yMin, Vertex.nearZ),
					tint = white,
					uv = new Vector2(quadUVRect.xMin, quadUVRect.yMax)
				};
				nativeSlice[2] = new Vertex
				{
					position = new Vector3((float)quadRect.xMax, (float)quadRect.yMin, Vertex.nearZ),
					tint = white,
					uv = new Vector2(quadUVRect.xMax, quadUVRect.yMax)
				};
				nativeSlice[3] = new Vertex
				{
					position = new Vector3((float)quadRect.xMax, (float)quadRect.yMax, Vertex.nearZ),
					tint = white,
					uv = new Vector2(quadUVRect.xMax, quadUVRect.yMin)
				};
				nativeSlice2[0] = 0;
				nativeSlice2[1] = 1;
				nativeSlice2[2] = 2;
				nativeSlice2[3] = 2;
				nativeSlice2[4] = 3;
				nativeSlice2[5] = 0;
				mgc.entryRecorder.DrawMesh(mgc.parentEntry, nativeSlice, nativeSlice2, renderTree.quadTextureId, true);
			}
			mgc.entryRecorder.DrawChildren(mgc.parentEntry);
		}

		private void BuildStandardElement(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderData renderData = mgc.renderData;
			Debug.Assert(visualElement.areAncestorsAndSelfDisplayed);
			bool isWorldSpaceRootUIDocument = visualElement.isWorldSpaceRootUIDocument;
			if (isWorldSpaceRootUIDocument)
			{
				mgc.entryRecorder.CutRenderChain(mgc.parentEntry);
			}
			bool isGroupTransform = renderData.isGroupTransform;
			bool flag = isGroupTransform;
			if (flag)
			{
				mgc.entryRecorder.PushGroupMatrix(mgc.parentEntry);
			}
			MaterialDefinition unityMaterial = visualElement.resolvedStyle.unityMaterial;
			bool flag2 = unityMaterial.material != null;
			bool flag3 = false;
			bool visible = visualElement.visible;
			if (visible)
			{
				bool flag4 = flag2;
				if (flag4)
				{
					mgc.entryRecorder.PushDefaultMaterial(mgc.parentEntry, unityMaterial);
				}
				this.DrawVisualElementBackground(mgc);
				this.DrawVisualElementBorder(mgc);
				this.PushVisualElementClipping(mgc);
				flag3 = true;
				BaseElementBuilder.InvokeGenerateVisualContent(mgc);
				bool flag5 = flag2;
				if (flag5)
				{
					mgc.entryRecorder.PopDefaultMaterial(mgc.parentEntry);
				}
			}
			else
			{
				bool flag6 = renderData.clipMethod == ClipMethod.Stencil;
				bool flag7 = renderData.clipMethod == ClipMethod.Scissor;
				bool flag8 = flag7 || flag6;
				if (flag8)
				{
					bool flag9 = flag2;
					if (flag9)
					{
						mgc.entryRecorder.PushDefaultMaterial(mgc.parentEntry, unityMaterial);
					}
					flag3 = true;
					this.PushVisualElementClipping(mgc);
					bool flag10 = flag2;
					if (flag10)
					{
						mgc.entryRecorder.PopDefaultMaterial(mgc.parentEntry);
					}
				}
			}
			mgc.entryRecorder.DrawChildren(mgc.parentEntry);
			bool flag11 = flag3;
			if (flag11)
			{
				bool flag12 = flag2;
				if (flag12)
				{
					mgc.entryRecorder.PushDefaultMaterial(mgc.parentEntry, unityMaterial);
				}
				BaseElementBuilder.PopVisualElementClipping(mgc);
				bool flag13 = flag2;
				if (flag13)
				{
					mgc.entryRecorder.PopDefaultMaterial(mgc.parentEntry);
				}
			}
			bool flag14 = isGroupTransform;
			if (flag14)
			{
				mgc.entryRecorder.PopGroupMatrix(mgc.parentEntry);
			}
		}

		protected abstract void DrawVisualElementBackground(MeshGenerationContext mgc);

		protected abstract void DrawVisualElementBorder(MeshGenerationContext mgc);

		protected abstract void DrawVisualElementStencilMask(MeshGenerationContext mgc);

		public abstract void ScheduleMeshGenerationJobs(MeshGenerationContext mgc);

		private void PushVisualElementClipping(MeshGenerationContext mgc)
		{
			RenderData renderData = mgc.renderData;
			bool flag = renderData.clipMethod == ClipMethod.Scissor;
			if (flag)
			{
				mgc.entryRecorder.PushScissors(mgc.parentEntry);
			}
			else
			{
				bool flag2 = renderData.clipMethod == ClipMethod.Stencil;
				if (flag2)
				{
					mgc.entryRecorder.BeginStencilMask(mgc.parentEntry);
					this.DrawVisualElementStencilMask(mgc);
					mgc.entryRecorder.EndStencilMask(mgc.parentEntry);
				}
			}
			mgc.entryRecorder.PushClippingRect(mgc.parentEntry);
		}

		private static void PopVisualElementClipping(MeshGenerationContext mgc)
		{
			RenderData renderData = mgc.renderData;
			mgc.entryRecorder.PopClippingRect(mgc.parentEntry);
			bool flag = renderData.clipMethod == ClipMethod.Scissor;
			if (flag)
			{
				mgc.entryRecorder.PopScissors(mgc.parentEntry);
			}
			else
			{
				bool flag2 = renderData.clipMethod == ClipMethod.Stencil;
				if (flag2)
				{
					mgc.entryRecorder.PopStencilMask(mgc.parentEntry);
				}
			}
		}

		private static void InvokeGenerateVisualContent(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			Painter2D.isPainterActive = true;
			visualElement.InvokeGenerateVisualContent(mgc);
			Painter2D.isPainterActive = false;
		}
	}
}
