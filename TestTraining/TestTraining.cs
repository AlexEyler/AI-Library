/* **********************
 * Alex Eyler
 * TestTraining class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning;
using System.IO;

namespace TestTraining
{
    /* TestTraining class */
    class TestTraining
    {
        /* Output the percentage of correct classifications from an input model file */
        static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("Usage: TestTraining type [model file] [data file]");
                return;
            }

            string modelFilename = args[1];
            string dataFilename = args[2];
            KeyValuePair<List<DTree<string, bool>>, double[]> models;
            List<Example<string, bool>> examples;

            Type type = Type.GetType("Learning." + args[0] + ", Learning");
            DataWrapper<string, bool> dw = (DataWrapper<string, bool>)System.Activator.CreateInstance(type);
            try {
                models = loadModels(modelFilename);
                examples = dw.LoadTrainingSet(dataFilename);
            } catch (IOException io) {
                Console.WriteLine(io);
                Console.ReadKey();
                return;
            }

            // Find the number of correct classifications
            List<DTree<string, bool>> trees = models.Key;
            double[] weights = models.Value;
            int correct = 0;
            int total = 0;
            foreach (Example<string, bool> example in examples) {
                total++;
                double weight = 0.0;
                for (int i = 0; i < weights.Length; i++) {
                    bool eq = trees[i].Root.TestNode(example);
                    if (eq) {
                        weight += weights[i];
                    } else {
                        weight -= weights[i];
                    }
                }
                if (weight > 0.0) {
                    correct++;
                } 
            }
            double percentage = correct / (total * 1.0);
            Console.WriteLine("This model is " + percentage * 100 + "% accurate (according to the test data)");
            Console.ReadKey();
            
        }

        /* Load stumps from a ModelReader */
        static KeyValuePair<List<DTree<string, bool>>, double[]> loadModels(string modelFilename) {
            return ModelReader<string, bool>.ReadModel(modelFilename);
        }

    }
}
