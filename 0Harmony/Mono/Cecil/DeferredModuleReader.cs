using System;
using Mono.Cecil.PE;

namespace Mono.Cecil
{
	internal sealed class DeferredModuleReader : ModuleReader
	{
		public DeferredModuleReader(Image image)
			: base(image, ReadingMode.Deferred)
		{
		}

		protected override void ReadModule()
		{
			this.module.Read<ModuleDefinition>(this.module, delegate(ModuleDefinition _, MetadataReader reader)
			{
				base.ReadModuleManifest(reader);
			});
		}

		public override void ReadSymbols(ModuleDefinition module)
		{
		}
	}
}
