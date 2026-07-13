using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace UnityEngine.Events
{
	[UsedByNativeCode]
	[Serializable]
	public abstract class UnityEventBase : ISerializationCallbackReceiver
	{
		protected UnityEventBase()
		{
			this.m_Calls = new InvokableCallList();
			this.m_PersistentCalls = new PersistentCallGroup();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.DirtyPersistentCalls();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.DirtyPersistentCalls();
		}

		protected MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return this.FindMethod_Impl(name, targetObj.GetType());
		}

		protected abstract MethodInfo FindMethod_Impl(string name, Type targetObjType);

		internal abstract BaseInvokableCall GetDelegate(object target, MethodInfo theFunction);

		internal MethodInfo FindMethod(PersistentCall call)
		{
			Type type = typeof(Object);
			bool flag = !string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName);
			if (flag)
			{
				type = Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(Object);
			}
			Type type2 = ((call.target != null) ? call.target.GetType() : Type.GetType(call.targetAssemblyTypeName, false));
			return this.FindMethod(call.methodName, type2, call.mode, type);
		}

		internal MethodInfo FindMethod(string name, Type listenerType, PersistentListenerMode mode, Type argumentType)
		{
			MethodInfo methodInfo;
			switch (mode)
			{
			case PersistentListenerMode.EventDefined:
				methodInfo = this.FindMethod_Impl(name, listenerType);
				break;
			case PersistentListenerMode.Void:
				methodInfo = UnityEventBase.GetValidMethodInfo(listenerType, name, new Type[0]);
				break;
			case PersistentListenerMode.Object:
				methodInfo = UnityEventBase.GetValidMethodInfo(listenerType, name, new Type[] { argumentType ?? typeof(Object) });
				break;
			case PersistentListenerMode.Int:
				methodInfo = UnityEventBase.GetValidMethodInfo(listenerType, name, new Type[] { typeof(int) });
				break;
			case PersistentListenerMode.Float:
				methodInfo = UnityEventBase.GetValidMethodInfo(listenerType, name, new Type[] { typeof(float) });
				break;
			case PersistentListenerMode.String:
				methodInfo = UnityEventBase.GetValidMethodInfo(listenerType, name, new Type[] { typeof(string) });
				break;
			case PersistentListenerMode.Bool:
				methodInfo = UnityEventBase.GetValidMethodInfo(listenerType, name, new Type[] { typeof(bool) });
				break;
			default:
				methodInfo = null;
				break;
			}
			return methodInfo;
		}

		internal int GetCallsCount()
		{
			return this.m_Calls.Count;
		}

		public int GetPersistentEventCount()
		{
			return this.m_PersistentCalls.Count;
		}

		public Object GetPersistentTarget(int index)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener != null) ? listener.target : null;
		}

		public string GetPersistentMethodName(int index)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener != null) ? listener.methodName : string.Empty;
		}

		private void DirtyPersistentCalls()
		{
			this.m_Calls.ClearPersistent();
			this.m_CallsDirty = true;
		}

		private void RebuildPersistentCallsIfNeeded()
		{
			bool callsDirty = this.m_CallsDirty;
			if (callsDirty)
			{
				this.m_PersistentCalls.Initialize(this.m_Calls, this);
				this.m_CallsDirty = false;
			}
		}

		public void SetPersistentListenerState(int index, UnityEventCallState state)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			bool flag = listener != null;
			if (flag)
			{
				listener.callState = state;
			}
			this.DirtyPersistentCalls();
		}

		public UnityEventCallState GetPersistentListenerState(int index)
		{
			bool flag = index < 0 || index > this.m_PersistentCalls.Count;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range of the {1} persistent listeners.", index, this.GetPersistentEventCount()));
			}
			return this.m_PersistentCalls.GetListener(index).callState;
		}

		protected void AddListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.AddListener(this.GetDelegate(targetObj, method));
		}

		internal void AddCall(BaseInvokableCall call)
		{
			this.m_Calls.AddListener(call);
		}

		protected void RemoveListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.RemoveListener(targetObj, method);
		}

		public void RemoveAllListeners()
		{
			this.m_Calls.Clear();
		}

		internal List<BaseInvokableCall> PrepareInvoke()
		{
			this.RebuildPersistentCallsIfNeeded();
			return this.m_Calls.PrepareInvoke();
		}

		protected void Invoke(object[] parameters)
		{
			List<BaseInvokableCall> list = this.PrepareInvoke();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Invoke(parameters);
			}
		}

		public override string ToString()
		{
			return base.ToString() + " " + base.GetType().FullName;
		}

		public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
		{
			return UnityEventBase.GetValidMethodInfo(obj.GetType(), functionName, argumentTypes);
		}

		public static MethodInfo GetValidMethodInfo(Type objectType, string functionName, Type[] argumentTypes)
		{
			while (objectType != typeof(object) && objectType != null)
			{
				MethodInfo method = objectType.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
				bool flag = method != null;
				if (flag)
				{
					ParameterInfo[] parameters = method.GetParameters();
					bool flag2 = true;
					int num = 0;
					foreach (ParameterInfo parameterInfo in parameters)
					{
						Type type = argumentTypes[num];
						Type parameterType = parameterInfo.ParameterType;
						flag2 = type.IsPrimitive == parameterType.IsPrimitive;
						bool flag3 = !flag2;
						if (flag3)
						{
							break;
						}
						num++;
					}
					bool flag4 = flag2;
					if (flag4)
					{
						return method;
					}
				}
				objectType = objectType.BaseType;
			}
			return null;
		}

		private InvokableCallList m_Calls;

		[SerializeField]
		[FormerlySerializedAs("m_PersistentListeners")]
		private PersistentCallGroup m_PersistentCalls;

		private bool m_CallsDirty = true;
	}
}
