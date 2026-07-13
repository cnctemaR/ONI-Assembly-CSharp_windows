using System;
using System.Threading;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class SecurityDeclaration
	{
		public SecurityAction Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		public bool HasSecurityAttributes
		{
			get
			{
				this.Resolve();
				return !this.security_attributes.IsNullOrEmpty<SecurityAttribute>();
			}
		}

		public Collection<SecurityAttribute> SecurityAttributes
		{
			get
			{
				this.Resolve();
				if (this.security_attributes == null)
				{
					Interlocked.CompareExchange<Collection<SecurityAttribute>>(ref this.security_attributes, new Collection<SecurityAttribute>(), null);
				}
				return this.security_attributes;
			}
		}

		internal bool HasImage
		{
			get
			{
				return this.module != null && this.module.HasImage;
			}
		}

		internal SecurityDeclaration(SecurityAction action, uint signature, ModuleDefinition module)
		{
			this.action = action;
			this.signature = signature;
			this.module = module;
		}

		public SecurityDeclaration(SecurityAction action)
		{
			this.action = action;
			this.resolved = true;
		}

		public SecurityDeclaration(SecurityAction action, byte[] blob)
		{
			this.action = action;
			this.resolved = false;
			this.blob = blob;
		}

		public byte[] GetBlob()
		{
			if (this.blob != null)
			{
				return this.blob;
			}
			if (!this.HasImage || this.signature == 0U)
			{
				throw new NotSupportedException();
			}
			return this.module.Read<SecurityDeclaration, byte[]>(ref this.blob, this, (SecurityDeclaration declaration, MetadataReader reader) => reader.ReadSecurityDeclarationBlob(declaration.signature));
		}

		private void Resolve()
		{
			if (this.resolved || !this.HasImage)
			{
				return;
			}
			object syncRoot = this.module.SyncRoot;
			lock (syncRoot)
			{
				if (!this.resolved)
				{
					this.module.Read<SecurityDeclaration>(this, delegate(SecurityDeclaration declaration, MetadataReader reader)
					{
						reader.ReadSecurityDeclarationSignature(declaration);
					});
					this.resolved = true;
				}
			}
		}

		internal readonly uint signature;

		private byte[] blob;

		private readonly ModuleDefinition module;

		internal bool resolved;

		private SecurityAction action;

		internal Collection<SecurityAttribute> security_attributes;
	}
}
