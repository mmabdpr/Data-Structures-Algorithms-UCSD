namespace A10
{
    public class HashUtils
    {
        public static int Hash(int x)
        {
            int p = 10000019;
            int a = 34;
            int b = 2;
            return (a * x + b) % p;
        }

        public static int Hash(string s, int bucketCount)
        {
            int multiplier = 263;
            int prime = 1000000007;
            long hash = 0;
            for (int i = s.Length - 1; i >= 0; --i)
                hash = (hash * multiplier + s[i]) % prime;
            return (int)hash % bucketCount;
        }
    }
}