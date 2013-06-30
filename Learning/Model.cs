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
        public static readonly string START_MODELS = "StartModels:";
        public static readonly string END_MODELS = "EndModels";
        public static readonly string START_MODEL = "StartModel:";
        public static readonly string END_MODEL = "EndModel";
        public static readonly string START_ATTRIBUTE = "StartAttribute:";
        public static readonly string END_ATTRIBUTE = "EndAttribute";
    }

    /* Model writer class */
    public class ModelWriter<T, V> where T : IComparable<T>
                                   where V : IComparable<V>
    {
        /* Static write model function, creates a model and saves it in the path */
        public static bool WriteModel(KeyValuePair<List<DTree<T, V>>, double[]> model, string path) {
            try {
                FileStream modelStream = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(modelStream);

                writer.WriteLine(Model.START_MODELS);
                int c = 0;
                foreach (DTree<T, V> tree in model.Key) {
                    writer.WriteLine(Model.START_MODEL);
                    Node<T, V> node = tree.Root;

                    writer.WriteLine(Model.START_ATTRIBUTE);
                    writeAttribute(node.Attribute, ref writer);
                    writeClassifications(node, ref writer);
                    writeWeight(model.Value[c], ref writer);
                    writer.WriteLine(Model.END_ATTRIBUTE);

                    c++;
                    writer.WriteLine(Model.END_MODEL);
                }
                writer.WriteLine(Model.END_MODELS);
                writer.Flush();
            } catch (Exception) {
                return false;
            }
            return true;
        }

        /* Helper function for WriteModel */
        private static void writeAttribute(Attribute<T> attr, ref StreamWriter writer) {
            writer.Write("\t" + attr.Question + ":");
            for (int i = 0; i < attr.Answers.Count - 1; i++) {
                writer.Write("" + attr.Answers[i] + ",");
            }
            writer.Write("" + attr.Answers[attr.Answers.Count - 1] + ".\r\n");
        }

        /* Helper function for WriteModel */
        private static void writeClassifications(Node<T, V> node, ref StreamWriter writer) {
            writer.Write("\tclassifications:");
            for (int i = 0; i < node.children.Count - 1; i++) {
                writer.Write("" + node.children[i].classification + ",");
            }
            writer.Write("" + node.children[node.children.Count - 1] + ".\r\n");
        }

        /* Helper function for WriteModel */
        private static void writeWeight(double weight, ref StreamWriter writer) {
            writer.Write("\tweight:" + Convert.ToString(weight) + ".\r\n");
        }
    }

    /* Model reader class */
    public class ModelReader<T, V> where T : IComparable<T>
                                   where V : IComparable<V>
    {
        /* Read a Model from the given file */
        public static KeyValuePair<List<DTree<T, V>>, double[]> ReadModel(string path) {
            FileStream modelStream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(modelStream);
            
            string line = reader.ReadLine();
            if (line.CompareTo(Model.START_MODELS) != 0) {
                throw new InvalidDataException("Invalid model format");
            }

            bool inModel = false;
            bool inAttribute = false;

            List<DTree<T, V>> trees = new List<DTree<T, V>>();
            List<double> weights = new List<double>();
            int curAttr = -1;
            while ((line = reader.ReadLine()).CompareTo(Model.END_MODELS) != 0) {
                if (inModel) {
                    if (line.Length > 16 && line.Substring(0, 16).CompareTo("\tclassifications") == 0) {
                        string[] chars = line.Substring(17).Split(',');
                        int c = 0;
                        Node<T, V> temp;
                        do {
                            temp = new Node<T, V>();
                            temp.SetClassification((V)Convert.ChangeType(chars[c], typeof(V)));
                            trees[curAttr].Root.AddChild(temp);
                            c++;
                        } while (chars[c][chars[c].Length - 1] != '.');

                        // add final classification
                        temp = new Node<T, V>();
                        temp.SetClassification((V)Convert.ChangeType(chars[c], typeof(V)));
                        trees[curAttr].Root.AddChild(temp);
                    } else if (line.Length > 7 && line.Substring(0, 7).CompareTo("\tweight") == 0) {
                        string weightString = line.Substring(8, line.Length - 9);
                        double weight = Convert.ToDouble(weightString);
                        weights.Add(weight);
                    } else if (line.CompareTo(Model.START_ATTRIBUTE) == 0) {
                        if (inAttribute) {
                            throw new InvalidDataException("Invalid model format");
                        }
                        trees.Add(new DTree<T, V>());
                        curAttr++;
                        inAttribute = true;
                    } else if (line.CompareTo(Model.END_ATTRIBUTE) == 0) {
                        if (!inAttribute) {
                            throw new InvalidDataException("Invalid model format");
                        }
                        inAttribute = false;
                    } else if (line.CompareTo(Model.END_MODEL) == 0) {
                        if (!inModel) {
                            throw new InvalidDataException("Invalid model format");
                        }
                        inModel = false;
                    } else {
                        // hopefully, line is question:a1,a2,a3
                        string question = "";
                        int i = 1;
                        do {
                            question += line[i];
                            i++;
                        } while (line[i] != ':');
                        i++;
                        trees[curAttr].Root.Attribute = new Attribute<T>(question);

                        string[] answers = line.Substring(i).Split(',');
                        int a = 0;
                        do {
                            trees[curAttr].Root.Attribute.AddAnswer(answers[a]);
                            a++;
                        } while (answers[a][answers[a].Length - 1] != '.');

                        trees[curAttr].Root.Attribute.AddAnswer(answers[a].Substring(0, answers[a].Length - 1));
                    }
                } else if (line.CompareTo(Model.START_MODEL) == 0) {
                    if (inModel) {
                        throw new InvalidDataException("Invalid model format");
                    }
                    inModel = true;
                } else if (line.CompareTo(Model.END_MODEL) == 0) {
                    if (!inModel) {
                        throw new InvalidDataException("Invalid model format");
                    }
                    inModel = false;
                } else {
                    throw new InvalidDataException("Invalid model format");
                }
            }
            return new KeyValuePair<List<DTree<T, V>>, double[]>(trees, weights.ToArray());
        }

    }
}
