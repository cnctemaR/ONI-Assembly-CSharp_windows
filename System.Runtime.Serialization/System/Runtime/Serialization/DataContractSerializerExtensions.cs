using System;
using System.ComponentModel;
using Unity;

namespace System.Runtime.Serialization
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DataContractSerializerExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ISerializationSurrogateProvider GetSerializationSurrogateProvider(this DataContractSerializer serializer)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetSerializationSurrogateProvider(this DataContractSerializer serializer, ISerializationSurrogateProvider provider)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
