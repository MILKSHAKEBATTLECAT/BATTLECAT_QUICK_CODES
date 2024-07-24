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
[RequireComponent(typeof(InteractableChild))]
public class Pickup : MonoBehaviour
{

    private Interactable myInteractable;

    private void Start()
    {
        // Start the coroutine to perform delayed actions
        StartCoroutine(LateStart());        
    }

    private IEnumerator LateStart()
    {
        // Wait for the end of the frame after Start is called
        yield return new WaitForEndOfFrame();

        // Place delayed initialization code here
        Debug.Log("LateStart executed after all Start methods.");

        InteractableChild interactableChild = GetComponent<InteractableChild>();
        myInteractable = interactableChild.GetInteractable();
        if(myInteractable == null)
        {
            Debug.LogWarning("Could not find interactable for " + gameObject.name);
        }
    }

    private void Update()
    {
        if (myInteractable != null && myInteractable.isPlayerInRange)
        {
            // Pick up the booty!
            Debug.Log(gameObject.name + " was picked up.");

            Destroy(gameObject);
        }
    }
	
} // end Pickup
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
