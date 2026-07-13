using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class PropertyChangedEvent : EventBase<PropertyChangedEvent>, IChangeEvent
	{
		static PropertyChangedEvent()
		{
			EventBase<PropertyChangedEvent>.SetCreateFunction(() => new PropertyChangedEvent());
		}

		public BindingId property { get; set; }

		public PropertyChangedEvent()
		{
			base.bubbles = false;
			base.tricklesDown = false;
		}

		public static PropertyChangedEvent GetPooled(in BindingId property)
		{
			PropertyChangedEvent pooled = EventBase<PropertyChangedEvent>.GetPooled();
			pooled.property = property;
			return pooled;
		}
	}
}
