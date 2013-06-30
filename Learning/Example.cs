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
    public class Example<TT, TV> where TT : System.IComparable<TT>
                               where TV : System.IComparable<TV>
    {
        // index in larger example list

        // id for comparison
        private readonly int id;

        // list of attributes
        public List<Attribute<TT>> Attributes;

        // final classification
        public TV Classification;

        // getters and setters
        public int Index { get; set; }

        public int Id {
            get { return this.id; }
        }

        /* Example constructor fills in attributes list and other fields */
        public Example(IReadOnlyList<string> questions, IReadOnlyList<TT> selectedAnswers, IReadOnlyCollection<List<TT>> attributeAnswers,
            TV output, int index, int id){
            if ((questions.Count != attributeAnswers.Count) || (questions.Count != selectedAnswers.Count)){
                throw new ArgumentException(
                    "The number of questions must be the same as the number of lists of answers and the number of selected answers");
            } else {
                Attributes = new List<Attribute<TT>>();
                var c = 0;
                foreach (var tempAttr in 
                    attributeAnswers.Select(answers => new Attribute<TT>(questions[c], answers))){
                    tempAttr.SelectedAnswer = selectedAnswers[c];
                    Attributes.Add(tempAttr);
                    c++;
                }
                this.Classification = output;
                this.Index = index;
                this.id = id;
            }
        }

        /* Call the other constructor with a default id of 0 */
        public Example(IReadOnlyList<string> questions, IReadOnlyList<TT> selectedAnswers, IReadOnlyCollection<List<TT>> attributeAnswers, TV output, int index) :
            this(questions, selectedAnswers, attributeAnswers, output, index, 0) {  }

        /* Determine if the example contains the input answer to the input question */
        public bool ContainsAnswer(string question, TT answer){
            return (from attr in Attributes 
                    where question.CompareTo(attr.Question) == 0 
                    select attr.SelectedAnswer.CompareTo(answer) == 0).FirstOrDefault();
        }

        /* Set the Attributes to the input answers */
        public void SetAnswers(TT[] answers) {
            var c = 0;
            foreach (var attr in Attributes) {
                attr.SelectedAnswer = answers[c];
                c++;
            }
        }
    }
}
