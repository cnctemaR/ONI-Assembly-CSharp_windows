using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Klei
{
	public static class YamlIO
	{
		public static void Save<T>(T some_object, string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				SerializerBuilder serializerBuilder = new SerializerBuilder();
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tuple in tagMappings)
					{
						serializerBuilder = serializerBuilder.WithTagMapping(tuple.first, tuple.second);
					}
				}
				Serializer serializer = serializerBuilder.Build();
				serializer.Serialize(streamWriter, some_object);
			}
		}

		public static void SaveOrWarnUser<T>(T some_object, string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			FileUtil.DoIODialog(delegate
			{
				YamlIO.Save<T>(some_object, filename, tagMappings);
			}, filename, 0);
		}

		public static T LoadFile<T>(FileHandle filehandle, YamlIO.ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			return YamlIO.Parse<T>(FileSystem.ConvertToText(filehandle.source.ReadBytes(filehandle.full_path)), filehandle, handle_error, tagMappings);
		}

		public static T LoadFile<T>(string filename, YamlIO.ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			FileHandle fileHandle = FileSystem.FindFileHandle(filename);
			return YamlIO.LoadFile<T>(fileHandle, handle_error, tagMappings);
		}

		public static void LogError(YamlIO.Error error, bool force_log_as_warning)
		{
			YamlIO.ErrorLogger errorLogger;
			if (force_log_as_warning || error.severity == YamlIO.Error.Severity.Recoverable)
			{
				errorLogger = new YamlIO.ErrorLogger(Debug.LogWarningFormat);
			}
			else
			{
				errorLogger = new YamlIO.ErrorLogger(Debug.LogErrorFormat);
			}
			YamlIO.ErrorLogger errorLogger2 = errorLogger;
			if (error.inner_exception == null)
			{
				errorLogger2("{0} parse error in {1}\n{2}", new object[]
				{
					error.severity,
					error.file.full_path,
					error.message
				});
			}
			else
			{
				errorLogger2("{0} parse error in {1}\n{2}\n{3}", new object[]
				{
					error.severity,
					error.file.full_path,
					error.message,
					error.inner_exception.Message
				});
			}
		}

		public static T Parse<T>(string readText, FileHandle debugFileHandle, YamlIO.ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			try
			{
				if (handle_error == null)
				{
					handle_error = new YamlIO.ErrorHandler(YamlIO.LogError);
				}
				readText = readText.Replace("\t", "    ");
				Action<string> action = delegate(string error)
				{
					handle_error(new YamlIO.Error
					{
						file = debugFileHandle,
						text = readText,
						message = error,
						severity = YamlIO.Error.Severity.Recoverable
					}, false);
				};
				DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
				deserializerBuilder.IgnoreUnmatchedProperties(action);
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tuple in tagMappings)
					{
						deserializerBuilder = deserializerBuilder.WithTagMapping(tuple.first, tuple.second);
					}
				}
				Deserializer deserializer = deserializerBuilder.Build();
				StringReader stringReader = new StringReader(readText);
				return deserializer.Deserialize<T>(stringReader);
			}
			catch (Exception ex)
			{
				handle_error(new YamlIO.Error
				{
					file = debugFileHandle,
					text = readText,
					message = ex.Message,
					inner_exception = ex.InnerException,
					severity = YamlIO.Error.Severity.Fatal
				}, false);
			}
			return default(T);
		}

		private const bool verbose_errors = false;

		public struct Error
		{
			public FileHandle file;

			public string message;

			public Exception inner_exception;

			public string text;

			public YamlIO.Error.Severity severity;

			public enum Severity
			{
				Fatal,
				Recoverable
			}
		}

		public delegate void ErrorHandler(YamlIO.Error error, bool force_log_as_warning);

		private delegate void ErrorLogger(string format, params object[] args);
	}
}
