using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class ColorPropertyBag : ContainerPropertyBag<Color>
	{
		public ColorPropertyBag()
		{
			base.AddProperty<float>(new ColorPropertyBag.RProperty());
			base.AddProperty<float>(new ColorPropertyBag.GProperty());
			base.AddProperty<float>(new ColorPropertyBag.BProperty());
			base.AddProperty<float>(new ColorPropertyBag.AProperty());
		}

		private class RProperty : Property<Color, float>
		{
			public override string Name
			{
				get
				{
					return "r";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Color container)
			{
				return container.r;
			}

			public override void SetValue(ref Color container, float value)
			{
				container.r = value;
			}
		}

		private class GProperty : Property<Color, float>
		{
			public override string Name
			{
				get
				{
					return "g";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Color container)
			{
				return container.g;
			}

			public override void SetValue(ref Color container, float value)
			{
				container.g = value;
			}
		}

		private class BProperty : Property<Color, float>
		{
			public override string Name
			{
				get
				{
					return "b";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Color container)
			{
				return container.b;
			}

			public override void SetValue(ref Color container, float value)
			{
				container.b = value;
			}
		}

		private class AProperty : Property<Color, float>
		{
			public override string Name
			{
				get
				{
					return "a";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Color container)
			{
				return container.a;
			}

			public override void SetValue(ref Color container, float value)
			{
				container.a = value;
			}
		}
	}
}
