using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public interface ISelectionService
	{
		event EventHandler SelectionChanged;

		event EventHandler SelectionChanging;

		bool GetComponentSelected(object component);

		ICollection GetSelectedComponents();

		void SetSelectedComponents(ICollection components, SelectionTypes selectionType);

		void SetSelectedComponents(ICollection components);

		object PrimarySelection { get; }

		int SelectionCount { get; }
	}
}
