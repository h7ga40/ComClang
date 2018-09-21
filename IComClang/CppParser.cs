using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CppSharp.AST
{
	[ComVisible(true)]
	public interface ICppParserOptions
	{
		int ArgumentsCount { get; }
		string GetArguments(int index);
		void AddArguments(string value);

		string LibraryFile { get; }
		// C/C++ header file names.
		int SourceFilesCount { get; }
		string GetSourceFiles(int index);
		void AddSourceFiles(string value);

		// Include directories
		int IncludeDirsCount { get; }
		string GetIncludeDirs(int index);
		void AddIncludeDirs(string value);

		int SystemIncludeDirsCount { get; }
		string GetSystemIncludeDirs(int index);
		void AddSystemIncludeDirs(string value);

		int DefinesCount { get; }
		string GetDefines(int index);
		void AddDefines(string value);

		int UndefinesCount { get; }
		string GetUndefines(int index);
		void AddUndefines(string value);

		int LibraryDirsCount { get; }
		string GetLibraryDirs(int index);
		void AddLibraryDirs(string value);

		int SupportedStdTypesCount { get; }
		string GetSupportedStdTypes(int index);
		void AddSupportedStdTypes(string value);


		IASTContext ASTContext { get; }

		int ToolSetToUse { get; set; }
		string TargetTriple { get; set; }
		string CurrentDir { get; set; }
		CppAbi Abi { get; set; }

		bool NoStandardIncludes { get; set; }
		bool NoBuiltinIncludes { get; set; }
		bool MicrosoftMode { get; set; }
		bool Verbose { get; set; }
		bool UnityBuild { get; set; }
	};

	[ComVisible(true)]

	public enum ParserDiagnosticLevel
	{
		Ignored,
		Note,
		Warning,
		Error,
		Fatal
	};

	[ComVisible(true)]
	public interface IParserDiagnostic
	{
		string FileName { get; set; }
		string Message { get; set; }
		ParserDiagnosticLevel Level { get; set; }
		int LineNumber { get; set; }
		int ColumnNumber { get; set; }
	};

	[ComVisible(true)]
	public enum ParserResultKind
	{
		Success,
		Error,
		FileNotFound
	};

	[ComVisible(true)]
	public interface IParserResult
	{
		ParserResultKind Kind { get; set; }
		int DiagnosticsCount { get; }
		IParserDiagnostic GetDiagnostics(int index);
		void AddDiagnostics(IParserDiagnostic value);

		INativeLibrary Library { get; set; }
		IParserTargetInfo TargetInfo { get; set; }
		IntPtr CodeParser { get; set; }
	};

	[ComVisible(true)]
	public enum SourceLocationKind
	{
		Invalid,
		Builtin,
		CommandLine,
		System,
		User
	};

	[ComVisible(true)]
	public interface IClangParser
	{
		IParserResult ParseHeader(ICppParserOptions Opts);
		IParserResult ParseLibrary(ICppParserOptions Opts);
	};

	[ComVisible(true)]
	public enum ParserIntType
	{
		NoInt = 0,
		SignedChar,
		UnsignedChar,
		SignedShort,
		UnsignedShort,
		SignedInt,
		UnsignedInt,
		SignedLong,
		UnsignedLong,
		SignedLongLong,
		UnsignedLongLong
	};

	[ComVisible(true)]

	public interface IParserTargetInfo
	{
		string ABI { get; set; }
		ParserIntType Char16Type { get; set; }
		ParserIntType Char32Type { get; set; }
		ParserIntType Int64Type { get; set; }
		ParserIntType IntMaxType { get; set; }
		ParserIntType IntPtrType { get; set; }
		ParserIntType SizeType { get; set; }
		ParserIntType UIntMaxType { get; set; }
		ParserIntType WCharType { get; set; }
		ParserIntType WIntType { get; set; }
		uint BoolAlign { get; set; }
		uint BoolWidth { get; set; }
		uint CharAlign { get; set; }
		uint CharWidth { get; set; }
		uint Char16Align { get; set; }
		uint Char16Width { get; set; }
		uint Char32Align { get; set; }
		uint Char32Width { get; set; }
		uint HalfAlign { get; set; }
		uint HalfWidth { get; set; }
		uint FloatAlign { get; set; }
		uint FloatWidth { get; set; }
		uint DoubleAlign { get; set; }
		uint DoubleWidth { get; set; }
		uint ShortAlign { get; set; }
		uint ShortWidth { get; set; }
		uint IntAlign { get; set; }
		uint IntWidth { get; set; }
		uint IntMaxTWidth { get; set; }
		uint LongAlign { get; set; }
		uint LongWidth { get; set; }
		uint LongDoubleAlign { get; set; }
		uint LongDoubleWidth { get; set; }
		uint LongLongAlign { get; set; }
		uint LongLongWidth { get; set; }
		uint PointerAlign { get; set; }
		uint PointerWidth { get; set; }
		uint WCharAlign { get; set; }
		uint WCharWidth { get; set; }
		uint Float128Align { get; set; }
		uint Float128Width { get; set; }
	};
}
