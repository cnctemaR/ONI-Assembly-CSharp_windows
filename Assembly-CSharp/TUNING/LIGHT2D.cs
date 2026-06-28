using System;
using UnityEngine;

namespace TUNING
{
	public class LIGHT2D
	{
		public const float FLOORLAMP_INTENSITY = 4f;

		public const float FLOORLAMP_RANGE = 4f;

		public const float FLOORLAMP_ANGLE = 0f;

		public const LightShape FLOORLAMP_SHAPE = LightShape.Circle;

		public const float CEILINGLIGHT_INTENSITY = 4f;

		public const float CEILINGLIGHT_RANGE = 8f;

		public const float CEILINGLIGHT_ANGLE = 2.6f;

		public const LightShape CEILINGLIGHT_SHAPE = LightShape.Cone;

		public const float HEADQUARTERS_INTENSITY = 4f;

		public const float HEADQUARTERS_RANGE = 5f;

		public const LightShape HEADQUARTERS_SHAPE = LightShape.Circle;

		public const float WALLLIGHT_INTENSITY = 4f;

		public const float WALLLIGHT_RANGE = 4f;

		public const float WALLLIGHT_ANGLE = 0f;

		public const LightShape WALLLIGHT_SHAPE = LightShape.Circle;

		private static Color YELLOWLIGHT = new Color(0.57f, 0.55f, 0.44f, 1f);

		private static Color OVERLAYLIGHT = new Color(0.56f, 0.56f, 0.56f, 1f);

		private static Vector2 DEFAULTDIRECTION = new Vector2(0f, -1f);

		public static readonly Color FLOORLAMP_COLOR = LIGHT2D.YELLOWLIGHT;

		public static readonly Color FLOORLAMP_OVERLAYCOLOR = LIGHT2D.OVERLAYLIGHT;

		public static readonly Vector2 FLOORLAMP_OFFSET = new Vector2(0.05f, 1.5f);

		public static readonly Vector2 FLOORLAMP_DIRECTION = LIGHT2D.DEFAULTDIRECTION;

		public static readonly Color CEILINGLIGHT_COLOR = LIGHT2D.YELLOWLIGHT;

		public static readonly Color CEILINGLIGHT_OVERLAYCOLOR = LIGHT2D.OVERLAYLIGHT;

		public static readonly Vector2 CEILINGLIGHT_OFFSET = new Vector2(0.05f, 0.65f);

		public static readonly Vector2 CEILINGLIGHT_DIRECTION = LIGHT2D.DEFAULTDIRECTION;

		public static readonly Color LIGHT_PREVIEW_COLOR = LIGHT2D.YELLOWLIGHT;

		public static readonly Color HEADQUARTERS_COLOR = LIGHT2D.YELLOWLIGHT;

		public static readonly Color HEADQUARTERS_OVERLAYCOLOR = LIGHT2D.OVERLAYLIGHT;

		public static readonly Vector2 HEADQUARTERS_OFFSET = new Vector2(0.5f, 3f);

		public static readonly Color WALLLIGHT_COLOR = LIGHT2D.YELLOWLIGHT;

		public static readonly Color WALLLIGHT_OVERLAYCOLOR = LIGHT2D.OVERLAYLIGHT;

		public static readonly Vector2 WALLLIGHT_OFFSET = new Vector2(0f, 0.5f);

		public static readonly Vector2 WALLLIGHT_DIRECTION = LIGHT2D.DEFAULTDIRECTION;
	}
}
