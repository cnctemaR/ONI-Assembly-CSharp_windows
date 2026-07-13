using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct TextShadow : IEquatable<TextShadow>
	{
		public override bool Equals(object obj)
		{
			return obj is TextShadow && this.Equals((TextShadow)obj);
		}

		public bool Equals(TextShadow other)
		{
			return other.offset == this.offset && other.blurRadius == this.blurRadius && other.color == this.color;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.offset.GetHashCode();
			num = num * -1521134295 + this.blurRadius.GetHashCode();
			return num * -1521134295 + this.color.GetHashCode();
		}

		public static bool operator ==(TextShadow style1, TextShadow style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(TextShadow style1, TextShadow style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("offset={0}, blurRadius={1}, color={2}", this.offset, this.blurRadius, this.color);
		}

		internal static TextShadow LerpUnclamped(TextShadow a, TextShadow b, float t)
		{
			return new TextShadow
			{
				offset = Vector2.LerpUnclamped(a.offset, b.offset, t),
				blurRadius = Mathf.LerpUnclamped(a.blurRadius, b.blurRadius, t),
				color = Color.LerpUnclamped(a.color, b.color, t)
			};
		}

		public Vector2 offset;

		public float blurRadius;

		public Color color;

		internal class PropertyBag : ContainerPropertyBag<TextShadow>
		{
			public PropertyBag()
			{
				base.AddProperty<Vector2>(new TextShadow.PropertyBag.OffsetProperty());
				base.AddProperty<float>(new TextShadow.PropertyBag.BlurRadiusProperty());
				base.AddProperty<Color>(new TextShadow.PropertyBag.ColorProperty());
			}

			private class OffsetProperty : Property<TextShadow, Vector2>
			{
				public override string Name { get; } = "offset";

				public override bool IsReadOnly { get; } = false;

				public override Vector2 GetValue(ref TextShadow container)
				{
					return container.offset;
				}

				public override void SetValue(ref TextShadow container, Vector2 value)
				{
					container.offset = value;
				}
			}

			private class BlurRadiusProperty : Property<TextShadow, float>
			{
				public override string Name { get; } = "blurRadius";

				public override bool IsReadOnly { get; } = false;

				public override float GetValue(ref TextShadow container)
				{
					return container.blurRadius;
				}

				public override void SetValue(ref TextShadow container, float value)
				{
					container.blurRadius = value;
				}
			}

			private class ColorProperty : Property<TextShadow, Color>
			{
				public override string Name { get; } = "color";

				public override bool IsReadOnly { get; } = false;

				public override Color GetValue(ref TextShadow container)
				{
					return container.color;
				}

				public override void SetValue(ref TextShadow container, Color value)
				{
					container.color = value;
				}
			}
		}
	}
}
