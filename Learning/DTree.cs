/* **********************
 * Alex Eyler
 * DTree class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    /* DTree Class (really just DStumps for now) */
    public class DTree<TT> where TT : IComparable<TT>
    {
        // list of all possible classifications
        private readonly List<int> classifications;

        // getter
        public Node<TT> Root { get; set; }

        /* Default constructor */
        public DTree() {
            Root = new Node<TT>();
        }

        /* Build the DStump when we create a DTree object */
        public DTree(IList<Example<TT>> examples, IEnumerable<Attribute<TT>> attributes, IList<double> weights, List<int> classifications) {
            this.classifications = classifications;
            if (weights.Count() != examples.Count) {
                throw new ArgumentException("There must be a weight for each example");
            }
            Root = buildDStump(examples, attributes, weights);
        }

        /* Build-DStump function */
        private Node<TT> buildDStump(IList<Example<TT>> examples, IEnumerable<Attribute<TT>> attributes, IList<double> weights) {
            // find best attribute
            var b = bestAttribute(examples, attributes, weights);
            var node = new Node<TT>(b);
            foreach (var selectedExamples in 
                b.Answers.Select(answer => getSelectedExamples(examples, b.Question, answer))){

                if (selectedExamples.Count == 0) {
                    // examples = {}
                    var child = new Node<TT>();
                    child.SetClassification(classifications[new Random().Next(classifications.Count - 1)]);
                    node.AddChild(child);
                } else {
                    // check to see if every example agrees
                    var output = selectedExamples[0].Classification;
                    var agreementFlag = selectedExamples.All(example => example.Classification.Equals(output));
                    if (agreementFlag) {
                        // set classification to agreement
                        var child = new Node<TT>();
                        child.SetClassification(output);
                        node.AddChild(child);
                    } else {
                        // find majority classification
                        var ws = new double[classifications.Count];
                        foreach (var idx in 
                            selectedExamples.Select(t => classifications.IndexOf(t.Classification))){
                            ws[idx] += weights[idx];
                        }
                        var child = new Node<TT>();
                        var maxIndex = ws.Select( (value, index) => new { Value = value, Index = index} )
                            .Aggregate
                            ( (x, y) => (x.Value > y.Value) ? x : y)
                            .Index;
                        child.SetClassification(classifications[maxIndex]);
                        node.AddChild(child);
                    }
                }
            }
            return node;
        }

        /* Get the examples whose answer to the input question is the input answer */
        private List<Example<TT>> getSelectedExamples(IEnumerable<Example<TT>> examples, string question, TT answer) {
            return examples.Where(example => example.ContainsAnswer(question, answer)).ToList();
        }

        /* Find the best attribute to use (the lowest entropy) */
        private Attribute<TT> bestAttribute(IList<Example<TT>> examples, IEnumerable<Attribute<TT>> attributes, IList<double> weights) {
            var bestE = Double.MaxValue;
            Attribute<TT> bestAttr = null;
            foreach (var attr in attributes) {
                var totalE = 0.0;
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (var answer in attr.Answers) {
                    // ReSharper restore LoopCanBeConvertedToQuery
                    var selectedExamples = getSelectedExamples(examples, attr.Question, answer);
                    var selectedWeights = selectedExamples.Select(ex => weights[ex.Index]).ToList();
                    var sumSelectedWeights = selectedWeights.Sum();
                    var sumWeights = weights.Sum();

                    var e = entropy(selectedExamples, selectedWeights.ToArray());
                    totalE += (sumSelectedWeights / sumWeights) * e;
                }
                if (!(totalE < bestE)) continue;

                bestE = totalE;
                bestAttr = attr;
            }
            return bestAttr;
        }

        /* Find the entropy of a list of examples and weights */
        private double entropy(IList<Example<TT>> examples, IList<double> weights) {
            var ws = new double[classifications.Count];
            var totalWeight = weights.Sum();
            for (var c = 0; c < examples.Count; c++) {
                var idx = classifications.IndexOf(examples[c].Classification);
                ws[idx] += weights[c];
            }
            return ws.Where(d => Math.Abs(d - 0.0) > .000000000001).Aggregate(0.0, (current, d) => current - (d/totalWeight)*Math.Log(d/totalWeight, 2));
        }
    }
}
