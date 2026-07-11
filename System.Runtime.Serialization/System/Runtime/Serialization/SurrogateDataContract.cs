using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization
{
	internal sealed class SurrogateDataContract : DataContract
	{
		[SecuritySafeCritical]
		internal SurrogateDataContract(Type type, ISerializationSurrogate serializationSurrogate)
			: base(new SurrogateDataContract.SurrogateDataContractCriticalHelper(type, serializationSurrogate))
		{
			this.helper = base.Helper as SurrogateDataContract.SurrogateDataContractCriticalHelper;
		}

		internal ISerializationSurrogate SerializationSurrogate
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.SerializationSurrogate;
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			SerializationInfo serializationInfo = new SerializationInfo(base.UnderlyingType, XmlObjectSerializer.FormatterConverter, !context.UnsafeTypeForwardingEnabled);
			this.SerializationSurrogateGetObjectData(obj, serializationInfo, context.GetStreamingContext());
			context.WriteSerializationInfo(xmlWriter, base.UnderlyingType, serializationInfo);
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private object SerializationSurrogateSetObjectData(object obj, SerializationInfo serInfo, StreamingContext context)
		{
			return this.SerializationSurrogate.SetObjectData(obj, serInfo, context, null);
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static object GetRealObject(IObjectReference obj, StreamingContext context)
		{
			return obj.GetRealObject(context);
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private object GetUninitializedObject(Type objType)
		{
			return FormatterServices.GetUninitializedObject(objType);
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SerializationSurrogateGetObjectData(object obj, SerializationInfo serInfo, StreamingContext context)
		{
			this.SerializationSurrogate.GetObjectData(obj, serInfo, context);
		}

		public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
		{
			xmlReader.Read();
			Type underlyingType = base.UnderlyingType;
			object obj = (underlyingType.IsArray ? Array.CreateInstance(underlyingType.GetElementType(), 0) : this.GetUninitializedObject(underlyingType));
			context.AddNewObject(obj);
			string objectId = context.GetObjectId();
			SerializationInfo serializationInfo = context.ReadSerializationInfo(xmlReader, underlyingType);
			object obj2 = this.SerializationSurrogateSetObjectData(obj, serializationInfo, context.GetStreamingContext());
			if (obj2 == null)
			{
				obj2 = obj;
			}
			if (obj2 is IDeserializationCallback)
			{
				((IDeserializationCallback)obj2).OnDeserialization(null);
			}
			if (obj2 is IObjectReference)
			{
				obj2 = SurrogateDataContract.GetRealObject((IObjectReference)obj2, context.GetStreamingContext());
			}
			context.ReplaceDeserializedObject(objectId, obj, obj2);
			xmlReader.ReadEndElement();
			return obj2;
		}

		[SecurityCritical]
		private SurrogateDataContract.SurrogateDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class SurrogateDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal SurrogateDataContractCriticalHelper(Type type, ISerializationSurrogate serializationSurrogate)
				: base(type)
			{
				this.serializationSurrogate = serializationSurrogate;
				string text;
				string text2;
				DataContract.GetDefaultStableName(DataContract.GetClrTypeFullName(type), out text, out text2);
				base.SetDataContractName(DataContract.CreateQualifiedName(text, text2));
			}

			internal ISerializationSurrogate SerializationSurrogate
			{
				get
				{
					return this.serializationSurrogate;
				}
			}

			private ISerializationSurrogate serializationSurrogate;
		}
	}
}
