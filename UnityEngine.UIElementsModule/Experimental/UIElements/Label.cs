using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Label : BaseTextElement
	{
		public Label()
			: this(string.Empty)
		{
		}

		public Label(string text)
		{
			this.text = text;
		}

		/// <summary>
		///   <para>Instantiates a Label using the data read from a UXML file.</para>
		/// </summary>
		public class LabelFactory : UxmlFactory<Label, Label.LabelUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the Label.</para>
		/// </summary>
		public class LabelUxmlTraits : BaseTextElement.BaseTextElementUxmlTraits
		{
		}
	}
}
