using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Transform/RectTransform.h")]
	[NativeClass("UI::RectTransform")]
	public sealed class RectTransform : Transform
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event RectTransform.ReapplyDrivenProperties reapplyDrivenProperties;

		public Rect rect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rect rect;
				RectTransform.get_rect_Injected(intPtr, out rect);
				return rect;
			}
		}

		public Vector2 anchorMin
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				RectTransform.get_anchorMin_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_anchorMin_Injected(intPtr, ref value);
			}
		}

		public Vector2 anchorMax
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				RectTransform.get_anchorMax_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_anchorMax_Injected(intPtr, ref value);
			}
		}

		public Vector2 anchoredPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				RectTransform.get_anchoredPosition_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_anchoredPosition_Injected(intPtr, ref value);
			}
		}

		public Vector2 sizeDelta
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				RectTransform.get_sizeDelta_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_sizeDelta_Injected(intPtr, ref value);
			}
		}

		public Vector2 pivot
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				RectTransform.get_pivot_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_pivot_Injected(intPtr, ref value);
			}
		}

		public Vector3 anchoredPosition3D
		{
			get
			{
				Vector2 anchoredPosition = this.anchoredPosition;
				return new Vector3(anchoredPosition.x, anchoredPosition.y, base.localPosition.z);
			}
			set
			{
				this.anchoredPosition = new Vector2(value.x, value.y);
				Vector3 localPosition = base.localPosition;
				localPosition.z = value.z;
				base.localPosition = localPosition;
			}
		}

		public Vector2 offsetMin
		{
			get
			{
				return this.anchoredPosition - Vector2.Scale(this.sizeDelta, this.pivot);
			}
			set
			{
				Vector2 vector = value - (this.anchoredPosition - Vector2.Scale(this.sizeDelta, this.pivot));
				this.sizeDelta -= vector;
				this.anchoredPosition += Vector2.Scale(vector, Vector2.one - this.pivot);
			}
		}

		public Vector2 offsetMax
		{
			get
			{
				return this.anchoredPosition + Vector2.Scale(this.sizeDelta, Vector2.one - this.pivot);
			}
			set
			{
				Vector2 vector = value - (this.anchoredPosition + Vector2.Scale(this.sizeDelta, Vector2.one - this.pivot));
				this.sizeDelta += vector;
				this.anchoredPosition += Vector2.Scale(vector, this.pivot);
			}
		}

		public Object drivenByObject
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Object>(RectTransform.get_drivenByObject_Injected(intPtr));
			}
			internal set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_drivenByObject_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(value));
			}
		}

		internal DrivenTransformProperties drivenProperties
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectTransform.get_drivenProperties_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_drivenProperties_Injected(intPtr, value);
			}
		}

		public bool sendChildDimensionsChange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RectTransform.get_sendChildDimensionsChange_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RectTransform.set_sendChildDimensionsChange_Injected(intPtr, value);
			}
		}

		[NativeMethod("UpdateIfTransformDispatchIsDirty")]
		public void ForceUpdateRectTransforms()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RectTransform>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RectTransform.ForceUpdateRectTransforms_Injected(intPtr);
		}

		public void GetLocalCorners(Vector3[] fourCornersArray)
		{
			bool flag = fourCornersArray == null || fourCornersArray.Length < 4;
			if (flag)
			{
				Debug.LogError("Calling GetLocalCorners with an array that is null or has less than 4 elements.");
			}
			else
			{
				Rect rect = this.rect;
				float x = rect.x;
				float y = rect.y;
				float xMax = rect.xMax;
				float yMax = rect.yMax;
				fourCornersArray[0] = new Vector3(x, y, 0f);
				fourCornersArray[1] = new Vector3(x, yMax, 0f);
				fourCornersArray[2] = new Vector3(xMax, yMax, 0f);
				fourCornersArray[3] = new Vector3(xMax, y, 0f);
			}
		}

		public void GetWorldCorners(Vector3[] fourCornersArray)
		{
			bool flag = fourCornersArray == null || fourCornersArray.Length < 4;
			if (flag)
			{
				Debug.LogError("Calling GetWorldCorners with an array that is null or has less than 4 elements.");
			}
			else
			{
				this.GetLocalCorners(fourCornersArray);
				Matrix4x4 localToWorldMatrix = base.localToWorldMatrix;
				for (int i = 0; i < 4; i++)
				{
					fourCornersArray[i] = localToWorldMatrix.MultiplyPoint(fourCornersArray[i]);
				}
			}
		}

		public void SetInsetAndSizeFromParentEdge(RectTransform.Edge edge, float inset, float size)
		{
			int num = ((edge == RectTransform.Edge.Top || edge == RectTransform.Edge.Bottom) ? 1 : 0);
			bool flag = edge == RectTransform.Edge.Top || edge == RectTransform.Edge.Right;
			float num2 = (float)(flag ? 1 : 0);
			Vector2 vector = this.anchorMin;
			vector[num] = num2;
			this.anchorMin = vector;
			vector = this.anchorMax;
			vector[num] = num2;
			this.anchorMax = vector;
			Vector2 sizeDelta = this.sizeDelta;
			sizeDelta[num] = size;
			this.sizeDelta = sizeDelta;
			Vector2 anchoredPosition = this.anchoredPosition;
			anchoredPosition[num] = (flag ? (-inset - size * (1f - this.pivot[num])) : (inset + size * this.pivot[num]));
			this.anchoredPosition = anchoredPosition;
		}

		public void SetSizeWithCurrentAnchors(RectTransform.Axis axis, float size)
		{
			Vector2 sizeDelta = this.sizeDelta;
			sizeDelta[(int)axis] = size - this.GetParentSize()[(int)axis] * (this.anchorMax[(int)axis] - this.anchorMin[(int)axis]);
			this.sizeDelta = sizeDelta;
		}

		[RequiredByNativeCode]
		internal static void SendReapplyDrivenProperties(RectTransform driven)
		{
			RectTransform.ReapplyDrivenProperties reapplyDrivenProperties = RectTransform.reapplyDrivenProperties;
			if (reapplyDrivenProperties != null)
			{
				reapplyDrivenProperties(driven);
			}
		}

		internal Rect GetRectInParentSpace()
		{
			Rect rect = this.rect;
			Vector2 vector = this.offsetMin + Vector2.Scale(this.pivot, rect.size);
			bool flag = base.parent;
			if (flag)
			{
				RectTransform component = base.parent.GetComponent<RectTransform>();
				bool flag2 = component;
				if (flag2)
				{
					vector += Vector2.Scale(this.anchorMin, component.rect.size);
				}
			}
			rect.x += vector.x;
			rect.y += vector.y;
			return rect;
		}

		private Vector2 GetParentSize()
		{
			RectTransform rectTransform = base.parent as RectTransform;
			bool flag = !rectTransform;
			Vector2 vector;
			if (flag)
			{
				vector = Vector2.zero;
			}
			else
			{
				vector = rectTransform.rect.size;
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rect_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_anchorMin_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anchorMin_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_anchorMax_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anchorMax_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_anchoredPosition_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anchoredPosition_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sizeDelta_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sizeDelta_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pivot_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pivot_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_drivenByObject_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_drivenByObject_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DrivenTransformProperties get_drivenProperties_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_drivenProperties_Injected(IntPtr _unity_self, DrivenTransformProperties value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sendChildDimensionsChange_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sendChildDimensionsChange_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ForceUpdateRectTransforms_Injected(IntPtr _unity_self);

		public enum Edge
		{
			Left,
			Right,
			Top,
			Bottom
		}

		public enum Axis
		{
			Horizontal,
			Vertical
		}

		public delegate void ReapplyDrivenProperties(RectTransform driven);
	}
}
