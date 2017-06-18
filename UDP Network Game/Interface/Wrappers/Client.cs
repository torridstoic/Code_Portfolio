using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MeatballTennis
{
    class Client
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Player
        {
	        public short Y;
            public short Score;

	        // NOTE: These are boolean variables, but bool is platform dependent.
            [MarshalAs(UnmanagedType.I1)]
            public bool KeyUp;
            [MarshalAs(UnmanagedType.I1)]
            public bool KeyDown;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GameState 
        {
            public byte GamePhase;

            public short BallX;
            public short BallY;

            public Player Player0;
            public Player Player1;
		}

	#if MONO
		[DllImport("libClient.so")]
        public static extern int init(string address, ushort port, byte player);

        [DllImport("libClient.so")]
        public static extern int run();

        [DllImport("libClient.so")]
        public static extern void stop();

        [DllImport("libClient.so")]
        public static extern int sendInput(bool keyUp, bool keyDown, bool keyQuit);

        [DllImport("libClient.so")]
		public static extern void getState(ref GameState state);
	#else
		[DllImport("Client.dll")]
        public static extern int init(string address, ushort port, byte player);

        [DllImport("Client.dll")]
        public static extern int run();

        [DllImport("Client.dll")]
        public static extern void stop();

        [DllImport("Client.dll")]
        public static extern int sendInput(bool keyUp, bool keyDown, bool keyQuit);

        [DllImport("Client.dll")]
        public static extern void getState(ref GameState state);
	#endif
	}
}
