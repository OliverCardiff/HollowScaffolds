using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hollow_Scaffs
{
	public class Genome
	{
		public Dictionary<string, Scaffold> _scaffolds;
		string _gfile;
		string _wFile;
		string _bFile;

		public Genome (string file, string wFile)
		{
			_gfile = file;
			_wFile = wFile;
			_scaffolds = new Dictionary<string, Scaffold> ();
		}

		public void GenerateAllAlleles()
		{
			int cnt = 0;
			int increment = 50000000;
			int threshold = increment;
			foreach (KeyValuePair<string, Scaffold> kvp in _scaffolds) {
				if (cnt > threshold) {
					threshold += increment;
					Console.WriteLine ("Processed " + cnt + " bases in Allele generation");
				}

				kvp.Value.GenerateAlleles ();
				cnt += kvp.Value._length;
			}
		}

		public void WriteHollowFile(string name)
		{
			string h1 = name + "_highs.fasta";
			string h2 = name + "_lows.fasta";
			using (StreamWriter sw = new StreamWriter (h1)) {
				foreach (KeyValuePair<string, Scaffold> kvp in _scaffolds) {
					kvp.Value.WriteHollowHigh (sw);
				}
			}
			using (StreamWriter sw = new StreamWriter (h2)) {
				foreach (KeyValuePair<string, Scaffold> kvp in _scaffolds) {
					kvp.Value.WriteHollowLow (sw);
				}
			}
		}

		public bool LoadWigs()
		{
			string line = "";
			bool retVal = false;
			string[] spsSp;
			string[] spsEq;
			string[] spsTab;
			int cnt = 0;

			Scaffold current = null;
			List<int> curWig = new List<int> ();

			char[] spChar = new char[1] { ' ' };
			using (StreamReader sr = new StreamReader (_wFile)) {
				while ((line = sr.ReadLine ()) != null) {
					if (cnt > 0) {
						retVal = true;
						spsTab = line.Split ('\t');

						if (cnt % 10000000 == 0) {
							Console.WriteLine ("Read " + cnt + " lines of wig!");
						}

						if (spsTab.Length < 2 && line.Length > 5) {

							spsSp = line.Split (spChar, StringSplitOptions.RemoveEmptyEntries);
							spsEq = spsSp[1].Split ('=');
							if (current != null) {
								current.GiveWig (curWig);
							}
							if (_scaffolds.ContainsKey (spsEq [1])) {
								current = _scaffolds [spsEq [1]];
							} else {
								current = null;
							}

							curWig = new List<int> ();
						} else if (spsTab.Length == 2) {
							int cov;
							if (int.TryParse (spsTab [1], out cov)) {
								curWig.Add (cov);
							}
						}
					}
					cnt++;
				}
				if (current != null) {
					current.GiveWig (curWig);
				}
			}

			return retVal;
		}

		public bool LoadGenome()
		{
			string line = "";
			string sequence = "";
			string id = "";
			bool retVal = false;
			int count = 0;
			int thresh = 50000000;
			int interval = 50000000;
			StringBuilder stb = new StringBuilder ();

			using (StreamReader sr = new StreamReader (_gfile)) {
				while ((line = sr.ReadLine ()) != null) {
					retVal = true;

					if (!line.StartsWith (">")) {
						stb.Append (line);
					} else {
						if (stb.Length > 0) {
							sequence = stb.ToString ();
							_scaffolds.Add (id, new Scaffold (id, ref sequence));
							count += sequence.Length;

							if (count > thresh) {
								thresh += interval;
								Console.WriteLine ("Bases read = " + count);
							}

							stb = new StringBuilder ();
						}
						id = line.TrimStart ('>');
					}
				}
				if (!_scaffolds.ContainsKey (id)) {
					_scaffolds.Add (id, new Scaffold (id, ref sequence));
				}
			}
			return retVal;
		}
	}
}

