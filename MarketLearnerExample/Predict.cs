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
    class Predict<TT>
        where TT : IComparable<TT>
    {
        /* Predict whether NASDAQ will go down, stay, or go up based on the DOW stocks for that day */
        public static void DoPredict(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("Usage: Predict type [model file] [data file]");
                return;
            }

            var modelFilename = args[1];
            var dataFilename = args[2];
            var models = new KeyValuePair<List<DTree<string>>, double[]>();
            List<Example<TT>> examples = null;

            var type = Type.GetType("Learning.DataWrapper" + args[0] + ", Learning");
            DataWrapper<TT> dw = null;
            if (type != null){
                dw = (DataWrapper<TT>) System.Activator.CreateInstance(type);
                try {
                    models = loadModels(modelFilename);
                    examples = dw.LoadData(dataFilename);
                } catch (IOException io) {
                    Console.WriteLine(io);
                    return;
                }
            }

            var trees = models.Key;
            var weights = models.Value;

            // Determine if the classifications are right
            if (examples != null){
                foreach (var example in examples) {
                    for (var i = 0; i < weights.Length; i++) {
                        var tree = trees[i];
                        foreach (var attr in example.Attributes) {
                            if (!tree.Root.Attribute.Question.Equals(attr.Question)) continue;

                            var ok = false;
                            for (var c = 0; c < tree.Root.Attribute.Answers.Count; c++){
                                if (!tree.Root.Attribute.Answers[c].Equals(attr.SelectedAnswer)) continue;

                                dw.ApplyClassification(tree.Root.Children[c].Classification);
                                ok = true;
                            }
                            if (ok) {
                                break;
                            }
                        }
                    }
                }
            }

            // Print out determination
            if (dw != null) Console.WriteLine(dw.Determiniation());
        }

        /* Load models from the ModelReader class */
        static KeyValuePair<List<DTree<string>>, double[]> loadModels(string modelFilename) {
            return ModelReader<string>.ReadModel(modelFilename);
        }

    }
}
