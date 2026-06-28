using System;
using System.Collections;
using System.IO;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class ComponentSerializationService
	{
		public abstract SerializationStore CreateStore();

		public abstract ICollection Deserialize(SerializationStore store);

		public abstract ICollection Deserialize(SerializationStore store, IContainer container);

		public abstract SerializationStore LoadStore(Stream stream);

		public abstract void Serialize(SerializationStore store, object value);

		public abstract void SerializeAbsolute(SerializationStore store, object value);

		public abstract void SerializeMember(SerializationStore store, object owningObject, MemberDescriptor member);

		public abstract void SerializeMemberAbsolute(SerializationStore store, object owningObject, MemberDescriptor member);

		public void DeserializeTo(SerializationStore store, IContainer container)
		{
			this.DeserializeTo(store, container, true);
		}

		public void DeserializeTo(SerializationStore store, IContainer container, bool validateRecycledTypes)
		{
			this.DeserializeTo(store, container, validateRecycledTypes, true);
		}

		public abstract void DeserializeTo(SerializationStore store, IContainer container, bool validateRecycledTypes, bool applyDefaults);
	}
}
