using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int blockCode;

    public void SetBlockCode(int _blockCode)
    {
        blockCode = _blockCode;
    }

    public int GetBlockCode()
    {
        return this.blockCode;
    }

}
