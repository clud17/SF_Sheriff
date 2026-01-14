using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour
{
    public Texture2D cursorIcon;
    void Start()
    {
        Debug.Log(cursorIcon == null ? "cursorIcon NULL" : "cursorIcon OK");
        Cursor.SetCursor(cursorIcon, 
                        new Vector2(cursorIcon.width/2, cursorIcon.height/2),
                        CursorMode.Auto);
    }
}
