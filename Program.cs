namespace Moscovium_Lite
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            try
            {
                Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            }
            catch
            {
               
            }

            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Form1());
        }
    }
}