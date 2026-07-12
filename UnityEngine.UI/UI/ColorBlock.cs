using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[Serializable]
	public struct ColorBlock : IEquatable<ColorBlock>
	{
		public Color normalColor
		{
			get
			{
				return this.m_NormalColor;
			}
			set
			{
				this.m_NormalColor = value;
			}
		}

		public Color highlightedColor
		{
			get
			{
				return this.m_HighlightedColor;
			}
			set
			{
				this.m_HighlightedColor = value;
			}
		}

		public Color pressedColor
		{
			get
			{
				return this.m_PressedColor;
			}
			set
			{
				this.m_PressedColor = value;
			}
		}

		public Color selectedColor
		{
			get
			{
				return this.m_SelectedColor;
			}
			set
			{
				this.m_SelectedColor = value;
			}
		}

		public Color disabledColor
		{
			get
			{
				return this.m_DisabledColor;
			}
			set
			{
				this.m_DisabledColor = value;
			}
		}

		public float colorMultiplier
		{
			get
			{
				return this.m_ColorMultiplier;
			}
			set
			{
				this.m_ColorMultiplier = value;
			}
		}

		public float fadeDuration
		{
			get
			{
				return this.m_FadeDuration;
			}
			set
			{
				this.m_FadeDuration = value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ColorBlock && this.Equals((ColorBlock)obj);
		}

		public bool Equals(ColorBlock other)
		{
			return this.normalColor == other.normalColor && this.highlightedColor == other.highlightedColor && this.pressedColor == other.pressedColor && this.selectedColor == other.selectedColor && this.disabledColor == other.disabledColor && this.colorMultiplier == other.colorMultiplier && this.fadeDuration == other.fadeDuration;
		}

		public static bool operator ==(ColorBlock point1, ColorBlock point2)
		{
			return point1.Equals(point2);
		}

		public static bool operator !=(ColorBlock point1, ColorBlock point2)
		{
			return !point1.Equals(point2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[FormerlySerializedAs("normalColor")]
		[SerializeField]
		private Color m_NormalColor;

		[FormerlySerializedAs("highlightedColor")]
		[SerializeField]
		private Color m_HighlightedColor;

		[FormerlySerializedAs("pressedColor")]
		[SerializeField]
		private Color m_PressedColor;

		[FormerlySerializedAs("m_HighlightedColor")]
		[SerializeField]
		private Color m_SelectedColor;

		[FormerlySerializedAs("disabledColor")]
		[SerializeField]
		private Color m_DisabledColor;

		[Range(1f, 5f)]
		[SerializeField]
		private float m_ColorMultiplier;

		[FormerlySerializedAs("fadeDuration")]
		[SerializeField]
		private float m_FadeDuration;

		public static ColorBlock defaultColorBlock = new ColorBlock
		{
			m_NormalColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
			m_HighlightedColor = new Color32(245, 245, 245, byte.MaxValue),
			m_PressedColor = new Color32(200, 200, 200, byte.MaxValue),
			m_SelectedColor = new Color32(245, 245, 245, byte.MaxValue),
			m_DisabledColor = new Color32(200, 200, 200, 128),
			colorMultiplier = 1f,
			fadeDuration = 0.1f
		};
	}
}
