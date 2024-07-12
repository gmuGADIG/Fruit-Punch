using UnityEngine.InputSystem;

public static class PlayerInfo
{ 
    public static readonly Character[] characters = new[] {Character.None, Character.None};
    public static readonly InputDevice[] playerInputDevices = {null, null};
    public static readonly string[] playerControlSchemes = {null, null};
    
    /// <summary>
    /// Gets the amount of players
    /// </summary>
    /// <returns></returns>
    public static int PlayerCount()
    {
        if (characters[0] == Character.None) return 0;
        else if (characters[1] == Character.None) return 1;
        else return 2;
    }
}
