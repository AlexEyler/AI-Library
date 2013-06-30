/* **********************
 * Alex Eyler
 * Node class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    /* Node class */
    public class Node<T, V> where T : IComparable<T>
                            where V : IComparable<V>
    {
        // the node's attribute
        private Attribute<T> attribute;

        // the node's children (currently can only be 
        // classified nodes)
        public List<Node<T, V>> children;

        // the classification of the node (initially false,
        // but once it is set, it "becomes" the node)
        public V classification;

        // getters and setters
        public Attribute<T> Attribute {
            get { return this.attribute; }
            set { this.attribute = value; }
        }

        /* Default node constructor */
        public Node() {
            this.attribute = null;
            this.children = new List<Node<T,V>>();
            this.classification = default(V);
        }

        /* Construct node with an attribute */
        public Node(Attribute<T> attr) {
            this.attribute = attr;
            this.children = new List<Node<T,V>>();
            this.classification = default(V);
        }

        /* Add a child node to the node class */
        public void AddChild(Node<T,V> child) {
            children.Add(child);
        }

        /* Set a classification for the node, this makes 
         * the node a classified (leaf) node and attribute and children
         * become null */
        public void SetClassification(V classification) {
            this.classification = classification;
            this.attribute = null;
            this.children = null;
        }

        /* Determine if a node outputs the correct classification
         * compared to a given example */
        public bool TestNode(Example<T,V> example) {
            foreach (Attribute<T> a in example.Attributes) {
                if (a.Question == attribute.Question) {
                    T answer = a.SelectedAnswer;
                    int c = 0;
                    foreach (T ans in attribute.Answers) {
                        if (ans.Equals(answer)) {
                            return example.Classification.Equals(children[c].classification);
                        }
                        c++;
                    }
                    break;
                }
            }
            return false;
        }

        /* ToString method */
        public override string ToString() {
            string returnString = "";
            if (attribute != null) {
                returnString += attribute.Question + "?\n";
                int c = 0;
                foreach (T answer in attribute.Answers) {
                    returnString += "   " + answer + ": ";
                    returnString += children[c] + "\n";
                    c++;
                }
            } else {
                returnString += classification;
            }
            return returnString;
        }
    }
}
