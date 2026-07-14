using System;
using System.IO;
using Klei;
using VYaml.Serialization;

public static class KYaml
{
	public static bool LoadFile<T>(string path, out T result, KYaml.ErrorHandler errorHandler = null)
	{
		bool flag;
		try
		{
			FileHandle fileHandle = FileSystem.FindFileHandle(path);
			if (fileHandle.source == null)
			{
				throw new FileNotFoundException("KYaml tried loading a file that doesn't exist: " + path);
			}
			result = YamlSerializer.Deserialize<T>(fileHandle.source.ReadBytes(fileHandle.full_path), KYaml.Options);
			flag = true;
		}
		catch (Exception ex)
		{
			if (errorHandler != null)
			{
				errorHandler(path, ex);
			}
			result = default(T);
			flag = false;
		}
		return flag;
	}

	public static bool LoadFile<T>(FileHandle file, out T result, KYaml.ErrorHandler errorHandler = null)
	{
		bool flag;
		try
		{
			byte[] array = ((file.source != null) ? file.source.ReadBytes(file.full_path) : File.ReadAllBytes(file.full_path));
			result = YamlSerializer.Deserialize<T>(array, KYaml.Options);
			flag = true;
		}
		catch (Exception ex)
		{
			if (errorHandler != null)
			{
				errorHandler(file.full_path, ex);
			}
			result = default(T);
			flag = false;
		}
		return flag;
	}

	private static readonly YamlSerializerOptions Options = new YamlSerializerOptions
	{
		Resolver = CompositeResolver.Create(new IYamlFormatter[]
		{
			new Vector2fFormatter(),
			new TagFormatter(),
			new SimHashesFormatter(),
			new ElementStateFormatter()
		}, new IYamlFormatterResolver[]
		{
			KleiResolver.Instance,
			StandardResolver.Instance
		})
	};

	public struct Error
	{
		public FileHandle file;

		public string message;

		public Exception inner_exception;

		public string text;

		public KYaml.Error.Severity severity;

		public enum Severity
		{
			Fatal,
			Recoverable
		}
	}

	public delegate void ErrorHandler(string path, Exception exception);
}
