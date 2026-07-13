using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[ExtensionOfNativeClass]
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
	[NativeHeader("Runtime/Scripting/DelayedCallUtility.h")]
	public class MonoBehaviour : Behaviour
	{
		public CancellationToken destroyCancellationToken
		{
			get
			{
				bool flag = this == null;
				if (flag)
				{
					throw new MissingReferenceException("DestroyCancellation token should be called atleast once before destroying the monobehaviour object");
				}
				bool flag2 = this.m_CancellationTokenSource == null;
				if (flag2)
				{
					this.m_CancellationTokenSource = new CancellationTokenSource();
					this.OnCancellationTokenCreated();
				}
				return this.m_CancellationTokenSource.Token;
			}
		}

		[RequiredByNativeCode]
		private void RaiseCancellation()
		{
			CancellationTokenSource cancellationTokenSource = this.m_CancellationTokenSource;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
		}

		public bool IsInvoking()
		{
			return MonoBehaviour.Internal_IsInvokingAll(this);
		}

		public void CancelInvoke()
		{
			MonoBehaviour.Internal_CancelInvokeAll(this);
		}

		public void Invoke(string methodName, float time)
		{
			MonoBehaviour.InvokeDelayed(this, methodName, time, 0f);
		}

		public void InvokeRepeating(string methodName, float time, float repeatRate)
		{
			bool flag = repeatRate <= 1E-05f && repeatRate != 0f;
			if (flag)
			{
				throw new UnityException("Invoke repeat rate has to be larger than 0.00001F");
			}
			MonoBehaviour.InvokeDelayed(this, methodName, time, repeatRate);
		}

		public void CancelInvoke(string methodName)
		{
			MonoBehaviour.CancelInvoke(this, methodName);
		}

		public bool IsInvoking(string methodName)
		{
			return MonoBehaviour.IsInvoking(this, methodName);
		}

		[ExcludeFromDocs]
		public Coroutine StartCoroutine(string methodName)
		{
			object obj = null;
			return this.StartCoroutine(methodName, obj);
		}

		public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
		{
			bool flag = string.IsNullOrEmpty(methodName);
			if (flag)
			{
				throw new NullReferenceException("methodName is null or empty");
			}
			bool flag2 = !MonoBehaviour.IsObjectMonoBehaviour(this);
			if (flag2)
			{
				throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
			}
			return this.StartCoroutineManaged(methodName, value);
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			bool flag = routine == null;
			if (flag)
			{
				throw new NullReferenceException("routine is null");
			}
			bool flag2 = !MonoBehaviour.IsObjectMonoBehaviour(this);
			if (flag2)
			{
				throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
			}
			return this.StartCoroutineManaged2(routine);
		}

		[Obsolete("StartCoroutine_Auto has been deprecated. Use StartCoroutine instead (UnityUpgradable) -> StartCoroutine([mscorlib] System.Collections.IEnumerator)", false)]
		public Coroutine StartCoroutine_Auto(IEnumerator routine)
		{
			return this.StartCoroutine(routine);
		}

		public void StopCoroutine(IEnumerator routine)
		{
			bool flag = routine == null;
			if (flag)
			{
				throw new NullReferenceException("routine is null");
			}
			bool flag2 = !MonoBehaviour.IsObjectMonoBehaviour(this);
			if (flag2)
			{
				throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
			}
			this.StopCoroutineFromEnumeratorManaged(routine);
		}

		public void StopCoroutine(Coroutine routine)
		{
			bool flag = routine == null;
			if (flag)
			{
				throw new NullReferenceException("routine is null");
			}
			bool flag2 = !MonoBehaviour.IsObjectMonoBehaviour(this);
			if (flag2)
			{
				throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
			}
			this.StopCoroutineManaged(routine);
		}

		public unsafe void StopCoroutine(string methodName)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				MonoBehaviour.StopCoroutine_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void StopAllCoroutines()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MonoBehaviour.StopAllCoroutines_Injected(intPtr);
		}

		public bool useGUILayout
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MonoBehaviour.get_useGUILayout_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				MonoBehaviour.set_useGUILayout_Injected(intPtr, value);
			}
		}

		public bool didStart
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MonoBehaviour.get_didStart_Injected(intPtr);
			}
		}

		public bool didAwake
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MonoBehaviour.get_didAwake_Injected(intPtr);
			}
		}

		public static void print(object message)
		{
			Debug.Log(message);
		}

		[FreeFunction("CancelInvoke")]
		private static void Internal_CancelInvokeAll([NotNull] MonoBehaviour self)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			MonoBehaviour.Internal_CancelInvokeAll_Injected(intPtr);
		}

		[FreeFunction("IsInvoking")]
		private static bool Internal_IsInvokingAll([NotNull] MonoBehaviour self)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(self);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			return MonoBehaviour.Internal_IsInvokingAll_Injected(intPtr);
		}

		[FreeFunction]
		private unsafe static void InvokeDelayed([NotNull] MonoBehaviour self, string methodName, float time, float repeatRate)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(self);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(self, "self");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				MonoBehaviour.InvokeDelayed_Injected(intPtr, ref managedSpanWrapper, time, repeatRate);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction]
		private unsafe static void CancelInvoke([NotNull] MonoBehaviour self, string methodName)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(self);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(self, "self");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				MonoBehaviour.CancelInvoke_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction]
		private unsafe static bool IsInvoking([NotNull] MonoBehaviour self, string methodName)
		{
			if (self == null)
			{
				ThrowHelper.ThrowArgumentNullException(self, "self");
			}
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(self);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(self, "self");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = MonoBehaviour.IsInvoking_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction]
		private static bool IsObjectMonoBehaviour([NotNull] Object obj)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(obj);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			return MonoBehaviour.IsObjectMonoBehaviour_Injected(intPtr);
		}

		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private unsafe Coroutine StartCoroutineManaged(string methodName, object value)
		{
			Coroutine coroutine;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				coroutine = MonoBehaviour.StartCoroutineManaged_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return coroutine;
		}

		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		private Coroutine StartCoroutineManaged2(IEnumerator enumerator)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MonoBehaviour.StartCoroutineManaged2_Injected(intPtr, enumerator);
		}

		private void StopCoroutineManaged(Coroutine routine)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MonoBehaviour.StopCoroutineManaged_Injected(intPtr, (routine == null) ? ((IntPtr)0) : Coroutine.BindingsMarshaller.ConvertToNative(routine));
		}

		private void StopCoroutineFromEnumeratorManaged(IEnumerator routine)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MonoBehaviour.StopCoroutineFromEnumeratorManaged_Injected(intPtr, routine);
		}

		internal string GetScriptClassName()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				MonoBehaviour.GetScriptClassName_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		private void OnCancellationTokenCreated()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MonoBehaviour>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MonoBehaviour.OnCancellationTokenCreated_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopCoroutine_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopAllCoroutines_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useGUILayout_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useGUILayout_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_didStart_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_didAwake_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CancelInvokeAll_Injected(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsInvokingAll_Injected(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InvokeDelayed_Injected(IntPtr self, ref ManagedSpanWrapper methodName, float time, float repeatRate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CancelInvoke_Injected(IntPtr self, ref ManagedSpanWrapper methodName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInvoking_Injected(IntPtr self, ref ManagedSpanWrapper methodName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsObjectMonoBehaviour_Injected(IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Coroutine StartCoroutineManaged_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, object value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Coroutine StartCoroutineManaged2_Injected(IntPtr _unity_self, IEnumerator enumerator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopCoroutineManaged_Injected(IntPtr _unity_self, IntPtr routine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopCoroutineFromEnumeratorManaged_Injected(IntPtr _unity_self, IEnumerator routine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetScriptClassName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OnCancellationTokenCreated_Injected(IntPtr _unity_self);

		private CancellationTokenSource m_CancellationTokenSource;
	}
}
