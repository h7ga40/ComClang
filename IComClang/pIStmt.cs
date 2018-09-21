using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	[ComVisible(true)]
	public enum StmtClass {
		NoStmt = 0,
#define STMT(CLASS, PARENT) CLASS,
#define STMT_RANGE(BASE, FIRST, LAST) \
		    first##BASE##Constant=FIRST, last##BASE##Constant=LAST,
#define LAST_STMT_RANGE(BASE, FIRST, LAST) \
		    first##BASE##Constant=FIRST, last##BASE##Constant=LAST
#define ABSTRACT_STMT(STMT)
#include "clang/AST/StmtNodes.inc"
	}

	
}
