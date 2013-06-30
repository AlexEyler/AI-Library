/* **********************
 * Alex Eyler
 * DataWrapper class
 * AI Project 2
 * **********************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning
{
    /* Abstract class to load data from files */
    public abstract class DataWrapper<T, V> where T : IComparable<T>
                                            where V : IComparable<V>
    {
        // type of dataWrapper
        public string type;
        
        // lists of attributes and their possible answers
        public List<List<T>> AttributeAnswers;
        public List<Attribute<T>> Attributes;

        /* Must constructor with the type of data wrapper */
        public DataWrapper(string type) {
            this.type = type;
            AttributeAnswers = new List<List<T>>();
            Attributes = new List<Attribute<T>>();
        }

        /* Load training set from a file */
        abstract public List<Example<T, V>> LoadTrainingSet(string filename);

        /* Load data from a file to predict */
        abstract public List<Example<T, V>> LoadData(string filename);
    }
}
