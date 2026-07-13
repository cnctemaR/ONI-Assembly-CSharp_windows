using System;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Properties.Internal
{
	[VisibleToOtherModules(new string[] { "UnityEditor.PropertiesModule" })]
	internal static class PropertiesInitialization
	{
		[RequiredByNativeCode(false)]
		public static void InitializeProperties()
		{
			PropertyBagStore.CreatePropertyBagProvider();
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
