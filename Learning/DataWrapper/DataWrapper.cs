/* **********************
 * Alex Eyler
 * DataWrapper class
 * AI Project 2
 * **********************/

using System;
using System.Collections.Generic;

namespace Learning.DataWrapper
{
    /* Abstract class to load data from files */
    public abstract class DataWrapper<TT> where TT : IComparable<TT>
    {
        // type of dataWrapper
        public string Type;
        
        // lists of attributes and their possible answers
        public List<List<TT>> AttributeAnswers;
        public List<Attribute<TT>> Attributes;

        // counts of classifications 
        public int[] ClassificationsCount;

        /* Must construct with the type of data wrapper */
        protected DataWrapper(string type) {
            this.Type = type;
            AttributeAnswers = new List<List<TT>>();
            Attributes = new List<Attribute<TT>>();
        }

        /* Keep track of whether the different types of classifications */
        public void ApplyClassification(int classification) {
            ClassificationsCount[classification]++;
        }

        /* Load training set from a file */
        abstract public List<Example<TT>> LoadTrainingSet(string filename);

        /* Load data from a file to predict */
        abstract public List<Example<TT>> LoadData(string filename);

        /* Return a determination for the future */
        public abstract string Determiniation();
    }
}
