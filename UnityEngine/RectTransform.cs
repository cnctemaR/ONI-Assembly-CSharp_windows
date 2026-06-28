using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeClass("UI::RectTransform")]
	public sealed class RectTransform : Transform
	{
		public Rect rect
		{
			get
			{
				Rect rect;
				this.INTERNAL_get_rect(out rect);
				return rect;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rect(out Rect value);

		public Vector2 anchorMin
		{
			get
			{
				Vector2 vector;
				this.INTERNAL_get_anchorMin(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_anchorMin(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchorMin(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchorMin(ref Vector2 value);

		public Vector2 anchorMax
		{
			get
			{
				Vector2 vector;
				this.INTERNAL_get_anchorMax(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_anchorMax(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchorMax(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchorMax(ref Vector2 value);

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

		public Vector2 anchoredPosition
		{
			get
			{
				Vector2 vector;
				this.INTERNAL_get_anchoredPosition(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_anchoredPosition(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchoredPosition(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchoredPosition(ref Vector2 value);

		public Vector2 sizeDelta
		{
			get
			{
				Vector2 vector;
				this.INTERNAL_get_sizeDelta(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_sizeDelta(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_sizeDelta(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_sizeDelta(ref Vector2 value);

		public Vector2 pivot
		{
			get
			{
				Vector2 vector;
				this.INTERNAL_get_pivot(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_pivot(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pivot(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_pivot(ref Vector2 value);

		internal extern Object drivenByObject
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern DrivenTransformProperties drivenProperties
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event RectTransform.ReapplyDrivenProperties reapplyDrivenProperties;

		[RequiredByNativeCode]
		internal static void SendReapplyDrivenProperties(RectTransform driven)
		{
			if (RectTransform.reapplyDrivenProperties != null)
			{
				RectTransform.reapplyDrivenProperties(driven);
			}
		}

		public void GetLocalCorners(Vector3[] fourCornersArray)
		{
			if (fourCornersArray == null || fourCornersArray.Length < 4)
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
			if (fourCornersArray == null || fourCornersArray.Length < 4)
			{
				Debug.LogError("Calling GetWorldCorners with an array that is null or has less than 4 elements.");
			}
			else
			{
				this.GetLocalCorners(fourCornersArray);
				Transform transform = base.transform;
				for (int i = 0; i < 4; i++)
				{
					fourCornersArray[i] = transform.TransformPoint(fourCornersArray[i]);
				}
			}
		}

		internal Rect GetRectInParentSpace()
		{
			Rect rect = this.rect;
			Vector2 vector = this.offsetMin + Vector2.Scale(this.pivot, rect.size);
			Transform parent = base.transform.parent;
			if (parent)
			{
				RectTransform component = parent.GetComponent<RectTransform>();
				if (component)
				{
					vector += Vector2.Scale(this.anchorMin, component.rect.size);
				}
			}
			rect.x += vector.x;
			rect.y += vector.y;
			return rect;
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

		public void SetInsetAndSizeFromParentEdge(RectTransform.Edge edge, float inset, float size)
		{
			int num = ((edge != RectTransform.Edge.Top && edge != RectTransform.Edge.Bottom) ? 0 : 1);
			bool flag = edge == RectTransform.Edge.Top || edge == RectTransform.Edge.Right;
			float num2 = (float)((!flag) ? 0 : 1);
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
			anchoredPosition[num] = ((!flag) ? (inset + size * this.pivot[num]) : (-inset - size * (1f - this.pivot[num])));
			this.anchoredPosition = anchoredPosition;
		}

		public void SetSizeWithCurrentAnchors(RectTransform.Axis axis, float size)
		{
			Vector2 sizeDelta = this.sizeDelta;
			sizeDelta[(int)axis] = size - this.GetParentSize()[(int)axis] * (this.anchorMax[(int)axis] - this.anchorMin[(int)axis]);
			this.sizeDelta = sizeDelta;
		}

		private Vector2 GetParentSize()
		{
			RectTransform rectTransform = base.parent as RectTransform;
			Vector2 vector;
			if (!rectTransform)
			{
				vector = Vector2.zero;
			}
			else
			{
				vector = rectTransform.rect.size;
			}
			return vector;
		}

		public delegate void ReapplyDrivenProperties(RectTransform driven);

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
	}
}
