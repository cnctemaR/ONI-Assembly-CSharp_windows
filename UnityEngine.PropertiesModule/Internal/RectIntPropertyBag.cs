using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class RectIntPropertyBag : ContainerPropertyBag<RectInt>
	{
		public RectIntPropertyBag()
		{
			base.AddProperty<int>(new RectIntPropertyBag.XProperty());
			base.AddProperty<int>(new RectIntPropertyBag.YProperty());
			base.AddProperty<int>(new RectIntPropertyBag.WidthProperty());
			base.AddProperty<int>(new RectIntPropertyBag.HeightProperty());
		}

		private class XProperty : Property<RectInt, int>
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

			public override int GetValue(ref RectInt container)
			{
				return container.x;
			}

			public override void SetValue(ref RectInt container, int value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<RectInt, int>
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

			public override int GetValue(ref RectInt container)
			{
				return container.y;
			}

			public override void SetValue(ref RectInt container, int value)
			{
				container.y = value;
			}
		}

		private class WidthProperty : Property<RectInt, int>
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

			public override int GetValue(ref RectInt container)
			{
				return container.width;
			}

			public override void SetValue(ref RectInt container, int value)
			{
				container.width = value;
			}
		}

		private class HeightProperty : Property<RectInt, int>
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

			public override int GetValue(ref RectInt container)
			{
				return container.height;
			}

			public override void SetValue(ref RectInt container, int value)
			{
				container.height = value;
			}
		}
	}
}
