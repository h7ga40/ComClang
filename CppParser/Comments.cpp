/************************************************************************
*
* CppSharp
* Licensed under the simplified BSD license. All rights reserved.
*
************************************************************************/

#include "Parser.h"

#include <clang/AST/Comment.h>
#include <clang/AST/ASTContext.h>

#include "ComClang.h"

using namespace CppSharp::CppParser;

//-----------------------------------//

static CommentKind
ConvertRawCommentKind(clang::RawComment::CommentKind Kind)
{
	using clang::RawComment;

	switch (Kind)
	{
	case RawComment::RCK_Invalid: return CommentKind_Invalid;
	case RawComment::RCK_OrdinaryBCPL: return CommentKind_BCPL;
	case RawComment::RCK_OrdinaryC: return CommentKind_C;
	case RawComment::RCK_BCPLSlash: return CommentKind_BCPLSlash;
	case RawComment::RCK_BCPLExcl: return CommentKind_BCPLExcl;
	case RawComment::RCK_JavaDoc: return CommentKind_JavaDoc;
	case RawComment::RCK_Qt: return CommentKind_Qt;
	case RawComment::RCK_Merged: return CommentKind_Merged;
	}

	llvm_unreachable("Unknown comment kind");
}

IRawCommentPtr Parser::WalkRawComment(const clang::RawComment* RC)
{
	using namespace clang;

	auto& SM = c->getSourceManager();
	IRawCommentPtr Comment = pComClang->CreateRawComment();
	Comment->Kind = ConvertRawCommentKind(RC->getKind());
	Comment->Text = RC->getRawText(SM).str().c_str();
	Comment->BriefText = RC->getBriefText(c->getASTContext());

	return Comment;
}

static InlineCommandComment_RenderKind
ConvertRenderKind(clang::comments::InlineCommandComment::RenderKind Kind)
{
	using namespace clang::comments;
	switch (Kind)
	{
	case clang::comments::InlineCommandComment::RenderNormal:
		return InlineCommandComment_RenderKind_RenderNormal;
	case clang::comments::InlineCommandComment::RenderBold:
		return InlineCommandComment_RenderKind_RenderBold;
	case clang::comments::InlineCommandComment::RenderMonospaced:
		return InlineCommandComment_RenderKind_RenderMonospaced;
	case clang::comments::InlineCommandComment::RenderEmphasized:
		return InlineCommandComment_RenderKind_RenderEmphasized;
	}
	llvm_unreachable("Unknown render kind");
}

static ParamCommandComment_PassDirection
ConvertParamPassDirection(clang::comments::ParamCommandComment::PassDirection Dir)
{
	using namespace clang::comments;
	switch (Dir)
	{
	case clang::comments::ParamCommandComment::In:
		return ParamCommandComment_PassDirection_In;
	case clang::comments::ParamCommandComment::Out:
		return ParamCommandComment_PassDirection_Out;
	case clang::comments::ParamCommandComment::InOut:
		return ParamCommandComment_PassDirection_InOut;
	}
	llvm_unreachable("Unknown parameter pass direction");
}

static void HandleInlineContent(const clang::comments::InlineContentComment *CK,
	IInlineContentCommentPtr IC)
{
	IC->HasTrailingNewline = CK->hasTrailingNewline();
}

static void HandleBlockCommand(IComClangPtr pComClang, const clang::comments::BlockCommandComment *CK,
	IBlockCommandCommentPtr BC)
{
	BC->CommandId = CK->getCommandID();
	for (unsigned I = 0, E = CK->getNumArgs(); I != E; ++I)
	{
		IBlockCommandComment_ArgumentPtr Arg = pComClang->CreateBlockCommandComment_Argument();
		Arg->Text = CK->getArgText(I).str().c_str();
		BC->AddArguments(Arg);
	}
}

static ICommentPtr ConvertCommentBlock(IComClangPtr pComClang, clang::comments::Comment* C)
{
	using namespace clang;
	using clang::comments::Comment;

	// This needs to have an underscore else we get an ICE under VS2012.
	ICommentPtr _Comment = nullptr;

	switch (C->getCommentKind())
	{
	case Comment::FullCommentKind:
	{
		auto CK = cast<clang::comments::FullComment>(C);
		IFullCommentPtr FC = pComClang->CreateFullComment();
		_Comment = ICommentPtr(FC);
		for (auto I = CK->child_begin(), E = CK->child_end(); I != E; ++I)
		{
			IBlockContentCommentPtr Content = IBlockContentCommentPtr(ConvertCommentBlock(pComClang, *I));
			FC->AddBlock(Content);
		}
		break;
	}
	case Comment::BlockCommandCommentKind:
	{
		auto CK = cast<const clang::comments::BlockCommandComment>(C);
		IBlockCommandCommentPtr BC = pComClang->CreateBlockCommandComment();
		_Comment = ICommentPtr(BC);
		HandleBlockCommand(pComClang, CK, BC);
		BC->ParagraphComment = IParagraphCommentPtr(ConvertCommentBlock(pComClang, CK->getParagraph()));
		break;
	}
	case Comment::ParamCommandCommentKind:
	{
		auto CK = cast<clang::comments::ParamCommandComment>(C);
		IParamCommandCommentPtr PC = pComClang->CreateParamCommandComment();
		_Comment = ICommentPtr(PC);
		IBlockCommandCommentPtr PCb = IBlockCommandCommentPtr(PC);
		HandleBlockCommand(pComClang, CK, PCb);
		PC->Direction = ConvertParamPassDirection(CK->getDirection());
		if (CK->isParamIndexValid() && !CK->isVarArgParam())
			PC->ParamIndex = CK->getParamIndex();
		PCb->ParagraphComment = IParagraphCommentPtr(ConvertCommentBlock(pComClang, CK->getParagraph()));
		break;
	}
	case Comment::TParamCommandCommentKind:
	{
		auto CK = cast<clang::comments::TParamCommandComment>(C);
		ITParamCommandCommentPtr TC = pComClang->CreateTParamCommandComment();
		_Comment = ICommentPtr(TC);
		IBlockCommandCommentPtr TCb = IBlockCommandCommentPtr(TC);
		HandleBlockCommand(pComClang, CK, TCb);
		if (CK->isPositionValid())
			for (unsigned I = 0, E = CK->getDepth(); I != E; ++I)
				TC->AddPosition(CK->getIndex(I));
		TCb->ParagraphComment = IParagraphCommentPtr(ConvertCommentBlock(pComClang, CK->getParagraph()));
		break;
	}
	case Comment::VerbatimBlockCommentKind:
	{
		auto CK = cast<clang::comments::VerbatimBlockComment>(C);
		IVerbatimBlockCommentPtr VB = pComClang->CreateVerbatimBlockComment();
		_Comment = ICommentPtr(VB);
		for (auto I = CK->child_begin(), E = CK->child_end(); I != E; ++I)
		{
			IVerbatimBlockLineCommentPtr Line = IVerbatimBlockLineCommentPtr(ConvertCommentBlock(pComClang, *I));
			VB->AddLine(Line);
		}
		break;
	}
	case Comment::VerbatimLineCommentKind:
	{
		auto CK = cast<clang::comments::VerbatimLineComment>(C);
		IVerbatimLineCommentPtr VL = pComClang->CreateVerbatimLineComment();
		_Comment = ICommentPtr(VL);
		VL->Text = CK->getText().str().c_str();
		break;
	}
	case Comment::ParagraphCommentKind:
	{
		auto CK = cast<clang::comments::ParagraphComment>(C);
		IParagraphCommentPtr PC = pComClang->CreateParagraphComment();
		_Comment = ICommentPtr(PC);
		for (auto I = CK->child_begin(), E = CK->child_end(); I != E; ++I)
		{
			auto Content = ConvertCommentBlock(pComClang, *I);
			PC->AddContent(IInlineContentCommentPtr(Content));
		}
		PC->IsWhitespace = CK->isWhitespace();
		break;
	}
	case Comment::HTMLStartTagCommentKind:
	{
		auto CK = cast<clang::comments::HTMLStartTagComment>(C);
		IHTMLStartTagCommentPtr TC = pComClang->CreateHTMLStartTagComment();
		_Comment = ICommentPtr(TC);
		HandleInlineContent(CK, IInlineContentCommentPtr(TC));
		TC->TagName = CK->getTagName().str().c_str();
		for (unsigned I = 0, E = CK->getNumAttrs(); I != E; ++I)
		{
			auto A = CK->getAttr(I);
			IHTMLStartTagComment_AttributePtr Attr = pComClang->CreateHTMLStartTagComment_Attribute();
			Attr->name = A.Name.str().c_str();
			Attr->value = A.Value.str().c_str();
			TC->AddAttribute(Attr);
		}
		break;
	}
	case Comment::HTMLEndTagCommentKind:
	{
		auto CK = cast<clang::comments::HTMLEndTagComment>(C);
		IHTMLEndTagCommentPtr TC = pComClang->CreateHTMLEndTagComment();
		_Comment = ICommentPtr(TC);
		HandleInlineContent(CK, IInlineContentCommentPtr(TC));
		TC->TagName = CK->getTagName().str().c_str();
		break;
	}
	case Comment::TextCommentKind:
	{
		auto CK = cast<clang::comments::TextComment>(C);
		ITextCommentPtr TC = pComClang->CreateTextComment();
		_Comment = ICommentPtr(TC);
		HandleInlineContent(CK, IInlineContentCommentPtr(TC));
		TC->Text = CK->getText().str().c_str();
		break;
	}
	case Comment::InlineCommandCommentKind:
	{
		auto CK = cast<clang::comments::InlineCommandComment>(C);
		IInlineCommandCommentPtr IC = pComClang->CreateInlineCommandComment();
		_Comment = ICommentPtr(IC);
		HandleInlineContent(CK, IInlineContentCommentPtr(IC));
		IC->CommandId = CK->getCommandID();
		IC->CommentRenderKind = ConvertRenderKind(CK->getRenderKind());
		for (unsigned I = 0, E = CK->getNumArgs(); I != E; ++I)
		{
			IInlineCommandComment_ArgumentPtr Arg = pComClang->CreateInlineCommandComment_Argument();
			Arg->Text = CK->getArgText(I).str().c_str();
			IC->AddArguments(Arg);
		}
		break;
	}
	case Comment::VerbatimBlockLineCommentKind:
	{
		auto CK = cast<clang::comments::VerbatimBlockLineComment>(C);
		IVerbatimBlockLineCommentPtr VL = pComClang->CreateVerbatimBlockLineComment();
		_Comment = ICommentPtr(VL);
		VL->Text = CK->getText().str().c_str();
		break;
	}
	case Comment::NoCommentKind: return nullptr;
	default:
		llvm_unreachable("Unknown comment kind");
	}

	assert(_Comment && "Invalid comment instance");
	return _Comment;
}

void Parser::HandleComments(const clang::Decl* D, IDeclarationPtr Decl)
{
	using namespace clang;

	const clang::RawComment* RC = 0;
	if (!(RC = c->getASTContext().getRawCommentForAnyRedecl(D)))
		return;

	auto RawComment = WalkRawComment(RC);
	Decl->Comment = RawComment;

	if (clang::comments::FullComment* FC = RC->parse(c->getASTContext(), &c->getPreprocessor(), D))
	{
		IFullCommentPtr CB = IFullCommentPtr(ConvertCommentBlock(pComClang, FC));
		RawComment->FullCommentBlock = CB;
	}
}
