using System;
using System.Collections.Generic;

namespace CppSharp.AST
{
	/// <summary>
	/// Represents a raw C++ comment.
	/// </summary>
	public class RawComment : IRawComment
	{
		/// <summary>
		/// Kind of the comment.
		/// </summary>
		public CommentKind Kind { get; set; }

		/// <summary>
		/// Raw text of the comment.
		/// </summary>
		public string Text { get; set; } = "";

		/// <summary>
		/// Brief text if it is a documentation comment.
		/// </summary>
		public string BriefText { get; set; } = "";

		/// <summary>
		/// Returns if the comment is invalid.
		/// </summary>
		public bool IsInvalid {
			get { return Kind == CommentKind.Invalid; }
		}

		/// <summary>
		/// Returns if the comment is ordinary (non-documentation).
		/// </summary>
		public bool IsOrdinary {
			get {
				return Kind == CommentKind.BCPL ||
					   Kind == CommentKind.C;
			}
		}

		/// <summary>
		/// Returns if this is a documentation comment.
		/// </summary>
		public bool IsDocumentation {
			get { return !IsInvalid && !IsOrdinary; }
		}

		public IFullComment FullCommentBlock { get => FullComment; set => FullComment = (FullComment)value; }

		/// <summary>
		/// Provides the full comment information.
		/// </summary>
		public FullComment FullComment;
	}

	/// <summary>
	/// Visitor for comments.
	/// </summary>
	public interface ICommentVisitor<out T>
	{
		T VisitBlockCommand(BlockCommandComment comment);
		T VisitParamCommand(ParamCommandComment comment);
		T VisitTParamCommand(TParamCommandComment comment);
		T VisitVerbatimBlock(VerbatimBlockComment comment);
		T VisitVerbatimLine(VerbatimLineComment comment);
		T VisitParagraphCommand(ParagraphComment comment);
		T VisitFull(FullComment comment);
		T VisitHTMLStartTag(HTMLStartTagComment comment);
		T VisitHTMLEndTag(HTMLEndTagComment comment);
		T VisitText(TextComment comment);
		T VisitInlineCommand(InlineCommandComment comment);
		T VisitVerbatimBlockLine(VerbatimBlockLineComment comment);
	}

	/// <summary>
	/// Any part of the comment.
	/// </summary>
	public abstract class Comment : IComment
	{
		public DocumentationCommentKind Kind { get; set; }

		public abstract void Visit<T>(ICommentVisitor<T> visitor);

		public static string GetMultiLineCommentPrologue(CommentKind kind)
		{
			switch (kind)
			{
			case CommentKind.BCPL:
			case CommentKind.BCPLExcl:
				return "//";
			case CommentKind.C:
			case CommentKind.JavaDoc:
			case CommentKind.Qt:
				return " *";
			case CommentKind.BCPLSlash:
				return "///";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static string GetLineCommentPrologue(CommentKind kind)
		{
			switch (kind)
			{
			case CommentKind.BCPL:
			case CommentKind.BCPLSlash:
				return string.Empty;
			case CommentKind.C:
				return "/*";
			case CommentKind.BCPLExcl:
				return "//!";
			case CommentKind.JavaDoc:
				return "/**";
			case CommentKind.Qt:
				return "/*!";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static string GetLineCommentEpilogue(CommentKind kind)
		{
			switch (kind)
			{
			case CommentKind.BCPL:
			case CommentKind.BCPLSlash:
			case CommentKind.BCPLExcl:
				return string.Empty;
			case CommentKind.C:
			case CommentKind.JavaDoc:
			case CommentKind.Qt:
				return " */";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	#region Comments

	/// <summary>
	/// A full comment attached to a declaration, contains block content.
	/// </summary>
	public class FullComment : Comment, IFullComment
	{
		public List<BlockContentComment> Blocks;

		public FullComment()
		{
			Blocks = new List<BlockContentComment>();
		}

		public int BlockCount => Blocks.Count;

		public void AddBlock(IBlockContentComment value)
		{
			Blocks.Add((BlockContentComment)value);
		}

		public IBlockContentComment GetBlock(int index)
		{
			return Blocks[index];
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitFull(this);
		}
	}

	/// <summary>
	/// Block content (contains inline content).
	/// </summary>
	public abstract class BlockContentComment : Comment, IBlockContentComment
	{

	}

	/// <summary>
	/// A command that has zero or more word-like arguments (number of
	/// word-like arguments depends on command name) and a paragraph as
	/// an argument (e. g., \brief).
	/// </summary>
	public class BlockCommandComment : BlockContentComment, IBlockCommandComment
	{
		public class Argument : IBlockCommandComment_Argument
		{
			public string Text { get; set; } = "";
		}

		public uint CommandId { get; set; }

		public CommentCommandKind CommandKind {
			get { return (CommentCommandKind)CommandId; }
		}

		public ParagraphComment ParagraphComment { get; set; }
		IParagraphComment IBlockCommandComment.ParagraphComment { get => ParagraphComment; set => ParagraphComment = (ParagraphComment)value; }

		public int ArgumentsCount => Arguments.Count;

		public List<Argument> Arguments;

		public BlockCommandComment()
		{
			Kind = DocumentationCommentKind.BlockCommandComment;
			Arguments = new List<Argument>();
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitBlockCommand(this);
		}

		public IBlockCommandComment_Argument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(IBlockCommandComment_Argument value)
		{
			Arguments.Add((Argument)value);
		}
	}

	/// <summary>
	/// Doxygen \param command.
	/// </summary>
	public class ParamCommandComment : BlockCommandComment, IParamCommandComment
	{
		public const uint InvalidParamIndex = ~0U;
		public const uint VarArgParamIndex = ~0U/*InvalidParamIndex*/ - 1U;

		public ParamCommandComment()
		{
			Kind = DocumentationCommentKind.ParamCommandComment;
		}

		public bool IsParamIndexValid {
			get { return ParamIndex != InvalidParamIndex; }
		}

		public bool IsVarArgParam {
			get { return ParamIndex == VarArgParamIndex; }
		}

		public uint ParamIndex { get; set; }

		public ParamCommandComment_PassDirection Direction { get; set; }

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitParamCommand(this);
		}
	}

	/// <summary>
	/// Doxygen \tparam command, describes a template parameter.
	/// </summary>
	public class TParamCommandComment : BlockCommandComment, ITParamCommandComment
	{
		/// If this template parameter name was resolved (found in template parameter
		/// list), then this stores a list of position indexes in all template
		/// parameter lists.
		///
		/// For example:
		/// \verbatim
		///     template<typename C, template<typename T> class TT>
		///     void test(TT<int> aaa);
		/// \endverbatim
		/// For C:  Position = { 0 }
		/// For TT: Position = { 1 }
		/// For T:  Position = { 1, 0 }
		public List<uint> Position;

		public TParamCommandComment()
		{
			Kind = DocumentationCommentKind.TParamCommandComment;
			Position = new List<uint>();
		}

		public int PositionCount => Position.Count;

		public void AddPosition(uint value)
		{
			Position.Add(value);
		}

		public uint GetPosition(int index)
		{
			return Position[index];
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitTParamCommand(this);
		}
	}

	/// <summary>
	/// A verbatim block command (e. g., preformatted code). Verbatim block
	/// has an opening and a closing command and contains multiple lines of
	/// text (VerbatimBlockLineComment nodes).
	/// </summary>
	public class VerbatimBlockComment : BlockCommandComment, IVerbatimBlockComment
	{
		public List<VerbatimBlockLineComment> Lines;

		public VerbatimBlockComment()
		{
			Kind = DocumentationCommentKind.VerbatimBlockComment;
			Lines = new List<VerbatimBlockLineComment>();
		}

		public int LineCount => Lines.Count;

		public void AddLine(IVerbatimBlockLineComment value)
		{
			Lines.Add((VerbatimBlockLineComment)value);
		}

		public IVerbatimBlockLineComment GetLine(int index)
		{
			return Lines[index];
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitVerbatimBlock(this);
		}
	}

	/// <summary>
	/// A verbatim line command. Verbatim line has an opening command, a
	/// single line of text (up to the newline after the opening command)
	/// and has no closing command.
	/// </summary>
	public class VerbatimLineComment : BlockCommandComment, IVerbatimLineComment
	{
		public string Text { get; set; } = "";

		public VerbatimLineComment()
		{
			Kind = DocumentationCommentKind.VerbatimLineComment;
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitVerbatimLine(this);
		}
	}

	/// <summary>
	/// A single paragraph that contains inline content.
	/// </summary>
	public class ParagraphComment : BlockContentComment, IParagraphComment
	{
		public List<InlineContentComment> Content;

		public bool IsWhitespace { get; set; }

		public int ContentCount => Content.Count;

		public ParagraphComment()
		{
			Kind = DocumentationCommentKind.ParagraphComment;
			Content = new List<InlineContentComment>();
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitParagraphCommand(this);
		}

		public IInlineContentComment GetContent(int index)
		{
			return Content[index];
		}

		public void AddContent(IInlineContentComment value)
		{
			Content.Add((InlineContentComment)value);
		}
	}

	/// <summary>
	/// Inline content (contained within a block).
	/// </summary>
	public abstract class InlineContentComment : Comment, IInlineContentComment
	{
		protected InlineContentComment()
		{
			Kind = DocumentationCommentKind.InlineContentComment;
		}

		public bool HasTrailingNewline { get; set; }
	}

	/// <summary>
	/// Abstract class for opening and closing HTML tags. HTML tags are
	/// always treated as inline content (regardless HTML semantics);
	/// opening and closing tags are not matched.
	/// </summary>
	public abstract class HTMLTagComment : InlineContentComment, IHTMLTagComment
	{
		public string TagName { get; set; } = "";

		protected HTMLTagComment()
		{
			Kind = DocumentationCommentKind.HTMLTagComment;
		}
	}

	/// <summary>
	/// An opening HTML tag with attributes.
	/// </summary>
	public class HTMLStartTagComment : HTMLTagComment, IHTMLStartTagComment
	{
		public class Attribute : IHTMLStartTagComment_Attribute
		{
			public string Name { get; set; } = "";
			public string Value { get; set; } = "";
		}

		public List<Attribute> Attributes;

		public int AttributeCount => Attributes.Count;

		public HTMLStartTagComment()
		{
			Kind = DocumentationCommentKind.HTMLStartTagComment;
			Attributes = new List<Attribute>();
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitHTMLStartTag(this);
		}

		public IHTMLStartTagComment_Attribute GetAttribute(int index)
		{
			return Attributes[index];
		}

		public void AddAttribute(IHTMLStartTagComment_Attribute value)
		{
			Attributes.Add((Attribute)value);
		}
	}

	/// <summary>
	/// A closing HTML tag.
	/// </summary>
	public class HTMLEndTagComment : HTMLTagComment, IHTMLEndTagComment
	{
		public HTMLEndTagComment()
		{
			Kind = DocumentationCommentKind.HTMLEndTagComment;
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitHTMLEndTag(this);
		}
	}

	/// <summary>
	/// Plain text.
	/// </summary>
	public class TextComment : InlineContentComment, ITextComment
	{
		public string Text { get; set; } = "";

		public TextComment()
		{
			Kind = DocumentationCommentKind.TextComment;
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitText(this);
		}
	}

	/// <summary>
	/// A command with word-like arguments that is considered inline content.
	/// </summary>
	public class InlineCommandComment : InlineContentComment, IInlineCommandComment
	{
		public class Argument : IInlineCommandComment_Argument
		{
			public string Text { get; set; } = "";
		}

		public uint CommandId { get; set; }

		public CommentCommandKind CommandKind {
			get { return (CommentCommandKind)CommandId; }
		}

		public int ArgumentsCount => Arguments.Count;

		public InlineCommandComment_RenderKind CommentRenderKind { get; set; }

		public List<Argument> Arguments;

		public InlineCommandComment()
		{
			Kind = DocumentationCommentKind.InlineCommandComment;
			Arguments = new List<Argument>();
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitInlineCommand(this);
		}

		public IInlineCommandComment_Argument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(IInlineCommandComment_Argument value)
		{
			Arguments.Add((Argument)value);
		}
	}

	/// <summary>
	/// A line of text contained in a verbatim block.
	/// </summary>
	public class VerbatimBlockLineComment : Comment, IVerbatimBlockLineComment
	{
		public string Text { get; set; } = "";

		public VerbatimBlockLineComment()
		{
			Kind = DocumentationCommentKind.VerbatimBlockLineComment;
		}

		public override void Visit<T>(ICommentVisitor<T> visitor)
		{
			visitor.VisitVerbatimBlockLine(this);
		}
	}

	#endregion

	#region Commands

	/// <summary>
	/// Kinds of comment commands.
	/// Synchronized from "clang/AST/CommentCommandList.inc".
	/// </summary>
	public enum CommentCommandKind
	{
		A,
		Abstract,
		Addtogroup,
		Arg,
		Attention,
		Author,
		Authors,
		B,
		Brief,
		Bug,
		C,
		Callback,
		Category,
		Class,
		Classdesign,
		Coclass,
		Code,
		Endcode,
		Const,
		Constant,
		Copyright,
		Date,
		Def,
		Defgroup,
		Dependency,
		Deprecated,
		Details,
		Discussion,
		Dot,
		Enddot,
		E,
		Em,
		Enum,
		Exception,
		Flbrace,
		Frbrace,
		Flsquare,
		Frsquare,
		Fdollar,
		Fn,
		Function,
		Functiongroup,
		Headerfile,
		Helper,
		Helperclass,
		Helps,
		Htmlonly,
		Endhtmlonly,
		Ingroup,
		Instancesize,
		Interface,
		Invariant,
		Latexonly,
		Endlatexonly,
		Li,
		Link,
		Slashlink,
		Mainpage,
		Manonly,
		Endmanonly,
		Method,
		Methodgroup,
		Msc,
		Endmsc,
		Name,
		Namespace,
		Note,
		Overload,
		Ownership,
		P,
		Par,
		Paragraph,
		Param,
		Performance,
		Post,
		Pre,
		Property,
		Protocol,
		Ref,
		Related,
		Relatedalso,
		Relates,
		Relatesalso,
		Remark,
		Remarks,
		Result,
		Return,
		Returns,
		Rtfonly,
		Endrtfonly,
		Sa,
		Section,
		Security,
		See,
		Seealso,
		Short,
		Since,
		Struct,
		Subpage,
		Subsection,
		Subsubsection,
		Superclass,
		Template,
		Templatefield,
		Textblock,
		Slashtextblock,
		Throw,
		Throws,
		Todo,
		Tparam,
		Typedef,
		Union,
		Var,
		Verbatim,
		Endverbatim,
		Version,
		Warning,
		Weakgroup,
		Xmlonly,
		Endxmlonly
	}

	#endregion

}
