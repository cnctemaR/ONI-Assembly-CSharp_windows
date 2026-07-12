using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class RectPropertyBag : ContainerPropertyBag<Rect>
	{
		public RectPropertyBag()
		{
			base.AddProperty<float>(new RectPropertyBag.XProperty());
			base.AddProperty<float>(new RectPropertyBag.YProperty());
			base.AddProperty<float>(new RectPropertyBag.WidthProperty());
			base.AddProperty<float>(new RectPropertyBag.HeightProperty());
		}

		private class XProperty : Property<Rect, float>
		{
			public override string Name
			{
				get
				{
					return "x";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Rect container)
			{
				return container.x;
			}

			public override void SetValue(ref Rect container, float value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Rect, float>
		{
			public override string Name
			{
				get
				{
					return "y";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Rect container)
			{
				return container.y;
			}

			public override void SetValue(ref Rect container, float value)
			{
				container.y = value;
			}
		}

		private class WidthProperty : Property<Rect, float>
		{
			public override string Name
			{
				get
				{
					return "width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Rect container)
			{
				return container.width;
			}

			public override void SetValue(ref Rect container, float value)
			{
				container.width = value;
			}
		}

		private class HeightProperty : Property<Rect, float>
		{
			public override string Name
			{
				get
				{
					return "height";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Rect container)
			{
				return container.height;
			}

			public override void SetValue(ref Rect container, float value)
			{
				container.height = value;
			}
		}
	}
}
