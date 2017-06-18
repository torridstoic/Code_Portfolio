using System.Runtime.InteropServices;

namespace MeatballTennis
{
    public class Server
    {
	#if MONO
        [DllImport("libServer.so")]
        public static extern int init(ushort Port);

        [DllImport("libServer.so")]
        public static extern int update();

        [DllImport("libServer.so")]
        public static extern void stop();

	#else
		[DllImport("Server.dll")]
        public static extern int init(ushort Port);

        [DllImport("Server.dll")]
        public static extern int update();

        [DllImport("Server.dll")]
        public static extern void stop();
	#endif
    }
}
