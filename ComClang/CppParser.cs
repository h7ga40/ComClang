using ComClang;
using CppSharp.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CppSharp
{

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class ParserDiagnostic : IParserDiagnostic
	{
		public string FileName { get; set; } = "";
		public string Message { get; set; } = "";
		public ParserDiagnosticLevel Level { get; set; }
		public int LineNumber { get; set; }
		public int ColumnNumber { get; set; }
	}

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class ParserResult : IParserResult
	{
		public List<ParserDiagnostic> Diagnostics { get; } = new List<ParserDiagnostic>();

		public ParserResultKind Kind { get; set; }

		public int DiagnosticsCount => Diagnostics.Count;

		public INativeLibrary Library { get; set; }
		public IParserTargetInfo TargetInfo { get; set; }
		public IntPtr CodeParser { get; set; }

		public event EventHandler<ParserDiagnostic> AddedDiagnostics;

		public void AddDiagnostics(IParserDiagnostic value)
		{
			var Diag = (ParserDiagnostic)value;

			Diagnostics.Add(Diag);

			AddedDiagnostics?.Invoke(this, Diag);
		}

		public IParserDiagnostic GetDiagnostics(int index)
		{
			return Diagnostics[index];
		}

		public void Dispose()
		{
		}
	}

	public class CppParserOptions : ICppParserOptions
	{
		public int ArgumentsCount => Arguments.Count;

		public string LibraryFile { get; set; } = "";

		public int SourceFilesCount => SourceFiles.Count;

		public int IncludeDirsCount => IncludeDirs.Count;

		public int SystemIncludeDirsCount => SystemIncludeDirs.Count;

		public int DefinesCount => Defines.Count;

		public int UndefinesCount => Undefines.Count;

		public int LibraryDirsCount => LibraryDirs.Count;

		public int SupportedStdTypesCount => SupportedStdTypes.Count;

		public IASTContext ASTContext { get; set; }

		public int ToolSetToUse { get; set; }
		public string TargetTriple { get; set; } = "";
		public string CurrentDir { get; set; } = "";
		public CppAbi Abi { get; set; }
		public bool NoStandardIncludes { get; set; }
		public bool NoBuiltinIncludes { get; set; }
		public bool MicrosoftMode { get; set; }
		public bool Verbose { get; set; }
		public bool UnityBuild { get; set; }

		public List<string> Arguments { get; } = new List<string>();
		public List<string> Defines { get; } = new List<string>();
		public List<string> IncludeDirs { get; } = new List<string>();
		public List<string> LibraryDirs { get; } = new List<string>();
		public List<string> SourceFiles { get; } = new List<string>();
		public List<string> SupportedStdTypes { get; } = new List<string>();
		public List<string> SystemIncludeDirs { get; } = new List<string>();
		public List<string> Undefines { get; } = new List<string>();

		public void AddArguments(string value)
		{
			Arguments.Add(value);
		}

		public void AddDefines(string value)
		{
			Defines.Add(value);
		}

		public void AddIncludeDirs(string value)
		{
			IncludeDirs.Add(value);
		}

		public void AddLibraryDirs(string value)
		{
			LibraryDirs.Add(value);
		}

		public void AddSourceFiles(string value)
		{
			SourceFiles.Add(value);
		}

		public void AddSupportedStdTypes(string value)
		{
			SupportedStdTypes.Add(value);
		}

		public void AddSystemIncludeDirs(string value)
		{
			SystemIncludeDirs.Add(value);
		}

		public void AddUndefines(string value)
		{
			Undefines.Add(value);
		}

		public string GetArguments(int index)
		{
			return Arguments[index];
		}

		public string GetDefines(int index)
		{
			return Defines[index];
		}

		public string GetIncludeDirs(int index)
		{
			return IncludeDirs[index];
		}

		public string GetLibraryDirs(int index)
		{
			return LibraryDirs[index];
		}

		public string GetSourceFiles(int index)
		{
			return SourceFiles[index];
		}

		public string GetSupportedStdTypes(int index)
		{
			return SupportedStdTypes[index];
		}

		public string GetSystemIncludeDirs(int index)
		{
			return SystemIncludeDirs[index];
		}

		public string GetUndefines(int index)
		{
			return Undefines[index];
		}
	}

	public class ClangParser : IClangParser
	{
		[DllImport("CppParser")]
		extern static IParserResult Parser_ParseHeader(IComClang pComClang, ICppParserOptions pOpts);
		[DllImport("CppParser")]
		extern static IParserResult Parser_ParseLibrary(IComClang pComClang, ICppParserOptions pOpts);
		[DllImport("CppParser")]
		extern static IParserResult Parser_ParseForAST(IComClang pComClang, ICppParserOptions pOpts);

		public ComClang.ComClang ComClang { get; }

		/// <summary>
		/// Context with translation units ASTs.
		/// </summary>
		public ASTContext ASTContext { get; private set; }

		/// <summary>
		/// Fired when source files are parsed.
		/// </summary>
		public Action<IEnumerable<string>, ParserResult> SourcesParsed = delegate { };

		/// <summary>
		/// Fired when library files are parsed.
		/// </summary>
		public Action<string, ParserResult> LibraryParsed = delegate { };

		public ClangParser()
		{
			ComClang = new ComClang.ComClang();
			ASTContext = new ASTContext();
		}

		public ClangParser(ASTContext context)
		{
			ASTContext = context;
		}

		public IParserResult ParseHeader(ICppParserOptions Opts)
		{
			return Parser_ParseHeader(ComClang, Opts);
		}

		public IParserResult ParseLibrary(ICppParserOptions Opts)
		{
			return Parser_ParseLibrary(ComClang, Opts);
		}

		public IParserResult ParseForAST(ICppParserOptions Opts)
		{
			return Parser_ParseForAST(ComClang, Opts);
		}

		/// <summary>
		/// Parses a C++ source file as a translation unit.
		/// </summary>
		public ParserResult ParseSourceFile(string file, ParserOptions options)
		{
			return ParseSourceFiles(new[] { file }, options);
		}

		/// <summary>
		/// Parses a set of C++ source files as a single translation unit.
		/// </summary>
		public ParserResult ParseSourceFiles(string[] files, ParserOptions options)
		{
			options.ASTContext = ASTContext;

			foreach (var file in files)
				options.AddSourceFiles(file);

			var result = (ParserResult)ParseHeader(options);
			//var result = (ParserResult)ParseForAST(options);
			SourcesParsed(files, result);

			return result;
		}

		/// <summary>
		/// Parses a library file with symbols.
		/// </summary>
		public ParserResult ParseLibrary(string file, ParserOptions options)
		{
			options.LibraryFile = file;

			var result = (ParserResult)ParseLibrary(options);
			LibraryParsed(file, result);

			return result;
		}

		/// <summary>
		/// Converts a native parser AST to a managed AST.
		/// </summary>
		static public ASTContext ConvertASTContext(ASTContext context)
		{
			return context;
		}

		public static AST.NativeLibrary ConvertLibrary(NativeLibrary library)
		{
			var newLibrary = new AST.NativeLibrary
			{
				FileName = library.FileName,
				ArchType = (ArchType)library.ArchType
			};

			for (int i = 0; i < library.SymbolsCount; ++i)
			{
				var symbol = library.GetSymbols(i);
				newLibrary.Symbols.Add(symbol);
			}
			for (int i = 0; i < library.DependenciesCount; i++)
			{
				newLibrary.Dependencies.Add(library.GetDependencies(i));
			}

			return newLibrary;
		}
	}
}
