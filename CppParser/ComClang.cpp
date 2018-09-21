#include "pch.h"
#include "ComClang.h"

using namespace clang;
using namespace llvm;

DoxPPCallbacks::DoxPPCallbacks(Preprocessor &pp) : PP(pp)
{
}

void DoxPPCallbacks::FileChanged(SourceLocation Loc, FileChangeReason Reason,
	SrcMgr::CharacteristicKind FileType,
	FileID PrevFID) {
}

void DoxPPCallbacks::FileSkipped(const FileEntry &SkippedFile,
	const Token &FilenameTok,
	SrcMgr::CharacteristicKind FileType) {
}

bool DoxPPCallbacks::FileNotFound(StringRef FileName,
	SmallVectorImpl<char> &RecoveryPath) {
	return false;
}

void DoxPPCallbacks::InclusionDirective(SourceLocation HashLoc,
	const Token &IncludeTok,
	StringRef FileName,
	bool IsAngled,
	CharSourceRange FilenameRange,
	const FileEntry *File,
	StringRef SearchPath,
	StringRef RelativePath,
	const clang::Module *Imported) {
}

void DoxPPCallbacks::moduleImport(SourceLocation ImportLoc,
	ModuleIdPath Path,
	const clang::Module *Imported) {
}

void DoxPPCallbacks::EndOfMainFile() {
}

void DoxPPCallbacks::Ident(SourceLocation Loc, StringRef str) {
}

void DoxPPCallbacks::PragmaDirective(SourceLocation Loc,
	PragmaIntroducerKind Introducer) {
}

void DoxPPCallbacks::PragmaComment(SourceLocation Loc, const IdentifierInfo *Kind,
	StringRef Str) {
}

void DoxPPCallbacks::PragmaDetectMismatch(SourceLocation Loc, StringRef Name,
	StringRef Value) {
}

void DoxPPCallbacks::PragmaDebug(SourceLocation Loc, StringRef DebugType) {
}

void DoxPPCallbacks::PragmaMessage(SourceLocation Loc, StringRef Namespace,
	PragmaMessageKind Kind, StringRef Str) {
}

void DoxPPCallbacks::PragmaDiagnosticPush(SourceLocation Loc,
	StringRef Namespace) {
}

void DoxPPCallbacks::PragmaDiagnosticPop(SourceLocation Loc,
	StringRef Namespace) {
}

void DoxPPCallbacks::PragmaDiagnostic(SourceLocation Loc, StringRef Namespace,
	diag::Severity mapping, StringRef Str) {
}

void DoxPPCallbacks::PragmaOpenCLExtension(SourceLocation NameLoc,
	const IdentifierInfo *Name,
	SourceLocation StateLoc, unsigned State) {
}

void DoxPPCallbacks::PragmaWarning(SourceLocation Loc, StringRef WarningSpec,
	ArrayRef<int> Ids) {
}

void DoxPPCallbacks::PragmaWarningPush(SourceLocation Loc, int Level) {
}

void DoxPPCallbacks::PragmaWarningPop(SourceLocation Loc) {
}

void DoxPPCallbacks::MacroExpands(const Token &MacroNameTok,
	const clang::MacroDefinition &MD, SourceRange Range,
	const MacroArgs *Args) {
}

void DoxPPCallbacks::MacroDefined(const Token &MacroNameTok,
	const MacroDirective *MD) {
}

void DoxPPCallbacks::Defined(const Token &MacroNameTok, const clang::MacroDefinition &MD,
	SourceRange Range) {
}

void DoxPPCallbacks::If(SourceLocation Loc, SourceRange ConditionRange,
	ConditionValueKind ConditionValue) {
}

void DoxPPCallbacks::Elif(SourceLocation Loc, SourceRange ConditionRange,
	ConditionValueKind ConditionValue, SourceLocation IfLoc) {
}

void DoxPPCallbacks::Ifdef(SourceLocation Loc, const Token &MacroNameTok,
	const clang::MacroDefinition &MD) {
}

void DoxPPCallbacks::Ifndef(SourceLocation Loc, const Token &MacroNameTok,
	const clang::MacroDefinition &MD) {
}

void DoxPPCallbacks::Else(SourceLocation Loc, SourceLocation IfLoc) {
}

void DoxPPCallbacks::Endif(SourceLocation Loc, SourceLocation IfLoc) {
}


ASTWalker::ASTWalker(IComClangPtr pComClang, CompilerInstance *CI) :
	pComClang(pComClang),
	Policy(PrintingPolicy(CI->getASTContext().getPrintingPolicy())),
	SM(CI->getASTContext().getSourceManager()),
	Parent(nullptr),
	ParentStmt(nullptr) {
	Policy.Bool = 1;
}

void ASTWalker::WalkTranslationUnitDecl(TranslationUnitDecl *tu)
{
	IDeclPtr parent = pComClang->CreateDecl(DeclKind_TranslationUnit, nullptr);
	WalkDeclContext(parent, tu);
}

void ASTWalker::WalkDeclContext(IDeclPtr parent, DeclContext *context) {
	auto temp = Parent;
	Parent = parent;
	for (auto D : context->decls()) {
		DeclVisitor<ASTWalker, IDecl*>::Visit(D);
	}
	Parent = temp;
}

void ASTWalker::WalkDecl(IDeclPtr dst, Decl *src)
{
	auto range = src->getSourceRange();
	auto begin = range.getBegin();
	ISourceLocationPtr bsl = pComClang->CreateSourceLocation(SM.getFilename(begin).str().c_str(), SM.getSpellingLineNumber(begin), SM.getSpellingColumnNumber(begin));
	auto end = range.getEnd();
	ISourceLocationPtr esl = pComClang->CreateSourceLocation(SM.getFilename(end).str().c_str(), SM.getSpellingLineNumber(end), SM.getSpellingColumnNumber(end));
	dst->SetRange(bsl, esl);
}

void ASTWalker::WalkNamedDecl(IDeclPtr dst, NamedDecl *src)
{
	WalkDecl(dst, src);
	auto name = src->getNameAsString();
	dst->SetName(name.c_str());
}

void ASTWalker::WalkObjCContainerDecl(IDeclPtr dst, ObjCContainerDecl *src)
{
	WalkNamedDecl(dst, src);
}

void ASTWalker::WalkObjCImplDecl(IDeclPtr dst, ObjCImplDecl *src)
{
	WalkObjCContainerDecl(dst, src);
}

void ASTWalker::WalkTemplateDecl(IDeclPtr dst, TemplateDecl *src)
{
	WalkNamedDecl(dst, src);
}

void ASTWalker::WalkRedeclarableTemplateDecl(IDeclPtr dst, RedeclarableTemplateDecl *src)
{
	WalkTemplateDecl(dst, src);
}

void ASTWalker::WalkTagDecl(IDeclPtr dst, TagDecl *src)
{
	WalkTypeDecl(dst, src);
}

void ASTWalker::WalkRecordDecl(IDeclPtr dst, RecordDecl *src)
{
	WalkTagDecl(dst, src);
}

void ASTWalker::WalkCXXRecordDecl(IDeclPtr dst, CXXRecordDecl *src)
{
	WalkRecordDecl(dst, src);
}

void ASTWalker::WalkClassTemplateSpecializationDecl(IDeclPtr dst, ClassTemplateSpecializationDecl *src)
{
	WalkCXXRecordDecl(dst, src);
}

void ASTWalker::WalkTypeDecl(IDeclPtr dst, TypeDecl *src)
{
	WalkNamedDecl(dst, src);
}

void ASTWalker::WalkTypedefNameDecl(IDeclPtr dst, clang::TypedefNameDecl *src)
{
	WalkTypeDecl(dst, src);
}

void ASTWalker::WalkUsingShadowDecl(IDeclPtr dst, UsingShadowDecl *src)
{
	WalkNamedDecl(dst, src);
}

void ASTWalker::WalkValueDecl(IDeclPtr dst, ValueDecl *src)
{
	WalkNamedDecl(dst, src);
}

void ASTWalker::WalkDeclaratorDecl(IDeclPtr dst, DeclaratorDecl *src)
{
	WalkValueDecl(dst, src);
}

void ASTWalker::WalkFieldDecl(IDeclPtr dst, FieldDecl *src)
{
	WalkDeclaratorDecl(dst, src);
}

void ASTWalker::WalkFunctionDecl(IDeclPtr dst, FunctionDecl *src)
{
	WalkDeclaratorDecl(dst, src);
	auto body = src->getBody();
	if (body != nullptr) {
		auto stmt = StmtVisitor<ASTWalker, IStmt*>::Visit(body);
		dst->SetStmt("Body", stmt);
	}
}

void ASTWalker::WalkCXXMethodDecl(IDeclPtr dst, CXXMethodDecl *src)
{
	WalkFunctionDecl(dst, src);
}

void ASTWalker::WalkVarDecl(IDeclPtr dst, VarDecl *src)
{
	WalkDeclaratorDecl(dst, src);
}

void ASTWalker::WalkVarTemplateSpecializationDecl(IDeclPtr dst, VarTemplateSpecializationDecl *src)
{
	WalkVarDecl(dst, src);
}

void ASTWalker::WalkStmt(IStmtPtr dst, Stmt *src)
{
	auto range = src->getSourceRange();
	auto begin = range.getBegin();
	ISourceLocationPtr bsl = pComClang->CreateSourceLocation(SM.getFilename(begin).str().c_str(), SM.getSpellingLineNumber(begin), SM.getSpellingColumnNumber(begin));
	auto end = range.getEnd();
	ISourceLocationPtr esl = pComClang->CreateSourceLocation(SM.getFilename(end).str().c_str(), SM.getSpellingLineNumber(end), SM.getSpellingColumnNumber(end));
	dst->SetRange(bsl, esl);

	auto temp = ParentStmt;
	ParentStmt = dst;
	for (auto SubStmt : src->children()) {
		if (SubStmt)
			StmtVisitor<ASTWalker, IStmt*>::Visit(SubStmt);
	}
	ParentStmt = temp;
}

IDeclPtr ASTWalker::VisitAccessSpecDecl(clang::AccessSpecDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_AccessSpec, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitBlockDecl(clang::BlockDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Block, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	auto body = src->getCompoundBody();
	if (body != nullptr) {
		auto stmt = StmtVisitor<ASTWalker, IStmt*>::Visit(body);
		decl->SetStmt("Body", stmt);
	}
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitCapturedDecl(clang::CapturedDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Captured, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitClassScopeFunctionSpecializationDecl(clang::ClassScopeFunctionSpecializationDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ClassScopeFunctionSpecialization, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitEmptyDecl(clang::EmptyDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Empty, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitExportDecl(clang::ExportDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Export, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitExternCContextDecl(clang::ExternCContextDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ExternCContext, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitFileScopeAsmDecl(clang::FileScopeAsmDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_FileScopeAsm, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitFriendDecl(clang::FriendDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Friend, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitFriendTemplateDecl(clang::FriendTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_FriendTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitImportDecl(clang::ImportDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Import, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitLinkageSpecDecl(clang::LinkageSpecDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_LinkageSpec, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitNamedDecl(clang::NamedDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Named, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitLabelDecl(clang::LabelDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Label, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	auto sub = src->getStmt();
	if (sub != nullptr) {
		auto stmt = StmtVisitor<ASTWalker, IStmt*>::Visit(sub);
		decl->SetStmt("Stmt", stmt);
	}
	return decl;
}

IDeclPtr ASTWalker::VisitNamespaceDecl(clang::NamespaceDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Namespace, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitNamespaceAliasDecl(clang::NamespaceAliasDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_NamespaceAlias, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCCompatibleAliasDecl(clang::ObjCCompatibleAliasDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCCompatibleAlias, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitObjCContainerDecl(clang::ObjCContainerDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCContainer, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitObjCCategoryDecl(clang::ObjCCategoryDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCCategory, Parent);
	decl->Native = (uintptr_t)src;
	WalkObjCContainerDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitObjCImplDecl(clang::ObjCImplDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCImpl, Parent);
	decl->Native = (uintptr_t)src;
	WalkObjCContainerDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitObjCCategoryImplDecl(clang::ObjCCategoryImplDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCCategoryImpl, Parent);
	decl->Native = (uintptr_t)src;
	WalkObjCImplDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCImplementationDecl(clang::ObjCImplementationDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCImplementation, Parent);
	decl->Native = (uintptr_t)src;
	WalkObjCImplDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCInterfaceDecl(clang::ObjCInterfaceDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCInterface, Parent);
	decl->Native = (uintptr_t)src;
	WalkObjCContainerDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCProtocolDecl(clang::ObjCProtocolDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCProtocol, Parent);
	decl->Native = (uintptr_t)src;
	WalkObjCContainerDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCMethodDecl(clang::ObjCMethodDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCMethod, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	auto body = src->getCompoundBody();
	if (body != nullptr) {
		auto stmt = StmtVisitor<ASTWalker, IStmt*>::Visit(body);
		decl->SetStmt("CompoundBody", stmt);
	}
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCPropertyDecl(clang::ObjCPropertyDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCProperty, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitTemplateDecl(clang::TemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Template, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitBuiltinTemplateDecl(clang::BuiltinTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_BuiltinTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkTemplateDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitRedeclarableTemplateDecl(clang::RedeclarableTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_RedeclarableTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkTemplateDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitClassTemplateDecl(clang::ClassTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ClassTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkRedeclarableTemplateDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitFunctionTemplateDecl(clang::FunctionTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_FunctionTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkRedeclarableTemplateDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitTypeAliasTemplateDecl(clang::TypeAliasTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_TypeAliasTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkRedeclarableTemplateDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitVarTemplateDecl(clang::VarTemplateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_VarTemplate, Parent);
	decl->Native = (uintptr_t)src;
	WalkRedeclarableTemplateDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitTemplateTemplateParmDecl(clang::TemplateTemplateParmDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_TemplateTemplateParm, Parent);
	decl->Native = (uintptr_t)src;
	WalkTemplateDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitTypeDecl(clang::TypeDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Type, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

/*IDeclPtr ASTWalker::VisitTagDecl(clang::TagDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Tag, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypeDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitEnumDecl(clang::EnumDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Enum, Parent);
	decl->Native = (uintptr_t)src;
	WalkTagDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitRecordDecl(clang::RecordDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Record, Parent);
	decl->Native = (uintptr_t)src;
	WalkTagDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitClassTemplatePartialSpecializationDecl(clang::ClassTemplatePartialSpecializationDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ClassTemplatePartialSpecialization, Parent);
	decl->Native = (uintptr_t)src;
	WalkClassTemplateSpecializationDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitTemplateTypeParmDecl(clang::TemplateTypeParmDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_TemplateTypeParm, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypeDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitTypedefNameDecl(clang::TypedefNameDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_TypedefName, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypeDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitObjCTypeParamDecl(clang::ObjCTypeParamDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCTypeParam, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypedefNameDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitTypeAliasDecl(clang::TypeAliasDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_TypeAlias, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypedefNameDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitTypedefDecl(clang::TypedefDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Typedef, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypedefNameDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitUnresolvedUsingTypenameDecl(clang::UnresolvedUsingTypenameDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_UnresolvedUsingTypename, Parent);
	decl->Native = (uintptr_t)src;
	WalkTypeDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitUsingDecl(clang::UsingDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Using, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitUsingDirectiveDecl(clang::UsingDirectiveDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_UsingDirective, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitUsingPackDecl(clang::UsingPackDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_UsingPack, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitUsingShadowDecl(clang::UsingShadowDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_UsingShadow, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitConstructorUsingShadowDecl(clang::ConstructorUsingShadowDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ConstructorUsingShadow, Parent);
	decl->Native = (uintptr_t)src;
	WalkUsingShadowDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitValueDecl(clang::ValueDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Value, Parent);
	decl->Native = (uintptr_t)src;
	WalkNamedDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitBindingDecl(clang::BindingDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Binding, Parent);
	decl->Native = (uintptr_t)src;
	WalkValueDecl(decl, src);
	return decl;
}

/*IDeclPtr ASTWalker::VisitDeclaratorDecl(clang::DeclaratorDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Declarator, Parent);
	decl->Native = (uintptr_t)src;
	WalkValueDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}*/

IDeclPtr ASTWalker::VisitFieldDecl(clang::FieldDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Field, Parent);
	decl->Native = (uintptr_t)src;
	WalkDeclaratorDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCAtDefsFieldDecl(clang::ObjCAtDefsFieldDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCAtDefsField, Parent);
	decl->Native = (uintptr_t)src;
	WalkFieldDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCIvarDecl(clang::ObjCIvarDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCIvar, Parent);
	decl->Native = (uintptr_t)src;
	WalkFieldDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitFunctionDecl(clang::FunctionDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Function, Parent);
	decl->Native = (uintptr_t)src;
	WalkFunctionDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitCXXDeductionGuideDecl(clang::CXXDeductionGuideDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_CXXDeductionGuide, Parent);
	decl->Native = (uintptr_t)src;
	WalkFunctionDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitCXXMethodDecl(clang::CXXMethodDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_CXXMethod, Parent);
	decl->Native = (uintptr_t)src;
	WalkFunctionDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitCXXConstructorDecl(clang::CXXConstructorDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_CXXConstructor, Parent);
	decl->Native = (uintptr_t)src;
	WalkCXXMethodDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitCXXConversionDecl(clang::CXXConversionDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_CXXConversion, Parent);
	decl->Native = (uintptr_t)src;
	WalkCXXMethodDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitCXXDestructorDecl(clang::CXXDestructorDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_CXXDestructor, Parent);
	decl->Native = (uintptr_t)src;
	WalkCXXMethodDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitMSPropertyDecl(clang::MSPropertyDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_MSProperty, Parent);
	decl->Native = (uintptr_t)src;
	WalkDeclaratorDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitNonTypeTemplateParmDecl(clang::NonTypeTemplateParmDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_NonTypeTemplateParm, Parent);
	decl->Native = (uintptr_t)src;
	WalkDeclaratorDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitVarDecl(clang::VarDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Var, Parent);
	decl->Native = (uintptr_t)src;
	WalkDeclaratorDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitDecompositionDecl(clang::DecompositionDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_Decomposition, Parent);
	decl->Native = (uintptr_t)src;
	WalkVarDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitImplicitParamDecl(clang::ImplicitParamDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ImplicitParam, Parent);
	decl->Native = (uintptr_t)src;
	WalkVarDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitOMPCapturedExprDecl(clang::OMPCapturedExprDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_OMPCapturedExpr, Parent);
	decl->Native = (uintptr_t)src;
	WalkVarDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitParmVarDecl(clang::ParmVarDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ParmVar, Parent);
	decl->Native = (uintptr_t)src;
	WalkVarDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitVarTemplateSpecializationDecl(clang::VarTemplateSpecializationDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_VarTemplateSpecialization, Parent);
	decl->Native = (uintptr_t)src;
	WalkVarDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitVarTemplatePartialSpecializationDecl(clang::VarTemplatePartialSpecializationDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_VarTemplatePartialSpecialization, Parent);
	decl->Native = (uintptr_t)src;
	WalkVarTemplateSpecializationDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitEnumConstantDecl(clang::EnumConstantDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_EnumConstant, Parent);
	decl->Native = (uintptr_t)src;
	WalkValueDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitIndirectFieldDecl(clang::IndirectFieldDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_IndirectField, Parent);
	decl->Native = (uintptr_t)src;
	WalkValueDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitOMPDeclareReductionDecl(clang::OMPDeclareReductionDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_OMPDeclareReduction, Parent);
	decl->Native = (uintptr_t)src;
	WalkValueDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitUnresolvedUsingValueDecl(clang::UnresolvedUsingValueDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_UnresolvedUsingValue, Parent);
	decl->Native = (uintptr_t)src;
	WalkValueDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitOMPThreadPrivateDecl(clang::OMPThreadPrivateDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_OMPThreadPrivate, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitObjCPropertyImplDecl(clang::ObjCPropertyImplDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_ObjCPropertyImpl, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitPragmaCommentDecl(clang::PragmaCommentDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_PragmaComment, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitPragmaDetectMismatchDecl(clang::PragmaDetectMismatchDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_PragmaDetectMismatch, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitStaticAssertDecl(clang::StaticAssertDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_StaticAssert, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	return decl;
}

IDeclPtr ASTWalker::VisitTranslationUnitDecl(clang::TranslationUnitDecl *src) {
	IDeclPtr decl = pComClang->CreateDecl(DeclKind_TranslationUnit, Parent);
	decl->Native = (uintptr_t)src;
	WalkDecl(decl, src);
	WalkDeclContext(decl, src);
	return decl;
}

/*IStmtPtr ASTWalker::VisitAsmStmt(clang::AsmStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_AsmStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitGCCAsmStmt(clang::GCCAsmStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_GCCAsmStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitMSAsmStmt(clang::MSAsmStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_MSAsmStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitAttributedStmt(clang::AttributedStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_AttributedStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBreakStmt(clang::BreakStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BreakStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXCatchStmt(clang::CXXCatchStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXCatchStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXForRangeStmt(clang::CXXForRangeStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXForRangeStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXTryStmt(clang::CXXTryStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXTryStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCapturedStmt(clang::CapturedStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CapturedStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCompoundStmt(clang::CompoundStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitContinueStmt(clang::ContinueStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ContinueStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCoreturnStmt(clang::CoreturnStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CoreturnStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCoroutineBodyStmt(clang::CoroutineBodyStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CoroutineBodyStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDeclStmt(clang::DeclStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DeclStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDoStmt(clang::DoStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DoStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitExpr(clang::Expr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_Expr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitAbstractConditionalOperator(clang::AbstractConditionalOperator *src) {
	/*IStmtPtr stmt = pComClang->CreateStmt(StmtClass_AbstractConditionalOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;*/
	return nullptr;
}

IStmtPtr ASTWalker::VisitBinaryConditionalOperator(clang::BinaryConditionalOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryConditionalOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitConditionalOperator(clang::ConditionalOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ConditionalOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitAddrLabelExpr(clang::AddrLabelExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_AddrLabelExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitArrayInitIndexExpr(clang::ArrayInitIndexExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ArrayInitIndexExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitArrayInitLoopExpr(clang::ArrayInitLoopExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ArrayInitLoopExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitArraySubscriptExpr(clang::ArraySubscriptExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ArraySubscriptExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitArrayTypeTraitExpr(clang::ArrayTypeTraitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ArrayTypeTraitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitAsTypeExpr(clang::AsTypeExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_AsTypeExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitAtomicExpr(clang::AtomicExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_AtomicExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinaryOperator(clang::BinaryOperator * src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCompoundAssignOperator(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBlockExpr(clang::BlockExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BlockExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXBindTemporaryExpr(clang::CXXBindTemporaryExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXBindTemporaryExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXBoolLiteralExpr(clang::CXXBoolLiteralExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXBoolLiteralExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXConstructExpr(clang::CXXConstructExpr * src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXConstructExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXTemporaryObjectExpr(clang::CXXTemporaryObjectExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXTemporaryObjectExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXDefaultArgExpr(clang::CXXDefaultArgExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXDefaultArgExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXDefaultInitExpr(clang::CXXDefaultInitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXDefaultInitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXDeleteExpr(clang::CXXDeleteExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXDeleteExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXDependentScopeMemberExpr(clang::CXXDependentScopeMemberExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXDependentScopeMemberExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXFoldExpr(clang::CXXFoldExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXFoldExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXInheritedCtorInitExpr(clang::CXXInheritedCtorInitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXInheritedCtorInitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXNewExpr(clang::CXXNewExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXNewExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXNoexceptExpr(clang::CXXNoexceptExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXNoexceptExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXNullPtrLiteralExpr(clang::CXXNullPtrLiteralExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXNullPtrLiteralExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXPseudoDestructorExpr(clang::CXXPseudoDestructorExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXPseudoDestructorExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXScalarValueInitExpr(clang::CXXScalarValueInitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXScalarValueInitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXStdInitializerListExpr(clang::CXXStdInitializerListExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXStdInitializerListExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXThisExpr(clang::CXXThisExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXThisExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXThrowExpr(clang::CXXThrowExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXThrowExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXTypeidExpr(clang::CXXTypeidExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXTypeidExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXUnresolvedConstructExpr(clang::CXXUnresolvedConstructExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXUnresolvedConstructExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXUuidofExpr(clang::CXXUuidofExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXUuidofExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCallExpr(clang::CallExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CallExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCUDAKernelCallExpr(clang::CUDAKernelCallExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CUDAKernelCallExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXMemberCallExpr(clang::CXXMemberCallExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXMemberCallExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXOperatorCallExpr(clang::CXXOperatorCallExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXOperatorCallExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUserDefinedLiteral(clang::UserDefinedLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UserDefinedLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitCastExpr(clang::CastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

/*IStmtPtr ASTWalker::VisitExplicitCastExpr(clang::ExplicitCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ExplicitCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitCStyleCastExpr(clang::CStyleCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CStyleCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXFunctionalCastExpr(clang::CXXFunctionalCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXFunctionalCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitCXXNamedCastExpr(clang::CXXNamedCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXNamedCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitCXXConstCastExpr(clang::CXXConstCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXConstCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXDynamicCastExpr(clang::CXXDynamicCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXDynamicCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXReinterpretCastExpr(clang::CXXReinterpretCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXReinterpretCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCXXStaticCastExpr(clang::CXXStaticCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CXXStaticCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCBridgedCastExpr(clang::ObjCBridgedCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCBridgedCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitImplicitCastExpr(clang::ImplicitCastExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ImplicitCastExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCharacterLiteral(clang::CharacterLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CharacterLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitChooseExpr(clang::ChooseExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ChooseExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCompoundLiteralExpr(clang::CompoundLiteralExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundLiteralExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitConvertVectorExpr(clang::ConvertVectorExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ConvertVectorExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitCoroutineSuspendExpr(clang::CoroutineSuspendExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CoroutineSuspendExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitCoawaitExpr(clang::CoawaitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CoawaitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitCoyieldExpr(clang::CoyieldExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CoyieldExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDeclRefExpr(clang::DeclRefExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DeclRefExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDependentCoawaitExpr(clang::DependentCoawaitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DependentCoawaitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDependentScopeDeclRefExpr(clang::DependentScopeDeclRefExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DependentScopeDeclRefExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDesignatedInitExpr(clang::DesignatedInitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DesignatedInitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDesignatedInitUpdateExpr(clang::DesignatedInitUpdateExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DesignatedInitUpdateExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitExprWithCleanups(clang::ExprWithCleanups *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ExprWithCleanups, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitExpressionTraitExpr(clang::ExpressionTraitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ExpressionTraitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitExtVectorElementExpr(clang::ExtVectorElementExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ExtVectorElementExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitFloatingLiteral(clang::FloatingLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_FloatingLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitFunctionParmPackExpr(clang::FunctionParmPackExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_FunctionParmPackExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitGNUNullExpr(clang::GNUNullExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_GNUNullExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitGenericSelectionExpr(clang::GenericSelectionExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_GenericSelectionExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitImaginaryLiteral(clang::ImaginaryLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ImaginaryLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitImplicitValueInitExpr(clang::ImplicitValueInitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ImplicitValueInitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitInitListExpr(clang::InitListExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_InitListExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitIntegerLiteral(clang::IntegerLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_IntegerLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitLambdaExpr(clang::LambdaExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_LambdaExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitMSPropertyRefExpr(clang::MSPropertyRefExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_MSPropertyRefExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitMSPropertySubscriptExpr(clang::MSPropertySubscriptExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_MSPropertySubscriptExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitMaterializeTemporaryExpr(clang::MaterializeTemporaryExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_MaterializeTemporaryExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitMemberExpr(clang::MemberExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_MemberExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitNoInitExpr(clang::NoInitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_NoInitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPArraySectionExpr(clang::OMPArraySectionExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPArraySectionExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCArrayLiteral(clang::ObjCArrayLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCArrayLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAvailabilityCheckExpr(clang::ObjCAvailabilityCheckExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAvailabilityCheckExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCBoolLiteralExpr(clang::ObjCBoolLiteralExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCBoolLiteralExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCBoxedExpr(clang::ObjCBoxedExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCBoxedExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCDictionaryLiteral(clang::ObjCDictionaryLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCDictionaryLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCEncodeExpr(clang::ObjCEncodeExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCEncodeExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCIndirectCopyRestoreExpr(clang::ObjCIndirectCopyRestoreExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCIndirectCopyRestoreExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCIsaExpr(clang::ObjCIsaExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCIsaExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCIvarRefExpr(clang::ObjCIvarRefExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCIvarRefExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCMessageExpr(clang::ObjCMessageExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCMessageExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCPropertyRefExpr(clang::ObjCPropertyRefExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCPropertyRefExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCProtocolExpr(clang::ObjCProtocolExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCProtocolExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCSelectorExpr(clang::ObjCSelectorExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCSelectorExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCStringLiteral(clang::ObjCStringLiteral *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCStringLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCSubscriptRefExpr(clang::ObjCSubscriptRefExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCSubscriptRefExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOffsetOfExpr(clang::OffsetOfExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OffsetOfExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOpaqueValueExpr(clang::OpaqueValueExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OpaqueValueExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitOverloadExpr(clang::OverloadExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OverloadExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitUnresolvedLookupExpr(clang::UnresolvedLookupExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnresolvedLookupExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnresolvedMemberExpr(clang::UnresolvedMemberExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnresolvedMemberExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitPackExpansionExpr(clang::PackExpansionExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_PackExpansionExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitParenExpr(clang::ParenExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ParenExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitParenListExpr(clang::ParenListExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ParenListExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitPredefinedExpr(clang::PredefinedExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_PredefinedExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitPseudoObjectExpr(clang::PseudoObjectExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_PseudoObjectExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitShuffleVectorExpr(clang::ShuffleVectorExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ShuffleVectorExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSizeOfPackExpr(clang::SizeOfPackExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SizeOfPackExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitStmtExpr(clang::StmtExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_StmtExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitStringLiteral(clang::StringLiteral * src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_StringLiteral, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitSubstNonTypeTemplateParmExpr(clang::SubstNonTypeTemplateParmExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SubstNonTypeTemplateParmExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSubstNonTypeTemplateParmPackExpr(clang::SubstNonTypeTemplateParmPackExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SubstNonTypeTemplateParmPackExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitTypeTraitExpr(clang::TypeTraitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_TypeTraitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitTypoExpr(clang::TypoExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_TypoExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryExprOrTypeTraitExpr(clang::UnaryExprOrTypeTraitExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryExprOrTypeTraitExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryOperator(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitVAArgExpr(clang::VAArgExpr *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_VAArgExpr, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitForStmt(clang::ForStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ForStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitGotoStmt(clang::GotoStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_GotoStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitIfStmt(clang::IfStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_IfStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitIndirectGotoStmt(clang::IndirectGotoStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_IndirectGotoStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitLabelStmt(clang::LabelStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_LabelStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitMSDependentExistsStmt(clang::MSDependentExistsStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_MSDependentExistsStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitNullStmt(clang::NullStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_NullStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitOMPExecutableDirective(clang::OMPExecutableDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPExecutableDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitOMPAtomicDirective(clang::OMPAtomicDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPAtomicDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPBarrierDirective(clang::OMPBarrierDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPBarrierDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPCancelDirective(clang::OMPCancelDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPCancelDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPCancellationPointDirective(clang::OMPCancellationPointDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPCancellationPointDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPCriticalDirective(clang::OMPCriticalDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPCriticalDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPFlushDirective(clang::OMPFlushDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPFlushDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/**IStmtPtr ASTWalker::VisitOMPLoopDirective(clang::OMPLoopDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPLoopDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitOMPDistributeDirective(clang::OMPDistributeDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPDistributeDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPDistributeParallelForDirective(clang::OMPDistributeParallelForDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPDistributeParallelForDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPDistributeParallelForSimdDirective(clang::OMPDistributeParallelForSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPDistributeParallelForSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPDistributeSimdDirective(clang::OMPDistributeSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPDistributeSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPForDirective(clang::OMPForDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPForDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPForSimdDirective(clang::OMPForSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPForSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPParallelForDirective(clang::OMPParallelForDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPParallelForDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPParallelForSimdDirective(clang::OMPParallelForSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPParallelForSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPSimdDirective(clang::OMPSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetParallelForSimdDirective(clang::OMPTargetParallelForSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetParallelForSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetSimdDirective(clang::OMPTargetSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetTeamsDistributeDirective(clang::OMPTargetTeamsDistributeDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetTeamsDistributeDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetTeamsDistributeParallelForDirective(clang::OMPTargetTeamsDistributeParallelForDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetTeamsDistributeParallelForDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetTeamsDistributeParallelForSimdDirective(clang::OMPTargetTeamsDistributeParallelForSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetTeamsDistributeParallelForSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetTeamsDistributeSimdDirective(clang::OMPTargetTeamsDistributeSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetTeamsDistributeSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTaskLoopDirective(clang::OMPTaskLoopDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTaskLoopDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTaskLoopSimdDirective(clang::OMPTaskLoopSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTaskLoopSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTeamsDistributeDirective(clang::OMPTeamsDistributeDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTeamsDistributeDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTeamsDistributeParallelForDirective(clang::OMPTeamsDistributeParallelForDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTeamsDistributeParallelForDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTeamsDistributeParallelForSimdDirective(clang::OMPTeamsDistributeParallelForSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTeamsDistributeParallelForSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTeamsDistributeSimdDirective(clang::OMPTeamsDistributeSimdDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTeamsDistributeSimdDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPMasterDirective(clang::OMPMasterDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPMasterDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPOrderedDirective(clang::OMPOrderedDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPOrderedDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPParallelDirective(clang::OMPParallelDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPParallelDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPParallelSectionsDirective(clang::OMPParallelSectionsDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPParallelSectionsDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPSectionDirective(clang::OMPSectionDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPSectionDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPSectionsDirective(clang::OMPSectionsDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPSectionsDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPSingleDirective(clang::OMPSingleDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPSingleDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetDataDirective(clang::OMPTargetDataDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetDataDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetDirective(clang::OMPTargetDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetEnterDataDirective(clang::OMPTargetEnterDataDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetEnterDataDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetExitDataDirective(clang::OMPTargetExitDataDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetExitDataDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetParallelDirective(clang::OMPTargetParallelDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetParallelDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetParallelForDirective(clang::OMPTargetParallelForDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetParallelForDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetTeamsDirective(clang::OMPTargetTeamsDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetTeamsDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTargetUpdateDirective(clang::OMPTargetUpdateDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTargetUpdateDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTaskDirective(clang::OMPTaskDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTaskDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTaskgroupDirective(clang::OMPTaskgroupDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTaskgroupDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTaskwaitDirective(clang::OMPTaskwaitDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTaskwaitDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTaskyieldDirective(clang::OMPTaskyieldDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTaskyieldDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitOMPTeamsDirective(clang::OMPTeamsDirective *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_OMPTeamsDirective, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAtCatchStmt(clang::ObjCAtCatchStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAtCatchStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAtFinallyStmt(clang::ObjCAtFinallyStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAtFinallyStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAtSynchronizedStmt(clang::ObjCAtSynchronizedStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAtSynchronizedStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAtThrowStmt(clang::ObjCAtThrowStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAtThrowStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAtTryStmt(clang::ObjCAtTryStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAtTryStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCAutoreleasePoolStmt(clang::ObjCAutoreleasePoolStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCAutoreleasePoolStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitObjCForCollectionStmt(clang::ObjCForCollectionStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ObjCForCollectionStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitReturnStmt(clang::ReturnStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_ReturnStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSEHExceptStmt(clang::SEHExceptStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SEHExceptStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSEHFinallyStmt(clang::SEHFinallyStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SEHFinallyStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSEHLeaveStmt(clang::SEHLeaveStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SEHLeaveStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSEHTryStmt(clang::SEHTryStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SEHTryStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

/*IStmtPtr ASTWalker::VisitSwitchCase(clang::SwitchCase *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SwitchCase, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}*/

IStmtPtr ASTWalker::VisitCaseStmt(clang::CaseStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CaseStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitDefaultStmt(clang::DefaultStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_DefaultStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitSwitchStmt(clang::SwitchStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_SwitchStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitWhileStmt(clang::WhileStmt *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_WhileStmt, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinPtrMemD(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinPtrMemI(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinMul(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinDiv(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinRem(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinAdd(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinSub(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinShl(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinShr(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinLT(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinGT(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinLE(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinGE(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinEQ(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinNE(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinAnd(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinXor(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinOr(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinLAnd(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinLOr(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinAssign(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinComma(clang::BinaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_BinaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinMulAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinDivAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinRemAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinAddAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinSubAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinShlAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinShrAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinAndAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitBinOrAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitBinXorAssign(clang::CompoundAssignOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_CompoundAssignOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryPostInc(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitUnaryPostDec(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryPreInc(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitUnaryPreDec(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryAddrOf(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitUnaryDeref(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryPlus(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitUnaryMinus(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryNot(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitUnaryLNot(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryReal(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
IStmtPtr ASTWalker::VisitUnaryImag(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryExtension(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitUnaryCoawait(clang::UnaryOperator *src) {
	IStmtPtr stmt = pComClang->CreateStmt(StmtClass_UnaryOperator, ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}

IStmtPtr ASTWalker::VisitStmt(clang::Stmt *src)
{
	IStmtPtr stmt = pComClang->CreateStmt((StmtClass)src->getStmtClass(), ParentStmt);
	stmt->Native = (uintptr_t)src;
	WalkStmt(stmt, src);
	return stmt;
}
