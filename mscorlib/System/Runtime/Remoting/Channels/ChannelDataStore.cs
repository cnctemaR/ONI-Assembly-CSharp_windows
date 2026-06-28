using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	[Serializable]
	public class ChannelDataStore : IChannelDataStore
	{
		public ChannelDataStore(string[] channelURIs)
		{
			this._channelURIs = channelURIs;
		}

		public string[] ChannelUris
		{
			get
			{
				return this._channelURIs;
			}
			set
			{
				this._channelURIs = value;
			}
		}

		public object this[object key]
		{
			get
			{
				if (this._extraData == null)
				{
					return null;
				}
				foreach (DictionaryEntry dictionaryEntry in this._extraData)
				{
					if (dictionaryEntry.Key.Equals(key))
					{
						return dictionaryEntry.Value;
					}
				}
				return null;
			}
			set
			{
				if (this._extraData == null)
				{
					this._extraData = new DictionaryEntry[]
					{
						new DictionaryEntry(key, value)
					};
				}
				else
				{
					DictionaryEntry[] array = new DictionaryEntry[this._extraData.Length + 1];
					this._extraData.CopyTo(array, 0);
					array[this._extraData.Length] = new DictionaryEntry(key, value);
					this._extraData = array;
				}
			}
		}

		private string[] _channelURIs;

		private DictionaryEntry[] _extraData;
	}
}
