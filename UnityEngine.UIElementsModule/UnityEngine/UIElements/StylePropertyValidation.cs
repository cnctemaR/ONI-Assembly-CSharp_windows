using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[UxmlObject]
	internal abstract class StylePropertyValidation : INotifyBindablePropertyChanged
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

		protected void NotifyPropertyChanged(in BindingId bindingId)
		{
			EventHandler<BindablePropertyChangedEventArgs> eventHandler = this.propertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new BindablePropertyChangedEventArgs(in bindingId));
			}
		}

		[ExcludeFromDocs]
		[Serializable]
		public abstract class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
		}
	}
}
