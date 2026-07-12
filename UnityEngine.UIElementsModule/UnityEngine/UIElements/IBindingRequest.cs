using System;

namespace UnityEngine.UIElements
{
	internal interface IBindingRequest
	{
		void Bind(VisualElement element);

		void Release();
	}
}
