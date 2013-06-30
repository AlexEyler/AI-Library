/* **********************
 * Alex Eyler
 * Predict class
 * AI Project 2
 * **********************/

using System;
using System.Collections.Generic;
using Learning;
using System.IO;
using Learning.DataWrapper;

namespace MarketLearner
{
    /* Predict class */
    class Predict
    {
        /* Predict whether NASDAQ will go down, stay, or go up based on the DOW stocks for that day */
        public static void DoPredict(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("Usage: Predict type [model file] [data file]");
                return;
            }

            var modelFilename = args[1];
            var dataFilename = args[2];
            KeyValuePair<List<DTree<string, bool>>, double[]> models;
            List<Example<String, bool>> examples;

            var type = Type.GetType("Learning.DataWrapper" + args[0] + ", Learning");
            var dw = (DataWrapper<string, bool>)System.Activator.CreateInstance(type);
            try {
                models = loadModels(modelFilename);
                examples = dw.LoadData(dataFilename);
            } catch (IOException io) {
                Console.WriteLine(io);
                return;
            }

            var trees = models.Key;
            var weights = models.Value;
            var up = 0;
            var down = 0;

            // Determine if the classifications are right
            foreach (var example in examples) {
                for (var i = 0; i < weights.Length; i++) {
                    var tree = trees[i];
                    foreach (var attr in example.Attributes) {
                        if (!tree.Root.Attribute.Question.Equals(attr.Question)) continue;

                        var ok = false;
                        for (var c = 0; c < tree.Root.Attribute.Answers.Count; c++){
                            if (!tree.Root.Attribute.Answers[c].Equals(attr.SelectedAnswer)) continue;

                            if (tree.Root.Children[c].Classification) {
                                ok = true;
                                up++;
                                break;
                            } else {
                                ok = true;
                                down++;
                                break;
                            }
                        }
                        if (ok) {
                            break;
                        }
                    }
                }
            }

            // determination
            if (up > down) {
                Console.WriteLine("The model predicts that the NASDAQ will go up today.");
            } else if (up < down) {
                Console.WriteLine("The model predicts that the NASDAQ will go down today.");
            } else {
                Console.WriteLine("The model predicts that the NASDAQ will stay the same today.");
            }

        }

        /* Load models from the ModelReader class */
        static KeyValuePair<List<DTree<string, bool>>, double[]> loadModels(string modelFilename) {
            return ModelReader<string, bool>.ReadModel(modelFilename);
        }

    }
}
