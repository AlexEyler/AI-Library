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
using Learning.DataWrapper;

namespace MarketLearner
{
    /* Learner class */
    class Learner
    {
        /* Learn the DOW stocks and determine if NASDAQ went up or down */
        public static void Main(string[] args) {
            if (args.Length != 4) {
                Console.Error.WriteLine("Usage: learner.exe type [examples file] [classifications file] [number of models] [output file] ");
                return;
            }

            // Get correct data wrapper for loading training set
            var type = Type.GetType("Learning.DataWrapper" + args[0] + ", Learning");
            var dw = (DataWrapper<string, bool>)System.Activator.CreateInstance(type);
            List<Example<string, bool>> examples;
            try {
                examples = dw.LoadTrainingSet(args[1]);
            } catch (Exception) {
                Console.WriteLine("Error reading from data file: " + args[1]);
                Console.ReadKey();
                return;
            }
            var classifications = new List<bool>(new bool[2] { true, false });
            // perform AdaBoost function on the examples
            var modelsAndWeights = AdaBoost<string, bool>(examples, dw.Attributes, Convert.ToInt32(args[3]), classifications);
            var x = 0;
            foreach (var dTree in modelsAndWeights.Key) {
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

            var h = new List<DTree<T, V>>();
            var w = new double[examples.Count];
            for (var i = 0; i < w.Length; i++) { w[i] = 1.0 / examples.Count; }
            var z = new double[K];

            for (var k = 0; k < K; k++) {
                h.Add(new DTree<T, V>(examples, attributes, w, classifications));
                var error = 0.0;
                for (var e = 0; e < examples.Count; e++) {
                    if (!h[k].Root.TestNode(examples[e])) {
                        error += w[e];
                    }
                }
                for (var e = 0; e < examples.Count; e++) {
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
            var sum = v.Sum();
            for (var i = 0; i < v.Count(); i++) {
                v[i] /= sum;
            }
        }
    }
}
