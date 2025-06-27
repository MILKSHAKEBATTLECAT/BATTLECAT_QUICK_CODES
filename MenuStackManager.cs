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
                var top = menuStack.Peek(); // Don't pop here
                CloseMenu(top); // CloseMenu will pop it
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
                // Log the action for debugging purposes
                Debug.Log($"{menu.name} was already open, restacked to top.");
                // No need to activate it again since it's already active, so exit out
                return;
            }

            // If !menu.activeSelf, normal opening logic: activate and stack
            menu.SetActive(true); // Activate the menu
            menuStack.Push(menu); // Add it to the stack
            // Log the action for debugging purposes
            Debug.Log($"{menu.name} is now open.");
        }

        // Closes the target menu by deactivating it and removing it from the stack.
        // If the menu is not in the stack, does nothing.
        public void CloseMenu(GameObject menu)
        {
            // If the menu is null or not even in our stack, there's nothing to do
            if (menu == null || !menuStack.Contains(menu))
            {
                return;
            }

            // Attempt to remove the menu from the stack.
            // This pulls the stack apart and rebuilds it without the menu in it.
            bool removed = RemoveFromStack(menuStack, menu);

            if (removed)
            {
                // Since menu has been removed from stack, we can safely deactivate it
                menu.SetActive(false);
                Debug.Log($"{menu.name} is now closed.");
            }
            else
            {
                // Shouldn’t happen, but we log if the menu somehow couldn’t be found
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

        /// <summary>
        /// Removes a specific GameObject from the stack, regardless of its position.
        /// This is done by popping items one by one into a temporary stack until the target is found.
        /// The target is then discarded (NOT added to the temp stack), effectively removing it.
        /// Remaining items are pushed back in their original order.
        /// Returns true if the target was found and removed, false if not.
        /// </summary>
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
                    // We've found the target menu.
                    // DO NOT push it into the temp stack. We are removing it.
                    // The rest will go back into the original stack below.
                    while (tempStack.Count > 0)
                        stack.Push(tempStack.Pop());

                    return true; // Indicate successful removal
                }

                // top != target, this wasn't the target menu. Store it in the temp stack for now
                tempStack.Push(top);
                count++;
            }

            // Whether we find the target or not, we still rebuild the stack
            // Note: 'stack' is a reference to the original Stack<GameObject> passed in,
            // so this modifies the actual stack, not a copy.
            while (tempStack.Count > 0)
            {
                stack.Push(tempStack.Pop());
            }

            return false; // Target was not found in the stack
        }

    } // end MenuStackManager

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
