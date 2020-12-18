namespace Kola.Infrastructure.Design
{
    public static class ScreenInfo
    {
        public static int Height { get; set; }
        public static int Width { get; set; }
        public static float Density { get; set; }

        public const int MinimumDeviceSizeToShowHeader = 500;
        public const int MinimumDeviceSizeToShowFooter = 600;
    }
}
