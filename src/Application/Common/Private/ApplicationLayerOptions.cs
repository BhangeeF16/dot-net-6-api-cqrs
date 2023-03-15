namespace Application.Common.Private
{
    public class ApplicationLayerOptions
    {
        public string? UIResource { get; set; }
        public string? APIResource { get; set; }

        public ApplicationLayerOptions(string? uIResource, string? aPIResource)
        {
            UIResource = uIResource;
            APIResource = aPIResource;
        }
    }
}
