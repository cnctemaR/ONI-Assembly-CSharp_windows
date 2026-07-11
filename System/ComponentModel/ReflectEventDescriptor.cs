using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	internal sealed class ReflectEventDescriptor : EventDescriptor
	{
		public ReflectEventDescriptor(Type componentClass, string name, Type type, Attribute[] attributes)
			: base(name, attributes)
		{
			if (componentClass == null)
			{
				throw new ArgumentException(global::SR.GetString("Null is not a valid value for {0}.", new object[] { "componentClass" }));
			}
			if (type == null || !typeof(Delegate).IsAssignableFrom(type))
			{
				throw new ArgumentException(global::SR.GetString("Invalid type for the {0} event.", new object[] { name }));
			}
			this.componentClass = componentClass;
			this.type = type;
		}

		public ReflectEventDescriptor(Type componentClass, EventInfo eventInfo)
			: base(eventInfo.Name, new Attribute[0])
		{
			if (componentClass == null)
			{
				throw new ArgumentException(global::SR.GetString("Null is not a valid value for {0}.", new object[] { "componentClass" }));
			}
			this.componentClass = componentClass;
			this.realEvent = eventInfo;
		}

		public ReflectEventDescriptor(Type componentType, EventDescriptor oldReflectEventDescriptor, Attribute[] attributes)
			: base(oldReflectEventDescriptor, attributes)
		{
			this.componentClass = componentType;
			this.type = oldReflectEventDescriptor.EventType;
			ReflectEventDescriptor reflectEventDescriptor = oldReflectEventDescriptor as ReflectEventDescriptor;
			if (reflectEventDescriptor != null)
			{
				this.addMethod = reflectEventDescriptor.addMethod;
				this.removeMethod = reflectEventDescriptor.removeMethod;
				this.filledMethods = true;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return this.componentClass;
			}
		}

		public override Type EventType
		{
			get
			{
				this.FillMethods();
				return this.type;
			}
		}

		public override bool IsMulticast
		{
			get
			{
				return typeof(MulticastDelegate).IsAssignableFrom(this.EventType);
			}
		}

		public override void AddEventHandler(object component, Delegate value)
		{
			this.FillMethods();
			if (component != null)
			{
				ISite site = MemberDescriptor.GetSite(component);
				IComponentChangeService componentChangeService = null;
				if (site != null)
				{
					componentChangeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
				}
				if (componentChangeService != null)
				{
					try
					{
						componentChangeService.OnComponentChanging(component, this);
					}
					catch (CheckoutException ex)
					{
						if (ex == CheckoutException.Canceled)
						{
							return;
						}
						throw ex;
					}
				}
				bool flag = false;
				if (site != null && site.DesignMode)
				{
					if (this.EventType != value.GetType())
					{
						throw new ArgumentException(global::SR.GetString("Invalid event handler for the {0} event.", new object[] { this.Name }));
					}
					IDictionaryService dictionaryService = (IDictionaryService)site.GetService(typeof(IDictionaryService));
					if (dictionaryService != null)
					{
						Delegate @delegate = (Delegate)dictionaryService.GetValue(this);
						@delegate = Delegate.Combine(@delegate, value);
						dictionaryService.SetValue(this, @delegate);
						flag = true;
					}
				}
				if (!flag)
				{
					SecurityUtils.MethodInfoInvoke(this.addMethod, component, new object[] { value });
				}
				if (componentChangeService != null)
				{
					componentChangeService.OnComponentChanged(component, this, null, value);
				}
			}
		}

		protected override void FillAttributes(IList attributes)
		{
			this.FillMethods();
			if (this.realEvent != null)
			{
				this.FillEventInfoAttribute(this.realEvent, attributes);
			}
			else
			{
				this.FillSingleMethodAttribute(this.removeMethod, attributes);
				this.FillSingleMethodAttribute(this.addMethod, attributes);
			}
			base.FillAttributes(attributes);
		}

		private void FillEventInfoAttribute(EventInfo realEventInfo, IList attributes)
		{
			string name = realEventInfo.Name;
			BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
			Type type = realEventInfo.ReflectedType;
			int num = 0;
			while (type != typeof(object))
			{
				num++;
				type = type.BaseType;
			}
			if (num > 0)
			{
				type = realEventInfo.ReflectedType;
				Attribute[][] array = new Attribute[num][];
				while (type != typeof(object))
				{
					MemberInfo @event = type.GetEvent(name, bindingFlags);
					if (@event != null)
					{
						array[--num] = ReflectTypeDescriptionProvider.ReflectGetAttributes(@event);
					}
					type = type.BaseType;
				}
				foreach (Attribute[] array3 in array)
				{
					if (array3 != null)
					{
						foreach (Attribute attribute in array3)
						{
							attributes.Add(attribute);
						}
					}
				}
			}
		}

		private void FillMethods()
		{
			if (this.filledMethods)
			{
				return;
			}
			if (this.realEvent != null)
			{
				this.addMethod = this.realEvent.GetAddMethod();
				this.removeMethod = this.realEvent.GetRemoveMethod();
				EventInfo eventInfo = null;
				if (this.addMethod == null || this.removeMethod == null)
				{
					Type baseType = this.componentClass.BaseType;
					while (baseType != null && baseType != typeof(object))
					{
						BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
						EventInfo @event = baseType.GetEvent(this.realEvent.Name, bindingFlags);
						if (@event.GetAddMethod() != null)
						{
							eventInfo = @event;
							break;
						}
					}
				}
				if (eventInfo != null)
				{
					this.addMethod = eventInfo.GetAddMethod();
					this.removeMethod = eventInfo.GetRemoveMethod();
					this.type = eventInfo.EventHandlerType;
				}
				else
				{
					this.type = this.realEvent.EventHandlerType;
				}
			}
			else
			{
				this.realEvent = this.componentClass.GetEvent(this.Name);
				if (this.realEvent != null)
				{
					this.FillMethods();
					return;
				}
				Type[] array = new Type[] { this.type };
				this.addMethod = MemberDescriptor.FindMethod(this.componentClass, "AddOn" + this.Name, array, typeof(void));
				this.removeMethod = MemberDescriptor.FindMethod(this.componentClass, "RemoveOn" + this.Name, array, typeof(void));
				if (this.addMethod == null || this.removeMethod == null)
				{
					throw new ArgumentException(global::SR.GetString("Accessor methods for the {0} event are missing.", new object[] { this.Name }));
				}
			}
			this.filledMethods = true;
		}

		private void FillSingleMethodAttribute(MethodInfo realMethodInfo, IList attributes)
		{
			string name = realMethodInfo.Name;
			BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
			Type type = realMethodInfo.ReflectedType;
			int num = 0;
			while (type != null && type != typeof(object))
			{
				num++;
				type = type.BaseType;
			}
			if (num > 0)
			{
				type = realMethodInfo.ReflectedType;
				Attribute[][] array = new Attribute[num][];
				while (type != null && type != typeof(object))
				{
					MemberInfo method = type.GetMethod(name, bindingFlags);
					if (method != null)
					{
						array[--num] = ReflectTypeDescriptionProvider.ReflectGetAttributes(method);
					}
					type = type.BaseType;
				}
				foreach (Attribute[] array3 in array)
				{
					if (array3 != null)
					{
						foreach (Attribute attribute in array3)
						{
							attributes.Add(attribute);
						}
					}
				}
			}
		}

		public override void RemoveEventHandler(object component, Delegate value)
		{
			this.FillMethods();
			if (component != null)
			{
				ISite site = MemberDescriptor.GetSite(component);
				IComponentChangeService componentChangeService = null;
				if (site != null)
				{
					componentChangeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
				}
				if (componentChangeService != null)
				{
					try
					{
						componentChangeService.OnComponentChanging(component, this);
					}
					catch (CheckoutException ex)
					{
						if (ex == CheckoutException.Canceled)
						{
							return;
						}
						throw ex;
					}
				}
				bool flag = false;
				if (site != null && site.DesignMode)
				{
					IDictionaryService dictionaryService = (IDictionaryService)site.GetService(typeof(IDictionaryService));
					if (dictionaryService != null)
					{
						Delegate @delegate = (Delegate)dictionaryService.GetValue(this);
						@delegate = Delegate.Remove(@delegate, value);
						dictionaryService.SetValue(this, @delegate);
						flag = true;
					}
				}
				if (!flag)
				{
					SecurityUtils.MethodInfoInvoke(this.removeMethod, component, new object[] { value });
				}
				if (componentChangeService != null)
				{
					componentChangeService.OnComponentChanged(component, this, null, value);
				}
			}
		}

		private Type type;

		private readonly Type componentClass;

		private MethodInfo addMethod;

		private MethodInfo removeMethod;

		private EventInfo realEvent;

		private bool filledMethods;
	}
}
