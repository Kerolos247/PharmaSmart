namespace WebApplication4.Application.ChatAi_Component.Dto
{
    public class CacheItem
    {
        public string Question { get; set; }
        public float[] Vector { get; set; }
        public string Answer { get; set; }

        public double Magnitude { get; set; }
    }
}
