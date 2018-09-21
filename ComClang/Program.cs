using ComClang;
using CppSharp.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppSharp
{
	public class Builder
	{

	}

	public class ASTWalker : AstVisitor
	{
		private readonly Builder m_Owner;
		public Class TDivMsgItemType;
		public List<Tuple<Class, List<Variable>>> Items { get; } = new List<Tuple<Class, List<Variable>>>();

		public ASTWalker(Builder owner)
		{
			m_Owner = owner;
		}

		private void AddVariable(Variable variable)
		{
			var item = Items.FirstOrDefault((a) => a.Item1 == variable.Namespace);
			List<Variable> variables;
			if (item == null)
			{
				variables = new List<Variable>();
				item = new Tuple<Class, List<Variable>>((Class)variable.Namespace, variables);
				Items.Add(item);
			}
			else
			{
				variables = item.Item2;
			}
			variables.Add(variable);
		}
	}

	/// <summary>
	/// This sample shows how to use the parser APIs to parse a C/C++ source code
	/// file into a syntax tree representation. It takes as an argument a path to 
	/// a source file and outputs the translation units parsed by the compiler.
	/// </summary>
	static class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.Error.WriteLine("A path to a file for parsing is required.");
				return;
			}

			var file = Path.GetFullPath(args[0]);
			ParseSourceFile(file);
		}

		public static bool ParseSourceFile(string file)
		{
			// Lets setup the options for parsing the file.
			var parserOptions = new ParserOptions
			{
				LanguageVersion = LanguageVersion.CPP11,

				// Verbose here will make sure the parser outputs some extra debugging
				// information regarding include directories, which can be helpful when
				// tracking down parsing issues.
				Verbose = true
			};

			// This will setup the necessary system include paths and arguments for parsing.
			// It will probe into the registry (on Windows) and filesystem to find the paths
			// of the system toolchains and necessary include directories.
			parserOptions.Setup();

			// We create the Clang parser and parse the source code.
			var parser = new ClangParser();
			var parserResult = parser.ParseSourceFile(file, parserOptions);

			// If there was some kind of error parsing, then lets print some diagnostics.
			if (parserResult.Kind != ParserResultKind.Success)
			{
				if (parserResult.Kind == ParserResultKind.FileNotFound)
					Console.Error.WriteLine($"{file} was not found.");

				for (int i = 0; i < parserResult.DiagnosticsCount; i++)
				{
					var diag = parserResult.GetDiagnostics(i);

					Console.WriteLine("{0}({1},{2}): {3}: {4}",
						diag.FileName, diag.LineNumber, diag.ColumnNumber,
						diag.Level.ToString().ToLower(), diag.Message);
				}

				parserResult.Dispose();
				return false;
			}

			// Now we can consume the output of the parser (syntax tree).

			// First we will convert the output, bindings for the native Clang AST,
			// to CppSharp's managed AST representation.
			var astContext = (ASTContext)parserOptions.ASTContext;

			// After its converted, we can dispose of the native AST bindings.
			parserResult.Dispose();

			// Now we can finally do what we please with the syntax tree.
			//foreach (var sourceUnit in astContext.TranslationUnits)
			//	Console.WriteLine(sourceUnit.FileName);

			var walker = new ASTWalker(new Builder());

			var types = new List<Class>(astContext.FindClass("TDivMsgItem"));
			if (types.Count != 1)
			{
				Console.WriteLine("TDivMsgItem class is not defined. \n");
				return false;
			}

			walker.TDivMsgItemType = types[0];

			foreach (var tu in astContext.TranslationUnits)
			{
				tu.Visit(walker);
			}

			foreach (var cls in walker.Items)
			{
				Console.WriteLine($"{cls.Item1.Name} // {cls.Item1.Comment?.BriefText}\n");

				foreach (var item in cls.Item2)
				{
					Console.WriteLine($"  {item.Name} // {item.Comment?.BriefText}\n");
				}
			}

			return true;
		}
	}

	static class AstContextExtension
	{
		public static IEnumerable<Class> FindClass(this ASTContext context, string name)
		{
			foreach (var tu in context.TranslationUnits)
			{
				foreach (var cls in tu.Classes)
				{
					if (cls.Name == name)
						yield return cls;
				}
			}
		}
	}
}
