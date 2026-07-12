using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Serialization
{
	[NativeHeader("Runtime/Serialize/ManagedReferenceUtility.h")]
	public sealed class ManagedReferenceUtility
	{
		[NativeMethod("SetManagedReferenceIdForObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetManagedReferenceIdForObjectInternal(Object obj, object scriptObj, long refId);

		public static bool SetManagedReferenceIdForObject(Object obj, object scriptObj, long refId)
		{
			bool flag = scriptObj == null;
			bool flag2;
			if (flag)
			{
				flag2 = refId == -2L;
			}
			else
			{
				Type type = scriptObj.GetType();
				bool flag3 = type == typeof(Object) || type.IsSubclassOf(typeof(Object));
				if (flag3)
				{
					throw new InvalidOperationException("Cannot assign an object deriving from UnityEngine.Object to a managed reference. This is not supported.");
				}
				flag2 = ManagedReferenceUtility.SetManagedReferenceIdForObjectInternal(obj, scriptObj, refId);
			}
			return flag2;
		}

		[NativeMethod("GetManagedReferenceIdForObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetManagedReferenceIdForObjectInternal(Object obj, object scriptObj);

		public static long GetManagedReferenceIdForObject(Object obj, object scriptObj)
		{
			return ManagedReferenceUtility.GetManagedReferenceIdForObjectInternal(obj, scriptObj);
		}

		[NativeMethod("GetManagedReference")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetManagedReferenceInternal(Object obj, long id);

		public static object GetManagedReference(Object obj, long id)
		{
			return ManagedReferenceUtility.GetManagedReferenceInternal(obj, id);
		}

		[NativeMethod("GetManagedReferenceIds")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long[] GetManagedReferenceIdsForObjectInternal(Object obj);

		public static long[] GetManagedReferenceIds(Object obj)
		{
			return ManagedReferenceUtility.GetManagedReferenceIdsForObjectInternal(obj);
		}

		public const long RefIdUnknown = -1L;

		public const long RefIdNull = -2L;
	}
}
