using System;


namespace Main
{
	/// <summary>Static class for lexical analysis of numbers. </summary>
	public static class NumberAnalysis
	{
		/// <summary>Analysis of the hexadecimal number system. </summary>
		/// <returns>Right lex</returns>
		public static Enum HexadecimalScan()
		{
			//Current symbol = 'x' or 'X'.
			string hexadecimal = "0123456789ABCDEFabcdef"; //Hexadecimal number system.
			Text.NextCh(); //Skip 'x' or 'X'.
			//Check that the first character in the hexadecimal number system.
			if(!hexadecimal.Contains(Text.Ch()) && Text.Ch() != '\\')
			{
				Error.LexError("Неправильная запись числа в шестнадцатеричной системе счисления");
			}
			else if(Text.Ch() == '\\')
			{
				char check = Scan.UnicodeTranslate();
				if(hexadecimal.Contains(check))
				{
					Text.NextCh();
				}
				else
				{
					Error.LexError("Неправильная запись числа в шестнадцатеричной системе счисления");
				}
			}
			//If the conditions above are completed, then go through the number.
			while(hexadecimal.Contains(Text.Ch()) || Text.Ch() == '_' || Text.Ch() == '\\')
			{
				//If in number met Unicode sequence.
				if(Text.Ch() == '\\')
				{
					char check = Scan.UnicodeTranslate();
					//If it is hexadecimal digit.
					if(hexadecimal.Contains(check) || check == '_')
					{
						Text.NextCh();
					}
					//If it is postfix Long type.
					else if(check == 'l' || check == 'L')
					{
						Text.NextCh();
						return Scan.Lex.LONGLITERAL;
					}
					//Else it is next lex.
					else
					{
						Scan.Count++;
						Scan.NextLex(check);
						return Scan.Lex.INTLITERAL;
					}
				}
				//Else skip symbol.
				else
				{
					Text.NextCh();
				}
			}
			//When hexadecimal digits ended.
			if(Text.Ch() == 'l' || Text.Ch() == 'L')
			{
				Text.NextCh();
				return Scan.Lex.LONGLITERAL;
			}
			else
			{
				return Scan.Lex.INTLITERAL;
			}	
		}

		/// <summary>Analysis of a number in octal number system</summary>
		/// <returns>Right lex</returns>
		public static Enum OctalNumber()
		{
			//'0' skipped.
			string octal = "01234567"; //Octal number system.
			//Same as Hexadecimal.
			while(octal.Contains(Text.Ch()) || Text.Ch() == '_' || Text.Ch() == '\\')
			{
				if(Text.Ch() == '\\')
				{
					char check = Scan.UnicodeTranslate();
					if(octal.Contains(check) || check == '_')
					{
						Text.NextCh();
					}
					else if(check == 'l' || check == 'L')
					{
						Text.NextCh();
						return Scan.Lex.LONGLITERAL;
					}
					else
					{
						Scan.Count++;
						Scan.NextLex(check);
						return Scan.Lex.INTLITERAL;
					}
				}
				else
				{
					Text.NextCh();
				}
			}
			if(Text.Ch() == 'l' || Text.Ch() == 'L')
			{
				Text.NextCh();
				return Scan.Lex.LONGLITERAL;
			}
			else
			{
				return Scan.Lex.INTLITERAL;
			}
		}

		/// <summary>Decimal analysis, including real numbers.</summary>
		/// <returns>Right lex.</returns>
		public static Enum Number()
		{
			while(Char.IsDigit(Text.Ch()) || Text.Ch() == '_' || Text.Ch() == '\\')
			{
				//If Unicode sequence met.
				if(Text.Ch() == '\\')
				{
					char check = Scan.UnicodeTranslate();
					//If it is Exponent symbol (real number).
					if(check == 'e' || check == 'E')
					{
						return ExponentScan();
					}
					//If it is Fractional Part (real number).
					else if(check == '.')
					{
						return FractionalPart(false);
					}
					//If it is prefix Long type (int number).
					else if(check == 'L' || check == 'l')
					{
						Text.NextCh();
						return Scan.Lex.LONGLITERAL;
					}
					//If it is number - continue.
					else if(Char.IsDigit(check))
					{
						Text.NextCh();
					}
					//If it is next lex.
					else
					{
						Scan.Count++;
						Scan.NextLex(check);
						return Scan.Lex.INTLITERAL;
					}
				}
				else
				{
					Text.NextCh();
				}
			}
			//Similar to actions in the 'while'.
			if(Text.Ch() == '.')
			{
				return FractionalPart(false);
			}
			else if(Text.Ch() == 'e' || Text.Ch() == 'E')
			{
				return ExponentScan();		
			}	
			else if(Text.Ch() == 'L' || Text.Ch() == 'l')
			{
				Text.NextCh();
				return Scan.Lex.LONGLITERAL;
			}
			else
			{
				return Scan.Lex.INTLITERAL;
			}
		}
		public static Enum ExponentScan()
		{
			//Current symbol 'e' or 'E'.
			//Check that the next character is a sign or a number.
			if(Char.IsDigit(Text.ChNext()) || Text.ChNext() == '-' || Text.ChNext() == '+' || Text.ChNext() == '\\')
			{
				Text.NextCh();//Skip 'e' or 'E'.
				//If next character is sign.
				if(Text.Ch() == '+' || Text.Ch() == '-')
				{
					Text.NextCh();
				}
				//If next character is sign in Unicode sequence.
				else if(Text.Ch() == '\\')
				{
					char check = Scan.UnicodeTranslate();
					if(check == '+' || check == '-' || Char.IsDigit(check))
					{
						Text.NextCh();
					}
					else
					{
						Error.LexError("После экспоненты должно быть число");
					}
				}
				//If there is no number after the exponent.
				if(!Char.IsDigit(Text.Ch()) && Text.Ch() != '\\')
				{
					Error.LexError("После экспоненты должно быть число");
				}

				while(Char.IsDigit(Text.Ch()) || (Text.Ch() == '_' && Char.IsDigit(Text.ChNext())) || Text.Ch() == '\\')
				{
					//Familiar actions, but with real numbers.
					if(Text.Ch() == '\\')
					{
						char check = Scan.UnicodeTranslate();
						if(Char.IsDigit(check) || check == '_' && Char.IsDigit(Text.ChNext()))
						{
							Text.NextCh();
						}
						else if(check == 'f' || check == 'F')
						{
							Text.NextCh();
							return Scan.Lex.FLOATLITERAL;
						}
						else if(check == 'd' || check == 'D')
						{
							Text.NextCh();
							return Scan.Lex.DOUBLELITERAL;
						}
						else
						{
							Scan.Count++;
							Scan.NextLex(check);
							return Scan.Lex.DOUBLELITERAL;
						}
					}
					else
					{
						Text.NextCh();
					}
				}
				if(Text.Ch() == 'f' || Text.Ch() == 'F')
				{
					Text.NextCh();
					return Scan.Lex.FLOATLITERAL;
				}
				else if(Text.Ch() == 'd' || Text.Ch() == 'D')
				{
					Text.NextCh();
					return Scan.Lex.DOUBLELITERAL;
				}
				else
				{
					return Scan.Lex.DOUBLELITERAL;
				}
			}
			//If there is no number or sign after the exponent.
			else
			{
				Text.NextCh();
				Error.LexError("За экспонентой должен быть знак, либо число");
				return null;
			}
		}

		/// <summary>Fractional part </summary>
		/// <param name="isZero">Is there a number before the dot</param>
		/// <returns>Right lex</returns>
		public static Enum FractionalPart(bool isZero)
		{
			//Current character is '.'
			Text.NextCh();//Skip '.'
			if(Char.IsDigit(Text.Ch()) || Text.Ch() == '\\')
			{
				if(Text.Ch() == '\\')
				{
					//Familar actions.
					char check = Scan.UnicodeTranslate();
					if(Char.IsDigit(check))
					{
						Text.NextCh();
					}
					else if((check == 'f' || check == 'F') && !isZero)
					{
						Text.NextCh();
						return Scan.Lex.FLOATLITERAL;
					}
					else if(check == 'd' || check == 'D' && !isZero)
					{
						Text.NextCh();
						return Scan.Lex.DOUBLELITERAL;
					}
					else if((check == 'e' || check == 'E') && !isZero)
					{
						return ExponentScan();
					}
					else if(isZero)
					{
						Scan.Count++;
						Scan.NextLex(check);
						return Scan.Lex.DOT;
					}
					else
					{
						Scan.Count++;
						Scan.NextLex(check);
						return Scan.Lex.DOUBLELITERAL;
					}
				}
				else
				{
					Text.NextCh();
				}

				while(Char.IsDigit(Text.Ch()) || (Text.Ch() == '_' && Char.IsDigit(Text.ChNext())) || Text.Ch() == '\\')
				{
					if(Text.Ch() == '\\')
					{
						char check = Scan.UnicodeTranslate();
						if(check == '_' && Char.IsDigit(Text.ChNext()) || Char.IsDigit(check))
						{
							Text.NextCh();
						}
						else if(check == 'e' || check == 'E')
						{
							return ExponentScan();
						}
						else if(check == 'f' || check == 'F')
						{
							Text.NextCh();
							return Scan.Lex.FLOATLITERAL;
						}
						else if(check == 'd' || check == 'D')
						{
							Text.NextCh();
							return Scan.Lex.DOUBLELITERAL;
						}
						else
						{
							Scan.Count++;
							Scan.NextLex(check);
							return Scan.Lex.DOUBLELITERAL;
						}
					}
					Text.NextCh();
				}

				if((Text.Ch() == 'e' || Text.Ch() == 'E'))
				{
					return ExponentScan();	
				}
				else if(Text.Ch() == 'f' || Text.Ch() == 'F')
				{
					Text.NextCh();
					return Scan.Lex.FLOATLITERAL;
				}
				else if(Text.Ch() == 'd' || Text.Ch() == 'D')
				{
					Text.NextCh();
					return Scan.Lex.DOUBLELITERAL;
				}
				else
				{
					return Scan.Lex.DOUBLELITERAL;
				}
			}
			else if((Text.Ch() == 'F' || Text.Ch() == 'f') && !isZero)
			{
				Text.NextCh();
				return  Scan.Lex.FLOATLITERAL;
			}
			else if((Text.Ch() == 'D' || Text.Ch() == 'd') && !isZero)
			{
				Text.NextCh();
				return Scan.Lex.DOUBLELITERAL;
			}
			else if((Text.Ch() == 'e' || Text.Ch() == 'e') && !isZero)
			{
				return ExponentScan();
			}
			else if(!isZero)
			{
				return Scan.Lex.DOUBLELITERAL;
			}
			else
			{
				return Scan.Lex.DOT;
			}
		}
	}
}
				
