using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YamlDotNet.Samples.Helpers
{
	public class ExampleRunner : MonoBehaviour
	{
		public static string[] GetAllTestNames()
		{
			List<string> list = new List<string>();
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.Namespace == "YamlDotNet.Samples" && type.IsClass)
				{
					bool flag = false;
					foreach (MethodInfo methodInfo in type.GetMethods())
					{
						if (methodInfo.Name == "Main" && (SampleAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(SampleAttribute)) != null)
						{
							list.Add(type.Name);
							break;
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			return list.ToArray();
		}

		public static string[] GetAllTestTitles()
		{
			List<string> list = new List<string>();
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.Namespace == "YamlDotNet.Samples" && type.IsClass)
				{
					bool flag = false;
					foreach (MethodInfo methodInfo in type.GetMethods())
					{
						if (methodInfo.Name == "Main")
						{
							SampleAttribute sampleAttribute = (SampleAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(SampleAttribute));
							if (sampleAttribute != null)
							{
								list.Add(sampleAttribute.Title);
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			return list.ToArray();
		}

		private void Start()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.Namespace == "YamlDotNet.Samples" && type.IsClass && Array.IndexOf<string>(this.disabledTests, type.Name) == -1)
				{
					bool flag = false;
					foreach (MethodInfo methodInfo in type.GetMethods())
					{
						if (methodInfo.Name == "Main")
						{
							SampleAttribute sampleAttribute = (SampleAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(SampleAttribute));
							if (sampleAttribute != null)
							{
								this.helper.WriteLine("{0} - {1}", new object[] { sampleAttribute.Title, sampleAttribute.Description });
								object obj = type.GetConstructor(new Type[] { typeof(ExampleRunner.StringTestOutputHelper) }).Invoke(new object[] { this.helper });
								methodInfo.Invoke(obj, new object[0]);
								global::Debug.Log(this.helper.ToString());
								this.helper.Clear();
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
		}

		private ExampleRunner.StringTestOutputHelper helper = new ExampleRunner.StringTestOutputHelper();

		public string[] disabledTests = new string[0];

		private class StringTestOutputHelper : ITestOutputHelper
		{
			public void WriteLine()
			{
				this.output.AppendLine();
			}

			public void WriteLine(string value)
			{
				this.output.AppendLine(value);
			}

			public void WriteLine(string format, params object[] args)
			{
				this.output.AppendFormat(format, args);
				this.output.AppendLine();
			}

			public override string ToString()
			{
				return this.output.ToString();
			}

			public void Clear()
			{
				this.output = new StringBuilder();
			}

			private StringBuilder output = new StringBuilder();
		}
	}
}
