using System;

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
			bool flag = (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.OpacityHierarchy) > RenderDataDirtyTypes.None;
			stats.recursiveOpacityUpdates += 1U;
			RenderEvents.DepthFirstOnOpacityChanged(renderChain, (ve.hierarchy.parent != null) ? ve.hierarchy.parent.renderChainData.compositeOpacity : 1f, ve, dirtyID, flag, ref stats, false);
		}

		internal static void ProcessOnColorChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats)
		{
			stats.colorUpdates += 1U;
			RenderEvents.OnColorChanged(renderChain, ve, dirtyID, ref stats);
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

		private static Matrix4x4 GetTransformIDTransformInfo(VisualElement ve)
		{
			Debug.Assert(RenderChainVEData.AllocatesID(ve.renderChainData.transformID) || (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None);
			bool flag = ve.renderChainData.groupTransformAncestor != null;
			Matrix4x4 worldTransform;
			if (flag)
			{
				VisualElement.MultiplyMatrix34(ve.renderChainData.groupTransformAncestor.worldTransformInverse, ve.worldTransformRef, out worldTransform);
			}
			else
			{
				worldTransform = ve.worldTransform;
			}
			worldTransform.m22 = 1f;
			return worldTransform;
		}

		private static Vector4 GetClipRectIDClipInfo(VisualElement ve)
		{
			Debug.Assert(RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID));
			bool flag = ve.renderChainData.groupTransformAncestor == null;
			Rect rect;
			if (flag)
			{
				rect = ve.worldClip;
			}
			else
			{
				rect = ve.worldClipMinusGroup;
				VisualElement.TransformAlignedRect(ve.renderChainData.groupTransformAncestor.worldTransformInverse, ref rect);
			}
			Vector2 min = rect.min;
			Vector2 max = rect.max;
			Vector2 vector = max - min;
			Vector2 vector2 = new Vector2(1f / (vector.x + 0.0001f), 1f / (vector.y + 0.0001f));
			Vector2 vector3 = 2f * vector2;
			Vector2 vector4 = -(min + max) * vector2;
			return new Vector4(vector3.x, vector3.y, vector4.x, vector4.y);
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
				ve.renderChainData.colorID = BMPAlloc.Invalid;
				ve.renderChainData.backgroundColorID = BMPAlloc.Invalid;
				ve.renderChainData.borderLeftColorID = BMPAlloc.Invalid;
				ve.renderChainData.borderTopColorID = BMPAlloc.Invalid;
				ve.renderChainData.borderRightColorID = BMPAlloc.Invalid;
				ve.renderChainData.borderBottomColorID = BMPAlloc.Invalid;
				ve.renderChainData.tintColorID = BMPAlloc.Invalid;
				ve.renderChainData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
				ve.renderChainData.compositeOpacity = float.MaxValue;
				RenderEvents.UpdateLocalFlipsWinding(ve);
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
				bool flag7 = RenderEvents.NeedsColorID(ve);
				if (flag7)
				{
					RenderEvents.InitColorIDs(renderChain, ve);
					RenderEvents.SetColorValues(renderChain, ve);
				}
				bool flag8 = !RenderChainVEData.AllocatesID(ve.renderChainData.transformID);
				if (flag8)
				{
					bool flag9 = parent != null && (ve.renderHints & RenderHints.GroupTransform) == RenderHints.None;
					if (flag9)
					{
						bool flag10 = RenderChainVEData.AllocatesID(parent.renderChainData.transformID);
						if (flag10)
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
			bool isInChain = ve.renderChainData.isInChain;
			if (isInChain)
			{
				renderChain.ChildWillBeRemoved(ve);
				CommandGenerator.ResetCommands(renderChain, ve);
				renderChain.ResetTextures(ve);
				ve.renderChainData.isInChain = false;
				ve.renderChainData.clipMethod = ClipMethod.Undetermined;
				bool flag = ve.renderChainData.next != null;
				if (flag)
				{
					ve.renderChainData.next.renderChainData.prev = ve.renderChainData.prev;
				}
				bool flag2 = ve.renderChainData.prev != null;
				if (flag2)
				{
					ve.renderChainData.prev.renderChainData.next = ve.renderChainData.next;
				}
				bool flag3 = RenderChainVEData.AllocatesID(ve.renderChainData.textCoreSettingsID);
				if (flag3)
				{
					renderChain.shaderInfoAllocator.FreeTextCoreSettings(ve.renderChainData.textCoreSettingsID);
					ve.renderChainData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
				}
				bool flag4 = RenderChainVEData.AllocatesID(ve.renderChainData.opacityID);
				if (flag4)
				{
					renderChain.shaderInfoAllocator.FreeOpacity(ve.renderChainData.opacityID);
					ve.renderChainData.opacityID = UIRVEShaderInfoAllocator.fullOpacity;
				}
				bool flag5 = RenderChainVEData.AllocatesID(ve.renderChainData.colorID);
				if (flag5)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.colorID);
					ve.renderChainData.colorID = BMPAlloc.Invalid;
				}
				bool flag6 = RenderChainVEData.AllocatesID(ve.renderChainData.backgroundColorID);
				if (flag6)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.backgroundColorID);
					ve.renderChainData.backgroundColorID = BMPAlloc.Invalid;
				}
				bool flag7 = RenderChainVEData.AllocatesID(ve.renderChainData.borderLeftColorID);
				if (flag7)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.borderLeftColorID);
					ve.renderChainData.borderLeftColorID = BMPAlloc.Invalid;
				}
				bool flag8 = RenderChainVEData.AllocatesID(ve.renderChainData.borderTopColorID);
				if (flag8)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.borderTopColorID);
					ve.renderChainData.borderTopColorID = BMPAlloc.Invalid;
				}
				bool flag9 = RenderChainVEData.AllocatesID(ve.renderChainData.borderRightColorID);
				if (flag9)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.borderRightColorID);
					ve.renderChainData.borderRightColorID = BMPAlloc.Invalid;
				}
				bool flag10 = RenderChainVEData.AllocatesID(ve.renderChainData.borderBottomColorID);
				if (flag10)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.borderBottomColorID);
					ve.renderChainData.borderBottomColorID = BMPAlloc.Invalid;
				}
				bool flag11 = RenderChainVEData.AllocatesID(ve.renderChainData.tintColorID);
				if (flag11)
				{
					renderChain.shaderInfoAllocator.FreeColor(ve.renderChainData.tintColorID);
					ve.renderChainData.tintColorID = BMPAlloc.Invalid;
				}
				bool flag12 = RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID);
				if (flag12)
				{
					renderChain.shaderInfoAllocator.FreeClipRect(ve.renderChainData.clipRectID);
					ve.renderChainData.clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
				}
				bool flag13 = RenderChainVEData.AllocatesID(ve.renderChainData.transformID);
				if (flag13)
				{
					renderChain.shaderInfoAllocator.FreeTransform(ve.renderChainData.transformID);
					ve.renderChainData.transformID = UIRVEShaderInfoAllocator.identityTransform;
				}
				ve.renderChainData.boneTransformAncestor = (ve.renderChainData.groupTransformAncestor = null);
				bool flag14 = ve.renderChainData.closingData != null;
				if (flag14)
				{
					renderChain.device.Free(ve.renderChainData.closingData);
					ve.renderChainData.closingData = null;
				}
				bool flag15 = ve.renderChainData.data != null;
				if (flag15)
				{
					renderChain.device.Free(ve.renderChainData.data);
					ve.renderChainData.data = null;
				}
			}
			ve.renderChainData.prev = null;
			ve.renderChainData.next = null;
			return num + 1U;
		}

		private static void DepthFirstOnClippingChanged(RenderChain renderChain, VisualElement parent, VisualElement ve, uint dirtyID, bool hierarchical, bool isRootOfChange, bool isPendingHierarchicalRepaint, bool inheritedClipRectIDChanged, bool inheritedMaskingChanged, UIRenderDevice device, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			bool flag2 = flag && !inheritedClipRectIDChanged && !inheritedMaskingChanged;
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
				bool flag6 = hierarchical || isRootOfChange || inheritedMaskingChanged;
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
				bool flag18 = false;
				bool flag19 = clipMethod != clipMethod2;
				if (flag19)
				{
					ve.renderChainData.clipMethod = clipMethod2;
					bool flag20 = clipMethod == ClipMethod.Stencil || clipMethod2 == ClipMethod.Stencil;
					if (flag20)
					{
						flag18 = true;
						flag6 = true;
					}
					bool flag21 = clipMethod == ClipMethod.Scissor || clipMethod2 == ClipMethod.Scissor;
					if (flag21)
					{
						flag7 = true;
					}
					bool flag22 = clipMethod2 == ClipMethod.ShaderDiscard || (clipMethod == ClipMethod.ShaderDiscard && RenderChainVEData.AllocatesID(ve.renderChainData.clipRectID));
					if (flag22)
					{
						flag9 = true;
					}
				}
				bool flag23 = flag11;
				if (flag23)
				{
					flag10 = true;
					flag8 = true;
				}
				bool flag24 = flag6;
				if (flag24)
				{
					int num = 0;
					int num2 = 0;
					bool flag25 = parent != null;
					if (flag25)
					{
						num = parent.renderChainData.childrenMaskDepth;
						num2 = parent.renderChainData.childrenStencilRef;
						bool flag26 = clipMethod2 == ClipMethod.Stencil;
						if (flag26)
						{
							bool flag27 = num > num2;
							if (flag27)
							{
								num2++;
							}
							num++;
						}
						bool flag28 = (ve.renderHints & RenderHints.MaskContainer) == RenderHints.MaskContainer && num < 7;
						if (flag28)
						{
							num2 = num;
						}
					}
					bool flag29 = ve.renderChainData.childrenMaskDepth != num || ve.renderChainData.childrenStencilRef != num2;
					if (flag29)
					{
						flag18 = true;
					}
					ve.renderChainData.childrenMaskDepth = num;
					ve.renderChainData.childrenStencilRef = num2;
				}
				bool flag30 = flag18;
				if (flag30)
				{
					flag10 = true;
					flag8 = true;
				}
				bool flag31 = (flag7 || flag8) && !isPendingHierarchicalRepaint;
				if (flag31)
				{
					renderChain.UIEOnVisualsChanged(ve, flag8);
					isPendingHierarchicalRepaint = true;
				}
				bool flag32 = flag9;
				if (flag32)
				{
					renderChain.UIEOnTransformOrSizeChanged(ve, false, true);
				}
				bool flag33 = flag10;
				if (flag33)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnClippingChanged(renderChain, ve, ve.hierarchy[i], dirtyID, hierarchical, false, isPendingHierarchicalRepaint, flag11, flag18, device, ref stats);
					}
				}
			}
		}

		private static void DepthFirstOnOpacityChanged(RenderChain renderChain, float parentCompositeOpacity, VisualElement ve, uint dirtyID, bool hierarchical, ref ChainBuilderStats stats, bool isDoingFullVertexRegeneration = false)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			if (!flag)
			{
				ve.renderChainData.dirtyID = dirtyID;
				stats.recursiveOpacityUpdatesExpanded += 1U;
				float compositeOpacity = ve.renderChainData.compositeOpacity;
				float num = ve.resolvedStyle.opacity * parentCompositeOpacity;
				bool flag2 = (compositeOpacity < RenderEvents.VisibilityTreshold) ^ (num < RenderEvents.VisibilityTreshold);
				bool flag3 = Mathf.Abs(compositeOpacity - num) > 0.0001f || flag2;
				bool flag4 = flag3;
				if (flag4)
				{
					ve.renderChainData.compositeOpacity = num;
				}
				bool flag5 = false;
				bool flag6 = num < parentCompositeOpacity - 0.0001f;
				bool flag7 = flag6;
				if (flag7)
				{
					bool flag8 = ve.renderChainData.opacityID.ownedState == OwnedState.Inherited;
					if (flag8)
					{
						flag5 = true;
						ve.renderChainData.opacityID = renderChain.shaderInfoAllocator.AllocOpacity();
					}
					bool flag9 = (flag5 || flag3) && ve.renderChainData.opacityID.IsValid();
					if (flag9)
					{
						renderChain.shaderInfoAllocator.SetOpacityValue(ve.renderChainData.opacityID, num);
					}
				}
				else
				{
					bool flag10 = ve.renderChainData.opacityID.ownedState == OwnedState.Inherited;
					if (flag10)
					{
						bool flag11 = ve.hierarchy.parent != null && !ve.renderChainData.opacityID.Equals(ve.hierarchy.parent.renderChainData.opacityID);
						if (flag11)
						{
							flag5 = true;
							ve.renderChainData.opacityID = ve.hierarchy.parent.renderChainData.opacityID;
							ve.renderChainData.opacityID.ownedState = OwnedState.Inherited;
						}
					}
					else
					{
						bool flag12 = flag3 && ve.renderChainData.opacityID.IsValid();
						if (flag12)
						{
							renderChain.shaderInfoAllocator.SetOpacityValue(ve.renderChainData.opacityID, num);
						}
					}
				}
				if (!isDoingFullVertexRegeneration)
				{
					bool flag13 = flag5 && (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.Visuals) == RenderDataDirtyTypes.None && (ve.renderChainData.data != null || ve.renderChainData.closingData != null);
					if (flag13)
					{
						renderChain.UIEOnOpacityIdChanged(ve);
					}
				}
				bool flag14 = flag3 || flag5 || hierarchical;
				if (flag14)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnOpacityChanged(renderChain, num, ve.hierarchy[i], dirtyID, hierarchical, ref stats, isDoingFullVertexRegeneration);
					}
				}
			}
		}

		private static void OnColorChanged(RenderChain renderChain, VisualElement ve, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == ve.renderChainData.dirtyID;
			if (!flag)
			{
				ve.renderChainData.dirtyID = dirtyID;
				stats.colorUpdatesExpanded += 1U;
				Color backgroundColor = ve.resolvedStyle.backgroundColor;
				bool flag2 = ve.renderChainData.backgroundColor.a == 0f && backgroundColor.a > 0f;
				if (flag2)
				{
					renderChain.UIEOnVisualsChanged(ve, false);
				}
				ve.renderChainData.backgroundColor = backgroundColor;
				bool flag3 = false;
				bool flag4 = (ve.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor && !ve.renderChainData.isIgnoringDynamicColorHint;
				if (flag4)
				{
					bool flag5 = RenderEvents.InitColorIDs(renderChain, ve);
					if (flag5)
					{
						flag3 = true;
					}
					RenderEvents.SetColorValues(renderChain, ve);
					bool flag6 = ve is TextElement && !RenderEvents.UpdateTextCoreSettings(renderChain, ve);
					if (flag6)
					{
						flag3 = true;
					}
				}
				else
				{
					flag3 = true;
				}
				bool flag7 = flag3;
				if (flag7)
				{
					renderChain.UIEOnVisualsChanged(ve, false);
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
				bool flag3 = transformChanged && RenderEvents.UpdateLocalFlipsWinding(ve);
				if (flag3)
				{
					renderChain.UIEOnVisualsChanged(ve, true);
				}
				bool flag4 = transformChanged;
				if (flag4)
				{
					RenderEvents.UpdateZeroScaling(ve);
				}
				bool flag5 = true;
				bool flag6 = RenderChainVEData.AllocatesID(ve.renderChainData.transformID);
				if (flag6)
				{
					renderChain.shaderInfoAllocator.SetTransformValue(ve.renderChainData.transformID, RenderEvents.GetTransformIDTransformInfo(ve));
					isAncestorOfChangeSkinned = true;
					stats.boneTransformed += 1U;
				}
				else
				{
					bool flag7 = !transformChanged;
					if (!flag7)
					{
						bool flag8 = (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None;
						if (flag8)
						{
							stats.groupTransformElementsChanged += 1U;
						}
						else
						{
							bool flag9 = isAncestorOfChangeSkinned;
							if (flag9)
							{
								Debug.Assert(RenderChainVEData.InheritsID(ve.renderChainData.transformID));
								flag5 = false;
								stats.skipTransformed += 1U;
							}
							else
							{
								bool flag10 = (ve.renderChainData.dirtiedValues & (RenderDataDirtyTypes.Visuals | RenderDataDirtyTypes.VisualsHierarchy)) == RenderDataDirtyTypes.None && ve.renderChainData.data != null;
								if (flag10)
								{
									bool flag11 = !ve.renderChainData.disableNudging && CommandGenerator.NudgeVerticesToNewSpace(ve, renderChain, device);
									if (flag11)
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
				bool flag12 = flag5;
				if (flag12)
				{
					ve.renderChainData.dirtyID = dirtyID;
				}
				bool drawInCameras = renderChain.drawInCameras;
				if (drawInCameras)
				{
					ve.EnsureWorldTransformAndClipUpToDate();
				}
				bool flag13 = (ve.renderHints & RenderHints.GroupTransform) == RenderHints.None;
				if (flag13)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						RenderEvents.DepthFirstOnTransformOrSizeChanged(renderChain, ve, ve.hierarchy[i], dirtyID, device, isAncestorOfChangeSkinned, transformChanged, ref stats);
					}
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
				bool flag4 = !hierarchical && (ve.renderChainData.dirtiedValues & RenderDataDirtyTypes.AllVisuals) == RenderDataDirtyTypes.VisualsOpacityId;
				if (flag4)
				{
					stats.opacityIdUpdates += 1U;
					CommandGenerator.UpdateOpacityId(ve, renderChain);
				}
				else
				{
					RenderEvents.UpdateWorldFlipsWinding(ve);
					Debug.Assert(ve.renderChainData.clipMethod > ClipMethod.Undetermined);
					Debug.Assert(RenderChainVEData.AllocatesID(ve.renderChainData.transformID) || ve.hierarchy.parent == null || ve.renderChainData.transformID.Equals(ve.hierarchy.parent.renderChainData.transformID) || (ve.renderHints & RenderHints.GroupTransform) > RenderHints.None);
					bool flag5 = ve is TextElement;
					if (flag5)
					{
						RenderEvents.UpdateTextCoreSettings(renderChain, ve);
					}
					bool flag6 = (ve.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor;
					if (flag6)
					{
						RenderEvents.SetColorValues(renderChain, ve);
					}
					UIRStylePainter.ClosingInfo closingInfo = CommandGenerator.PaintElement(renderChain, ve, ref stats);
					bool flag7 = hierarchical;
					if (flag7)
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
						CommandGenerator.ClosePaintElement(ve, closingInfo, renderChain, ref stats);
					}
				}
			}
		}

		private static bool UpdateTextCoreSettings(RenderChain renderChain, VisualElement ve)
		{
			bool flag = ve == null || !TextUtilities.IsFontAssigned(ve);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = RenderChainVEData.AllocatesID(ve.renderChainData.textCoreSettingsID);
				TextCoreSettings textCoreSettingsForElement = TextUtilities.GetTextCoreSettingsForElement(ve);
				bool flag4 = !RenderEvents.NeedsColorID(ve);
				bool flag5 = flag4 && !RenderEvents.NeedsTextCoreSettings(ve) && !flag3;
				if (flag5)
				{
					ve.renderChainData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
					flag2 = true;
				}
				else
				{
					bool flag6 = !flag3;
					if (flag6)
					{
						ve.renderChainData.textCoreSettingsID = renderChain.shaderInfoAllocator.AllocTextCoreSettings(textCoreSettingsForElement);
					}
					bool flag7 = RenderChainVEData.AllocatesID(ve.renderChainData.textCoreSettingsID);
					if (flag7)
					{
						bool flag8 = ve.panel.contextType == ContextType.Editor;
						if (flag8)
						{
							textCoreSettingsForElement.faceColor *= UIElementsUtility.editorPlayModeTintColor;
							textCoreSettingsForElement.outlineColor *= UIElementsUtility.editorPlayModeTintColor;
							textCoreSettingsForElement.underlayColor *= UIElementsUtility.editorPlayModeTintColor;
						}
						renderChain.shaderInfoAllocator.SetTextCoreSettingValue(ve.renderChainData.textCoreSettingsID, textCoreSettingsForElement, ve.panel.contextType == ContextType.Editor);
					}
					flag2 = true;
				}
			}
			return flag2;
		}

		private static bool IsElementHierarchyHidden(VisualElement ve)
		{
			return ve.resolvedStyle.display == DisplayStyle.None;
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
				ClipMethod clipMethod2 = (((ve.renderHints & RenderHints.DirtyOffset) > RenderHints.None) ? ClipMethod.Scissor : ClipMethod.ShaderDiscard);
				bool flag2 = !UIRUtility.IsRoundRect(ve) && !UIRUtility.IsVectorImageBackground(ve);
				if (flag2)
				{
					clipMethod = clipMethod2;
				}
				else
				{
					int num = 0;
					VisualElement parent = ve.hierarchy.parent;
					bool flag3 = parent != null;
					if (flag3)
					{
						num = parent.renderChainData.childrenMaskDepth;
					}
					bool flag4 = num == 7;
					if (flag4)
					{
						clipMethod = clipMethod2;
					}
					else
					{
						clipMethod = (renderChain.drawInCameras ? clipMethod2 : ClipMethod.Stencil);
					}
				}
			}
			return clipMethod;
		}

		private static bool UpdateLocalFlipsWinding(VisualElement ve)
		{
			bool localFlipsWinding = ve.renderChainData.localFlipsWinding;
			Vector3 scale = ve.transform.scale;
			float num = scale.x * scale.y;
			bool flag = Math.Abs(num) < 0.001f;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = num < 0f;
				bool flag4 = localFlipsWinding != flag3;
				if (flag4)
				{
					ve.renderChainData.localFlipsWinding = flag3;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		private static void UpdateWorldFlipsWinding(VisualElement ve)
		{
			bool localFlipsWinding = ve.renderChainData.localFlipsWinding;
			bool flag = false;
			VisualElement parent = ve.hierarchy.parent;
			bool flag2 = parent != null;
			if (flag2)
			{
				flag = parent.renderChainData.worldFlipsWinding;
			}
			ve.renderChainData.worldFlipsWinding = flag ^ localFlipsWinding;
		}

		private static void UpdateZeroScaling(VisualElement ve)
		{
			bool flag = Math.Abs(ve.transform.scale.x * ve.transform.scale.y) < 0.001f;
			ve.renderChainData.localTransformScaleZero = flag;
			bool flag2 = false;
			VisualElement parent = ve.hierarchy.parent;
			bool flag3 = parent != null;
			if (flag3)
			{
				flag2 = parent.renderChainData.worldTransformScaleZero;
			}
			ve.renderChainData.worldTransformScaleZero = flag2 || flag;
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

		internal static bool NeedsColorID(VisualElement ve)
		{
			return (ve.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor;
		}

		internal static bool NeedsTextCoreSettings(VisualElement ve)
		{
			TextCoreSettings textCoreSettingsForElement = TextUtilities.GetTextCoreSettingsForElement(ve);
			return textCoreSettingsForElement.outlineWidth != 0f || textCoreSettingsForElement.underlayOffset != Vector2.zero || textCoreSettingsForElement.underlaySoftness != 0f;
		}

		private static bool InitColorIDs(RenderChain renderChain, VisualElement ve)
		{
			IResolvedStyle resolvedStyle = ve.resolvedStyle;
			bool flag = false;
			bool flag2 = !ve.renderChainData.colorID.IsValid() && resolvedStyle.color != Color.white;
			if (flag2)
			{
				ve.renderChainData.colorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag3 = !ve.renderChainData.backgroundColorID.IsValid() && resolvedStyle.backgroundColor != Color.clear;
			if (flag3)
			{
				ve.renderChainData.backgroundColorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag4 = !ve.renderChainData.borderLeftColorID.IsValid() && resolvedStyle.borderLeftWidth > 0f;
			if (flag4)
			{
				ve.renderChainData.borderLeftColorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag5 = !ve.renderChainData.borderTopColorID.IsValid() && resolvedStyle.borderTopWidth > 0f;
			if (flag5)
			{
				ve.renderChainData.borderTopColorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag6 = !ve.renderChainData.borderRightColorID.IsValid() && resolvedStyle.borderRightWidth > 0f;
			if (flag6)
			{
				ve.renderChainData.borderRightColorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag7 = !ve.renderChainData.borderBottomColorID.IsValid() && resolvedStyle.borderBottomWidth > 0f;
			if (flag7)
			{
				ve.renderChainData.borderBottomColorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag8 = !ve.renderChainData.tintColorID.IsValid() && resolvedStyle.unityBackgroundImageTintColor != Color.white;
			if (flag8)
			{
				ve.renderChainData.tintColorID = renderChain.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			return flag;
		}

		private static void ResetColorIDs(VisualElement ve)
		{
			ve.renderChainData.colorID = BMPAlloc.Invalid;
			ve.renderChainData.backgroundColorID = BMPAlloc.Invalid;
			ve.renderChainData.borderLeftColorID = BMPAlloc.Invalid;
			ve.renderChainData.borderTopColorID = BMPAlloc.Invalid;
			ve.renderChainData.borderRightColorID = BMPAlloc.Invalid;
			ve.renderChainData.borderBottomColorID = BMPAlloc.Invalid;
			ve.renderChainData.tintColorID = BMPAlloc.Invalid;
		}

		private static void SetColorValues(RenderChain renderChain, VisualElement ve)
		{
			bool flag = ve.panel.contextType == ContextType.Editor;
			IResolvedStyle resolvedStyle = ve.resolvedStyle;
			bool flag2 = ve.renderChainData.colorID.IsValid();
			if (flag2)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.colorID, resolvedStyle.color, flag);
			}
			bool flag3 = ve.renderChainData.backgroundColorID.IsValid();
			if (flag3)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.backgroundColorID, resolvedStyle.backgroundColor, flag);
			}
			bool flag4 = ve.renderChainData.borderLeftColorID.IsValid();
			if (flag4)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.borderLeftColorID, resolvedStyle.borderLeftColor, flag);
			}
			bool flag5 = ve.renderChainData.borderTopColorID.IsValid();
			if (flag5)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.borderTopColorID, resolvedStyle.borderTopColor, flag);
			}
			bool flag6 = ve.renderChainData.borderRightColorID.IsValid();
			if (flag6)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.borderRightColorID, resolvedStyle.borderRightColor, flag);
			}
			bool flag7 = ve.renderChainData.borderBottomColorID.IsValid();
			if (flag7)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.borderBottomColorID, resolvedStyle.borderBottomColor, flag);
			}
			bool flag8 = ve.renderChainData.tintColorID.IsValid();
			if (flag8)
			{
				renderChain.shaderInfoAllocator.SetColorValue(ve.renderChainData.tintColorID, resolvedStyle.unityBackgroundImageTintColor, flag);
			}
		}

		private static readonly float VisibilityTreshold = 1E-30f;
	}
}
