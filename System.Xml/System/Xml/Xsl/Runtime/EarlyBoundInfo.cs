using System;
using System.Reflection;

namespace System.Xml.Xsl.Runtime
{
	internal sealed class EarlyBoundInfo
	{
		public EarlyBoundInfo(string namespaceUri, Type ebType)
		{
			this.namespaceUri = namespaceUri;
			this.constrInfo = ebType.GetConstructor(Type.EmptyTypes);
		}

		public string NamespaceUri
		{
			get
			{
				return this.namespaceUri;
			}
		}

		public Type EarlyBoundType
		{
			get
			{
				return this.constrInfo.DeclaringType;
			}
		}

		public object CreateObject()
		{
			return this.constrInfo.Invoke(new object[0]);
		}

		public override bool Equals(object obj)
		{
			EarlyBoundInfo earlyBoundInfo = obj as EarlyBoundInfo;
			return earlyBoundInfo != null && this.namespaceUri == earlyBoundInfo.namespaceUri && this.constrInfo == earlyBoundInfo.constrInfo;
		}

		public override int GetHashCode()
		{
			return this.namespaceUri.GetHashCode();
		}

		private string namespaceUri;

		private ConstructorInfo constrInfo;
	}
}
