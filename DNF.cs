using System.Collections.Generic;
using System.Linq;

namespace Disjunctive_Normal_Form
{
    internal class DNF
    {
        public List<List<int>> Data { get; set; }

        private List<List<int>> dataToCalc = new List<List<int>>();
        public string Calculate()
        {
            List<int> positives = GetPositives();
            List<List<int>> h = new List<List<int>>();
            while (positives.Count > 0)
            {
                dataToCalc = new List<List<int>>(Data);
                List<int> result = new List<int>();
                List<int> negatives = GetNegatives();
                while (negatives.Count() > 0)
                {
                    if(!CanSolve(result))
                    {
                        return "Cannot Solve";
                    }
                    result.Add(GetMostPositiveCoverageLogicFeature());
                    List<int> cases = getCases(GetMostPositiveCoverageLogicFeature(), negatives);
                    negatives = negatives.Except(cases).ToList();
                    for (int i = 0; i < cases.Count(); i++)
                    {
                        dataToCalc[cases[i]] = Enumerable.Repeat(0, Data[0].Count()).ToList();
                    }

                }

                h.Add(result);
                List<int> coverage = GetCoverage(result, positives);

                if (!CanCover(coverage))
                {
                    return "Cannot Cover";
                }
                else
                {
                    positives = positives.Except(coverage).ToList();
                }

                for (int i = 0; i < coverage.Count(); i++)
                {
                    Data[coverage[i]] = Enumerable.Repeat(0, Data[0].Count()).ToList();
                }
            }

            return GetAsString(h);
        }

        private bool CanSolve(List<int> r)
        {
            return r.Count() != Data[0].Count();
        }

        private bool CanCover(List<int> coverage)
        {
            return coverage.Count() != 0;
        }

        private string GetAsString(List<List<int>> h)
        {
            string resultString = "";
            for (int i = 0; i < h.Count(); i++)
            {
                resultString += "( ";
                for (int j = 0; j < h[i].Count() - 1; j++)
                {
                    resultString += "f" + h[i][j].ToString() + " " + '\u2227';
                }
                resultString += " f" + h[i].Last().ToString();
                if (i == h.Count - 1)
                {
                    resultString += " ) ";
                }
                else
                {
                    resultString += " ) " + '\u2228' + " ";
                }
            }
            return resultString;
        }
        private List<int> GetPositives()
        {
            List<int> positives = new List<int>();

            for (int i = 0; i < Data.Count(); i++)
            {
                if (Data[i].Last() == 1)
                {
                    positives.Add(i);
                }
            }

            return positives;
        }

        private List<int> GetNegatives()
        {
            List<int> negatives = new List<int>();

            for (int i = 0; i < Data.Count(); i++)
            {
                if (Data[i].Last() == 0)
                {
                    negatives.Add(i);
                }
            }
            return negatives;
        }

        private int GetMostPositiveCoverageLogicFeature()
        {
            List<double> measures = new List<double>();
            for (int i = 0; i < dataToCalc[0].Count() - 1; i++)
            {
                int num = 0;
                double den = 0;
                for (int j = 0; j < dataToCalc.Count; j++)
                {
                    if (dataToCalc[j][i] == 1 && dataToCalc[j][dataToCalc[0].Count() - 1] == 1)
                    {
                        num++;
                    }

                    if (dataToCalc[j][i] == 1 && dataToCalc[j][dataToCalc[0].Count() - 1] == 0)
                    {
                        den++;
                    }
                }
                if (den.Equals(0))
                {
                    den = 0.0001;
                }
                measures.Add(num / den);
            }
            return measures.IndexOf(measures.Max());
        }

        private List<int> getCases(int bestIndex, List<int> negatives)
        {
            List<int> cases = new List<int>();
            for (int i = 0; i < negatives.Count(); i++)
            {
                if (Data[negatives[i]][bestIndex] == 0)
                {
                    cases.Add(negatives[i]);
                }
            }
            return cases;
        }

        private List<int> GetCoverage(List<int> result, List<int> positive)
        {
            List<int> coverage = new List<int>();
            bool check = true;
            for (int i = 0; i < positive.Count(); i++)
            {
                for (int j = 0; j < result.Count(); j++)
                {
                    if (Data[positive[i]][result[j]] == 1)
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }
                }
                if (check.Equals(true))
                {
                    coverage.Add(positive[i]);
                }
            }
            return coverage;
        }
    }

}
