using System;
using System.Runtime.CompilerServices;

namespace System.Windows.Input
{
	[TypeForwardedFrom("PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
	public interface ICommand
	{
		bool CanExecute(object parameter);

		void Execute(object parameter);

		event EventHandler CanExecuteChanged;
	}
}
