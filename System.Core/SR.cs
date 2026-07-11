using System;
using System.Globalization;

internal static class SR
{
	internal static string GetString(string name, params object[] args)
	{
		return global::SR.GetString(CultureInfo.InvariantCulture, name, args);
	}

	internal static string GetString(CultureInfo culture, string name, params object[] args)
	{
		return string.Format(culture, name, args);
	}

	internal static string GetString(string name)
	{
		return name;
	}

	internal static string GetString(CultureInfo culture, string name)
	{
		return name;
	}

	internal static string Format(string resourceFormat, params object[] args)
	{
		if (args != null)
		{
			return string.Format(CultureInfo.InvariantCulture, resourceFormat, args);
		}
		return resourceFormat;
	}

	internal static string Format(string resourceFormat, object p1)
	{
		return string.Format(CultureInfo.InvariantCulture, resourceFormat, p1);
	}

	internal static string Format(string resourceFormat, object p1, object p2)
	{
		return string.Format(CultureInfo.InvariantCulture, resourceFormat, p1, p2);
	}

	internal static string Format(string resourceFormat, object p1, object p2, object p3)
	{
		return string.Format(CultureInfo.InvariantCulture, resourceFormat, p1, p2, p3);
	}

	public const string ArgumentOutOfRange_NeedNonNegNum = "Non negative number is required.";

	public const string Argument_WrongAsyncResult = "IAsyncResult object did not come from the corresponding async method on this type.";

	public const string Argument_InvalidOffLen = "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.";

	public const string Argument_NeedNonemptyPipeName = "pipeName cannot be an empty string.";

	public const string Argument_EmptyServerName = "serverName cannot be an empty string.  Use \".\" for current machine.";

	public const string Argument_NonContainerInvalidAnyFlag = "This flag may not be set on a pipe.";

	public const string Argument_InvalidHandle = "Invalid handle.";

	public const string ArgumentNull_Buffer = "Buffer cannot be null.";

	public const string ArgumentNull_ServerName = "serverName cannot be null. Use \".\" for current machine.";

	public const string ArgumentOutOfRange_AdditionalAccessLimited = "additionalAccessRights is limited to the PipeAccessRights.ChangePermissions, PipeAccessRights.TakeOwnership, and PipeAccessRights.AccessSystemSecurity flags when creating NamedPipeServerStreams.";

	public const string ArgumentOutOfRange_AnonymousReserved = "The pipeName \"anonymous\" is reserved.";

	public const string ArgumentOutOfRange_TransmissionModeByteOrMsg = "For named pipes, transmission mode can be TransmissionMode.Byte or PipeTransmissionMode.Message. For anonymous pipes, transmission mode can be TransmissionMode.Byte.";

	public const string ArgumentOutOfRange_DirectionModeInOrOut = "PipeDirection.In or PipeDirection.Out required.";

	public const string ArgumentOutOfRange_DirectionModeInOutOrInOut = "For named pipes, the pipe direction can be PipeDirection.In, PipeDirection.Out or PipeDirection.InOut. For anonymous pipes, the pipe direction can be PipeDirection.In or PipeDirection.Out.";

	public const string ArgumentOutOfRange_ImpersonationInvalid = "TokenImpersonationLevel.None, TokenImpersonationLevel.Anonymous, TokenImpersonationLevel.Identification, TokenImpersonationLevel.Impersonation or TokenImpersonationLevel.Delegation required.";

	public const string ArgumentOutOfRange_ImpersonationOptionsInvalid = "impersonationOptions contains an invalid flag.";

	public const string ArgumentOutOfRange_OptionsInvalid = "options contains an invalid flag.";

	public const string ArgumentOutOfRange_HandleInheritabilityNoneOrInheritable = "HandleInheritability.None or HandleInheritability.Inheritable required.";

	public const string ArgumentOutOfRange_InvalidPipeAccessRights = "Invalid PipeAccessRights flag.";

	public const string ArgumentOutOfRange_InvalidTimeout = "Timeout must be non-negative or equal to -1 (Timeout.Infinite)";

	public const string ArgumentOutOfRange_MaxNumServerInstances = "maxNumberOfServerInstances must either be a value between 1 and 254, or NamedPipeServerStream.MaxAllowedServerInstances (to obtain the maximum number allowed by system resources).";

	public const string ArgumentOutOfRange_NeedValidPipeAccessRights = "Need valid PipeAccessRights value.";

	public const string IndexOutOfRange_IORaceCondition = "Probable I/O race condition detected while copying memory. The I/O package is not thread safe by default unless stated otherwise. In multithreaded applications, access streams in a thread-safe way, such as a thread-safe wrapper returned by TextReader's or TextWriter's Synchronized methods. This also applies to classes like StreamWriter and StreamReader.";

	public const string InvalidOperation_EndReadCalledMultiple = "EndRead can only be called once for each asynchronous operation.";

	public const string InvalidOperation_EndWriteCalledMultiple = "EndWrite can only be called once for each asynchronous operation.";

	public const string InvalidOperation_EndWaitForConnectionCalledMultiple = "EndWaitForConnection can only be called once for each asynchronous operation.";

	public const string InvalidOperation_PipeNotYetConnected = "Pipe hasn't been connected yet.";

	public const string InvalidOperation_PipeDisconnected = "Pipe is in a disconnected state.";

	public const string InvalidOperation_PipeHandleNotSet = "Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?";

	public const string InvalidOperation_PipeNotAsync = "Pipe is not opened in asynchronous mode.";

	public const string InvalidOperation_PipeReadModeNotMessage = "ReadMode is not of PipeTransmissionMode.Message.";

	public const string InvalidOperation_PipeMessageTypeNotSupported = "This pipe does not support message type transmission.";

	public const string InvalidOperation_PipeAlreadyConnected = "Already in a connected state.";

	public const string InvalidOperation_PipeAlreadyDisconnected = "Already in a disconnected state.";

	public const string InvalidOperation_PipeClosed = "Pipe is closed.";

	public const string IO_FileTooLongOrHandleNotSync = "IO operation will not work. Most likely the file will become too long or the handle was not opened to support synchronous IO operations.";

	public const string IO_EOF_ReadBeyondEOF = "Unable to read beyond the end of the stream.";

	public const string IO_FileNotFound = "Unable to find the specified file.";

	public const string IO_FileNotFound_FileName = "Could not find file '{0}'.";

	public const string IO_IO_AlreadyExists_Name = "Cannot create \"{0}\" because a file or directory with the same name already exists.";

	public const string IO_IO_BindHandleFailed = "BindHandle for ThreadPool failed on this handle.";

	public const string IO_IO_FileExists_Name = "The file '{0}' already exists.";

	public const string IO_IO_NoPermissionToDirectoryName = "<Path discovery permission to the specified directory was denied.>";

	public const string IO_IO_SharingViolation_File = "The process cannot access the file '{0}' because it is being used by another process.";

	public const string IO_IO_SharingViolation_NoFileName = "The process cannot access the file because it is being used by another process.";

	public const string IO_IO_PipeBroken = "Pipe is broken.";

	public const string IO_IO_InvalidPipeHandle = "Invalid pipe handle.";

	public const string IO_OperationAborted = "IO operation was aborted unexpectedly.";

	public const string IO_DriveNotFound_Drive = "Could not find the drive '{0}'. The drive might not be ready or might not be mapped.";

	public const string IO_PathNotFound_Path = "Could not find a part of the path '{0}'.";

	public const string IO_PathNotFound_NoPathName = "Could not find a part of the path.";

	public const string IO_PathTooLong = "The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.";

	public const string NotSupported_MemStreamNotExpandable = "Memory stream is not expandable.";

	public const string NotSupported_UnreadableStream = "Stream does not support reading.";

	public const string NotSupported_UnseekableStream = "Stream does not support seeking.";

	public const string NotSupported_UnwritableStream = "Stream does not support writing.";

	public const string NotSupported_AnonymousPipeUnidirectional = "Anonymous pipes can only be in one direction.";

	public const string NotSupported_AnonymousPipeMessagesNotSupported = "Anonymous pipes do not support PipeTransmissionMode.Message ReadMode.";

	public const string ObjectDisposed_FileClosed = "Cannot access a closed file.";

	public const string ObjectDisposed_PipeClosed = "Cannot access a closed pipe.";

	public const string ObjectDisposed_ReaderClosed = "Cannot read from a closed TextReader.";

	public const string ObjectDisposed_StreamClosed = "Cannot access a closed Stream.";

	public const string ObjectDisposed_WriterClosed = "Cannot write to a closed TextWriter.";

	public const string PlatformNotSupported_NamedPipeServers = "Named Pipe Servers are not supported on Windows 95/98/ME.";

	public const string UnauthorizedAccess_IODenied_Path = "Access to the path '{0}' is denied.";

	public const string UnauthorizedAccess_IODenied_NoPathName = "Access to the path is denied.";

	public const string TraceAsTraceSource = "Trace";

	public const string ArgumentOutOfRange_NeedValidLogRetention = "Need valid log retention option.";

	public const string ArgumentOutOfRange_NeedMaxFileSizeGEBufferSize = "Maximum file size value should be greater than or equal to bufferSize.";

	public const string ArgumentOutOfRange_NeedValidMaxNumFiles = "Maximum number of files value should be greater than or equal to '{0}' for this retention";

	public const string ArgumentOutOfRange_NeedValidId = "The ID parameter must be in the range {0} through {1}.";

	public const string ArgumentOutOfRange_MaxArgExceeded = "The total number of parameters must not exceed {0}.";

	public const string ArgumentOutOfRange_MaxStringsExceeded = "The number of String parameters must not exceed {0}.";

	public const string NotSupported_DownLevelVista = "This functionality is only supported in Windows Vista and above.";

	public const string Argument_NeedNonemptyDelimiter = "Delimiter cannot be an empty string.";

	public const string NotSupported_SetTextWriter = "Setting TextWriter is unsupported on this listener.";

	public const string Perflib_PlatformNotSupported = "Classes in System.Diagnostics.PerformanceData is only supported in Windows Vista and above.";

	public const string Perflib_Argument_CounterSetAlreadyRegister = "CounterSet '{0}' already registered.";

	public const string Perflib_Argument_InvalidCounterType = "CounterType '{0}' is not a valid CounterType.";

	public const string Perflib_Argument_InvalidCounterSetInstanceType = "CounterSetInstanceType '{0}' is not a valid CounterSetInstanceType.";

	public const string Perflib_Argument_InstanceAlreadyExists = "Instance '{0}' already exists in CounterSet '{1}'.";

	public const string Perflib_Argument_CounterAlreadyExists = "CounterId '{0}' already added to CounterSet '{1}'.";

	public const string Perflib_Argument_CounterNameAlreadyExists = "CounterName '{0}' already added to CounterSet '{1}'.";

	public const string Perflib_Argument_ProviderNotFound = "CounterSet provider '{0}' not found.";

	public const string Perflib_Argument_InvalidInstance = "Single instance type CounterSet '{0}' can only have 1 CounterSetInstance.";

	public const string Perflib_Argument_EmptyInstanceName = "Non-empty instanceName required.";

	public const string Perflib_Argument_EmptyCounterName = "Non-empty counterName required.";

	public const string Perflib_InsufficientMemory_InstanceCounterBlock = "Cannot allocate raw counter data for CounterSet '{0}' Instance '{1}'.";

	public const string Perflib_InsufficientMemory_CounterSetTemplate = "Cannot allocate memory for CounterSet '{0}' template with size '{1}'.";

	public const string Perflib_InvalidOperation_CounterRefValue = "Cannot locate raw counter data location for CounterSet '{0}', Counter '{1}, in Instance '{2}'.";

	public const string Perflib_InvalidOperation_CounterSetNotInstalled = "CounterSet '{0}' not installed yet.";

	public const string Perflib_InvalidOperation_InstanceNotFound = "Cannot find Instance '{0}' in CounterSet '{1}'.";

	public const string Perflib_InvalidOperation_AddCounterAfterInstance = "Cannot AddCounter to CounterSet '{0}' after CreateCounterSetInstance.";

	public const string Perflib_InvalidOperation_NoActiveProvider = "CounterSet provider '{0}' not active.";

	public const string Perflib_InvalidOperation_CounterSetContainsNoCounter = "CounterSet '{0}' does not include any counters.";

	public const string Arg_ArrayPlusOffTooSmall = "Destination array is not long enough to copy all the items in the collection. Check array index and length.";

	public const string Arg_HSCapacityOverflow = "HashSet capacity is too big.";

	public const string InvalidOperation_EnumFailedVersion = "Collection was modified; enumeration operation may not execute.";

	public const string InvalidOperation_EnumOpCantHappen = "Enumeration has either not started or has already finished.";

	public const string Serialization_MissingKeys = "The Keys for this dictionary are missing.";

	public const string LockRecursionException_RecursiveReadNotAllowed = "Recursive read lock acquisitions not allowed in this mode.";

	public const string LockRecursionException_RecursiveWriteNotAllowed = "Recursive write lock acquisitions not allowed in this mode.";

	public const string LockRecursionException_RecursiveUpgradeNotAllowed = "Recursive upgradeable lock acquisitions not allowed in this mode.";

	public const string LockRecursionException_ReadAfterWriteNotAllowed = "A read lock may not be acquired with the write lock held in this mode.";

	public const string LockRecursionException_WriteAfterReadNotAllowed = "Write lock may not be acquired with read lock held. This pattern is prone to deadlocks. Please ensure that read locks are released before taking a write lock. If an upgrade is necessary, use an upgrade lock in place of the read lock.";

	public const string LockRecursionException_UpgradeAfterReadNotAllowed = "Upgradeable lock may not be acquired with read lock held.";

	public const string LockRecursionException_UpgradeAfterWriteNotAllowed = "Upgradeable lock may not be acquired with write lock held in this mode. Acquiring Upgradeable lock gives the ability to read along with an option to upgrade to a writer.";

	public const string SynchronizationLockException_MisMatchedRead = "The read lock is being released without being held.";

	public const string SynchronizationLockException_MisMatchedWrite = "The write lock is being released without being held.";

	public const string SynchronizationLockException_MisMatchedUpgrade = "The upgradeable lock is being released without being held.";

	public const string SynchronizationLockException_IncorrectDispose = "The lock is being disposed while still being used. It either is being held by a thread and/or has active waiters waiting to acquire the lock.";

	public const string Cryptography_ArgECDHKeySizeMismatch = "The keys from both parties must be the same size to generate a secret agreement.";

	public const string Cryptography_ArgECDHRequiresECDHKey = "Keys used with the ECDiffieHellmanCng algorithm must have an algorithm group of ECDiffieHellman.";

	public const string Cryptography_ArgECDsaRequiresECDsaKey = "Keys used with the ECDsaCng algorithm must have an algorithm group of ECDsa.";

	public const string Cryptography_ArgExpectedECDiffieHellmanCngPublicKey = "DeriveKeyMaterial requires an ECDiffieHellmanCngPublicKey.";

	public const string Cryptography_ArgMustBeCngAlgorithm = "Object must be of type CngAlgorithm.";

	public const string Cryptography_ArgMustBeCngAlgorithmGroup = "Object must be of type CngAlgorithmGroup.";

	public const string Cryptography_ArgMustBeCngKeyBlobFormat = "Object must be of type CngKeyBlobFormat.";

	public const string Cryptography_ArgMustBeCngProvider = "Object must be of type CngProvider.";

	public const string Cryptography_DecryptWithNoKey = "Decrypting a value requires that a key be set on the algorithm object.";

	public const string Cryptography_ECXmlSerializationFormatRequired = "XML serialization of an elliptic curve key requires using an overload which specifies the XML format to be used.";

	public const string Cryptography_InvalidAlgorithmGroup = "The algorithm group '{0}' is invalid.";

	public const string Cryptography_InvalidAlgorithmName = "The algorithm name '{0}' is invalid.";

	public const string Cryptography_InvalidCipherMode = "The specified cipher mode is not valid for this algorithm.";

	public const string Cryptography_InvalidIVSize = "The specified initialization vector (IV) does not match the block size for this algorithm.";

	public const string Cryptography_InvalidKeyBlobFormat = "The key blob format '{0}' is invalid.";

	public const string Cryptography_InvalidKeySize = "The specified key is not a valid size for this algorithm.";

	public const string Cryptography_InvalidPadding = "Padding is invalid and cannot be removed.";

	public const string Cryptography_InvalidProviderName = "The provider name '{0}' is invalid.";

	public const string Cryptography_MissingDomainParameters = "Could not read the domain parameters from the XML string.";

	public const string Cryptography_MissingPublicKey = "Could not read the public key from the XML string.";

	public const string Cryptography_MissingIV = "The cipher mode specified requires that an initialization vector (IV) be used.";

	public const string Cryptography_MustTransformWholeBlock = "TransformBlock may only process bytes in block sized increments.";

	public const string Cryptography_NonCompliantFIPSAlgorithm = "This implementation is not part of the Windows Platform FIPS validated cryptographic algorithms.";

	public const string Cryptography_OpenInvalidHandle = "Cannot open an invalid handle.";

	public const string Cryptography_OpenEphemeralKeyHandleWithoutEphemeralFlag = "The CNG key handle being opened was detected to be ephemeral, but the EphemeralKey open option was not specified.";

	public const string Cryptography_PartialBlock = "The input data is not a complete block.";

	public const string Cryptography_PlatformNotSupported = "The specified cryptographic algorithm is not supported on this platform.";

	public const string Cryptography_TlsRequiresLabelAndSeed = "The TLS key derivation function requires both the label and seed properties to be set.";

	public const string Cryptography_TransformBeyondEndOfBuffer = "Attempt to transform beyond end of buffer.";

	public const string Cryptography_UnknownEllipticCurve = "Unknown elliptic curve.";

	public const string Cryptography_UnknownEllipticCurveAlgorithm = "Unknown elliptic curve algorithm.";

	public const string Cryptography_UnknownPaddingMode = "Unknown padding mode used.";

	public const string Cryptography_UnexpectedXmlNamespace = "The XML namespace '{0}' was unexpected, expected '{1}'.";

	public const string ArgumentException_RangeMinRangeMaxRangeType = "Cannot accept MinRange {0} because it is not the same type as MaxRange {1}. Verify that the MaxRange and MinRange values are of the same type and try again.";

	public const string ArgumentException_RangeNotIComparable = "Cannot accept MaxRange and MinRange because they are not IComparable.";

	public const string ArgumentException_RangeMaxRangeSmallerThanMinRange = "Cannot accept MaxRange because it is less than MinRange. Specify a MaxRange value that is greater than or equal to the MinRange value and try again.";

	public const string ArgumentException_CountMaxLengthSmallerThanMinLength = "MaxLength should be greater than MinLength.";

	public const string ArgumentException_LengthMaxLengthSmallerThanMinLength = "Cannot accept MaxLength value. Specify MaxLength value greater than the value of MinLength and try again.";

	public const string ArgumentException_UnregisteredParameterName = "Parameter {0} has not been added to this parser.";

	public const string ArgumentException_InvalidParameterName = "{0} is an invalid parameter name.";

	public const string ArgumentException_DuplicateName = "The name {0} is already in use.";

	public const string ArgumentException_DuplicatePosition = "The position {0} is already in use.";

	public const string ArgumentException_NoParametersFound = "The object has no parameters associated with it.";

	public const string ArgumentException_HelpMessageBaseNameNullOrEmpty = "Help message base name may not be null or empty.";

	public const string ArgumentException_HelpMessageResourceIdNullOrEmpty = "Help message resource id may not be null or empty.";

	public const string ArgumentException_HelpMessageNullOrEmpty = "Help message may not be null or empty.";

	public const string ArgumentException_RegexPatternNullOrEmpty = "The regular expression pattern may not be null or empty.";

	public const string ArgumentException_RequiredPositionalAfterOptionalPositional = "Optional positional parameter {0} cannot precede required positional parameter {1}.";

	public const string ArgumentException_DuplicateParameterAttribute = "Duplicate parameter attributes with the same parameter set on parameter {0}.";

	public const string ArgumentException_MissingBaseNameOrResourceId = "On parameter {0}, either both HelpMessageBaseName and HelpMessageResourceId must be set or neither can be set.";

	public const string ArgumentException_DuplicateRemainingArgumets = "Can not set {0} as the remaining arguments parameter for parameter set {1} because that parameter set already has a parameter set as the remaining arguments parameter.";

	public const string ArgumentException_TypeMismatchForRemainingArguments = "Parameter {0} must be an array of strings if it can have its value from the remaining arguments.";

	public const string ArgumentException_ValidationParameterTypeMismatch = "Validator {0} may not be applied to a parameter of type {1}.";

	public const string ArgumentException_ParserBuiltWithValueType = "The parameter toBind may not be an instance of a value type.";

	public const string InvalidOperationException_GetParameterTypeMismatch = "Parameter {0} may not retrieved with type {1} since it is of type {2}.";

	public const string InvalidOperationException_GetParameterValueBeforeParse = "Parse must be called before retrieving parameter values.";

	public const string InvalidOperationException_SetRemainingArgumentsParameterAfterParse = "AllowRemainingArguments may not be set after Parse has been called.";

	public const string InvalidOperationException_AddParameterAfterParse = "Parameters may not be added after Parse has been called.";

	public const string InvalidOperationException_BindAfterBind = "Parse may only be called once.";

	public const string InvalidOperationException_GetRemainingArgumentsNotAllowed = "GetRemainingArguments may not be called unless AllowRemainingArguments is set to true.";

	public const string InvalidOperationException_ParameterSetBeforeParse = "The SpecifiedParameterSet property may only be accessed after Parse has been called successfully.";

	public const string CommandLineParser_Aliases = "Aliases";

	public const string CommandLineParser_ErrorMessagePrefix = "Error";

	public const string CommandLineParser_HelpMessagePrefix = "Usage";

	public const string ParameterBindingException_AmbiguousParameterName = "Prefix {0} resolves to multiple parameters: {1}.  Use a more specific prefix for this parameter.";

	public const string ParameterBindingException_ParameterValueAlreadySpecified = "Parameter {0} already given value of {1}.";

	public const string ParameterBindingException_UnknownParameteName = "Unknown parameter {0}.";

	public const string ParameterBindingException_RequiredParameterMissingCommandLineValue = "Parameter {0} must be followed by a value.";

	public const string ParameterBindingException_UnboundCommandLineArguments = "Unbound parameters left on command line: {0}.";

	public const string ParameterBindingException_UnboundMandatoryParameter = "Values for required parameters missing: {0}.";

	public const string ParameterBindingException_ResponseFileException = "Could not open response file {0}: {1}";

	public const string ParameterBindingException_ValididationError = "Could not validate parameter {0}: {1}";

	public const string ParameterBindingException_TransformationError = "Could not convert {0} to type {1}.";

	public const string ParameterBindingException_AmbiguousParameterSet = "Named parameters specify an ambiguous parameter set.  Specify more parameters by name.";

	public const string ParameterBindingException_UnknownParameterSet = "No valid parameter set for named parameters.  Make sure all named parameters belong to the same parameter set.";

	public const string ParameterBindingException_NestedResponseFiles = "A response file may not contain references to other response files.";

	public const string ValidateMetadataException_RangeGreaterThanMaxRangeFailure = "The value {0} was greater than the maximum value {1}. Specify a value less than or equal to the maximum value and try again.";

	public const string ValidateMetadataException_RangeSmallerThanMinRangeFailure = "The value {0} was smaller than the minimum value {1}. Specify a value greater than or equal to the minimum value and try again.";

	public const string ValidateMetadataException_PatternFailure = "The value {0} does not match the pattern {1}.";

	public const string ValidateMetadataException_CountMinLengthFailure = "The number of values should be greater than or equal to {0} instead of {1}.";

	public const string ValidateMetadataException_CountMaxLengthFailure = "The number of values should be less than or equal to {0} instead of {1}.";

	public const string ValidateMetadataException_LengthMinLengthFailure = "The length should be greater than or equal to {0} instead of {1}.";

	public const string ValidateMetadataException_LengthMaxLengthFailure = "The length should be less than or equal to {0} instead of {1}.";

	public const string Argument_MapNameEmptyString = "Map name cannot be an empty string.";

	public const string Argument_EmptyFile = "A positive capacity must be specified for a Memory Mapped File backed by an empty file.";

	public const string Argument_NewMMFWriteAccessNotAllowed = "MemoryMappedFileAccess.Write is not permitted when creating new memory mapped files. Use MemoryMappedFileAccess.ReadWrite instead.";

	public const string Argument_ReadAccessWithLargeCapacity = "When specifying MemoryMappedFileAccess.Read access, the capacity must not be larger than the file size.";

	public const string Argument_NewMMFAppendModeNotAllowed = "FileMode.Append is not permitted when creating new memory mapped files. Instead, use MemoryMappedFileView to ensure write-only access within a specified region.";

	public const string ArgumentNull_MapName = "Map name cannot be null.";

	public const string ArgumentNull_FileStream = "fileStream cannot be null.";

	public const string ArgumentOutOfRange_CapacityLargerThanLogicalAddressSpaceNotAllowed = "The capacity cannot be greater than the size of the system's logical address space.";

	public const string ArgumentOutOfRange_NeedPositiveNumber = "A positive number is required.";

	public const string ArgumentOutOfRange_PositiveOrDefaultCapacityRequired = "The capacity must be greater than or equal to 0. 0 represents the the size of the file being mapped.";

	public const string ArgumentOutOfRange_PositiveOrDefaultSizeRequired = "The size must be greater than or equal to 0. If 0 is specified, the view extends from the specified offset to the end of the file mapping.";

	public const string ArgumentOutOfRange_PositionLessThanCapacityRequired = "The position may not be greater or equal to the capacity of the accessor.";

	public const string ArgumentOutOfRange_CapacityGEFileSizeRequired = "The capacity may not be smaller than the file size.";

	public const string IO_NotEnoughMemory = "Not enough memory to map view.";

	public const string InvalidOperation_CalledTwice = "Cannot call this operation twice.";

	public const string InvalidOperation_CantCreateFileMapping = "Cannot create file mapping.";

	public const string InvalidOperation_ViewIsNull = "The underlying MemoryMappedView object is null.";

	public const string NotSupported_DelayAllocateFileBackedNotAllowed = "The MemoryMappedFileOptions.DelayAllocatePages option is not supported with memory mapped files mapping files on disk.";

	public const string NotSupported_MMViewStreamsFixedLength = "MemoryMappedViewStreams are fixed length.";

	public const string ObjectDisposed_ViewAccessorClosed = "Cannot access a closed accessor.";

	public const string ObjectDisposed_StreamIsClosed = "Cannot access a closed Stream.";

	public const string NotSupported_Method = "Method not supported.";

	public const string NotSupported_SubclassOverride = "Method not supported. Derived class must override.";

	public const string Cryptography_ArgDSARequiresDSAKey = "Keys used with the DSACng algorithm must have an algorithm group of DSA.";

	public const string Cryptography_ArgRSAaRequiresRSAKey = "Keys used with the RSACng algorithm must have an algorithm group of RSA.";

	public const string Cryptography_CngKeyWrongAlgorithm = "This key is for algorithm '{0}'. Expected '{1}'.";

	public const string Cryptography_DSA_HashTooShort = "The supplied hash cannot be shorter in length than the DSA key's Q value.";

	public const string Cryptography_HashAlgorithmNameNullOrEmpty = "The hash algorithm name cannot be null or empty.";

	public const string Cryptography_InvalidDsaParameters_MissingFields = "The specified DSA parameters are not valid; P, Q, G and Y are all required.";

	public const string Cryptography_InvalidDsaParameters_MismatchedPGY = "The specified DSA parameters are not valid; P, G and Y must be the same length (the key size).";

	public const string Cryptography_InvalidDsaParameters_MismatchedQX = "The specified DSA parameters are not valid; Q and X (if present) must be the same length.";

	public const string Cryptography_InvalidDsaParameters_MismatchedPJ = "The specified DSA parameters are not valid; J (if present) must be shorter than P.";

	public const string Cryptography_InvalidDsaParameters_SeedRestriction_ShortKey = "The specified DSA parameters are not valid; Seed, if present, must be 20 bytes long for keys shorter than 1024 bits.";

	public const string Cryptography_InvalidDsaParameters_QRestriction_ShortKey = "The specified DSA parameters are not valid; Q must be 20 bytes long for keys shorter than 1024 bits.";

	public const string Cryptography_InvalidDsaParameters_QRestriction_LargeKey = "The specified DSA parameters are not valid; Q's length must be one of 20, 32 or 64 bytes.";

	public const string Cryptography_InvalidRsaParameters = "The specified RSA parameters are not valid; both Exponent and Modulus are required fields.";

	public const string Cryptography_InvalidSignatureAlgorithm = "The hash algorithm is not supported for signatures. Only MD5, SHA1, SHA256,SHA384, and SHA512 are supported at this time.";

	public const string Cryptography_KeyBlobParsingError = "Key Blob not in expected format.";

	public const string Cryptography_NotSupportedKeyAlgorithm = "Key Algorithm is not supported.";

	public const string Cryptography_NotValidPublicOrPrivateKey = "Key is not a valid public or private key.";

	public const string Cryptography_NotValidPrivateKey = "Key is not a valid private key.";

	public const string Cryptography_UnexpectedTransformTruncation = "CNG provider unexpectedly terminated encryption or decryption prematurely.";

	public const string Cryptography_UnsupportedPaddingMode = "The specified PaddingMode is not supported.";

	public const string Cryptography_WeakKey = "Specified key is a known weak key for this algorithm and cannot be used.";

	public const string Cryptography_CurveNotSupported = "The specified curve '{0}' or its parameters are not valid for this platform.";

	public const string Cryptography_InvalidCurve = "The specified curve '{0}' is not valid for this platform.";

	public const string Cryptography_InvalidCurveOid = "The specified Oid is not valid. The Oid.FriendlyName or Oid.Value property must be set.";

	public const string Cryptography_InvalidCurveKeyParameters = "The specified key parameters are not valid. Q.X and Q.Y are required fields. Q.X, Q.Y must be the same length. If D is specified it must be the same length as Q.X and Q.Y for named curves or the same length as Order for explicit curves.";

	public const string Cryptography_InvalidECCharacteristic2Curve = "The specified Characteristic2 curve parameters are not valid. Polynomial, A, B, G.X, G.Y, and Order are required. A, B, G.X, G.Y must be the same length, and the same length as Q.X, Q.Y and D if those are specified. Seed, Cofactor and Hash are optional. Other parameters are not allowed.";

	public const string Cryptography_InvalidECPrimeCurve = "The specified prime curve parameters are not valid. Prime, A, B, G.X, G.Y and Order are required and must be the same length, and the same length as Q.X, Q.Y and D if those are specified. Seed, Cofactor and Hash are optional. Other parameters are not allowed.";

	public const string Cryptography_InvalidECNamedCurve = "The specified named curve parameters are not valid. Only the Oid parameter must be set.";

	public const string Cryptography_UnknownHashAlgorithm = "'{0}' is not a known hash algorithm.";

	public const string ReducibleMustOverrideReduce = "reducible nodes must override Expression.Reduce()";

	public const string MustReduceToDifferent = "node cannot reduce to itself or null";

	public const string ReducedNotCompatible = "cannot assign from the reduced node type to the original node type";

	public const string SetterHasNoParams = "Setter must have parameters.";

	public const string PropertyCannotHaveRefType = "Property cannot have a managed pointer type.";

	public const string IndexesOfSetGetMustMatch = "Indexing parameters of getter and setter must match.";

	public const string AccessorsCannotHaveVarArgs = "Accessor method should not have VarArgs.";

	public const string AccessorsCannotHaveByRefArgs = "Accessor indexes cannot be passed ByRef.";

	public const string BoundsCannotBeLessThanOne = "Bounds count cannot be less than 1";

	public const string TypeMustNotBeByRef = "Type must not be ByRef";

	public const string TypeMustNotBePointer = "Type must not be a pointer type";

	public const string TypeDoesNotHaveConstructorForTheSignature = "Type doesn't have constructor with a given signature";

	public const string SetterMustBeVoid = "Setter should have void type.";

	public const string PropertyTypeMustMatchGetter = "Property type must match the value type of getter";

	public const string PropertyTypeMustMatchSetter = "Property type must match the value type of setter";

	public const string BothAccessorsMustBeStatic = "Both accessors must be static.";

	public const string OnlyStaticFieldsHaveNullInstance = "Static field requires null instance, non-static field requires non-null instance.";

	public const string OnlyStaticPropertiesHaveNullInstance = "Static property requires null instance, non-static property requires non-null instance.";

	public const string OnlyStaticMethodsHaveNullInstance = "Static method requires null instance, non-static method requires non-null instance.";

	public const string PropertyTypeCannotBeVoid = "Property cannot have a void type.";

	public const string InvalidUnboxType = "Can only unbox from an object or interface type to a value type.";

	public const string ExpressionMustBeWriteable = "Expression must be writeable";

	public const string ArgumentMustNotHaveValueType = "Argument must not have a value type.";

	public const string MustBeReducible = "must be reducible node";

	public const string AllTestValuesMustHaveSameType = "All test values must have the same type.";

	public const string AllCaseBodiesMustHaveSameType = "All case bodies and the default body must have the same type.";

	public const string DefaultBodyMustBeSupplied = "Default body must be supplied if case bodies are not System.Void.";

	public const string LabelMustBeVoidOrHaveExpression = "Label type must be System.Void if an expression is not supplied";

	public const string LabelTypeMustBeVoid = "Type must be System.Void for this label argument";

	public const string QuotedExpressionMustBeLambda = "Quoted expression must be a lambda";

	public const string VariableMustNotBeByRef = "Variable '{0}' uses unsupported type '{1}'. Reference types are not supported for variables.";

	public const string DuplicateVariable = "Found duplicate parameter '{0}'. Each ParameterExpression in the list must be a unique object.";

	public const string StartEndMustBeOrdered = "Start and End must be well ordered";

	public const string FaultCannotHaveCatchOrFinally = "fault cannot be used with catch or finally clauses";

	public const string TryMustHaveCatchFinallyOrFault = "try must have at least one catch, finally, or fault clause";

	public const string BodyOfCatchMustHaveSameTypeAsBodyOfTry = "Body of catch must have the same type as body of try.";

	public const string ExtensionNodeMustOverrideProperty = "Extension node must override the property {0}.";

	public const string UserDefinedOperatorMustBeStatic = "User-defined operator method '{0}' must be static.";

	public const string UserDefinedOperatorMustNotBeVoid = "User-defined operator method '{0}' must not be void.";

	public const string CoercionOperatorNotDefined = "No coercion operator is defined between types '{0}' and '{1}'.";

	public const string UnaryOperatorNotDefined = "The unary operator {0} is not defined for the type '{1}'.";

	public const string BinaryOperatorNotDefined = "The binary operator {0} is not defined for the types '{1}' and '{2}'.";

	public const string ReferenceEqualityNotDefined = "Reference equality is not defined for the types '{0}' and '{1}'.";

	public const string OperandTypesDoNotMatchParameters = "The operands for operator '{0}' do not match the parameters of method '{1}'.";

	public const string OverloadOperatorTypeDoesNotMatchConversionType = "The return type of overload method for operator '{0}' does not match the parameter type of conversion method '{1}'.";

	public const string ConversionIsNotSupportedForArithmeticTypes = "Conversion is not supported for arithmetic types without operator overloading.";

	public const string ArgumentMustBeArray = "Argument must be array";

	public const string ArgumentMustBeBoolean = "Argument must be boolean";

	public const string EqualityMustReturnBoolean = "The user-defined equality method '{0}' must return a boolean value.";

	public const string ArgumentMustBeFieldInfoOrPropertyInfo = "Argument must be either a FieldInfo or PropertyInfo";

	public const string ArgumentMustBeFieldInfoOrPropertyInfoOrMethod = "Argument must be either a FieldInfo, PropertyInfo or MethodInfo";

	public const string ArgumentMustBeInstanceMember = "Argument must be an instance member";

	public const string ArgumentMustBeInteger = "Argument must be of an integer type";

	public const string ArgumentMustBeArrayIndexType = "Argument for array index must be of type Int32";

	public const string ArgumentMustBeSingleDimensionalArrayType = "Argument must be single-dimensional, zero-based array type";

	public const string ArgumentTypesMustMatch = "Argument types do not match";

	public const string CannotAutoInitializeValueTypeElementThroughProperty = "Cannot auto initialize elements of value type through property '{0}', use assignment instead";

	public const string CannotAutoInitializeValueTypeMemberThroughProperty = "Cannot auto initialize members of value type through property '{0}', use assignment instead";

	public const string IncorrectTypeForTypeAs = "The type used in TypeAs Expression must be of reference or nullable type, {0} is neither";

	public const string CoalesceUsedOnNonNullType = "Coalesce used with type that cannot be null";

	public const string ExpressionTypeCannotInitializeArrayType = "An expression of type '{0}' cannot be used to initialize an array of type '{1}'";

	public const string ArgumentTypeDoesNotMatchMember = " Argument type '{0}' does not match the corresponding member type '{1}'";

	public const string ArgumentMemberNotDeclOnType = " The member '{0}' is not declared on type '{1}' being created";

	public const string ExpressionTypeDoesNotMatchReturn = "Expression of type '{0}' cannot be used for return type '{1}'";

	public const string ExpressionTypeDoesNotMatchAssignment = "Expression of type '{0}' cannot be used for assignment to type '{1}'";

	public const string ExpressionTypeDoesNotMatchLabel = "Expression of type '{0}' cannot be used for label of type '{1}'";

	public const string ExpressionTypeNotInvocable = "Expression of type '{0}' cannot be invoked";

	public const string FieldNotDefinedForType = "Field '{0}' is not defined for type '{1}'";

	public const string InstanceFieldNotDefinedForType = "Instance field '{0}' is not defined for type '{1}'";

	public const string FieldInfoNotDefinedForType = "Field '{0}.{1}' is not defined for type '{2}'";

	public const string IncorrectNumberOfIndexes = "Incorrect number of indexes";

	public const string IncorrectNumberOfLambdaDeclarationParameters = "Incorrect number of parameters supplied for lambda declaration";

	public const string IncorrectNumberOfMembersForGivenConstructor = " Incorrect number of members for constructor";

	public const string IncorrectNumberOfArgumentsForMembers = "Incorrect number of arguments for the given members ";

	public const string LambdaTypeMustBeDerivedFromSystemDelegate = "Lambda type parameter must be derived from System.MulticastDelegate";

	public const string MemberNotFieldOrProperty = "Member '{0}' not field or property";

	public const string MethodContainsGenericParameters = "Method {0} contains generic parameters";

	public const string MethodIsGeneric = "Method {0} is a generic method definition";

	public const string MethodNotPropertyAccessor = "The method '{0}.{1}' is not a property accessor";

	public const string PropertyDoesNotHaveGetter = "The property '{0}' has no 'get' accessor";

	public const string PropertyDoesNotHaveSetter = "The property '{0}' has no 'set' accessor";

	public const string PropertyDoesNotHaveAccessor = "The property '{0}' has no 'get' or 'set' accessors";

	public const string NotAMemberOfType = "'{0}' is not a member of type '{1}'";

	public const string NotAMemberOfAnyType = "'{0}' is not a member of any type";

	public const string ExpressionNotSupportedForType = "The expression '{0}' is not supported for type '{1}'";

	public const string UnsupportedExpressionType = "The expression type '{0}' is not supported";

	public const string ParameterExpressionNotValidAsDelegate = "ParameterExpression of type '{0}' cannot be used for delegate parameter of type '{1}'";

	public const string PropertyNotDefinedForType = "Property '{0}' is not defined for type '{1}'";

	public const string InstancePropertyNotDefinedForType = "Instance property '{0}' is not defined for type '{1}'";

	public const string InstancePropertyWithoutParameterNotDefinedForType = "Instance property '{0}' that takes no argument is not defined for type '{1}'";

	public const string InstancePropertyWithSpecifiedParametersNotDefinedForType = "Instance property '{0}{1}' is not defined for type '{2}'";

	public const string InstanceAndMethodTypeMismatch = "Method '{0}' declared on type '{1}' cannot be called with instance of type '{2}'";

	public const string TypeContainsGenericParameters = "Type {0} contains generic parameters";

	public const string TypeIsGeneric = "Type {0} is a generic type definition";

	public const string TypeMissingDefaultConstructor = "Type '{0}' does not have a default constructor";

	public const string ElementInitializerMethodNotAdd = "Element initializer method must be named 'Add'";

	public const string ElementInitializerMethodNoRefOutParam = "Parameter '{0}' of element initializer method '{1}' must not be a pass by reference parameter";

	public const string ElementInitializerMethodWithZeroArgs = "Element initializer method must have at least 1 parameter";

	public const string ElementInitializerMethodStatic = "Element initializer method must be an instance method";

	public const string TypeNotIEnumerable = "Type '{0}' is not IEnumerable";

	public const string UnexpectedCoalesceOperator = "Unexpected coalesce operator.";

	public const string InvalidCast = "Cannot cast from type '{0}' to type '{1}";

	public const string UnhandledBinary = "Unhandled binary: {0}";

	public const string UnhandledBinding = "Unhandled binding ";

	public const string UnhandledBindingType = "Unhandled Binding Type: {0}";

	public const string UnhandledConvert = "Unhandled convert: {0}";

	public const string UnhandledUnary = "Unhandled unary: {0}";

	public const string UnknownBindingType = "Unknown binding type";

	public const string UserDefinedOpMustHaveConsistentTypes = "The user-defined operator method '{1}' for operator '{0}' must have identical parameter and return types.";

	public const string UserDefinedOpMustHaveValidReturnType = "The user-defined operator method '{1}' for operator '{0}' must return the same type as its parameter or a derived type.";

	public const string LogicalOperatorMustHaveBooleanOperators = "The user-defined operator method '{1}' for operator '{0}' must have associated boolean True and False operators.";

	public const string MethodWithArgsDoesNotExistOnType = "No method '{0}' on type '{1}' is compatible with the supplied arguments.";

	public const string GenericMethodWithArgsDoesNotExistOnType = "No generic method '{0}' on type '{1}' is compatible with the supplied type arguments and arguments. No type arguments should be provided if the method is non-generic. ";

	public const string MethodWithMoreThanOneMatch = "More than one method '{0}' on type '{1}' is compatible with the supplied arguments.";

	public const string PropertyWithMoreThanOneMatch = "More than one property '{0}' on type '{1}' is compatible with the supplied arguments.";

	public const string IncorrectNumberOfTypeArgsForFunc = "An incorrect number of type arguments were specified for the declaration of a Func type.";

	public const string IncorrectNumberOfTypeArgsForAction = "An incorrect number of type arguments were specified for the declaration of an Action type.";

	public const string ArgumentCannotBeOfTypeVoid = "Argument type cannot be System.Void.";

	public const string OutOfRange = "{0} must be greater than or equal to {1}";

	public const string LabelTargetAlreadyDefined = "Cannot redefine label '{0}' in an inner block.";

	public const string LabelTargetUndefined = "Cannot jump to undefined label '{0}'.";

	public const string ControlCannotLeaveFinally = "Control cannot leave a finally block.";

	public const string ControlCannotLeaveFilterTest = "Control cannot leave a filter test.";

	public const string AmbiguousJump = "Cannot jump to ambiguous label '{0}'.";

	public const string ControlCannotEnterTry = "Control cannot enter a try block.";

	public const string ControlCannotEnterExpression = "Control cannot enter an expression--only statements can be jumped into.";

	public const string NonLocalJumpWithValue = "Cannot jump to non-local label '{0}' with a value. Only jumps to labels defined in outer blocks can pass values.";

	public const string ExtensionNotReduced = "Extension should have been reduced.";

	public const string CannotCompileConstant = "CompileToMethod cannot compile constant '{0}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.";

	public const string CannotCompileDynamic = "Dynamic expressions are not supported by CompileToMethod. Instead, create an expression tree that uses System.Runtime.CompilerServices.CallSite.";

	public const string InvalidLvalue = "Invalid lvalue for assignment: {0}.";

	public const string UnknownLiftType = "unknown lift type: '{0}'.";

	public const string UndefinedVariable = "variable '{0}' of type '{1}' referenced from scope '{2}', but it is not defined";

	public const string CannotCloseOverByRef = "Cannot close over byref parameter '{0}' referenced in lambda '{1}'";

	public const string UnexpectedVarArgsCall = "Unexpected VarArgs call to method '{0}'";

	public const string RethrowRequiresCatch = "Rethrow statement is valid only inside a Catch block.";

	public const string TryNotAllowedInFilter = "Try expression is not allowed inside a filter body.";

	public const string MustRewriteToSameNode = "When called from '{0}', rewriting a node of type '{1}' must return a non-null value of the same type. Alternatively, override '{2}' and change it to not visit children of this type.";

	public const string MustRewriteChildToSameType = "Rewriting child expression from type '{0}' to type '{1}' is not allowed, because it would change the meaning of the operation. If this is intentional, override '{2}' and change it to allow this rewrite.";

	public const string MustRewriteWithoutMethod = "Rewritten expression calls operator method '{0}', but the original node had no operator method. If this is intentional, override '{1}' and change it to allow this rewrite.";

	public const string InvalidNullValue = "The value null is not of type '{0}' and cannot be used in this collection.";

	public const string InvalidObjectType = "The value '{0}' is not of type '{1}' and cannot be used in this collection.";

	public const string TryNotSupportedForMethodsWithRefArgs = "TryExpression is not supported as an argument to method '{0}' because it has an argument with by-ref type. Construct the tree so the TryExpression is not nested inside of this expression.";

	public const string TryNotSupportedForValueTypeInstances = "TryExpression is not supported as a child expression when accessing a member on type '{0}' because it is a value type. Construct the tree so the TryExpression is not nested inside of this expression.";

	public const string EnumerationIsDone = "Enumeration has either not started or has already finished.";

	public const string TestValueTypeDoesNotMatchComparisonMethodParameter = "Test value of type '{0}' cannot be used for the comparison method parameter of type '{1}'";

	public const string SwitchValueTypeDoesNotMatchComparisonMethodParameter = "Switch value of type '{0}' cannot be used for the comparison method parameter of type '{1}'";

	public const string PdbGeneratorNeedsExpressionCompiler = "DebugInfoGenerator created by CreatePdbGenerator can only be used with LambdaExpression.CompileToMethod.";

	public const string InvalidArgumentValue = "Invalid argument value";

	public const string NonEmptyCollectionRequired = "Non-empty collection required";

	public const string CollectionModifiedWhileEnumerating = "Collection was modified; enumeration operation may not execute.";

	public const string ExpressionMustBeReadable = "Expression must be readable";

	public const string ExpressionTypeDoesNotMatchMethodParameter = "Expression of type '{0}' cannot be used for parameter of type '{1}' of method '{2}'";

	public const string ExpressionTypeDoesNotMatchParameter = "Expression of type '{0}' cannot be used for parameter of type '{1}'";

	public const string ExpressionTypeDoesNotMatchConstructorParameter = "Expression of type '{0}' cannot be used for constructor parameter of type '{1}'";

	public const string IncorrectNumberOfMethodCallArguments = "Incorrect number of arguments supplied for call to method '{0}'";

	public const string IncorrectNumberOfLambdaArguments = "Incorrect number of arguments supplied for lambda invocation";

	public const string IncorrectNumberOfConstructorArguments = "Incorrect number of arguments for constructor";

	public const string OperatorNotImplementedForType = "The operator '{0}' is not implemented for type '{1}'";

	public const string NonStaticConstructorRequired = "The constructor should not be static";

	public const string NonAbstractConstructorRequired = "Can't compile a NewExpression with a constructor declared on an abstract class";

	public const string FirstArgumentMustBeCallSite = "First argument of delegate must be CallSite";

	public const string NoOrInvalidRuleProduced = "No or Invalid rule produced";

	public const string TypeMustBeDerivedFromSystemDelegate = "Type must be derived from System.Delegate";

	public const string TypeParameterIsNotDelegate = "Type parameter is {0}. Expected a delegate.";

	public const string ArgumentTypeCannotBeVoid = "Argument type cannot be void";

	public const string ArgCntMustBeGreaterThanNameCnt = "Argument count must be greater than number of named arguments.";

	public const string BinderNotCompatibleWithCallSite = "The result type '{0}' of the binder '{1}' is not compatible with the result type '{2}' expected by the call site.";

	public const string BindingCannotBeNull = "Bind cannot return null.";

	public const string DynamicBinderResultNotAssignable = "The result type '{0}' of the dynamic binding produced by binder '{1}' is not compatible with the result type '{2}' expected by the call site.";

	public const string DynamicBindingNeedsRestrictions = "The result of the dynamic binding produced by the object with type '{0}' for the binder '{1}' needs at least one restriction.";

	public const string DynamicObjectResultNotAssignable = "The result type '{0}' of the dynamic binding produced by the object with type '{1}' for the binder '{2}' is not compatible with the result type '{3}' expected by the call site.";

	public const string InvalidMetaObjectCreated = "An IDynamicMetaObjectProvider {0} created an invalid DynamicMetaObject instance.";

	public const string AmbiguousMatchInExpandoObject = "More than one key matching '{0}' was found in the ExpandoObject.";

	public const string CollectionReadOnly = "Collection is read-only.";

	public const string KeyDoesNotExistInExpando = "The specified key '{0}' does not exist in the ExpandoObject.";

	public const string SameKeyExistsInExpando = "An element with the same key '{0}' already exists in the ExpandoObject.";

	public const string EmptyEnumerable = "Enumeration yielded no results";

	public const string MoreThanOneElement = "Sequence contains more than one element";

	public const string MoreThanOneMatch = "Sequence contains more than one matching element";

	public const string NoElements = "Sequence contains no elements";

	public const string NoMatch = "Sequence contains no matching element";

	public const string ParallelPartitionable_NullReturn = "The return value must not be null.";

	public const string ParallelPartitionable_IncorretElementCount = "The returned array's length must equal the number of partitions requested.";

	public const string ParallelPartitionable_NullElement = "Elements returned must not be null.";

	public const string PLINQ_CommonEnumerator_Current_NotStarted = "Enumeration has not started. MoveNext must be called to initiate enumeration.";

	public const string PLINQ_ExternalCancellationRequested = "The query has been canceled via the token supplied to WithCancellation.";

	public const string PLINQ_DisposeRequested = "The query enumerator has been disposed.";

	public const string ParallelQuery_DuplicateTaskScheduler = "The WithTaskScheduler operator may be used at most once in a query.";

	public const string ParallelQuery_DuplicateDOP = "The WithDegreeOfParallelism operator may be used at most once in a query.";

	public const string ParallelQuery_DuplicateExecutionMode = "The WithExecutionMode operator may be used at most once in a query.";

	public const string PartitionerQueryOperator_NullPartitionList = "Partitioner returned null instead of a list of partitions.";

	public const string PartitionerQueryOperator_WrongNumberOfPartitions = "Partitioner returned a wrong number of partitions.";

	public const string PartitionerQueryOperator_NullPartition = "Partitioner returned a null partition.";

	public const string ParallelQuery_DuplicateWithCancellation = "The WithCancellation operator may by used at most once in a query.";

	public const string ParallelQuery_DuplicateMergeOptions = "The WithMergeOptions operator may be used at most once in a query.";

	public const string PLINQ_EnumerationPreviouslyFailed = "The query enumerator previously threw an exception.";

	public const string ParallelQuery_PartitionerNotOrderable = "AsOrdered may not be used with a partitioner that is not orderable.";

	public const string ParallelQuery_InvalidAsOrderedCall = "AsOrdered may only be called on the result of AsParallel, ParallelEnumerable.Range, or ParallelEnumerable.Repeat.";

	public const string ParallelQuery_InvalidNonGenericAsOrderedCall = "Non-generic AsOrdered may only be called on the result of the non-generic AsParallel.";

	public const string ParallelEnumerable_BinaryOpMustUseAsParallel = "The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>.";

	public const string ParallelEnumerable_WithQueryExecutionMode_InvalidMode = "The executionMode argument contains an invalid value.";

	public const string ParallelEnumerable_WithMergeOptions_InvalidOptions = "The mergeOptions argument contains an invalid value.";

	public const string ArgumentNotIEnumerableGeneric = "{0} is not IEnumerable<>";

	public const string ArgumentNotValid = "Argument {0} is not valid";

	public const string NoMethodOnType = "There is no method '{0}' on type '{1}'";

	public const string NoMethodOnTypeMatchingArguments = "There is no method '{0}' on type '{1}' that matches the specified arguments";

	public const string EnumeratingNullEnumerableExpression = "Cannot enumerate a query created from a null IEnumerable<>";

	public const string MethodBuilderDoesNotHaveTypeBuilder = "MethodBuilder does not have a valid TypeBuilder";
}
