using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryCore
	{
		public UnixBinaryCore(object owner, IDictionary properties, string[] allowedProperties)
		{
			this._properties = properties;
			foreach (object obj in properties)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text = (string)dictionaryEntry.Key;
				if (Array.IndexOf<string>(allowedProperties, text) == -1)
				{
					throw new RemotingException(owner.GetType().Name + " does not recognize '" + text + "' configuration property");
				}
				if (!(text == "includeVersions"))
				{
					if (text == "strictBinding")
					{
						this._strictBinding = Convert.ToBoolean(dictionaryEntry.Value);
					}
				}
				else
				{
					this._includeVersions = Convert.ToBoolean(dictionaryEntry.Value);
				}
			}
			this.Init();
		}

		public UnixBinaryCore()
		{
			this._properties = new Hashtable();
			this.Init();
		}

		public void Init()
		{
			RemotingSurrogateSelector remotingSurrogateSelector = new RemotingSurrogateSelector();
			StreamingContext streamingContext = new StreamingContext(StreamingContextStates.Remoting, null);
			this._serializationFormatter = new BinaryFormatter(remotingSurrogateSelector, streamingContext);
			this._deserializationFormatter = new BinaryFormatter(null, streamingContext);
			if (!this._includeVersions)
			{
				this._serializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				this._deserializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
			}
			if (!this._strictBinding)
			{
				this._serializationFormatter.Binder = SimpleBinder.Instance;
				this._deserializationFormatter.Binder = SimpleBinder.Instance;
			}
		}

		public BinaryFormatter Serializer
		{
			get
			{
				return this._serializationFormatter;
			}
		}

		public BinaryFormatter Deserializer
		{
			get
			{
				return this._deserializationFormatter;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return this._properties;
			}
		}

		private BinaryFormatter _serializationFormatter;

		private BinaryFormatter _deserializationFormatter;

		private bool _includeVersions = true;

		private bool _strictBinding;

		private IDictionary _properties;

		public static UnixBinaryCore DefaultInstance = new UnixBinaryCore();
	}
}
