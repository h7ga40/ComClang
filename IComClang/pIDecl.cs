using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	[ComVisible(true)]
	public enum DeclKind {
#define DECL(DERIVED, BASE) DERIVED,
#define ABSTRACT_DECL(DECL)
#define DECL_RANGE(BASE, START, END) \
		first##BASE = START, last##BASE = END,
#define LAST_DECL_RANGE(BASE, START, END) \
		first##BASE = START, last##BASE = END
#include "clang/AST/DeclNodes.inc"
	}

	
}
