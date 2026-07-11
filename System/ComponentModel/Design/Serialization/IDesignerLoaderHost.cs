using System;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public interface IDesignerLoaderHost : IServiceProvider, IDesignerHost, IServiceContainer
	{
		void EndLoad(string baseClassName, bool successful, ICollection errorCollection);

		void Reload();
	}
}
