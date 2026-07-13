using System;
using UnityEngine;

namespace TUNING
{
	public class LIGHT2D
	{
		public const int SUNLIGHT_MAX_DEFAULT = 80000;

		public static readonly Color LIGHT_BLUE = new Color(0.38f, 0.61f, 1f, 1f);

		public static readonly Color LIGHT_PURPLE = new Color(0.9f, 0.4f, 0.74f, 1f);

		public static readonly Color LIGHT_PINK = new Color(0.9f, 0.4f, 0.6f, 1f);

		public static readonly Color LIGHT_YELLOW = new Color(0.57f, 0.55f, 0.44f, 1f);

		public static readonly Color LIGHT_OVERLAY = new Color(0.56f, 0.56f, 0.56f, 1f);

		public static readonly Vector2 DEFAULT_DIRECTION = new Vector2(0f, -1f);

		public const int FLOORLAMP_LUX = 1000;

		public const float FLOORLAMP_RANGE = 4f;

		public const float FLOORLAMP_ANGLE = 0f;

		public const global::LightShape FLOORLAMP_SHAPE = global::LightShape.Circle;

		public static readonly Color FLOORLAMP_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color FLOORLAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 FLOORLAMP_OFFSET = new Vector2(0.05f, 1.5f);

		public static readonly Vector2 FLOORLAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const float CEILINGLIGHT_RANGE = 8f;

		public const float CEILINGLIGHT_ANGLE = 2.6f;

		public const global::LightShape CEILINGLIGHT_SHAPE = global::LightShape.Cone;

		public static readonly Color CEILINGLIGHT_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color CEILINGLIGHT_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 CEILINGLIGHT_OFFSET = new Vector2(0.05f, 0.65f);

		public static readonly Vector2 CEILINGLIGHT_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const int CEILINGLIGHT_LUX = 1800;

		public const float FOSSILSCULPTURE_RANGE = 8f;

		public const float FOSSILSCULPTURE_CEILING_RANGE = 8f;

		public const float FOSSILSCULPTURE_ANGLE = 0f;

		public const float FOSSILSCULPTURE_CEILING_ANGLE = 2.6f;

		public const int FOSSILSCULPTURE_LIGHT_WIDTH = 3;

		public const DiscreteShadowCaster.Direction FOSSILSCULPTURE_LIGHT_DIRECTION = DiscreteShadowCaster.Direction.North;

		public const DiscreteShadowCaster.Direction FOSSILSCULPTURE_CEILING_LIGHT_DIRECTION = DiscreteShadowCaster.Direction.South;

		public const global::LightShape FOSSILSCULPTURE_SHAPE = global::LightShape.Quad;

		public const global::LightShape FOSSILSCULPTURE_CEILING_SHAPE = global::LightShape.Quad;

		public static readonly Color FOSSILSCULPTURE_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color FOSSILSCULPTURE_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 FOSSILSCULPTURE_OFFSET = new Vector2(0.05f, 0.65f);

		public static readonly Vector2 FOSSILSCULPTURE_CEILING_OFFSET = new Vector2(0.05f, 1.65f);

		public static readonly Vector2 FOSSILSCULPTURE_DIRECTION = Vector2.up;

		public static readonly Vector2 FOSSILSCULPTURE_CEILING_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const int FOSSILSCULPTURE_LUX = 3000;

		public static readonly int SUNLAMP_LUX = (int)((float)BeachChairConfig.TAN_LUX * 4f);

		public const float SUNLAMP_RANGE = 16f;

		public const float SUNLAMP_ANGLE = 5.2f;

		public const global::LightShape SUNLAMP_SHAPE = global::LightShape.Cone;

		public static readonly Color SUNLAMP_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color SUNLAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 SUNLAMP_OFFSET = new Vector2(0f, 3.5f);

		public static readonly Vector2 SUNLAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const int MERCURYCEILINGLIGHT_LUX = 60000;

		public const float MERCURYCEILINGLIGHT_RANGE = 8f;

		public const float MERCURYCEILINGLIGHT_ANGLE = 2.6f;

		public const float MERCURYCEILINGLIGHT_FALLOFFRATE = 0.4f;

		public const int MERCURYCEILINGLIGHT_WIDTH = 3;

		public const global::LightShape MERCURYCEILINGLIGHT_SHAPE = global::LightShape.Quad;

		public static readonly Color MERCURYCEILINGLIGHT_LUX_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Color MERCURYCEILINGLIGHT_COLOR = LIGHT2D.LIGHT_PINK;

		public static readonly Vector2 MERCURYCEILINGLIGHT_OFFSET = new Vector2(0.05f, 0.65f);

		public static readonly Vector2 MERCURYCEILINGLIGHT_DIRECTIONVECTOR = LIGHT2D.DEFAULT_DIRECTION;

		public const DiscreteShadowCaster.Direction MERCURYCEILINGLIGHT_DIRECTION = DiscreteShadowCaster.Direction.South;

		public static readonly Color LIGHT_PREVIEW_COLOR = LIGHT2D.LIGHT_YELLOW;

		public const float HEADQUARTERS_RANGE = 5f;

		public const global::LightShape HEADQUARTERS_SHAPE = global::LightShape.Circle;

		public static readonly Color HEADQUARTERS_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color HEADQUARTERS_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 HEADQUARTERS_OFFSET = new Vector2(0.5f, 3f);

		public static readonly Vector2 EXOBASE_HEADQUARTERS_OFFSET = new Vector2(0f, 2.5f);

		public const float POI_TECH_UNLOCK_RANGE = 5f;

		public const float POI_TECH_UNLOCK_ANGLE = 2.6f;

		public const global::LightShape POI_TECH_UNLOCK_SHAPE = global::LightShape.Cone;

		public static readonly Color POI_TECH_UNLOCK_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color POI_TECH_UNLOCK_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 POI_TECH_UNLOCK_OFFSET = new Vector2(0f, 3.4f);

		public const int POI_TECH_UNLOCK_LUX = 1800;

		public static readonly Vector2 POI_TECH_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const float ENGINE_RANGE = 10f;

		public const global::LightShape ENGINE_SHAPE = global::LightShape.Circle;

		public const int ENGINE_LUX = 80000;

		public const float WALLLIGHT_RANGE = 4f;

		public const float WALLLIGHT_ANGLE = 0f;

		public const global::LightShape WALLLIGHT_SHAPE = global::LightShape.Circle;

		public static readonly Color WALLLIGHT_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color WALLLIGHT_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 WALLLIGHT_OFFSET = new Vector2(0f, 0.5f);

		public static readonly Vector2 WALLLIGHT_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const float LIGHTBUG_RANGE = 5f;

		public const float LIGHTBUG_ANGLE = 0f;

		public const global::LightShape LIGHTBUG_SHAPE = global::LightShape.Circle;

		public const int LIGHTBUG_LUX = 1800;

		public static readonly Color LIGHTBUG_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color LIGHTBUG_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Color LIGHTBUG_COLOR_ORANGE = new Color(0.5686275f, 0.48235294f, 0.4392157f, 1f);

		public static readonly Color LIGHTBUG_COLOR_PURPLE = new Color(0.49019608f, 0.4392157f, 0.5686275f, 1f);

		public static readonly Color LIGHTBUG_COLOR_PINK = new Color(0.5686275f, 0.4392157f, 0.5686275f, 1f);

		public static readonly Color LIGHTBUG_COLOR_BLUE = new Color(0.4392157f, 0.4862745f, 0.5686275f, 1f);

		public static readonly Color LIGHTBUG_COLOR_CRYSTAL = new Color(0.5137255f, 0.6666667f, 0.6666667f, 1f);

		public static readonly Color LIGHTBUG_COLOR_GREEN = new Color(0.43137255f, 1f, 0.53333336f, 1f);

		public const int MAJORFOSSILDIGSITE_LAMP_LUX = 1000;

		public const float MAJORFOSSILDIGSITE_LAMP_RANGE = 3f;

		public static readonly Vector2 MAJORFOSSILDIGSITE_LAMP_OFFSET = new Vector2(-0.15f, 2.35f);

		public static readonly Vector2 LIGHTBUG_OFFSET = new Vector2(0.05f, 0.25f);

		public static readonly Vector2 LIGHTBUG_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const int PLASMALAMP_LUX = 666;

		public const float PLASMALAMP_RANGE = 2f;

		public const float PLASMALAMP_ANGLE = 0f;

		public const global::LightShape PLASMALAMP_SHAPE = global::LightShape.Circle;

		public static readonly Color PLASMALAMP_COLOR = LIGHT2D.LIGHT_PURPLE;

		public static readonly Color PLASMALAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 PLASMALAMP_OFFSET = new Vector2(0.05f, 0.5f);

		public static readonly Vector2 PLASMALAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const int MAGMALAMP_LUX = 666;

		public const float MAGMALAMP_RANGE = 2f;

		public const float MAGMALAMP_ANGLE = 0f;

		public const global::LightShape MAGMALAMP_SHAPE = global::LightShape.Cone;

		public static readonly Color MAGMALAMP_COLOR = LIGHT2D.LIGHT_YELLOW;

		public static readonly Color MAGMALAMP_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 MAGMALAMP_OFFSET = new Vector2(0.05f, 0.33f);

		public static readonly Vector2 MAGMALAMP_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const int BIOLUMROCK_LUX = 666;

		public const float BIOLUMROCK_RANGE = 2f;

		public const float BIOLUMROCK_ANGLE = 0f;

		public const global::LightShape BIOLUMROCK_SHAPE = global::LightShape.Cone;

		public static readonly Color BIOLUMROCK_COLOR = LIGHT2D.LIGHT_BLUE;

		public static readonly Color BIOLUMROCK_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 BIOLUMROCK_OFFSET = new Vector2(0.05f, 0.33f);

		public static readonly Vector2 BIOLUMROCK_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;

		public const float PINKROCK_RANGE = 2f;

		public const float PINKROCK_ANGLE = 0f;

		public const global::LightShape PINKROCK_SHAPE = global::LightShape.Circle;

		public static readonly Color PINKROCK_COLOR = LIGHT2D.LIGHT_PINK;

		public static readonly Color PINKROCK_OVERLAYCOLOR = LIGHT2D.LIGHT_OVERLAY;

		public static readonly Vector2 PINKROCK_OFFSET = new Vector2(0.05f, 0.33f);

		public static readonly Vector2 PINKROCK_DIRECTION = LIGHT2D.DEFAULT_DIRECTION;
	}
}
