namespace ConsoleAppOpenAI.DALL_E.Models.Images
{
    public class GenerateImageResponse
    {
        public long Created { get; set; }

        public GeneratedImageData[] Data { get; set; }
    }

    public class GeneratedImageData
    {
        public string Url { get; set; }
    }
}
