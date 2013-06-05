using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyMatcher {
    public class EditDistance {

        private const double APPROVE = 0.05D;
        private const double DISAPPROVE = 0.28D;

        private readonly int logLevel = 0;

        public EditDistance() {
            logLevel = 2;
        }

        public double Distance(String s1, String s2) {
            double dist = DistanceInt(s1, s2);
            double approveLevel = Math.Max(s1.Length, s2.Length) * APPROVE;
            double disapproveLevel = Math.Max(s1.Length, s2.Length) * DISAPPROVE;
            if (logLevel >= 2) {
                Logger.Log("EditDistance", String.Format("Compared: {0}=={1}", s1, s2));
                Logger.Log("EditDistance", String.Format("Distance: {0} [{1}-{2}]", dist, approveLevel, disapproveLevel));
            }

            if (dist > disapproveLevel) {
                return 0;
            } else if (dist <= approveLevel) {
                return 100;
            } else {
                if (logLevel >= 2) {
                    Logger.Log("EditDistance", String.Format("Fuzzy distance: {0}", (100 + 100 / (approveLevel - disapproveLevel) * (dist - approveLevel))));
                }

                return (100 + 100 / (approveLevel - disapproveLevel) * (dist - approveLevel));
            }
        }

        public double Edits(string s1, string s2) {
            return DistanceInt(s1, s2);
        }

        private double DistanceInt(string str1, string str2) {
            int m = str1.Length;
            int n = str2.Length;
            str1 = str1.ToUpper();
            str2 = str2.ToUpper();
            //int[][] mat = new int[m + 1][n + 1];
            int[,] mat = new int[m + 1, n + 1];

            if (m == 0 || n == 0) {
                return Math.Max(m, n);
            } else {
                for (int k = 0; k < m + 1; k++) {
                    mat[k, 0] = k;
                }
                for (int k = 0; k < n + 1; k++) {
                    mat[0, k] = k;
                }

                for (int k = 1; k < m + 1; k++) {
                    for (int l = 1; l < n + 1; l++) {
                        int cost = 0;
                        if (str1[k - 1] == str2[l - 1]) {
                            cost = 0;
                        } else {
                            cost = 1;
                        }
                        mat[k, l] = Minimum(mat[k - 1, l] + 1, mat[k, l - 1] + 1, mat[k - 1, l - 1] + cost);
                        if (k > 1 && l > 1 && str1[k - 1] == str2[l - 2] && str1[k - 2] == str2[l - 1]) {
                            mat[k, l] = Math.Min(mat[k, l], mat[k - 2, l - 2] + cost);
                        }
                    }
                }

                return mat[m, n];
            }
        }

        private int Minimum(int i, int j, int k) {
            return Math.Min(i, Math.Min(j, k));
        }
    }
}
