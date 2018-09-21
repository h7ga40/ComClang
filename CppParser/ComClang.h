#ifndef _CLANGDOX_H_
#define _CLANGDOX_H_

#include "clang/AST/AST.h"
#include "clang/AST/ASTContext.h"
#include "clang/AST/ASTConsumer.h"
#include "clang/AST/DeclVisitor.h"
#include "clang/Frontend/ASTConsumers.h"
#include "clang/Frontend/FrontendActions.h"
#include "clang/Frontend/CompilerInstance.h"
#include "clang/Lex/Preprocessor.h"
#include "clang/Tooling/CommonOptionsParser.h"
#include "clang/Tooling/Tooling.h"

#include "AST.h"
#include "CppParser.h"

struct IComClang;
enum DeclKind;
enum StmtClass;
struct IDecl;
struct IStmt;
struct ISourceLocation;
struct ISourceRange;
struct ICorRuntimeHost;

class DoxPPCallbacks : public clang::PPCallbacks {
private:
	clang::Preprocessor & PP;
public:
	DoxPPCallbacks(clang::Preprocessor &pp);
	void FileChanged(clang::SourceLocation Loc, FileChangeReason Reason,
		clang::SrcMgr::CharacteristicKind FileType,
		clang::FileID PrevFID = clang::FileID()) override;
	void FileSkipped(const clang::FileEntry &SkippedFile,
		const clang::Token &FilenameTok,
		clang::SrcMgr::CharacteristicKind FileType) override;
	bool FileNotFound(StringRef FileName,
		llvm::SmallVectorImpl<char> &RecoveryPath) override;
	void InclusionDirective(clang::SourceLocation HashLoc,
		const clang::Token &IncludeTok,
		StringRef FileName,
		bool IsAngled,
		clang::CharSourceRange FilenameRange,
		const clang::FileEntry *File,
		StringRef SearchPath,
		StringRef RelativePath,
		const clang::Module *Imported) override;
	void moduleImport(clang::SourceLocation ImportLoc,
		clang::ModuleIdPath Path,
		const clang::Module *Imported) override;
	void EndOfMainFile() override;
	void Ident(clang::SourceLocation Loc, StringRef str) override;
	void PragmaDirective(clang::SourceLocation Loc,
		clang::PragmaIntroducerKind Introducer) override;
	void PragmaComment(clang::SourceLocation Loc, const clang::IdentifierInfo *Kind,
		StringRef Str) override;
	void PragmaDetectMismatch(clang::SourceLocation Loc, StringRef Name,
		StringRef Value) override;
	void PragmaDebug(clang::SourceLocation Loc, StringRef DebugType) override;
	void PragmaMessage(clang::SourceLocation Loc, StringRef Namespace,
		PragmaMessageKind Kind, StringRef Str) override;
	void PragmaDiagnosticPush(clang::SourceLocation Loc,
		StringRef Namespace) override;
	void PragmaDiagnosticPop(clang::SourceLocation Loc,
		StringRef Namespace) override;
	void PragmaDiagnostic(clang::SourceLocation Loc, StringRef Namespace,
		clang::diag::Severity mapping, StringRef Str) override;
	void PragmaOpenCLExtension(clang::SourceLocation NameLoc,
		const clang::IdentifierInfo *Name,
		clang::SourceLocation StateLoc, unsigned State) override;
	void PragmaWarning(clang::SourceLocation Loc, StringRef WarningSpec,
		llvm::ArrayRef<int> Ids) override;
	void PragmaWarningPush(clang::SourceLocation Loc, int Level) override;
	void PragmaWarningPop(clang::SourceLocation Loc) override;
	void MacroExpands(const clang::Token &MacroNameTok,
		const clang::MacroDefinition &MD, clang::SourceRange Range,
		const clang::MacroArgs *Args) override;
	void MacroDefined(const clang::Token &MacroNameTok,
		const clang::MacroDirective *MD) override;
	void Defined(const clang::Token &MacroNameTok, const clang::MacroDefinition &MD,
		clang::SourceRange Range) override;
	void If(clang::SourceLocation Loc, clang::SourceRange ConditionRange,
		ConditionValueKind ConditionValue) override;
	void Elif(clang::SourceLocation Loc, clang::SourceRange ConditionRange,
		ConditionValueKind ConditionValue, clang::SourceLocation IfLoc) override;
	void Ifdef(clang::SourceLocation Loc, const clang::Token &MacroNameTok,
		const clang::MacroDefinition &MD) override;
	void Ifndef(clang::SourceLocation Loc, const clang::Token &MacroNameTok,
		const clang::MacroDefinition &MD) override;
	void Else(clang::SourceLocation Loc, clang::SourceLocation IfLoc) override;
	void Endif(clang::SourceLocation Loc, clang::SourceLocation IfLoc) override;
};

class ASTWalker :
	public clang::DeclVisitor<ASTWalker, IDecl*>,
	public clang::StmtVisitor<ASTWalker, IStmt*> {
private:
	IComClangPtr pComClang;
	clang::PrintingPolicy      Policy;
	const clang::SourceManager &SM;
	IDeclPtr Parent;
	IStmtPtr ParentStmt;
public:
	ASTWalker(IComClangPtr pComClang, clang::CompilerInstance *CI);
	void WalkTranslationUnitDecl(clang::TranslationUnitDecl *tu);
public:
	void WalkDeclContext(IDeclPtr parent, clang::DeclContext *context);
	void WalkDecl(IDeclPtr dst, clang::Decl *src);
	void WalkNamedDecl(IDeclPtr dst, clang::NamedDecl *src);
	void WalkObjCContainerDecl(IDeclPtr dst, clang::ObjCContainerDecl *src);
	void WalkObjCImplDecl(IDeclPtr dst, clang::ObjCImplDecl *src);
	void WalkTemplateDecl(IDeclPtr dst, clang::TemplateDecl *src);
	void WalkRedeclarableTemplateDecl(IDeclPtr dst, clang::RedeclarableTemplateDecl *src);
	void WalkTagDecl(IDeclPtr dst, clang::TagDecl *src);
	void WalkRecordDecl(IDeclPtr dst, clang::RecordDecl *src);
	void WalkCXXRecordDecl(IDeclPtr dst, clang::CXXRecordDecl *src);
	void WalkClassTemplateSpecializationDecl(IDeclPtr dst, clang::ClassTemplateSpecializationDecl *src);
	void WalkTypeDecl(IDeclPtr dst, clang::TypeDecl *src);
	void WalkTypedefNameDecl(IDeclPtr dst, clang::TypedefNameDecl *src);
	void WalkUsingShadowDecl(IDeclPtr dst, clang::UsingShadowDecl *src);
	void WalkValueDecl(IDeclPtr dst, clang::ValueDecl *src);
	void WalkDeclaratorDecl(IDeclPtr dst, clang::DeclaratorDecl *src);
	void WalkFieldDecl(IDeclPtr dst, clang::FieldDecl *src);
	void WalkFunctionDecl(IDeclPtr dst, clang::FunctionDecl *src);
	void WalkCXXMethodDecl(IDeclPtr dst, clang::CXXMethodDecl *src);
	void WalkVarDecl(IDeclPtr dst, clang::VarDecl *src);
	void WalkVarTemplateSpecializationDecl(IDeclPtr dst, clang::VarTemplateSpecializationDecl *src);
	void WalkStmt(IStmtPtr dst, clang::Stmt *src);
	IDeclPtr VisitAccessSpecDecl(clang::AccessSpecDecl *src);
	IDeclPtr VisitBlockDecl(clang::BlockDecl *src);
	IDeclPtr VisitCapturedDecl(clang::CapturedDecl *src);
	IDeclPtr VisitClassScopeFunctionSpecializationDecl(clang::ClassScopeFunctionSpecializationDecl *src);
	IDeclPtr VisitEmptyDecl(clang::EmptyDecl *src);
	IDeclPtr VisitExportDecl(clang::ExportDecl *src);
	IDeclPtr VisitExternCContextDecl(clang::ExternCContextDecl *src);
	IDeclPtr VisitFileScopeAsmDecl(clang::FileScopeAsmDecl *src);
	IDeclPtr VisitFriendDecl(clang::FriendDecl *src);
	IDeclPtr VisitFriendTemplateDecl(clang::FriendTemplateDecl *src);
	IDeclPtr VisitImportDecl(clang::ImportDecl *src);
	IDeclPtr VisitLinkageSpecDecl(clang::LinkageSpecDecl *src);
	//IDeclPtr VisitNamedDecl(clang::NamedDecl *src);
	IDeclPtr VisitLabelDecl(clang::LabelDecl *src);
	IDeclPtr VisitNamespaceDecl(clang::NamespaceDecl *src);
	IDeclPtr VisitNamespaceAliasDecl(clang::NamespaceAliasDecl *src);
	IDeclPtr VisitObjCCompatibleAliasDecl(clang::ObjCCompatibleAliasDecl *src);
	//IDeclPtr VisitObjCContainerDecl(clang::ObjCContainerDecl *src);
	IDeclPtr VisitObjCCategoryDecl(clang::ObjCCategoryDecl *src);
	//IDeclPtr VisitObjCImplDecl(clang::ObjCImplDecl *src);
	IDeclPtr VisitObjCCategoryImplDecl(clang::ObjCCategoryImplDecl *src);
	IDeclPtr VisitObjCImplementationDecl(clang::ObjCImplementationDecl *src);
	IDeclPtr VisitObjCInterfaceDecl(clang::ObjCInterfaceDecl *src);
	IDeclPtr VisitObjCProtocolDecl(clang::ObjCProtocolDecl *src);
	IDeclPtr VisitObjCMethodDecl(clang::ObjCMethodDecl *src);
	IDeclPtr VisitObjCPropertyDecl(clang::ObjCPropertyDecl *src);
	//IDeclPtr VisitTemplateDecl(clang::TemplateDecl *src);
	IDeclPtr VisitBuiltinTemplateDecl(clang::BuiltinTemplateDecl *src);
	//IDeclPtr VisitRedeclarableTemplateDecl(clang::RedeclarableTemplateDecl *src);
	IDeclPtr VisitClassTemplateDecl(clang::ClassTemplateDecl *src);
	IDeclPtr VisitFunctionTemplateDecl(clang::FunctionTemplateDecl *src);
	IDeclPtr VisitTypeAliasTemplateDecl(clang::TypeAliasTemplateDecl *src);
	IDeclPtr VisitVarTemplateDecl(clang::VarTemplateDecl *src);
	IDeclPtr VisitTemplateTemplateParmDecl(clang::TemplateTemplateParmDecl *src);
	//IDeclPtr VisitTypeDecl(clang::TypeDecl *src);
	//IDeclPtr VisitTagDecl(clang::TagDecl *src);
	IDeclPtr VisitEnumDecl(clang::EnumDecl *src);
	IDeclPtr VisitRecordDecl(clang::RecordDecl *src);
	IDeclPtr VisitClassTemplatePartialSpecializationDecl(clang::ClassTemplatePartialSpecializationDecl *src);
	IDeclPtr VisitTemplateTypeParmDecl(clang::TemplateTypeParmDecl *src);
	//IDeclPtr VisitTypedefNameDecl(clang::TypedefNameDecl *src);
	IDeclPtr VisitObjCTypeParamDecl(clang::ObjCTypeParamDecl *src);
	IDeclPtr VisitTypeAliasDecl(clang::TypeAliasDecl *src);
	IDeclPtr VisitTypedefDecl(clang::TypedefDecl *src);
	IDeclPtr VisitUnresolvedUsingTypenameDecl(clang::UnresolvedUsingTypenameDecl *src);
	IDeclPtr VisitUsingDecl(clang::UsingDecl *src);
	IDeclPtr VisitUsingDirectiveDecl(clang::UsingDirectiveDecl *src);
	IDeclPtr VisitUsingPackDecl(clang::UsingPackDecl *src);
	IDeclPtr VisitUsingShadowDecl(clang::UsingShadowDecl *src);
	IDeclPtr VisitConstructorUsingShadowDecl(clang::ConstructorUsingShadowDecl *src);
	//IDeclPtr VisitValueDecl(clang::ValueDecl *src);
	IDeclPtr VisitBindingDecl(clang::BindingDecl *src);
	//IDeclPtr VisitDeclaratorDecl(clang::DeclaratorDecl *src);
	IDeclPtr VisitFieldDecl(clang::FieldDecl *src);
	IDeclPtr VisitObjCAtDefsFieldDecl(clang::ObjCAtDefsFieldDecl *src);
	IDeclPtr VisitObjCIvarDecl(clang::ObjCIvarDecl *src);
	IDeclPtr VisitFunctionDecl(clang::FunctionDecl *src);
	IDeclPtr VisitCXXDeductionGuideDecl(clang::CXXDeductionGuideDecl *src);
	IDeclPtr VisitCXXMethodDecl(clang::CXXMethodDecl *src);
	IDeclPtr VisitCXXConstructorDecl(clang::CXXConstructorDecl *src);
	IDeclPtr VisitCXXConversionDecl(clang::CXXConversionDecl *src);
	IDeclPtr VisitCXXDestructorDecl(clang::CXXDestructorDecl *src);
	IDeclPtr VisitMSPropertyDecl(clang::MSPropertyDecl *src);
	IDeclPtr VisitNonTypeTemplateParmDecl(clang::NonTypeTemplateParmDecl *src);
	IDeclPtr VisitVarDecl(clang::VarDecl *src);
	IDeclPtr VisitDecompositionDecl(clang::DecompositionDecl *src);
	IDeclPtr VisitImplicitParamDecl(clang::ImplicitParamDecl *src);
	IDeclPtr VisitOMPCapturedExprDecl(clang::OMPCapturedExprDecl *src);
	IDeclPtr VisitParmVarDecl(clang::ParmVarDecl *src);
	IDeclPtr VisitVarTemplateSpecializationDecl(clang::VarTemplateSpecializationDecl *src);
	IDeclPtr VisitVarTemplatePartialSpecializationDecl(clang::VarTemplatePartialSpecializationDecl *src);
	IDeclPtr VisitEnumConstantDecl(clang::EnumConstantDecl *src);
	IDeclPtr VisitIndirectFieldDecl(clang::IndirectFieldDecl *src);
	IDeclPtr VisitOMPDeclareReductionDecl(clang::OMPDeclareReductionDecl *src);
	IDeclPtr VisitUnresolvedUsingValueDecl(clang::UnresolvedUsingValueDecl *src);
	IDeclPtr VisitOMPThreadPrivateDecl(clang::OMPThreadPrivateDecl *src);
	IDeclPtr VisitObjCPropertyImplDecl(clang::ObjCPropertyImplDecl *src);
	IDeclPtr VisitPragmaCommentDecl(clang::PragmaCommentDecl *src);
	IDeclPtr VisitPragmaDetectMismatchDecl(clang::PragmaDetectMismatchDecl *src);
	IDeclPtr VisitStaticAssertDecl(clang::StaticAssertDecl *src);
	IDeclPtr VisitTranslationUnitDecl(clang::TranslationUnitDecl *src);

	//IStmtPtr VisitAsmStmt(clang::AsmStmt *src);
	IStmtPtr VisitGCCAsmStmt(clang::GCCAsmStmt *src);
	IStmtPtr VisitMSAsmStmt(clang::MSAsmStmt *src);
	IStmtPtr VisitAttributedStmt(clang::AttributedStmt *src);
	IStmtPtr VisitBreakStmt(clang::BreakStmt *src);
	IStmtPtr VisitCXXCatchStmt(clang::CXXCatchStmt *src);
	IStmtPtr VisitCXXForRangeStmt(clang::CXXForRangeStmt *src);
	IStmtPtr VisitCXXTryStmt(clang::CXXTryStmt *src);
	IStmtPtr VisitCapturedStmt(clang::CapturedStmt *src);
	IStmtPtr VisitCompoundStmt(clang::CompoundStmt *src);
	IStmtPtr VisitContinueStmt(clang::ContinueStmt *src);
	IStmtPtr VisitCoreturnStmt(clang::CoreturnStmt *src);
	IStmtPtr VisitCoroutineBodyStmt(clang::CoroutineBodyStmt *src);
	IStmtPtr VisitDeclStmt(clang::DeclStmt *src);
	IStmtPtr VisitDoStmt(clang::DoStmt *src);
	//IStmtPtr VisitExpr(clang::Expr *src);
	IStmtPtr VisitAbstractConditionalOperator(clang::AbstractConditionalOperator *src);
	IStmtPtr VisitBinaryConditionalOperator(clang::BinaryConditionalOperator *src);
	IStmtPtr VisitConditionalOperator(clang::ConditionalOperator *src);
	IStmtPtr VisitAddrLabelExpr(clang::AddrLabelExpr *src);
	IStmtPtr VisitArrayInitIndexExpr(clang::ArrayInitIndexExpr *src);
	IStmtPtr VisitArrayInitLoopExpr(clang::ArrayInitLoopExpr *src);
	IStmtPtr VisitArraySubscriptExpr(clang::ArraySubscriptExpr *src);
	IStmtPtr VisitArrayTypeTraitExpr(clang::ArrayTypeTraitExpr *src);
	IStmtPtr VisitAsTypeExpr(clang::AsTypeExpr *src);
	IStmtPtr VisitAtomicExpr(clang::AtomicExpr *src);
	IStmtPtr VisitBinaryOperator(clang::BinaryOperator *src);
	IStmtPtr VisitCompoundAssignOperator(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBlockExpr(clang::BlockExpr *src);
	IStmtPtr VisitCXXBindTemporaryExpr(clang::CXXBindTemporaryExpr *src);
	IStmtPtr VisitCXXBoolLiteralExpr(clang::CXXBoolLiteralExpr *src);
	IStmtPtr VisitCXXConstructExpr(clang::CXXConstructExpr *src);
	IStmtPtr VisitCXXTemporaryObjectExpr(clang::CXXTemporaryObjectExpr *src);
	IStmtPtr VisitCXXDefaultArgExpr(clang::CXXDefaultArgExpr *src);
	IStmtPtr VisitCXXDefaultInitExpr(clang::CXXDefaultInitExpr *src);
	IStmtPtr VisitCXXDeleteExpr(clang::CXXDeleteExpr *src);
	IStmtPtr VisitCXXDependentScopeMemberExpr(clang::CXXDependentScopeMemberExpr *src);
	IStmtPtr VisitCXXFoldExpr(clang::CXXFoldExpr *src);
	IStmtPtr VisitCXXInheritedCtorInitExpr(clang::CXXInheritedCtorInitExpr *src);
	IStmtPtr VisitCXXNewExpr(clang::CXXNewExpr *src);
	IStmtPtr VisitCXXNoexceptExpr(clang::CXXNoexceptExpr *src);
	IStmtPtr VisitCXXNullPtrLiteralExpr(clang::CXXNullPtrLiteralExpr *src);
	IStmtPtr VisitCXXPseudoDestructorExpr(clang::CXXPseudoDestructorExpr *src);
	IStmtPtr VisitCXXScalarValueInitExpr(clang::CXXScalarValueInitExpr *src);
	IStmtPtr VisitCXXStdInitializerListExpr(clang::CXXStdInitializerListExpr *src);
	IStmtPtr VisitCXXThisExpr(clang::CXXThisExpr *src);
	IStmtPtr VisitCXXThrowExpr(clang::CXXThrowExpr *src);
	IStmtPtr VisitCXXTypeidExpr(clang::CXXTypeidExpr *src);
	IStmtPtr VisitCXXUnresolvedConstructExpr(clang::CXXUnresolvedConstructExpr *src);
	IStmtPtr VisitCXXUuidofExpr(clang::CXXUuidofExpr *src);
	IStmtPtr VisitCallExpr(clang::CallExpr *src);
	IStmtPtr VisitCUDAKernelCallExpr(clang::CUDAKernelCallExpr *src);
	IStmtPtr VisitCXXMemberCallExpr(clang::CXXMemberCallExpr *src);
	IStmtPtr VisitCXXOperatorCallExpr(clang::CXXOperatorCallExpr *src);
	IStmtPtr VisitUserDefinedLiteral(clang::UserDefinedLiteral *src);
	//IStmtPtr VisitCastExpr(clang::CastExpr *src);
	//IStmtPtr VisitExplicitCastExpr(clang::ExplicitCastExpr *src);
	IStmtPtr VisitCStyleCastExpr(clang::CStyleCastExpr *src);
	IStmtPtr VisitCXXFunctionalCastExpr(clang::CXXFunctionalCastExpr *src);
	//IStmtPtr VisitCXXNamedCastExpr(clang::CXXNamedCastExpr *src);
	IStmtPtr VisitCXXConstCastExpr(clang::CXXConstCastExpr *src);
	IStmtPtr VisitCXXDynamicCastExpr(clang::CXXDynamicCastExpr *src);
	IStmtPtr VisitCXXReinterpretCastExpr(clang::CXXReinterpretCastExpr *src);
	IStmtPtr VisitCXXStaticCastExpr(clang::CXXStaticCastExpr *src);
	IStmtPtr VisitObjCBridgedCastExpr(clang::ObjCBridgedCastExpr *src);
	IStmtPtr VisitImplicitCastExpr(clang::ImplicitCastExpr *src);
	IStmtPtr VisitCharacterLiteral(clang::CharacterLiteral *src);
	IStmtPtr VisitChooseExpr(clang::ChooseExpr *src);
	IStmtPtr VisitCompoundLiteralExpr(clang::CompoundLiteralExpr *src);
	IStmtPtr VisitConvertVectorExpr(clang::ConvertVectorExpr *src);
	//IStmtPtr VisitCoroutineSuspendExpr(clang::CoroutineSuspendExpr *src);
	IStmtPtr VisitCoawaitExpr(clang::CoawaitExpr *src);
	IStmtPtr VisitCoyieldExpr(clang::CoyieldExpr *src);
	IStmtPtr VisitDeclRefExpr(clang::DeclRefExpr *src);
	IStmtPtr VisitDependentCoawaitExpr(clang::DependentCoawaitExpr *src);
	IStmtPtr VisitDependentScopeDeclRefExpr(clang::DependentScopeDeclRefExpr *src);
	IStmtPtr VisitDesignatedInitExpr(clang::DesignatedInitExpr *src);
	IStmtPtr VisitDesignatedInitUpdateExpr(clang::DesignatedInitUpdateExpr *src);
	IStmtPtr VisitExprWithCleanups(clang::ExprWithCleanups *src);
	IStmtPtr VisitExpressionTraitExpr(clang::ExpressionTraitExpr *src);
	IStmtPtr VisitExtVectorElementExpr(clang::ExtVectorElementExpr *src);
	IStmtPtr VisitFloatingLiteral(clang::FloatingLiteral *src);
	IStmtPtr VisitFunctionParmPackExpr(clang::FunctionParmPackExpr *src);
	IStmtPtr VisitGNUNullExpr(clang::GNUNullExpr *src);
	IStmtPtr VisitGenericSelectionExpr(clang::GenericSelectionExpr *src);
	IStmtPtr VisitImaginaryLiteral(clang::ImaginaryLiteral *src);
	IStmtPtr VisitImplicitValueInitExpr(clang::ImplicitValueInitExpr *src);
	IStmtPtr VisitInitListExpr(clang::InitListExpr *src);
	IStmtPtr VisitIntegerLiteral(clang::IntegerLiteral *src);
	IStmtPtr VisitLambdaExpr(clang::LambdaExpr *src);
	IStmtPtr VisitMSPropertyRefExpr(clang::MSPropertyRefExpr *src);
	IStmtPtr VisitMSPropertySubscriptExpr(clang::MSPropertySubscriptExpr *src);
	IStmtPtr VisitMaterializeTemporaryExpr(clang::MaterializeTemporaryExpr *src);
	IStmtPtr VisitMemberExpr(clang::MemberExpr *src);
	IStmtPtr VisitNoInitExpr(clang::NoInitExpr *src);
	IStmtPtr VisitOMPArraySectionExpr(clang::OMPArraySectionExpr *src);
	IStmtPtr VisitObjCArrayLiteral(clang::ObjCArrayLiteral *src);
	IStmtPtr VisitObjCAvailabilityCheckExpr(clang::ObjCAvailabilityCheckExpr *src);
	IStmtPtr VisitObjCBoolLiteralExpr(clang::ObjCBoolLiteralExpr *src);
	IStmtPtr VisitObjCBoxedExpr(clang::ObjCBoxedExpr *src);
	IStmtPtr VisitObjCDictionaryLiteral(clang::ObjCDictionaryLiteral *src);
	IStmtPtr VisitObjCEncodeExpr(clang::ObjCEncodeExpr *src);
	IStmtPtr VisitObjCIndirectCopyRestoreExpr(clang::ObjCIndirectCopyRestoreExpr *src);
	IStmtPtr VisitObjCIsaExpr(clang::ObjCIsaExpr *src);
	IStmtPtr VisitObjCIvarRefExpr(clang::ObjCIvarRefExpr *src);
	IStmtPtr VisitObjCMessageExpr(clang::ObjCMessageExpr *src);
	IStmtPtr VisitObjCPropertyRefExpr(clang::ObjCPropertyRefExpr *src);
	IStmtPtr VisitObjCProtocolExpr(clang::ObjCProtocolExpr *src);
	IStmtPtr VisitObjCSelectorExpr(clang::ObjCSelectorExpr *src);
	IStmtPtr VisitObjCStringLiteral(clang::ObjCStringLiteral *src);
	IStmtPtr VisitObjCSubscriptRefExpr(clang::ObjCSubscriptRefExpr *src);
	IStmtPtr VisitOffsetOfExpr(clang::OffsetOfExpr *src);
	IStmtPtr VisitOpaqueValueExpr(clang::OpaqueValueExpr *src);
	//IStmtPtr VisitOverloadExpr(clang::OverloadExpr *src);
	IStmtPtr VisitUnresolvedLookupExpr(clang::UnresolvedLookupExpr *src);
	IStmtPtr VisitUnresolvedMemberExpr(clang::UnresolvedMemberExpr *src);
	IStmtPtr VisitPackExpansionExpr(clang::PackExpansionExpr *src);
	IStmtPtr VisitParenExpr(clang::ParenExpr *src);
	IStmtPtr VisitParenListExpr(clang::ParenListExpr *src);
	IStmtPtr VisitPredefinedExpr(clang::PredefinedExpr *src);
	IStmtPtr VisitPseudoObjectExpr(clang::PseudoObjectExpr *src);
	IStmtPtr VisitShuffleVectorExpr(clang::ShuffleVectorExpr *src);
	IStmtPtr VisitSizeOfPackExpr(clang::SizeOfPackExpr *src);
	IStmtPtr VisitStmtExpr(clang::StmtExpr *src);
	//IStmtPtr VisitStringLiteral(clang::StringLiteral *src);
	IStmtPtr VisitSubstNonTypeTemplateParmExpr(clang::SubstNonTypeTemplateParmExpr *src);
	IStmtPtr VisitSubstNonTypeTemplateParmPackExpr(clang::SubstNonTypeTemplateParmPackExpr *src);
	IStmtPtr VisitTypeTraitExpr(clang::TypeTraitExpr *src);
	IStmtPtr VisitTypoExpr(clang::TypoExpr *src);
	IStmtPtr VisitUnaryExprOrTypeTraitExpr(clang::UnaryExprOrTypeTraitExpr *src);
	IStmtPtr VisitUnaryOperator(clang::UnaryOperator *src);
	IStmtPtr VisitVAArgExpr(clang::VAArgExpr *src);
	IStmtPtr VisitForStmt(clang::ForStmt *src);
	IStmtPtr VisitGotoStmt(clang::GotoStmt *src);
	IStmtPtr VisitIfStmt(clang::IfStmt *src);
	IStmtPtr VisitIndirectGotoStmt(clang::IndirectGotoStmt *src);
	IStmtPtr VisitLabelStmt(clang::LabelStmt *src);
	IStmtPtr VisitMSDependentExistsStmt(clang::MSDependentExistsStmt *src);
	IStmtPtr VisitNullStmt(clang::NullStmt *src);
	//IStmtPtr VisitOMPExecutableDirective(clang::OMPExecutableDirective *src);
	IStmtPtr VisitOMPAtomicDirective(clang::OMPAtomicDirective *src);
	IStmtPtr VisitOMPBarrierDirective(clang::OMPBarrierDirective *src);
	IStmtPtr VisitOMPCancelDirective(clang::OMPCancelDirective *src);
	IStmtPtr VisitOMPCancellationPointDirective(clang::OMPCancellationPointDirective *src);
	IStmtPtr VisitOMPCriticalDirective(clang::OMPCriticalDirective *src);
	IStmtPtr VisitOMPFlushDirective(clang::OMPFlushDirective *src);
	//IStmtPtr VisitOMPLoopDirective(clang::OMPLoopDirective *src);
	IStmtPtr VisitOMPDistributeDirective(clang::OMPDistributeDirective *src);
	IStmtPtr VisitOMPDistributeParallelForDirective(clang::OMPDistributeParallelForDirective *src);
	IStmtPtr VisitOMPDistributeParallelForSimdDirective(clang::OMPDistributeParallelForSimdDirective *src);
	IStmtPtr VisitOMPDistributeSimdDirective(clang::OMPDistributeSimdDirective *src);
	IStmtPtr VisitOMPForDirective(clang::OMPForDirective *src);
	IStmtPtr VisitOMPForSimdDirective(clang::OMPForSimdDirective *src);
	IStmtPtr VisitOMPParallelForDirective(clang::OMPParallelForDirective *src);
	IStmtPtr VisitOMPParallelForSimdDirective(clang::OMPParallelForSimdDirective *src);
	IStmtPtr VisitOMPSimdDirective(clang::OMPSimdDirective *src);
	IStmtPtr VisitOMPTargetParallelForSimdDirective(clang::OMPTargetParallelForSimdDirective *src);
	IStmtPtr VisitOMPTargetSimdDirective(clang::OMPTargetSimdDirective *src);
	IStmtPtr VisitOMPTargetTeamsDistributeDirective(clang::OMPTargetTeamsDistributeDirective *src);
	IStmtPtr VisitOMPTargetTeamsDistributeParallelForDirective(clang::OMPTargetTeamsDistributeParallelForDirective *src);
	IStmtPtr VisitOMPTargetTeamsDistributeParallelForSimdDirective(clang::OMPTargetTeamsDistributeParallelForSimdDirective *src);
	IStmtPtr VisitOMPTargetTeamsDistributeSimdDirective(clang::OMPTargetTeamsDistributeSimdDirective *src);
	IStmtPtr VisitOMPTaskLoopDirective(clang::OMPTaskLoopDirective *src);
	IStmtPtr VisitOMPTaskLoopSimdDirective(clang::OMPTaskLoopSimdDirective *src);
	IStmtPtr VisitOMPTeamsDistributeDirective(clang::OMPTeamsDistributeDirective *src);
	IStmtPtr VisitOMPTeamsDistributeParallelForDirective(clang::OMPTeamsDistributeParallelForDirective *src);
	IStmtPtr VisitOMPTeamsDistributeParallelForSimdDirective(clang::OMPTeamsDistributeParallelForSimdDirective *src);
	IStmtPtr VisitOMPTeamsDistributeSimdDirective(clang::OMPTeamsDistributeSimdDirective *src);
	IStmtPtr VisitOMPMasterDirective(clang::OMPMasterDirective *src);
	IStmtPtr VisitOMPOrderedDirective(clang::OMPOrderedDirective *src);
	IStmtPtr VisitOMPParallelDirective(clang::OMPParallelDirective *src);
	IStmtPtr VisitOMPParallelSectionsDirective(clang::OMPParallelSectionsDirective *src);
	IStmtPtr VisitOMPSectionDirective(clang::OMPSectionDirective *src);
	IStmtPtr VisitOMPSectionsDirective(clang::OMPSectionsDirective *src);
	IStmtPtr VisitOMPSingleDirective(clang::OMPSingleDirective *src);
	IStmtPtr VisitOMPTargetDataDirective(clang::OMPTargetDataDirective *src);
	IStmtPtr VisitOMPTargetDirective(clang::OMPTargetDirective *src);
	IStmtPtr VisitOMPTargetEnterDataDirective(clang::OMPTargetEnterDataDirective *src);
	IStmtPtr VisitOMPTargetExitDataDirective(clang::OMPTargetExitDataDirective *src);
	IStmtPtr VisitOMPTargetParallelDirective(clang::OMPTargetParallelDirective *src);
	IStmtPtr VisitOMPTargetParallelForDirective(clang::OMPTargetParallelForDirective *src);
	IStmtPtr VisitOMPTargetTeamsDirective(clang::OMPTargetTeamsDirective *src);
	IStmtPtr VisitOMPTargetUpdateDirective(clang::OMPTargetUpdateDirective *src);
	IStmtPtr VisitOMPTaskDirective(clang::OMPTaskDirective *src);
	IStmtPtr VisitOMPTaskgroupDirective(clang::OMPTaskgroupDirective *src);
	IStmtPtr VisitOMPTaskwaitDirective(clang::OMPTaskwaitDirective *src);
	IStmtPtr VisitOMPTaskyieldDirective(clang::OMPTaskyieldDirective *src);
	IStmtPtr VisitOMPTeamsDirective(clang::OMPTeamsDirective *src);
	IStmtPtr VisitObjCAtCatchStmt(clang::ObjCAtCatchStmt *src);
	IStmtPtr VisitObjCAtFinallyStmt(clang::ObjCAtFinallyStmt *src);
	IStmtPtr VisitObjCAtSynchronizedStmt(clang::ObjCAtSynchronizedStmt *src);
	IStmtPtr VisitObjCAtThrowStmt(clang::ObjCAtThrowStmt *src);
	IStmtPtr VisitObjCAtTryStmt(clang::ObjCAtTryStmt *src);
	IStmtPtr VisitObjCAutoreleasePoolStmt(clang::ObjCAutoreleasePoolStmt *src);
	IStmtPtr VisitObjCForCollectionStmt(clang::ObjCForCollectionStmt *src);
	IStmtPtr VisitReturnStmt(clang::ReturnStmt *src);
	IStmtPtr VisitSEHExceptStmt(clang::SEHExceptStmt *src);
	IStmtPtr VisitSEHFinallyStmt(clang::SEHFinallyStmt *src);
	IStmtPtr VisitSEHLeaveStmt(clang::SEHLeaveStmt *src);
	IStmtPtr VisitSEHTryStmt(clang::SEHTryStmt *src);
	//IStmtPtr VisitSwitchCase(clang::SwitchCase *src);
	IStmtPtr VisitCaseStmt(clang::CaseStmt *src);
	IStmtPtr VisitDefaultStmt(clang::DefaultStmt *src);
	IStmtPtr VisitSwitchStmt(clang::SwitchStmt *src);
	IStmtPtr VisitWhileStmt(clang::WhileStmt *src);

	IStmtPtr VisitBinPtrMemD(clang::BinaryOperator *src);
	IStmtPtr VisitBinPtrMemI(clang::BinaryOperator *src);
	IStmtPtr VisitBinMul(clang::BinaryOperator *src);
	IStmtPtr VisitBinDiv(clang::BinaryOperator *src);
	IStmtPtr VisitBinRem(clang::BinaryOperator *src);
	IStmtPtr VisitBinAdd(clang::BinaryOperator *src);
	IStmtPtr VisitBinSub(clang::BinaryOperator *src);
	IStmtPtr VisitBinShl(clang::BinaryOperator *src);
	IStmtPtr VisitBinShr(clang::BinaryOperator *src);
	IStmtPtr VisitBinLT(clang::BinaryOperator *src);
	IStmtPtr VisitBinGT(clang::BinaryOperator *src);
	IStmtPtr VisitBinLE(clang::BinaryOperator *src);
	IStmtPtr VisitBinGE(clang::BinaryOperator *src);
	IStmtPtr VisitBinEQ(clang::BinaryOperator *src);
	IStmtPtr VisitBinNE(clang::BinaryOperator *src);
	IStmtPtr VisitBinAnd(clang::BinaryOperator *src);
	IStmtPtr VisitBinXor(clang::BinaryOperator *src);
	IStmtPtr VisitBinOr(clang::BinaryOperator *src);
	IStmtPtr VisitBinLAnd(clang::BinaryOperator *src);
	IStmtPtr VisitBinLOr(clang::BinaryOperator *src);
	IStmtPtr VisitBinAssign(clang::BinaryOperator *src);
	IStmtPtr VisitBinComma(clang::BinaryOperator *src);

	IStmtPtr VisitBinMulAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinDivAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinRemAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinAddAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinSubAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinShlAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinShrAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinAndAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinOrAssign(clang::CompoundAssignOperator *src);
	IStmtPtr VisitBinXorAssign(clang::CompoundAssignOperator *src);

	IStmtPtr VisitUnaryPostInc(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryPostDec(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryPreInc(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryPreDec(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryAddrOf(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryDeref(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryPlus(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryMinus(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryNot(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryLNot(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryReal(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryImag(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryExtension(clang::UnaryOperator *src);
	IStmtPtr VisitUnaryCoawait(clang::UnaryOperator *src);

	IStmtPtr VisitStmt(clang::Stmt *src);
};

#endif // _CLANGDOX_H_
