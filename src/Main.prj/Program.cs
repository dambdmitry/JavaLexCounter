
using System;
using System.Collections.Generic;

namespace Main
{
	/// <summary>Run analyzer.</summary>
	public class Program
	{
		public static char ChEOL = '\n';
		public static char ChEOT = (char)4;
		public static char ChSPACE = ' ';
		public static char ChTAB = '\t';
		//public static string[] files; //Массив путей к файлам.
		static void Main(string[] args)
		{
			int CountLex = 0;
			List<string> allFiles = new List<string>();
			Init(args, allFiles);

			foreach(var file in allFiles)
			{
				Text.ResetText(file);
				CountLex += Scan.CountLex();
			}

			Console.WriteLine();
			Console.WriteLine("Общее количество лексем: " + CountLex);
			
		}

		/// <summary>Init all analyzer files. </summary>
		/// <param name="args">All files.</param>
		/// <param name="allFile">There are writing files.</param>
		static void Init(string[] args, List<string> allFile)
		{
			Console.WriteLine("Лексический анализатор языка программирования Java");
			if(args.Length == 0)
			{
				Error.GlobalError("Запуск программы: Main.exe <Файл программы>");
			}
			else
			{
				foreach(string fileName in args)
				{
					string[] allFiles = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), fileName);
					if(allFiles.Length != 0)
					{
						foreach(string name in allFiles)
						{
							allFile.Add(name);	
						}	
					}
					else
					{
						Error.GlobalError($"Ошибка открытия файла {fileName}");
					}
				}	
			}
		}
	}
}
