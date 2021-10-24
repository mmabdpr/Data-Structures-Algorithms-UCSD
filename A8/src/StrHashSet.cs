using System.Collections.Generic;
using System.Linq;

namespace A10
{
    public class StrHashSet
    {
        private LinkedList<string>[] chains;
        private int bucketCount;
        
        public StrHashSet(int bucketCount)
        {
            this.bucketCount = bucketCount;
            chains = new LinkedList<string>[bucketCount];
        }

        public void Add(string s)
        {
            int hash = Hash(s);

            if (Find(s))
                return;

            if (chains[hash] == null)
                chains[hash] = new LinkedList<string>();
            
            chains[hash].AddFirst(s);
        }

        public bool Find(string s)
        {
            int hash = Hash(s);
            
            if (chains[hash] == null)
                return false;
            
            if (chains[hash].Contains(s))
                return true;
            
            return false;
        }

        public void Remove(string s)
        {
            int hash = Hash(s);

            if (chains[hash] == null)
                return;

            chains[hash].Remove(s);

            if (chains[hash].Count == 0)
                chains[hash] = null;
        }

        public string Check(int i)
        {
            return chains[i] == null ? "-" : string.Join(' ', chains[i]); 
        }

        private const long BigPrimeNumber = 1000000007;
        private const long ChosenX = 263;

        private int Hash(string str)
        {
            long hash = 0;
            for (int i = str.Length - 1; i >= 0; --i)
                hash = (hash * ChosenX + str[i]) % BigPrimeNumber;
            return (int)hash % bucketCount;
        }
    }
}