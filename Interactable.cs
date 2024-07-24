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
public class Interactable : MonoBehaviour
{

    public bool isPlayerInRange;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
	
} // end Interactable
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
