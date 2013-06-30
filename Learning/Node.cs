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
    public class Node<TT> where TT : IComparable<TT>
    {
        // the node's attribute
        private Attribute<TT> attribute;

        // the node's children (currently can only be 
        // classified nodes)
        public List<Node<TT>> Children;

        // the classification of the node (initially -1,
        // but once it is set, it "becomes" the node)
        public int Classification;

        // getters and setters
        public Attribute<TT> Attribute {
            get { return this.attribute; }
            set { this.attribute = value; }
        }

        /* Default node constructor */
        public Node() {
            this.attribute = null;
            this.Children = new List<Node<TT>>();
            this.Classification = -1;
        }

        /* Construct node with an attribute */
        public Node(Attribute<TT> attr) {
            this.attribute = attr;
            this.Children = new List<Node<TT>>();
            this.Classification = -1;
        }

        /* Add a child node to the node class */
        public void AddChild(Node<TT> child) {
            Children.Add(child);
        }

        /* Set a classification for the node, this makes 
         * the node a classified (leaf) node and attribute and children
         * become null */
        public void SetClassification(int classification) {
            this.Classification = classification;
            this.attribute = null;
            this.Children = null;
        }

        /* Determine if a node outputs the correct classification
         * compared to a given example */
        public bool TestNode(Example<TT> example) {
            foreach (var a in example.Attributes) {
                if (a.Question != attribute.Question) continue;
                var answer = a.SelectedAnswer;
                var c = 0;
                foreach (var ans in attribute.Answers) {
                    if (ans.Equals(answer))
                        return example.Classification.Equals(Children[c].Classification);
                    c++;
                }
                break;
            }
            return false;
        }

        /* ToString method */
        public override string ToString() {
            var returnString = "";
            if (attribute != null) {
                returnString += attribute.Question + "?\n";
                var c = 0;
                foreach (var answer in attribute.Answers) {
                    returnString += "   " + answer + ": ";
                    returnString += Children[c] + "\n";
                    c++;
                }
            } else {
                returnString += Classification;
            }
            return returnString;
        }
    }
}
