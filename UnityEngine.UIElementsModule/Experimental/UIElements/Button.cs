using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Button : TextElement
	{
		public Button()
			: this(null)
		{
		}

		public Button(Action clickEvent)
		{
			this.clickable = new Clickable(clickEvent);
			this.AddManipulator(this.clickable);
		}

		public Clickable clickable;

		public new class UxmlFactory : UxmlFactory<Button, Button.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextElement.UxmlTraits
		{
		}
	}
}
