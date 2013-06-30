/* **********************
 * Alex Eyler
 * Predict class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning;
using System.IO;

namespace Predict
{
    /* Predict class */
    class Predict
    {
        /* Predict whether NASDAQ will go down, stay, or go up based on the DOW stocks for that day */
        static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("Usage: Predict type [model file] [data file]");
                return;
            }

            string modelFilename = args[1];
            string dataFilename = args[2];
            KeyValuePair<List<DTree<string, bool>>, double[]> models;
            List<Example<String, bool>> examples;

            Type type = Type.GetType("Learning." + args[0] + ", Learning");
            DataWrapper<string, bool> dw = (DataWrapper<string, bool>)System.Activator.CreateInstance(type);
            try {
                models = loadModels(modelFilename);
                examples = dw.LoadData(dataFilename);
            } catch (IOException io) {
                Console.WriteLine(io);
                return;
            }

            List<DTree<string, bool>> trees = models.Key;
            double[] weights = models.Value;
            int up = 0;
            int down = 0;

            // Determine if the classifications are right
            foreach (Example<string, bool> example in examples) {
                for (int i = 0; i < weights.Length; i++) {
                    DTree<string, bool> tree = trees[i];
                    foreach (Attribute<string> attr in example.Attributes) {
                        if (tree.Root.Attribute.Question.Equals(attr.Question)) {
                            bool ok = false;
                            for (int c = 0; c < tree.Root.Attribute.Answers.Count; c++) {
                                if (tree.Root.Attribute.Answers[c].Equals(attr.SelectedAnswer)) {
                                    if (tree.Root.children[c].classification) {
                                        ok = true;
                                        up++;
                                        break;
                                    } else {
                                        ok = true;
                                        down++;
                                        break;
                                    }
                                }
                            }
                            if (ok) {
                                break;
                            }
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
