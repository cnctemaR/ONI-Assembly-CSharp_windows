using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[Serializable]
	internal struct AnimationEventBlittable : IDisposable
	{
		internal static AnimationEventBlittable FromAnimationEvent(AnimationEvent animationEvent)
		{
			bool flag = AnimationEventBlittable.s_handlePool == null;
			if (flag)
			{
				AnimationEventBlittable.s_handlePool = new GCHandlePool();
			}
			GCHandlePool gchandlePool = AnimationEventBlittable.s_handlePool;
			return new AnimationEventBlittable
			{
				m_Time = animationEvent.m_Time,
				m_FunctionName = gchandlePool.AllocHandleIfNotNull(animationEvent.m_FunctionName),
				m_StringParameter = gchandlePool.AllocHandleIfNotNull(animationEvent.m_StringParameter),
				m_ObjectReferenceParameter = gchandlePool.AllocHandleIfNotNull(animationEvent.m_ObjectReferenceParameter),
				m_FloatParameter = animationEvent.m_FloatParameter,
				m_IntParameter = animationEvent.m_IntParameter,
				m_MessageOptions = animationEvent.m_MessageOptions,
				m_Source = animationEvent.m_Source,
				m_StateSender = gchandlePool.AllocHandleIfNotNull(animationEvent.m_StateSender),
				m_AnimatorStateInfo = animationEvent.m_AnimatorStateInfo,
				m_AnimatorClipInfo = animationEvent.m_AnimatorClipInfo
			};
		}

		internal unsafe static void FromAnimationEvents(AnimationEvent[] animationEvents, AnimationEventBlittable* animationEventBlittables)
		{
			bool flag = AnimationEventBlittable.s_handlePool == null;
			if (flag)
			{
				AnimationEventBlittable.s_handlePool = new GCHandlePool();
			}
			GCHandlePool gchandlePool = AnimationEventBlittable.s_handlePool;
			AnimationEventBlittable* ptr = animationEventBlittables;
			foreach (AnimationEvent animationEvent in animationEvents)
			{
				ptr->m_Time = animationEvent.m_Time;
				ptr->m_FunctionName = gchandlePool.AllocHandleIfNotNull(animationEvent.m_FunctionName);
				ptr->m_StringParameter = gchandlePool.AllocHandleIfNotNull(animationEvent.m_StringParameter);
				ptr->m_ObjectReferenceParameter = gchandlePool.AllocHandleIfNotNull(animationEvent.m_ObjectReferenceParameter);
				ptr->m_FloatParameter = animationEvent.m_FloatParameter;
				ptr->m_IntParameter = animationEvent.m_IntParameter;
				ptr->m_MessageOptions = animationEvent.m_MessageOptions;
				ptr->m_Source = animationEvent.m_Source;
				ptr->m_StateSender = gchandlePool.AllocHandleIfNotNull(animationEvent.m_StateSender);
				ptr->m_AnimatorStateInfo = animationEvent.m_AnimatorStateInfo;
				ptr->m_AnimatorClipInfo = animationEvent.m_AnimatorClipInfo;
				ptr++;
			}
		}

		[RequiredByNativeCode]
		internal unsafe static AnimationEvent PointerToAnimationEvent(IntPtr animationEventBlittable)
		{
			return AnimationEventBlittable.ToAnimationEvent(*(AnimationEventBlittable*)(void*)animationEventBlittable);
		}

		internal unsafe static AnimationEvent[] PointerToAnimationEvents(IntPtr animationEventBlittableArray, int size)
		{
			AnimationEvent[] array = new AnimationEvent[size];
			AnimationEventBlittable* ptr = (AnimationEventBlittable*)(void*)animationEventBlittableArray;
			for (int i = 0; i < size; i++)
			{
				array[i] = AnimationEventBlittable.PointerToAnimationEvent((IntPtr)((void*)(ptr + i)));
			}
			return array;
		}

		internal unsafe static void DisposeEvents(IntPtr animationEventBlittableArray, int size)
		{
			AnimationEventBlittable* ptr = (AnimationEventBlittable*)(void*)animationEventBlittableArray;
			for (int i = 0; i < size; i++)
			{
				ptr[i].Dispose();
			}
			AnimationEventBlittable.FreeEventsInternal(animationEventBlittableArray);
		}

		[FreeFunction(Name = "AnimationClipBindings::FreeEventsInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeEventsInternal(IntPtr value);

		internal static AnimationEvent ToAnimationEvent(AnimationEventBlittable animationEventBlittable)
		{
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.m_Time = animationEventBlittable.m_Time;
			bool flag = animationEventBlittable.m_FunctionName != IntPtr.Zero;
			if (flag)
			{
				animationEvent.m_FunctionName = (string)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_FunctionName).Target;
			}
			bool flag2 = animationEventBlittable.m_StringParameter != IntPtr.Zero;
			if (flag2)
			{
				animationEvent.m_StringParameter = (string)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_StringParameter).Target;
			}
			bool flag3 = animationEventBlittable.m_ObjectReferenceParameter != IntPtr.Zero;
			if (flag3)
			{
				animationEvent.m_ObjectReferenceParameter = (Object)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_ObjectReferenceParameter).Target;
			}
			animationEvent.m_FloatParameter = animationEventBlittable.m_FloatParameter;
			animationEvent.m_IntParameter = animationEventBlittable.m_IntParameter;
			animationEvent.m_MessageOptions = animationEventBlittable.m_MessageOptions;
			animationEvent.m_Source = animationEventBlittable.m_Source;
			bool flag4 = animationEventBlittable.m_StateSender != IntPtr.Zero;
			if (flag4)
			{
				animationEvent.m_StateSender = (AnimationState)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_StateSender).Target;
			}
			animationEvent.m_AnimatorStateInfo = animationEventBlittable.m_AnimatorStateInfo;
			animationEvent.m_AnimatorClipInfo = animationEventBlittable.m_AnimatorClipInfo;
			return animationEvent;
		}

		public unsafe void Dispose()
		{
			bool flag = AnimationEventBlittable.s_handlePool == null;
			if (flag)
			{
				AnimationEventBlittable.s_handlePool = new GCHandlePool();
			}
			GCHandlePool gchandlePool = AnimationEventBlittable.s_handlePool;
			bool flag2 = this.m_FunctionName != IntPtr.Zero;
			if (flag2)
			{
				gchandlePool.Free(*UnsafeUtility.As<IntPtr, GCHandle>(ref this.m_FunctionName));
			}
			bool flag3 = this.m_StringParameter != IntPtr.Zero;
			if (flag3)
			{
				gchandlePool.Free(*UnsafeUtility.As<IntPtr, GCHandle>(ref this.m_StringParameter));
			}
			bool flag4 = this.m_ObjectReferenceParameter != IntPtr.Zero;
			if (flag4)
			{
				gchandlePool.Free(*UnsafeUtility.As<IntPtr, GCHandle>(ref this.m_ObjectReferenceParameter));
			}
			bool flag5 = this.m_StateSender != IntPtr.Zero;
			if (flag5)
			{
				gchandlePool.Free(*UnsafeUtility.As<IntPtr, GCHandle>(ref this.m_StateSender));
			}
		}

		internal float m_Time;

		internal IntPtr m_FunctionName;

		internal IntPtr m_StringParameter;

		internal IntPtr m_ObjectReferenceParameter;

		internal float m_FloatParameter;

		internal int m_IntParameter;

		internal int m_MessageOptions;

		internal AnimationEventSource m_Source;

		internal IntPtr m_StateSender;

		internal AnimatorStateInfo m_AnimatorStateInfo;

		internal AnimatorClipInfo m_AnimatorClipInfo;

		[ThreadStatic]
		private static GCHandlePool s_handlePool;
	}
}
