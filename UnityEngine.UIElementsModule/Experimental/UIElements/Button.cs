using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Button : BaseTextElement
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

		/// <summary>
		///   <para>Instantiates a Button using the data read from a UXML file.</para>
		/// </summary>
		public class ButtonFactory : UxmlFactory<Button, Button.ButtonUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the Button.</para>
		/// </summary>
		public class ButtonUxmlTraits : BaseTextElement.BaseTextElementUxmlTraits
		{
		}
	}
}
