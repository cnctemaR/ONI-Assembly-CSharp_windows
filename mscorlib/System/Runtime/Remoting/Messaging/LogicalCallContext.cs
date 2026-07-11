using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	[Serializable]
	public sealed class LogicalCallContext : ICloneable, ISerializable
	{
		internal LogicalCallContext()
		{
		}

		internal LogicalCallContext(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "__RemotingData")
				{
					this._remotingData = (CallContextRemotingData)serializationEntry.Value;
				}
				else
				{
					this.SetData(serializationEntry.Name, serializationEntry.Value);
				}
			}
		}

		public bool HasInfo
		{
			get
			{
				return this._data != null && this._data.Count > 0;
			}
		}

		public void FreeNamedDataSlot(string name)
		{
			if (this._data != null)
			{
				this._data.Remove(name);
			}
		}

		public object GetData(string name)
		{
			if (this._data != null)
			{
				return this._data[name];
			}
			return null;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("__RemotingData", this._remotingData);
			if (this._data != null)
			{
				foreach (object obj in this._data)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					info.AddValue((string)dictionaryEntry.Key, dictionaryEntry.Value);
				}
			}
		}

		public void SetData(string name, object data)
		{
			if (this._data == null)
			{
				this._data = new Hashtable();
			}
			this._data[name] = data;
		}

		public object Clone()
		{
			LogicalCallContext logicalCallContext = new LogicalCallContext();
			logicalCallContext._remotingData = (CallContextRemotingData)this._remotingData.Clone();
			if (this._data != null)
			{
				logicalCallContext._data = new Hashtable();
				foreach (object obj in this._data)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					logicalCallContext._data[dictionaryEntry.Key] = dictionaryEntry.Value;
				}
			}
			return logicalCallContext;
		}

		internal Hashtable Datastore
		{
			get
			{
				return this._data;
			}
		}

		private Hashtable _data;

		private CallContextRemotingData _remotingData = new CallContextRemotingData();
	}
}
