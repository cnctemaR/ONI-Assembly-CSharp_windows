using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Label : TextElement
	{
		public Label()
			: this(string.Empty)
		{
		}

		public Label(string text)
		{
			this.text = text;
		}

		public new class UxmlFactory : UxmlFactory<Label, Label.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextElement.UxmlTraits
		{
		}
	}
}
