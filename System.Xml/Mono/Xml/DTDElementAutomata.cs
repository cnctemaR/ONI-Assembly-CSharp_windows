using System;

namespace Mono.Xml
{
	internal class DTDElementAutomata : DTDAutomata
	{
		public DTDElementAutomata(DTDObjectModel root, string name)
			: base(root)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public override DTDAutomata TryStartElement(string name)
		{
			if (name == this.Name)
			{
				return base.Root.Empty;
			}
			return base.Root.Invalid;
		}

		private string name;
	}
}
