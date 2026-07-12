using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal static class DefaultPropertyBagInitializer
	{
		internal static void Initialize()
		{
			PropertyBag.Register<Color>(new ColorPropertyBag());
			PropertyBag.Register<Vector2>(new Vector2PropertyBag());
			PropertyBag.Register<Vector3>(new Vector3PropertyBag());
			PropertyBag.Register<Vector4>(new Vector4PropertyBag());
			PropertyBag.Register<Vector2Int>(new Vector2IntPropertyBag());
			PropertyBag.Register<Vector3Int>(new Vector3IntPropertyBag());
			PropertyBag.Register<Rect>(new RectPropertyBag());
			PropertyBag.Register<RectInt>(new RectIntPropertyBag());
			PropertyBag.Register<Bounds>(new BoundsPropertyBag());
			PropertyBag.Register<BoundsInt>(new BoundsIntPropertyBag());
			PropertyBag.Register<Version>(new SystemVersionPropertyBag());
		}
	}
}
