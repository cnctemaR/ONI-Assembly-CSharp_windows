using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Utility class containing helper methods for working with  RectTransform.</para>
	/// </summary>
	[NativeHeader("Runtime/Transform/RectTransform.h")]
	[NativeHeader("Runtime/UI/RectTransformUtil.h")]
	[NativeHeader("Runtime/Camera/Camera.h")]
	[NativeHeader("Runtime/UI/Canvas.h")]
	[StaticAccessor("UI", StaticAccessorType.DoubleColon)]
	public sealed class RectTransformUtility
	{
		private RectTransformUtility()
		{
		}

		public static bool RectangleContainsScreenPoint(RectTransform rect, Vector2 screenPoint)
		{
			return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, null);
		}

		/// <summary>
		///   <para>Does the RectTransform contain the screen point as seen from the given camera?</para>
		/// </summary>
		/// <param name="rect">The RectTransform to test with.</param>
		/// <param name="screenPoint">The screen point to test.</param>
		/// <param name="cam">The camera from which the test is performed from. (Optional)</param>
		/// <returns>
		///   <para>True if the point is inside the rectangle.</para>
		/// </returns>
		public static bool RectangleContainsScreenPoint(RectTransform rect, Vector2 screenPoint, Camera cam)
		{
			return RectTransformUtility.PointInRectangle(screenPoint, rect, cam);
		}

		public static bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
		{
			worldPoint = Vector2.zero;
			Ray ray = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
			Plane plane = new Plane(rect.rotation * Vector3.back, rect.position);
			float num;
			bool flag;
			if (!plane.Raycast(ray, out num))
			{
				flag = false;
			}
			else
			{
				worldPoint = ray.GetPoint(num);
				flag = true;
			}
			return flag;
		}

		public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint)
		{
			localPoint = Vector2.zero;
			Vector3 vector;
			bool flag;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, out vector))
			{
				localPoint = rect.InverseTransformPoint(vector);
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static Ray ScreenPointToRay(Camera cam, Vector2 screenPos)
		{
			Ray ray;
			if (cam != null)
			{
				ray = cam.ScreenPointToRay(screenPos);
			}
			else
			{
				Vector3 vector = screenPos;
				vector.z -= 100f;
				ray = new Ray(vector, Vector3.forward);
			}
			return ray;
		}

		public static Vector2 WorldToScreenPoint(Camera cam, Vector3 worldPoint)
		{
			Vector2 vector;
			if (cam == null)
			{
				vector = new Vector2(worldPoint.x, worldPoint.y);
			}
			else
			{
				vector = cam.WorldToScreenPoint(worldPoint);
			}
			return vector;
		}

		public static Bounds CalculateRelativeRectTransformBounds(Transform root, Transform child)
		{
			RectTransform[] componentsInChildren = child.GetComponentsInChildren<RectTransform>(false);
			Bounds bounds2;
			if (componentsInChildren.Length > 0)
			{
				Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
				Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					componentsInChildren[i].GetWorldCorners(RectTransformUtility.s_Corners);
					for (int j = 0; j < 4; j++)
					{
						Vector3 vector3 = worldToLocalMatrix.MultiplyPoint3x4(RectTransformUtility.s_Corners[j]);
						vector = Vector3.Min(vector3, vector);
						vector2 = Vector3.Max(vector3, vector2);
					}
					i++;
				}
				Bounds bounds = new Bounds(vector, Vector3.zero);
				bounds.Encapsulate(vector2);
				bounds2 = bounds;
			}
			else
			{
				bounds2 = new Bounds(Vector3.zero, Vector3.zero);
			}
			return bounds2;
		}

		public static Bounds CalculateRelativeRectTransformBounds(Transform trans)
		{
			return RectTransformUtility.CalculateRelativeRectTransformBounds(trans, trans);
		}

		/// <summary>
		///   <para>Flips the alignment of the RectTransform along the horizontal or vertical axis, and optionally its children as well.</para>
		/// </summary>
		/// <param name="rect">The RectTransform to flip.</param>
		/// <param name="keepPositioning">Flips around the pivot if true. Flips within the parent rect if false.</param>
		/// <param name="recursive">Flip the children as well?</param>
		/// <param name="axis">The axis to flip along. 0 is horizontal and 1 is vertical.</param>
		public static void FlipLayoutOnAxis(RectTransform rect, int axis, bool keepPositioning, bool recursive)
		{
			if (!(rect == null))
			{
				if (recursive)
				{
					for (int i = 0; i < rect.childCount; i++)
					{
						RectTransform rectTransform = rect.GetChild(i) as RectTransform;
						if (rectTransform != null)
						{
							RectTransformUtility.FlipLayoutOnAxis(rectTransform, axis, false, true);
						}
					}
				}
				Vector2 pivot = rect.pivot;
				pivot[axis] = 1f - pivot[axis];
				rect.pivot = pivot;
				if (!keepPositioning)
				{
					Vector2 anchoredPosition = rect.anchoredPosition;
					anchoredPosition[axis] = -anchoredPosition[axis];
					rect.anchoredPosition = anchoredPosition;
					Vector2 anchorMin = rect.anchorMin;
					Vector2 anchorMax = rect.anchorMax;
					float num = anchorMin[axis];
					anchorMin[axis] = 1f - anchorMax[axis];
					anchorMax[axis] = 1f - num;
					rect.anchorMin = anchorMin;
					rect.anchorMax = anchorMax;
				}
			}
		}

		/// <summary>
		///   <para>Flips the horizontal and vertical axes of the RectTransform size and alignment, and optionally its children as well.</para>
		/// </summary>
		/// <param name="rect">The RectTransform to flip.</param>
		/// <param name="keepPositioning">Flips around the pivot if true. Flips within the parent rect if false.</param>
		/// <param name="recursive">Flip the children as well?</param>
		public static void FlipLayoutAxes(RectTransform rect, bool keepPositioning, bool recursive)
		{
			if (!(rect == null))
			{
				if (recursive)
				{
					for (int i = 0; i < rect.childCount; i++)
					{
						RectTransform rectTransform = rect.GetChild(i) as RectTransform;
						if (rectTransform != null)
						{
							RectTransformUtility.FlipLayoutAxes(rectTransform, false, true);
						}
					}
				}
				rect.pivot = RectTransformUtility.GetTransposed(rect.pivot);
				rect.sizeDelta = RectTransformUtility.GetTransposed(rect.sizeDelta);
				if (!keepPositioning)
				{
					rect.anchoredPosition = RectTransformUtility.GetTransposed(rect.anchoredPosition);
					rect.anchorMin = RectTransformUtility.GetTransposed(rect.anchorMin);
					rect.anchorMax = RectTransformUtility.GetTransposed(rect.anchorMax);
				}
			}
		}

		private static Vector2 GetTransposed(Vector2 input)
		{
			return new Vector2(input.y, input.x);
		}

		/// <summary>
		///   <para>Convert a given point in screen space into a pixel correct point.</para>
		/// </summary>
		/// <param name="point"></param>
		/// <param name="elementTransform"></param>
		/// <param name="canvas"></param>
		/// <returns>
		///   <para>Pixel adjusted point.</para>
		/// </returns>
		public static Vector2 PixelAdjustPoint(Vector2 point, Transform elementTransform, Canvas canvas)
		{
			Vector2 vector;
			RectTransformUtility.PixelAdjustPoint_Injected(ref point, elementTransform, canvas, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Given a rect transform, return the corner points in pixel accurate coordinates.</para>
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="canvas"></param>
		/// <returns>
		///   <para>Pixel adjusted rect.</para>
		/// </returns>
		public static Rect PixelAdjustRect(RectTransform rectTransform, Canvas canvas)
		{
			Rect rect;
			RectTransformUtility.PixelAdjustRect_Injected(rectTransform, canvas, out rect);
			return rect;
		}

		private static bool PointInRectangle(Vector2 screenPoint, RectTransform rect, Camera cam)
		{
			return RectTransformUtility.PointInRectangle_Injected(ref screenPoint, rect, cam);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PixelAdjustPoint_Injected(ref Vector2 point, Transform elementTransform, Canvas canvas, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PixelAdjustRect_Injected(RectTransform rectTransform, Canvas canvas, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool PointInRectangle_Injected(ref Vector2 screenPoint, RectTransform rect, Camera cam);

		private static readonly Vector3[] s_Corners = new Vector3[4];
	}
}
