using System;


public class ResourceHelper
{
    public const string UI_PATH = "UI/";
    public static string GetUIPath(string name) 
    {
        return UI_PATH + name;
    }
}
