using System;

namespace UnityEngine.Experimental.UIElements
{
	internal interface ITextInputField : IEventHandler
	{
		bool hasFocus { get; }

		string text { get; }

		bool doubleClickSelectsWord { get; }

		bool tripleClickSelectsLine { get; }

		void SyncTextEngine();

		bool AcceptCharacter(char c);

		string CullString(string s);

		void UpdateText(string value);
	}
}
