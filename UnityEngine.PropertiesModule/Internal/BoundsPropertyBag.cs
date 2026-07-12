using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class BoundsPropertyBag : ContainerPropertyBag<Bounds>
	{
		public BoundsPropertyBag()
		{
			base.AddProperty<Vector3>(new BoundsPropertyBag.CenterProperty());
			base.AddProperty<Vector3>(new BoundsPropertyBag.ExtentsProperty());
		}

		private class CenterProperty : Property<Bounds, Vector3>
		{
			public override string Name
			{
				get
				{
					return "center";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override Vector3 GetValue(ref Bounds container)
			{
				return container.center;
			}

			public override void SetValue(ref Bounds container, Vector3 value)
			{
				container.center = value;
			}
		}

		private class ExtentsProperty : Property<Bounds, Vector3>
		{
			public override string Name
			{
				get
				{
					return "extents";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override Vector3 GetValue(ref Bounds container)
			{
				return container.extents;
			}

			public override void SetValue(ref Bounds container, Vector3 value)
			{
				container.extents = value;
			}
		}
	}
}
