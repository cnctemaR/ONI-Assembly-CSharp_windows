using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	internal class HybridObjectCache
	{
		internal HybridObjectCache()
		{
		}

		internal void Add(string id, object obj)
		{
			if (this.objectDictionary == null)
			{
				this.objectDictionary = new Dictionary<string, object>();
			}
			object obj2;
			if (this.objectDictionary.TryGetValue(id, out obj2))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid XML encountered. The same Id value '{0}' is defined more than once. Multiple objects cannot be deserialized using the same Id.", new object[] { id })));
			}
			this.objectDictionary.Add(id, obj);
		}

		internal void Remove(string id)
		{
			if (this.objectDictionary != null)
			{
				this.objectDictionary.Remove(id);
			}
		}

		internal object GetObject(string id)
		{
			if (this.referencedObjectDictionary == null)
			{
				this.referencedObjectDictionary = new Dictionary<string, object>();
				this.referencedObjectDictionary.Add(id, null);
			}
			else if (!this.referencedObjectDictionary.ContainsKey(id))
			{
				this.referencedObjectDictionary.Add(id, null);
			}
			if (this.objectDictionary != null)
			{
				object obj;
				this.objectDictionary.TryGetValue(id, out obj);
				return obj;
			}
			return null;
		}

		internal bool IsObjectReferenced(string id)
		{
			return this.referencedObjectDictionary != null && this.referencedObjectDictionary.ContainsKey(id);
		}

		private Dictionary<string, object> objectDictionary;

		private Dictionary<string, object> referencedObjectDictionary;
	}
}
