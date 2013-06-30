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
    public abstract class DataWrapper<TT, TV> where TT : IComparable<TT>
                                            where TV : IComparable<TV>
    {
        // type of dataWrapper
        public string Type;
        
        // lists of attributes and their possible answers
        public List<List<TT>> AttributeAnswers;
        public List<Attribute<TT>> Attributes;

        /* Must constructor with the type of data wrapper */
        protected DataWrapper(string type) {
            this.Type = type;
            AttributeAnswers = new List<List<TT>>();
            Attributes = new List<Attribute<TT>>();
        }

        /* Load training set from a file */
        abstract public List<Example<TT, TV>> LoadTrainingSet(string filename);

        /* Load data from a file to predict */
        abstract public List<Example<TT, TV>> LoadData(string filename);
    }
}
