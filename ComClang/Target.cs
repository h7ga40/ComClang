using CppSharp.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	public class ParserTargetInfo : IParserTargetInfo
	{
		public string ABI { get; set; }
		public ParserIntType Char16Type { get; set; }
		public ParserIntType Char32Type { get; set; }
		public ParserIntType Int64Type { get; set; }
		public ParserIntType IntMaxType { get; set; }
		public ParserIntType IntPtrType { get; set; }
		public ParserIntType SizeType { get; set; }
		public ParserIntType UIntMaxType { get; set; }
		public ParserIntType WCharType { get; set; }
		public ParserIntType WIntType { get; set; }
		public uint BoolAlign { get; set; }
		public uint BoolWidth { get; set; }
		public uint CharAlign { get; set; }
		public uint CharWidth { get; set; }
		public uint Char16Align { get; set; }
		public uint Char16Width { get; set; }
		public uint Char32Align { get; set; }
		public uint Char32Width { get; set; }
		public uint HalfAlign { get; set; }
		public uint HalfWidth { get; set; }
		public uint FloatAlign { get; set; }
		public uint FloatWidth { get; set; }
		public uint DoubleAlign { get; set; }
		public uint DoubleWidth { get; set; }
		public uint ShortAlign { get; set; }
		public uint ShortWidth { get; set; }
		public uint IntAlign { get; set; }
		public uint IntWidth { get; set; }
		public uint IntMaxTWidth { get; set; }
		public uint LongAlign { get; set; }
		public uint LongWidth { get; set; }
		public uint LongDoubleAlign { get; set; }
		public uint LongDoubleWidth { get; set; }
		public uint LongLongAlign { get; set; }
		public uint LongLongWidth { get; set; }
		public uint PointerAlign { get; set; }
		public uint PointerWidth { get; set; }
		public uint WCharAlign { get; set; }
		public uint WCharWidth { get; set; }
		public uint Float128Align { get; set; }
		public uint Float128Width { get; set; }
	}
}
