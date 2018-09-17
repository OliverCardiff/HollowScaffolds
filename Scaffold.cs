using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hollow_Scaffs
{
	public class Scaffold
	{
		public override string ToString ()
		{
			return _name;
		}
		protected List<int> _wig;
		protected List<Allele> _alleles;

		public string _name;
		public int _length;
		public string _sequence;

		public Scaffold(string id, ref string seq)
		{
			_sequence = seq;
			_name = id;
			_length = seq.Length;
			_alleles = new List<Allele> ();
		}

		public void GiveWig(List<int> wig)
		{
			_wig = wig;
		}
			

		public void WriteHollowHigh(StreamWriter sw)
		{
			if (_length > 100000) {
				_alleles.Sort ();

				int len = _alleles.Count;
				int oldEd = 0;
				StringBuilder stb = new StringBuilder ();

				for (int i = 0; i < len; i++) {
					if (i > 0) {
						int nCnt = _alleles [i]._st - oldEd;
						if (nCnt > 0) {
							stb.Append (new String ('N', nCnt));
						}
					} else {
						if (_alleles [i]._st > 1) {
							int nCnt = _alleles [i]._st - 1;
							if (nCnt > 0) {
								stb.Append (new String ('N', nCnt));
							}
						}
					}

					if (_alleles [i]._st + _alleles [i].Length > _length) {
						stb.Append (_sequence.Substring (_alleles [i]._st, _length - _alleles[i]._st - 1));
					} else
						stb.Append (_sequence.Substring (_alleles [i]._st, _alleles [i].Length-1));

					oldEd = _alleles [i]._ed;
				}

				if (_length - oldEd > 10) {
					stb.Append (new String ('N', _length - oldEd));
				}
				string finalSeq = stb.ToString ();
				sw.WriteLine (">" + _name);
				WriteSequence (sw, ref finalSeq);
			}
		}

		public void WriteHollowLow(StreamWriter sw)
		{
			if (_length > 1000) {
				_alleles.Sort ();

				int len = _alleles.Count;
				int oldEd = 0;
				StringBuilder stb = new StringBuilder ();

				for (int i = 0; i < len; i++) {
					if (i > 0) {
						int nCnt = _alleles [i]._st - oldEd;
						if (nCnt > 0) {
							if (oldEd + nCnt-1 > _length) {
								stb.Append (_sequence.Substring (_alleles [i]._st, _length - oldEd - 1));
							} else
								stb.Append (_sequence.Substring (oldEd, nCnt-1));
						}
					} else {
						if (_alleles [i]._st > 1) {
							int nCnt = _alleles [i]._st - 1;
							if (nCnt > 0) {
								if (nCnt > _length) {
									stb.Append (_sequence.Substring (_alleles [i]._st, _length - 1));
								} else
									stb.Append (_sequence.Substring (1, nCnt-1));
							}
						}
					}

					stb.Append (new String ('N', _alleles [i].Length));

					oldEd = _alleles [i]._ed;
				}

				if (_length - oldEd > 10) {
					stb.Append (_sequence.Substring (oldEd, _length - oldEd-1));
				}
				string finalSeq = stb.ToString ();
				sw.WriteLine (">" + _name);
				WriteSequence (sw, ref finalSeq);
			}
		}

		public void GenerateAlleles()
		{
			int primaryBroad = 0;
			int primaryNarrow = 0;
			int finalMerge = 0;
			int joins = 0;
			int total = 0;
			int tot_len = 0;
			List<int> broadWindow = new List<int>();
			List<int> narrowWindow = new List<int>();

			RunWindows (broadWindow, narrowWindow);

			List<Allele> nAllele = PopulateAlleleList(narrowWindow, 100);
			List<Allele> bAllele = PopulateAlleleList(broadWindow, 500);

			primaryBroad = bAllele.Count;
			primaryNarrow = nAllele.Count;

			List<Allele> Merged = MergeAlleleLists (bAllele, nAllele);

			finalMerge = Merged.Count;

			_alleles = JoinAdjacentAlleles (Merged);

			total = _alleles.Count;
			joins = finalMerge - total;
			tot_len = SumofAlleles (_alleles);

			if (total > 99999) {
				Console.WriteLine ("Allele Generation Report for: " + _name);
				Console.WriteLine ("Inital Broad:\t" + primaryBroad);
				Console.WriteLine ("Inital Narrow:\t" + primaryNarrow);
				Console.WriteLine ("Merged Count:\t" + finalMerge);
				Console.WriteLine ("Joins Made:\t" + joins);
				Console.WriteLine ("Final Count:\t" + total);
				int cnt2 = 0;
				foreach (Allele a in _alleles) {
					cnt2++;
					if (a.Length > 2000) {
						Console.WriteLine ("Allele " + cnt2 + " st: " + a._st + "\ted: " + a._ed + "\tlen: " + a.Length);
					}
				}
				Console.WriteLine ("Final Sum:\t" + tot_len);
				Console.ReadLine ();
			}
		}

		protected int SumofAlleles(List<Allele> al)
		{
			int accu = 0;
			foreach (Allele a in al) {
				accu += a.Length;
			}
			return accu;
		}

		protected List<Allele> JoinAdjacentAlleles(List<Allele> all)
		{
			int len = all.Count;
			List<Allele> final = new List<Allele> ();

			for (int i = 0; i < len - 1; i++) {
				for (int j = i + 1; j < len; j++) {
					int prox = all [i].GetProximity (all [j]);
					int lenBoth = Math.Max(all[i].Length, all[j].Length);
					int bound = Math.Min (lenBoth / 10, 1000);

					if (prox < bound) {
						all [j] = new Allele (all [i], all [j]);
						all [i] = null;
						break;
					}
				}
			}

			foreach (Allele a in all) {
				if (a != null) {
					final.Add (a);
				}
			}

			return final;
		}

		protected List<Allele> MergeAlleleLists(List<Allele> broad, List<Allele> narrow)
		{
			int nLen = narrow.Count;
			int bLen = broad.Count;

			for (int j = 0; j < bLen; j++) 
			{
				for (int i = 0; i < nLen; i++) {
					if (narrow [i] != null) {
						if (broad [j].Contains (narrow [i])) {
							narrow [i] = null;
						} else if (broad [j].Overlaps (narrow [i])) {
							broad [j] = new Allele (broad [j], narrow [i]);
							narrow [i] = null;
						}
					}
				}
			}

			return broad;
		}

		protected List<Allele> PopulateAlleleList(List<int> wiggo, int minAl)
		{
			List<Allele> nAllele = new List<Allele> ();

			bool current = false;
			int tolerance = 3;
			bool inTolerance = false;
			int st = 0; int ed = 0;

			for (int i = 0; i < wiggo.Count; i++) {

				if (wiggo [i] > MainClass.HIGHCOV - MainClass.RANGE) {
					if (!current) {
						st = i * 10;
						current = true;
					}
					inTolerance = false;
					tolerance = Math.Min (tolerance + 1, 3);

				} else {

					if (current) {
						if (tolerance > 0) {
							if (inTolerance == false) {
								ed = i * 10;
							}
							inTolerance = true;
							tolerance--;
						} else {
							if (ed - st > minAl) {
								nAllele.Add (new Allele (this, st, ed));
							}
							current = false;
							st = 0; ed = 0;
							tolerance = 3;
							inTolerance = false;
						}
					}
				}
			}
			return nAllele;
		}

		protected void RunWindows(List<int> broadWindow, List<int> narrowWindow)
		{
			if (_wig == null) {
				FillNullWig ();
			}
			int len = _wig.Count;

			for (int i = 0; i < len; i++) {

				double cnt = 0; double accu = 0;

				int winLenAB = Math.Max (i - 15, 0);
				int winLenBB = Math.Min (i + 15, len - 1);

				int winLenAN = Math.Max (i - 5, 0);
				int winLenBN = Math.Min (i + 5, len - 1);

				InnerLoop (i, winLenAB, ref accu, ref cnt);
				InnerLoop (i, winLenBB, ref accu, ref cnt);

				broadWindow.Add((int)(accu / cnt));

				accu = 0; cnt = 0;

				InnerLoop (i, winLenAN, ref accu, ref cnt);
				InnerLoop (i, winLenBN, ref accu, ref cnt);

				narrowWindow.Add((int)(accu / cnt));
			}
		}

		protected void FillNullWig()
		{
			int len = _length / 10;

			_wig = new List<int> ();
			for (int i = 0; i < len; i++) {
				_wig.Add (MainClass.LOWCOV);
			}
		}

		protected void InnerLoop(int i, int winLen, ref double accu, ref double cnt)
		{
			int increment = -1;
			if (i < winLen) {
				increment = 1;
			}
			winLen += increment;

			for (int j = i; j != winLen; j += increment) {
				cnt++;
				Accrue(MainClass.LOWCOV, ref accu, _wig[j]);
			}
		}

		protected void Accrue(int bound, ref double accu, int lev)
		{
			if (lev == 0) {
				accu += bound;
			} else {
				accu += Math.Max (lev, bound);
			}
		}

		public static void WriteSequence(StreamWriter sw, ref string sequence)
		{
			int len = sequence.Length;
			int head = 0;
			while (head < len) {
				if (head + 100 < len) {
					sw.WriteLine (sequence.Substring (head, 100));
				} else {
					int linLen = len - head;
					sw.WriteLine (sequence.Substring (head, linLen));
				}
				head += 100;
			}
		}
	}
}

