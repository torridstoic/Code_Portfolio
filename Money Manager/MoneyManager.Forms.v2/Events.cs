using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Forms.v2
{
	public enum EventType
	{
        None,
        Login,
        NewUser,
		Logout,
		Exit
	}

	public class MenuEvents : EventArgs
	{
		EventType type;

		public MenuEvents(EventType type)
		{
			this.type = type;
		}

		public EventType getEventType()
		{
			return type;
		}
	}
}
