/* **********************
 * Alex Eyler
 * Attribute class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    /* Attribute class */
    public class Attribute<T> where T : System.IComparable<T>
    {
        // list of possible answers to the attribute
        private readonly List<T> answers;

        // the question that the attribute asks
        private readonly string question;

        // the selected answer (used for examples)

        // getters and setters
        public List<T> Answers {
            get { return this.answers; }
        }

        public string Question {
            get { return this.question; }
        }

        public T SelectedAnswer { get; set; }

        /* Default Constructor */
        public Attribute() {
            answers = new List<T>();
            question = "";
            SelectedAnswer = default(T);
        }

        /* Constructor with question input */
        public Attribute(string question) {
            this.question = question;
            this.answers = new List<T>();
            SelectedAnswer = default(T);
        }

        /* Constructor with question and possible answers input */
        public Attribute(string question, List<T> answers) {
            this.question = question;
            this.answers = answers;
            SelectedAnswer = default(T);
        }

        /* Add an answer to the possible answers */
        public void AddAnswer(string answer) {
            answers.Add((T)Convert.ChangeType(answer, typeof(T)));
        }
    }
}
