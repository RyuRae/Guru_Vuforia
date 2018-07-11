using System.Collections;

public class ManagerEvent
{
    public static string MSG_DiviceInfo = "MSG_DiviceInfo";
    public static string MSG_ProgressBar = "MSG_ProgressBar";
    public static string MSG_ServerConnection = "MSG_ServerConnection";

    private static Hashtable listeners = new Hashtable();

    public delegate void Handler(params object[] args);

    public static void Register(string message, Handler action)
    {
        var actions = listeners[message] as Handler;
        if (actions != null)
        {
            listeners[message] = actions + action;
        }
        else
        {
            listeners[message] = action;
        }
    }

    public static void Unregister(string message, Handler action)
    {
        var actions = listeners[message] as Handler;
        if (actions != null)
        {
            listeners[message] = actions - action;
        }
    }

    public static void Send(string message, params object[] args)
    {
        var actions = listeners[message] as Handler;
        if (actions != null)
        {
            actions(args);
        }
    }
}