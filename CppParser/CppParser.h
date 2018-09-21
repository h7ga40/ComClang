/************************************************************************
*
* CppSharp
* Licensed under the simplified BSD license. All rights reserved.
*
************************************************************************/

#pragma once

#include "AST.h"
#include "Helpers.h"

#include <llvm/Support/Host.h>
#include <llvm/Support/Path.h>
#include <llvm/Support/raw_ostream.h>
#include <llvm/Support/TargetSelect.h>
#include <llvm/Object/Archive.h>
#include <llvm/Object/COFF.h>
#include <llvm/Object/ObjectFile.h>
#include <llvm/Object/ELFObjectFile.h>
#include <llvm/Object/MachO.h>
#include <llvm/Object/SymbolicFile.h>
#include <llvm/Option/ArgList.h>
#include <llvm/IR/LLVMContext.h>
#include <llvm/IR/Module.h>
#include <llvm/IR/DataLayout.h>
#include <clang/Basic/Version.h>
#include <clang/Config/config.h>
#include <clang/AST/ASTContext.h>
#include <clang/AST/Comment.h>
#include <clang/AST/DeclFriend.h>
#include <clang/AST/ExprCXX.h>
#include <clang/Lex/DirectoryLookup.h>
#include <clang/Lex/HeaderSearch.h>
#include <clang/Lex/Preprocessor.h>
#include <clang/Lex/PreprocessorOptions.h>
#include <clang/Lex/PreprocessingRecord.h>
#include <clang/Parse/ParseAST.h>
#include <clang/Sema/Sema.h>
#include <clang/Sema/SemaConsumer.h>
#include <clang/Frontend/Utils.h>
#include <clang/Driver/Driver.h>
#include <clang/Driver/ToolChain.h>
#include <clang/Driver/Util.h>
#include <clang/Index/USRGeneration.h>
#include <clang/AST/ASTFwd.h>
#include <clang/AST/DeclTemplate.h>
#include <clang/AST/Type.h>
#include <clang/Basic/TargetInfo.h>
#include <clang/Frontend/CompilerInstance.h>
#include <clang/Sema/Scope.h>
#include <CodeGen/CodeGenModule.h>
#include <CodeGen/CodeGenTypes.h>
#include <CodeGen/TargetInfo.h>
#include <CodeGen/CGCall.h>
#include <CodeGen/CGCXXABI.h>
#include <Driver/ToolChains/Linux.h>

#include "CXXABI.h"

#ifndef _WIN64
#import "..\\Debug\\IComClang32.tlb" no_namespace
#else
#import "..\\x64\\Debug\\IComClang64.tlb" no_namespace
#endif
#undef mix
#undef max
