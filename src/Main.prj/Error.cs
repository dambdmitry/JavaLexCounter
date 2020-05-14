using System;


namespace Main
{
	class Error
	{
		/// <summary>Error output.</summary>
		/// <param name="msg">Error text.</param>
		/// <param name="pos">Place where found error.</param>
		private static void _error(string msg, string pos)
		{
			while(Text.Ch() != Text.ChEOL && Text.Ch() != Text.ChEOT)
			{
				Text.NextCh();
			}
			Console.WriteLine();
			if(pos == "")
			{
				Console.WriteLine(pos + '^');
			}
			else
			{
				Console.WriteLine(pos.Remove(pos.Length - 1) + '^');
			}
			Console.Write(msg);
			Environment.Exit(1);
		}

		/// <summary>Error for lex errors. </summary>
		/// <param name="msg">Error text.</param>
		public static void LexError(string msg)
		{
			_error(msg, Loc.Pos);
		}
		/// <summary>Error for not lex errors. </summary>
		/// <param name="msg">Error text. </param>
		public static void GlobalError(string msg)
		{
			Console.WriteLine();
			Console.WriteLine(msg);
			Environment.Exit(2);
		}
	}
}
