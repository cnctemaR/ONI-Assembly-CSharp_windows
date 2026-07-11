using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>Contains normalized linear color values for red, green, blue in the range of 0 to 1, and an additional intensity value.</para>
	/// </summary>
	public struct LinearColor
	{
		/// <summary>
		///   <para>The red color value in the range of 0.0 to 1.0.</para>
		/// </summary>
		public float red
		{
			get
			{
				return this.m_red;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("Red color (" + value + ") must be in range [0;1].");
				}
				this.m_red = value;
			}
		}

		/// <summary>
		///   <para>The green color value in the range of 0.0 to 1.0.</para>
		/// </summary>
		public float green
		{
			get
			{
				return this.m_green;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("Green color (" + value + ") must be in range [0;1].");
				}
				this.m_green = value;
			}
		}

		/// <summary>
		///   <para>The blue color value in the range of 0.0 to 1.0.</para>
		/// </summary>
		public float blue
		{
			get
			{
				return this.m_blue;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("Blue color (" + value + ") must be in range [0;1].");
				}
				this.m_blue = value;
			}
		}

		/// <summary>
		///   <para>The intensity value used to scale the red, green and blue values.</para>
		/// </summary>
		public float intensity
		{
			get
			{
				return this.m_intensity;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("Intensity (" + value + ") must be positive.");
				}
				this.m_intensity = value;
			}
		}

		/// <summary>
		///   <para>Converts a Light's color value to a normalized linear color value, automatically handling gamma conversion if necessary.</para>
		/// </summary>
		/// <param name="color">Light color.</param>
		/// <param name="intensity">Light intensity.</param>
		/// <returns>
		///   <para>Returns the normalized linear color value.</para>
		/// </returns>
		public static LinearColor Convert(Color color, float intensity)
		{
			Color color2 = ((!GraphicsSettings.lightsUseLinearIntensity) ? color.RGBMultiplied(intensity).linear : color.linear.RGBMultiplied(intensity));
			float maxColorComponent = color2.maxColorComponent;
			LinearColor linearColor;
			if (maxColorComponent <= 0f)
			{
				linearColor = LinearColor.Black();
			}
			else
			{
				float num = 1f / color2.maxColorComponent;
				LinearColor linearColor2;
				linearColor2.m_red = color2.r * num;
				linearColor2.m_green = color2.g * num;
				linearColor2.m_blue = color2.b * num;
				linearColor2.m_intensity = maxColorComponent;
				linearColor = linearColor2;
			}
			return linearColor;
		}

		/// <summary>
		///   <para>Returns a black color.</para>
		/// </summary>
		/// <returns>
		///   <para>Returns a black color.</para>
		/// </returns>
		public static LinearColor Black()
		{
			LinearColor linearColor;
			linearColor.m_red = (linearColor.m_green = (linearColor.m_blue = (linearColor.m_intensity = 0f)));
			return linearColor;
		}

		private float m_red;

		private float m_green;

		private float m_blue;

		private float m_intensity;
	}
}
