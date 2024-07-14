using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  _._    CODE BY   _._   //
//       /| ________________
// O|===|* >________________>
//       \|
//    _._     _,-'""`-._         _
//   (,-.`._,'(       |\`-/| ___/___
//       `-.-' \ )-`( , o o) |_____|
//             `-    \`_`"'-  | - |
//                            | - |
//  MILKSHAKE BATTLECAT 2024   ``` 
public class Hotbar : MonoBehaviour
{
    public ActionSlotUI[] actionSlots;
    public List<ItemSlotUI> playerInventory;
    
    private int currentStartIndex = 0;

    private void Start()
    {
        playerInventory = Inventory.Instance.itemSlotUIs;

        UpdateActionSlots();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                CycleActionSlotsBackward();
            }
            else
            {
                CycleActionSlots();
            }
        }
    }

    public void UpdateActionSlots()
    {
        for (int i = 0; i < actionSlots.Length; i++)
        {
            Debug.Log("Updating action slot...");
            int inventoryIndex = currentStartIndex + i;
            if (inventoryIndex < playerInventory.Count)
            {
                actionSlots[i].SetItem(playerInventory[inventoryIndex].MyItem());
            }
            else
            {
                actionSlots[i].SetItem(null);
            }
        }
    }

    public void CycleActionSlots()
    {
        currentStartIndex += actionSlots.Length;
        if (currentStartIndex >= playerInventory.Count)
        {
            currentStartIndex = 0;
        }
        UpdateActionSlots();
    }

    public void CycleActionSlotsBackward()
    {
        currentStartIndex -= actionSlots.Length;
        if (currentStartIndex < 0)
        {
            currentStartIndex = Mathf.Max(0, playerInventory.Count - actionSlots.Length);
        }
        UpdateActionSlots();
    }

    public void AssignKeyCodes()
    {
        for (int i = 0; i < actionSlots.Length; i++)
        {
            if (i < UtzSettings.keyCodes.Length)
            {
                actionSlots[i].SetKeyCode(UtzSettings.keyCodes[i]);
            }
            else
            {
                actionSlots[i].SetKeyCode(KeyCode.None);
            }
        }
    }

} // end Hotbar
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
