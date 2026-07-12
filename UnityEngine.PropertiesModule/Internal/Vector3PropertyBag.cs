using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class Vector3PropertyBag : ContainerPropertyBag<Vector3>
	{
		public Vector3PropertyBag()
		{
			base.AddProperty<float>(new Vector3PropertyBag.XProperty());
			base.AddProperty<float>(new Vector3PropertyBag.YProperty());
			base.AddProperty<float>(new Vector3PropertyBag.ZProperty());
		}

		private class XProperty : Property<Vector3, float>
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

			public override float GetValue(ref Vector3 container)
			{
				return container.x;
			}

			public override void SetValue(ref Vector3 container, float value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Vector3, float>
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

			public override float GetValue(ref Vector3 container)
			{
				return container.y;
			}

			public override void SetValue(ref Vector3 container, float value)
			{
				container.y = value;
			}
		}

		private class ZProperty : Property<Vector3, float>
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

			public override float GetValue(ref Vector3 container)
			{
				return container.z;
			}

			public override void SetValue(ref Vector3 container, float value)
			{
				container.z = value;
			}
		}
	}
}
