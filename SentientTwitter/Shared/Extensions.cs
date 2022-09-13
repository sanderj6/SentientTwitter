namespace SentientTwitter.Data
{
    public static class Extensions
    {
        public static List<T> SearchModelForValue<T>(List<T> records, string searchTerm)
        {
            List<T> filteredRecords = new();

            foreach (var record in records)
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    var propValue = property.GetValue(record);
                    if (propValue is null) continue;

                    var value = propValue.ToString();

                    if (value is null) continue;

                    if (value.ToUpper().Trim().Contains(searchTerm.ToUpper().Trim())
                        && !filteredRecords.Contains(record))
                    {
                        filteredRecords.Add(record);
                    }
                }
            }

            return filteredRecords;
        }
    }
}