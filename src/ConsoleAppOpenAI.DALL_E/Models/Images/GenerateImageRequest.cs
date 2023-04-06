namespace ConsoleAppOpenAI.DALL_E.Models.Images
{
    public class GenerateImageRequest
    {
        public string Prompt { get; set; }

        public int N { get; set; }

        public string Size { get; set; }
    }
}
