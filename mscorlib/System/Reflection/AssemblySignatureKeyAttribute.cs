using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	public sealed class AssemblySignatureKeyAttribute : Attribute
	{
		public AssemblySignatureKeyAttribute(string publicKey, string countersignature)
		{
			this._publicKey = publicKey;
			this._countersignature = countersignature;
		}

		public string PublicKey
		{
			get
			{
				return this._publicKey;
			}
		}

		public string Countersignature
		{
			get
			{
				return this._countersignature;
			}
		}

		private string _publicKey;

		private string _countersignature;
	}
}
