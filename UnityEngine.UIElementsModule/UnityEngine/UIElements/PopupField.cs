using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
	public class PopupField<T> : BasePopupField<T, T>
	{
		public virtual Func<T, string> formatSelectedValueCallback
		{
			get
			{
				return this.m_FormatSelectedValueCallback;
			}
			set
			{
				this.m_FormatSelectedValueCallback = value;
				base.textElement.text = this.GetValueToDisplay();
			}
		}

		public virtual Func<T, string> formatListItemCallback
		{
			get
			{
				return this.m_FormatListItemCallback;
			}
			set
			{
				this.m_FormatListItemCallback = value;
			}
		}

		internal override string GetValueToDisplay()
		{
			bool flag = this.m_FormatSelectedValueCallback != null;
			string text;
			if (flag)
			{
				text = this.m_FormatSelectedValueCallback(this.value);
			}
			else
			{
				bool flag2 = this.value != null;
				if (flag2)
				{
					T value = this.value;
					text = UIElementsUtility.ParseMenuName(value.ToString());
				}
				else
				{
					text = string.Empty;
				}
			}
			return text;
		}

		internal override string GetListItemToDisplay(T value)
		{
			bool flag = this.m_FormatListItemCallback != null;
			string text;
			if (flag)
			{
				text = this.m_FormatListItemCallback(value);
			}
			else
			{
				text = ((value != null && this.m_Choices.Contains(value)) ? value.ToString() : string.Empty);
			}
			return text;
		}

		public override T value
		{
			get
			{
				return base.value;
			}
			set
			{
				int index = this.m_Index;
				List<T> choices = this.m_Choices;
				this.m_Index = ((choices != null) ? choices.IndexOf(value) : (-1));
				base.value = value;
				bool flag = this.m_Index != index;
				if (flag)
				{
					base.NotifyPropertyChanged(in PopupField<T>.indexProperty);
				}
			}
		}

		public override void SetValueWithoutNotify(T newValue)
		{
			List<T> choices = this.m_Choices;
			this.m_Index = ((choices != null) ? choices.IndexOf(newValue) : (-1));
			base.SetValueWithoutNotify(newValue);
		}

		[CreateProperty]
		public int index
		{
			get
			{
				return this.m_Index;
			}
			set
			{
				bool flag = value != this.m_Index;
				if (flag)
				{
					this.m_Index = value;
					bool flag2 = this.m_Index >= 0 && this.m_Index < this.m_Choices.Count;
					if (flag2)
					{
						this.value = this.m_Choices[this.m_Index];
					}
					else
					{
						this.value = default(T);
					}
					base.NotifyPropertyChanged(in PopupField<T>.indexProperty);
				}
			}
		}

		public PopupField()
			: this(null)
		{
		}

		public PopupField(string label = null)
			: base(label)
		{
			base.AddToClassList(PopupField<T>.ussClassName);
			base.labelElement.AddToClassList(PopupField<T>.labelUssClassName);
			base.visualInput.AddToClassList(PopupField<T>.inputUssClassName);
		}

		public PopupField(List<T> choices, T defaultValue, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
			: this(null, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback)
		{
		}

		public PopupField(string label, List<T> choices, T defaultValue, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
			: this(label)
		{
			bool flag = defaultValue == null;
			if (flag)
			{
				throw new ArgumentNullException("defaultValue");
			}
			this.choices = choices;
			bool flag2 = !this.m_Choices.Contains(defaultValue);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Default value {0} is not present in the list of possible values", defaultValue));
			}
			this.SetValueWithoutNotify(defaultValue);
			this.formatListItemCallback = formatListItemCallback;
			this.formatSelectedValueCallback = formatSelectedValueCallback;
		}

		public PopupField(List<T> choices, int defaultIndex, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
			: this(null, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
		{
		}

		public PopupField(string label, List<T> choices, int defaultIndex, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
			: this(label)
		{
			this.choices = choices;
			this.SetIndexWithoutNotify(defaultIndex);
			this.formatListItemCallback = formatListItemCallback;
			this.formatSelectedValueCallback = formatSelectedValueCallback;
		}

		internal override void AddMenuItems(AbstractGenericMenu menu)
		{
			bool flag = menu == null;
			if (flag)
			{
				throw new ArgumentNullException("menu");
			}
			bool flag2 = this.m_Choices == null;
			if (!flag2)
			{
				using (List<T>.Enumerator enumerator = this.m_Choices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T item = enumerator.Current;
						bool flag3 = EqualityComparer<T>.Default.Equals(item, this.value) && !base.showMixedValue;
						menu.AddItem(this.GetListItemToDisplay(item), flag3, delegate
						{
							this.ChangeValueFromMenu(item);
						});
					}
				}
			}
		}

		internal void SetIndexWithoutNotify(int index)
		{
			this.m_Index = index;
			bool flag = this.m_Index >= 0 && this.m_Index < this.m_Choices.Count;
			if (flag)
			{
				this.SetValueWithoutNotify(this.m_Choices[this.m_Index]);
			}
			else
			{
				this.SetValueWithoutNotify(default(T));
			}
		}

		private void ChangeValueFromMenu(T menuItem)
		{
			this.value = menuItem;
		}

		internal static readonly BindingId indexProperty = "index";

		internal const int kPopupFieldDefaultIndex = -1;

		private int m_Index = -1;

		public new static readonly string ussClassName = "unity-popup-field";

		public new static readonly string labelUssClassName = PopupField<T>.ussClassName + "__label";

		public new static readonly string inputUssClassName = PopupField<T>.ussClassName + "__input";
	}
}
