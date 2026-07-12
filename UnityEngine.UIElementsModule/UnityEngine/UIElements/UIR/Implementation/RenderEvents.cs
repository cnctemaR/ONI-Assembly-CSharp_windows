using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR.Implementation
{
	internal static class RenderEvents
	{
		internal static void ProcessOnClippingChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.ClippingHierarchy) > RenderDataDirtyTypes.None;
			bool flag2 = flag;
			if (flag2)
			{
				stats.recursiveClipUpdates += 1U;
			}
			else
			{
				stats.nonRecursiveClipUpdates += 1U;
			}
			RenderEvents.DepthFirstOnClippingChanged(renderChain, ve.hierarchy.parent, ve, dirtyID, flag, true, false, false, false, renderChain.device, ref stats);
		}

		internal static void ProcessOnOpacityChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats)
		{
			stats.recursiveOpacityUpdates += 1U;
			RenderEvents.DepthFirstOnOpacityChanged(renderChain, (ve.hierarchy.parent != null) ? ve.hierarchy.parent.renderChainData.compositeOpacity : 1f, ve, dirtyID, ref stats, false);
		}

		internal static void ProcessOnTransformOrSizeChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats)
		{
			stats.recursiveTransformUpdates += 1U;
			RenderEvents.DepthFirstOnTransformOrSizeChanged(renderChain, ve.hierarchy.parent, ve, dirtyID, renderChain.device, false, false, ref stats);
		}

		internal static void ProcessOnVisualsChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) > RenderDataDirtyTypes.None;
			bool flag2 = flag;
			if (flag2)
			{
				stats.recursiveVisualUpdates += 1U;
			}
			else
			{
				stats.nonRecursiveVisualUpdates += 1U;
			}
			VisualElement parent = ve.hierarchy.parent;
			bool flag3 = parent != null && (parent.renderChainData.isHierarchyHidden || RenderEvents.IsElementHierarchyHidden(parent));
			RenderEvents.DepthFirstOnVisualsChanged(renderChain, ve, dirtyID, flag3, flag, ref stats);
		}

		internal static void ProcessRegenText(RenderChain renderChain, VisualElement ve, UIRTextUpdatePainter painter, UIRenderDevice device, ref ChainBuilderStats stats)
		{
			stats.textUpdates += 1U;
			painter.Begin(ve, device);
			ve.InvokeGenerateVisualContent(painter.meshGenerationContext);
			painter.End();
		}

		private static Matrix4x4 GetTransformIDTransformInfo(VisualElement ve)
		{
			Debug.Assert(RenderChainVEData.AllocatesID(ve.renderChainData.transformID) || (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None);
			bool flag = ve.renderChainData.groupTransformAncestor != null;
			Matrix4x4 matrix4x;
			if (flag)
			{
				matrix4x = ve.renderChainData.groupTransformAncestor.worldTransform.inverse * ve.worldTransform;
			}
			else
			{
				matrix4x = ve.worldTransform;
			}
			matrix4x.m22 = (matrix4x.m33 = 1f);
			return matrix4x;
		}

		private static Vector4 GetClipRectIDClipInfo(VisualElement ve)
		{
			Debug.Assert(RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID));
			bool flag = ve.renderChainData.groupTransformAncestor == null;
			Vector4 vector;
			if (flag)
			{
				vector = UIRUtility.ToVector4(ve.worldClip);
			}
			else
			{
				Rect worldClipMinusGroup = ve.worldClipMinusGroup;
				Matrix4x4 inverse = ve.renderChainData.groupTransformAncestor.worldTransform.inverse;
				Vector3 vector2 = inverse.MultiplyPoint3x4(new Vector3(worldClipMinusGroup.xMin, worldClipMinusGroup.yMin, 0f));
				Vector3 vector3 = inverse.MultiplyPoint3x4(new Vector3(worldClipMinusGroup.xMax, worldClipMinusGroup.yMax, 0f));
				vector = new Vector4(Mathf.Min(vector2.x, vector3.x), Mathf.Min(vector2.y, vector3.y), Mathf.Max(vector2.x, vector3.x), Mathf.Max(vector2.y, vector3.y));
			}
			return vector;
		}

		private static void GetVerticesTransformInfo(VisualElement ve, out Matrix4x4 transform)
		{
			bool flag = RenderChainVEData.AllocatesID(ve.renderChainData.transformID) || (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None;
			if (flag)
			{
				transform = Matrix4x4.identity;
			}
			else
			{
				bool flag2 = ve.renderChainData.boneTransformAncestor != null;
				if (flag2)
				{
					transform = ve.renderChainData.boneTransformAncestor.worldTransform.inverse * ve.worldTransform;
				}
				else
				{
					bool flag3 = ve.renderChainData.groupTransformAncestor != null;
					if (flag3)
					{
						transform = ve.renderChainData.groupTransformAncestor.worldTransform.inverse * ve.worldTransform;
					}
					else
					{
						transform = ve.worldTransform;
					}
				}
			}
			transform.m22 = (transform.m33 = 1f);
		}

		internal static uint DepthFirstOnChildAdded(RenderChain renderChain, VisualElement parent, VisualElement ve, int index, bool resetState)
		{
			Debug.Assert(ve.panel != null);
			bool isInChain = ve.renderChainData.isInChain;
			uint num;
			if (isInChain)
			{
				num = 0U;
			}
			else
			{
				if (resetState)
				{
					ve.renderChainData = default(RenderChainVEData);
				}
				ve.renderChainData.isInChain = true;
				ve.renderChainData.verticesSpace = Matrix4x4.identity;
				ve.renderChainData.transformID = UIRVEShaderInfoAllocator.identityTransform;
				ve.renderChainData.clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
				ve.renderChainData.opacityID = UIRVEShaderInfoAllocator.fullOpacity;
				ve.renderChainData.compositeOpacity = float.MaxValue;
				bool flag = parent != null;
				if (flag)
				{
					bool flag2 = (parent.renderHints & RenderHints.GroupTransform) > RenderHints.None;
					if (flag2)
					{
						ve.renderChainData.groupTransformAncestor = parent;
					}
					else
					{
						ve.renderChainData.groupTransformAncestor = parent.renderChainData.groupTransformAncestor;
					}
					ve.renderChainData.hierarchyDepth = parent.renderChainData.hierarchyDepth + 1;
				}
				else
				{
					ve.renderChainData.groupTransformAncestor = null;
					ve.renderChainData.hierarchyDepth = 0;
				}
				renderChain.EnsureFitsDepth(ve.renderChainData.hierarchyDepth);
				bool flag3 = index > 0;
				if (flag3)
				{
					Debug.Assert(parent != null);
					ve.renderChainData.prev = RenderEvents.GetLastDeepestChild(parent.hierarchy[index - 1]);
				}
				else
				{
					ve.renderChainData.prev = parent;
				}
				ve.renderChainData.next = ((ve.renderChainData.prev != null) ? ve.renderChainData.prev.renderChainData.next : null);
				bool flag4 = ve.renderChainData.prev != null;
				if (flag4)
				{
					ve.renderChainData.prev.renderChainData.next = ve;
				}
				bool flag5 = ve.renderChainData.next != null;
				if (flag5)
				{
					ve.renderChainData.next.renderChainData.prev = ve;
				}
				Debug.Assert(!RenderChainVEData.AllocatesID(ve.renderChainData.transformID));
				bool flag6 = RenderEvents.NeedsTransformID(ve);
				if (flag6)
				{
					ve.renderChainData.transformID = renderChain.shaderInfoAllocator.AllocTransform();
				}
				else
				{
					ve.renderChainData.transformID = BMPAlloc.Invalid;
				}
				ve.renderChainData.boneTransformAncestor = null;
				bool flag7 = !RenderChainVEData.AllocatesID(ve.renderChainData.transformID);
				if (flag7)
				{
					bool flag8 = parent != null && (ve.renderHints & RenderHints.GroupTransform) == RenderHints.None;
					if (flag8)
					{
						bool flag9 = RenderChainVEData.AllocatesID(parent.renderChainData.transformID);
						if (flag9)
						{
							ve.renderChainData.boneTransformAncestor = parent;
						}
						else
						{
							ve.renderChainData.boneTransformAncestor = parent.renderChainData.boneTransformAncestor;
						}
						ve.renderChainData.transformID = parent.renderChainData.transformID;
						ve.renderChainData.transformID.ownedState = OwnedState.Inherited;
					}
					else
					{
						ve.renderChainData.transformID = UIRVEShaderInfoAllocator.identityTransform;
					}
				}
				else
				{
					renderChain.shaderInfoAllocator.SetTransformValue(ve.renderChainData.transformID, RenderEvents.GetTransformIDTransformInfo(ve));
				}
				int childCount = ve.hierarchy.childCount;
				uint num2 = 0U;
				for (int i = 0; i < childCount; i++)
				{
					num2 += RenderEvents.DepthFirstOnChildAdded(renderChain, ve, ve.hierarchy[i], i, resetState);
				}
				num = 1U + num2;
			}
			return num;
		}

		internal static uint DepthFirstOnChildRemoving(RenderChain renderChain, VisualElement ve)
		{
			int i = ve.hierarchy.childCount - 1;
			uint num = 0U;
			while (i >= 0)
			{
				num += RenderEvents.DepthFirstOnChildRemoving(renderChain, ve.hierarchy[i--]);
			}
			bool flag = (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None;
			if (flag)
			{
				renderChain.StopTrackingGroupTransformElement(ve);
			}
			bool isInChain = ve.renderChainData.isInChain;
			if (isInChain)
			{
				renderChain.ChildWillBeRemoved(ve);
				RenderEvents.ResetCommands(renderChain, ve);
				ve.renderChainData.isInChain = false;
				ve.renderChainData.clipMethod = ClipMethod.Undetermined;
				bool flag2 = ve.renderChainData.next != null;
				if (flag2)
				{
					ve.renderChainData.next.renderChainData.prev = ve.renderChainData.prev;
				}
				bool flag3 = ve.renderChainData.prev != null;
				if (flag3)
				{
					ve.renderChainData.prev.renderChainData.next = ve.renderChainData.next;
				}
				bool flag4 = RenderChainVEData.AllocatesID(ve.renderChainData.opacityID);
				if (flag4)
				{
					renderChain.shaderInfoAllocator.FreeOpacity(ve.renderChainData.opacityID);
					ve.renderChainData.opacityID = UIRVEShaderInfoAllocator.fullOpacity;
				}
				bool flag5 = RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID);
				if (flag5)
				{
					renderChain.shaderInfoAllocator.FreeClipRect(ve.renderChainData.clipRectID);
					ve.renderChainData.clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
				}
				bool flag6 = RenderChainVEData.AllocatesID(ve.renderChainData.transformID);
				if (flag6)
				{
					renderChain.shaderInfoAllocator.FreeTransform(ve.renderChainData.transformID);
					ve.renderChainData.transformID = UIRVEShaderInfoAllocator.identityTransform;
				}
				ve.renderChainData.boneTransformAncestor = (ve.renderChainData.groupTransformAncestor = null);
				bool flag7 = ve.renderChainData.closingData != null;
				if (flag7)
				{
					renderChain.device.Free(ve.renderChainData.closingData);
					ve.renderChainData.closingData = null;
				}
				bool flag8 = ve.renderChainData.data != null;
				if (flag8)
				{
					renderChain.device.Free(ve.renderChainData.data);
					ve.renderChainData.data = null;
				}
			}
			return num + 1U;
		}

		private static void DepthFirstOnClippingChanged(RenderChain renderChain, VisualElement parent, VisualElement ve, uint dirtyID, bool hierarchical, bool isRootOfChange, bool isPendingHierarchicalRepaint, bool inheritedClipRectIDChanged, bool inheritedStencilClippedChanged, UIRenderDevice device, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			bool flag2 = flag && !inheritedClipRectIDChanged && !inheritedStencilClippedChanged;
			if (!flag2)
			{
				ve.renderChainData.dirtyID = dirtyID;
				bool flag3 = !isRootOfChange;
				if (flag3)
				{
					stats.recursiveClipUpdatesExpanded += 1U;
				}
				isPendingHierarchicalRepaint |= (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) > RenderDataDirtyTypes.None;
				bool flag4 = hierarchical || isRootOfChange || inheritedClipRectIDChanged;
				bool flag5 = hierarchical || isRootOfChange;
				bool flag6 = hierarchical || isRootOfChange || inheritedStencilClippedChanged;
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				bool flag10 = hierarchical;
				ClipMethod clipMethod = ve.renderChainData.clipMethod;
				ClipMethod clipMethod2 = (flag5 ? RenderEvents.DetermineSelfClipMethod(renderChain, ve) : clipMethod);
				bool flag11 = false;
				bool flag12 = flag4;
				if (flag12)
				{
					BMPAlloc bmpalloc = ve.renderChainData.clipRectID;
					bool flag13 = clipMethod2 == ClipMethod.ShaderDiscard;
					if (flag13)
					{
						bool flag14 = !RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID);
						if (flag14)
						{
							bmpalloc = renderChain.shaderInfoAllocator.AllocClipRect();
							bool flag15 = !bmpalloc.IsValid();
							if (flag15)
							{
								clipMethod2 = ClipMethod.Scissor;
								bmpalloc = UIRVEShaderInfoAllocator.infiniteClipRect;
							}
						}
					}
					else
					{
						bool flag16 = RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID);
						if (flag16)
						{
							renderChain.shaderInfoAllocator.FreeClipRect(ve.renderChainData.clipRectID);
						}
						bool flag17 = (ve.renderHints & RenderHints.GroupTransform) == RenderHints.None;
						if (flag17)
						{
							bmpalloc = ((clipMethod2 != ClipMethod.Scissor && parent != null) ? parent.renderChainData.clipRectID : UIRVEShaderInfoAllocator.infiniteClipRect);
							bmpalloc.ownedState = OwnedState.Inherited;
						}
					}
					flag11 = !ve.renderChainData.clipRectID.Equals(bmpalloc);
					Debug.Assert((ve.renderHints & RenderHints.GroupTransform) == RenderHints.None || !flag11);
					ve.renderChainData.clipRectID = bmpalloc;
				}
				bool flag18 = clipMethod != clipMethod2;
				if (flag18)
				{
					ve.renderChainData.clipMethod = clipMethod2;
					bool flag19 = clipMethod == ClipMethod.Stencil || clipMethod2 == ClipMethod.Stencil;
					if (flag19)
					{
						flag6 = true;
						flag8 = true;
					}
					bool flag20 = clipMethod == ClipMethod.Scissor || clipMethod2 == ClipMethod.Scissor;
					if (flag20)
					{
						flag7 = true;
					}
					bool flag21 = clipMethod2 == ClipMethod.ShaderDiscard || (clipMethod == ClipMethod.ShaderDiscard && RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID));
					if (flag21)
					{
						flag9 = true;
					}
				}
				bool flag22 = flag11;
				if (flag22)
				{
					flag10 = true;
					flag8 = true;
				}
				bool flag23 = false;
				bool flag24 = flag6;
				if (flag24)
				{
					bool isStencilClipped = ve.renderChainData.isStencilClipped;
					bool flag25 = clipMethod2 == ClipMethod.Stencil || (parent != null && parent.renderChainData.isStencilClipped);
					ve.renderChainData.isStencilClipped = flag25;
					bool flag26 = isStencilClipped != flag25;
					if (flag26)
					{
						flag23 = true;
						flag10 = true;
					}
				}
				bool flag27 = (flag7 || flag8) && !isPendingHierarchicalRepaint;
				if (flag27)
				{
					renderChain.UIEOnVisualsChanged(ve, flag8);
					isPendingHierarchicalRepaint = true;
				}
				bool flag28 = flag9;
				if (flag28)
				{
					renderChain.UIEOnTransformOrSizeChanged(ve, false, true);
				}
				bool flag29 = flag10;
				if (flag29)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnClippingChanged(renderChain, ve, ve.hierarchy[i], dirtyID, hierarchical, false, isPendingHierarchicalRepaint, flag11, flag23, device, ref stats);
					}
				}
			}
		}

		private static void DepthFirstOnOpacityChanged(RenderChain renderChain, float parentCompositeOpacity, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats, bool isDoingFullVertexRegeneration = false)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			if (!flag)
			{
				ve.renderChainData.dirtyID = dirtyID;
				stats.recursiveOpacityUpdatesExpanded += 1U;
				float compositeOpacity = ve.renderChainData.compositeOpacity;
				float num = ve.resolvedStyle.opacity * parentCompositeOpacity;
				bool flag2 = (compositeOpacity < RenderEvents.VisibilityTreshold) ^ (num < RenderEvents.VisibilityTreshold);
				bool flag3 = compositeOpacity < RenderEvents.VisibilityTreshold && num >= RenderEvents.VisibilityTreshold;
				bool flag4 = Mathf.Abs(compositeOpacity - num) > 0.0001f || flag2;
				bool flag5 = flag4;
				if (flag5)
				{
					ve.renderChainData.compositeOpacity = num;
				}
				bool flag6 = false;
				bool flag7 = num < parentCompositeOpacity - 0.0001f;
				bool flag8 = flag7;
				if (flag8)
				{
					bool flag9 = ve.renderChainData.opacityID.ownedState == OwnedState.Inherited;
					if (flag9)
					{
						flag6 = true;
						ve.renderChainData.opacityID = renderChain.shaderInfoAllocator.AllocOpacity();
					}
					bool flag10 = (flag6 || flag4) && ve.renderChainData.opacityID.IsValid();
					if (flag10)
					{
						renderChain.shaderInfoAllocator.SetOpacityValue(ve.renderChainData.opacityID, num);
					}
				}
				else
				{
					bool flag11 = ve.renderChainData.opacityID.ownedState == OwnedState.Inherited;
					if (flag11)
					{
						bool flag12 = ve.hierarchy.parent != null && !ve.renderChainData.opacityID.Equals(ve.hierarchy.parent.renderChainData.opacityID);
						if (flag12)
						{
							flag6 = true;
							ve.renderChainData.opacityID = ve.hierarchy.parent.renderChainData.opacityID;
							ve.renderChainData.opacityID.ownedState = OwnedState.Inherited;
						}
					}
					else
					{
						bool flag13 = flag4 && ve.renderChainData.opacityID.IsValid();
						if (flag13)
						{
							renderChain.shaderInfoAllocator.SetOpacityValue(ve.renderChainData.opacityID, num);
						}
					}
				}
				bool flag14 = isDoingFullVertexRegeneration;
				if (!flag14)
				{
					bool flag15 = flag3;
					if (flag15)
					{
						renderChain.UIEOnVisualsChanged(ve, true);
						isDoingFullVertexRegeneration = true;
					}
					else
					{
						bool flag16 = flag6 && (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.Visuals) == RenderDataDirtyTypes.None;
						if (flag16)
						{
							renderChain.UIEOnVisualsChanged(ve, false);
						}
					}
				}
				bool flag17 = flag4 || flag6;
				if (flag17)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnOpacityChanged(renderChain, num, ve.hierarchy[i], dirtyID, ref stats, isDoingFullVertexRegeneration);
					}
				}
			}
		}

		private static void DepthFirstOnTransformOrSizeChanged(RenderChain renderChain, VisualElement parent, VisualElement ve, uint dirtyID, UIRenderDevice device, bool isAncestorOfChangeSkinned, bool transformChanged, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			if (!flag)
			{
				stats.recursiveTransformUpdatesExpanded += 1U;
				transformChanged |= (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.Transform) > RenderDataDirtyTypes.None;
				bool flag2 = RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID);
				if (flag2)
				{
					renderChain.shaderInfoAllocator.SetClipRectValue(ve.renderChainData.clipRectID, RenderEvents.GetClipRectIDClipInfo(ve));
				}
				bool flag3 = true;
				bool flag4 = RenderChainVEData.AllocatesID(ve.renderChainData.transformID);
				if (flag4)
				{
					renderChain.shaderInfoAllocator.SetTransformValue(ve.renderChainData.transformID, RenderEvents.GetTransformIDTransformInfo(ve));
					isAncestorOfChangeSkinned = true;
					stats.boneTransformed += 1U;
				}
				else
				{
					bool flag5 = !transformChanged;
					if (!flag5)
					{
						bool flag6 = (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None;
						if (flag6)
						{
							stats.groupTransformElementsChanged += 1U;
						}
						else
						{
							bool flag7 = isAncestorOfChangeSkinned;
							if (flag7)
							{
								Debug.Assert(RenderChainVEData.InheritsID(ve.renderChainData.transformID));
								flag3 = false;
								stats.skipTransformed += 1U;
							}
							else
							{
								bool flag8 = (ve.renderChainData.dirtiedValues & (RenderDataDirtyTypes.Visuals | RenderDataDirtyTypes.VisualsHierarchy)) == RenderDataDirtyTypes.None && ve.renderChainData.data != null;
								if (flag8)
								{
									bool flag9 = !ve.renderChainData.disableNudging && RenderEvents.NudgeVerticesToNewSpace(ve, device);
									if (flag9)
									{
										stats.nudgeTransformed += 1U;
									}
									else
									{
										renderChain.UIEOnVisualsChanged(ve, false);
										stats.visualUpdateTransformed += 1U;
									}
								}
							}
						}
					}
				}
				bool flag10 = flag3;
				if (flag10)
				{
					ve.renderChainData.dirtyID = dirtyID;
				}
				bool drawInCameras = renderChain.drawInCameras;
				if (drawInCameras)
				{
					ve.EnsureWorldTransformAndClipUpToDate();
				}
				bool flag11 = (ve.renderHints & RenderHints.GroupTransform) == RenderHints.None;
				if (flag11)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnTransformOrSizeChanged(renderChain, ve, ve.hierarchy[i], dirtyID, device, isAncestorOfChangeSkinned, transformChanged, ref stats);
					}
				}
				else
				{
					renderChain.OnGroupTransformElementChangedTransform(ve);
				}
			}
		}

		private static void DepthFirstOnVisualsChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, bool parentHierarchyHidden, bool hierarchical, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			if (!flag)
			{
				ve.renderChainData.dirtyID = dirtyID;
				bool flag2 = hierarchical;
				if (flag2)
				{
					stats.recursiveVisualUpdatesExpanded += 1U;
				}
				bool isHierarchyHidden = ve.renderChainData.isHierarchyHidden;
				ve.renderChainData.isHierarchyHidden = parentHierarchyHidden || RenderEvents.IsElementHierarchyHidden(ve);
				bool flag3 = isHierarchyHidden != ve.renderChainData.isHierarchyHidden;
				if (flag3)
				{
					hierarchical = true;
				}
				Debug.Assert(ve.renderChainData.clipMethod > ClipMethod.Undetermined);
				Debug.Assert(RenderChainVEData.AllocatesID(ve.renderChainData.transformID) || ve.hierarchy.parent == null || ve.renderChainData.transformID.Equals(ve.hierarchy.parent.renderChainData.transformID) || (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None);
				UIRStylePainter.ClosingInfo closingInfo = RenderEvents.PaintElement(renderChain, ve, ref stats);
				bool flag4 = hierarchical;
				if (flag4)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnVisualsChanged(renderChain, ve.hierarchy[i], dirtyID, ve.renderChainData.isHierarchyHidden, true, ref stats);
					}
				}
				bool needsClosing = closingInfo.needsClosing;
				if (needsClosing)
				{
					RenderEvents.ClosePaintElement(ve, closingInfo, renderChain, ref stats);
				}
			}
		}

		private static bool IsElementHierarchyHidden(VisualElement ve)
		{
			return ve.resolvedStyle.opacity < RenderEvents.VisibilityTreshold || ve.resolvedStyle.display == DisplayStyle.None;
		}

		private static bool IsElementSelfHidden(VisualElement ve)
		{
			return ve.resolvedStyle.visibility == Visibility.Hidden;
		}

		private static VisualElement GetLastDeepestChild(VisualElement ve)
		{
			for (int i = ve.hierarchy.childCount; i > 0; i = ve.hierarchy.childCount)
			{
				ve = ve.hierarchy[i - 1];
			}
			return ve;
		}

		private static VisualElement GetNextDepthFirst(VisualElement ve)
		{
			for (VisualElement visualElement = ve.hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				int num = visualElement.hierarchy.IndexOf(ve);
				int childCount = visualElement.hierarchy.childCount;
				bool flag = num < childCount - 1;
				if (flag)
				{
					return visualElement.hierarchy[num + 1];
				}
				ve = visualElement;
			}
			return null;
		}

		private static bool IsParentOrAncestorOf(this VisualElement ve, VisualElement child)
		{
			while (child.hierarchy.parent != null)
			{
				bool flag = child.hierarchy.parent == ve;
				if (flag)
				{
					return true;
				}
				child = child.hierarchy.parent;
			}
			return false;
		}

		private static ClipMethod DetermineSelfClipMethod(RenderChain renderChain, VisualElement ve)
		{
			bool flag = !ve.ShouldClip();
			ClipMethod clipMethod;
			if (flag)
			{
				clipMethod = ClipMethod.NotClipped;
			}
			else
			{
				bool flag2 = !UIRUtility.IsRoundRect(ve) && !UIRUtility.IsVectorImageBackground(ve);
				if (flag2)
				{
					bool flag3 = (ve.renderHints & (RenderHints.GroupTransform | RenderHints.ClipWithScissors)) > RenderHints.None;
					if (flag3)
					{
						clipMethod = ClipMethod.Scissor;
					}
					else
					{
						clipMethod = ClipMethod.ShaderDiscard;
					}
				}
				else
				{
					VisualElement parent = ve.hierarchy.parent;
					bool flag4 = parent != null && parent.renderChainData.isStencilClipped;
					if (flag4)
					{
						clipMethod = ClipMethod.ShaderDiscard;
					}
					else
					{
						clipMethod = (renderChain.drawInCameras ? ClipMethod.ShaderDiscard : ClipMethod.Stencil);
					}
				}
			}
			return clipMethod;
		}

		private static bool NeedsTransformID(VisualElement ve)
		{
			return (ve.renderHints & RenderHints.GroupTransform) == RenderHints.None && (ve.renderHints & RenderHints.BoneTransform) == RenderHints.BoneTransform;
		}

		private static bool TransformIDHasChanged(Alloc before, Alloc after)
		{
			bool flag = before.size == 0U && after.size == 0U;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = before.size != after.size || before.start != after.start;
				flag2 = flag3;
			}
			return flag2;
		}

		internal static UIRStylePainter.ClosingInfo PaintElement(RenderChain renderChain, VisualElement ve, ref ChainBuilderStats stats)
		{
			bool flag = ve.renderChainData.clipMethod == ClipMethod.Stencil;
			bool flag2 = (RenderEvents.IsElementSelfHidden(ve) && !flag) || ve.renderChainData.isHierarchyHidden;
			UIRStylePainter.ClosingInfo closingInfo2;
			if (flag2)
			{
				bool flag3 = ve.renderChainData.data != null;
				if (flag3)
				{
					renderChain.painter.device.Free(ve.renderChainData.data);
					ve.renderChainData.data = null;
				}
				bool flag4 = ve.renderChainData.firstCommand != null;
				if (flag4)
				{
					RenderEvents.ResetCommands(renderChain, ve);
				}
				UIRStylePainter.ClosingInfo closingInfo = default(UIRStylePainter.ClosingInfo);
				closingInfo2 = closingInfo;
			}
			else
			{
				RenderChainCommand firstCommand = ve.renderChainData.firstCommand;
				RenderChainCommand renderChainCommand = ((firstCommand != null) ? firstCommand.prev : null);
				RenderChainCommand lastCommand = ve.renderChainData.lastCommand;
				RenderChainCommand renderChainCommand2 = ((lastCommand != null) ? lastCommand.next : null);
				bool flag5 = ve.renderChainData.firstClosingCommand != null && renderChainCommand2 == ve.renderChainData.firstClosingCommand;
				bool flag6 = flag5;
				RenderChainCommand renderChainCommand4;
				RenderChainCommand renderChainCommand3;
				if (flag6)
				{
					renderChainCommand2 = ve.renderChainData.lastClosingCommand.next;
					renderChainCommand3 = (renderChainCommand4 = null);
				}
				else
				{
					RenderChainCommand firstClosingCommand = ve.renderChainData.firstClosingCommand;
					renderChainCommand4 = ((firstClosingCommand != null) ? firstClosingCommand.prev : null);
					RenderChainCommand lastClosingCommand = ve.renderChainData.lastClosingCommand;
					renderChainCommand3 = ((lastClosingCommand != null) ? lastClosingCommand.next : null);
				}
				Debug.Assert(((renderChainCommand != null) ? renderChainCommand.owner : null) != ve);
				Debug.Assert(((renderChainCommand2 != null) ? renderChainCommand2.owner : null) != ve);
				Debug.Assert(((renderChainCommand4 != null) ? renderChainCommand4.owner : null) != ve);
				Debug.Assert(((renderChainCommand3 != null) ? renderChainCommand3.owner : null) != ve);
				RenderEvents.ResetCommands(renderChain, ve);
				UIRStylePainter painter = renderChain.painter;
				painter.Begin(ve);
				bool visible = ve.visible;
				if (visible)
				{
					painter.DrawVisualElementBackground();
					painter.DrawVisualElementBorder();
					painter.ApplyVisualElementClipping();
					ve.InvokeGenerateVisualContent(painter.meshGenerationContext);
				}
				else
				{
					bool flag7 = ve.renderChainData.clipMethod == ClipMethod.Stencil;
					if (flag7)
					{
						painter.ApplyVisualElementClipping();
					}
				}
				MeshHandle meshHandle = ve.renderChainData.data;
				bool flag8 = (long)painter.totalVertices > (long)((ulong)renderChain.device.maxVerticesPerPage);
				if (flag8)
				{
					Debug.LogError(string.Format("A {0} must not allocate more than {1} vertices.", "VisualElement", renderChain.device.maxVerticesPerPage));
					bool flag9 = meshHandle != null;
					if (flag9)
					{
						painter.device.Free(meshHandle);
						meshHandle = null;
					}
					painter.Reset();
					painter.Begin(ve);
				}
				List<UIRStylePainter.Entry> entries = painter.entries;
				bool flag10 = entries.Count > 0;
				if (flag10)
				{
					NativeSlice<Vertex> nativeSlice = default(NativeSlice<Vertex>);
					NativeSlice<ushort> nativeSlice2 = default(NativeSlice<ushort>);
					ushort num = 0;
					bool flag11 = painter.totalVertices > 0;
					if (flag11)
					{
						RenderEvents.UpdateOrAllocate(ref meshHandle, painter.totalVertices, painter.totalIndices, painter.device, out nativeSlice, out nativeSlice2, out num, ref stats);
					}
					int num2 = 0;
					int num3 = 0;
					RenderChainCommand renderChainCommand5 = renderChainCommand;
					RenderChainCommand renderChainCommand6 = renderChainCommand2;
					bool flag12 = renderChainCommand == null && renderChainCommand2 == null;
					if (flag12)
					{
						RenderEvents.FindCommandInsertionPoint(ve, out renderChainCommand5, out renderChainCommand6);
					}
					bool flag13 = false;
					Matrix4x4 identity = Matrix4x4.identity;
					Color32 color = new Color32(0, 0, 0, 0);
					Color32 color2 = new Color32(0, 0, 0, 0);
					Color32 color3 = new Color32(0, 0, 0, 0);
					int num4 = -1;
					int num5 = -1;
					foreach (UIRStylePainter.Entry entry in painter.entries)
					{
						NativeSlice<Vertex> nativeSlice3 = entry.vertices;
						bool flag14;
						if (nativeSlice3.Length > 0)
						{
							NativeSlice<ushort> nativeSlice4 = entry.indices;
							flag14 = nativeSlice4.Length > 0;
						}
						else
						{
							flag14 = false;
						}
						bool flag15 = flag14;
						if (flag15)
						{
							bool flag16 = !flag13;
							if (flag16)
							{
								flag13 = true;
								RenderEvents.GetVerticesTransformInfo(ve, out identity);
								ve.renderChainData.verticesSpace = identity;
								Color32 color4 = renderChain.shaderInfoAllocator.TransformAllocToVertexData(ve.renderChainData.transformID);
								Color32 color5 = renderChain.shaderInfoAllocator.OpacityAllocToVertexData(ve.renderChainData.opacityID);
								color.r = color4.r;
								color.g = color4.g;
								color2.r = color4.b;
								color3.r = color5.r;
								color3.g = color5.g;
								color2.b = color5.b;
							}
							Color32 color6 = renderChain.shaderInfoAllocator.ClipRectAllocToVertexData(entry.clipRectID);
							color.b = color6.r;
							color.a = color6.g;
							color2.g = color6.b;
							color2.a = (byte)entry.addFlags;
							NativeSlice<Vertex> nativeSlice5 = nativeSlice;
							int num6 = num2;
							nativeSlice3 = entry.vertices;
							NativeSlice<Vertex> nativeSlice6 = nativeSlice5.Slice<Vertex>(num6, nativeSlice3.Length);
							bool uvIsDisplacement = entry.uvIsDisplacement;
							if (uvIsDisplacement)
							{
								bool flag17 = num4 < 0;
								if (flag17)
								{
									num4 = num2;
									int num7 = num2;
									nativeSlice3 = entry.vertices;
									num5 = num7 + nativeSlice3.Length;
								}
								else
								{
									bool flag18 = num5 == num2;
									if (flag18)
									{
										int num8 = num5;
										nativeSlice3 = entry.vertices;
										num5 = num8 + nativeSlice3.Length;
									}
									else
									{
										ve.renderChainData.disableNudging = true;
									}
								}
								RenderEvents.CopyTransformVertsPosAndVec(entry.vertices, nativeSlice6, identity, color, color2, color3);
							}
							else
							{
								RenderEvents.CopyTransformVertsPos(entry.vertices, nativeSlice6, identity, color, color2, color3);
							}
							NativeSlice<ushort> nativeSlice4 = entry.indices;
							int length = nativeSlice4.Length;
							int num9 = num2 + (int)num;
							NativeSlice<ushort> nativeSlice7 = nativeSlice2.Slice<ushort>(num3, length);
							bool flag19 = entry.isClipRegisterEntry || !entry.isStencilClipped;
							if (flag19)
							{
								RenderEvents.CopyTriangleIndices(entry.indices, nativeSlice7, num9);
							}
							else
							{
								RenderEvents.CopyTriangleIndicesFlipWindingOrder(entry.indices, nativeSlice7, num9);
							}
							bool isClipRegisterEntry = entry.isClipRegisterEntry;
							if (isClipRegisterEntry)
							{
								painter.LandClipRegisterMesh(nativeSlice6, nativeSlice7, num9);
							}
							RenderChainCommand renderChainCommand7 = RenderEvents.InjectMeshDrawCommand(renderChain, ve, ref renderChainCommand5, ref renderChainCommand6, meshHandle, length, num3, entry.material, entry.custom, entry.font);
							bool flag20 = entry.isTextEntry && ve.renderChainData.usesLegacyText;
							if (flag20)
							{
								bool flag21 = ve.renderChainData.textEntries == null;
								if (flag21)
								{
									ve.renderChainData.textEntries = new List<RenderChainTextEntry>(1);
								}
								List<RenderChainTextEntry> textEntries = ve.renderChainData.textEntries;
								RenderChainTextEntry renderChainTextEntry = default(RenderChainTextEntry);
								renderChainTextEntry.command = renderChainCommand7;
								renderChainTextEntry.firstVertex = num2;
								nativeSlice3 = entry.vertices;
								renderChainTextEntry.vertexCount = nativeSlice3.Length;
								textEntries.Add(renderChainTextEntry);
							}
							int num10 = num2;
							nativeSlice3 = entry.vertices;
							num2 = num10 + nativeSlice3.Length;
							num3 += length;
						}
						else
						{
							bool flag22 = entry.customCommand != null;
							if (flag22)
							{
								RenderEvents.InjectCommandInBetween(renderChain, entry.customCommand, ref renderChainCommand5, ref renderChainCommand6);
							}
							else
							{
								Debug.Assert(false);
							}
						}
					}
					bool flag23 = !ve.renderChainData.disableNudging && num4 >= 0;
					if (flag23)
					{
						ve.renderChainData.displacementUVStart = num4;
						ve.renderChainData.displacementUVEnd = num5;
					}
				}
				else
				{
					bool flag24 = meshHandle != null;
					if (flag24)
					{
						painter.device.Free(meshHandle);
						meshHandle = null;
					}
				}
				ve.renderChainData.data = meshHandle;
				bool usesLegacyText = ve.renderChainData.usesLegacyText;
				if (usesLegacyText)
				{
					renderChain.AddTextElement(ve);
				}
				UIRStylePainter.ClosingInfo closingInfo = painter.closingInfo;
				bool flag25 = closingInfo.clipperRegisterIndices.Length == 0 && ve.renderChainData.closingData != null;
				if (flag25)
				{
					painter.device.Free(ve.renderChainData.closingData);
					ve.renderChainData.closingData = null;
				}
				bool needsClosing = painter.closingInfo.needsClosing;
				if (needsClosing)
				{
					RenderChainCommand renderChainCommand8 = renderChainCommand4;
					RenderChainCommand renderChainCommand9 = renderChainCommand3;
					bool flag26 = flag5;
					if (flag26)
					{
						renderChainCommand8 = ve.renderChainData.lastCommand;
						renderChainCommand9 = renderChainCommand8.next;
					}
					else
					{
						bool flag27 = renderChainCommand8 == null && renderChainCommand9 == null;
						if (flag27)
						{
							RenderEvents.FindClosingCommandInsertionPoint(ve, out renderChainCommand8, out renderChainCommand9);
						}
					}
					closingInfo = painter.closingInfo;
					bool flag28 = closingInfo.clipperRegisterIndices.Length > 0;
					if (flag28)
					{
						painter.LandClipUnregisterMeshDrawCommand(RenderEvents.InjectClosingMeshDrawCommand(renderChain, ve, ref renderChainCommand8, ref renderChainCommand9, null, 0, 0, null, null, null));
					}
					bool popViewMatrix = painter.closingInfo.popViewMatrix;
					if (popViewMatrix)
					{
						RenderChainCommand renderChainCommand10 = renderChain.AllocCommand();
						renderChainCommand10.type = CommandType.PopView;
						renderChainCommand10.closing = true;
						renderChainCommand10.owner = ve;
						RenderEvents.InjectClosingCommandInBetween(renderChain, renderChainCommand10, ref renderChainCommand8, ref renderChainCommand9);
					}
					bool popScissorClip = painter.closingInfo.popScissorClip;
					if (popScissorClip)
					{
						RenderChainCommand renderChainCommand11 = renderChain.AllocCommand();
						renderChainCommand11.type = CommandType.PopScissor;
						renderChainCommand11.closing = true;
						renderChainCommand11.owner = ve;
						RenderEvents.InjectClosingCommandInBetween(renderChain, renderChainCommand11, ref renderChainCommand8, ref renderChainCommand9);
					}
				}
				UIRStylePainter.ClosingInfo closingInfo3 = painter.closingInfo;
				painter.Reset();
				closingInfo2 = closingInfo3;
			}
			return closingInfo2;
		}

		private static void ClosePaintElement(VisualElement ve, UIRStylePainter.ClosingInfo closingInfo, RenderChain renderChain, ref ChainBuilderStats stats)
		{
			bool flag = closingInfo.clipperRegisterIndices.Length > 0;
			if (flag)
			{
				NativeSlice<Vertex> nativeSlice = default(NativeSlice<Vertex>);
				NativeSlice<ushort> nativeSlice2 = default(NativeSlice<ushort>);
				ushort num = 0;
				RenderEvents.UpdateOrAllocate(ref ve.renderChainData.closingData, closingInfo.clipperRegisterVertices.Length, closingInfo.clipperRegisterIndices.Length, renderChain.painter.device, out nativeSlice, out nativeSlice2, out num, ref stats);
				nativeSlice.CopyFrom(closingInfo.clipperRegisterVertices);
				RenderEvents.CopyTriangleIndicesFlipWindingOrder(closingInfo.clipperRegisterIndices, nativeSlice2, (int)num - closingInfo.clipperRegisterIndexOffset);
				closingInfo.clipUnregisterDrawCommand.mesh = ve.renderChainData.closingData;
				closingInfo.clipUnregisterDrawCommand.indexCount = nativeSlice2.Length;
			}
		}

		private static void UpdateOrAllocate(ref MeshHandle data, int vertexCount, int indexCount, UIRenderDevice device, out NativeSlice<Vertex> verts, out NativeSlice<ushort> indices, out ushort indexOffset, ref ChainBuilderStats stats)
		{
			bool flag = data != null;
			if (flag)
			{
				bool flag2 = (ulong)data.allocVerts.size >= (ulong)((long)vertexCount) && (ulong)data.allocIndices.size >= (ulong)((long)indexCount);
				if (flag2)
				{
					device.Update(data, (uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
					stats.updatedMeshAllocations += 1U;
				}
				else
				{
					device.Free(data);
					data = device.Allocate((uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
					stats.newMeshAllocations += 1U;
				}
			}
			else
			{
				data = device.Allocate((uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
				stats.newMeshAllocations += 1U;
			}
		}

		private static void CopyTransformVertsPos(NativeSlice<Vertex> source, NativeSlice<Vertex> target, Matrix4x4 mat, Color32 xformClipPages, Color32 idsAddFlags, Color32 opacityPage)
		{
			int length = source.Length;
			for (int i = 0; i < length; i++)
			{
				Vertex vertex = source[i];
				vertex.position = mat.MultiplyPoint3x4(vertex.position);
				vertex.xformClipPages = xformClipPages;
				vertex.idsFlags.r = idsAddFlags.r;
				vertex.idsFlags.g = idsAddFlags.g;
				vertex.idsFlags.b = idsAddFlags.b;
				vertex.idsFlags.a = vertex.idsFlags.a + idsAddFlags.a;
				vertex.opacityPageSVGSettingIndex.r = opacityPage.r;
				vertex.opacityPageSVGSettingIndex.g = opacityPage.g;
				target[i] = vertex;
			}
		}

		private static void CopyTransformVertsPosAndVec(NativeSlice<Vertex> source, NativeSlice<Vertex> target, Matrix4x4 mat, Color32 xformClipPages, Color32 idsAddFlags, Color32 opacityPage)
		{
			int length = source.Length;
			Vector3 vector = new Vector3(0f, 0f, 0f);
			for (int i = 0; i < length; i++)
			{
				Vertex vertex = source[i];
				vertex.position = mat.MultiplyPoint3x4(vertex.position);
				vector.x = vertex.uv.x;
				vector.y = vertex.uv.y;
				vertex.uv = mat.MultiplyVector(vector);
				vertex.xformClipPages = xformClipPages;
				vertex.idsFlags.r = idsAddFlags.r;
				vertex.idsFlags.g = idsAddFlags.g;
				vertex.idsFlags.b = idsAddFlags.b;
				vertex.idsFlags.a = vertex.idsFlags.a + idsAddFlags.a;
				vertex.opacityPageSVGSettingIndex.r = opacityPage.r;
				vertex.opacityPageSVGSettingIndex.g = opacityPage.g;
				target[i] = vertex;
			}
		}

		private static void CopyTriangleIndicesFlipWindingOrder(NativeSlice<ushort> source, NativeSlice<ushort> target)
		{
			Debug.Assert(source != target);
			int length = source.Length;
			for (int i = 0; i < length; i += 3)
			{
				ushort num = source[i];
				target[i] = source[i + 1];
				target[i + 1] = num;
				target[i + 2] = source[i + 2];
			}
		}

		private static void CopyTriangleIndicesFlipWindingOrder(NativeSlice<ushort> source, NativeSlice<ushort> target, int indexOffset)
		{
			Debug.Assert(source != target);
			int length = source.Length;
			for (int i = 0; i < length; i += 3)
			{
				ushort num = (ushort)((int)source[i] + indexOffset);
				target[i] = (ushort)((int)source[i + 1] + indexOffset);
				target[i + 1] = num;
				target[i + 2] = (ushort)((int)source[i + 2] + indexOffset);
			}
		}

		private static void CopyTriangleIndices(NativeSlice<ushort> source, NativeSlice<ushort> target, int indexOffset)
		{
			int length = source.Length;
			for (int i = 0; i < length; i++)
			{
				target[i] = (ushort)((int)source[i] + indexOffset);
			}
		}

		private static bool NudgeVerticesToNewSpace(VisualElement ve, UIRenderDevice device)
		{
			Debug.Assert(!ve.renderChainData.disableNudging);
			Matrix4x4 matrix4x;
			RenderEvents.GetVerticesTransformInfo(ve, out matrix4x);
			Matrix4x4 matrix4x2 = matrix4x * ve.renderChainData.verticesSpace.inverse;
			Matrix4x4 matrix4x3 = matrix4x2 * ve.renderChainData.verticesSpace;
			float num = Mathf.Abs(matrix4x.m00 - matrix4x3.m00);
			num += Mathf.Abs(matrix4x.m01 - matrix4x3.m01);
			num += Mathf.Abs(matrix4x.m02 - matrix4x3.m02);
			num += Mathf.Abs(matrix4x.m03 - matrix4x3.m03);
			num += Mathf.Abs(matrix4x.m10 - matrix4x3.m10);
			num += Mathf.Abs(matrix4x.m11 - matrix4x3.m11);
			num += Mathf.Abs(matrix4x.m12 - matrix4x3.m12);
			num += Mathf.Abs(matrix4x.m13 - matrix4x3.m13);
			num += Mathf.Abs(matrix4x.m20 - matrix4x3.m20);
			num += Mathf.Abs(matrix4x.m21 - matrix4x3.m21);
			num += Mathf.Abs(matrix4x.m22 - matrix4x3.m22);
			num += Mathf.Abs(matrix4x.m23 - matrix4x3.m23);
			bool flag = num > 0.0001f;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ve.renderChainData.verticesSpace = matrix4x;
				int size = (int)ve.renderChainData.data.allocVerts.size;
				NativeSlice<Vertex> nativeSlice = ve.renderChainData.data.allocPage.vertices.cpuData.Slice<Vertex>((int)ve.renderChainData.data.allocVerts.start, size);
				NativeSlice<Vertex> nativeSlice2;
				device.Update(ve.renderChainData.data, (uint)size, out nativeSlice2);
				int displacementUVStart = ve.renderChainData.displacementUVStart;
				int displacementUVEnd = ve.renderChainData.displacementUVEnd;
				for (int i = 0; i < displacementUVStart; i++)
				{
					Vertex vertex = nativeSlice[i];
					vertex.position = matrix4x2.MultiplyPoint3x4(vertex.position);
					nativeSlice2[i] = vertex;
				}
				for (int j = displacementUVStart; j < displacementUVEnd; j++)
				{
					Vertex vertex2 = nativeSlice[j];
					vertex2.position = matrix4x2.MultiplyPoint3x4(vertex2.position);
					vertex2.uv = matrix4x2.MultiplyVector(vertex2.uv);
					nativeSlice2[j] = vertex2;
				}
				for (int k = displacementUVEnd; k < size; k++)
				{
					Vertex vertex3 = nativeSlice[k];
					vertex3.position = matrix4x2.MultiplyPoint3x4(vertex3.position);
					nativeSlice2[k] = vertex3;
				}
				flag2 = true;
			}
			return flag2;
		}

		private static RenderChainCommand InjectMeshDrawCommand(RenderChain renderChain, VisualElement ve, ref RenderChainCommand cmdPrev, ref RenderChainCommand cmdNext, MeshHandle mesh, int indexCount, int indexOffset, Material material, Texture custom, Texture font)
		{
			RenderChainCommand renderChainCommand = renderChain.AllocCommand();
			renderChainCommand.type = CommandType.Draw;
			renderChainCommand.state = new State
			{
				material = material,
				custom = custom,
				font = font
			};
			renderChainCommand.mesh = mesh;
			renderChainCommand.indexOffset = indexOffset;
			renderChainCommand.indexCount = indexCount;
			renderChainCommand.owner = ve;
			RenderEvents.InjectCommandInBetween(renderChain, renderChainCommand, ref cmdPrev, ref cmdNext);
			return renderChainCommand;
		}

		private static RenderChainCommand InjectClosingMeshDrawCommand(RenderChain renderChain, VisualElement ve, ref RenderChainCommand cmdPrev, ref RenderChainCommand cmdNext, MeshHandle mesh, int indexCount, int indexOffset, Material material, Texture custom, Texture font)
		{
			RenderChainCommand renderChainCommand = renderChain.AllocCommand();
			renderChainCommand.type = CommandType.Draw;
			renderChainCommand.closing = true;
			renderChainCommand.state = new State
			{
				material = material,
				custom = custom,
				font = font
			};
			renderChainCommand.mesh = mesh;
			renderChainCommand.indexOffset = indexOffset;
			renderChainCommand.indexCount = indexCount;
			renderChainCommand.owner = ve;
			RenderEvents.InjectClosingCommandInBetween(renderChain, renderChainCommand, ref cmdPrev, ref cmdNext);
			return renderChainCommand;
		}

		private static void FindCommandInsertionPoint(VisualElement ve, out RenderChainCommand prev, out RenderChainCommand next)
		{
			VisualElement visualElement = ve.renderChainData.prev;
			while (visualElement != null && visualElement.renderChainData.lastCommand == null)
			{
				visualElement = visualElement.renderChainData.prev;
			}
			bool flag = visualElement != null && visualElement.renderChainData.lastCommand != null;
			if (flag)
			{
				bool flag2 = visualElement.hierarchy.parent == ve.hierarchy.parent;
				if (flag2)
				{
					prev = visualElement.renderChainData.lastClosingOrLastCommand;
				}
				else
				{
					bool flag3 = visualElement.IsParentOrAncestorOf(ve);
					if (flag3)
					{
						prev = visualElement.renderChainData.lastCommand;
					}
					else
					{
						RenderChainCommand renderChainCommand = visualElement.renderChainData.lastClosingOrLastCommand;
						bool flag5;
						do
						{
							prev = renderChainCommand;
							renderChainCommand = renderChainCommand.next;
							bool flag4 = renderChainCommand == null || renderChainCommand.owner == ve || !renderChainCommand.closing;
							if (flag4)
							{
								break;
							}
							flag5 = renderChainCommand.owner.IsParentOrAncestorOf(ve);
						}
						while (!flag5);
					}
				}
				next = prev.next;
			}
			else
			{
				VisualElement visualElement2 = ve.renderChainData.next;
				while (visualElement2 != null && visualElement2.renderChainData.firstCommand == null)
				{
					visualElement2 = visualElement2.renderChainData.next;
				}
				next = ((visualElement2 != null) ? visualElement2.renderChainData.firstCommand : null);
				prev = null;
				Debug.Assert(next == null || next.prev == null);
			}
		}

		private static void FindClosingCommandInsertionPoint(VisualElement ve, out RenderChainCommand prev, out RenderChainCommand next)
		{
			VisualElement visualElement = ve.renderChainData.next;
			while (visualElement != null && visualElement.renderChainData.firstCommand == null)
			{
				visualElement = visualElement.renderChainData.next;
			}
			bool flag = visualElement != null && visualElement.renderChainData.firstCommand != null;
			if (flag)
			{
				bool flag2 = visualElement.hierarchy.parent == ve.hierarchy.parent;
				if (flag2)
				{
					next = visualElement.renderChainData.firstCommand;
					prev = next.prev;
				}
				else
				{
					bool flag3 = ve.IsParentOrAncestorOf(visualElement);
					if (flag3)
					{
						bool flag4;
						do
						{
							prev = visualElement.renderChainData.lastClosingOrLastCommand;
							RenderChainCommand next2 = prev.next;
							visualElement = ((next2 != null) ? next2.owner : null);
							flag4 = visualElement == null || !ve.IsParentOrAncestorOf(visualElement);
						}
						while (!flag4);
						next = prev.next;
					}
					else
					{
						prev = ve.renderChainData.lastCommand;
						next = prev.next;
					}
				}
			}
			else
			{
				prev = ve.renderChainData.lastCommand;
				next = prev.next;
			}
		}

		private static void InjectCommandInBetween(RenderChain renderChain, RenderChainCommand cmd, ref RenderChainCommand prev, ref RenderChainCommand next)
		{
			bool flag = prev != null;
			if (flag)
			{
				cmd.prev = prev;
				prev.next = cmd;
			}
			bool flag2 = next != null;
			if (flag2)
			{
				cmd.next = next;
				next.prev = cmd;
			}
			VisualElement owner = cmd.owner;
			owner.renderChainData.lastCommand = cmd;
			bool flag3 = owner.renderChainData.firstCommand == null;
			if (flag3)
			{
				owner.renderChainData.firstCommand = cmd;
			}
			renderChain.OnRenderCommandAdded(cmd);
			prev = cmd;
			next = cmd.next;
		}

		private static void InjectClosingCommandInBetween(RenderChain renderChain, RenderChainCommand cmd, ref RenderChainCommand prev, ref RenderChainCommand next)
		{
			Debug.Assert(cmd.closing);
			bool flag = prev != null;
			if (flag)
			{
				cmd.prev = prev;
				prev.next = cmd;
			}
			bool flag2 = next != null;
			if (flag2)
			{
				cmd.next = next;
				next.prev = cmd;
			}
			VisualElement owner = cmd.owner;
			owner.renderChainData.lastClosingCommand = cmd;
			bool flag3 = owner.renderChainData.firstClosingCommand == null;
			if (flag3)
			{
				owner.renderChainData.firstClosingCommand = cmd;
			}
			renderChain.OnRenderCommandAdded(cmd);
			prev = cmd;
			next = cmd.next;
		}

		private static void ResetCommands(RenderChain renderChain, VisualElement ve)
		{
			bool flag = ve.renderChainData.firstCommand != null;
			if (flag)
			{
				renderChain.OnRenderCommandsRemoved(ve.renderChainData.firstCommand, ve.renderChainData.lastCommand);
			}
			RenderChainCommand renderChainCommand = ((ve.renderChainData.firstCommand != null) ? ve.renderChainData.firstCommand.prev : null);
			RenderChainCommand renderChainCommand2 = ((ve.renderChainData.lastCommand != null) ? ve.renderChainData.lastCommand.next : null);
			Debug.Assert(renderChainCommand == null || renderChainCommand.owner != ve);
			Debug.Assert(renderChainCommand2 == null || renderChainCommand2 == ve.renderChainData.firstClosingCommand || renderChainCommand2.owner != ve);
			bool flag2 = renderChainCommand != null;
			if (flag2)
			{
				renderChainCommand.next = renderChainCommand2;
			}
			bool flag3 = renderChainCommand2 != null;
			if (flag3)
			{
				renderChainCommand2.prev = renderChainCommand;
			}
			bool flag4 = ve.renderChainData.firstCommand != null;
			if (flag4)
			{
				RenderChainCommand renderChainCommand3;
				RenderChainCommand next;
				for (renderChainCommand3 = ve.renderChainData.firstCommand; renderChainCommand3 != ve.renderChainData.lastCommand; renderChainCommand3 = next)
				{
					next = renderChainCommand3.next;
					renderChain.FreeCommand(renderChainCommand3);
				}
				renderChain.FreeCommand(renderChainCommand3);
			}
			ve.renderChainData.firstCommand = (ve.renderChainData.lastCommand = null);
			renderChainCommand = ((ve.renderChainData.firstClosingCommand != null) ? ve.renderChainData.firstClosingCommand.prev : null);
			renderChainCommand2 = ((ve.renderChainData.lastClosingCommand != null) ? ve.renderChainData.lastClosingCommand.next : null);
			Debug.Assert(renderChainCommand == null || renderChainCommand.owner != ve);
			Debug.Assert(renderChainCommand2 == null || renderChainCommand2.owner != ve);
			bool flag5 = renderChainCommand != null;
			if (flag5)
			{
				renderChainCommand.next = renderChainCommand2;
			}
			bool flag6 = renderChainCommand2 != null;
			if (flag6)
			{
				renderChainCommand2.prev = renderChainCommand;
			}
			bool flag7 = ve.renderChainData.firstClosingCommand != null;
			if (flag7)
			{
				renderChain.OnRenderCommandsRemoved(ve.renderChainData.firstClosingCommand, ve.renderChainData.lastClosingCommand);
				RenderChainCommand renderChainCommand4;
				RenderChainCommand next2;
				for (renderChainCommand4 = ve.renderChainData.firstClosingCommand; renderChainCommand4 != ve.renderChainData.lastClosingCommand; renderChainCommand4 = next2)
				{
					next2 = renderChainCommand4.next;
					renderChain.FreeCommand(renderChainCommand4);
				}
				renderChain.FreeCommand(renderChainCommand4);
			}
			ve.renderChainData.firstClosingCommand = (ve.renderChainData.lastClosingCommand = null);
			bool usesLegacyText = ve.renderChainData.usesLegacyText;
			if (usesLegacyText)
			{
				Debug.Assert(ve.renderChainData.textEntries.Count > 0);
				renderChain.RemoveTextElement(ve);
				ve.renderChainData.textEntries.Clear();
				ve.renderChainData.usesLegacyText = false;
			}
		}

		private static readonly float VisibilityTreshold = Mathf.Epsilon;
	}
}
