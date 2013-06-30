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
using Learning.DataWrapper;

namespace TestTraining
{
    /* TestTraining class */
    class TestTraining
    {
        /* Output the percentage of correct classifications from an input model file */
        public static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("Usage: TestTraining type [model file] [data file]");
                return;
            }

            var modelFilename = args[1];
            var dataFilename = args[2];
            var models = new KeyValuePair<List<DTree<string>>, double[]>();
            List<Example<string>> examples = null;

            var type = Type.GetType("Learning.DataWrapper" + args[0] + ", Learning");
            if (type != null){
                var dw = (DataWrapper<string>)System.Activator.CreateInstance(type);
                try {
                    models = loadModels(modelFilename);
                    examples = dw.LoadTrainingSet(dataFilename);
                } catch (IOException io) {
                    Console.WriteLine(io);
                    Console.ReadKey();
                    return;
                }
            }

            // Find the number of correct classifications
            var trees = models.Key;
            var weights = models.Value;
            var correct = 0;
            var total = 0;

            if (examples != null)
                foreach (var example in examples){
                    total++;
                    var weight = 0.0;
                    for (var i = 0; i < weights.Length; i++){
                        var eq = trees[i].Root.TestNode(example);
                        if (eq){
                            weight += weights[i];
                        } else{
                            weight -= weights[i];
                        }
                    }
                    if (weight > 0.0){
                        correct++;
                    }
                }

            var percentage = correct / (total * 1.0);
            Console.WriteLine("This model is " + percentage * 100 + "% accurate (according to the test data)");
            Console.ReadKey();
            
        }

        /* Load stumps from a ModelReader */
        static KeyValuePair<List<DTree<string>>, double[]> loadModels(string modelFilename) {
            return ModelReader<string>.ReadModel(modelFilename);
        }

    }
}
