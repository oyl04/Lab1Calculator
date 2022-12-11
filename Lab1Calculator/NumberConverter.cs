namespace Lab1Calculator
{
    public struct CellCoordinates
    {
        public int row;
        public int column;
    }
    static public class NumberConverter
    {
        private const int CharCodea = 'a';
        private const int CharCodeA = 'A';
        private const int CharCode0 = '0';
        private const int AlphabetSize = 26;
        private const int DigitsSize = 10;

        private static string StrReverse(string s)
        {
            string res = "";
            if (s.Length != 0)
            {
                for (int i = s.Length - 1; i >= 0; --i)
                {
                    res += s[i];
                }
            }
            return res;
        }

            public static string FromIntegerToChars(int number)
        {
            number++;
            int mod;
            string columnName = "";
            if (number == 0) ((char)(CharCodeA - 1)).ToString();
            while (number > 0)
            {
                mod = (number - 1) % AlphabetSize;
                columnName += (((char)(CharCodeA + mod)).ToString());
                number = (number - mod) / AlphabetSize;
            }
            return StrReverse(columnName);
        }
        public static CellCoordinates From26System(string x)
        {
            CellCoordinates ans = new CellCoordinates(); ans.column = 0;
            ans.row = 0;
            for (int i = 0; i < x.Length; ++i)
            {
                if (x[i] >= CharCodeA && x[i] < CharCodeA + AlphabetSize)
                {
                    ans.column *= AlphabetSize;
                    ans.column += x[i] - CharCodeA + 1;
                }
                if (x[i] >= CharCodea && x[i] < CharCodea + AlphabetSize)
                {
                    ans.column *= AlphabetSize;
                    ans.column += x[i] - CharCodea + 1;
                }
                else if (x[i] >= CharCode0 && x[i] < CharCode0 + DigitsSize)
                {
                    ans.row *= DigitsSize;
                    ans.row += x[i] - CharCode0;
                }
            }
            ans.column--;
            ans.row--;
            return ans;
        }
    }
}
