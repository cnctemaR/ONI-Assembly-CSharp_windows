using System;

namespace Microsoft.Cci.Pdb
{
	internal class PdbLines
	{
		internal PdbLines(PdbSource file, uint count)
		{
			this.file = file;
			this.lines = new PdbLine[count];
		}

		internal PdbSource file;

		internal PdbLine[] lines;
	}
}
