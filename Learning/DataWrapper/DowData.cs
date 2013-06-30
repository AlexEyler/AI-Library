/* **********************
 * Alex Eyler
 * DowData class
 * AI Project 2
 * **********************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Learning.DataWrapper
{
    /* ClassificationId into array */
    public enum DowClassificationId {
        Up = 0,
        Down = 1
    }

    /* Implementation of DataWrapper for the DOW prediction project */
    public class DowData : DataWrapper<string>
    {
        // Attribute questions and their selected answer
        private readonly Dictionary<string, string> attrs;

        public DowData() : base("dow") {

            // DOW TICKERS
            var temp = new string[30] { 
                                    "MMM", "AA", "AXP", "T", "BAC", "BA", 
                                    "CAT", "CVX", "CSCO", "KO", "DD", "XOM",
                                    "GE", "HPQ", "HD", "INTC", "IBM", "JNJ",
                                    "JPM", "MCD", "MRK", "MSFT", "PFE", "PG",
                                    "TRV", "UNH", "UTX", "VZ", "WMT", "DIS"};
            attrs = new Dictionary<string, string>();

            foreach (var t in temp){
                attrs.Add(t, "null");
                AttributeAnswers.Add(new List<string>(new string[] { "up_v-low", "up_low", "up_med", "up_high", "up_v-high", "stay",
                                                                     "down_v-low", "down_low", "down_med", "down_high", "down_v-high",}));
            }
            
            for (var i = 0; i < temp.Length; i++) {
                Attributes.Add(new Attribute<string>(temp[i], AttributeAnswers[i]));
            }

            ClassificationsCount = new int[2];
        }

        /* Load the Training Set from a filename
         * The data is formatted thusly:
         * Date(long),Ticker(string),Open(double),High(double),Low(double),Close(double),Volume(long),NasdaqUp?(bool) 
         * This will output a list of examples, who have each ticker as an attribute, and the possible answers (each
         * attribute has the same possible answers).
         */
        public override List<Example<string>> LoadTrainingSet(string filename) {
            var totals = findTotals(filename);

            // Read each line from the training set file, and throw an
            // error if there is an error reading the file (either incorrect format
            // or IO error)
            using (var reader = new StreamReader(filename)) {
                string line;
                var answers =
                    new Dictionary<Example<string>, Dictionary<string, string>>();

                while ((line = reader.ReadLine()) != null) {
                    var input = line.Split(',');
                    var date = Convert.ToInt32(input[0]);
                    var ticker = input[1];
                    var open = Convert.ToDouble(input[2]);
                    var volume = Convert.ToInt64(input[6]);
                    var close = Convert.ToDouble(input[5]);

                    // Try to find an example if we've already seen it
                    var example = findExample(answers.Keys.ToList(), date);
                    if (example == null) {
                        // haven't found example, create a new one and fill in an attribute's selectedAnswer
                        var tempAttrs = new Dictionary<string, string>(attrs);
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example = new Example<string>(new List<string>(attrs.Keys), new List<string>(tempAttrs.Values),
                            AttributeAnswers, input[input.Count() - 1].Equals("True") ? (int)DowClassificationId.Up : (int)DowClassificationId.Down, 0, date);
                        answers.Add(example, tempAttrs);
                    } else {
                        // we have found an example, update an attribute's selectedAnswer
                        var tempAttrs = answers[example];
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example.SetAnswers(tempAttrs.Values.ToArray());
                    }
                }
                var examples = answers.Keys.ToList();

                // Remove any missing data (if there aren't 30 tickers for each day)
                var c = 0;
                var okExamples = new List<Example<string>>();
                foreach (var ex in examples) {
                    var ok = true;
                    foreach (var attr in 
                        ex.Attributes.Where(attr => attr.SelectedAnswer.Equals("null"))){
                        ok = false;
                    }
                    if (!ok) continue;
                    ex.Index = c;
                    c++;
                    okExamples.Add(ex);
                }
                return okExamples;
            }
        }

        /* Similar to the LoadTrainingData function, but this will load a file with no known 
         * classification (to predict on)
         * */
        public override List<Example<string>> LoadData(string filename) {
            var totals = findTotals(filename);

            // Read each line from the training set file, and throw an
            // error if there is an error reading the file (either incorrect format
            // or IO error)
            using (var reader = new StreamReader(filename)) {
                string line;
                var answers = new Dictionary<Example<string>, Dictionary<string, string>>();

                while ((line = reader.ReadLine()) != null) {
                    var input = line.Split(',');
                    var date = Convert.ToInt32(input[0]);
                    var ticker = input[1];
                    var open = Convert.ToDouble(input[2]);
                    var volume = Convert.ToInt64(input[6]);
                    var close = Convert.ToDouble(input[5]);

                    // Try to find an example if we've already seen it
                    var example = findExample(answers.Keys.ToList(), date);
                    if (example == null) {
                        // haven't found example, create a new one and fill in an attribute's selectedAnswer
                        var tempAttrs = new Dictionary<string, string>(attrs);
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example = new Example<string>(new List<string>(attrs.Keys), new List<string>(tempAttrs.Values),
                            AttributeAnswers, (int)DowClassificationId.Up, 0, date);
                        answers.Add(example, tempAttrs);
                    } else {
                        // we have found an example, update an attribute's selectedAnswer
                        var tempAttrs = answers[example];
                        tempAttrs[ticker] = discretize(open, close, volume, totals.Key, totals.Value);
                        example.SetAnswers(tempAttrs.Values.ToArray());
                    }
                }

                var examples = answers.Keys.ToList();

                // Remove any missing data (if there aren't 30 tickers for each day)
                var c = 0;
                var okExamples = new List<Example<string>>();
                foreach (var ex in examples) {
                    var ok = true;
                    foreach (var attr in 
                        ex.Attributes.Where(attr => attr.SelectedAnswer.Equals("null"))){
                        ok = false;
                    }
                    if (!ok) continue;
                    ex.Index = c;
                    c++;
                    okExamples.Add(ex);
                }
                return okExamples;
            }
        }

        public override string Determiniation() {
            if (ClassificationsCount[(int)DowClassificationId.Up] > ClassificationsCount[(int)DowClassificationId.Down]) {
                return "The model predicts that the NASDAQ will go up today.";
            }
            if (ClassificationsCount[(int)DowClassificationId.Up] < ClassificationsCount[(int)DowClassificationId.Down]) {
                return "The model predicts that the NASDAQ will go down today.";
            }
            return "The model predicts that the NASDAQ will stay the same today.";

        }
        /* Find the total volume and total count from the input data */
        private KeyValuePair<long, int> findTotals(string filename) {
            var volume = 0L;
            var total = 0;
            using (var reader = new StreamReader(filename)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    var vol = Convert.ToInt64(line.Split(',')[6]);
                    volume += vol;
                    total++;
                }
            }
            return new KeyValuePair<long,int>(volume, total);
        }

        /* Find Example if it exists */
        private Example<string> findExample(IEnumerable<Example<string>> examples, int date){
            return examples.FirstOrDefault(example => example.Id == date);
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
            var importance = volume / (totalVolume * 1.0);
            var diff = close - open;
            
            var discretizedValue = "";
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
