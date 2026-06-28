using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Random
	{
		[Obsolete("Deprecated. Use InitState() function or Random.state property instead.")]
		public static extern int seed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InitState(int seed);

		public static Random.State state
		{
			get
			{
				Random.State state;
				Random.INTERNAL_get_state(out state);
				return state;
			}
			set
			{
				Random.INTERNAL_set_state(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_state(out Random.State value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_state(ref Random.State value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float Range(float min, float max);

		public static int Range(int min, int max)
		{
			return Random.RandomRangeInt(min, max);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RandomRangeInt(int min, int max);

		public static extern float value
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Vector3 insideUnitSphere
		{
			get
			{
				Vector3 vector;
				Random.INTERNAL_get_insideUnitSphere(out vector);
				return vector;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_insideUnitSphere(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRandomUnitCircle(out Vector2 output);

		public static Vector2 insideUnitCircle
		{
			get
			{
				Vector2 vector;
				Random.GetRandomUnitCircle(out vector);
				return vector;
			}
		}

		public static Vector3 onUnitSphere
		{
			get
			{
				Vector3 vector;
				Random.INTERNAL_get_onUnitSphere(out vector);
				return vector;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_onUnitSphere(out Vector3 value);

		public static Quaternion rotation
		{
			get
			{
				Quaternion quaternion;
				Random.INTERNAL_get_rotation(out quaternion);
				return quaternion;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rotation(out Quaternion value);

		public static Quaternion rotationUniform
		{
			get
			{
				Quaternion quaternion;
				Random.INTERNAL_get_rotationUniform(out quaternion);
				return quaternion;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rotationUniform(out Quaternion value);

		[Obsolete("Use Random.Range instead")]
		public static float RandomRange(float min, float max)
		{
			return Random.Range(min, max);
		}

		[Obsolete("Use Random.Range instead")]
		public static int RandomRange(int min, int max)
		{
			return Random.Range(min, max);
		}

		public static Color ColorHSV()
		{
			return Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax)
		{
			return Random.ColorHSV(hueMin, hueMax, 0f, 1f, 0f, 1f, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax)
		{
			return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, 0f, 1f, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax)
		{
			return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax)
		{
			float num = Mathf.Lerp(hueMin, hueMax, Random.value);
			float num2 = Mathf.Lerp(saturationMin, saturationMax, Random.value);
			float num3 = Mathf.Lerp(valueMin, valueMax, Random.value);
			Color color = Color.HSVToRGB(num, num2, num3, true);
			color.a = Mathf.Lerp(alphaMin, alphaMax, Random.value);
			return color;
		}

		[Serializable]
		public struct State
		{
			[SerializeField]
			private int s0;

			[SerializeField]
			private int s1;

			[SerializeField]
			private int s2;

			[SerializeField]
			private int s3;
		}
	}
}
