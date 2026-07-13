using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Editor/Src/Properties/DrivenPropertyManager.h")]
	internal class DrivenPropertyManager
	{
		[Conditional("UNITY_EDITOR")]
		public static void RegisterProperty(Object driver, Object target, string propertyPath)
		{
			DrivenPropertyManager.RegisterPropertyPartial(driver, target, propertyPath);
		}

		[Conditional("UNITY_EDITOR")]
		public static void TryRegisterProperty(Object driver, Object target, string propertyPath)
		{
			DrivenPropertyManager.TryRegisterPropertyPartial(driver, target, propertyPath);
		}

		[Conditional("UNITY_EDITOR")]
		public static void UnregisterProperty(Object driver, Object target, string propertyPath)
		{
			DrivenPropertyManager.UnregisterPropertyPartial(driver, target, propertyPath);
		}

		[Conditional("UNITY_EDITOR")]
		[NativeConditional("UNITY_EDITOR")]
		[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
		public static void UnregisterProperties([NotNull] Object driver)
		{
			if (driver == null)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(driver);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			DrivenPropertyManager.UnregisterProperties_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
		private unsafe static void RegisterPropertyPartial([NotNull] Object driver, [NotNull] Object target, [NotNull] string propertyPath)
		{
			if (driver == null)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			if (target == null)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			if (propertyPath == null)
			{
				ThrowHelper.ThrowArgumentNullException(propertyPath, "propertyPath");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(driver);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(driver, "driver");
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Object>(target);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(target, "target");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = propertyPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				DrivenPropertyManager.RegisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
		[NativeConditional("UNITY_EDITOR")]
		private unsafe static void TryRegisterPropertyPartial([NotNull] Object driver, [NotNull] Object target, [NotNull] string propertyPath)
		{
			if (driver == null)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			if (target == null)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			if (propertyPath == null)
			{
				ThrowHelper.ThrowArgumentNullException(propertyPath, "propertyPath");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(driver);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(driver, "driver");
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Object>(target);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(target, "target");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = propertyPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				DrivenPropertyManager.TryRegisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
		private unsafe static void UnregisterPropertyPartial([NotNull] Object driver, [NotNull] Object target, [NotNull] string propertyPath)
		{
			if (driver == null)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			if (target == null)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			if (propertyPath == null)
			{
				ThrowHelper.ThrowArgumentNullException(propertyPath, "propertyPath");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(driver);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(driver, "driver");
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Object>(target);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(target, "target");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = propertyPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				DrivenPropertyManager.UnregisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterProperties_Injected(IntPtr driver);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterPropertyPartial_Injected(IntPtr driver, IntPtr target, ref ManagedSpanWrapper propertyPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TryRegisterPropertyPartial_Injected(IntPtr driver, IntPtr target, ref ManagedSpanWrapper propertyPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterPropertyPartial_Injected(IntPtr driver, IntPtr target, ref ManagedSpanWrapper propertyPath);
	}
}
