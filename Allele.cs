using System;
using System.Collections.Generic;
using System.IO;

namespace Hollow_Scaffs
{
	public class Allele : IComparable
	{
		protected Scaffold _ref;

		public int _st;
		public int _ed;

		public int Length { get { return _ed - _st; } }

		public Allele (Scaffold sref, int st, int ed)
		{
			_ref = sref;
			_st = st;
			_ed = ed;
		}

		public Allele(Allele Broad, Allele Narrow)
		{
			_st = Math.Min (Broad._st, Narrow._st);
			_ed = Math.Max (Broad._ed, Narrow._ed);
			_ref = Broad._ref;
		}

		public int CompareTo(Object oth)
		{
			Allele b = (Allele)oth;

			return this._st.CompareTo (b._st);
		}

		public bool Overlaps(Allele oth)
		{
			if ((_st <= oth._ed && _st >= oth._st) || (_ed <= oth._ed && _ed >= oth._st)) {
				return true;
			} else {
				return false;
			}
		}

		public bool Contains(Allele oth)
		{
			if (_st >= oth._st && _ed <= oth._ed) {
				return true;
			} else {
				return false;
			}
		}

		public int GetProximity(Allele oth)
		{
			int testA = oth._st - _ed;
			int testB = _st - oth._ed;

			return Math.Max (testA, testB);
		}
	}
}

