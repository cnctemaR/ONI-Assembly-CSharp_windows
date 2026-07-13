using System;

namespace Mono.Cecil
{
	internal class ModuleReference : IMetadataScope, IMetadataTokenProvider
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public virtual MetadataScopeType MetadataScopeType
		{
			get
			{
				return MetadataScopeType.ModuleReference;
			}
		}

		public MetadataToken MetadataToken
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		internal ModuleReference()
		{
			this.token = new MetadataToken(TokenType.ModuleRef);
		}

		public ModuleReference(string name)
			: this()
		{
			this.name = name;
		}

		public override string ToString()
		{
			return this.name;
		}

		private string name;

		internal MetadataToken token;
	}
}
