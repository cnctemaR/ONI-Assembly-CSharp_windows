using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct BackgroundRepeat : IEquatable<BackgroundRepeat>
	{
		public BackgroundRepeat(Repeat repeatX, Repeat repeatY)
		{
			this.x = repeatX;
			this.y = repeatY;
		}

		internal static BackgroundRepeat Initial()
		{
			return BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.StretchToFill);
		}

		public override bool Equals(object obj)
		{
			return obj is BackgroundRepeat && this.Equals((BackgroundRepeat)obj);
		}

		public bool Equals(BackgroundRepeat other)
		{
			return other.x == this.x && other.y == this.y;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.x.GetHashCode();
			return num * -1521134295 + this.y.GetHashCode();
		}

		public static bool operator ==(BackgroundRepeat style1, BackgroundRepeat style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(BackgroundRepeat style1, BackgroundRepeat style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("(x:{0}, y:{1})", this.x, this.y);
		}

		public Repeat x;

		public Repeat y;

		internal class PropertyBag : ContainerPropertyBag<BackgroundRepeat>
		{
			public PropertyBag()
			{
				base.AddProperty<Repeat>(new BackgroundRepeat.PropertyBag.XProperty());
				base.AddProperty<Repeat>(new BackgroundRepeat.PropertyBag.YProperty());
			}

			private class XProperty : Property<BackgroundRepeat, Repeat>
			{
				public override string Name { get; } = "x";

				public override bool IsReadOnly { get; } = false;

				public override Repeat GetValue(ref BackgroundRepeat container)
				{
					return container.x;
				}

				public override void SetValue(ref BackgroundRepeat container, Repeat value)
				{
					container.x = value;
				}
			}

			private class YProperty : Property<BackgroundRepeat, Repeat>
			{
				public override string Name { get; } = "y";

				public override bool IsReadOnly { get; } = false;

				public override Repeat GetValue(ref BackgroundRepeat container)
				{
					return container.y;
				}

				public override void SetValue(ref BackgroundRepeat container, Repeat value)
				{
					container.y = value;
				}
			}
		}
	}
}
