using System;

namespace Mono.Cecil
{
	public abstract class MemberReference : IMetadataTokenProvider
	{
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.IsWindowsRuntimeProjection && value != this.name)
				{
					throw new InvalidOperationException();
				}
				this.name = value;
			}
		}

		public abstract string FullName { get; }

		public virtual TypeReference DeclaringType
		{
			get
			{
				return this.declaring_type;
			}
			set
			{
				this.declaring_type = value;
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

		public bool IsWindowsRuntimeProjection
		{
			get
			{
				return this.projection != null;
			}
		}

		internal bool HasImage
		{
			get
			{
				ModuleDefinition module = this.Module;
				return module != null && module.HasImage;
			}
		}

		public virtual ModuleDefinition Module
		{
			get
			{
				if (this.declaring_type == null)
				{
					return null;
				}
				return this.declaring_type.Module;
			}
		}

		public virtual bool IsDefinition
		{
			get
			{
				return false;
			}
		}

		public virtual bool ContainsGenericParameter
		{
			get
			{
				return this.declaring_type != null && this.declaring_type.ContainsGenericParameter;
			}
		}

		internal MemberReference()
		{
		}

		internal MemberReference(string name)
		{
			this.name = name ?? string.Empty;
		}

		internal string MemberFullName()
		{
			if (this.declaring_type == null)
			{
				return this.name;
			}
			return this.declaring_type.FullName + "::" + this.name;
		}

		public IMemberDefinition Resolve()
		{
			return this.ResolveDefinition();
		}

		protected abstract IMemberDefinition ResolveDefinition();

		public override string ToString()
		{
			return this.FullName;
		}

		private string name;

		private TypeReference declaring_type;

		internal MetadataToken token;

		internal object projection;
	}
}
