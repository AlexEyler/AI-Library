using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    class Classification : IComparable<Classification>
    {
        public List<string> Options;
        private int selectedIndex;

        public Classification() {
            Options = new List<string>();
            selectedIndex = -1;
        }

        public Classification(string filename) {
            Options = ReadClassifications(filename);
            selectedIndex = -1;
        }

        public Classification(List<string> options) {
            Options = options;
            selectedIndex = -1;
        }

        public void SelectOption(string option) {
            selectedIndex = Options.IndexOf(option);
        }

        /* Get the list of possible classifications from a given file
         * The classifications are formatted as follows (without line numbers)
         * 1: Classification 1
         * 2: Classification 2
         * ...
         * n: Classification n
         * */
        public static List<string> ReadClassifications(string path) {
            var classifications = new List<string>();
            using (var reader = new StreamReader(path)) {
                string line;
                while ((line = reader.ReadLine()) != null)
                    classifications.Add(line);
            }
            return classifications;
        }

        public int CompareTo(Classification other) {
            if (other.selectedIndex == -1) {
                return 1;
            } else if (this.selectedIndex == -1) {
                return -1;
            } else {
                if (selectedIndex == other.selectedIndex) {
                    return 0;
                } else {
                    return this.selectedIndex > other.selectedIndex ? 1 : 0;
                }
            }
        }

        public bool Equals(Classification other) {
            return this.CompareTo(other) == 0;
        }
    }
}
