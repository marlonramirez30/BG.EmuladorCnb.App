namespace BG.EmuladorCnb.App
{
    internal static class Program
    {
        [MTAThread]
        private static void Main() => Application.Run((Form)new FrmEmulator("1"));
    }
}