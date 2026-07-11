using System;

namespace System.Security.AccessControl
{
	public abstract class QualifiedAce : KnownAce
	{
		internal QualifiedAce(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AceQualifier aceQualifier, bool isCallback, byte[] opaque)
			: base(inheritanceFlags, propagationFlags)
		{
			this.ace_qualifier = aceQualifier;
			this.is_callback = isCallback;
			this.SetOpaque(opaque);
		}

		public AceQualifier AceQualifier
		{
			get
			{
				return this.ace_qualifier;
			}
		}

		public bool IsCallback
		{
			get
			{
				return this.is_callback;
			}
		}

		public int OpaqueLength
		{
			get
			{
				return this.opaque.Length;
			}
		}

		public byte[] GetOpaque()
		{
			return (byte[])this.opaque.Clone();
		}

		public void SetOpaque(byte[] opaque)
		{
			if (opaque == null)
			{
				throw new ArgumentNullException("opaque");
			}
			this.opaque = (byte[])opaque.Clone();
		}

		private AceQualifier ace_qualifier;

		private bool is_callback;

		private byte[] opaque;
	}
}
