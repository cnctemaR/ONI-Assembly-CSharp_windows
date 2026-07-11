using System;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>VisualElementExtensions is a set of extension methods useful for VisualElement.</para>
	/// </summary>
	public static class VisualElementExtensions
	{
		public static Vector2 WorldToLocal(this VisualElement ele, Vector2 p)
		{
			return ele.worldTransform.inverse.MultiplyPoint3x4(p);
		}

		public static Vector2 LocalToWorld(this VisualElement ele, Vector2 p)
		{
			return ele.worldTransform.MultiplyPoint3x4(p);
		}

		public static Rect WorldToLocal(this VisualElement ele, Rect r)
		{
			Matrix4x4 inverse = ele.worldTransform.inverse;
			Vector2 vector = inverse.MultiplyPoint3x4(r.position);
			r.position = vector;
			r.size = inverse.MultiplyVector(r.size);
			return r;
		}

		public static Rect LocalToWorld(this VisualElement ele, Rect r)
		{
			Matrix4x4 worldTransform = ele.worldTransform;
			r.position = worldTransform.MultiplyPoint3x4(r.position);
			r.size = worldTransform.MultiplyVector(r.size);
			return r;
		}

		public static Vector2 ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Vector2 point)
		{
			return dest.WorldToLocal(src.LocalToWorld(point));
		}

		public static Rect ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Rect rect)
		{
			return dest.WorldToLocal(src.LocalToWorld(rect));
		}

		public static void StretchToParentSize(this VisualElement elem)
		{
			IStyle style = elem.style;
			style.positionType = PositionType.Absolute;
			style.positionLeft = 0f;
			style.positionTop = 0f;
			style.positionRight = 0f;
			style.positionBottom = 0f;
		}

		/// <summary>
		///   <para>The given VisualElement's left and right edges will be aligned with the corresponding edges of the parent element.</para>
		/// </summary>
		/// <param name="elem"></param>
		public static void StretchToParentWidth(this VisualElement elem)
		{
			IStyle style = elem.style;
			style.positionType = PositionType.Absolute;
			style.positionLeft = 0f;
			style.positionRight = 0f;
		}

		/// <summary>
		///   <para>Add a manipulator associated to a VisualElement.</para>
		/// </summary>
		/// <param name="ele">VisualElement associated to the manipulator.</param>
		/// <param name="manipulator">Manipulator to be added to the VisualElement.</param>
		public static void AddManipulator(this VisualElement ele, IManipulator manipulator)
		{
			if (manipulator != null)
			{
				manipulator.target = ele;
			}
		}

		/// <summary>
		///   <para>Remove a manipulator associated to a VisualElement.</para>
		/// </summary>
		/// <param name="ele">VisualElement associated to the manipulator.</param>
		/// <param name="manipulator">Manipulator to be removed from the VisualElement.</param>
		public static void RemoveManipulator(this VisualElement ele, IManipulator manipulator)
		{
			if (manipulator != null)
			{
				manipulator.target = null;
			}
		}
	}
}
