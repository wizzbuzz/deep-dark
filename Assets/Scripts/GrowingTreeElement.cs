using UnityEngine;

public class GrowingTreeElement : MonoBehaviour
{
    public GameObject posX, negX, posZ, negZ, posXPosZ, negXNegZ, negXPosZ, posXNegZ;

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
