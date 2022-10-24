namespace Utils.Strings
{
    public static class StringUtils
    {
        public static string ToCurrencyString(this int amount)
        {
            if (amount == 0) return "0";

            string str = amount.ToString("##,#");
            str = str.Replace(',', '.');
            return str;
        }

        public static string ToCurrencyString(this float amount)
        {
            if (amount == 0) return "0";
            
            string str = amount.ToString();
            str = str.Replace(',', '.');
            return str;
        }
    }
}