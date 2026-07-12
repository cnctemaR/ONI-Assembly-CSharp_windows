using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class Vector3IntPropertyBag : ContainerPropertyBag<Vector3Int>
	{
		public Vector3IntPropertyBag()
		{
			base.AddProperty<int>(new Vector3IntPropertyBag.XProperty());
			base.AddProperty<int>(new Vector3IntPropertyBag.YProperty());
			base.AddProperty<int>(new Vector3IntPropertyBag.ZProperty());
		}

		private class XProperty : Property<Vector3Int, int>
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

			public override int GetValue(ref Vector3Int container)
			{
				return container.x;
			}

			public override void SetValue(ref Vector3Int container, int value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Vector3Int, int>
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

			public override int GetValue(ref Vector3Int container)
			{
				return container.y;
			}

			public override void SetValue(ref Vector3Int container, int value)
			{
				container.y = value;
			}
		}

		private class ZProperty : Property<Vector3Int, int>
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

			public override int GetValue(ref Vector3Int container)
			{
				return container.z;
			}

			public override void SetValue(ref Vector3Int container, int value)
			{
				container.z = value;
			}
		}
	}
}
