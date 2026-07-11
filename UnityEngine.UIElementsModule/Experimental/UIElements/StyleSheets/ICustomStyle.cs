using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	public interface ICustomStyle
	{
		void ApplyCustomProperty(string propertyName, ref StyleValue<float> target);

		void ApplyCustomProperty(string propertyName, ref StyleValue<int> target);

		void ApplyCustomProperty(string propertyName, ref StyleValue<bool> target);

		void ApplyCustomProperty(string propertyName, ref StyleValue<Color> target);

		void ApplyCustomProperty(string propertyName, ref StyleValue<Texture2D> target);

		void ApplyCustomProperty(string propertyName, ref StyleValue<string> target);
	}
}
