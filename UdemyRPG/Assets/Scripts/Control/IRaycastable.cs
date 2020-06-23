
using UnityEngine;

namespace RPG.Controller
{
    interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingController);
        CursorType GetCursorType(PlayerController callingController);
    }
}
