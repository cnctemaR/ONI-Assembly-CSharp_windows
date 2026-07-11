using System;

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

		public new class UxmlFactory : UxmlFactory<Label, Label.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextElement.UxmlTraits
		{
		}
	}
}
