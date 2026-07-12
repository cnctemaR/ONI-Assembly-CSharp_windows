using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImDrawListPtr
	{
		public unsafe readonly ImDrawList* NativePtr { get; }

		public unsafe ImDrawListPtr(ImDrawList* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawListPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawList*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawListPtr(ImDrawList* nativePtr)
		{
			return new ImDrawListPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawList*(ImDrawListPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawListPtr(IntPtr nativePtr)
		{
			return new ImDrawListPtr(nativePtr);
		}

		public unsafe ImPtrVector<ImDrawCmdPtr> CmdBuffer
		{
			get
			{
				return new ImPtrVector<ImDrawCmdPtr>(this.NativePtr->CmdBuffer, Unsafe.SizeOf<ImDrawCmd>());
			}
		}

		public unsafe ImVector<ushort> IdxBuffer
		{
			get
			{
				return new ImVector<ushort>(this.NativePtr->IdxBuffer);
			}
		}

		public unsafe ImPtrVector<ImDrawVertPtr> VtxBuffer
		{
			get
			{
				return new ImPtrVector<ImDrawVertPtr>(this.NativePtr->VtxBuffer, Unsafe.SizeOf<ImDrawVert>());
			}
		}

		public unsafe ref ImDrawListFlags Flags
		{
			get
			{
				return Unsafe.AsRef<ImDrawListFlags>((void*)(&this.NativePtr->Flags));
			}
		}

		public unsafe ref uint _VtxCurrentIdx
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->_VtxCurrentIdx));
			}
		}

		public unsafe ref IntPtr _Data
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->_Data));
			}
		}

		public unsafe NullTerminatedString _OwnerName
		{
			get
			{
				return new NullTerminatedString(this.NativePtr->_OwnerName);
			}
		}

		public unsafe ImDrawVertPtr _VtxWritePtr
		{
			get
			{
				return new ImDrawVertPtr(this.NativePtr->_VtxWritePtr);
			}
		}

		public unsafe IntPtr _IdxWritePtr
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->_IdxWritePtr);
			}
			set
			{
				this.NativePtr->_IdxWritePtr = (ushort*)(void*)value;
			}
		}

		public unsafe ImVector<Vector4> _ClipRectStack
		{
			get
			{
				return new ImVector<Vector4>(this.NativePtr->_ClipRectStack);
			}
		}

		public unsafe ImVector<IntPtr> _TextureIdStack
		{
			get
			{
				return new ImVector<IntPtr>(this.NativePtr->_TextureIdStack);
			}
		}

		public unsafe ImVector<Vector2> _Path
		{
			get
			{
				return new ImVector<Vector2>(this.NativePtr->_Path);
			}
		}

		public unsafe ref ImDrawCmdHeader _CmdHeader
		{
			get
			{
				return Unsafe.AsRef<ImDrawCmdHeader>((void*)(&this.NativePtr->_CmdHeader));
			}
		}

		public unsafe ref ImDrawListSplitter _Splitter
		{
			get
			{
				return Unsafe.AsRef<ImDrawListSplitter>((void*)(&this.NativePtr->_Splitter));
			}
		}

		public unsafe ref float _FringeScale
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->_FringeScale));
			}
		}

		public int _CalcCircleAutoSegmentCount(float radius)
		{
			return ImGuiNative.ImDrawList__CalcCircleAutoSegmentCount(this.NativePtr, radius);
		}

		public void _ClearFreeMemory()
		{
			ImGuiNative.ImDrawList__ClearFreeMemory(this.NativePtr);
		}

		public void _OnChangedClipRect()
		{
			ImGuiNative.ImDrawList__OnChangedClipRect(this.NativePtr);
		}

		public void _OnChangedTextureID()
		{
			ImGuiNative.ImDrawList__OnChangedTextureID(this.NativePtr);
		}

		public void _OnChangedVtxOffset()
		{
			ImGuiNative.ImDrawList__OnChangedVtxOffset(this.NativePtr);
		}

		public void _PathArcToFastEx(Vector2 center, float radius, int a_min_sample, int a_max_sample, int a_step)
		{
			ImGuiNative.ImDrawList__PathArcToFastEx(this.NativePtr, center, radius, a_min_sample, a_max_sample, a_step);
		}

		public void _PathArcToN(Vector2 center, float radius, float a_min, float a_max, int num_segments)
		{
			ImGuiNative.ImDrawList__PathArcToN(this.NativePtr, center, radius, a_min, a_max, num_segments);
		}

		public void _PopUnusedDrawCmd()
		{
			ImGuiNative.ImDrawList__PopUnusedDrawCmd(this.NativePtr);
		}

		public void _ResetForNewFrame()
		{
			ImGuiNative.ImDrawList__ResetForNewFrame(this.NativePtr);
		}

		public void AddBezierCubic(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, uint col, float thickness)
		{
			int num = 0;
			ImGuiNative.ImDrawList_AddBezierCubic(this.NativePtr, p1, p2, p3, p4, col, thickness, num);
		}

		public void AddBezierCubic(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, uint col, float thickness, int num_segments)
		{
			ImGuiNative.ImDrawList_AddBezierCubic(this.NativePtr, p1, p2, p3, p4, col, thickness, num_segments);
		}

		public void AddBezierQuadratic(Vector2 p1, Vector2 p2, Vector2 p3, uint col, float thickness)
		{
			int num = 0;
			ImGuiNative.ImDrawList_AddBezierQuadratic(this.NativePtr, p1, p2, p3, col, thickness, num);
		}

		public void AddBezierQuadratic(Vector2 p1, Vector2 p2, Vector2 p3, uint col, float thickness, int num_segments)
		{
			ImGuiNative.ImDrawList_AddBezierQuadratic(this.NativePtr, p1, p2, p3, col, thickness, num_segments);
		}

		public unsafe void AddCallback(IntPtr callback, IntPtr callback_data)
		{
			void* ptr = callback_data.ToPointer();
			ImGuiNative.ImDrawList_AddCallback(this.NativePtr, callback, ptr);
		}

		public void AddCircle(Vector2 center, float radius, uint col)
		{
			int num = 0;
			float num2 = 1f;
			ImGuiNative.ImDrawList_AddCircle(this.NativePtr, center, radius, col, num, num2);
		}

		public void AddCircle(Vector2 center, float radius, uint col, int num_segments)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_AddCircle(this.NativePtr, center, radius, col, num_segments, num);
		}

		public void AddCircle(Vector2 center, float radius, uint col, int num_segments, float thickness)
		{
			ImGuiNative.ImDrawList_AddCircle(this.NativePtr, center, radius, col, num_segments, thickness);
		}

		public void AddCircleFilled(Vector2 center, float radius, uint col)
		{
			int num = 0;
			ImGuiNative.ImDrawList_AddCircleFilled(this.NativePtr, center, radius, col, num);
		}

		public void AddCircleFilled(Vector2 center, float radius, uint col, int num_segments)
		{
			ImGuiNative.ImDrawList_AddCircleFilled(this.NativePtr, center, radius, col, num_segments);
		}

		public unsafe void AddConvexPolyFilled(ref Vector2 points, int num_points, uint col)
		{
			fixed (Vector2* ptr = &points)
			{
				Vector2* ptr2 = ptr;
				ImGuiNative.ImDrawList_AddConvexPolyFilled(this.NativePtr, ptr2, num_points, col);
			}
		}

		public void AddDrawCmd()
		{
			ImGuiNative.ImDrawList_AddDrawCmd(this.NativePtr);
		}

		public void AddImage(IntPtr user_texture_id, Vector2 p_min, Vector2 p_max)
		{
			Vector2 vector = default(Vector2);
			Vector2 vector2 = new Vector2(1f, 1f);
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImage(this.NativePtr, user_texture_id, p_min, p_max, vector, vector2, maxValue);
		}

		public void AddImage(IntPtr user_texture_id, Vector2 p_min, Vector2 p_max, Vector2 uv_min)
		{
			Vector2 vector = new Vector2(1f, 1f);
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImage(this.NativePtr, user_texture_id, p_min, p_max, uv_min, vector, maxValue);
		}

		public void AddImage(IntPtr user_texture_id, Vector2 p_min, Vector2 p_max, Vector2 uv_min, Vector2 uv_max)
		{
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImage(this.NativePtr, user_texture_id, p_min, p_max, uv_min, uv_max, maxValue);
		}

		public void AddImage(IntPtr user_texture_id, Vector2 p_min, Vector2 p_max, Vector2 uv_min, Vector2 uv_max, uint col)
		{
			ImGuiNative.ImDrawList_AddImage(this.NativePtr, user_texture_id, p_min, p_max, uv_min, uv_max, col);
		}

		public void AddImageQuad(IntPtr user_texture_id, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
		{
			Vector2 vector = default(Vector2);
			Vector2 vector2 = new Vector2(1f, 0f);
			Vector2 vector3 = new Vector2(1f, 1f);
			Vector2 vector4 = new Vector2(0f, 1f);
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImageQuad(this.NativePtr, user_texture_id, p1, p2, p3, p4, vector, vector2, vector3, vector4, maxValue);
		}

		public void AddImageQuad(IntPtr user_texture_id, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 uv1)
		{
			Vector2 vector = new Vector2(1f, 0f);
			Vector2 vector2 = new Vector2(1f, 1f);
			Vector2 vector3 = new Vector2(0f, 1f);
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImageQuad(this.NativePtr, user_texture_id, p1, p2, p3, p4, uv1, vector, vector2, vector3, maxValue);
		}

		public void AddImageQuad(IntPtr user_texture_id, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 uv1, Vector2 uv2)
		{
			Vector2 vector = new Vector2(1f, 1f);
			Vector2 vector2 = new Vector2(0f, 1f);
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImageQuad(this.NativePtr, user_texture_id, p1, p2, p3, p4, uv1, uv2, vector, vector2, maxValue);
		}

		public void AddImageQuad(IntPtr user_texture_id, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			Vector2 vector = new Vector2(0f, 1f);
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImageQuad(this.NativePtr, user_texture_id, p1, p2, p3, p4, uv1, uv2, uv3, vector, maxValue);
		}

		public void AddImageQuad(IntPtr user_texture_id, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
		{
			uint maxValue = uint.MaxValue;
			ImGuiNative.ImDrawList_AddImageQuad(this.NativePtr, user_texture_id, p1, p2, p3, p4, uv1, uv2, uv3, uv4, maxValue);
		}

		public void AddImageQuad(IntPtr user_texture_id, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4, uint col)
		{
			ImGuiNative.ImDrawList_AddImageQuad(this.NativePtr, user_texture_id, p1, p2, p3, p4, uv1, uv2, uv3, uv4, col);
		}

		public void AddImageRounded(IntPtr user_texture_id, Vector2 p_min, Vector2 p_max, Vector2 uv_min, Vector2 uv_max, uint col, float rounding)
		{
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			ImGuiNative.ImDrawList_AddImageRounded(this.NativePtr, user_texture_id, p_min, p_max, uv_min, uv_max, col, rounding, imDrawFlags);
		}

		public void AddImageRounded(IntPtr user_texture_id, Vector2 p_min, Vector2 p_max, Vector2 uv_min, Vector2 uv_max, uint col, float rounding, ImDrawFlags flags)
		{
			ImGuiNative.ImDrawList_AddImageRounded(this.NativePtr, user_texture_id, p_min, p_max, uv_min, uv_max, col, rounding, flags);
		}

		public void AddLine(Vector2 p1, Vector2 p2, uint col)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_AddLine(this.NativePtr, p1, p2, col, num);
		}

		public void AddLine(Vector2 p1, Vector2 p2, uint col, float thickness)
		{
			ImGuiNative.ImDrawList_AddLine(this.NativePtr, p1, p2, col, thickness);
		}

		public void AddNgon(Vector2 center, float radius, uint col, int num_segments)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_AddNgon(this.NativePtr, center, radius, col, num_segments, num);
		}

		public void AddNgon(Vector2 center, float radius, uint col, int num_segments, float thickness)
		{
			ImGuiNative.ImDrawList_AddNgon(this.NativePtr, center, radius, col, num_segments, thickness);
		}

		public void AddNgonFilled(Vector2 center, float radius, uint col, int num_segments)
		{
			ImGuiNative.ImDrawList_AddNgonFilled(this.NativePtr, center, radius, col, num_segments);
		}

		public unsafe void AddPolyline(ref Vector2 points, int num_points, uint col, ImDrawFlags flags, float thickness)
		{
			fixed (Vector2* ptr = &points)
			{
				Vector2* ptr2 = ptr;
				ImGuiNative.ImDrawList_AddPolyline(this.NativePtr, ptr2, num_points, col, flags, thickness);
			}
		}

		public void AddQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, uint col)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_AddQuad(this.NativePtr, p1, p2, p3, p4, col, num);
		}

		public void AddQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, uint col, float thickness)
		{
			ImGuiNative.ImDrawList_AddQuad(this.NativePtr, p1, p2, p3, p4, col, thickness);
		}

		public void AddQuadFilled(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, uint col)
		{
			ImGuiNative.ImDrawList_AddQuadFilled(this.NativePtr, p1, p2, p3, p4, col);
		}

		public void AddRect(Vector2 p_min, Vector2 p_max, uint col)
		{
			float num = 0f;
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			float num2 = 1f;
			ImGuiNative.ImDrawList_AddRect(this.NativePtr, p_min, p_max, col, num, imDrawFlags, num2);
		}

		public void AddRect(Vector2 p_min, Vector2 p_max, uint col, float rounding)
		{
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			float num = 1f;
			ImGuiNative.ImDrawList_AddRect(this.NativePtr, p_min, p_max, col, rounding, imDrawFlags, num);
		}

		public void AddRect(Vector2 p_min, Vector2 p_max, uint col, float rounding, ImDrawFlags flags)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_AddRect(this.NativePtr, p_min, p_max, col, rounding, flags, num);
		}

		public void AddRect(Vector2 p_min, Vector2 p_max, uint col, float rounding, ImDrawFlags flags, float thickness)
		{
			ImGuiNative.ImDrawList_AddRect(this.NativePtr, p_min, p_max, col, rounding, flags, thickness);
		}

		public void AddRectFilled(Vector2 p_min, Vector2 p_max, uint col)
		{
			float num = 0f;
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			ImGuiNative.ImDrawList_AddRectFilled(this.NativePtr, p_min, p_max, col, num, imDrawFlags);
		}

		public void AddRectFilled(Vector2 p_min, Vector2 p_max, uint col, float rounding)
		{
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			ImGuiNative.ImDrawList_AddRectFilled(this.NativePtr, p_min, p_max, col, rounding, imDrawFlags);
		}

		public void AddRectFilled(Vector2 p_min, Vector2 p_max, uint col, float rounding, ImDrawFlags flags)
		{
			ImGuiNative.ImDrawList_AddRectFilled(this.NativePtr, p_min, p_max, col, rounding, flags);
		}

		public void AddRectFilledMultiColor(Vector2 p_min, Vector2 p_max, uint col_upr_left, uint col_upr_right, uint col_bot_right, uint col_bot_left)
		{
			ImGuiNative.ImDrawList_AddRectFilledMultiColor(this.NativePtr, p_min, p_max, col_upr_left, col_upr_right, col_bot_right, col_bot_left);
		}

		public void AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3, uint col)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_AddTriangle(this.NativePtr, p1, p2, p3, col, num);
		}

		public void AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3, uint col, float thickness)
		{
			ImGuiNative.ImDrawList_AddTriangle(this.NativePtr, p1, p2, p3, col, thickness);
		}

		public void AddTriangleFilled(Vector2 p1, Vector2 p2, Vector2 p3, uint col)
		{
			ImGuiNative.ImDrawList_AddTriangleFilled(this.NativePtr, p1, p2, p3, col);
		}

		public void ChannelsMerge()
		{
			ImGuiNative.ImDrawList_ChannelsMerge(this.NativePtr);
		}

		public void ChannelsSetCurrent(int n)
		{
			ImGuiNative.ImDrawList_ChannelsSetCurrent(this.NativePtr, n);
		}

		public void ChannelsSplit(int count)
		{
			ImGuiNative.ImDrawList_ChannelsSplit(this.NativePtr, count);
		}

		public ImDrawListPtr CloneOutput()
		{
			return new ImDrawListPtr(ImGuiNative.ImDrawList_CloneOutput(this.NativePtr));
		}

		public void Destroy()
		{
			ImGuiNative.ImDrawList_destroy(this.NativePtr);
		}

		public unsafe Vector2 GetClipRectMax()
		{
			Vector2 vector;
			ImGuiNative.ImDrawList_GetClipRectMax(&vector, this.NativePtr);
			return vector;
		}

		public unsafe Vector2 GetClipRectMin()
		{
			Vector2 vector;
			ImGuiNative.ImDrawList_GetClipRectMin(&vector, this.NativePtr);
			return vector;
		}

		public void PathArcTo(Vector2 center, float radius, float a_min, float a_max)
		{
			int num = 0;
			ImGuiNative.ImDrawList_PathArcTo(this.NativePtr, center, radius, a_min, a_max, num);
		}

		public void PathArcTo(Vector2 center, float radius, float a_min, float a_max, int num_segments)
		{
			ImGuiNative.ImDrawList_PathArcTo(this.NativePtr, center, radius, a_min, a_max, num_segments);
		}

		public void PathArcToFast(Vector2 center, float radius, int a_min_of_12, int a_max_of_12)
		{
			ImGuiNative.ImDrawList_PathArcToFast(this.NativePtr, center, radius, a_min_of_12, a_max_of_12);
		}

		public void PathBezierCubicCurveTo(Vector2 p2, Vector2 p3, Vector2 p4)
		{
			int num = 0;
			ImGuiNative.ImDrawList_PathBezierCubicCurveTo(this.NativePtr, p2, p3, p4, num);
		}

		public void PathBezierCubicCurveTo(Vector2 p2, Vector2 p3, Vector2 p4, int num_segments)
		{
			ImGuiNative.ImDrawList_PathBezierCubicCurveTo(this.NativePtr, p2, p3, p4, num_segments);
		}

		public void PathBezierQuadraticCurveTo(Vector2 p2, Vector2 p3)
		{
			int num = 0;
			ImGuiNative.ImDrawList_PathBezierQuadraticCurveTo(this.NativePtr, p2, p3, num);
		}

		public void PathBezierQuadraticCurveTo(Vector2 p2, Vector2 p3, int num_segments)
		{
			ImGuiNative.ImDrawList_PathBezierQuadraticCurveTo(this.NativePtr, p2, p3, num_segments);
		}

		public void PathClear()
		{
			ImGuiNative.ImDrawList_PathClear(this.NativePtr);
		}

		public void PathFillConvex(uint col)
		{
			ImGuiNative.ImDrawList_PathFillConvex(this.NativePtr, col);
		}

		public void PathLineTo(Vector2 pos)
		{
			ImGuiNative.ImDrawList_PathLineTo(this.NativePtr, pos);
		}

		public void PathLineToMergeDuplicate(Vector2 pos)
		{
			ImGuiNative.ImDrawList_PathLineToMergeDuplicate(this.NativePtr, pos);
		}

		public void PathRect(Vector2 rect_min, Vector2 rect_max)
		{
			float num = 0f;
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			ImGuiNative.ImDrawList_PathRect(this.NativePtr, rect_min, rect_max, num, imDrawFlags);
		}

		public void PathRect(Vector2 rect_min, Vector2 rect_max, float rounding)
		{
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			ImGuiNative.ImDrawList_PathRect(this.NativePtr, rect_min, rect_max, rounding, imDrawFlags);
		}

		public void PathRect(Vector2 rect_min, Vector2 rect_max, float rounding, ImDrawFlags flags)
		{
			ImGuiNative.ImDrawList_PathRect(this.NativePtr, rect_min, rect_max, rounding, flags);
		}

		public void PathStroke(uint col)
		{
			ImDrawFlags imDrawFlags = ImDrawFlags.None;
			float num = 1f;
			ImGuiNative.ImDrawList_PathStroke(this.NativePtr, col, imDrawFlags, num);
		}

		public void PathStroke(uint col, ImDrawFlags flags)
		{
			float num = 1f;
			ImGuiNative.ImDrawList_PathStroke(this.NativePtr, col, flags, num);
		}

		public void PathStroke(uint col, ImDrawFlags flags, float thickness)
		{
			ImGuiNative.ImDrawList_PathStroke(this.NativePtr, col, flags, thickness);
		}

		public void PopClipRect()
		{
			ImGuiNative.ImDrawList_PopClipRect(this.NativePtr);
		}

		public void PopTextureID()
		{
			ImGuiNative.ImDrawList_PopTextureID(this.NativePtr);
		}

		public void PrimQuadUV(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 uv_a, Vector2 uv_b, Vector2 uv_c, Vector2 uv_d, uint col)
		{
			ImGuiNative.ImDrawList_PrimQuadUV(this.NativePtr, a, b, c, d, uv_a, uv_b, uv_c, uv_d, col);
		}

		public void PrimRect(Vector2 a, Vector2 b, uint col)
		{
			ImGuiNative.ImDrawList_PrimRect(this.NativePtr, a, b, col);
		}

		public void PrimRectUV(Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, uint col)
		{
			ImGuiNative.ImDrawList_PrimRectUV(this.NativePtr, a, b, uv_a, uv_b, col);
		}

		public void PrimReserve(int idx_count, int vtx_count)
		{
			ImGuiNative.ImDrawList_PrimReserve(this.NativePtr, idx_count, vtx_count);
		}

		public void PrimUnreserve(int idx_count, int vtx_count)
		{
			ImGuiNative.ImDrawList_PrimUnreserve(this.NativePtr, idx_count, vtx_count);
		}

		public void PrimVtx(Vector2 pos, Vector2 uv, uint col)
		{
			ImGuiNative.ImDrawList_PrimVtx(this.NativePtr, pos, uv, col);
		}

		public void PrimWriteIdx(ushort idx)
		{
			ImGuiNative.ImDrawList_PrimWriteIdx(this.NativePtr, idx);
		}

		public void PrimWriteVtx(Vector2 pos, Vector2 uv, uint col)
		{
			ImGuiNative.ImDrawList_PrimWriteVtx(this.NativePtr, pos, uv, col);
		}

		public void PushClipRect(Vector2 clip_rect_min, Vector2 clip_rect_max)
		{
			byte b = 0;
			ImGuiNative.ImDrawList_PushClipRect(this.NativePtr, clip_rect_min, clip_rect_max, b);
		}

		public void PushClipRect(Vector2 clip_rect_min, Vector2 clip_rect_max, bool intersect_with_current_clip_rect)
		{
			byte b = (intersect_with_current_clip_rect ? 1 : 0);
			ImGuiNative.ImDrawList_PushClipRect(this.NativePtr, clip_rect_min, clip_rect_max, b);
		}

		public void PushClipRectFullScreen()
		{
			ImGuiNative.ImDrawList_PushClipRectFullScreen(this.NativePtr);
		}

		public void PushTextureID(IntPtr texture_id)
		{
			ImGuiNative.ImDrawList_PushTextureID(this.NativePtr, texture_id);
		}

		public unsafe void AddText(Vector2 pos, uint col, string text_begin)
		{
			int byteCount = Encoding.UTF8.GetByteCount(text_begin);
			byte* ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			fixed (string text = text_begin)
			{
				char* ptr2 = text;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				int bytes = Encoding.UTF8.GetBytes(ptr2, text_begin.Length, ptr, byteCount);
				ptr[bytes] = 0;
			}
			byte* ptr3 = null;
			ImGuiNative.ImDrawList_AddTextVec2(this.NativePtr, pos, col, ptr, ptr3);
		}

		public unsafe void AddText(ImFontPtr font, float font_size, Vector2 pos, uint col, string text_begin)
		{
			ImFont* nativePtr = font.NativePtr;
			int byteCount = Encoding.UTF8.GetByteCount(text_begin);
			byte* ptr = stackalloc byte[(UIntPtr)(byteCount + 1)];
			fixed (string text = text_begin)
			{
				char* ptr2 = text;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				int bytes = Encoding.UTF8.GetBytes(ptr2, text_begin.Length, ptr, byteCount);
				ptr[bytes] = 0;
			}
			byte* ptr3 = null;
			float num = 0f;
			Vector4* ptr4 = null;
			ImGuiNative.ImDrawList_AddTextFontPtr(this.NativePtr, nativePtr, font_size, pos, col, ptr, ptr3, num, ptr4);
		}
	}
}
