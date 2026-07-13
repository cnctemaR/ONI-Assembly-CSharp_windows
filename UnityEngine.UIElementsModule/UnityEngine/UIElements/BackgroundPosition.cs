using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct BackgroundPosition : IEquatable<BackgroundPosition>
	{
		public BackgroundPosition(BackgroundPositionKeyword keyword)
		{
			this.keyword = keyword;
			this.offset = new Length(0f);
		}

		public BackgroundPosition(BackgroundPositionKeyword keyword, Length offset)
		{
			this.keyword = keyword;
			this.offset = offset;
		}

		internal static BackgroundPosition Initial()
		{
			return BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
		}

		public override bool Equals(object obj)
		{
			return obj is BackgroundPosition && this.Equals((BackgroundPosition)obj);
		}

		public bool Equals(BackgroundPosition other)
		{
			return other.offset == this.offset && other.keyword == this.keyword;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.keyword.GetHashCode();
			return num * -1521134295 + this.offset.GetHashCode();
		}

		public static bool operator ==(BackgroundPosition style1, BackgroundPosition style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(BackgroundPosition style1, BackgroundPosition style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("(type:{0} x:{1})", this.keyword, this.offset);
		}

		public BackgroundPositionKeyword keyword;

		public Length offset;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal enum Axis
		{
			Horizontal,
			Vertical
		}

		internal class PropertyBag : ContainerPropertyBag<BackgroundPosition>
		{
			public PropertyBag()
			{
				base.AddProperty<BackgroundPositionKeyword>(new BackgroundPosition.PropertyBag.KeywordProperty());
				base.AddProperty<Length>(new BackgroundPosition.PropertyBag.OffsetProperty());
			}

			private class KeywordProperty : Property<BackgroundPosition, BackgroundPositionKeyword>
			{
				public override string Name { get; } = "keyword";

				public override bool IsReadOnly { get; } = false;

				public override BackgroundPositionKeyword GetValue(ref BackgroundPosition container)
				{
					return container.keyword;
				}

				public override void SetValue(ref BackgroundPosition container, BackgroundPositionKeyword value)
				{
					container.keyword = value;
				}
			}

			private class OffsetProperty : Property<BackgroundPosition, Length>
			{
				public override string Name { get; } = "offset";

				public override bool IsReadOnly { get; } = false;

				public override Length GetValue(ref BackgroundPosition container)
				{
					return container.offset;
				}

				public override void SetValue(ref BackgroundPosition container, Length value)
				{
					container.offset = value;
				}
			}
		}
	}
}
