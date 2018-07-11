
public abstract class Singleton<T> where T : new()
{
    private static T mInstance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (mInstance== null)
            {
                lock (_lock)
                {
                    if (mInstance == null)
                    {
                        mInstance = new T();
                    }
                }           
            }               
            return mInstance;
        }
    }
}
