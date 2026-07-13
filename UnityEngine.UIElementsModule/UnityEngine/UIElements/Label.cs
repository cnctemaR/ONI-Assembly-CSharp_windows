using System;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Label : TextElement
	{
		public Label()
			: this(string.Empty)
		{
		}

		public Label(string text)
		{
			base.AddToClassList(Label.ussClassName);
			this.text = text;
		}

		public new static readonly string ussClassName = "unity-label";

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : TextElement.UxmlSerializedData
		{
			public override object CreateInstance()
			{
				return new Label();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Label, Label.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : TextElement.UxmlTraits
		{
		}
	}
}
