/* **********************
 * Alex Eyler
 * DowData class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Learning
{
    /* Implementation of DataWrapper for the DOW prediction project */
    public class DowData : DataWrapper<string, bool>
    {
        // Attribute questions and their selected answer
        private Dictionary<string, string> attrs;

        public DowData() : base("dow") {

            // DOW TICKERS
            string[] temp = new string[30] { 
                                    "MMM", "AA", "AXP", "T", "BAC", "BA", 
                                    "CAT", "CVX", "CSCO", "KO", "DD", "XOM",
                                    "GE", "HPQ", "HD", "INTC", "IBM", "JNJ",
                                    "JPM", "MCD", "MRK", "MSFT", "PFE", "PG",
                                    "TRV", "UNH", "UTX", "VZ", "WMT", "DIS"};
            attrs = new Dictionary<string, string>();

            for (int i = 0; i < temp.Length; i++) {
                attrs.Add(temp[i], "null");
                AttributeAnswers.Add(new List<string>(new string[] { "up_v-low", "up_low", "up_med", "up_high", "up_v-high", "stay",
                                                                     "down_v-low", "down_low", "down_med", "down_high", "down_v-high",}));
            }
            
            for (int i = 0; i < temp.Length; i++) {
                Attributes.Add(new Attribute<string>(temp[i], AttributeAnswers[i]));
            }
        }

        /* Load the Training Set from a filename
         * The data is formatted thusly:
         * Date(long),Ticker(string),Open(double),High(double),Low(double),Close(double),Volume(long),NasdaqUp?(bool) 
         * This will output a list of examples, who have each ticker as an attribute, and the possible answers (each
         * attribute has the same possible answers).
         */
        public override List<Example<string, bool>> LoadTrainingSet(string filename) {
            List<Example<string, bool>> examples = new List<Example<string, bool>>();
            KeyValuePair<long, int> totals = findTotals(filename);

            // Read each line from the training set file, and throw an
            // error if there is an error reading the file (either incorrect format
            // or IO error)
            using (StreamReader reader = new StreamReader(filename)) {
                string line;
                Dictionary<Example<string, bool>, Dictionary<string, string>> answers =
                    new Dictionary<Example<string, bool>, Dictionary<string, string>>();

                while ((line = reader.ReadLine()) != null) {
                    string[] input = line.Split(',');
                    int date = Convert.ToInt32(input[0]);
                    string ticker = input[1];
                    double open = Convert.ToDouble(input[2]);
                    long volume = Convert.ToInt64(input[6]);
                    double close = Convert.ToDouble(input[5]);

                    // Try to find an example if we've already seen it
                    Example<string, bool> example = findExample(answers.Keys.ToList(), date);
                    if (example == null) {
                        // haven't found example, create a new one and fill in an attribute's selectedAnswer
                        Dictionary<string, string> tempAttrs = new Dictionary<string, string>(attrs);
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example = new Example<string, bool>(new List<string>(attrs.Keys), new List<string>(tempAttrs.Values),
                            AttributeAnswers, input[input.Count() - 1].Equals("True"), 0, date);
                        answers.Add(example, tempAttrs);
                    } else {
                        // we have found an example, update an attribute's selectedAnswer
                        Dictionary<string, string> tempAttrs = answers[example];
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example.SetAnswers(tempAttrs.Values.ToArray());
                    }
                }
                examples = answers.Keys.ToList();

                // Remove any missing data (if there aren't 30 tickers for each day)
                int c = 0;
                List<Example<string, bool>> okExamples = new List<Example<string, bool>>();
                foreach (Example<string, bool> ex in examples) {
                    bool ok = true;
                    foreach (Attribute<string> attr in ex.Attributes) {
                        if (attr.SelectedAnswer.Equals("null")) {
                            ok = false;
                        }
                    }
                    if (ok) {
                        ex.Index = c;
                        c++;
                        okExamples.Add(ex);
                    }
                }
                return okExamples;
            }
        }

        /* Similar to the LoadTrainingData function, but this will load a file with no known 
         * classification (to predict on)
         * */
        public override List<Example<string, bool>> LoadData(string filename) {
            List<Example<string, bool>> examples = new List<Example<string, bool>>();
            KeyValuePair<long, int> totals = findTotals(filename);

            // Read each line from the training set file, and throw an
            // error if there is an error reading the file (either incorrect format
            // or IO error)
            using (StreamReader reader = new StreamReader(filename)) {
                string line;
                Dictionary<Example<string, bool>, Dictionary<string, string>> answers =
                    new Dictionary<Example<string, bool>, Dictionary<string, string>>();

                while ((line = reader.ReadLine()) != null) {
                    string[] input = line.Split(',');
                    int date = Convert.ToInt32(input[0]);
                    string ticker = input[1];
                    double open = Convert.ToDouble(input[2]);
                    long volume = Convert.ToInt64(input[6]);
                    double close = Convert.ToDouble(input[5]);

                    // Try to find an example if we've already seen it
                    Example<string, bool> example = findExample(answers.Keys.ToList(), date);
                    if (example == null) {
                        // haven't found example, create a new one and fill in an attribute's selectedAnswer
                        Dictionary<string, string> tempAttrs = new Dictionary<string, string>(attrs);
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example = new Example<string, bool>(new List<string>(attrs.Keys), new List<string>(tempAttrs.Values),
                            AttributeAnswers, false, 0, date);
                        answers.Add(example, tempAttrs);
                    } else {
                        // we have found an example, update an attribute's selectedAnswer
                        Dictionary<string, string> tempAttrs = answers[example];
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example.SetAnswers(tempAttrs.Values.ToArray());
                    }
                }
                examples = answers.Keys.ToList();

                // Remove any missing data (if there aren't 30 tickers for each day)
                int c = 0;
                List<Example<string, bool>> okExamples = new List<Example<string, bool>>();
                foreach (Example<string, bool> ex in examples) {
                    bool ok = true;
                    foreach (Attribute<string> attr in ex.Attributes) {
                        if (attr.SelectedAnswer.Equals("null")) {
                            ok = false;
                        }
                    }
                    if (ok) {
                        ex.Index = c;
                        c++;
                        okExamples.Add(ex);
                    }
                }
                return okExamples;
            }
        }

        /* Find the total volume and total count from the input data */
        private KeyValuePair<long, int> findTotals(string filename) {
            long volume = 0L;
            int total = 0;
            using (StreamReader reader = new StreamReader(filename)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    long vol = Convert.ToInt64(line.Split(',')[6]);
                    volume += vol;
                    total++;
                }
            }
            return new KeyValuePair<long,int>(volume, total);
        }

        /* Find Example if it exists */
        private Example<string, bool> findExample(List<Example<string, bool>> examples, int date) {
            foreach (Example<string, bool> example in examples) {
                if (example.Id == date) {
                    return example;
                }
            }
            return null;
        }

        /* Discretize a particular attribute to find a good answer. The discretization function is this:
         * n = total number of input lines
         * r1 = (sign of) close - open
         * if close - open = 0: stay
         * else:
         * r2 = volume / totalVolume
         * R = r1 * r2
         *  R < 1 / 2n -> v-low
         *  1 / 2n < R < 1 / n -> low
         *  1 / n < R < 2 / n -> med
         *  2 / n < R < 3 / n -> high
         *  3 / n < R ->  v-high
         *  Prepend down or up according to r1 */
        private string discretize(double open, double close, long volume, long totalVolume, int total) {
            double importance = volume / (totalVolume * 1.0);
            double diff = close - open;
            
            string discretizedValue = "";
            if (diff < 0) {
                discretizedValue += "down";
            } else if (diff > 0) {
                discretizedValue += "up";
            } else {
                return "stay";
            }

            if (importance < 1.0 / (total * 2)) {
                discretizedValue += "_v-low";
            } else if (importance < 1.0 / total) {
                discretizedValue += "_low";
            } else if (importance < 2.0 / total) {
                discretizedValue += "_med";
            } else if (importance < 3.0 / total) {
                discretizedValue += "_high";
            } else {
                discretizedValue += "_v-high";
            }

            return discretizedValue;
        }
    }
}
