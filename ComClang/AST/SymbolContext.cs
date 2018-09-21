﻿using System;
using System.Collections.Generic;

namespace CppSharp.AST
{
	/// <summary>
	/// Represents a shared library or a static library archive.
	/// </summary>
	public class NativeLibrary : INativeLibrary
	{
		public NativeLibrary(string file)
			: this()
		{
			FileName = file;
		}

		public NativeLibrary()
		{
			Symbols = new List<string>();
			Dependencies = new List<string>();
		}

		/// <summary>
		/// File name of the library.
		/// </summary>
		public string FileName { get; set; } = "";

		public ArchType ArchType { get; set; }

		/// <summary>
		/// Symbols gathered from the library.
		/// </summary>
		public IList<string> Symbols;

		public IList<string> Dependencies { get; private set; }

		public int SymbolsCount => Symbols.Count;

		public int DependenciesCount => Dependencies.Count;

		public string GetSymbols(int index)
		{
			return Symbols[index];
		}

		public void AddSymbols(string value)
		{
			Symbols.Add(value);
		}

		public string GetDependencies(int index)
		{
			return Dependencies[index];
		}

		public void AddDependencies(string value)
		{
			Dependencies.Add(value);
		}
	}

	public class SymbolContext
	{
		/// <summary>
		/// List of native libraries.
		/// </summary>
		public List<NativeLibrary> Libraries;

		/// <summary>
		/// Index of all symbols to their respective libraries.
		/// </summary>
		public Dictionary<string, NativeLibrary> Symbols;

		public SymbolContext()
		{
			Libraries = new List<NativeLibrary>();
			Symbols = new Dictionary<string, NativeLibrary>();
		}

		public NativeLibrary FindOrCreateLibrary(string file)
		{
			var library = Libraries.Find(m => m.FileName.Equals(file));

			if (library == null)
			{
				library = new NativeLibrary(file);
				Libraries.Add(library);
			}

			return library;
		}

		public void IndexSymbols()
		{
			foreach (var library in Libraries)
			{
				foreach (var symbol in library.Symbols)
				{
					if (!Symbols.ContainsKey(symbol))
						Symbols[symbol] = library;
					if (symbol.StartsWith("__", StringComparison.Ordinal))
					{
						string stripped = symbol.Substring(1);
						if (!Symbols.ContainsKey(stripped))
							Symbols[stripped] = library;
					}
				}
			}
		}

		public bool FindSymbol(ref string symbol)
		{
			NativeLibrary lib;

			if (FindLibraryBySymbol(symbol, out lib))
				return true;

			string alternativeSymbol;

			// Check for C symbols with a leading underscore.
			alternativeSymbol = "_" + symbol;
			if (FindLibraryBySymbol(alternativeSymbol, out lib))
			{
				symbol = alternativeSymbol;
				return true;
			}

			alternativeSymbol = symbol.TrimStart('_');
			if (FindLibraryBySymbol(alternativeSymbol, out lib))
			{
				symbol = alternativeSymbol;
				return true;
			}

			alternativeSymbol = "_imp_" + symbol;
			if (FindLibraryBySymbol(alternativeSymbol, out lib))
			{
				symbol = alternativeSymbol;
				return true;
			}

			alternativeSymbol = "__imp_" + symbol;
			if (FindLibraryBySymbol("__imp_" + symbol, out lib))
			{
				symbol = alternativeSymbol;
				return true;
			}

			return false;
		}

		public bool FindLibraryBySymbol(string symbol, out NativeLibrary library)
		{
			return Symbols.TryGetValue(symbol, out library);
		}
	}
}
