using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal static class RenderEvents
	{
		internal static void ProcessOnClippingChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = (renderData.dirtiedValues & RenderDataDirtyTypes.ClippingHierarchy) > RenderDataDirtyTypes.None;
			bool flag2 = flag;
			if (flag2)
			{
				stats.recursiveClipUpdates += 1U;
			}
			else
			{
				stats.nonRecursiveClipUpdates += 1U;
			}
			RenderEvents.DepthFirstOnClippingChanged(renderTreeManager, renderData.parent, renderData, dirtyID, flag, true, false, false, false, renderTreeManager.device, ref stats);
		}

		internal static void ProcessOnOpacityChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = (renderData.dirtiedValues & RenderDataDirtyTypes.OpacityHierarchy) > RenderDataDirtyTypes.None;
			stats.recursiveOpacityUpdates += 1U;
			RenderEvents.DepthFirstOnOpacityChanged(renderTreeManager, (renderData.parent != null) ? renderData.parent.compositeOpacity : 1f, renderData, dirtyID, flag, ref stats, false);
		}

		internal static void ProcessOnColorChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
		{
			stats.colorUpdates += 1U;
			RenderEvents.OnColorChanged(renderTreeManager, renderData, dirtyID, ref stats);
		}

		internal static void ProcessOnTransformOrSizeChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
		{
			stats.recursiveTransformUpdates += 1U;
			RenderEvents.DepthFirstOnTransformOrSizeChanged(renderTreeManager, renderData, dirtyID, renderTreeManager.device, false, false, ref stats);
		}

		private static Matrix4x4 GetTransformIDTransformInfo(RenderData renderData)
		{
			Debug.Assert(RenderData.AllocatesID(renderData.transformID) || renderData.isGroupTransform);
			RenderData groupTransformAncestor = renderData.groupTransformAncestor;
			bool flag = groupTransformAncestor != null;
			Matrix4x4 matrix4x;
			if (flag)
			{
				VisualElement.MultiplyMatrix34(groupTransformAncestor.owner.worldTransformInverse, renderData.owner.worldTransformRef, out matrix4x);
			}
			else
			{
				UIRUtility.ComputeMatrixRelativeToRenderTree(renderData, out matrix4x);
			}
			matrix4x.m22 = 1f;
			return matrix4x;
		}

		private static Vector4 GetClipRectIDClipInfo(RenderData renderData)
		{
			Debug.Assert(RenderData.AllocatesID(renderData.clipRectID));
			bool flag = renderData.groupTransformAncestor == null;
			Rect rect;
			if (flag)
			{
				rect = renderData.clippingRect;
			}
			else
			{
				rect = renderData.clippingRectMinusGroup;
			}
			Vector2 min = rect.min;
			Vector2 max = rect.max;
			Vector2 vector = max - min;
			Vector2 vector2 = new Vector2(1f / (vector.x + 0.0001f), 1f / (vector.y + 0.0001f));
			Vector2 vector3 = 2f * vector2;
			Vector2 vector4 = -(min + max) * vector2;
			return new Vector4(vector3.x, vector3.y, vector4.x, vector4.y);
		}

		internal static uint DepthFirstOnChildAdded(RenderTreeManager renderTreeManager, VisualElement parent, VisualElement ve, int index)
		{
			Debug.Assert(ve.panel != null);
			Debug.Assert(ve.renderData == null);
			Debug.Assert(ve.nestedRenderData == null);
			bool flag = ve.insertionIndex >= 0;
			if (flag)
			{
				renderTreeManager.CancelInsertion(ve);
			}
			RenderData renderData = null;
			RenderData pooledRenderData = renderTreeManager.GetPooledRenderData();
			pooledRenderData.owner = ve;
			ve.renderData = pooledRenderData;
			ve.flags &= ~VisualElementFlags.WorldClipDirty;
			bool useRenderTexture = ve.useRenderTexture;
			if (useRenderTexture)
			{
				pooledRenderData.flags |= RenderDataFlags.IsSubTreeQuad;
			}
			bool flag2 = parent == null;
			if (flag2)
			{
				pooledRenderData.renderTree = renderTreeManager.GetPooledRenderTree(renderTreeManager, pooledRenderData);
				renderTreeManager.rootRenderTree = pooledRenderData.renderTree;
			}
			else
			{
				renderData = parent.nestedRenderData ?? parent.renderData;
				pooledRenderData.parent = renderData;
				pooledRenderData.renderTree = pooledRenderData.parent.renderTree;
				pooledRenderData.depthInRenderTree = pooledRenderData.parent.depthInRenderTree + 1;
				bool isGroupTransform = renderData.isGroupTransform;
				if (isGroupTransform)
				{
					pooledRenderData.groupTransformAncestor = renderData;
				}
				else
				{
					pooledRenderData.groupTransformAncestor = renderData.groupTransformAncestor;
				}
			}
			pooledRenderData.renderTree.dirtyTracker.EnsureFits(pooledRenderData.depthInRenderTree);
			bool flag3 = (ve.renderHints & RenderHints.GroupTransform) != RenderHints.None && !pooledRenderData.isSubTreeQuad && !renderTreeManager.drawInCameras;
			if (flag3)
			{
				pooledRenderData.flags |= RenderDataFlags.IsGroupTransform;
			}
			bool isSubTreeQuad = pooledRenderData.isSubTreeQuad;
			if (isSubTreeQuad)
			{
				RenderData pooledRenderData2 = renderTreeManager.GetPooledRenderData();
				ve.nestedRenderData = pooledRenderData2;
				pooledRenderData2.owner = ve;
				pooledRenderData2.flags |= RenderDataFlags.IsNestedRenderTreeRoot;
				pooledRenderData2.transformID = UIRVEShaderInfoAllocator.identityTransform;
				pooledRenderData2.renderTree = renderTreeManager.GetPooledRenderTree(renderTreeManager, pooledRenderData2);
				pooledRenderData2.renderTree.dirtyTracker.EnsureFits(pooledRenderData2.depthInRenderTree);
				renderTreeManager.UIEOnClippingChanged(ve, true);
				renderTreeManager.UIEOnOpacityChanged(ve, false);
				renderTreeManager.UIEOnVisualsChanged(ve, true);
				RenderTree renderTree = pooledRenderData.renderTree;
				Debug.Assert(renderTree != null);
				RenderTree firstChild = renderTree.firstChild;
				renderTree.firstChild = pooledRenderData2.renderTree;
				pooledRenderData2.renderTree.nextSibling = firstChild;
				pooledRenderData2.renderTree.parent = renderTree;
			}
			RenderEvents.UpdateLocalFlipsWinding(pooledRenderData);
			bool flag4 = renderData != null;
			if (flag4)
			{
				RenderData renderData2 = null;
				for (int i = index - 1; i >= 0; i--)
				{
					renderData2 = parent.hierarchy[i].renderData;
					bool flag5 = renderData2 != null;
					if (flag5)
					{
						break;
					}
				}
				bool flag6 = renderData2 != null;
				RenderData renderData3;
				if (flag6)
				{
					renderData3 = renderData2.nextSibling;
					renderData2.nextSibling = pooledRenderData;
					pooledRenderData.prevSibling = renderData2;
				}
				else
				{
					renderData3 = renderData.firstChild;
					renderData.firstChild = pooledRenderData;
				}
				bool flag7 = renderData3 != null;
				if (flag7)
				{
					pooledRenderData.nextSibling = renderData3;
					renderData3.prevSibling = pooledRenderData;
				}
				else
				{
					renderData.lastChild = pooledRenderData;
				}
			}
			Debug.Assert(!RenderData.AllocatesID(pooledRenderData.transformID));
			bool flag8 = RenderEvents.NeedsTransformID(ve);
			if (flag8)
			{
				pooledRenderData.transformID = renderTreeManager.shaderInfoAllocator.AllocTransform();
			}
			else
			{
				pooledRenderData.transformID = BMPAlloc.Invalid;
			}
			pooledRenderData.boneTransformAncestor = null;
			bool flag9 = RenderEvents.NeedsColorID(ve);
			if (flag9)
			{
				RenderEvents.InitColorIDs(renderTreeManager, ve);
				RenderEvents.SetColorValues(renderTreeManager, ve);
			}
			bool flag10 = !RenderData.AllocatesID(pooledRenderData.transformID);
			if (flag10)
			{
				bool flag11 = pooledRenderData.parent != null && !pooledRenderData.isGroupTransform;
				if (flag11)
				{
					bool flag12 = RenderData.AllocatesID(pooledRenderData.parent.transformID);
					if (flag12)
					{
						pooledRenderData.boneTransformAncestor = pooledRenderData.parent;
					}
					else
					{
						pooledRenderData.boneTransformAncestor = pooledRenderData.parent.boneTransformAncestor;
					}
					pooledRenderData.transformID = pooledRenderData.parent.transformID;
					pooledRenderData.transformID.ownedState = OwnedState.Inherited;
				}
				else
				{
					pooledRenderData.transformID = UIRVEShaderInfoAllocator.identityTransform;
				}
			}
			else
			{
				renderTreeManager.shaderInfoAllocator.SetTransformValue(pooledRenderData.transformID, RenderEvents.GetTransformIDTransformInfo(pooledRenderData));
			}
			int childCount = ve.hierarchy.childCount;
			uint num = 0U;
			for (int j = 0; j < childCount; j++)
			{
				num += RenderEvents.DepthFirstOnChildAdded(renderTreeManager, ve, ve.hierarchy[j], j);
			}
			return 1U + num;
		}

		internal static uint DepthFirstOnElementRemoving(RenderTreeManager renderTreeManager, VisualElement ve)
		{
			bool flag = ve.insertionIndex >= 0;
			if (flag)
			{
				renderTreeManager.CancelInsertion(ve);
			}
			int i = ve.hierarchy.childCount - 1;
			uint num = 0U;
			while (i >= 0)
			{
				num += RenderEvents.DepthFirstOnElementRemoving(renderTreeManager, ve.hierarchy[i--]);
			}
			RenderData renderData = ve.renderData;
			RenderData nestedRenderData = ve.nestedRenderData;
			bool flag2 = renderData != null;
			if (flag2)
			{
				RenderEvents.DepthFirstRemoveRenderData(renderTreeManager, renderData);
				Debug.Assert(ve.renderData == null);
			}
			bool flag3 = nestedRenderData != null;
			if (flag3)
			{
				RenderEvents.DepthFirstRemoveRenderData(renderTreeManager, nestedRenderData);
				Debug.Assert(ve.nestedRenderData == null);
			}
			return num + 1U;
		}

		private static void DepthFirstRemoveRenderData(RenderTreeManager renderTreeManager, RenderData renderData)
		{
			RenderEvents.DisconnectSubTree(renderData);
			bool isNestedRenderTreeRoot = renderData.isNestedRenderTreeRoot;
			if (isNestedRenderTreeRoot)
			{
				renderData.owner.nestedRenderData = null;
			}
			else
			{
				renderData.owner.renderData = null;
			}
			RenderData renderData2 = renderData.firstChild;
			RenderEvents.ResetRenderData(renderTreeManager, renderData);
			while (renderData2 != null)
			{
				RenderData nextSibling = renderData2.nextSibling;
				RenderEvents.DoDepthFirstRemoveRenderData(renderTreeManager, renderData2);
				renderData2 = nextSibling;
			}
		}

		private static void DoDepthFirstRemoveRenderData(RenderTreeManager renderTreeManager, RenderData renderData)
		{
			Debug.Assert(!renderData.isNestedRenderTreeRoot);
			renderData.owner.renderData = null;
			RenderData renderData2 = renderData.firstChild;
			RenderEvents.ResetRenderData(renderTreeManager, renderData);
			while (renderData2 != null)
			{
				RenderData nextSibling = renderData2.nextSibling;
				RenderEvents.DoDepthFirstRemoveRenderData(renderTreeManager, renderData2);
				renderData2 = nextSibling;
			}
		}

		private static void DisconnectSubTree(RenderData renderData)
		{
			RenderData parent = renderData.parent;
			bool flag = parent != null;
			if (flag)
			{
				bool flag2 = renderData.prevSibling == null;
				if (flag2)
				{
					parent.firstChild = renderData.nextSibling;
				}
				bool flag3 = renderData.nextSibling == null;
				if (flag3)
				{
					parent.lastChild = renderData.prevSibling;
				}
			}
			bool flag4 = renderData.prevSibling != null;
			if (flag4)
			{
				renderData.prevSibling.nextSibling = renderData.nextSibling;
			}
			bool flag5 = renderData.nextSibling != null;
			if (flag5)
			{
				renderData.nextSibling.prevSibling = renderData.prevSibling;
			}
		}

		private static void DisconnectRenderTreeFromParent(RenderTree parentTree, RenderTree nestedTree)
		{
			bool flag = nestedTree == null || parentTree == null || parentTree == nestedTree;
			if (!flag)
			{
				bool flag2 = parentTree.firstChild == nestedTree;
				if (flag2)
				{
					parentTree.firstChild = nestedTree.nextSibling;
				}
				else
				{
					RenderTree renderTree = parentTree.firstChild;
					while (renderTree.nextSibling != nestedTree)
					{
						renderTree = renderTree.nextSibling;
					}
					renderTree.nextSibling = nestedTree.nextSibling;
				}
			}
		}

		private static void ResetRenderData(RenderTreeManager renderTreeManager, RenderData renderData)
		{
			renderData.renderTree.ChildWillBeRemoved(renderData);
			CommandManipulator.ResetCommands(renderTreeManager, renderData);
			bool flag = renderData.parent == null;
			if (flag)
			{
				RenderTree parent = renderData.renderTree.parent;
				RenderEvents.DisconnectRenderTreeFromParent(parent, renderData.renderTree);
				renderTreeManager.ReturnPoolRenderTree(renderData.renderTree);
			}
			renderData.parent = null;
			renderData.prevSibling = null;
			renderData.nextSibling = null;
			renderData.firstChild = null;
			renderData.lastChild = null;
			renderData.renderTree = null;
			renderTreeManager.ResetGraphicEntries(renderData);
			bool hasExtraData = renderData.hasExtraData;
			if (hasExtraData)
			{
				renderTreeManager.FreeExtraMeshes(renderData);
				renderTreeManager.FreeExtraData(renderData);
			}
			renderData.clipMethod = ClipMethod.Undetermined;
			bool flag2 = RenderData.AllocatesID(renderData.textCoreSettingsID);
			if (flag2)
			{
				renderTreeManager.shaderInfoAllocator.FreeTextCoreSettings(renderData.textCoreSettingsID);
				renderData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
			}
			bool flag3 = RenderData.AllocatesID(renderData.opacityID);
			if (flag3)
			{
				renderTreeManager.shaderInfoAllocator.FreeOpacity(renderData.opacityID);
				renderData.opacityID = UIRVEShaderInfoAllocator.fullOpacity;
			}
			bool flag4 = RenderData.AllocatesID(renderData.colorID);
			if (flag4)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.colorID);
				renderData.colorID = BMPAlloc.Invalid;
			}
			bool flag5 = RenderData.AllocatesID(renderData.backgroundColorID);
			if (flag5)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.backgroundColorID);
				renderData.backgroundColorID = BMPAlloc.Invalid;
			}
			bool flag6 = RenderData.AllocatesID(renderData.borderLeftColorID);
			if (flag6)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderLeftColorID);
				renderData.borderLeftColorID = BMPAlloc.Invalid;
			}
			bool flag7 = RenderData.AllocatesID(renderData.borderTopColorID);
			if (flag7)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderTopColorID);
				renderData.borderTopColorID = BMPAlloc.Invalid;
			}
			bool flag8 = RenderData.AllocatesID(renderData.borderRightColorID);
			if (flag8)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderRightColorID);
				renderData.borderRightColorID = BMPAlloc.Invalid;
			}
			bool flag9 = RenderData.AllocatesID(renderData.borderBottomColorID);
			if (flag9)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderBottomColorID);
				renderData.borderBottomColorID = BMPAlloc.Invalid;
			}
			bool flag10 = RenderData.AllocatesID(renderData.tintColorID);
			if (flag10)
			{
				renderTreeManager.shaderInfoAllocator.FreeColor(renderData.tintColorID);
				renderData.tintColorID = BMPAlloc.Invalid;
			}
			bool flag11 = RenderData.AllocatesID(renderData.clipRectID);
			if (flag11)
			{
				renderTreeManager.shaderInfoAllocator.FreeClipRect(renderData.clipRectID);
				renderData.clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
			}
			bool flag12 = RenderData.AllocatesID(renderData.transformID);
			if (flag12)
			{
				renderTreeManager.shaderInfoAllocator.FreeTransform(renderData.transformID);
				renderData.transformID = UIRVEShaderInfoAllocator.identityTransform;
			}
			renderData.boneTransformAncestor = (renderData.groupTransformAncestor = null);
			bool flag13 = renderData.tailMesh != null;
			if (flag13)
			{
				renderTreeManager.device.Free(renderData.tailMesh);
				renderData.tailMesh = null;
			}
			bool flag14 = renderData.headMesh != null;
			if (flag14)
			{
				renderTreeManager.device.Free(renderData.headMesh);
				renderData.headMesh = null;
			}
			renderTreeManager.ReturnPoolRenderData(renderData);
		}

		private static void DepthFirstOnClippingChanged(RenderTreeManager renderTreeManager, RenderData parentRenderData, RenderData renderData, uint dirtyID, bool hierarchical, bool isRootOfChange, bool isPendingHierarchicalRepaint, bool inheritedClipRectIDChanged, bool inheritedMaskingChanged, UIRenderDevice device, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == renderData.dirtyID;
			bool flag2 = flag && !inheritedClipRectIDChanged && !inheritedMaskingChanged;
			if (!flag2)
			{
				renderData.dirtyID = dirtyID;
				bool flag3 = !isRootOfChange;
				if (flag3)
				{
					stats.recursiveClipUpdatesExpanded += 1U;
				}
				isPendingHierarchicalRepaint |= (renderData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) > RenderDataDirtyTypes.None;
				hierarchical |= (renderData.dirtiedValues & RenderDataDirtyTypes.ClippingHierarchy) > RenderDataDirtyTypes.None;
				bool flag4 = hierarchical || isRootOfChange || inheritedClipRectIDChanged;
				bool flag5 = hierarchical || isRootOfChange;
				bool flag6 = hierarchical || isRootOfChange || inheritedMaskingChanged;
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				bool flag10 = hierarchical;
				ClipMethod clipMethod = renderData.clipMethod;
				ClipMethod clipMethod2 = (flag5 ? RenderEvents.DetermineSelfClipMethod(renderTreeManager, renderData) : clipMethod);
				bool flag11 = false;
				bool flag12 = flag4;
				if (flag12)
				{
					BMPAlloc bmpalloc = renderData.clipRectID;
					bool flag13 = clipMethod2 == ClipMethod.ShaderDiscard;
					if (flag13)
					{
						bool flag14 = !RenderData.AllocatesID(renderData.clipRectID);
						if (flag14)
						{
							bmpalloc = renderTreeManager.shaderInfoAllocator.AllocClipRect();
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
						bool flag16 = RenderData.AllocatesID(renderData.clipRectID);
						if (flag16)
						{
							renderTreeManager.shaderInfoAllocator.FreeClipRect(renderData.clipRectID);
						}
						bool flag17 = !renderData.isGroupTransform;
						if (flag17)
						{
							bmpalloc = ((clipMethod2 != ClipMethod.Scissor && parentRenderData != null) ? parentRenderData.clipRectID : UIRVEShaderInfoAllocator.infiniteClipRect);
							bmpalloc.ownedState = OwnedState.Inherited;
						}
					}
					flag11 = !renderData.clipRectID.Equals(bmpalloc);
					Debug.Assert(!renderData.isGroupTransform || !flag11);
					renderData.clipRectID = bmpalloc;
				}
				bool flag18 = false;
				bool flag19 = clipMethod != clipMethod2;
				if (flag19)
				{
					renderData.clipMethod = clipMethod2;
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
					bool flag22 = clipMethod2 == ClipMethod.ShaderDiscard || (clipMethod == ClipMethod.ShaderDiscard && RenderData.AllocatesID(renderData.clipRectID));
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
					bool flag25 = parentRenderData != null;
					if (flag25)
					{
						num = parentRenderData.childrenMaskDepth;
						num2 = parentRenderData.childrenStencilRef;
					}
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
					bool flag28 = (renderData.owner.renderHints & RenderHints.MaskContainer) == RenderHints.MaskContainer && num < 7;
					if (flag28)
					{
						num2 = num;
					}
					bool flag29 = renderData.childrenMaskDepth != num || renderData.childrenStencilRef != num2;
					if (flag29)
					{
						flag18 = true;
					}
					renderData.childrenMaskDepth = num;
					renderData.childrenStencilRef = num2;
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
					renderData.renderTree.OnRenderDataVisualsChanged(renderData, flag8);
					isPendingHierarchicalRepaint = true;
				}
				bool flag32 = flag9;
				if (flag32)
				{
					renderData.renderTree.OnRenderDataTransformOrSizeChanged(renderData, false, true);
				}
				bool flag33 = flag10;
				if (flag33)
				{
					for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
					{
						RenderEvents.DepthFirstOnClippingChanged(renderTreeManager, renderData, renderData2, dirtyID, hierarchical, false, isPendingHierarchicalRepaint, flag11, flag18, device, ref stats);
					}
				}
			}
		}

		private static void DepthFirstOnOpacityChanged(RenderTreeManager renderTreeManager, float parentCompositeOpacity, RenderData renderData, uint dirtyID, bool hierarchical, ref ChainBuilderStats stats, bool isDoingFullVertexRegeneration = false)
		{
			bool flag = dirtyID == renderData.dirtyID;
			if (!flag)
			{
				renderData.dirtyID = dirtyID;
				bool isSubTreeQuad = renderData.isSubTreeQuad;
				if (!isSubTreeQuad)
				{
					stats.recursiveOpacityUpdatesExpanded += 1U;
					float compositeOpacity = renderData.compositeOpacity;
					float num = renderData.owner.resolvedStyle.opacity * parentCompositeOpacity;
					bool flag2 = (compositeOpacity < RenderEvents.VisibilityTreshold) ^ (num < RenderEvents.VisibilityTreshold);
					bool flag3 = Mathf.Abs(compositeOpacity - num) > 0.0001f || flag2;
					bool flag4 = flag3;
					if (flag4)
					{
						renderData.compositeOpacity = num;
					}
					bool flag5 = false;
					bool flag6 = num < parentCompositeOpacity - 0.0001f;
					bool flag7 = flag6;
					if (flag7)
					{
						bool flag8 = renderData.opacityID.ownedState == OwnedState.Inherited;
						if (flag8)
						{
							flag5 = true;
							renderData.opacityID = renderTreeManager.shaderInfoAllocator.AllocOpacity();
						}
						bool flag9 = (flag5 || flag3) && renderData.opacityID.IsValid();
						if (flag9)
						{
							renderTreeManager.shaderInfoAllocator.SetOpacityValue(renderData.opacityID, num);
						}
					}
					else
					{
						bool flag10 = renderData.opacityID.ownedState == OwnedState.Inherited;
						if (flag10)
						{
							bool flag11 = renderData.parent != null && !renderData.opacityID.Equals(renderData.parent.opacityID);
							if (flag11)
							{
								flag5 = true;
								renderData.opacityID = renderData.parent.opacityID;
								renderData.opacityID.ownedState = OwnedState.Inherited;
							}
						}
						else
						{
							bool flag12 = flag3 && renderData.opacityID.IsValid();
							if (flag12)
							{
								renderTreeManager.shaderInfoAllocator.SetOpacityValue(renderData.opacityID, num);
							}
						}
					}
					bool flag13 = (renderData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) > RenderDataDirtyTypes.None;
					if (flag13)
					{
						isDoingFullVertexRegeneration = true;
					}
					bool flag14 = isDoingFullVertexRegeneration;
					if (!flag14)
					{
						bool flag15 = flag5 && (renderData.dirtiedValues & RenderDataDirtyTypes.Visuals) == RenderDataDirtyTypes.None && (renderData.headMesh != null || renderData.tailMesh != null);
						if (flag15)
						{
							renderData.renderTree.OnRenderDataOpacityIdChanged(renderData);
						}
					}
					bool flag16 = flag3 || flag5 || hierarchical;
					if (flag16)
					{
						for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
						{
							RenderEvents.DepthFirstOnOpacityChanged(renderTreeManager, num, renderData2, dirtyID, hierarchical, ref stats, isDoingFullVertexRegeneration);
						}
					}
				}
			}
		}

		private static void OnColorChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == renderData.dirtyID;
			if (!flag)
			{
				renderData.dirtyID = dirtyID;
				bool isSubTreeQuad = renderData.isSubTreeQuad;
				if (!isSubTreeQuad)
				{
					stats.colorUpdatesExpanded += 1U;
					Color backgroundColor = renderData.owner.resolvedStyle.backgroundColor;
					bool flag2 = renderData.backgroundAlpha == 0f && backgroundColor.a > 0f;
					if (flag2)
					{
						renderData.renderTree.OnRenderDataVisualsChanged(renderData, false);
					}
					renderData.backgroundAlpha = backgroundColor.a;
					bool flag3 = false;
					bool flag4 = (renderData.owner.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor && !renderData.isIgnoringDynamicColorHint;
					if (flag4)
					{
						bool flag5 = RenderEvents.InitColorIDs(renderTreeManager, renderData.owner);
						if (flag5)
						{
							flag3 = true;
						}
						RenderEvents.SetColorValues(renderTreeManager, renderData.owner);
						bool flag6 = renderData.owner is TextElement && !RenderEvents.UpdateTextCoreSettings(renderTreeManager, renderData.owner);
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
						renderData.renderTree.OnRenderDataVisualsChanged(renderData, false);
					}
				}
			}
		}

		private static void DepthFirstOnTransformOrSizeChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, UIRenderDevice device, bool isAncestorOfChangeSkinned, bool transformChanged, ref ChainBuilderStats stats)
		{
			bool flag = dirtyID == renderData.dirtyID;
			if (!flag)
			{
				stats.recursiveTransformUpdatesExpanded += 1U;
				renderData.flags |= RenderDataFlags.IsClippingRectDirty;
				transformChanged |= (renderData.dirtiedValues & RenderDataDirtyTypes.Transform) > RenderDataDirtyTypes.None;
				bool flag2 = RenderData.AllocatesID(renderData.clipRectID);
				if (flag2)
				{
					Debug.Assert(!renderData.isSubTreeQuad);
					renderTreeManager.shaderInfoAllocator.SetClipRectValue(renderData.clipRectID, RenderEvents.GetClipRectIDClipInfo(renderData));
				}
				bool flag3 = transformChanged;
				if (flag3)
				{
					bool flag4 = RenderEvents.UpdateLocalFlipsWinding(renderData);
					if (flag4)
					{
						renderData.renderTree.OnRenderDataVisualsChanged(renderData, true);
					}
					RenderEvents.UpdateZeroScaling(renderData);
				}
				bool flag5 = true;
				bool flag6 = RenderData.AllocatesID(renderData.transformID);
				if (flag6)
				{
					Debug.Assert(!renderData.isNestedRenderTreeRoot);
					renderTreeManager.shaderInfoAllocator.SetTransformValue(renderData.transformID, RenderEvents.GetTransformIDTransformInfo(renderData));
					isAncestorOfChangeSkinned = true;
					stats.boneTransformed += 1U;
				}
				else
				{
					bool flag7 = !transformChanged;
					if (!flag7)
					{
						bool isGroupTransform = renderData.isGroupTransform;
						if (isGroupTransform)
						{
							stats.groupTransformElementsChanged += 1U;
						}
						else
						{
							bool flag8 = isAncestorOfChangeSkinned;
							if (flag8)
							{
								Debug.Assert(RenderData.InheritsID(renderData.transformID));
								flag5 = false;
								stats.skipTransformed += 1U;
							}
							else
							{
								bool flag9 = (renderData.dirtiedValues & (RenderDataDirtyTypes.Visuals | RenderDataDirtyTypes.VisualsHierarchy)) == RenderDataDirtyTypes.None && (renderData.headMesh != null || renderData.tailMesh != null);
								if (flag9)
								{
									bool flag10 = RenderEvents.NudgeVerticesToNewSpace(renderData, renderTreeManager, device);
									if (flag10)
									{
										stats.nudgeTransformed += 1U;
									}
									else
									{
										renderData.renderTree.OnRenderDataVisualsChanged(renderData, false);
										stats.visualUpdateTransformed += 1U;
									}
								}
							}
						}
					}
				}
				bool flag11 = flag5;
				if (flag11)
				{
					renderData.dirtyID = dirtyID;
				}
				bool drawInCameras = renderTreeManager.drawInCameras;
				if (drawInCameras)
				{
					renderData.owner.EnsureWorldTransformAndClipUpToDate();
				}
				bool flag12 = !renderData.isGroupTransform;
				if (flag12)
				{
					for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
					{
						RenderEvents.DepthFirstOnTransformOrSizeChanged(renderTreeManager, renderData2, dirtyID, device, isAncestorOfChangeSkinned, transformChanged, ref stats);
					}
				}
			}
		}

		public static bool UpdateTextCoreSettings(RenderTreeManager renderTreeManager, VisualElement ve)
		{
			bool flag = ve == null || !TextUtilities.IsFontAssigned(ve);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				RenderData renderData = ve.nestedRenderData ?? ve.renderData;
				bool flag3 = RenderData.AllocatesID(renderData.textCoreSettingsID);
				TextCoreSettings textCoreSettingsForElement = TextUtilities.GetTextCoreSettingsForElement(ve, false);
				bool flag4 = !RenderEvents.NeedsColorID(ve);
				bool flag5 = flag4 && !RenderEvents.NeedsTextCoreSettings(ve) && !flag3;
				if (flag5)
				{
					renderData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
					flag2 = true;
				}
				else
				{
					bool flag6 = !flag3;
					if (flag6)
					{
						renderData.textCoreSettingsID = renderTreeManager.shaderInfoAllocator.AllocTextCoreSettings(textCoreSettingsForElement);
					}
					bool flag7 = RenderData.AllocatesID(renderData.textCoreSettingsID);
					if (flag7)
					{
						bool flag8 = ve.panel.contextType == ContextType.Editor;
						if (flag8)
						{
							Color playModeTintColor = ve.playModeTintColor;
							textCoreSettingsForElement.faceColor *= playModeTintColor;
							textCoreSettingsForElement.outlineColor *= playModeTintColor;
							textCoreSettingsForElement.underlayColor *= playModeTintColor;
						}
						renderTreeManager.shaderInfoAllocator.SetTextCoreSettingValue(renderData.textCoreSettingsID, textCoreSettingsForElement);
					}
					flag2 = true;
				}
			}
			return flag2;
		}

		private static bool NudgeVerticesToNewSpace(RenderData renderData, RenderTreeManager renderTreeManager, UIRenderDevice device)
		{
			Matrix4x4 matrix4x;
			UIRUtility.GetVerticesTransformInfo(renderData, out matrix4x);
			Matrix4x4 matrix4x2 = matrix4x * renderData.verticesSpace.inverse;
			Matrix4x4 matrix4x3 = matrix4x2 * renderData.verticesSpace;
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
				renderData.verticesSpace = matrix4x;
				NudgeJobData nudgeJobData = new NudgeJobData
				{
					transform = matrix4x2
				};
				bool flag3 = renderData.headMesh != null;
				if (flag3)
				{
					RenderEvents.PrepareNudgeVertices(device, renderData.headMesh, out nudgeJobData.headSrc, out nudgeJobData.headDst, out nudgeJobData.headCount);
				}
				bool flag4 = renderData.tailMesh != null;
				if (flag4)
				{
					RenderEvents.PrepareNudgeVertices(device, renderData.tailMesh, out nudgeJobData.tailSrc, out nudgeJobData.tailDst, out nudgeJobData.tailCount);
				}
				renderTreeManager.jobManager.Add(ref nudgeJobData);
				bool hasExtraMeshes = renderData.hasExtraMeshes;
				if (hasExtraMeshes)
				{
					ExtraRenderData orAddExtraData = renderTreeManager.GetOrAddExtraData(renderData);
					for (BasicNode<MeshHandle> basicNode = orAddExtraData.extraMesh; basicNode != null; basicNode = basicNode.next)
					{
						NudgeJobData nudgeJobData2 = new NudgeJobData
						{
							transform = nudgeJobData.transform
						};
						RenderEvents.PrepareNudgeVertices(device, basicNode.data, out nudgeJobData2.headSrc, out nudgeJobData2.headDst, out nudgeJobData2.headCount);
						renderTreeManager.jobManager.Add(ref nudgeJobData2);
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		private static void PrepareNudgeVertices(UIRenderDevice device, MeshHandle mesh, out IntPtr src, out IntPtr dst, out int count)
		{
			int size = (int)mesh.allocVerts.size;
			NativeSlice<Vertex> nativeSlice = mesh.allocPage.vertices.cpuData.Slice<Vertex>((int)mesh.allocVerts.start, size);
			NativeSlice<Vertex> nativeSlice2;
			device.Update(mesh, (uint)size, out nativeSlice2);
			src = (IntPtr)nativeSlice.GetUnsafePtr<Vertex>();
			dst = (IntPtr)nativeSlice2.GetUnsafePtr<Vertex>();
			count = size;
		}

		private static ClipMethod DetermineSelfClipMethod(RenderTreeManager renderTreeManager, RenderData renderData)
		{
			bool isSubTreeQuad = renderData.isSubTreeQuad;
			ClipMethod clipMethod;
			if (isSubTreeQuad)
			{
				clipMethod = ClipMethod.NotClipped;
			}
			else
			{
				bool flag = !renderData.owner.ShouldClip();
				if (flag)
				{
					clipMethod = ClipMethod.NotClipped;
				}
				else
				{
					bool drawInCameras = renderTreeManager.drawInCameras;
					if (drawInCameras)
					{
						clipMethod = ClipMethod.ShaderDiscard;
					}
					else
					{
						ClipMethod clipMethod2 = ((renderData.isGroupTransform || (renderData.owner.renderHints & RenderHints.ClipWithScissors) > RenderHints.None) ? ClipMethod.Scissor : ClipMethod.ShaderDiscard);
						bool flag2 = !renderTreeManager.elementBuilder.RequiresStencilMask(renderData.owner);
						if (flag2)
						{
							clipMethod = clipMethod2;
						}
						else
						{
							int num = 0;
							RenderData parent = renderData.parent;
							bool flag3 = parent != null;
							if (flag3)
							{
								num = parent.childrenMaskDepth;
							}
							bool flag4 = num == 7;
							if (flag4)
							{
								clipMethod = clipMethod2;
							}
							else
							{
								clipMethod = ClipMethod.Stencil;
							}
						}
					}
				}
			}
			return clipMethod;
		}

		private static bool UpdateLocalFlipsWinding(RenderData renderData)
		{
			bool flag = !renderData.owner.elementPanel.isFlat;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = false;
				bool flag4 = !renderData.isNestedRenderTreeRoot;
				if (flag4)
				{
					Vector3 value = renderData.owner.resolvedStyle.scale.value;
					float num = value.x * value.y;
					bool flag5 = Math.Abs(num) < 0.001f;
					if (flag5)
					{
						return false;
					}
					flag3 = num < 0f;
				}
				bool localFlipsWinding = renderData.localFlipsWinding;
				bool flag6 = localFlipsWinding != flag3;
				if (flag6)
				{
					renderData.localFlipsWinding = flag3;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		private static void UpdateZeroScaling(RenderData renderData)
		{
			bool isNestedRenderTreeRoot = renderData.isNestedRenderTreeRoot;
			if (!isNestedRenderTreeRoot)
			{
				VisualElement owner = renderData.owner;
				bool flag = Math.Abs(owner.resolvedStyle.scale.value.x * owner.resolvedStyle.scale.value.y) < 0.001f;
				bool flag2 = false;
				VisualElement parent = owner.hierarchy.parent;
				bool flag3 = parent != null;
				if (flag3)
				{
					flag2 = parent.renderData.worldTransformScaleZero;
				}
				renderData.worldTransformScaleZero = flag2 || flag;
			}
		}

		private static bool NeedsTransformID(VisualElement ve)
		{
			return !ve.renderData.isGroupTransform && (ve.renderHints & RenderHints.BoneTransform) > RenderHints.None;
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
			TextCoreSettings textCoreSettingsForElement = TextUtilities.GetTextCoreSettingsForElement(ve, true);
			return textCoreSettingsForElement.outlineWidth != 0f || textCoreSettingsForElement.underlayOffset != Vector2.zero || textCoreSettingsForElement.underlaySoftness != 0f;
		}

		private static bool InitColorIDs(RenderTreeManager renderTreeManager, VisualElement ve)
		{
			IResolvedStyle resolvedStyle = ve.resolvedStyle;
			bool flag = false;
			bool flag2 = !ve.renderData.colorID.IsValid() && ve is TextElement;
			if (flag2)
			{
				ve.renderData.colorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag3 = !ve.renderData.backgroundColorID.IsValid();
			if (flag3)
			{
				ve.renderData.backgroundColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag4 = !ve.renderData.borderLeftColorID.IsValid() && resolvedStyle.borderLeftWidth > 0f;
			if (flag4)
			{
				ve.renderData.borderLeftColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag5 = !ve.renderData.borderTopColorID.IsValid() && resolvedStyle.borderTopWidth > 0f;
			if (flag5)
			{
				ve.renderData.borderTopColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag6 = !ve.renderData.borderRightColorID.IsValid() && resolvedStyle.borderRightWidth > 0f;
			if (flag6)
			{
				ve.renderData.borderRightColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag7 = !ve.renderData.borderBottomColorID.IsValid() && resolvedStyle.borderBottomWidth > 0f;
			if (flag7)
			{
				ve.renderData.borderBottomColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			bool flag8 = !ve.renderData.tintColorID.IsValid();
			if (flag8)
			{
				ve.renderData.tintColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
				flag = true;
			}
			return flag;
		}

		private static void ResetColorIDs(VisualElement ve)
		{
			ve.renderData.colorID = BMPAlloc.Invalid;
			ve.renderData.backgroundColorID = BMPAlloc.Invalid;
			ve.renderData.borderLeftColorID = BMPAlloc.Invalid;
			ve.renderData.borderTopColorID = BMPAlloc.Invalid;
			ve.renderData.borderRightColorID = BMPAlloc.Invalid;
			ve.renderData.borderBottomColorID = BMPAlloc.Invalid;
			ve.renderData.tintColorID = BMPAlloc.Invalid;
		}

		public static void SetColorValues(RenderTreeManager renderTreeManager, VisualElement ve)
		{
			IResolvedStyle resolvedStyle = ve.resolvedStyle;
			bool flag = ve.renderData.colorID.IsValid();
			if (flag)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.colorID, resolvedStyle.color);
			}
			bool flag2 = ve.renderData.backgroundColorID.IsValid();
			if (flag2)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.backgroundColorID, resolvedStyle.backgroundColor);
			}
			bool flag3 = ve.renderData.borderLeftColorID.IsValid();
			if (flag3)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderLeftColorID, resolvedStyle.borderLeftColor);
			}
			bool flag4 = ve.renderData.borderTopColorID.IsValid();
			if (flag4)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderTopColorID, resolvedStyle.borderTopColor);
			}
			bool flag5 = ve.renderData.borderRightColorID.IsValid();
			if (flag5)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderRightColorID, resolvedStyle.borderRightColor);
			}
			bool flag6 = ve.renderData.borderBottomColorID.IsValid();
			if (flag6)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderBottomColorID, resolvedStyle.borderBottomColor);
			}
			bool flag7 = ve.renderData.tintColorID.IsValid();
			if (flag7)
			{
				renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.tintColorID, resolvedStyle.unityBackgroundImageTintColor);
			}
		}

		private static readonly ProfilerMarker k_NudgeVerticesMarker = new ProfilerMarker("UIR.NudgeVertices");

		private static readonly float VisibilityTreshold = 1E-30f;
	}
}
