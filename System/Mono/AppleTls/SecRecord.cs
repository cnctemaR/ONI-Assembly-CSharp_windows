using System;
using Mono.Net;

namespace Mono.AppleTls
{
	internal class SecRecord : IDisposable
	{
		static SecRecord()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/Security.framework/Security", 0);
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			try
			{
				SecRecord.SecClassKey = CFObject.GetIntPtr(intPtr, "kSecClass");
			}
			finally
			{
				CFObject.dlclose(intPtr);
			}
		}

		internal CFMutableDictionary QueryDict
		{
			get
			{
				return this._queryDict;
			}
		}

		internal void SetValue(IntPtr key, IntPtr value)
		{
			this._queryDict.SetValue(key, value);
		}

		public SecRecord(SecKind secKind)
		{
			IntPtr intPtr = SecClass.FromSecKind(secKind);
			this._queryDict = CFMutableDictionary.Create();
			this._queryDict.SetValue(SecRecord.SecClassKey, intPtr);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._queryDict != null && disposing)
			{
				this._queryDict.Dispose();
				this._queryDict = null;
			}
		}

		~SecRecord()
		{
			this.Dispose(false);
		}

		internal static readonly IntPtr SecClassKey;

		private CFMutableDictionary _queryDict;
	}
}
