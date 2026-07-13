using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal interface IScreenRaycaster
	{
		void Update();

		[return: TupleElementNames(new string[] { "ray", "camera", "isInsideCameraRect" })]
		IEnumerable<ValueTuple<Ray, Camera, bool>> MakeRay(Vector2 mousePosition, int pointerId, int? targetDisplay);
	}
}
