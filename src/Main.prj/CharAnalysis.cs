using System;

namespace Main
{
	/// <summary>Static character analysis class. </summary>
	public static class CharAnalysis
	{
		/// <summary>If character is not Unicode sequence.</summary>
		/// <returns>Right lex.</returns>
		public static Enum SingleChar()
		{
			//First "'" skipped.
			//" '' " - not allowed.
			if(Text.Ch() != (char)39)
			{
				Text.NextCh();
				ApostropheCheck();				
				return Scan.Lex.SINGLECHAR;

			}
			else
			{
				Error.LexError("Недопустимая запись символа.");
				return null;
			}
		}
		/// <summary>If character is unicode sequence. </summary>
		/// <returns>Right lex.</returns>
		public static Enum EscapeSequence()
		{
			string octal = "01234567"; //Octal number system.
			Text.NextCh(); //Skip \

			if(Text.Ch() == 'u')
			{
				HexadecimalCharScan();
				ApostropheCheck();
				return Scan.Lex.CHARLITERAL;	
			}
			else if(Text.Ch() == 't' || Text.Ch() == 'r' || Text.Ch() == 'f' || Text.Ch() == 'b' || Text.Ch() == 'n')
			{
				Text.NextCh();
				ApostropheCheck();			
				return Scan.Lex.CHARLITERAL;			
			}
			//If it is \\ or \" character.
			else if(Text.Ch() == '\\' || Text.Ch() == (char)39)
			{
				Text.NextCh();
				ApostropheCheck();
				return Scan.Lex.CHARLITERAL;
			}
			else if(octal.Contains(Text.Ch()))
			{
				OctalCharScan();
				ApostropheCheck();			
				return Scan.Lex.CHARLITERAL;
			}
			else
			{
				Error.LexError("Недопустимая запись символа");
				return null;
			}
		}

		/// <summary>Checks if the hexadecimal number is written correctly.</summary>
		public static void HexadecimalCharScan()
		{
			//Current character is u.
			Text.NextCh();
			string code = "";
			string hexadecimal = "0123456789ABCDEFabcdef"; //Символы шестнадцатеричной системы счисления.
			for(int i = 1; i <= 4; i++)
			{
				if(hexadecimal.Contains(Text.Ch()))
				{
					code += Text.Ch();
					if(i != 4)
					{
						Text.NextCh();
					}
				}
				else
				{
					Error.LexError("Неправильня запись Unicode последовательности");
				}
			}
			//Not allowed symbols.
			if(code != "000a" && code != "000A" && code != "000d" && code != "000D" && code == "0027")
			{
				Text.NextCh();
			}
			else if(code == "0027")
			{
				Error.LexError("Запись пустого символа недопустима");
			}
			else
			{
				Error.LexError("Недопустимая Unicode последовательность в символе");
			}
		}

		/// <summary>Checks if the octal number is written correctly. </summary>
		public static void OctalCharScan()
		{
			string octal = "01234567"; //Octal number system.
			string subOctal = "0123";
			//It is bad code :(
			if(subOctal.Contains(Text.Ch()))
			{
				Text.NextCh();
				if(octal.Contains(Text.Ch()))
				{
					Text.NextCh();
					if(octal.Contains(Text.Ch()))
					{
						Text.NextCh();
					}
				}
			}
			else if(octal.Contains(Text.Ch()))
			{
				Text.NextCh();
				if(octal.Contains(Text.Ch()))
				{
					Text.NextCh();
				}
			}
		}

		/// <summary>Checking the closing apostrophe.</summary>
		public static void ApostropheCheck()
		{
			if(Text.Ch() == (char)39)
			{
				Text.NextCh();
			}
			else if(Text.Ch() == '\\')
			{
				char check = Scan.UnicodeTranslate();
				if(check == (char)39)
				{
					Text.NextCh();
				}
				else
				{
					Error.LexError("Недопустимая запись символа");
				}
			}
			else
			{
				Error.LexError("Недопустимая запись символа");
			}
		}

		/// <summary>Correct Unicode writing for strings. </summary>
		public static void UnicodeString()
		{
			//Current character is u.
			Text.NextCh();
			string code = "";
			string hexadecimal = "0123456789ABCDEFabcdef"; //Hexadedimal number system.
			for(int i = 1; i <= 4; i++)
			{
				if(hexadecimal.Contains(Text.Ch()))
				{
					code += Text.Ch();
					if(i != 4)
					{
						Text.NextCh();
					}
				}
				else
				{
					Error.LexError("Неправильня запись Unicode последовательности");
				}
			}
			//Not allowed sequences.
			if(code != "000a" && code != "000A" && code != "000d" && code != "000D")
			{
				Text.NextCh();
			}
			else
			{
				Error.LexError("Недопустимая Unicode последовательность в строке");
			}
		}
	}	
}
