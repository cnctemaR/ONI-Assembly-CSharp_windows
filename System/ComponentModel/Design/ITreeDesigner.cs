using System;
using System.Collections;

namespace System.ComponentModel.Design
{
	public interface ITreeDesigner : IDisposable, IDesigner
	{
		ICollection Children { get; }

		IDesigner Parent { get; }
	}
}
