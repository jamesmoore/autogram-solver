namespace Autogram
{
    public class Status
    {
        public string CurrentString { get; set; }
        public bool Success { get; set; }
        public int HistoryCount { get; set; }
        public bool RandomReset { get; set; }
        public int[] GuessError { get; set; }
    }
}
