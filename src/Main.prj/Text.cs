
using System;
using System.IO;

namespace Main
{
	/// <summary>Class controlling the text of the analyzed program</summary>
	public class Text : Program
	{
		private static string _src = ""; //All text of program.
		private static int _i = 0; //Current character position.
		private static char _ch = (char) 0; //Current character.
		private static char _prevCh = (char)0; //Previous character.

		/// <summary>Open program file and writes it to _src variable.</summary>
		/// <param name="file">File path.</param>
		public static void ResetText(string file)
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

			using(StreamReader streamReader = new StreamReader(path))
			{
				_src = streamReader.ReadToEnd();
				_i = 0;
			}
		}

		/// <summary>Moves the current character to next character of _src</summary>
		public static void NextCh()
		{
			if(_i < _src.Length)
			{
				_prevCh = _ch;
				_ch = _src[_i];
				Console.Write(_ch);
				if(_ch == ChTAB)
				{
					Loc.Pos += ChTAB;
				}
				else
				{
					Loc.Pos += ChSPACE;
				}
				_i++;
				if(_ch == '\n' || _ch == '\r'){
					_ch = ChEOL;
					Loc.Pos = "";
				}
			}
			else
			{
				_ch = ChEOT;
			}
		}

		/// <summary>Gives the next character, but does not go to it</summary>
		/// <returns>Next character.</returns>
		public static char ChNext()
		{
			if(_i < _src.Length)
			{
				return _src[_i];
			}
			else
			{
				return ChEOT;
			}
		}

		/// <summary></summary>
		/// <returns>Previous character</returns>
		public static char PrevCh()
		{
			return _prevCh;
		}

		/// <summary></summary>
		/// <returns>Current character.</returns>
		public static char Ch()
		{
			return _ch;
		}
	}
}
