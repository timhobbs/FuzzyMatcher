using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FuzzyMatcher {

    public class AddressDistance : EditDistance {
        private readonly Index indexStreet;
        private readonly Index indexSecondary;

        private bool resolveSecondary = true;

        public AddressDistance() {
            var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            ////indexStreet = new Index(Path.Combine(path, "./scripts/addr-str-suffix.txt"));
            ////indexSecondary = new Index(Path.Combine(path, "./scripts/addr-secondary-units.txt"));
            indexStreet = new Index(@"C:\work\Demo\FRIL-v2.1.5\scripts\addr-str-suffix.txt");
            indexSecondary = new Index(@"C:\work\Demo\FRIL-v2.1.5\scripts\addr-secondary-units.txt");
        }

        public double Distance(string a1, string a2) {
            Logger.Log("AddressDistance", String.Format("Comparing addresses:\r\n{0}\r\n{1}", a1, a2));

            StreetAddress c1 = Normalize(a1);
            StreetAddress c2 = Normalize(a2);

            Logger.Log("AddressDistance", String.Format("Parsed address: {0}", c1));
            Logger.Log("AddressDistance", String.Format("Parsed address: {0}", c2));

            if ((c1.SecondLevel == null && c2.SecondLevel == null) || !resolveSecondary) {
                return MatchStreetName(c1, c2, 0.5, 0.5);
            } else {
                //1. Street name - 50 (name = 40, prefix = 10)
                //2. Street number - 30
                double score = MatchStreetName(c1, c2, 0.3, 0.5);

                //3. Second level - 20
                if (score != 0) {
                    score += MatchSecondLevelAddr(c1, c2, 0.2);
                }

                return score;
            }
        }

        internal StreetAddress Normalize(string addr) {
            String[] addrComponents = Regex.Split(addr.ToUpper().Trim(), "\\s+");

            if (addrComponents.Length == 0) {
                return null;
            }

            StreetAddress address = new StreetAddress() {
                Number = addrComponents[0],
            };
            SecondLevelAddress activeAddrL2 = null;
            var buffer = new StringBuilder();
            for (int i = 1; i < addrComponents.Length; i++) {
                String test = ExtractString(addrComponents[i]);
                String strSuffix = indexStreet.Lookup(test);
                String secondLevel = indexSecondary.Lookup(test);
                if (strSuffix != null && !IsNextSpecial(addrComponents, i + 1)) {
                    address.Name = buffer.ToString();
                    address.Suffix = strSuffix;
                    buffer = new StringBuilder();
                } else if (secondLevel != null) {
                    var s2String = buffer.ToString();
                    SecondLevelAddress newAddrL2 = new SecondLevelAddress();
                    if (address.Name == null) {
                        address.Name = s2String.Replace("-", "");
                    } else {
                        if (activeAddrL2 == null) {
                            newAddrL2.Value = s2String.Replace("-", "");
                        } else {
                            activeAddrL2.Value = s2String.Replace("-", "");
                        }
                    }

                    buffer = new StringBuilder();
                    newAddrL2.Prefix = secondLevel;
                    if (address.SecondLevel == null) {
                        address.SecondLevel = newAddrL2;
                    } else {
                        activeAddrL2.Child = newAddrL2;
                    }

                    activeAddrL2 = newAddrL2;
                } else {
                    buffer.Append(addrComponents[i]);
                }
            }

            var bufferString = buffer.ToString();

            if (!String.IsNullOrEmpty(bufferString)) {
                if (address.Name == null) {
                    address.Name = bufferString.Replace("-", "");
                } else {
                    if (activeAddrL2 == null) {
                        activeAddrL2 = new SecondLevelAddress();
                        address.SecondLevel = activeAddrL2;
                    }
                    activeAddrL2.Value = bufferString.Replace("-", "");
                }
            }

            return address;
        }

        private double MatchStreetName(StreetAddress c1, StreetAddress c2, double weightNumber, double weightStrName) {
            double score = 0;

            if (c1.Number != null && c2.Number != null) {
                score += weightNumber * base.Distance(c1.Number, c2.Number);
            }

            double maxScoreStr = 0;
            if (c1.Name != null && c2.Name != null) {
                //Try perfect match
                if (c1.Suffix != null && c2.Suffix != null) {
                    maxScoreStr = base.Distance(c1.Name, c2.Name) * 0.8;
                    if (maxScoreStr != 0) {
                        maxScoreStr += base.Distance(c1.Suffix, c2.Suffix) * 0.2;
                    }
                }

                //Try removing either of suffixes, adding the other to the street name
                if (c1.Suffix != null) {
                    maxScoreStr = TryMatch(String.Format("{0} {1}", c1.Name, c1.Suffix), c2.Name, maxScoreStr, 1.0);
                }
                if (c2.Suffix != null) {
                    maxScoreStr = TryMatch(String.Format("{0} {1}", c2.Name, c2.Suffix), c1.Name, maxScoreStr, 1.0);
                }

                //Try ignoring suffixes, but dump value by 0.3
                maxScoreStr = TryMatch(c1.Name, c2.Name, maxScoreStr, (c1.Suffix == null && c2.Suffix == null) ? 1.0 : 0.7);
            } else {
                //No street in one of addresses or both, test each with prefix of the other
                if (c1.Name != null && c2.Suffix != null) {
                    maxScoreStr = TryMatch(String.Format("{0} {1}", c1.Name, c1.Suffix), c2.Suffix, maxScoreStr, 0.7);
                    maxScoreStr = TryMatch(c1.Name, c2.Suffix, maxScoreStr, 0.7);
                }
                if (c2.Name != null && c1.Suffix != null) {
                    maxScoreStr = TryMatch(String.Format("{0} {1}", c2.Name, c2.Suffix), c1.Suffix, maxScoreStr, 0.7);
                    maxScoreStr = TryMatch(c2.Name, c1.Suffix, maxScoreStr, 0.7);
                }
                if (c1.Name == null && c2.Name == null && c1.Suffix != null && c2.Suffix != null) {
                    maxScoreStr = TryMatch(c1.Suffix, c2.Suffix, maxScoreStr, 0.1);
                }
            }

            if (maxScoreStr != 0) {
                score += maxScoreStr * weightStrName;
            }

            //Try to ignore number, and just compare strings of number + street + suffix. This will be adjusted by 0.6
            score = Math.Max(score, base.Distance(c1.toStringNoL2(), c2.toStringNoL2()) * (weightNumber + weightStrName) * 0.6);

            return score;
        }

        private double TryMatch(String s1, String s2, double maxScore, double adjustment) {
            return Math.Max(maxScore, adjustment * Distance(s1, s2));
        }

        private double MatchSecondLevelAddr(StreetAddress c1, StreetAddress c2, double weight) {
            if (c1.SecondLevel != null && c2.SecondLevel != null) {
                return MatchSecondLevelAddr(c1.SecondLevel, c2.SecondLevel, weight);
            }
            return 0;
        }

        private double MatchSecondLevelAddr(SecondLevelAddress s1, SecondLevelAddress s2, double weight) {
            double maxScore = 0;
            if (s1.Value != null && s2.Value != null) {
                if (s1.Prefix != null && s2.Prefix != null) {
                    maxScore = base.Distance(s1.Value, s2.Value) * 0.8;
                    maxScore += base.Distance(s1.Prefix, s2.Prefix) * 0.2;
                    maxScore = AdjustForChildren(s1, s2, weight, maxScore);
                }
                if (s1.Prefix != null) {
                    double score = TryMatch(String.Format("{0} {1}", s1.Prefix, s1.Value), s2.Value, maxScore, 1.0);
                    maxScore = Math.Max(maxScore, AdjustForChildren(s1, s2, weight, score));
                }
                if (s2.Prefix != null) {
                    double score = TryMatch(String.Format("{0} {1}", s2.Prefix, s2.Value), s1.Value, maxScore, 1.0);
                    maxScore = Math.Max(maxScore, AdjustForChildren(s1, s2, weight, score));
                }

                maxScore = TryMatch(s1.Value, s2.Value, maxScore, (s1.Prefix == null && s2.Prefix == null) ? 1.0 : 0.7);
            } else {
                if (s1.Value != null) {
                    maxScore = TryMatch(s1.Prefix + s1.Value, s2.Prefix, maxScore, 0.6);
                    maxScore = TryMatch(s1.Value, s2.Prefix, maxScore, 0.6);
                }

                if (s2.Value != null) {
                    maxScore = TryMatch(s2.Prefix + s2.Value, s1.Prefix, maxScore, 0.6);
                    maxScore = TryMatch(s2.Value, s1.Prefix, maxScore, 0.6);
                }
            }

            maxScore = TryMatch(s1.ToStringWithPref(), s2.ToStringWithPref(), maxScore, 0.8);
            maxScore = TryMatch(s1.ToStringNoPref(), s2.ToStringNoPref(), maxScore, 0.5);

            return maxScore * weight;
        }

        private double AdjustForChildren(SecondLevelAddress s1, SecondLevelAddress s2, double weight, double score) {
            if (s1.Child != null && s2.Child != null) {
                score = score * 0.7 + MatchSecondLevelAddr(s1.Child, s2.Child, weight) * 0.3;
            } else if (s1.Child != null || s2.Child != null) {
                //Adjust score a bit as no perfect match
                score = score * 0.8;
            }

            return score;
        }

        private string ExtractString(string test) {
            int trim = test.Length - 1;
            while (trim > 0 && test[trim] == '.') trim--;
            test = test.Substring(0, trim + 1);
            return test;
        }

        private bool IsNextSpecial(string[] addrComponents, int index) {
            if (index >= addrComponents.Length) {
                return false;
            }

            string test = ExtractString(addrComponents[index]);

            return indexStreet.Lookup(test) != null;
        }

        internal class StreetAddress {

            public string Number { get; set; }

            public string Name { get; set; }

            public string Suffix { get; set; }

            public SecondLevelAddress SecondLevel { get; set; }

            public override string ToString() {
                return String.Format("Street={0}; number={1}; StrSuffix={2}; Second level addr={3}",
                    Name,
                    (Number == null ? "none" : Number),
                    (Suffix == null ? "none" : Suffix),
                    (SecondLevel == null ? "none" : ("(" + SecondLevel.ToString() + ")")));
            }

            public String toStringNoL2() {
                return (Number == null ? "" : (Number + " ")) +
                    (Name == null ? "" : (Name + " ")) +
                    (Suffix == null ? "" : Suffix);
            }
        }

        internal class SecondLevelAddress {

            public string Value { get; set; }

            public string Prefix { get; set; }

            public SecondLevelAddress Child { get; set; }

            public override string ToString() {
                return String.Format("Prefix={0}; Value={1}; additional info={2}",
                    (Prefix == null ? "none" : Prefix),
                    (Value == null ? "none" : Value),
                    (Child == null ? "none" : (String.Format("({0})", Child))));
            }

            public string ToStringNoPref() {
                return String.Format("{0} {1}",
                    (Value != null ? Value : ""),
                    (Child != null ? Child.ToStringNoPref() : ""));
            }

            public string ToStringWithPref() {
                return String.Format("{0} {1} {2}",
                    (Prefix != null ? Prefix : ""),
                    (Value != null ? Value : ""),
                    (Child != null ? Child.ToStringWithPref() : ""));
            }
        }

        private class Index {
            private IDictionary<string, string> index = new Dictionary<string, string>();

            public Index(String fName) {
                try {
                    var reader = System.IO.File.ReadLines(fName);
                    foreach (var line in reader) {
                        //Ignore comments
                        if (!line.StartsWith("#")) {
                            string[] arr = line.Split(',');

                            if (!index.ContainsKey(arr[0])) {
                                //Self-mapping (Street -> Street)
                                index.Add(arr[0], arr[0]);

                                //Remaining mappings (St -> Street)]
                                for (int i = 1; i < arr.Length; i++) {
                                    if (index.ContainsKey(arr[i]) && index[arr[i]].Equals(arr[0])) {
                                        ////var msg = String.Format("ERROR IN file {0}. Conflict for line {1} (previously {2} -> {3}). Ignoring.",
                                        ////    fName,
                                        ////    line,
                                        ////    arr[i],
                                        ////    index[arr[i]]);
                                        ////Console.WriteLine(msg);
                                    } else {
                                        index.Add(arr[i], arr[0]);
                                    }
                                }
                            } else {
                                ////var msg = String.Format("ERROR IN file {0}. Conflict for line {1} (previously {2} -> {3}). Ignoring.",
                                ////    fName,
                                ////    line,
                                ////    arr[0],
                                ////    index.Count > 0 ? 
                                ////        index[arr[0]] ?? String.Empty : 
                                ////        String.Empty);
                                ////Console.WriteLine(msg);
                            }
                        }
                    }
                } catch (FileNotFoundException e) {
                    throw new Exception("Error reading file: " + fName, e);
                }
            }

            public string Lookup(string key) {
                string value = null;
                index.TryGetValue(key, out value);
                return value;
            }
        }
    }
}