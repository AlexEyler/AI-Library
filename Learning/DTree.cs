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
    public class DTree<T, V> where T : IComparable<T>
                          where V : IComparable<V>
    {
        // root node
        private Node<T, V> root;

        // list of all possible classifications
        private List<V> classifications;

        // getter
        public Node<T, V> Root {
            get { return this.root; }
        }

        /* Default constructor */
        public DTree() {
            root = new Node<T, V>();
        }

        /* Build the DStump when we create a DTree object */
        public DTree(List<Example<T, V>> examples, List<Attribute<T>> attributes, double[] weights, List<V> classifications) {
            this.classifications = classifications;
            if (weights.Count() != examples.Count) {
                throw new ArgumentException("There must be a weight for each example");
            }
            root = buildDStump(examples, attributes, weights);
        }

        /* Build-DStump function */
        private Node<T,V> buildDStump(List<Example<T, V>> examples, List<Attribute<T>> attributes, double[] weights) {
            // find best attribute
            Attribute<T> b = bestAttribute(examples, attributes, weights);
            Node<T, V> node = new Node<T, V>(b);
            foreach (T answer in b.Answers) {
                List<Example<T, V>> selectedExamples = getSelectedExamples(examples, b.Question, answer);
                if (selectedExamples.Count == 0) {
                    // examples = {}
                    Node<T, V> child = new Node<T, V>();
                    child.SetClassification(classifications[new Random().Next(classifications.Count - 1)]);
                    node.AddChild(child);
                } else {
                    // check to see if every example agrees
                    bool agreementFlag = true;
                    V output = selectedExamples[0].Classification;
                    foreach (Example<T,V> example in selectedExamples) {
                        if (!(example.Classification.Equals(output))) {
                            agreementFlag = false;
                            break;
                        }
                    }
                    if (agreementFlag) {
                        // set classification to agreement
                        Node<T,V> child = new Node<T,V>();
                        child.SetClassification(output);
                        node.AddChild(child);
                    } else {
                        // find majority classification
                        double[] ws = new double[classifications.Count];
                        for (int e = 0; e < selectedExamples.Count; e++) {
                            int idx = classifications.IndexOf(selectedExamples[e].Classification);
                            ws[idx] += weights[idx];
                        }
                        Node<T,V> child = new Node<T,V>();
                        int maxIndex = ws.Select( (value, index) => new { Value = value, Index = index} )
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
        private List<Example<T,V>> getSelectedExamples(List<Example<T,V>> examples, string question, T answer) {
            List<Example<T,V>> answeredExamples = new List<Example<T,V>>();
            foreach (Example<T,V> example in examples) {
                if (example.ContainsAnswer(question, answer)) {
                    answeredExamples.Add(example);
                }
            }
            return answeredExamples;
        }

        /* Find the best attribute to use (the lowest entropy) */
        private Attribute<T> bestAttribute(List<Example<T,V>> examples, List<Attribute<T>> attributes, double[] weights) {
            double bestE = Double.MaxValue;
            Attribute<T> bestAttr = null;
            foreach (Attribute<T> attr in attributes) {
                double totalE = 0.0;
                foreach (T answer in attr.Answers) {
                    List<Example<T,V>> selectedExamples = getSelectedExamples(examples, attr.Question, answer);
                    List<double> selectedWeights = new List<double>();
                    foreach (Example<T,V> ex in selectedExamples) {
                        selectedWeights.Add(weights[ex.Index]);
                    }
                    double sumSelectedWeights = selectedWeights.Sum();
                    double sumWeights = weights.Sum();

                    double E = entropy(selectedExamples, selectedWeights.ToArray());
                    totalE += (sumSelectedWeights / sumWeights) * E;
                }
                if (totalE < bestE) {
                    bestE = totalE;
                    bestAttr = attr;
                }
            }
            return bestAttr;
        }

        /* Find the entropy of a list of examples and weights */
        private double entropy(List<Example<T,V>> examples, double[] weights) {
            double[] ws = new double[classifications.Count];
            double totalWeight = weights.Sum();
            for (int c = 0; c < examples.Count; c++) {
                int idx = classifications.IndexOf(examples[c].Classification);
                ws[idx] += weights[c];
            }
            double ent = 0.0;
            foreach (double w in ws) {
                ent -= (w / totalWeight) * Math.Log(w / totalWeight, classifications.Count);
            }
            return ent;
        }
    }
}
