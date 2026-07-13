using System;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Box : VisualElement
	{
		public Box()
		{
			base.AddToClassList(Box.ussClassName);
		}

		public static readonly string ussClassName = "unity-box";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			public override object CreateInstance()
			{
				return new Box();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Box>
		{
		}
	}
}
