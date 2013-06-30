/* **********************
 * Alex Eyler
 * Model class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    /* Model class, just contains static strings for the model file format */
    public static class Model
    {
        public static readonly string StartModels = "StartModels:";
        public static readonly string EndModels = "EndModels";
        public static readonly string StartModel = "StartModel:";
        public static readonly string EndModel = "EndModel";
        public static readonly string StartAttribute = "StartAttribute:";
        public static readonly string EndAttribute = "EndAttribute";
    }

    /* Model writer class */
    public class ModelWriter<TT, TV> where TT : IComparable<TT>
                                   where TV : IComparable<TV>
    {
        /* Static write model function, creates a model and saves it in the path */
        public static bool WriteModel(KeyValuePair<List<DTree<TT, TV>>, double[]> model, string path) {
            try {
                var modelStream = new FileStream(path, FileMode.OpenOrCreate);
                var writer = new StreamWriter(modelStream);

                writer.WriteLine(Model.StartModels);
                var c = 0;
                foreach (var tree in model.Key) {
                    writer.WriteLine(Model.StartModel);
                    var node = tree.Root;

                    writer.WriteLine(Model.StartAttribute);
                    writeAttribute(node.Attribute, ref writer);
                    writeClassifications(node, ref writer);
                    writeWeight(model.Value[c], ref writer);
                    writer.WriteLine(Model.EndAttribute);

                    c++;
                    writer.WriteLine(Model.EndModel);
                }
                writer.WriteLine(Model.EndModels);
                writer.Flush();
            } catch (Exception) {
                return false;
            }
            return true;
        }

        /* Helper function for WriteModel */
        private static void writeAttribute(Attribute<TT> attr, ref StreamWriter writer) {
            writer.Write("\t" + attr.Question + ":");
            for (var i = 0; i < attr.Answers.Count - 1; i++) {
                writer.Write("" + attr.Answers[i] + ",");
            }
            writer.Write("" + attr.Answers[attr.Answers.Count - 1] + ".\r\n");
        }

        /* Helper function for WriteModel */
        private static void writeClassifications(Node<TT, TV> node, ref StreamWriter writer) {
            writer.Write("\tclassifications:");
            for (var i = 0; i < node.Children.Count - 1; i++) {
                writer.Write("" + node.Children[i].Classification + ",");
            }
            writer.Write("" + node.Children[node.Children.Count - 1] + ".\r\n");
        }

        /* Helper function for WriteModel */
        private static void writeWeight(double weight, ref StreamWriter writer) {
            writer.Write("\tweight:" + Convert.ToString(weight) + ".\r\n");
        }
    }

    /* Model reader class */
    public class ModelReader<TT, TV> where TT : IComparable<TT>
                                   where TV : IComparable<TV>
    {
        /* Read a Model from the given file */
        public static KeyValuePair<List<DTree<TT, TV>>, double[]> ReadModel(string path) {
            var modelStream = new FileStream(path, FileMode.Open);
            var reader = new StreamReader(modelStream);
            
            var line = reader.ReadLine();
            if (line != null && line.CompareTo(Model.StartModels) != 0) {
                throw new InvalidDataException("Invalid model format");
            }

            var inModel = false;
            var inAttribute = false;

            var trees = new List<DTree<TT, TV>>();
            var weights = new List<double>();
            var curAttr = -1;
            var s = line = reader.ReadLine();
            while (s != null && s.CompareTo(Model.EndModels) != 0) {
                if (inModel) {
                    if (line.Length > 16 && line.Substring(0, 16).CompareTo("\tclassifications") == 0) {
                        var chars = line.Substring(17).Split(',');
                        var c = 0;
                        Node<TT, TV> temp;
                        do {
                            temp = new Node<TT, TV>();
                            temp.SetClassification((TV)Convert.ChangeType(chars[c], typeof(TV)));
                            trees[curAttr].Root.AddChild(temp);
                            c++;
                        } while (chars[c][chars[c].Length - 1] != '.');

                        // add final classification
                        temp = new Node<TT, TV>();
                        temp.SetClassification((TV)Convert.ChangeType(chars[c], typeof(TV)));
                        trees[curAttr].Root.AddChild(temp);
                    } else if (line.Length > 7 && line.Substring(0, 7).CompareTo("\tweight") == 0) {
                        var weightString = line.Substring(8, line.Length - 9);
                        var weight = Convert.ToDouble(weightString);
                        weights.Add(weight);
                    } else if (line.CompareTo(Model.StartAttribute) == 0) {
                        if (inAttribute) {
                            throw new InvalidDataException("Invalid model format");
                        }
                        trees.Add(new DTree<TT, TV>());
                        curAttr++;
                        inAttribute = true;
                    } else if (line.CompareTo(Model.EndAttribute) == 0) {
                        if (!inAttribute) {
                            throw new InvalidDataException("Invalid model format");
                        }
                        inAttribute = false;
                    } else if (line.CompareTo(Model.EndModel) == 0) {
                        inModel = false;
                    } else {
                        // hopefully, line is question:a1,a2,a3
                        var question = "";
                        var i = 1;
                        do {
                            question += line[i];
                            i++;
                        } while (line[i] != ':');
                        i++;
                        trees[curAttr].Root.Attribute = new Attribute<TT>(question);

                        var answers = line.Substring(i).Split(',');
                        var a = 0;
                        do {
                            trees[curAttr].Root.Attribute.AddAnswer(answers[a]);
                            a++;
                        } while (answers[a][answers[a].Length - 1] != '.');

                        trees[curAttr].Root.Attribute.AddAnswer(answers[a].Substring(0, answers[a].Length - 1));
                    }
                } else if (line.CompareTo(Model.StartModel) == 0) {
                    inModel = true;
                } else if (line.CompareTo(Model.EndModel) == 0) {
                    throw new InvalidDataException("Invalid model format");
                } else {
                    throw new InvalidDataException("Invalid model format");
                }
            }
            return new KeyValuePair<List<DTree<TT, TV>>, double[]>(trees, weights.ToArray());
        }

    }
}
