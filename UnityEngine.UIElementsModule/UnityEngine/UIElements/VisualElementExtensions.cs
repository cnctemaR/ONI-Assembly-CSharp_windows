using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace UnityEngine.UIElements
{
	public static class VisualElementExtensions
	{
		public static void StretchToParentSize(this VisualElement elem)
		{
			bool flag = elem == null;
			if (flag)
			{
				throw new ArgumentNullException("elem");
			}
			IStyle style = elem.style;
			style.position = Position.Absolute;
			style.left = 0f;
			style.top = 0f;
			style.right = 0f;
			style.bottom = 0f;
		}

		public static void StretchToParentWidth(this VisualElement elem)
		{
			bool flag = elem == null;
			if (flag)
			{
				throw new ArgumentNullException("elem");
			}
			IStyle style = elem.style;
			style.position = Position.Absolute;
			style.left = 0f;
			style.right = 0f;
		}

		public static void AddManipulator(this VisualElement ele, IManipulator manipulator)
		{
			bool flag = manipulator != null;
			if (flag)
			{
				manipulator.target = ele;
			}
		}

		public static void RemoveManipulator(this VisualElement ele, IManipulator manipulator)
		{
			bool flag = manipulator != null;
			if (flag)
			{
				manipulator.target = null;
			}
		}

		public static Vector2 WorldToLocal(this VisualElement ele, Vector2 p)
		{
			bool flag = ele == null;
			if (flag)
			{
				throw new ArgumentNullException("ele");
			}
			return VisualElement.MultiplyMatrix44Point2(ele.worldTransformInverse, p);
		}

		internal static Vector3 WorldToLocal3D(this VisualElement ele, Vector3 p)
		{
			bool flag = ele == null;
			if (flag)
			{
				throw new ArgumentNullException("ele");
			}
			return ele.worldTransformInverse.MultiplyPoint3x4(p);
		}

		public static Vector2 LocalToWorld(this VisualElement ele, Vector2 p)
		{
			bool flag = ele == null;
			if (flag)
			{
				throw new ArgumentNullException("ele");
			}
			return VisualElement.MultiplyMatrix44Point2(ele.worldTransformRef, p);
		}

		internal static Vector3 LocalToWorld3D(this VisualElement ele, Vector3 p)
		{
			bool flag = ele == null;
			if (flag)
			{
				throw new ArgumentNullException("ele");
			}
			return ele.worldTransformRef.MultiplyPoint3x4(p);
		}

		public static Rect WorldToLocal(this VisualElement ele, Rect r)
		{
			bool flag = ele == null;
			if (flag)
			{
				throw new ArgumentNullException("ele");
			}
			return VisualElement.CalculateConservativeRect(ele.worldTransformInverse, r);
		}

		public static Rect LocalToWorld(this VisualElement ele, Rect r)
		{
			bool flag = ele == null;
			if (flag)
			{
				throw new ArgumentNullException("ele");
			}
			return VisualElement.CalculateConservativeRect(ele.worldTransformRef, r);
		}

		internal static Ray LocalToWorld([NotNull] this VisualElement ele, Ray r)
		{
			ref Matrix4x4 worldTransformRef = ref ele.worldTransformRef;
			return new Ray(worldTransformRef.MultiplyPoint3x4(r.origin), worldTransformRef.MultiplyVector(r.direction));
		}

		internal static Ray WorldToLocal([NotNull] this VisualElement ele, Ray r)
		{
			ref Matrix4x4 worldTransformInverse = ref ele.worldTransformInverse;
			return new Ray(worldTransformInverse.MultiplyPoint3x4(r.origin), worldTransformInverse.MultiplyVector(r.direction));
		}

		public static Vector2 ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Vector2 point)
		{
			bool flag = src == null;
			if (flag)
			{
				throw new ArgumentNullException("src");
			}
			bool flag2 = dest == null;
			if (flag2)
			{
				throw new ArgumentNullException("dest");
			}
			BaseVisualElementPanel elementPanel = src.elementPanel;
			return (elementPanel != null && !elementPanel.isFlat) ? src.ChangeCoordinatesTo_3D(dest, point) : src.ChangeCoordinatesTo_2D(dest, point);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 ChangeCoordinatesTo_2D([NotNull] this VisualElement src, [NotNull] VisualElement dest, Vector2 point)
		{
			Vector2 vector = VisualElement.MultiplyMatrix44Point2(src.worldTransformRef, point);
			return VisualElement.MultiplyMatrix44Point2(dest.worldTransformInverse, vector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 ChangeCoordinatesTo_3D([NotNull] this VisualElement src, [NotNull] VisualElement dest, Vector2 point)
		{
			Vector3 vector = VisualElement.MultiplyMatrix44Point2ToPoint3(src.worldTransformRef, point);
			return VisualElement.MultiplyMatrix44Point3ToPoint2(dest.worldTransformInverse, vector);
		}

		public static Rect ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Rect rect)
		{
			bool flag = src == null;
			if (flag)
			{
				throw new ArgumentNullException("src");
			}
			bool flag2 = dest == null;
			if (flag2)
			{
				throw new ArgumentNullException("dest");
			}
			bool flag3 = float.IsNaN(rect.height) || float.IsNaN(rect.width) || float.IsNaN(rect.x) || float.IsNaN(rect.y);
			Rect rect2;
			if (flag3)
			{
				rect2 = new Rect(float.NaN, float.NaN, float.NaN, float.NaN);
			}
			else
			{
				Vector2 vector = new Vector2(rect.xMin, rect.yMin);
				Vector2 vector2 = new Vector2(rect.xMax, rect.yMax);
				Vector2 vector3 = new Vector2(rect.xMax, rect.yMin);
				Vector2 vector4 = new Vector2(rect.xMin, rect.yMax);
				BaseVisualElementPanel elementPanel = src.elementPanel;
				bool flag4 = elementPanel != null && !elementPanel.isFlat;
				Vector2 vector5;
				Vector2 vector6;
				Vector2 vector7;
				Vector2 vector8;
				if (flag4)
				{
					vector5 = src.ChangeCoordinatesTo_3D(dest, vector);
					vector6 = src.ChangeCoordinatesTo_3D(dest, vector3);
					vector7 = src.ChangeCoordinatesTo_3D(dest, vector4);
					vector8 = src.ChangeCoordinatesTo_3D(dest, vector2);
				}
				else
				{
					vector5 = src.ChangeCoordinatesTo_2D(dest, vector);
					vector6 = src.ChangeCoordinatesTo_2D(dest, vector3);
					vector7 = src.ChangeCoordinatesTo_2D(dest, vector4);
					vector8 = src.ChangeCoordinatesTo_2D(dest, vector2);
				}
				Vector2 vector9 = new Vector2(VisualElement.Min(vector5.x, vector6.x, vector7.x, vector8.x), VisualElement.Min(vector5.y, vector6.y, vector7.y, vector8.y));
				Vector2 vector10 = new Vector2(VisualElement.Max(vector5.x, vector6.x, vector7.x, vector8.x), VisualElement.Max(vector5.y, vector6.y, vector7.y, vector8.y));
				rect2 = new Rect(vector9.x, vector9.y, vector10.x - vector9.x, vector10.y - vector9.y);
			}
			return rect2;
		}

		internal static Ray ChangeCoordinatesTo([NotNull] this VisualElement src, [NotNull] VisualElement dest, Ray ray)
		{
			return dest.WorldToLocal(src.LocalToWorld(ray));
		}

		internal static bool IntersectWorldRay([NotNull] this VisualElement ve, Ray worldRay, out float distance, out Vector3 localPoint)
		{
			ref Matrix4x4 worldTransformInverse = ref ve.worldTransformInverse;
			Ray ray = new Ray(worldTransformInverse.MultiplyPoint3x4(worldRay.origin), worldTransformInverse.MultiplyVector(worldRay.direction));
			bool flag = ve.IntersectLocalRay(ray, out localPoint);
			Vector3 vector = ve.worldTransformRef.MultiplyPoint3x4(localPoint);
			distance = Vector3.Distance(worldRay.origin, vector);
			return flag;
		}

		internal static bool IntersectLocalRay([NotNull] this VisualElement ve, Ray localRay, out Vector3 localPoint)
		{
			float num = -localRay.origin.z / localRay.direction.z;
			localPoint = localRay.origin + localRay.direction * num;
			bool flag = num <= 0f;
			return !flag && ve.rect.Contains(localPoint);
		}

		internal static Ray TransformRay(this Matrix4x4 m, Ray ray)
		{
			return new Ray(m.MultiplyPoint3x4(ray.origin), m.MultiplyVector(ray.direction));
		}
	}
}
