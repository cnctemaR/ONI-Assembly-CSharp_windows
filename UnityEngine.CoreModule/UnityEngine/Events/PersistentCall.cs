using System;
using System.Reflection;
using UnityEngine.Serialization;

namespace UnityEngine.Events
{
	[Serializable]
	internal class PersistentCall
	{
		public Object target
		{
			get
			{
				return this.m_Target;
			}
		}

		public string methodName
		{
			get
			{
				return this.m_MethodName;
			}
		}

		public PersistentListenerMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		public ArgumentCache arguments
		{
			get
			{
				return this.m_Arguments;
			}
		}

		public UnityEventCallState callState
		{
			get
			{
				return this.m_CallState;
			}
			set
			{
				this.m_CallState = value;
			}
		}

		public bool IsValid()
		{
			return this.target != null && !string.IsNullOrEmpty(this.methodName);
		}

		public BaseInvokableCall GetRuntimeCall(UnityEventBase theEvent)
		{
			bool flag = this.m_CallState == UnityEventCallState.Off || theEvent == null;
			BaseInvokableCall baseInvokableCall;
			if (flag)
			{
				baseInvokableCall = null;
			}
			else
			{
				MethodInfo methodInfo = theEvent.FindMethod(this);
				bool flag2 = methodInfo == null;
				if (flag2)
				{
					baseInvokableCall = null;
				}
				else
				{
					switch (this.m_Mode)
					{
					case PersistentListenerMode.EventDefined:
						baseInvokableCall = theEvent.GetDelegate(this.target, methodInfo);
						break;
					case PersistentListenerMode.Void:
						baseInvokableCall = new InvokableCall(this.target, methodInfo);
						break;
					case PersistentListenerMode.Object:
						baseInvokableCall = PersistentCall.GetObjectCall(this.target, methodInfo, this.m_Arguments);
						break;
					case PersistentListenerMode.Int:
						baseInvokableCall = new CachedInvokableCall<int>(this.target, methodInfo, this.m_Arguments.intArgument);
						break;
					case PersistentListenerMode.Float:
						baseInvokableCall = new CachedInvokableCall<float>(this.target, methodInfo, this.m_Arguments.floatArgument);
						break;
					case PersistentListenerMode.String:
						baseInvokableCall = new CachedInvokableCall<string>(this.target, methodInfo, this.m_Arguments.stringArgument);
						break;
					case PersistentListenerMode.Bool:
						baseInvokableCall = new CachedInvokableCall<bool>(this.target, methodInfo, this.m_Arguments.boolArgument);
						break;
					default:
						baseInvokableCall = null;
						break;
					}
				}
			}
			return baseInvokableCall;
		}

		private static BaseInvokableCall GetObjectCall(Object target, MethodInfo method, ArgumentCache arguments)
		{
			Type type = typeof(Object);
			bool flag = !string.IsNullOrEmpty(arguments.unityObjectArgumentAssemblyTypeName);
			if (flag)
			{
				type = Type.GetType(arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(Object);
			}
			Type typeFromHandle = typeof(CachedInvokableCall<>);
			Type type2 = typeFromHandle.MakeGenericType(new Type[] { type });
			ConstructorInfo constructor = type2.GetConstructor(new Type[]
			{
				typeof(Object),
				typeof(MethodInfo),
				type
			});
			Object @object = arguments.unityObjectArgument;
			bool flag2 = @object != null && !type.IsAssignableFrom(@object.GetType());
			if (flag2)
			{
				@object = null;
			}
			return constructor.Invoke(new object[] { target, method, @object }) as BaseInvokableCall;
		}

		public void RegisterPersistentListener(Object ttarget, string mmethodName)
		{
			this.m_Target = ttarget;
			this.m_MethodName = mmethodName;
		}

		public void UnregisterPersistentListener()
		{
			this.m_MethodName = string.Empty;
			this.m_Target = null;
		}

		[SerializeField]
		[FormerlySerializedAs("instance")]
		private Object m_Target;

		[SerializeField]
		[FormerlySerializedAs("methodName")]
		private string m_MethodName;

		[SerializeField]
		[FormerlySerializedAs("mode")]
		private PersistentListenerMode m_Mode = PersistentListenerMode.EventDefined;

		[FormerlySerializedAs("arguments")]
		[SerializeField]
		private ArgumentCache m_Arguments = new ArgumentCache();

		[FormerlySerializedAs("m_Enabled")]
		[FormerlySerializedAs("enabled")]
		[SerializeField]
		private UnityEventCallState m_CallState = UnityEventCallState.RuntimeOnly;
	}
}
