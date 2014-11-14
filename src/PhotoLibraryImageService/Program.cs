using Microsoft.Owin.Hosting;

namespace PhotoLibraryImageService
{
    class Program
    {
        static void Main(string[] args)
        {
            const string baseAddress = "http://localhost:54321/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                while (true)
                { }
            }
        }
    }
}
