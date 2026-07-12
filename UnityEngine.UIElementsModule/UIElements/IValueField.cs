using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public interface IValueField<T>
	{
		T value { get; set; }

		void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, T startValue);

		void StartDragging();

		void StopDragging();
	}
}
