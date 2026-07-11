using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public abstract class ECDiffieHellmanPublicKey : IDisposable
	{
		protected ECDiffieHellmanPublicKey()
		{
			this.m_keyBlob = new byte[0];
		}

		protected ECDiffieHellmanPublicKey(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentNullException("keyBlob");
			}
			this.m_keyBlob = keyBlob.Clone() as byte[];
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual byte[] ToByteArray()
		{
			return this.m_keyBlob.Clone() as byte[];
		}

		public virtual string ToXmlString()
		{
			throw new NotImplementedException(global::SR.GetString("Method not supported. Derived class must override."));
		}

		public virtual ECParameters ExportParameters()
		{
			throw new NotSupportedException(global::SR.GetString("Method not supported. Derived class must override."));
		}

		public virtual ECParameters ExportExplicitParameters()
		{
			throw new NotSupportedException(global::SR.GetString("Method not supported. Derived class must override."));
		}

		private byte[] m_keyBlob;
	}
}
