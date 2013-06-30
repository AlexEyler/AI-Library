/* **********************
 * Alex Eyler
 * NurseryData class
 * AI Project 2
 * **********************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Learning.DataWrapper
{
    public class NurseryData : DataWrapper<string, string>
    {
        private readonly string[] attrs;

        public NurseryData() : base("nursery") {
            attrs = new string[8]{ "parents", "has-nurs", "form", 
                                    "children", "housing", "finance",
                                    "social", "health"};

            AttributeAnswers.Add(new List<string>(new string[] { "usual", "pretentious", "great_pret" }));
            AttributeAnswers.Add(new List<string>(new string[] { "proper", "less_proper", "improper", "critical", "very_crit" }));
            AttributeAnswers.Add(new List<string>(new string[] { "complete", "completed", "incomplete", "foster" }));
            AttributeAnswers.Add(new List<string>(new string[] { "1", "2", "3", "more" }));
            AttributeAnswers.Add(new List<string>(new string[] { "convenient", "less_conv", "critical" }));
            AttributeAnswers.Add(new List<string>(new string[] { "convenient", "inconv" }));
            AttributeAnswers.Add(new List<string>(new string[] { "nonprob", "slightly_prob", "problematic" }));
            AttributeAnswers.Add(new List<string>(new string[] { "recommended", "priority", "not_recom" }));

            for (var i = 0; i < attrs.Length; i++) {
                Attributes.Add(new Attribute<string>(attrs[i], AttributeAnswers[i]));
            }
        }

        // Throws an ArgumentException if it is not formatted correctly
        public override List<Example<string, string>> LoadTrainingSet(string filename) {
            var examples = new List<Example<string, string>>();

            using (var reader = new StreamReader(filename)) {
                string line;
                var c = 0;
                while ((line = reader.ReadLine()) != null) {
                    var input = line.Split(',');
                    var selectedAnswers = input.Take(input.Count() - 1).ToArray();
                    examples.Add(new Example<string, string>(new List<string>(attrs), new List<String>(selectedAnswers),
                         AttributeAnswers, input[input.Count() - 1], c));

                    c++;
                }
            }

            return examples;
        }

        public override List<Example<string, string>> LoadData(string filename) {
            throw new NotImplementedException();
        }
    }
}
