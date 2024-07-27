using Data;

namespace Utils
{
    public static class FormattingHelper
    {
        public static string ToSeparatedTimeFormat(int timeSeconds)
        {
            var hours = timeSeconds / 3600;
            var restSeconds = timeSeconds % 3600;
            var minutes = restSeconds / 60;
            restSeconds %= 60;
            
            return hours > 0 ? $"{GetTwoDigitsString(hours)}:{GetTwoDigitsString(minutes)}:{GetTwoDigitsString(restSeconds)}" : $"{GetTwoDigitsString(minutes)}:{GetTwoDigitsString(restSeconds)}";
        }

        public static string ToCommaSeparatedNumber(int amount)
        {
            return $"{amount:n0}";
        }

        public static string ToMoneyWithIconTextFormat(int moneyAmount)
        {
            return $"{Constants.TextIconMoney}{moneyAmount}";
        }

        private static string GetTwoDigitsString(int value)
        {
            return value < 10 ? $"0{value}" : value.ToString();
        }
    }
}
