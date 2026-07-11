using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono;

namespace System.Reflection
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_EventInfo))]
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class EventInfo : MemberInfo, _EventInfo
	{
		public abstract EventAttributes Attributes { get; }

		public virtual Type EventHandlerType
		{
			get
			{
				ParameterInfo[] parametersInternal = this.GetAddMethod(true).GetParametersInternal();
				if (parametersInternal.Length != 0)
				{
					return parametersInternal[0].ParameterType;
				}
				return null;
			}
		}

		public virtual bool IsMulticast
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
				return (this.Attributes & EventAttributes.SpecialName) > EventAttributes.None;
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
		public virtual void AddEventHandler(object target, Delegate handler)
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
			return EmptyArray<MethodInfo>.Value;
		}

		public MethodInfo[] GetOtherMethods()
		{
			return this.GetOtherMethods(false);
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public virtual void RemoveEventHandler(object target, Delegate handler)
		{
			MethodInfo removeMethod = this.GetRemoveMethod();
			if (removeMethod == null)
			{
				throw new InvalidOperationException("Cannot remove a handler to an event that doesn't have a visible remove method");
			}
			removeMethod.Invoke(target, new object[] { handler });
		}

		public override bool Equals(object obj)
		{
			return obj == this;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(EventInfo left, EventInfo right)
		{
			return left == right || (!((left == null) ^ (right == null)) && left.Equals(right));
		}

		public static bool operator !=(EventInfo left, EventInfo right)
		{
			return left != right && (((left == null) ^ (right == null)) || !left.Equals(right));
		}

		void _EventInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		Type _EventInfo.GetType()
		{
			return base.GetType();
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
				array = new Type[] { method.GetParametersInternal()[0].ParameterType };
				type = typeof(EventInfo.StaticAddEvent<>);
				text = "StaticAddEventAdapterFrame";
			}
			else
			{
				array = new Type[]
				{
					method.DeclaringType,
					method.GetParametersInternal()[0].ParameterType
				};
				type = typeof(EventInfo.AddEvent<, >);
				text = "AddEventFrame";
			}
			object obj = Delegate.CreateDelegate(type.MakeGenericType(array), method);
			MethodInfo methodInfo = typeof(EventInfo).GetMethod(text, BindingFlags.Static | BindingFlags.NonPublic);
			methodInfo = methodInfo.MakeGenericMethod(array);
			return (EventInfo.AddEventAdapter)Delegate.CreateDelegate(typeof(EventInfo.AddEventAdapter), obj, methodInfo, true);
		}

		public virtual MethodInfo AddMethod
		{
			get
			{
				return this.GetAddMethod(true);
			}
		}

		public virtual MethodInfo RaiseMethod
		{
			get
			{
				return this.GetRaiseMethod(true);
			}
		}

		public virtual MethodInfo RemoveMethod
		{
			get
			{
				return this.GetRemoveMethod(true);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern EventInfo internal_from_handle_type(IntPtr event_handle, IntPtr type_handle);

		internal static EventInfo GetEventFromHandle(RuntimeEventHandle handle)
		{
			if (handle.Value == IntPtr.Zero)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			return EventInfo.internal_from_handle_type(handle.Value, IntPtr.Zero);
		}

		internal static EventInfo GetEventFromHandle(RuntimeEventHandle handle, RuntimeTypeHandle reflectedType)
		{
			if (handle.Value == IntPtr.Zero)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			EventInfo eventInfo = EventInfo.internal_from_handle_type(handle.Value, reflectedType.Value);
			if (eventInfo == null)
			{
				throw new ArgumentException("The event handle and the type handle are incompatible.");
			}
			return eventInfo;
		}

		private EventInfo.AddEventAdapter cached_add_event;

		private delegate void AddEventAdapter(object _this, Delegate dele);

		private delegate void AddEvent<T, D>(T _this, D dele);

		private delegate void StaticAddEvent<D>(D dele);
	}
}
