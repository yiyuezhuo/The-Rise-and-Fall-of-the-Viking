using UnityEngine;
using GameCore;

public class UnityUserMessageService : IUserMessageService
{
    public void ShowMessage(string message, string title = "Message")
    {
        DialogRoot.Instance.PopupMessageDialog(message, title);
    }

    static UnityUserMessageService instance = new();
    public static UnityUserMessageService Instance => instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void RegisterToServiceLocator()
    {
        ServiceLocator.Register<IUserMessageService>(Instance);
    }
}