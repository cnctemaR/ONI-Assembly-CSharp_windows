using System;

namespace Mono.Cecil
{
	internal sealed class LinkedResource : Resource
	{
		public byte[] Hash
		{
			get
			{
				return this.hash;
			}
		}

		public string File
		{
			get
			{
				return this.file;
			}
			set
			{
				this.file = value;
			}
		}

		public override ResourceType ResourceType
		{
			get
			{
				return ResourceType.Linked;
			}
		}

		public LinkedResource(string name, ManifestResourceAttributes flags)
			: base(name, flags)
		{
		}

		public LinkedResource(string name, ManifestResourceAttributes flags, string file)
			: base(name, flags)
		{
			this.file = file;
		}

		internal byte[] hash;

		private string file;
	}
}
