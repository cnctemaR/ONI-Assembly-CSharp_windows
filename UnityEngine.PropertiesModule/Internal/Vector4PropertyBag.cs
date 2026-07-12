using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class Vector4PropertyBag : ContainerPropertyBag<Vector4>
	{
		public Vector4PropertyBag()
		{
			base.AddProperty<float>(new Vector4PropertyBag.XProperty());
			base.AddProperty<float>(new Vector4PropertyBag.YProperty());
			base.AddProperty<float>(new Vector4PropertyBag.ZProperty());
			base.AddProperty<float>(new Vector4PropertyBag.WProperty());
		}

		private class XProperty : Property<Vector4, float>
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

			public override float GetValue(ref Vector4 container)
			{
				return container.x;
			}

			public override void SetValue(ref Vector4 container, float value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Vector4, float>
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

			public override float GetValue(ref Vector4 container)
			{
				return container.y;
			}

			public override void SetValue(ref Vector4 container, float value)
			{
				container.y = value;
			}
		}

		private class ZProperty : Property<Vector4, float>
		{
			public override string Name
			{
				get
				{
					return "z";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Vector4 container)
			{
				return container.z;
			}

			public override void SetValue(ref Vector4 container, float value)
			{
				container.z = value;
			}
		}

		private class WProperty : Property<Vector4, float>
		{
			public override string Name
			{
				get
				{
					return "w";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override float GetValue(ref Vector4 container)
			{
				return container.w;
			}

			public override void SetValue(ref Vector4 container, float value)
			{
				container.w = value;
			}
		}
	}
}
