using UnityEngine;

public class GrowingTreeElement : MonoBehaviour
{
    // Neighbouring cell references for each cardinal and diagonal direction
    public GameObject posX, negX, posZ, negZ, posXPosZ, negXNegZ, negXPosZ, posXNegZ;

    // Destroys diagonal corner pieces that have no adjacent cardinal neighbours to connect to
    public void RemoveCorners()
    {
        if(posX == null && posZ == null)
        {   
            Destroy(posXPosZ);
        }
        if(posX == null && negZ == null)
        {
            Destroy(posXNegZ);
        }
        if(negX == null && negZ == null)
        {
            Destroy(negXNegZ);
        }
        if(negX == null && posZ == null)
        {
            Destroy(negXPosZ);
        }
        
    }
}
