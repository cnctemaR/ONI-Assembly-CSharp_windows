using System;

namespace UnityEngine.UIElements
{
	public struct FillGradient
	{
		public Gradient gradient { readonly get; set; }

		public GradientType gradientType { readonly get; set; }

		public AddressMode addressMode { readonly get; set; }

		public Vector2 start { readonly get; set; }

		public Vector2 end { readonly get; set; }

		public Vector2 center { readonly get; set; }

		public Vector2 focus { readonly get; set; }

		public float radius { readonly get; set; }

		public static FillGradient MakeLinearGradient(Color startColor, Color endColor, Vector2 start, Vector2 end, AddressMode addressMode = AddressMode.Clamp)
		{
			Gradient gradient = new Gradient
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey
					{
						color = startColor,
						time = 0f
					},
					new GradientColorKey
					{
						color = endColor,
						time = 1f
					}
				}
			};
			return FillGradient.MakeLinearGradient(gradient, start, end, addressMode);
		}

		public static FillGradient MakeLinearGradient(Gradient gradient, Vector2 start, Vector2 end, AddressMode addressMode = AddressMode.Clamp)
		{
			return new FillGradient
			{
				gradient = gradient,
				gradientType = GradientType.Linear,
				addressMode = addressMode,
				start = start,
				end = end,
				center = Vector2.zero,
				focus = Vector2.zero,
				radius = 0f
			};
		}

		public static FillGradient MakeRadialGradient(Color startColor, Color endColor, Vector2 center, float radius, Vector2 focus, AddressMode addressMode = AddressMode.Clamp)
		{
			Gradient gradient = new Gradient
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey
					{
						color = startColor,
						time = 0f
					},
					new GradientColorKey
					{
						color = endColor,
						time = 1f
					}
				}
			};
			return FillGradient.MakeRadialGradient(gradient, center, radius, focus, addressMode);
		}

		public static FillGradient MakeRadialGradient(Gradient gradient, Vector2 center, float radius, Vector2 focus, AddressMode addressMode = AddressMode.Clamp)
		{
			return new FillGradient
			{
				gradient = gradient,
				gradientType = GradientType.Radial,
				addressMode = addressMode,
				start = Vector2.zero,
				end = Vector2.zero,
				center = center,
				focus = focus,
				radius = radius
			};
		}
	}
}
