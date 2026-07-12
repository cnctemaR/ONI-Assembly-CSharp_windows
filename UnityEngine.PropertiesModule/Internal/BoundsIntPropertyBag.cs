using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class BoundsIntPropertyBag : ContainerPropertyBag<BoundsInt>
	{
		public BoundsIntPropertyBag()
		{
			base.AddProperty<Vector3Int>(new BoundsIntPropertyBag.PositionProperty());
			base.AddProperty<Vector3Int>(new BoundsIntPropertyBag.SizeProperty());
		}

		private class PositionProperty : Property<BoundsInt, Vector3Int>
		{
			public override string Name
			{
				get
				{
					return "position";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override Vector3Int GetValue(ref BoundsInt container)
			{
				return container.position;
			}

			public override void SetValue(ref BoundsInt container, Vector3Int value)
			{
				container.position = value;
			}
		}

		private class SizeProperty : Property<BoundsInt, Vector3Int>
		{
			public override string Name
			{
				get
				{
					return "size";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override Vector3Int GetValue(ref BoundsInt container)
			{
				return container.size;
			}

			public override void SetValue(ref BoundsInt container, Vector3Int value)
			{
				container.size = value;
			}
		}
	}
}
