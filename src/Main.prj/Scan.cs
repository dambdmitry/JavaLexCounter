using System;
using System.Collections.Generic;


namespace Main
{
	public static class Scan
	{
		/// <summary>Decryption of lexs can be seen in the method "NextLex".</summary>
		public enum Lex
		{
			ONE, OF, ABSTRACT, DEFAULT, IF, PRIVATE, THROW, BOOLEAN, DO, IMPLEMENTS, PROTECTED, THROWS, BREAK,
			DOUBLE, IMPORT, PUBLIC, TRANSIENT, BYTE, ELSE, INSTANCEOF, RETURN, TRY, CASE, EXTENDS, INT, SHORT, 
			VOID, CATCH, FINAL, INTERFACE, STATIC, VOLATILE, CHAR, FINALLY, LONG, SUPER, WHILE, CLASS, FLOAT,
			NATIVE, SWITCH, CONST, FOR, NEW, SYNCHRONIZED, CONTINUE, GOTO, PACKAGE, THIS, NAME, EOT, NONE, LPAR, RPAR,
			LBRACE,  RBRACE, LSQUAREPAR, RSQUAREPAR, COMMA, DOT, SEMI, DIV, ASS, EQ, GE, GT, LE, LT, NE, INVERSION,
			BITADD, TERNARYIF, TERNARYELSE, AND, ASSBITAND, BITAND, OR, ASSBITOR, BITOR, INC, ASSADD, PLUS, DEC,
			ASSMINUS, MINUS, ASSMULT, MULT, ASSDIV, ASSXOR, XOR, ASSMOD, MOD, LSHIFT, ASSLSHIFT, ASSRSHIFT, ASSZEROSHIFT,
			ZEROSHIFT, RSHIFT, INTLITERAL, FLOATLITERAL, DOUBLELITERAL, LONGLITERAL, BOOLEANLITERAL, SINGLECHAR, CHARLITERAL,
			NULL, STRINGLITERAL
		}
		public static int Count; //Bad example. It is count of lex.
		private static Enum _lex = Lex.NONE; //Current lex.

		private static Dictionary<string, Enum> _kw = new Dictionary<string, Enum> // Dictionary of Java keywords.
		{
			["one"] = Lex.ONE, ["of"] = Lex.OF, ["abstract"] = Lex.ABSTRACT, ["default"] = Lex.DEFAULT, ["if"] = Lex.IF,
			["private"] = Lex.PRIVATE, ["throw"] = Lex.THROW, ["boolean"] = Lex.BOOLEAN, ["do"] = Lex.DO,
			["implements"] = Lex.IMPLEMENTS, ["protected"] = Lex.PROTECTED, ["throws"] = Lex.THROWS, ["break"] = Lex.BREAK,
			["double"] = Lex.DOUBLE, ["import"] = Lex.IMPORT, ["public"] = Lex.PUBLIC, ["transient"] = Lex.TRANSIENT,
			["byte"] = Lex.BYTE, ["else"] = Lex.ELSE, ["instanceof"] = Lex.INSTANCEOF, ["return"] = Lex.RETURN,
			["try"] = Lex.TRY, ["case"] = Lex.CASE, ["extends"] = Lex.EXTENDS, ["int"] = Lex.INT, ["short"] = Lex.SHORT,
			["void"] = Lex.VOID, ["catch"] = Lex.CATCH, ["final"] = Lex.FINAL, ["interface"] = Lex.INTERFACE,
			["static"] = Lex.STATIC, ["volatile"] = Lex.VOLATILE, ["char"] = Lex.CHAR, ["finally"] = Lex.FINALLY,
			["long"] = Lex.LONG, ["super"] = Lex.SUPER, ["while"] = Lex.WHILE, ["class"] = Lex.CLASS, ["float"] = Lex.FLOAT,
			["native"] = Lex.NATIVE, ["switch"] = Lex.SWITCH, ["const"] = Lex.CONST, ["for"] = Lex.FOR, ["new"] = Lex.NEW,
			["synchronized"] = Lex.SYNCHRONIZED, ["continue"] = Lex.CONTINUE, ["goto"] = Lex.GOTO, ["package"] = Lex.PACKAGE,
			["this"] = Lex.THIS
		};

		private static Dictionary<string, Enum> _literals = new Dictionary<string, Enum> //Dictionary of Java literals.
		{
			["true"] = Lex.BOOLEANLITERAL,
			["false"] = Lex.BOOLEANLITERAL,
			["null"] = Lex.NULL
		};

		/// <summary>Identifier literal Scan (and keywords). </summary>
		/// <param name="ch">Current symbol.</param>
		private static void ScanIdent(char ch)
		{
			string name = "";
			name += ch;
			Text.NextCh();
			while(Char.IsLetter(Text.Ch()) || Char.IsDigit(Text.Ch()) || Text.Ch() == '\u005f' || Text.Ch() == '\u0024' || Text.Ch() == '\\')
			{
				if(Text.Ch() == '\\')
				{
					char check = UnicodeTranslate();
					if(Char.IsLetter(check) || Char.IsDigit(check) || check == '\u005f' || check == '\u0024')
					{
						name += check;
						Text.NextCh();
					}
					else
					{
						Count++;
						NextLex(check);
						break;
					}
				}
				else
				{
					name += Text.Ch();
					Text.NextCh();
				}
			}
			if(_kw.ContainsKey(name))
			{
				_lex = _kw[name];
			}
			else if(_literals.ContainsKey(name))
			{
				_lex = _literals[name];
			}
			else
			{
				_lex = Lex.NAME;
			}
		}

		/// <summary>Translates unicode sequence to character</summary>
		/// <returns>Unicode character.</returns>
		public static char UnicodeTranslate()
		{
			string unicodeSequence = "";
			string hexadecimal = "0123456789ABCDEFabcdef";
			Text.NextCh();
			if(Text.Ch() != 'u')
			{
				Error.LexError("Неправильная запись Unicode последовательности");
			}
			Text.NextCh();
			for(int i = 1; i <= 4; i++) 
			{
				if(hexadecimal.Contains(Text.Ch()))
				{
					unicodeSequence += Text.Ch();
					if(i != 4)
					{
						Text.NextCh();
					}
				}
				else
				{
					Error.LexError("Неправильная запись Unicode последовательности");
				}
			}
			string buf = "";
			try
			{
				int a = Convert.ToInt32(unicodeSequence, 16);
				buf = Char.ConvertFromUtf32(a);
			}
			catch
			{
				Error.LexError("Недопустимое значение Unicode последовательности.");
			}
			char unicodeChar = Convert.ToChar(buf);
			if(unicodeChar == '\u005c')
			{
				Error.LexError("Символ наклонной черты недопустим без Unicode последовательности.");
			}
			return unicodeChar;
		}

		public static void Comment()
		{
			// ch = '*' or '/'.
			if(Text.Ch() == '/')
			{
				while(Text.Ch() != '\n' && Text.Ch() != '\r' && Text.Ch() != Text.ChEOT)
				{
					Text.NextCh();
				}
			}
			else
			{
				Text.NextCh();//Skip *
				Text.NextCh();//Skip first comment character.
				while((Text.PrevCh() != '*' || Text.Ch() != '/') && Text.Ch() != Text.ChEOT)
				{
					//Inserted comment.
					if(Text.PrevCh() == '/' && Text.Ch() == '*')
					{
						Comment();
					}
					Text.NextCh();
				}

				if(Text.Ch() == Text.ChEOT)
				{
					Error.LexError("Не законченный комментарий");
				}
				Text.NextCh();
				
			}
		}

		/// <summary>Character lex scan. </summary>
		public static void ScanChar()
		{
			//Current character = "'".
			Text.NextCh(); //Skip '

			if(Text.Ch() == '\\')
			{
				_lex = CharAnalysis.EscapeSequence();
			}
			else
			{
				_lex = CharAnalysis.SingleChar();
			}
		}

		/// <summary>String literal scan.</summary>
		public static void ScanString()
		{
			//Current character = "
			Text.NextCh(); //Skip "
			string octal = "01234567";
			if(Text.Ch() == '"')
			{
				Text.NextCh();
				_lex = Lex.STRINGLITERAL;
			}
			else
			{

				while(Text.Ch() != '"' && Text.ChNext() != '\n' && Text.ChNext() != '\r' && Text.Ch() != Text.ChEOT)
				{
					if(Text.Ch() == '\\')
					{
						Text.NextCh();
						if(Text.Ch() == 'u')
						{
							CharAnalysis.UnicodeString();
						}
						else if(Text.Ch() == 't' || Text.Ch() == 'r' || Text.Ch() == 'f' || Text.Ch() == 'b' || Text.Ch() == 'n')
						{
							Text.NextCh();
						}
						else if(Text.Ch() == '\\' || Text.Ch() == '"')
						{
							Text.NextCh();
						}
						else if(octal.Contains(Text.Ch()))
						{
							CharAnalysis.OctalCharScan();
						}
						else
						{
							Error.LexError("Неправильная запись ");
						}
					}
					else
					{
						Text.NextCh();
					}
				}
				if(Text.Ch() == '"')
				{
					Text.NextCh();
					_lex = Lex.STRINGLITERAL;
				}
				else
				{
					Error.LexError("Кавычка не закрыта");
				}
				
			}
		}

		/// <summary>Number literal scan.</summary>
		public static void ScanNumber()
		{
			//Text.Ch() is first digit.

			string octal = "01234567";
			if(Text.Ch() == '0')
			{
				Text.NextCh();
				if(Text.Ch() == 'x' || Text.Ch() == 'X')
				{
					_lex = NumberAnalysis.HexadecimalScan();
				}
				else if(octal.Contains(Text.Ch()))
				{
					_lex = NumberAnalysis.OctalNumber();
				}
				else if(Text.Ch() == 'L' || Text.Ch() == 'l')
				{
					Text.NextCh();
					_lex = Lex.LONGLITERAL;
				}
				else if(Text.Ch() == 'F' || Text.Ch() == 'f')
				{
					Text.NextCh();
					_lex = Lex.FLOATLITERAL;
				}
				else if(Text.Ch() == 'd' || Text.Ch() == 'D')
				{
					Text.NextCh();
					_lex = Lex.DOUBLELITERAL;
				}
				else if(Char.IsDigit(Text.Ch()))
				{
					_lex = Lex.INTLITERAL;
				}
				else if(Text.Ch() == '.')
				{
					Text.NextCh();
					_lex = NumberAnalysis.FractionalPart(false);
				}
				else if(Text.Ch() == 'e' || Text.Ch() == 'E')
				{
					_lex = NumberAnalysis.ExponentScan();
				}
				else if(Text.Ch() == '\\')
				{
					char check = UnicodeTranslate();
					if(check == 'x' || check == 'X')
					{
						_lex = NumberAnalysis.HexadecimalScan();
					}
					else if(octal.Contains(check))
					{
						Text.NextCh();
						_lex = NumberAnalysis.OctalNumber();
					}
					else if(check == 'L' || check == 'l')
					{
						Text.NextCh();
						_lex = Lex.LONGLITERAL;
					}
					else if(check == 'F' || check == 'f')
					{
						Text.NextCh();
						_lex = Lex.FLOATLITERAL;
					}
					else if(check == 'd' || check == 'D')
					{
						Text.NextCh();
						_lex = Lex.DOUBLELITERAL;
					}
					else if(Char.IsDigit(check))
					{
						Count++;
						NextLex(check);
						_lex = Lex.INTLITERAL;
					}
					else if(check == '.')
					{
						Text.NextCh();
						_lex = NumberAnalysis.FractionalPart(false);
					}
					else if(check == 'e' || check == 'E')
					{
						_lex = NumberAnalysis.ExponentScan();
					}
					else
					{
						Count++;
						NextLex(check);
						_lex = Lex.INTLITERAL;
					}
				}
				else
				{
					_lex = Lex.INTLITERAL;
				}
			}
			//If number in decimal number system.
			else
			{
				_lex = NumberAnalysis.Number();
			}
		}
		/// <summary>Skip lex and get value to _lex</summary>
		/// <param name="ch">Current character.</param>
		public static void NextLex(char ch)
		{
			//ch = Text.Ch().
			if(ch == Text.ChSPACE || ch == Text.ChTAB || ch == Text.ChEOL)
			{
				Text.NextCh();
				while(Text.Ch() == Text.ChSPACE || Text.Ch() == Text.ChTAB || Text.Ch() == Text.ChEOL)
				{
					Text.NextCh();
				}
				ch = Text.Ch();
			}	

			if(Char.IsLetter(ch) || ch == '\u005f' || ch == '\u0024')
			{
				ScanIdent(ch);
			}
			else if(ch == '/')
			{
				Text.NextCh();
				ch = Text.Ch();
				if(ch == '*' || ch == '/')
				{
					Comment();
					NextLex(Text.Ch());
				}
				else if(ch == '=')
				{
					_lex = Lex.ASSDIV; // /=.
					Text.NextCh();
				}
				else
				{
					_lex = Lex.DIV; // /.
				}
			}
			else if(ch == '(')
			{
				_lex = Lex.LPAR;
				Text.NextCh();
			}
			else if(ch == ')')
			{
				_lex = Lex.RPAR;
				Text.NextCh();
			}
			else if(ch == '{')
			{
				_lex = Lex.LBRACE;
				Text.NextCh();
			}
			else if(ch == '}')
			{
				_lex = Lex.RBRACE;
				Text.NextCh();
			}
			else if(ch == '[')
			{
				_lex = Lex.LSQUAREPAR;
				Text.NextCh();
			}
			else if(ch == ']')
			{
				_lex = Lex.RSQUAREPAR;
				Text.NextCh();
			}
			else if(ch == '.')
			{
				_lex = NumberAnalysis.FractionalPart(true);
			}
			else if(ch == ',')
			{
				_lex = Lex.COMMA;
				Text.NextCh();
			}
			else if(ch == ';')
			{
				_lex = Lex.SEMI;
				Text.NextCh();
			}
			else if(ch == '=')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.EQ;
					Text.NextCh();
				}
				else
				{
					_lex = Lex.ASS;
				}
			}
			else if(ch == '>')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.GE;
					Text.NextCh();
				}
				else if(Text.Ch() == '>')
				{
					Text.NextCh();
					if(Text.Ch() == '=')
					{
						_lex = Lex.ASSRSHIFT; // >>=
						Text.NextCh();
					}
					else if(Text.Ch() == '>')
					{
						Text.NextCh();
						if(Text.Ch() == '=')
						{
							_lex = Lex.ASSZEROSHIFT; // >>>= .
							Text.NextCh();
						}
						else
						{
							_lex = Lex.ZEROSHIFT; // >>>.
						}
					}
					else
					{
						_lex = Lex.RSHIFT; // >>.
					}
				}
				else
				{
					_lex = Lex.GT; // >=.
				}
			}
			else if(ch == '<')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.LE;
					Text.NextCh();
				}
				else if(Text.Ch() == '<')
				{
					Text.NextCh();
					if(Text.Ch() == '=')
					{
						_lex = Lex.ASSLSHIFT; // <<= .
						Text.NextCh();
					}
					else
					{
						_lex = Lex.LSHIFT; // <<.
					}
				}
				else
				{
					_lex = Lex.LT; // <=.
				}
			}
			else if(ch == '!')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.NE;
					Text.NextCh();
				}
				else
				{
					_lex = Lex.INVERSION;
				}
			}
			else if(ch == '~')
			{
				_lex = Lex.BITADD; //Bit addition.
				Text.NextCh();
			}
			else if(ch == '?')
			{
				_lex = Lex.TERNARYIF; //Ternary operator.
				Text.NextCh();
			}
			else if(ch == ':')
			{
				_lex = Lex.TERNARYELSE; //Ternary operator.
				Text.NextCh();
			}
			else if(ch == '&')
			{
				Text.NextCh();
				if(Text.Ch() == '&')
				{
					_lex = Lex.AND; // && .
					Text.NextCh();
				}
				else if(Text.Ch() == '=')
				{
					_lex = Lex.ASSBITAND; // &= .
					Text.NextCh();
				}
				else
				{
					_lex = Lex.BITAND; // &.
				}
			}
			else if(ch == '|')
			{
				Text.NextCh();
				if(Text.Ch() == '|')
				{
					_lex = Lex.OR; 
					Text.NextCh();
				}
				else if(Text.Ch() == '=')
				{
					_lex = Lex.ASSBITOR; 
					Text.NextCh();
				}
				else
				{
					_lex = Lex.BITOR; 
				}
			}
			else if(ch == '+')
			{
				Text.NextCh();
				if(Text.Ch() == '+')
				{
					_lex = Lex.INC; // ++.
					Text.NextCh();
				}
				else if(Text.Ch() == '=')
				{
					_lex = Lex.ASSADD; 
					Text.NextCh();
				}
				else
				{
					_lex = Lex.PLUS; // +.
				}
			}
			else if(ch == '-')
			{
				Text.NextCh();
				if(Text.Ch() == '-')
				{
					_lex = Lex.DEC; // --.
					Text.NextCh();
				}
				else if(Text.Ch() == '=')
				{
					_lex = Lex.ASSMINUS; // =-.
					Text.NextCh();
				}
				else
				{
					_lex = Lex.MINUS; // -.
				}
			}
			else if(ch == '*')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.ASSMULT; // *= .
					Text.NextCh();
				}
				else
				{
					_lex = Lex.MULT; //*.
				}
			}
			else if(ch == '^')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.ASSXOR; 
					Text.NextCh();
				}
				else
				{
					_lex = Lex.XOR; 
				}
			}
			else if(ch == '%')
			{
				Text.NextCh();
				if(Text.Ch() == '=')
				{
					_lex = Lex.ASSMOD; 
					Text.NextCh();
				}
				else
				{
					_lex = Lex.MOD; 
				}
			}
			else if(Char.IsDigit(ch)){
				ScanNumber();
			}
			else if(ch == (char)39)
			{
				ScanChar();
			}
			else if(ch == '"')
			{
				ScanString();
			}
			else if(ch == Text.ChEOT)
			{
				_lex = Lex.EOT;
			}
			else if(ch == '\\')
			{
				NextLex(UnicodeTranslate());
			}
			else
			{
				Error.LexError("Недопустимый символ");
			}
		}

		/// <summary>Count number of lex.</summary>
		/// <returns>Count lex.</returns>
		public static int CountLex()
		{
			Count = 0;
			Text.NextCh();
			NextLex(Text.Ch());
			while(_lex.CompareTo(Lex.EOT) != 0) {
				Count++;
				NextLex(Text.Ch());
			}
			return Count;
		}
	}		
}
