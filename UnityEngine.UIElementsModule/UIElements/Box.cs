using System;

namespace UnityEngine.UIElements
{
	public class Box : VisualElement
	{
		public Box()
		{
			base.AddToClassList(Box.ussClassName);
		}

		public static readonly string ussClassName = "unity-box";

		public new class UxmlFactory : UxmlFactory<Box>
		{
		}
	}
}
