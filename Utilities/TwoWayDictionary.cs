using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{

    //Classe pour un dictionaire bi-directionel, à compléter au fur et à mesure des besoins
    public class TwoWayDictionaryIndexer<T3, T4>
    {
        private Dictionary<T3, T4> _dictionary;
        public TwoWayDictionaryIndexer(Dictionary<T3, T4> dictionary)
        {
            _dictionary = dictionary;
        }
        public T4 this[T3 index]
        {
            get { return _dictionary[index]; }
            set { _dictionary[index] = value; }
        }
    }

    public class TwoWayDictionary<T1, T2>
    {
        private Dictionary<T1, T2> _KeyToValue = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _ValueToKey = new Dictionary<T2, T1>();

        public TwoWayDictionary()
        {
            this.KeyToValue = new TwoWayDictionaryIndexer<T1, T2>(_KeyToValue);
            this.ValueToKey = new TwoWayDictionaryIndexer<T2, T1>(_ValueToKey);
        }

        public TwoWayDictionary(params Tuple<T1, T2>[] i_initMap)
        {
            int nbItems = i_initMap.Count();

            //petit gains de perfo en allouant directement le bon nombre d'items
            _KeyToValue = new Dictionary<T1, T2>(nbItems);
            _ValueToKey = new Dictionary<T2, T1>(nbItems);

            foreach (Tuple<T1, T2> t in i_initMap)
            {
                this.Add(t.Item1, t.Item2);
            }
        }



        public void Add(T1 t1, T2 t2)
        {
            try
            {
                _KeyToValue.Add(t1, t2);
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                _ValueToKey.Add(t2, t1);
            }
            catch (Exception e)
            {
                _KeyToValue.Remove(t1);
                throw e;
            }

        }


        public T2 this[T1 index]
        {
            get { return _KeyToValue[index]; }
            set { _KeyToValue[index] = value; }
        }

        public T1 this[T2 index]
        {
            get { return _ValueToKey[index]; }
            set { _ValueToKey[index] = value; }
        }

        public TwoWayDictionaryIndexer<T1, T2> KeyToValue { get; private set; }
        public TwoWayDictionaryIndexer<T2, T1> ValueToKey { get; private set; }

    }
}
