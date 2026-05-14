namespace ViaEurope.Business.Helpers
{
    public static class OrderCodeGenerator
    {
        public static string Generate()
            => $"VE-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}