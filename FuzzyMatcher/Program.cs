using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyMatcher {
    class Program {
        static void Main(string[] args) {
            var ed = new EditDistance();
            Console.WriteLine(String.Format("Comparing: '{0}' and '{1}'", "90021", "99021"));
            Console.WriteLine(String.Format("dst = {0}", ed.Distance("90021", "99021")));
            Console.WriteLine();
            Console.WriteLine(String.Format("Comparing: '{0}' and '{1}'", "Adam Smith", "Smith Adam"));
            Console.WriteLine(String.Format("dst = {0}", ed.Distance("Adam Smith", "Smith Adam")));
            Console.WriteLine();
            Console.WriteLine(String.Format("Comparing: '{0}' and '{1}'", "1234567890", "(123)456-7890"));
            Console.WriteLine(String.Format("dst = {0}", ed.Distance("1234567890", "(123)456-7890")));
            Console.WriteLine("Press 'Enter' to continue...");
            Console.ReadLine();

            var ad = new AddressDistance();
            Console.WriteLine(ad.Normalize("1129 Clairemont LN. Apt D BLDG. B"));
            Console.WriteLine();
            Console.WriteLine(ad.Normalize("1129 Clairemont Ave Apt E"));
            Console.WriteLine();
            Console.WriteLine(ad.Normalize("1129 Clairemont Ave.   E   "));
            Console.WriteLine();
            Console.WriteLine(ad.Normalize("  1129 Clairemont   Ave.. .. Apt. E"));
            Console.WriteLine();
            Console.WriteLine(ad.Normalize("1129 Clairemont Ave Apt. E"));
            Console.WriteLine();
            Console.WriteLine(ad.Normalize("1129 Clairemont Ave Apt. E"));
            Console.WriteLine();
            Console.WriteLine(ad.Normalize("1129 Clairemont ST Apt. E"));
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to continue...");
            Console.ReadLine();

            var t1 = "1563 sandpiper court bldg E apt G";
            var t2 = "1563 sandpiper ct apt E";
            Console.WriteLine(String.Format("dst = {0}", ad.Distance(t1, t2)));
            Console.WriteLine();
            
            Console.WriteLine("Press 'Enter' to close.");
            Console.ReadLine();

        }
    }
}
