/************************************************************************
*
* CppSharp
* Licensed under the simplified BSD license. All rights reserved.
*
************************************************************************/

#ifdef DEBUG
#undef DEBUG // workaround DEBUG define messing with LLVM COFF headers
#endif

#include "Parser.h"
#include "ELFDumper.h"


#include "ComClang.h"

#if defined(__APPLE__) || defined(__linux__)
#ifndef _GNU_SOURCE
#define _GNU_SOURCE
#endif
#include <dlfcn.h>

#define HAVE_DLFCN
#endif

using namespace CppSharp::CppParser;

// We use this as a placeholder for pointer values that should be ignored.
void* IgnorePtr = reinterpret_cast<void*>(0x1);

//-----------------------------------//

Parser::Parser(IComClangPtr pComClang, ICppParserOptionsPtr Opts)
	: pComClang(pComClang), opts(Opts), index(0)
{
	for (int i = 0; i < Opts->SupportedStdTypesCount; i++) {
		const auto& SupportedStdType = std::string(Opts->GetSupportedStdTypes(i));
		supportedStdTypes.insert(SupportedStdType);
	}
	supportedStdTypes.insert("allocator");
	supportedStdTypes.insert("basic_string");
}

ILayoutFieldPtr Parser::WalkVTablePointer(IClassPtr Class,
	const clang::CharUnits& Offset, const std::string& prefix)
{
	ILayoutFieldPtr LayoutField = pComClang->CreateLayoutField();
	LayoutField->Offset = Offset.getQuantity();
	IDeclarationPtr Classd = IDeclarationPtr(Class);
	LayoutField->name = (prefix + "_" + std::string(Classd->name)).c_str();
	LayoutField->QualifiedType = GetQualifiedType(c->getASTContext().VoidPtrTy);
	return LayoutField;
}

void Parser::ReadClassLayout(IClassPtr Class, const clang::RecordDecl* RD,
	clang::CharUnits Offset, bool IncludeVirtualBases)
{
	using namespace clang;

	const auto &Layout = c->getASTContext().getASTRecordLayout(RD);
	auto CXXRD = dyn_cast<CXXRecordDecl>(RD);

	IClassPtr Parent = IClassPtr(WalkDeclaration(RD));

	if (Class != Parent)
	{
		ILayoutBasePtr LayoutBase = pComClang->CreateLayoutBase();
		LayoutBase->Offset = Offset.getQuantity();
		LayoutBase->Class = Parent;
		Class->Layout->AddBase(LayoutBase);
	}

	// Dump bases.
	if (CXXRD) {
		const CXXRecordDecl *PrimaryBase = Layout.getPrimaryBase();
		bool HasOwnVFPtr = Layout.hasOwnVFPtr();
		bool HasOwnVBPtr = Layout.hasOwnVBPtr();

		// Vtable pointer.
		if (CXXRD->isDynamicClass() && !PrimaryBase &&
			!c->getTarget().getCXXABI().isMicrosoft()) {
			auto VPtr = WalkVTablePointer(Parent, Offset, "vptr");
			Class->Layout->AddField(VPtr);
		}
		else if (HasOwnVFPtr) {
			auto VTPtr = WalkVTablePointer(Parent, Offset, "vfptr");
			Class->Layout->AddField(VTPtr);
		}

		// Collect nvbases.
		SmallVector<const CXXRecordDecl *, 4> Bases;
		for (const CXXBaseSpecifier &Base : CXXRD->bases()) {
			assert(!Base.getType()->isDependentType() &&
				"Cannot layout class with dependent bases.");
			if (!Base.isVirtual())
				Bases.push_back(Base.getType()->getAsCXXRecordDecl());
		}

		// Sort nvbases by offset.
		std::stable_sort(Bases.begin(), Bases.end(),
			[&](const CXXRecordDecl *L, const CXXRecordDecl *R) {
			return Layout.getBaseClassOffset(L) < Layout.getBaseClassOffset(R);
		});

		// Dump (non-virtual) bases
		for (const CXXRecordDecl *Base : Bases) {
			CharUnits BaseOffset = Offset + Layout.getBaseClassOffset(Base);
			ReadClassLayout(Class, Base, BaseOffset,
				/*IncludeVirtualBases=*/false);
		}

		// vbptr (for Microsoft C++ ABI)
		if (HasOwnVBPtr) {
			auto VBPtr = WalkVTablePointer(Parent,
				Offset + Layout.getVBPtrOffset(), "vbptr");
			Class->Layout->AddField(VBPtr);
		}
	}

	// Dump fields.
	uint64_t FieldNo = 0;
	for (RecordDecl::field_iterator I = RD->field_begin(),
		E = RD->field_end(); I != E; ++I, ++FieldNo) {
		auto Field = *I;
		uint64_t LocalFieldOffsetInBits = Layout.getFieldOffset(FieldNo);
		CharUnits FieldOffset =
			Offset + c->getASTContext().toCharUnitsFromBits(LocalFieldOffsetInBits);

		auto F = WalkFieldCXX(Field, Parent);
		ILayoutFieldPtr LayoutField = pComClang->CreateLayoutField();
		LayoutField->Offset = FieldOffset.getQuantity();
		IDeclarationPtr Fd = IDeclarationPtr(F);
		LayoutField->name = Fd->name;
		LayoutField->QualifiedType = GetQualifiedType(Field->getType());
		LayoutField->FieldPtr = (intptr_t)Field;
		Class->Layout->AddField(LayoutField);
	}

	// Dump virtual bases.
	if (CXXRD && IncludeVirtualBases) {
		const ASTRecordLayout::VBaseOffsetsMapTy &VtorDisps =
			Layout.getVBaseOffsetsMap();

		for (const CXXBaseSpecifier &Base : CXXRD->vbases()) {
			assert(Base.isVirtual() && "Found non-virtual class!");
			const CXXRecordDecl *VBase = Base.getType()->getAsCXXRecordDecl();

			CharUnits VBaseOffset = Offset + Layout.getVBaseClassOffset(VBase);

			if (VtorDisps.find(VBase)->second.hasVtorDisp()) {
				auto VtorDisp = WalkVTablePointer(Parent,
					VBaseOffset - CharUnits::fromQuantity(4), "vtordisp");
				Class->Layout->AddField(VtorDisp);
			}

			ReadClassLayout(Class, VBase, VBaseOffset,
				/*IncludeVirtualBases=*/false);
		}
	}
}

static std::string GetClangResourceDir(std::string CurrentDir)
{
	using namespace llvm;
	using namespace clang;

	// Compute the path to the resource directory.
	StringRef ClangResourceDir(CLANG_RESOURCE_DIR);
	SmallString<128> P(CurrentDir);
	llvm::sys::path::remove_filename(P);

	if (ClangResourceDir != "")
		llvm::sys::path::append(P, ClangResourceDir);
	else
		llvm::sys::path::append(P, "lib", "clang", CLANG_VERSION_STRING);

	return P.str();
}

//-----------------------------------//

static clang::TargetCXXABI::Kind
ConvertToClangTargetCXXABI(CppAbi abi)
{
	using namespace clang;

	switch (abi)
	{
	case CppAbi_Itanium:
		return TargetCXXABI::GenericItanium;
	case CppAbi_Microsoft:
		return TargetCXXABI::Microsoft;
	case CppAbi_ARM:
		return TargetCXXABI::GenericARM;
	case CppAbi_iOS:
		return TargetCXXABI::iOS;
	case CppAbi_iOS64:
		return TargetCXXABI::iOS64;
	}

	llvm_unreachable("Unsupported C++ ABI.");
}

void Parser::Setup()
{
	llvm::InitializeAllTargets();
	llvm::InitializeAllTargetMCs();
	llvm::InitializeAllAsmParsers();

	using namespace clang;

	std::vector<std::string> _args;
	std::vector<const char*> args;
	args.push_back("-cc1");

	for (unsigned I = 0, E = opts->ArgumentsCount; I != E; ++I)
	{
		_args.push_back(std::string(opts->GetArguments(I)));
	}
	for (const auto &a : _args)
	{
		args.push_back(a.c_str());
	}

	c.reset(new CompilerInstance());
	c->createDiagnostics();

	CompilerInvocation* Inv = new CompilerInvocation();
	CompilerInvocation::CreateFromArgs(*Inv, args.data(), args.data() + args.size(),
		c->getDiagnostics());
	c->setInvocation(std::shared_ptr<CompilerInvocation>(Inv));
	c->getLangOpts() = *Inv->LangOpts;

	auto& TO = Inv->TargetOpts;
	targetABI = ConvertToClangTargetCXXABI(opts->ABI);

	if (std::string(opts->TargetTriple).empty())
		opts->TargetTriple = llvm::sys::getDefaultTargetTriple().c_str();
	TO->Triple = llvm::Triple::normalize(std::string(opts->TargetTriple));

	TargetInfo* TI = TargetInfo::CreateTargetInfo(c->getDiagnostics(), TO);
	if (!TI)
	{
		// We might have no target info due to an invalid user-provided triple.
		// Try again with the default triple.
		opts->TargetTriple = llvm::sys::getDefaultTargetTriple().c_str();
		TO->Triple = llvm::Triple::normalize(std::string(opts->TargetTriple));
		TI = TargetInfo::CreateTargetInfo(c->getDiagnostics(), TO);
	}

	assert(TI && "Expected valid target info");

	c->setTarget(TI);

	c->createFileManager();
	c->createSourceManager(c->getFileManager());

	auto& HSOpts = c->getHeaderSearchOpts();
	auto& PPOpts = c->getPreprocessorOpts();
	auto& LangOpts = c->getLangOpts();

	if (opts->NoStandardIncludes)
	{
		HSOpts.UseStandardSystemIncludes = false;
		HSOpts.UseStandardCXXIncludes = false;
	}

	if (opts->NoBuiltinIncludes)
		HSOpts.UseBuiltinIncludes = false;

	if (opts->Verbose)
		HSOpts.Verbose = true;

#ifndef __APPLE__
	// Initialize the default platform headers.
	HSOpts.ResourceDir = GetClangResourceDir(std::string(opts->CurrentDir));

	llvm::SmallString<128> ResourceDir(HSOpts.ResourceDir);
	llvm::sys::path::append(ResourceDir, "include");
	HSOpts.AddPath(ResourceDir.str(), clang::frontend::System, /*IsFramework=*/false,
		/*IgnoreSysRoot=*/false);
#endif

	for (unsigned I = 0, E = opts->IncludeDirsCount; I != E; ++I)
	{
		const auto& s = std::string(opts->GetIncludeDirs(I));
		HSOpts.AddPath(s, frontend::Angled, false, false);
	}

	for (unsigned I = 0, E = opts->SystemIncludeDirsCount; I != E; ++I)
	{
		const auto& s = std::string(opts->GetSystemIncludeDirs(I));
		HSOpts.AddPath(s, frontend::System, false, false);
	}

	for (unsigned I = 0, E = opts->DefinesCount; I != E; ++I)
	{
		const auto& define = std::string(opts->GetDefines(I));
		PPOpts.addMacroDef(define);
	}

	for (unsigned I = 0, E = opts->UndefinesCount; I != E; ++I)
	{
		const auto& undefine = std::string(opts->GetUndefines(I));
		PPOpts.addMacroUndef(undefine);
	}

#ifdef _MSC_VER
	if (opts->MicrosoftMode)
	{
		LangOpts.MSCompatibilityVersion = opts->ToolSetToUse;
		if (!LangOpts.MSCompatibilityVersion) LangOpts.MSCompatibilityVersion = 1700;
	}
#endif

	llvm::opt::InputArgList Args(0, 0);
	clang::driver::Driver D("", TO->Triple, c->getDiagnostics());
	clang::driver::ToolChain *TC = nullptr;
	llvm::Triple Target(TO->Triple);
	switch (Target.getOS()) {
		// Extend this for other triples if needed, see clang's Driver::getToolChain.
	case llvm::Triple::Linux:
		TC = new clang::driver::toolchains::Linux(D, Target, Args);
		break;
	default:
		break;
	}

	if (TC && !opts->NoStandardIncludes) {
		llvm::opt::ArgStringList Includes;
		TC->AddClangSystemIncludeArgs(Args, Includes);
		TC->AddClangCXXStdlibIncludeArgs(Args, Includes);
		for (auto& Arg : Includes) {
			if (strlen(Arg) > 0 && Arg[0] != '-')
				HSOpts.AddPath(Arg, frontend::System, /*IsFramework=*/false,
					/*IgnoreSysRoot=*/false);
		}
	}

	// Enable preprocessing record.
	PPOpts.DetailedRecord = true;

	c->createPreprocessor(TU_Complete);

	Preprocessor& PP = c->getPreprocessor();
	PP.getBuiltinInfo().initializeBuiltins(PP.getIdentifierTable(),
		PP.getLangOpts());

	c->createASTContext();
}

//-----------------------------------//

std::string Parser::GetDeclMangledName(const clang::Decl* D)
{
	using namespace clang;

	if (!D || !isa<NamedDecl>(D))
		return "";

	bool CanMangle = isa<FunctionDecl>(D) || isa<VarDecl>(D)
		|| isa<CXXConstructorDecl>(D) || isa<CXXDestructorDecl>(D);

	if (!CanMangle) return "";

	auto ND = cast<NamedDecl>(D);
	std::unique_ptr<MangleContext> MC;

	auto& AST = c->getASTContext();
	switch (targetABI)
	{
	default:
		MC.reset(ItaniumMangleContext::create(AST, AST.getDiagnostics()));
		break;
	case TargetCXXABI::Microsoft:
		MC.reset(MicrosoftMangleContext::create(AST, AST.getDiagnostics()));
		break;
	}

	if (!MC)
		llvm_unreachable("Unknown mangling ABI");

	std::string Mangled;
	llvm::raw_string_ostream Out(Mangled);

	bool IsDependent = false;
	if (const ValueDecl *VD = dyn_cast<ValueDecl>(ND))
		IsDependent |= VD->getType()->isDependentType();

	if (!IsDependent)
		IsDependent |= ND->getDeclContext()->isDependentContext();

	if (!MC->shouldMangleDeclName(ND) || IsDependent)
		return ND->getDeclName().getAsString();

	if (const CXXConstructorDecl *CD = dyn_cast<CXXConstructorDecl>(ND))
		MC->mangleCXXCtor(CD, Ctor_Base, Out);
	else if (const CXXDestructorDecl *DD = dyn_cast<CXXDestructorDecl>(ND))
		MC->mangleCXXDtor(DD, Dtor_Base, Out);
	else
		MC->mangleName(ND, Out);

	Out.flush();

	// Strip away LLVM name marker.
	if (!Mangled.empty() && Mangled[0] == '\01')
		Mangled = Mangled.substr(1);

	return Mangled;
}

//-----------------------------------//

static std::string GetDeclName(const clang::NamedDecl* D)
{
	if (const clang::IdentifierInfo *II = D->getIdentifier())
		return II->getName();
	return D->getNameAsString();
}

static std::string GetTagDeclName(const clang::TagDecl* D)
{
	using namespace clang;

	if (auto Typedef = D->getTypedefNameForAnonDecl())
	{
		assert(Typedef->getIdentifier() && "Typedef without identifier?");
		return GetDeclName(Typedef);
	}

	return GetDeclName(D);
}

static std::string GetDeclUSR(const clang::Decl* D)
{
	using namespace clang;
	SmallString<128> usr;
	if (!index::generateUSRForDecl(D, usr))
		return usr.str();
	return "<invalid>";
}

static clang::Decl* GetPreviousDeclInContext(const clang::Decl* D)
{
	assert(!D->getLexicalDeclContext()->decls_empty());

	clang::Decl* prevDecl = nullptr;
	for (auto it = D->getDeclContext()->decls_begin();
		it != D->getDeclContext()->decls_end(); it++)
	{
		if ((*it) == D)
			return prevDecl;
		prevDecl = (*it);
	}

	return nullptr;
}

bool IsExplicit(const clang::Decl* D)
{
	using namespace clang;

	auto CTS = llvm::dyn_cast<ClassTemplateSpecializationDecl>(D);
	return !CTS ||
		CTS->getSpecializationKind() == TSK_ExplicitSpecialization ||
		CTS->getSpecializationKind() == TSK_ExplicitInstantiationDeclaration ||
		CTS->getSpecializationKind() == TSK_ExplicitInstantiationDefinition;
}

static clang::SourceLocation GetDeclStartLocation(clang::CompilerInstance* C,
	const clang::Decl* D)
{
	auto& SM = C->getSourceManager();
	auto startLoc = SM.getExpansionLoc(D->getLocStart());
	auto startOffset = SM.getFileOffset(startLoc);

	if (clang::dyn_cast_or_null<clang::TranslationUnitDecl>(D) || !startLoc.isValid())
		return startLoc;

	auto lineNo = SM.getExpansionLineNumber(startLoc);
	auto lineBeginLoc = SM.translateLineCol(SM.getFileID(startLoc), lineNo, 1);
	auto lineBeginOffset = SM.getFileOffset(lineBeginLoc);
	assert(lineBeginOffset <= startOffset);

	if (D->getLexicalDeclContext()->decls_empty())
		return lineBeginLoc;

	auto prevDecl = GetPreviousDeclInContext(D);
	if (!prevDecl || !IsExplicit(prevDecl))
		return lineBeginLoc;

	auto prevDeclEndLoc = SM.getExpansionLoc(prevDecl->getLocEnd());
	auto prevDeclEndOffset = SM.getFileOffset(prevDeclEndLoc);

	if (SM.getFileID(prevDeclEndLoc) != SM.getFileID(startLoc))
		return lineBeginLoc;

	// TODO: Figure out why this asserts
	//assert(prevDeclEndOffset <= startOffset);

	if (prevDeclEndOffset < lineBeginOffset)
		return lineBeginLoc;

	// Declarations don't share same macro expansion
	if (SM.getExpansionLoc(prevDecl->getLocStart()) != startLoc)
		return prevDeclEndLoc;

	return GetDeclStartLocation(C, prevDecl);
}

std::string Parser::GetTypeName(const clang::Type* Type)
{
	using namespace clang;

	if (Type->isAnyPointerType() || Type->isReferenceType())
		Type = Type->getPointeeType().getTypePtr();

	if (Type->isEnumeralType() || Type->isRecordType())
	{
		const clang::TagType* Tag = Type->getAs<clang::TagType>();
		return GetTagDeclName(Tag->getDecl());
	}

	PrintingPolicy pp(c->getLangOpts());
	pp.SuppressTagKeyword = true;

	std::string TypeName;
	QualType::getAsStringInternal(Type, Qualifiers(), TypeName, pp);

	return TypeName;
}

static ITypeQualifiersPtr GetTypeQualifiers(IComClangPtr pComClang, const clang::QualType& Type)
{
	ITypeQualifiersPtr quals = pComClang->CreateTypeQualifiers();
	quals->IsConst = Type.isLocalConstQualified();
	quals->IsRestrict = Type.isLocalRestrictQualified();
	quals->IsVolatile = Type.isVolatileQualified();
	return quals;
}

IQualifiedTypePtr Parser::GetQualifiedType(const clang::QualType& qual, const clang::TypeLoc* TL)
{
	IQualifiedTypePtr qualType = pComClang->CreateQualifiedType();
	qualType->Type = WalkType(qual, TL);
	qualType->Qualifiers = GetTypeQualifiers(pComClang, qual);
	return qualType;
}

//-----------------------------------//

static AccessSpecifier ConvertToAccess(clang::AccessSpecifier AS)
{
	switch (AS)
	{
	case clang::AS_private:
		return AccessSpecifier_Private;
	case clang::AS_protected:
		return AccessSpecifier_Protected;
	case clang::AS_public:
		return AccessSpecifier_Public;
	case clang::AS_none:
		return AccessSpecifier_Public;
	}

	llvm_unreachable("Unknown AccessSpecifier");
}

IVTableComponentPtr Parser::WalkVTableComponent(const clang::VTableComponent& Component)
{
	using namespace clang;
	IVTableComponentPtr VTC = pComClang->CreateVTableComponent();

	switch (Component.getKind())
	{
	case clang::VTableComponent::CK_VCallOffset:
	{
		VTC->Kind = VTableComponentKind_VBaseOffset;
		VTC->Offset = Component.getVCallOffset().getQuantity();
		break;
	}
	case clang::VTableComponent::CK_VBaseOffset:
	{
		VTC->Kind = VTableComponentKind_VBaseOffset;
		VTC->Offset = Component.getVBaseOffset().getQuantity();
		break;
	}
	case clang::VTableComponent::CK_OffsetToTop:
	{
		VTC->Kind = VTableComponentKind_OffsetToTop;
		VTC->Offset = Component.getOffsetToTop().getQuantity();
		break;
	}
	case clang::VTableComponent::CK_RTTI:
	{
		VTC->Kind = VTableComponentKind_RTTI;
		auto RD = Component.getRTTIDecl();
		VTC->Declaration = IDeclarationPtr(WalkRecordCXX(RD));
		break;
	}
	case clang::VTableComponent::CK_FunctionPointer:
	{
		VTC->Kind = VTableComponentKind_FunctionPointer;
		auto MD = Component.getFunctionDecl();
		VTC->Declaration = IDeclarationPtr(WalkMethodCXX(MD));
		break;
	}
	case clang::VTableComponent::CK_CompleteDtorPointer:
	{
		VTC->Kind = VTableComponentKind_CompleteDtorPointer;
		auto MD = Component.getDestructorDecl();
		VTC->Declaration = IDeclarationPtr(WalkMethodCXX(MD));
		break;
	}
	case clang::VTableComponent::CK_DeletingDtorPointer:
	{
		VTC->Kind = VTableComponentKind_DeletingDtorPointer;
		auto MD = Component.getDestructorDecl();
		VTC->Declaration = IDeclarationPtr(WalkMethodCXX(MD));
		break;
	}
	case clang::VTableComponent::CK_UnusedFunctionPointer:
	{
		VTC->Kind = VTableComponentKind_UnusedFunctionPointer;
		auto MD = Component.getUnusedFunctionDecl();
		VTC->Declaration = IDeclarationPtr(WalkMethodCXX(MD));
		break;
	}
	default:
		llvm_unreachable("Unknown vtable component kind");
	}

	return VTC;
}

IVTableLayoutPtr Parser::WalkVTableLayout(const clang::VTableLayout& VTLayout)
{
	IVTableLayoutPtr Layout = pComClang->CreateVTableLayout();

	for (const auto& VTC : VTLayout.vtable_components())
	{
		auto VTComponent = WalkVTableComponent(VTC);
		Layout->AddComponent(VTComponent);
	}

	return Layout;
}


void Parser::WalkVTable(const clang::CXXRecordDecl* RD, IClassPtr C)
{
	using namespace clang;

	assert(RD->isDynamicClass() && "Only dynamic classes have virtual tables");

	if (!C->Layout)
		C->Layout = pComClang->CreateClassLayout();

	auto& AST = c->getASTContext();
	switch (targetABI)
	{
	case TargetCXXABI::Microsoft:
	{
		C->Layout->ABI = CppAbi_Microsoft;
		MicrosoftVTableContext VTContext(AST);

		const auto& VFPtrs = VTContext.getVFPtrOffsets(RD);
		for (const auto& VFPtrInfo : VFPtrs)
		{
			IVFTableInfoPtr Info = pComClang->CreateVFTableInfo();
			Info->VFPtrOffset = VFPtrInfo->NonVirtualOffset.getQuantity();
			Info->VFPtrFullOffset = VFPtrInfo->FullOffsetInMDC.getQuantity();

			auto& VTLayout = VTContext.getVFTableLayout(RD, VFPtrInfo->FullOffsetInMDC);
			Info->Layout = WalkVTableLayout(VTLayout);

			C->Layout->AddVFTable(Info);
		}
		break;
	}
	case TargetCXXABI::GenericItanium:
	{
		C->Layout->ABI = CppAbi_Itanium;
		ItaniumVTableContext VTContext(AST);

		auto& VTLayout = VTContext.getVTableLayout(RD);
		C->Layout->Layout = WalkVTableLayout(VTLayout);
		break;
	}
	default:
		llvm_unreachable("Unsupported C++ ABI kind");
	}
}

void Parser::EnsureCompleteRecord(const clang::RecordDecl* Record,
	IDeclarationContextPtr NS, IClassPtr RC)
{
	using namespace clang;
	IDeclarationPtr RCd = IDeclarationPtr(RC);

	if (!RCd->IsIncomplete || RCd->CompleteDeclaration)
		return;

	Decl* Definition;
	if (auto CXXRecord = dyn_cast<CXXRecordDecl>(Record))
		Definition = CXXRecord->getDefinition();
	else
		Definition = Record->getDefinition();

	if (!Definition)
		return;

	RCd->CompleteDeclaration = WalkDeclaration(Definition);
}

IClassPtr Parser::GetRecord(const clang::RecordDecl* Record, bool& Process)
{
	using namespace clang;
	Process = false;

	auto NS = GetNamespace(Record);
	assert(NS && "Expected a valid namespace");

	bool isCompleteDefinition = Record->isCompleteDefinition();

	IClassPtr RC = nullptr;

	auto Name = GetTagDeclName(Record);
	auto HasEmptyName = Record->getDeclName().isEmpty();

	if (HasEmptyName)
	{
		auto USR = GetDeclUSR(Record);
		if (auto AR = NS->FindAnonymous(USR.c_str())) {
			RC = IClassPtr(AR);
		}
	}
	else
	{
		RC = NS->FindClass_2(opts->UnityBuild ? (intptr_t)Record : 0, Name.c_str(),
			isCompleteDefinition, /*Create=*/false);
	}

	if (RC)
		return RC;

	RC = NS->FindClass_2(opts->UnityBuild ? (intptr_t)Record : 0, Name.c_str(),
		isCompleteDefinition, /*Create=*/true);
	RC->IsInjected = Record->isInjectedClassName();
	IDeclarationPtr RCd = IDeclarationPtr(RC);
	HandleDeclaration(Record, RCd);
	EnsureCompleteRecord(Record, NS, RC);

	for (auto Redecl : Record->redecls())
	{
		if (Redecl->isImplicit() || Redecl == Record)
			continue;

		RCd->AddRedeclaration(WalkDeclaration(Redecl));
	}

	if (HasEmptyName)
	{
		auto USR = GetDeclUSR(Record);
		NS->AddAnonymous(USR.c_str(), RCd);
	}

	if (!isCompleteDefinition)
		return RC;

	Process = true;
	return RC;
}

IClassPtr Parser::WalkRecord(const clang::RecordDecl* Record)
{
	bool Process;
	auto RC = GetRecord(Record, Process);

	if (!RC || !Process)
		return RC;

	WalkRecord(Record, RC);

	return RC;
}

IClassPtr Parser::WalkRecordCXX(const clang::CXXRecordDecl* Record)
{
	bool Process;
	auto RC = GetRecord(Record, Process);

	if (!RC || !Process)
		return RC;

	WalkRecordCXX(Record, RC);

	return RC;
}

static int I = 0;

static bool IsRecordValid(const clang::RecordDecl* RC,
	std::unordered_set<const clang::RecordDecl*>& Visited)
{
	using namespace clang;

	if (Visited.find(RC) != Visited.end())
		return true;

	Visited.insert(RC);
	if (RC->isInvalidDecl())
		return false;
	for (auto Field : RC->fields())
	{
		auto Type = Field->getType()->getUnqualifiedDesugaredType();
		const auto* RD = const_cast<CXXRecordDecl*>(Type->getAsCXXRecordDecl());
		if (!RD)
			RD = Type->getPointeeCXXRecordDecl();
		if (RD && !IsRecordValid(RD, Visited))
			return false;
	}
	return true;
}

static bool IsRecordValid(const clang::RecordDecl* RC)
{
	std::unordered_set<const clang::RecordDecl*> Visited;
	return IsRecordValid(RC, Visited);
}

static clang::CXXRecordDecl* GetCXXRecordDeclFromBaseType(const clang::QualType& Ty) {
	using namespace clang;

	if (auto RT = Ty->getAs<clang::RecordType>())
		return dyn_cast<clang::CXXRecordDecl>(RT->getDecl());
	else if (auto TST = Ty->getAs<clang::TemplateSpecializationType>())
		return dyn_cast<clang::CXXRecordDecl>(
			TST->getTemplateName().getAsTemplateDecl()->getTemplatedDecl());
	else if (auto Injected = Ty->getAs<clang::InjectedClassNameType>())
		return Injected->getDecl();

	assert("Could not get base CXX record from type");
	return nullptr;
}

static bool HasLayout(const clang::RecordDecl* Record)
{
	if (Record->isDependentType() || !Record->getDefinition() ||
		!IsRecordValid(Record))
		return false;

	if (auto CXXRecord = llvm::dyn_cast<clang::CXXRecordDecl>(Record))
		for (auto Base : CXXRecord->bases())
		{
			auto CXXBase = GetCXXRecordDeclFromBaseType(Base.getType());
			if (!CXXBase || !HasLayout(CXXBase))
				return false;
		}

	return true;
}

bool Parser::IsSupported(const clang::NamedDecl* ND)
{
	return !c->getSourceManager().isInSystemHeader(ND->getLocStart()) ||
		(llvm::isa<clang::RecordDecl>(ND) &&
			supportedStdTypes.find(ND->getName()) != supportedStdTypes.end());
}

bool Parser::IsSupported(const clang::CXXMethodDecl* MD)
{
	using namespace clang;

	return !c->getSourceManager().isInSystemHeader(MD->getLocStart()) ||
		isa<CXXConstructorDecl>(MD) || isa<CXXDestructorDecl>(MD) ||
		(MD->getDeclName().isIdentifier() && MD->getName() == "c_str" &&
			supportedStdTypes.find(MD->getParent()->getName()) !=
			supportedStdTypes.end());
}

void Parser::WalkRecord(const clang::RecordDecl* Record, IClassPtr RC)
{
	using namespace clang;

	if (Record->isImplicit())
		return;

	IDeclarationPtr RCd = IDeclarationPtr(RC);

	if (IsExplicit(Record))
	{
		auto headStartLoc = GetDeclStartLocation(c.get(), Record);
		auto headEndLoc = Record->getLocation(); // identifier location
		auto bodyEndLoc = Record->getLocEnd();

		auto headRange = clang::SourceRange(headStartLoc, headEndLoc);
		auto bodyRange = clang::SourceRange(headEndLoc, bodyEndLoc);

		HandlePreprocessedEntities(RCd, headRange, MacroLocation_ClassHead);
		HandlePreprocessedEntities(RCd, bodyRange, MacroLocation_ClassBody);
	}

	auto& Sema = c->getSema();

	RC->IsUnion = Record->isUnion();
	RCd->IsDependent = Record->isDependentType();
	RC->IsExternCContext = Record->isExternCContext();

	bool hasLayout = HasLayout(Record);

	if (hasLayout)
	{
		const auto& Layout = c->getASTContext().getASTRecordLayout(Record);
		if (!RC->Layout)
			RC->Layout = pComClang->CreateClassLayout();
		RC->Layout->Alignment = (int)Layout.getAlignment().getQuantity();
		RC->Layout->Size = (int)Layout.getSize().getQuantity();
		RC->Layout->DataSize = (int)Layout.getDataSize().getQuantity();
		ReadClassLayout(RC, Record, CharUnits(), true);
	}

	for (auto FD : Record->fields())
		WalkFieldCXX(FD, RC);

	if (c->getSourceManager().isInSystemHeader(Record->getLocStart()))
	{
		if (supportedStdTypes.find(Record->getName()) != supportedStdTypes.end())
		{
			for (auto D : Record->decls())
			{
				switch (D->getKind())
				{
				case Decl::CXXConstructor:
				case Decl::CXXDestructor:
				case Decl::CXXConversion:
				case Decl::CXXMethod:
				{
					auto MD = cast<CXXMethodDecl>(D);
					if (IsSupported(MD))
						WalkMethodCXX(MD);
					break;
				}
				}
			}
		}
		return;
	}

	for (auto D : Record->decls())
	{
		switch (D->getKind())
		{
		case Decl::CXXConstructor:
		case Decl::CXXDestructor:
		case Decl::CXXConversion:
		case Decl::CXXMethod:
		{
			auto MD = cast<CXXMethodDecl>(D);
			WalkMethodCXX(MD);
			break;
		}
		case Decl::AccessSpec:
		{
			AccessSpecDecl* AS = cast<AccessSpecDecl>(D);

			IAccessSpecifierDeclPtr AccessDecl = pComClang->CreateAccessSpecifierDecl();
			IDeclarationPtr AccessDecld = IDeclarationPtr(AccessDecl);
			HandleDeclaration(AS, AccessDecld);

			AccessDecld->Access = ConvertToAccess(AS->getAccess());
			AccessDecld->Namespace = IDeclarationContextPtr(RC);

			RC->AddSpecifier(AccessDecl);
			break;
		}
		case Decl::Field: // fields already handled
		case Decl::IndirectField: // FIXME: Handle indirect fields
			break;
		case Decl::CXXRecord:
			// Handle implicit records inside the class.
			if (D->isImplicit())
				continue;
			WalkDeclaration(D);
			break;
		case Decl::Friend:
		{
			FriendDecl* FD = cast<FriendDecl>(D);
			auto decl = FD->getFriendDecl();

			// Skip every friend declaration that isn't a function declaration
			if (decl && !isa<FunctionDecl>(decl))
				continue;
			WalkDeclaration(D);
			break;
		}
		case Decl::FriendTemplate:
		{
			// In this case always skip the declaration since, unlike Decl::Friend handled above,
			// it never is a declaration of a friend function or method
			break;
		}
		default:
		{
			WalkDeclaration(D);
			break;
		}
		}
	}
}

void Parser::WalkRecordCXX(const clang::CXXRecordDecl* Record, IClassPtr RC)
{
	using namespace clang;

	if (Record->isImplicit())
		return;

	auto& Sema = c->getSema();
	Sema.ForceDeclarationOfImplicitMembers(const_cast<clang::CXXRecordDecl*>(Record));

	WalkRecord(Record, RC);

	RC->IsPOD = Record->isPOD();
	RC->IsAbstract = Record->isAbstract();
	RC->IsDynamic = Record->isDynamicClass();
	RC->IsPolymorphic = Record->isPolymorphic();
	RC->HasNonTrivialDefaultConstructor = Record->hasNonTrivialDefaultConstructor();
	RC->HasNonTrivialCopyConstructor = Record->hasNonTrivialCopyConstructor();
	RC->HasNonTrivialDestructor = Record->hasNonTrivialDestructor();

	bool hasLayout = HasLayout(Record) &&
		Record->getDeclName() != c->getSema().VAListTagName;

	// Get the record layout information.
	const ASTRecordLayout* Layout = 0;
	if (hasLayout)
	{
		Layout = &c->getASTContext().getASTRecordLayout(Record);

		assert(RC->Layout && "Expected a valid AST layout");
		RC->Layout->HasOwnVFPtr = Layout->hasOwnVFPtr();
		RC->Layout->VBPtrOffset = Layout->getVBPtrOffset().getQuantity();
	}

	// Iterate through the record bases.
	for (auto BS : Record->bases())
	{
		IBaseClassSpecifierPtr Base = pComClang->CreateBaseClassSpecifier();
		Base->Access = ConvertToAccess(BS.getAccessSpecifier());
		Base->IsVirtual = BS.isVirtual();

		auto BSTL = BS.getTypeSourceInfo()->getTypeLoc();
		Base->Type = WalkType(BS.getType(), &BSTL);

		auto BaseDecl = GetCXXRecordDeclFromBaseType(BS.getType());
		if (BaseDecl && Layout)
		{
			auto Offset = BS.isVirtual() ? Layout->getVBaseClassOffset(BaseDecl)
				: Layout->getBaseClassOffset(BaseDecl);
			Base->Offset = Offset.getQuantity();
		}

		RC->AddBase(Base);
	}

	// Process the vtables
	if (hasLayout && Record->isDynamicClass())
		WalkVTable(Record, RC);
}

//-----------------------------------//

static TemplateSpecializationKind
WalkTemplateSpecializationKind(clang::TemplateSpecializationKind Kind)
{
	switch (Kind)
	{
	case clang::TSK_Undeclared:
		return TemplateSpecializationKind_Undeclared;
	case clang::TSK_ImplicitInstantiation:
		return TemplateSpecializationKind_ImplicitInstantiation;
	case clang::TSK_ExplicitSpecialization:
		return TemplateSpecializationKind_ExplicitSpecialization;
	case clang::TSK_ExplicitInstantiationDeclaration:
		return TemplateSpecializationKind_ExplicitInstantiationDeclaration;
	case clang::TSK_ExplicitInstantiationDefinition:
		return TemplateSpecializationKind_ExplicitInstantiationDefinition;
	}

	llvm_unreachable("Unknown template specialization kind");
}

//-----------------------------------//

struct Diagnostic
{
	clang::SourceLocation Location;
	llvm::SmallString<100> Message;
	clang::DiagnosticsEngine::Level Level;
};

struct DiagnosticConsumer : public clang::DiagnosticConsumer
{
	virtual void HandleDiagnostic(clang::DiagnosticsEngine::Level Level,
		const clang::Diagnostic& Info) override {
		// Update the base type NumWarnings and NumErrors variables.
		if (Level == clang::DiagnosticsEngine::Warning)
			NumWarnings++;

		if (Level == clang::DiagnosticsEngine::Error ||
			Level == clang::DiagnosticsEngine::Fatal)
		{
			NumErrors++;
			if (Decl)
			{
				Decl->setInvalidDecl();
				Decl = 0;
			}
		}

		auto Diag = Diagnostic();
		Diag.Location = Info.getLocation();
		Diag.Level = Level;
		Info.FormatDiagnostic(Diag.Message);
		Diagnostics.push_back(Diag);
	}

	std::vector<Diagnostic> Diagnostics;
	clang::Decl* Decl;
};

IClassTemplateSpecializationPtr Parser::WalkClassTemplateSpecialization(const clang::ClassTemplateSpecializationDecl* CTS)
{
	using namespace clang;

	auto CT = WalkClassTemplate(CTS->getSpecializedTemplate());
	auto USR = GetDeclUSR(CTS);
	auto TS = CT->FindSpecialization(USR.c_str());
	if (TS != nullptr)
		return TS;

	TS = pComClang->CreateClassTemplateSpecialization();
	IDeclarationPtr TSd = IDeclarationPtr(TS);
	HandleDeclaration(CTS, TSd);

	auto NS = GetNamespace(CTS);
	assert(NS && "Expected a valid namespace");
	TSd->Namespace = NS;
	TSd->name = CTS->getName().str().c_str();
	TS->TemplatedDecl = CT;
	TS->SpecializationKind = WalkTemplateSpecializationKind(CTS->getSpecializationKind());
	CT->AddSpecialization(TS);

	auto& TAL = CTS->getTemplateArgs();
	auto TSI = CTS->getTypeAsWritten();
	if (TSI)
	{
		auto TL = TSI->getTypeLoc();
		auto TSL = TL.getAs<TemplateSpecializationTypeLoc>();
		for (auto i : WalkTemplateArgumentList(&TAL, &TSL)) {
			TS->AddArguments(i);
		}
	}
	else
	{
		for (auto i : WalkTemplateArgumentList(&TAL, (clang::TemplateSpecializationTypeLoc*) 0)) {
			TS->AddArguments(i);
		}
	}

	if (CTS->isCompleteDefinition())
	{
		IClassPtr TSc = IClassPtr(TS);
		WalkRecordCXX(CTS, TSc);
	}
	else
	{
		TSd->IsIncomplete = true;
		if (CTS->getDefinition())
		{
			auto Complete = WalkDeclarationDef(CTS->getDefinition());
			if (!Complete->IsIncomplete)
				TSd->CompleteDeclaration = Complete;
		}
	}

	return TS;
}

//-----------------------------------//

IClassTemplatePartialSpecializationPtr Parser::WalkClassTemplatePartialSpecialization(const clang::ClassTemplatePartialSpecializationDecl* CTS)
{
	using namespace clang;

	auto CT = WalkClassTemplate(CTS->getSpecializedTemplate());
	auto USR = GetDeclUSR(CTS);
	auto TS = CT->FindPartialSpecialization(USR.c_str());
	if (TS != nullptr)
		return TS;

	TS = pComClang->CreateClassTemplatePartialSpecialization();
	IDeclarationPtr TSd = IDeclarationPtr(TS);
	HandleDeclaration(CTS, TSd);

	auto NS = GetNamespace(CTS);
	assert(NS && "Expected a valid namespace");
	TSd->Namespace = NS;
	TSd->name = CTS->getName().str().c_str();
	IClassTemplateSpecializationPtr TScts = IClassTemplateSpecializationPtr(TS);
	TScts->TemplatedDecl = CT;
	TScts->SpecializationKind = WalkTemplateSpecializationKind(CTS->getSpecializationKind());
	CT->AddSpecialization(TScts);

	auto& TAL = CTS->getTemplateArgs();
	if (auto TSI = CTS->getTypeAsWritten())
	{
		auto TL = TSI->getTypeLoc();
		auto TSL = TL.getAs<TemplateSpecializationTypeLoc>();
		for (auto i : WalkTemplateArgumentList(&TAL, &TSL)) {
			TScts->AddArguments(i);
		}
	}

	if (CTS->isCompleteDefinition())
	{
		WalkRecordCXX(CTS, IClassPtr(TS));
	}
	else
	{
		TSd->IsIncomplete = true;
		if (CTS->getDefinition())
		{
			auto Complete = WalkDeclarationDef(CTS->getDefinition());
			if (!Complete->IsIncomplete)
				TSd->CompleteDeclaration = Complete;
		}
	}

	return TS;
}

//-----------------------------------//

std::vector<IDeclaration *> Parser::WalkTemplateParameterList(const clang::TemplateParameterList* TPL)
{
	auto params = std::vector<IDeclaration *>();

	for (auto it = TPL->begin(); it != TPL->end(); ++it)
	{
		auto ND = *it;
		auto TP = WalkDeclaration(ND);
		params.push_back(TP);
	}

	return params;
}

//-----------------------------------//

IClassTemplatePtr Parser::WalkClassTemplate(const clang::ClassTemplateDecl* TD)
{
	auto NS = GetNamespace(TD);
	assert(NS && "Expected a valid namespace");

	auto USR = GetDeclUSR(TD);
	auto T = NS->FindTemplate(USR.c_str());
	IClassTemplatePtr CT = IClassTemplatePtr(T);
	if (CT != nullptr)
		return CT;

	CT = pComClang->CreateClassTemplate();
	IDeclarationPtr CTd = IDeclarationPtr(CT);
	HandleDeclaration(TD, CTd);

	CTd->name = GetDeclName(TD).c_str();
	CTd->Namespace = NS;
	NS->AddTemplate(T);

	bool Process;
	auto RC = GetRecord(TD->getTemplatedDecl(), Process);
	T->TemplatedDecl = IDeclarationPtr(RC);

	if (Process)
		WalkRecordCXX(TD->getTemplatedDecl(), RC);

	for (auto i : WalkTemplateParameterList(TD->getTemplateParameters())) {
		T->AddParameter(i);
	}

	return CT;
}

//-----------------------------------//

ITemplateTemplateParameterPtr Parser::WalkTemplateTemplateParameter(const clang::TemplateTemplateParmDecl* TTP)
{
	auto TP = walkedTemplateTemplateParameters[TTP];
	if (TP)
		return TP;

	TP = pComClang->CreateTemplateTemplateParameter();
	ITemplatePtr T = ITemplatePtr(TP);
	HandleDeclaration(TTP, IDeclarationPtr(TP));
	for (auto i : WalkTemplateParameterList(TTP->getTemplateParameters())) {
		T->AddParameter(i);
	}
	TP->IsParameterPack = TTP->isParameterPack();
	TP->IsPackExpansion = TTP->isPackExpansion();
	TP->IsExpandedParameterPack = TTP->isExpandedParameterPack();
	if (TTP->getTemplatedDecl())
	{
		auto TD = WalkDeclaration(TTP->getTemplatedDecl());
		T->TemplatedDecl = TD;
	}
	walkedTemplateTemplateParameters[TTP] = TP;
	return TP;
}

//-----------------------------------//

ITypeTemplateParameterPtr Parser::WalkTypeTemplateParameter(const clang::TemplateTypeParmDecl* TTPD)
{
	auto TP = walkedTypeTemplateParameters[TTPD];
	if (TP)
		return TP;

	TP = pComClang->CreateTypeTemplateParameter();
	IDeclarationPtr TPd = IDeclarationPtr(TP);
	ITemplateParameterPtr TPtp = ITemplateParameterPtr(TP);
	TPd->name = GetDeclName(TTPD).c_str();
	HandleDeclaration(TTPD, TPd);
	if (TTPD->hasDefaultArgument())
		TP->DefaultArgument = GetQualifiedType(TTPD->getDefaultArgument());
	TPtp->Depth = TTPD->getDepth();
	TPtp->index = TTPD->getIndex();
	TPtp->IsParameterPack = TTPD->isParameterPack();
	walkedTypeTemplateParameters[TTPD] = TP;
	return TP;
}

//-----------------------------------//

INonTypeTemplateParameterPtr Parser::WalkNonTypeTemplateParameter(const clang::NonTypeTemplateParmDecl* NTTPD)
{
	auto NTP = walkedNonTypeTemplateParameters[NTTPD];
	if (NTP)
		return NTP;

	NTP = pComClang->CreateNonTypeTemplateParameter();
	IDeclarationPtr NTPd = IDeclarationPtr(NTP);
	ITemplateParameterPtr NTPtp = ITemplateParameterPtr(NTP);
	NTPd->name = GetDeclName(NTTPD).c_str();
	HandleDeclaration(NTTPD, NTPd);
	if (NTTPD->hasDefaultArgument())
		NTP->DefaultArgument = WalkExpression(NTTPD->getDefaultArgument());
	NTPtp->Depth = NTTPD->getDepth();
	NTPtp->index = NTTPD->getIndex();
	NTPtp->IsParameterPack = NTTPD->isParameterPack();
	walkedNonTypeTemplateParameters[NTTPD] = NTP;
	return NTP;
}

//-----------------------------------//

template<typename TypeLoc>
std::vector<ITemplateArgument*>
Parser::WalkTemplateArgumentList(const clang::TemplateArgumentList* TAL,
	TypeLoc* TSTL)
{
	using namespace clang;

	auto LocValid = TSTL && !TSTL->isNull() && TSTL->getTypePtr();

	auto params = std::vector<ITemplateArgument*>();
	auto typeLocNumArgs = LocValid ? TSTL->getNumArgs() : 0;

	for (size_t i = 0, e = TAL->size(); i < e; i++)
	{
		auto TA = TAL->get(i);
		TemplateArgumentLoc TArgLoc;
		TemplateArgumentLoc *ArgLoc = 0;
		if (i < typeLocNumArgs && e == typeLocNumArgs)
		{
			TArgLoc = TSTL->getArgLoc(i);
			ArgLoc = &TArgLoc;
		}
		auto Arg = WalkTemplateArgument(TA, ArgLoc);
		params.push_back(Arg);
	}

	return params;
}

//-----------------------------------//

std::vector<ITemplateArgument*>
Parser::WalkTemplateArgumentList(const clang::TemplateArgumentList* TAL,
	const clang::ASTTemplateArgumentListInfo* TALI)
{
	using namespace clang;

	auto params = std::vector<ITemplateArgument*>();

	for (size_t i = 0, e = TAL->size(); i < e; i++)
	{
		auto TA = TAL->get(i);
		if (TALI)
		{
			auto ArgLoc = TALI->operator[](i);
			auto TP = WalkTemplateArgument(TA, &ArgLoc);
			params.push_back(TP);
		}
		else
		{
			auto TP = WalkTemplateArgument(TA, 0);
			params.push_back(TP);
		}
	}

	return params;
}

//-----------------------------------//

ITemplateArgumentPtr Parser::WalkTemplateArgument(const clang::TemplateArgument& TA, clang::TemplateArgumentLoc* ArgLoc)
{
	ITemplateArgumentPtr Arg = pComClang->CreateTemplateArgument();

	switch (TA.getKind())
	{
	case clang::TemplateArgument::Type:
	{
		Arg->Kind = ArgumentKind_Type;
		clang::TypeLoc ArgTL;
		if (ArgLoc && ArgLoc->getTypeSourceInfo())
		{
			ArgTL = ArgLoc->getTypeSourceInfo()->getTypeLoc();
		}
		auto Type = TA.getAsType();
		CompleteIfSpecializationType(Type);
		Arg->Type = GetQualifiedType(Type, &ArgTL);
		break;
	}
	case clang::TemplateArgument::Declaration:
		Arg->Kind = ArgumentKind_Declaration;
		Arg->Declaration = WalkDeclaration(TA.getAsDecl());
		break;
	case clang::TemplateArgument::NullPtr:
		Arg->Kind = ArgumentKind_NullPtr;
		break;
	case clang::TemplateArgument::Integral:
		Arg->Kind = ArgumentKind_Integral;
		//Arg->Type = WalkType(TA.getIntegralType(), 0);
		Arg->Integral = TA.getAsIntegral().getLimitedValue();
		break;
	case clang::TemplateArgument::Template:
		Arg->Kind = ArgumentKind_Template;
		break;
	case clang::TemplateArgument::TemplateExpansion:
		Arg->Kind = ArgumentKind_TemplateExpansion;
		break;
	case clang::TemplateArgument::Expression:
		Arg->Kind = ArgumentKind_Expression;
		break;
	case clang::TemplateArgument::Pack:
		Arg->Kind = ArgumentKind_Pack;
		break;
	case clang::TemplateArgument::Null:
	default:
		llvm_unreachable("Unknown TemplateArgument");
	}

	return Arg;
}

//-----------------------------------//

ITypeAliasTemplatePtr Parser::WalkTypeAliasTemplate(
	const clang::TypeAliasTemplateDecl* TD)
{
	using namespace clang;

	auto NS = GetNamespace(TD);
	assert(NS && "Expected a valid namespace");

	auto USR = GetDeclUSR(TD);
	auto T = NS->FindTemplate(USR.c_str());
	ITypeAliasTemplatePtr TA = ITypeAliasTemplatePtr(T);
	if (TA != nullptr)
		return TA;

	TA = pComClang->CreateTypeAliasTemplate();
	IDeclarationPtr TAd = IDeclarationPtr(TA);
	HandleDeclaration(TD, TAd);

	TAd->name = GetDeclName(TD).c_str();
	T->TemplatedDecl = WalkDeclaration(TD->getTemplatedDecl());
	for (auto i : WalkTemplateParameterList(TD->getTemplateParameters())) {
		T->AddParameter(i);
	}

	NS->AddTemplate(T);

	return TA;
}

//-----------------------------------//

IFunctionTemplatePtr Parser::WalkFunctionTemplate(const clang::FunctionTemplateDecl* TD)
{
	using namespace clang;

	auto NS = GetNamespace(TD);
	assert(NS && "Expected a valid namespace");

	auto USR = GetDeclUSR(TD);
	ITemplatePtr T = NS->FindTemplate(USR.c_str());
	IFunctionTemplatePtr FT = IFunctionTemplatePtr(T);
	if (FT != nullptr)
		return FT;

	IFunctionPtr Function = nullptr;
	auto TemplatedDecl = TD->getTemplatedDecl();

	if (auto MD = dyn_cast<CXXMethodDecl>(TemplatedDecl)) {
		Function = IFunctionPtr(WalkMethodCXX(MD));
	}
	else
		Function = WalkFunction(TemplatedDecl, /*IsDependent=*/true,
			/*AddToNamespace=*/false);

	FT = pComClang->CreateFunctionTemplate();
	IDeclarationPtr FTd = IDeclarationPtr(FT);
	T = ITemplatePtr(FT);
	HandleDeclaration(TD, FTd);

	FTd->name = GetDeclName(TD).c_str();
	FTd->Namespace = NS;
	IDeclarationPtr Functiond = IDeclarationPtr(Function);
	T->TemplatedDecl = Functiond;
	for (auto i : WalkTemplateParameterList(TD->getTemplateParameters())) {
		T->AddParameter(i);
	}

	NS->AddTemplate(T);

	return FT;
}

//-----------------------------------//

IFunctionTemplateSpecializationPtr Parser::WalkFunctionTemplateSpec(clang::FunctionTemplateSpecializationInfo* FTSI, IFunctionPtr Function)
{
	using namespace clang;

	IFunctionTemplateSpecializationPtr FTS = pComClang->CreateFunctionTemplateSpecialization();
	FTS->SpecializationKind = WalkTemplateSpecializationKind(FTSI->getTemplateSpecializationKind());
	FTS->SpecializedFunction = Function;
	FTS->Template = WalkFunctionTemplate(FTSI->getTemplate());
	FTS->Template->AddSpecialization(FTS);
	if (auto TSA = FTSI->TemplateArguments)
	{
		if (auto TSAW = FTSI->TemplateArgumentsAsWritten)
		{
			if (TSA->size() == TSAW->NumTemplateArgs)
			{
				for (auto i : WalkTemplateArgumentList(TSA, TSAW)) {
					FTS->AddArguments(i);
				}
				return FTS;
			}
		}
		for (auto i : WalkTemplateArgumentList(TSA,
			(const clang::ASTTemplateArgumentListInfo*) 0)) {
			FTS->AddArguments(i);
		}
	}

	return FTS;
}

//-----------------------------------//

IVarTemplatePtr Parser::WalkVarTemplate(const clang::VarTemplateDecl* TD)
{
	auto NS = GetNamespace(TD);
	assert(NS && "Expected a valid namespace");

	auto USR = GetDeclUSR(TD);
	auto T = NS->FindTemplate(USR.c_str());
	IVarTemplatePtr VT = IVarTemplatePtr(T);
	if (VT != nullptr)
		return VT;

	VT = pComClang->CreateVarTemplate();
	IDeclarationPtr VTd = IDeclarationPtr(VT);
	HandleDeclaration(TD, VTd);

	VTd->name = GetDeclName(TD).c_str();
	VTd->Namespace = NS;
	NS->AddTemplate(T);

	auto RC = WalkVariable(TD->getTemplatedDecl());
	T->TemplatedDecl = IDeclarationPtr(RC);

	for (auto i : WalkTemplateParameterList(TD->getTemplateParameters())) {
		T->AddParameter(i);
	}

	return VT;
}

IVarTemplateSpecializationPtr Parser::WalkVarTemplateSpecialization(const clang::VarTemplateSpecializationDecl* VTS)
{
	using namespace clang;

	auto VT = WalkVarTemplate(VTS->getSpecializedTemplate());
	auto USR = GetDeclUSR(VTS);
	auto TS = VT->FindSpecialization(USR.c_str());
	if (TS != nullptr)
		return TS;

	TS = pComClang->CreateVarTemplateSpecialization();
	IDeclarationPtr TSd = IDeclarationPtr(TS);
	HandleDeclaration(VTS, TSd);

	auto NS = GetNamespace(VTS);
	assert(NS && "Expected a valid namespace");
	TSd->Namespace = NS;
	TSd->name = VTS->getName().str().c_str();
	TS->TemplatedDecl = VT;
	TS->SpecializationKind = WalkTemplateSpecializationKind(VTS->getSpecializationKind());
	VT->AddSpecialization(TS);

	auto& TAL = VTS->getTemplateArgs();
	auto TSI = VTS->getTypeAsWritten();
	if (TSI)
	{
		auto TL = TSI->getTypeLoc();
		auto TSL = TL.getAs<TemplateSpecializationTypeLoc>();
		for (auto i : WalkTemplateArgumentList(&TAL, &TSL)) {
			TS->AddArguments(i);
		}
	}
	else
	{
		for (auto i : WalkTemplateArgumentList(&TAL, (clang::TemplateSpecializationTypeLoc*) 0)) {
			TS->AddArguments(i);
		}
	}

	IVariablePtr TSv = IVariablePtr(TS);
	WalkVariable(VTS, TSv);

	return TS;
}

IVarTemplatePartialSpecializationPtr Parser::WalkVarTemplatePartialSpecialization(const clang::VarTemplatePartialSpecializationDecl* VTS)
{
	using namespace clang;

	auto VT = WalkVarTemplate(VTS->getSpecializedTemplate());
	auto USR = GetDeclUSR(VTS);
	auto TS = VT->FindPartialSpecialization(USR.c_str());
	if (TS != nullptr)
		return TS;

	TS = pComClang->CreateVarTemplatePartialSpecialization();
	IDeclarationPtr TSd = IDeclarationPtr(TS);
	IVarTemplateSpecializationPtr TSv = IVarTemplateSpecializationPtr(TS);
	HandleDeclaration(VTS, TSd);

	auto NS = GetNamespace(VTS);
	assert(NS && "Expected a valid namespace");
	TSd->Namespace = NS;
	TSd->name = VTS->getName().str().c_str();
	TSv->TemplatedDecl = VT;
	TSv->SpecializationKind = WalkTemplateSpecializationKind(VTS->getSpecializationKind());
	VT->AddSpecialization(TSv);

	auto& TAL = VTS->getTemplateArgs();
	if (auto TSI = VTS->getTypeAsWritten())
	{
		auto TL = TSI->getTypeLoc();
		auto TSL = TL.getAs<TemplateSpecializationTypeLoc>();
		for (auto i : WalkTemplateArgumentList(&TAL, &TSL)) {
			TSv->AddArguments(i);
		}
	}

	IVariablePtr TSvar = IVariablePtr(TS);
	WalkVariable(VTS, TSvar);

	return TS;
}

//-----------------------------------//

static CXXMethodKind GetMethodKindFromDecl(clang::DeclarationName Name)
{
	using namespace clang;

	switch (Name.getNameKind())
	{
	case DeclarationName::Identifier:
	case DeclarationName::CXXDeductionGuideName:
	case DeclarationName::ObjCZeroArgSelector:
	case DeclarationName::ObjCOneArgSelector:
	case DeclarationName::ObjCMultiArgSelector:
		return CXXMethodKind_Normal;
	case DeclarationName::CXXConstructorName:
		return CXXMethodKind_Constructor;
	case DeclarationName::CXXDestructorName:
		return CXXMethodKind_Destructor;
	case DeclarationName::CXXConversionFunctionName:
		return CXXMethodKind_Conversion;
	case DeclarationName::CXXOperatorName:
	case DeclarationName::CXXLiteralOperatorName:
		return CXXMethodKind_Operator;
	case DeclarationName::CXXUsingDirective:
		return CXXMethodKind_UsingDirective;
	}
	return CXXMethodKind_Normal;
}

static CXXOperatorKind GetOperatorKindFromDecl(clang::DeclarationName Name)
{
	using namespace clang;

	if (Name.getNameKind() != DeclarationName::CXXOperatorName)
		return CXXOperatorKind_None;

	switch (Name.getCXXOverloadedOperator())
	{
	case OO_None:
		return CXXOperatorKind_None;
	case NUM_OVERLOADED_OPERATORS:
		break;

#define OVERLOADED_OPERATOR(Name,Spelling,Token,Unary,Binary,MemberOnly) \
    case OO_##Name: return CXXOperatorKind_##Name;
#include "clang/Basic/OperatorKinds.def"
	}

	llvm_unreachable("Unknown OverloadedOperator");
}

IMethodPtr Parser::WalkMethodCXX(const clang::CXXMethodDecl* MD)
{
	using namespace clang;

	// We could be in a redeclaration, so process the primary context.
	if (MD->getPrimaryContext() != MD)
		return WalkMethodCXX(cast<CXXMethodDecl>(MD->getPrimaryContext()));

	auto RD = MD->getParent();
	auto Decl = WalkDeclaration(RD);

	IClassPtr Class = IClassPtr(Decl);

	// Check for an already existing method that came from the same declaration.
	auto USR = GetDeclUSR(MD);
	for (int i = 0; i < Class->MethodCount; i++)
	{
		auto M = Class->GetMethod(i);
		IDeclarationPtr Md = IDeclarationPtr(M);
		if (std::string(Md->USR) == USR)
		{
			return M;
		}
	}
	IDeclarationContextPtr dc = IDeclarationContextPtr(Class);
	for (unsigned I = 0, E = dc->TemplateCount; I != E; ++I)
	{
		ITemplatePtr Template = dc->GetTemplate(I);
		if (std::string(Template->TemplatedDecl->USR) == USR) {
			return IMethodPtr(Template->TemplatedDecl);
		}
	}

	IMethodPtr Method = pComClang->CreateMethod();
	IDeclarationPtr Methodd = IDeclarationPtr(Method);
	HandleDeclaration(MD, Methodd);

	Methodd->Access = ConvertToAccess(MD->getAccess());
	Method->MethodKind = GetMethodKindFromDecl(MD->getDeclName());
	Method->IsStatic = MD->isStatic();
	Method->IsVirtual = MD->isVirtual();
	Method->IsConst = MD->isConst();
	for (auto OverriddenMethod : MD->overridden_methods())
	{
		auto OM = WalkMethodCXX(OverriddenMethod);
		Method->AddOverriddenMethod(OM);
	}
	switch (MD->getRefQualifier())
	{
	case clang::RefQualifierKind::RQ_None:
		Method->_RefQualifier = RefQualifier_None;
		break;
	case clang::RefQualifierKind::RQ_LValue:
		Method->_RefQualifier = RefQualifier_LValue;
		break;
	case clang::RefQualifierKind::RQ_RValue:
		Method->_RefQualifier = RefQualifier_RValue;
		break;
	}

	Class->AddMethod(Method);

	WalkFunction(MD, IFunctionPtr(Method));

	if (const CXXConstructorDecl* CD = dyn_cast<CXXConstructorDecl>(MD))
	{
		Method->IsDefaultConstructor = CD->isDefaultConstructor();
		Method->IsCopyConstructor = CD->isCopyConstructor();
		Method->IsMoveConstructor = CD->isMoveConstructor();
		Method->IsExplicit = CD->isExplicit();
	}
	else if (const CXXDestructorDecl* DD = dyn_cast<CXXDestructorDecl>(MD))
	{
	}
	else if (const CXXConversionDecl* CD = dyn_cast<CXXConversionDecl>(MD))
	{
		auto TL = MD->getTypeSourceInfo()->getTypeLoc().castAs<FunctionTypeLoc>();
		auto RTL = TL.getReturnLoc();
		Method->ConversionType = GetQualifiedType(CD->getConversionType(), &RTL);
	}

	return Method;
}

//-----------------------------------//

IFieldPtr Parser::WalkFieldCXX(const clang::FieldDecl* FD, IClassPtr Class)
{
	using namespace clang;

	const auto& USR = GetDeclUSR(FD);

	IFieldPtr FoundField = nullptr;
	for (int i = 0; i < Class->FieldCount; i++) {
		IFieldPtr Field = Class->GetField(i);
		IDeclarationPtr Fieldd = IDeclarationPtr(Field);
		if (std::string(Fieldd->USR) == USR) {
			FoundField = Field;
			break;
		}
	}

	if (FoundField != nullptr)
		return FoundField;

	IFieldPtr F = pComClang->CreateField();
	IDeclarationPtr Fd = IDeclarationPtr(F);
	HandleDeclaration(FD, Fd);

	IDeclarationContextPtr Classd = IDeclarationContextPtr(Class);
	Fd->Namespace = Classd;
	Fd->name = FD->getName().str().c_str();
	auto TL = FD->getTypeSourceInfo()->getTypeLoc();
	F->QualifiedType = GetQualifiedType(FD->getType(), &TL);
	Fd->Access = ConvertToAccess(FD->getAccess());
	F->Class = Class;
	F->IsBitField = FD->isBitField();
	if (F->IsBitField && !Fd->IsDependent && !FD->getBitWidth()->isInstantiationDependent())
		F->BitWidth = FD->getBitWidthValue(c->getASTContext());

	Class->AddField(F);

	return F;
}

//-----------------------------------//

ITranslationUnitPtr Parser::GetTranslationUnit(clang::SourceLocation Loc,
	SourceLocationKind *Kind)
{
	using namespace clang;

	clang::SourceManager& SM = c->getSourceManager();

	if (Loc.isMacroID())
		Loc = SM.getExpansionLoc(Loc);

	StringRef File;

	auto LocKind = GetLocationKind(Loc);
	switch (LocKind)
	{
	case SourceLocationKind_Invalid:
		File = "<invalid>";
		break;
	case SourceLocationKind_Builtin:
		File = "<built-in>";
		break;
	case SourceLocationKind_CommandLine:
		File = "<command-line>";
		break;
	default:
		File = SM.getFilename(Loc);
		assert(!File.empty() && "Expected to find a valid file");
		break;
	}

	if (Kind)
		*Kind = LocKind;

	auto Unit = opts->ASTContext->FindOrCreateModule(File.str().c_str());
	IDeclarationPtr Unitd = IDeclarationPtr(Unit);

	Unitd->OriginalPtr = (intptr_t)Unit;
	assert(Unitd->OriginalPtr != 0);

	if (LocKind != SourceLocationKind_Invalid)
		Unit->IsSystemHeader = SM.isInSystemHeader(Loc);

	return Unit;
}

//-----------------------------------//

ITranslationUnitPtr Parser::GetTranslationUnit(const clang::Decl* D)
{
	clang::SourceLocation Loc = D->getLocation();

	SourceLocationKind Kind;
	ITranslationUnitPtr Unit = GetTranslationUnit(Loc, &Kind);

	return Unit;
}

IDeclarationContextPtr Parser::GetNamespace(const clang::Decl* D,
	const clang::DeclContext *Ctx)
{
	using namespace clang;

	auto Context = Ctx;

	// If the declaration is at global scope, just early exit.
	if (Context->isTranslationUnit()) {
		return IDeclarationContextPtr(GetTranslationUnit(D));
	}

	ITranslationUnitPtr Unit = GetTranslationUnit(cast<Decl>(Context));

	// Else we need to do a more expensive check to get all the namespaces,
	// and then perform a reverse iteration to get the namespaces in order.
	typedef SmallVector<const DeclContext *, 8> ContextsTy;
	ContextsTy Contexts;

	for (; Context != nullptr; Context = Context->getParent())
		Contexts.push_back(Context);

	assert(Contexts.back()->isTranslationUnit());
	Contexts.pop_back();

	IDeclarationContextPtr DC = IDeclarationContextPtr(Unit);

	for (auto I = Contexts.rbegin(), E = Contexts.rend(); I != E; ++I)
	{
		const auto* Ctx = *I;

		switch (Ctx->getDeclKind())
		{
		case Decl::Namespace:
		{
			auto ND = cast<NamespaceDecl>(Ctx);
			if (ND->isAnonymousNamespace())
				continue;
			auto Name = ND->getName();
			auto DCns = DC->FindCreateNamespace(Name.str().c_str());
			IDeclarationContextPtr DCnsdc = IDeclarationContextPtr(DCns);
			DCnsdc->IsAnonymous = ND->isAnonymousNamespace();
			DCns->IsInline = ND->isInline();
			IDeclarationPtr DCd = IDeclarationPtr(DC);
			HandleDeclaration(ND, DCd);
			continue;
		}
		case Decl::LinkageSpec:
		{
			const LinkageSpecDecl* LD = cast<LinkageSpecDecl>(Ctx);
			continue;
		}
		case Decl::CXXRecord:
		{
			auto RD = cast<CXXRecordDecl>(Ctx);
			DC = IDeclarationContextPtr(WalkRecordCXX(RD));
			continue;
		}
		default:
		{
			auto D = cast<Decl>(Ctx);
			auto Decl = WalkDeclaration(D);
			DC = IDeclarationContextPtr(Decl);
		}
		}
	}

	return DC;
}

IDeclarationContextPtr Parser::GetNamespace(const clang::Decl *D)
{
	return GetNamespace(D, D->getDeclContext());
}

static PrimitiveType WalkBuiltinType(const clang::BuiltinType* Builtin)
{
	assert(Builtin && "Expected a builtin type");

	switch (Builtin->getKind())
	{
	case clang::BuiltinType::Void: return PrimitiveType_Void;
	case clang::BuiltinType::Bool: return PrimitiveType_Bool;

	case clang::BuiltinType::SChar: return PrimitiveType_SChar;
	case clang::BuiltinType::Char_S: return PrimitiveType_Char;

	case clang::BuiltinType::UChar:
	case clang::BuiltinType::Char_U: return PrimitiveType_UChar;

	case clang::BuiltinType::WChar_S:
	case clang::BuiltinType::WChar_U: return PrimitiveType_WideChar;

	case clang::BuiltinType::Char16: return PrimitiveType_Char16;
	case clang::BuiltinType::Char32: return PrimitiveType_Char32;

	case clang::BuiltinType::Short: return PrimitiveType_Short;
	case clang::BuiltinType::UShort: return PrimitiveType_UShort;

	case clang::BuiltinType::Int: return PrimitiveType_Int;
	case clang::BuiltinType::UInt: return PrimitiveType_UInt;

	case clang::BuiltinType::Long: return PrimitiveType_Long;
	case clang::BuiltinType::ULong: return PrimitiveType_ULong;

	case clang::BuiltinType::LongLong: return PrimitiveType_LongLong;
	case clang::BuiltinType::ULongLong: return PrimitiveType_ULongLong;

	case clang::BuiltinType::Int128: return PrimitiveType_Int128;
	case clang::BuiltinType::UInt128: return PrimitiveType_UInt128;

	case clang::BuiltinType::Half: return PrimitiveType_Half;
	case clang::BuiltinType::Float: return PrimitiveType_Float;
	case clang::BuiltinType::Double: return PrimitiveType_Double;
	case clang::BuiltinType::LongDouble: return PrimitiveType_LongDouble;
	case clang::BuiltinType::Float128: return PrimitiveType_Float128;

	case clang::BuiltinType::NullPtr: return PrimitiveType_Null;

	default: break;
	}

	return PrimitiveType_Null;
}

//-----------------------------------//

clang::TypeLoc ResolveTypeLoc(clang::TypeLoc TL, clang::TypeLoc::TypeLocClass Class)
{
	using namespace clang;

	auto TypeLocClass = TL.getTypeLocClass();

	if (TypeLocClass == Class)
	{
		return TL;
	}
	if (TypeLocClass == TypeLoc::Qualified)
	{
		auto UTL = TL.getUnqualifiedLoc();
		TL = UTL;
	}
	else if (TypeLocClass == TypeLoc::Elaborated)
	{
		auto ETL = TL.getAs<ElaboratedTypeLoc>();
		auto ITL = ETL.getNextTypeLoc();
		TL = ITL;
	}
	else if (TypeLocClass == TypeLoc::Paren)
	{
		auto PTL = TL.getAs<ParenTypeLoc>();
		TL = PTL.getNextTypeLoc();
	}

	assert(TL.getTypeLocClass() == Class);
	return TL;
}

static FriendKind ConvertFriendKind(clang::Decl::FriendObjectKind FK)
{
	using namespace clang;

	switch (FK)
	{
	case Decl::FriendObjectKind::FOK_Declared:
		return FriendKind_Declared;
	case Decl::FriendObjectKind::FOK_Undeclared:
		return FriendKind_Undeclared;
	default:
		return FriendKind_None;
	}
}

static CallingConvention ConvertCallConv(clang::CallingConv CC)
{
	using namespace clang;

	switch (CC)
	{
	case CC_C:
		return CallingConvention_C;
	case CC_X86StdCall:
		return CallingConvention_StdCall;
	case CC_X86FastCall:
		return CallingConvention_FastCall;
	case CC_X86ThisCall:
		return CallingConvention_ThisCall;
	default:
		return CallingConvention_Unknown;
	}
}

static ExceptionSpecType ConvertExceptionType(clang::ExceptionSpecificationType EST)
{
	using namespace clang;

	switch (EST)
	{
	case ExceptionSpecificationType::EST_BasicNoexcept:
		return ExceptionSpecType_BasicNoexcept;
	case ExceptionSpecificationType::EST_ComputedNoexcept:
		return ExceptionSpecType_ComputedNoexcept;
	case ExceptionSpecificationType::EST_Dynamic:
		return ExceptionSpecType_Dynamic;
	case ExceptionSpecificationType::EST_DynamicNone:
		return ExceptionSpecType_DynamicNone;
	case ExceptionSpecificationType::EST_MSAny:
		return ExceptionSpecType_MSAny;
	case ExceptionSpecificationType::EST_Unevaluated:
		return ExceptionSpecType_Unevaluated;
	case ExceptionSpecificationType::EST_Uninstantiated:
		return ExceptionSpecType_Uninstantiated;
	case ExceptionSpecificationType::EST_Unparsed:
		return ExceptionSpecType_Unparsed;
	default:
		return ExceptionSpecType_None;
	}
}

static ParserIntType ConvertIntType(clang::TargetInfo::IntType IT)
{
	switch (IT)
	{
	case clang::TargetInfo::IntType::NoInt:
		return ParserIntType_NoInt;
	case clang::TargetInfo::IntType::SignedChar:
		return ParserIntType_SignedChar;
	case clang::TargetInfo::IntType::UnsignedChar:
		return ParserIntType_UnsignedChar;
	case clang::TargetInfo::IntType::SignedShort:
		return ParserIntType_SignedShort;
	case clang::TargetInfo::IntType::UnsignedShort:
		return ParserIntType_UnsignedShort;
	case clang::TargetInfo::IntType::SignedInt:
		return ParserIntType_SignedInt;
	case clang::TargetInfo::IntType::UnsignedInt:
		return ParserIntType_UnsignedInt;
	case clang::TargetInfo::IntType::SignedLong:
		return ParserIntType_SignedLong;
	case clang::TargetInfo::IntType::UnsignedLong:
		return ParserIntType_UnsignedLong;
	case clang::TargetInfo::IntType::SignedLongLong:
		return ParserIntType_SignedLongLong;
	case clang::TargetInfo::IntType::UnsignedLongLong:
		return ParserIntType_UnsignedLongLong;
	}

	llvm_unreachable("Unknown parser integer type");
}

static const clang::Type* GetFinalType(const clang::Type* Ty)
{
	auto FinalType = Ty;
	while (true)
	{
		FinalType = FinalType->getUnqualifiedDesugaredType();
		if (FinalType->getPointeeType().isNull())
			return FinalType;
		FinalType = FinalType->getPointeeType().getTypePtr();
	}
}

ITypePtr Parser::WalkType(clang::QualType QualType, const clang::TypeLoc* TL,
	bool DesugarType)
{
	using namespace clang;

	if (QualType.isNull())
		return nullptr;

	auto LocValid = TL && !TL->isNull();

	const clang::Type* Type = QualType.getTypePtr();

	auto& AST = c->getASTContext();
	if (DesugarType)
	{
		clang::QualType Desugared = QualType.getDesugaredType(AST);
		assert(!Desugared.isNull() && "Expected a valid desugared type");
		Type = Desugared.getTypePtr();
	}

	ITypePtr Ty = nullptr;

	assert(Type && "Expected a valid type");
	switch (Type->getTypeClass())
	{
	case clang::Type::Atomic:
	{
		auto Atomic = Type->getAs<clang::AtomicType>();
		assert(Atomic && "Expected an atomic type");

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		Ty = WalkType(Atomic->getValueType(), &Next);
		break;
	}
	case clang::Type::Attributed:
	{
		auto Attributed = Type->getAs<clang::AttributedType>();
		assert(Attributed && "Expected an attributed type");

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		IAttributedTypePtr AT = pComClang->CreateAttributedType();

		auto Modified = Attributed->getModifiedType();
		AT->Modified = GetQualifiedType(Modified, &Next);

		auto Equivalent = Attributed->getEquivalentType();
		AT->Equivalent = GetQualifiedType(Equivalent, &Next);

		Ty = ITypePtr(AT);
		break;
	}
	case clang::Type::Builtin:
	{
		auto Builtin = Type->getAs<clang::BuiltinType>();
		assert(Builtin && "Expected a builtin type");

		IBuiltinTypePtr BT = pComClang->CreateBuiltinType();
		BT->Type = WalkBuiltinType(Builtin);

		Ty = ITypePtr(BT);
		break;
	}
	case clang::Type::Enum:
	{
		auto ET = Type->getAs<clang::EnumType>();
		EnumDecl* ED = ET->getDecl();

		ITagTypePtr TT = pComClang->CreateTagType();
		//TT->Declaration = TT->Declaration = WalkDeclaration(ED);
		TT->Declaration = WalkDeclaration(ED);

		Ty = ITypePtr(TT);
		break;
	}
	case clang::Type::Pointer:
	{
		auto Pointer = Type->getAs<clang::PointerType>();

		IPointerTypePtr P = pComClang->CreatePointerType();
		P->Modifier = TypeModifier_Pointer;

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		auto Pointee = Pointer->getPointeeType();
		P->QualifiedPointee = GetQualifiedType(Pointee, &Next);

		Ty = ITypePtr(P);
		break;
	}
	case clang::Type::Typedef:
	{
		auto TT = Type->getAs<clang::TypedefType>();
		auto TD = TT->getDecl();

		auto TTL = TD->getTypeSourceInfo()->getTypeLoc();
		ITypedefNameDeclPtr TDD = ITypedefNameDeclPtr(WalkDeclaration(TD));

		ITypedefTypePtr Type = pComClang->CreateTypedefType();
		Type->Declaration = TDD;

		Ty = ITypePtr(Type);
		break;
	}
	case clang::Type::Decayed:
	{
		auto DT = Type->getAs<clang::DecayedType>();

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		IDecayedTypePtr Type = pComClang->CreateDecayedType();
		Type->Decayed = GetQualifiedType(DT->getDecayedType(), &Next);
		Type->Original = GetQualifiedType(DT->getOriginalType(), &Next);
		Type->Pointee = GetQualifiedType(DT->getPointeeType(), &Next);

		Ty = ITypePtr(Type);;
		break;
	}
	case clang::Type::Elaborated:
	{
		auto ET = Type->getAs<clang::ElaboratedType>();

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		Ty = WalkType(ET->getNamedType(), &Next);
		break;
	}
	case clang::Type::Record:
	{
		auto RT = Type->getAs<clang::RecordType>();
		RecordDecl* RD = RT->getDecl();

		ITagTypePtr TT = pComClang->CreateTagType();
		TT->Declaration = WalkDeclaration(RD);

		Ty = ITypePtr(TT);;
		break;
	}
	case clang::Type::Paren:
	{
		auto PT = Type->getAs<clang::ParenType>();

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		Ty = WalkType(PT->getInnerType(), &Next);
		break;
	}
	case clang::Type::ConstantArray:
	{
		auto AT = AST.getAsConstantArrayType(QualType);

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		IArrayTypePtr A = pComClang->CreateArrayType();
		auto ElemTy = AT->getElementType();
		A->QualifiedType = GetQualifiedType(ElemTy, &Next);
		A->SizeType = ArraySize_Constant;
		A->Size = AST.getConstantArrayElementCount(AT);
		if (!ElemTy->isDependentType())
			A->ElementSize = (long)AST.getTypeSize(ElemTy);

		Ty = ITypePtr(A);
		break;
	}
	case clang::Type::IncompleteArray:
	{
		auto AT = AST.getAsIncompleteArrayType(QualType);

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		IArrayTypePtr A = pComClang->CreateArrayType();
		A->QualifiedType = GetQualifiedType(AT->getElementType(), &Next);
		A->SizeType = ArraySize_Incomplete;

		Ty = ITypePtr(A);
		break;
	}
	case clang::Type::DependentSizedArray:
	{
		auto AT = AST.getAsDependentSizedArrayType(QualType);

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		IArrayTypePtr A = pComClang->CreateArrayType();
		A->QualifiedType = GetQualifiedType(AT->getElementType(), &Next);
		A->SizeType = ArraySize_Dependent;
		//A->Size = AT->getSizeExpr();

		Ty = ITypePtr(A);
		break;
	}
	case clang::Type::FunctionNoProto:
	{
		auto FP = Type->getAs<clang::FunctionNoProtoType>();

		FunctionNoProtoTypeLoc FTL;
		TypeLoc RL;
		TypeLoc Next;
		if (LocValid)
		{
			while (!TL->isNull() && TL->getTypeLocClass() != TypeLoc::FunctionNoProto)
			{
				Next = TL->getNextTypeLoc();
				TL = &Next;
			}

			if (!TL->isNull() && TL->getTypeLocClass() == TypeLoc::FunctionNoProto)
			{
				FTL = TL->getAs<FunctionNoProtoTypeLoc>();
				RL = FTL.getReturnLoc();
			}
		}

		IFunctionTypePtr F = pComClang->CreateFunctionType();
		F->ReturnType = GetQualifiedType(FP->getReturnType(), &RL);
		F->_CallingConvention = ConvertCallConv(FP->getCallConv());

		Ty = ITypePtr(F);
		break;
	}
	case clang::Type::FunctionProto:
	{
		auto FP = Type->getAs<clang::FunctionProtoType>();

		FunctionProtoTypeLoc FTL;
		TypeLoc RL;
		TypeLoc Next;
		clang::SourceLocation ParamStartLoc;
		if (LocValid)
		{
			while (!TL->isNull() && TL->getTypeLocClass() != TypeLoc::FunctionProto)
			{
				Next = TL->getNextTypeLoc();
				TL = &Next;
			}

			if (!TL->isNull() && TL->getTypeLocClass() == TypeLoc::FunctionProto)
			{
				FTL = TL->getAs<FunctionProtoTypeLoc>();
				RL = FTL.getReturnLoc();
				ParamStartLoc = FTL.getLParenLoc();
			}
		}

		IFunctionTypePtr F = pComClang->CreateFunctionType();
		F->ReturnType = GetQualifiedType(FP->getReturnType(), &RL);
		F->_CallingConvention = ConvertCallConv(FP->getCallConv());
		F->_ExceptionSpecType = ConvertExceptionType(FP->getExceptionSpecType());

		for (unsigned i = 0; i < FP->getNumParams(); ++i)
		{
			if (FTL && FTL.getParam(i))
			{
				auto PVD = FTL.getParam(i);
				auto FA = WalkParameter(PVD, ParamStartLoc);
				F->AddParameter(FA);
			}
			else
			{
				IParameterPtr FA = pComClang->CreateParameter();
				auto Arg = FP->getParamType(i);
				IDeclarationPtr FAd = IDeclarationPtr(FA);
				FAd->name = "";
				FA->QualifiedType = GetQualifiedType(Arg);

				// In this case we have no valid value to use as a pointer so
				// use a special value known to the managed side to make sure
				// it gets ignored.
				FAd->OriginalPtr = (intptr_t)IgnorePtr;
				F->AddParameter(FA);
			}
		}

		Ty = ITypePtr(F);
		break;
	}
	case clang::Type::TypeOf:
	{
		auto TO = Type->getAs<clang::TypeOfType>();

		Ty = WalkType(TO->getUnderlyingType());
		break;
	}
	case clang::Type::TypeOfExpr:
	{
		auto TO = Type->getAs<clang::TypeOfExprType>();

		Ty = WalkType(TO->getUnderlyingExpr()->getType());
		break;
	}
	case clang::Type::MemberPointer:
	{
		auto MP = Type->getAs<clang::MemberPointerType>();

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		IMemberPointerTypePtr MPT = pComClang->CreateMemberPointerType();
		MPT->Pointee = GetQualifiedType(MP->getPointeeType(), &Next);

		Ty = ITypePtr(MPT);
		break;
	}
	case clang::Type::TemplateSpecialization:
	{
		auto TS = Type->getAs<clang::TemplateSpecializationType>();
		ITemplateSpecializationTypePtr TST = pComClang->CreateTemplateSpecializationType();

		TemplateName Name = TS->getTemplateName();
		TST->Template = ITemplatePtr(WalkDeclaration(
			Name.getAsTemplateDecl()));
		if (TS->isSugared())
			TST->Desugared = GetQualifiedType(TS->desugar(), TL);

		TypeLoc UTL, ETL, ITL;

		if (LocValid)
		{
			auto TypeLocClass = TL->getTypeLocClass();
			if (TypeLocClass == TypeLoc::Qualified)
			{
				UTL = TL->getUnqualifiedLoc();
				TL = &UTL;
			}
			else if (TypeLocClass == TypeLoc::Elaborated)
			{
				ETL = TL->getAs<ElaboratedTypeLoc>();
				ITL = ETL.getNextTypeLoc();
				TL = &ITL;
			}

			assert(TL->getTypeLocClass() == TypeLoc::TemplateSpecialization);
		}

		TemplateSpecializationTypeLoc TSpecTL;
		TemplateSpecializationTypeLoc *TSTL = 0;
		if (LocValid)
		{
			TSpecTL = TL->getAs<TemplateSpecializationTypeLoc>();
			TSTL = &TSpecTL;
		}

		ArrayRef<clang::TemplateArgument> TSArgs(TS->getArgs(), TS->getNumArgs());
		TemplateArgumentList TArgs(TemplateArgumentList::OnStack, TSArgs);
		for (auto i : WalkTemplateArgumentList(&TArgs, TSTL)) {
			TST->AddArguments(i);
		}

		Ty = ITypePtr(TST);
		break;
	}
	case clang::Type::DependentTemplateSpecialization:
	{
		auto TS = Type->getAs<clang::DependentTemplateSpecializationType>();
		IDependentTemplateSpecializationTypePtr TST = pComClang->CreateDependentTemplateSpecializationType();

		if (TS->isSugared())
			TST->Desugared = GetQualifiedType(TS->desugar(), TL);

		TypeLoc UTL, ETL, ITL;

		if (LocValid)
		{
			auto TypeLocClass = TL->getTypeLocClass();
			if (TypeLocClass == TypeLoc::Qualified)
			{
				UTL = TL->getUnqualifiedLoc();
				TL = &UTL;
			}
			else if (TypeLocClass == TypeLoc::Elaborated)
			{
				ETL = TL->getAs<ElaboratedTypeLoc>();
				ITL = ETL.getNextTypeLoc();
				TL = &ITL;
			}

			assert(TL->getTypeLocClass() == TypeLoc::DependentTemplateSpecialization);
		}

		DependentTemplateSpecializationTypeLoc TSpecTL;
		DependentTemplateSpecializationTypeLoc *TSTL = 0;
		if (LocValid)
		{
			TSpecTL = TL->getAs<DependentTemplateSpecializationTypeLoc>();
			TSTL = &TSpecTL;
		}

		ArrayRef<clang::TemplateArgument> TSArgs(TS->getArgs(), TS->getNumArgs());
		TemplateArgumentList TArgs(TemplateArgumentList::OnStack, TSArgs);
		for (auto i : WalkTemplateArgumentList(&TArgs, TSTL)) {
			TST->AddArguments(i);
		}

		Ty = ITypePtr(TST);
		break;
	}
	case clang::Type::TemplateTypeParm:
	{
		auto TP = Type->getAs<TemplateTypeParmType>();

		ITemplateParameterTypePtr TPT = pComClang->CreateTemplateParameterType();

		if (auto Ident = TP->getIdentifier()) {
			IDeclarationPtr pd = IDeclarationPtr(TPT->parameter);
			pd->name = Ident->getName().str().c_str();
		}

		TypeLoc UTL, ETL, ITL, Next;

		if (LocValid)
		{
			auto TypeLocClass = TL->getTypeLocClass();
			if (TypeLocClass == TypeLoc::Qualified)
			{
				UTL = TL->getUnqualifiedLoc();
				TL = &UTL;
			}
			else if (TypeLocClass == TypeLoc::Elaborated)
			{
				ETL = TL->getAs<ElaboratedTypeLoc>();
				ITL = ETL.getNextTypeLoc();
				TL = &ITL;
			}

			while (TL->getTypeLocClass() != TypeLoc::TemplateTypeParm)
			{
				Next = TL->getNextTypeLoc();
				TL = &Next;
			}

			assert(TL->getTypeLocClass() == TypeLoc::TemplateTypeParm);
			auto TTTL = TL->getAs<TemplateTypeParmTypeLoc>();

			TPT->parameter = WalkTypeTemplateParameter(TTTL.getDecl());
		}
		else if (TP->getDecl())
			TPT->parameter = WalkTypeTemplateParameter(TP->getDecl());
		TPT->Depth = TP->getDepth();
		TPT->index = TP->getIndex();
		TPT->IsParameterPack = TP->isParameterPack();

		Ty = ITypePtr(TPT);
		break;
	}
	case clang::Type::SubstTemplateTypeParm:
	{
		auto TP = Type->getAs<SubstTemplateTypeParmType>();
		ITemplateParameterSubstitutionTypePtr TPT = pComClang->CreateTemplateParameterSubstitutionType();

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		auto RepTy = TP->getReplacementType();
		TPT->Replacement = GetQualifiedType(RepTy, &Next);
		TPT->ReplacedParameter = ITemplateParameterTypePtr(
			WalkType(clang::QualType(TP->getReplacedParameter(), 0), 0));
		TPT->ReplacedParameter->parameter = WalkTypeTemplateParameter(
			TP->getReplacedParameter()->getDecl());

		Ty = ITypePtr(TPT);
		break;
	}
	case clang::Type::InjectedClassName:
	{
		auto ICN = Type->getAs<clang::InjectedClassNameType>();
		IInjectedClassNameTypePtr ICNT = pComClang->CreateInjectedClassNameType();
		ICNT->Class = IClassPtr(WalkDeclaration(
			ICN->getDecl()));
		ICNT->InjectedSpecializationType = GetQualifiedType(
			ICN->getInjectedSpecializationType());

		Ty = ITypePtr(ICNT);
		break;
	}
	case clang::Type::DependentName:
	{
		auto DN = Type->getAs<clang::DependentNameType>();
		IDependentNameTypePtr DNT = pComClang->CreateDependentNameType();
		switch (DN->getQualifier()->getKind())
		{
		case clang::NestedNameSpecifier::SpecifierKind::TypeSpec:
		case clang::NestedNameSpecifier::SpecifierKind::TypeSpecWithTemplate:
		{
			const auto& Qualifier = clang::QualType(DN->getQualifier()->getAsType(), 0);
			if (LocValid)
			{
				const auto& DNTL = TL->getAs<DependentNameTypeLoc>();
				if (!DNTL.isNull())
				{
					const auto& QL = DNTL.getQualifierLoc();
					const auto& NNSL = QL.getTypeLoc();
					DNT->Qualifier = GetQualifiedType(Qualifier, &NNSL);
				}
				else
				{
					DNT->Qualifier = GetQualifiedType(Qualifier, 0);
				}
			}
			else
			{
				DNT->Qualifier = GetQualifiedType(Qualifier, 0);
			}
			break;
		}
		default: break;
		}
		DNT->Identifier = DN->getIdentifier()->getName().str().c_str();

		Ty = ITypePtr(DNT);
		break;
	}
	case clang::Type::LValueReference:
	{
		auto LR = Type->getAs<clang::LValueReferenceType>();

		IPointerTypePtr P = pComClang->CreatePointerType();
		P->Modifier = TypeModifier_LVReference;

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		auto Pointee = LR->getPointeeType();
		P->QualifiedPointee = GetQualifiedType(Pointee, &Next);

		Ty = ITypePtr(P);
		break;
	}
	case clang::Type::RValueReference:
	{
		auto LR = Type->getAs<clang::RValueReferenceType>();

		IPointerTypePtr P = pComClang->CreatePointerType();
		P->Modifier = TypeModifier_RVReference;

		TypeLoc Next;
		if (LocValid) Next = TL->getNextTypeLoc();

		auto Pointee = LR->getPointeeType();
		P->QualifiedPointee = GetQualifiedType(Pointee, &Next);

		Ty = ITypePtr(P);
		break;
	}
	case clang::Type::UnaryTransform:
	{
		auto UT = Type->getAs<clang::UnaryTransformType>();

		IUnaryTransformTypePtr UTT = pComClang->CreateUnaryTransformType();
		auto Loc = TL->getAs<UnaryTransformTypeLoc>().getUnderlyingTInfo()->getTypeLoc();
		UTT->Desugared = GetQualifiedType(UT->isSugared() ? UT->desugar() : UT->getBaseType(), &Loc);
		UTT->BaseType = GetQualifiedType(UT->getBaseType(), &Loc);

		Ty = ITypePtr(UTT);
		break;
	}
	case clang::Type::Vector:
	{
		auto V = Type->getAs<clang::VectorType>();

		IVectorTypePtr VT = pComClang->CreateVectorType();
		VT->ElementType = GetQualifiedType(V->getElementType());
		VT->NumElements = V->getNumElements();

		Ty = ITypePtr(VT);
		break;
	}
	case clang::Type::PackExpansion:
	{
		// TODO: stubbed
		Ty = ITypePtr(pComClang->CreatePackExpansionType());
		break;
	}
	case clang::Type::Auto:
	{
		auto AT = Type->getAs<clang::AutoType>();
		if (AT->isSugared())
			Ty = WalkType(AT->desugar());
		else
			return nullptr;
		break;
	}
	case clang::Type::Decltype:
	{
		auto DT = Type->getAs<clang::DecltypeType>();
		Ty = WalkType(DT->getUnderlyingType(), TL);
		break;
	}
	default:
	{
		Debug("Unhandled type class '%s'\n", Type->getTypeClassName());
		return nullptr;
	}
	}

	Ty->IsDependent = Type->isDependentType();
	return Ty;
}

//-----------------------------------//

IEnumerationPtr Parser::WalkEnum(const clang::EnumDecl* ED)
{
	using namespace clang;

	auto NS = GetNamespace(ED);
	assert(NS && "Expected a valid namespace");

	auto E = NS->FindEnum((intptr_t)ED->getCanonicalDecl());
	if (E) {
		IDeclarationPtr Ed = IDeclarationPtr(E);
		if (!Ed->IsIncomplete)
			return E;
	}

	if (!E)
	{
		auto Name = GetTagDeclName(ED);
		if (!Name.empty())
			E = NS->FindEnum_2(Name.c_str(), /*Create=*/false);
		else
		{
			// Enum with no identifier - try to find existing enum through enum items
			for (auto it = ED->enumerator_begin(); it != ED->enumerator_end(); ++it)
			{
				EnumConstantDecl* ECD = (*it);
				auto EnumItemName = ECD->getNameAsString();
				E = NS->FindEnumWithItem(EnumItemName.c_str());
				break;
			}
		}
	}

	if (E) {
		IDeclarationPtr Ed = IDeclarationPtr(E);
		if (!Ed->IsIncomplete)
			return E;
	}

	if (!E)
	{
		auto Name = GetTagDeclName(ED);
		IDeclarationPtr Ed = nullptr;

		if (!Name.empty()) {
			E = NS->FindEnum_2(Name.c_str(), /*Create=*/true);
			Ed = IDeclarationPtr(E);
		}
		else
		{
			E = pComClang->CreateEnumeration();
			Ed = IDeclarationPtr(E);
			Ed->name = Name.c_str();
			Ed->Namespace = NS;
			NS->AddEnum(E);
		}
		HandleDeclaration(ED, Ed);
	}

	if (ED->isScoped())
		E->Modifiers = (EnumModifiers)
		((int)E->Modifiers | (int)EnumModifiers_Scoped);

	// Get the underlying integer backing the enum.
	clang::QualType IntType = ED->getIntegerType();
	E->Type = WalkType(IntType, 0);
	E->BuiltinType = IBuiltinTypePtr(WalkType(IntType, 0,
		/*DesugarType=*/true));

	if (!ED->isThisDeclarationADefinition())
	{
		IDeclarationPtr Ed = IDeclarationPtr(E);
		Ed->IsIncomplete = true;
		return E;
	}

	{
		IDeclarationPtr Ed = IDeclarationPtr(E);
		Ed->IsIncomplete = false;
	}
	for (auto it = ED->enumerator_begin(); it != ED->enumerator_end(); ++it)
	{
		E->AddItem(WalkEnumItem(*it));
	}

	return E;
}

IEnumeration_ItemPtr Parser::WalkEnumItem(clang::EnumConstantDecl* ECD)
{
	IEnumeration_ItemPtr EnumItem = pComClang->CreateEnumeration_Item();
	IDeclarationPtr EnumItemd = IDeclarationPtr(EnumItem);
	HandleDeclaration(ECD, EnumItemd);

	EnumItemd->name = ECD->getNameAsString().c_str();
	auto Value = ECD->getInitVal();
	EnumItem->value = Value.isSigned() ? Value.getSExtValue()
		: Value.getZExtValue();
	EnumItemd->Namespace = GetNamespace(ECD);

	std::string Text;
	if (GetDeclText(ECD->getSourceRange(), Text))
		EnumItem->Expression = Text.c_str();

	return EnumItem;
}

//-----------------------------------//

static const clang::CodeGen::CGFunctionInfo& GetCodeGenFunctionInfo(
	clang::CodeGen::CodeGenTypes* CodeGenTypes, const clang::FunctionDecl* FD)
{
	using namespace clang;
	if (auto CD = dyn_cast<clang::CXXConstructorDecl>(FD)) {
		return CodeGenTypes->arrangeCXXStructorDeclaration(CD, clang::CodeGen::StructorType::Base);
	}
	else if (auto DD = dyn_cast<clang::CXXDestructorDecl>(FD)) {
		return CodeGenTypes->arrangeCXXStructorDeclaration(DD, clang::CodeGen::StructorType::Base);
	}

	return CodeGenTypes->arrangeFunctionDeclaration(FD);
}

bool Parser::CanCheckCodeGenInfo(clang::Sema& S, const clang::Type* Ty)
{
	auto FinalType = GetFinalType(Ty);

	if (FinalType->isDependentType() ||
		FinalType->isInstantiationDependentType() || FinalType->isUndeducedType())
		return false;

	if (auto RT = FinalType->getAs<clang::RecordType>())
		return HasLayout(RT->getDecl());

	// Lock in the MS inheritance model if we have a member pointer to a class,
	// else we get an assertion error inside Clang's codegen machinery.
	if (c->getTarget().getCXXABI().isMicrosoft())
	{
		if (auto MPT = Ty->getAs<clang::MemberPointerType>())
			if (!MPT->isDependentType())
				S.RequireCompleteType(clang::SourceLocation(), clang::QualType(Ty, 0), 1);
	}

	return true;
}

static clang::TypeLoc DesugarTypeLoc(const clang::TypeLoc& Loc)
{
	using namespace clang;

	switch (Loc.getTypeLocClass())
	{
	case TypeLoc::TypeLocClass::Attributed:
	{
		auto ATL = Loc.getAs<AttributedTypeLoc>();
		return ATL.getModifiedLoc();
	}
	case TypeLoc::TypeLocClass::Paren:
	{
		auto PTL = Loc.getAs<ParenTypeLoc>();
		return PTL.getInnerLoc();
	}
	default:
		break;
	}

	return Loc;
}

void Parser::CompleteIfSpecializationType(const clang::QualType& QualType)
{
	using namespace clang;

	auto Type = QualType->getUnqualifiedDesugaredType();
	auto RD = Type->getAsCXXRecordDecl();
	if (!RD)
		RD = const_cast<CXXRecordDecl*>(Type->getPointeeCXXRecordDecl());
	ClassTemplateSpecializationDecl* CTS;
	if (!RD ||
		!(CTS = llvm::dyn_cast<ClassTemplateSpecializationDecl>(RD)) ||
		CTS->isCompleteDefinition())
		return;

	auto Diagnostics = c->getSema().getDiagnostics().getClient();
	auto SemaDiagnostics = static_cast<::DiagnosticConsumer*>(Diagnostics);
	c->getSema().InstantiateClassTemplateSpecialization(CTS->getLocStart(),
		CTS, TSK_ImplicitInstantiation, false);
}

IParameterPtr Parser::WalkParameter(const clang::ParmVarDecl* PVD,
	const clang::SourceLocation& ParamStartLoc)
{
	using namespace clang;

	auto P = walkedParameters[PVD];
	if (P)
		return P;

	P = pComClang->CreateParameter();
	IDeclarationPtr Pd = IDeclarationPtr(P);
	Pd->name = PVD->getNameAsString().c_str();

	TypeLoc PTL;
	if (auto TSI = PVD->getTypeSourceInfo())
		PTL = TSI->getTypeLoc();

	auto paramRange = PVD->getSourceRange();
	paramRange.setBegin(ParamStartLoc);

	HandlePreprocessedEntities(Pd, paramRange, MacroLocation_FunctionParameters);

	const auto& Type = PVD->getOriginalType();
	auto Function = PVD->getParentFunctionOrMethod();
	if (Function && cast<NamedDecl>(Function)->isExternallyVisible())
		CompleteIfSpecializationType(Type);
	P->QualifiedType = GetQualifiedType(Type, &PTL);
	P->HasDefaultValue = PVD->hasDefaultArg();
	P->index = PVD->getFunctionScopeIndex();
	if (PVD->hasDefaultArg() && !PVD->hasUnparsedDefaultArg())
	{
		if (PVD->hasUninstantiatedDefaultArg())
			P->DefaultArgument = WalkExpression(PVD->getUninstantiatedDefaultArg());
		else
			P->DefaultArgument = WalkExpression(PVD->getDefaultArg());
	}
	HandleDeclaration(PVD, Pd);
	walkedParameters[PVD] = P;
	auto Context = cast<Decl>(PVD->getDeclContext());
	Pd->Namespace = IDeclarationContextPtr(WalkDeclaration(Context));

	return P;
}

void Parser::SetBody(const clang::FunctionDecl* FD, IFunctionPtr F)
{
	F->Body = GetFunctionBody(FD).c_str();
	F->IsInline = FD->isInlined();
	if (!F->Body.length() == 0 && F->IsInline)
		return;
	for (const auto& R : FD->redecls())
	{
		if (F->Body.length() == 0)
			F->Body = GetFunctionBody(R).c_str();
		F->IsInline |= R->isInlined();
		if (!F->Body.length() == 0 && F->IsInline)
			break;
	}
}

static bool IsInvalid(clang::Stmt* Body, std::unordered_set<clang::Stmt*>& Bodies)
{
	using namespace clang;

	if (Bodies.find(Body) != Bodies.end())
		return false;
	Bodies.insert(Body);

	Decl* D = 0;
	switch (Body->getStmtClass())
	{
	case Stmt::StmtClass::DeclRefExprClass:
		D = cast<DeclRefExpr>(Body)->getDecl();
		break;
	case Stmt::StmtClass::MemberExprClass:
		D = cast<MemberExpr>(Body)->getMemberDecl();
		break;
	}
	if (D)
	{
		if (D->isInvalidDecl())
			return true;
		if (auto F = dyn_cast<FunctionDecl>(D))
			if (IsInvalid(F->getBody(), Bodies))
				return true;
	}
	for (auto C : Body->children())
		if (IsInvalid(C, Bodies))
			return true;
	return false;
}

std::stack<clang::Scope> Parser::GetScopesFor(clang::FunctionDecl* FD)
{
	using namespace clang;

	std::stack<DeclContext*> Contexts;
	DeclContext* DC = FD;
	while (DC)
	{
		Contexts.push(DC);
		DC = DC->getParent();
	}
	std::stack<Scope> Scopes;
	while (!Contexts.empty())
	{
		Scope S(Scopes.empty() ? 0 : &Scopes.top(),
			Scope::ScopeFlags::DeclScope, c->getDiagnostics());
		S.setEntity(Contexts.top());
		Scopes.push(S);
		Contexts.pop();
	}
	return Scopes;
}

void Parser::MarkValidity(IFunctionPtr F)
{
	using namespace clang;

	IDeclarationPtr Fd = IDeclarationPtr(F);
	auto FD = (FunctionDecl*)(Fd->OriginalPtr);

	if (!FD->getTemplateInstantiationPattern() ||
		FD->getTemplateInstantiationPattern()->isLateTemplateParsed() ||
		!FD->isExternallyVisible() ||
		c->getSourceManager().isInSystemHeader(FD->getLocStart()))
		return;

	auto Diagnostics = c->getSema().getDiagnostics().getClient();
	auto SemaDiagnostics = static_cast<::DiagnosticConsumer*>(Diagnostics);
	SemaDiagnostics->Decl = FD;
	auto TUScope = c->getSema().TUScope;
	std::stack<Scope> Scopes = GetScopesFor(FD);
	c->getSema().TUScope = &Scopes.top();
	c->getSema().InstantiateFunctionDefinition(FD->getLocStart(), FD,
		/*Recursive*/true);
	c->getSema().TUScope = TUScope;
	Fd->IsInvalid = FD->isInvalidDecl();
	if (!Fd->IsInvalid)
	{
		std::unordered_set<Stmt*> Bodies{ 0 };
		Fd->IsInvalid = IsInvalid(FD->getBody(), Bodies);
	}
}

void Parser::WalkFunction(const clang::FunctionDecl* FD, IFunctionPtr F,
	bool IsDependent)
{
	using namespace clang;

	assert(FD->getBuiltinID() == 0);
	auto FT = FD->getType()->getAs<clang::FunctionType>();

	auto NS = GetNamespace(FD);
	assert(NS && "Expected a valid namespace");

	IDeclarationPtr Fd = IDeclarationPtr(F);

	Fd->name = FD->getNameAsString().c_str();
	Fd->Namespace = NS;
	F->IsConstExpr = FD->isConstexpr();
	F->IsVariadic = FD->isVariadic();
	Fd->IsDependent = FD->isDependentContext();
	F->IsPure = FD->isPure();
	F->IsDeleted = FD->isDeleted();
	F->IsDefaulted = FD->isDefaulted();
	SetBody(FD, F);
	if (auto InstantiatedFrom = FD->getTemplateInstantiationPattern()) {
		F->InstantiatedFrom = IFunctionPtr(WalkDeclaration(InstantiatedFrom));
	}

	auto FK = FD->getFriendObjectKind();
	F->_FriendKind = ConvertFriendKind(FK);
	auto CC = FT->getCallConv();
	F->_CallingConvention = ConvertCallConv(CC);

	F->OperatorKind = GetOperatorKindFromDecl(FD->getDeclName());

	TypeLoc RTL;
	FunctionTypeLoc FTL;
	if (auto TSI = FD->getTypeSourceInfo())
	{
		auto Loc = DesugarTypeLoc(TSI->getTypeLoc());
		FTL = Loc.getAs<FunctionTypeLoc>();
		if (FTL)
		{
			RTL = FTL.getReturnLoc();

			auto& SM = c->getSourceManager();
			auto headStartLoc = GetDeclStartLocation(c.get(), FD);
			auto headEndLoc = SM.getExpansionLoc(FTL.getLParenLoc());
			auto headRange = clang::SourceRange(headStartLoc, headEndLoc);

			HandlePreprocessedEntities(Fd, headRange, MacroLocation_FunctionHead);
			HandlePreprocessedEntities(Fd, FTL.getParensRange(), MacroLocation_FunctionParameters);
		}
	}

	auto ReturnType = FD->getReturnType();
	if (FD->isExternallyVisible())
		CompleteIfSpecializationType(ReturnType);
	F->ReturnType = GetQualifiedType(ReturnType, &RTL);
	F->QualifiedType = GetQualifiedType(FD->getType(), &FTL);

	const auto& Mangled = GetDeclMangledName(FD);
	F->Mangled = Mangled.c_str();

	const auto& Body = GetFunctionBody(FD);
	F->Body = Body.c_str();

	clang::SourceLocation ParamStartLoc = FD->getLocStart();
	clang::SourceLocation ResultLoc;

	auto FTSI = FD->getTypeSourceInfo();
	if (FTSI)
	{
		auto FTL = FTSI->getTypeLoc();
		while (FTL && !FTL.getAs<FunctionTypeLoc>())
			FTL = FTL.getNextTypeLoc();

		if (FTL)
		{
			auto FTInfo = FTL.castAs<FunctionTypeLoc>();
			assert(!FTInfo.isNull());

			ParamStartLoc = FTInfo.getLParenLoc();
			ResultLoc = FTInfo.getReturnLoc().getLocStart();
		}
	}

	clang::SourceLocation BeginLoc = FD->getLocStart();
	if (ResultLoc.isValid())
		BeginLoc = ResultLoc;

	clang::SourceRange Range(BeginLoc, FD->getLocEnd());

	std::string Sig;
	if (GetDeclText(Range, Sig))
		F->Signature = Sig.c_str();

	for (auto VD : FD->parameters())
	{
		auto P = WalkParameter(VD, ParamStartLoc);
		F->AddParameter(P);

		ParamStartLoc = VD->getLocEnd();
	}

	auto& CXXABI = codeGenTypes->getCXXABI();
	bool HasThisReturn = false;
	if (auto CD = dyn_cast<CXXConstructorDecl>(FD))
		HasThisReturn = CXXABI.HasThisReturn(GlobalDecl(CD, Ctor_Complete));
	else if (auto DD = dyn_cast<CXXDestructorDecl>(FD))
		HasThisReturn = CXXABI.HasThisReturn(GlobalDecl(DD, Dtor_Complete));
	else
		HasThisReturn = CXXABI.HasThisReturn(FD);

	F->HasThisReturn = HasThisReturn;

	if (auto FTSI = FD->getTemplateSpecializationInfo())
		F->SpecializationInfo = WalkFunctionTemplateSpec(FTSI, F);

	if (FD->isDependentContext())
		return;

	const CXXMethodDecl* MD;
	if ((MD = dyn_cast<CXXMethodDecl>(FD)) && !MD->isStatic() &&
		!HasLayout(cast<CXXRecordDecl>(MD->getDeclContext())))
		return;

	if (!CanCheckCodeGenInfo(c->getSema(), FD->getReturnType().getTypePtr()))
		return;

	for (const auto& P : FD->parameters())
		if (!CanCheckCodeGenInfo(c->getSema(), P->getType().getTypePtr()))
			return;

	auto& CGInfo = GetCodeGenFunctionInfo(codeGenTypes, FD);
	F->IsReturnIndirect = CGInfo.getReturnInfo().isIndirect();

	unsigned Index = 0;
	for (auto I = CGInfo.arg_begin(), E = CGInfo.arg_end(); I != E; I++)
	{
		// Skip the first argument as it's the return type.
		if (I == CGInfo.arg_begin())
			continue;
		if (Index >= F->ParameterCount)
			continue;
		F->GetParameter(Index++)->IsIndirect = I->info.isIndirect();
	}

	MarkValidity(F);
}

IFunctionPtr Parser::WalkFunction(const clang::FunctionDecl* FD, bool IsDependent,
	bool AddToNamespace)
{
	using namespace clang;

	assert(FD->getBuiltinID() == 0);

	auto NS = GetNamespace(FD);
	assert(NS && "Expected a valid namespace");

	auto USR = GetDeclUSR(FD);
	auto F = NS->FindFunction(USR.c_str());
	if (F != nullptr)
		return F;

	F = pComClang->CreateFunction();
	HandleDeclaration(FD, IDeclarationPtr(F));

	if (AddToNamespace)
		NS->AddFunction(F);

	WalkFunction(FD, IFunctionPtr(F), IsDependent);

	return F;
}

//-----------------------------------//

SourceLocationKind Parser::GetLocationKind(const clang::SourceLocation& Loc)
{
	using namespace clang;

	clang::SourceManager& SM = c->getSourceManager();
	clang::PresumedLoc PLoc = SM.getPresumedLoc(Loc);

	if (PLoc.isInvalid())
		return SourceLocationKind_Invalid;

	const char *FileName = PLoc.getFilename();

	if (strcmp(FileName, "<built-in>") == 0)
		return SourceLocationKind_Builtin;

	if (strcmp(FileName, "<command line>") == 0)
		return SourceLocationKind_CommandLine;

	if (SM.getFileCharacteristic(Loc) == clang::SrcMgr::C_User)
		return SourceLocationKind_User;

	return SourceLocationKind_System;
}

bool Parser::IsValidDeclaration(const clang::SourceLocation& Loc)
{
	auto Kind = GetLocationKind(Loc);

	return Kind == SourceLocationKind_User;
}

//-----------------------------------//

void Parser::WalkAST()
{
	auto TU = c->getASTContext().getTranslationUnitDecl();
	for (auto D : TU->decls())
	{
		if (D->getLocStart().isValid() &&
			!c->getSourceManager().isInSystemHeader(D->getLocStart()))
			WalkDeclarationDef(D);
	}
}

//-----------------------------------//

void Parser::WalkVariable(const clang::VarDecl* VD, IVariablePtr Var)
{
	IDeclarationPtr Vard = IDeclarationPtr(Var);

	HandleDeclaration(VD, Vard);

	Vard->name = VD->getName().str().c_str();
	Vard->Access = ConvertToAccess(VD->getAccess());

	if (auto Init = VD->getInit())
		Var->Init = WalkExpression(Init);

	auto TL = VD->getTypeSourceInfo()->getTypeLoc();
	Var->QualifiedType = GetQualifiedType(VD->getType(), &TL);

	auto Mangled = GetDeclMangledName(VD);
	Var->Mangled = Mangled.c_str();
}

IVariablePtr Parser::WalkVariable(const clang::VarDecl *VD)
{
	using namespace clang;

	auto NS = GetNamespace(VD);
	assert(NS && "Expected a valid namespace");

	auto USR = GetDeclUSR(VD);
	if (auto Var = NS->FindVariable(USR.c_str())) {
		if (auto Init = VD->getInit())
			Var->Init = WalkExpression(Init);
		return Var;
	}

	IVariablePtr Var = pComClang->CreateVariable();
	IDeclarationPtr Vard = IDeclarationPtr(Var);
	Vard->Namespace = NS;

	WalkVariable(VD, Var);

	NS->AddVariable(Var);

	return Var;
}

//-----------------------------------//

IFriendPtr Parser::WalkFriend(const clang::FriendDecl *FD)
{
	using namespace clang;

	auto NS = GetNamespace(FD);
	assert(NS && "Expected a valid namespace");

	auto FriendDecl = FD->getFriendDecl();

	// Work around clangIndex's lack of USR handling for friends and pass the
	// pointed to friend declaration instead.
	auto USR = GetDeclUSR(FriendDecl ? ((Decl*)FriendDecl) : FD);
	if (auto F = NS->FindFriend(USR.c_str()))
		return F;

	IFriendPtr F = pComClang->CreateFriend();
	IDeclarationPtr Fd = IDeclarationPtr(F);
	HandleDeclaration(FD, Fd);
	Fd->Namespace = NS;

	if (FriendDecl)
	{
		F->Declaration = GetDeclarationFromFriend(FriendDecl);
	}

	NS->AddFriend(F);

	return F;
}

//-----------------------------------//

bool Parser::GetDeclText(clang::SourceRange SR, std::string& Text)
{
	using namespace clang;
	clang::SourceManager& SM = c->getSourceManager();
	const LangOptions &LangOpts = c->getLangOpts();

	auto Range = CharSourceRange::getTokenRange(SR);

	bool Invalid;
	Text = Lexer::getSourceText(Range, SM, LangOpts, &Invalid);

	return !Invalid && !Text.empty();
}

IPreprocessedEntityPtr Parser::WalkPreprocessedEntity(
	IDeclarationPtr Decl, clang::PreprocessedEntity* PPEntity)
{
	using namespace clang;

	for (unsigned I = 0, E = Decl->PreprocessedEntitieCount;
		I != E; ++I)
	{
		auto Entity = Decl->GetPreprocessedEntitie(I);
		if (Entity->OriginalPtr == (intptr_t)PPEntity)
			return Entity;
	}

	auto& P = c->getPreprocessor();

	IPreprocessedEntityPtr Entity = 0;

	switch (PPEntity->getKind())
	{
	case clang::PreprocessedEntity::MacroExpansionKind:
	{
		auto ME = cast<clang::MacroExpansion>(PPEntity);
		IMacroExpansionPtr Expansion = pComClang->CreateMacroExpansion();
		auto MD = ME->getDefinition();
		if (MD && MD->getKind() != clang::PreprocessedEntity::InvalidKind) {
			Expansion->Definition = IMacroDefinitionPtr(
				WalkPreprocessedEntity(Decl, ME->getDefinition()));
		}
		Entity = IPreprocessedEntityPtr(Expansion);

		std::string Text;
		GetDeclText(PPEntity->getSourceRange(), Text);

		IMacroExpansionPtr(Entity)->Text = Text.c_str();
		break;
	}
	case clang::PreprocessedEntity::MacroDefinitionKind:
	{
		auto MD = cast<clang::MacroDefinitionRecord>(PPEntity);

		if (!IsValidDeclaration(MD->getLocation()))
			break;

		const IdentifierInfo* II = MD->getName();
		assert(II && "Expected valid identifier info");

		MacroInfo* MI = P.getMacroInfo((IdentifierInfo*)II);

		if (!MI || MI->isBuiltinMacro() || MI->isFunctionLike())
			break;

		clang::SourceManager& SM = c->getSourceManager();
		const LangOptions &LangOpts = c->getLangOpts();

		auto Loc = MI->getDefinitionLoc();

		if (!IsValidDeclaration(Loc))
			break;

		clang::SourceLocation BeginExpr =
			Lexer::getLocForEndOfToken(Loc, 0, SM, LangOpts);

		auto Range = clang::CharSourceRange::getTokenRange(
			BeginExpr, MI->getDefinitionEndLoc());

		bool Invalid;
		StringRef Expression = Lexer::getSourceText(Range, SM, LangOpts,
			&Invalid);

		if (Invalid || Expression.empty())
			break;

		IMacroDefinitionPtr Definition = pComClang->CreateMacroDefinition();
		Definition->LineNumberStart = SM.getExpansionLineNumber(MD->getLocation());
		Definition->LineNumberEnd = SM.getExpansionLineNumber(MD->getLocation());
		Entity = IPreprocessedEntityPtr(Definition);

		Definition->name = II->getName().trim().str().c_str();
		Definition->Expression = Expression.trim().str().c_str();
	}
	case clang::PreprocessedEntity::InclusionDirectiveKind:
		// nothing to be done for InclusionDirectiveKind
		break;
	default:
		llvm_unreachable("Unknown PreprocessedEntity");
	}

	if (!Entity)
		return nullptr;

	Entity->OriginalPtr = (intptr_t)PPEntity;
	auto Namespace = GetTranslationUnit(PPEntity->getSourceRange().getBegin());

	if (Decl->DeclKind == DeclarationKind_TranslationUnit)
	{
		IDeclarationPtr Namespaced = IDeclarationPtr(Namespace);
		Namespaced->AddPreprocessedEntitie(Entity);
	}
	else
	{
		Decl->AddPreprocessedEntitie(Entity);
	}

	return Entity;
}

void Parser::HandlePreprocessedEntities(IDeclarationPtr Decl)
{
	using namespace clang;
	auto PPRecord = c->getPreprocessor().getPreprocessingRecord();

	for (auto it = PPRecord->begin(); it != PPRecord->end(); ++it)
	{
		clang::PreprocessedEntity* PPEntity = (*it);
		auto Entity = WalkPreprocessedEntity(Decl, PPEntity);
	}
}

IStatementPtr Parser::WalkStatement(const clang::Stmt* Stmt)
{
	return pComClang->CreateStatement(GetStringFromStatement(Stmt).c_str(), StatementClass_Any, nullptr);
}

IExpressionPtr Parser::WalkExpression(const clang::Expr* Expr)
{
	using namespace clang;

	switch (Expr->getStmtClass())
	{
	case Stmt::BinaryOperatorClass:
	{
		auto BinaryOperator = cast<clang::BinaryOperator>(Expr);
		return IExpressionPtr(pComClang->CreateBinaryOperator(GetStringFromStatement(Expr).c_str(),
			WalkExpression(BinaryOperator->getLHS()),
			WalkExpression(BinaryOperator->getRHS()),
			BinaryOperator->getOpcodeStr().str().c_str()));
	}
	case Stmt::CallExprClass:
	{
		auto CallExpr = cast<clang::CallExpr>(Expr);
		ICallExprPtr CallExpression = pComClang->CreateCallExpr(GetStringFromStatement(CallExpr).c_str(),
			CallExpr->getCalleeDecl() ? WalkDeclaration(CallExpr->getCalleeDecl()) : nullptr);
		for (auto arg : CallExpr->arguments())
		{
			CallExpression->AddArguments(WalkExpression(arg));
		}
		return IExpressionPtr(CallExpression);
	}
	case Stmt::DeclRefExprClass:
		return pComClang->CreateExpression(GetStringFromStatement(Expr).c_str(),
			StatementClass_DeclarationReference,
			WalkDeclaration(cast<DeclRefExpr>(Expr)->getDecl()));
	case Stmt::CStyleCastExprClass:
	case Stmt::CXXConstCastExprClass:
	case Stmt::CXXDynamicCastExprClass:
	case Stmt::CXXFunctionalCastExprClass:
	case Stmt::CXXReinterpretCastExprClass:
	case Stmt::CXXStaticCastExprClass:
	case Stmt::ImplicitCastExprClass:
		return WalkExpression(cast<CastExpr>(Expr)->getSubExprAsWritten());
	case Stmt::CXXOperatorCallExprClass:
	{
		auto OperatorCallExpr = cast<CXXOperatorCallExpr>(Expr);
		return pComClang->CreateExpression(GetStringFromStatement(Expr).c_str(), StatementClass_CXXOperatorCall,
			OperatorCallExpr->getCalleeDecl() ? WalkDeclaration(OperatorCallExpr->getCalleeDecl()) : nullptr);
	}
	case Stmt::CXXConstructExprClass:
	case Stmt::CXXTemporaryObjectExprClass:
	{
		auto ConstructorExpr = cast<clang::CXXConstructExpr>(Expr);
		if (ConstructorExpr->getNumArgs() == 1)
		{
			auto Arg = ConstructorExpr->getArg(0);
			auto TemporaryExpr = dyn_cast<MaterializeTemporaryExpr>(Arg);
			if (TemporaryExpr)
			{
				auto SubTemporaryExpr = TemporaryExpr->GetTemporaryExpr();
				auto Cast = dyn_cast<CastExpr>(SubTemporaryExpr);
				if (!Cast ||
					(Cast->getSubExprAsWritten()->getStmtClass() != Stmt::IntegerLiteralClass &&
						Cast->getSubExprAsWritten()->getStmtClass() != Stmt::CXXNullPtrLiteralExprClass))
					return WalkExpression(SubTemporaryExpr);
				return IExpressionPtr(pComClang->CreateCXXConstructExpr(GetStringFromStatement(ConstructorExpr).c_str(),
					WalkDeclaration(ConstructorExpr->getConstructor())));
			}
		}
		ICXXConstructExprPtr ConstructorExpression = pComClang->CreateCXXConstructExpr(GetStringFromStatement(ConstructorExpr).c_str(),
			WalkDeclaration(ConstructorExpr->getConstructor()));
		IStatementPtr ConstructorExpressions = IStatementPtr(ConstructorExpression);
		for (auto arg : ConstructorExpr->arguments())
		{
			ConstructorExpression->AddArguments(WalkExpression(arg));
		}
		return IExpressionPtr(ConstructorExpression);
	}
	case Stmt::CXXBindTemporaryExprClass:
		return WalkExpression(cast<CXXBindTemporaryExpr>(Expr)->getSubExpr());
	case Stmt::MaterializeTemporaryExprClass:
		return WalkExpression(cast<MaterializeTemporaryExpr>(Expr)->GetTemporaryExpr());
	case Stmt::AttributedStmtClass:
	{
		auto stmt = cast<clang::AttributedStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		SubExpr->AddStatement(WalkStatement(stmt->getSubStmt()));
		return IExpressionPtr(SubExpr);
	}
	case Stmt::BreakStmtClass:
	{
		auto stmt = cast<clang::BreakStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXCatchStmtClass:
	{
		auto stmt = cast<clang::CXXCatchStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXForRangeStmtClass:
	{
		auto stmt = cast<clang::CXXForRangeStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXTryStmtClass:
	{
		auto stmt = cast<clang::CXXTryStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CapturedStmtClass:
	{
		auto stmt = cast<clang::CapturedStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CompoundStmtClass:
	{
		auto stmt = cast<clang::CompoundStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ContinueStmtClass:
	{
		auto stmt = cast<clang::ContinueStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CoreturnStmtClass:
	{
		auto stmt = cast<clang::CoreturnStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CoroutineBodyStmtClass:
	{
		auto stmt = cast<clang::CoroutineBodyStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DeclStmtClass:
	{
		auto stmt = cast<clang::DeclStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DoStmtClass:
	{
		auto stmt = cast<clang::DoStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::BinaryConditionalOperatorClass:
	{
		auto stmt = cast<clang::BinaryConditionalOperator>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ConditionalOperatorClass:
	{
		auto stmt = cast<clang::ConditionalOperator>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::AddrLabelExprClass:
	{
		auto stmt = cast<clang::AddrLabelExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ArrayInitIndexExprClass:
	{
		auto stmt = cast<clang::ArrayInitIndexExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ArrayInitLoopExprClass:
	{
		auto stmt = cast<clang::ArrayInitLoopExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ArraySubscriptExprClass:
	{
		auto stmt = cast<clang::ArraySubscriptExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ArrayTypeTraitExprClass:
	{
		auto stmt = cast<clang::ArrayTypeTraitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::AsTypeExprClass:
	{
		auto stmt = cast<clang::AsTypeExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::AtomicExprClass:
	{
		auto stmt = cast<clang::AtomicExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CompoundAssignOperatorClass:
	{
		auto stmt = cast<clang::CompoundAssignOperator>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::BlockExprClass:
	{
		auto stmt = cast<clang::BlockExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXBoolLiteralExprClass:
	{
		auto stmt = cast<clang::CXXBoolLiteralExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXDefaultArgExprClass:
	{
		auto stmt = cast<clang::CXXDefaultArgExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXDefaultInitExprClass:
	{
		auto stmt = cast<clang::CXXDefaultInitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXDeleteExprClass:
	{
		auto stmt = cast<clang::CXXDeleteExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXDependentScopeMemberExprClass:
	{
		auto stmt = cast<clang::CXXDependentScopeMemberExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXFoldExprClass:
	{
		auto stmt = cast<clang::CXXFoldExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXInheritedCtorInitExprClass:
	{
		auto stmt = cast<clang::CXXInheritedCtorInitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXNewExprClass:
	{
		auto stmt = cast<clang::CXXNewExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXNoexceptExprClass:
	{
		auto stmt = cast<clang::CXXNoexceptExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXNullPtrLiteralExprClass:
	{
		auto stmt = cast<clang::CXXNullPtrLiteralExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXPseudoDestructorExprClass:
	{
		auto stmt = cast<clang::CXXPseudoDestructorExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXScalarValueInitExprClass:
	{
		auto stmt = cast<clang::CXXScalarValueInitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXStdInitializerListExprClass:
	{
		auto stmt = cast<clang::CXXStdInitializerListExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXThisExprClass:
	{
		auto stmt = cast<clang::CXXThisExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXThrowExprClass:
	{
		auto stmt = cast<clang::CXXThrowExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXTypeidExprClass:
	{
		auto stmt = cast<clang::CXXTypeidExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXUnresolvedConstructExprClass:
	{
		auto stmt = cast<clang::CXXUnresolvedConstructExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXUuidofExprClass:
	{
		auto stmt = cast<clang::CXXUuidofExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CUDAKernelCallExprClass:
	{
		auto stmt = cast<clang::CUDAKernelCallExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CXXMemberCallExprClass:
	{
		auto stmt = cast<clang::CXXMemberCallExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::UserDefinedLiteralClass:
	{
		auto stmt = cast<clang::UserDefinedLiteral>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ObjCBridgedCastExprClass:
	{
		auto stmt = cast<clang::ObjCBridgedCastExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CharacterLiteralClass:
	{
		auto stmt = cast<clang::CharacterLiteral>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ChooseExprClass:
	{
		auto stmt = cast<clang::ChooseExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CompoundLiteralExprClass:
	{
		auto stmt = cast<clang::CompoundLiteralExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ConvertVectorExprClass:
	{
		auto stmt = cast<clang::ConvertVectorExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CoawaitExprClass:
	{
		auto stmt = cast<clang::CoawaitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CoyieldExprClass:
	{
		auto stmt = cast<clang::CoyieldExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DependentCoawaitExprClass:
	{
		auto stmt = cast<clang::DependentCoawaitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DependentScopeDeclRefExprClass:
	{
		auto stmt = cast<clang::DependentScopeDeclRefExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DesignatedInitExprClass:
	{
		auto stmt = cast<clang::DesignatedInitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DesignatedInitUpdateExprClass:
	{
		auto stmt = cast<clang::DesignatedInitUpdateExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ExprWithCleanupsClass:
	{
		auto stmt = cast<clang::ExprWithCleanups>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ExpressionTraitExprClass:
	{
		auto stmt = cast<clang::ExpressionTraitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ExtVectorElementExprClass:
	{
		auto stmt = cast<clang::ExtVectorElementExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::FloatingLiteralClass:
	{
		auto stmt = cast<clang::FloatingLiteral>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::FunctionParmPackExprClass:
	{
		auto stmt = cast<clang::FunctionParmPackExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::GNUNullExprClass:
	{
		auto stmt = cast<clang::GNUNullExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::GenericSelectionExprClass:
	{
		auto stmt = cast<clang::GenericSelectionExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ImaginaryLiteralClass:
	{
		auto stmt = cast<clang::ImaginaryLiteral>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ImplicitValueInitExprClass:
	{
		auto stmt = cast<clang::ImplicitValueInitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::InitListExprClass:
	{
		auto InitListExpr = cast<clang::InitListExpr>(Expr);
		IInitListExprPtr InitListExpression = pComClang->CreateInitListExpr(GetStringFromStatement(InitListExpr).c_str(),
			InitListExpr->getInitializedFieldInUnion() ? WalkDeclaration(InitListExpr->getInitializedFieldInUnion()) : nullptr);
		for (auto init : InitListExpr->inits())
		{
			InitListExpression->AddInit(WalkExpression(init));
		}
		return IExpressionPtr(InitListExpression);
	}
	case Stmt::IntegerLiteralClass:
	{
		auto stmt = cast<clang::IntegerLiteral>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::LambdaExprClass:
	{
		auto stmt = cast<clang::LambdaExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::MSPropertyRefExprClass:
	{
		auto stmt = cast<clang::MSPropertyRefExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::MSPropertySubscriptExprClass:
	{
		auto stmt = cast<clang::MSPropertySubscriptExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::MemberExprClass:
	{
		auto stmt = cast<clang::MemberExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::NoInitExprClass:
	{
		auto stmt = cast<clang::NoInitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::OMPArraySectionExprClass:
	{
		auto stmt = cast<clang::OMPArraySectionExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::OffsetOfExprClass:
	{
		auto stmt = cast<clang::OffsetOfExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::OpaqueValueExprClass:
	{
		auto stmt = cast<clang::OpaqueValueExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::UnresolvedLookupExprClass:
	{
		auto stmt = cast<clang::UnresolvedLookupExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::UnresolvedMemberExprClass:
	{
		auto stmt = cast<clang::UnresolvedMemberExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::PackExpansionExprClass:
	{
		auto stmt = cast<clang::PackExpansionExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ParenExprClass:
	{
		auto stmt = cast<clang::ParenExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ParenListExprClass:
	{
		auto stmt = cast<clang::ParenListExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::PredefinedExprClass:
	{
		auto stmt = cast<clang::PredefinedExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::PseudoObjectExprClass:
	{
		auto stmt = cast<clang::PseudoObjectExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ShuffleVectorExprClass:
	{
		auto stmt = cast<clang::ShuffleVectorExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SizeOfPackExprClass:
	{
		auto stmt = cast<clang::SizeOfPackExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::StmtExprClass:
	{
		auto stmt = cast<clang::StmtExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::StringLiteralClass:
	{
		auto stmt = cast<clang::StringLiteral>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SubstNonTypeTemplateParmExprClass:
	{
		auto stmt = cast<clang::SubstNonTypeTemplateParmExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SubstNonTypeTemplateParmPackExprClass:
	{
		auto stmt = cast<clang::SubstNonTypeTemplateParmPackExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::TypeTraitExprClass:
	{
		auto stmt = cast<clang::TypeTraitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::TypoExprClass:
	{
		auto stmt = cast<clang::TypoExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::UnaryExprOrTypeTraitExprClass:
	{
		auto stmt = cast<clang::UnaryExprOrTypeTraitExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::UnaryOperatorClass:
	{
		auto stmt = cast<clang::UnaryOperator>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::VAArgExprClass:
	{
		auto stmt = cast<clang::VAArgExpr>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ForStmtClass:
	{
		auto stmt = cast<clang::ForStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::GotoStmtClass:
	{
		auto stmt = cast<clang::GotoStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::IfStmtClass:
	{
		auto stmt = cast<clang::IfStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::IndirectGotoStmtClass:
	{
		auto stmt = cast<clang::IndirectGotoStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::LabelStmtClass:
	{
		auto stmt = cast<clang::LabelStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::MSDependentExistsStmtClass:
	{
		auto stmt = cast<clang::MSDependentExistsStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::NullStmtClass:
	{
		auto stmt = cast<clang::NullStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::ReturnStmtClass:
	{
		auto stmt = cast<clang::ReturnStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SEHExceptStmtClass:
	{
		auto stmt = cast<clang::SEHExceptStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SEHFinallyStmtClass:
	{
		auto stmt = cast<clang::SEHFinallyStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SEHLeaveStmtClass:
	{
		auto stmt = cast<clang::SEHLeaveStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SEHTryStmtClass:
	{
		auto stmt = cast<clang::SEHTryStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::CaseStmtClass:
	{
		auto stmt = cast<clang::CaseStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::DefaultStmtClass:
	{
		auto stmt = cast<clang::DefaultStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::SwitchStmtClass:
	{
		auto stmt = cast<clang::SwitchStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::WhileStmtClass:
	{
		auto stmt = cast<clang::WhileStmt>(Expr);
		ISubStmtExprPtr SubExpr = pComClang->CreateSubStmtExpr(GetStringFromStatement(stmt).c_str(), nullptr);
		return IExpressionPtr(SubExpr);
	}
	case Stmt::OMPAtomicDirectiveClass:
	case Stmt::OMPBarrierDirectiveClass:
	case Stmt::OMPCancelDirectiveClass:
	case Stmt::OMPCancellationPointDirectiveClass:
	case Stmt::OMPCriticalDirectiveClass:
	case Stmt::OMPFlushDirectiveClass:
	case Stmt::OMPDistributeDirectiveClass:
	case Stmt::OMPDistributeParallelForDirectiveClass:
	case Stmt::OMPDistributeParallelForSimdDirectiveClass:
	case Stmt::OMPDistributeSimdDirectiveClass:
	case Stmt::OMPForDirectiveClass:
	case Stmt::OMPForSimdDirectiveClass:
	case Stmt::OMPParallelForDirectiveClass:
	case Stmt::OMPParallelForSimdDirectiveClass:
	case Stmt::OMPSimdDirectiveClass:
	case Stmt::OMPTargetParallelForSimdDirectiveClass:
	case Stmt::OMPTargetSimdDirectiveClass:
	case Stmt::OMPTargetTeamsDistributeDirectiveClass:
	case Stmt::OMPTargetTeamsDistributeParallelForDirectiveClass:
	case Stmt::OMPTargetTeamsDistributeParallelForSimdDirectiveClass:
	case Stmt::OMPTargetTeamsDistributeSimdDirectiveClass:
	case Stmt::OMPTaskLoopDirectiveClass:
	case Stmt::OMPTaskLoopSimdDirectiveClass:
	case Stmt::OMPTeamsDistributeDirectiveClass:
	case Stmt::OMPTeamsDistributeParallelForDirectiveClass:
	case Stmt::OMPTeamsDistributeParallelForSimdDirectiveClass:
	case Stmt::OMPTeamsDistributeSimdDirectiveClass:
	case Stmt::OMPMasterDirectiveClass:
	case Stmt::OMPOrderedDirectiveClass:
	case Stmt::OMPParallelDirectiveClass:
	case Stmt::OMPParallelSectionsDirectiveClass:
	case Stmt::OMPSectionDirectiveClass:
	case Stmt::OMPSectionsDirectiveClass:
	case Stmt::OMPSingleDirectiveClass:
	case Stmt::OMPTargetDataDirectiveClass:
	case Stmt::OMPTargetDirectiveClass:
	case Stmt::OMPTargetEnterDataDirectiveClass:
	case Stmt::OMPTargetExitDataDirectiveClass:
	case Stmt::OMPTargetParallelDirectiveClass:
	case Stmt::OMPTargetParallelForDirectiveClass:
	case Stmt::OMPTargetTeamsDirectiveClass:
	case Stmt::OMPTargetUpdateDirectiveClass:
	case Stmt::OMPTaskDirectiveClass:
	case Stmt::OMPTaskgroupDirectiveClass:
	case Stmt::OMPTaskwaitDirectiveClass:
	case Stmt::OMPTaskyieldDirectiveClass:
	case Stmt::OMPTeamsDirectiveClass:
		break;
	case Stmt::ObjCArrayLiteralClass:
	case Stmt::ObjCAvailabilityCheckExprClass:
	case Stmt::ObjCBoolLiteralExprClass:
	case Stmt::ObjCBoxedExprClass:
	case Stmt::ObjCDictionaryLiteralClass:
	case Stmt::ObjCEncodeExprClass:
	case Stmt::ObjCIndirectCopyRestoreExprClass:
	case Stmt::ObjCIsaExprClass:
	case Stmt::ObjCIvarRefExprClass:
	case Stmt::ObjCMessageExprClass:
	case Stmt::ObjCPropertyRefExprClass:
	case Stmt::ObjCProtocolExprClass:
	case Stmt::ObjCSelectorExprClass:
	case Stmt::ObjCStringLiteralClass:
	case Stmt::ObjCSubscriptRefExprClass:
	case Stmt::ObjCAtCatchStmtClass:
	case Stmt::ObjCAtFinallyStmtClass:
	case Stmt::ObjCAtSynchronizedStmtClass:
	case Stmt::ObjCAtThrowStmtClass:
	case Stmt::ObjCAtTryStmtClass:
	case Stmt::ObjCAutoreleasePoolStmtClass:
	case Stmt::ObjCForCollectionStmtClass:
		break;
	default:
		break;
	}
	llvm::APSInt integer;
	if (Expr->getStmtClass() != Stmt::CharacterLiteralClass &&
		Expr->getStmtClass() != Stmt::CXXBoolLiteralExprClass &&
		Expr->getStmtClass() != Stmt::UnaryExprOrTypeTraitExprClass &&
		!Expr->isValueDependent() &&
		Expr->EvaluateAsInt(integer, c->getASTContext()))
		return pComClang->CreateExpression(GetStringFromStatement(Expr).c_str(), StatementClass_Any, nullptr);
	return pComClang->CreateExpression(GetStringFromStatement(Expr).c_str(), StatementClass_Any, nullptr);
}

std::string Parser::GetStringFromStatement(const clang::Stmt* Statement)
{
	using namespace clang;

	PrintingPolicy Policy(c->getLangOpts());
	std::string s;
	llvm::raw_string_ostream as(s);
	Statement->printPretty(as, 0, Policy);
	return as.str();
}

std::string Parser::GetFunctionBody(const clang::FunctionDecl* FD)
{
	if (!FD->getBody())
		return "";

	clang::PrintingPolicy Policy(c->getLangOpts());
	std::string s;
	llvm::raw_string_ostream as(s);
	FD->getBody()->printPretty(as, 0, Policy);
	return as.str();
}

void Parser::HandlePreprocessedEntities(IDeclarationPtr Decl,
	clang::SourceRange sourceRange,
	MacroLocation macroLocation)
{
	if (sourceRange.isInvalid()) return;

	auto& SourceMgr = c->getSourceManager();
	auto isBefore = SourceMgr.isBeforeInTranslationUnit(sourceRange.getEnd(),
		sourceRange.getBegin());

	if (isBefore) return;

	assert(!SourceMgr.isBeforeInTranslationUnit(sourceRange.getEnd(),
		sourceRange.getBegin()));

	using namespace clang;
	auto PPRecord = c->getPreprocessor().getPreprocessingRecord();

	auto Range = PPRecord->getPreprocessedEntitiesInRange(sourceRange);

	for (auto PPEntity : Range)
	{
		auto Entity = WalkPreprocessedEntity(Decl, PPEntity);
		if (!Entity) continue;

		if (Entity->_MacroLocation == MacroLocation_Unknown)
			Entity->_MacroLocation = macroLocation;
	}
}

void Parser::HandleOriginalText(const clang::Decl* D, IDeclarationPtr Decl)
{
	auto& SM = c->getSourceManager();
	auto& LangOpts = c->getLangOpts();

	auto Range = clang::CharSourceRange::getTokenRange(D->getSourceRange());

	bool Invalid;
	auto DeclText = clang::Lexer::getSourceText(Range, SM, LangOpts, &Invalid);

	if (!Invalid)
		Decl->DebugText = DeclText.str().c_str();
}

void Parser::HandleDeclaration(const clang::Decl* D, IDeclarationPtr Decl)
{
	if (Decl->OriginalPtr != 0)
		return;

	Decl->OriginalPtr = (intptr_t)D;
	Decl->USR = GetDeclUSR(D).c_str();
	Decl->IsImplicit = D->isImplicit();

	Decl->Location = pComClang->CreateSourceLocation(
		c->getSourceManager().getFilename(D->getLocation()).str().c_str(),
		c->getSourceManager().getSpellingLineNumber(D->getLocation()),
		c->getSourceManager().getSpellingColumnNumber(D->getLocation()));
	auto IsDeclExplicit = IsExplicit(D);
	if (IsDeclExplicit)
	{
		Decl->LineNumberStart = c->getSourceManager().getExpansionLineNumber(D->getLocStart());
		Decl->LineNumberEnd = c->getSourceManager().getExpansionLineNumber(D->getLocEnd());
	}
	else
	{
		Decl->LineNumberStart = -1;
		Decl->LineNumberEnd = -1;
	}

	if (Decl->PreprocessedEntitieCount == 0 && !D->isImplicit())
	{
		if (clang::dyn_cast<clang::TranslationUnitDecl>(D))
		{
			HandlePreprocessedEntities(Decl);
		}
		else if (clang::dyn_cast<clang::ParmVarDecl>(D))
		{
			// Ignore function parameters as we already walk their preprocessed entities.
		}
		else if (IsDeclExplicit)
		{
			auto startLoc = GetDeclStartLocation(c.get(), D);
			auto endLoc = D->getLocEnd();
			auto range = clang::SourceRange(startLoc, endLoc);

			HandlePreprocessedEntities(Decl, range, MacroLocation_Unknown);
		}
	}

	if (IsDeclExplicit)
		HandleOriginalText(D, Decl);
	HandleComments(D, Decl);

	if (const clang::ValueDecl *VD = clang::dyn_cast_or_null<clang::ValueDecl>(D))
		Decl->IsDependent = VD->getType()->isDependentType();

	if (const clang::DeclContext *DC = clang::dyn_cast_or_null<clang::DeclContext>(D))
		Decl->IsDependent |= DC->isDependentContext();

	Decl->Access = ConvertToAccess(D->getAccess());
}

//-----------------------------------//

IDeclarationPtr Parser::WalkDeclarationDef(clang::Decl* D)
{
	auto Decl = WalkDeclaration(D);
	if (!Decl || Decl->DefinitionOrder > 0)
		return Decl;
	// We store a definition order index into the declarations.
	// This is needed because declarations are added to their contexts as
	// soon as they are referenced and we need to know the original order
	// of the declarations.
	clang::RecordDecl* RecordDecl;
	if ((RecordDecl = llvm::dyn_cast<clang::RecordDecl>(D)) &&
		RecordDecl->isCompleteDefinition())
		Decl->DefinitionOrder = index++;
	return Decl;
}

IDeclarationPtr Parser::WalkDeclaration(const clang::Decl* D)
{
	using namespace clang;

	IDeclarationPtr Decl = nullptr;

	auto Kind = D->getKind();
	switch (D->getKind())
	{
	case Decl::Record:
	{
		auto RD = cast<RecordDecl>(D);
		Decl = IDeclarationPtr(WalkRecord(RD));
		break;
	}
	case Decl::CXXRecord:
	{
		auto RD = cast<CXXRecordDecl>(D);
		Decl = IDeclarationPtr(WalkRecordCXX(RD));
		break;
	}
	case Decl::ClassTemplate:
	{
		auto TD = cast<ClassTemplateDecl>(D);
		auto Template = WalkClassTemplate(TD);

		Decl = IDeclarationPtr(Template);
		break;
	}
	case Decl::ClassTemplateSpecialization:
	{
		auto TS = cast<ClassTemplateSpecializationDecl>(D);
		auto CT = WalkClassTemplateSpecialization(TS);

		Decl = IDeclarationPtr(CT);
		break;
	}
	case Decl::ClassTemplatePartialSpecialization:
	{
		auto TS = cast<ClassTemplatePartialSpecializationDecl>(D);
		auto CT = WalkClassTemplatePartialSpecialization(TS);

		Decl = IDeclarationPtr(CT);
		break;
	}
	case Decl::FunctionTemplate:
	{
		auto TD = cast<FunctionTemplateDecl>(D);
		auto FT = WalkFunctionTemplate(TD);

		Decl = IDeclarationPtr(FT);
		break;
	}
	case Decl::VarTemplate:
	{
		auto TD = cast<VarTemplateDecl>(D);
		auto Template = WalkVarTemplate(TD);

		Decl = IDeclarationPtr(Template);
		break;
	}
	case Decl::VarTemplateSpecialization:
	{
		auto TS = cast<VarTemplateSpecializationDecl>(D);
		auto CT = WalkVarTemplateSpecialization(TS);

		Decl = IDeclarationPtr(CT);
		break;
	}
	case Decl::VarTemplatePartialSpecialization:
	{
		auto TS = cast<VarTemplatePartialSpecializationDecl>(D);
		auto CT = WalkVarTemplatePartialSpecialization(TS);

		Decl = IDeclarationPtr(CT);
		break;
	}
	case Decl::TypeAliasTemplate:
	{
		auto TD = cast<TypeAliasTemplateDecl>(D);
		auto TA = WalkTypeAliasTemplate(TD);

		Decl = IDeclarationPtr(TA);
		break;
	}
	case Decl::Enum:
	{
		auto ED = cast<EnumDecl>(D);
		Decl = IDeclarationPtr(WalkEnum(ED));
		break;
	}
	case Decl::EnumConstant:
	{
		auto ED = cast<EnumConstantDecl>(D);
		IEnumerationPtr E = GetNamespace(ED);
		assert(E && "Expected a valid enumeration");
		Decl = IDeclarationPtr(E->FindItemByName(ED->getNameAsString().c_str()));
		break;
	}
	case Decl::Function:
	{
		auto FD = cast<FunctionDecl>(D);

		// Check for and ignore built-in functions.
		if (FD->getBuiltinID() != 0)
			break;

		Decl = IDeclarationPtr(WalkFunction(FD));
		break;
	}
	case Decl::LinkageSpec:
	{
		auto LS = cast<LinkageSpecDecl>(D);

		for (auto it = LS->decls_begin(); it != LS->decls_end(); ++it)
		{
			clang::Decl* D = (*it);
			Decl = WalkDeclarationDef(D);
		}

		break;
	}
	case Decl::Typedef:
	{
		auto TD = cast<clang::TypedefDecl>(D);

		auto NS = GetNamespace(TD);
		auto Name = GetDeclName(TD);
		auto Typedef = NS->FindTypedef(Name.c_str(), /*Create=*/false);
		if (Typedef) return IDeclarationPtr(Typedef);

		Typedef = NS->FindTypedef(Name.c_str(), /*Create=*/true);
		Decl = IDeclarationPtr(Typedef);
		HandleDeclaration(TD, Decl);

		auto TTL = TD->getTypeSourceInfo()->getTypeLoc();
		// resolve the typedef before adding it to the list otherwise it might be found and returned prematurely
		// see "typedef _Aligned<16, char>::type type;" and the related classes in Common.h in the tests
		ITypedefNameDeclPtr Typedefd = ITypedefNameDeclPtr(Typedef);
		Typedefd->QualifiedType = GetQualifiedType(TD->getUnderlyingType(), &TTL);
		ITypedefDeclPtr Existing;
		// if the typedef was added along the way, the just created one is useless, delete it
		if ((Existing = NS->FindTypedef(Name.c_str(), /*Create=*/false)))
			Typedef->Release();
		else
			NS->AddTypedef(Existing = Typedef);

		Decl = IDeclarationPtr(Existing);
		break;
	}
	case Decl::TypeAlias:
	{
		auto TD = cast<clang::TypeAliasDecl>(D);

		auto NS = GetNamespace(TD);
		auto Name = GetDeclName(TD);
		auto TypeAlias = NS->FindTypeAlias(Name.c_str(), /*Create=*/false);
		if (TypeAlias) return IDeclarationPtr(TypeAlias);

		TypeAlias = NS->FindTypeAlias(Name.c_str(), /*Create=*/true);
		HandleDeclaration(TD, IDeclarationPtr(TypeAlias));

		auto TTL = TD->getTypeSourceInfo()->getTypeLoc();
		// see above the case for "Typedef"
		ITypedefNameDeclPtr TypeAliasd = ITypedefNameDeclPtr(TypeAlias);
		TypeAliasd->QualifiedType = GetQualifiedType(TD->getUnderlyingType(), &TTL);
		ITypeAliasPtr Existing;
		if ((Existing = NS->FindTypeAlias(Name.c_str(), /*Create=*/false)))
			TypeAlias->Release();
		else
			NS->AddTypeAliase(Existing = TypeAlias);

		if (auto TAT = TD->getDescribedAliasTemplate())
			TypeAlias->DescribedAliasTemplate = WalkTypeAliasTemplate(TAT);

		 Decl = IDeclarationPtr(Existing);
		break;
	}
	case Decl::TranslationUnit:
	{
		Decl = IDeclarationPtr(GetTranslationUnit(D));
		break;
	}
	case Decl::Namespace:
	{
		auto ND = cast<NamespaceDecl>(D);

		for (auto D : ND->decls())
		{
			if (!isa<NamedDecl>(D) || IsSupported(cast<NamedDecl>(D)))
				Decl = WalkDeclarationDef(D);
		}

		break;
	}
	case Decl::Var:
	{
		auto VD = cast<VarDecl>(D);
		Decl = IDeclarationPtr(WalkVariable(VD));
		break;
	}
	case Decl::CXXConstructor:
	case Decl::CXXDestructor:
	case Decl::CXXConversion:
	case Decl::CXXMethod:
	{
		auto MD = cast<CXXMethodDecl>(D);
		Decl = IDeclarationPtr(WalkMethodCXX(MD));

		auto NS = GetNamespace(MD);
		Decl->Namespace = NS;
		break;
	}
	case Decl::Friend:
	{
		auto FD = cast<FriendDecl>(D);
		Decl = IDeclarationPtr(WalkFriend(FD));
		break;
	}
	case Decl::TemplateTemplateParm:
	{
		auto TTP = cast<TemplateTemplateParmDecl>(D);
		Decl = IDeclarationPtr(WalkTemplateTemplateParameter(TTP));
		break;
	}
	case Decl::TemplateTypeParm:
	{
		auto TTPD = cast<TemplateTypeParmDecl>(D);
		Decl = IDeclarationPtr(WalkTypeTemplateParameter(TTPD));
		break;
	}
	case Decl::NonTypeTemplateParm:
	{
		auto NTTPD = cast<NonTypeTemplateParmDecl>(D);
		Decl = IDeclarationPtr(WalkNonTypeTemplateParameter(NTTPD));
		break;
	}
	case Decl::BuiltinTemplate:
	case Decl::ClassScopeFunctionSpecialization:
	case Decl::PragmaComment:
	case Decl::PragmaDetectMismatch:
	case Decl::Empty:
	case Decl::AccessSpec:
	case Decl::Using:
	case Decl::UsingDirective:
	case Decl::UsingShadow:
	case Decl::ConstructorUsingShadow:
	case Decl::UnresolvedUsingTypename:
	case Decl::UnresolvedUsingValue:
	case Decl::IndirectField:
	case Decl::StaticAssert:
	case Decl::NamespaceAlias:
		break;
	default:
	{
		Debug("Unhandled declaration kind: %s\n", D->getDeclKindName());

		auto& SM = c->getSourceManager();
		auto Loc = D->getLocation();
		auto FileName = SM.getFilename(Loc);
		auto Offset = SM.getFileOffset(Loc);
		auto LineNo = SM.getLineNumber(SM.getFileID(Loc), Offset);
		Debug("  %s (line %u)\n", FileName.str().c_str(), LineNo);

		break;
	}
	};

	if (Decl && D->hasAttrs())
	{
		for (auto it = D->attr_begin(); it != D->attr_end(); ++it)
		{
			Attr* Attr = (*it);
			if (Attr->getKind() == clang::attr::Kind::MaxFieldAlignment)
			{
				auto MFA = cast<clang::MaxFieldAlignmentAttr>(Attr);
				Decl->MaxFieldAlignment = MFA->getAlignment() / 8; // bits to bytes.
			}
		}
	}

	return Decl;
}

void Parser::HandleDiagnostics(IParserResultPtr res)
{
	auto DiagClient = (DiagnosticConsumer&)c->getDiagnosticClient();
	auto& Diags = DiagClient.Diagnostics;

	// Convert the diagnostics to the managed types
	for (unsigned I = 0, E = Diags.size(); I != E; ++I)
	{
		auto& Diag = DiagClient.Diagnostics[I];
		auto& Source = c->getSourceManager();
		auto FileName = Source.getFilename(Source.getFileLoc(Diag.Location));

		IParserDiagnosticPtr PDiag = pComClang->CreateParserDiagnostic();
		PDiag->FileName = FileName.str().c_str();
		PDiag->Message = Diag.Message.str().str().c_str();
		PDiag->LineNumber = 0;
		PDiag->ColumnNumber = 0;

		if (!Diag.Location.isInvalid())
		{
			clang::PresumedLoc PLoc = Source.getPresumedLoc(Diag.Location);
			if (PLoc.isValid())
			{
				PDiag->LineNumber = PLoc.getLine();
				PDiag->ColumnNumber = PLoc.getColumn();
			}
		}

		switch (Diag.Level)
		{
		case clang::DiagnosticsEngine::Ignored:
			PDiag->Level = ParserDiagnosticLevel_Ignored;
			break;
		case clang::DiagnosticsEngine::Note:
			PDiag->Level = ParserDiagnosticLevel_Note;
			break;
		case clang::DiagnosticsEngine::Warning:
			PDiag->Level = ParserDiagnosticLevel_Warning;
			break;
		case clang::DiagnosticsEngine::Error:
			PDiag->Level = ParserDiagnosticLevel_Error;
			break;
		case clang::DiagnosticsEngine::Fatal:
			PDiag->Level = ParserDiagnosticLevel_Fatal;
			break;
		default:
			assert(0);
		}

		res->AddDiagnostics(PDiag);
	}
}

IParserResultPtr Parser::ParseHeader(const std::vector<std::string>& SourceFiles)
{
	assert(opts->ASTContext && "Expected a valid ASTContext");

	IParserResultPtr res = pComClang->CreateParserResult();

	if (SourceFiles.empty())
	{
		res->Kind = ParserResultKind_FileNotFound;
		return res;
	}

	Setup();

	std::unique_ptr<clang::SemaConsumer> SC(new clang::SemaConsumer());
	c->setASTConsumer(std::move(SC));

	c->createSema(clang::TU_Complete, 0);

	auto DiagClient = new DiagnosticConsumer();
	c->getDiagnostics().setClient(DiagClient);

	// Check that the file is reachable.
	const clang::DirectoryLookup *Dir;
	llvm::SmallVector<
		std::pair<const clang::FileEntry *, const clang::DirectoryEntry *>,
		0> Includers;

	std::vector<const clang::FileEntry*> FileEntries;
	for (const auto& SourceFile : SourceFiles)
	{
		auto FileEntry = c->getPreprocessor().getHeaderSearchInfo().LookupFile(SourceFile,
			clang::SourceLocation(), /*isAngled*/true,
			nullptr, Dir, Includers, nullptr, nullptr, nullptr, nullptr, nullptr);

		if (!FileEntry)
		{
			res->Kind = ParserResultKind_FileNotFound;
			return res;
		}
		FileEntries.push_back(FileEntry);
	}

	// Create a virtual file that includes the header. This gets rid of some
	// Clang warnings about parsing an header file as the main file.

	std::string str;
	for (const auto& SourceFile : SourceFiles)
	{
		str += "#include \"" + SourceFile + "\"" + "\n";
	}
	str += "\0";

	auto buffer = llvm::MemoryBuffer::getMemBuffer(str);
	auto& SM = c->getSourceManager();
	SM.setMainFileID(SM.createFileID(std::move(buffer)));

	clang::DiagnosticConsumer* client = c->getDiagnostics().getClient();
	client->BeginSourceFile(c->getLangOpts(), &c->getPreprocessor());

	ParseAST(c->getSema());

	client->EndSourceFile();

	HandleDiagnostics(res);

	if (client->getNumErrors() != 0)
	{
		res->Kind = ParserResultKind_Error;
		return res;
	}

	auto& AST = c->getASTContext();

	auto FileEntry = FileEntries[0];
	auto FileName = FileEntry->getName();
	auto Unit = opts->ASTContext->FindOrCreateModule(FileName.str().c_str());

	auto TU = AST.getTranslationUnitDecl();
	IDeclarationPtr Unitd = IDeclarationPtr(Unit);
	HandleDeclaration(TU, Unitd);

	if (Unitd->OriginalPtr == 0)
		Unitd->OriginalPtr = (intptr_t)FileEntry;

	// Initialize enough Clang codegen machinery so we can get at ABI details.
	llvm::LLVMContext Ctx;
	std::unique_ptr<llvm::Module> M(new llvm::Module("", Ctx));

	M->setTargetTriple(c->getTarget().getTriple().getTriple());
	M->setDataLayout(c->getTarget().getDataLayout());

	std::unique_ptr<clang::CodeGen::CodeGenModule> CGM(
		new clang::CodeGen::CodeGenModule(c->getASTContext(), c->getHeaderSearchOpts(),
			c->getPreprocessorOpts(), c->getCodeGenOpts(), *M, c->getDiagnostics()));

	std::unique_ptr<clang::CodeGen::CodeGenTypes> CGT(
		new clang::CodeGen::CodeGenTypes(*CGM.get()));

	codeGenTypes = CGT.get();

	WalkAST();

	res->TargetInfo = GetTargetInfo();

	res->Kind = ParserResultKind_Success;
	return res;
}

ParserResultKind Parser::ParseArchive(llvm::StringRef File,
	llvm::object::Archive* Archive,
	INativeLibraryPtr & NativeLib)
{
	auto LibName = File;
	NativeLib = pComClang->CreateNativeLibrary();
	NativeLib->FileName = LibName.str().c_str();

	for (auto it = Archive->symbol_begin(); it != Archive->symbol_end(); ++it)
	{
		llvm::StringRef SymRef = it->getName();
		NativeLib->AddSymbols(SymRef.str().c_str());
	}

	return ParserResultKind_Success;
}

IParserResultPtr Parser::ParseForAST(const std::vector<std::string>& SourceFiles)
{
	assert(opts->ASTContext && "Expected a valid ASTContext");

	IParserResultPtr res = pComClang->CreateParserResult();

	if (SourceFiles.empty())
	{
		res->Kind = ParserResultKind_FileNotFound;
		return res;
	}

	Setup();

	std::unique_ptr<clang::SemaConsumer> SC(new clang::SemaConsumer());
	c->setASTConsumer(std::move(SC));

	c->createSema(clang::TU_Complete, 0);

	auto DiagClient = new DiagnosticConsumer();
	c->getDiagnostics().setClient(DiagClient);

	// Check that the file is reachable.
	const clang::DirectoryLookup *Dir;
	llvm::SmallVector<
		std::pair<const clang::FileEntry *, const clang::DirectoryEntry *>,
		0> Includers;

	std::vector<const clang::FileEntry*> FileEntries;
	for (const auto& SourceFile : SourceFiles)
	{
		auto FileEntry = c->getPreprocessor().getHeaderSearchInfo().LookupFile(SourceFile,
			clang::SourceLocation(), /*isAngled*/true,
			nullptr, Dir, Includers, nullptr, nullptr, nullptr, nullptr, nullptr);

		if (!FileEntry)
		{
			res->Kind = ParserResultKind_FileNotFound;
			return res;
		}
		FileEntries.push_back(FileEntry);
	}

	// Create a virtual file that includes the header. This gets rid of some
	// Clang warnings about parsing an header file as the main file.

	std::string str;
	for (const auto& SourceFile : SourceFiles)
	{
		str += "#include \"" + SourceFile + "\"" + "\n";
	}
	str += "\0";

	auto buffer = llvm::MemoryBuffer::getMemBuffer(str);
	auto& SM = c->getSourceManager();
	SM.setMainFileID(SM.createFileID(std::move(buffer)));

	clang::DiagnosticConsumer* client = c->getDiagnostics().getClient();
	client->BeginSourceFile(c->getLangOpts(), &c->getPreprocessor());

	ParseAST(c->getSema());

	client->EndSourceFile();

	HandleDiagnostics(res);

	if (client->getNumErrors() != 0)
	{
		res->Kind = ParserResultKind_Error;
		return res;
	}

	auto& AST = c->getASTContext();
	auto TU = AST.getTranslationUnitDecl();
	auto walker = new ASTWalker(pComClang, c.get());
	walker->WalkTranslationUnitDecl(TU);

	res->Kind = ParserResultKind_Success;
	return res;
}

static ArchType ConvertArchType(unsigned int archType)
{
	switch (archType)
	{
	case llvm::Triple::ArchType::x86:
		return ArchType_x86;
	case llvm::Triple::ArchType::x86_64:
		return ArchType_x86_64;
	}
	return ArchType_UnknownArch;
}

template<class ELFT>
static void ReadELFDependencies(const llvm::object::ELFFile<ELFT>* ELFFile, INativeLibraryPtr & NativeLib)
{
	ELFDumper<ELFT> ELFDumper(ELFFile);
	for (const auto& Dependency : ELFDumper.getNeededLibraries())
		NativeLib->AddDependencies(Dependency.str().c_str());
}

ParserResultKind Parser::ParseSharedLib(llvm::StringRef File,
	llvm::object::ObjectFile* ObjectFile,
	INativeLibraryPtr & NativeLib)
{
	auto LibName = File;
	NativeLib = pComClang->CreateNativeLibrary();
	NativeLib->FileName = LibName.str().c_str();
	NativeLib->_ArchType = ConvertArchType(ObjectFile->getArch());

	if (ObjectFile->isELF())
	{
		auto IDyn = llvm::cast<llvm::object::ELFObjectFileBase>(ObjectFile)->getDynamicSymbolIterators();
		for (auto it = IDyn.begin(); it != IDyn.end(); ++it)
		{
			std::string Sym;
			llvm::raw_string_ostream SymStream(Sym);

			if (it->printName(SymStream))
				continue;

			SymStream.flush();
			if (!Sym.empty())
				NativeLib->AddSymbols(Sym.c_str());
		}
		if (auto ELFObjectFile = llvm::dyn_cast<llvm::object::ELF32LEObjectFile>(ObjectFile))
		{
			ReadELFDependencies(ELFObjectFile->getELFFile(), NativeLib);
		}
		else if (auto ELFObjectFile = llvm::dyn_cast<llvm::object::ELF32BEObjectFile>(ObjectFile))
		{
			ReadELFDependencies(ELFObjectFile->getELFFile(), NativeLib);
		}
		else if (auto ELFObjectFile = llvm::dyn_cast<llvm::object::ELF64LEObjectFile>(ObjectFile))
		{
			ReadELFDependencies(ELFObjectFile->getELFFile(), NativeLib);
		}
		else if (auto ELFObjectFile = llvm::dyn_cast<llvm::object::ELF64BEObjectFile>(ObjectFile))
		{
			ReadELFDependencies(ELFObjectFile->getELFFile(), NativeLib);
		}
		return ParserResultKind_Success;
	}

	if (ObjectFile->isCOFF())
	{
		auto COFFObjectFile = static_cast<llvm::object::COFFObjectFile*>(ObjectFile);
		for (auto ExportedSymbol : COFFObjectFile->export_directories())
		{
			llvm::StringRef Symbol;
			if (!ExportedSymbol.getSymbolName(Symbol))
				NativeLib->AddSymbols(Symbol.str().c_str());
		}
		for (auto ImportedSymbol : COFFObjectFile->import_directories())
		{
			llvm::StringRef Name;
			if (!ImportedSymbol.getName(Name) && (Name.endswith(".dll") || Name.endswith(".DLL")))
				NativeLib->AddDependencies(Name.str().c_str());
		}
		return ParserResultKind_Success;
	}

	if (ObjectFile->isMachO())
	{
		auto MachOObjectFile = static_cast<llvm::object::MachOObjectFile*>(ObjectFile);
		for (const auto& Load : MachOObjectFile->load_commands())
		{
			if (Load.C.cmd == llvm::MachO::LC_ID_DYLIB ||
				Load.C.cmd == llvm::MachO::LC_LOAD_DYLIB ||
				Load.C.cmd == llvm::MachO::LC_LOAD_WEAK_DYLIB ||
				Load.C.cmd == llvm::MachO::LC_REEXPORT_DYLIB ||
				Load.C.cmd == llvm::MachO::LC_LAZY_LOAD_DYLIB ||
				Load.C.cmd == llvm::MachO::LC_LOAD_UPWARD_DYLIB)
			{
				auto dl = MachOObjectFile->getDylibIDLoadCommand(Load);
				auto lib = llvm::sys::path::filename(Load.Ptr + dl.dylib.name);
				NativeLib->AddDependencies(lib.str().c_str());
			}
		}
		auto Error = llvm::Error::success();
		for (const auto& Entry : MachOObjectFile->exports(Error))
		{
			NativeLib->AddSymbols(Entry.name().str().c_str());
		}
		if (Error)
		{
			return ParserResultKind_Error;
		}
		return ParserResultKind_Success;
	}

	return ParserResultKind_Error;
}

ParserResultKind Parser::ReadSymbols(llvm::StringRef File,
	llvm::object::basic_symbol_iterator Begin,
	llvm::object::basic_symbol_iterator End,
	INativeLibraryPtr & NativeLib)
{
	auto LibName = File;
	NativeLib = pComClang->CreateNativeLibrary();
	NativeLib->FileName = LibName.str().c_str();

	for (auto it = Begin; it != End; ++it)
	{
		std::string Sym;
		llvm::raw_string_ostream SymStream(Sym);

		if (it->printName(SymStream))
			continue;

		SymStream.flush();
		if (!Sym.empty())
			NativeLib->AddSymbols(Sym.c_str());
	}

	return ParserResultKind_Success;
}

IParserResultPtr Parser::ParseLibrary(const std::string& File)
{
	IParserResultPtr res = pComClang->CreateParserResult();
	if (File.empty())
	{
		res->Kind = ParserResultKind_FileNotFound;
		return res;
	}

	llvm::StringRef FileEntry;

	for (unsigned I = 0, E = opts->LibraryDirsCount; I != E; ++I)
	{
		const auto& LibDir = std::string(opts->GetLibraryDirs(I));
		llvm::SmallString<256> Path(LibDir);
		llvm::sys::path::append(Path, File);

		if (!(FileEntry = Path.str()).empty() && llvm::sys::fs::exists(FileEntry))
			break;
	}

	if (FileEntry.empty())
	{
		res->Kind = ParserResultKind_FileNotFound;
		return res;
	}

	auto BinaryOrErr = llvm::object::createBinary(FileEntry);
	if (!BinaryOrErr)
	{
		res->Kind = ParserResultKind_Error;
		return res;
	}
	auto OwningBinary = std::move(BinaryOrErr.get());
	auto Bin = OwningBinary.getBinary();
	if (auto Archive = llvm::dyn_cast<llvm::object::Archive>(Bin)) {
		INativeLibraryPtr library = res->Library;
		res->Kind = ParseArchive(File, Archive, library);
		res->Library = library;
		if (res->Kind == ParserResultKind_Success)
			return res;
	}
	if (auto ObjectFile = llvm::dyn_cast<llvm::object::ObjectFile>(Bin))
	{
		INativeLibraryPtr library = res->Library;
		res->Kind = ParseSharedLib(File, ObjectFile, library);
		res->Library = library;
		if (res->Kind == ParserResultKind_Success)
			return res;
	}
	res->Kind = ParserResultKind_Error;
	return res;
}

IParserResultPtr Parser::ParseHeader(IComClangPtr pComClang, ICppParserOptionsPtr Opts)
{
	if (!Opts)
		return nullptr;

	Parser Parser(pComClang, Opts);
	std::vector<std::string> SourceFiles;
	for (int i = 0; i < Opts->SourceFilesCount; i++) {
		SourceFiles.push_back(std::string(Opts->GetSourceFiles(i)));
	}
	return Parser.ParseHeader(SourceFiles);
}

IParserResultPtr Parser::ParseLibrary(IComClangPtr pComClang, ICppParserOptionsPtr Opts)
{
	if (!Opts)
		return nullptr;

	Parser Parser(pComClang, Opts);
	return Parser.ParseLibrary(std::string(Opts->LibraryFile));
}

IParserResultPtr Parser::ParseForAST(IComClangPtr pComClang, ICppParserOptionsPtr Opts)
{
	if (!Opts)
		return nullptr;

	Parser Parser(pComClang, Opts);
	std::vector<std::string> SourceFiles;
	for (int i = 0; i < Opts->SourceFilesCount; i++) {
		SourceFiles.push_back(std::string(Opts->GetSourceFiles(i)));
	}
	return Parser.ParseForAST(SourceFiles);
}

IParserTargetInfoPtr Parser::GetTargetInfo()
{
	IParserTargetInfoPtr parserTargetInfo = pComClang->CreateParserTargetInfo();

	auto& TI = c->getTarget();
	parserTargetInfo->ABI = TI.getABI().str().c_str();

	parserTargetInfo->Char16Type = ConvertIntType(TI.getChar16Type());
	parserTargetInfo->Char32Type = ConvertIntType(TI.getChar32Type());
	parserTargetInfo->Int64Type = ConvertIntType(TI.getInt64Type());
	parserTargetInfo->IntMaxType = ConvertIntType(TI.getIntMaxType());
	parserTargetInfo->IntPtrType = ConvertIntType(TI.getIntPtrType());
	parserTargetInfo->SizeType = ConvertIntType(TI.getSizeType());
	parserTargetInfo->UIntMaxType = ConvertIntType(TI.getUIntMaxType());
	parserTargetInfo->WCharType = ConvertIntType(TI.getWCharType());
	parserTargetInfo->WIntType = ConvertIntType(TI.getWIntType());

	parserTargetInfo->BoolAlign = TI.getBoolAlign();
	parserTargetInfo->BoolWidth = TI.getBoolWidth();
	parserTargetInfo->CharAlign = TI.getCharAlign();
	parserTargetInfo->CharWidth = TI.getCharWidth();
	parserTargetInfo->Char16Align = TI.getChar16Align();
	parserTargetInfo->Char16Width = TI.getChar16Width();
	parserTargetInfo->Char32Align = TI.getChar32Align();
	parserTargetInfo->Char32Width = TI.getChar32Width();
	parserTargetInfo->HalfAlign = TI.getHalfAlign();
	parserTargetInfo->HalfWidth = TI.getHalfWidth();
	parserTargetInfo->FloatAlign = TI.getFloatAlign();
	parserTargetInfo->FloatWidth = TI.getFloatWidth();
	parserTargetInfo->DoubleAlign = TI.getDoubleAlign();
	parserTargetInfo->DoubleWidth = TI.getDoubleWidth();
	parserTargetInfo->ShortAlign = TI.getShortAlign();
	parserTargetInfo->ShortWidth = TI.getShortWidth();
	parserTargetInfo->IntAlign = TI.getIntAlign();
	parserTargetInfo->IntWidth = TI.getIntWidth();
	parserTargetInfo->IntMaxTWidth = TI.getIntMaxTWidth();
	parserTargetInfo->LongAlign = TI.getLongAlign();
	parserTargetInfo->LongWidth = TI.getLongWidth();
	parserTargetInfo->LongDoubleAlign = TI.getLongDoubleAlign();
	parserTargetInfo->LongDoubleWidth = TI.getLongDoubleWidth();
	parserTargetInfo->LongLongAlign = TI.getLongLongAlign();
	parserTargetInfo->LongLongWidth = TI.getLongLongWidth();
	parserTargetInfo->PointerAlign = TI.getPointerAlign(0);
	parserTargetInfo->PointerWidth = TI.getPointerWidth(0);
	parserTargetInfo->WCharAlign = TI.getWCharAlign();
	parserTargetInfo->WCharWidth = TI.getWCharWidth();
	parserTargetInfo->Float128Align = TI.getFloat128Align();
	parserTargetInfo->Float128Width = TI.getFloat128Width();

	return parserTargetInfo;
}

IDeclarationPtr Parser::GetDeclarationFromFriend(clang::NamedDecl* FriendDecl)
{
	IDeclarationPtr Decl = WalkDeclarationDef(FriendDecl);
	if (!Decl) return nullptr;

	int MinLineNumberStart = std::numeric_limits<int>::max();
	int MinLineNumberEnd = std::numeric_limits<int>::max();
	auto& SM = c->getSourceManager();
	for (auto it = FriendDecl->redecls_begin(); it != FriendDecl->redecls_end(); it++)
	{
		if (it->getLocation() != FriendDecl->getLocation())
		{
			auto DecomposedLocStart = SM.getDecomposedLoc(it->getLocation());
			int NewLineNumberStart = SM.getLineNumber(DecomposedLocStart.first, DecomposedLocStart.second);
			auto DecomposedLocEnd = SM.getDecomposedLoc(it->getLocEnd());
			int NewLineNumberEnd = SM.getLineNumber(DecomposedLocEnd.first, DecomposedLocEnd.second);
			if (NewLineNumberStart < MinLineNumberStart)
			{
				MinLineNumberStart = NewLineNumberStart;
				MinLineNumberEnd = NewLineNumberEnd;
			}
		}
	}
	if (MinLineNumberStart < std::numeric_limits<int>::max())
	{
		Decl->LineNumberStart = MinLineNumberStart;
		Decl->LineNumberEnd = MinLineNumberEnd;
	}
	return Decl;
}

extern "C" __declspec(dllexport) IParserResult* Parser_ParseHeader(IComClang* pComClang, ICppParserOptions* pOpts)
{
	return Parser::ParseHeader(pComClang, pOpts);
}

extern "C" __declspec(dllexport) IParserResult* Parser_ParseLibrary(IComClang* pComClang, ICppParserOptions* pOpts)
{
	return Parser::ParseLibrary(pComClang, pOpts);
}

extern "C" __declspec(dllexport) IParserResult* Parser_ParseForAST(IComClang* pComClang, ICppParserOptions* pOpts)
{
	return Parser::ParseForAST(pComClang, pOpts);
}
