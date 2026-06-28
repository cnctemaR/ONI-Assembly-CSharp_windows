using System;

namespace System.Net
{
	public enum FtpStatusCode
	{
		Undefined,
		RestartMarker = 110,
		ServiceTemporarilyNotAvailable = 120,
		DataAlreadyOpen = 125,
		OpeningData = 150,
		CommandOK = 200,
		CommandExtraneous = 202,
		DirectoryStatus = 212,
		FileStatus,
		SystemType = 215,
		SendUserCommand = 220,
		ClosingControl,
		ClosingData = 226,
		EnteringPassive,
		LoggedInProceed = 230,
		ServerWantsSecureSession = 234,
		FileActionOK = 250,
		PathnameCreated = 257,
		SendPasswordCommand = 331,
		NeedLoginAccount,
		FileCommandPending = 350,
		ServiceNotAvailable = 421,
		CantOpenData = 425,
		ConnectionClosed,
		ActionNotTakenFileUnavailableOrBusy = 450,
		ActionAbortedLocalProcessingError,
		ActionNotTakenInsufficientSpace,
		CommandSyntaxError = 500,
		ArgumentSyntaxError,
		CommandNotImplemented,
		BadCommandSequence,
		NotLoggedIn = 530,
		AccountNeeded = 532,
		ActionNotTakenFileUnavailable = 550,
		ActionAbortedUnknownPageType,
		FileActionAborted,
		ActionNotTakenFilenameNotAllowed
	}
}
