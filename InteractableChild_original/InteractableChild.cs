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
public class InteractableChild : MonoBehaviour
{

    public bool showGizmos = false;
    public float triggerRadius = 3f;
    public float yPosOffset = 0f;

    public Transform interactableParentOverride;
    

    private Interactable interactable;    

    void Start()
    {
        CreateInteractableChild();
    }

    public Interactable GetInteractable()
    {
        return interactable;
    }

    private void CreateInteractableChild()
    {
        // Create a new GameObject as a child of this object
        GameObject childObject = new GameObject("Interactable");
        if(interactableParentOverride == null)
        {
            childObject.transform.SetParent(transform);
        }
        else
        {
            childObject.transform.SetParent(interactableParentOverride);
        }
        
        
        // Add a spherecollider component
        SphereCollider sphereCollider = childObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = triggerRadius;

        // Position the SphereCollider relative to the object's position
        sphereCollider.transform.localPosition = new Vector3(0, yPosOffset, 0);

        // Set the layer of the child object to the Interactable layer.
        childObject.layer = LayerMask.NameToLayer("Interactable");

        // Add Interactable script.
        interactable = childObject.AddComponent<Interactable>();

    }

    // Draw gizmos in the scene view
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            // Set the color for the gizmo
            Gizmos.color = Color.red;  

            Vector3 pos = transform.position;

            if(interactableParentOverride != null)
            {
                pos = interactableParentOverride.position;
            }

            Vector3 corrected = new Vector3(pos.x, pos.y + yPosOffset, pos.z);

            // Draw a wireframe sphere at the position with the trigger radius
            Gizmos.DrawWireSphere(corrected, triggerRadius);  
        }
    }
	
} // end CreateInteractableChild
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
