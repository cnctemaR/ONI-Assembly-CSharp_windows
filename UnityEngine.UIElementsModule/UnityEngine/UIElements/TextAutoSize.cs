using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public struct TextAutoSize : IEquatable<TextAutoSize>
	{
		public TextAutoSizeMode mode { readonly get; set; }

		public Length minSize { readonly get; set; }

		public Length maxSize { readonly get; set; }

		public TextAutoSize(TextAutoSizeMode mode, Length minSize, Length maxSize)
		{
			this.mode = mode;
			this.minSize = minSize;
			this.maxSize = maxSize;
		}

		public static TextAutoSize None()
		{
			return new TextAutoSize
			{
				mode = TextAutoSizeMode.None,
				maxSize = 100f,
				minSize = 10f
			};
		}

		public bool Equals(TextAutoSize other)
		{
			return this.mode == other.mode && this.minSize.Equals(other.minSize) && this.maxSize.Equals(other.maxSize);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is TextAutoSize)
			{
				TextAutoSize textAutoSize = (TextAutoSize)obj;
				flag = this.Equals(textAutoSize);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.mode.GetHashCode();
			num = num * -1521134295 + this.minSize.GetHashCode();
			return num * -1521134295 + this.maxSize.GetHashCode();
		}

		public static bool operator ==(TextAutoSize left, TextAutoSize right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TextAutoSize left, TextAutoSize right)
		{
			return !(left == right);
		}

		internal class PropertyBag : ContainerPropertyBag<TextAutoSize>
		{
			public PropertyBag()
			{
				base.AddProperty<TextAutoSizeMode>(new TextAutoSize.PropertyBag.ModeProperty());
				base.AddProperty<Length>(new TextAutoSize.PropertyBag.MinSizeProperty());
				base.AddProperty<Length>(new TextAutoSize.PropertyBag.MaxSizeProperty());
			}

			private class ModeProperty : Property<TextAutoSize, TextAutoSizeMode>
			{
				public override string Name { get; } = "mode";

				public override bool IsReadOnly { get; } = false;

				public override TextAutoSizeMode GetValue(ref TextAutoSize container)
				{
					return container.mode;
				}

				public override void SetValue(ref TextAutoSize container, TextAutoSizeMode value)
				{
					container.mode = value;
				}
			}

			private class MinSizeProperty : Property<TextAutoSize, Length>
			{
				public override string Name { get; } = "minSize";

				public override bool IsReadOnly { get; } = false;

				public override Length GetValue(ref TextAutoSize container)
				{
					return container.minSize;
				}

				public override void SetValue(ref TextAutoSize container, Length value)
				{
					container.minSize = value;
				}
			}

			private class MaxSizeProperty : Property<TextAutoSize, Length>
			{
				public override string Name { get; } = "maxSize";

				public override bool IsReadOnly { get; } = false;

				public override Length GetValue(ref TextAutoSize container)
				{
					return container.maxSize;
				}

				public override void SetValue(ref TextAutoSize container, Length value)
				{
					container.maxSize = value;
				}
			}
		}
	}
}
