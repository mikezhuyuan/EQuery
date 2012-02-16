using System;

namespace EQuery.Core
{
    class BracketedName
    {
        public BracketedName(string raw)
        {
            if(string.IsNullOrEmpty(raw))
                throw new ArgumentNullException();
            
            if ((raw[0] == '[') != (raw[raw.Length-1] == ']'))
                throw new ArgumentException("Invalid name");
            
            Raw = raw[0] == '[' ? raw.Substring(1, raw.Length - 2) : raw;

            Value = '[' + Raw + ']';            
        }

        public string Raw { get; private set; }
        public string Value { get; private set; }

        public static implicit operator BracketedName(string raw)
        {
            return new BracketedName(raw);
        }

        public static implicit operator string(BracketedName name)
        {
            return name.Value;
        }

        public static string operator +(BracketedName name1, BracketedName name2)
        {
            return name1.Value + '.' + name2.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}