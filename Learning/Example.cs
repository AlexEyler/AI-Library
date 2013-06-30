/* **********************
 * Alex Eyler
 * Example class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    /* Example class */
    public class Example<T, V> where T : System.IComparable<T>
                               where V : System.IComparable<V>
    {
        // index in larger example list
        private int index;
        
        // id for comparison
        private int id;

        // list of attributes
        public List<Attribute<T>> Attributes;

        // final classification
        public V Classification;

        // getters and setters
        public int Index {
            get { return this.index; }
            set { this.index = value; }
        }

        public int Id {
            get { return this.id; }
        }

        /* Example constructor fills in attributes list and other fields */
        public Example(List<string> questions, List<T> selectedAnswers, List<List<T>> attributeAnswers, V output, int index, int id) {
            if ((questions.Count == attributeAnswers.Count) && (questions.Count == selectedAnswers.Count)) {
                Attributes = new List<Attribute<T>>();
                int c = 0;
                foreach (List<T> answers in attributeAnswers) {
                    Attribute<T> tempAttr = new Attribute<T>(questions[c], answers);
                    tempAttr.SelectedAnswer = selectedAnswers[c];
                    Attributes.Add(tempAttr);
                    c++;
                }
                this.Classification = output;
                this.index = index;
                this.id = id;
            } else {
                throw new ArgumentException("The number of questions must be the same as the number of lists of answers and the number of selected answers");
            }
        }

        /* Call the other constructor with a default id of 0 */
        public Example(List<string> questions, List<T> selectedAnswers, List<List<T>> attributeAnswers, V output, int index) :
            this(questions, selectedAnswers, attributeAnswers, output, index, 0) {  }

        /* Determine if the example contains the input answer to the input question */
        public bool ContainsAnswer(string question, T answer) {
            foreach (Attribute<T> attr in Attributes) {
                if (question.CompareTo(attr.Question) == 0) {
                    return attr.SelectedAnswer.CompareTo(answer) == 0;
                }
            }
            return false;
        }

        /* Set the Attributes to the input answers */
        public void SetAnswers(T[] answers) {
            int c = 0;
            foreach (Attribute<T> attr in Attributes) {
                attr.SelectedAnswer = answers[c];
                c++;
            }
        }
    }
}
