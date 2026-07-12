using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class Vector2IntPropertyBag : ContainerPropertyBag<Vector2Int>
	{
		public Vector2IntPropertyBag()
		{
			base.AddProperty<int>(new Vector2IntPropertyBag.XProperty());
			base.AddProperty<int>(new Vector2IntPropertyBag.YProperty());
		}

		private class XProperty : Property<Vector2Int, int>
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

			public override int GetValue(ref Vector2Int container)
			{
				return container.x;
			}

			public override void SetValue(ref Vector2Int container, int value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Vector2Int, int>
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

			public override int GetValue(ref Vector2Int container)
			{
				return container.y;
			}

			public override void SetValue(ref Vector2Int container, int value)
			{
				container.y = value;
			}
		}
	}
}
