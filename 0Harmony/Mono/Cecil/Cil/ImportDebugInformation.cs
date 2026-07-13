using System;
using System.Threading;
using Mono.Collections.Generic;

namespace Mono.Cecil.Cil
{
	internal sealed class ImportDebugInformation : DebugInformation
	{
		public bool HasTargets
		{
			get
			{
				return !this.targets.IsNullOrEmpty<ImportTarget>();
			}
		}

		public Collection<ImportTarget> Targets
		{
			get
			{
				if (this.targets == null)
				{
					Interlocked.CompareExchange<Collection<ImportTarget>>(ref this.targets, new Collection<ImportTarget>(), null);
				}
				return this.targets;
			}
		}

		public ImportDebugInformation Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public ImportDebugInformation()
		{
			this.token = new MetadataToken(TokenType.ImportScope);
		}

		internal ImportDebugInformation parent;

		internal Collection<ImportTarget> targets;
	}
}
