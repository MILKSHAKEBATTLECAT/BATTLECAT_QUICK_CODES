using UnityEngine;
using System.Collections.Generic;
//  _._    CODE BY   _._   //
//       /| ________________
// O|===|* >________________>
//       \|
//    _._     _,-'""`-._         _
//   (,-.`._,'(       |\`-/| ___/___
//       `-.-' \ )-`( , o o) |_____|
//             `-    \`_`"'-  | - |
//                            | - |
//  MILKSHAKE BATTLECAT 2025   ``` 
/// <summary>
/// Manages a stack of menus, allowing for opening and closing of menus in a stack.
/// Menus can be restacked if they are already open, based on the restackIfAlreadyOpen bool.
/// This script is designed to be attached to a GameObject in the scene and used for all Open/Close menu operations.
/// </summary>
namespace BATTLECAT
{
    public class MenuStackManager : MonoBehaviour
    {                
        private Stack<GameObject> menuStack = new Stack<GameObject>();
        [SerializeField] private bool restackIfAlreadyOpen = true; // Optional bool; if true, will restack the menus that are already open when trying to open them again

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTopMenu();
            }
        }  

        public void CloseTopMenu()
        {
            if (menuStack.Count > 0)
            {
                // Get and remove the top menu from the stack
                var top = menuStack.Pop();

                // Deactivate that menu
                top.SetActive(false);
                Debug.Log($"{top.name} is now closed.");
            }
            else
            {
                Debug.Log("No menus left to close.");
            }
        }         
        
        public void OpenMenu(GameObject menu)
        {
            if (menu == null) // Do nothing on a null menu
            {
                Debug.LogWarning("Null GameObject sent to OpenMenu.");
                return;
            }

            // If it's already active in the hierarchy, either ignore or restack it
            if (menu.activeSelf)
            {
                if (!menuStack.Contains(menu)) // Menu is active but not in the stack?
                {
                    // This script assumes menus start inactive, so this ideally should not occur, but if it does, we fix it
                    menuStack.Push(menu);
                    Debug.LogWarning($"{menu.name} was active but missing from stack. Pushing to top for consistency.");
                    return;
                }

                if (!restackIfAlreadyOpen) // Do nothing in this case
                {
                    Debug.Log($"{menu.name} is already open.");
                    return;
                }

                // Use shared removal logic
                bool removed = RemoveFromStack(menuStack, menu);
                if (!removed)
                {
                    Debug.LogWarning($"{menu.name} was expected in the stack but couldn't be removed. Stack may be corrupted.");
                }

                menuStack.Push(menu); // Push it to the top
                Debug.Log($"{menu.name} was already open, restacked to top.");
                return;
            }

            // If !menu.activeSelf, normal opening logic: activate and stack
            menu.SetActive(true);
            menuStack.Push(menu);
            Debug.Log($"{menu.name} is now open.");
        }

        public void CloseMenu(GameObject menu)
        {
            if (menu == null || !menuStack.Contains(menu))
                return;

            // Use shared removal logic
            bool removed = RemoveFromStack(menuStack, menu);

            if (removed)
            {
                menu.SetActive(false);
                Debug.Log($"{menu.name} is now closed.");
            }
            else
            {
                Debug.LogWarning($"{menu.name} could not be found during close operation; may have been already closed or never stacked.");
            }
        }

        public void ToggleMenu(GameObject menu)
        {
            if (menu == null)
            {
                Debug.LogWarning("Null GameObject sent to ToggleMenu.");
                return;
            }

            if (menu.activeSelf)
            {
                CloseMenu(menu);
            }
            else
            {
                OpenMenu(menu);
            }
        }

        private bool RemoveFromStack(Stack<GameObject> stack, GameObject target, int safetyLimit = 50)
        {
            // Temporary stack to hold items while we search for the target
            Stack<GameObject> tempStack = new Stack<GameObject>();

            // Counter to avoid infinite loop in case of unexpected behavior
            int count = 0;

            // Search through the stack from the top down
            while (stack.Count > 0 && count < safetyLimit)
            {
                var top = stack.Pop();
                if (top == target)
                {
                    // We've found the menu we want to remove; do NOT push it to tempStack (this removes it)
                    // Rebuild the original stack by restoring all saved menus in their original order
                    while (tempStack.Count > 0)
                        stack.Push(tempStack.Pop());

                    return true; // Indicate successful removal
                }

                // This wasn't the target menu. Store it in the temp stack for now
                tempStack.Push(top);
                count++;
            }

            // Whether we find the target or not, we still rebuild the stack
            while (tempStack.Count > 0)
                stack.Push(tempStack.Pop());

            return false; // Target was not found in the stack
        }

    } // end MyScript

} // end namespace BATTLECAT
//                    ^           ^
//                   /  \______ /  \
//                  /  ^          ^ \
//                 |                 |
//                |       "    "     |
//             -- |  _.--._   _.--._  | --
//       (@_  ===(= .      '        . =) ===
//  _     ) \_____\  '. :   <->  :'.  /_______________________
// (_)@8@8{}<__________|\____Y____/|___________________________>
//        )_/  milk  \       -     /  battle
//       (@   shake    `----------`    cat
