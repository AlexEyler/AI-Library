/* **********************
 * Alex Eyler
 * DataManipulation class
 * AI Project 2
 * **********************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataManipulation
{
    class CreateDowTrainingSet
    {
        // Create a training set for the DOW + NASDAQ project (basically, see if NASDAQ went up or down on a certain day 
        // and add that to each line (with that date) in the DOW file, creating a new file instead).
        public static void Main(string[] args) {
            
            if (args.Length != 3) {
                Console.Error.WriteLine("Usage: CreateDowTrainingSet.exe [DOW file] [NASDAQ file] [out file]");
                Console.ReadKey();
            }

            
            FileStream outStream = new FileStream(args[2], FileMode.Create);
            StreamWriter writer = new StreamWriter(outStream);

            using (StreamReader nasdaqReader = new StreamReader(args[1])) {
                string nasdaqLine;

                while ((nasdaqLine = nasdaqReader.ReadLine()) != null) {
                    string[] nasdaqLineSplit = nasdaqLine.Split(',');
                    string nasdaqDate = nasdaqLineSplit[0];
                    string upString = nasdaqLineSplit[nasdaqLineSplit.Length - 1];
                    int upInt = Convert.ToInt32(upString);
                    bool up = upInt == 1;
                    using (StreamReader dowReader = new StreamReader(args[0])) {
                        string dowLine;
                        while ((dowLine = dowReader.ReadLine()) != null) {
                            string dowDate = dowLine.Split(',')[0];
                            if (dowDate.Equals(nasdaqDate)) {
                                string newLine = dowLine + "," + up;
                                writer.WriteLine(newLine);
                            }
                        }
                    }
                }
            }
            writer.Flush();
            outStream.Close();
        }
    }
}
