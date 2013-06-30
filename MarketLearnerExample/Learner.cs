/* **********************
 * Alex Eyler
 * Learner class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning;

namespace MarketLearner
{
    /* Learner class */
    class Learner
    {
        /* Learn the DOW stocks and determine if NASDAQ went up or down */
        static void Main(string[] args) {
            if (args.Length != 4) {
                Console.Error.WriteLine("Usage: learner.exe type [examples file] [classifications file] [number of models] [output file] ");
                return;
            }

            // Get correct data wrapper for loading training set
            Type type = Type.GetType("Learning." + args[0] + ", Learning");
            DataWrapper<string, classificationType> dw = (DataWrapper<string, bool>)System.Activator.CreateInstance(type);
            List<Example<string, bool>> examples;
            try {
                examples = dw.LoadTrainingSet(args[1]);
            } catch (Exception) {
                Console.WriteLine("Error reading from data file: " + args[1]);
                Console.ReadKey();
                return;
            }
            List<bool> classifications = new List<bool>(new bool[2] { true, false });
            // perform AdaBoost function on the examples
            KeyValuePair<List<DTree<string, bool>>, double[]> modelsAndWeights = AdaBoost<string, bool>(examples, dw.Attributes, Convert.ToInt32(args[3]), classifications);
            int x = 0;
            foreach (DTree<string, bool> dTree in modelsAndWeights.Key) {
                Console.WriteLine(dTree.Root);
                Console.WriteLine("weight: " + modelsAndWeights.Value[x]);
                x++;
            }

            // atempt to write model to a file
            try {
                ModelWriter<string, bool>.WriteModel(modelsAndWeights, args[4]);
            } catch (Exception) {
                Console.WriteLine("Error writing to model file");
                return;
            }
            Console.WriteLine("The data has been saved to " + args[4]);
            Console.ReadKey();
        }

        /* AdaBoost function, creates K DStumps based on weights and examples */
        static KeyValuePair<List<DTree<T, V>>, double[]> AdaBoost<T, V>(List<Example<T, V>> examples, 
            List<Attribute<T>> attributes, int K, List<V> classifications)
            where T : IComparable<T> where V : IComparable<V> {

            List<DTree<T, V>> h = new List<DTree<T, V>>();
            double[] w = new double[examples.Count];
            for (int i = 0; i < w.Length; i++) { w[i] = 1.0 / examples.Count; }
            double[] z = new double[K];

            for (int k = 0; k < K; k++) {
                h.Add(new DTree<T, V>(examples, attributes, w, classifications));
                double error = 0.0;
                for (int e = 0; e < examples.Count; e++) {
                    if (!h[k].Root.TestNode(examples[e])) {
                        error += w[e];
                    }
                }
                for (int e = 0; e < examples.Count; e++) {
                    if (h[k].Root.TestNode(examples[e])) {
                        w[e] *= (error / (1 - error));
                    }
                }
                normalize(ref w);
                z[k] = Math.Log((1 - error) / error, 2);
            }
            return new KeyValuePair<List<DTree<T, V>>, double[]>(h, z);
        }

        /* Normalize a vector (so the sum of the vector = 1) */
        static void normalize(ref double[] v) {
            double sum = v.Sum();
            for (int i = 0; i < v.Count(); i++) {
                v[i] /= sum;
            }
        }
    }
}
