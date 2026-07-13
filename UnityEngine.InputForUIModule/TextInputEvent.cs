using System;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct TextInputEvent : IEventProperties
	{
		public DiscreteTime timestamp { readonly get; set; }

		public EventSource eventSource { readonly get; set; }

		public uint playerId { readonly get; set; }

		public EventModifiers eventModifiers { readonly get; set; }

		public override string ToString()
		{
			string text = ((this.character == '\0') ? string.Empty : this.character.ToString());
			return string.Format("text input 0x{0:x8} '{1}'", (int)this.character, text);
		}

		public static bool ShouldBeProcessed(char character)
		{
			return character > '\u001f' && character != '\u007f';
		}

		public char character;
	}
}
