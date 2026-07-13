using System;

namespace UnityEngine.UIElements
{
	public interface ITransform
	{
		[Obsolete("When reading the value, use VisualElement.resolvedStyle.translate. When writing the value, use VisualElement.style.translate instead.")]
		Vector3 position { get; set; }

		[Obsolete("When reading the value, use VisualElement.resolvedStyle.rotate. When writing the value, use VisualElement.style.rotate instead.")]
		Quaternion rotation { get; set; }

		[Obsolete("When reading the value, use VisualElement.resolvedStyle.scale. When writing the value, use VisualElement.style.scale instead.")]
		Vector3 scale { get; set; }

		Matrix4x4 matrix { get; }
	}
}
