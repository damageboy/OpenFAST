using System;

namespace OpenFAST.Session
{
	public class NullConnectionListener : ConnectionListener
	{
	}

    public abstract class ConnectionListener
	{
        static ConnectionListener()
		{
            NULL = new NullConnectionListener();
        }
        public readonly static ConnectionListener NULL;
        public virtual void OnConnect(Connection connection)
        {
        }
	}
}