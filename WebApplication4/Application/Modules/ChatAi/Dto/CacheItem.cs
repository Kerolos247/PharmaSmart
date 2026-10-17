namespace WebApplication4.Application.Modules.ChatAi.Dto
{
    public class CacheItem
    {
        public string Question { get; set; }
        public float[] Vector { get; set; }
        public string Answer { get; set; }

        public double Magnitude { get; set; }
    }
}
