using System;

public struct ModError
{
	public ModError.ErrorType errorType;

	public ModInfo modInfo;

	public enum ErrorType
	{
		LoadError
	}
}
