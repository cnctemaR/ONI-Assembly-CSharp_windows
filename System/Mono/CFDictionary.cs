using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mono
{
	internal class CFDictionary : CFObject
	{
		static CFDictionary()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				CFDictionary.KeyCallbacks = CFObject.GetIndirect(intPtr, "kCFTypeDictionaryKeyCallBacks");
				CFDictionary.ValueCallbacks = CFObject.GetIndirect(intPtr, "kCFTypeDictionaryValueCallBacks");
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		public CFDictionary(IntPtr handle, bool own)
			: base(handle, own)
		{
		}

		public static CFDictionary FromObjectAndKey(IntPtr obj, IntPtr key)
		{
			return new CFDictionary(CFDictionary.CFDictionaryCreate(IntPtr.Zero, new IntPtr[] { key }, new IntPtr[] { obj }, (IntPtr)1, CFDictionary.KeyCallbacks, CFDictionary.ValueCallbacks), true);
		}

		public static CFDictionary FromKeysAndObjects(IList<Tuple<IntPtr, IntPtr>> items)
		{
			IntPtr[] array = new IntPtr[items.Count];
			IntPtr[] array2 = new IntPtr[items.Count];
			for (int i = 0; i < items.Count; i++)
			{
				array[i] = items[i].Item1;
				array2[i] = items[i].Item2;
			}
			return new CFDictionary(CFDictionary.CFDictionaryCreate(IntPtr.Zero, array, array2, (IntPtr)items.Count, CFDictionary.KeyCallbacks, CFDictionary.ValueCallbacks), true);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDictionaryCreate(IntPtr allocator, IntPtr[] keys, IntPtr[] vals, IntPtr len, IntPtr keyCallbacks, IntPtr valCallbacks);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDictionaryGetValue(IntPtr handle, IntPtr key);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDictionaryCreateCopy(IntPtr allocator, IntPtr handle);

		public CFDictionary Copy()
		{
			return new CFDictionary(CFDictionary.CFDictionaryCreateCopy(IntPtr.Zero, base.Handle), true);
		}

		public CFMutableDictionary MutableCopy()
		{
			return new CFMutableDictionary(CFDictionary.CFDictionaryCreateMutableCopy(IntPtr.Zero, IntPtr.Zero, base.Handle), true);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFDictionaryCreateMutableCopy(IntPtr allocator, IntPtr capacity, IntPtr theDict);

		public IntPtr GetValue(IntPtr key)
		{
			return CFDictionary.CFDictionaryGetValue(base.Handle, key);
		}

		public IntPtr this[IntPtr key]
		{
			get
			{
				return this.GetValue(key);
			}
		}

		private static readonly IntPtr KeyCallbacks;

		private static readonly IntPtr ValueCallbacks;
	}
}
