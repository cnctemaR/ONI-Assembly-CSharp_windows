using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_EventInfo))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	public abstract class EventInfo : MemberInfo, _EventInfo
	{
		void _EventInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _EventInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _EventInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _EventInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public abstract EventAttributes Attributes { get; }

		public Type EventHandlerType
		{
			get
			{
				MethodInfo addMethod = this.GetAddMethod(true);
				ParameterInfo[] parameters = addMethod.GetParameters();
				if (parameters.Length > 0)
				{
					return parameters[0].ParameterType;
				}
				return null;
			}
		}

		public bool IsMulticast
		{
			get
			{
				return true;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & EventAttributes.SpecialName) != EventAttributes.None;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Event;
			}
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public void AddEventHandler(object target, Delegate handler)
		{
			if (this.cached_add_event == null)
			{
				MethodInfo addMethod = this.GetAddMethod();
				if (addMethod == null)
				{
					throw new InvalidOperationException("Cannot add a handler to an event that doesn't have a visible add method");
				}
				if (addMethod.DeclaringType.IsValueType)
				{
					if (target == null && !addMethod.IsStatic)
					{
						throw new TargetException("Cannot add a handler to a non static event with a null target");
					}
					addMethod.Invoke(target, new object[] { handler });
					return;
				}
				else
				{
					this.cached_add_event = EventInfo.CreateAddEventDelegate(addMethod);
				}
			}
			this.cached_add_event(target, handler);
		}

		public MethodInfo GetAddMethod()
		{
			return this.GetAddMethod(false);
		}

		public abstract MethodInfo GetAddMethod(bool nonPublic);

		public MethodInfo GetRaiseMethod()
		{
			return this.GetRaiseMethod(false);
		}

		public abstract MethodInfo GetRaiseMethod(bool nonPublic);

		public MethodInfo GetRemoveMethod()
		{
			return this.GetRemoveMethod(false);
		}

		public abstract MethodInfo GetRemoveMethod(bool nonPublic);

		public virtual MethodInfo[] GetOtherMethods(bool nonPublic)
		{
			return new MethodInfo[0];
		}

		public MethodInfo[] GetOtherMethods()
		{
			return this.GetOtherMethods(false);
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public void RemoveEventHandler(object target, Delegate handler)
		{
			MethodInfo removeMethod = this.GetRemoveMethod();
			if (removeMethod == null)
			{
				throw new InvalidOperationException("Cannot remove a handler to an event that doesn't have a visible remove method");
			}
			removeMethod.Invoke(target, new object[] { handler });
		}

		private static void AddEventFrame<T, D>(EventInfo.AddEvent<T, D> addEvent, object obj, object dele)
		{
			if (obj == null)
			{
				throw new TargetException("Cannot add a handler to a non static event with a null target");
			}
			if (!(obj is T))
			{
				throw new TargetException("Object doesn't match target");
			}
			addEvent((T)((object)obj), (D)((object)dele));
		}

		private static void StaticAddEventAdapterFrame<D>(EventInfo.StaticAddEvent<D> addEvent, object obj, object dele)
		{
			addEvent((D)((object)dele));
		}

		private static EventInfo.AddEventAdapter CreateAddEventDelegate(MethodInfo method)
		{
			Type[] array;
			Type type;
			string text;
			if (method.IsStatic)
			{
				array = new Type[] { method.GetParameters()[0].ParameterType };
				type = typeof(EventInfo.StaticAddEvent<>);
				text = "StaticAddEventAdapterFrame";
			}
			else
			{
				array = new Type[]
				{
					method.DeclaringType,
					method.GetParameters()[0].ParameterType
				};
				type = typeof(EventInfo.AddEvent<, >);
				text = "AddEventFrame";
			}
			Type type2 = type.MakeGenericType(array);
			object obj = Delegate.CreateDelegate(type2, method);
			MethodInfo methodInfo = typeof(EventInfo).GetMethod(text, BindingFlags.Static | BindingFlags.NonPublic);
			methodInfo = methodInfo.MakeGenericMethod(array);
			return (EventInfo.AddEventAdapter)Delegate.CreateDelegate(typeof(EventInfo.AddEventAdapter), obj, methodInfo, true);
		}

		virtual Type System.Runtime.InteropServices._EventInfo.GetType()
		{
			return base.GetType();
		}

		private EventInfo.AddEventAdapter cached_add_event;

		private delegate void AddEventAdapter(object _this, Delegate dele);

		private delegate void AddEvent<T, D>(T _this, D dele);

		private delegate void StaticAddEvent<D>(D dele);
	}
}
