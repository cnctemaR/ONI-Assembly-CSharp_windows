using System;

namespace Mono.Cecil.Cil
{
	internal sealed class ImportTarget
	{
		public string Namespace
		{
			get
			{
				return this.@namespace;
			}
			set
			{
				this.@namespace = value;
			}
		}

		public TypeReference Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public AssemblyNameReference AssemblyReference
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		public string Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = value;
			}
		}

		public ImportTargetKind Kind
		{
			get
			{
				return this.kind;
			}
			set
			{
				this.kind = value;
			}
		}

		public ImportTarget(ImportTargetKind kind)
		{
			this.kind = kind;
		}

		internal ImportTargetKind kind;

		internal string @namespace;

		internal TypeReference type;

		internal AssemblyNameReference reference;

		internal string alias;
	}
}
