using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public interface ITextEdition
	{
		bool multiline { get; set; }

		bool isReadOnly { get; set; }

		int maxLength { get; set; }

		string placeholder { get; set; }

		bool isDelayed { get; set; }

		void ResetValueAndText();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		void SaveValueAndText();

		void RestoreValueAndText();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		Func<char, bool> AcceptCharacter { get; set; }

		Action<bool> UpdateScrollOffset { get; set; }

		Action UpdateValueFromText { get; set; }

		Action UpdateTextFromValue { get; set; }

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		Action MoveFocusToCompositeRoot { get; set; }

		Func<string> GetDefaultValueType { get; set; }

		void UpdateText(string value);

		string CullString(string s);

		char maskChar { get; set; }

		bool isPassword { get; set; }

		bool hidePlaceholderOnFocus { get; set; }

		bool autoCorrection
		{
			get
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for autoCorrection. Calling ITextEdition.autoCorrection of this type will always return false.");
				return false;
			}
			set
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for autoCorrection. Assigning a value to ITextEdition.autoCorrection will not update its value.");
			}
		}

		bool hideMobileInput
		{
			get
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for hideMobileInput. Calling ITextEdition.hideMobileInput of this type will always return false.");
				return false;
			}
			set
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for hideMobileInput. Assigning a value to ITextEdition.hideMobileInput will not update its value.");
			}
		}

		TouchScreenKeyboard touchScreenKeyboard
		{
			get
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for touchScreenKeyboard. Calling ITextEdition.touchScreenKeyboard of this type will always return null.");
				return null;
			}
		}

		TouchScreenKeyboardType keyboardType
		{
			get
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for keyboardType. Calling ITextEdition.keyboardType of this type will always return Default.");
				return TouchScreenKeyboardType.Default;
			}
			set
			{
				Debug.Log("Type " + base.GetType().Name + " implementing interface ITextEdition is missing the implementation for keyboardType. Assigning a value to ITextEdition.keyboardType will not update its value.");
			}
		}
	}
}
