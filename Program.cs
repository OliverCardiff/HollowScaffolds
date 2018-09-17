using System;

namespace Hollow_Scaffs
{
	class MainClass
	{
		public static int LOWCOV = 50;
		public static int HIGHCOV = 95;
		public static int RANGE = 20;

		public static void Main (string[] args)
		{
			string gen_file = "";
			string wig_file = "";
			string hollows = "";

			try {
				gen_file = args[0];
				wig_file = args [1];
				hollows = args [2];
			} catch (Exception e) {
				Console.WriteLine ("Input Error!");
				Console.WriteLine (e.Message);
				Console.WriteLine ("usage: Hollow_Scaffs.exe <genome.fasta> <wigfile> <hollows.fasta>");
				return;
			}

			Console.WriteLine ("Reading Genome..");
			Genome genome = new Genome (gen_file, wig_file);
			genome.LoadGenome ();
			Console.WriteLine ("Reading Wig..");
			genome.LoadWigs ();
			Console.WriteLine ("Generating all non-collapse alleles\n");
			genome.GenerateAllAlleles ();
			Console.WriteLine ("Writing out Hollows file to: " + hollows + "..\n");
			genome.WriteHollowFile (hollows);
			Console.WriteLine ("Done");
		}
	}
}
