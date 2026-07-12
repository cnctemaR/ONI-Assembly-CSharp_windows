using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class Vector2PropertyBag : ContainerPropertyBag<Vector2>
	{
		public Vector2PropertyBag()
		{
			base.AddProperty<float>(new Vector2PropertyBag.XProperty());
			base.AddProperty<float>(new Vector2PropertyBag.YProperty());
		}

		private class XProperty : Property<Vector2, float>
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

			public override float GetValue(ref Vector2 container)
			{
				return container.x;
			}

			public override void SetValue(ref Vector2 container, float value)
			{
				container.x = value;
			}
		}

		private class YProperty : Property<Vector2, float>
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

			public override float GetValue(ref Vector2 container)
			{
				return container.y;
			}

			public override void SetValue(ref Vector2 container, float value)
			{
				container.y = value;
			}
		}
	}
}
