#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2009 Adrien H�mery ( iliak@mimicprod.net )
//
//ArcEngine is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//any later version.
//
//ArcEngine is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using Microsoft.VisualBasic;
using ArcEngine.Interface;


// TODO Pouvoir ajouter des references a des dll soit depuis le ResourceManager
//      soit directement en parametre dans le fichier xml du script
//
// http://www.csscript.net/
// 
namespace ArcEngine.Asset
{

	/// <summary>
	/// Script resource
	/// </summary>
	public class Script : IAsset
	{

		/// <summary>
		/// constructor
		/// </summary>
		public Script()
		{


			// Configure parameters
			Params = new CompilerParameters();
			Params.GenerateExecutable = false;
			Params.GenerateInMemory = true;
			Params.WarningLevel = 3;
			Params.CompilerOptions = "/optimize /define:DEBUG;TRACE";
			

			DebugMode = false;


			// Adds the executable itself
			AddReference(Assembly.GetExecutingAssembly().Location);
			AddReference(Assembly.GetCallingAssembly().Location);
			AddReference(Assembly.GetEntryAssembly().Location);
			AddReference("System.dll");
			AddReference("System.Drawing.dll");
			AddReference("System.Xml.dll");
			AddReference("System.Windows.Forms.dll");

	//		AssemblyName[] ass = Assembly.GetCallingAssembly().GetReferencedAssemblies();
	//		ass = Assembly.GetExecutingAssembly().GetReferencedAssemblies();


/*

			// Find all interfaces
			foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
			{
				if (t.IsInterface)
				{

				}
			}
*/

			IsDisposed = false;
		}



		#region Statics

		/// <summary>
		/// Loads and compile a script from a bank's asset
		/// </summary>
		/// <param name="name">Name of the script</param>
		/// <returns>Script handle</returns>
		public Script LoadFromBank(string name)
		{
			Script script = new Script();

			if (!string.IsNullOrEmpty(name))
			{
				script = ResourceManager.CreateAsset<Script>(name);
				script.Compile();
			}

			return script;
		}

		#endregion


		/// <summary>
		/// Initializes the asset
		/// </summary>
		/// <returns>True on success</returns>
		public bool Init()
		{
			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
		}


		/// <summary>
		/// Invoke a method in the script
		/// </summary>
		/// <param name="method">Method's name</param>
		/// <param name="args">Arguments</param>
		/// <returns>true if args or false</returns>
		// http://msdn.microsoft.com/en-us/library/3y322t50%28v=VS.90%29.aspx => Reflection.Emmit()
		public bool Invoke(string method, params object[] args)
		{
			if (!IsCompiled)
			{
				Trace.WriteLine("Invoking method \"" + method + "\" on uncompiled script \"" + Name + "\"");
				return false;
			}



			try
			{
				Type[] type = CompiledAssembly.GetTypes();
				MethodInfo[] mi = type[0].GetMethods();

				object instance = Activator.CreateInstance(type[0]);
				object result = type[0].InvokeMember(method,
					BindingFlags.Default | BindingFlags.InvokeMethod,
					null,
					instance,
					args);
			}
			catch (Exception e)
			{
				Trace.WriteLine("Exception invoking \"" + method + "\" on uncompiled script \"" + Name + "\" : " + e.Message);
			}

			return true;
		}


		/// <summary>
		/// Returns all methods
		/// </summary>
		/// <returns>Returns a list of method's name</returns>
		public List<string> GetMethods()
		{
			List<string> list = new List<string>();

			if (!IsCompiled)
				Compile();


			if (CompiledAssembly == null)
				return list;

			// Loop through types looking for one that implements the given interface
			foreach (Type t in CompiledAssembly.GetTypes())
			{

				foreach (MethodInfo m in t.GetMethods())
				{

					list.Add(m.Name);
				}

			}

			list.Sort();

			return list;
		}



		/// <summary>
		/// Gets a list of specific methods
		/// </summary>
		/// <param name="types">An array of type's parameters</param>
		/// <param name="ret">Retrun value</param>
		/// <returns></returns>
		public List<string> GetMethods(Type[] types, Type ret)
		{
			List<string> list = new List<string>();

			if (!IsCompiled)
				Compile();


			if (CompiledAssembly == null)
				return list;

			// Loop through types looking for one that implements the given interface
			foreach (Type t in CompiledAssembly.GetTypes())
			{

				foreach (MethodInfo m in t.GetMethods())
				{
					// return value
					if (m.ReturnType != ret)
						continue;

					// Parameters
					bool ok = false;
					int id = 0;
					foreach (ParameterInfo pi in m.GetParameters())
					{
						// Index overrun
						if (id >= types.Length)
						{
							ok = false;
							break;
						}

						// Parameters differents
						if (pi.ParameterType != types[id])
						{
							ok = false;
							break;
						}

						ok = true;

						// Next
						id++;
					}

					if (ok && id == types.Length)
						list.Add(m.Name);

					
				}

			}

			return list;
		}


		/// <summary>
		/// Gets all class implementing an interface
		/// </summary>
		/// <param name="type">Type of the interface or null for evey interfaces</param>
		/// <returns>List of classes</returns>
		public List<string> GetImplementedInterfaces(Type type)
		{
			List<string> list = new List<string>();

			if (!IsCompiled)
				Compile();


			if (CompiledAssembly == null)
				return list;

			// Loop through types looking for one that implements the given interface
			foreach (Type t in CompiledAssembly.GetTypes())
			{
				foreach(Type ty in t.GetInterfaces())
					if (ty == type || type == null)
						list.Add(t.Name);
			}

			list.Sort();

			return list;
		}


		/// <summary>
		/// Compiles a given string script
		/// </summary>
		/// <returns>True if compilation ok</returns>
		public bool Compile()
		{
			// Already compiled ?
			if (IsCompiled)
				return true;

			IsCompiled = false;
			IsModified = false;
			HasErrors = false;
			Errors = null;
			CompiledAssembly = null;

			// Generate the compiler provider
			switch (Language)
			{
				case ScriptLanguage.CSharp:
				{
					ScriptProvider = new Microsoft.CSharp.CSharpCodeProvider();
					if (ScriptProvider == null)
					{
						Trace.WriteLine("Failed to initialize a new instance of the CSharpCodeProvider class");

						return false;
					}
				}
				break;

				case ScriptLanguage.VBNet:
				{
					ScriptProvider = new Microsoft.VisualBasic.VBCodeProvider();
					if (ScriptProvider == null)
					{
						Trace.WriteLine("Failed to initialize a new instance of the VBCodeProvider class");
						return false;
					}
				}
				break;

				default:
				{
					Trace.WriteLine("Unknown scripting language !!!");
					return false;
				}

			}




			// Compile
			CompilerResults results = ScriptProvider.CompileAssemblyFromSource(Params, sourceCode);
			if (results.Errors.Count == 0)
			{
				CompiledAssembly = results.CompiledAssembly;
				IsCompiled = true;
				return true;
			}



			Errors = results.Errors;
			HasErrors = true;

			Trace.WriteLine("Compile complete -- " + Errors.Count + " error(s).");

			foreach (CompilerError error in Errors)
				Trace.WriteLine("line " + error.Line + " : " + error.ErrorText);



			return IsCompiled;
		}



		/// <summary>
		/// Adds reference to the assembly
		/// </summary>
		/// <param name="name">Name of the reference</param>
		public void AddReference(string name)
		{
			Params.ReferencedAssemblies.Add(name);
		}


		/// <summary>
		/// Creates an instance of T
		/// </summary>
		/// <typeparam name="T">The type of object to create</typeparam>
		/// <param name="name">Name of the class</param>
		/// <returns>A reference to the newly created object.</returns>
		public T CreateInstance<T>(string name) where T : class
		{
			if (!IsCompiled)
			{
				Trace.WriteLine("Script \"" + Name + "\" is not compiled. Can't create a new instance \"" + name + "\" of type \"" + typeof(T).Name + "\"");
				return default(T);
			}

			// Get type
			Type t = CompiledAssembly.GetType(name);
			if (t == null)
			{
				Trace.WriteLine("Type \"" + name + "\" not found.");
				return default(T);
			}


			// Check interface
			if (!typeof(T).IsAssignableFrom(t))
			{
				Trace.WriteLine("Type \"" + name + "\" found, but not an instance of \"" + typeof(T).Name + "\".");
				return default(T);
			}


			// Create an instance
			return Activator.CreateInstance(t) as T;
		}






		#region IO routines

		///
		///<summary>
		/// Save the collection to a xml node
		/// </summary>
		public bool Save(XmlWriter writer)
		{
			if (writer == null)
				return false;

			writer.WriteStartElement(Tag);
			writer.WriteAttributeString("name", Name);


			writer.WriteStartElement("language");
			writer.WriteAttributeString("name", Language.ToString());
			writer.WriteEndElement();


			writer.WriteStartElement("source");
//			writer.WriteRaw(HttpUtility.HtmlEncode(SourceCode));
			writer.WriteString(SourceCode);
			writer.WriteEndElement();

			//writer.WriteStartElement("debug");
			//writer.WriteAttributeString("value", DebugMode.ToString());
			//writer.WriteEndElement();

			writer.WriteEndElement();

			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public bool Load(XmlNode xml)
		{
			if (xml == null)
				return false;

			Name = xml.Attributes["name"].Value;

			foreach (XmlNode node in xml)
			{
				switch (node.Name.ToLower())
				{
					case "language":
					{
						Language = (ScriptLanguage)Enum.Parse(typeof(ScriptLanguage), node.Attributes["name"].Value, true);
					}
					break;


					case "source":
					{
						//sourceCode = HttpUtility.HtmlDecode(xml.InnerText);
						sourceCode = xml.InnerText;
					}
					break;

					case "reference":
					{
						AddReference(node.Attributes["name"].Value.ToString());
					}
					break;


					//case "debug":
					//{
					//   DebugMode = Boolean.Parse(node.Attributes["value"].Value);
					//}
					//break;

					default:
					{
						Trace.WriteLine("{0} : Unknown node element found ({1}) !", XmlTag, node.Name);
					}
					break;
				}
			}


			return true;
		}

		#endregion


		#region Properties


		/// <summary>
		/// Name of the script
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Is asset disposed
		/// </summary>
		public bool IsDisposed { get; private set; }



		/// <summary>
		/// Tag
		/// </summary>
		public const string Tag = "script";

		/// <summary>
		/// Xml tag of the asset in bank
		/// </summary>
		public string XmlTag
		{
			get
			{
				return Tag;
			}
		}

		/// <summary>
		/// Compiler parameters
		/// </summary>
		CompilerParameters Params;


		/// <summary>
		/// Source code of the script
		/// </summary>
		[Browsable(false)]
		public string SourceCode
		{
			get
			{
				return sourceCode;
			}

			set
			{
				sourceCode = value;
				IsModified = true;
				IsCompiled = false;
			}
		}
		string sourceCode;


		/// <summary>
		/// Enables / disables the debug mode
		/// </summary>
		[CategoryAttribute("Debug")]
		[Description("Enables / disables the debug mode")]
		public bool DebugMode
		{
			get
			{
				return Params.IncludeDebugInformation;
			}
			set
			{
				Params.IncludeDebugInformation = value;
			}
		}


		/// <summary>
		/// Is source code modified ?
		/// </summary>
		[Browsable(false)]
		public bool IsModified
		{
			get;
			private set;
		}



		/// <summary>
		/// Returns the state of the script compilation
		/// </summary>
		[Browsable(false)]
		public bool IsCompiled
		{
			get;
			private set;
		}



		/// <summary>
		/// Gets the compilation log
		/// </summary>
		[Browsable(false)]
		public CompilerErrorCollection Errors
		{
			get;
			private set;
		}


		/// <summary>
		/// Does the script contains error
		/// </summary>
		[Browsable(false)]
		public bool HasErrors
		{
			get;
			private set;
		}



		/// <summary>
		/// Compiled assembly
		/// </summary>
		Assembly CompiledAssembly;


		/// <summary>
		/// Script langage
		/// </summary>
		[CategoryAttribute("Language")]
		[Description("Script's language")]
		public ScriptLanguage Language
		{
			get;
			set;
		}



		/// <summary>
		/// http://www.divil.co.uk/net/articles/plugins/scripting.asp
		/// </summary>
		CodeDomProvider ScriptProvider;


		#endregion

	}

	/// <summary>
	/// Supported languages
	/// </summary>
	public enum ScriptLanguage
	{
		/// <summary>
		/// CSharp language
		/// </summary>
		CSharp,

		/// <summary>
		/// VB.Net language
		/// </summary>
		VBNet
	}




}
