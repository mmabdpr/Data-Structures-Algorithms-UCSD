using System;
using System.Linq;
using System.Collections.Generic;
using TestCommon;

namespace A10
{
    public class PhoneBook : Processor
    {
        public PhoneBook(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string[], string[]>)Solve);

        protected string[] PhoneBookList;

        public string[] Solve(string [] commands)
        {
            PhoneBookList = new string[(int)Math.Pow(10, 7)];
            List<string> result = new List<string>();
            foreach(var cmd in commands)
            {
                var toks = cmd.Split();
                var cmdType = toks[0];
                var args = toks.Skip(1).ToArray();
                int number = int.Parse(args[0]);
                switch (cmdType)
                {
                    case "add":
                        Add(args[1], number);
                        break;
                    case "del":
                        Delete(number);
                        break;
                    case "find":
                        result.Add(Find(number));
                        break;
                }
            }
            return result.ToArray();
        }

        public void Add(string name, int number)
        {
            PhoneBookList[number] = name;
        }

        public string Find(int number)
        {
            if (PhoneBookList[number] == null)
                return "not found";
            return PhoneBookList[number];
        }

        public void Delete(int number)
        {
            PhoneBookList[number] = null;
        }
    }
}
