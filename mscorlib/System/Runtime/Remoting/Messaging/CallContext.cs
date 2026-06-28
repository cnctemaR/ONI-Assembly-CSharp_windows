using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	[Serializable]
	public sealed class CallContext
	{
		private CallContext()
		{
		}

		public static object HostContext
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public static void FreeNamedDataSlot(string name)
		{
			CallContext.Datastore.Remove(name);
		}

		public static object GetData(string name)
		{
			return CallContext.Datastore[name];
		}

		public static void SetData(string name, object data)
		{
			CallContext.Datastore[name] = data;
		}

		[MonoTODO]
		public static object LogicalGetData(string name)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void LogicalSetData(string name, object data)
		{
			throw new NotImplementedException();
		}

		public static Header[] GetHeaders()
		{
			return CallContext.Headers;
		}

		public static void SetHeaders(Header[] headers)
		{
			CallContext.Headers = headers;
		}

		internal static LogicalCallContext CreateLogicalCallContext(bool createEmpty)
		{
			LogicalCallContext logicalCallContext = null;
			if (CallContext.datastore != null)
			{
				foreach (object obj in CallContext.datastore)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (dictionaryEntry.Value is ILogicalThreadAffinative)
					{
						if (logicalCallContext == null)
						{
							logicalCallContext = new LogicalCallContext();
						}
						logicalCallContext.SetData((string)dictionaryEntry.Key, dictionaryEntry.Value);
					}
				}
			}
			if (logicalCallContext == null && createEmpty)
			{
				return new LogicalCallContext();
			}
			return logicalCallContext;
		}

		internal static object SetCurrentCallContext(LogicalCallContext ctx)
		{
			object obj = CallContext.datastore;
			if (ctx != null && ctx.HasInfo)
			{
				CallContext.datastore = (Hashtable)ctx.Datastore.Clone();
			}
			else
			{
				CallContext.datastore = null;
			}
			return obj;
		}

		internal static void UpdateCurrentCallContext(LogicalCallContext ctx)
		{
			Hashtable hashtable = ctx.Datastore;
			foreach (object obj in hashtable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				CallContext.SetData((string)dictionaryEntry.Key, dictionaryEntry.Value);
			}
		}

		internal static void RestoreCallContext(object oldContext)
		{
			CallContext.datastore = (Hashtable)oldContext;
		}

		private static Hashtable Datastore
		{
			get
			{
				Hashtable hashtable = CallContext.datastore;
				if (hashtable == null)
				{
					return CallContext.datastore = new Hashtable();
				}
				return hashtable;
			}
		}

		[ThreadStatic]
		private static Header[] Headers;

		[ThreadStatic]
		private static Hashtable datastore;
	}
}
