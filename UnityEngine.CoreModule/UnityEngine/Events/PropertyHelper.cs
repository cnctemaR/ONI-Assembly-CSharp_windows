using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Events
{
	internal class PropertyHelper<TBase>
	{
		public bool SetProperty<TData>(TBase instance, ref TData currentPropertyValue, TData newValue, [CallerMemberName] string propertyName = "")
		{
			bool flag = object.Equals(currentPropertyValue, newValue);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				currentPropertyValue = newValue;
				this.NotifyValueChange(instance, propertyName);
				flag2 = true;
			}
			return flag2;
		}

		public void NotifyValueChange(TBase instance, [CallerMemberName] string propertyName = "")
		{
			this.propertyChangedEvent.Notify(instance, propertyName);
		}

		public PropertyHelper<TBase>.PropertyChangedEvent propertyChangedEvent = new PropertyHelper<TBase>.PropertyChangedEvent();

		private interface IEventHolder
		{
			void Invoke(TBase setting, string property);

			bool IsEmpty();
		}

		private class EventHolder<T> : PropertyHelper<TBase>.IEventHolder where T : class, TBase
		{
			[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action<T, string> propertyEvent;

			void PropertyHelper<TBase>.IEventHolder.Invoke(TBase setting, string property)
			{
				Action<T, string> action = this.propertyEvent;
				if (action != null)
				{
					action(setting as T, property);
				}
			}

			bool PropertyHelper<TBase>.IEventHolder.IsEmpty()
			{
				return this.propertyEvent == null;
			}
		}

		public struct PropertyChangedEvent
		{
			public PropertyChangedEvent()
			{
				this.m_Subscriptions = new Dictionary<Type, PropertyHelper<TBase>.IEventHolder>();
			}

			public void Subscribe<TChild>(Action<TChild, string> callback) where TChild : class, TBase
			{
				PropertyHelper<TBase>.IEventHolder eventHolder;
				bool flag = this.m_Subscriptions.TryGetValue(typeof(TChild), out eventHolder);
				PropertyHelper<TBase>.EventHolder<TChild> eventHolder2;
				if (flag)
				{
					eventHolder2 = eventHolder as PropertyHelper<TBase>.EventHolder<TChild>;
				}
				else
				{
					eventHolder2 = new PropertyHelper<TBase>.EventHolder<TChild>();
					this.m_Subscriptions.Add(typeof(TChild), eventHolder2);
				}
				eventHolder2.propertyEvent += callback;
			}

			public void Unsubscribe<TChild>(Action<TChild, string> callback) where TChild : class, TBase
			{
				PropertyHelper<TBase>.IEventHolder eventHolder;
				bool flag = !this.m_Subscriptions.TryGetValue(typeof(TChild), out eventHolder);
				if (!flag)
				{
					PropertyHelper<TBase>.EventHolder<TChild> eventHolder2 = eventHolder as PropertyHelper<TBase>.EventHolder<TChild>;
					eventHolder2.propertyEvent -= callback;
					bool flag2 = eventHolder.IsEmpty();
					if (flag2)
					{
						this.m_Subscriptions.Remove(typeof(TChild));
					}
				}
			}

			public void Notify(TBase instance, [CallerMemberName] string propertyName = "")
			{
				PropertyHelper<TBase>.IEventHolder eventHolder;
				bool flag = this.m_Subscriptions.TryGetValue(instance.GetType(), out eventHolder);
				if (flag)
				{
					eventHolder.Invoke(instance, propertyName);
				}
			}

			private Dictionary<Type, PropertyHelper<TBase>.IEventHolder> m_Subscriptions;
		}
	}
}
