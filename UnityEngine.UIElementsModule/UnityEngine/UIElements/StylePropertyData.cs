using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct StylePropertyData<TInline, TComputedValue> : IEquatable<StylePropertyData<TInline, TComputedValue>>, IDisposable
	{
		public VisualElement target { readonly get; internal set; }

		[CreateProperty(ReadOnly = true)]
		public TInline inlineValue { readonly get; internal set; }

		[CreateProperty(ReadOnly = true)]
		public UxmlStyleProperty uxmlValue { readonly get; internal set; }

		[CreateProperty(ReadOnly = true)]
		public TComputedValue computedValue { readonly get; internal set; }

		[CreateProperty(ReadOnly = true)]
		public Binding binding { readonly get; internal set; }

		[CreateProperty(ReadOnly = true)]
		public SelectorMatchRecord selector { readonly get; internal set; }

		[CreateProperty(ReadOnly = true)]
		public bool isUxmlOverridden
		{
			get
			{
				return this.uxmlValue.isInlined || this.binding != null;
			}
		}

		public bool Equals(StylePropertyData<TInline, TComputedValue> other)
		{
			return EqualityComparer<TInline>.Default.Equals(this.inlineValue, other.inlineValue) && EqualityComparer<UxmlStyleProperty>.Default.Equals(this.uxmlValue, other.uxmlValue) && EqualityComparer<TComputedValue>.Default.Equals(this.computedValue, other.computedValue) && this.binding == other.binding && EqualityComparer<SelectorMatchRecord>.Default.Equals(this.selector, other.selector);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StylePropertyData<StyleLength, Length>)
			{
				StylePropertyData<StyleLength, Length> stylePropertyData = (StylePropertyData<StyleLength, Length>)obj;
				flag = this.Equals(stylePropertyData);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<TInline, UxmlStyleProperty, TComputedValue, Binding, SelectorMatchRecord>(this.inlineValue, this.uxmlValue, this.computedValue, this.binding, this.selector);
		}

		public static bool operator ==(StylePropertyData<TInline, TComputedValue> lhs, StylePropertyData<TInline, TComputedValue> rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(StylePropertyData<TInline, TComputedValue> lhs, StylePropertyData<TInline, TComputedValue> rhs)
		{
			return !(lhs == rhs);
		}

		public void Dispose()
		{
			this.uxmlValue.Dispose();
		}
	}
}
