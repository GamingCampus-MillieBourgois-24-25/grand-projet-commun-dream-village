using System;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class NotificationManager
{
    public static Dictionary<int, AndroidNotification> androidNotifications = new Dictionary<int, AndroidNotification>();
    private static int nextNotificationId = 1; // ID unique incrémental

    static AndroidNotificationChannel channel = new AndroidNotificationChannel()
    {
        Id = "channel_id",
        Name = "Default Channel",
        Importance = Importance.Default,
        Description = "Generic notifications",
        CanBypassDnd = true,
        CanShowBadge = true,
        EnableLights = true,
        EnableVibration = true,
    };

    static bool launched = false;




    public static void SetupNotifications()
    {
        AskNotificationPermission();
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        CancelAllNotifications();
    }


    private static void AskNotificationPermission()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }

        }
    }

    public static int CreateNotification(string _title, string _text, DateTime _dateTime)
    {
        AndroidNotification notification = new AndroidNotification();
        notification.Title = _title;
        notification.Text = _text;
        notification.FireTime = _dateTime;

        int id = nextNotificationId++;
        androidNotifications[id] = notification; // Stocker la notification avec un ID unique
        return id;
    }

    public static int CreateNotification(string _title, string _text, float _seconds)
    {
        return CreateNotification(_title, _text, DateTime.Now.AddSeconds(_seconds));
    }

    public static void CancelNotification(int id)
    {
        if (androidNotifications.ContainsKey(id))
        {
            androidNotifications.Remove(id); // Supprimer uniquement la notification avec l'ID donné
        }
    }




    public static void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        launched = false; // Réinitialiser le flag de lancement
    }

    public static void LaunchNotifications()
    {
        if (launched)
            return;

        launched = true; // Indiquer que les notifications ont été lancées
        foreach (var kvp in androidNotifications)
        {
            AndroidNotification notification = kvp.Value; // Récupérer la notification

            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

}
