using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;

namespace System.Runtime.Serialization
{
	internal class SerializationEvents
	{
		private List<MethodInfo> GetMethodsWithAttribute(Type attribute, Type t)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			Type type = t;
			while (type != null && type != typeof(object))
			{
				foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo.IsDefined(attribute, false))
					{
						list.Add(methodInfo);
					}
				}
				type = type.BaseType;
			}
			list.Reverse();
			if (list.Count != 0)
			{
				return list;
			}
			return null;
		}

		internal SerializationEvents(Type t)
		{
			this.m_OnSerializingMethods = this.GetMethodsWithAttribute(typeof(OnSerializingAttribute), t);
			this.m_OnSerializedMethods = this.GetMethodsWithAttribute(typeof(OnSerializedAttribute), t);
			this.m_OnDeserializingMethods = this.GetMethodsWithAttribute(typeof(OnDeserializingAttribute), t);
			this.m_OnDeserializedMethods = this.GetMethodsWithAttribute(typeof(OnDeserializedAttribute), t);
		}

		internal bool HasOnSerializingEvents
		{
			get
			{
				return this.m_OnSerializingMethods != null || this.m_OnSerializedMethods != null;
			}
		}

		[SecuritySafeCritical]
		internal void InvokeOnSerializing(object obj, StreamingContext context)
		{
			if (this.m_OnSerializingMethods != null)
			{
				SerializationEventHandler serializationEventHandler = null;
				foreach (MethodInfo methodInfo in this.m_OnSerializingMethods)
				{
					SerializationEventHandler serializationEventHandler2 = (SerializationEventHandler)Delegate.CreateDelegateNoSecurityCheck((RuntimeType)typeof(SerializationEventHandler), obj, methodInfo);
					serializationEventHandler = (SerializationEventHandler)Delegate.Combine(serializationEventHandler, serializationEventHandler2);
				}
				serializationEventHandler(context);
			}
		}

		[SecuritySafeCritical]
		internal void InvokeOnDeserializing(object obj, StreamingContext context)
		{
			if (this.m_OnDeserializingMethods != null)
			{
				SerializationEventHandler serializationEventHandler = null;
				foreach (MethodInfo methodInfo in this.m_OnDeserializingMethods)
				{
					SerializationEventHandler serializationEventHandler2 = (SerializationEventHandler)Delegate.CreateDelegateNoSecurityCheck((RuntimeType)typeof(SerializationEventHandler), obj, methodInfo);
					serializationEventHandler = (SerializationEventHandler)Delegate.Combine(serializationEventHandler, serializationEventHandler2);
				}
				serializationEventHandler(context);
			}
		}

		[SecuritySafeCritical]
		internal void InvokeOnDeserialized(object obj, StreamingContext context)
		{
			if (this.m_OnDeserializedMethods != null)
			{
				SerializationEventHandler serializationEventHandler = null;
				foreach (MethodInfo methodInfo in this.m_OnDeserializedMethods)
				{
					SerializationEventHandler serializationEventHandler2 = (SerializationEventHandler)Delegate.CreateDelegateNoSecurityCheck((RuntimeType)typeof(SerializationEventHandler), obj, methodInfo);
					serializationEventHandler = (SerializationEventHandler)Delegate.Combine(serializationEventHandler, serializationEventHandler2);
				}
				serializationEventHandler(context);
			}
		}

		[SecurityCritical]
		internal SerializationEventHandler AddOnSerialized(object obj, SerializationEventHandler handler)
		{
			if (this.m_OnSerializedMethods != null)
			{
				foreach (MethodInfo methodInfo in this.m_OnSerializedMethods)
				{
					SerializationEventHandler serializationEventHandler = (SerializationEventHandler)Delegate.CreateDelegateNoSecurityCheck((RuntimeType)typeof(SerializationEventHandler), obj, methodInfo);
					handler = (SerializationEventHandler)Delegate.Combine(handler, serializationEventHandler);
				}
			}
			return handler;
		}

		[SecurityCritical]
		internal SerializationEventHandler AddOnDeserialized(object obj, SerializationEventHandler handler)
		{
			if (this.m_OnDeserializedMethods != null)
			{
				foreach (MethodInfo methodInfo in this.m_OnDeserializedMethods)
				{
					SerializationEventHandler serializationEventHandler = (SerializationEventHandler)Delegate.CreateDelegateNoSecurityCheck((RuntimeType)typeof(SerializationEventHandler), obj, methodInfo);
					handler = (SerializationEventHandler)Delegate.Combine(handler, serializationEventHandler);
				}
			}
			return handler;
		}

		private List<MethodInfo> m_OnSerializingMethods;

		private List<MethodInfo> m_OnSerializedMethods;

		private List<MethodInfo> m_OnDeserializingMethods;

		private List<MethodInfo> m_OnDeserializedMethods;
	}
}
